using System;
using System.Collections.Generic;
using System.Net;

using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Client;

namespace CCNET.TFS.Plugin
{

    [ReflectorType("vstsbychangesetSourceControl")]
    public class VSTSByChangeSetSourceControl : ISourceControl
    {

        #region Fields

        private string _Server;
        private string _ProjectPath;
        private bool _ApplyLabel = false;
        private string _Username;
        private string _Password;
        private string _Domain;
        private TeamFoundationServer _TeamFoundationServer = null;
        private NetworkCredential _NetworkCredential;
        private VersionControlServer _SourceControl;
        private ChangesetQueue _ChangesetQueue;
        private VSTSMonitor _Monitor;
        private string _StateFilePath;
        private int _Port;

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
        /// Gets or sets whether this repository should be labeled.
        /// </summary>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel
        {
            get
            {
                return _ApplyLabel;
            }
            set
            {
                _ApplyLabel = value;
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

        #endregion Private Properties

        #region ISourceControl Members

        public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            if (this.ChangesetQueue.Count == 0)
                return new Modification[] { };

            List<Modification> Modifications = new List<Modification>();

            Changeset Set = this.ChangesetQueue.Peek();
            Modifications.AddRange(toModifcations(Set));

            return Modifications.ToArray();
        }

        public void GetSource(IIntegrationResult result)
        {
            //do nothing
        }

        
        public void Initialize(IProject project)
        {
            this.Monitor.Subscribe();
        }

        public void LabelSourceControl(IIntegrationResult result)
        {
            this.PerformLabel(result);
            this.ChangesetQueue.Dequeue();
        }

        public void Purge(IProject project)
        {
            //do nothing
        }

        #endregion

        #region Helpers
        
        /// <summary>
        ///   Convert the passed changeset to an array of modifcations.
        /// </summary>
        private Modification[] toModifcations(Changeset changeset)
        {
            List<Modification> modifications = new List<Modification>();

            string userName = changeset.Committer;
            string comment = changeset.Comment;
            int changeNumber = changeset.ChangesetId;
            // In VSTS, the version of the file is the same as the changeset number it was checked in with.
            string version = Convert.ToString(changeNumber);

            DateTime modifedTime = this.TFS.TimeZone.ToLocalTime(changeset.CreationDate);

            foreach (Change change in changeset.Changes)
            {
                Modification modification = new Modification();
                modification.UserName = userName;
                modification.Comment = comment;
                modification.ChangeNumber = changeNumber;
                modification.ModifiedTime = modifedTime;
                modification.Version = version;
                modification.Type = PendingChange.GetLocalizedStringForChangeType(change.ChangeType);

                // Populate fields from change item
                Item item = change.Item;
                if (item.ItemType == ItemType.File)
                {
                    // split into foldername and filename
                    int lastSlash = item.ServerItem.LastIndexOf('/');
                    modification.FileName = item.ServerItem.Substring(lastSlash + 1);
                    // patch to the following line submitted by Ralf Kretzschmar.
                    modification.FolderName = item.ServerItem.Substring(0, lastSlash);
                }
                else
                {
                    // TODO - what should filename be if dir??  Empty string or null?
                    modification.FileName = string.Empty;
                    modification.FolderName = item.ServerItem;
                }

                modifications.Add(modification);
            }

            return modifications.ToArray();
        }

        private void PerformLabel(IIntegrationResult result)
        {
            if (ApplyLabel)
            {
                Log.Debug(String.Format("Applying label \"{0}\"", result.Label));
                VersionControlLabel Label = new VersionControlLabel(this.SourceControl, result.Label, _SourceControl.AuthenticatedUser, this.ProjectPath, "Labeled by CruiseControl.NET");

                Changeset Set = this.ChangesetQueue.Peek();

                LabelItemSpec[] LabelSpec = new LabelItemSpec[] {  
                    new LabelItemSpec(new ItemSpec(this.ProjectPath, RecursionType.Full), new ChangesetVersionSpec(Set.ChangesetId), false)
                };

                this.SourceControl.CreateLabel(Label, LabelSpec, LabelChildOption.Replace);
            }
        }

        #endregion
    }
}