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
            openWave.Filter = "Wave File (*.wav)|*.wav;";
            if (openWave.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            WaveGraph.WaveStream = new WaveFileReader(openWave.FileName);
            WaveGraph.fitToScreen();
            openWave.Dispose();
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            WaveGraph.Dispose();
        }
    }
}
