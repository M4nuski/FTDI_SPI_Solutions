namespace USB_Interface_v2_Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.binarySelector5 = new USB_Interface_v2_Test.BinarySelector();
            this.binarySelector4 = new USB_Interface_v2_Test.BinarySelector();
            this.binarySelector3 = new USB_Interface_v2_Test.BinarySelector();
            this.binarySelector2 = new USB_Interface_v2_Test.BinarySelector();
            this.binarySelector1 = new USB_Interface_v2_Test.BinarySelector();
            this.outDataCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Reload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(93, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(469, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 273);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(549, 252);
            this.textBox1.TabIndex = 12;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(568, 39);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(199, 483);
            this.textBox2.TabIndex = 13;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(12, 44);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(67, 17);
            this.radioButton1.TabIndex = 14;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "BitSelect";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(12, 68);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(117, 17);
            this.radioButton2.TabIndex = 15;
            this.radioButton2.Text = "Even/Odd BitSwap";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(12, 91);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(100, 17);
            this.radioButton3.TabIndex = 16;
            this.radioButton3.Text = "On/Off BitSwap";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // binarySelector5
            // 
            this.binarySelector5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.binarySelector5.Description = "InputByte1";
            this.binarySelector5.Enabled = false;
            this.binarySelector5.Location = new System.Drawing.Point(12, 195);
            this.binarySelector5.Name = "binarySelector5";
            this.binarySelector5.Size = new System.Drawing.Size(272, 72);
            this.binarySelector5.TabIndex = 11;
            // 
            // binarySelector4
            // 
            this.binarySelector4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.binarySelector4.Description = "InputByte0";
            this.binarySelector4.Enabled = false;
            this.binarySelector4.Location = new System.Drawing.Point(12, 117);
            this.binarySelector4.Name = "binarySelector4";
            this.binarySelector4.Size = new System.Drawing.Size(272, 72);
            this.binarySelector4.TabIndex = 10;
            // 
            // binarySelector3
            // 
            this.binarySelector3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.binarySelector3.Description = "OutputByte2";
            this.binarySelector3.Location = new System.Drawing.Point(289, 195);
            this.binarySelector3.Name = "binarySelector3";
            this.binarySelector3.Size = new System.Drawing.Size(272, 72);
            this.binarySelector3.TabIndex = 9;
            // 
            // binarySelector2
            // 
            this.binarySelector2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.binarySelector2.Description = "OutputByte1";
            this.binarySelector2.Location = new System.Drawing.Point(290, 117);
            this.binarySelector2.Name = "binarySelector2";
            this.binarySelector2.Size = new System.Drawing.Size(272, 72);
            this.binarySelector2.TabIndex = 8;
            // 
            // binarySelector1
            // 
            this.binarySelector1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.binarySelector1.Description = "OutputByte0";
            this.binarySelector1.Location = new System.Drawing.Point(289, 39);
            this.binarySelector1.Name = "binarySelector1";
            this.binarySelector1.Size = new System.Drawing.Size(272, 72);
            this.binarySelector1.TabIndex = 7;
            // 
            // outDataCheckbox
            // 
            this.outDataCheckbox.AutoSize = true;
            this.outDataCheckbox.Location = new System.Drawing.Point(600, 16);
            this.outDataCheckbox.Name = "outDataCheckbox";
            this.outDataCheckbox.Size = new System.Drawing.Size(145, 17);
            this.outDataCheckbox.TabIndex = 17;
            this.outDataCheckbox.Text = "Show Output Buffer Data";
            this.outDataCheckbox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 537);
            this.Controls.Add(this.outDataCheckbox);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.binarySelector5);
            this.Controls.Add(this.binarySelector4);
            this.Controls.Add(this.binarySelector3);
            this.Controls.Add(this.binarySelector2);
            this.Controls.Add(this.binarySelector1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "USB Interface v2.x Test App";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private BinarySelector binarySelector1;
        private BinarySelector binarySelector2;
        private BinarySelector binarySelector3;
        private BinarySelector binarySelector4;
        private BinarySelector binarySelector5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.CheckBox outDataCheckbox;
    }
}

