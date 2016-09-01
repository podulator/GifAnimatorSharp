using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json.Linq;
using log4net;

namespace GifFace {
    public class GifMaker {
        private ILog _logger;

        public GifMaker(ILog logger) {
            _logger = logger;
        }

        public MagickImageCollection MakeGif(JToken frames_json, List<OverlayImage> overlay_images) {

            MagickImageCollection output_images = new MagickImageCollection();
            FrameBuilder frameBuilder = new FrameBuilder(_logger, overlay_images);

            foreach (var recipe_frame in frames_json) {

                MagickImage compositedFrame = frameBuilder.Compose(recipe_frame);
                // add the composited background to the collection
                output_images.Add(compositedFrame);

            }// each frame

            if (output_images.Count > 0) {
                _logger.Info(string.Format("Optimising collection of {0} images", output_images.Count));

                output_images.Coalesce();
                output_images.OptimizeTransparency();
                
                _logger.Info("Collection optimised");
                return output_images;
            }
            else {
                _logger.Info("No images in collection");
                return null;
            }

        }   // makegif

    }   // class
}
