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
    public class PasswordInformation : ForceFilterClientInfo
    {
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
            }
        }

    }
}
