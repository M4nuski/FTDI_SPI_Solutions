using System.Windows.Forms;

namespace MCP3553E
{
    static class SignalGenerator
    {
        // SPI version directly to FTDI UM245R
        //Interface
        public static byte[] OutputBytes = new byte[3200];

        public static int InputInt;

        public const int SPI_SDA_bit = 1; // Input

        private const int SPI_SCK_bit = 2; // latch on clk up
        
        private const int SPI_CS_bit = 0;
        private const bool SPI_CS_default = true;


        private static int _buffer_index;

        private static uint[] Powers =
        {
            0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080,
            0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000,
            0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
            0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000
        };

        public static bool GetBit(byte data, int bit)
        {
            return ((data & Powers[bit]) > 0);
        }

        public static int SetBit(int input, int bitToSet, bool set)
        {
            return (input | (int)(set ? Powers[bitToSet] : 0));
        }
        public static byte SetBit(byte input, int bitToSet, bool set)
        {
            return (byte)(input | (set ? Powers[bitToSet] : 0));
        }




        public static int Serialize(ref byte[] buffer)
        {
            //reset index
            _buffer_index = 0;

            //dummy bit to set CS low
            buffer[_buffer_index] = genByte(true, !SPI_CS_default);


            for (var i = 0; i < 24; i++) //gen clock
            {
                buffer[_buffer_index] = genByte(false, !SPI_CS_default);
                buffer[_buffer_index] = genByte(true, !SPI_CS_default);
            }

            //dummy bit to read last byte and set CS high
            //todo add lock for continuous pooling
            buffer[_buffer_index] = genByte(true, SPI_CS_default);

            return _buffer_index;
        }

        public static void Deserialize(byte[] buffer)
        {
            //TODO
            //clocked out on falling edge

            InputInt = 0;
            for (var i = 0; i < 24; i++) // MSB first
            {
                InputInt = SetBit(InputInt, 23-i, GetBit(buffer[3+(i*2)], SPI_SDA_bit));
            }
        }

        public static byte genByte(bool clock, bool cs)
        {
            byte result = 0;

            result = SetBit(result, SPI_CS_bit, cs);

            result = SetBit(result, SPI_SCK_bit, clock);

            _buffer_index++;

            return result;
        }


    }
}
