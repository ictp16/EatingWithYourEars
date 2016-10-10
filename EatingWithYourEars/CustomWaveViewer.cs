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
        private List<int> DrawBiteLocation = new List<int>();

        //for detectChew2(short highestChewValue)
        private int counter2 = 0;
        private bool detectingChew2 = false;
        private int numOfChews2 = 0;
        private short globalHighest2 = 0;

        //for DetectBite(short highestChewValue)
        private bool detectingChew3 = false;
        private int numOfBites = 0;
        private short globalHighest3 = 0;

        //For DataAllAvg no touchies
        private int AvgBiteCount = 0;
        private int AvgChewCount = 0;

        //Drawing:
        private List<int> drawableCoords2 = new List<int>();
        private int xTemp2 = 0;

        //constant samplesPerPixel value (usid in readData):
        public int constSamplesPerPixel = 1764; //1764

        // flag for view samplesize:
        public bool isZoomed = false;

        // for scrolling:
        private int drawPosition = 0;

        /// <summary>
        /// Graphing:
        /// </summary>

        //determining the scale of the y axis:
        private int highestVal = 0;
        private int lowestVal = 0;

        // Padding For Graph (defined in onPaint):
        private int leftOffset = 0;
        private int rightOffset = 0;
        private int bottomOffset = 0;
        private int topOffset = 0;

        // threshold variables for liam:
        private float lowVariableForLiam = 120.0f, highVarableForLiam = 80.0f;


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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            
            //MessageBox.Show("Test");
            if (isZoomed)
            {
                if (e.Delta > 0)
                {
                    scroll(0, e.Delta);
                }
                else if (e.Delta < 0)
                {
                    scroll(1, e.Delta);
                }
            }
            base.OnMouseWheel(e);
        }

        private void scroll(int direction, int amount)
        {
            switch (direction)
            {
                default:
                case 0: // scrolling to the right
                   // Console.Write(drawPosition);
                   // Console.WriteLine("\t" + waveStream.Length);

                    if (drawPosition > waveStream.Length - 151)
                    {
                        drawPosition = (int)waveStream.Length - 150;
                        Console.WriteLine("TESSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSST");
                        break;
                    }
                    drawPosition += amount;
                    break;
                case 1: // scrolling to the left
                    if (drawPosition + amount < 0)
                    {
                        drawPosition = 0;
                        break;
                    }
                    drawPosition += amount;
                    break;
            }
            Invalidate();
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

            //defining graph padding:
            leftOffset = 100;
            rightOffset = this.Width - 50;
            topOffset = 30;
            bottomOffset = this.Height - 100;

            //reset num of chews in case of repaint:
            numOfChews = 0;
            numOfChews2 = 0;
            numOfBites = 0;
            AvgBiteCount = 0;
            AvgChewCount = 0;
            //read the the audio data:
            readThroughData();

            //display results:
            Font f = new Font(FontFamily.GenericSansSerif, 12);
            Brush b = new SolidBrush(Color.Red);
            //e.Graphics.DrawString("Samples Per Pixel (Visual): " + samplesPerPixel.ToString(), f, b, new Point(0, 10));
            e.Graphics.DrawString("Amount of Chews: " + numOfChews.ToString() + "\tAmount of Chews (2): " + numOfChews2.ToString() + "\tAmount of Bites: " + numOfBites.ToString() + "\t AllDataAvg: " + AvgBiteCount.ToString() + " " + AvgChewCount.ToString(), f, b, new Point(0, this.Height - 20));
            
            //drawable wave stream:
            int sampleCount = 0;
            if (waveStream != null)
            {
                waveStream.Position = 0;
                int bytesRead;
                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];
                waveStream.Position = startPosition + (drawPosition * bytesPerSample * samplesPerPixel);
                
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
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset, topOffset), new Point(leftOffset, bottomOffset));
            //horizontal:
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset, bottomOffset), new Point(rightOffset, bottomOffset));



            //draw Amplitude values:
            int length = bottomOffset - topOffset;

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
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.5f)), new PointF(leftOffset, topOffset + (length * 0.5f)));
            e.Graphics.DrawString("0", f, b, new PointF(leftOffset - 80, topOffset + (length * 0.5f) - 10));

            // +- a quater
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.375f)), new PointF(leftOffset, topOffset + (length * 0.375f)));
            e.Graphics.DrawString((largestAmpValue / 4).ToString(), f, b, new PointF(leftOffset - 80, (topOffset + (length * 0.375f)) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.625f)), new PointF(leftOffset, topOffset + (length * 0.625f)));
            e.Graphics.DrawString((largestAmpValue / 4).ToString(), f, b, new PointF(leftOffset - 80, (topOffset + (length * 0.625f)) - 10));

            // +- a half
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.25f)), new PointF(leftOffset, topOffset + (length * 0.25f)));
            e.Graphics.DrawString((largestAmpValue / 2).ToString(), f, b, new PointF(leftOffset - 80, topOffset + (length * 0.25f) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.75f)), new PointF(leftOffset, topOffset + (length * 0.75f)));
            e.Graphics.DrawString((largestAmpValue / 2).ToString(), f, b, new PointF(leftOffset - 80, topOffset + (length * 0.75f) - 10));

            // +- three quarters
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.125f)), new PointF(leftOffset, topOffset + (length * 0.125f)));
            e.Graphics.DrawString((largestAmpValue * 0.75f).ToString(), f, b, new PointF(leftOffset - 80, topOffset + (length * 0.125f) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.875f)), new PointF(leftOffset, topOffset + (length * 0.875f)));
            e.Graphics.DrawString((largestAmpValue * 0.75f).ToString(), f, b, new PointF(leftOffset - 80, topOffset + (length * 0.875f) - 10 ));

            // +- full
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset - 10, topOffset), new Point(leftOffset, topOffset));
            e.Graphics.DrawString((largestAmpValue).ToString(), f, b, new PointF(leftOffset - 80, (topOffset - 10)));

            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset - 10, bottomOffset), new Point(leftOffset, bottomOffset));
            e.Graphics.DrawString((largestAmpValue).ToString(), f, b, new PointF(leftOffset - 80, bottomOffset - 10));



            // Drawing time values:

            double sampleToSeconds = 44100.00 / samplesPerPixel;
            double fullTime = sampleCount / sampleToSeconds;

            length = rightOffset - leftOffset;

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.1f), bottomOffset), new PointF(leftOffset + (length * 0.1f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.1).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.1f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.2f), bottomOffset), new PointF(leftOffset + (length * 0.2f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.2).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.2f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.3f), bottomOffset), new PointF(leftOffset + (length * 0.3f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.3).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.3f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.4f), bottomOffset), new PointF(leftOffset + (length * 0.4f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.4).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.4f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.5f), bottomOffset), new PointF(leftOffset + (length * 0.5f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.5).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.5f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.6f), bottomOffset), new PointF(leftOffset + (length * 0.6f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.6).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.6f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.7f), bottomOffset), new PointF(leftOffset + (length * 0.7f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.7).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.7f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.8f), bottomOffset), new PointF(leftOffset + (length * 0.8f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.8).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.8f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.9f), bottomOffset), new PointF(leftOffset + (length * 0.9f), bottomOffset + 10));
            e.Graphics.DrawString((fullTime * 0.9).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.9f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 1.0f), bottomOffset), new PointF(leftOffset + (length * 1.0f), bottomOffset + 10));
            e.Graphics.DrawString(fullTime.ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 1.0f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset, bottomOffset), new PointF(leftOffset, bottomOffset + 10));
            e.Graphics.DrawString(("0.00").ToString(), f, b, new PointF(leftOffset - 20, bottomOffset + 20));

            //plotting chew threshhold:
            lowVariableForLiam += bottomOffset / 2;
            highVarableForLiam += bottomOffset / 2;
            e.Graphics.DrawLine(Pens.MediumPurple, new PointF(leftOffset, lowVariableForLiam), new PointF(rightOffset, lowVariableForLiam));
            e.Graphics.DrawLine(Pens.MediumPurple, new PointF(leftOffset, highVarableForLiam), new PointF(rightOffset, highVarableForLiam));
            
            
            // Plotting Chew Points (Commented out until i fix it up):
             
            float divisor = samplesPerPixel / constSamplesPerPixel;

            /*for (int i = 0; i < drawableCoords.Count; i++)
            {
                float trueXCoord = (drawableCoords[i] / divisor);
                e.Graphics.DrawLine(Pens.Red, 100 + trueXCoord, this.Height / 2 - 20, 100 + trueXCoord, this.Height / 2 - 30);
            }

            for (int i = 0; i < drawableCoords2.Count; i++)
            {
                float trueXCoord = drawableCoords2[i] / divisor;
                if (trueXCoord > this.Width - 50) ;
                e.Graphics.DrawLine(Pens.Green, 100 + trueXCoord, this.Height / 2 - 50 , 100 + trueXCoord, this.Height / 2 - 60);
            }
            */

            for (int i = 0; i < DrawBiteLocation.Count; i++)
            {
                float trueXCoord = DrawBiteLocation[i] / divisor;
                //if (trueXCoord > this.Width - 50);
                e.Graphics.DrawLine(Pens.Green, 100 + trueXCoord, this.Height / 2 - 100, 100 + trueXCoord, this.Height / 2 - 200);
            }
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

        private void AllDataAvg(List<short> data)
        {
            int sum = 0;
            int high = data[0];
            int low = data[0];

            for(int i = 0; i < data.Count; i++)
            {
                sum += Convert.ToInt32(data[i]);
                if(high < data[i])
                {
                    high = data[i];
                }
                if(low > data[i] && data[i] != 0)
                {
                    low = data[i];
                }
                //Console.WriteLine(high + " " + low);
                //Console.Out.WriteLine(data[i]);
            }
            int avg = (sum / data.Count);

            //For Checking the avg Amp for the local highs
            //Console.WriteLine(avg);

            for(int i = 0; i < data.Count; i++)
            {
                if (data[i] > avg * 12.5)
                {
                    AvgBiteCount++;
                    DrawBiteLocation.Add(i);
                }
                if((data[i] < avg) && (data[i] > (avg / 1.25)))
                {
                    AvgChewCount++;
                }
            }
            
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
                    if (globalHighest3 - highestSampleValue > 6000)
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
                List<short> AllData = new List<short>();
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
                    AllData.Add(high);

                    if (waveStream.Position >= waveStream.Length - 1)
                    {
                        break;
                    }
                }
                AllDataAvg(AllData);
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
