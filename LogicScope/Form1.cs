#undef LOG_MOUSE
#undef PREFILL_RANDOM
#define PREFILL_PATTERN
#undef PREFILL_FROMFILE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace LogicScope
{
    public partial class Form1 : Form
    {
        public delegate void loopDelegate();
        public bool run = true;

        private USB_Control usb = new USB_Control();
        private const int BUFFER_LEN = 1024;
        private byte[] buffer = new byte[BUFFER_LEN];

        private Graphics graph;
        private Bitmap bm;
        private const int STREAM_LEN = 1024 * 64;

        private Stream dataStream;

        private float zoom = 1.0f;
        private int zoom_level = 0;
        private const int MAX_ZOOM_LEVEL = 6; // 64
        private const int MIN_ZOOM_LEVEL = -12; // 1/4096

        private const int SAMPLES_PER_VIEW = 512;
        private int samplesPerView = SAMPLES_PER_VIEW;
        private int maxBufferIndex = 0;

        private const int BIT_OFFSET = -5;
        private const int BIT_STRIDE = 60;
        private const int BIT0_HEIGHT = 4;
        private const int BIT1_HEIGHT = 50;

        private const int TICK_LENGTH = 5;
        private const int BIGTICK_LENGTH = 10;

        private int bufferIndex;
        private int mouseIndex;

        private int sampleRate = 500000;
        private float timePerView = SAMPLES_PER_VIEW / 500000.0f;
        private float timePerDiv = 0.1f / 500000.0f;

        private bool sampling = false;
        private int invert_mask = 0x00;
        private int trig_mask = 0x00;
        private int last_trig = 0;
        private bool triggered = false;

        private long last_length = 0;

        private int mark1_s = 0;
        private float mark1_t = 0.0f;
        private int mark2_s = 9000;
        private float mark2_t = 0.0f;

        private bool buttonColorChanged = false;

        private List<int> tickList = new List<int>();
        private List<int> bigTickList = new List<int>();
        private int tickBit = 0;

        private dataReader reader;

        #region form setup and cleanup
        public Form1()
        {
            InitializeComponent();
            bm = new Bitmap(graphControl1.Width, graphControl1.Height);
            graph = Graphics.FromImage(bm);
            graphControl1.Image = bm;
            graphControl1.MouseWheel += graphControl1_MouseMove;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            reloadDevicesButton_Click(null, null);
            checkBoxInvb0_CheckedChanged(null, null);
            radioTrig0_CheckedChanged(null, null);

            dataStream = new MemoryStream(STREAM_LEN);

#if PREFILL_RANDOM
            var r = new Random();
            for (int i = 0; i < BUFFER_LEN; ++i)
            {
                int sb = i & 0x0F;
                int rb = r.Next() & 0xF0;
                dataStream.WriteByte((byte)(sb | rb));
            }
            setScrollBarMax();

#elif PREFILL_PATTERN

            for (int i = 0; i < BUFFER_LEN; ++i)
            {
                dataStream.WriteByte((byte)(i & 0xFF));
            }
            setScrollBarMax();
#elif PREFILL_FROMFILE
            var fileName = "2023-01-05 20-15-32 500000 M2B2SD.data";
            if (File.Exists(fileName))
                try
                {
                    var inputStream = File.OpenRead(fileName);
                    int o = 0;
                    while (o < (int)inputStream.Length)
                    {
                        int l = inputStream.Read(buffer, 0, BUFFER_LEN);
                        dataStream.Write(buffer, 0, l);
                        o += l;
                    }
                    inputStream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Debug] PREFILL_FROMFILE exception: " + ex.Message);
                }
            else
            {
                Console.WriteLine("[Debug] PREFILL_FROMFILE file not found: " + fileName);
            }
            setScrollBarMax();
#endif


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new loopDelegate(Application_Loop));
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            usb.CloseDevice();
            run = false;
        }
        #endregion
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (usb.IsOpen)
            {
                Sampling_State_label.Text = (sampling ? "Sampling " : "Hold ") + (triggered ? " Trig" : "");
                triggered = false;
                Sampling_Rate_label.Text = formatSampleQty((dataStream.Length - last_length) * 10) + "S/s";
                last_length = dataStream.Length;
            }
            else
            {
                Sampling_State_label.Text = "";
            }

            Sampling_Total_label.Text = formatSampleQty(dataStream.Length);
            Sampling_start_button.Enabled = usb.IsOpen;
            Sampling_cut_button.Enabled = !sampling;

            Device_load_button.Enabled = !usb.IsOpen;
            Device_close_button.Enabled = usb.IsOpen;

            if (mark1_s != mark2_s)
            {
                Mark1_t_label.Text = "t1: " + formatSampleTime(mark1_t);
                Mark2_t_label.Text = "t2: " + formatSampleTime(mark2_t);
                var delta = Math.Abs(mark2_t - mark1_t);
                t2t1_info_label.Text = "t2 - t1: " + formatSampleTime(delta) + ", " + (1.0f / (delta)) + "Hz";
                t2t1_nSamples_label.Text = (mark2_s - mark1_s).ToString() + " samples";
            }
        }

        private void ResetButtonColors()
        {
            if (!buttonColorChanged) return;
            buttonNext0b0.BackColor = DefaultBackColor;
            buttonNext0b1.BackColor = DefaultBackColor;
            buttonNext0b2.BackColor = DefaultBackColor;
            buttonNext0b3.BackColor = DefaultBackColor;

            buttonNext0b4.BackColor = DefaultBackColor;
            buttonNext0b5.BackColor = DefaultBackColor;
            buttonNext0b6.BackColor = DefaultBackColor;
            buttonNext0b7.BackColor = DefaultBackColor;

            buttonSkipb0.BackColor = DefaultBackColor;
            buttonSkipb1.BackColor = DefaultBackColor;
            buttonSkipb2.BackColor = DefaultBackColor;
            buttonSkipb3.BackColor = DefaultBackColor;

            buttonSkipb4.BackColor = DefaultBackColor;
            buttonSkipb5.BackColor = DefaultBackColor;
            buttonSkipb6.BackColor = DefaultBackColor;
            buttonSkipb7.BackColor = DefaultBackColor;

            buttonColorChanged = false;
        }

        private void Application_Loop()
        {
            int ba;
            while (run) if (usb.IsOpen)
                {
                    ba = usb.GetRXbytesAvailable();
                    //DeviceState_label.Text = ba.ToString();
                    if (ba == 0)
                    {
                        Thread.Sleep(2); // USB pool only every 2 ms at best
                    }
                    else while (ba > 0)
                        {
                            if (ba > BUFFER_LEN) ba = BUFFER_LEN; // clamp
                            usb.Read(buffer, ba);
                            if (invert_mask != 0) processInvert(ba);

                            if (sampling && (radioTrigA.Checked || checkTrig(ba)))
                            {
                                dataStream.Write(buffer, 0, ba);
                                bufferIndex += ba;
                                triggered = true;
                            }
                            ba = usb.GetRXbytesAvailable();
                        }
                    if (Device_preview_checkBox.Checked || triggered) drawUSBGraph();
                    Application.DoEvents();
                }
                else
                {
                    Thread.Sleep(250); // throttle down
                    Application.DoEvents();
                }
        }

        private bool checkTrig(int len)
        {
            if (trig_level_comboBox.Text == "0")
            {
                for (int i = 0; i < len; ++i)
                {
                    if ((~buffer[i] & trig_mask) != 0)
                    {
                        last_trig = 0;
                        return true;
                    }
                }
                return false;
            }
            else if (trig_level_comboBox.Text == "1")
            {
                for (int i = 0; i < len; ++i)
                {
                    if ((buffer[i] & trig_mask) != 0)
                    {
                        last_trig = 1;
                        return true;
                    }
                }
                return false;
            }
            else // X exhcange 
            {
                if (last_trig == 1)
                {
                    for (int i = 0; i < len; ++i)
                    {
                        if ((~buffer[i] & trig_mask) != 0)
                        {
                            last_trig = 0;
                            return true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < len; ++i)
                    {
                        if ((buffer[i] & trig_mask) != 0)
                        {
                            last_trig = 1;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private void processInvert(int len)
        {
            for (int i = 0; i < len; ++i) buffer[i] = (byte)(buffer[i] ^ invert_mask);
        }

        private Pen markPen = new Pen(Color.Cyan);
        private Pen tickPen = new Pen(Color.White);
        private Pen divPen = new Pen(Color.Red);
        private Brush bitBrush = new SolidBrush(Color.FromArgb(0, 255, 0));
        //private Brush darkBrush = new SolidBrush(Color.DarkRed);
        //private Brush markBrush = new SolidBrush(Color.Cyan);
        private Brush[] bitLevelBrushes = new SolidBrush[8] {    new SolidBrush(Color.FromArgb(0, 80, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 105, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 130, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 155, 0)),

                                                        new SolidBrush(Color.FromArgb(0, 180, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 205, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 230, 0)),
                                                        new SolidBrush(Color.FromArgb(0, 255, 0)) };

        private void drawUSBGraph()
        {
            graph.Clear(Color.Black);
            for (int x = 0; x < SAMPLES_PER_VIEW; ++x)
            {
                drawBit(graph, x * 2, BIT_STRIDE * 1, 2, (buffer[x] & 0x01) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 2, 2, (buffer[x] & 0x02) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 3, 2, (buffer[x] & 0x04) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 4, 2, (buffer[x] & 0x08) == 0x00);

                drawBit(graph, x * 2, BIT_STRIDE * 5, 2, (buffer[x] & 0x10) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 6, 2, (buffer[x] & 0x20) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 7, 2, (buffer[x] & 0x40) == 0x00);
                drawBit(graph, x * 2, BIT_STRIDE * 8, 2, (buffer[x] & 0x80) == 0x00);
            }
            graphControl1.Refresh();
        }


        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            //t2t1_info_label.Text = hScrollBar1.Value + " / " + hScrollBar1.Maximum;
            updateViewInfo();
            drawFileGraph(e.NewValue);
            ResetButtonColors();
        }

        private void drawFileGraph(int offset)
        {
            graph.Clear(Color.Black);
            if (offset > ((int)dataStream.Length - samplesPerView)) offset = (int)dataStream.Length - samplesPerView;
            bufferIndex = offset;
            if (offset < 0) return;

            dataStream.Seek(offset, SeekOrigin.Begin);

            if (zoom >= 1.0f)
            {
                int z = (int)zoom;
                z = z * 2;
                dataStream.Read(buffer, 0, samplesPerView);

                for (int x = 0; x < samplesPerView; ++x)
                {
                    graph.DrawLine(divPen, x * z, 0, x * z, 479);

                    drawBit(graph, x * z, BIT_STRIDE * 1, z, (buffer[x] & 0x01) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 2, z, (buffer[x] & 0x02) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 3, z, (buffer[x] & 0x04) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 4, z, (buffer[x] & 0x08) == 0x00);

                    drawBit(graph, x * z, BIT_STRIDE * 5, z, (buffer[x] & 0x10) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 6, z, (buffer[x] & 0x20) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 7, z, (buffer[x] & 0x40) == 0x00);
                    drawBit(graph, x * z, BIT_STRIDE * 8, z, (buffer[x] & 0x80) == 0x00);

                    if (offset + x == mark1_s) graph.DrawLine(markPen, x * z, 0, x * z, 479);
                    if (offset + x == mark2_s) graph.DrawLine(markPen, x * z, 0, x * z, 479);
                }
                if (tickList.Count > 0)
                {
                    foreach (var tick in tickList)
                    {
                        if ((tick >= offset) && (tick <= (offset + samplesPerView)))
                        {
                            var x = tick - offset;
                            x = x * z;
                            graph.DrawLine(tickPen, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET - TICK_LENGTH);
                        }
                    }
                }
                if (bigTickList.Count > 0)
                {
                    foreach (var tick in bigTickList)
                    {
                        if ((tick >= offset) && (tick <= (offset + samplesPerView)))
                        {
                            var x = tick - offset;
                            x = x * z;
                            graph.DrawLine(tickPen, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET - BIGTICK_LENGTH);
                        }
                    }
                }
            }
            else if (zoom < 1.0f)
            {
                int nSamples = (int)(1.0f / zoom);
                int[] accum = new int[8];

                double offset_d = (double)offset / sampleRate;

                int baseViewDiv = (int)Math.Floor((double)offset / samplesPerView);
                int baseViewRem = offset - (baseViewDiv * samplesPerView);

                double currentDivIndex = Math.Floor(offset_d / timePerDiv);

                for (int x = 0; x < SAMPLES_PER_VIEW; ++x)
                {
                    double t0 = Math.Floor((offset_d + ((x * nSamples) / sampleRate)) / timePerDiv);
                    double t1 = Math.Floor((offset_d + (((x + 1.0) * nSamples) / sampleRate)) / timePerDiv);
                    if ((t0 <= currentDivIndex) && (t1 >= currentDivIndex))
                    {
                        graph.DrawLine(divPen, x * 2, 0, x * 2, 479);
                        currentDivIndex++;
                    }

                    accum[0] = 0;
                    accum[1] = 0;
                    accum[2] = 0;
                    accum[3] = 0;

                    accum[4] = 0;
                    accum[5] = 0;
                    accum[6] = 0;
                    accum[7] = 0;

                    for (int s = 0; s < nSamples; ++s)
                    {
                        int b = dataStream.ReadByte();
                        accum[0] = accum[0] + ((b & 0x01) == 0x00 ? 0 : 1);
                        accum[1] = accum[1] + ((b & 0x02) == 0x00 ? 0 : 1);
                        accum[2] = accum[2] + ((b & 0x04) == 0x00 ? 0 : 1);
                        accum[3] = accum[3] + ((b & 0x08) == 0x00 ? 0 : 1);

                        accum[4] = accum[4] + ((b & 0x10) == 0x00 ? 0 : 1);
                        accum[5] = accum[5] + ((b & 0x20) == 0x00 ? 0 : 1);
                        accum[6] = accum[6] + ((b & 0x40) == 0x00 ? 0 : 1);
                        accum[7] = accum[7] + ((b & 0x80) == 0x00 ? 0 : 1);
                    }

                    drawBit3State(graph, x * 2, BIT_STRIDE * 1, 2, accum[0], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 2, 2, accum[1], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 3, 2, accum[2], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 4, 2, accum[3], nSamples);

                    drawBit3State(graph, x * 2, BIT_STRIDE * 5, 2, accum[4], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 6, 2, accum[5], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 7, 2, accum[6], nSamples);
                    drawBit3State(graph, x * 2, BIT_STRIDE * 8, 2, accum[7], nSamples);

                    var s1 = offset + (x * nSamples);
                    var s2 = offset + ((x + 1) * nSamples);
                    if ((mark1_s >= s1) && (mark1_s < s2)) graph.DrawLine(markPen, x * 2, 0, x * 2, 479);
                    if ((mark2_s >= s1) && (mark2_s < s2)) graph.DrawLine(markPen, x * 2, 0, x * 2, 479);
                }

                if (tickList.Count > 0)
                {
                    foreach (var tick in tickList)
                    {
                        if ((tick >= offset) && (tick <= (offset + samplesPerView)))
                        {
                            var x = tick - offset;
                            x = 2 * x / nSamples;
                            graph.DrawLine(tickPen, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET - TICK_LENGTH);
                        }
                    }
                }
                if (bigTickList.Count > 0)
                {
                    foreach (var tick in bigTickList)
                    {
                        if ((tick >= offset) && (tick <= (offset + samplesPerView)))
                        {
                            var x = tick - offset;
                            x = 2 * x / nSamples;
                            graph.DrawLine(tickPen, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET, x, (BIT_STRIDE * (tickBit + 1)) - BIT1_HEIGHT + BIT_OFFSET - BIGTICK_LENGTH);
                        }
                    }
                }
            }

            graphControl1.Refresh();
            // graphControl1.Show();
        }



        private void drawBit3State(Graphics g, int x, int y, int width, int sum, int max)
        {
            if (sum == 0)
            {
                g.FillRectangle(bitBrush, x, y - BIT0_HEIGHT + BIT_OFFSET, width, BIT0_HEIGHT);

            }
            else if (sum == max)
            {
                g.FillRectangle(bitBrush, x, y - BIT1_HEIGHT + BIT_OFFSET, width, BIT1_HEIGHT);
            }
            else
            {
                int l = (int)(Math.Round(7.0 * sum / max));
                g.FillRectangle(bitBrush, x, y - BIT0_HEIGHT + BIT_OFFSET, width, BIT0_HEIGHT);
                g.FillRectangle(bitLevelBrushes[l], x, y - BIT1_HEIGHT + BIT_OFFSET, width, BIT1_HEIGHT - BIT0_HEIGHT);
            }
        }

        private void drawBit(Graphics g, int x, int y, int width, bool value)
        {
            if (value)
            {
                g.FillRectangle(bitBrush, x, y - BIT0_HEIGHT + BIT_OFFSET, width, BIT0_HEIGHT);
            }
            else
            {
                g.FillRectangle(bitBrush, x, y - BIT1_HEIGHT + BIT_OFFSET, width, BIT1_HEIGHT);
            }
        }



        private void setScrollBarMax()
        {
            samplesPerView = (int)Math.Floor(SAMPLES_PER_VIEW / zoom);
            timePerView = (float)samplesPerView / sampleRate;
            if (zoom >= 1.0f)
            {
                timePerDiv = 1.0f / sampleRate;
            }
            else
            {
                timePerDiv = (float)Math.Pow(10.0, Math.Floor(Math.Log10(timePerView) - 1.0));
            }
            maxBufferIndex = (int)dataStream.Length - samplesPerView;
            if (maxBufferIndex < 0) maxBufferIndex = 0;
            hScrollBar1.Maximum = (samplesPerView / 2) + maxBufferIndex - 1; // weird shit to get actual maximum
            hScrollBar1.LargeChange = samplesPerView / 2;
            if (bufferIndex < 0) bufferIndex = 0;
            if (bufferIndex > maxBufferIndex) bufferIndex = maxBufferIndex;
            hScrollBar1.Value = bufferIndex;

            updateViewInfo();
            drawFileGraph(bufferIndex);
        }


        private void updateViewInfo()
        {
            double bufferOffset = (double)bufferIndex / sampleRate;
            double mouseOffset = (double)mouseIndex / sampleRate;

            ViewZoom_label.Text = "zoom: " + formatZoom(zoom) + "x";
            ViewWidth_s_label.Text = samplesPerView + " samples/view";
            ViewWidth_t_label.Text = formatSampleTime(timePerView) + "/view";
            ViewTperDiv_label.Text = formatSampleTime(timePerDiv) + "/DIV";

            StartPos_s_label.Text = "SartPos: " + bufferIndex;
            StartPos_t_label.Text = formatSampleTime(bufferOffset);
            MousePos_s_label.Text = "MousePos: " + mouseIndex;
            MousePos_t_label.Text = formatSampleTime(mouseOffset);

            viewDataBox.Refresh();
        }

        private string formatZoom(float z)
        {
            return (z >= 1.0f) ? z.ToString() : "1/" + (1.0f / z).ToString("F0");
        }

        private string formatSampleTime(double time)
        {
            time = time * 1000000.0f;
            if (time >= 1000.0f)
            {
                time = time / 1000.0f;
                return time.ToString("F3") + "ms";
            }
            else
            {
                return time.ToString("F3") + "us";
            }
        }

        private string formatSampleQty(long q)
        {
            if (q < 1000) return q.ToString();
            if (q < 1000000) return (q / 1024.0).ToString("F3") + "K";
            if (q < 1000000000) return (q / (1024.0 * 1024.0)).ToString("F3") + "M";
            return (q / (1024.0 * 1024.0 * 1024.0)).ToString("F3") + "G";
        }

        private void buttonNext0_Click(object sender, EventArgs e)
        {   // seek next data sample with different bit value
            if (dataStream == null) return;
            ResetButtonColors();

            var btn = (Button)sender;

            var bitmask = int.Parse((string)(btn.Tag));
            bitmask = (int)Math.Pow(2, bitmask);

            // read current start of view data
            dataStream.Seek(bufferIndex++, SeekOrigin.Begin);
            var val = dataStream.ReadByte() & bitmask;

            // seek next sample which bit changed
            while ((bufferIndex < dataStream.Length - 1) && ((dataStream.ReadByte() & bitmask) == val)) bufferIndex++;
            if (bufferIndex > maxBufferIndex)
            {
                btn.BackColor = Color.Red;
                buttonColorChanged = true;
                hScrollBar1.Value = maxBufferIndex;
                return;
            }

            hScrollBar1.Value = bufferIndex;
            drawFileGraph(bufferIndex);
            updateViewInfo();
        }

        private void buttonSkipb0_Click(object sender, EventArgs e)
        {   // skip a view and seek next data sample with different bit value
            if (dataStream == null) return;
            ResetButtonColors();

            var btn = (Button)sender;

            var bitmask = int.Parse((string)(btn.Tag));
            bitmask = (int)Math.Pow(2, bitmask);

            // read current start of view data
            dataStream.Seek(bufferIndex++, SeekOrigin.Begin);
            var val = dataStream.ReadByte() & bitmask;

            // skip to next page view
            bufferIndex += samplesPerView;
            dataStream.Seek(bufferIndex++, SeekOrigin.Begin);

            // seek next sample with same bit
            while ((bufferIndex < dataStream.Length - 1) && ((dataStream.ReadByte() & bitmask) == val)) bufferIndex++;
            if (bufferIndex > maxBufferIndex)
            {
                btn.BackColor = Color.Red;
                buttonColorChanged = true;
                hScrollBar1.Value = maxBufferIndex;
                return;
            }

            bufferIndex--;
            hScrollBar1.Value = bufferIndex;
            drawFileGraph(bufferIndex);
            updateViewInfo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int sr = 1;
            if (int.TryParse(Device_Samplerate_textBox.Text, out sr))
            {
                sampleRate = sr;
                updateViewInfo();
            }
        }

        #region graph control events

        private void graphControl1_MouseMove(object sender, MouseEventArgs e)
        {
#if LOG_MOUSE
            log("mm " + e.Location.X.ToString() + " " + e.Button);
#endif
            mouseIndex = (e.X * samplesPerView) / graphControl1.Width + bufferIndex;

            if (e.Delta > 0)
            {
                setZoomLevel(++zoom_level);

                int newSamplesPerView = (int)Math.Floor(SAMPLES_PER_VIEW / zoom);
                bufferIndex = mouseIndex - (newSamplesPerView * e.X / graphControl1.Width);
                setScrollBarMax();

            }
            else if (e.Delta < 0)
            {
                setZoomLevel(--zoom_level);

                int newSamplesPerView = (int)Math.Floor(SAMPLES_PER_VIEW / zoom);
                bufferIndex = mouseIndex - (newSamplesPerView * e.X / graphControl1.Width);
                setScrollBarMax();
            }

            updateViewInfo();
        }

        private void setZoomLevel(int newLevel)
        {
            if (newLevel < MIN_ZOOM_LEVEL) newLevel = MIN_ZOOM_LEVEL;

            while ((SAMPLES_PER_VIEW / Math.Pow(2.0, newLevel)) > dataStream.Length) newLevel++;

            if (newLevel > MAX_ZOOM_LEVEL) newLevel = MAX_ZOOM_LEVEL;

            zoom_level = newLevel;
            zoom = (float)Math.Pow(2.0, zoom_level);
        }

        private void graphControl1_Paint(object sender, PaintEventArgs e)
        {
            // e.Graphics.DrawImage(bm, 0, 0);
        }

        #endregion

        #region sampling
        private void button6_Click(object sender, EventArgs e)
        {
            sampling = false;
            log("Sampling Paused");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sampling = true;
            usb.Purge(true, true);
            log("Sampling Started");
            //dataStream.Seek(0, SeekOrigin.End);
            bufferIndex = 0;
            setZoomLevel(0); // 512 samples per view
            setScrollBarMax();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // save to file
            if (dataStream.Length == 0)
            {
                log("No sample data to save");
                return;
            }
            if (dataStream.Length > Int32.MaxValue)
            {
                log("Data sample too big (> 2gb)");
                return;
            }

            var d = DateTime.Now;
            saveFileDialog1.FileName = d.ToString("yyyy-MM-dd HH-mm-ss ") + sampleRate + ".data";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            var outputStream = File.Create(saveFileDialog1.FileName, (int)dataStream.Length);
            int o = 0;
            dataStream.Seek(0, SeekOrigin.Begin);
            while (o < (int)dataStream.Length)
            {
                int l = dataStream.Read(buffer, 0, BUFFER_LEN);
                outputStream.Write(buffer, 0, l);
                o += l;
            }
            outputStream.Flush();
            outputStream.Close();
            log("Sampling Saved to " + saveFileDialog1.FileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sampling) button6_Click(sender, e); // pause sampling

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var inputStream = File.OpenRead(openFileDialog1.FileName);
                zoom = 1.0f;
                zoom_level = 0;
                bufferIndex = 0;
                dataStream.Close();
                dataStream = new MemoryStream((int)inputStream.Length);
                int o = 0;
                while (o < (int)inputStream.Length)
                {
                    int l = inputStream.Read(buffer, 0, BUFFER_LEN);
                    dataStream.Write(buffer, 0, l);
                    o += l;
                }
                inputStream.Close();

                bufferIndex = 0;
                setScrollBarMax();
                log("Sampling Loaded from " + openFileDialog1.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // clear buffer
            dataStream.Close();
            dataStream = new MemoryStream(STREAM_LEN);
            log("Sampling Reset");

            bufferIndex = 0;
            setScrollBarMax();
        }
        #endregion

        #region device handling
        private void reloadDevicesButton_Click(object sender, EventArgs e)
        {
            Device_comboBox.Items.Clear();
            var list = usb.GetDevicesList();
            log(list.Count + " Devices Found");
            if (list.Count == 0) return;
            for (var i = 0; i < list.Count; ++i)
            {
                Device_comboBox.Items.Add(list[i].SerialNumber + ' ' + list[i].Description);
            }
            Device_comboBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device_State_label.Text = "Closed";
            if (Device_comboBox.Items.Count == 0) return;
            try
            {
                usb.OpenDeviceBySerialNumber(Device_comboBox.SelectedItem.ToString().Split(' ')[0]);
            }
            catch (Exception ex)
            {
                log(ex.Message);
                return;
            }
            if (usb.IsOpen)
            {
                Device_State_label.Text = Device_comboBox.SelectedItem.ToString().Split(' ')[1] + " Open";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Device_State_label.Text = "Closed";
            usb.CloseDevice();
        }
        #endregion

        #region log
        private void log(string text)
        {
            log_textBox.AppendText(text + "\r\n");
        }

        private void clearLog()
        {
            log_textBox.Clear();
        }
        #endregion

        #region data decoding
        private void Decode_UART_button_Click(object sender, EventArgs e)
        {
            if ((mark1_s == mark2_s) && (Decode_range_comboBox.SelectedIndex == 0))
            {
                log("no selection in samples");
                return;
            }

            int samplesPerBit = sampleRate / int.Parse(Decode_baudrate_textBox.Text);
            if (samplesPerBit <= 1)
            {
                log("samplerate too low to decode UART");
                return;
            }

            reader = new dataReader(dataStream);
            reader.index = mark1_s;

            if (Decode_range_comboBox.SelectedIndex == 0) reader.maxIndex = mark2_s;

            var dataBit = Decode_Data_comboBox.SelectedIndex;

            if (reader.getBit(dataBit) == 0)
            {
                log("mark 1 must be on idle state (1)");
                return;
            }

            reader.seekBit(dataBit, 0); // start bit

            if (reader.maxReached)
            {
                log("reached end mark before start bit");
                return;
            }

            tickList.Clear();
            bigTickList.Clear();
            tickBit = dataBit;
            bigTickList.Add(reader.index);
            var s = "";

            reader.offsetIndex(samplesPerBit / 2); // middle of start bit

            bigTickList.Add(reader.index);
            if (reader.getBit(dataBit) != 0)
            {
                log("invalid start bit length (or incorrect baud rate / sample rate)");
                bigTickList.Add(reader.index);
                drawFileGraph(bufferIndex);
                return;
            }

            while (!reader.maxReached)
            {
                for (var b = 0; b < 8; ++b)
                {
                    var bitChangedInRange = reader.seekBitChangedInRange(dataBit, samplesPerBit);

                    if (bitChangedInRange)
                    {
                        // clock recovery on edge
                        bigTickList.Add(reader.index);
                        reader.offsetIndex(samplesPerBit / 2);
                    }
                    tickList.Add(reader.index);

                    s += (reader.getBit(dataBit) == 0) ? "0" : "1";
                }

                // assert stop bit, should be 1
                reader.offsetIndex(samplesPerBit);
                bigTickList.Add(reader.index);
                if (!reader.maxReached && (reader.getBit(dataBit) == 1))
                {
                    var bitChangedInRange = reader.seekBitChanged(dataBit);
                    bigTickList.Add(reader.index);
                    //check for start bit 0 and clock recovery
                    if (!bitChangedInRange || (reader.getBit(dataBit) != 0)) reader.index = reader.maxIndex;
                    reader.offsetIndex(samplesPerBit / 2);
                }
            }

            log(s);
            s = reverseBitStringByteLSBFirst(s);
            decodeBitString(s);
            // start bit
            // 8 data bits, lsb to msb
            // no parity
            // 1 stop bit
            // idle is 1
            // ¯¯¯¯¯¯_01234567¯
            // iiiiiiabbbbbbbbe­­

            drawFileGraph(bufferIndex);
        }
        private void Decode_DHT11_button_Click(object sender, EventArgs e)
        {
            if ((mark1_s == mark2_s) && (Decode_range_comboBox.SelectedIndex == 0))
            {
                log("no selection in samples");
                return;
            }

            reader = new dataReader(dataStream);
            reader.index = mark1_s;

            if (Decode_range_comboBox.SelectedIndex == 0) reader.maxIndex = mark2_s;

            var dataBit = Decode_Data_comboBox.SelectedIndex;

            bigTickList.Clear();
            tickList.Clear();
            tickBit = dataBit;

            // assume idle before MCU low pulse
            if (reader.getBit(dataBit) == 1) reader.seekBit(dataBit, 0);
            if (reader.maxReached)
            {
                log("reached end mark before start of MCU low pulse");
                return;
            }
            bigTickList.Add(reader.index);

            // end of MCU low pulse
            reader.seekBit(dataBit, 1); 
            if (reader.maxReached)
            {
                log("reached end mark before end of MCU low pulse");
                return;
            }
            bigTickList.Add(reader.index);

            reader.seekBit(dataBit, 0);
            if (reader.maxReached)
            {
                log("reached end mark before start of DHT11 low response setup pulse");
                return;
            }
            bigTickList.Add(reader.index);

            reader.seekBit(dataBit, 1);
            if (reader.maxReached)
            {
                log("reached end mark before end of DHT11 low response setup pulse");
                return;
            }
            bigTickList.Add(reader.index);

            reader.seekBit(dataBit, 0);
            if (reader.maxReached)
            {
                log("reached end mark before start of DHT11 response pulse");
                return;
            }
            bigTickList.Add(reader.index);

            var nBits = 0;
            int refClockPeriod;
            int bitClockPeriod;
            var s = "";

            while ((nBits < 40) && !reader.maxReached) // signal now at 0
            {
                refClockPeriod = reader.index;
                // next 1 is rising edge and end of ref time, start of bit time
                reader.seekBit(dataBit, 1);
                bigTickList.Add(reader.index);

                refClockPeriod = reader.index - refClockPeriod;
                bitClockPeriod = reader.index;

                tickList.Add(reader.index + refClockPeriod);

                // next 0 is end of bit time, start of next ref time
                reader.seekBit(dataBit, 0);
                bitClockPeriod = reader.index - bitClockPeriod;

                s += (bitClockPeriod > refClockPeriod) ? "1" : "0";
                nBits++;
            }

            log(s + " " + s.Length + " bits");
            decodeBitString(s);

            if (reader.maxReached)
            {
                log("reached end mark before reading 40 bits of data");
                return;
            }

            var b = getBytesFromBitString(s);

            // total is 40 bits, checksum is sum(byte0 to byte3) mod 0xFF
            if (b.Length != 5) return;
            int chk = b[0] + b[1] + b[2] + b[3];
            if ((chk & 0xFF) != b[4])
            {
                log("Invalid checksum in DHT11 data");
                return;
            }
            log("RH: " + b[0] + "." + b[1] + "%, T: " + b[2] + "." + b[3] + "C");
            drawFileGraph(bufferIndex);
        }


        private void Decode_Manchester_button_Click(object sender, EventArgs e)
        {
            if ((mark1_s == mark2_s) && (Decode_range_comboBox.SelectedIndex == 0))
            {
                log("no selection in samples");
                return;
            }
            var index = mark1_s;
            var endIndex = (Decode_range_comboBox.SelectedIndex == 0) ? mark2_s : dataStream.Length - 1;
            var clockPediod = 256;
            var quaterClock = 0;
            var clockLimit = 0;
            try
            {
                clockPediod = Int32.Parse(Decode_ClockPer_textBox.Text);
                quaterClock = clockPediod / 4;
                clockLimit = (int)(clockPediod * 2.5);

                if ((endIndex - index) < clockPediod) throw new Exception("selection length too small for clock period");
            }
            catch (Exception ex)
            {
                log("error in clock period value: " + ex.Message);
                return;
            }

            var mask = getDecodeDataMask();
            dataStream.Seek(index, SeekOrigin.Begin);
            var currentVal = dataStream.ReadByte() & mask;
            // seek first change
            while ((currentVal == (dataStream.ReadByte() & mask)) && (index <= endIndex)) ++index;

            var currentPediod = 0;
            var s = "";
            var bitHalf1 = 0;
            var bitHalf2 = 0;

            tickList.Clear();
            bigTickList.Clear();
            tickBit = 7;

            bigTickList.Add(index + 1);

            {
                var data = new List<bool>();
                while ((currentPediod < clockLimit) && (index <= endIndex))
                {
                    currentPediod = index;
                    // advance quater clock
                    index += quaterClock;
                    dataStream.Seek(index++, SeekOrigin.Begin);
                    currentVal = dataStream.ReadByte() & mask;
                    bitHalf1 = currentVal;

                    tickList.Add(index);

                    // advance half clock
                    index += quaterClock * 2;
                    dataStream.Seek(index++, SeekOrigin.Begin);
                    currentVal = dataStream.ReadByte() & mask;
                    bitHalf2 = currentVal;

                    tickList.Add(index);

                    if (Decode_invData_checkBox.Checked)
                    {
                        bitHalf1 = ~bitHalf1;
                        bitHalf2 = ~bitHalf2;
                    }

                    data.Add(bitHalf1 != 0);
                    data.Add(bitHalf2 != 0);


                    s += (bitHalf1 != bitHalf2) ? "0" : "1"; // for debug
                    // L HL 1 LH 0 // HL and LH phase change can overlap clock cycle

                    // M different 1 same 0 // always a change at every clock cycle
                    // S different 0 same 1 // always a change at every clock cycle

                    // D HL vs LH phase change are 1, same phases are 0 // HL and LH phase change can overlap clock cycle

                    // seek next change on clock period
                    while ((currentVal == (dataStream.ReadByte() & mask)) && (index <= endIndex)) ++index;
                    bigTickList.Add(index);
                    currentPediod = index - currentPediod;
                }
                bigTickList.Add(index);
                log(s + " " + s.Length + " bits");
                decodeBitString(s);

                var s2 = reverseBitString(s); // msb - lsb -> lsb - msb
                decodeBitString(s2);

                s2 = reverseBitStringBytes(s); // msB - lsB -> lsB -> msB
                decodeBitString(s2);

                s2 = reverseBitString(s2); // msb - lsb -> lsb - msb
                decodeBitString(s2);


                log("b = ~b");
                s = invertBitString(s); // b = ~b
                decodeBitString(s);

                s2 = reverseBitString(s); // msb - lsb -> lsb - msb
                decodeBitString(s2);

                s2 = reverseBitStringBytes(s); // msB - lsB -> lsB -> msB
                decodeBitString(s2);

                s2 = reverseBitString(s2); // msb - lsb -> lsb - msb
                decodeBitString(s2);

                log("toggle");
                s = invertBitString(s); // b = ~b
                s2 = convertBitStringToToggle(s, '0');
                log(s2 + " " + s2.Length + " bits");
                decodeBitString(s2);

                s2 = convertBitStringToToggle(s, '1');
                log(s2 + " " + s2.Length + " bits");
                decodeBitString(s2);

                log("b = ~b");
                s = invertBitString(s); // b = ~b
                s2 = convertBitStringToToggle(s, '0');
                log(s2 + " " + s2.Length + " bits");
                decodeBitString(s2);

                s2 = convertBitStringToToggle(s, '1');
                log(s2 + " " + s2.Length + " bits");
                decodeBitString(s2);


            } // read cycle

            drawFileGraph(bufferIndex);
        }

        private string convertBitStringToToggle(string s, char lastChar)
        {
            var res = "";
            for (var i = 0; i < s.Length; ++i) {
                if (s[i] == '0') res += lastChar; else
                {
                    lastChar = (lastChar == '0') ? '1' : '0';
                    res += lastChar;
                }
            }
            return res;
        }

        private string invertBitString(string s)
        {
            var res = "";
            for (var i = 0; i < s.Length; ++i) res += (s[i] == '0') ? "1" : "0";
            return res;
        }
        private string reverseBitString(string s)
        {
            var res = "";
            for (var i = s.Length - 1; i >= 0; --i) res += s[i];
            return res;
        }

        private string reverseBitStringByteLSBFirst(string s)
        {
            var res = reverseBitString(s);
            return reverseBitStringBytes(res);
        }

        private string reverseBitStringBytes(string s)
        {
            if (s.Length == 0) return "";
            while (s.Length % 8 != 0) s += "0";
            var res = "";
            var i = s.Length - 8;
            while (i >= 0)
            {
                res += s.Substring(i, 8);
                i -= 8;
            }
            return res;
        }

        private void decodeBitString(string s)
        {
            if (s.Length == 0)
            {
                log("Cannot decode Empty String");
                return;
            }

            while (s.Length % 8 != 0) s += "0"; // pad

            var sbH = "";
            var sbC = "";
            var sb = new StringBuilder();
            var byteIndex = 0;
            for (var i = 0; i < s.Length; i += 8)
            {
                if ((byteIndex != 0) && ((byteIndex % 8) == 0))
                {
                    sb.Append(sbH);
                    sb.Append(" ");
                    sb.AppendLine(sbC);
                    sbH = "";
                    sbC = "";
                }
                var val = binaryString8charToInt(s.Substring(i, 8));
                sbH += Convert.ToString(val, 16).ToUpper().PadLeft(2, '0') + " ";
                sbC += safeCharConvert(val);
                byteIndex++;
            }
            //if ((byteIndex % 4) != 0) sb.AppendLine();
            sb.Append(sbH.PadRight(24, ' '));
            sb.Append(" ");
            sb.Append(sbC);

            log(sb.ToString());
            //log(byteIndex + " bytes.");
        }
        private byte[] getBytesFromBitString(string s)
        {
            if (s.Length == 0)
            {
                log("Cannot get bytes from Empty String");
                return new byte[0];
            }

            while (s.Length % 8 != 0) s += "0"; // pad

            var res = new byte[s.Length / 8];
            for (var i = 0; i < s.Length; i += 8) res[i / 8] = (byte)binaryString8charToInt(s.Substring(i, 8));

            return res;
        }

        private int binaryString8charToInt(string s) // msb - lsb
        {
            var res = 0;
            if (s[0] == '1') res |= 0x80;
            if (s[1] == '1') res |= 0x40;
            if (s[2] == '1') res |= 0x20;
            if (s[3] == '1') res |= 0x10;

            if (s[4] == '1') res |= 0x08;
            if (s[5] == '1') res |= 0x04;
            if (s[6] == '1') res |= 0x02;
            if (s[7] == '1') res |= 0x01;

            return res;
        }

        private string safeCharConvert(int b)
        {
            if ((b < 32) || (b >= 127)) return ".";
            return "" + Convert.ToChar(b);
        }


        private void button23_Click(object sender, EventArgs e)
        {
            //decode spi
        }

        private int getDecodeClockMask()
        {
            return (int)Math.Pow(2, Decode_Clock_comboBox.SelectedIndex);
        }

        private int getDecodeDataMask()
        {
            return (int)Math.Pow(2, Decode_Data_comboBox.SelectedIndex);
        }

        #endregion

        private void checkBoxInvb0_CheckedChanged(object sender, EventArgs e)
        {
            invert_mask = 0;
            if (checkBoxInvb0.Checked) invert_mask = invert_mask | 0x01;
            if (checkBoxInvb1.Checked) invert_mask = invert_mask | 0x02;
            if (checkBoxInvb2.Checked) invert_mask = invert_mask | 0x04;
            if (checkBoxInvb3.Checked) invert_mask = invert_mask | 0x08;

            if (checkBoxInvb4.Checked) invert_mask = invert_mask | 0x10;
            if (checkBoxInvb5.Checked) invert_mask = invert_mask | 0x20;
            if (checkBoxInvb6.Checked) invert_mask = invert_mask | 0x40;
            if (checkBoxInvb7.Checked) invert_mask = invert_mask | 0x80;
            //log("invert mask 0x" + invert_mask.ToString("X2").ToUpper());
        }


        private void radioTrig0_CheckedChanged(object sender, EventArgs e)
        {
            trig_mask = 0;
            if (radioTrig0.Checked) trig_mask = trig_mask | 0x01;
            if (radioTrig1.Checked) trig_mask = trig_mask | 0x02;
            if (radioTrig2.Checked) trig_mask = trig_mask | 0x04;
            if (radioTrig3.Checked) trig_mask = trig_mask | 0x08;

            if (radioTrig4.Checked) trig_mask = trig_mask | 0x10;
            if (radioTrig5.Checked) trig_mask = trig_mask | 0x20;
            if (radioTrig6.Checked) trig_mask = trig_mask | 0x40;
            if (radioTrig7.Checked) trig_mask = trig_mask | 0x80;
            //log("trigger mask 0x" + trig_mask.ToString("X2").ToUpper());
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (sampling) return;

            if (e.KeyCode == Keys.D1) // mark t1
            {
                mark1_s = mouseIndex;
                if (mark1_s > mark2_s)
                {
                    mark2_s = mouseIndex;
                    mark1_s = mark2_s;
                }
                mark1_t = (float)mouseIndex / sampleRate;
                drawFileGraph(bufferIndex);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.D2) // mark t2
            {
                mark2_s = mouseIndex;
                if (mark1_s > mark2_s)
                {
                    mark2_s = mark1_s;
                    mark1_s = mouseIndex;
                }
                mark2_t = (float)mouseIndex / sampleRate;
                drawFileGraph(bufferIndex);
                e.Handled = true;
            }
        }

        private void Sampling_cut_button_Click(object sender, EventArgs e)
        {
            if (mark1_s == mark2_s) return;
            if (mark1_s > mark2_s) return;
            if ((mark1_s > dataStream.Length - 1) || (mark2_s > dataStream.Length - 1)) return;

            int delta = mark2_s - mark1_s;
            var oldStream = dataStream;
            var newStream = new MemoryStream((int)dataStream.Length - delta);

            int o = 0;
            oldStream.Seek(0, SeekOrigin.Begin);
            while (o < mark1_s)
            {
                var l = (int)oldStream.Length - o;
                if (l > BUFFER_LEN) l = BUFFER_LEN;
                if ((o + l) >= mark1_s) l = mark1_s - o;
                oldStream.Read(buffer, 0, l);
                newStream.Write(buffer, 0, l);
                o += l;
            }
            // bl 512
            // m1 1200
            // l  2048

            // o = 0;
            // l = 2048 = 512
            // copy 0-511
            // o = 512
            // l = 1536 = 512
            // copy 512-1023
            // o = 1024
            // l = 1024 = 512, o+l = 1536 >> = 176
            // copy 1024-1200
            // o = 1200

            // m2 = 1400
            oldStream.Seek(mark2_s, SeekOrigin.Begin);
            o = mark2_s;
            while (o < oldStream.Length)
            {
                var l = (int)oldStream.Length - o;
                if (l > BUFFER_LEN) l = BUFFER_LEN;
                oldStream.Read(buffer, 0, l);
                newStream.Write(buffer, 0, l);
                o += l;
            }

            // l = 648 = 512
            // copy 1400-1912
            // o = 1912
            // l = 136
            // copy 1912-136

            dataStream = newStream;
            oldStream.Close();

            if ((mark1_s >= dataStream.Length) || (mark2_s >= dataStream.Length))
            {
                mark1_s = 0;
                mark2_s = 0;
            }

            setScrollBarMax();
            log("Sampling cut from " + mark1_s + " to " + mark2_s);

        }

        private void Graph_Seek0_button_Click(object sender, EventArgs e)
        {
            if (sampling) return;
            bufferIndex = 0;
            hScrollBar1.Value = 0;
            drawFileGraph(bufferIndex);
        }

        private void Log_clear_button_Click(object sender, EventArgs e)
        {
            clearLog();
        }

        private void graphControl1_MouseEnter(object sender, EventArgs e)
        {
            graphControl1.Focus();
        }


    }


    public class dataReader {

        private int[] _masks = new int[8] {0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80};

        public bool maxReached { get; private set; }

        private int _index = 0;
        public int index { 
            get {
                return _index;
            } 
            set {
                setIndex(value);
            } 
        }

        private int _maxIndex = 0;
        public int maxIndex {
            get {
                return _maxIndex;
            }
            set {
                setMaxIndex(value);
            }
        }

        private int _streamEndIndex;
        private Stream _data;

        public int value { get; private set; }


        public dataReader(Stream dataSource)
        {
            _data = dataSource;
            _maxIndex = (int)dataSource.Length - 1;
            _streamEndIndex = _maxIndex;

            _data.Seek(0, SeekOrigin.Begin);
            value = (dataSource.Length > 0) ?_data.ReadByte() : -1;

            maxReached = (index == _maxIndex);
        }

        private void setIndex(int index)
        {
            if (index < 0) index = 0;
            if (index > _streamEndIndex) index = _streamEndIndex;
            if (index > _maxIndex) index = _maxIndex;
            _index = index;

            _data.Seek(_index, SeekOrigin.Begin);
            value = _data.ReadByte();

            maxReached = (index >= _maxIndex); 
        }

        private void setMaxIndex(int index)
        {
            if (index < 0) index = 0;
            if (index > _streamEndIndex) index = _streamEndIndex;
            _maxIndex = index;
            if (_index > _maxIndex) setIndex(_maxIndex);
        }

        public void offsetIndex(int offset)
        {
            setIndex(_index + offset);
        }



        public int getByteAt(int index)
        {
            setIndex(index);
            return value;
        }

        public int getNextByte()
        {
            if (maxReached) return value;
            value = _data.ReadByte();
            index++;
            maxReached = (index >= _maxIndex);
            return value;
        }

        public bool seekByte(int val)
        {
            if (val == value) return true;

            while ((value != val) && !maxReached) getNextByte();

            return !maxReached;
        }

        public bool seekByteChanged(int indexLimit)
        {
            return seekByteChangedBeforeIndex(maxIndex);
        }
        public bool seekByteChangedBeforeIndex(int indexLimit)
        {
            if (indexLimit > _maxIndex) indexLimit = _maxIndex;

            int initialValue = value;
            while ((value == initialValue) && !maxReached && (_index < indexLimit)) getNextByte();

            return _index < indexLimit;
        }

        public bool seekByteChangedInRange(int range)
        {
            var indexLimit = _maxIndex + range;
            if (indexLimit > _maxIndex) indexLimit = _maxIndex;

            int initialValue = value;
            while ((value == initialValue) && !maxReached && (_index < indexLimit)) getNextByte();

            return _index < indexLimit;
        }

        public int getBit(int bit)
        {
            return (value & _masks[bit]) >> bit;
        }

        public int getBitAt(int bit, int index)
        {
            getByteAt(index);
            return (value & _masks[bit]) >> bit;
        }

        public int getNextBit(int bit)
        {
            getNextByte();
            return (value & _masks[bit]) >> bit;
        }

        public bool seekBit(int bit, int val)
        {
            int currentValue = (value & _masks[bit]) >> bit;
            if (val == currentValue) return true;

            while ((currentValue != val) && !maxReached)
            {
                getNextByte();
                currentValue = (value & _masks[bit]) >> bit;
            }
            return !maxReached;
        }

        public bool seekBitChanged(int bit)
        {
            return seekBitChangedBeforeIndex(bit, _maxIndex);
        }

        public bool seekBitChangedBeforeIndex(int bit, int indexLimit)
        {
            if (indexLimit > _maxIndex) indexLimit = _maxIndex;
            int initialValue = value & _masks[bit];
            int currentValue = value & _masks[bit];

            while ((currentValue == initialValue) && !maxReached && (_index < indexLimit))
            {
                getNextByte();
                currentValue = value & _masks[bit];
            }
            return _index < indexLimit;
        }

        public bool seekBitChangedInRange(int bit, int range)
        {
            var indexLimit = _index + range;
            if (indexLimit > _maxIndex) indexLimit = _maxIndex;
            int initialValue = value & _masks[bit];
            int currentValue = value & _masks[bit];

            while ((currentValue == initialValue) && !maxReached && (_index < indexLimit))
            {
                getNextByte();
                currentValue = value & _masks[bit];
            }
            return _index < indexLimit;
        }
    }

    public class dataDecoder
    {
        public enum bitOrder
        {
            LSbitFirst, MSbitFirst
        }
        public enum wordLength
        {
            w7 = 7, 
            w8 = 8,
            w16 = 16,
            w24 = 24,
            w32 = 32,
            w64 = 64
        }
    }


}
