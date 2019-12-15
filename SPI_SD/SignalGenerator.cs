using System.Windows.Forms;

namespace SPI_SD
{
    static class SignalGenerator
    {
        // SPI version directly to FTDI UM245R
        //Interface

        // B0 Output 3.3v enable // connected to 3v3
        // B1 Clock
        // B2 Master Data Out Slave Data In
        // B3 Master Data In  Slave Data Out
        // B4 CS

        // Buffers
        public static byte[] OutputDataBytes = new byte[65535];
        public static byte[] InputDataBytes = new byte[65535];
        public static int OutputDataLength;
        public static int InputDataLength;

        private static bool _purgeLeading1s = true;

        // Interface
        private const int SPI_3v3_bit = 0;
        private const int SPI_SCK_bit = 1; // latch on clk up
        private const int SPI_MOSI_bit = 2;
        private const bool SPI_MOSI_default = true;
        private const int SPI_MISO_bit = 3;
        private const int SPI_CS_bit = 4;

        public static bool SPI_CS = true;
        //   private const bool SPI_CS_default = true;

        //public bool SPI_ChipSelect;

        // cs 0
        // clock 0, data x, then clock 1
        // clock 0, data x+1, clock 1

        // start bits 01
        // index
        // argument
        // crc
        // stop bit 1

        // data in 1 until start bits 01

        private static int _buffer_index;

        private static byte[] Powers = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };

        public static bool GetBit(byte data, int bit)
        {
            return ((data & Powers[bit]) > 0);
        }

        public static byte SetBit(byte input, int bitToSet)
        {
            return (byte)(input | Powers[bitToSet]);
        }

        public static byte ClearBit(byte input, int bitToClear)
        {
            return (byte)(input & ~Powers[bitToClear]);
        }

        public static byte ChangeBit(byte input, int bit, bool value)
        {
            return (value) ? SetBit(input, bit) : ClearBit(input, bit);
        }




        public static int Serialize(ref byte[] SPIDataBuffer)
        {
            if ((OutputDataLength <= 65535) && (OutputDataLength > 0))
            {
                //reset index
                _buffer_index = 0;

                //dummy bit to set CS low
         //       buffer[_buffer_index] = genByte(SPI_MOSI_default, false, CS);// !SPI_CS_default);

                for (var d = 0; d < OutputDataLength; d++)
                {

                    for (var i = 7; i >= 0; i--) // MSB first
                    {
                        SPIDataBuffer[_buffer_index] = genByte(GetBit(OutputDataBytes[d], i), false);// !SPI_CS_default);
                        SPIDataBuffer[_buffer_index] = genByte(GetBit(OutputDataBytes[d], i), true);// !SPI_CS_default);
                    }
                }

                //dummy bit to set CS high
             //   buffer[_buffer_index] = genByte(SPI_MOSI_default, false, CS);// !SPI_CS_default);
                OutputDataLength = 0;
                return _buffer_index;
            }
            return 0;
        }

        public static void purgeLeading1s()
        {
            _purgeLeading1s = true;
        }
        public static void Deserialize(byte[] SPIDataBuffer, int buflen)
        {
            InputDataLength = 0;
    //        ExtLog.AddLine("Decoding " + buflen + " data");
            var bitIndex = 0;
            // skip trailing 0s
            //      while ((GetBit(SPIDataBuffer[bitIndex], SPI_MISO_bit) == false) && (bitIndex < buflen)) bitIndex++;
            //    ExtLog.AddLine("Skipped 0s, index at " + bitIndex);


            // skip idling 1s
            if (_purgeLeading1s)
            {
                while ((GetBit(SPIDataBuffer[bitIndex], SPI_MISO_bit) == true) && (bitIndex < buflen)) bitIndex++;

                _purgeLeading1s = false;
            }
            // ExtLog.AddLine("Skipped idle 1s, index at " + bitIndex);

            var byteIndex = 0;
            // check at least 16/2 bits left to read
            while ((buflen - bitIndex) >= 16)
            {
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 7, GetBit(SPIDataBuffer[bitIndex+1], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 6, GetBit(SPIDataBuffer[bitIndex+3], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 5, GetBit(SPIDataBuffer[bitIndex+5], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 4, GetBit(SPIDataBuffer[bitIndex+7], SPI_MISO_bit));

                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 3, GetBit(SPIDataBuffer[bitIndex+9], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 2, GetBit(SPIDataBuffer[bitIndex+11], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 1, GetBit(SPIDataBuffer[bitIndex+13], SPI_MISO_bit));
                InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 0, GetBit(SPIDataBuffer[bitIndex+15], SPI_MISO_bit));

                bitIndex += 16;
                byteIndex++;

             //   // for read 8 only
           //     while ((GetBit(buffer[bitIndex], SPI_MISO_bit) == true) && (bitIndex < buflen)) bitIndex++;
            }
            InputDataLength = byteIndex;
        }

        public static byte genByte(bool data, bool clock, bool en3v3 = true)
        {
            byte result = 0;

            result = ChangeBit(result, SPI_CS_bit, SPI_CS);
            result = ChangeBit(result, SPI_SCK_bit, clock);
            result = ChangeBit(result, SPI_MOSI_bit, data);
            result = ChangeBit(result, SPI_3v3_bit, en3v3);
            //  result = ChangeBit(result, SPI_RESET_bit, SPI_Reset);
            // result = ChangeBit(result, SPI_A0_bit, SPI_A0);

            _buffer_index++;

            return result;
        }


    }
}
