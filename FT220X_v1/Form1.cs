using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTD2XX_NET;

namespace FT220X_v1
{

    public partial class Form1 : Form
    {
        private USB_Control usb = new USB_Control();
        private string sn;
        private uint locID;
        private uint sumRx = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var dl = usb.GetDevicesList();
                foreach (var d in dl)
                {
                    listBox1.Items.Add($"ID:{d.ID}, Desc:{d.Description}, SN:{d.SerialNumber}, LocID:{d.LocId}");
                    sn = d.SerialNumber;
                    locID = d.LocId;
                }
            } catch (Exception ex)
            {
                AddLine(ex.Message);
            }
        }

        private void AddLine(string text)
        {
            textBox1.AppendText(text +"\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
             //   usb.OpenDeviceBySerialNumber(sn);
                usb.OpenDeviceByLocation(locID);
                textBox1.AppendText("Opened" + "\r\n");
                //      usb.USB_Interface.SetBitMode(0x00, FTDI.FT_BIT_MODES.FT_BIT_MODE_RESET);
                byte latency = 0;
                var ftStatus = usb.USB_Interface.GetLatency(ref latency);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    AddLine("Failed to get Latency (error " + ftStatus + ")");
                }
                else AddLine("Get Latency: " + latency);
                latency = 2;
                ftStatus = usb.USB_Interface.SetLatency(latency);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    AddLine("Failed to set Latency (error " + ftStatus + ")");
                }
                else AddLine("Set Latency: " + latency);

            } catch (Exception ex)
            {
                AddLine(ex.Message);
            }
}

        private void button3_Click(object sender, EventArgs e)
        {
            var nB = 8;
            byte[] buffer = new byte[nB];
            for (byte i = 0; i < nB; ++i) buffer[i] = i;
            uint written = 0;
            //          var ftstatus = usb.USB_Interface.SetBitMode(0xFF, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG);
            var ftstatus = usb.USB_Interface.Write(buffer, nB, ref written);
            AddLine(ftstatus + " " + written);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var nB = 8;
            byte[] buffer = new byte[nB];
            for (byte i = 0; i < nB; ++i) buffer[i] = (byte)~i;
            uint written = 0;
            //         var ftstatus =  usb.USB_Interface.SetBitMode(0x0F, FTDI.FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG);
            var ftstatus = usb.USB_Interface.Write(buffer, nB, ref written);
            AddLine(ftstatus + " " + written);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            usb.CloseDevice();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            uint rxAvail = 666;
            uint txWait = 666;
            const int bLength = 1024 * 64;
            byte[] buffer = new byte[bLength];

            if (usb.IsOpen)
            {
                usb.USB_Interface.GetRxBytesAvailable(ref rxAvail);
                usb.USB_Interface.GetTxBytesWaiting(ref txWait);
                label1.Text = $"rx: {rxAvail}, tx: {txWait}";

                while (rxAvail != 0)
                    {
                        uint readed = 0;
                        var ftstatus = usb.USB_Interface.Read(buffer, (rxAvail < bLength) ? rxAvail : bLength, ref readed);
                        if (ftstatus == FTDI.FT_STATUS.FT_OK)
                        {
                            sumRx += readed;
                  //          textBox1.AppendText(ftstatus + " " + buffer[0].ToString("X2").PadLeft(2, '0') + "readLen: " + readed +  "\r\n");
                            usb.USB_Interface.GetRxBytesAvailable(ref rxAvail);
                    } else
                        {
                        AddLine(ftstatus.ToString());
                            rxAvail = 0;
                        }
                    }
                } else
            {
                label1.Text = "Closed";
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label2.Text = sumRx.ToString();
            sumRx = 0;
        }
    }
}
