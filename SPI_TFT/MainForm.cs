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


namespace SPI_TFT
{
    public partial class MainForm : Form
    {
        private const byte SCREEN_WIDTH = 128;
        private const byte SCREEN_HEIGHT = 160;

        private const byte ST7735_NOP = 0x0;
        private const byte ST7735_SWRESET = 0x01;
        private const byte ST7735_RDDID = 0x04;
        private const byte ST7735_RDDST = 0x09;

        private const byte ST7735_SLPIN = 0x10;
        private const byte ST7735_SLPOUT = 0x11;
        private const byte ST7735_PTLON = 0x12;
        private const byte ST7735_NORON = 0x13;

        private const byte ST7735_INVOFF = 0x20;
        private const byte ST7735_INVON = 0x21;
        private const byte ST7735_DISPOFF = 0x28;
        private const byte ST7735_DISPON = 0x29;
        private const byte ST7735_CASET = 0x2A;
        private const byte ST7735_RASET = 0x2B;
        private const byte ST7735_RAMWR = 0x2C;
        private const byte ST7735_RAMRD = 0x2E;

        private const byte ST7735_COLMOD = 0x3A;
        private const byte ST7735_MADCTL = 0x36;

        private const byte ST7735_FRMCTR1 = 0xB1;
        private const byte ST7735_FRMCTR2 = 0xB2;
        private const byte ST7735_FRMCTR3 = 0xB3;
        private const byte ST7735_INVCTR = 0xB4;
        private const byte ST7735_DISSET5 = 0xB6;

        private const byte ST7735_PWCTR1 = 0xC0;
        private const byte ST7735_PWCTR2 = 0xC1;
        private const byte ST7735_PWCTR3 = 0xC2;
        private const byte ST7735_PWCTR4 = 0xC3;
        private const byte ST7735_PWCTR5 = 0xC4;
        private const byte ST7735_VMCTR1 = 0xC5;

        private const byte ST7735_RDID1 = 0xDA;
        private const byte ST7735_RDID2 = 0xDB;
        private const byte ST7735_RDID3 = 0xDC;
        private const byte ST7735_RDID4 = 0xDD;

        private const byte ST7735_PWCTR6 = 0xFC;

        private const byte ST7735_GMCTRP1 = 0xE0;
        private const byte ST7735_GMCTRN1 = 0xE1;

        private int bulksize = 0;
        private USB_Control usb = new USB_Control();

        public MainForm()
        {
            InitializeComponent();
            ExtLog.bx = textBox1;
        }

        private void loadDeviceButton_Click(object sender, EventArgs e)
        {
            ExtLog.AddLine("Loading Devices:");
            var dl = usb.GetDevicesList();
            if (dl.Count == 1)
            {
                usb.OpenDeviceByLocation(uint.Parse(dl[0].Substring(0, 4)));
                if (usb.IsOpen)
                {
                    usb.SetLatency(2);
                    resetDisplay();
                }
            }
            else
            {
                ExtLog.AddLine("Wrong number of devices found: " + dl.Count);
            }
        }

        private void sendCMD(byte b)
        {
            SignalGenerator.SPI_A0 = SignalGenerator.SPI_A0_command;
            SignalGenerator.OutputBytes[0] = b;
            SignalGenerator.OutputLength = 1;
            usb.Transfer();
        }

        private void sendDTA(byte b)
        {
            SignalGenerator.SPI_A0 = SignalGenerator.SPI_A0_data;
            SignalGenerator.OutputBytes[0] = b;
            SignalGenerator.OutputLength = 1;
            usb.Transfer();
        }

        private void addtobulk(byte b)
        {
            if (bulksize >= 3200)
            {
                sendbulk();
            }
            SignalGenerator.OutputBytes[bulksize] = b;
            bulksize++;
        }

        private void sendbulk()
        {
            SignalGenerator.SPI_A0 = SignalGenerator.SPI_A0_data;
            SignalGenerator.OutputLength = bulksize;
            usb.Transfer();
            bulksize = 0;
        }

