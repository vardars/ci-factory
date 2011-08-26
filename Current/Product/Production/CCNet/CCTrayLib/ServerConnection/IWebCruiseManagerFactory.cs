using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    public interface IWebCruiseManagerFactory
    {
        IWebCruiseManager GetCruiseManager(string url);
    }
}
