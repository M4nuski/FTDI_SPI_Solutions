using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_FLASH
{
    public partial class MainForm : Form
    {
        private const byte SPI_read = 0x03; //Aa3 Du0 Da1-inf
        private const byte SPI_HS_read = 0x0B; //Aa3 Du1 Da0
        private const byte SPI_4K_sector_erease = 0x20; //Aa3 Du0 Da0
        private const byte SPI_32K_block_erease = 0x52; //Aa3 Du0 Da0
        private const byte SPI_64K_block_erease = 0xD8; //Aa3 Du0 Da0
        private const byte SPI_chip_erease = 0x60; //Aa3 Du0 Da0
        private const byte SPI_byte_program = 0x02; //Aa3 Du0 Da1
        private const byte SPI_AAI_program = 0xAD; //Aa3 Du0 Da2-inf

        private const byte SPI_read_SSR = 0x05; //Aa0 Du0 Da1-inf
        private const byte SPI_read_SSR2 = 0x35; //Aa0 Du0 Da1-inf
        private const byte SPI_read_SSR3 = 0x15; //Aa0 Du0 Da1-inf

        private const byte SPI_enable_write_SSR = 0x50; //Aa0 Du0 Da0
        private const byte SPI_write_SSR = 0x01; //Aa0 Du0 Da1

        private const byte SPI_write_SSR2 = 0x31; //Aa0 Du0 Da1
        private const byte SPI_write_SSR3 = 0x11; //Aa0 Du0 Da1

        private const byte SPI_write_enable = 0x06; //Aa0 Du0 Da0
        private const byte SPI_write_disable = 0x04; //Aa0 Du0 Da0
        private const byte SPI_read_ID = 0x90; //Aa3 Du0 Da1-inf
        private const byte SPI_read_JEDEC = 0x9F; //Aa0 Du0 Da1-inf
        private const byte SPI_busy_on_MISO = 0x70; //AAI //Aa0 Du0 Da0
        private const byte SPI_data_on_MISO = 0x80; //Aa0 Du0 Da0

        private const byte SPI_QPI_enable = 0x38; //Aa0 Du0 Da0
        private const byte SPI_reset_enable = 0x66; //Aa0 Du0 Da0
        private const byte SPI_reset_device = 0x99; //Aa0 Du0 Da0

        private const string JEDEC_ID_W25Q128FV_SPI = "EF4018";
        private const string JEDEC_ID_W25Q128FV_QPI = "EF6018";
        private const string JEDEC_ID_SST25VF016B = "BF2541";

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
                    usb.BitBang(SignalGenerator.genByte(true, true, true, true, true));
                    label3.Text = "Unkown Device";
                }
            }
            else
            {
                ExtLog.AddLine("Wrong number of devices found: " + dl.Count);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            setCommand(SPI_read_SSR, 0, 1);
            //SignalGenerator.OutputBytes[0] = SPI_read_SSR;
            //SignalGenerator.OutputBytes[1] = 0; //result byte;
            //SignalGenerator.OutputLength = 2;

            usb.Transfer();

            ExtLog.AddLine("SSR:" + formatSSR(SignalGenerator.InputBytes[1]));
        }

        private static string tobin(byte val)
        {
            return Convert.ToString(val, 2).PadLeft(8, '0');
        }

        private  string formatSSR(byte val)
        {
            var sb = new StringBuilder();

            if (textBox5.Text == JEDEC_ID_SST25VF016B)
            {
                sb.Append(" BUSY:" + boolTo01(SignalGenerator.GetBit(val, 0)));
                sb.Append(" WEL:" + boolTo01(SignalGenerator.GetBit(val, 1)));
                sb.Append(" BP0:" + boolTo01(SignalGenerator.GetBit(val, 2)));
                sb.Append(" BP1:" + boolTo01(SignalGenerator.GetBit(val, 3)));

                sb.Append(" BP2:" + boolTo01(SignalGenerator.GetBit(val, 4)));
                sb.Append(" BP3:" + boolTo01(SignalGenerator.GetBit(val, 5)));
                sb.Append(" AAI:" + boolTo01(SignalGenerator.GetBit(val, 6)));
                sb.Append(" BPL:" + boolTo01(SignalGenerator.GetBit(val, 7)));
            }

            if (textBox5.Text == JEDEC_ID_W25Q128FV_SPI)
            {
                sb.Append(" BUSY:" + boolTo01(SignalGenerator.GetBit(val, 0)));
                sb.Append(" WEL:" + boolTo01(SignalGenerator.GetBit(val, 1)));
                sb.Append(" BP0:" + boolTo01(SignalGenerator.GetBit(val, 2)));
                sb.Append(" BP1:" + boolTo01(SignalGenerator.GetBit(val, 3)));

                sb.Append(" BP2:" + boolTo01(SignalGenerator.GetBit(val, 4)));
                sb.Append(" TP:" + boolTo01(SignalGenerator.GetBit(val, 5)));
                sb.Append(" SEC:" + boolTo01(SignalGenerator.GetBit(val, 6)));
                sb.Append(" SPR0:" + boolTo01(SignalGenerator.GetBit(val, 7)));
            }
            if (textBox5.Text == JEDEC_ID_W25Q128FV_QPI) sb.Append("W25Q128FV QPI Mode Not Implemented.");

                return sb.ToString();
        }

        private static string boolTo01(bool val)
        {
            return val ? "1" : "0";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            setCommand(SPI_read_JEDEC, 0, 3);
            usb.Transfer();
            ExtLog.AddLine("MFRID:" + tobin(SignalGenerator.InputBytes[1]) + " ID:" + tobin(SignalGenerator.InputBytes[2]) + " " + tobin(SignalGenerator.InputBytes[3]));
            textBox5.Text = SignalGenerator.InputBytes[1].ToString("X2") + SignalGenerator.InputBytes[2].ToString("X2") +
                            SignalGenerator.InputBytes[3].ToString("X2");

            if (textBox5.Text == JEDEC_ID_SST25VF016B) label3.Text = "SST25VF016B";
            if (textBox5.Text == JEDEC_ID_W25Q128FV_SPI) label3.Text = "W25Q128FV";
            if (textBox5.Text == JEDEC_ID_W25Q128FV_QPI) label3.Text = "W25Q128FV (QPI)";
        }

        private static void setCommand(byte command, byte A23A16, byte A15A08, byte A07A00, int numDummy, int numResult)
        {
            SignalGenerator.OutputBytes[0] = command;

            SignalGenerator.OutputBytes[1] = A23A16;
            SignalGenerator.OutputBytes[2] = A15A08;
            SignalGenerator.OutputBytes[3] = A07A00;

            var length = 4;

            for (var i = 0; i < numDummy; i++)
            {
                SignalGenerator.OutputBytes[length] = 0;
                length++;
            }

            for (var i = 0; i < numResult; i++)
            {
                SignalGenerator.OutputBytes[length] = 0;
                length++;
            }

            SignalGenerator.OutputLength = length;
        }

        private static void setCommand(byte command, int numDummy, int numResult)
        {

            SignalGenerator.OutputBytes[0] = command;
            var length = 1;
            
            for (var i = 0; i < numDummy; i++)
            {
                SignalGenerator.OutputBytes[length] = 0;
                length++;
            }

            for (var i = 0; i < numResult; i++)
            {
                SignalGenerator.OutputBytes[length] = 0;
                length++;
            }

            SignalGenerator.OutputLength = length;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (label3.Text == "W25Q128FV")
            {
                setCommand(SPI_read_SSR, 0, 1);
                usb.Transfer();
                ExtLog.AddLine("SSR1:" + tobin(SignalGenerator.InputBytes[1]));
                setCommand(SPI_read_SSR2, 0, 1);
                usb.Transfer();
                ExtLog.AddLine("SSR2:" + tobin(SignalGenerator.InputBytes[1]));
                setCommand(SPI_read_SSR3, 0, 1);
                usb.Transfer();
                ExtLog.AddLine("SSR3:" + tobin(SignalGenerator.InputBytes[1]));
            } else ExtLog.AddLine("Not Supported.");
        }

        private void button5_Click(object sender, EventArgs e)//WE
        {
            setCommand(SPI_write_enable, 0, 0);
            usb.Transfer();
            setCommand(SPI_read_SSR, 0, 1);
            usb.Transfer();
            ExtLog.AddLine("SSR:" + formatSSR(SignalGenerator.InputBytes[1]));
        }

        private void button6_Click(object sender, EventArgs e)//WD
        {
            setCommand(SPI_write_disable, 0, 0);
            usb.Transfer();
            setCommand(SPI_read_SSR, 0, 1);
            usb.Transfer();
            ExtLog.AddLine("SSR:" + formatSSR(SignalGenerator.InputBytes[1]));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            setCommand(SPI_reset_enable, 0, 0);
            usb.Transfer();
            setCommand(SPI_reset_device, 0, 1);
            usb.Transfer();
            ExtLog.AddLine("Reset");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var addr = int.Parse(textBox2.Text);
            var a0 = (addr & 0x000000FF);
            var a1 = (addr & 0x0000FF00) >> 8;
            var a2 = (addr & 0x00FF0000) >> 16;

            setCommand(SPI_4K_sector_erease, (byte)a2, (byte)a1, (byte)a0, 0, 0);
            usb.Transfer();
            ExtLog.AddLine("4K Sector Erase...");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var addr = int.Parse(textBox2.Text);
            var a0 = (addr & 0x000000FF);
            var a1 = (addr & 0x0000FF00) >> 8;
            var a2 = (addr & 0x00FF0000) >> 16;

            setCommand(SPI_32K_block_erease, (byte)a2, (byte)a1, (byte)a0, 0, 0);
            usb.Transfer();
            ExtLog.AddLine("32K Block Erase...");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var addr = int.Parse(textBox2.Text);
            var a0 = (addr & 0x000000FF);
            var a1 = (addr & 0x0000FF00) >> 8;
            var a2 = (addr & 0x00FF0000) >> 16;

            setCommand(SPI_64K_block_erease, (byte)a2, (byte)a1, (byte)a0, 0, 0);
            usb.Transfer();
            ExtLog.AddLine("64K Block Erase...");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            setCommand(SPI_chip_erease, 0, 0);
            usb.Transfer();
            ExtLog.AddLine("Chip Erase...");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var addr = int.Parse(textBox2.Text);
            var a0 = (addr & 0x000000FF);
            var a1 = (addr & 0x0000FF00) >> 8;
            var a2 = (addr & 0x00FF0000) >> 16;

            var numBytes = int.Parse(textBox3.Text);
           // if (numBytes > 255) numBytes = 255;
            setCommand(SPI_read, (byte)a2, (byte)a1, (byte)a0, 0, numBytes);
            ExtLog.AddLine("Reading byte(s)...");
            usb.Transfer();
            var sb = "";
            for (var i = 0; i < numBytes; i++)
            {
                sb += SignalGenerator.InputBytes[4 + i].ToString("X2") + " ";
            }
            ExtLog.AddLine(sb);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var addr = int.Parse(textBox2.Text);
            var a0 = (addr & 0x000000FF);
            var a1 = (addr & 0x0000FF00) >> 8;
            var a2 = (addr & 0x00FF0000) >> 16;

            var dta = textBox4.Text.Split(',');
            var bdta = new byte[dta.Length];
            for (var i = 0; i < dta.Length; i++)
            {
                bdta[i] = byte.Parse(dta[i].Trim(), NumberStyles.AllowHexSpecifier);
            }
            
            setCommand(SPI_byte_program, (byte)a2, (byte)a1, (byte)a0, 0, dta.Length);
            for (var i = 0; i < dta.Length; i++)
            {
                SignalGenerator.OutputBytes[4 + i] = bdta[i];
            }

            ExtLog.AddLine("Writing byte(s)...");
            usb.Transfer();
        }

        private void MainForm_Load(object sender, EventArgs e)
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
