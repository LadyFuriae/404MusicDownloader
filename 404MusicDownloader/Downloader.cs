using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace _404MusicDownloader
{
    public class DownloadManager
    {
        public DownloadManager() 
        {
            Client = new YoutubeClient();  
            VideoQueue = new Queue<VideoData>(); 
        }

        public async Task<string> SearchVideo(string URL) 
        {
           VideoData Video = new VideoData();
           Debug.WriteLine("Buscando esa vaina");
           Video.Song = await Client.Videos.GetAsync(URL);
           Debug.WriteLine("Se encontró");
           Video.Manifest = await Client.Videos.Streams.GetManifestAsync(URL);
           Video.GetInfo();
           Video.FormatSongName();
           VideoQueue.Enqueue(Video);
           Debug.WriteLine("Video encolado");

           return Video.FormatedSongName;
        }

        public async Task Download(string FinalPath, VideoData Video) 
        {
            await Client.Videos.Streams.DownloadAsync(Video.Info, FinalPath);
        }

        public string GetPreparedPath(VideoData Video) 
        {
            return Path.Combine(FolderPath, $"{Video.FormatedSongName}.{Video.Info.Container}");
        }


        public async Task StartListener() 
        {
             Task.Run(async () => 
            {
                while (true)
                {
                    if (VideoQueue.Count < 1)
                        continue;
                    Debug.WriteLine("Preparando Vídeo");
                    VideoData Video = VideoQueue.Dequeue();
                    string FinalPath = GetPreparedPath(Video);
                    await Download(FinalPath, Video);

                    Task.Run(() =>
                    {
                        FormatConverter Converter = new FormatConverter();
                        Converter.ProcessAudio(FinalPath);
                    });
                }
            });

        }
        //public void RunPipeline()
        //{
        //    Task.Run(async () =>
        //    {
        //        while (VideoQueue.Count > 0)
        //        {                   
        //            
        //            Debug.Write("DESCARGADO\n");
        //            FormatConverter Converter = new FormatConverter();
        //            Debug.Write("SE VA A CONVERTIR\n");
        //            Converter.ProcessAudio(FinalPath);
        //            Debug.Write("CONVERTIDO\n");
        //            VIDEO_QUEUED_COUNT--;
        //        }
        //    });
        //}

        public YoutubeClient Client;
        public string FolderPath = @"C:\Users\LadyFuriae\Desktop\FilesTest";
        public short VIDEO_QUEUED_COUNT = 0;
        private Queue<VideoData> VideoQueue;
    }
}
