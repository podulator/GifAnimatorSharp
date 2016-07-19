using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json.Linq;

namespace GifFace {
    class FrameBuilder {
        private Logger _logger;
        private List<OverlayImage> _overlay_images;
        public FrameBuilder(Logger logger, List<OverlayImage> overlay_images) {
            _logger = logger;
            _overlay_images = overlay_images;
        }

        public MagickImage Compose(JToken frame_json) {

            _logger.log("Processing new frame");
            string background_filename = (string)frame_json["background"];
            int delay = (int)frame_json["delay"];

            _logger.log("Loading background image" + background_filename);
            MagickImage background = new MagickImage(background_filename);
            background.AnimationDelay = delay;

            LayerBuilder layerBuilder = new LayerBuilder(_logger, _overlay_images);
            foreach (var layer_json in frame_json["layers"]) {
                layerBuilder.Compose(layer_json, background);
            }

            return background;

        }   //  Compose
    }   // class
}
