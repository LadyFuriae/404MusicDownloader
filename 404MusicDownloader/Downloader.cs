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
using YoutubeExplode.Playlists;

#pragma warning disable CS4014

namespace _404MusicDownloader
{

    public struct VideoResult
    {

        public VideoResult(VideoData instance)
        {
            Video = instance;
            Message = String.Empty;
        }
        
        public VideoData Video;
        public string Message;
    };

    public struct PlayListResult
    {

        public void PrepareList() 
        {
            Videos = new List<PlaylistVideo>();
            PreparedVideos = new List<VideoResult>();
        }

        public List<PlaylistVideo> Videos;
        public List<VideoResult> PreparedVideos;
        public string Message;
    }

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

        public bool IsFromPlayList(string URL) 
        {

            PlaylistId? PlayList = PlaylistId.TryParse(URL);
            if( PlayList == null )
                return false;
            return true;
        }

        public async Task<PlayListResult> SearchPlayList(string URL) 
        {

            IAsyncEnumerator<PlaylistVideo> Enum = null;
            PlayListResult Result = new PlayListResult();
            Result.PrepareList();

            try
            {
                Debug.WriteLine("Buscando...");
                Enum = Client.Playlists.GetVideosAsync(URL).GetAsyncEnumerator();
                while (await Enum.MoveNextAsync())
                {
                    Debug.WriteLine("Video obtenido");
                    PlaylistVideo ListVideo = Enum.Current;
                    Result.Videos.Add(ListVideo);
                    Debug.WriteLine("Video agregado");
                }
            }
            catch (PlaylistUnavailableException e)
            {
                Result.Videos = null;
                Result.Message = MSG_PLAYLIST_NOT_AVAILABLE + e.Message;
            }
            catch (Exception e) 
            {
                Result.Videos = null;
                Result.Message = MSG_GENERIC_ERROR + e.Message;
            }

            return Result;     
        }

        public async Task SearchPlayListStreams(PlayListResult Results) 
        {
            foreach (var Song in Results.Videos) 
            {
                VideoData SongData = new VideoData();
                VideoResult SongFromPlayList = new VideoResult(SongData);
                SongFromPlayList.Video.FormatSongName(Song.Title);
                SongFromPlayList.Video.URL = Song.Url;
                await SearchStreams(SongFromPlayList);
                Results.PreparedVideos.Add(SongFromPlayList);
            }
        }

        public async Task<VideoResult> GetMetaData(string URL) 
        {
            VideoData SongData = new VideoData();
            VideoResult Song = new VideoResult(SongData);
            try
            {
                Video video  = await Client.Videos.GetAsync(URL);
                Song.Video.FormatSongName(video.Title);
                Song.Video.URL = URL;
            }
            catch (VideoUnavailableException e)
            {
                Song.Video = null;
                Song.Message = MSG_VIDEO_NOT_AVAILABLE + e.Message;
            }
            catch (VideoRequiresPurchaseException e)
            {
                Song.Video = null;
                Song.Message = MSG_VIDEO_REQUIRES_PURCHASE;
            }
            catch (FormatException e)
            {
                Song.Video = null;
                Song.Message = MSG_VIDEO_URL_FORMAT_NOT_VALID;
            }
            catch (Exception e)
            {
                Song.Video = null;
                Song.Message = MSG_GENERIC_ERROR + e.Message;
            }

            return Song;
        }

        public async Task SearchStreams(VideoResult Song) 
        {
            Debug.WriteLine("Buscando esa vaina");

            Debug.WriteLine("Se encontró");
            Song.Video.Manifest = await Client.Videos.Streams.GetManifestAsync(Song.Video.URL);
            Song.Video.GetInfo();
            Debug.WriteLine("Song encolado");
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

        public void ConvertSong(VideoData Song, string Format) 
        {

            FormatConverter Converter = new FormatConverter();
            Converter.ProcessAudio(Song.FinalPath, Song.Container, Format);

        }

        public void CancelDownloads() 
        {
            foreach(var Song in SongsDownloading) 
            {
                Song.Cancel();
            }
        }

        public YoutubeClient Client;
        public string FolderPath = @"";
        public short VIDEO_QUEUED_COUNT = 0;
        public static Queue<CancellationTokenSource> SongsDownloading = new Queue<CancellationTokenSource>();
 

        private const string MSG_VIDEO_NOT_AVAILABLE = "Video no disponible. Motivo: ";
        private const string MSG_VIDEO_REQUIRES_PURCHASE = "El vídeo es de pago.";
        private const string MSG_VIDEO_URL_FORMAT_NOT_VALID = "Url inválida.";
        private const string MSG_PLAYLIST_NOT_AVAILABLE = "Playlist no disponible: ";
        private const string MSG_GENERIC_ERROR = "Error: ";
        private const string SAVED_JSON_PATH = "LastPath.json";
    }
}
