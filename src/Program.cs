using System;
using System.Diagnostics;
using System.IO;
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
            var oldFolderLocation = args.GetArgumentValue("oldFolder", _ => _);
            var newFolderLocation = args.GetArgumentValue("newFolder", _ => _);
            var patchFolderLocation = args.GetArgumentValue("patchFolder", _ => _);
		    System.Console.ReadLine();
            if(apply)
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
