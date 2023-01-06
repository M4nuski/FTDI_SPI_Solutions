using System.Collections.Generic;
using System;
using FTD2XX_NET;

namespace FT220X_v1
{
    public class USB_Control
    {
        public FTDI USB_Interface = new FTDI();
        public byte[] OutputBuffer = new byte[65535];
        public byte[] InputBuffer { get; set; }
        public int dataSize { get; private set; }
        public bool IsOpen => USB_Interface.IsOpen;

        public USB_Control() 
        {
            InputBuffer  = new byte[64];
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

        public bool OpenDeviceByLocation(uint LocationID)
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();

            var ftStatus = USB_Interface.OpenByLocation(LocationID);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus + ")");

            //ExtLog.AddLine("Setting default bauld rate (" + GlobalProperties.baudRate + ")");
        /*    ftStatus = USB_Interface.SetBaudRate(GlobalProperties.baudRate);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to set Baud rate (error " + ftStatus + ")");
                return false;
            }
          */
           // ExtLog.AddLine("Setting BitMode");
         //   ftStatus = USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG);
         //   if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to set BitMode (error " + ftStatus + ")");

            /*byte latency = 0;
            ftStatus = USB_Interface.GetLatency(ref latency);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to get Latency (error " + ftStatus + ")");
            } else ExtLog.AddLine("Latency: " + latency);
            */

            return true;
        }

        public void OpenDeviceBySerialNumber(string sn)
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();

            var ftStatus = USB_Interface.OpenBySerialNumber(sn);
            if (ftStatus != FTDI.FT_STATUS.FT_OK) throw new Exception("Failed to open device (error " + ftStatus + ")");
        }
            /*
            public bool SetLatency(byte latency)
            {

                var ftStatus = USB_Interface.SetLatency(latency);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    ExtLog.AddLine("Failed to set Latency (error " + ftStatus + ")");
                    return false;
                }
                return true;
            }
            */
            public void CloseDevice()
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();
        }

        public void Transfer()
        {
            if (USB_Interface.IsOpen)
            {
                dataSize = SignalGenerator.Serialize(ref OutputBuffer);
                //USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);

                if ((dataSize > 0) && (SendToUSB() ))
                {
                    SignalGenerator.Deserialize(InputBuffer);
                }
 
            }

        }

        private bool SendToUSB()
        {
            var result = true;
            uint res = 0;
            var ftStatus = USB_Interface.Write(OutputBuffer, dataSize, ref res);
            if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
            {
                USB_Interface.Close();
            //    ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                result = false;
            }
            else
            {
                ftStatus = USB_Interface.Read(InputBuffer, (uint)dataSize, ref res);
                if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
                {
                    USB_Interface.Close();
             //       ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                    result = false;
                }
                USB_Interface.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            }
            return result;
        }

    }
}
