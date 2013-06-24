using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
namespace KaBlooey
{

	public class MagicalVersionChanger
		{
			//
			// Static Methods
			//
			public static void ApplyPatch (string patchFolderLocation, string newFolderLocation)
			{
				Stopwatch stopwatch = Stopwatch.StartNew ();
				Parallel.ForEach<string> (Directory.GetFiles (newFolderLocation), delegate (string newFileLocation)
				{
					//TODO
					//MagicalVersionChanger.ApplyPatchToFile (patchFolderLocation, newFileLocation);
				});
				string[] files = Directory.GetFiles (patchFolderLocation, "*.add");
				for (int i = 0; i < files.Length; i++)
				{
					string text = files [i];
					if (File.Exists (text))
					{
						//TODO
						//MagicalVersionChanger.Trace ("Adding File:", text);
						File.Copy (text, Path.Combine (newFolderLocation, Path.GetFileName (text.Replace (".add", ""))));
					}
				}
				Parallel.ForEach<string> (Directory.GetDirectories (patchFolderLocation), delegate (string directory)
				{
					//TODO
					//MagicalVersionChanger.Trace ("Recursing folder:", directory);
					string str = directory.Replace (patchFolderLocation, string.Empty);
					string newFolderLocation2 = newFolderLocation + str;
					string patchFolderLocation2 = patchFolderLocation + str;
					MagicalVersionChanger.ApplyPatch (patchFolderLocation2, newFolderLocation2);
				});
				stopwatch.Stop ();
				Console.WriteLine ("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
			}

			public static void CreatePatch (string oldFolderLocation, string newFolderLocation, string patchLocation)
			{
				Stopwatch stopwatch = Stopwatch.StartNew ();
				//TODO
				//Magi
				stopwatch.Stop ();
				Console.WriteLine ("Time Taken = {0} sec", stopwatch.Elapsed.TotalSeconds);
			}
		}
}
