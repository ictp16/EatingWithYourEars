using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;

namespace EatingWithYourEars
{
    public partial class Client : Form
    {
        //For playing audio:
        private WaveFileReader waveReader = null; // data source (File)
        private DirectSoundOut waveOutput = null; // data sink (headset)

        public Client()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openWave = new OpenFileDialog();
            String exeDir = Path.GetDirectoryName(Application.ExecutablePath.ToString());
            exeDir = Directory.GetParent(exeDir).ToString();
            exeDir = Directory.GetParent(exeDir).ToString();

            exeDir = exeDir + @"\Audio_Files\";

            openWave.InitialDirectory = exeDir;
            openWave.Filter = "Wave File (*.wav)|*.wav;";
           
            if (openWave.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string chopPath = openWave.FileName.Split('\\')[openWave.FileName.Split('\\').Length - 1];
            NameField.Text = chopPath;

            // ready track for playing:
            disposeWaveTrack();
            waveReader = new WaveFileReader(openWave.FileName);
            byte[] b = new byte[16000];
            waveReader.Read(b, 0, 15000);

            // setup custom wave viewer and draw graph:
            WaveGraph.isZoomed = false;
            fullFileToolStripMenuItem.Checked = true;
            WaveGraph.WaveStream = new WaveFileReader(openWave.FileName);
            WaveGraph.fitToGraph();
            openWave.Dispose();
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            WaveGraph.Dispose();
            disposeWaveTrack();
        }

        private void fullFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fullFileToolStripMenuItem.Checked)
            {
                fullFileToolStripMenuItem.Checked = false;
                WaveGraph.isZoomed = true;
                WaveGraph.SamplesPerPixel = WaveGraph.constSamplesPerPixel;
                WaveGraph.Invalidate();
            }
            else
            {
                fullFileToolStripMenuItem.Checked = true;
                WaveGraph.isZoomed = false;
                WaveGraph.fitToGraph();
                WaveGraph.Invalidate();
            }
        }

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            if (PlayPauseButton.Text == "Play")
            {
                PlayPauseButton.Text = "Pause";
                // Play the song!
                playTrack();
            }
            else if (PlayPauseButton.Text == "Pause")
            {
                PlayPauseButton.Text = "Play";
                // Pause the song!
                pauseTrack();
            }

        }

        private void TrackSpeedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // pause the song

            // play the song at a new speed.
        }

        private void pauseTrack()
        {
            if (waveOutput != null)
            {
                if (waveOutput.PlaybackState == PlaybackState.Playing)
                {
                    waveOutput.Pause();
                }
            }
        }

        private void playTrack()
        {
            if (waveOutput == null)
            {
                if (waveReader != null)
                {
                    waveOutput = new DirectSoundOut();
                    waveOutput.Init(new WaveChannel32(waveReader));
                    waveOutput.Play();
                }
            }

            else if (waveOutput != null)
            {
                if (waveOutput.PlaybackState == PlaybackState.Paused)
                {
                    waveOutput.Play();
                }
            }
        }

        private void disposeWaveTrack()
        {
            if (waveOutput != null)
            {
                if (waveOutput.PlaybackState == PlaybackState.Playing)
                {
                    waveOutput.Stop();
                }
                waveOutput.Dispose();
                waveOutput = null;

            }
            if (waveReader != null)
            {
                waveReader.Dispose();
                waveReader = null;
            }
        }


    }
}
