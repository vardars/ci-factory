using System;
using System.Collections.Generic;
using System.Net;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Client;

namespace CCNET.TFS.Plugin
{
    [ReflectorType("vstsbychangesetTrigger")]
    public class VSTSByChangeSetTrigger : ITrigger
    {

        #region Fields

        private string _Server;
        private string _ProjectPath;
        private TeamFoundationServer _TeamFoundationServer = null;
        private NetworkCredential _NetworkCredential;
        private VersionControlServer _SourceControl;
        private string _Username;
        private string _Password;
        private string _Domain;
        private VSTSMonitor _Monitor;
        private string _StateFilePath;
        private int _Port;
        private ChangesetQueue _ChangesetQueue;

        #endregion

        #region NetReflectored Properties

        /// <summary>
        ///   The name or URL of the team foundation server.  For example http://vstsb2:8080 or vstsb2 if it
        ///   has already been registered on the machine.
        /// </summary>
        [ReflectorProperty("server", Required = true)]
        public string Server
        {
            get
            {
                return _Server;
            }
            set
            {
                _Server = value;
            }
        }

        [ReflectorProperty("port", Required = true)]
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        [ReflectorProperty("statefilepath", Required = true)]
        public string StateFilePath
        {
            get
            {
                return _StateFilePath;
            }
            set
            {
                _StateFilePath = value;
            }
        }

        /// <summary>
        ///   The path to the project in source control, for example $\VSTSPlugins
        /// </summary>
        [ReflectorProperty("project", Required = true)]
        public string ProjectPath
        {
            get
            {
                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
            }
        }

        /// <summary>
        ///   Username that should be used.  Domain cannot be placed here, rather in domain property.
        /// </summary>
        [ReflectorProperty("username", Required = false)]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }

        /// <summary>
        ///   The password in clear test of the domain user to be used.
        /// </summary>
        [ReflectorProperty("password", Required = false)]
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

        /// <summary>
        ///  The domain of the user to be used.
        /// </summary>
        [ReflectorProperty("domain", Required = false)]
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

        #endregion NetReflectored Properties

        #region Properties

        public VSTSMonitor Monitor
        {
            get
            {
                if (_Monitor == null)
                    _Monitor = MonitorFactory.GetMonitor(this.TFS, this.StateFilePath, this.ProjectPath, this.Port);
                return _Monitor;
            }
            set
            {
                _Monitor = value;
            }
        }

        public ChangesetQueue ChangesetQueue
        {
            get
            {
                if (_ChangesetQueue == null)
                    _ChangesetQueue = QueueFactory.GetChangesetQueue(this.ProjectPath);
                return _ChangesetQueue;
            }
            set
            {
                _ChangesetQueue = value;
            }
        }

        /// <summary>
        ///   Cached instance of TeamFoundationServer.
        /// </summary>
        public TeamFoundationServer TFS
        {
            get
            {
                if (null == _TeamFoundationServer)
                {
                    _TeamFoundationServer = new TeamFoundationServer(this.Server, this.Credentials);
                }
                return _TeamFoundationServer;
            }
            set
            {
                _TeamFoundationServer = value;
            }
        }

        /// <summary>
        ///   Network credentials used to interact with TFS.
        /// </summary>
        public NetworkCredential Credentials
        {
            get
            {
                if (null == _NetworkCredential)
                {
                    if (Username != null && Password != null)
                    {
                        if (Domain != null)
                            _NetworkCredential = new NetworkCredential(Username, Password, Domain);
                        else
                            _NetworkCredential = new NetworkCredential(Username, Password);
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

        #endregion

        #region ITrigger Members

        public void IntegrationCompleted()
        {
            //do nothing
        }

        public DateTime NextBuild
        {
            get { return new DateTime(); }
        }

        public BuildCondition ShouldRunIntegration()
        {
            if (this.Monitor.Status != MonitorStatus.Subscribed)
                this.Monitor.Subscribe();

            if (this.ChangesetQueue.Count > 0)
                return BuildCondition.IfModificationExists;

            return BuildCondition.NoBuild;
        }

        #endregion
    }

}
