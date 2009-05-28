using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Configuration;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using System.Xml.XPath;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace ThoughtWorks.CruiseControl.Core
{
    public class RemoteCruiseServer : ICruiseServer
    {
        #region Constants

        public const string DefaultUri = "tcp://localhost:21234/{0}" + URI;

        public const string URI = "CruiseManager.rem";

        #endregion

        #region Fields

        private bool _disposed;
        private WebServiceHost _Host;
        private ICruiseServer _server;
        private static RemoteCruiseServer _self;

        #endregion

        #region Constructors

        public RemoteCruiseServer(ICruiseServer server, string configFile)
        {
            _self = this;
            _server = server;
            RegisterForRemoting(configFile);

            XmlDocument document = new XmlDocument();
            document.Load(configFile); XPathNavigator Navigator = document.CreateNavigator();
            string RestUri = Navigator.SelectSingleNode("/cruisecontrol/@restUri").ToString();

            if (string.IsNullOrEmpty(RestUri))
                throw new InvalidProgramException(@"Please set the attribute ""restUri"" on the root node ""cruisecontrol"" in the ccnet project file.");

            Uri uri = new Uri(RestUri);
            Host = new WebServiceHost(typeof(CIFactoryServer), uri);

            Host.AddServiceEndpoint(typeof(ICIFactoryServer), new WebHttpBinding(), uri);
            Host.Open();
        }

        #endregion

        #region Properties

        public ICruiseManager CruiseManager
        {
            get { return _server.CruiseManager; }
        }

        public WebServiceHost Host
        {
            get
            {
                return _Host;
            }
            set
            {
                _Host = value;
            }
        }
        public static RemoteCruiseServer Instance
        {
            get
            {
                return _self;
            }
        }

        #endregion

        #region Public Methods

        public void Abort()
        {
            _server.Abort();
        }

        public void AddProject(string serializedProject)
        {
            _server.AddProject(serializedProject);
        }

        public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            _server.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
        }

        public bool ForceBuild(string projectName, ForceFilterClientInfo[] clientInfo)
        {
            return _server.ForceBuild(projectName, clientInfo);
        }

        public string[] GetBuildNames(string projectName)
        {
            return _server.GetBuildNames(projectName);
        }

        public ExternalLink[] GetExternalLinks(string projectName)
        {
            return _server.GetExternalLinks(projectName);
        }

        public string GetLatestBuildName(string projectName)
        {
            return _server.GetLatestBuildName(projectName);
        }

        public string GetLog(string projectName, string buildName)
        {
            return _server.GetLog(projectName, buildName);
        }

        public string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            return _server.GetMostRecentBuildNames(projectName, buildCount);
        }

        public string GetProject(string name)
        {
            return _server.GetProject(name);
        }

        public ProjectStatus[] GetProjectStatus()
        {
            return _server.GetProjectStatus();
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            return _server.GetProjectStatus(projectName);
        }

        public string GetServerLog()
        {
            return _server.GetServerLog();
        }

        public string GetVersion()
        {
            return _server.GetVersion();
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        public void Start(string projectName)
        {
            _server.Start(projectName);
        }

        public void Stop(string projectName)
        {
            _server.Stop(projectName);
        }

        public void UpdateProject(string projectName, string serializedProject)
        {
            _server.UpdateProject(projectName, serializedProject);
        }

        public void WaitForExit()
        {
            _server.WaitForExit();
        }

        public void WaitForExit(string projectName)
        {
            _server.WaitForExit(projectName);
        }

        #endregion

        #region Private Methods

        void IDisposable.Dispose()
        {

            if (_disposed) return;
            _disposed = true;
            Log.Info("Disconnecting remote server: ");
            RemotingServices.Disconnect((MarshalByRefObject)_server.CruiseManager);
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Log.Info("Unregistering channel: " + channel.ChannelName);
                ChannelServices.UnregisterChannel(channel);
            }

            Host.Close();

            _server.Dispose();
        }

        private void RegisterForRemoting(string configFile)
        {
            XmlDocument document = new XmlDocument();
            document.Load(configFile); XPathNavigator Navigator = document.CreateNavigator();
            string StringPort = Navigator.SelectSingleNode("/cruisecontrol/@port").ToString();

            if (string.IsNullOrEmpty(StringPort))
                throw new InvalidProgramException(@"Please set the attribute ""port"" on the root node ""cruisecontrol"" in the ccnet project file.");
            int Port = int.Parse(StringPort);

            TcpChannel CCNetTcpChannel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(CCNetTcpChannel, false);
            MarshalByRefObject marshalByRef = (MarshalByRefObject)_server.CruiseManager;
            RemotingServices.Marshal(marshalByRef, URI);

            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Log.Info("Registered channel: " + channel.ChannelName);
                if (channel is IChannelReceiver)
                {
                    foreach (string url in ((IChannelReceiver)channel).GetUrlsForUri(URI))
                    {
                        Log.Info("CruiseManager: Listening on url: " + url);
                    }
                }
            }
        }

        #endregion


        #region ICruiseServer Members


        public string GetBuildLogDirectory(string projectName)
        {
            return this._server.GetBuildLogDirectory(projectName);
        }

        #endregion

        #region ICruiseServer Members


        public string GetHostServerName(string projectName)
        {
            return this._server.GetHostServerName(projectName);
        }

        #endregion

        #region ICruiseServer Members


        public string[] GetProjectNames()
        {
            return _server.GetProjectNames();
        }

        #endregion
    }
}