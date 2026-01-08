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
using System.IO;

#pragma warning disable CS4014

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
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Manager.FolderPath)) 
            {
                MessageBox.Show("Selecciona una carpeta antes de descargar");
                return;
            }

            Task.Run(async () =>
            {

                VideoResult Song = await Manager.SearchVideo(Link.Text);

                if (Song.Video == null) 
                {
                    MessageBox.Show(Song.Message); 
                    return;  
                }

                if (!DownloadQueue.InvokeRequired)
                    return;

                ListViewItem item = new ListViewItem(Song.Video.FormatedSongName);

                item.SubItems.Add(MSG_DOWNLOADING);

                DownloadQueue.BeginInvoke(new Action(() =>
                {
                    DownloadQueue.Items.Add(item);
                }));

                await Manager.StartDownload(Song.Video);

                DownloadQueue.BeginInvoke(new Action(() => { item.SubItems[UI_STATUS_INDEX].Text = MSG_CONVERSION_PROGRESS; }));
                
                Manager.ConvertSong(Song.Video);

                DownloadQueue.BeginInvoke(new Action(() => { item.SubItems[UI_STATUS_INDEX].Text = MSG_SUCCESS; }));
            });
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FileExplorer = new FolderBrowserDialog();
            if(FileExplorer.ShowDialog() == DialogResult.OK)
            {
                Manager.FolderPath = FileExplorer.SelectedPath;
                Manager.ReplacePath(Manager.FolderPath);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            Manager.CancelDownloads();

            if (FormatConverter.FFMPEGPROCESS == null)
                return;

            //Program already Diposes the process. See ProcessAudio Method
            if (FormatConverter.FFMPEGPROCESS != null && !FormatConverter.FFMPEGPROCESS.HasExited)
            {
                FormatConverter.FFMPEGPROCESS.Kill();
                FormatConverter.FFMPEGPROCESS.WaitForExit();
                File.Delete(FormatConverter.MP3Finalpath);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private const short  UI_STATUS_INDEX = 1;
        private const string MSG_DOWNLOADING = "Descargando...";
        private const string MSG_CONVERSION_PROGRESS = "Convirtiendo a MP3...";
        private const string MSG_SUCCESS = "¡Completado!";

        private void DownloadQueue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

