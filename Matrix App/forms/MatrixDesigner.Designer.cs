using System.IO.Ports;
using System.Drawing;

namespace Matrix_App
{
    partial class MatrixDesignerMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            IMAGE_DRAWER.Stop();
            MatrixGifGenerator.Close();
            commandQueue.Close();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatrixDesignerMain));
            this.Ports = new System.Windows.Forms.ComboBox();
            this.Modus = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.Save = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.FramesLabel = new System.Windows.Forms.Label();
            this.configButton = new System.Windows.Forms.Button();
            this.DelayLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FrameCount = new System.Windows.Forms.NumericUpDown();
            this.Delay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.matrixHeight = new System.Windows.Forms.NumericUpDown();
            this.matrixWidth = new System.Windows.Forms.NumericUpDown();
            this.Zeichnen = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.BrushSizeSlider = new System.Windows.Forms.TrackBar();
            this.showGridCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ZeichnenTextBoxRed = new System.Windows.Forms.TextBox();
            this.ZeichnenTrackBarRed = new System.Windows.Forms.TrackBar();
            this.ZeichnenTrackBarGreen = new System.Windows.Forms.TrackBar();
            this.ZeichnenTrackBarBlue = new System.Windows.Forms.TrackBar();
            this.ZeichnenTextBoxBlue = new System.Windows.Forms.TextBox();
            this.ZeichnenTextBoxGreen = new System.Windows.Forms.TextBox();
            this.Clear = new System.Windows.Forms.Button();
            this.fill = new System.Windows.Forms.Button();
            this.ZeichnenFarbRad = new ColorWheel();
            this.pregeneratedMods = new System.Windows.Forms.TabPage();
            this.pregeneratedModsBase = new System.Windows.Forms.FlowLayoutPanel();
            this.ToolBar = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Timeline = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Play = new System.Windows.Forms.Button();
            this.Apply = new System.Windows.Forms.Button();
            this.DragDropButton = new System.Windows.Forms.Button();
            matrixView = new Matrix_App.Matrix();
            this.panel3 = new System.Windows.Forms.Panel();
            this.Modus.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Delay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixWidth)).BeginInit();
            this.Zeichnen.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushSizeSlider)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarBlue)).BeginInit();
            this.pregeneratedMods.SuspendLayout();
            this.ToolBar.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Timeline)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // Ports
            // 
            this.Ports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Ports.Location = new System.Drawing.Point(62, 130);
            this.Ports.Name = "Ports";
            this.Ports.Size = new System.Drawing.Size(163, 23);
            this.Ports.TabIndex = 0;
            this.Ports.SelectedIndexChanged += new System.EventHandler(this.Ports_SelectedIndexChanged);
            // 
            // Modus
            // 
            this.Modus.Controls.Add(this.tabPage1);
            this.Modus.Controls.Add(this.Zeichnen);
            this.Modus.Controls.Add(this.pregeneratedMods);
            this.Modus.Dock = System.Windows.Forms.DockStyle.Left;
            this.Modus.HotTrack = true;
            this.Modus.ItemSize = new System.Drawing.Size(75, 0);
            this.Modus.Location = new System.Drawing.Point(0, 0);
            this.Modus.Margin = new System.Windows.Forms.Padding(0);
            this.Modus.Name = "Modus";
            this.Modus.Padding = new System.Drawing.Point(0, 0);
            this.Modus.SelectedIndex = 0;
            this.Modus.Size = new System.Drawing.Size(240, 663);
            this.Modus.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.Modus.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.Save);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.FramesLabel);
            this.tabPage1.Controls.Add(this.configButton);
            this.tabPage1.Controls.Add(this.DelayLabel);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.FrameCount);
            this.tabPage1.Controls.Add(this.Ports);
            this.tabPage1.Controls.Add(this.Delay);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.matrixHeight);
            this.tabPage1.Controls.Add(this.matrixWidth);
            this.tabPage1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(232, 635);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel4);
            this.groupBox3.Location = new System.Drawing.Point(3, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 89);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Appearance";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button2);
            this.panel4.Controls.Add(this.button1);
            this.panel4.Location = new System.Drawing.Point(4, 22);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(218, 54);
            this.panel4.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(216, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Light Mode (Default)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ApplyLightModeButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(216, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Dark Mode (Experimental)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ApplyDarkModeButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 208);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Save GIF";
            // 
            // Save
            // 
            this.Save.Image = ((System.Drawing.Image)(resources.GetObject("Save.Image")));
            this.Save.Location = new System.Drawing.Point(62, 196);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(163, 38);
            this.Save.TabIndex = 13;
            this.Save.Text = "Save";
            this.Save.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Config";
            // 
            // FramesLabel
            // 
            this.FramesLabel.AutoSize = true;
            this.FramesLabel.Location = new System.Drawing.Point(5, 71);
            this.FramesLabel.Name = "FramesLabel";
            this.FramesLabel.Size = new System.Drawing.Size(45, 15);
            this.FramesLabel.TabIndex = 12;
            this.FramesLabel.Text = "Frames";
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(62, 159);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(163, 23);
            this.configButton.TabIndex = 6;
            this.configButton.Text = "Load";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // DelayLabel
            // 
            this.DelayLabel.AutoSize = true;
            this.DelayLabel.Location = new System.Drawing.Point(5, 100);
            this.DelayLabel.Name = "DelayLabel";
            this.DelayLabel.Size = new System.Drawing.Size(36, 15);
            this.DelayLabel.TabIndex = 11;
            this.DelayLabel.Text = "Delay";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Height";
            // 
            // FrameCount
            // 
            this.FrameCount.Location = new System.Drawing.Point(62, 72);
            this.FrameCount.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.FrameCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameCount.Name = "FrameCount";
            this.FrameCount.Size = new System.Drawing.Size(163, 23);
            this.FrameCount.TabIndex = 10;
            this.FrameCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameCount.ValueChanged += new System.EventHandler(this.FrameCount_ValueChanged);
            // 
            // Delay
            // 
            this.Delay.Location = new System.Drawing.Point(62, 101);
            this.Delay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.Delay.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.Delay.Name = "Delay";
            this.Delay.Size = new System.Drawing.Size(163, 23);
            this.Delay.TabIndex = 9;
            this.Delay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.Delay.ValueChanged += new System.EventHandler(this.Delay_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Width";
            // 
            // matrixHeight
            // 
            this.matrixHeight.Location = new System.Drawing.Point(62, 43);
            this.matrixHeight.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.matrixHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.matrixHeight.Name = "matrixHeight";
            this.matrixHeight.Size = new System.Drawing.Size(163, 23);
            this.matrixHeight.TabIndex = 1;
            this.matrixHeight.Value = new decimal(new int[] {
                Defaults.MatrixStartHeight,
            0,
            0,
            0});
            this.matrixHeight.ValueChanged += new System.EventHandler(this.Height_ValueChanged);
            // 
            // matrixWidth
            // 
            this.matrixWidth.Location = new System.Drawing.Point(62, 14);
            this.matrixWidth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.matrixWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.matrixWidth.Name = "matrixWidth";
            this.matrixWidth.Size = new System.Drawing.Size(163, 23);
            this.matrixWidth.TabIndex = 0;
            this.matrixWidth.Value = new decimal(new int[] {
                Defaults.MatrixStartWidth,
            0,
            0,
            0});
            this.matrixWidth.ValueChanged += new System.EventHandler(this.Width_ValueChanged);
            // 
            // Zeichnen
            // 
            this.Zeichnen.BackColor = System.Drawing.Color.Transparent;
            this.Zeichnen.Controls.Add(this.groupBox2);
            this.Zeichnen.Controls.Add(this.groupBox1);
            this.Zeichnen.Controls.Add(this.Clear);
            this.Zeichnen.Controls.Add(this.fill);
            this.Zeichnen.Controls.Add(this.ZeichnenFarbRad);
            this.Zeichnen.Location = new System.Drawing.Point(4, 24);
            this.Zeichnen.Margin = new System.Windows.Forms.Padding(0);
            this.Zeichnen.Name = "Zeichnen";
            this.Zeichnen.Size = new System.Drawing.Size(232, 635);
            this.Zeichnen.TabIndex = 1;
            this.Zeichnen.Text = "Edit/Paint";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.BrushSizeSlider);
            this.groupBox2.Controls.Add(this.showGridCheckbox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(3, 481);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 113);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Brush Settings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "Show grid";
            // 
            // trackBar1
            // 
            this.BrushSizeSlider.Location = new System.Drawing.Point(63, 27);
            this.BrushSizeSlider.Maximum = 64;
            this.BrushSizeSlider.Minimum = 1;
            this.BrushSizeSlider.Name = "BrushSizeSlider";
            this.BrushSizeSlider.Size = new System.Drawing.Size(159, 45);
            this.BrushSizeSlider.TabIndex = 5;
            this.BrushSizeSlider.TickFrequency = 8;
            this.BrushSizeSlider.Value = 1;
            this.BrushSizeSlider.Scroll += new System.EventHandler(this.BrushSizeSlider_Scroll);
            // 
            // showGridCheckbox
            // 
            this.showGridCheckbox.AutoSize = true;
            this.showGridCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.showGridCheckbox.Checked = true;
            this.showGridCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGridCheckbox.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showGridCheckbox.Location = new System.Drawing.Point(72, 76);
            this.showGridCheckbox.Name = "showGridCheckbox";
            this.showGridCheckbox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.showGridCheckbox.Size = new System.Drawing.Size(15, 14);
            this.showGridCheckbox.TabIndex = 4;
            this.showGridCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.showGridCheckbox.UseVisualStyleBackColor = false;
            this.showGridCheckbox.CheckedChanged += new System.EventHandler(this.showGridCheckbox_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Brush size";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ZeichnenTextBoxRed);
            this.groupBox1.Controls.Add(this.ZeichnenTrackBarRed);
            this.groupBox1.Controls.Add(this.ZeichnenTrackBarGreen);
            this.groupBox1.Controls.Add(this.ZeichnenTrackBarBlue);
            this.groupBox1.Controls.Add(this.ZeichnenTextBoxBlue);
            this.groupBox1.Controls.Add(this.ZeichnenTextBoxGreen);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.groupBox1.Location = new System.Drawing.Point(4, 225);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(223, 152);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RGB";
            // 
            // ZeichnenTextBoxRed
            // 
            this.ZeichnenTextBoxRed.Location = new System.Drawing.Point(6, 22);
            this.ZeichnenTextBoxRed.Name = "ZeichnenTextBoxRed";
            this.ZeichnenTextBoxRed.Size = new System.Drawing.Size(44, 23);
            this.ZeichnenTextBoxRed.TabIndex = 4;
            this.ZeichnenTextBoxRed.Text = "0";
            this.ZeichnenTextBoxRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ZeichnenTextBoxRed.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DrawTextBoxRed_KeyUp);
            // 
            // ZeichnenTrackBarRed
            // 
            this.ZeichnenTrackBarRed.BackColor = System.Drawing.Color.White;
            this.ZeichnenTrackBarRed.Location = new System.Drawing.Point(53, 22);
            this.ZeichnenTrackBarRed.Maximum = 255;
            this.ZeichnenTrackBarRed.Name = "ZeichnenTrackBarRed";
            this.ZeichnenTrackBarRed.Size = new System.Drawing.Size(169, 45);
            this.ZeichnenTrackBarRed.TabIndex = 1;
            this.ZeichnenTrackBarRed.TickFrequency = 15;
            this.ZeichnenTrackBarRed.Scroll += new System.EventHandler(this.ZeichnenTrackBarRed_Scroll);
            // 
            // ZeichnenTrackBarGreen
            // 
            this.ZeichnenTrackBarGreen.BackColor = System.Drawing.Color.White;
            this.ZeichnenTrackBarGreen.Location = new System.Drawing.Point(53, 63);
            this.ZeichnenTrackBarGreen.Maximum = 255;
            this.ZeichnenTrackBarGreen.Name = "ZeichnenTrackBarGreen";
            this.ZeichnenTrackBarGreen.Size = new System.Drawing.Size(169, 45);
            this.ZeichnenTrackBarGreen.TabIndex = 3;
            this.ZeichnenTrackBarGreen.TickFrequency = 15;
            this.ZeichnenTrackBarGreen.Scroll += new System.EventHandler(this.ZeichnenTrackBarGreen_Scroll);
            // 
            // ZeichnenTrackBarBlue
            // 
            this.ZeichnenTrackBarBlue.BackColor = System.Drawing.Color.White;
            this.ZeichnenTrackBarBlue.Location = new System.Drawing.Point(54, 104);
            this.ZeichnenTrackBarBlue.Maximum = 255;
            this.ZeichnenTrackBarBlue.Name = "ZeichnenTrackBarBlue";
            this.ZeichnenTrackBarBlue.Size = new System.Drawing.Size(168, 45);
            this.ZeichnenTrackBarBlue.TabIndex = 2;
            this.ZeichnenTrackBarBlue.TickFrequency = 15;
            this.ZeichnenTrackBarBlue.Scroll += new System.EventHandler(this.ZeichnenTrackBarBlue_Scroll);
            // 
            // ZeichnenTextBoxBlue
            // 
            this.ZeichnenTextBoxBlue.Location = new System.Drawing.Point(6, 104);
            this.ZeichnenTextBoxBlue.Name = "ZeichnenTextBoxBlue";
            this.ZeichnenTextBoxBlue.Size = new System.Drawing.Size(44, 23);
            this.ZeichnenTextBoxBlue.TabIndex = 6;
            this.ZeichnenTextBoxBlue.Text = "0";
            this.ZeichnenTextBoxBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ZeichnenTextBoxBlue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DrawTextBoxBlue_KeyUp);
            // 
            // ZeichnenTextBoxGreen
            // 
            this.ZeichnenTextBoxGreen.Location = new System.Drawing.Point(6, 63);
            this.ZeichnenTextBoxGreen.Name = "ZeichnenTextBoxGreen";
            this.ZeichnenTextBoxGreen.Size = new System.Drawing.Size(44, 23);
            this.ZeichnenTextBoxGreen.TabIndex = 5;
            this.ZeichnenTextBoxGreen.Text = "0";
            this.ZeichnenTextBoxGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ZeichnenTextBoxGreen.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DrawTextBoxGreen_KeyUp);
            // 
            // Clear
            // 
            this.Clear.Image = ((System.Drawing.Image)(resources.GetObject("Clear.Image")));
            this.Clear.Location = new System.Drawing.Point(4, 428);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(222, 36);
            this.Clear.TabIndex = 8;
            this.Clear.Text = "Clear";
            this.Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Clear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.DrawClear_Click);
            // 
            // fill
            // 
            this.fill.Image = ((System.Drawing.Image)(resources.GetObject("fill.Image")));
            this.fill.Location = new System.Drawing.Point(3, 383);
            this.fill.Name = "fill";
            this.fill.Size = new System.Drawing.Size(223, 39);
            this.fill.TabIndex = 7;
            this.fill.Text = "Fill";
            this.fill.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.fill.UseVisualStyleBackColor = true;
            this.fill.Click += new System.EventHandler(this.DrawFill_Click);
            // 
            // ZeichnenFarbRad
            // 
            this.ZeichnenFarbRad.BackColor = System.Drawing.SystemColors.Control;
            this.ZeichnenFarbRad.Location = new System.Drawing.Point(8, 12);
            this.ZeichnenFarbRad.Name = "ZeichnenFarbRad";
            this.ZeichnenFarbRad.Size = new System.Drawing.Size(209, 214);
            this.ZeichnenFarbRad.TabIndex = 0;
            // 
            // pregeneratedMods
            // 
            this.pregeneratedMods.Controls.Add(this.pregeneratedModsBase);
            this.pregeneratedMods.Location = new System.Drawing.Point(4, 24);
            this.pregeneratedMods.Margin = new System.Windows.Forms.Padding(0);
            this.pregeneratedMods.Name = "pregeneratedMods";
            this.pregeneratedMods.Size = new System.Drawing.Size(232, 635);
            this.pregeneratedMods.TabIndex = 2;
            this.pregeneratedMods.Text = "Filter";
            this.pregeneratedMods.UseVisualStyleBackColor = true;
            // 
            // pregeneratedModsBase
            // 
            this.pregeneratedModsBase.Dock = System.Windows.Forms.DockStyle.Top;
            this.pregeneratedModsBase.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pregeneratedModsBase.Location = new System.Drawing.Point(0, 0);
            this.pregeneratedModsBase.Name = "pregeneratedModsBase";
            this.pregeneratedModsBase.Size = new System.Drawing.Size(232, 635);
            this.pregeneratedModsBase.TabIndex = 0;
            // 
            // ToolBar
            // 
            this.ToolBar.Controls.Add(this.panel2);
            this.ToolBar.Controls.Add(this.panel1);
            this.ToolBar.Controls.Add(this.DragDropButton);
            this.ToolBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ToolBar.Location = new System.Drawing.Point(240, 589);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Padding = new System.Windows.Forms.Padding(6);
            this.ToolBar.Size = new System.Drawing.Size(659, 74);
            this.ToolBar.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Timeline);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(161, 6);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
            this.panel2.Size = new System.Drawing.Size(337, 62);
            this.panel2.TabIndex = 5;
            // 
            // Timeline
            // 
            this.Timeline.Cursor = System.Windows.Forms.Cursors.Default;
            this.Timeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Timeline.Enabled = false;
            this.Timeline.LargeChange = 1;
            this.Timeline.Location = new System.Drawing.Point(0, 18);
            this.Timeline.Maximum = 1;
            this.Timeline.Name = "Timeline";
            this.Timeline.Size = new System.Drawing.Size(337, 44);
            this.Timeline.TabIndex = 5;
            this.Timeline.ValueChanged += new System.EventHandler(this.Timeline_ValueChanged);
            this.Timeline.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Timeline_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Play);
            this.panel1.Controls.Add(this.Apply);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(6, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 62);
            this.panel1.TabIndex = 4;
            // 
            // Play
            // 
            this.Play.Dock = System.Windows.Forms.DockStyle.Right;
            this.Play.Image = ((System.Drawing.Image)(resources.GetObject("Play.Image")));
            this.Play.Location = new System.Drawing.Point(85, 0);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(70, 62);
            this.Play.TabIndex = 3;
            this.Play.Text = "Play";
            this.Play.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.Play.UseVisualStyleBackColor = true;
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // Apply
            // 
            this.Apply.Dock = System.Windows.Forms.DockStyle.Left;
            this.Apply.Image = ((System.Drawing.Image)(resources.GetObject("Apply.Image")));
            this.Apply.Location = new System.Drawing.Point(0, 0);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(85, 62);
            this.Apply.TabIndex = 2;
            this.Apply.Text = "Apply Frame";
            this.Apply.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // DragDropButton
            // 
            this.DragDropButton.AllowDrop = true;
            this.DragDropButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DragDropButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.DragDropButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.DragDropButton.ForeColor = System.Drawing.Color.Black;
            this.DragDropButton.Image = ((System.Drawing.Image)(resources.GetObject("DragDropButton.Image")));
            this.DragDropButton.Location = new System.Drawing.Point(498, 6);
            this.DragDropButton.Name = "DragDropButton";
            this.DragDropButton.Size = new System.Drawing.Size(155, 62);
            this.DragDropButton.TabIndex = 0;
            this.DragDropButton.Text = "Image/Gif\r\ndrag drop";
            this.DragDropButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DragDropButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.DragDropButton.UseVisualStyleBackColor = true;
            this.DragDropButton.Click += new System.EventHandler(this.DragDrop_Click);
            this.DragDropButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDrop_DragDrop);
            this.DragDropButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragDrop_DragEnter);
            // 
            // matrixView
            // 
            matrixView.Dock = System.Windows.Forms.DockStyle.Fill;
            matrixView.Location = new System.Drawing.Point(16, 16);
            matrixView.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            matrixView.Name = "matrixView";
            matrixView.Size = new System.Drawing.Size(627, 557);
            matrixView.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(matrixView);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(240, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(16);
            this.panel3.Size = new System.Drawing.Size(659, 589);
            this.panel3.TabIndex = 0;
            // 
            // MatrixDesignerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 663);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.ToolBar);
            this.Controls.Add(this.Modus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(915, 702);
            this.Name = "MatrixDesignerMain";
            this.Text = "Matrix Designer";
            this.Modus.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Delay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixWidth)).EndInit();
            this.Zeichnen.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrushSizeSlider)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZeichnenTrackBarBlue)).EndInit();
            this.pregeneratedMods.ResumeLayout(false);
            this.ToolBar.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Timeline)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.ComboBox Ports;
        private System.Windows.Forms.TabControl Modus;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage Zeichnen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel ToolBar;
        private System.Windows.Forms.NumericUpDown matrixHeight;
        private System.Windows.Forms.NumericUpDown matrixWidth;
        private ColorWheel ZeichnenFarbRad;
        private System.Windows.Forms.TrackBar ZeichnenTrackBarGreen;
        private System.Windows.Forms.TrackBar ZeichnenTrackBarBlue;
        private System.Windows.Forms.TrackBar ZeichnenTrackBarRed;
        private System.Windows.Forms.TextBox ZeichnenTextBoxGreen;
        private System.Windows.Forms.TextBox ZeichnenTextBoxRed;
        private System.Windows.Forms.TextBox ZeichnenTextBoxBlue;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.Button fill;
        private System.Windows.Forms.Button DragDropButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.TrackBar Timeline;
        private System.Windows.Forms.NumericUpDown FrameCount;
        private System.Windows.Forms.NumericUpDown Delay;
        private System.Windows.Forms.Label FramesLabel;
        private System.Windows.Forms.Label DelayLabel;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button configButton;
        private static Matrix matrixView;
        private System.Windows.Forms.TabPage pregeneratedMods;
        private System.Windows.Forms.FlowLayoutPanel pregeneratedModsBase;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.CheckBox showGridCheckbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar BrushSizeSlider;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

