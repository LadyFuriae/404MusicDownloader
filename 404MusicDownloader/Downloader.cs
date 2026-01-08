using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Instrumentation;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using System.Text.Json;

#pragma warning disable CS4014

namespace _404MusicDownloader
{

    public struct VideoResult
    {
        public VideoData Video;
        public string Message;

        public VideoResult(VideoData instance) 
        {
            Video = instance;
            Message = String.Empty;   
        }
    };

    public class PathSerialize
    {
        public string LastPath { get; set; }
    }
    public class DownloadManager
    {
        public DownloadManager() 
        {
            Client = new YoutubeClient();
            GetLastPath();
        }

        public async Task<VideoResult> SearchVideo(string URL) 
        {
            
            VideoData VideoData = new VideoData();
            VideoResult Video = new VideoResult(VideoData);

            Debug.WriteLine("Buscando esa vaina");
            try
            {
                Video.Video.Song = await Client.Videos.GetAsync(URL);
                Debug.WriteLine("Se encontró");
                Video.Video.Manifest = await Client.Videos.Streams.GetManifestAsync(URL);
                Video.Video.GetInfo();
                Video.Video.FormatSongName();
                Debug.WriteLine("Video encolado");
            }
            catch (VideoUnavailableException e) 
            {
                Video.Video = null;  
                Video.Message = MSG_VIDEO_NOT_AVAILABLE + e.Message;
            }
            catch(VideoRequiresPurchaseException e) 
            {
                 Video.Video = null;
                 Video.Message = MSG_VIDEO_REQUIRES_PURCHASE;
            }
            catch(FormatException e) 
            {
                 Video.Video = null;
                 Video.Message = MSG_VIDEO_URL_FORMAT_NOT_VALID;
            }
            catch(Exception e) 
            {
                 Video.Video = null;
                 Video.Message = MSG_GENERIC_ERROR + e.Message;
            }
          
            return Video; 
        }

        public void GetLastPath()
        {
            string LastPath = File.ReadAllText(SAVED_JSON_PATH);
        
            JsonDocument doc = JsonDocument.Parse(LastPath);
            JsonElement root = doc.RootElement;
            JsonElement PropertyPath = root.GetProperty("LastPath");
            FolderPath = PropertyPath.GetString();
        }

        public void ReplacePath(string Path) 
        {
            var json = File.ReadAllText(SAVED_JSON_PATH);
            PathSerialize JsonPath = JsonSerializer.Deserialize<PathSerialize>(json);
            JsonPath.LastPath = Path;
            File.WriteAllText(SAVED_JSON_PATH, JsonSerializer.Serialize(JsonPath, new JsonSerializerOptions { WriteIndented = true }));
        }
        public async Task Download(VideoData Song) 
        {
            CancellationTokenSource TokenSource = new CancellationTokenSource();
            SongsDownloading.Enqueue(TokenSource);

            CancellationToken CancelToken = TokenSource.Token;
            try
            {
                await Client.Videos.Streams.DownloadAsync(Song.Info, Song.FinalPath, null, CancelToken);
            }
            catch (OperationCanceledException) 
            {
                if (File.Exists(Song.FinalPath))
                    File.Delete(Song.FinalPath);
            }
            finally 
            {
                TokenSource.Dispose();
            }
        }

        public async Task StartDownload(VideoData Song) 
        {      
            Debug.WriteLine("Preparando Vídeo");
            Song.SetPreparedPath(FolderPath);
            
            await Download(Song);
        }

        public void ConvertSong(VideoData Song) 
        {
            //Thread ConversionTask = new Thread(() => 
            //
            //{
            //    
            //
            //});
            //
            //ConversionTask.Start();
            //ConversionTask.Join();

            FormatConverter Converter = new FormatConverter();
            Converter.ProcessAudio(Song.FinalPath, Song.Container);

        }

        public void CancelDownloads() 
        {
            foreach(var Song in SongsDownloading) 
            {
                Song.Cancel();
            }
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
        public string FolderPath = @"";
        public short VIDEO_QUEUED_COUNT = 0;
        public static Queue<CancellationTokenSource> SongsDownloading = new Queue<CancellationTokenSource>();

        private const string MSG_VIDEO_NOT_AVAILABLE = "Video no disponible. Motivo: ";
        private const string MSG_VIDEO_REQUIRES_PURCHASE = "El vídeo es de pago.";
        private const string MSG_VIDEO_URL_FORMAT_NOT_VALID = "Url inválida.";
        private const string MSG_GENERIC_ERROR = "Error: ";
        private const string SAVED_JSON_PATH = "LastPath.json";
    }
}
