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
            for (int i = 0; i < 20; i++) SignalGenerator.OutputDataBytes[i] = 255;
            SignalGenerator.OutputDataLength = 20;
            usb.Transfer();


            usb.ChangeCS(false);


            ExtLog.AddLine("Reset to SPI (CMD0)");
            sendCmd(0, 0, 0, 0, 0, 149);
            var cmdR1 = getResponseR1(16);
            ExtLog.AddLine(formatR1(cmdR1));


            ExtLog.AddLine("check support for ver2.0 (CMD8)");
            sendCmd(8, 0, 0, 0, 0, 15);
            var cmdR7 = getResponseR3R7(16);
            ExtLog.AddLine(formatR3R7(cmdR7));
            

            usb.ChangeCS(true);
            usb.ChangeCS(false);


            ExtLog.AddLine("check for init done (CMD58)");
            sendCmd(58, 0, 0, 0, 0, 117);
            cmdR7 = getResponseR3R7(16);
            ExtLog.AddLine(formatR3R7(cmdR7));


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
            ExtLog.AddLine("send application command prefix (CMD55)");
            sendCmd(55, 0, 0, 0, 0, 1);
            var cmdR7 = getResponseR3R7(16);
            ExtLog.AddLine(formatR3R7(cmdR7));


            // ACMD41 init and capacity
            ExtLog.AddLine("send HCS and start init (ACMD41)");
            sendCmd(41, 0, 0, 0, 0, 1);
            cmdR7 = getResponseR3R7(16);
            ExtLog.AddLine(formatR3R7(cmdR7));


            usb.ChangeCS(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);

            // CMD9 SEND_CSD
            ExtLog.AddLine("check Card Specific Data Reg CMD9");
            sendCmd(9, 0, 0, 0, 0, 1);
            var cmdRBlock = getResponseSingleBlock(32, 16); 

            for (int i = 0; i < 16; i++)
            {
                ExtLog.AddLine("[Block " + i.ToString("D4") + "] " + Convert.ToString(cmdRBlock[i], 2).PadLeft(8, '0'));
            }

            decodeCSD(cmdRBlock, "Struct", 126, 127);
            decodeCSD(cmdRBlock, "CCC", 84, 95);
            decodeCSD(cmdRBlock, "READ_BL_LEN", 80, 83);
            decodeCSD(cmdRBlock, "C_SIZE", 62, 73);
            decodeCSD(cmdRBlock, "C_SIZE_MULT", 47, 49);

            var CSIZE = readBitsFromPackedStruct(16, cmdRBlock, 62, 73);
            var CSIZEMULT = readBitsFromPackedStruct(16, cmdRBlock, 47, 49);
            var READBLLEN = readBitsFromPackedStruct(16, cmdRBlock, 80, 83);

            var MULT = (int)Math.Pow(2, CSIZEMULT + 2);
            ExtLog.AddLine("MULT " + MULT);
            var BLOCKNR = (CSIZE + 1) * MULT;
            ExtLog.AddLine("BLOCKNR " + BLOCKNR);
            var BLOCKLEN = (int)Math.Pow(2, READBLLEN);
            ExtLog.AddLine("BLOCKLEN " + BLOCKLEN);
            ExtLog.AddLine("Capacity " + (BLOCKLEN * BLOCKNR));

            usb.ChangeCS(true);
        }

        private void decodeCSD(byte[] data, string name, int start, int stop)
        {
            var res = readBitsFromPackedStruct(16, data, start, stop);
            ExtLog.AddLine("CSD " + name + ": " + Convert.ToString(res, 2).PadLeft(1 + stop - start, '0') + " " + res.ToString("D"));

        }

        private void button_ReadBlock_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);

            // CMD17 Read Single Block
            ExtLog.AddLine("Read single block at address 0x" + textBoxAddr3.Text + textBoxAddr2.Text + textBoxAddr1.Text + textBoxAddr0.Text +" (CMD17)");
            sendCmd(17, safeHexParse(textBoxAddr3.Text),
                        safeHexParse(textBoxAddr2.Text),
                        safeHexParse(textBoxAddr1.Text),
                        safeHexParse(textBoxAddr0.Text), 1);

            var dataBlock = getResponseSingleBlock(64, 512);

            var sb = new StringBuilder();
            var sbc = new StringBuilder();
            for (int i = 0; i < dataBlock.Length; i++)
            {
                if ((i % 8) == 0)
                {
                    sb.Append(" ");
                    sbc.Append(" ");
                }
                if ((i % 16) == 0)
                {
                    sb.AppendLine(sbc.ToString());
                    sbc.Clear();
                    sb.Append("[" + i.ToString("D4") + "] ");
                }
                sb.Append(Convert.ToString(dataBlock[i], 16).PadLeft(2, '0') + " ");
                sbc.Append(safeCharConvert(dataBlock[i]));
            }

            sb.AppendLine(" " + sbc.ToString());
            ExtLog.AddLine(sb.ToString());



            usb.ChangeCS(true);
        }


        private string safeCharConvert(byte b)
        {
            if ((b < 32) || (b >= 127)) return ".";
            return "" + Convert.ToChar(b);
        }

        private void sendCmd(byte cmd, byte b3, byte b2, byte b1, byte b0, byte crc7)
        {
            // cmd with start token 01xx xxxx

            cmd = (byte)(cmd & 0x7F); // start bit 0
            cmd = (byte)(cmd | 0x40); // start bit 1

            SignalGenerator.OutputDataBytes[0] = cmd;
            SignalGenerator.OutputDataBytes[1] = b3;
            SignalGenerator.OutputDataBytes[2] = b2;
            SignalGenerator.OutputDataBytes[3] = b1;
            SignalGenerator.OutputDataBytes[4] = b0;
            SignalGenerator.OutputDataBytes[5] = crc7;// CRC7(SignalGenerator.OutputDataBytes, 5); //TODO proper CRC7
            SignalGenerator.OutputDataBytes[6] = 255;

            SignalGenerator.OutputDataLength = 7;

            usb.Transfer(); // push to usb, ignore input while sending command
        }

        private byte getResponseR1(int nbTry)
        {
            // 1 byte starting with 0

            var tryNumber = 0;

            while (tryNumber < nbTry) {

                SignalGenerator.OutputDataBytes[0] = 255;
                SignalGenerator.OutputDataLength = 1;
             //   SignalGenerator.purgeLeading1s();
                usb.Transfer();

                if (SignalGenerator.InputDataBytes[0] != (byte)0xFF) return SignalGenerator.InputDataBytes[0];
               tryNumber++;
            }

            ExtLog.AddLine("getResponseR1 nbTry exceeded");

            return (byte)0xFF; 
        }

        private byte[] getResponseR2(int nbTry)
        {
            // 2 byte starting with 0

            ExtLog.AddLine("getResponseR2 not implemented");
            return new byte[2] { 0, 0 };
        }

        private byte[] getResponseR3R7(int nbTry)
        {
            // 5 byte starting with 0


            var res = new byte[5] { 0, 0, 0, 0, 0 };
            var tryNumber = 0;
            var responseDone = false;

            while ((tryNumber < nbTry) && !responseDone) //TODO prevent parsing of leading 0s or 1s
            {

                SignalGenerator.OutputDataBytes[0] = 255;
                SignalGenerator.OutputDataLength = 1;
          //      SignalGenerator.purgeLeading1s();
                usb.Transfer();
                if (SignalGenerator.InputDataBytes[0] != 0xFF)
                {
                    res[0] = SignalGenerator.InputDataBytes[0];

                    SignalGenerator.OutputDataBytes[0] = 255;
                    SignalGenerator.OutputDataBytes[1] = 255;
                    SignalGenerator.OutputDataBytes[2] = 255;
                    SignalGenerator.OutputDataBytes[3] = 255;

                    SignalGenerator.OutputDataLength = 4;
                    usb.Transfer();
                    res[1] = SignalGenerator.InputDataBytes[0];
                    res[2] = SignalGenerator.InputDataBytes[1];
                    res[3] = SignalGenerator.InputDataBytes[2];
                    res[4] = SignalGenerator.InputDataBytes[3];

                    responseDone = true;
                }

                tryNumber++;
            }

             if (tryNumber >= nbTry) ExtLog.AddLine("getResponseR1 nbTry exceeded");
            return res;
        }

        private byte[] getResponseSingleBlock(int nbTry, int blockLen)
        {
            // blockLen byte starting with 0/R1, FF then FE token, blockLen bytes then CRC16
            var res = new byte[blockLen];
            var tryNumber = 0;

            var readyForNextStep = false;

            // wait for R1
            while ((tryNumber < nbTry) && !readyForNextStep)
            {
                SignalGenerator.OutputDataBytes[0] = 255;
                SignalGenerator.OutputDataLength = 1;
                //      SignalGenerator.purgeLeading1s();
                usb.Transfer();
                if (SignalGenerator.InputDataBytes[0] != 0xFF)
                {
                    ExtLog.AddLine(formatR1(SignalGenerator.InputDataBytes[0]));
                    readyForNextStep = true;
                }

                tryNumber++;
            }

            // wait for start token
            readyForNextStep = false;
            while ((tryNumber < nbTry) && !readyForNextStep)
            {
                SignalGenerator.OutputDataBytes[0] = 255;
                SignalGenerator.OutputDataLength = 1;
                //      SignalGenerator.purgeLeading1s();
                usb.Transfer();
                if (SignalGenerator.InputDataBytes[0] == (byte)0xFE)
                {
                    ExtLog.AddLine("Data block start Token");
                    readyForNextStep = true;
                }
               tryNumber++;
            }

            for (int i = 0; i < blockLen+3; i++) SignalGenerator.OutputDataBytes[i] = 255;
            SignalGenerator.OutputDataLength = blockLen+3; // block + crc16 + end of block

            usb.Transfer();

            for (int i = 0; i < blockLen; i++) res[i] = SignalGenerator.InputDataBytes[i];

            ExtLog.AddLine("CRC16 " + SignalGenerator.InputDataBytes[blockLen].ToString("X2") + SignalGenerator.InputDataBytes[blockLen+1].ToString("X2"));
            if (tryNumber >= nbTry) ExtLog.AddLine("getResponseR1 nbTry exceeded");

            return res;
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

            button_ReadBlock_Click(null, null);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockDec.Text, 10);

            address *= 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button_ReadBlock_Click(null, null);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockHex.Text, 16);

            address *= 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button_ReadBlock_Click(null, null);
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

        private string formatR1(byte r1)
        {
        //    ExtLog.AddLine("[R1 Status] " + Convert.ToString(cmdR1, 2).PadLeft(8, '0') + " " + );
            var sb = new StringBuilder();
            sb.Append("[R1  Status] ");
            sb.Append(Convert.ToString(r1, 2).PadLeft(8, '0') + " ");

            if (SignalGenerator.GetBit(r1, 6)) sb.Append("ParamError ");
            if (SignalGenerator.GetBit(r1, 5)) sb.Append("AddressError ");
            if (SignalGenerator.GetBit(r1, 4)) sb.Append("EraseSeqError ");
            if (SignalGenerator.GetBit(r1, 3)) sb.Append("ComCRCError ");
            if (SignalGenerator.GetBit(r1, 2)) sb.Append("IllegalCommand ");
            if (SignalGenerator.GetBit(r1, 1)) sb.Append("EraseReset ");
            if (SignalGenerator.GetBit(r1, 0)) sb.Append("Idle ");
            return sb.ToString();
        }

        private string formatR3R7(byte[] data)
        {
            var sb = new StringBuilder();
            sb.AppendLine(formatR1(data[0]));
            for (int i = 1; i < 5; i++) sb.AppendLine("[R3/R7 " + i.ToString("D4") + "] " + Convert.ToString(data[i], 2).PadLeft(8, '0'));
            return sb.ToString();
        }

        private int readBitsFromPackedStruct(int size, byte[] data, int startBit, int stopBit)
        {
            var nbBit = stopBit - startBit + 1;
            var byteOffset = startBit >> 3;
            var bitOffset = startBit - (8 * byteOffset); //& 0xF7;
                                                         // bitOffset = bitOffset - (8 * byteOffset);

            // byte 0
            var res0 = (byte)0;
            for (int i = 0; (i < nbBit) && (i < 8); i++)
            {
                res0 = SignalGenerator.ChangeBit(res0, i,
                    SignalGenerator.GetBit(data[size - 1 - byteOffset], bitOffset)
                   );
                ++bitOffset;
                if (bitOffset == 8)
                {
                    bitOffset = 0;
                    ++byteOffset;
                }
            }

            // byte 1
            var res1 = (byte)0;
            if (nbBit > 0)
            {
                for (int i = 8; (i < nbBit); i++)
                {
                    res1 = SignalGenerator.ChangeBit(res1, i-8,
                        SignalGenerator.GetBit(data[size - 1 - byteOffset], bitOffset)
                       );
                    ++bitOffset;
                    if (bitOffset == 8)
                    {
                        bitOffset = 0;
                        ++byteOffset;
                    }
                }
            }

            return (res1 << 8) + res0;
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
