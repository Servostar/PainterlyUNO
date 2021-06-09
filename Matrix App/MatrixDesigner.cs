#define DEBUG_ENABLED
#define DEBUG_ENABLED

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.Timers;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;

using static Matrix_App.Defaults;
using static Matrix_App.ArduinoInstruction;
using static Matrix_App.Utils;
using Matrix_App.Themes;
using Timer = System.Timers.Timer;

namespace Matrix_App
{
    public partial class MatrixDesignerMain : Form
    {
        #region Private-Members

        /// <summary>
        /// Port update Timer
        /// Reloads available port names at consecutive rates
        /// </summary>
        private Timer? portNameUpdater;
        private Timer? delay;

        private static SerialPort _port = new SerialPort();

        private uint portNumber;

        private bool runningGif;

        private readonly PortCommandQueue commandQueue = new PortCommandQueue(ref _port);
        private readonly Regex comRegex = new Regex(@"COM[\d]+");
        /// <summary>
        /// Gif like frame video buffer
        /// </summary>
        public byte[][] gifBuffer = CreateImageRGB_NT(MatrixStartWidth, MatrixStartHeight, MatrixStartFrames);
       
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
        }
        
        private void Init()
        {
            // Create port name update timer
            portNameUpdater = new Timer(PortNameUpdateInterval);
            portNameUpdater.Elapsed += UpdatePortNames;
            portNameUpdater.AutoReset = true;
            portNameUpdater.Enabled = true;
            
            // create gif playback timer
            delay = new Timer((int) Delay.Value);
            delay.Elapsed += Timelineupdate;
            delay.AutoReset = true;

            // Set color wheel event handler
            ZeichnenFarbRad.handler = ColorWheel_Handler!;

            // setup port settings
            _port.BaudRate = BaudRate;
            _port.ReadTimeout = ReadTimeoutMs;
            _port.WriteTimeout = WriteTimeoutMs;

            // setup matrix
            AdjustMatrixTable();

            // search for initial ports
            GatherPortNames();

            HideEasterEgg();
        }

        private void HideEasterEgg()
        {
            if (((int) DateTime.Now.DayOfWeek) != 3) 
                return;
            
            if (new Random().Next(0, 9) <= 1)
                return;
            
            using (Bitmap wednesdayFrog = new Bitmap(Properties.Resources.Frosch))
            {
                matrixWidth.Value = wednesdayFrog.Width;
                matrixHeight.Value = wednesdayFrog.Height;
                ResizeGif();
                
                for (var x = 0; x < wednesdayFrog.Width; x++)
                {
                    for (var y = 0; y < wednesdayFrog.Height; y++)
                    {
                        var pixel = wednesdayFrog.GetPixel(x, y);

                        matrixView.SetPixelNoRefresh(x, y, pixel);

                    }
                }
            }

            matrixView.Refresh();
        }

        #endregion

        #region UI-Methods

        #region Port-ComboBox
        /// <summary>
        /// Updates the port names to newest available ports.
        /// Called by <see cref="portNameUpdater"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void UpdatePortNames(object source, ElapsedEventArgs e)
        {
            if (Ports.InvokeRequired)
            {
                // invoke on the combo-boxes thread
                Ports.Invoke(new Action(GatherPortNames));
            }
            else
            {
                // run on this thread
                GatherPortNames();
            }
        }

        /// <summary>
        /// Gathers all available ports and sets them to the combobox <see cref="Ports"/>
        /// </summary>
        [SuppressMessage("ReSharper", "CoVariantArrayConversion", Justification = "Never got an exception, so seems to be just fine")]
        private void GatherPortNames()
        {
            var ports = SerialPort.GetPortNames();
            // save previously selected 
            var selected = this.Ports.SelectedItem;
            // get device names from ports
            var newPorts = GetDeviceNames(ports);
            // add virtual port
            newPorts.AddLast("Virtual-Unlimited (COM257)");

            // search for new port
            foreach (var newPort in newPorts)
            {
                // find any new port
                var found = Ports.Items.Cast<object?>().Any(oldPort => (string) oldPort! == newPort);

                // some port wasn't found, recreate list
                if (!found)
                {
                    commandQueue.InvalidatePort();

                    Ports.Items.Clear();

                    Ports.Items.AddRange(newPorts.ToArray()!);

                    // select previously selected port if port is still accessible
                    if (selected != null && Ports.Items.Contains(selected))
                    {
                        Ports.SelectedItem = selected;
                    } else
                    {
                        Ports.SelectedIndex = 0;
                    }
                    break;
                }
            }
        }

