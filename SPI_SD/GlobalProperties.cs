using System;
using System.Windows.Forms;

namespace SPI_SD
{
    static class GlobalProperties
    {
        //Interface config
        public static uint baudRate = 14400;
        public static byte portDirectionMask = 23;// 23 = 0x17 = b'00010111'
        // 0001 0111 
        // FTDI pinout
        // B0 Output 3.3v enable
        // B1 Clock
        // B2 Master Data Out Slave Data In
        // B3 Master Data In  Slave Data Out
        // B4 CS
        // B5
        // B6
        // B7

        // TXE
        // RXF (del)
        // WR# clock source
        // RD# clock source
        // 3v3 SD VDD

        // SD Card pinout
        // SD / SPI
        // x Key
        // 1 DAT2 / NC
        // 2 DAT3 / CS
        // 3 CMD  / DI
        // 4 VDD  / VDD
        // 5 CLK  / SCLK
        // 6 VSS  / VSS
        // 7 DAT0 / DO
        // 8 DAT1 / NC

        // 48 bit data frames
        // MSB first
        // 01 start bits

    }
}
