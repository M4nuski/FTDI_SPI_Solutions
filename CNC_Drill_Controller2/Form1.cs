using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Essy.FTDIWrapper;
using FTDIWrapper;

namespace CNC_Drill_Controller2
{
    public partial class Form1 : Form
    {
        public Essy.FTDIWrapper.FTDI_Device  wrapper = new FTDI_Device(new FTDI_DeviceInfo());
        public Form1()
        {
            InitializeComponent();
            var di = FTDI_DeviceInfo.EnumerateDevices();
            Text = di.ToString();
        }
    }
}
