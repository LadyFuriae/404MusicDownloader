using _404MusicDownloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _404MusicDownloaderUI
{
    public partial class AdvertiseMessage : Form
    {
        public AdvertiseMessage(string text)
        {
            InitializeComponent();
            this.label1.Text = text;
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
        }
        public void Open()
        {
            _timer.Interval = PROCESSINGTIMEOUT;
            _timer.Tick += (sender, e) =>
            {
                _timer.Stop();
                this.Close();
            };
            _timer.Start();
            this.ShowDialog(); 
        }
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private const int PROCESSINGTIMEOUT = 3000;

        private void AdvertiseMessage_Load(object sender, EventArgs e)
        {

        }
    }
}
