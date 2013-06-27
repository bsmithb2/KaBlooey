using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BrightSword.CommandLineUtilities;
using BrightSword.SwissKnife;

namespace KaBlooey.Console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		    var apply = args.GetArg<bool>("apply", _ => Boolean.Parse(_));
		    var oldFolderLocation = args.GetArg("oldFolder", _ => _);
            var newFolderLocation = args.GetArg("newFolder", _ => _);
            var patchFolderLocation = args.GetArg("patchFolder", _ => _);
		    System.Console.ReadLine();
            if(apply)
            {
                KaBlooey.KaBlooeyEngine.ApplyPatch(patchFolderLocation, oldFolderLocation);
            }
            else
            {
                KaBlooey.KaBlooeyEngine.CreatePatch(oldFolderLocation, newFolderLocation, patchFolderLocation);
            }

		}

	}

}
