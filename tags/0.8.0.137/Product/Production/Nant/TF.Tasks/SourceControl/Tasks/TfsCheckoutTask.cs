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
    [TaskName("tfscheckout")]
    public class TfsCheckoutTask : Task
    {

        #region Fields

        private bool _Recursive;
        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;
        private string _LocalItem;
        private WorkspaceAssistant _WorkspaceHelper;
        private FileSet _LocalItems;

        #endregion

        #region Properties

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
                throw new BuildException("There is nothing to checkout.", this.Location);

            Workspace MyWorkspace = this.WorkspaceHelper.GetWorkspaceByName(this.WorkspaceName, this.ServerConnection.SourceControl);

            RecursionType Recursion = RecursionType.None;
            if (this.Recursive)
                Recursion = RecursionType.Full;

            if (!String.IsNullOrEmpty(this.LocalItem))
            {
                MyWorkspace.PendEdit(this.LocalItem, Recursion);
                Log(Level.Verbose, "Checkingout: '{0}', Recursive Flag: {1}, Workspace: '{2}'", this.LocalItem, this.Recursive.ToString(), this.WorkspaceName);
            }
            if (this.LocalItems != null)
            {
                foreach (String CurrentItem in this.LocalItems.DirectoryNames)
                {
                    MyWorkspace.PendEdit(CurrentItem, Recursion);
                    Log(Level.Verbose, "Checkingout Directory: '{0}', Recursive Flag: {1}, Workspace: '{2}'", CurrentItem, this.Recursive.ToString(), this.WorkspaceName);
                }

                foreach (String CurrentItem in this.LocalItems.FileNames)
                {
                    MyWorkspace.PendEdit(CurrentItem);
                    Log(Level.Verbose, "Checkingout File: '{0}', Workspace: '{1}'", CurrentItem, this.WorkspaceName);
                }
            }
        }

        #endregion

    }
}
