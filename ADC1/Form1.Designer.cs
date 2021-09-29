
namespace ADC1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_an0data = new System.Windows.Forms.Label();
            this.label_an1data = new System.Windows.Forms.Label();
            this.label_an2data = new System.Windows.Forms.Label();
            this.label_an3data = new System.Windows.Forms.Label();
            this.button_relist = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox_pause = new System.Windows.Forms.CheckBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label_updateKBPS = new System.Windows.Forms.Label();
            this.label_AVGkbps = new System.Windows.Forms.Label();
            this.button_clear = new System.Windows.Forms.Button();
            this.textBoxAN0scale = new System.Windows.Forms.TextBox();
            this.textBoxAN0datum = new System.Windows.Forms.TextBox();
            this.textBoxAN1datum = new System.Windows.Forms.TextBox();
            this.textBoxAN1scale = new System.Windows.Forms.TextBox();
            this.textBoxAN2datum = new System.Windows.Forms.TextBox();
            this.textBoxAN2scale = new System.Windows.Forms.TextBox();
            this.textBoxAN3datum = new System.Windows.Forms.TextBox();
            this.textBoxAN3scale = new System.Windows.Forms.TextBox();
            this.label_an0float = new System.Windows.Forms.Label();
            this.label_an1float = new System.Windows.Forms.Label();
            this.label_an2float = new System.Windows.Forms.Label();
            this.label_an3float = new System.Windows.Forms.Label();
            this.radioButton_raw = new System.Windows.Forms.RadioButton();
            this.radioButton_float = new System.Windows.Forms.RadioButton();
            this.checkBox_an0 = new System.Windows.Forms.CheckBox();
            this.checkBox_an1 = new System.Windows.Forms.CheckBox();
            this.checkBox_an2 = new System.Windows.Forms.CheckBox();
            this.checkBox_an3 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_poolStat = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 138);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(776, 300);
            this.textBox1.TabIndex = 0;
            // 
            // label_an0data
            // 
            this.label_an0data.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an0data.Location = new System.Drawing.Point(12, 9);
            this.label_an0data.Name = "label_an0data";
            this.label_an0data.Size = new System.Drawing.Size(222, 19);
            this.label_an0data.TabIndex = 1;
            this.label_an0data.Text = "1023 0xFFF 000000000000";
            // 
            // label_an1data
            // 
            this.label_an1data.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an1data.Location = new System.Drawing.Point(12, 28);
            this.label_an1data.Name = "label_an1data";
            this.label_an1data.Size = new System.Drawing.Size(222, 19);
            this.label_an1data.TabIndex = 2;
            this.label_an1data.Text = "AN1";
            // 
            // label_an2data
            // 
            this.label_an2data.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an2data.Location = new System.Drawing.Point(12, 47);
            this.label_an2data.Name = "label_an2data";
            this.label_an2data.Size = new System.Drawing.Size(222, 19);
            this.label_an2data.TabIndex = 3;
            this.label_an2data.Text = "AN2";
            // 
            // label_an3data
            // 
            this.label_an3data.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an3data.Location = new System.Drawing.Point(12, 66);
            this.label_an3data.Name = "label_an3data";
            this.label_an3data.Size = new System.Drawing.Size(222, 19);
            this.label_an3data.TabIndex = 4;
            this.label_an3data.Text = "AN3";
            // 
            // button_relist
            // 
            this.button_relist.Location = new System.Drawing.Point(713, 5);
            this.button_relist.Name = "button_relist";
            this.button_relist.Size = new System.Drawing.Size(75, 23);
            this.button_relist.TabIndex = 5;
            this.button_relist.Text = "List";
            this.button_relist.UseVisualStyleBackColor = true;
            this.button_relist.Click += new System.EventHandler(this.button_relist_Click);
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(632, 5);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 6;
            this.button_open.Text = "Open";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(429, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(197, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBox_pause
            // 
            this.checkBox_pause.AutoSize = true;
            this.checkBox_pause.Location = new System.Drawing.Point(714, 19);
            this.checkBox_pause.Name = "checkBox_pause";
            this.checkBox_pause.Size = new System.Drawing.Size(56, 17);
            this.checkBox_pause.TabIndex = 8;
            this.checkBox_pause.Text = "Pause";
            this.checkBox_pause.UseVisualStyleBackColor = true;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label_updateKBPS
            // 
            this.label_updateKBPS.AutoSize = true;
            this.label_updateKBPS.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_updateKBPS.Location = new System.Drawing.Point(429, 30);
            this.label_updateKBPS.Name = "label_updateKBPS";
            this.label_updateKBPS.Size = new System.Drawing.Size(18, 19);
            this.label_updateKBPS.TabIndex = 9;
            this.label_updateKBPS.Text = "0";
            // 
            // label_AVGkbps
            // 
            this.label_AVGkbps.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AVGkbps.Location = new System.Drawing.Point(429, 47);
            this.label_AVGkbps.Name = "label_AVGkbps";
            this.label_AVGkbps.Size = new System.Drawing.Size(368, 19);
            this.label_AVGkbps.TabIndex = 10;
            this.label_AVGkbps.Text = "0";
            // 
            // button_clear
            // 
            this.button_clear.Location = new System.Drawing.Point(633, 15);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 11;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // textBoxAN0scale
            // 
            this.textBoxAN0scale.Location = new System.Drawing.Point(240, 10);
            this.textBoxAN0scale.Name = "textBoxAN0scale";
            this.textBoxAN0scale.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN0scale.TabIndex = 12;
            this.textBoxAN0scale.Text = "0.00503";
            // 
            // textBoxAN0datum
            // 
            this.textBoxAN0datum.Location = new System.Drawing.Point(294, 10);
            this.textBoxAN0datum.Name = "textBoxAN0datum";
            this.textBoxAN0datum.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN0datum.TabIndex = 13;
            this.textBoxAN0datum.Text = "0.000";
            // 
            // textBoxAN1datum
            // 
            this.textBoxAN1datum.Location = new System.Drawing.Point(294, 29);
            this.textBoxAN1datum.Name = "textBoxAN1datum";
            this.textBoxAN1datum.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN1datum.TabIndex = 15;
            this.textBoxAN1datum.Text = "0.000";
            // 
            // textBoxAN1scale
            // 
            this.textBoxAN1scale.Location = new System.Drawing.Point(240, 29);
            this.textBoxAN1scale.Name = "textBoxAN1scale";
            this.textBoxAN1scale.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN1scale.TabIndex = 14;
            this.textBoxAN1scale.Text = "0.00503";
            // 
            // textBoxAN2datum
            // 
            this.textBoxAN2datum.Location = new System.Drawing.Point(294, 48);
            this.textBoxAN2datum.Name = "textBoxAN2datum";
            this.textBoxAN2datum.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN2datum.TabIndex = 17;
            this.textBoxAN2datum.Text = "0.000";
            // 
            // textBoxAN2scale
            // 
            this.textBoxAN2scale.Location = new System.Drawing.Point(240, 48);
            this.textBoxAN2scale.Name = "textBoxAN2scale";
            this.textBoxAN2scale.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN2scale.TabIndex = 16;
            this.textBoxAN2scale.Text = "0.00503";
            // 
            // textBoxAN3datum
            // 
            this.textBoxAN3datum.Location = new System.Drawing.Point(294, 67);
            this.textBoxAN3datum.Name = "textBoxAN3datum";
            this.textBoxAN3datum.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN3datum.TabIndex = 19;
            this.textBoxAN3datum.Text = "0.000";
            // 
            // textBoxAN3scale
            // 
            this.textBoxAN3scale.Location = new System.Drawing.Point(240, 67);
            this.textBoxAN3scale.Name = "textBoxAN3scale";
            this.textBoxAN3scale.Size = new System.Drawing.Size(48, 20);
            this.textBoxAN3scale.TabIndex = 18;
            this.textBoxAN3scale.Text = "0.00503";
            // 
            // label_an0float
            // 
            this.label_an0float.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an0float.Location = new System.Drawing.Point(348, 9);
            this.label_an0float.Name = "label_an0float";
            this.label_an0float.Size = new System.Drawing.Size(75, 19);
            this.label_an0float.TabIndex = 20;
            this.label_an0float.Text = "3.3333";
            // 
            // label_an1float
            // 
            this.label_an1float.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an1float.Location = new System.Drawing.Point(348, 27);
            this.label_an1float.Name = "label_an1float";
            this.label_an1float.Size = new System.Drawing.Size(75, 19);
            this.label_an1float.TabIndex = 21;
            this.label_an1float.Text = "3.3333";
            // 
            // label_an2float
            // 
            this.label_an2float.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an2float.Location = new System.Drawing.Point(348, 47);
            this.label_an2float.Name = "label_an2float";
            this.label_an2float.Size = new System.Drawing.Size(75, 19);
            this.label_an2float.TabIndex = 22;
            this.label_an2float.Text = "3.3333";
            // 
            // label_an3float
            // 
            this.label_an3float.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_an3float.Location = new System.Drawing.Point(348, 65);
            this.label_an3float.Name = "label_an3float";
            this.label_an3float.Size = new System.Drawing.Size(75, 19);
            this.label_an3float.TabIndex = 23;
            this.label_an3float.Text = "3.3333";
            // 
            // radioButton_raw
            // 
            this.radioButton_raw.AutoSize = true;
            this.radioButton_raw.Checked = true;
            this.radioButton_raw.Location = new System.Drawing.Point(478, 18);
            this.radioButton_raw.Name = "radioButton_raw";
            this.radioButton_raw.Size = new System.Drawing.Size(71, 17);
            this.radioButton_raw.TabIndex = 24;
            this.radioButton_raw.TabStop = true;
            this.radioButton_raw.Text = "Raw data";
            this.radioButton_raw.UseVisualStyleBackColor = true;
            this.radioButton_raw.CheckedChanged += new System.EventHandler(this.radioButton_raw_CheckedChanged);
            // 
            // radioButton_float
            // 
            this.radioButton_float.AutoSize = true;
            this.radioButton_float.Location = new System.Drawing.Point(555, 18);
            this.radioButton_float.Name = "radioButton_float";
            this.radioButton_float.Size = new System.Drawing.Size(72, 17);
            this.radioButton_float.TabIndex = 25;
            this.radioButton_float.Text = "Float data";
            this.radioButton_float.UseVisualStyleBackColor = true;
            this.radioButton_float.CheckedChanged += new System.EventHandler(this.radioButton_float_CheckedChanged);
            // 
            // checkBox_an0
            // 
            this.checkBox_an0.AutoSize = true;
            this.checkBox_an0.Checked = true;
            this.checkBox_an0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_an0.Location = new System.Drawing.Point(15, 19);
            this.checkBox_an0.Name = "checkBox_an0";
            this.checkBox_an0.Size = new System.Drawing.Size(47, 17);
            this.checkBox_an0.TabIndex = 26;
            this.checkBox_an0.Text = "AN0";
            this.checkBox_an0.UseVisualStyleBackColor = true;
            // 
            // checkBox_an1
            // 
            this.checkBox_an1.AutoSize = true;
            this.checkBox_an1.Checked = true;
            this.checkBox_an1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_an1.Location = new System.Drawing.Point(68, 19);
            this.checkBox_an1.Name = "checkBox_an1";
            this.checkBox_an1.Size = new System.Drawing.Size(47, 17);
            this.checkBox_an1.TabIndex = 27;
            this.checkBox_an1.Text = "AN1";
            this.checkBox_an1.UseVisualStyleBackColor = true;
            // 
            // checkBox_an2
            // 
            this.checkBox_an2.AutoSize = true;
            this.checkBox_an2.Checked = true;
            this.checkBox_an2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_an2.Location = new System.Drawing.Point(121, 19);
            this.checkBox_an2.Name = "checkBox_an2";
            this.checkBox_an2.Size = new System.Drawing.Size(47, 17);
            this.checkBox_an2.TabIndex = 28;
            this.checkBox_an2.Text = "AN2";
            this.checkBox_an2.UseVisualStyleBackColor = true;
            // 
            // checkBox_an3
            // 
            this.checkBox_an3.AutoSize = true;
            this.checkBox_an3.Checked = true;
            this.checkBox_an3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_an3.Location = new System.Drawing.Point(174, 19);
            this.checkBox_an3.Name = "checkBox_an3";
            this.checkBox_an3.Size = new System.Drawing.Size(47, 17);
            this.checkBox_an3.TabIndex = 29;
            this.checkBox_an3.Text = "AN3";
            this.checkBox_an3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBox_an3);
            this.groupBox1.Controls.Add(this.radioButton_raw);
            this.groupBox1.Controls.Add(this.radioButton_float);
            this.groupBox1.Controls.Add(this.checkBox_an0);
            this.groupBox1.Controls.Add(this.checkBox_an2);
            this.groupBox1.Controls.Add(this.checkBox_an1);
            this.groupBox1.Controls.Add(this.checkBox_pause);
            this.groupBox1.Controls.Add(this.button_clear);
            this.groupBox1.Location = new System.Drawing.Point(12, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 39);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // label_poolStat
            // 
            this.label_poolStat.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_poolStat.Location = new System.Drawing.Point(429, 65);
            this.label_poolStat.Name = "label_poolStat";
            this.label_poolStat.Size = new System.Drawing.Size(368, 19);
            this.label_poolStat.TabIndex = 31;
            this.label_poolStat.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label_poolStat);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label_an3float);
            this.Controls.Add(this.label_an2float);
            this.Controls.Add(this.label_an1float);
            this.Controls.Add(this.label_an0float);
            this.Controls.Add(this.textBoxAN3datum);
            this.Controls.Add(this.textBoxAN3scale);
            this.Controls.Add(this.textBoxAN2datum);
            this.Controls.Add(this.textBoxAN2scale);
            this.Controls.Add(this.textBoxAN1datum);
            this.Controls.Add(this.textBoxAN1scale);
            this.Controls.Add(this.textBoxAN0datum);
            this.Controls.Add(this.textBoxAN0scale);
            this.Controls.Add(this.label_AVGkbps);
            this.Controls.Add(this.label_updateKBPS);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button_open);
            this.Controls.Add(this.button_relist);
            this.Controls.Add(this.label_an3data);
            this.Controls.Add(this.label_an2data);
            this.Controls.Add(this.label_an1data);
            this.Controls.Add(this.label_an0data);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "ADC v1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label_an0data;
        private System.Windows.Forms.Label label_an1data;
        private System.Windows.Forms.Label label_an2data;
        private System.Windows.Forms.Label label_an3data;
        private System.Windows.Forms.Button button_relist;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBox_pause;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label_updateKBPS;
        private System.Windows.Forms.Label label_AVGkbps;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.TextBox textBoxAN0scale;
        private System.Windows.Forms.TextBox textBoxAN0datum;
        private System.Windows.Forms.TextBox textBoxAN1datum;
        private System.Windows.Forms.TextBox textBoxAN1scale;
        private System.Windows.Forms.TextBox textBoxAN2datum;
        private System.Windows.Forms.TextBox textBoxAN2scale;
        private System.Windows.Forms.TextBox textBoxAN3datum;
        private System.Windows.Forms.TextBox textBoxAN3scale;
        private System.Windows.Forms.Label label_an0float;
        private System.Windows.Forms.Label label_an1float;
        private System.Windows.Forms.Label label_an2float;
        private System.Windows.Forms.Label label_an3float;
        private System.Windows.Forms.RadioButton radioButton_raw;
        private System.Windows.Forms.RadioButton radioButton_float;
        private System.Windows.Forms.CheckBox checkBox_an0;
        private System.Windows.Forms.CheckBox checkBox_an1;
        private System.Windows.Forms.CheckBox checkBox_an2;
        private System.Windows.Forms.CheckBox checkBox_an3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_poolStat;
    }
}

