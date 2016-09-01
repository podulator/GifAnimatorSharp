using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Newtonsoft.Json.Linq;
using log4net;

namespace GifFace
{

	class MainClass {

        private static ILog logger;
		public static string showHelp() {
			return "Run with 1 arg, the path to the recipe";
		}

		public static int Main (string[] args) {
			logger = LogManager.GetLogger(typeof(MainClass));
            logger.Info("Run started");

            if (args.Length != 1) {
				logger.Info (showHelp ());
				return 1;
			}

			string recipe_file = args [0];
			if (!File.Exists (recipe_file)) {
				logger.Info ("Couldn't find recipe file :: " + recipe_file);
				return 2;
			}

			// load the recipe
			JObject recipe = JObject.Parse (File.ReadAllText (recipe_file));
            // logger.Info(recipe.ToString ());
			string output_file = (string)recipe ["output"];

            OverlayLoader loader = new OverlayLoader(logger);
            List<OverlayImage> overlay_images = loader.LoadImages(recipe["overlay_images"]);
            

            GifMaker gifMaker = new GifMaker(logger);
            MagickImageCollection theGif =  gifMaker.MakeGif(recipe["frames"], overlay_images);

            if (null != theGif) {
                logger.Info(string.Format("Saving Gif to :: {0}", output_file));
                theGif.Write(output_file);
            }

            logger.Info ("Run completed");
            Console.ReadLine();
			return 0;
		}// Main

    }
}
