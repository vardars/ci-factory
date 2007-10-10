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
using TF.Tasks.SourceControl.Helpers;

namespace TF.Tasks.SourceControl.Tasks
{
    [TaskName("tfscheckin")]
    public class TfsCheckinTask : Task
    {

        #region Fields

        private bool _Recursive;
        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;
        private string _LocalItem;
        private WorkspaceAssistant _WorkspaceHelper;
        private FileSet _LocalItems;
        private string _Comment;

        #endregion

        #region Properties

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

        [BuildElement("localitems", Required = false)]
        public FileSet LocalItems
        {
            get
            {
                return _LocalItems;
            }
            set
            {
                _LocalItems = value;
            }
        }

        public WorkspaceAssistant WorkspaceHelper
        {
            get
            {
                if (_WorkspaceHelper == null)
                    _WorkspaceHelper = new WorkspaceAssistant();
                return _WorkspaceHelper;
            }
            set
            {
                _WorkspaceHelper = value;
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

        #endregion

        #region Methods

        protected override void ExecuteTask()
        {
            if (this.LocalItem == null && (this.LocalItems == null || this.LocalItems.FileNames.Count == 0 || this.LocalItems.DirectoryNames.Count == 0))
                throw new BuildException("There is nothing to add.", this.Location);

            Workspace MyWorkspace = this.WorkspaceHelper.GetWorkspaceByName(this.WorkspaceName, this.ServerConnection.SourceControl);

            RecursionType Recursion = RecursionType.None;
            if (this.Recursive)
                Recursion = RecursionType.Full;

            List<String> Items = new List<string>();

            if (!String.IsNullOrEmpty(this.LocalItem))
            {
                Items.Add(this.LocalItem);
                Log(Level.Verbose, "Adding file '{0}' to changeset.", this.LocalItem);
            }
            if (this.LocalItems != null)
            {
                if (this.LocalItems.DirectoryNames.Count > 0)
                {
                    foreach (String CurrentItem in this.LocalItems.DirectoryNames)
                    {
                        Items.Add(CurrentItem);
                        Log(Level.Verbose, "Adding directory '{0}' to changeset.", CurrentItem);
                    }
                }
                if (this.LocalItems.FileNames.Count > 0)
                {
                    foreach (String CurrentItem in this.LocalItems.FileNames)
                    {
                        Items.Add(CurrentItem);
                        Log(Level.Verbose, "Adding file '{0}' to changeset.", CurrentItem);
                    }
                }
            }

            PendingChange[] Changes = MyWorkspace.GetPendingChanges(Items.ToArray(), Recursion);
            MyWorkspace.CheckIn(Changes, this.Comment, null, null, null);
        }

        #endregion

    }
}
