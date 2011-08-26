using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
//using CCNET.Extensions.Plugin.ForceFilters.UserFilter;
using System.Web;
using CCNET.Extensions.Plugin.ForceFilters.PasswordFilter;
using System.Windows.Forms;

namespace CCNET.Extensions.Plugin.ForceFilters
{
    public class HostNameHelper : IHostNameHelper
    {
        public string GetHostName()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostName;
            }
            else
                return Environment.MachineName;
        }
    }
}
