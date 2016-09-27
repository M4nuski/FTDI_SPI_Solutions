using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCP3553E
{
    public partial class MainForm : Form
    {
        private USB_Control usb = new USB_Control();

        public MainForm()
        {
            InitializeComponent();
            ExtLog.bx = textBox1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExtLog.AddLine("Loading Devices:");
            var dl = usb.GetDevicesList();
            if (dl.Count == 1)
            {
                usb.OpenDeviceByLocation(uint.Parse(dl[0].Substring(0, 4)));
                if (usb.IsOpen)
                {
                    usb.SetLatency(2);
                    usb.BitBang(SignalGenerator.genByte(true, true));

                }
            }
            else
            {
                ExtLog.AddLine("Wrong number of devices found: " + dl.Count);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            usb.BitBang(SignalGenerator.genByte(true, false));
            usb.BitBang(SignalGenerator.genByte(true, true));
            Thread.Sleep(20);
            usb.BitBang(SignalGenerator.genByte(true, false));
            var data = usb.BitBang(SignalGenerator.genByte(true, false));
            if (SignalGenerator.GetBit(data, SignalGenerator.SPI_SDA_bit))
            {
                ExtLog.AddLine("Data not ready");
            }
            else
            {
                usb.Transfer();
                var boosted_data = ((double)((0x3FFFFF & SignalGenerator.InputInt) << 10) / 0x7FFFFE00);
                if ((SignalGenerator.InputInt & 0x00400000) > 0) ExtLog.AddLine("OVH:" + boosted_data.ToString("F8"));
                else if ((SignalGenerator.InputInt & 0x00800000) > 0) ExtLog.AddLine("OVL:" + boosted_data.ToString("F8"));
                else ExtLog.AddLine("Data:"+ boosted_data.ToString("F8"));

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
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
