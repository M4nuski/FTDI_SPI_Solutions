using System.Windows.Forms;

namespace SPI_LCDv2
{
    static class SignalGenerator
    {
        // FTDI UM245R to LCD v2.0 Interface (3wire 4bits)

        // B0 Clock to 74LS164
        // B1 E signal directly to LCD module
        // B2 Data to 74LS164

        // Buffers
        public static byte[] OutputDataBytes = new byte[65535];
        public static byte[] InputDataBytes = new byte[65535];
        public static int OutputDataLength = 0;
        public static int InputDataLength = 0;

        // Interface
        private const int LCD_Clock_bit = 0; // low to high latch data
        private const int LCD_E_bit = 1; // latch on clk down, default low
        private const int LCD_Data_bit = 2; // Q0 to Q3 are DB4 to DB7, Q7 is RS, MSB first(q7)

        //private static int _buffer_index = 0;

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


        public static void addByte(bool RS, bool db7, bool db6, bool db5, bool db4)
        {
            OutputDataBytes[OutputDataLength++] = genByte(RS, false, false); //RS
            OutputDataBytes[OutputDataLength++] = genByte(RS, true, false);

            OutputDataBytes[OutputDataLength++] = genByte(db7, false, false); //DB7
            OutputDataBytes[OutputDataLength++] = genByte(db7, true, false);

            OutputDataBytes[OutputDataLength++] = genByte(db6, false, false); //DB6
            OutputDataBytes[OutputDataLength++] = genByte(db6, true, true);

            OutputDataBytes[OutputDataLength++] = genByte(db5, false, true); //DB5
            OutputDataBytes[OutputDataLength++] = genByte(db5, true, true);

            OutputDataBytes[OutputDataLength++] = genByte(db4, false, true); //DB4
            OutputDataBytes[OutputDataLength++] = genByte(db4, true, true);

            OutputDataBytes[OutputDataLength++] = genByte(false, false, true); //strobe E
            OutputDataBytes[OutputDataLength++] = genByte(false, false, false);
        }


        public static int Serialize(ref byte[] SPIDataBuffer)
        {
            for (int i = 0; i < OutputDataLength; i++)
            {
                SPIDataBuffer[i] = OutputDataBytes[i];
            }
           // OutputDataBytes.CopyTo(SPIDataBuffer, 0);
            var res = OutputDataLength;
            OutputDataLength = 0;
            return res;
        }

        public static void Deserialize(byte[] SPIDataBuffer, int buflen)
        {
            InputDataLength = 0;
    //        ExtLog.AddLine("Decoding " + buflen + " data");
            var bitIndex = 0;
            // skip trailing 0s
            //      while ((GetBit(SPIDataBuffer[bitIndex], SPI_MISO_bit) == false) && (bitIndex < buflen)) bitIndex++;
            //    ExtLog.AddLine("Skipped 0s, index at " + bitIndex);


            // ExtLog.AddLine("Skipped idle 1s, index at " + bitIndex);

            var byteIndex = 0;
            // check at least 16/2 bits left to read
            while ((buflen - bitIndex) >= 16)
            {
             //   InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 7, GetBit(SPIDataBuffer[bitIndex+1], SPI_MISO_bit));
              //  InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 6, GetBit(SPIDataBuffer[bitIndex+3], SPI_MISO_bit));
             //   InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 5, GetBit(SPIDataBuffer[bitIndex+5], SPI_MISO_bit));
              //  InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 4, GetBit(SPIDataBuffer[bitIndex+7], SPI_MISO_bit));

            //    InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 3, GetBit(SPIDataBuffer[bitIndex+9], SPI_MISO_bit));
              //  InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 2, GetBit(SPIDataBuffer[bitIndex+11], SPI_MISO_bit));
             //   InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 1, GetBit(SPIDataBuffer[bitIndex+13], SPI_MISO_bit));
               // InputDataBytes[byteIndex] = ChangeBit(InputDataBytes[byteIndex], 0, GetBit(SPIDataBuffer[bitIndex+15], SPI_MISO_bit));

                bitIndex += 16;
                byteIndex++;

             //   // for read 8 only
           //     while ((GetBit(buffer[bitIndex], SPI_MISO_bit) == true) && (bitIndex < buflen)) bitIndex++;
            }
            InputDataLength = byteIndex;
        }

        public static byte genByte(bool data, bool clock, bool E)
        {
            byte result = 0;

            result = ChangeBit(result, LCD_Clock_bit, clock);
            result = ChangeBit(result, LCD_E_bit, E);
            result = ChangeBit(result, LCD_Data_bit, data);

            //_buffer_index++;

            return result;
        }




    }
}
