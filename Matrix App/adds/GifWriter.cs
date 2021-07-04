using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Matrix_App
{
    /// <summary>
    /// Creates a GIF using .Net GIF encoding and additional animation headers.
    /// </summary>
    public class GifWriter : IDisposable
    {
        #region Fields
        
        private const long SourceGlobalColorInfoPosition = 10, SourceImageBlockPosition = 789;

        private readonly object syncLock = new object();
        private readonly BinaryWriter writer;
        private bool firstFrame = true;
        
        #endregion

        /// <summary>
        /// Creates a new instance of GifWriter.
        /// </summary>
        /// <param name="outStream">The <see cref="Stream"/> to output the Gif to.</param>
        /// <param name="defaultFrameDelay">Default Delay between consecutive frames... FrameRate = 1000 / DefaultFrameDelay.</param>
        /// <param name="repeat">No of times the Gif should repeat... -1 not to repeat, 0 to repeat indefinitely.</param>
        public GifWriter(Stream outStream, int defaultFrameDelay = 500, int repeat = 0)
        {
            if (outStream == null)
                throw new ArgumentNullException(nameof(outStream));

            if (defaultFrameDelay <= 0)
                throw new ArgumentOutOfRangeException(nameof(defaultFrameDelay));

            if (repeat < -1)
                throw new ArgumentOutOfRangeException(nameof(repeat));

            writer = new BinaryWriter(outStream);
            this.DefaultFrameDelay = defaultFrameDelay;
            this.Repeat = repeat;
        }

        /// <summary>
        /// Creates a new instance of GifWriter.
        /// </summary>
        /// <param name="fileName">The path to the file to output the Gif to.</param>
        /// <param name="defaultFrameDelay">Default Delay between consecutive frames... FrameRate = 1000 / DefaultFrameDelay.</param>
        /// <param name="repeat">No of times the Gif should repeat... -1 not to repeat, 0 to repeat indefinitely.</param>
        public GifWriter(string fileName, int defaultFrameDelay = 500, int repeat = -1)
            : this(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read), defaultFrameDelay, repeat) { }

        #region Properties
        /// <summary>
        /// Gets or Sets the Default Width of a Frame. Used when unspecified.
        /// </summary>
        public int DefaultWidth { get; set; }

        /// <summary>
        /// Gets or Sets the Default Height of a Frame. Used when unspecified.
        /// </summary>
        public int DefaultHeight { get; set; }

        /// <summary>
        /// Gets or Sets the Default Delay in Milliseconds.
        /// </summary>
        public int DefaultFrameDelay { get; set; }

        /// <summary>
        /// The Number of Times the Animation must repeat.
        /// -1 indicates no repeat. 0 indicates repeat indefinitely
        /// </summary>
        public int Repeat { get; }
        #endregion

        /// <summary>
        /// Adds a frame to this animation.
        /// </summary>
        /// <param name="image">The image to add</param>
        /// <param name="delay">Delay in Milliseconds between this and last frame... 0 = <see cref="DefaultFrameDelay"/></param>
        public void WriteFrame(Image image, int delay = 0)
        {
            lock (syncLock)
                using (var gifStream = new MemoryStream())
                {
                    image.Save(gifStream, ImageFormat.Gif);

                    // Steal the global color table info
                    if (firstFrame)
                        InitHeader(gifStream, writer, image.Width, image.Height);

                    WriteGraphicControlBlock(gifStream, writer, delay == 0 ? DefaultFrameDelay : delay);
                    WriteImageBlock(gifStream, writer, !firstFrame, 0, 0, image.Width, image.Height);
                }

            if (firstFrame)
                firstFrame = false;
        }

        #region Write
        private void InitHeader(Stream sourceGif, BinaryWriter writer, int width, int height)
        {
            // File Header
            writer.Write("GIF".ToCharArray()); // File type
            writer.Write("89a".ToCharArray()); // File Version

            writer.Write((short)(DefaultWidth == 0 ? width : DefaultWidth)); // Initial Logical Width
            writer.Write((short)(DefaultHeight == 0 ? height : DefaultHeight)); // Initial Logical Height

            sourceGif.Position = SourceGlobalColorInfoPosition;
            writer.Write((byte)sourceGif.ReadByte()); // Global Color Table Info
            writer.Write((byte)0); // Background Color Index
            writer.Write((byte)0); // Pixel aspect ratio
            WriteColorTable(sourceGif, writer);

            // App Extension Header for Repeating
            if (Repeat == -1)
                return;

            writer.Write(unchecked((short)0xff21)); // Application Extension Block Identifier
            writer.Write((byte)0x0b); // Application Block Size
            writer.Write("NETSCAPE2.0".ToCharArray()); // Application Identifier
            writer.Write((byte)3); // Application block length
            writer.Write((byte)1);
            writer.Write((short)Repeat); // Repeat count for images.
            writer.Write((byte)0); // terminator
        }

        static void WriteColorTable(Stream sourceGif, BinaryWriter writer)
        {
            sourceGif.Position = 13; // Locating the image color table
            var colorTable = new byte[768];
            sourceGif.Read(colorTable, 0, colorTable.Length);
            writer.Write(colorTable, 0, colorTable.Length);
        }

        static void WriteGraphicControlBlock(Stream sourceGif, BinaryWriter writer, int frameDelay)
        {
            sourceGif.Position = 781; // Locating the source GCE
            var blockhead = new byte[8];
            sourceGif.Read(blockhead, 0, blockhead.Length); // Reading source GCE

            writer.Write(unchecked((short)0xf921)); // Identifier
            writer.Write((byte)0x04); // Block Size
            writer.Write((byte)(blockhead[3] & 0xf7 | 0x08)); // Setting disposal flag
            writer.Write((short)(frameDelay / 10)); // Setting frame delay
            writer.Write(blockhead[6]); // Transparent color index
            writer.Write((byte)0); // Terminator
        }

        static void WriteImageBlock(Stream sourceGif, BinaryWriter writer, bool includeColorTable, int x, int y, int width, int height)
        {
            sourceGif.Position = SourceImageBlockPosition; // Locating the image block
            var header = new byte[11];
            sourceGif.Read(header, 0, header.Length);
            writer.Write(header[0]); // Separator
            writer.Write((short)x); // Position X
            writer.Write((short)y); // Position Y
            writer.Write((short)width); // Width
            writer.Write((short)height); // Height

            if (includeColorTable) // If first frame, use global color table - else use local
            {
                sourceGif.Position = SourceGlobalColorInfoPosition;
                writer.Write((byte)(sourceGif.ReadByte() & 0x3f | 0x80)); // Enabling local color table
                WriteColorTable(sourceGif, writer);
            }
            else writer.Write((byte)(header[9] & 0x07 | 0x07)); // Disabling local color table

            writer.Write(header[10]); // LZW Min Code Size

            // Read/Write image data
            sourceGif.Position = SourceImageBlockPosition + header.Length;

            var dataLength = sourceGif.ReadByte();
            while (dataLength > 0)
            {
                var imgData = new byte[dataLength];
                sourceGif.Read(imgData, 0, dataLength);

                writer.Write((byte)dataLength);
                writer.Write(imgData, 0, dataLength);
                dataLength = sourceGif.ReadByte();
            }

            writer.Write((byte)0); // Terminator
        }
        #endregion

        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            // Complete File
            writer.Write((byte)0x3b); // File Trailer

            writer.BaseStream.Dispose();
            writer.Dispose();
        }
    }
}
