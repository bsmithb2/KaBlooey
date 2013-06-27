using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BsDiff;

namespace KaBlooey
{
    /// <summary>
    /// KaBlooey - A patching engine for files and folders, using BSDIFF to create patches of directories.
    /// </summary>
    public class KaBlooeyEngine
    {
        public static void ApplyPatch(string patchFolderLocation, string newFolderLocation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (!Directory.Exists(patchFolderLocation))
            {
                throw new ArgumentException("directory doesn't exist", "patchFolderLocation");
            }
            if (!Directory.Exists(newFolderLocation))
            {
                Directory.CreateDirectory(newFolderLocation);
            }
            Parallel.ForEach<string>(Directory.GetFiles(newFolderLocation), delegate(string newFileLocation)
            {
                if(newFileLocation.EndsWith(".add") || newFileLocation.EndsWith(".delete") || newFileLocation.EndsWith(".temp")) return;
                var folderRelativePath = newFileLocation.Replace(newFolderLocation, string.Empty);
                if (Path.IsPathRooted(folderRelativePath))
                {
                    folderRelativePath = folderRelativePath.Remove(0, 1);
                }
                var patchFolderChildPath = Path.Combine(patchFolderLocation, folderRelativePath);

                using (FileStream input = new FileStream(newFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (FileStream output = new FileStream(newFileLocation + ".temp", FileMode.Create))
                    {
                        BinaryPatchUtility.Apply(input,
                                                 () =>
                                                 new FileStream(patchFolderChildPath + ".changed", FileMode.Open,
                                                                FileAccess.Read, FileShare.Read), output);
                    }
                }
                File.Copy(newFileLocation + ".temp", newFileLocation, true);
                File.Delete(newFileLocation + ".temp");
            });
            string[] files = Directory.GetFiles(patchFolderLocation, "*.add");
            for (int i = 0; i < files.Length; i++)
            {
                string text = files[i];
                if (File.Exists(text))
                {
                    Trace("Adding File:", text);
                    //var newFileLocation = Path.GetDirectoryName(newFolderLocation);
                    if (!Directory.Exists(newFolderLocation))
                    {
                        Directory.CreateDirectory(newFolderLocation);
                    }
                    File.Copy(text, Path.Combine(newFolderLocation, Path.GetFileName(text.Replace(".add", ""))));
                }
            }
            string[] deletedFiles = Directory.GetFiles(patchFolderLocation, "*.delete");
            for (int i = 0; i < deletedFiles.Length; i++)
            {
                string text = deletedFiles[i];
                var deletedFileLocation = Path.Combine(newFolderLocation, Path.GetFileName(text.Replace(".delete", "")));
                File.Delete(deletedFileLocation);
            }
            Parallel.ForEach<string>(Directory.GetDirectories(patchFolderLocation), delegate(string directory)
            {
                Trace("Recursing folder:", directory);
                string str = directory.Replace(patchFolderLocation, string.Empty);
                string newFolderLocation2 = newFolderLocation + str;
                string patchFolderLocation2 = patchFolderLocation + str;
                ApplyPatch(patchFolderLocation2, newFolderLocation2);
            });
            stopwatch.Stop();
            Console.WriteLine("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
        }

        public static void CreatePatch(string oldFolderLocation, string newFolderLocation, string patchLocation)
        {
            foreach(var newFolderChildPath in Directory.GetDirectories(newFolderLocation))
            {
                var folderRelativePath = newFolderChildPath.Replace(newFolderLocation, string.Empty);
                if(Path.IsPathRooted(folderRelativePath))
                {
                    folderRelativePath = folderRelativePath.Remove(0, 1);
                }
                var oldFolderChildPath = Path.Combine(oldFolderLocation, folderRelativePath);
                var patchFolderChildPath = Path.Combine(patchLocation, folderRelativePath);
                CreatePatch(oldFolderChildPath, newFolderChildPath, patchFolderChildPath);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            //TODO
            foreach (var file in Directory.GetFiles(newFolderLocation))
            {
                var relativePath = GetRelativePath(newFolderLocation, file);

                var oldFolderPath = Path.Combine(oldFolderLocation, relativePath);
                var patchFilePath = Path.Combine(patchLocation, relativePath);
                var patchFolderPath = Path.GetDirectoryName(patchFilePath);
                if (!Directory.Exists(patchFolderPath))
                {
                    Directory.CreateDirectory(patchFolderPath);
                }
                if (!File.Exists(oldFolderPath))
                {
                    // file missing, add it
                    File.Copy(file, patchFilePath + ".add");
                }
                else
                {
                    // file exists - take a diff
                    using (FileStream output = new FileStream(patchFilePath + ".changed", FileMode.Create))
						BinaryPatchUtility.Create(File.ReadAllBytes(oldFolderPath), File.ReadAllBytes(file), output);
                }
            }

            MarkFilesAsDeleted(oldFolderLocation, newFolderLocation, patchLocation);

            stopwatch.Stop();
            Console.WriteLine("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
        }

        private static void MarkFilesAsDeleted(string oldFolderLocation, string newFolderLocation, string patchLocation)
        {
            if (Directory.Exists(oldFolderLocation))
            {
                foreach (var oldFolderChildPath in Directory.GetDirectories(oldFolderLocation))
                {
                    var folderRelativePath = oldFolderChildPath.Replace(oldFolderLocation, string.Empty);
                    if (Path.IsPathRooted(folderRelativePath))
                    {
                        folderRelativePath = folderRelativePath.Remove(0, 1);
                    }
                    var newFolderChildPath = Path.Combine(newFolderLocation, folderRelativePath);
                    var patchFolderChildPath = Path.Combine(patchLocation, folderRelativePath);
                    MarkFilesAsDeleted(oldFolderChildPath, newFolderChildPath, patchFolderChildPath);
                }
                foreach (var file in Directory.GetFiles(oldFolderLocation))
                {
                    // find deletable files
                    var relativePath = GetRelativePath(oldFolderLocation, file);
                    var newFolderPath = Path.Combine(newFolderLocation, relativePath);
                    if (!File.Exists(newFolderPath))
                    {
                        // file has been deleted. Mark in patch folder as deleted file
                        var patchFilePath = Path.Combine(patchLocation, relativePath + ".delete");
                        var patchFolderPath = Path.GetDirectoryName(patchFilePath);
                        if (!Directory.Exists(patchFolderPath))
                        {
                            Directory.CreateDirectory(patchFolderPath);
                        }
                        File.Copy(file, patchFilePath);
                    }
                }
            }
        }

        private static string GetRelativePath(string newFolderLocation, string file)
        {
            var relativePath = file.Replace(newFolderLocation, "");
            if (Path.IsPathRooted(relativePath))
            {
                relativePath = relativePath.Remove(0, 1);
            }
            return relativePath;
        }

        public static void Trace(string format, string param)
        {
            Debug.WriteLine(string.Format(format + "{0}", param ));
        }
    }
}
