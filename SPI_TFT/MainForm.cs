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

        private byte[] ST7735_RGB_MODES = { 0x03, 0x05, 0x06 }; // 444 565 666

        private int bulksize;
        private int numPixels;
        private byte pixelDataBuffer;

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
            if (bulksize >= 65535)
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

                numPixels = 0;

                    sendCMD(ST7735_COLMOD);
                    sendDTA(ST7735_RGB_MODES[BPPcomboBox.SelectedIndex]); //rgb666


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
            switch (BPPcomboBox.SelectedIndex)
            {
                case 0: // 444 12bits 4K 1.5Bytes/Pixel
                    if ((numPixels % 2) == 0) //"first" pixel
                    {
                        pixelDataBuffer = (byte)(r & 0xF0);     //R1 HN
                        // r7 r6 r5 r4  00 00 00 00
                        pixelDataBuffer |= (byte)(g >> 4);      //G1 LN
                        // r7 r6 r5 r4  g7 g6 g5 g4
                        addtobulk(pixelDataBuffer);             //Byte 0 : R1 G1

                        pixelDataBuffer = (byte)(b & 0xF0);     //B1 HN
                        // b7 b6 b5 b4  00 00 00 00
                    }
                    else                    //"second" pixel
                    {
                        pixelDataBuffer |= (byte)(r >> 4);      //R2 LN
                        // b7 b6 b5  b4 r7 r6 r5 r4
                        addtobulk(pixelDataBuffer);             //Byte 1 : B1 R2

                        pixelDataBuffer = (byte)(g & 0xF0);   //G2 HN
                        // g7 g6 g5 g4  00 00 00 00
                        pixelDataBuffer |= (byte)(b >> 4);      //B2 LN
                        addtobulk(pixelDataBuffer);             //Byte 2 : G2 B2
                        // g7 g6 g5 g4  b7 b6 b5 b4
                    }
                    numPixels++;
                    break;


                case 1: // 565 16bits 65K 2Bytes/pixel
                    pixelDataBuffer = (byte)(r & 0xF8);     //R 5 top bits
                    // r7 r6 r5 r4  r3 00 00 00
                    pixelDataBuffer |= (byte)(g >> 5);      //G high 3 bits
                    // r7 r6 r5 r4  r3 g7 g6 g5
                    addtobulk(pixelDataBuffer);             //Byte 0

                    pixelDataBuffer = (byte)((g << 3) & 0xE0); //G low 3 bits
                    // g4 g3 g2 00  00 00 00 00
                    pixelDataBuffer |= (byte)(b >> 3);      //B 5 top bit
                    // g4 g3 g2 b7  b6 b5 b4 b3
                    addtobulk(pixelDataBuffer);             //Byte 1

                    break;

                case 2: // 666 18bits 262K 3Bytes/pixel
                    //sent as RGB888
                    addtobulk(r);
                    addtobulk(g);
                    addtobulk(b);
                    break;
            }
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

                numPixels = 0;
                    sendCMD(ST7735_COLMOD);
                    sendDTA(ST7735_RGB_MODES[BPPcomboBox.SelectedIndex]); //rgb666
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

        private void MainForm_Activated(object sender, EventArgs e)
        {
            BPPcomboBox.SelectedIndex = 0;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //D:\Prog\Media\Media\8x8font.png
            pictureBox1.Image = new Bitmap(256, 480);
            var g = Graphics.FromImage(pictureBox1.Image);

            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, 256, 480);
            var wb = new SolidBrush(Color.White);
            var fs = FontFamily.GenericMonospace.GetEmHeight(FontStyle.Regular);
            var f = new Font(FontFamily.GenericMonospace, 16, FontStyle.Regular);
            //var points = f.SizeInPoints;
            //var pixels = points * g.DpiX / 72;
            //var em = pixels;
            var points = 16 * 72 / g.DpiX;
            var em = points * fs;
            var f2 = new Font("Consolas", 16, GraphicsUnit.Pixel);
            f = new Font(FontFamily.GenericMonospace, 16 * g.DpiY / 72, FontStyle.Bold);
            //ExtLog.AddLine(pixels.ToString());
            for (var l = 2; l < 8; ++l)
            {
                var s = "";
                for (var c = 0; c < 32; ++c) s += Convert.ToChar(c + (l*32));

                g.DrawString(s, f2, wb, 0, (l-2) * 16);
            }
            for (var l = 10; l < 64; ++l)
            {
                var s = "";
                for (var c = 0; c < 32; ++c) s += Convert.ToChar(c + (l * 32));

                g.DrawString(s, f2, wb, 0, (l - 4) * 16);
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
