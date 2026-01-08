using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;


namespace _404MusicDownloader
{
    enum VideoType 
    {
        ADAPTIVE,
        PROGRESSIVE
    }
    public class VideoData
    {
        private void GetVideoType() 
        {
            
        }
        public void GetInfo() 
        {
            Info = Manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            Container = Info.Container.ToString();
            SongSize = Info.Size.Bytes;
        }
        public void FormatSongName()
        {
            FormatedSongName = Song.Title;
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
        public Video Song;
        public StreamManifest Manifest;
        public IStreamInfo Info;

    }
}
