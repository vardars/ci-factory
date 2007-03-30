using System;
using System.IO;
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
        private bool _AutoGetSource = false;
        private string _WorkingDirectory;
        private bool _CleanCopy = false;
        private bool _DeleteWorkspace = false;
        private string _WorkspaceName;
        private bool _Force = false;

        #endregion

        #region NetReflectored Properties

        [ReflectorProperty("force", Required = false)]
        public bool Force
        {
            get
            {
                return _Force;
            }
            set
            {
                _Force = value;
            }
        }

        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource
        {
            get
            {
                return _AutoGetSource;
            }
            set
            {
                _AutoGetSource = value;
            }
        }

        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory
        {
            get
            {
                return _WorkingDirectory;
            }
            set
            {
                _WorkingDirectory = value;
            }
        }

        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy
        {
            get
            {
                return _CleanCopy;
            }
            set
            {
                _CleanCopy = value;
            }
        }
        
        /// <summary>
        ///   Name of the workspace to create.  This will revert to the DEFAULT_WORKSPACE_NAME if not passed.
        /// </summary>
        [ReflectorProperty("workspace", Required = false)]
        public string Workspace
        {
            get
            {
                if (_WorkspaceName == null)
                {
                    _WorkspaceName = string.Format("CCNET-{0}", Environment.MachineName);
                }
                return _WorkspaceName;
            }
            set
            {
                _WorkspaceName = string.Format(value, Environment.MachineName);
            }
        }

        /// <summary>
        ///   Flag indicating if workspace should be deleted every time or if it should be 
        ///   left (the default).  Leaving the workspace will mean that subsequent gets 
        ///   will only need to transfer the modified files, improving performance considerably.
        /// </summary>
        [ReflectorProperty("deleteWorkspace", Required = false)]
        public bool DeleteWorkspace
        {
            get
            {
                return _DeleteWorkspace;
            }
            set
            {
                _DeleteWorkspace = value;
            }
        }

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
                    _ChangesetQueue = QueueFactory.GetChangesetQueue(this.ProjectPath, this.SourceControl);
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

            this.ChangesetQueue.BeginIntegration();

            List<Modification> Modifications = new List<Modification>();

            Changeset Set = this.ChangesetQueue.GetCurrentIntegrationSet();
            Modifications.AddRange(toModifcations(Set));

            return Modifications.ToArray();
        }

        public void GetSource(IIntegrationResult result)
        {
            this.ChangesetQueue.BeginIntegration();
            Changeset Set = this.ChangesetQueue.GetCurrentIntegrationSet();
            
            string ChangesetIdTo = ChangesetIdTo = Set.ChangesetId.ToString();
            result.AddIntegrationProperty("CCNetVSTSChangeSetId", ChangesetIdTo);

            if (AutoGetSource)
            {
                if (CleanCopy)
                {
                    // If we have said we want a clean copy, then delete old copy before getting.
                    Log.Debug("Deleting " + this.WorkingDirectory);
                    this.DeleteDirectory(this.WorkingDirectory);
                }

                Workspace[] Workspaces = this.SourceControl.QueryWorkspaces(Workspace, this.SourceControl.AuthenticatedUser, Workstation.Current.Name);
                Workspace MyWorkspace = null;

                if (Workspaces.Length > 0)
                {
                    // The workspace exists.  
                    if (DeleteWorkspace)
                    {
                        // We have asked for a new workspace every time, therefore delete the existing one.
                        Log.Debug("Removing existing workspace " + Workspace);
                        this.SourceControl.DeleteWorkspace(Workspace, this.SourceControl.AuthenticatedUser);
                        Workspaces = new Workspace[0];
                    }
                    else
                    {
                        Log.Debug("Existing workspace detected - reusing");
                        MyWorkspace = Workspaces[0];
                    }
                }
                if (Workspaces.Length == 0)
                {
                    Log.Debug("Creating new workspace name: " + Workspace);
                    MyWorkspace = this.SourceControl.CreateWorkspace(Workspace, this.SourceControl.AuthenticatedUser, "Created By CCNet vstsbychangesetSourceControl.");
                }

                try
                {
                    MyWorkspace.Map(ProjectPath, WorkingDirectory);

                    Log.Debug(String.Format("Getting {0} to {1}", ProjectPath, WorkingDirectory));
                    GetRequest GetInfo;
                    GetInfo = new GetRequest(new ItemSpec(ProjectPath, RecursionType.Full), Set.ChangesetId);
                    
                    this.SourceControl.Getting += new GettingEventHandler(OnGet);
                    if (CleanCopy || Force)
                    {
                        Log.Debug("Forcing a Get Specific with the options \"get all files\" and \"overwrite read/write files\"");
                        MyWorkspace.Get(GetInfo, GetOptions.GetAll | GetOptions.Overwrite);
                    }
                    else
                    {
                        Log.Debug("Performing a Get Latest");
                        MyWorkspace.Get(GetInfo, GetOptions.None);
                    }
                }
                finally
                {
                    if (MyWorkspace != null && DeleteWorkspace)
                    {
                        Log.Debug("Deleting the workspace");
                        MyWorkspace.Delete();
                    }
                    this.SourceControl.Getting -= new GettingEventHandler(OnGet);
                }
            }
        }

        
        public void Initialize(IProject project)
        {
            this.Monitor.Subscribe();
        }

        public void LabelSourceControl(IIntegrationResult result)
        {
            if (ApplyLabel && result.Succeeded)
            {
                Log.Debug(String.Format("Applying label \"{0}\"", result.Label));
                VersionControlLabel Label = new VersionControlLabel(this.SourceControl, result.Label, _SourceControl.AuthenticatedUser, this.ProjectPath, "Labeled by CruiseControl.NET");

                Changeset Set = this.ChangesetQueue.GetCurrentIntegrationSet();

                LabelItemSpec[] LabelSpec = new LabelItemSpec[] {  
                    new LabelItemSpec(new ItemSpec(this.ProjectPath, RecursionType.Full), new ChangesetVersionSpec(Set.ChangesetId), false)
                };

                this.SourceControl.CreateLabel(Label, LabelSpec, LabelChildOption.Replace);
            }
            this.ChangesetQueue.EndIntegration();
        }

        public void Purge(IProject project)
        {
            //do nothing
        }

        #endregion

        #region Helpers

        private void OnGet(object sender, GettingEventArgs e)
        {
            Log.Debug(String.Format(
                "Status: '{0}',  ChangeType: '{1}', TargetLocalItem: '{2}' ServerItem: '{3}' Version: '{4}'", 
                e.Status.ToString(), e.ChangeType.ToString(), e.TargetLocalItem, e.ServerItem, e.Version));
        }

        /// <summary>
        ///   Delete a directory, even if it contains readonly files.
        /// </summary>
        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(WorkingDirectory))
            {
                this.MarkAllFilesReadWrite(path);
                Directory.Delete(path, true);
            }
        }

        private void MarkAllFilesReadWrite(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                file.IsReadOnly = false;
            }

            // Now recurse down the directories
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                this.MarkAllFilesReadWrite(dir.FullName);
            }
        }

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

        #endregion
    }
}