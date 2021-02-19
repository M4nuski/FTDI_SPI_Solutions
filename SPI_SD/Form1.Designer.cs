namespace SPI_SD
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.textBoxAddr0 = new System.Windows.Forms.TextBox();
            this.textBoxAddr1 = new System.Windows.Forms.TextBox();
            this.textBoxAddr2 = new System.Windows.Forms.TextBox();
            this.textBoxAddr3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.textBoxReadBlockDec = new System.Windows.Forms.TextBox();
            this.textBoxReadBlockHex = new System.Windows.Forms.TextBox();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Devices";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 167);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(776, 762);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Init/Reset";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 88);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(146, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "55+41 HCS and init";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 126);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(146, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "9 Get CSD";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(164, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(146, 23);
            this.button5.TabIndex = 5;
            this.button5.Text = "Read Address (HEX)";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button_ReadBlock_Click);
            // 
            // textBoxAddr0
            // 
            this.textBoxAddr0.Location = new System.Drawing.Point(409, 14);
            this.textBoxAddr0.Name = "textBoxAddr0";
            this.textBoxAddr0.Size = new System.Drawing.Size(25, 20);
            this.textBoxAddr0.TabIndex = 6;
            this.textBoxAddr0.Text = "00";
            // 
            // textBoxAddr1
            // 
            this.textBoxAddr1.Location = new System.Drawing.Point(378, 14);
            this.textBoxAddr1.Name = "textBoxAddr1";
            this.textBoxAddr1.Size = new System.Drawing.Size(25, 20);
            this.textBoxAddr1.TabIndex = 7;
            this.textBoxAddr1.Text = "00";
            // 
            // textBoxAddr2
            // 
            this.textBoxAddr2.Location = new System.Drawing.Point(347, 14);
            this.textBoxAddr2.Name = "textBoxAddr2";
            this.textBoxAddr2.Size = new System.Drawing.Size(25, 20);
            this.textBoxAddr2.TabIndex = 8;
            this.textBoxAddr2.Text = "00";
            // 
            // textBoxAddr3
            // 
            this.textBoxAddr3.Location = new System.Drawing.Point(316, 14);
            this.textBoxAddr3.Name = "textBoxAddr3";
            this.textBoxAddr3.Size = new System.Drawing.Size(25, 20);
            this.textBoxAddr3.TabIndex = 9;
            this.textBoxAddr3.Text = "00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(316, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "B3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(347, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "B2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(378, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "B1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(409, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "B0";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(164, 50);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(146, 23);
            this.button6.TabIndex = 14;
            this.button6.Text = "Read next sector/block";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(164, 88);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(146, 23);
            this.button7.TabIndex = 15;
            this.button7.Text = "Read Block (DEC)";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.buttonReadBlockDecimal_Click);
            // 
            // textBoxReadBlockDec
            // 
            this.textBoxReadBlockDec.Location = new System.Drawing.Point(316, 90);
            this.textBoxReadBlockDec.Name = "textBoxReadBlockDec";
            this.textBoxReadBlockDec.Size = new System.Drawing.Size(100, 20);
            this.textBoxReadBlockDec.TabIndex = 16;
            this.textBoxReadBlockDec.Text = "0";
            // 
            // textBoxReadBlockHex
            // 
            this.textBoxReadBlockHex.Location = new System.Drawing.Point(316, 128);
            this.textBoxReadBlockHex.Name = "textBoxReadBlockHex";
            this.textBoxReadBlockHex.Size = new System.Drawing.Size(100, 20);
            this.textBoxReadBlockHex.TabIndex = 18;
            this.textBoxReadBlockHex.Text = "0";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(164, 126);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(146, 23);
            this.button8.TabIndex = 17;
            this.button8.Text = "Read Block (HEX)";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.buttonReadBlockHex_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button12);
            this.groupBox1.Controls.Add(this.button11);
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Location = new System.Drawing.Point(499, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 135);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FAT";
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(6, 105);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(146, 23);
            this.button12.TabIndex = 24;
            this.button12.Text = "Read Directory Sector";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.buttonReadFATDirectory);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(6, 76);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(146, 23);
            this.button11.TabIndex = 23;
            this.button11.Text = "Read FAT32 Boot Sector";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.buttonReadFAT32Sector_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(6, 47);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(146, 23);
            this.button10.TabIndex = 21;
            this.button10.Text = "Read FAT16 Boot Sector";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.buttonReadFATBoot_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(6, 19);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(146, 23);
            this.button9.TabIndex = 20;
            this.button9.Text = "Read MBR/Part.Table Sect.";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.buttonReadMBR_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(316, 61);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(146, 23);
            this.button13.TabIndex = 20;
            this.button13.Text = "9 Get OCR 58";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 941);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxReadBlockHex);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.textBoxReadBlockDec);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAddr3);
            this.Controls.Add(this.textBoxAddr2);
            this.Controls.Add(this.textBoxAddr1);
            this.Controls.Add(this.textBoxAddr0);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "SPI SD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBoxAddr0;
        private System.Windows.Forms.TextBox textBoxAddr1;
        private System.Windows.Forms.TextBox textBoxAddr2;
        private System.Windows.Forms.TextBox textBoxAddr3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textBoxReadBlockDec;
        private System.Windows.Forms.TextBox textBoxReadBlockHex;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
    }
}