        private static LinkedList<string> GetDeviceNames(string[] ports)
        {
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection devicePortNames = processClass.GetInstances();

            var newPorts = new LinkedList<string>();

            foreach (var currentPort in ports)
            {
                foreach (var o in devicePortNames)
                {
                    var name = ((ManagementObject) o).GetPropertyValue("Name");
                    
                    if (name == null || !name.ToString()!.Contains(currentPort)) 
                        continue;
                    
                    newPorts.AddLast(name.ToString()!);
                    break;
                }
            }

            return newPorts;
        }

        /// <summary>
        /// Invoked when the selected port has changed.
        /// Applies the new port settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ports_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (_port)
            {
                if (!_port.IsOpen)
                {
                    var item = (string)((ComboBox)sender).SelectedItem;
                    if (item != null)
                    {
                        // extract port
                        var matches = comRegex.Matches(item);
                        
                        if(matches.Count > 0)
                        {
                            // only select valid port numbers (up to (including) 256)
                            portNumber = UInt32.Parse(matches[0].Value.Split('M')[1]);

                            if (portNumber <= 256)
                            {
                                // set valid port
                                _port.PortName = matches[0].Value;
                                commandQueue.ValidatePort();
                            } else if (portNumber == 257)
                            {
                                // virtual mode, increase limitations as no real arduino is connected
                                matrixWidth.Maximum = MatrixLimitedWidth;
                                matrixHeight.Maximum = MatrixLimitedHeight;
                            } else
                            {
                                // no port selected reset settings
                                matrixWidth.Maximum = MatrixStartWidth;
                                matrixHeight.Maximum = MatrixStartHeight;
                            }
                        }
                    }
                }
            }
        }
       
        #endregion

        #region Scale
        /// <summary>
        /// Applies a new size to the gif and matrix
        /// </summary>
        private void AdjustMatrixTable()
        {
            int width = (int)this.matrixWidth.Value;
            int height = (int)this.matrixHeight.Value;

            matrixView.resize(width, height);
            ResizeGif();
           // Delay.Minimum = Math.Min(Width.Value * Height.Value * 5, 500);
        }

        private void Width_ValueChanged(object sender, EventArgs e)
        {
            AdjustMatrixTable();
            commandQueue.EnqueueArduinoCommand(
                OpcodeScale,  // opcode
                (byte)matrixWidth.Value,
                (byte)matrixHeight.Value
            );
        }

        private void Height_ValueChanged(object sender, EventArgs e)
        {
            AdjustMatrixTable();
            commandQueue.EnqueueArduinoCommand(
                OpcodeScale,  // opcode
                (byte)matrixWidth.Value,
                (byte)matrixHeight.Value
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
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarRed.Value = 255;
                ZeichnenTextBoxRed.Text = @"255";
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));
        }

        private void DrawTextBoxGreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(ZeichnenTextBoxGreen.Text, out var value) && value < 256 && value >= 0)
            {
                ZeichnenTrackBarGreen.Value = value;
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarGreen.Value = 255;
                ZeichnenTextBoxGreen.Text = @"255";
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));

        }

        private void DrawTextBoxBlue_KeyUp(object sender, KeyEventArgs e)
        {
            if (int.TryParse(ZeichnenTextBoxBlue.Text, out var value) && value < 256 && value >= 0)
            {
                ZeichnenTrackBarBlue.Value = value;
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            else if (value >= 256)
            {
                ZeichnenTrackBarBlue.Value = 255;
                ZeichnenTextBoxBlue.Text = @"255";
                ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            }
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));

        }
        #endregion

        #region TackBars
        private void ZeichnenTrackBarRed_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxRed.Text = ZeichnenTrackBarRed.Value.ToString();
            ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));
        }

        private void ZeichnenTrackBarGreen_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxGreen.Text = ZeichnenTrackBarGreen.Value.ToString();
            ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));
        }

        private void ZeichnenTrackBarBlue_Scroll(object sender, EventArgs e)
        {
            ZeichnenTextBoxBlue.Text = ZeichnenTrackBarBlue.Value.ToString();
            ZeichnenFarbRad.setRGB((byte)ZeichnenTrackBarRed.Value, (byte)ZeichnenTrackBarGreen.Value, (byte)ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));
        }
        #endregion

        /// <summary>
        /// Sets a new color to the edit tab
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
        /// Updates trackbars and RGB-textboxes according to color wheel settings
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

            matrixView.SetPaintColor(Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value));
        }

        /// <summary>
        /// Fills the entire Matrix with a color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawFill_Click(object sender, EventArgs e)
        {
            var color = Color.FromArgb(ZeichnenTrackBarRed.Value, ZeichnenTrackBarGreen.Value, ZeichnenTrackBarBlue.Value);
            matrixView.SetPaintColor(color);
            matrixView.Fill(color);

            commandQueue.EnqueueArduinoCommand(
                 OpcodeFill, // Opcode
                (byte)ZeichnenTrackBarRed.Value,   // Red
                (byte)ZeichnenTrackBarGreen.Value,// Green
                (byte)ZeichnenTrackBarBlue.Value   // Blue
             );
        }

        /// <summary>
        /// Sets the entire Matrix to black
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawClear_Click(object sender, EventArgs e)
        {
            matrixView.Fill(Color.Black);

            commandQueue.EnqueueArduinoCommand(
                OpcodeFill,  // opcode
                0,  // red
                0,  // green
                0   // blue
            );
        }

        #endregion

        #region Image-Drag-Drop
        
        /// <summary>
        /// Handles click event, opens a file dialog to choose and image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragDrop_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
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
        /// Loads an image file froim disk and sets the matrix to it.
        /// If the image is an gif, the gif buffer will be set to the gif, as well as the matrix itself.
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
                {
                    MessageBox.Show(@"Das Gif ist zu Groß. Die Maximalgröße sind 120 Frames. Das Gif wird abgeschnitten sein, damit es in die Maximalgröße passt.", @"Gif to large");
                }

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
                    {
                        for (var y = 0; y < bitmap.Height; y++)
                        {
                            var pixel = bitmap.GetPixel(x, y);

                            var index = x + y * bitmap.Width;

                            matrixView.SetPixelNoRefresh(x, y, pixel);

                            gifBuffer[i][index * 3] = pixel.G;
                            gifBuffer[i][index * 3 + 1] = pixel.R;
                            gifBuffer[i][index * 3 + 2] = pixel.B;
                        }
                    }
                }
                matrixView.Refresh();
                Timeline.Value = 0;
               
            }
            else
            {
                Bitmap bitmap = new Bitmap(filePath);
                bitmap = ResizeImage(bitmap, matrixView.matrixWidth(), matrixView.matrixHeight());
                matrixView.SetImage(bitmap);


                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(x, y);

                        int index = x + y * bitmap.Width;

                        gifBuffer[Timeline.Value][index * 3] = pixel.G;
                        gifBuffer[Timeline.Value][index * 3 + 1] = pixel.R;
                        gifBuffer[Timeline.Value][index * 3 + 2] = pixel.B;
                    }
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
            string[] picturePath = (string[])e.Data.GetData(DataFormats.FileDrop);

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
            return (int)Delay.Value;
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
                Timeline.Maximum = (int)FrameCount.Value - 1;
            }
        }

        private void Timeline_ValueChanged(object sender, EventArgs e)
        {
            int width = matrixView.matrixWidth();
            int height = matrixView.matrixHeight();

            for (int y = 0; y < height; y++)
            {
                int index = y * width;

                for (int x = 0; x < width; x++)
                {
                    int tmp = (index + x) * 3;

                    var color = Color.FromArgb(gifBuffer[Timeline.Value][tmp + 1], gifBuffer[Timeline.Value][tmp], gifBuffer[Timeline.Value][tmp + 2]);

                    matrixView.SetPixelNoRefresh(x, y, color);
                }
            }
            matrixView.Refresh();
            WriteImage(gifBuffer[Timeline.Value]);
        }

        /// <summary>
        /// Stores the current matrix at the index noted by the timeline into the Gif
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Click(object sender, EventArgs e)
        {
            int width = matrixView.matrixWidth();
            int height = matrixView.matrixHeight();

            for (int y = 0; y < height; y++)
            {
                int i = y * width;

                for (int x = 0; x < width; x++)
                {
                    int tmp = (i + x) * 3;

                    var color = matrixView.GetPixel(x, y);

                    gifBuffer[Timeline.Value][tmp] = color.G;
                    gifBuffer[Timeline.Value][tmp + 1] = color.R;
                    gifBuffer[Timeline.Value][tmp + 2] = color.B;
                }
            }
        }

        private void Timelineupdate(Object source, ElapsedEventArgs e)
        {
            if (Timeline.InvokeRequired)
            {
                // invoke on the comboboxes thread
                Timeline.Invoke(new Action(() =>
                {
                    if (Timeline.Value < Timeline.Maximum)
                    {
                        Timeline.Value = Timeline.Value + 1;
                    }
                    else
                    {
                        Timeline.Value = 0;
                    }
                }));
            }
        }

        /// <summary>
        /// Starts playing the timeline
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

                    Play.Image = new Bitmap(Properties.Resources.Stop);
                }
                else
                {
                    Play.Image = new Bitmap(Properties.Resources.Play);
                    Play.Text = @"Play";
                    runningGif = false;
                    
                    if (delay != null) 
                        delay.Enabled = false;
                }
            }
        }

        private void Timeline_MouseDown(object sender, MouseEventArgs e)
        {
            if (runningGif)
            {
                Play.Image = new Bitmap(Properties.Resources.Play);
                Play.Text = @"Play";
                runningGif = false;
                
                if (delay != null) 
                    delay.Enabled = false;
            }
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
            SaveFileDialog save = new SaveFileDialog
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
                GifWriter writer = new GifWriter(File.Create(filePath));
                for (var i = 0; i < FrameCount.Value; i++)
                {
                    gifBitmap[i] = new Bitmap((int)matrixWidth.Value, (int)matrixHeight.Value);

                    for (var j = 0; j < gifBuffer[i].Length / 3; j++)
                    {
                        var y = j / (int)matrixWidth.Value;
                        var x = j % (int)matrixWidth.Value;

                        gifBitmap[i].SetPixel(x, y, Color.FromArgb(gifBuffer[i][j * 3 + 1], gifBuffer[i][j * 3], gifBuffer[i][j * 3 + 2]));
                    }
                    writer.WriteFrame(gifBitmap[i], (int)Delay.Value);
                }
                writer.Dispose();
            }
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            commandQueue.EnqueueArduinoCommand(4);
            commandQueue.WaitForLastDequeue();
            byte[] data = commandQueue.GetLastData();

            if (commandQueue.GetMark() > 0)
            {
                int width = data[0];
                int height = data[1];

                this.matrixWidth.Value = width;
                this.matrixHeight.Value = height;

                for (var x = 0; x < width * height * 3; x++)
                {
                    gifBuffer[0][x] = data[2 + x];
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
        /// Resizes the Gif image buffer
        /// </summary>
        private void ResizeGif()
        {
            int frames = (int)FrameCount.Value;
            gifBuffer = new byte[frames + 1][];
            for (int i = 0; i <= frames; i++)
            {
                gifBuffer[i] = new byte[matrixView.matrixWidth() * matrixView.matrixHeight() * 3];
            }
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
            commandQueue.EnqueueArduinoCommand(OpcodeImage, rgbImageData);
        }

        /// <summary>
        /// Converts the matrix's pixel ARGB buffer to an 3-byte RGB tuple array and sends them to the arduino
        /// </summary>
        public void EnqueuePixelSet()
        {
            var pixels = matrixView.getPixels();

            byte[] image = new byte[pixels.Length * 3];

            for (int x = 0; x < pixels.Length; x++)
            {
                image[x * 3] = (byte)(pixels[x] >> 8 & 0xFF);
                image[x * 3 + 1] = (byte)(pixels[x] >> 16 & 0xFF);
                image[x * 3 + 2] = (byte)(pixels[x] & 0xFF);
            }

            WriteImage(image);
        }

        #endregion
    }
}
