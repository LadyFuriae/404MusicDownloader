using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using _404MusicDownloader;

namespace _404MusicDownloaderUI
{
    public partial class Form1 : Form
    {

        DownloadManager Manager;
        App App;
        public Form1()
        {
            InitializeComponent();
            Manager = new DownloadManager();
            App = new App();
            App.Run(Manager);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                string SongName = await Manager.SearchVideo(Link.Text);
                if (!DownloadQueue.InvokeRequired)
                    return;
                DownloadQueue.BeginInvoke(new Action(() => 
                {

                    ListViewItem item = new ListViewItem(SongName); // Columna 1: Título
                       
                    item.SubItems.Add("100%");      // Columna 2: Canal
                    DownloadQueue.Items.Add(item);
                }));
                
            });
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

