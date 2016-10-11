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
            WaveGraph.isZoomed = false;
            fullFileToolStripMenuItem.Checked = true;
            WaveGraph.WaveStream = new WaveFileReader(openWave.FileName);
            WaveGraph.fitToGraph();
            openWave.Dispose();
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            WaveGraph.Dispose();
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
            }
            else if (PlayPauseButton.Text == "Pause")
            {
                PlayPauseButton.Text = "Play";
                // Pause the song!
            }

        }

        private void TrackSpeedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // pause the song

            // play the song at a new speed.
        }

        private void pauseTrack()
        {

        }

        private void playTrack()
        {

        }


    }
}
