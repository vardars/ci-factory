using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    public static class Credentials
    {
        private static string _userName = string.Empty;
        private static string _password = string.Empty;

        public static string UserName
        {
            set { _userName = value; }
            get { return _userName; }
        }

        public static string Password
        {
            set { _password = value; }
            get { return _password; }
        }
    }
}
