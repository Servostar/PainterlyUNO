using Matrix_App.PregeneratedMods;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Matrix_App.minecraft;
using Matrix_App.PregeneratedMods.reflection;
using static Matrix_App.Utils;
using static Matrix_App.Defaults;
using Timer = System.Windows.Forms.Timer;

namespace Matrix_App
{
    public abstract class MatrixGifGenerator
    {
        private static readonly MatrixGifGenerator[] Generators =
        {
            new SimpleRainbow(),
            new Rain(),
            new Spiral(),
            new UvGrid(),
            new RandomPixels(),
            new Boxblur(),
            new ColorAdjust(),
            new Grayscale(),
            new Invert(),
            new Transfom(),
            new Minecraft()
        };
        
        // Static generator accessible members
        // must work on multiple threads
        protected static int totalFrames;   // total amount of frames to generate
        protected static byte[][]? actualStore; // image copy of previous GIF for generator

        protected static int width;
        protected static int height;
        
        // updates the preview matrix for animation
        private static readonly Timer PlaybackTimer = new Timer();
        // current frame to play
        private static int _playbackFrame;

        // temporary buffer for storing snapshots for buttons 
        private static readonly byte[][] Snapshot;
        private static byte[][] _initialBuffer; // temporary buffer for swapping

        // Generator renderer
        private static readonly ThreadQueue Renderer;

        // Current generator to use
        private static MatrixGifGenerator? _generator;

        static MatrixGifGenerator()
        {
            PlaybackTimer.Tick += PlaybackFrame;
            
            // Generate buffer for button filter snapshots
            Snapshot     = CreateImageRGB_NT(FilterPreviewWidth, FilterPreviewHeight, 1);
            _initialBuffer = CreateImageRGB_NT(FilterPreviewWidth, FilterPreviewHeight, 1);

            Renderer = new ThreadQueue("Matrix Gif Renderer", 2);
        }
        
        /// <summary>
        /// Plays the next frame of what is currently in the animation buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PlaybackFrame(object? sender, EventArgs e)
        {
            if (_playbackFrame >= _animationBuffer.Length - 1)
            {
                _playbackFrame = 0;
            }

            _preview.SetImage(_animationBuffer[_playbackFrame]);

            _playbackFrame++;
        }

        public delegate void update();

        /// <summary>
        /// Colors a single fragment at the specified pixel location (x|y) at frame frame.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="u">Normalized pixel X-coordinate</param>
        /// <param name="v">Normalized pixel Y-coordinate</param>
        /// <param name="frame">Current frame</param>
        /// <param name="r">Pixel Red value in range [0, 1] (saturated)</param>
        /// <param name="g">Pixel Green value in range [0, 1] (saturated)</param>
        /// <param name="b">Pixel Blue value in range [0, 1] (saturated)</param>
        protected abstract void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b);

        protected abstract void CreateUi(FlowLayoutPanel anchor, update runner);

        // Buffer to store generator result in
        private static byte[][] _animationBuffer = null!;

        // Main application reference
        private static MatrixDesignerMain _form = null!;
        private static Matrix _preview = null!; // preview matrix

        public static void GenerateBaseUi(FlowLayoutPanel anchor, Matrix matrix, MatrixDesignerMain form1)
        {
            _form = form1;

            // generate access buttons for available generators
            foreach (var generator in Generators)
            {
                // generate button
                var button = new Button
                {
                    AutoSize = true,
                    Text = FieldWidgets.GetBetterFieldName(generator.GetType().Name)
                };
                button.Width = anchor.ClientSize.Width - button.Margin.Right - button.Margin.Left;
                button.Click += (sender, e) =>
                {
                    lock (matrix)
                    {
                        OpenGeneratorUi(generator, matrix);
                    }
                };
                button.Image = CreateSnapshot(generator);
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.TextAlign = ContentAlignment.MiddleRight;
                button.ImageAlign = ContentAlignment.MiddleLeft;

                anchor.Controls.Add(button);
            }
        }

        private static Image CreateSnapshot(MatrixGifGenerator matrixGifGenerator)
        {
            _generator = new RandomPixels();
            // put some random pixels in as default initial image to operate on for filter
            SetGlobalArgs(FilterPreviewWidth, FilterPreviewHeight, 1, null, _initialBuffer);
            InvokeGenerator();

            BlockBuffer();
            
            _generator = matrixGifGenerator;

            // render filter
            SetGlobalArgs(FilterPreviewWidth, FilterPreviewHeight, 1, _initialBuffer, Snapshot);
            InvokeGenerator();

            BlockBuffer();
            
            // convert to bitmap
            return ImageWrap(Snapshot[0], width, height);
        }

        /// <summary>
        /// Blocks this thread until no more work is done by the Renderer thread queue 
        /// </summary>
        private static void BlockBuffer()
        {
            while (Renderer.HasWork())
            {
                Thread.Sleep(50);
            }
        }
        
