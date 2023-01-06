using System;
using System.Windows.Forms;

namespace FT220X_v1
{
    static class GlobalProperties
    {
        //Interface config
        public static uint baudRate = 3000000;
        public static byte portDirectionMask = 255;//255 = 0xFF = b'11111010' = all outputs

    }
}
