using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BsDiff;
using Ionic.Zip;

namespace KaBlooey.Engine
{
    /// <summary>
    /// KaBlooey - A patching engine for files and folders, using BSDIFF to create patches of directories.
    /// </summary>
    public class KaBlooeyEngine
    {
        public static void ApplyPatch(string patchFolderLocation, string applyLocation, bool ignoreMD5 = false)
        {
            _hash.Clear();
            if (!ignoreMD5)
            {
                _hash =
                    ChangeFileHashDetails.DeserializeListFromFile(patchFolderLocation + "\\_changedFileList.hashstore");
                var files = Directory.GetFiles(patchFolderLocation, "*.*", SearchOption.AllDirectories);
                if (_hash.Count != files.Length - 1)
                {
                    throw new InvalidOperationException("Incorrect number of patch files to hashes");
                }
                var foundFilesInHash = 0;
                foreach (var file in files)
                {
                    var relativePath = file.Replace(patchFolderLocation, "");
                    var validHashForFile = _hash.FirstOrDefault(s => relativePath == s._relativePatchFileLocation);
                    if (validHashForFile == null)
                    {
                        throw new InvalidOperationException("Couldn't Find Valid Hash");
                    }
                    foundFilesInHash++;
                }
                if (foundFilesInHash != _hash.Count)
                {
                    throw new InvalidOperationException("Didn't find every file in the hash file");
                }
            }
            ApplyPatchImpl(patchFolderLocation, applyLocation);
        }

        private static void ApplyPatchImpl(string patchFolderLocation, string applyLocation)
        {
            Trace(string.Format("Patch Location = {0} ; apply location = {1}", patchFolderLocation, applyLocation), "");
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (!Directory.Exists(patchFolderLocation))
            {
                throw new ArgumentException("directory doesn't exist", "patchFolderLocation");
            }
            if (!Directory.Exists(applyLocation))
            {
                Directory.CreateDirectory(applyLocation);
            }
            Parallel.ForEach(Directory.GetFiles(applyLocation), delegate(string applyFileLocation)
            {
                if (applyFileLocation.EndsWith(".add") || applyFileLocation.EndsWith(".delete") ||
                    applyFileLocation.EndsWith(".temp")) return;
                var folderRelativePath = applyFileLocation.Replace(applyLocation, string.Empty);
                if (Path.IsPathRooted(folderRelativePath))
                {
                    folderRelativePath = folderRelativePath.Remove(0, 1);
                }
                var patchFolderChildPath = Path.Combine(patchFolderLocation, folderRelativePath);
                var patchFile = patchFolderChildPath + ".changed";
                if (File.Exists(patchFile))
                {
                    Trace("Applying : {0}", patchFile);
                    using (
                        FileStream input = new FileStream(applyFileLocation, FileMode.Open, FileAccess.Read,
                                                          FileShare.Read))
                    {
                        using (FileStream output = new FileStream(applyFileLocation + ".temp", FileMode.Create))
                        {
                            BinaryPatchUtility.Apply(input,
                                                     () =>
                                                     new FileStream(patchFolderChildPath + ".changed", FileMode.Open,
                                                                    FileAccess.Read, FileShare.Read), output);
                        }
                        input.Close();
                    }
                    new FileInfo(applyFileLocation).Attributes = FileAttributes.Normal;
                    File.Delete(applyFileLocation);
                    File.Copy(applyFileLocation + ".temp", applyFileLocation, true);
                    File.Delete(applyFileLocation + ".temp");
                }
            });
            string[] files = Directory.GetFiles(patchFolderLocation, "*.add");
            for (int i = 0; i < files.Length; i++)
            {
                string text = files[i];
                if (File.Exists(text))
                {
                    Trace("Adding File:", text);

                    if (!Directory.Exists(applyLocation))
                    {
                        Directory.CreateDirectory(applyLocation);
                    }
                    File.Copy(text, Path.Combine(applyLocation, Path.GetFileName(text.Replace(".add", ""))));
                }
            }
            string[] deletedFiles = Directory.GetFiles(patchFolderLocation, "*.delete");
            for (int i = 0; i < deletedFiles.Length; i++)
            {
                string text = deletedFiles[i];
                Trace("Deleting : {0}", text);
                var deletedFileLocation = Path.Combine(applyLocation, Path.GetFileName(text.Replace(".delete", "")));
                File.Delete(deletedFileLocation);
            }
            Parallel.ForEach(Directory.GetDirectories(patchFolderLocation), delegate(string directory)
            {
                Trace("Recursing folder:", directory);
                string str = directory.Replace(patchFolderLocation, string.Empty);
                string newFolderLocation2 = applyLocation + str;
                string patchFolderLocation2 = patchFolderLocation + str;
                ApplyPatchImpl(patchFolderLocation2, newFolderLocation2);
            });
            stopwatch.Stop();
            Console.WriteLine("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
        }

        private static List<ChangeFileHashDetails> _hash = new List<ChangeFileHashDetails>();
        private static string _rootOldFolderlocation;
        private static string _rootNewFolderLocation;
        private static string _rootPatchFolderLocation;

        public static void CreatePatch(string oldFolderLocation, string newFolderLocation, string patchLocation)
        {
            _hash.Clear();
            _rootOldFolderlocation = oldFolderLocation;
            _rootNewFolderLocation = newFolderLocation;
            _rootPatchFolderLocation = patchLocation;
            CreatePatchImpl(oldFolderLocation, newFolderLocation, patchLocation);
            ChangeFileHashDetails.SerializeListToFile(_hash, patchLocation + "\\_changedFileList.hashstore");
        }

        private static void CreatePatchImpl(string oldFolderLocation, string newFolderLocation, string patchLocation)
        {
            Parallel.ForEach(Directory.GetDirectories(newFolderLocation), (newFolderChildPath) =>
                {
                    var folderRelativePath = newFolderChildPath.Replace(newFolderLocation, string.Empty);
                    if (Path.IsPathRooted(folderRelativePath))
                    {
                        folderRelativePath = folderRelativePath.Remove(0, 1);
                    }
                    var oldFolderChildPath = Path.Combine(oldFolderLocation, folderRelativePath);
                    var patchFolderChildPath = Path.Combine(patchLocation, folderRelativePath);
                    CreatePatchImpl(oldFolderChildPath, newFolderChildPath, patchFolderChildPath);
                });
            Stopwatch stopwatch = Stopwatch.StartNew();

            Parallel.ForEach(Directory.GetFiles(newFolderLocation), file =>
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
                        _hash.Add(new ChangeFileHashDetails(patchFilePath + ".changed", oldFolderPath, file, _rootNewFolderLocation, _rootOldFolderlocation, _rootPatchFolderLocation));
                    }
                });

