using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NAudio.Wave;

namespace EatingWithYourEars
{
    /// <summary>
    /// Control for viewing waveforms
    /// </summary>
    public class CustomWaveViewer : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Custom Variables.
        /// </summary>

        //for detectChew(short highestChewValue)
        private int counter = 0;
        private bool detectingChew = false;
        private int numOfChews = 0;
        private short globalHighest = 0;

        // for drawing:
        private List<int> drawableCoords = new List<int>();
        private int xTemp = 0;

        //for detectChew2(short highestChewValue)
        private int counter2 = 0;
        private bool detectingChew2 = false;
        private int numOfChews2 = 0;
        private short globalHighest2 = 0;

        //Drawing:
        private List<int> drawableCoords2 = new List<int>();
        private int xTemp2 = 0;

        //constant samplesPerPixel value (usid in readData):
        int constSamplesPerPixel = 1280;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private WaveStream waveStream;
        private int samplesPerPixel = 128;
        private long startPosition;
        private int bytesPerSample;
        /// <summary>
        /// Creates a new WaveViewer control
        /// </summary>
        public CustomWaveViewer()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.DoubleBuffered = true;

        }

        /// <summary>
        /// sets the associated wavestream.
        /// </summary>
        public WaveStream WaveStream
        {
            get
            {
                return waveStream;
            }
            set
            {
                waveStream = value;
                if (waveStream != null)
                {
                    bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// The zoom level, in samples per pixel
        /// </summary>
        public int SamplesPerPixel
        {
            get
            {
                return samplesPerPixel;
            }
            set
            {
                samplesPerPixel = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Start position (currently in bytes)
        /// </summary>
        public long StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                startPosition = value;
            }
        }

        public void fitToScreen()
        {
            if (waveStream == null)
            {
                return;
            }
            int samples = (int)(waveStream.Length / bytesPerSample);
            startPosition = 0;
            samplesPerPixel = samples / this.Width;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            fitToScreen();
            this.Invalidate();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// <see cref="Control.OnPaint"/>
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            //reset num of chews in case of repaint:
            numOfChews = 0;
            numOfChews2 = 0;

            //read the the audio data:
            readThroughData();

            //display results:
            Font f = new Font(FontFamily.GenericSansSerif, 16);
            Brush b = new SolidBrush(Color.Red);
            e.Graphics.DrawString("Samples Per Pixel (Visual): " + samplesPerPixel.ToString(), f, b, new Point(0, 10));
            e.Graphics.DrawString("Amount of Chews: " + numOfChews.ToString() + "\tAmount of Chews (2): " + numOfChews2.ToString(), f, b, new Point(0, 30));

            //drawing chew points:
            float divisor = samplesPerPixel / constSamplesPerPixel;

            e.Graphics.DrawString("Chew points for first method", f, new SolidBrush(Color.Blue), new Point(0, this.Height - 30));
            for (int i = 0; i < drawableCoords.Count; i++)
            {
                float trueXCoord = drawableCoords[i] / divisor;
                e.Graphics.DrawLine(Pens.Blue, trueXCoord, this.Height - 30, trueXCoord, this.Height - 40);
            }

            e.Graphics.DrawString("Chew points for second method", f, new SolidBrush(Color.Green), new Point(0, this.Height - 70));
            for (int i = 0; i < drawableCoords2.Count; i++)
            {
                float trueXCoord = drawableCoords2[i] / divisor;
                e.Graphics.DrawLine(Pens.Green, trueXCoord, this.Height - 70, trueXCoord, this.Height - 80);
            }

            //drawable wave stream:
            if (waveStream != null)
            {
                waveStream.Position = 0;
                int bytesRead;
                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];
                waveStream.Position = startPosition + (e.ClipRectangle.Left * bytesPerSample * samplesPerPixel);

                for (float x = e.ClipRectangle.X; x < e.ClipRectangle.Right; x += 1)
                {
                    short low = 0;
                    short high = 0;
                    bytesRead = waveStream.Read(waveData, 0, samplesPerPixel * bytesPerSample);
                    if (bytesRead == 0)
                        break;
                    for (int n = 0; n < bytesRead; n += 2)
                    {
                        short sample = BitConverter.ToInt16(waveData, n);
                        if (sample < low) low = sample;
                        if (sample > high) high = sample;
                    }
                    float lowPercent = ((((float)low) - short.MinValue) / ushort.MaxValue);
                    float highPercent = ((((float)high) - short.MinValue) / ushort.MaxValue);
                    e.Graphics.DrawLine(Pens.Black, x, this.Height * lowPercent, x, this.Height * highPercent);
                }
            }

            
            base.OnPaint(e);
        }

        private bool detectChew(short highestSampleValue, int xValue)
        {
            if (highestSampleValue > globalHighest)
            {
                    detectingChew = true;
                    globalHighest = highestSampleValue;
                    counter = 0;
                    xTemp = xValue;
            }
            else if (highestSampleValue < globalHighest)
            {
                if (detectingChew)
                {
                    counter++;
                    if (counter == 10)
                    {
                        counter = 0;
                        numOfChews++;
                        detectingChew = false;
                        drawableCoords.Add(xTemp);
                        return true;
                    }
                }
                else
                {
                    globalHighest = highestSampleValue;
                }
            }
            return false;
        }

        private bool detectChew2(short highestSampleValue, int xValue)
        {
            if (highestSampleValue > globalHighest2)
            {
                globalHighest2 = highestSampleValue;
                detectingChew2 = true;
                counter2 = 1;
                xTemp2 = xValue;
            }
            else if (highestSampleValue < globalHighest2)
            {
                if (detectingChew2)
                {
                    if (globalHighest2 - highestSampleValue > 500)
                    {
                        numOfChews2++;
                        detectingChew2 = false;
                        drawableCoords2.Add(xTemp2);
                        return true;
                    }
                    else
                    {
                        counter2++;
                    }
                }
                else
                {
                    globalHighest2 = highestSampleValue;
                }
            }
            return false;
        }


        private void readThroughData()
        {
            if (waveStream != null)
            {
                waveStream.Position = 0;
                int bytesRead;
                byte[] waveData = new byte[constSamplesPerPixel * bytesPerSample];
                waveStream.Position = startPosition + (0 * bytesPerSample * constSamplesPerPixel);

                for (int x = 0; x < 160000; x += 1)
                {
                    short low = 0;
                    short high = 0;
                    bytesRead = waveStream.Read(waveData, 0, constSamplesPerPixel * bytesPerSample);
                    if (bytesRead == 0)
                        break;
                    for (int n = 0; n < bytesRead; n += 2)
                    {
                        short sample = BitConverter.ToInt16(waveData, n);
                        if (sample < low) low = sample;
                        if (sample > high) high = sample;
                    }

                    detectChew(high, x);
                    detectChew2(high, x);

                    if (waveStream.Position >= waveStream.Length - 1)
                    {
                        break;
                    }

                }
            }
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
