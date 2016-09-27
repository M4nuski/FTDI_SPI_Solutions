using System;
using System.Windows.Forms;

namespace USB_Interface_v2_Test
{
    static class GlobalProperties
    {
        //Interface config
        public static uint baudRate = 3000000;
        public static byte portDirectionMask = 250;//250 = 0xFA = b'11111010' = out out out out  out in out in

        //UI refresh throttler
        public static int USB_Refresh_Period = 250;
    }
}
