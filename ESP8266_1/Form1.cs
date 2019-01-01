using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using FTD2XX_NET;
using M4nuskomponents;

namespace ESP8266_1
{



    public partial class Form1 : Form
    {
        private USB_Control ctrl;
        public usbInterfaceProperty GlobalProperties = new usbInterfaceProperty();
        private string[] hexToBin = new string[16];
        private StringBuilder sb = new StringBuilder();

        private string lastStump = "";

        public Form1()
        {
            InitializeComponent();
            GlobalProperties.baudRate = 115000;
            GlobalProperties.portDirectionMask = 0;
            GlobalProperties.portMode = FTDI.FT_BIT_MODES.FT_BIT_MODE_RESET;

            for (var i = 0; i < 16; i++)
            {
                hexToBin[i] = Convert.ToString(i, 2).PadLeft(4, '0');
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var devId = "";
            ctrl = new USB_Control(ExtLog, GlobalProperties);
            foreach (var dev in ctrl.GetDevicesList())
            {
                ExtLog.AddLine(dev);
                devId = dev.Split(':')[0];

            }
            if (devId != "")
            {
                ctrl.OpenDeviceByLocation((uint) int.Parse(devId));
                checkBox1.Enabled = true;
                ctrl.SetLatency(2);
                ctrl.purgeDevice();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ctrl?.CloseDevice();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          //  ctrl.thdWaitHandle.WaitOne(50);
            if (checkBox1.Checked)
            {

                var numBytes = ctrl.FIFObyte;

                ExtLog.AddLine(numBytes.ToString("D"));

                if (numBytes > 0)
                {
                    var buffer = new byte[65536];

                    var nread = ctrl.readFIFO(buffer, numBytes, 65536);
                    if (nread > 0)
                    {
                        ExtLog.Enabled = false;

                        var index = 0;
                        for (var i = 0; i < nread; i++)
                        {
                            sb.Clear();
                            for (var j = 0; j < Math.Min(64, nread); j++)
                            {

                                // str += buffer[index].ToString("X2");
                                sb.Append(buffer[index] == 255 ? "1" : "0");
                                index++;
                            }

                            ExtLog.AddLine(sb.ToString());

                            i += 64;
                        }
                        ExtLog.Enabled = true;
                    }
                }
            }
            else
            {
                ctrl?.purgeDevice();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExtLog.Clear();
        }



        private static int safeHexParse(string s)
        {
            var result = 0;
            int.TryParse(s, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out result);
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var indx = 0;
            var txt = textBox1.Text;
            while ((indx < txt.Length) && (txt[indx] == 'F'))
            { 
                indx ++;
            }
            if (indx < txt.Length)
                textBox1.Text += "\n";

            {
                for (var i = indx; i < txt.Length; i = i + 2)
                {
                   // var val = safeHexParse(""+txt[i+1]);
                    textBox1.Text += (txt[i] == 'F') ? "1" : "0"; //hexToBin[val];
                }
            }
        }
    }


    public static class UART_Decode
    {
        public static byte[] b0InputToBinary(byte[] buffer, int len, int bit = 0)
        {
            //state : inRun, lastItem, currentIndex, uartMode

            //check if 1, 
            //if not assume resume from last state
            //find first 0 block start bit
            //decode until end of buffer

            //lastStump
            var start = "0";
            var stop = "1";
            var stumpLength = 4;

            checkOffset("");
            return new byte[1];
        }

        public static Tuple<byte, int> checkOffset(string stump)
        {
            if (stump == "1111") return new Tuple<byte, int>(1, 0); //result, pre offset, post-offset
            if (stump == "0000") return new Tuple<byte, int>(0, 0);

            if (stump == "1110") return new Tuple<byte, int>(1, -1);
            if (stump == "0111") return new Tuple<byte, int>(1, 1);

            if (stump == "1000") return new Tuple<byte, int>(0, 1);
            if (stump == "0001") return new Tuple<byte, int>(0, -1);

            return new Tuple<byte, int>(255, 255);

        }

    }


    public class USB_Control
    {
        private FTDI USB_Interface = new FTDI();
        public byte[] OutputBuffer = new byte[65535 * 20];
        public byte[] InputBuffer { get; set; }
        public int dataSize { get; private set; }
        public bool IsOpen => USB_Interface.IsOpen;
        private Logger ExtLog;
        private usbInterfaceProperty interfaceProperties;

        public void purgeDevice()
        {
            if (USB_Interface.IsOpen) USB_Interface.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
        }

        public int FIFObyte
        {
            get
            {
                uint rxBytes = 0;
                USB_Interface.GetRxBytesAvailable(ref rxBytes);
                return (int)rxBytes;
            }
        }

        public EventWaitHandle thdWaitHandle;

        public int readFIFO(byte[] buffer, int toRead,  int buflen)
        {
            uint bread = 0;
            var status = USB_Interface.Read(buffer, (uint) toRead, ref bread);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Error reading FIFO (error " + status + ")");
                bread = 0;
            }
            return (int)bread;

        }

        public USB_Control(Logger logger, usbInterfaceProperty properties)
        {
            InputBuffer = new byte[64];
            ExtLog = logger;
            interfaceProperties = properties;

            thdWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            USB_Interface.SetEventNotification(FTDI.FT_EVENTS.FT_EVENT_RXCHAR, thdWaitHandle);
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

            ExtLog.AddLine("Setting default bauld rate (" + interfaceProperties.baudRate + ")");
            ftStatus = USB_Interface.SetBaudRate(interfaceProperties.baudRate);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                ExtLog.AddLine("Failed to set Baud rate (error " + ftStatus + ")");
                return false;
            }

            ExtLog.AddLine("Setting BitMode");
            ftStatus = USB_Interface.SetBitMode(interfaceProperties.portDirectionMask, interfaceProperties.portMode);
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
            }
            else ExtLog.AddLine("Latency: " + latency);


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
             //   dataSize = SignalGenerator.Serialize(ref OutputBuffer);
                //USB_Interface.SetBitMode(GlobalProperties.portDirectionMask, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);

                if ((dataSize > 0) && (SendToUSB()))
                {
               //     SignalGenerator.Deserialize(InputBuffer);
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
                ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                result = false;
            }
            else
            {
                ftStatus = USB_Interface.Read(InputBuffer, (uint)dataSize, ref res);
                if ((ftStatus != FTDI.FT_STATUS.FT_OK) && (res != dataSize))
                {
                    USB_Interface.Close();
                    ExtLog.AddLine($"Failed to write data (error {ftStatus}) ({res}/{dataSize})");
                    result = false;
                }
                USB_Interface.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            }
            return result;
        }

    }
    public class usbInterfaceProperty
    {
        public uint baudRate { get; set; }
        public byte portDirectionMask;
        public byte portMode;
    };

}
