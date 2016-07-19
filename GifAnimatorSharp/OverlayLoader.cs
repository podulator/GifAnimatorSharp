using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json.Linq;

namespace GifFace {
    class OverlayLoader {
        private Logger _logger;

        public OverlayLoader(Logger logger) {
            _logger = logger;
        }

        public List<OverlayImage> LoadImages(JToken overlay_json) {

            _logger.log("Loading overlay images");
            List<OverlayImage> results = new List<OverlayImage>();
            foreach (var overlay in overlay_json) {
                string image_ref = overlay["ref"].ToString();
                string image_filename = overlay["filename"].ToString();
                int max_w = (overlay["max_w"] == null) ? 0 : (int)overlay["max_w"];
                int max_h = (overlay["max_h"] == null) ? 0 : (int)overlay["max_h"];
                _logger.log("Loading overlay image :: " + image_filename);
                OverlayImage overlay_image = new OverlayImage(image_ref, image_filename);
                overlay_image.Image.PreserveColorType();

                // check it's the within the max size bounds or resize it
                if ((max_w > 0 && max_h > 0) && (overlay_image.Image.Width > max_w || overlay_image.Image.Height > max_h)) {
                    _logger.log("Resizing overlay image to fit its bounds");
                    MagickGeometry size = new MagickGeometry(max_w, max_h);
                    size.IgnoreAspectRatio = false;
                    overlay_image.Image.Resize(size);
                    _logger.log("Overlay image resized");
                }
                results.Add(overlay_image);
            }
            _logger.log(string.Format("{0} overlay images loaded", results.Count));
            return results;
        }   // LoadImages

    }   // class
}
