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

        private void SetArgs(string Path, string Format) 
        {
            args = $"-i \"{Path}\" -vn {Formats[Format]} \"{FormatFinalPath}\"";
            FFMPEGEXECUTEINFO = new ProcessStartInfo
            {
                FileName = FFMPEGPATH,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
        }

        public static void SetFormats() 
        {
            //{ ".mp3", ".wav", ".flac", ".opus" }
            Formats[".mp3"]  = "-c:a libmp3lame -q:a 0 -joint_stereo 1";
            Formats[".opus"] = "-c:a libopus -b:a 128k -vbr on -compression_level 5";
            Formats[".flac"] = "-c:a flac -compression_level 5";
            Formats[".wav"]  = "-c:a pcm_s16le";
        }

        private void SetFormatPath(string Path, string Container, string Format) 
        {
            FormatFinalPath = Path.Replace("." + Container, Format);
        }

        public void ProcessAudio(string Path, string Container, string Format)
        { 
            lock (_lock)
            {
                DownloadManager.SongsDownloading.Dequeue();
                SetFormatPath(Path, Container, Format);
                if (File.Exists(FormatFinalPath)) { File.Delete(FormatFinalPath); } 
                SetArgs(Path, Format);
                FFMPEGPROCESS = Process.Start(FFMPEGEXECUTEINFO);
                FFMPEGPROCESS.WaitForExit();
                FFMPEGPROCESS?.Dispose();
                FFMPEGPROCESS = null;
                File.Delete(Path);
            }
        }
        string args;
        private const string FFMPEGPATH = @"ffmpeg\ffmpeg.exe";
        public static string FormatFinalPath;
        public static Dictionary<string, string> Formats = new Dictionary<string, string>();
        private Object _lock = new Object();
        public static Process FFMPEGPROCESS;
        ProcessStartInfo FFMPEGEXECUTEINFO;
    }
}
