using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Client;

namespace TF.Tasks.SourceControl.Types
{
    [ElementName("tfsserverconnection")]
    public class TfsServerConnection : DataTypeBase
    {

        private string _ServerUrl;
        private string _UserName;
        private string _Password;
        private string _Domain;

        public TfsServerConnection()
        {
        }
        
        public TfsServerConnection(string serverUrl)
        {
            _ServerUrl = serverUrl;
        }
        
        [TaskAttribute("domain", Required = false)]
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                _Domain = value;
            }
        }

        [TaskAttribute("password", Required = false)]
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

        [TaskAttribute("username", Required = false)]
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

        [TaskAttribute("serverurl", Required = true)]
        public string ServerUrl
        {
            get
            {
                return _ServerUrl;
            }
            set
            {
                _ServerUrl = value;
            }
        }


        private TeamFoundationServer _TeamFoundationServer = null;
        /// <summary>
        ///   Cached instance of TeamFoundationServer.
        /// </summary>
        public TeamFoundationServer TFS
        {
            get
            {
                if (null == _TeamFoundationServer)
                {
                    _TeamFoundationServer = new TeamFoundationServer(this.ServerUrl, this.Credentials);
                }
                return _TeamFoundationServer;
            }
            set
            {
                _TeamFoundationServer = value;
            }
        }

        private NetworkCredential _NetworkCredential;
        /// <summary>
        ///   Network credentials used to interact with TFS.
        /// </summary>
        public NetworkCredential Credentials
        {
            get
            {
                if (null == _NetworkCredential)
                {
                    if (this.UserName != null && this.Password != null)
                    {
                        if (Domain != null)
                            _NetworkCredential = new NetworkCredential(this.UserName, this.Password, this.Domain);
                        else
                            _NetworkCredential = new NetworkCredential(this.UserName, this.Password);
                    }
                    else
                    {
                        _NetworkCredential = CredentialCache.DefaultNetworkCredentials;
                    }
                }
                return _NetworkCredential;
            }
            set
            {
                _NetworkCredential = value;
            }
        }

        private VersionControlServer _SourceControl;
        /// <summary>
        ///   The cached instace of the SourceControl object that we are connected to.
        /// </summary>
        public VersionControlServer SourceControl
        {
            get
            {
                if (null == _SourceControl)
                {
                    _SourceControl = (VersionControlServer)this.TFS.GetService(typeof(VersionControlServer));
                }
                return _SourceControl;
            }
            set
            {
                _SourceControl = value;
            }
        }
    }
}
