using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json.Linq;
using log4net;

namespace GifFace {
    class LayerBuilder {
        private ILog _logger;
        private List<OverlayImage> _overlay_images;
        public LayerBuilder(ILog logger, List<OverlayImage> overlay_images) {
            _logger = logger;
            _overlay_images = overlay_images;
        }

        public void Compose(JToken layer_json, MagickImage background) {

            string layer_type = (string)layer_json["type"];
            switch (layer_type) {
                case "image":
                    _logger.Info("Adding an image");
                    handleImage(background, layer_json, _overlay_images);
                    break;
                case "text":
                    _logger.Info("Adding some text");
                    handleText(background, layer_json);
                    break;
                default:
                    _logger.Info("Unhandled layer type :: " + layer_type);
                    break;
            };
        }
        private void handleText(MagickImage background_image, JToken text_json) {

            string content = (string)text_json["content"];
            string font = (string)text_json["font"] ?? "Arial";
            string font_x = (string)text_json["x"] ?? "center";
            string font_y = (string)text_json["y"] ?? "0";
            int font_size = (null != text_json["size"]) ? Int32.Parse((string)text_json["size"]) : 120;
            string colour = (null != text_json["colour"]) ? (string)text_json["colour"] : "Black";
            int outline_size = (null != text_json["outline_size"]) ? Int32.Parse((string)text_json["outline_size"]) : 0;
            string outline_colour = (string)text_json["outline_colour"] ?? colour;
            Percentage opacity_percentage = (null != text_json["opacity_percent"]) ? new Percentage(Int32.Parse((string)text_json["opacity_percent"])) : new Percentage(100);

            DrawableFont the_font = new DrawableFont(font);
            DrawableFontPointSize pointSize = new DrawableFontPointSize(font_size);
            DrawableFillColor fillColor = new DrawableFillColor(System.Drawing.Color.FromName(colour));
            DrawableFillOpacity fillOpacity = new DrawableFillOpacity(opacity_percentage);

            // Outline
            DrawableStrokeColor strokeColor = new DrawableStrokeColor(System.Drawing.Color.FromName(outline_colour));
            DrawableStrokeWidth strokeWidth = new DrawableStrokeWidth(outline_size);

            // Location
            DrawableText text = new DrawableText(0, Int32.Parse(font_y), content);
            DrawableGravity gravity = new DrawableGravity(Gravity.North);

            // Antialiasing
            DrawableStrokeAntialias strokeAntialias = new DrawableStrokeAntialias(true);
            DrawableTextAntialias textAntialias = new DrawableTextAntialias(true);

            DrawableStrokeOpacity strokeOpacity = new DrawableStrokeOpacity(opacity_percentage);

            // Settings
            IDrawable[] parameters;
            if (outline_size > 0) {
                parameters = new IDrawable[] { the_font, pointSize, fillColor, fillOpacity, strokeColor, strokeWidth, strokeOpacity, strokeAntialias, textAntialias, text, gravity };
            }
            else {
                parameters = new IDrawable[] { the_font, pointSize, fillColor, fillOpacity, strokeAntialias, textAntialias, text, gravity };
            }

            _logger.Info("Adding text to image :: " + content);
            background_image.Draw(parameters);

        }// handleText
        private void handleImage(MagickImage background_image, JToken image_json, List<OverlayImage> overlay_images) {

            int overlay_x = (int)image_json["overlay_x"];
            int overlay_y = (int)image_json["overlay_y"];
            int rotation = (int)image_json["rotation"];
            string image_ref = (string)image_json["overlay_ref"];

            // optimise by seeing if we're on the canvas or not
            if (overlay_x >= 0 && overlay_x >= 0) {
                // retrieve the real overlay image from its ref
                _logger.Info("Cloning overlay image :: " + image_ref);
                MagickImage frame_overlay_image = overlay_images.Where(x => image_ref == x.Ref).First().Image.Clone();

                frame_overlay_image.BackgroundColor = MagickColors.Transparent;
                frame_overlay_image.GifDisposeMethod = GifDisposeMethod.None;

                // rotate?
                if (rotation != 0) {
                    _logger.Info(string.Format("Rotating overlay image by {0} degrees", (rotation)));
                    frame_overlay_image.Rotate(rotation);
                }
                // create a final geometry
                MagickGeometry geometry = new MagickGeometry(overlay_x, overlay_y, frame_overlay_image.Width, frame_overlay_image.Height);

                // overlay to result
                background_image.Composite(frame_overlay_image, geometry, CompositeOperator.Over, "15");
            }
            else _logger.Info(string.Format("Overlay {0} not on screen", image_ref));

        }// handleImage

    }// class
}
