using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;


namespace _404MusicDownloader
{
    public class VideoData
    {
        public void GetInfo() 
        {
            Info = Manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Container = Info.Container.ToString();
            SongSize = Info.Size.Bytes;
        }
        public void FormatSongName(string Title)
        {
            FormatedSongName = Title;
            foreach (char InvalidChars in Path.GetInvalidFileNameChars())
            {
                FormatedSongName = FormatedSongName.Replace(InvalidChars, '_');
            }
        }

        public void SetPreparedPath(string FolderPath)
        {
            FinalPath =  Path.Combine(FolderPath, $"{FormatedSongName}.{Info.Container}");
        }

        public string FinalPath; 
        public long SongSize;
        public string Container;
        public string FormatedSongName;
        public string URL;
        public StreamManifest Manifest;
        public IStreamInfo Info;
    }


}
