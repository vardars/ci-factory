using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("connectioninformation")]
    public class ConnectionInformation : DataTypeBase
    {

        private string _UserName = String.Empty;
        private string _Password = String.Empty;
        private string _RootServiceUrl;

        [TaskAttribute("username", Required = true)]
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

        [TaskAttribute("password", Required = true)]
        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        [TaskAttribute("rootserviceurl", Required = true)]
        public string RootServiceUrl
        {
            get { return _RootServiceUrl; }
            set
            {
                _RootServiceUrl = value;
            }
        }


    }
}