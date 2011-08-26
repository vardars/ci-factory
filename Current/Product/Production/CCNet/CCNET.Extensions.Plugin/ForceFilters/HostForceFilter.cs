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
    [ReflectorType("hostForceFilter")]
    public class HostForceFilter : IForceFilter
    {
        #region Fields

        private StringCollection _HostList;

        private string[] _Hosts = new string[0];

        private ILogHelper _Logger;

        private IHostNameHelper _NewHostNameHelper;

        #endregion

        #region Properties

        public IHostNameHelper HostHelper
        {
            get
            {
                if (_NewHostNameHelper == null)
                    _NewHostNameHelper = new HostNameHelper();
                return _NewHostNameHelper;
            }
            set
            {
                this._NewHostNameHelper = value;
            }
        }

        private StringCollection HostList
        {
            get
            {
                if (_HostList == null)
                {
                    _HostList = new StringCollection();
                    _HostList.AddRange(this.Hosts);
                }
                return _HostList;
            }
            set
            {
                _HostList = value;
            }
        }

        [ReflectorArray("includedHosts", Required = false)]
        public string[] Hosts
        {
            get
            {
                return _Hosts;
            }
            set
            {
                _Hosts = value;
            }
        }

        public ILogHelper Logger
        {
            get
            {
                if (_Logger == null)
                    _Logger = new LogHelper();
                return _Logger;
            }
            set
            {
                _Logger = value;
            }
        }

        public bool RequiresClientInformation
        {
            get { return true; }
        }

        #endregion

        #region Public Methods

        public ForceFilterClientInfo GetClientInfo()
        {
            HostInformation Host = new HostInformation();
            Host.Name = this.HostHelper.GetHostName();
            return Host;
        }

        public bool ShouldRunIntegration(ForceFilterClientInfo[] clientInfo, IIntegrationResult result)
        {
            HostInformation HostInfo = null;
            foreach (ForceFilterClientInfo Info in clientInfo)
            {
                if (Info is HostInformation)
                {
                    HostInfo = (HostInformation)Info;
                    break;
                }
            }

            if (HostInfo == null)
                throw new InvalidOperationException("No host information was found.");

            if (this.HostList.Contains(HostInfo.Name))
            {
                return true;
            }

            Logger.LogInfo(string.Format("{0} is not allowed to force the build for project {1}.", HostInfo.Name, result.ProjectName));
            return false;
        }

        #endregion

    }
}