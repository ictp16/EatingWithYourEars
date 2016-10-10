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
                WaveGraph.SamplesPerPixel = 1764;
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

    }
}
