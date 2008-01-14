using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using Zeta.WebSpider;
using Zeta.WebSpider.Spider;
using System.IO;

namespace CIFactory.NAnt.Types
{
    [ElementName("url")]
    public class SpiderUrl : DataTypeBase
    {

        private WebSiteDownloaderOptions _Options;

        public WebSiteDownloaderOptions Options
        {
            get
            {
                if (_Options == null)
                {
                    _Options = new WebSiteDownloaderOptions();
                    _Options.DownloadUri = new Uri(this.DownloadUri);
                    _Options.MaximumLinkDepth = this.MaximumLinkDepth;
                    _Options.StayOnSite = this.StayOnSite;
                }
                return _Options;
            }
        }

        private int _MaximumLinkDepth;
        [TaskAttribute("maximumlinkdepth")]
        public int MaximumLinkDepth
        {
            get { return _MaximumLinkDepth; }
            set
            {
                _MaximumLinkDepth = value;
            }
        }

        private bool _StayOnSite;
        [TaskAttribute("stayonsite")]
        public bool StayOnSite
        {
            get { return _StayOnSite; }
            set
            {
                _StayOnSite = value;
            }
        }

        private string _DownloadUri;
        [TaskAttribute("downloaduri")]
        public string DownloadUri
        {
            get { return _DownloadUri; }
            set
            {
                _DownloadUri = value;
            }
        }

    }
}
