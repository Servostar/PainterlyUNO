using System.ComponentModel;

namespace Matrix_App.forms
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateRealtimeCheckbox = new System.Windows.Forms.CheckBox();
            this.WritePortCombobox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.BluetoothFlagLabel = new System.Windows.Forms.Label();
            this.USBFlagLabel = new System.Windows.Forms.Label();
            this.UploadFlagLabel = new System.Windows.Forms.Label();
            this.Controller = new System.Windows.Forms.Label();
            this.ControllerNameLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "COM Settings";
            // 
            // UpdateRealtimeCheckbox
            // 
            this.UpdateRealtimeCheckbox.Location = new System.Drawing.Point(18, 40);
            this.UpdateRealtimeCheckbox.Name = "UpdateRealtimeCheckbox";
            this.UpdateRealtimeCheckbox.Size = new System.Drawing.Size(159, 26);
            this.UpdateRealtimeCheckbox.TabIndex = 2;
            this.UpdateRealtimeCheckbox.Text = "Realtime update";
            this.UpdateRealtimeCheckbox.UseVisualStyleBackColor = true;
            this.UpdateRealtimeCheckbox.CheckedChanged += new System.EventHandler(this.UpdateRealtimeCheckbox_CheckedChanged);
            // 
            // WritePortCombobox
            // 
            this.WritePortCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WritePortCombobox.FormattingEnabled = true;
            this.WritePortCombobox.Location = new System.Drawing.Point(6, 24);
            this.WritePortCombobox.Name = "WritePortCombobox";
            this.WritePortCombobox.Size = new System.Drawing.Size(430, 21);
            this.WritePortCombobox.TabIndex = 5;
            this.WritePortCombobox.SelectedIndexChanged += new System.EventHandler(this.WritePortCombobox_SelectedIndexChanged_1);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(442, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Serial Port";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ControllerNameLabel);
            this.groupBox1.Controls.Add(this.Controller);
            this.groupBox1.Controls.Add(this.UploadFlagLabel);
            this.groupBox1.Controls.Add(this.USBFlagLabel);
            this.groupBox1.Controls.Add(this.BluetoothFlagLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.WritePortCombobox);
            this.groupBox1.Location = new System.Drawing.Point(12, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(536, 211);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Port";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "USB";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Bluetooth";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 25);
            this.label2.TabIndex = 8;
            this.label2.Text = "Features";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 24);
            this.button1.TabIndex = 7;
            this.button1.Text = "Auto detect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Upload";
            // 
            // BluetoothFlagLabel
            // 
            this.BluetoothFlagLabel.Location = new System.Drawing.Point(89, 85);
            this.BluetoothFlagLabel.Name = "BluetoothFlagLabel";
            this.BluetoothFlagLabel.Size = new System.Drawing.Size(129, 17);
            this.BluetoothFlagLabel.TabIndex = 12;
            this.BluetoothFlagLabel.Text = "-";
            // 
            // USBFlagLabel
            // 
            this.USBFlagLabel.Location = new System.Drawing.Point(89, 102);
            this.USBFlagLabel.Name = "USBFlagLabel";
            this.USBFlagLabel.Size = new System.Drawing.Size(129, 17);
            this.USBFlagLabel.TabIndex = 13;
            this.USBFlagLabel.Text = "-";
            // 
            // UploadFlagLabel
            // 
            this.UploadFlagLabel.Location = new System.Drawing.Point(89, 119);
            this.UploadFlagLabel.Name = "UploadFlagLabel";
            this.UploadFlagLabel.Size = new System.Drawing.Size(129, 17);
            this.UploadFlagLabel.TabIndex = 14;
            this.UploadFlagLabel.Text = "-";
            // 
            // Controller
            // 
            this.Controller.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Controller.Location = new System.Drawing.Point(6, 150);
            this.Controller.Name = "Controller";
            this.Controller.Size = new System.Drawing.Size(129, 17);
            this.Controller.TabIndex = 15;
            this.Controller.Text = "Controller:";
            // 
            // ControllerNameLabel
            // 
            this.ControllerNameLabel.Location = new System.Drawing.Point(89, 150);
            this.ControllerNameLabel.Name = "ControllerNameLabel";
            this.ControllerNameLabel.Size = new System.Drawing.Size(129, 17);
            this.ControllerNameLabel.TabIndex = 16;
            this.ControllerNameLabel.Text = "-";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 295);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.UpdateRealtimeCheckbox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label ControllerNameLabel;

        private System.Windows.Forms.Label Controller;

        private System.Windows.Forms.Label label7;

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label BluetoothFlagLabel;
        private System.Windows.Forms.Label USBFlagLabel;
        private System.Windows.Forms.Label UploadFlagLabel;

        private System.Windows.Forms.Label label5;

        private System.Windows.Forms.Label label4;

        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.CheckBox UpdateRealtimeCheckbox;

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;

        private System.Windows.Forms.ComboBox WritePortCombobox;

        private System.Windows.Forms.Label label1;

        #endregion
    }
}