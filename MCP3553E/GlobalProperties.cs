using System;
using System.Windows.Forms;

namespace MCP3553E
{
    static class GlobalProperties
    {
        //Interface config
        public static uint baudRate = 600;
        public static byte portDirectionMask = 253;//253 = 0xFD = b'11111101'


    }
}
