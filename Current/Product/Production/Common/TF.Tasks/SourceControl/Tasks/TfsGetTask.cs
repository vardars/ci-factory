using System;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

using TF.Tasks.SourceControl.Types;

namespace TF.Tasks.SourceControl.Tasks
{
    [TaskName("tfsget")]
    public class TfsGetTask : Task
    {

        #region Fields

        private string _ServerItem;
        private VersionSpecElement _VersionSpec;
        private bool _All;
        private bool _OverWrite;
        private bool _Recursive;
        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;
        private string _LocalItem;

        #endregion

        #region Properties

        [TaskAttribute("all", Required = false), BooleanValidator]
        public bool All
        {
            get
            {
                return _All;
            }
            set
            {
                _All = value;
            }
        }

        [TaskAttribute("localitem", Required = false)]
        public string LocalItem
        {
            get
            {
                return _LocalItem;
            }
            set
            {
                _LocalItem = value;
            }
        }

        [TaskAttribute("workspacename", Required = false)]
        public string WorkspaceName
        {
            get
            {
                return _WorkspaceName;
            }
            set
            {
                _WorkspaceName = value;
            }
        }

        [TaskAttribute("overwrite", Required = false), BooleanValidator]
        public bool OverWrite
        {
            get
            {
                return _OverWrite;
            }
            set        
            {
                _OverWrite = value;
            }
        }       

        [TaskAttribute("recursive", Required = false), BooleanValidator]
        public bool Recursive
        {
            get
            {
                return _Recursive;
            }
            set
            {
                _Recursive = value;
            }
        }

        [TaskAttribute("serveritem", Required = false)]
        public string ServerItem
        {
            get
            {
                return _ServerItem;
            }
            set
            {
                _ServerItem = value;
            }
        }

        [BuildElement("versionspec", Required = false)]
        public VersionSpecElement VersionSpec
        {
            get
            {
                if (_VersionSpec == null)
                    _VersionSpec = new VersionSpecElement();
                return _VersionSpec;
            }
            set
            {
                _VersionSpec = value;
            }
        }

        [BuildElement("tfsserverconnection", Required = false)]
        public TfsServerConnection ServerConnection
        {
            get
            {
                return _ServerConnection;
            }
            set
            {
                _ServerConnection = value;
            }
        }

        #endregion


        #region Methods

        protected override void ExecuteTask()
        {
            if (String.IsNullOrEmpty(this.WorkspaceName) && String.IsNullOrEmpty(this.LocalItem))
                throw new BuildException("Unable to determine Workspace to use as both the WorkspaceName and LocalItem are not set.");

            Workspace MyWorkspace = null;

            if (!String.IsNullOrEmpty(this.WorkspaceName))
            {
                Workspace[] Workspaces = this.ServerConnection.SourceControl.QueryWorkspaces(this.WorkspaceName, this.ServerConnection.SourceControl.AuthenticatedUser, Workstation.Current.Name);
                if (Workspaces.Length > 0)
                    MyWorkspace = Workspaces[0];
            }

            if (MyWorkspace == null && !String.IsNullOrEmpty(this.LocalItem))
            {
                if (!Workstation.Current.IsMapped(this.LocalItem))
                    throw new BuildException(String.Format("Unable to determine the Workspace to use, the path {0} does not map to a workspace.", this.LocalItem));

                WorkspaceInfo MyWorkSpaceInfo = Workstation.Current.GetLocalWorkspaceInfo(this.LocalItem);

                if (this.ServerConnection == null)
                    this.ServerConnection = new TfsServerConnection(MyWorkSpaceInfo.ServerUri.ToString());

                this.WorkspaceName = MyWorkSpaceInfo.Name;
                MyWorkspace = MyWorkSpaceInfo.GetWorkspace(this.ServerConnection.TFS);
            }

            if (MyWorkspace == null)
                throw new BuildException(String.Format("Unable to determine Workspace to use: WorkspaceName is set to {0} and LocalItem is set to {1}.", this.WorkspaceName, this.LocalItem));
            
            GetOptions Options = GetOptions.None;
            if (this.All && this.OverWrite)
            {
                Options = GetOptions.GetAll | GetOptions.Overwrite;
            }
            else if (this.All)
            {
                Options = GetOptions.GetAll;
            }
            else if (this.OverWrite)
            {
                Options = GetOptions.Overwrite;
            }

            if (!String.IsNullOrEmpty(this.ServerItem))
            {
                RecursionType Recursion = RecursionType.None;
                if (this.Recursive)
                    Recursion = RecursionType.Full;

                GetRequest GetReq = new GetRequest(new ItemSpec(this.ServerItem, Recursion), this.VersionSpec.GetVersionSpec());
                MyWorkspace.Get(GetReq, Options);
            }
            else
            {
                MyWorkspace.Get(this.VersionSpec.GetVersionSpec() ,Options);
            }
        }

        #endregion
    }
}
