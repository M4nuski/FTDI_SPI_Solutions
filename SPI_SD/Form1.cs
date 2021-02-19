using System;
using System.Text;
using System.Windows.Forms;

namespace SPI_SD
{
    public partial class Form1 : Form
    {
        private USB_Control usb = new USB_Control();

        private byte[] dataBlock;
        private byte[] dataCluster;
    //    private int clusterStartAddress;
    //    private int bytesPerCluster;
    //    private int blockPerCluster;

        public Form1()
        {
            InitializeComponent();
            ExtLog.bx = textBox1;

        dataBlock = new byte[512];
        dataCluster = new byte[1024]; // TODO set per FAT data
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

            dataBlock = getResponseSingleBlock(64, 512);

            ExtLog.AddLine(formatDataBlock(dataBlock, 512));


            usb.ChangeCS(true);
        }


        private string safeCharConvert(byte b)
        {
            if ((b < 32) || (b >= 127)) return ".";
            return "" + Convert.ToChar(b);
        }

        private char safeCharConvertW(int b)
        {
            if (b < 32) return ' ';
            return Convert.ToChar(b);
        }

        private string formatDataBlock(byte[] data, int len)
        {
            var sb = new StringBuilder();
            var sbc = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
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
                sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0') + " ");
                sbc.Append(safeCharConvert(data[i]));
            }

            sb.AppendLine(" " + sbc.ToString());
            return sb.ToString();
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

        private void buttonReadBlockDecimal_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockDec.Text, 10);
            ExtLog.AddLine("Block " + address + " (dec)");
            address *= 512;

            textBoxAddr0.Text = byteToHex(address);
            textBoxAddr1.Text = byteToHex(address >> 8);
            textBoxAddr2.Text = byteToHex(address >> 16);
            textBoxAddr3.Text = byteToHex(address >> 24);

