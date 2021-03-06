﻿using System.Collections.Generic;
using FTD2XX_NET;

namespace SPI_SD
{
    public class USB_Control
    {
        private FTDI USB_Interface = new FTDI();
        public byte[] RawOutputBuffer = new byte[65535 * 2]; // max 64k per messages
        public byte[] RawInputBuffer = new byte[65535 * 2];
        public int dataSize { get; private set; }
        public bool IsOpen => USB_Interface.IsOpen;

        public USB_Control() 
        {
          //  InputBuffer  = new byte[OutputBuffer.Length];
         //   var x = OutputBuffer.Length
        }

        public List<string> GetDevicesList()
        {
            var result = new List<string>();

            uint numUI = 0;
            var ftStatus = USB_Interface.GetNumberOfDevices(ref numUI);
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                if (numUI > 0)
                {
                    ExtLog.AddLine(numUI.ToString("D") + " Devices Found.");

                    var ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[numUI];
                    ftStatus = USB_Interface.GetDeviceList(ftdiDeviceList);
                   
                    if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    {
                        for (var i = 0; i < numUI; i++)
                        {
                            result.Add(ftdiDeviceList[i].LocId.ToString("D4") + ":" + ftdiDeviceList[i].Description);
                        }
                    }
                    else
                    {
                        ExtLog.AddLine("Failed to list devices (error " + ftStatus + ")");
                    }
                }
                else ExtLog.AddLine("No devices found.");
            }
            else
            {
                ExtLog.AddLine("Failed to get number of devices (error " + ftStatus + ")");
            }
            return result;
        }

        public bool OpenDeviceByLocation(uint LocationID)
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();

            ExtLog.AddLine("Opening device");
            var ftStatus = USB_Interface.OpenByLocation(LocationID);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to open device (error " + ftStatus + ")");
                return false;
            }

            ExtLog.AddLine("Setting default bauld rate (" + GlobalProperties.baudRate + ")");
            ftStatus = USB_Interface.SetBaudRate(GlobalProperties.baudRate);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to set Baud rate (error " + ftStatus + ")");
                return false;
            }

            ExtLog.AddLine("Setting BitMode");
            //     ftStatus = USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_FIFO);
            ftStatus = USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to set BitMode (error " + ftStatus + ")");
                return false;
            }

            byte latency = 0;
            ftStatus = USB_Interface.GetLatency(ref latency);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to get Latency (error " + ftStatus + ")");
            } else ExtLog.AddLine("Latency: " + latency);
            

            return true;
        }

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

        public void CloseDevice()
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();
        }

        public void Transfer()
        {
            if (USB_Interface.IsOpen)
            {
                dataSize = SignalGenerator.Serialize(ref RawOutputBuffer);
                //USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);

                if ((dataSize > 0) && (SendAndReadUSBSync() ))
                {
                    SignalGenerator.Deserialize(RawInputBuffer, dataSize);
                }
 
            }

        }

        private bool SendAndReadUSBSync()
        {
            var result = true;
            uint res = 0;
            var ftStatus = USB_Interface.Write(RawOutputBuffer, dataSize, ref res);
            if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
            {
                USB_Interface.Close();
                ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                result = false;
            }
            else
            {
                ftStatus = USB_Interface.Read(RawInputBuffer, (uint)dataSize, ref res);
                if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
                {
                    USB_Interface.Close();
                    ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                    result = false;
                }
           //     USB_Interface.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            }
            return result;
        }

        public byte ChangeCS(bool CS)
        {
            SignalGenerator.SPI_CS = CS;
         //   SignalGenerator.purgeLeading1s();
            return BitBang(SignalGenerator.genByte(true, true));
        }

        public byte BitBang(byte data)
        {
            uint res = 0;
            RawOutputBuffer[0] = data;
            var ftStatus = USB_Interface.Write(RawOutputBuffer, 1, ref res);
            if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != 1))
            {
                USB_Interface.Close();
                ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/1)");
            }
            else
            {
                ftStatus = USB_Interface.Read(RawInputBuffer, 1, ref res);
                if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != 1))
                {
                    USB_Interface.Close();
                    ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/1)");
                }
              //  USB_Interface.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            }
            return RawInputBuffer[0];
        }



    }
}
