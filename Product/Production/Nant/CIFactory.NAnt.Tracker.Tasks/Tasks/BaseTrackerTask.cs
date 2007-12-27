using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
using Tracker.Common;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
 {
     
    public abstract class BaseTrackerTask : Task
    {

#region Fields
        
        private ConnectionInformation _ConnectionInformation;
        private TrackerServer _TrackerServer;
        private IPVCSToolKit _ToolKit;

        #endregion

#region Properties

        private IPVCSToolKit ToolKit
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

        [BuildElement("connectioninformation", Required = false)]
        public ConnectionInformation ConnectionInformation
        {
            get
            {
                return this._ConnectionInformation;
            }
            set
            {
                this._ConnectionInformation = value;
            }
        }

        protected TrackerServer TrackerServer
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
        #endregion
        
#region Constructors

        public BaseTrackerTask()
        {
        }

        /// <summary>
        /// Creates a new instance of BaseTrackerTask
        /// </summary>
        /// <param name="toolKit"></param>
        public BaseTrackerTask(IPVCSToolKit toolKit)
        {
            ToolKit = toolKit;
        }

        #endregion

#region State Control
        protected void Login()
        {
            this.TrackerServer = new TrackerServer(this.ToolKit);
            TrackerServer server1 = this.TrackerServer;
            server1.UserName = this.ConnectionInformation.UserName;
            server1.UserPWD = this.ConnectionInformation.UserPWD;
            server1.DBMSServer = this.ConnectionInformation.DBMSServer;
            server1.DBMSType = this.ConnectionInformation.DBMSType;
            server1.DBMSUserName = this.ConnectionInformation.DBMSUserName;
            server1.DBMSPassword = this.ConnectionInformation.DBMSPassword;
            server1.DBMSLoginMode = this.ConnectionInformation.DBMSLoginMode;
            server1.ProjectName = this.ConnectionInformation.ProjectName;
            server1 = null;
            this.TrackerServer.Login();
        }

        protected void Logout()
        {
            this.TrackerServer.Logout();
        }
        #endregion

    }
}