using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using System.Threading;

namespace LogicScope
{   
    public class USB_Control
    {
        public FTDI USB_Interface = new FTDI();
        public const int MAX_TX_CHUNK = 64;
        public int MAX_TX_TIMEOUT = 1000;
        public bool IsOpen => USB_Interface.IsOpen;

        public USB_Control()
        {
            //InputBuffer = new byte[64];
        }

        public List<FTDI.FT_DEVICE_INFO_NODE> GetDevicesList()
        {
            var result = new List<FTDI.FT_DEVICE_INFO_NODE>();

            uint numUI = 0;
            var ftStatus = USB_Interface.GetNumberOfDevices(ref numUI);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to get number of devices (error " + ftStatus + ")");


            if (numUI > 0)
            {
                var ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[numUI];
                ftStatus = USB_Interface.GetDeviceList(ftdiDeviceList);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to list devices (error " + ftStatus + ")");

                for (var i = 0; i < numUI; i++) result.Add(ftdiDeviceList[i]);
            }
            return result;
        }

        public void OpenDeviceByLocation(uint LocationID)
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();

            var ftStatus = USB_Interface.OpenByLocation(LocationID);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus + ")");

            USB_Interface.SetTimeouts(1000, 1000);
        }

        public void OpenDeviceBySerialNumber(string sn)
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();

            var ftStatus = USB_Interface.OpenBySerialNumber(sn);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus + ")");

            USB_Interface.SetTimeouts(1000, 1000);
        }

        public void SetLatency(byte latency)
        {
            var ftStatus = USB_Interface.SetLatency(latency);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to set Latency (error " + ftStatus + ")");
        }

        public byte GetLatency()
        {
            byte latency = 0;
            var ftStatus = USB_Interface.GetLatency(ref latency);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to get Latency (error " + ftStatus + ")");
            return latency;
        }

        public void CloseDevice()
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();
        }

        public void Purge(bool RX, bool TX)
        {
            uint mask = 0;
            if (RX) mask |= FTDI.FT_PURGE.FT_PURGE_RX;
            if (TX) mask |= FTDI.FT_PURGE.FT_PURGE_TX;
            var ftStatus = USB_Interface.Purge(mask);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to purge buffers (error " + ftStatus + ")");
        }

        public int Write(byte[] buffer, int length)
        {
            if (length > buffer.Length) length = buffer.Length;

           // int startTime = Environment.TickCount;


            //if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to change transfer size (error " + ftStatus + ")");
            //uint txWaiting = 0;
           // USB_Interface.GetTxBytesWaiting(ref txWaiting);
           /* Console.WriteLine("a " + txWaiting);
            while (txWaiting != 0) {
                Thread.Sleep(1);
                USB_Interface.GetTxBytesWaiting(ref txWaiting);
                if ((Environment.TickCount - startTime) > MAX_TX_TIMEOUT) throw new Exception("Failed to write device (timeout exceeded)");
                Console.WriteLine("a loop " + txWaiting);
            }
            //if (txWaiting != 0) Console.WriteLine(txWaiting);
           */
           // if (length <= MAX_TX_CHUNK)
          //  {
                uint written = 0;
                var ftStatus = USB_Interface.Write(buffer, length, ref written);
                if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to write device (error " + ftStatus + ")");
                return (int)written;
         /*   } else
            {

                int cursor = 0;
                uint written = 0;
                uint totalWritten = 0;
                byte[] subBuffer = new byte[MAX_TX_CHUNK];
                while (cursor < length) {
                    Array.Copy(buffer, cursor, subBuffer, 0, (MAX_TX_CHUNK <= (length - cursor)) ? MAX_TX_CHUNK : (length - cursor));
                    cursor += MAX_TX_CHUNK;
                    ftStatus = USB_Interface.Write(subBuffer, MAX_TX_CHUNK, ref written);
                    if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to write device (error " + ftStatus + ")");
                    totalWritten += written;
                    Thread.Sleep(1);
                    USB_Interface.GetTxBytesWaiting(ref txWaiting);
                    Console.WriteLine("b " + txWaiting);
                    while (txWaiting != 0)
                    {
                        USB_Interface.GetTxBytesWaiting(ref txWaiting);
                        if ((Environment.TickCount - startTime) > MAX_TX_TIMEOUT) throw new Exception("Failed to write device (timeout exceeded)");
                        Console.WriteLine("b loop " + txWaiting);
                    }
                }
                return (int)totalWritten;
            }*/
        }
        public int Read(byte[] buffer, int length)
        {
            uint rxAvail = 0;
            USB_Interface.GetRxBytesAvailable(ref rxAvail);
            if (rxAvail == 0) return 0;
            uint readed = 0;
            var ftStatus = USB_Interface.Read(buffer, (uint)length, ref readed);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to read device (error " + ftStatus + ")");
            return (int)readed;
        }

        public void WriteString(string s)
        {
            // TODO with encoding and such
        }
        public void ReadString(ref string s) 
        {
            // TODO with encoding and such
        }
        public int GetRXbytesAvailable()
        {
            uint rxAvail = 0;
            var ftStatus = USB_Interface.GetRxBytesAvailable(ref rxAvail);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to get TX buffer available bytes (error " + ftStatus + ")");
            return (int)rxAvail;
        }
        public int GetTXbytesWaiting()
        {
            uint txWaiting = 0;
            var ftStatus = USB_Interface.GetTxBytesWaiting(ref txWaiting);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to get TX buffer waiting bytes (error " + ftStatus + ")");
            return (int)txWaiting;
        }




    }
}
