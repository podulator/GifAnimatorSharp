using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Newtonsoft.Json.Linq;

namespace GifFace
{

	class MainClass {

        private static Logger logger;
		public static string showHelp() {
			return "Run with 1 arg, the path to the recipe";
		}

		public static int Main (string[] args) {
			logger = new Logger();
            logger.log("Run started");

            if (args.Length != 1) {
				logger.log (showHelp ());
				return 1;
			}

			string recipe_file = args [0];
			if (!File.Exists (recipe_file)) {
				logger.log ("Couldn't find recipe file :: " + recipe_file);
				return 2;
			}

			// load the recipe
			JObject recipe = JObject.Parse (File.ReadAllText (recipe_file));
            // logger.log(recipe.ToString ());
			string output_file = (string)recipe ["output"];

            OverlayLoader loader = new OverlayLoader(logger);
            List<OverlayImage> overlay_images = loader.LoadImages(recipe["overlay_images"]);
            

            GifMaker gifMaker = new GifMaker(logger);
            MagickImageCollection theGif =  gifMaker.MakeGif(recipe["frames"], overlay_images);

            if (null != theGif) {
                logger.log(string.Format("Saving Gif to :: {0}", output_file));
                theGif.Write(output_file);
            }

            logger.log ("Run completed");
            Console.ReadLine();
			return 0;
		}// Main

    }
}
