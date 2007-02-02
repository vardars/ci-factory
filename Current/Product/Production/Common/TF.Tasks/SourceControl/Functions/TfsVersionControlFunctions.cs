using System;
using System.Collections;
using NAnt.Core;
using NAnt.Core.Attributes;
using Microsoft.TeamFoundation.VersionControl.Client;
using TF.Tasks.SourceControl.Tasks;
using TF.Tasks.SourceControl.Types;
using TF.Tasks.SourceControl.Helpers;

namespace TF.Tasks.SourceControl.Functions
{

    [FunctionSet("tfs-vc", "version control")]
    public class TfsVersionControlFunctions : FunctionSetBase
    {
        public TfsVersionControlFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #region GetServerConnection

        private TfsServerConnection GetServerConnection()
        {
            IServiceProvider ServiceProvider = TfsBlockServices.GetInstance();
            return (TfsServerConnection)ServiceProvider.GetService(typeof(TfsServerConnection));
        }

        private TfsServerConnection GetServerConnection(string connectionRefId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRefId))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRefId));

            TfsServerConnection ServerConnection = (TfsServerConnection)this.Project.DataTypeReferences[connectionRefId];
            return ServerConnection;
        }

        #endregion

        #region GetWorkspace

        private Workspace GetWorkspace(string connectionRefId, string workspaceName)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            WorkspaceAssistant Helper = new WorkspaceAssistant();
            Workspace MyWorkspace = Helper.GetWorkspaceByName(workspaceName, ServerConnection.SourceControl);
            return MyWorkspace;
        }

        private Workspace GetWorkspace()
        {
            IServiceProvider ServiceProvider = TfsBlockServices.GetInstance();
            return (Workspace)ServiceProvider.GetService(typeof(Workspace));
        }

        #endregion

        #region workspace-exists

        private bool WorkspaceExists(string workspaceName, TfsServerConnection ServerConnection)
        {
            bool Result = false;
            Workspace[] Workspaces = ServerConnection.SourceControl.QueryWorkspaces(workspaceName, ServerConnection.SourceControl.AuthenticatedUser, Workstation.Current.Name);
            if (Workspaces.Length > 0)
                Result = true;
            return Result;
        }

        [Function("workspace-exists")]
        public bool WorkspaceExists(string workspaceName)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection();
            bool Result = WorkspaceExists(workspaceName, ServerConnection);
            return Result;
        }

        [Function("workspace-exists")]
        public bool WorkspaceExists(string connectionRefId, string workspaceName)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            bool Result = WorkspaceExists(workspaceName, ServerConnection);
            return Result;
        }

        #endregion

        #region get-workspace-name

        private string GetWorkSpaceName(string path, TfsServerConnection serverConnection)
        {
            WorkspaceInfo Info = Workstation.Current.GetLocalWorkspaceInfo(path);
            string Name;
            Name = Info.Name;
            return Name;
        }

        [Function("get-workspace-name")]
        public string GetWorkSpaceName(string path)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection();
            string Name = this.GetWorkSpaceName(path, ServerConnection);

            return Name;
        }

        [Function("get-workspace-name")]
        public string GetWorkSpaceName(string connectionRefId, string path)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            string Name = this.GetWorkSpaceName(path, ServerConnection);

            return Name;
        }

        #endregion

        #region label-exists

        private bool LabelExists(string label, string scope, TfsServerConnection ServerConnection)
        {
            VersionControlLabel[] Labels = ServerConnection.SourceControl.QueryLabels(label, scope, null, true);
            bool Result;
            Result = Labels.Length > 0;
            return Result;
        }

        [Function("label-exists")]
        public bool LabelExists(string label, string scope)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection();

            bool Result = this.LabelExists(label, scope, ServerConnection);

            return Result;
        }

        [Function("label-exists")]
        public bool LabelExists(string connectionRefId, string label, string scope)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);

            bool Result = this.LabelExists(label, scope, ServerConnection);

            return Result;
        }

        #endregion

        #region get-latest-changeset-id

        public int GetLatestChangeSetId(TfsServerConnection serverConnection)
        {
            return serverConnection.SourceControl.GetLatestChangesetId();
        }

        [Function("get-latest-changeset-id")]
        public int GetLatestChangeSetId()
        {
            TfsServerConnection ServerConnection = this.GetServerConnection();
            return this.GetLatestChangeSetId(ServerConnection);
        }

        [Function("get-latest-changeset-id")]
        public int GetLatestChangeSetId(string connectionRefId)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            return this.GetLatestChangeSetId(ServerConnection);
        }

        #endregion

        #region get-latest-changeset-id-frompath

        public int GetLatestChangeSetIdFromPath(TfsServerConnection serverConnection, string path)
        {
            IEnumerable Enumerable = serverConnection.SourceControl.QueryHistory(path, VersionSpec.Latest, 0, RecursionType.Full, null, null, null, 1, false, false);
            IEnumerator Enumerator = Enumerable.GetEnumerator();
            bool Exists = Enumerator.MoveNext();
            if (Exists)
                return ((Changeset)Enumerator.Current).ChangesetId;
            return 0;
        }

        [Function("get-latest-changeset-id-frompath")]
        public int GetLatestChangeSetIdFromPath(string path)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection();
            return this.GetLatestChangeSetIdFromPath(ServerConnection, path);
        }

        [Function("get-latest-changeset-id-frompath")]
        public int GetLatestChangeSetIdFromPath(string connectionRefId, string path)
        {
            TfsServerConnection ServerConnection = this.GetServerConnection(connectionRefId);
            return this.GetLatestChangeSetIdFromPath(ServerConnection, path);
        }

        #endregion

        #region is-server-path-mapped

        public bool IsServerPathMapped(Workspace workspace, string serverItem)
        {
            return workspace.IsServerPathMapped(serverItem);
        }

        [Function("is-server-path-mapped")]
        public bool IsServerPathMapped(string serverItem)
        {
            return this.IsServerPathMapped(this.GetWorkspace(), serverItem);
        }

        [Function("is-server-path-mapped")]
        public bool IsServerPathMapped(string connectionRefId, string workspaceName, string serverItem)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.IsServerPathMapped(MyWorkspace, serverItem);
        }

        #endregion

        #region is-local-path-mapped

        public bool IsLocalPathMapped(Workspace workspace, string localItem)
        {
            return workspace.IsLocalPathMapped(localItem);
        }

        [Function("is-local-path-mapped")]
        public bool IsLocalPathMapped(string localItem)
        {
            return this.IsLocalPathMapped(this.GetWorkspace(), localItem);
        }

        [Function("is-local-path-mapped")]
        public bool IsLocalPathMapped(string connectionRefId, string workspaceName, string localItem)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.IsLocalPathMapped(MyWorkspace, localItem);
        }

        #endregion

        #region get-localitem-for-serveritem

        public string GetLocalItemForServerItem(Workspace workspace, string serverItem)
        {
            return workspace.GetLocalItemForServerItem(serverItem);
        }

        [Function("get-localitem-for-serveritem")]
        public string GetLocalItemForServerItem(string serverItem)
        {
            Workspace MyWorkspace = this.GetWorkspace();
            return this.GetLocalItemForServerItem(MyWorkspace, serverItem);
        }

        [Function("get-localitem-for-serveritem")]
        public string GetLocalItemForServerItem(string connectionRefId, string workspaceName, string serverItem)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.GetLocalItemForServerItem(MyWorkspace, serverItem);
        }

        #endregion

        #region has-pending-changes

        public bool HasPendingChanges(Workspace workspace)
        {
            return workspace.GetPendingChanges().Length != 0;
        }

        [Function("has-pending-changes")]
        public bool HasPendingChanges()
        {
            Workspace MyWorkspace = this.GetWorkspace();
            return this.HasPendingChanges(MyWorkspace);
        }

        [Function("has-pending-changes")]
        public bool HasPendingChanges(string connectionRefId, string workspaceName)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.HasPendingChanges(MyWorkspace);
        }

        public bool HasPendingChanges(Workspace workspace, string localItem)
        {
            return workspace.GetPendingChanges(localItem).Length != 0;
        }

        [Function("has-pending-changes")]
        public bool HasPendingChanges(string localItem)
        {
            Workspace MyWorkspace = this.GetWorkspace();
            return this.HasPendingChanges(MyWorkspace, localItem);
        }

        [Function("has-pending-changes")]
        public bool HasPendingChanges(string connectionRefId, string workspaceName, string localItem)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.HasPendingChanges(MyWorkspace, localItem);
        }

        #endregion

        #region get-serveritem-for-localitem

        public string GetServerItemForLocalItem(Workspace workspace, string localItem)
        {
            return workspace.GetServerItemForLocalItem(localItem);
        }

        [Function("get-serveritem-for-localitem")]
        public string GetServerItemForLocalItem(string localItem)
        {
            Workspace MyWorkspace = this.GetWorkspace();
            return this.GetServerItemForLocalItem(MyWorkspace, localItem);
        }

        [Function("get-serveritem-for-localitem")]
        public string GetServerItemForLocalItem(string connectionRefId, string workspaceName, string localItem)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.GetServerItemForLocalItem(MyWorkspace, localItem);
        }

        #endregion

        #region has-confilcts

        public bool HasConfilcts(Workspace workspace, string pathfilter, bool recursive)
        {
            return workspace.QueryConflicts(new String[1] { pathfilter }, recursive).Length != 0;
        }

        [Function("has-confilcts")]
        public bool HasConfilcts(string pathfilter, bool recursive)
        {
            Workspace MyWorkspace = this.GetWorkspace();
            return this.HasConfilcts(MyWorkspace, pathfilter, recursive);
        }

        [Function("has-confilcts")]
        public bool HasConfilcts(string connectionRefId, string workspaceName, string pathfilter, bool recursive)
        {
            Workspace MyWorkspace = this.GetWorkspace(connectionRefId, workspaceName);
            return this.HasConfilcts(MyWorkspace, pathfilter, recursive);
        }

        #endregion

    }

}
