using System.Windows.Forms;

namespace SPI_TFT
{
    static class SignalGenerator
    {
        // SPI version directly to FTDI UM245R
        //Interface
        public static byte[] OutputBytes = new byte[65535];
        public static int OutputLength;
        private const int SPI_SDA_bit = 2;
        private const int SPI_SCK_bit = 3; // latch on clk up

        private const int SPI_RESET_bit = 0;
        public const bool SPI_RESET_default = true;
        public static bool SPI_Reset = SPI_RESET_default;

        private const int SPI_A0_bit = 1; // 0:cmd 1:data
        public const bool SPI_A0_command = false;
        public const bool SPI_A0_data = true;
        public static bool SPI_A0 = SPI_A0_data;


        private const int SPI_CS_bit = 4;
        private const bool SPI_CS_default = true;


        private static int _buffer_index;

        private static byte[] Powers = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };

        public static bool GetBit(byte data, int bit)
        {
            return ((data & Powers[bit]) > 0);
        }

        public static byte SetBit(byte input, int bitToSet, bool set)
        {
            return (byte)(input | (set ? Powers[bitToSet] : 0));
        }




        public static int Serialize(ref byte[] buffer)
        {
            if ((OutputLength <= 65535) && (OutputLength > 0))
            {
                //reset index
                _buffer_index = 0;

                //dummy bit to set CS low
                buffer[_buffer_index] = genByte(false, false, !SPI_CS_default);

                for (var d = 0; d < OutputLength; d++)
                {

                    for (var i = 7; i >= 0; i--) // MSB first
                    {
                        buffer[_buffer_index] = genByte(GetBit(OutputBytes[d], i), false, !SPI_CS_default);
                        buffer[_buffer_index] = genByte(GetBit(OutputBytes[d], i), true, !SPI_CS_default);
                    }
                }

                //dummy bit to set CS high
                buffer[_buffer_index] = genByte(false, false, SPI_CS_default);
                OutputLength = 0;
                return _buffer_index;
            }
            return 0;
        }

        public static void Deserialize(byte[] buffer)
        {
            //unused yet.
        }

        private static byte genByte(bool data, bool clock, bool cs)
        {
            byte result = 0;

            result = SetBit(result, SPI_SCK_bit, clock);

            result = SetBit(result, SPI_RESET_bit, SPI_Reset);
            result = SetBit(result, SPI_A0_bit, SPI_A0);
            result = SetBit(result, SPI_CS_bit, cs);

            result = SetBit(result, SPI_SDA_bit, data);


            _buffer_index++;

            return result;
        }


    }
}
