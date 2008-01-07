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
    [TaskName("tfsdeleteworkspace")]
    public class TfsDeleteWorkspace : Task
    {

        #region Fields

        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;

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

        protected override void ExecuteTask()
        {
            this.ServerConnection.SourceControl.DeleteWorkspace(this.WorkspaceName, this.ServerConnection.SourceControl.AuthenticatedUser);
        }
    }

}
