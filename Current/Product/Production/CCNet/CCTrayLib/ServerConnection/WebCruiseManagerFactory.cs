using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    //This replaced RemoteCruiseManagerFactory and does not use Remoting.
    public class WebCruiseManagerFactory : IWebCruiseManagerFactory
    {
        // This is not unit tested right now
        public IWebCruiseManager GetCruiseManager(string url)
        {
            IWebCruiseManager cruiseManager = new WebCruiseManager(url);

            return cruiseManager;
        }
    }
}
