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
using YoutubeExplode.Videos;
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
            RetryDownloadsButton.Enabled = true;
            CleanTasksButton.Enabled = true;


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
                MessageBox.Show("Link no válido. Por favor verifique la URL e inténtelo de nuevo");
                return;
            }

            if (PlaylistCheckBox.Checked && Manager.IsFromPlayList(Link.Text))
            {
                AdvertiseMessage.Open(WINDOW_SEARCHING_PLAYLIST);
                ManagePlayList(Format, Link.Text);
                return;
            }
            AdvertiseMessage.Open(WINDOW_SEARCHING_VIDEO);
            ManageSingleVideo(Format, Link.Text);
        }

        public async Task ManagePlayList(string Format, string URL) 
        {
            Task.Run(async () =>
            {
                PlayListResult Result = new PlayListResult();
                Result.CheckPlaylist(URL, Manager.Client); 

                if (!string.IsNullOrEmpty(Result.Message))
                {
                    MessageBox.Show(Result.Message);
                    return;
                }

                while (await Result.Enum.MoveNextAsync()) 
                {
                    REQUESTS++; 
                    if (REQUESTS >= MAX_REQUESTS_UNTIL_TIMEOUT) 
                    { 
                        REQUESTS = 0;
                        int delay = NumberGenerator.Next(5, 15);
                        await Task.Delay(delay * 1000);
                    }

                    PlaylistVideo Video = Result.SearchVideoFromPlayList();
                    ListViewItem item = new ListViewItem(Video.Title);

                    item.SubItems.Add(MSG_WAITING);
                    DownloadQueue.Invoke(new Action(() =>
                    {
                        DownloadQueue.Items.Add(item);
                    }));

                    Task.Run(async () => 
                    {
                        try
                        {
                            await Semaphore.WaitAsync();
                            VideoResult PreparedSong = await Manager.SearchPlaylistVideoStream(Video);

                            if (PreparedSong == null)
                            {
                                DownloadQueue.BeginInvoke(new Action(() =>
                                {
                                    item.SubItems[UI_STATUS_INDEX].Text = PreparedSong.Message;
                                }));
                                FailedTasks.Add(URL);
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
                        }
                        finally 
                        {
                            FinishedTasks.Add(item);
                            Semaphore.Release();
                        }
                    });

                }
            });

        }
        private async Task ManageSingleVideo(string Format, string URL) 
        {
            Task.Run(async () =>
            {
                VideoResult Song = await Manager.GetMetaData(URL);


                if (Song.Video == null)
                {
                    MessageBox.Show($"No se ha podido encontrar el vídeo: {URL}. Motivo: {Song.Message}");
                    return;
                }

                ListViewItem item = new ListViewItem(Song.Video.RawSongName);

                item.SubItems.Add(MSG_WAITING);

                DownloadQueue.Invoke(new Action(() =>
                {
                    DownloadQueue.Items.Add(item);
                }));

                try
                {
                    await Semaphore.WaitAsync();
                    await Manager.SearchStreams(Song);

                    if (Song.Video == null)
                    {
                        DownloadQueue.Invoke(new Action(() =>
                        {
                            item.SubItems[UI_STATUS_INDEX].Text = Song.Message;
                        }));
                        FailedTasks.Add(URL);
                        return;
                    }

                    await Manager.StartDownload(Song.Video);

                    DownloadQueue.Invoke(new Action(() =>
                    {
                        Format = FormatComboBox.Text;
                        item.SubItems[UI_STATUS_INDEX].Text = MSG_CONVERSION_PROGRESS + Format + "...";
                    }));

                    Manager.ConvertSong(Song.Video, Format);
                    DownloadQueue.BeginInvoke(new Action(() => { item.SubItems[UI_STATUS_INDEX].Text = MSG_SUCCESS; }));
                }
                finally 
                {
                    FinishedTasks.Add(item);
                    Semaphore.Release();
                }
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


        private void RetryDownloadsButton_Click(object sender, EventArgs e)
        {
            RetryDownloadsButton.Enabled = false;
            try
            {
                if (FailedTasks.Count < 1)
                {
                    MessageBox.Show("No hay descargas fallidas");
                    return;
                }

                string Format = FormatComboBox.Text;
                AdvertiseMessage.Open(MSG_RETRYING_DOWNLOADS);
                foreach (var URL in FailedTasks)
                {
                    ManageSingleVideo(Format, URL);
                }
            }
            finally {RetryDownloadsButton.Enabled = true;}
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

        private void Link_Click(object sender, EventArgs e)
        {
            Link.SelectAll();
        }

        private void CleanTasksButton_Click(object sender, EventArgs e)
        {

            CleanTasksButton.Enabled = false;
            try
            {
                if (FinishedTasks.Count < 1)
                {
                    MessageBox.Show("No hay tareas finalizadas");
                    return;
                }


                foreach (ListViewItem Item in FinishedTasks)
                {
                    DownloadQueue.Invoke(new Action(() =>
                    {
                        DownloadQueue.Items.Remove(Item);
                    }));
                }

                FinishedTasks.Clear();
            }
            finally { CleanTasksButton.Enabled = true; }
            
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


        private const short  UI_STATUS_INDEX = 1;
        private const string MSG_SEARCHING = "Buscando vídeo...";
        private const string MSG_DOWNLOADING = "Descargando...";
        private const string MSG_RETRYING_DOWNLOADS = "Reintentando descarga(s)...";
        private const string MSG_WAITING = "Esperando...";
        private const string MSG_CONVERSION_PROGRESS = "Convirtiendo a ";
        private const string MSG_SUCCESS = "¡Completado!";

        private const string MSG_TOOL_TIP_PLAYLIST = "Al marcar esta opción, descargarás todos los videos de una playlist insertando el link de un vídeo perteneciente a esa playlist";
        private const string WINDOW_SEARCHING_VIDEO = "Buscando vídeo. Por favor espere";
        private const string WINDOW_SEARCHING_PLAYLIST = "Buscando Playlist... Por favor espere";
        private static short REQUESTS = 0;
        private static short MAX_REQUESTS_UNTIL_TIMEOUT = 20;

        private List<ListViewItem> FinishedTasks = new List<ListViewItem>();
        private List<string> FailedTasks = new List<string>();

        private SemaphoreSlim Semaphore = new SemaphoreSlim(8, 8);
        Random NumberGenerator;

    }
}

