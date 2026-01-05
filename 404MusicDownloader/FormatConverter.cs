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
            SetMP3Path(Path);
            args = $"-i \"{Path}\" -codec:a libmp3lame -q:a 2 \"{MP3Finalpath}\"";
            FFMPEGEXECUTEINFO = new ProcessStartInfo
            {
                FileName = FFMPEGPATH,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardOutput = true
            };
        }

        private void SetMP3Path(string Path) 
        {
            MP3Finalpath = Path.Replace("webm", "mp3");
        }

        public void ProcessAudio(string Path) 
        {
            
            SetArgs(Path);
            lock (_lock)
            {
                Process FFMPEGPROCESS = Process.Start(FFMPEGEXECUTEINFO);
                FFMPEGPROCESS.WaitForExit();
                File.Delete(Path);
            }
        }
        private Object _lock = new Object();
        string args;
        string MP3Finalpath;
        ProcessStartInfo FFMPEGEXECUTEINFO;
        const string FFMPEGPATH = @"C:\Users\LadyFuriae\source\repos\404MusicDownloader\404MusicDownloader\bin\Debug\net48\ffmpeg.exe";
    }
}
