#define DEBUG_ENABLED
#define DEBUG_ENABLED

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Timers;
using System.Windows.Forms;
using Matrix_App.adds;
using Matrix_App.forms;
using Matrix_App.Properties;
using Matrix_App.Themes;
using static Matrix_App.Defaults;
using static Matrix_App.ArduinoInstruction;
using static Matrix_App.Utils;
using Timer = System.Timers.Timer;

namespace Matrix_App
{
    public partial class MatrixDesignerMain : Form
    {
        private void PortsOnClick(object? sender, EventArgs e)
        {
            new Settings(ref commandQueue, ref port);
        }

        #region Private-Members

        /// <summary>
        ///     Port update Timer
        ///     Reloads available port names at consecutive rates
        /// </summary>
        private Timer? delay;

        private bool runningGif;

        private static SerialPort port = new();

        private PortCommandQueue commandQueue = new(ref port);

        /// <summary>
        ///     Gif like frame video buffer
        /// </summary>
        public static byte[][] gifBuffer = CreateImageRGB_NT(MatrixStartWidth, MatrixStartHeight, MatrixStartFrames);

        public static readonly ThreadQueue IMAGE_DRAWER = new("Matrix Image Drawer", 4);

        #endregion

        #region Setup

        public MatrixDesignerMain()
        {
            // generate UI
            InitializeComponent();
            // Set Parent of our matrix
            matrixView.Instance(this);
            // Generate filter access buttons
            MatrixGifGenerator.GenerateBaseUi(pregeneratedModsBase, matrixView, this);

            Init();
            // apply light-mode by default
            new LightMode().ApplyTheme(this);

            Show();
        }

        private void Init()
        {
            // create gif playback timer
            delay = new Timer((int) Delay.Value);
            delay.Elapsed += Timelineupdate;
            delay.AutoReset = true;

            // Set color wheel event handler
            ZeichnenFarbRad.handler = ColorWheel_Handler!;

            // setup port settings
            port.BaudRate = BaudRate;
            port.ReadTimeout = ReadTimeoutMs;
            port.WriteTimeout = WriteTimeoutMs;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;

            // setup matrix
            AdjustMatrixTable();

            // search for initial ports
            //GatherPortNames();

            HideEasterEgg();
        }

        private void HideEasterEgg()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Wednesday)
                return;

            if (new Random().Next(0, 9) >= 1)
                return;

            using (Bitmap wednesdayFrog = new(Resources.Frosch))
            {
                matrixWidth.Value = wednesdayFrog.Width;
                matrixHeight.Value = wednesdayFrog.Height;
                ResizeGif();

                for (var x = 0; x < wednesdayFrog.Width; x++)
                for (var y = 0; y < wednesdayFrog.Height; y++)
                {
                    var pixel = wednesdayFrog.GetPixel(x, y);

                    matrixView.SetPixelNoRefresh(x, y, pixel);
                }
            }