        private static void OpenGeneratorUi(MatrixGifGenerator matrixGifGenerator, Matrix matrix)
        {
            _generator = matrixGifGenerator;
            
            if (!ShowEditDialog(matrix))
                return;

            if (Renderer.HasWork())
            {
                if (DialogResult.Yes ==
                    MessageBox.Show($@"The filter {_generator.GetType().Name} hasn't finished yet, wait for completion?",
                        @"Filter incomplete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    BlockBuffer();
                    ColorStore(_animationBuffer, MatrixDesignerMain.gifBuffer);
                    _form.ResetTimeline();
                }
                else
                {
                    MessageBox.Show($@"The filter {_generator.GetType().Name} has timedout", @"Failed applying filter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ColorStore(_animationBuffer, MatrixDesignerMain.gifBuffer);
                _form.ResetTimeline();
            }
        }

        private static void SetGlobalArgs(int w, int h, int f, in byte[][]? previous, in byte[][] preview)
        {
            totalFrames = f;

            width = w;
            height = h;

            _animationBuffer = preview;
            actualStore = previous;
        }

        private static bool ShowEditDialog(Matrix matrix)
        {
            if (_generator == null)
            {
                return false;
            }
            
            var success = false;

            Initialize(matrix);

            Form prompt = new Form
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Text = @"Vorgenerierter Modus: " + _generator.GetType().Name
            };
            
            var confirmation = new Button {Text = @"Apply", Anchor = AnchorStyles.Top | AnchorStyles.Left};
            confirmation.Click += (sender, e) => {
                success = true;
                prompt.Close();
            };

            FlowLayoutPanel controlPanel = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                FlowDirection = FlowDirection.BottomUp,
                Dock = DockStyle.Fill,
                WrapContents = false,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                AutoSize = true
            };

            _generator.CreateUi(controlPanel, InvokeGenerator);
            
            PlaybackTimer.Interval = _form.GetDelayTime();
            PlaybackTimer.Enabled = true;

            var type = _generator.GetType();
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            CreateDivider(controlPanel, 2);
            foreach (var field in fields)
            {
                var widget = FieldWidgets.GetFieldWidget(field, _generator, InvokeGenerator);

                if (widget == null) 
                    continue;
                
                controlPanel.Controls.AddRange(widget);
                CreateDivider(controlPanel, 1);
            }

            controlPanel.Controls.Add(_preview);

            FlowLayoutPanel southPane = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, 
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            southPane.Controls.Add(confirmation);
            southPane.AutoSize = true;
            
            // render once
            InvokeGenerator();

            prompt.MinimumSize = prompt.Size;
            prompt.Controls.Add(controlPanel);
            prompt.Controls.Add(southPane);
            prompt.MaximizeBox = false;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();

            PlaybackTimer.Enabled = false;

            return success;
        }

        private static void Initialize(in Matrix matrix)
        {
            // Create new initial buffer and copy what ever was in the Gif buffer to it
            _initialBuffer = CreateImageRGB_NT(matrix.matrixWidth(), matrix.matrixHeight(), MatrixDesignerMain.gifBuffer.Length);
            ColorStore(MatrixDesignerMain.gifBuffer, _initialBuffer);
            // Set Generator args
            SetGlobalArgs(matrix.matrixWidth(),
                          matrix.matrixHeight(),
                          MatrixDesignerMain.gifBuffer.Length - 1,
                          _initialBuffer,
                          CreateImageRGB_NT(matrix.matrixWidth(), matrix.matrixHeight(), MatrixDesignerMain.gifBuffer.Length)
                    );
            
            // Create preview matrix
            _preview = new Matrix();
            _preview.SetEditable(false);
            _preview.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _preview.Size = new Size(480, 200);
            _preview.resize(matrix.matrixWidth(), matrix.matrixHeight());
        }

        /// <summary>
        /// Adds a separating line to the controls
        /// </summary>
        /// <param name="controlPanel"></param>
        /// <param name="lineHeight"></param>
        private static void CreateDivider(Control controlPanel, int lineHeight)
        {
            var divider = new Label
            {
                BorderStyle = BorderStyle.Fixed3D, 
                AutoSize = false,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Height = lineHeight,
            };

            controlPanel.Controls.Add(divider);
        }

        private static void InvokeGenerator()
        {
            Renderer.Enqueue(delegate
            {
                for (var frame = 0; frame < _animationBuffer.Length; frame++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var u = x / (float) width;

                        for (var y = 0; y < height; y++)
                        {
                            var v = y / (float) height;

                            _generator!.ColorFragment(x, y, u, v, frame, out var r, out var g, out var b);

                            var index = (x + y * width) * 3;

                            _animationBuffer[frame][index]     = (byte) Math.Clamp( (int)(r * 255), 0, 255);
                            _animationBuffer[frame][index + 1] = (byte) Math.Clamp( (int)(g * 255), 0, 255);
                            _animationBuffer[frame][index + 2] = (byte) Math.Clamp( (int)(b * 255), 0, 255);
                        }
                    }
                }
                
                return true;
            });
        }

        public static void Close()
        {
            Renderer.Stop();
        }
    }
}
