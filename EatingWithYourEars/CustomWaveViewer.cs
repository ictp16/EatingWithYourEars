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

        //for DetectBite(short highestChewValue)
        private bool detectingChew3 = false;
        private int numOfBites = 0;
        private short globalHighest3 = 0;

        //Drawing:
        private List<int> drawableCoords2 = new List<int>();
        private int xTemp2 = 0;

        //constant samplesPerPixel value (usid in readData):
        int constSamplesPerPixel = 1764; //1764

        // flag for view samplesize:
        public bool isZoomed = false;

        /// <summary>
        /// Graphing:
        /// </summary>

        //determining the scale of the y axis:
        private int highestVal = 0;
        private int lowestVal = 0;

        
        // THESE ARE THE LEGACY VARIABLES:
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

        public void fitToGraph()
        {
            if (waveStream == null)
            {
                return;
            }
            int samples = (int)(waveStream.Length / bytesPerSample);
            startPosition = 0;
            samplesPerPixel = samples / (this.Width - 50 - 101);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!isZoomed)
            {
                fitToGraph();
                this.Invalidate();
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            if (isZoomed)
            {
                
            }
            base.OnScroll(se);
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
            numOfBites = 0;

            //read the the audio data:
            readThroughData();

            //display results:
            Font f = new Font(FontFamily.GenericSansSerif, 12);
            Brush b = new SolidBrush(Color.Red);
            //e.Graphics.DrawString("Samples Per Pixel (Visual): " + samplesPerPixel.ToString(), f, b, new Point(0, 10));
            e.Graphics.DrawString("Amount of Chews: " + numOfChews.ToString() + "\tAmount of Chews (2): " + numOfChews2.ToString() + "\tAmount of Bites: " + numOfBites.ToString(), f, b, new Point(0, this.Height - 20));
         
            //drawable wave stream:
            int sampleCount = 0;
            if (waveStream != null)
            {
                waveStream.Position = 0;
                int bytesRead;
                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];
                waveStream.Position = startPosition + (0 * bytesPerSample * samplesPerPixel);
                
                for (float x = 101; x < this.Width - 50; x += 1)
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
                    e.Graphics.DrawLine(Pens.Black, x, ((this.Height - 130) * lowPercent) + 30, x, ((this.Height - 130) * highPercent) + 30 );
                    sampleCount++;
                    if (lowestVal > low)
                    {
                        lowestVal = low;
                    }
                    if (highestVal < high)
                    {
                        highestVal = high;
                    }
                }

            }

            // drawing the graph lines:
            //Veritcal:
            e.Graphics.DrawLine(Pens.Black, new Point(100, 30), new Point(100, this.Height - 100));
            //horizontal:
            e.Graphics.DrawLine(Pens.Black, new Point(100, this.Height - 100), new Point(this.Width - 50, this.Height - 100));



            //draw Amplitude values:
            int length = this.Height - 100 - 30;

            // work out if the lowest or the highest value is the largest amplitude value for the file:
            int largestAmpValue = 0;
            if ( (lowestVal * -1) < highestVal)
            {
                largestAmpValue = highestVal;
            }
            else
            {
                largestAmpValue = lowestVal * -1;
            }

            //zero:
            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.5f)), new PointF(100, 30 + (length * 0.5f)));
            e.Graphics.DrawString("0", f, b, new Point(20, ((this.Height - 100) / 2) + 4));

            // +- a quater
            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.375f)), new PointF(100, 30 + (length * 0.375f)));
            e.Graphics.DrawString((largestAmpValue / 4).ToString(), f, b, new PointF(20, ((this.Height - 100) * 0.375f) + 4));

            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.625f)), new PointF(100, 30 + (length * 0.625f)));
            e.Graphics.DrawString((largestAmpValue / 4).ToString(), f, b, new PointF(20, ((this.Height - 100) * 0.625f) + 4));

            // +- a half
            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.25f)), new PointF(100, 30 + (length * 0.25f)));
            e.Graphics.DrawString((largestAmpValue / 2).ToString(), f, b, new PointF(20, ((this.Height - 100) / 4) + 4));

            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.75f)), new PointF(100, 30 + (length * 0.75f)));
            e.Graphics.DrawString((largestAmpValue / 2).ToString(), f, b, new PointF(20, ((this.Height - 100) * 0.75f) + 4));

            // +- three quarters
            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.125f)), new PointF(100, 30 + (length * 0.125f)));
            e.Graphics.DrawString((largestAmpValue * 0.75f).ToString(), f, b, new PointF(20, ((this.Height - 100) * 0.125f) + 4));

            e.Graphics.DrawLine(Pens.Black, new PointF(90, 30 + (length * 0.875f)), new PointF(100, 30 + (length * 0.875f)));
            e.Graphics.DrawString((largestAmpValue * 0.75f).ToString(), f, b, new PointF(20, ((this.Height - 100) * 0.875f) + 4));

            // +- full
            e.Graphics.DrawLine(Pens.Black, new Point(90, 30), new Point(100, 30));
            e.Graphics.DrawString((largestAmpValue).ToString(), f, b, new PointF(20, (30 - 10)));

            e.Graphics.DrawLine(Pens.Black, new Point(90, (this.Height - 100)), new Point(100, (this.Height - 100)));
            e.Graphics.DrawString((largestAmpValue).ToString(), f, b, new PointF(20, (this.Height - 100) - 10));



            // Drawing time values:

            double sampleToSeconds = 44100.00 / samplesPerPixel;
            double fullTime = sampleCount / sampleToSeconds;

            length = this.Width - 50 - 100;

            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.1f), this.Height - 100), new PointF(100 + (length * 0.1f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.1).ToString("0.00"), f, b, new PointF(80 + (length * 0.1f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.2f), this.Height - 100), new PointF(100 + (length * 0.2f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.2).ToString("0.00"), f, b, new PointF(80 + (length * 0.2f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.3f), this.Height - 100), new PointF(100 + (length * 0.3f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.3).ToString("0.00"), f, b, new PointF(80 + (length * 0.3f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.4f), this.Height - 100), new PointF(100 + (length * 0.4f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.4).ToString("0.00"), f, b, new PointF(80 + (length * 0.4f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.5f), this.Height - 100), new PointF(100 + (length * 0.5f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.5).ToString("0.00"), f, b, new PointF(80 + (length * 0.5f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.6f), this.Height - 100), new PointF(100 + (length * 0.6f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.6).ToString("0.00"), f, b, new PointF(80 + (length * 0.6f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.7f), this.Height - 100), new PointF(100 + (length * 0.7f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.7).ToString("0.00"), f, b, new PointF(80 + (length * 0.7f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.8f), this.Height - 100), new PointF(100 + (length * 0.8f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.8).ToString("0.00"), f, b, new PointF(80 + (length * 0.8f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 0.9f), this.Height - 100), new PointF(100 + (length * 0.9f), this.Height - 90));
            e.Graphics.DrawString((fullTime * 0.9).ToString("0.00"), f, b, new PointF(80 + (length * 0.9f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100 + (length * 1.0f), this.Height - 100), new PointF(100 + (length * 1.0f), this.Height - 90));
            e.Graphics.DrawString(fullTime.ToString("0.00"), f, b, new PointF(80 + (length * 1.0f), this.Height - 80));
            e.Graphics.DrawLine(Pens.Black, new PointF(100, this.Height - 100), new PointF(100, this.Height - 90));
            e.Graphics.DrawString(("0.00").ToString(), f, b, new PointF(80, this.Height - 80));

            // Plotting Chew Points (Commented out until i fix it up):
            /* 
            float divisor = samplesPerPixel / constSamplesPerPixel;

            for (int i = 0; i < drawableCoords.Count; i++)
            {
                float trueXCoord = (drawableCoords[i] / divisor);
                e.Graphics.DrawLine(Pens.Red, 100 + trueXCoord, this.Height / 2, 100 + trueXCoord, this.Height / 2 - 10);
            }

            for (int i = 0; i < drawableCoords2.Count; i++)
            {
                float trueXCoord = drawableCoords2[i] / divisor;
                e.Graphics.DrawLine(Pens.Green, 100 + trueXCoord, this.Height / 2 - 50 , 100 + trueXCoord, this.Height / 2 - 60);
            }*/

            base.OnPaint(e);
        }

        private void detectChew(short highestSampleValue, int xValue)
        {
            if (highestSampleValue > globalHighest)
            {
                    detectingChew = true;
                    globalHighest = highestSampleValue;
                    counter = 0;
                    xTemp = xValue + 1;
            }
            else if (highestSampleValue < globalHighest)
            {
                if (detectingChew)
                {
                    counter++;
                    if (counter == 8)
                    {
                        counter = 0;
                        numOfChews++;
                        detectingChew = false;
                        drawableCoords.Add(xTemp);
                        return;
                    }
                }
                else
                {
                    globalHighest = highestSampleValue;
                }
            }
            return;
        }

        private void detectChew2(short highestSampleValue, int xValue)
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
                    if (globalHighest2 - highestSampleValue > 550)
                    {
                        numOfChews2++;
                        detectingChew2 = false;
                        drawableCoords2.Add(xTemp2);
                        return;
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
            return;
        }






        private bool DetectBite(short highestSampleValue, int xValue)
        {
            



            if (highestSampleValue > globalHighest3)
            {
                globalHighest3 = highestSampleValue;
                detectingChew3 = true;
            

            }


            else if (highestSampleValue < globalHighest3)
            {

                if (detectingChew3)
                {
                    if (globalHighest3 - highestSampleValue > 5500)
                    {
                        numOfBites++;
                        detectingChew3 = false;
                      
                        return true;
                    }
                
                }
                else
                {
                    globalHighest3 = highestSampleValue;
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
                    DetectBite(high, x);


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
            this.SuspendLayout();
            // 
            // CustomWaveViewer
            // 
            this.Name = "CustomWaveViewer";
            this.Size = new System.Drawing.Size(483, 374);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
