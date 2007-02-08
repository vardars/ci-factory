using System;
using System.IO;
using System.Xml;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using Tracker.Common;

namespace Tracker.CCNET.Plugin.IntegrationFilters
{
    [ReflectorType("trackerRequired")]
    public class TrackerRequired : IIntegrationFilter
    {
        
#region Fields

        private bool _Condition = true;
        private bool _WithModifications = true;
        private ConnectionInformation _ConnectionInformation;
        private Query _QueryInforation;
        private TrackerServer _TrackerServer;
        private IPVCSToolKit _ToolKit;

#endregion
        
#region Properties

        [ReflectorProperty("withModifications", Required = false)]
        public bool WithModifications
        {
            get
            {
                return _WithModifications;
            }
            set
            {
                _WithModifications = value;
            }
        }

        [ReflectorProperty("query", InstanceType = typeof(Query), Required = true)]
        public Query QueryInforation
        {
            get
            {
                return _QueryInforation;
            }
            set
            {
                _QueryInforation = value;
            }
        }

        [ReflectorProperty("connectionInformation", InstanceType = typeof(ConnectionInformation), Required = true)]
        public ConnectionInformation ConnectionInformation
        {
            get
            {
                return _ConnectionInformation;
            }
            set
            {
                _ConnectionInformation = value;
            }
        }

        [ReflectorProperty("condition", Required = false)]
        public bool Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
            }
        }

        private TrackerServer TrackerServer
        {
            get
            {
                return this._TrackerServer;
            }
            set
            {
                this._TrackerServer = value;
            }
        }

        public IPVCSToolKit ToolKit
        {
            get
            {
                if (_ToolKit == null)
                    _ToolKit = new PVCSToolKit();
                return _ToolKit;
            }
            set
            {
                _ToolKit = value;
            }
        }

#endregion

#region IIntegrationFilter Members

        public bool ShouldRunBuild(IIntegrationResult result)
        {
            if (!this.Condition)
                return true;

            bool HasTrackers = this.GetQueryCount() != 0;

            bool Modifications = true;
            if (this.WithModifications)
                Modifications = result.HasModifications();

            bool Integratable = HasTrackers && Modifications;

            if (!Integratable)
                Log.Info(string.Format("No trackers were found in query {0}.", this.QueryInforation.Name));

            return Integratable;
        }

#endregion
        
#region Query Logic

        public int GetQueryCount()
        {
            this.Login();
            int[] IDs = this.TrackerServer.GetSCRIDListFromQuery(this.QueryInforation.Name);
            this.Logout();
            if (IDs != null)
                return IDs.Length;
            return 0;
        }

#endregion

#region State Control

        private void Login()
        {
            this.TrackerServer = new TrackerServer(this.ToolKit);

            this.TrackerServer.UserName = this.ConnectionInformation.UserName;
            this.TrackerServer.UserPWD = this.ConnectionInformation.Password;
            this.TrackerServer.DBMSServer = this.ConnectionInformation.dbmsServer;
            this.TrackerServer.DBMSType = this.ConnectionInformation.dbmsType;
            this.TrackerServer.DBMSUserName = this.ConnectionInformation.dbmsUserName;
            this.TrackerServer.DBMSPassword = this.ConnectionInformation.dbmsPassword;
            this.TrackerServer.DBMSLoginMode = this.ConnectionInformation.dbmsLoginMode;
            this.TrackerServer.ProjectName = this.ConnectionInformation.ProjectName;
            
            this.TrackerServer.Login();
        }

        private void Logout()
        {
            this.TrackerServer.Logout();
            this.TrackerServer = null;
        }

#endregion

        
    }
}