            matrixView.Refresh();
        }

        #endregion

        #region UI-Methods

        #region Port-ComboBox

        #endregion

        #region Scale

        /// <summary>
        ///     Applies a new size to the gif and matrix
        /// </summary>
        private void AdjustMatrixTable()
        {
            var width = (int) matrixWidth.Value;
            var height = (int) matrixHeight.Value;

            matrixView.resize(width, height);
            ResizeGif();
            // Delay.Minimum = Math.Min(Width.Value * Height.Value * 5, 500);
        }

        private void Width_ValueChanged(object sender, EventArgs e)
        {
            AdjustMatrixTable();
            commandQueue.EnqueueArduinoCommand(
                OpcodeScale, // opcode
                (byte) matrixWidth.Value,
                (byte) matrixHeight.Value
            );
        }

        private void Height_ValueChanged(object sender, EventArgs e)
        {
            AdjustMatrixTable();
            commandQueue.EnqueueArduinoCommand(
                OpcodeScale, // opcode
                (byte) matrixWidth.Value,
                (byte) matrixHeight.Value
            );
        }

        #endregion

        #region Edit/Draw

        #region TextBoxen

        private void DrawTextBoxRed_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(ZeichnenTextBoxRed.Text, out var value) && value < 256 && value >= 0)
            {
                ZeichnenTrackBarRed.Value = value;
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarRed.Value = 255;
                ZeichnenTextBoxRed.Text = @"255";
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }

            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        private void DrawTextBoxGreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(ZeichnenTextBoxGreen.Text, out var value) && value < 256 && value >= 0)
            {
                ZeichnenTrackBarGreen.Value = value;
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarGreen.Value = 255;
                ZeichnenTextBoxGreen.Text = @"255";
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }

            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        private void DrawTextBoxBlue_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(ZeichnenTextBoxBlue.Text, out var value) && value < 256 && value >= 0)
            {
                ZeichnenTrackBarBlue.Value = value;
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarBlue.Value = 255;
                ZeichnenTextBoxBlue.Text = @"255";
                ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                    (byte) ZeichnenTrackBarBlue.Value);
            }

            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        #endregion

        #region TackBars

        private void ZeichnenTrackBarRed_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxRed.Text = ZeichnenTrackBarRed.Value.ToString();
            ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                (byte) ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        private void ZeichnenTrackBarGreen_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxGreen.Text = ZeichnenTrackBarGreen.Value.ToString();
            ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                (byte) ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        private void ZeichnenTrackBarBlue_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxBlue.Text = ZeichnenTrackBarBlue.Value.ToString();
            ZeichnenFarbRad.setRGB((byte) ZeichnenTrackBarRed.Value, (byte) ZeichnenTrackBarGreen.Value,
                (byte) ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        #endregion

        /// <summary>
        ///     Sets a new color to the edit tab
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            ZeichnenTrackBarRed.Value = color.R;
            ZeichnenTrackBarGreen.Value = color.G;
            ZeichnenTrackBarBlue.Value = color.B;

            ZeichnenTextBoxRed.Text = color.R.ToString();
            ZeichnenTextBoxGreen.Text = color.G.ToString();
            ZeichnenTextBoxBlue.Text = color.B.ToString();

            ZeichnenFarbRad.setRGB(color.R, color.G, color.B);
        }

        /// <summary>
        ///     Updates trackbars and RGB-textboxes according to color wheel settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorWheel_Handler(object sender, EventArgs e)
        {
            ZeichnenTrackBarRed.Value = ZeichnenFarbRad.getRed();
            ZeichnenTrackBarGreen.Value = ZeichnenFarbRad.getGreen();
            ZeichnenTrackBarBlue.Value = ZeichnenFarbRad.getBlue();

            ZeichnenTextBoxRed.Text = ZeichnenFarbRad.getRed().ToString();
            ZeichnenTextBoxGreen.Text = ZeichnenFarbRad.getGreen().ToString();
            ZeichnenTextBoxBlue.Text = ZeichnenFarbRad.getBlue().ToString();

            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value));
        }

        /// <summary>
        ///     Fills the entire Matrix with a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawFill_Click(object sender, EventArgs e)
        {
            var color = Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value,
                ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(color);
            matrixView.Fill(color);

            commandQueue.EnqueueArduinoCommand(
                OpcodeFill, // Opcode
                (byte) ZeichnenTrackBarGreen.Value, // Red
                (byte) ZeichnenTrackBarRed.Value, // Green
                (byte) ZeichnenTrackBarBlue.Value // Blue
            );
        }

        /// <summary>
        ///     Sets the entire Matrix to black
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawClear_Click(object sender, EventArgs e)
        {
            matrixView.Fill(Color.Black);

            commandQueue.EnqueueArduinoCommand(
                OpcodeFill, // opcode
                0, // red
                0, // green
                0 // blue
            );
        }

        #endregion

        #region Image-Drag-Drop

        /// <summary>
        ///     Handles click event, opens a file dialog to choose and image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDrop_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = "c:\\",
                Filter = @"image files (*.PNG;*.JPG;*.GIF)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                LoadFromFile(filePath);
            }
        }

        /// <summary>
        ///     Loads an image file froim disk and sets the matrix to it.
        ///     If the image is an gif, the gif buffer will be set to the gif, as well as the matrix itself.
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadFromFile(string filePath)
        {
            // load gif
            if (filePath.ToLower().EndsWith(".gif"))
            {
                var gif = Image.FromFile(filePath);

                var frames = Math.Min(gif.GetFrameCount(FrameDimension.Time), 120);

                if (gif.GetFrameCount(FrameDimension.Time) > 120)
                    MessageBox.Show(
                        @"Das Gif ist zu Groß. Die Maximalgröße sind 120 Frames. Das Gif wird abgeschnitten sein, damit es in die Maximalgröße passt.",
                        @"Gif to large");

                FrameCount.Value = frames;
                Timeline.Maximum = frames - 1;
                // resize gif buffer to fit loaded gif frame count
                ResizeGif();

                // fetch and store frames
                for (var i = 0; i < frames; i++)
                {
                    gif.SelectActiveFrame(FrameDimension.Time, i);

                    // resize gif to fit scale
                    var bitmap = ResizeImage(gif, matrixView.matrixWidth(), matrixView.matrixHeight());

                    // fetch each pixel and store
                    for (var x = 0; x < bitmap.Width; x++)
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(x, y);

                        var index = x + y * bitmap.Width;

                        matrixView.SetPixelNoRefresh(x, y, pixel);

                        gifBuffer[i][index * 3 + 0] = pixel.R;
                        gifBuffer[i][index * 3 + 1] = pixel.G;
                        gifBuffer[i][index * 3 + 2] = pixel.B;
                    }
                }

                matrixView.Refresh();
                Timeline.Value = 0;
            }
            else
            {
                Bitmap bitmap = ResizeImage(new Bitmap(filePath), matrixView.matrixWidth(), matrixView.matrixHeight());
                matrixView.SetImage(bitmap);

                for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);

                    var index = x + y * bitmap.Width;

                    gifBuffer[Timeline.Value][index * 3 + 0] = pixel.R;
                    gifBuffer[Timeline.Value][index * 3 + 1] = pixel.G;
                    gifBuffer[Timeline.Value][index * 3 + 2] = pixel.B;
                }
            }

            WriteImage(gifBuffer[Timeline.Value]);
        }

        private void DragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void DragDrop_DragDrop(object sender, DragEventArgs e)
        {
            string[] picturePath = (string[]) e.Data.GetData(DataFormats.FileDrop);

            LoadFromFile(picturePath[0]);
        }

        #endregion

        #region Timeline

        public void ResetTimeline()
        {
            Timeline.Value = 1;
            Timeline.Value = 0;
        }

        public int GetDelayTime()
        {
            return (int) Delay.Value;
        }

        private void FrameCount_ValueChanged(object sender, EventArgs e)
        {
            ResizeGif();
            Timeline.Value = 0;
            if (FrameCount.Value == 1)
            {
                Timeline.Enabled = false;
            }
            else
            {
                Timeline.Enabled = true;
                Timeline.Maximum = (int) FrameCount.Value - 1;
            }
        }

        private void Timeline_ValueChanged(object sender, EventArgs e)
        {
            var timeFrame = Timeline.Value;

            WriteImage(gifBuffer[timeFrame]);

            lock (matrixView)
            {
                matrixView.SetImage(gifBuffer[timeFrame]);
            }
        }

        /// <summary>
        ///     Stores the current matrix at the index noted by the timeline into the Gif
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Click(object sender, EventArgs e)
        {
            var width = matrixView.matrixWidth();
            var height = matrixView.matrixHeight();

            for (var y = 0; y < height; y++)
            {
                var i = y * width;

                for (var x = 0; x < width; x++)
                {
                    var tmp = (i + x) * 3;

                    var color = matrixView.GetPixel(x, y);

                    gifBuffer[Timeline.Value][tmp + 0] = color.R;
                    gifBuffer[Timeline.Value][tmp + 1] = color.G;
                    gifBuffer[Timeline.Value][tmp + 2] = color.B;
                }
            }
        }

        private void Timelineupdate(object source, ElapsedEventArgs e)
        {
            if (Timeline.InvokeRequired)
                // invoke on the combo-boxes thread
                Timeline.Invoke(new Action(() =>
                {
                    if (Timeline.Value < Timeline.Maximum)
                        Timeline.Value = Timeline.Value + 1;
                    else
                        Timeline.Value = 0;
                }));
        }

        /// <summary>
        ///     Starts playing the timeline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_Click(object sender, EventArgs e)
        {
            if (FrameCount.Value != 1)
            {
                if (!runningGif)
                {
                    Play.Text = @"Stop";
                    Timeline.Value = 0;

                    runningGif = true;

                    if (delay != null)
                        delay.Enabled = true;

                    Play.Image = new Bitmap(Resources.Stop);
                }
                else
                {
                    Play.Image = new Bitmap(Resources.Play);
                    Play.Text = @"Play";
                    runningGif = false;

                    if (delay != null)
                        delay.Enabled = false;
                }
            }
        }

        private void Timeline_MouseDown(object sender, MouseEventArgs e)
        {
            if (!runningGif) return;
            Play.Image = new Bitmap(Resources.Play);
            Play.Text = @"Play";
            runningGif = false;

            if (delay != null)
                delay.Enabled = false;
        }

        private void Delay_ValueChanged(object sender, EventArgs _)
        {
            if (delay != null)
                delay.Interval = (int) Delay.Value;
        }

        #endregion

        #region Properties

        private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new()
            {
                InitialDirectory = "c:\\",
                Filter = @"image files (*.PNG;*.JPG;*.GIF)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (save.ShowDialog() == DialogResult.OK)
            {
                string filePath = save.FileName;
                Bitmap[] gifBitmap = new Bitmap[gifBuffer.Length];
                GifWriter writer = new(File.Create(filePath));
                for (var i = 0; i < FrameCount.Value; i++)
                {
                    gifBitmap[i] = new Bitmap((int) matrixWidth.Value, (int) matrixHeight.Value);

                    for (var j = 0; j < gifBuffer[i].Length / 3; j++)
                    {
                        var y = j / (int) matrixWidth.Value;
                        var x = j % (int) matrixWidth.Value;

                        gifBitmap[i].SetPixel(x, y,
                            Color.FromArgb(gifBuffer[i][j * 3], gifBuffer[i][j * 3 + 1], gifBuffer[i][j * 3 + 2]));
                    }

                    writer.WriteFrame(gifBitmap[i], (int) Delay.Value);
                }

                writer.Dispose();
            }
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            commandQueue.EnqueueArduinoCommand(4);
            PortCommandQueue.WaitForLastDequeue();
            byte[] data = commandQueue.GetLastData();

            if (commandQueue.GetMark() > 0)
            {
                int width = data[0];
                int height = data[1];

                matrixWidth.Value = width;
                matrixHeight.Value = height;

                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var i0 = x * 3 + y * width * 3;

                    var x1 = height - y - 1;
                    var y1 = width - x - 1;

                    var i1 = x1 * 3 + y1 * width * 3;

                    // degamma
                    gifBuffer[0][i0 + 0] = (byte) MathF.Sqrt(data[i1 + 0 + 2] / 258.0f * 65536.0f);
                    gifBuffer[0][i0 + 1] = (byte) MathF.Sqrt(data[i1 + 1 + 2] / 258.0f * 65536.0f);
                    gifBuffer[0][i0 + 2] = (byte) MathF.Sqrt(data[i1 + 2 + 2] / 258.0f * 65536.0f);
                }

                Timeline.Value = 1;
                Timeline.Value = 0;
            }
        }

        private void showGridCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            matrixView.DrawGrid = showGridCheckbox.Checked;
            matrixView.Refresh();
        }

        private void BrushSizeSlider_Scroll(object sender, EventArgs e)
        {
            matrixView.brushSize = BrushSizeSlider.Value;
        }

        /// <summary>
        ///     Resizes the Gif image buffer
        /// </summary>
        private void ResizeGif()
        {
            var frames = (int) FrameCount.Value;
            gifBuffer = new byte[frames + 1][];
            for (var i = 0; i <= frames; i++)
                gifBuffer[i] = new byte[matrixView.matrixWidth() * matrixView.matrixHeight() * 3];
        }

        #endregion

        #region Appearance

        private void ApplyDarkModeButton_Click(object sender, EventArgs e)
        {
            new DarkMode().ApplyTheme(this);
        }

        private void ApplyLightModeButton_Click(object sender, EventArgs e)
        {
            new LightMode().ApplyTheme(this);
        }

        #endregion

        #endregion

        #region IO-Utils

        private void WriteImage(byte[] rgbImageData)
        {
            var gammaImage = new byte[rgbImageData.Length];

            var width = matrixView.matrixWidth();
            var height = matrixView.matrixHeight();

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var i0 = x * 3 + y * width * 3;

                ;
                var x1 = height - y - 1;
                var y1 = width - x - 1;

                var i1 = x1 * 3 + y1 * width * 3;

                gammaImage[i0 + 0] = rgbImageData[i1 + 0];
                gammaImage[i0 + 1] = rgbImageData[i1 + 1];
                gammaImage[i0 + 2] = rgbImageData[i1 + 2];
            }

            for (var i = 0; i < rgbImageData.Length; i++)
                gammaImage[i] = (byte) ((gammaImage[i] * gammaImage[i] * 258) >> 16);

            commandQueue.EnqueueArduinoCommand(OpcodeImage, gammaImage);
        }

        /// <summary>
        ///     Converts the matrix's pixel ARGB buffer to an 3-byte RGB tuple array and sends them to the arduino
        /// </summary>
        public void EnqueuePixelSet()
        {
            var pixels = matrixView.getPixels();

            byte[] image = new byte[pixels.Length * 3];

            for (var x = 0; x < pixels.Length; x++)
            {
                image[x * 3 + 0] = (byte) ((pixels[x] >> 16) & 0xFF);
                image[x * 3 + 1] = (byte) ((pixels[x] >> 8) & 0xFF);
                image[x * 3 + 2] = (byte) ((pixels[x] >> 0) & 0xFF);
            }

            WriteImage(image);
        }

        #endregion

        private void PushButtonOnClick(object? sender, EventArgs e)
        {
            var bytes = matrixWidth.Value * matrixHeight.Value * 3 * gifBuffer.Length + 5;
            var data = new byte[(int) bytes];

            data[0] = (byte) matrixWidth.Value;
            data[1] = (byte) matrixHeight.Value;
            data[2] = (byte) gifBuffer.Length;
            data[3] = (byte) ((int) Delay.Value >> 8);
            data[4] = (byte) ((int) Delay.Value & 0xFF);

            for (var frame = 0; frame < gifBuffer.Length; frame++)
            {
                for (var pixel = 0; pixel < gifBuffer[0].Length; pixel++)
                {
                    data[frame * gifBuffer[0].Length + pixel + 5] = (byte) (gifBuffer[frame][pixel] * gifBuffer[frame][pixel] * 258 >> 16);
                }
            }
            
            commandQueue.EnqueueArduinoCommand(OpcodePush, data);
        }
    }
}