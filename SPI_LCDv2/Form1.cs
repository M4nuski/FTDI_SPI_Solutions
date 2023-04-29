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

namespace SPI_LCDv2
{
    public partial class Form1 : Form
    {
        private USB_Control usb = new USB_Control();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ExtLog.bx = textBox_log;
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            var list = usb.GetDevicesList();
            ExtLog.AddLine(list.Count + " Devices Found");
            if (list.Count == 0) return;
            uint loc = 0;

            for (var i = 0; i < list.Count; ++i)
            {
                ExtLog.AddLine(i + ", ID: " + list[i].ID + ", locID: " + list[i].LocId + ", Desc: " + list[i].Description + ", SN: " + list[i].SerialNumber);
                if (list[i].Description == "UM245R") loc = list[i].LocId;
            }

            if (loc != 0) try
                {
                    usb.OpenDeviceByLocation(loc);
                }
                catch (Exception ex)
                {
                    ExtLog.AddLine("Error opening devide: " + ex.Message);
                    return;
                }
            if (usb.IsOpen)
            {
                ExtLog.AddLine("Open");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //    ExtLog.AddLine = "Closed";
            usb.CloseDevice();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (!usb.IsOpen)
            {
                ExtLog.AddLine("not connected");
                return;
            }

            SignalGenerator.addByte(false, true, false, false, false); // DD 1 0 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            ExtLog.AddLine("Sending 16 chars address 0x00");
            for (var i = 0; i < 16; i++)
            {
                byte c = (byte)(i + 65);
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            ExtLog.AddLine("Sending 16 chars address 0x30");
            SignalGenerator.addByte(false, true, true, false, false); // DD  1 1 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            for (var i = 0; i < 16; i++)
            {
                byte c = (byte)(i + 97);
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            usb.Transfer();
            ExtLog.AddLine("Done");
        }

        private void button_init_Click(object sender, EventArgs e)
        {
            if (!usb.IsOpen)
            {
                ExtLog.AddLine("not connected");
                return;
            }

            ExtLog.AddLine("Reset sequence");

            SignalGenerator.addByte(false, false, false, true, true); // function set 0 0 1 1 reset
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, true, false); // function set 0 0 1 0 4bit interface
            SignalGenerator.addByte(false, true, true, false, false); // function set N F 0 0  2line 5x10
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, true, false); // function set 0 0 1 0 4bit interface
            SignalGenerator.addByte(false, true, true, false, false); // function set N F 0 0  2line 5x10
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, false, false); // display off 0 0 0 0
            SignalGenerator.addByte(false, true, false, false, false); // function set 1 D C B display cursor blink
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, true, true); // function set 0 0 1 1 reset
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, true, false); // function set 0 0 1 0 4bit interface
            SignalGenerator.addByte(false, true, true, false, false); // function set N F 0 0  2line 5x10
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, true, false); // function set 0 0 1 0 4bit interface
            SignalGenerator.addByte(false, true, true, false, false); // function set N F 0 0  2line 5x10
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, false, false); // display clear 0 0 0 0
            SignalGenerator.addByte(false, false, false, false, true); // function set 0 0 0 1 
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, false, false); // display on 0 0 0 0
            SignalGenerator.addByte(false, true, true, true, false); // function set 1 D C B display cursor blink
            usb.Transfer();
            Thread.Sleep(50);

            SignalGenerator.addByte(false, false, false, false, false); // entry mode 0 0 0 0
            SignalGenerator.addByte(false, false, true, true, false); // function set 0 1 ID SH inc rightshift
            usb.Transfer();
            Thread.Sleep(50);
            ExtLog.AddLine("Done");
        }


        private static string byteToHex(byte b)
        {
            return Convert.ToString(b, 16).PadLeft(2, '0').ToUpperInvariant();
        }

        private static string byteToBin(byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        private void checkBox_RS_CheckedChanged(object sender, EventArgs e)
        {
            SignalGenerator.addByte(checkBox_RS.Checked, checkBox_DB7.Checked, checkBox_DB6.Checked, checkBox_DB5.Checked, checkBox_DB4.Checked);
            usb.Transfer();
        }

        private void checkBox_E_CheckedChanged(object sender, EventArgs e)
        {
            byte b = SignalGenerator.genByte(false, false, checkBox_E.Checked); //strobe E
            usb.BitBang(b); ExtLog.AddLine(byteToHex(b) + " " + byteToBin(b));
        }

        private void button_sendHex_Click(object sender, EventArgs e)
        {
            if (!usb.IsOpen)
            {
                ExtLog.AddLine("not connected");
                return;
            }

            SignalGenerator.addByte(false, true, false, false, false); // DD 1 0 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            ExtLog.AddLine("Sending 16 chars address 0x00");
            for (var i = 0; i < 16; i++)
            {
                byte c = (byte)(i + 32);
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            ExtLog.AddLine("Sending 16 chars address 0x30");
            SignalGenerator.addByte(false, true, true, false, false); // DD  1 1 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            for (var i = 0; i < 16; i++)
            {
                byte c = (byte)(i + 48);
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            usb.Transfer();
            ExtLog.AddLine("Done");
        }

        private void button_sendText_Click(object sender, EventArgs e)
        {
            if (!usb.IsOpen)
            {
                ExtLog.AddLine("not connected");
                return;
            }
            var l1 = (textBox_input.Lines.Length > 0) ? textBox_input.Lines[0] : "";
            var l2 = (textBox_input.Lines.Length > 1) ? textBox_input.Lines[1] : "";

            var l3 = (textBox_input.Lines.Length > 2) ? textBox_input.Lines[2] : "";
            var l4 = (textBox_input.Lines.Length > 3) ? textBox_input.Lines[3] : "";

            l1 = l1.PadRight(20, ' ').Substring(0, 20);
            l2 = l2.PadRight(20, ' ').Substring(0, 20);
            l3 = l3.PadRight(20, ' ').Substring(0, 20);
            l4 = l4.PadRight(20, ' ').Substring(0, 20);

            var b1 = Encoding.ASCII.GetBytes(l1);
            var b2 = Encoding.ASCII.GetBytes(l2);
            var b3 = Encoding.ASCII.GetBytes(l3);
            var b4 = Encoding.ASCII.GetBytes(l4);

            var lw = comboBox_size.Text.Substring(0, 2) == "16" ? 16 : 20;
            var lh = comboBox_size.Text.Substring(3, 1) == "2" ? 2 : 4;

            SignalGenerator.addByte(false, true, false, false, false); // DD 1 0 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            ExtLog.AddLine($"Sending {lw} chars address 0x00");
            for (var i = 0; i < lw; i++)
            {
                byte c = b1[i];
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            ExtLog.AddLine($"Sending {lw} chars address 0x40");
            SignalGenerator.addByte(false, true, true, false, false); // DD  1 1 0 0
            SignalGenerator.addByte(false, false, false, false, false); //   0 0 0 0
            for (var i = 0; i < lw; i++)
            {
                byte c = b2[i];
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
            }

            if (lh == 4)
            {
                ExtLog.AddLine($"Sending {lw} chars address 0x14");
                SignalGenerator.addByte(false, true, false, false, true); // DD  1 0 0 1
                SignalGenerator.addByte(false, false, true, false, false); //   0 1 0 0
                for (var i = 0; i < lw; i++)
                {
                    byte c = b3[i];
                    SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                    SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
                }
                ExtLog.AddLine($"Sending {lw} chars address 0x54");
                SignalGenerator.addByte(false, true, true, false, true); // DD  1 1 0 1
                SignalGenerator.addByte(false, false, true, false, false); //   0 1 0 0
                for (var i = 0; i < lw; i++)
                {
                    byte c = b4[i];
                    SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 7), SignalGenerator.GetBit(c, 6), SignalGenerator.GetBit(c, 5), SignalGenerator.GetBit(c, 4));
                    SignalGenerator.addByte(true, SignalGenerator.GetBit(c, 3), SignalGenerator.GetBit(c, 2), SignalGenerator.GetBit(c, 1), SignalGenerator.GetBit(c, 0));
                }
            }

            usb.Transfer();
            ExtLog.AddLine("Done");
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            if (!usb.IsOpen)
            {
                ExtLog.AddLine("not connected");
                return;
            }

            SignalGenerator.addByte(false, false, false, false, false); // display clear 0 0 0 0
            SignalGenerator.addByte(false, false, false, false, true);  //               0 0 0 1 
            usb.Transfer();
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