            MarkFilesAsDeleted(oldFolderLocation, newFolderLocation, patchLocation);

            stopwatch.Stop();
            Console.WriteLine("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
        }

        private static void MarkFilesAsDeleted(string oldFolderLocation, string newFolderLocation, string patchLocation)
        {
            if (Directory.Exists(oldFolderLocation))
            {
                Parallel.ForEach(Directory.GetDirectories(oldFolderLocation), (oldFolderChildPath) =>
                    {
                        var folderRelativePath = oldFolderChildPath.Replace(oldFolderLocation, string.Empty);
                        if (Path.IsPathRooted(folderRelativePath))
                        {
                            folderRelativePath = folderRelativePath.Remove(0, 1);
                        }
                        var newFolderChildPath = Path.Combine(newFolderLocation, folderRelativePath);
                        if (Directory.Exists(newFolderChildPath))
                        {
                            //done already, no need to try again. 
                            return;
                        }
                        var patchFolderChildPath = Path.Combine(patchLocation, folderRelativePath);
                        MarkFilesAsDeleted(oldFolderChildPath, newFolderChildPath, patchFolderChildPath);
                    });
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

        public static void CreatePatchIntoZip(string oldFolderLocation, string newFolderLocation, string patchFileLocation)
        {
            var patchFolderLocation = GetTempPatchFolderLocation();
            CreatePatch(oldFolderLocation, newFolderLocation, patchFolderLocation);
            CreateZipFileFromPatchFolderLocation(patchFolderLocation, patchFileLocation);
        }

        private static void CreateZipFileFromPatchFolderLocation(string patchFolderLocation, string patchFileLocation)
        {
            if (File.Exists(patchFileLocation))
            {
                throw new ApplicationException(string.Format("File {0} exists already - delete it first", patchFileLocation));
            }
            var patchPath = Path.GetDirectoryName(patchFileLocation);
            if (!Directory.Exists(patchPath))
            {
                Directory.CreateDirectory(patchPath);
            }
            using (var file = new ZipFile(patchFileLocation))
            {
                file.AddDirectory(patchFolderLocation);
                file.Save();
            }
        }

        private static string GetTempPatchFolderLocation()
        {
            var tempPath = Path.GetTempPath();
            var patchFolderLocation = Path.Combine(tempPath, "KaBlooeyPatches");
            if (Directory.Exists(patchFolderLocation))
            {
                Directory.Delete(patchFolderLocation, true);
            }
            return patchFolderLocation;
        }

        public static void ApplyPatchFromZip(string applyLocation, string patchFileLocation)
        {
            if (!File.Exists(patchFileLocation))
            {
                throw new ApplicationException(string.Format("File {0} doesnt exist - find me a patch file!", patchFileLocation));
            }
            var tempPatchPath = GetTempPatchFolderLocation();
            if (!Directory.Exists(tempPatchPath))
            {
                Directory.CreateDirectory(tempPatchPath);
            }
            using (var file = new ZipFile(patchFileLocation))
            {
                file.ExtractAll(tempPatchPath);
            }

            ApplyPatch(tempPatchPath, applyLocation);
        }

        public static List<ChangeFileHashDetails> GetHashSummary()
        {
            return _hash;
        }
    }
}
