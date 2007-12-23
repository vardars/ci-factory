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

namespace TF.Tasks.SourceControl.Helpers
{

    public class WorkspaceAssistant
    {

        public Workspace GetWorkspaceByName(string workspaceName, VersionControlServer sourceControl)
        {
            Workspace MyWorkspace = null;

            if (String.IsNullOrEmpty(workspaceName))
            {
                throw new BuildException("Unable to determine Workspace to use as both the WorkspaceName and LocalItem are not set.");
            }

            Workspace[] Workspaces = sourceControl.QueryWorkspaces(workspaceName, sourceControl.AuthenticatedUser, Workstation.Current.Name);
            if (Workspaces.Length > 0)
                MyWorkspace = Workspaces[0];

            return MyWorkspace;
        }

        public Workspace GetLocalWorkspace(string localItem, TfsServerConnection serverConnection)
        {
            Workspace MyWorkspace;

            if (!Workstation.Current.IsMapped(localItem))
                throw new BuildException(String.Format("Unable to determine the Workspace to use, the path {0} does not map to a workspace.", localItem));

            WorkspaceInfo MyWorkSpaceInfo = Workstation.Current.GetLocalWorkspaceInfo(localItem);

            MyWorkspace = MyWorkSpaceInfo.GetWorkspace(serverConnection.TFS);

            return MyWorkspace;
        }

        public Workspace GetWorkspace(string workspaceName, string localItem, TfsServerConnection serverConnection)
        {
            Workspace MyWorkspace = null;

            if (String.IsNullOrEmpty(workspaceName) && String.IsNullOrEmpty(localItem))
                throw new BuildException("Unable to determine Workspace to use as both the WorkspaceName and LocalItem are not set.");

            if (!String.IsNullOrEmpty(workspaceName))
                MyWorkspace = this.GetWorkspaceByName(workspaceName, serverConnection.SourceControl);

            if (MyWorkspace == null && !String.IsNullOrEmpty(localItem))
                MyWorkspace = this.GetLocalWorkspace(localItem, serverConnection);

            if (MyWorkspace == null)
                throw new BuildException(String.Format("Unable to determine Workspace to use: WorkspaceName is set to {0} and LocalItem is set to {1}.", workspaceName, localItem));

            return MyWorkspace;
        }

    }

}
