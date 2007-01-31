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

namespace TF.Tasks.SourceControl.Tasks
{

    [FunctionSet("tfs-vc", "version control")]
    public class TfsVersionControlFunctions : FunctionSetBase
    {
        public TfsVersionControlFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        private TfsServerConnection GetServerConnection(string connectionRefId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRefId))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRefId));

            TfsServerConnection ServerConnection = (TfsServerConnection)this.Project.DataTypeReferences[connectionRefId];
            return ServerConnection;
        }
        
        [Function("workspace-exists")]
        public bool WorkspaceExists(string connectionRefId, string workspaceName)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            Workspace[] Workspaces = ServerConnection.SourceControl.QueryWorkspaces(workspaceName, ServerConnection.SourceControl.AuthenticatedUser, Workstation.Current.Name);
            if (Workspaces.Length > 0)
                return true;
            return false;
        }

        [Function("get-workspace-name")]
        public string GetWorkSpaceName(string connectionRefId, string path)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            WorkspaceInfo Info = Workstation.Current.GetLocalWorkspaceInfo(path);
            return Info.Name;
        }

        [Function("label-exists")]
        public bool LabelExists(string connectionRefId, string label, string scope)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            VersionControlLabel[] Labels = ServerConnection.SourceControl.QueryLabels(label, scope, null, true);
            return Labels.Length > 0;
        }
    }

}
