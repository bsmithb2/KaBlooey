using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
                //TODO
                //MagicalVersionChanger.ApplyPatchToFile (patchFolderLocation, newFileLocation);
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
                var relativePath = file.Replace(newFolderLocation, "");
                if (Path.IsPathRooted(relativePath))
                {
                    relativePath = relativePath.Remove(0, 1);
                }

                var oldFolderPath = Path.Combine(oldFolderLocation, relativePath);
                if (!File.Exists(oldFolderPath))
                {
                    var patchFilePath = Path.Combine(patchLocation, relativePath + ".add");
                    var patchFolderPath = Path.GetDirectoryName(patchFilePath);
                    if (!Directory.Exists(patchFolderPath))
                    {
                        Directory.CreateDirectory(patchFolderPath);
                    }
                    File.Copy(file, patchFilePath);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
        }

        public static void Trace(string format, string param)
        {
            Debug.WriteLine(string.Format(format + "{0}", param ));
        }
    }
}