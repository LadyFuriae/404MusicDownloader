using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace _404MusicDownloader
{
    public class App
    {
        public void Run(DownloadManager Manager)
        {   
            Manager.StartListener(); 
        }
    }
}
