using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BrightSword.CommandLineUtilities;
using BrightSword.SwissKnife;
using KaBlooey.Engine;

namespace KaBlooey.Console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		    var apply = args.GetArgumentValue<bool>("apply", _ => Boolean.Parse(_));
            var useZip = args.GetArgumentValue<bool>("useZip", _ => Boolean.Parse(_));
            var oldFolderLocation = args.GetArgumentValue("oldFolder", _ => _);
            var newFolderLocation = args.GetArgumentValue("newFolder", _ => _);
            var patchFolderLocation = args.GetArgumentValue("patchFolder", _ => _);
		    var patchZipLocation = args.GetArgumentValue("patchZipFile", _ => _);

		    if (!args.Any())
		    {
		        System.Console.WriteLine("Please use some arguments (apply, useZip, oldFolder, newFolder, patchFolder, patchZipFile");
		        return;
		    }

		    if (useZip)
		    {
                if (apply)
                {
                    KaBlooeyEngine.ApplyPatchFromZip(oldFolderLocation, patchZipLocation);
                }
                else
                {
                    KaBlooeyEngine.CreatePatchIntoZip(oldFolderLocation, newFolderLocation, patchZipLocation);
                }
		    }
		    else
		    {
		        if (apply)
		        {
		            KaBlooeyEngine.ApplyPatch(patchFolderLocation, oldFolderLocation);
		        }
		        else
		        {
		            KaBlooeyEngine.CreatePatch(oldFolderLocation, newFolderLocation, patchFolderLocation);
		        }
		    }

		}

	}

}
