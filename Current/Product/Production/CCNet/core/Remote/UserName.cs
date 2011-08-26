using System;
using System.Collections.Specialized;
//using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
//using ThoughtWorks.CruiseControl.Core.Util;
//using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Remote
{
    [Serializable]
    public class UserInformation : ForceFilterClientInfo
    {
        private string _Name;
        private StringCollection _Groups;

        public StringCollection Groups
        {
            get
            {
                return _Groups;
            }
            set
            {
                _Groups = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }
}