        private void resetDisplay()
        {

            SignalGenerator.SPI_A0 = SignalGenerator.SPI_A0_command;

            SignalGenerator.SPI_Reset = SignalGenerator.SPI_RESET_default;
            usb.Transfer();
            SignalGenerator.SPI_Reset = !SignalGenerator.SPI_RESET_default;
            usb.Transfer();
            Thread.Sleep(10);
            SignalGenerator.SPI_Reset = SignalGenerator.SPI_RESET_default;
            usb.Transfer();

            sendCMD(ST7735_SWRESET);
            sendCMD(ST7735_SLPOUT);
            sendCMD(ST7735_NORON);

            sendCMD(ST7735_COLMOD);
            sendDTA(0x06);

            sendCMD(ST7735_DISPON);

            sendCMD(ST7735_CASET);
            sendDTA(0x00);sendDTA(0x00);
            sendDTA(0x00);sendDTA(0x7F);

            sendCMD(ST7735_RASET);
            sendDTA(0x00);sendDTA(0x00);
            sendDTA(0x00);sendDTA(0x9F);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usb.IsOpen)
            {
                sendCMD(ST7735_CASET);
                sendDTA(0x00); sendDTA(0x00);
                sendDTA(0x00); sendDTA(0x7F);

                sendCMD(ST7735_RASET);
                sendDTA(0x00); sendDTA(0x00);
                sendDTA(0x00); sendDTA(0x9F);
                sendCMD(ST7735_RAMWR);
                using (var img = new Bitmap("quinn.bmp"))

                    for (var i = 0; i < 160; i++)
                    {
                        for (var j = 0; j < 128; j++)
                        {
                            var c = img.GetPixel(j, i);
                            sendcolor(c.R, c.G, c.B);
                        }
                    }

                sendbulk(); //purge bulk buffer

                //horLine(0x0001);
                //horLine(0x0003);
                //horLine(0x0007);
                //horLine(0x000F);
                //horLine(0x001F);

                //horLine(0x0020);
                //horLine(0x0060);
                //horLine(0x00E0);
                //horLine(0x01E0);
                //horLine(0x03E0);
                //horLine(0x07E0);


                //horLine(0x0800);
                //horLine(0x1800);
                //horLine(0x3800);
                //horLine(0x7800);
                //horLine(0xF800);

            }
        }

        private void sendcolor(byte r, byte g, byte b)
        {
            //    var rr = ((r >> 3) << 11);
            //    var gg = ((g >> 2) << 5);
            //    var bb = (b >> 3);

            //    var c =  (rr | gg | bb);

            //  sendlong((ushort)c);
            addtobulk(r);
            addtobulk(g);
            addtobulk(b);
        }

        private void sendlong(ushort l)
        {
            addtobulk((byte) (l >> 8));
            addtobulk((byte) (l & 0x00FF));
        }

        private void horLine(ushort col)
        {
            for (var i = 0; i < 128; i++)
            {
                addtobulk((byte) (col >> 8));
                addtobulk((byte) (col & 0x00FF));
            }
            sendbulk(); //purge bulk buffer
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendCMD(0x39);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sendCMD(0x38);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sendCMD(ST7735_DISPON);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            sendCMD(ST7735_DISPOFF);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            sendCMD(ST7735_FRMCTR1);
            sendDTA(0x0F);
            sendDTA(0x3F);
            sendDTA(0x3F);

            sendCMD(ST7735_RAMWR);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            sendCMD(ST7735_FRMCTR1);
            sendDTA(0x01);
            sendDTA(0x2C);
            sendDTA(0x2B);

            sendCMD(ST7735_RAMWR);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            var dta = SignalGenerator.SetBit(0, 7, checkBox1.Checked);
            dta = SignalGenerator.SetBit(dta, 6, checkBox2.Checked);
            dta = SignalGenerator.SetBit(dta, 5, checkBox3.Checked);
            dta = SignalGenerator.SetBit(dta, 4, checkBox4.Checked);
            dta = SignalGenerator.SetBit(dta, 3, checkBox5.Checked);
            dta = SignalGenerator.SetBit(dta, 2, checkBox6.Checked);

            sendCMD(ST7735_MADCTL);
            // d7 d6 d5 d4 d3 d2 d1 d0
            // my mx mv ml rg mh  0  0

            sendDTA(dta);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (usb.IsOpen)
            {
                sendCMD(ST7735_CASET);
                sendDTA(0x00); sendDTA(0x00);
                sendDTA(0x00); sendDTA(0x9F);

                sendCMD(ST7735_RASET);
                sendDTA(0x00); sendDTA(0x00);
                sendDTA(0x00); sendDTA(0x7F);

                sendCMD(ST7735_RAMWR);
                using (var img = new Bitmap("legologo.bmp"))

                    for (var i = 0; i < 128; i++)
                    {
                        for (var j = 0; j < 160; j++)
                        {
                            var c = img.GetPixel(j, i);
                            sendcolor(c.R, c.G, c.B);
                        }
                    }

                sendbulk(); //purge bulk buffer
            }
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
