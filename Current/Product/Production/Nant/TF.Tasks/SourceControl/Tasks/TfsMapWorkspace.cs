using System;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Functions;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

using TF.Tasks.SourceControl.Types;
using TF.Tasks.SourceControl.Helpers;

namespace TF.Tasks.SourceControl.Tasks
{

    [TaskName("tfsmapworkspace")]
    public class TfsMapWorkspace : Task
    {

        #region Fields

        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;
        private string _Comment;
        private MappingList _Mappings;

        #endregion

        #region Properties

        [BuildElement("tfsserverconnection", Required = true)]
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

        [BuildElement("mappings", Required = false)]
        public MappingList Mappings
        {
            get
            {
                return _Mappings;
            }
            set
            {
                _Mappings = value;
            }
        }

        [TaskAttribute("comment", Required = false)]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }

        [TaskAttribute("workspacename", Required = true)]
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

        #endregion

        #region Methods

        protected override void ExecuteTask()
        {
            Workspace[] Workspaces = this.ServerConnection.SourceControl.QueryWorkspaces(this.WorkspaceName, this.ServerConnection.SourceControl.AuthenticatedUser, Workstation.Current.Name);
            if (Workspaces.Length == 0)
            {
                this.ServerConnection.SourceControl.CreateWorkspace(this.WorkspaceName, this.ServerConnection.SourceControl.AuthenticatedUser, this.Comment, this.Mappings.GetMappings());
            }
            else
            {
                WorkspaceAssistant Helper = new WorkspaceAssistant();
                Workspace MyWorkspace = Helper.GetWorkspaceByName(this.WorkspaceName, this.ServerConnection.SourceControl);
                foreach (WorkingFolder Map in this.Mappings.GetMappings())
                {
                    MyWorkspace.Map(Map.ServerItem, Map.LocalItem);
                }
            }
        }

        #endregion

    }
}
