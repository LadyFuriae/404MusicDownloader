using _404MusicDownloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Playlists;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            LabelPath.Text = Manager.FolderPath;
            FormatConverter.SetFormats();
            FillComboBox();
            FormatComboBox.Text = FormatConverter.Formats.Keys.First();
            System.Windows.Forms.ToolTip PlayListMessage = new System.Windows.Forms.ToolTip();

            PlayListMessage.AutoPopDelay = 5000; 
            PlayListMessage.InitialDelay = 50;  
            PlayListMessage.ReshowDelay = 100;   
            PlayListMessage.ShowAlways = true;

            PlayListMessage.SetToolTip(this.PlaylistCheckBox, MSG_TOOL_TIP_PLAYLIST);
            NumberGenerator = new Random();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {

            string Format = FormatComboBox.Text;
            
            if (string.IsNullOrEmpty(Manager.FolderPath)) 
            {
                MessageBox.Show("Selecciona una carpeta antes de descargar");
                return;
            }

            if (string.IsNullOrEmpty(Link.Text))
            {
                MessageBox.Show("Introduce un link antes de descargar.");
                return;
            }

            if (string.IsNullOrEmpty(FormatComboBox.Text)) 
            {
                MessageBox.Show("Selecciona un formato antes de descargar");
                return;
            }

            if (!Manager.CheckIfValidURL(Link.Text))
            {
                Debug.WriteLine("No es un link de youtube");
                MessageBox.Show("Link no válido. Por favor verifique la URL e inténtelo de nuevo");
                return;
            }

            if (PlaylistCheckBox.Checked && Manager.IsFromPlayList(Link.Text))
            {
                Debug.WriteLine("Este vídeo es una playlist");
                ManagePlayList(Format);
                return;
            }

            ManageSingleVideo(Format);
        }

        public async Task ManagePlayList(string Format) 
        {
            Task.Run(async () =>
            {
                AdvertiseMessage.Open(WINDOW_SEARCHING_PLAYLIST);
                PlayListResult Result = new PlayListResult();
                Result.CheckPlaylist(Link.Text, Manager.Client); 

                if (!string.IsNullOrEmpty(Result.Message))
                {
                    MessageBox.Show(Result.Message);
                    return;
                }

                while (await Result.Enum.MoveNextAsync()) 
                {
                    REQUESTS++; 
                    if (REQUESTS >= 50) 
                    { 
                        REQUESTS = 0;
                        int delay = NumberGenerator.Next(5, 15);
                        await Task.Delay(delay * 1000);
                    }

                    PlaylistVideo Video = Result.SearchVideoFromPlayList();
                    ListViewItem item = new ListViewItem(Video.Title);

                    item.SubItems.Add(MSG_WAITING);
                    DownloadQueue.BeginInvoke(new Action(() =>
                    {
                        DownloadQueue.Items.Add(item);
                    }));

                    Task.Run(async () => 
                    {
                        Semaphore.Wait();
                        VideoResult PreparedSong = await Manager.SearchPlaylistVideoStream(Video);
                        
                        if (!String.IsNullOrEmpty(PreparedSong.Message))
                        {
                            DownloadQueue.BeginInvoke(new Action(() => 
                            {
                                item.SubItems[UI_STATUS_INDEX].Text = PreparedSong.Message;
                            }));
                            return;
                        }

                        DownloadQueue.Invoke(new Action(() =>
                        {
                            item.SubItems[UI_STATUS_INDEX].Text = MSG_DOWNLOADING;
                        }));
                        await Manager.StartDownload(PreparedSong.Video);
                        DownloadQueue.Invoke(new Action(() =>
                        {
                            item.SubItems[UI_STATUS_INDEX].Text = MSG_CONVERSION_PROGRESS + Format + "...";
                        }));

                        Manager.ConvertSong(PreparedSong.Video, Format);

                        DownloadQueue.BeginInvoke(new Action(() => { item.SubItems[UI_STATUS_INDEX].Text = MSG_SUCCESS; }));
                        Semaphore.Release();
                    });
                }
            });

        }
        private async Task ManageSingleVideo(string Format) 
        {
            Task.Run(async () =>
            {

                AdvertiseMessage.Open(WINDOW_SEARCHING_VIDEO);

                VideoResult Song = await Manager.GetMetaData(Link.Text);

                if (!string.IsNullOrEmpty(Song.Message))
                {
                    MessageBox.Show(Song.Message);
                    return;
                }

                ListViewItem item = new ListViewItem(Song.Video.FormatedSongName);

                item.SubItems.Add(MSG_WAITING);

                DownloadQueue.Invoke(new Action(() =>
                {
                    DownloadQueue.Items.Add(item);
                }));

                Semaphore.Wait();

                await Manager.SearchStreams(Song);
                if (!String.IsNullOrEmpty(Song.Message))
                {
                    DownloadQueue.Invoke(new Action(() =>
                    {
                        item.SubItems[UI_STATUS_INDEX].Text = Song.Message;
                    }));
                    return;
                }


                await Manager.StartDownload(Song.Video);
                Semaphore.Release();
                DownloadQueue.Invoke(new Action(() =>
                {
                    Format = FormatComboBox.Text;
                    item.SubItems[UI_STATUS_INDEX].Text = MSG_CONVERSION_PROGRESS + Format + "...";
                }));

                Manager.ConvertSong(Song.Video, Format);

                DownloadQueue.BeginInvoke(new Action(() => { item.SubItems[UI_STATUS_INDEX].Text = MSG_SUCCESS; }));
            });

        }

        private void FillComboBox()
        {
            foreach (var Format in FormatConverter.Formats.Keys)
            {
                FormatComboBox.Items.Add(Format);
            }
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
                LabelPath.Text = Manager.FolderPath;
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
                File.Delete(FormatConverter.FormatFinalPath);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

      

        private void DownloadQueue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Link_TextChanged(object sender, EventArgs e)
        {

        }

        private void Link_Click(object sender, EventArgs e)
        {
            Link.SelectAll();
        }
        private const short  UI_STATUS_INDEX = 1;
        private const string MSG_SEARCHING = "Buscando vídeo...";
        private const string MSG_DOWNLOADING = "Descargando...";
        private const string MSG_WAITING = "Esperando...";
        private const string MSG_CONVERSION_PROGRESS = "Convirtiendo a ";
        private const string MSG_SUCCESS = "¡Completado!";

        private const string MSG_TOOL_TIP_PLAYLIST = "Al marcar esta opción, descargarás todos los videos de una playlist insertando el link de un vídeo perteneciente a esa playlist";
        private const string WINDOW_SEARCHING_VIDEO = "Buscando vídeo. Por favor espere";
        private const string WINDOW_SEARCHING_PLAYLIST = "Buscando Playlist... Por favor sea paciente";
        private static short REQUESTS = 0;

        private SemaphoreSlim Semaphore = new SemaphoreSlim(8, 8);
        Random NumberGenerator;

    }
}

