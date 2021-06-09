using Matrix_App.PregeneratedMods;
using System;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static Matrix_App.Utils;
using static MatrixDesigner.Defaults;
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
            new Invert()
        };

        private static readonly Timer PlaybackTimer = new Timer();

        private static int _playbackFrame;

        protected static int totalFrames;
        protected static byte[][]? actualStore;

        protected static int width;
        protected static int height;

        private static readonly byte[][] Snapshot;
        private static byte[][] _initialBuffer;

        private static readonly ThreadQueue Renderer;

        private static MatrixGifGenerator? _generator;

        static MatrixGifGenerator()
        {
            PlaybackTimer.Tick += PlaybackFrame;

            Snapshot     = CreateImageRGB_NT(FILTER_PREVIEW_WIDTH, FILTER_PREVIEW_HEIGHT, 1);
            _initialBuffer = CreateImageRGB_NT(FILTER_PREVIEW_WIDTH, FILTER_PREVIEW_HEIGHT, 1);

            Renderer = new ThreadQueue("Matrix Gif Renderer", 2);
        }
        private static void PlaybackFrame(object? sender, EventArgs e)
        {
            if (_playbackFrame >= _animationBuffer.Length - 1)
            {
                _playbackFrame = 0;
            }

            _preview.SetImage(_animationBuffer[_playbackFrame]);

            _playbackFrame++;
        }

        protected abstract void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b);

        private static byte[][] _animationBuffer = null!;

        private static MatrixDesignerMain _form = null!;
        private static Matrix _preview = null!;

        public static void GenerateBaseUi(FlowLayoutPanel anchor, Matrix matrix, MatrixDesignerMain form1)
        {
            _form = form1;

            // generate access buttons for available generators
            foreach (var generator in Generators)
            {
                var button = new Button
                {
                    Width = 215, 
                    Text = GetBetterFieldName(generator.GetType().Name)
                };
                button.Click += (sender, e) => OpenGeneratorUi(generator, matrix);
                button.Image = CreateSnapshot(generator);
                button.TextImageRelation = TextImageRelation.ImageAboveText;
                button.Height = FILTER_PREVIEW_HEIGHT * 2;

                anchor.Controls.Add(button);
            }
        }

        private static Image CreateSnapshot(MatrixGifGenerator matrixGifGenerator)
        {
            _generator = new RandomPixels();
            // put some random pixels in as default initial image to operate on for filter
            SetGlobalArgs(FILTER_PREVIEW_WIDTH, FILTER_PREVIEW_HEIGHT, 1, null, _initialBuffer);
            InvokeGenerator();

            BlockBuffer();
            
            _generator = matrixGifGenerator;

            // render filter
            SetGlobalArgs(FILTER_PREVIEW_WIDTH, FILTER_PREVIEW_HEIGHT, 1, _initialBuffer, Snapshot);
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
            
            FlipColorStoreRG_GR(_animationBuffer, _form.Gif);
            _form.ResetTimeline();
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
            var success = false;

            Initialize(matrix);

            Form prompt = new Form
            {
                Width = 500, 
                Height = 320, 
                Text = @"Vorgenerierter Modus: " + _generator.GetType().Name
            };

            var confirmation = new Button {Text = "Apply", Anchor = AnchorStyles.Top | AnchorStyles.Left};
            confirmation.Click += (sender, e) => {
                success = true;
                prompt.Close();
            };

            FlowLayoutPanel controlPanel = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                FlowDirection = FlowDirection.BottomUp,
                Dock = DockStyle.Top,
                WrapContents = false,
                AutoSize = true
            };

            var fields = _generator.GetType().GetFields();

            PlaybackTimer.Interval = _form.GetDelayTime();
            PlaybackTimer.Enabled = true;

            CreateDivider(controlPanel);
            foreach (var field in fields)
            {
                if (field.IsStatic || !field.IsPublic) 
                    continue;
                
                var fieldValue = field.GetValue(_generator);

                controlPanel.Controls.Add(GetFieldUi(field, fieldValue, _generator));
            }
            
            if (controlPanel.Controls.Count > 1)
            {
                CreateDivider(controlPanel);
 
                var label = new Label() { Text = "Settings" };
                label.Font = new Font(label.Font, FontStyle.Bold);
                controlPanel.Controls.Add(label);
            }

            controlPanel.Controls.Add(_preview);
            CreateDivider(controlPanel);
            var playLabel = new Label() { Text = "Playback preview" };
            playLabel.Font = new Font(playLabel.Font, FontStyle.Bold);
            controlPanel.Controls.Add(playLabel);

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
            prompt.Height = southPane.Height * 2 + controlPanel.Height + 16;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.MaximizeBox = false;
            prompt.ShowDialog();

            PlaybackTimer.Enabled = false;

            return success;
        }

        private static Control GetFieldUi(FieldInfo field, object? fieldValue, MatrixGifGenerator generator)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true
            };

            panel.Controls.Add(new Label
            {
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Text = GetBetterFieldName(field.Name),
                Dock = DockStyle.Left,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Width = 100
            });

            switch (fieldValue)
            {
                case int value:
                {
                    var upDown = new NumericUpDown
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Width = 360,
                        Value = value
                    };
                    upDown.ValueChanged += (a, b) =>
                    {
                        field.SetValue(generator, (int) upDown.Value);
                        InvokeGenerator();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
                case bool value1:
                {
                    var upDown = new CheckBox
                    {
                        Dock = DockStyle.Fill, Anchor = AnchorStyles.Top | AnchorStyles.Right, Checked = value1
                    };
                    upDown.CheckedChanged += (a, b) =>
                    {
                        field.SetValue(generator, (bool) upDown.Checked);
                        InvokeGenerator();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
                case float floatValue:
                {
                    var upDown = new TrackBar
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Maximum = 100,
                        Minimum = 0,
                        Value = (int) (floatValue * 100.0f),
                        TickFrequency = 10,
                        Width = 360
                    };
                    upDown.ValueChanged += (a, b) =>
                    {
                        field.SetValue(generator, upDown.Value / 1e2f);
                        InvokeGenerator();
                    };

                    panel.Controls.Add(upDown);
                    break;
                }
            }

            return panel;
        }

        /// <summary>
        /// Generates a new name from standard class names
        /// Example: SomeClassA --> some class a
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetBetterFieldName(string name)
        {
            var groups = Regex.Match(name, @"([A-Z]*[a-z]+)([A-Z]+[a-z]*)|(.*)").Groups;

            var newName = "";

            for (var c = 1; c < groups.Count; c++)
            {
                newName += groups[c].Value.ToLower() + " ";
            }

            return newName;
        }

        private static void Initialize(in Matrix matrix)
        {
            // Create new initial buffer and copy what ever was in the Gif buffer to it
            _initialBuffer = CreateImageRGB_NT(matrix.matrixWidth(), matrix.matrixHeight(), _form.Gif.Length);
            FlipColorStoreRG_GR(_form.Gif, _initialBuffer);

            // Set Generator args
            SetGlobalArgs(matrix.matrixWidth(),
                          matrix.matrixHeight(),
                          _form.Gif.Length - 1,
                          _initialBuffer,
                          CreateImageRGB_NT(matrix.matrixWidth(), matrix.matrixHeight(), _form.Gif.Length)
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
        private static void CreateDivider(Control controlPanel)
        {
            var divider = new Label
            {
                BorderStyle = BorderStyle.Fixed3D, 
                AutoSize = false, 
                Height = 2, 
                Width = 500
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
                        var u = x / (float)width;

                        for (var y = 0; y < height; y++)
                        {
                            var v = y / (float)height;

                            _generator.ColorFragment(x, y, u, v, frame, out var r, out var g, out var b);

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
