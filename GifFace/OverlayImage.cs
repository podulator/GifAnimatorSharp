using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace GifFace {
    class OverlayImage {
        public OverlayImage(string image_ref, string path) {
            this.Ref = image_ref;
            this.Image = new MagickImage(path);
        }
        public string Ref { get; set; }
        public MagickImage Image { get; set; }

    }
}
