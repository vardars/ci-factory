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
    [Serializable]
    public class PasswordGetter : IPasswordHelper
    {

        #region IPasswordHelper Members

        public string GetPassword()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.Params["password"];
            }
            else
            {
                PasswordForm Questionare = new PasswordFilter.PasswordForm();
                Questionare.ShowDialog();
                return Questionare.Password;
            }
        }

        #endregion
    }
}
