
namespace SPI_LCDv2
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
            this.textBox_log = new System.Windows.Forms.TextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.textBox_input = new System.Windows.Forms.TextBox();
            this.button_sendLetters = new System.Windows.Forms.Button();
            this.button_init = new System.Windows.Forms.Button();
            this.checkBox_RS = new System.Windows.Forms.CheckBox();
            this.checkBox_E = new System.Windows.Forms.CheckBox();
            this.checkBox_DB7 = new System.Windows.Forms.CheckBox();
            this.checkBox_DB6 = new System.Windows.Forms.CheckBox();
            this.checkBox_DB5 = new System.Windows.Forms.CheckBox();
            this.checkBox_DB4 = new System.Windows.Forms.CheckBox();
            this.button_sendHex = new System.Windows.Forms.Button();
            this.button_sendText = new System.Windows.Forms.Button();
            this.comboBox_size = new System.Windows.Forms.ComboBox();
            this.button_clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_log
            // 
            this.textBox_log.Location = new System.Drawing.Point(12, 87);
            this.textBox_log.Multiline = true;
            this.textBox_log.Name = "textBox_log";
            this.textBox_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_log.Size = new System.Drawing.Size(673, 304);
            this.textBox_log.TabIndex = 0;
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(12, 12);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(75, 23);
            this.button_connect.TabIndex = 1;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // textBox_input
            // 
            this.textBox_input.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_input.Location = new System.Drawing.Point(174, 11);
            this.textBox_input.Multiline = true;
            this.textBox_input.Name = "textBox_input";
            this.textBox_input.Size = new System.Drawing.Size(154, 66);
            this.textBox_input.TabIndex = 2;
            this.textBox_input.Text = "01234567890123456789\r\nabcdefghijKLMNOPQRST\r\nLine 3 text of 20 ch\r\nLigne 4 avec bu" +
    "llshit";
            this.textBox_input.WordWrap = false;
            // 
            // button_sendLetters
            // 
            this.button_sendLetters.Location = new System.Drawing.Point(334, 45);
            this.button_sendLetters.Name = "button_sendLetters";
            this.button_sendLetters.Size = new System.Drawing.Size(75, 23);
            this.button_sendLetters.TabIndex = 3;
            this.button_sendLetters.Text = "Fill Letters";
            this.button_sendLetters.UseVisualStyleBackColor = true;
            this.button_sendLetters.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_init
            // 
            this.button_init.Location = new System.Drawing.Point(93, 12);
            this.button_init.Name = "button_init";
            this.button_init.Size = new System.Drawing.Size(75, 23);
            this.button_init.TabIndex = 4;
            this.button_init.Text = "Initialize";
            this.button_init.UseVisualStyleBackColor = true;
            this.button_init.Click += new System.EventHandler(this.button_init_Click);
            // 
            // checkBox_RS
            // 
            this.checkBox_RS.AutoSize = true;
            this.checkBox_RS.Location = new System.Drawing.Point(36, 411);
            this.checkBox_RS.Name = "checkBox_RS";
            this.checkBox_RS.Size = new System.Drawing.Size(41, 17);
            this.checkBox_RS.TabIndex = 5;
            this.checkBox_RS.Text = "RS";
            this.checkBox_RS.UseVisualStyleBackColor = true;
            this.checkBox_RS.CheckedChanged += new System.EventHandler(this.checkBox_RS_CheckedChanged);
            // 
            // checkBox_E
            // 
            this.checkBox_E.AutoSize = true;
            this.checkBox_E.Location = new System.Drawing.Point(36, 434);
            this.checkBox_E.Name = "checkBox_E";
            this.checkBox_E.Size = new System.Drawing.Size(33, 17);
            this.checkBox_E.TabIndex = 6;
            this.checkBox_E.Text = "E";
            this.checkBox_E.UseVisualStyleBackColor = true;
            this.checkBox_E.CheckedChanged += new System.EventHandler(this.checkBox_E_CheckedChanged);
            // 
            // checkBox_DB7
            // 
            this.checkBox_DB7.AutoSize = true;
            this.checkBox_DB7.Location = new System.Drawing.Point(93, 411);
            this.checkBox_DB7.Name = "checkBox_DB7";
            this.checkBox_DB7.Size = new System.Drawing.Size(47, 17);
            this.checkBox_DB7.TabIndex = 7;
            this.checkBox_DB7.Text = "DB7";
            this.checkBox_DB7.UseVisualStyleBackColor = true;
            this.checkBox_DB7.CheckedChanged += new System.EventHandler(this.checkBox_RS_CheckedChanged);
            // 
            // checkBox_DB6
            // 
            this.checkBox_DB6.AutoSize = true;
            this.checkBox_DB6.Location = new System.Drawing.Point(140, 411);
            this.checkBox_DB6.Name = "checkBox_DB6";
            this.checkBox_DB6.Size = new System.Drawing.Size(47, 17);
            this.checkBox_DB6.TabIndex = 8;
            this.checkBox_DB6.Text = "DB6";
            this.checkBox_DB6.UseVisualStyleBackColor = true;
            this.checkBox_DB6.CheckedChanged += new System.EventHandler(this.checkBox_RS_CheckedChanged);
            // 
            // checkBox_DB5
            // 
            this.checkBox_DB5.AutoSize = true;
            this.checkBox_DB5.Location = new System.Drawing.Point(187, 411);
            this.checkBox_DB5.Name = "checkBox_DB5";
            this.checkBox_DB5.Size = new System.Drawing.Size(47, 17);
            this.checkBox_DB5.TabIndex = 9;
            this.checkBox_DB5.Text = "DB5";
            this.checkBox_DB5.UseVisualStyleBackColor = true;
            this.checkBox_DB5.CheckedChanged += new System.EventHandler(this.checkBox_RS_CheckedChanged);
            // 
            // checkBox_DB4
            // 
            this.checkBox_DB4.AutoSize = true;
            this.checkBox_DB4.Location = new System.Drawing.Point(233, 411);
            this.checkBox_DB4.Name = "checkBox_DB4";
            this.checkBox_DB4.Size = new System.Drawing.Size(47, 17);
            this.checkBox_DB4.TabIndex = 10;
            this.checkBox_DB4.Text = "DB4";
            this.checkBox_DB4.UseVisualStyleBackColor = true;
            this.checkBox_DB4.CheckedChanged += new System.EventHandler(this.checkBox_RS_CheckedChanged);
            // 
            // button_sendHex
            // 
            this.button_sendHex.Location = new System.Drawing.Point(415, 45);
            this.button_sendHex.Name = "button_sendHex";
            this.button_sendHex.Size = new System.Drawing.Size(75, 23);
            this.button_sendHex.TabIndex = 11;
            this.button_sendHex.Text = "Fill Special";
            this.button_sendHex.UseVisualStyleBackColor = true;
            this.button_sendHex.Click += new System.EventHandler(this.button_sendHex_Click);
            // 
            // button_sendText
            // 
            this.button_sendText.Location = new System.Drawing.Point(334, 12);
            this.button_sendText.Name = "button_sendText";
            this.button_sendText.Size = new System.Drawing.Size(75, 23);
            this.button_sendText.TabIndex = 12;
            this.button_sendText.Text = "Send";
            this.button_sendText.UseVisualStyleBackColor = true;
            this.button_sendText.Click += new System.EventHandler(this.button_sendText_Click);
            // 
            // comboBox_size
            // 
            this.comboBox_size.FormattingEnabled = true;
            this.comboBox_size.Items.AddRange(new object[] {
            "16x2",
            "20x2",
            "16x4",
            "20x4"});
            this.comboBox_size.Location = new System.Drawing.Point(415, 12);
            this.comboBox_size.Name = "comboBox_size";
            this.comboBox_size.Size = new System.Drawing.Size(75, 21);
            this.comboBox_size.TabIndex = 13;
            this.comboBox_size.Text = "16x2";
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(93, 45);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 14;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 474);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.comboBox_size);
            this.Controls.Add(this.button_sendText);
            this.Controls.Add(this.button_sendHex);
            this.Controls.Add(this.checkBox_DB4);
            this.Controls.Add(this.checkBox_DB5);
            this.Controls.Add(this.checkBox_DB6);
            this.Controls.Add(this.checkBox_DB7);
            this.Controls.Add(this.checkBox_E);
            this.Controls.Add(this.checkBox_RS);
            this.Controls.Add(this.button_init);
            this.Controls.Add(this.button_sendLetters);
            this.Controls.Add(this.textBox_input);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.textBox_log);
            this.Name = "Form1";
            this.Text = "LCD interface v2.0 UM245R SPI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_log;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.TextBox textBox_input;
        private System.Windows.Forms.Button button_sendLetters;
        private System.Windows.Forms.Button button_init;
        private System.Windows.Forms.CheckBox checkBox_RS;
        private System.Windows.Forms.CheckBox checkBox_E;
        private System.Windows.Forms.CheckBox checkBox_DB7;
        private System.Windows.Forms.CheckBox checkBox_DB6;
        private System.Windows.Forms.CheckBox checkBox_DB5;
        private System.Windows.Forms.CheckBox checkBox_DB4;
        private System.Windows.Forms.Button button_sendHex;
        private System.Windows.Forms.Button button_sendText;
        private System.Windows.Forms.ComboBox comboBox_size;
        private System.Windows.Forms.Button button_clear;
    }
}

