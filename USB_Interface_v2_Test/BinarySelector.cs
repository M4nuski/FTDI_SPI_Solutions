using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace USB_Interface_v2_Test
{
    public partial class BinarySelector : UserControl
    {
        public byte Result;

        [Category("Action"), Description("Occurs when any of the checkboxes changes.")]
        public Action OnChange;

        [Category("Appearance"), Description("Specifies the content of the description label")]
        public string Description
        {
            get { return label3.Text; }
            set { label3.Text = value; }
        }

        public BinarySelector()
        {
            InitializeComponent();
        }

        public void ChangeValue(byte value)
        {
            checkBox1.Checked = SignalGenerator.GetBit(value, 0);
            checkBox2.Checked = SignalGenerator.GetBit(value, 1);
            checkBox3.Checked = SignalGenerator.GetBit(value, 2);
            checkBox4.Checked = SignalGenerator.GetBit(value, 3);

            checkBox5.Checked = SignalGenerator.GetBit(value, 4);
            checkBox6.Checked = SignalGenerator.GetBit(value, 5);
            checkBox7.Checked = SignalGenerator.GetBit(value, 6);
            checkBox8.Checked = SignalGenerator.GetBit(value, 7);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            byte res = 0;

            res = SignalGenerator.SetBit(res, 0, checkBox1.Checked);
            res = SignalGenerator.SetBit(res, 1, checkBox2.Checked);
            res = SignalGenerator.SetBit(res, 2, checkBox3.Checked);
            res = SignalGenerator.SetBit(res, 3, checkBox4.Checked);

            res = SignalGenerator.SetBit(res, 4, checkBox5.Checked);
            res = SignalGenerator.SetBit(res, 5, checkBox6.Checked);
            res = SignalGenerator.SetBit(res, 6, checkBox7.Checked);
            res = SignalGenerator.SetBit(res, 7, checkBox8.Checked);
            
            label1.Text = res.ToString("D3");
            label2.Text = res.ToString("x2").ToUpper();

            Result = res;
            if (OnChange != null) OnChange();
        }
    }
}