            button_ReadBlock_Click(null, null);
        }

        private void buttonReadBlockHex_Click(object sender, EventArgs e)
        {
            int address = Convert.ToInt32(textBoxReadBlockHex.Text, 16);
            ExtLog.AddLine("Block 0x" + address + " (hex)");
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
        { // decode up to 32bits / 4bytes of data in tightly packed data
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
            if (nbBit > 8)
            {
                for (int i = 8; (i < nbBit) && (i < 16); i++)
                {
                    res1 = SignalGenerator.ChangeBit(res1, i - 8,
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
            // byte 2
            var res2 = (byte)0;
            if (nbBit > 16)
            {
                for (int i = 16; (i < nbBit) && (i < 24); i++)
                {
                    res2 = SignalGenerator.ChangeBit(res2, i - 16,
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
            // byte 3
            var res3 = (byte)0;
            if (nbBit > 24)
            {
                for (int i = 24; (i < nbBit) && (i < 32); i++)
                {
                    res3 = SignalGenerator.ChangeBit(res3, i - 24,
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


            return (res3 << 24) + (res2 << 16) + (res1 << 8) + res0;
        }

        private byte read1ByteFromPackedStruct(byte[] data, int offset)
        {
            return data[offset];
        }

        private int read2ByteFromPackedStruct(byte[] data, int offset)
        {
            return (data[offset + 1] << 8) + data[offset + 0];
        }

        private int read4ByteFromPackedStruct(byte[] data, int offset)
        {
            return (data[offset + 3] << 24) + (data[offset + 2] << 16) + (data[offset + 1] << 8) + data[offset + 0];
        }

        private void buttonReadMBR_Click(object sender, EventArgs e)
        { // read and decode Boot Sector
         //   textBoxReadBlockDec.Text = textBoxFATsector.Text;

            //textBoxAddr0.Text = "00";
            //textBoxAddr1.Text = "00";
            //textBoxAddr2.Text = "00";
            //textBoxAddr3.Text = "00";

            buttonReadBlockDecimal_Click(null, null);
            //dataBlock
            ExtLog.AddLine("Partition 0");
            readPartitionEntry(0x1BE);

            ExtLog.AddLine("Partition 1");
            readPartitionEntry(0x1CE);

            ExtLog.AddLine("Partition 2");
            readPartitionEntry(0x1DE);

            ExtLog.AddLine("Partition 3");
            readPartitionEntry(0x1EE);

            ExtLog.AddLine("Boot Executable Marker " + read2ByteFromPackedStruct(dataBlock, 0x1FE).ToString("X4"));
        }
        private void readPartitionEntry(int offset)
        {
            ExtLog.AddLine("State: 0x" + read1ByteFromPackedStruct(dataBlock, offset).ToString("X2"));
            ExtLog.AddLine("Start Head: 0x" + read1ByteFromPackedStruct(dataBlock, offset + 1).ToString("X2"));
            ExtLog.AddLine("Start Cyl/Sector: 0x" + read2ByteFromPackedStruct(dataBlock, offset + 2).ToString("X4"));
            ExtLog.AddLine("Type: 0x" + read1ByteFromPackedStruct(dataBlock, offset + 4).ToString("X2"));
            ExtLog.AddLine("End Head: 0x" + read1ByteFromPackedStruct(dataBlock, offset + 5).ToString("X2"));
            ExtLog.AddLine("End Cyl/Sector: 0x" + read2ByteFromPackedStruct(dataBlock, offset + 6).ToString("X4"));
            ExtLog.AddLine("Nb Sectors between MBR and Partition 1st Sector: " + read4ByteFromPackedStruct(dataBlock, offset + 8).ToString("D"));
            ExtLog.AddLine("Nb Sectors in partition: " + read4ByteFromPackedStruct(dataBlock, offset + 12).ToString("D"));
        }

        private void buttonReadFATBoot_Click(object sender, EventArgs e)
        {
           /// int address = 512 * 39;
          //  textBoxReadBlockDec.Text = textBoxMBR.Text;

//            textBoxAddr0.Text = byteToHex(address);
  //          textBoxAddr1.Text = byteToHex(address >> 8);
    //        textBoxAddr2.Text = byteToHex(address >> 16);
      //      textBoxAddr3.Text = byteToHex(address >> 24);

        //    button_ReadBlock_Click(null, null);
       //     textBoxReadBlockDec.Text = textBoxFATsector.Text;

            //textBoxAddr0.Text = "00";
            //textBoxAddr1.Text = "00";
            //textBoxAddr2.Text = "00";
            //textBoxAddr3.Text = "00";

            buttonReadBlockDecimal_Click(null, null);

            ExtLog.AddLine("Boot Code Jump: 0x" + (read4ByteFromPackedStruct(dataBlock, 0x0000) & 0x00FFFFFF).ToString("X4"));
            ExtLog.AddLine("OEM ID: " + readStringFromDataBlock(dataBlock, 0x0003, 8));
            ExtLog.AddLine("Bytes per Sector: " + read2ByteFromPackedStruct(dataBlock, 0x000B));
            ExtLog.AddLine("Sectors per Cluster: " + read1ByteFromPackedStruct(dataBlock, 0x000D));
            ExtLog.AddLine("Reserved Sectors from volume start: " + read2ByteFromPackedStruct(dataBlock, 0x000E));
            ExtLog.AddLine("Nb FAT copies: " + read1ByteFromPackedStruct(dataBlock, 0x0010));
            ExtLog.AddLine("Nb Max root entries: " + read2ByteFromPackedStruct(dataBlock, 0x0011));
            ExtLog.AddLine("Nb sectors (Small vol.): " + read2ByteFromPackedStruct(dataBlock, 0x0013));
            ExtLog.AddLine("Media Descriptor: 0x" + read1ByteFromPackedStruct(dataBlock, 0x0015).ToString("X2"));
            ExtLog.AddLine("Sectors per FAT: " + read2ByteFromPackedStruct(dataBlock, 0x0016));
            ExtLog.AddLine("Sectors per track: " + read2ByteFromPackedStruct(dataBlock, 0x0018));
            ExtLog.AddLine("Nb heads: " + read2ByteFromPackedStruct(dataBlock, 0x001A));
            ExtLog.AddLine("Nb hidden sectors: " + read4ByteFromPackedStruct(dataBlock, 0x001C));
            ExtLog.AddLine("Nb sectors (Large vol.): " + read4ByteFromPackedStruct(dataBlock, 0x0020));
            ExtLog.AddLine("Drive number: 0x" + read1ByteFromPackedStruct(dataBlock, 0x0024).ToString("X2"));
            ExtLog.AddLine("Reserved (NTFS): " + read1ByteFromPackedStruct(dataBlock, 0x0025));
            ExtLog.AddLine("Extended boot signature (0x29 == true): 0x" + read1ByteFromPackedStruct(dataBlock, 0x0026).ToString("X2"));
            ExtLog.AddLine("Volume Serial Number: 0x" + read4ByteFromPackedStruct(dataBlock, 0x0027).ToString("X4"));
            ExtLog.AddLine("Volume Label: " + readStringFromDataBlock(dataBlock, 0x002B, 11));
            ExtLog.AddLine("File System Type: " + readStringFromDataBlock(dataBlock, 0x0036, 8));
            ExtLog.AddLine("Boot Executable Marker: 0x" + read2ByteFromPackedStruct(dataBlock, 0x1FE).ToString("X4"));


            var b_per_sect = read2ByteFromPackedStruct(dataBlock, 0x000B);
            var sect_per_cluster = read1ByteFromPackedStruct(dataBlock, 0x000D);
            var nb_FAT = read1ByteFromPackedStruct(dataBlock, 0x0010);
            var sect_per_FAT = read2ByteFromPackedStruct(dataBlock, 0x0016);
            var nb_res_sect = read2ByteFromPackedStruct(dataBlock, 0x000E);
            var nb_sect_hidden = read4ByteFromPackedStruct(dataBlock, 0x001C);
            var nb_entries_for_root = read2ByteFromPackedStruct(dataBlock, 0x0011);
            var extended_entries = read1ByteFromPackedStruct(dataBlock, 0x0026) == 0x29;

            var start_sector_volume = nb_sect_hidden;
            ExtLog.AddLine("Start Sector for this volume: " + start_sector_volume);
            var start_sector_FAT = start_sector_volume + nb_res_sect;
            ExtLog.AddLine("Start Sector for first FAT: " + start_sector_FAT);
            var start_sector_root = start_sector_FAT + (nb_FAT * sect_per_FAT);
            ExtLog.AddLine("Start Sector for root: " + start_sector_root);
            var byte_per_dir_entries = 32;
            if (b_per_sect != 0)
            {
                var start_sector_data = start_sector_root + ((nb_entries_for_root * byte_per_dir_entries) / b_per_sect);
                ExtLog.AddLine("Start Sector for this volume's data: " + start_sector_data);
            }
        }

        private string readStringFromDataBlock(byte[] data, int offset, int length)
        {
            string res = "";
            for (int i = offset; i < (offset + length); i++)
            {
                res += safeCharConvert(data[i]);// Convert.ToChar(data[i]);
            }
            return res;
        }

        private void buttonReadFAT32Sector_Click(object sender, EventArgs e)
        {
        //    textBoxReadBlockDec.Text = textBoxFATsector.Text;
            buttonReadBlockDecimal_Click(null, null);

            ExtLog.AddLine("Boot Code Jump: 0x" + (read4ByteFromPackedStruct(dataBlock, 0x0000) & 0x00FFFFFF).ToString("X4"));
            ExtLog.AddLine("OEM ID: " + readStringFromDataBlock(dataBlock, 0x0003, 8));
            ExtLog.AddLine("Bytes per Sector: " + read2ByteFromPackedStruct(dataBlock, 0x000B));
            ExtLog.AddLine("Sectors per Cluster: " + read1ByteFromPackedStruct(dataBlock, 0x000D));
            ExtLog.AddLine("Reserved Sectors from volume start: " + read2ByteFromPackedStruct(dataBlock, 0x000E));
            ExtLog.AddLine("Nb FAT copies: " + read1ByteFromPackedStruct(dataBlock, 0x0010));
            ExtLog.AddLine("Media Descriptor: 0x" + read1ByteFromPackedStruct(dataBlock, 0x0015).ToString("X2"));
            ExtLog.AddLine("Sectors per track: " + read2ByteFromPackedStruct(dataBlock, 0x0018));
            ExtLog.AddLine("Nb heads: " + read2ByteFromPackedStruct(dataBlock, 0x001A));
            ExtLog.AddLine("Nb hidden sectors: " + read4ByteFromPackedStruct(dataBlock, 0x001C));
            ExtLog.AddLine("Nb sectors (Large vol.): " + read4ByteFromPackedStruct(dataBlock, 0x0020));

            ExtLog.AddLine("Sectors per FAT: " + read4ByteFromPackedStruct(dataBlock, 0x0024));
            ExtLog.AddLine("FAT handling Flags: 0x" + read2ByteFromPackedStruct(dataBlock, 0x0028).ToString("X4"));
            ExtLog.AddLine("FAT version: 0x" + read2ByteFromPackedStruct(dataBlock, 0x002A).ToString("X4"));
            ExtLog.AddLine("Cluster Nb for root: " + read4ByteFromPackedStruct(dataBlock, 0x002C));
            ExtLog.AddLine("File Sys Info Sector offset: " + read2ByteFromPackedStruct(dataBlock, 0x0030));
            ExtLog.AddLine("Backup boot sect. sect. offset: " + read2ByteFromPackedStruct(dataBlock, 0x0032));
            ExtLog.AddLine("Logical Drive Number: 0x" + read1ByteFromPackedStruct(dataBlock, 0x0040).ToString("X2"));
            ExtLog.AddLine("Current Head: " + read1ByteFromPackedStruct(dataBlock, 0x0041));
            ExtLog.AddLine("Signature (0x28 or 0x29) 0x: " + read1ByteFromPackedStruct(dataBlock, 0x0042).ToString("X2"));
            ExtLog.AddLine("Volume Serial Number: 0x" + read4ByteFromPackedStruct(dataBlock, 0x0043).ToString("X4"));
            ExtLog.AddLine("Volume Label: " + readStringFromDataBlock(dataBlock, 0x0047, 11));
            ExtLog.AddLine("File System Type / ID: " + readStringFromDataBlock(dataBlock, 0x0052, 8));
            ExtLog.AddLine("Boot Executable Marker: 0x" + read2ByteFromPackedStruct(dataBlock, 0x1FE).ToString("X4"));
        }

        private void buttonReadFATDirectory(object sender, EventArgs e)
        {
            buttonReadBlockDecimal_Click(null, null);

            for (int i = 0; i < 512 / 32; i++)
            {
                ExtLog.AddLine("Entry: " + i);
                var b0 = read1ByteFromPackedStruct(dataBlock, (i * 32) + 0x00);
                if (b0 == 0x00) {
                    ExtLog.AddLine("Last Entry.");
                }
                else if (b0 == 0xE5) {
                    ExtLog.AddLine("Free Entry.");
                } else {
                   var flags = read1ByteFromPackedStruct(dataBlock, (i * 32) + 0x0B);
                    if (flags == 0x0F)
                    {
                        ExtLog.AddLine("LFN Entry:");
                        if (SignalGenerator.GetBit(b0, 7)) ExtLog.AddLine("Deleted.");
                        if (SignalGenerator.GetBit(b0, 6)) ExtLog.AddLine("Last.");

                        ExtLog.AddLine("Ord: " + (b0 & 0x3F));
                        ExtLog.AddLine("Data: " + decodeWideCharDirEntry(dataBlock, (i * 32)));



                    } else if ( SignalGenerator.GetBit(flags, 3))
                    {
                        ExtLog.AddLine("Volume Name: " + readStringFromDataBlock(dataBlock, (i * 32) + 0x00, 11));
                    } else
                    {
                        ExtLog.AddLine("Name: " + readStringFromDataBlock(dataBlock, (i * 32) + 0x00, 8));
                        ExtLog.AddLine("Extension: " + readStringFromDataBlock(dataBlock, (i * 32) + 0x08, 3));
                        ExtLog.AddLine("Attributes: " + decodeFATFlags(flags));

                        ExtLog.AddLine("Creation Time 100th of sec: " + read1ByteFromPackedStruct(dataBlock, (i * 32) + 0x0D));
                        ExtLog.AddLine("Creation Time: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x0E).ToString("X4"));
                        ExtLog.AddLine("Creation Date: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x10).ToString("X4"));
                        ExtLog.AddLine("Last Access Date: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x12).ToString("X4"));
                        ExtLog.AddLine("Last Write Time: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x16).ToString("X4"));
                        ExtLog.AddLine("Last Write Date: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x18).ToString("X4"));

                        ExtLog.AddLine("Starting Cluster: " + read2ByteFromPackedStruct(dataBlock, (i * 32) + 0x1A));
                        ExtLog.AddLine("File Size: " + read4ByteFromPackedStruct(dataBlock, (i * 32) + 0x1C));

                    }
                }
            }
        }

        private string decodeFATFlags(byte flags)
        {
            var sb = new StringBuilder();

            if (SignalGenerator.GetBit(flags, 0)) sb.Append("ReadOnly ");
            if (SignalGenerator.GetBit(flags, 1)) sb.Append("Hidden ");
            if (SignalGenerator.GetBit(flags, 2)) sb.Append("System ");
            if (SignalGenerator.GetBit(flags, 3)) sb.Append("VolumeName ");
            if (SignalGenerator.GetBit(flags, 4)) sb.Append("Directory ");
            if (SignalGenerator.GetBit(flags, 5)) sb.Append("Archive ");

            return sb.ToString();
        }

        private string decodeWideCharDirEntry(byte[] data, int offset)
        {
            var sb = new StringBuilder();

            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x01)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x03)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x05)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x07)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x09)));

            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x0E)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x10)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x12)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x14)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x16)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x18)));

            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x1C)));
            sb.Append(safeCharConvertW(read2ByteFromPackedStruct(data, offset + 0x1E)));

            return sb.ToString();

        }

        private void button13_Click(object sender, EventArgs e)
        {
            usb.ChangeCS(false);


            ExtLog.AddLine("Read OCR (CMD58)");
            sendCmd(58, 0, 0, 0, 0, 117);
            var cmdR7 = getResponseR3R7(16);
            ExtLog.AddLine(formatR3R7(cmdR7));
            ExtLog.AddLine( SignalGenerator.GetBit(cmdR7[1], 7) ? "Power Up: OK" : "Power Up: Busy");
            ExtLog.AddLine( SignalGenerator.GetBit(cmdR7[1], 6) ? "CCS: HC" : "CCS: SC");

            var sb = new StringBuilder();
            sb.Append("Voltages: ");
            if (SignalGenerator.GetBit(cmdR7[3], 7)) sb.Append("2.7-2.8 ");
            if (SignalGenerator.GetBit(cmdR7[2], 0)) sb.Append("2.8-2.9 ");
            if (SignalGenerator.GetBit(cmdR7[2], 1)) sb.Append("2.9-3.0 ");

            if (SignalGenerator.GetBit(cmdR7[2], 2)) sb.Append("3.0-3.1 ");
            if (SignalGenerator.GetBit(cmdR7[2], 3)) sb.Append("3.1-3.2 ");
            if (SignalGenerator.GetBit(cmdR7[2], 4)) sb.Append("3.2-3.3 ");

            if (SignalGenerator.GetBit(cmdR7[2], 5)) sb.Append("3.3-3.4 ");
            if (SignalGenerator.GetBit(cmdR7[2], 6)) sb.Append("3.4-3.5 ");
            if (SignalGenerator.GetBit(cmdR7[2], 7)) sb.Append("3.5-3.6 ");

            ExtLog.AddLine(sb.ToString());

            usb.ChangeCS(true);
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
