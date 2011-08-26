using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Web;
using System.Windows.Forms;
//using CCNET.Extensions.Plugin.ForceFilters.UserFilter;
using CCNET.Extensions.Plugin.ForceFilters.PasswordFilter;

namespace CCNET.Extensions.Plugin.ForceFilters
{
    [ReflectorType("passwordForceFilter")]
    public class PasswordForceFilter : IForceFilter
    {

        private string _Password;
        private IPasswordHelper _PasswordHelper;
        private ILogHelper _Logger;
        
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

        public IPasswordHelper PasswordHelper
        {
            get 
            {
                if (_PasswordHelper == null)
                    _PasswordHelper = new PasswordGetter();
                return _PasswordHelper; 
            }
            set
            {
                _PasswordHelper = value;
            }
        }

        [ReflectorProperty("password")]
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
            }
        }

        #region IForceFilter Members


        public ForceFilterClientInfo GetClientInfo()
        {
            PasswordInformation PasswordInfo = new PasswordInformation();
            PasswordInfo.Password = this.PasswordHelper.GetPassword();
            return PasswordInfo;
        }

        public bool ShouldRunIntegration(ForceFilterClientInfo[] clientInfo, IIntegrationResult result)
        {
            PasswordInformation PasswordInfo = null;
            foreach (ForceFilterClientInfo Info in clientInfo)
            {
                if (Info is PasswordInformation)
                {
                    PasswordInfo = (PasswordInformation)Info;
                    break;
                }
            }

            if (PasswordInfo == null)
                throw new InvalidOperationException("No password information was found.");

            if (this.Password == PasswordInfo.Password)
            {
                return true;
            }

            Logger.LogInfo(string.Format("The password '{0}' is not correct and the build will not be forced for project {1}.", PasswordInfo.Password, result.ProjectName));
            return false;
        }

        public bool RequiresClientInformation
        {
            get { return true; }
        }

        #endregion
    }
}
