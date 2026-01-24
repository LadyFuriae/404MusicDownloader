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
        private AdvertiseMessage(string text)
        {
            InitializeComponent();
            this.label1.Text = text;

        }
        static public void Open(string text)
        {
            Task.Run(() =>
            {
                AdvertiseMessage msg = new AdvertiseMessage(text);
                msg._timer.Interval = PROCESSINGTIMEOUT;
                msg._timer.Tick += (sender, e) =>
                {
                    msg._timer.Stop();
                    msg.Close();
                };
                msg._timer.Start();
                msg.ShowDialog();
            });
        }
        public readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        public const int PROCESSINGTIMEOUT = 4000;

        private void AdvertiseMessage_Load(object sender, EventArgs e)
        {

        }
    }
}
