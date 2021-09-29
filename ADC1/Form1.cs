using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADC1
{
    public partial class Form1 : Form
    {


        public USB_Control usb = new USB_Control();
        private static uint bufferSize = 0xFFFF;
        private byte[] buffer = new byte[bufferSize];
        private int datalensum;
        private int updatesum;
        private int poolsum;

        public Form1()
        {
            InitializeComponent();
            ExtLog.bx = textBox1;
            button_relist_Click(null, null);

            timer1.Enabled = true;

            //System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        static public float SafeTextToFloat(string text, float fallback)
        {
          
            float res;
            if (float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
           // ExtLog.AddLine("Failed to convert value: " + text);
            return fallback;
        }

        private void button_relist_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            if (usb.IsOpen) usb.CloseDevice();
            var l = usb.GetDevicesList();
            foreach (string d in l) comboBox1.Items.Add(d);
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
          //      usb.OpenDeviceByLocation()
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usb.IsOpen) usb.CloseDevice();
            var sel = comboBox1.SelectedItem.ToString();
            comboBox1.Text = sel;
            usb.OpenDeviceByLocation(uint.Parse(sel.Substring(0, 4)));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!usb.IsOpen) return;
            poolsum++;
            var datalen = usb.ReadUSB(buffer, bufferSize);
            if (datalen < 1) return;
            label_updateKBPS.Text = datalen.ToString("D5") +" bytes / update";
            datalensum += (int)datalen;
            updatesum++;
            //ExtLog.AddLine(datalen.ToString("D"));
            if (checkBox_pause­.Checked) return;
            var changed = 0;
            int i0 = 0;
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            float f0 = 0.0f;
            float f1 = 0.0f;
            float f2 = 0.0f;
            float f3 = 0.0f;
            for (var i = (int)datalen-1; i > 0; --i)
            {
                if ( (buffer[i] & 0b11000000) == (buffer[i-1] & 0b11000000) )
                {
                    int val;
                    float scale = 1.0f;
                    float datum = 0.0f;
                    var channel = (buffer[i] & 0b11000000) >> 6;
                    int val1 = buffer[i-1] & 0b00011111;
                    int val2 = buffer[i] & 0b00011111;
                    if ((buffer[i-1] & 0b00100000) == 0)
                    {
                        val = val1 + (val2 << 5);
                    } else
                    {
                        val = val2 + (val1 << 5);
                    }
                    switch (channel) {
                        case 0:
                            i0 = val;
                            changed |= 0x01;
                            scale = SafeTextToFloat(textBoxAN0scale.Text, 1.0f);
                            datum = SafeTextToFloat(textBoxAN0datum.Text, 0.0f);
                            label_an0data.Text = val.ToString("D4") + " 0x" + Convert.ToString(val, 16).PadLeft(3, '0') + " " + Convert.ToString(val, 2).PadLeft(10, '0');
                            f0 = (val * scale + datum);
                            label_an0float.Text = f0.ToString("F4");
                            break;
                        case 1:
                            i1 = val;
                            changed |= 0x02;
                            scale = SafeTextToFloat(textBoxAN1scale.Text, 1.0f);
                            datum = SafeTextToFloat(textBoxAN1datum.Text, 0.0f);
                            label_an1data.Text = val.ToString("D4") + " 0x" + Convert.ToString(val, 16).PadLeft(3, '0') + " " + Convert.ToString(val, 2).PadLeft(10, '0');
                            f1 = (val * scale + datum);
                            label_an1float.Text = f1.ToString("F4");
                            break;
                        case 2:
                            i2 = val;
                            changed |= 0x04;
                            scale = SafeTextToFloat(textBoxAN2scale.Text, 1.0f);
                            datum = SafeTextToFloat(textBoxAN2datum.Text, 0.0f);
                            label_an2data.Text = val.ToString("D4") + " 0x" + Convert.ToString(val, 16).PadLeft(3, '0') + " " + Convert.ToString(val, 2).PadLeft(10, '0');
                            f2 = (val * scale + datum);
                            label_an2float.Text = f2.ToString("F4");
                            break;
                        case 3:
                            i3 = val;
                            changed |= 0x08;
                            scale = SafeTextToFloat(textBoxAN3scale.Text, 1.0f);
                            datum = SafeTextToFloat(textBoxAN3datum.Text, 0.0f);
                            label_an3data.Text = val.ToString("D4") + " 0x" + Convert.ToString(val, 16).PadLeft(3, '0') + " " + Convert.ToString(val, 2).PadLeft(10, '0');
                            f3 = (val * scale + datum);
                            label_an3float.Text = f3.ToString("F4");
                            break;
                    }
                    i--;
                    if (changed == 0x0F)
                    {
                        i = 1;
                        if (radioButton_raw.Checked)
                        {
                            var logstring = "";
                            if (checkBox_an0.Checked) logstring = i0.ToString("D4");
                            if (checkBox_an1.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += i1.ToString("D4");
                            }
                            if (checkBox_an2.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += i2.ToString("D4");
                            }
                            if (checkBox_an3.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += i3.ToString("D4");
                            }
                            ExtLog.AddLine(logstring);
                        } else
                        {
                            var logstring = "";
                            if (checkBox_an0.Checked) logstring = f0.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                            if (checkBox_an1.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += f1.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (checkBox_an2.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += f2.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            if (checkBox_an3.Checked)
                            {
                                if (logstring != "") logstring += ", ";
                                logstring += f3.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            ExtLog.AddLine(logstring);
                        }
                    }
                }

            //    ExtLog.AddLine(Convert.ToString(buffer[i], 2).PadLeft(8, '0'));
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label_AVGkbps.Text = datalensum.ToString() + " bytes / sec";
            label_poolStat.Text = updatesum.ToString() + " updates / " + poolsum.ToString() + " pools";
            datalensum = 0;
            updatesum = 0;
            poolsum = 0;
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void radioButton_raw_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton_float_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public static class ExtLog
    {
        public static TextBox bx;
        public static void AddLine(string s)
        {
            bx?.AppendText(s + "\r\n");
        }
    }
}
