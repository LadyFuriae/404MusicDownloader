using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management.Instrumentation;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

#pragma warning disable CS4014

namespace _404MusicDownloader
{

    public struct Messages 
    {
        public const string MSG_VIDEO_NOT_AVAILABLE = "Video no disponible. Motivo: ";
        public const string MSG_VIDEO_REQUIRES_PURCHASE = "El vídeo es de pago.";
        public const string MSG_VIDEO_URL_FORMAT_NOT_VALID = "Url inválida.";
        public const string MSG_VIDEO_IS_RESTRICTED = "Video con restricción de edad, privado o no disponible en tu país";
        public const string MSG_PLAYLIST_NOT_AVAILABLE = "Playlist no disponible: ";
        public const string MSG_GENERIC_ERROR = "Error: ";
        public const string MSG_CONNECTION_ERROR = "Error de conexión.";
        public const string SAVED_JSON_PATH = "LastPath.json";
    }

    public class VideoResult
    {

        public VideoResult(VideoData instance)
        {
            Video = instance;
            Message = String.Empty;
        }
        
        public VideoData Video;
        public string Message;
    };

    public class PlayListResult
    {

        public void CheckPlaylist(string URL, YoutubeClient Client) 
        {
            try
            {
                Enum = Client.Playlists.GetVideosAsync(URL).GetAsyncEnumerator();
            }
            catch (PlaylistUnavailableException e)
            {
                Message = Messages.MSG_PLAYLIST_NOT_AVAILABLE + e.Message;
            }
            catch (Exception e)
            {
                Message = Messages.MSG_GENERIC_ERROR + e.Message;
            }
        }

        public PlaylistVideo SearchVideoFromPlayList()
        {
            Debug.WriteLine("Buscando...");
            PlaylistVideo ListVideo = Enum.Current;
            Debug.WriteLine("Video obtenido");
            Debug.WriteLine("Video agregado");
            return ListVideo;
        }

        public IAsyncEnumerator<PlaylistVideo> Enum;
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

        public async Task<VideoResult> SearchPlaylistVideoStream(PlaylistVideo Video) 
        {
           VideoData SongData = new VideoData();
           VideoResult SongFromPlayList = new VideoResult(SongData);
           SongFromPlayList.Video.FormatSongName(Video.Title);
           SongFromPlayList.Video.URL = Video.Url;
           await SearchStreams(SongFromPlayList);
           return SongFromPlayList;
        }

        public async Task<VideoResult> GetMetaData(string URL) 
        {
            Debug.WriteLine("Se va a buscar");
            VideoData SongData = new VideoData();
            VideoResult Song = new VideoResult(SongData);
            try
            {
                Video video  = await Client.Videos.GetAsync(URL);
                if (video == null) 
                {
                    Song.Message = "No se han podido obtener los datos.";
                    return Song;
                }
                Song.Video.FormatSongName(video.Title);
                Song.Video.URL = URL;
                Song.Video.RawSongName = video.Title;
                Debug.WriteLine("Se encontró");

            }
            catch (VideoUnavailableException e)
            {
                Song.Message = Messages.MSG_VIDEO_NOT_AVAILABLE + e.Message;
            }
            catch (VideoRequiresPurchaseException e)
            {
                Song.Message = Messages.MSG_VIDEO_REQUIRES_PURCHASE;
            }
            catch (FormatException e)
            {
                Song.Message = Messages.MSG_VIDEO_URL_FORMAT_NOT_VALID;
            }
            catch (Exception e)
            {
                Song.Message = Messages.MSG_GENERIC_ERROR + e.Message;
            }

            return Song;
        }

        public async Task SearchStreams(VideoResult Song) 
        {
            Debug.WriteLine("Buscando esa vaina");
            try
            {
                Debug.WriteLine("Se encontró");
                Song.Video.Manifest = await Client.Videos.Streams.GetManifestAsync(Song.Video.URL);
                Song.Video.GetInfo();
                Debug.WriteLine("Song encolado");
            }
            catch (VideoUnavailableException ex)
            {
                Song.Message = Messages.MSG_VIDEO_IS_RESTRICTED;
            }
            catch (VideoUnplayableException ex)
            {
                Song.Message = Messages.MSG_VIDEO_NOT_AVAILABLE + ex.Message;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("403"))
            {
                Song.Message = Messages.MSG_GENERIC_ERROR + ex.Message;
            }
            catch (HttpRequestException ex)
            {
                Song.Message = Messages.MSG_CONNECTION_ERROR + ex.Message;
            }
            catch (Exception ex)
            {
                Song.Message = Messages.MSG_GENERIC_ERROR + ex.Message;
            }
        }

        public void GetLastPath()
        {
            string LastPath = File.ReadAllText(Messages.SAVED_JSON_PATH);
        
            JsonDocument doc = JsonDocument.Parse(LastPath);
            JsonElement root = doc.RootElement;
            JsonElement PropertyPath = root.GetProperty("LastPath");
            FolderPath = PropertyPath.GetString();
        }

        public void ReplacePath(string Path) 
        {
            var json = File.ReadAllText(Messages.SAVED_JSON_PATH);
            PathSerialize JsonPath = JsonSerializer.Deserialize<PathSerialize>(json);
            JsonPath.LastPath = Path;
            File.WriteAllText(Messages.SAVED_JSON_PATH, JsonSerializer.Serialize(JsonPath, new JsonSerializerOptions { WriteIndented = true }));
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

        public bool CheckIfValidURL(string URL)
        {
            if (VideoId.TryParse(URL) == null) 
                return false; 
            return true;
        }

        public YoutubeClient Client;
        public string FolderPath = @"";
        public short VIDEO_QUEUED_COUNT = 0;
        public static Queue<CancellationTokenSource> SongsDownloading = new Queue<CancellationTokenSource>();
 


    }
}
