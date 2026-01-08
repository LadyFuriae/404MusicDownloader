using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _404MusicDownloader
{
    public class FormatConverter
    {
        private void SetArgs(string Path) 
        {
            args = $"-i \"{Path}\" -codec:a libmp3lame -q:a 2 \"{MP3Finalpath}\"";
            FFMPEGEXECUTEINFO = new ProcessStartInfo
            {
                FileName = FFMPEGPATH,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
        }

        private void SetMP3Path(string Path, string Container) 
        {
            string full = "." + Container;
            MP3Finalpath = Path.Replace(full, ".mp3");
        }

        public void ProcessAudio(string Path, string Container)
        { 
            lock (_lock)
            {
                DownloadManager.SongsDownloading.Dequeue();
                SetMP3Path(Path, Container);
                if (File.Exists(MP3Finalpath)) { File.Delete(MP3Finalpath); } 
                SetArgs(Path);
                FFMPEGPROCESS = Process.Start(FFMPEGEXECUTEINFO);
                FFMPEGPROCESS.WaitForExit();
                FFMPEGPROCESS.Dispose();
                FFMPEGPROCESS = null;
                File.Delete(Path);
            }
        }
        private Object _lock = new Object();
        public static Process FFMPEGPROCESS;
        string args;
        public static string MP3Finalpath;
        ProcessStartInfo FFMPEGEXECUTEINFO;
        const string FFMPEGPATH = @"ffmpeg\ffmpeg.exe";
    }
}
