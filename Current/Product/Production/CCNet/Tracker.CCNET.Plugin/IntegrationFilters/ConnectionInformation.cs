using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace Tracker.CCNET.Plugin.IntegrationFilters
{

    [ReflectorType("connectionInformation")]
    public class ConnectionInformation
    {
        private string _UserName;
        private string _Password;
        private string _dbmsServer;
        private string _dbmsType = "Tracker SQL Server Sys";
        private int _dbmsLoginMode = 2;
        private string _dbmsUserName;
        private string _dbmsPassword;
        private string _ProjectName;

        [ReflectorProperty("userName", Required = true)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        [ReflectorProperty("password", Required = true)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        [ReflectorProperty("dbmsServer", Required = true)]
        public string dbmsServer
        {
            get
            {
                return _dbmsServer;
            }
            set
            {
                _dbmsServer = value;
            }
        }

        [ReflectorProperty("dbmsType", Required = false)]
        public string dbmsType
        {
            get
            {
                return _dbmsType;
            }
            set
            {
                _dbmsType = value;
            }
        }

        [ReflectorProperty("dbmsLoginMode", Required = false)]
        public int dbmsLoginMode
        {
            get
            {
                return _dbmsLoginMode;
            }
            set
            {
                _dbmsLoginMode = value;
            }
        }

        [ReflectorProperty("dbmsUserName", Required = true)]
        public string dbmsUserName
        {
            get
            {
                return _dbmsUserName;
            }
            set
            {
                _dbmsUserName = value;
            }
        }

        [ReflectorProperty("dbmsPassword", Required = true)]
        public string dbmsPassword
        {
            get
            {
                return _dbmsPassword;
            }
            set
            {
                _dbmsPassword = value;
            }
        }

        [ReflectorProperty("projectName", Required = true)]
        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }
    }

}
