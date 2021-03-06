using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.ForceFilters.UserFilter;
using System.Web;
using CCNET.Extensions.Plugin.ForceFilters.PasswordFilter;
using System.Windows.Forms;

namespace CCNET.Extensions.Plugin.ForceFilters
{
    [Serializable]
    public class HostInformation : ForceFilterClientInfo
    {

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
    }
}
