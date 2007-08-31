using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{

    [Serializable, ElementName("connectioninformation")]
    public class ConnectionInformation : DataTypeBase
    {
        // Methods
        public ConnectionInformation()
        {
            this._DBMSLoginModeWasSet = false;
        }


        // Properties
        [TaskAttribute("dbmsloginmode")]
        public int DBMSLoginMode
        {
            get
            {
                return this._DBMSLoginMode;
            }
            set
            {
                this._DBMSLoginMode = value;
                this.DBMSLoginModeWasSet = true;
            }
        }

        public bool DBMSLoginModeWasSet
        {
            get
            {
                return this._DBMSLoginModeWasSet;
            }
            set
            {
                this._DBMSLoginModeWasSet = value;
            }
        }

        [TaskAttribute("dbmspassword")]
        public string DBMSPassword
        {
            get
            {
                return this._DBMSPassword;
            }
            set
            {
                this._DBMSPassword = value;
            }
        }

        [TaskAttribute("dbmsserver")]
        public string DBMSServer
        {
            get
            {
                return this._DBMSServer;
            }
            set
            {
                this._DBMSServer = value;
            }
        }

        [TaskAttribute("dbmstype")]
        public string DBMSType
        {
            get
            {
                return this._DBMSType;
            }
            set
            {
                this._DBMSType = value;
            }
        }

        [TaskAttribute("dbmsusername")]
        public string DBMSUserName
        {
            get
            {
                return this._DBMSUserName;
            }
            set
            {
                this._DBMSUserName = value;
            }
        }

        [TaskAttribute("projectname")]
        public string ProjectName
        {
            get
            {
                return this._ProjectName;
            }
            set
            {
                this._ProjectName = value;
            }
        }

        [TaskAttribute("username")]
        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        [TaskAttribute("password")]
        public string UserPWD
        {
            get
            {
                return this._UserPWD;
            }
            set
            {
                this._UserPWD = value;
            }
        }


        // Fields
        private int _DBMSLoginMode;
        private bool _DBMSLoginModeWasSet;
        private string _DBMSPassword;
        private string _DBMSServer;
        private string _DBMSType;
        private string _DBMSUserName;
        private string _ProjectName;
        private string _UserName;
        private string _UserPWD;
    }
}