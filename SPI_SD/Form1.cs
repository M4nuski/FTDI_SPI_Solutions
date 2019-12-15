using System;
using System.Text;
using System.Windows.Forms;

namespace SPI_SD
{
    public partial class Form1 : Form
    {
        private USB_Control usb = new USB_Control();


        public Form1()
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
                    usb.SetLatency(16);
                    usb.ChangeCS(true);               
                }
            }
            else
            {
                ExtLog.AddLine("Wrong number of devices found: " + dl.Count);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(true);


            // cs high, data high
            // at least 74 clocks 
            // cs to 0
            // send cmd: reset
            // keep mosi high
            // cycle until miso goes down to low (start bit) about 16 clock cycles
            // receive ack 8 bits 

            ExtLog.AddLine("Init");
            for (int i = 0; i < 10; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataLength = 10;
            usb.Transfer();

            usb.ChangeCS(false);

            ExtLog.AddLine("Reset to SPI (CMD0)");
            for (int i = 0; i < 32; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }

            // cmd 0
            // 01000000 00000000 00000000 00000000 00000000 10010101
            // 64 = 0x40 = b'01000000'  ...0...   149 = 0x95 = b'10010101'
            SignalGenerator.OutputDataBytes[0] = 64;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 0;
            SignalGenerator.OutputDataBytes[4] = 0;
            SignalGenerator.OutputDataBytes[5] = 149;

            SignalGenerator.OutputDataLength = 32; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }

            usb.ChangeCS(true);
            usb.ChangeCS(false);
            // cmd8 0100 1000  0000 0000  0000 0000  0000 0001  1010 1010  0000 1111
            // 72
            // 0
            // 0
            // 1
            // 170
            // 15
            // check version

            ExtLog.AddLine("check CMD8");
            for (int i = 0; i < 32; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 72;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 1;
            SignalGenerator.OutputDataBytes[4] = 170;
            SignalGenerator.OutputDataBytes[5] = 15;

            SignalGenerator.OutputDataLength = 32; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }

            usb.ChangeCS(true);
            usb.ChangeCS(false);

            // CMD58
            // 0111 1010 00000000 00000000 00000000 00000000 0111 0101
            // 122
            // 0
            // 0
            // 0
            // 0
            // 117
            ExtLog.AddLine("check CMD58");
            for (int i = 0; i < 32; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 122;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 0;
            SignalGenerator.OutputDataBytes[4] = 0;
            SignalGenerator.OutputDataBytes[5] = 117;

            SignalGenerator.OutputDataLength = 32; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }

            usb.ChangeCS(true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            usb.ChangeCS(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);
            // CMD55 next command in App cmd
            // 01 + 55
            // 119
            // 0
            // 0
            // 0
            // 0
            // 0
            ExtLog.AddLine("check CMD55");
            for (int i = 0; i < 32; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 119;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 0;
            SignalGenerator.OutputDataBytes[4] = 0;
            SignalGenerator.OutputDataBytes[5] = 1;

            SignalGenerator.OutputDataLength = 32; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }

        //    usb.ChangeCS(true);
         //   usb.ChangeCS(false);

            // ACMD41 init and capacity

            // 105
            // 0
            // 0
            // 0
            // 0
            // 1
            ExtLog.AddLine("check ACMD41");
            for (int i = 0; i < 32; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 105;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 0;
            SignalGenerator.OutputDataBytes[4] = 0;
            SignalGenerator.OutputDataBytes[5] = 1;

            SignalGenerator.OutputDataLength = 32; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }

            usb.ChangeCS(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);

            // CMD9 SEND_CSD

            // 73
            // 0
            // 0
            // 0
            // 0
            // 1
            ExtLog.AddLine("check Card Specific Data Reg CMD9");
            for (int i = 0; i < 48; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 73;
            SignalGenerator.OutputDataBytes[1] = 0;
            SignalGenerator.OutputDataBytes[2] = 0;
            SignalGenerator.OutputDataBytes[3] = 0;
            SignalGenerator.OutputDataBytes[4] = 0;
            SignalGenerator.OutputDataBytes[5] = 1;

            SignalGenerator.OutputDataLength = 48; // 5 + 2 (16) + (2 (8 + whatever))
            usb.Transfer();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputDataBytes[i], 2).PadLeft(8, '0'));
            }
            usb.ChangeCS(true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);

            // CMD17 Read Single Block

            // 81
            // 0 address byte 3 
            // 0 address byte 2
            // 0 address byte 1
            // 0 address byte 0
            // 1
            ExtLog.AddLine("read single block at address 0x" + textBoxAddr3.Text + textBoxAddr2.Text + textBoxAddr1.Text + textBoxAddr0.Text +" CMD17");
            for (int i = 0; i < 540; i++)
            {
                SignalGenerator.OutputDataBytes[i] = 255;
            }
            SignalGenerator.OutputDataBytes[0] = 81;
            SignalGenerator.OutputDataBytes[1] = safeHexParse(textBoxAddr3.Text); // MSB
            SignalGenerator.OutputDataBytes[2] = safeHexParse(textBoxAddr2.Text);
            SignalGenerator.OutputDataBytes[3] = safeHexParse(textBoxAddr1.Text);
            SignalGenerator.OutputDataBytes[4] = safeHexParse(textBoxAddr0.Text);
            SignalGenerator.OutputDataBytes[5] = 1;
          //  03B5DC00
            SignalGenerator.OutputDataLength = 540; // header + block size of 512
            usb.Transfer();
            var sb = new StringBuilder();
            for (int i = 0; i < SignalGenerator.InputDataLength; i++)
            {
                if ((i % 8) == 0)
                {
                    sb.AppendLine();
                    sb.Append("[" + i.ToString("D4") + "] ");
                }
                sb.Append(Convert.ToString(SignalGenerator.InputDataBytes[i], 16).PadLeft(2, '0') + " ");
             //   ExtLog.AddLine("[" + i.ToString("D4") + "] " + Convert.ToString(SignalGenerator.InputBytes[i], 2).PadLeft(8, '0'));
            }
            ExtLog.AddLine(sb.ToString());
            usb.ChangeCS(true);
        }


        private void sendCmd(byte cmd)
        {

        }

        private void sendCmd(byte cmd, byte b3, byte b2, byte b1, byte b0, byte crc7)
        {
           // cmd or 01000000
           // crc7 and 00000001 
        }

        private byte getResponseR1(int nbTry)
        {
            // 1 byte starting with 0
            return 0; 
        }

        private byte[] getResponseR2(int nbTry)
        {
            // 2 byte starting with 0
            return new byte[2] { 0, 0 };
        }

        private byte[] getResponseR3R7(int nbTry)
        {
            // 5 byte starting with 0
            return new byte[5] { 0, 0, 0, 0, 0 };
        }

        private byte[] getResponseSingleBlock(int nbTry, int blockLen)
        {
            // blockLen byte starting with 0/R1, FF then FE token, blockLen bytes then CRC16
            return new byte[blockLen];
        }


        private static byte safeHexParse(string s)
        {
            //    int result;
            //    int.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out result);
            //    return result;

            byte result = 0;
            try
            {
                result = Convert.ToByte(s, 16);
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        private static string byteToHex(byte b)
        {
            return Convert.ToString(b, 16).PadLeft(2, '0');
        }
        private static string byteToHex(int i)
        {
            return Convert.ToString((i & 0xFF), 16).PadLeft(2, '0');
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var address = 0;

            address += safeHexParse(textBoxAddr0.Text);
            address += safeHexParse(textBoxAddr1.Text) << 8;
            address += safeHexParse(textBoxAddr2.Text) << 16;
            address += safeHexParse(textBoxAddr3.Text) << 24;

            address += 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button5_Click(null, null);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockDec.Text, 10);

            address *= 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button5_Click(null, null);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockHex.Text, 16);

            address *= 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button5_Click(null, null);
        }

        private byte CRC7(byte[] message, int length) {
          var poly = (byte)0b10001001;
          var crc = (byte)0x00;
          for (var i = 0; i < length; i++) {
             crc ^= message[i];
             for (int j = 0; j< 8; j++) {
              // crc = crc & 0x1 ? (crc >> 1) ^ poly : crc >> 1;       
              crc = ((crc & 0x80) > 0) ? (byte)((crc << 1) ^ (poly << 1)) : (byte)(crc << 1);
            }
        }
          //return crc;
          return (byte)(crc >> 1);
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
