using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.ForceFilters.UserFilter;

namespace CCNET.Extensions.Plugin.ForceFilters
{
    [ReflectorType("hostForceFilter")]
    public class HostForceFilter : IForceFilter
    {
        private string[] _Hosts = new string[0];
        private ILogHelper _Logger;
        private IHostNameHelper _NewHostNameHelper;
        private StringCollection _HostList;

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

        #region IForceFilter Members

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

        public ForceFilterClientInfo GetClientInfo()
        {
            HostInformation Host = new HostInformation();
            Host.Name = this.HostHelper.GetHostName();
            return Host;
        }

        public bool RequiresClientInformation
        {
            get { return true; }
        }

        #endregion
    }

    public interface ILogHelper
    {
        void LogInfo(string message);
    }
    public class LogHelper : ILogHelper
    {

        public void LogInfo(string message)
        {
            Log.Info(message);
        }
    }

    public interface IHostNameHelper
    {
        string GetHostName();
    }
    public class HostNameHelper : IHostNameHelper
    {
        public string GetHostName()
        {
            return Environment.MachineName;
        }
    }

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
