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
        private int numOfChews = 0;

        //for detectChew2(short highestChewValue)
        private int numOfChews2 = 0;

        //for DetectBite(short highestChewValue)
        
        private int numOfBites = 0;
       

        //For DataAllAvg no touchies
        private int AvgBiteCount = 0;
        private int AvgChewCount = 0;
        private int Avg = 0;

        //Drawing:


        //constant samplesPerPixel value (usid in readData):
        public int constSamplesPerPixel = 1764; //1764

        // flag for view samplesize:
        public bool isZoomed = false;

        // for scrolling:
        private int drawPosition = 0;
        private int constSampleCount = 0;

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
        private float lowVariableForLiam = 120.0f, highVariableForLiam = 80.0f;

        // variables for trackBar:
        public bool showTrackBar = false;
        public volatile float trackBarX = 0;

        //Drawing POI's:
        private List<List<int>> chewingAnalysisLists = new List<List<int>>(); // A list containing lists of x co-ords for the various analysis methods.
        private List<Brush> brushList = new List<Brush>();
        private Brush chewingMethodBrush1 = null, chewingMethodBrush2 = null, biteMethodBrush1 = null, chewingMethodBrush3 = null, biteMethodBrush2 = null;
        private int radius = 4; // how big the dots will appear.

        //show/hide data:
        public List<int> dataToHide = new List<int>();

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
            resetBrushList();
            
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
            samplesPerPixel = samples / (rightOffset - leftOffset);
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
                case 0: 
                    
                    if (drawPosition + amount > constSampleCount - (rightOffset - leftOffset - 2))
                    {
                        drawPosition = constSampleCount - (rightOffset - leftOffset - 2);
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

        private void resetBrushList()
        {
            chewingMethodBrush1 = new SolidBrush(Color.DarkOrange);
            brushList.Add(chewingMethodBrush1);
            chewingMethodBrush2 = new SolidBrush(Color.Red);
            brushList.Add(chewingMethodBrush2);            
            chewingMethodBrush3 = new SolidBrush(Color.DarkSeaGreen);
            brushList.Add(chewingMethodBrush3);
            biteMethodBrush1 = new SolidBrush(Color.Goldenrod);
            brushList.Add(biteMethodBrush1);
            biteMethodBrush2 = new SolidBrush(Color.Fuchsia);
            brushList.Add(biteMethodBrush2);
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
            leftOffset = 150;
            rightOffset = this.Width - 50;
            topOffset = 30;
            bottomOffset = this.Height - 100;

            //reset num of chews in case of repaint:
            numOfChews = 0;
            numOfChews2 = 0;
            numOfBites = 0;
            AvgBiteCount = 0;
            AvgChewCount = 0;
            highestVal = 0;
            lowestVal = 0;
            constSampleCount = 0;
            chewingAnalysisLists.Clear();
            brushList.Clear();
            resetBrushList();

            //read the the audio data:
            readThroughData();

            //display results:
            Font f = new Font(FontFamily.GenericSansSerif, 12);
            Brush b = new SolidBrush(Color.Red);

            e.Graphics.DrawString("Amount of Chews: " + numOfChews.ToString(), f, chewingMethodBrush1, 0, this.Height - 40);
            e.Graphics.DrawString("Amount of Chews (Alternate 2nd Method): " + numOfChews2.ToString(), f, chewingMethodBrush2, 0, this.Height - 20);
            e.Graphics.DrawString("Amount of Bites (third method): " + AvgBiteCount.ToString(), f, biteMethodBrush1, 380, this.Height - 20);
            e.Graphics.DrawString("Amount of Chews (third method): " + AvgChewCount.ToString(), f, chewingMethodBrush3, 380, this.Height - 40);
            e.Graphics.DrawString("Amount of Bites (fourth method): " + numOfBites.ToString(), f, biteMethodBrush2, 850, this.Height - 20); 

            // work out if the lowest or the highest value is the largest amplitude value for the file:
            float largestAmpValue = 0;
            if ((lowestVal * -1) < highestVal)
            {
                largestAmpValue = highestVal;
            }
            else
            {
                largestAmpValue = lowestVal * -1;
            }

            

            //drawable wave stream:
            int sampleCount = 0;
            if (waveStream != null)
            {
                if (!isZoomed)
                {
                    drawPosition = 0;
                }
                waveStream.Position = 0;
                int bytesRead;
                byte[] waveData = new byte[samplesPerPixel * bytesPerSample];
                waveStream.Position = startPosition + (drawPosition * bytesPerSample * samplesPerPixel);
                
                for (float x = leftOffset + 1; x < rightOffset; x += 1)
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
                    
                    // legacy drawing method:
                    /*
                    float lowPercent = ((((float)low) - short.MinValue) / ushort.MaxValue);
                    float highPercent = ((((float)high) - short.MinValue) / ushort.MaxValue);
                    e.Graphics.DrawLine(Pens.Black, x, ((this.Height - 130) * lowPercent) + 30, x, ((this.Height - 130) * highPercent) + 30 );
                    */

                    float highPercent = (float)topOffset + ( (1.0f - ((float)high / (float)largestAmpValue)) * ( (bottomOffset - topOffset) / 2.0f));
                    float lowPercent = (topOffset + ((bottomOffset - topOffset) / 2.0f)) - ( ((float)low / (float)largestAmpValue) * ( (bottomOffset - topOffset) / 2.0f) );
                    e.Graphics.DrawLine(Pens.Black, x, highPercent, x, lowPercent);
                    sampleCount++;
                   
                }

            }

            // drawing the graph lines:
            //Veritcal:
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset, topOffset), new Point(leftOffset, bottomOffset));
            StringFormat drawFormat = new StringFormat(StringFormatFlags.DirectionVertical);
            e.Graphics.DrawString("Amplidtude", f, b, new Point(leftOffset - 130 ,(bottomOffset / 2) - 30), drawFormat);
            
            //horizontal:
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset, bottomOffset), new Point(rightOffset, bottomOffset));
            e.Graphics.DrawString("Time (Seconds)", f, b, new Point(leftOffset + ((rightOffset - leftOffset) / 2) - 100, bottomOffset + 50));


            // resizing the y scale depending if zommed in or not:
            if (isZoomed)
            {
                int samples = (int)(waveStream.Length / bytesPerSample);
                float screenSamples = samples / (this.Width - 50.0f - 101.0f);
                largestAmpValue = largestAmpValue * ((float)constSamplesPerPixel / screenSamples);
            }

            //draw Amplitude values:
            int length = bottomOffset - topOffset;


            //zero:
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.5f)), new PointF(leftOffset, topOffset + (length * 0.5f)));
            e.Graphics.DrawString("0.00", f, b, new PointF(leftOffset - 90, topOffset + (length * 0.5f) - 10));

            // +- a quater
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.375f)), new PointF(leftOffset, topOffset + (length * 0.375f)));
            e.Graphics.DrawString((largestAmpValue / 4).ToString("0.00"), f, b, new PointF(leftOffset - 90, (topOffset + (length * 0.375f)) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.625f)), new PointF(leftOffset, topOffset + (length * 0.625f)));
            e.Graphics.DrawString("-" + (largestAmpValue / 4).ToString("0.00"), f, b, new PointF(leftOffset - 90, (topOffset + (length * 0.625f)) - 10));

            // +- a half
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.25f)), new PointF(leftOffset, topOffset + (length * 0.25f)));
            e.Graphics.DrawString((largestAmpValue / 2).ToString("0.00"), f, b, new PointF(leftOffset - 90, topOffset + (length * 0.25f) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.75f)), new PointF(leftOffset, topOffset + (length * 0.75f)));
            e.Graphics.DrawString("-" + (largestAmpValue / 2).ToString("0.00"), f, b, new PointF(leftOffset - 90, topOffset + (length * 0.75f) - 10));

            // +- three quarters
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.125f)), new PointF(leftOffset, topOffset + (length * 0.125f)));
            e.Graphics.DrawString((largestAmpValue * 0.75f).ToString("0.00"), f, b, new PointF(leftOffset - 90, topOffset + (length * 0.125f) - 10));

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset - 10, topOffset + (length * 0.875f)), new PointF(leftOffset, topOffset + (length * 0.875f)));
            e.Graphics.DrawString("-" + (largestAmpValue * 0.75f).ToString("0.00"), f, b, new PointF(leftOffset - 90, topOffset + (length * 0.875f) - 10 ));

            // +- full
            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset - 10, topOffset), new Point(leftOffset, topOffset));
            e.Graphics.DrawString((largestAmpValue).ToString("0.00"), f, b, new PointF(leftOffset - 90, (topOffset - 10)));

            e.Graphics.DrawLine(Pens.Black, new Point(leftOffset - 10, bottomOffset), new Point(leftOffset, bottomOffset));
            e.Graphics.DrawString("-" + (largestAmpValue).ToString("0.00"), f, b, new PointF(leftOffset - 90, bottomOffset - 10));


            // Drawing time values:
            double sampleToSeconds = 44100.00 / (double)samplesPerPixel;
            double fullTime = (double)sampleCount / sampleToSeconds;
            double scrollAddition = 0;

            if (isZoomed)
            {
                scrollAddition = drawPosition / sampleToSeconds;
            }
            length = rightOffset - leftOffset;

            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.1f), bottomOffset), new PointF(leftOffset + (length * 0.1f), bottomOffset + 10));
            e.Graphics.DrawString(( (fullTime * 0.1) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.1f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.2f), bottomOffset), new PointF(leftOffset + (length * 0.2f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.2) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.2f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.3f), bottomOffset), new PointF(leftOffset + (length * 0.3f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.3) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.3f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.4f), bottomOffset), new PointF(leftOffset + (length * 0.4f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.4) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.4f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.5f), bottomOffset), new PointF(leftOffset + (length * 0.5f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.5) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.5f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.6f), bottomOffset), new PointF(leftOffset + (length * 0.6f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.6) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.6f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.7f), bottomOffset), new PointF(leftOffset + (length * 0.7f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.7) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.7f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.8f), bottomOffset), new PointF(leftOffset + (length * 0.8f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.8) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.8f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 0.9f), bottomOffset), new PointF(leftOffset + (length * 0.9f), bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.9) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 0.9f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset + (length * 1.0f), bottomOffset), new PointF(leftOffset + (length * 1.0f), bottomOffset + 10));
            e.Graphics.DrawString( (fullTime + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20 + (length * 1.0f), bottomOffset + 20));
            e.Graphics.DrawLine(Pens.Black, new PointF(leftOffset, bottomOffset), new PointF(leftOffset, bottomOffset + 10));
            e.Graphics.DrawString(((fullTime * 0.0) + scrollAddition).ToString("0.00"), f, b, new PointF(leftOffset - 20, bottomOffset + 20));

            //plotting chew threshhold:
            if (lowVariableForLiam != 0 && highVariableForLiam != 0 && largestAmpValue != 0)
            {
                lowVariableForLiam = bottomOffset / 2 - ((lowVariableForLiam / largestAmpValue) * ((bottomOffset / 2) - topOffset));
                highVariableForLiam = bottomOffset / 2 - ((highVariableForLiam / largestAmpValue) * ((bottomOffset / 2) - topOffset));

                //MessageBox.Show("low:" + lowVariableForLiam + " High: " + highVariableForLiam);
                e.Graphics.DrawLine(Pens.MediumPurple, new PointF(leftOffset, lowVariableForLiam), new PointF(rightOffset, lowVariableForLiam));
                e.Graphics.DrawLine(Pens.MediumPurple, new PointF(leftOffset, highVariableForLiam), new PointF(rightOffset, highVariableForLiam));
            }
            
            // Drawing Track Bar:
            if (showTrackBar)
            {
                if (!isZoomed)
                {
                    e.Graphics.DrawLine(Pens.Blue, new PointF((float)leftOffset + (((float)rightOffset - (float)leftOffset) * trackBarX), (float)topOffset), new PointF((float)leftOffset + (((float)rightOffset - (float)leftOffset) * trackBarX), (float)topOffset + (((float)bottomOffset - (float)topOffset) / 2.0f)));
                }

                else
                {
                    // state 1 (move to halfway on the graph):
                    float barXPos = ((float)trackBarX * ((float)waveStream.Length / ((float)constSamplesPerPixel * 4.0f))) - drawPosition;
                    if (barXPos >= (rightOffset - leftOffset))
                    {
                        scroll(0, (rightOffset - leftOffset));
                    }
                    e.Graphics.DrawLine(Pens.Blue, new PointF((float)leftOffset + barXPos, (float)topOffset), new PointF((float)leftOffset + barXPos, (float)topOffset + (((float)bottomOffset - (float)topOffset) / 2.0f)));
                }
            }



            // Plotting Chew Points (Commented out until i fix it up):

            for (int i = dataToHide.Count - 1; i > -1; i--)
            {
                    chewingAnalysisLists.RemoveAt(dataToHide[i]);
                    brushList.RemoveAt(dataToHide[i]);
            }

            float multiplier = (float)constSamplesPerPixel / (float)samplesPerPixel;

            for (int i = 0; i < chewingAnalysisLists.Count; i++)
            {
                for (int j = 0; j < chewingAnalysisLists[i].Count; j++)
                {
                    if (isZoomed)
                    {
                        if (chewingAnalysisLists[i][j] < drawPosition)
                        {
                            continue;
                        }
                        if (chewingAnalysisLists[i][j] > drawPosition + rightOffset - leftOffset)
                        {
                            break;
                        }
                        e.Graphics.FillEllipse(brushList[i], new Rectangle(leftOffset + chewingAnalysisLists[i][j] - drawPosition - radius, topOffset - radius + (i * 10), radius * 2, radius * 2));
                    }
                    else
                    {
                        e.Graphics.FillEllipse(brushList[i], new RectangleF((float)leftOffset + ((float)chewingAnalysisLists[i][j] * multiplier) - (radius / 2.0f), (float)topOffset - (radius / 2.0f) + (i * 5), radius, radius));
                    }
                }
            }

            //float divisor = samplesPerPixel / constSamplesPerPixel;

            base.OnPaint(e);
        }

        private void detectChew(List<short> allData)
        {
            List<int> xDrawData = new List<int>();

            int highestValue = 0;
            int highestValueX = 0;
            bool detectingChew = false;
            int counter = 0;

            for (int i = 0; i < allData.Count; i++)
            {
                if (allData[i] > highestValue)
                {
                    detectingChew = true;
                    highestValue = allData[i];
                    highestValueX = i;
                    counter = 0;
                }

                else if (allData[i] < highestValue)
                {
                    if (detectingChew)
                    {
                        counter++;
                        if (counter == 8)
                        {
                            counter = 0;
                            numOfChews++;
                            detectingChew = false;
                            xDrawData.Add(highestValueX);
                        }
                    }
                    else
                    {
                        highestValue = allData[i];
                    }
                }
            }
            chewingAnalysisLists.Add(xDrawData);
            return;
        }

        private void detectChew2(List<short> allData)
        {
            List<int> xDrawData = new List<int>();
            int highestValue = 0;
            int highestValueX = 0;
            bool detectingChew = false;

            for (int i = 0; i < allData.Count; i++)
            {
                if (allData[i] > highestValue)
                {
                    highestValue = allData[i];
                    highestValueX = i;
                    detectingChew = true;
                }
                else if (allData[i] < highestValue)
                {
                    if (detectingChew)
                    {
                        if (highestValue - allData[i] > 550)
                        {
                            numOfChews2++;
                            detectingChew = false;
                            xDrawData.Add(highestValueX);
                        }
                    }
                    else
                    {
                        highestValue = allData[i];
                    }
                }
            }
            chewingAnalysisLists.Add(xDrawData);
            return;
        }

        private void AllDataAvg(List<short> data)
        {
            List<int> xDrawDataBite = new List<int>();
            List<int> xDrawDataChew = new List<int>();
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
            }
            int avg = (sum / data.Count);
            Avg = avg;

            for(int i = 0; i < data.Count; i++)
            {
                if (data[i] > avg * 12.5)
                {
                    highVariableForLiam = high;
                    lowVariableForLiam = avg;
                    AvgBiteCount++;
                    xDrawDataBite.Add(i);
                }
                if((data[i] < avg) && (data[i] > (avg / 1.25)))
                {
                    AvgChewCount++;
                    xDrawDataChew.Add(i);
                }
            }
            
            chewingAnalysisLists.Add(xDrawDataChew);
            chewingAnalysisLists.Add(xDrawDataBite);

        }




        private void DetectBite(List<short> data)
        {
            //figuring out second largest array value
            List<int> xDrawData = new List<int>();
            int largest = int.MinValue;
            int second = int.MinValue;
            bool detectingBite = false;
            int highestValue = 0;

            foreach (int j in data)
            {
                if (j > largest)
                {
                    second = largest;
                    largest = j;
                }
                else if (j > second)
                    second = j;
            }

            //figuring out range
            int range = 0;
            if (data.Max() > second * 1.5)
            { range = second-data.Min(); }
            else
            { range = data.Max() - data.Min(); }
            
            //figuring out average
            int sum = 0;
           
            for (int i = 0; i < data.Count; i++)
            {
                sum += Convert.ToInt32(data[i]);
            }
            int avg = (sum / data.Count);

            //average for peaks
            int PeakSum = 0;
            int counter = 0;

            for (int i = 0; i < data.Count; i++)
            { 
                if (data[i] > range*0.3)
                {
                    PeakSum += Convert.ToInt32(data[i]);
                    counter++;
                }
            }
            int PeakAvg = (PeakSum / counter);
            //MessageBox.Show("Peaksum:"+PeakSum.ToString() + "    avg: " + avg.ToString() + "    PeakAvg: " + PeakAvg.ToString() + "    counter: " + counter.ToString()+ "     range:" + range.ToString());

            //apple: 12 //carrot: 8 //cashews: 18 //dried prumes: 6



            if (avg > 480) //for overly LOUD & CRUNCHY foods e.g. carrot
            {

                for (int i = 0; i < data.Count; i++)
                {


                    if (data[i] > highestValue)
                    {
                        highestValue = data[i];
                        detectingBite = true;
                    }
                    else if (data[i] < highestValue)
                    {
                        if (detectingBite)
                        {
                            if (data[i] > PeakAvg)
                            {
                                numOfBites++;
                                xDrawData.Add(i);
                                detectingBite = false;
                            }
                        }
                        else
                        {
                            highestValue = data[i];
                        }

                    }
                }
            }
            
            
            else if (avg < 480) //seems to work well for softer foods
            {
                for (int i = 0; i < data.Count; i++)
                {

                    if (data[i] > highestValue)
                    {
                        highestValue = data[i];
                        detectingBite = true;
                    }
                    else if (data[i] < highestValue)
                    {

                        if (data[i] > PeakAvg*0.4 /* avg * 7.5*/)
                        {
                            
             /*the following conditions are checked to eliminate redundant detection of bites to a certain degree*/

                            if (data[i] - data[i + 1] < highestValue * 0.36) /*refinement (to eliminate redundant detections)*/
                            {
                                detectingBite = false;
                                
                            }
                            else if (data[i] - data[i + 1] > highestValue * 0.4) /*refinement (to eliminate redundant detections)*/
                            {
                                xDrawData.Add(i);
                                numOfBites++;
                                detectingBite = false;
                            }
                        }

                    }
                }

            }

            chewingAnalysisLists.Add(xDrawData);

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

                   
                    
                    AllData.Add(high);

                    if (waveStream.Position >= waveStream.Length - 1)
                    {
                        break;
                    }

                    if (lowestVal > low)
                    {
                        lowestVal = low;
                    }
                    if (highestVal < high)
                    {
                        highestVal = high;
                    }
                    constSampleCount++;

                }
                detectChew(AllData);
                detectChew2(AllData);
                AllDataAvg(AllData);
                DetectBite(AllData);

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
