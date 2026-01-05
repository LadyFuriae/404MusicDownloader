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
    public class VideoData
    {
        public void GetInfo() 
        {
            Info = Manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        }
        public void FormatSongName()
        {
            FormatedSongName = Song.Title;
            foreach (char InvalidChars in Path.GetInvalidFileNameChars())
            {
                FormatedSongName = FormatedSongName.Replace(InvalidChars, '_');
            }
        }

        public string FormatedSongName;
        public Video Song;
        public StreamManifest Manifest;
        public IStreamInfo Info;
    }
}
