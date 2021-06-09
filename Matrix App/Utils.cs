using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using static MatrixDesigner.Defaults;

namespace Matrix_App
{
    public sealed class Utils
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

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        /// <summary>
        /// Stores every 3-byte tuple from source into dest, by flipping the 0st and 1st byte of each tuple.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void FlipColorStoreRG_GR(byte[][] source, byte[][] dest)
        {
            for (int f = 0; f < dest.Length; f++)
            {
                for (int x = 0; x < dest[0].Length; x += 3)
                {
                    // flip r, g
                    dest[f][x + 0] = source[f][x + 1];
                    dest[f][x + 1] = source[f][x + 0];
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
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = (x + y * width) * BPP;

                    image.SetPixel(x, y, Color.FromArgb(
                            (byte) buffer[index + 0],
                            (byte) buffer[index + 1],
                            (byte) buffer[index + 2]
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

            for (int frame = 0; frame < frames; frame++)
            {
                bytes[frame] = new byte[width * height * BPP];
            }

            return bytes;
        }
    }
}
