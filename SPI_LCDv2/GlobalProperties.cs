using System;
using System.Windows.Forms;

namespace SPI_LCDv2
{
    static class GlobalProperties
    {
        //Interface config
        public static uint baudRate = 9600;//14400;
        public static byte portDirectionMask = 0xFF; // all outputs
        public static byte latency = 2;
        // FTDI pinout
        // B0 Output 3.3v enable
        // B1 Clock
        // B2 Master Data Out Slave Data In
        // B3 Master Data In  Slave Data Out
        // B4 CS
        // B5
        // B6
        // B7
    }
}
