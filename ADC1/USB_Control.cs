﻿using System.Collections.Generic;
using FTD2XX_NET;

namespace ADC1
{
    public class USB_Control
    {
        private FTDI USB_Interface = new FTDI();
        public byte[] OutputBuffer = new byte[64];
        public byte[] InputBuffer { get; set; }
        public int dataSize { get; private set; }
        public bool IsOpen { get { return USB_Interface.IsOpen; } }
        
        public USB_Control() 
        {
            InputBuffer  = new byte[64];
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

            ExtLog.AddLine("Setting default bauld rate (" + (3000000) + ")");
            ftStatus = USB_Interface.SetBaudRate(3000000);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to set Baud rate (error " + ftStatus + ")");
                return false;
            }

            ExtLog.AddLine("Setting BitMode");
            ftStatus = USB_Interface.SetBitMode(0, FTDI.FT_BIT_MODES.FT_BIT_MODE_RESET);
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

        public void CloseDevice()
        {
            if (USB_Interface.IsOpen) USB_Interface.Close();
        }

        /*  public void Transfer()
          {
              if (USB_Interface.IsOpen)
              {
                  dataSize = SignalGenerator.Serialize(ref OutputBuffer);
                  //USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);
                  if (SendToUSB())
                  {
                      SignalGenerator.Deserialize(InputBuffer);
                  }

              }

          }*/

        /*  private bool SendToUSB()
          {
              var result = true;
              uint res = 0;
              var ftStatus = USB_Interface.Write(OutputBuffer, dataSize, ref res);
              if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
              {
                  USB_Interface.Close();
                  ExtLog.AddLine(string.Format("Failed to write data (error {0}) ({1}/{2})", ftStatus, res, dataSize));
                  result = false;
              }
              else
              {
                  ftStatus = USB_Interface.Read(InputBuffer, (uint)dataSize, ref res);
                  if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
                  {
                      USB_Interface.Close();
                      ExtLog.AddLine(string.Format("Failed to write data (error {0}) ({1}/{2})", ftStatus, res, dataSize));
                      result = false;
                  }
              }
              return result;
          }*/

        public uint ReadUSB(byte[] buffer, uint bsize)
        {
            uint result = 0;
            uint res = 0;

            USB_Interface.GetRxBytesAvailable(ref res);
            if (res < 1) return 0;

            var ftStatus = USB_Interface.Read(buffer, (bsize < res) ? bsize : res, ref result);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                USB_Interface.Close();
                ExtLog.AddLine(string.Format("Failed to read data (error {0})", ftStatus));
                return 0;
            }

            return result;
        }


    }
}
