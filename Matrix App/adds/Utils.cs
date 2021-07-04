using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static Matrix_App.Defaults;

namespace Matrix_App
{
    public static class Utils
    {
        /// <summary>
        /// Resizes and image to the specified size in pixels.
        /// Sclaing is done via an Bicubic filter.
        /// Upscaling will result in blurry images.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }

        /// <summary>
        /// Stores every 3-byte tuple from source into dest, by flipping the 0st and 1st byte of each tuple.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void ColorStore(byte[][] source, byte[][] dest)
        {
            for (var f = 0; f < source.Length; f++)
            {
                for (var x = 0; x < source[0].Length; x += 3)
                {
                    dest[f][x + 0] = source[f][x + 0];
                    dest[f][x + 1] = source[f][x + 1];
                    dest[f][x + 2] = source[f][x + 2];
                }
            }
        }

        /// <summary>
        /// Wraps buffer into a bitmap with width and height
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ImageWrap(byte[] buffer, int width, int height)
        {
            var image = new Bitmap(width, height);
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var index = (x + y * width) * Bpp;

                    image.SetPixel(x, y, Color.FromArgb(
                             buffer[index + 0],
                             buffer[index + 1],
                            buffer[index + 2]
                        ));
                }
            }

            return image;
        }

        /// <summary>
        /// Creates an buffer of RGB-bytes and n-frames.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static byte[][] CreateImageRGB_NT(int width, int height, int frames)
        {
            byte[][] bytes = new byte[frames][];

            for (var frame = 0; frame < frames; frame++)
            {
                bytes[frame] = new byte[width * height * Bpp];
            }

            return bytes;
        }
    }
}
