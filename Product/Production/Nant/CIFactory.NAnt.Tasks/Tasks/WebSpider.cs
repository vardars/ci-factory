using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using Zeta.WebSpider;
using Zeta.WebSpider.Spider;
using System.IO;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("webspider")]
    public class WebSpider : Task
    {


        private string _DestinationFolderPath;
        [TaskAttribute("destinationfolderpath")]
        public string DestinationFolderPath
        {
            get { return _DestinationFolderPath; }
            set
            {
                _DestinationFolderPath = value;
            }
        }

        private SpiderUrl[] _SpiderUrls;
        [BuildElementArray("url", ElementType = typeof(SpiderUrl), Required = true)]
        public SpiderUrl[] SpiderUrls
        {
            get { return _SpiderUrls; }
            set
            {
                _SpiderUrls = value;
            }
        }
        
        protected override void ExecuteTask()
        {
            WebSiteDownloaderOptions options;
            WebSiteDownloader downloader;
            foreach (SpiderUrl spiderUrl in SpiderUrls)
            {
                options = spiderUrl.Options;
                options.DestinationFolderPath = new DirectoryInfo(this.DestinationFolderPath);
                downloader = new WebSiteDownloader(options);
                downloader.Process();
            }
        }

    }
}
