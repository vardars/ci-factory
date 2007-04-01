using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using TF.Tasks.SourceControl.Types;
using TF.Tasks.SourceControl.Helpers;

namespace TF.Tasks.SourceControl.Tasks
{
    [TaskName("tfsblock")]
    public class TfsBlock : Task
    {

        #region Fields

        private TfsServerConnection _ServerConnection;
        private TaskContainer _Tasks;
        private bool _Clean;
        private string _WorkspaceName;
        private WorkspaceAssistant _WorkspaceHelper;

        #endregion

        #region Properties

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

        [TaskAttribute("clean", Required = false), BooleanValidator()]
        public bool Clean
        {
            get
            {
                return _Clean;
            }
            set
            {
                _Clean = value;
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

        [BuildElement("do", Required = true)]
        public TaskContainer Tasks
        {
            get
            {
                return _Tasks;
            }
            set
            {
                _Tasks = value;
            }
        }

        #endregion

        #region Methods

        private void ProvideServerConnection(TfsBlockServices Services)
        {
            if (this.ServerConnection != null)
                Services.AddService(this.ServerConnection);
        }

        private void ProvideWorkSpace(TfsBlockServices Services)
        {
            if (this.ServerConnection == null)
                this.ServerConnection = (TfsServerConnection)Services.GetService(typeof(TfsServerConnection));

            if (this.WorkspaceName != null)
                Services.AddService(this.WorkspaceHelper.GetWorkspaceByName(this.WorkspaceName, this.ServerConnection.SourceControl));
        }

        protected override void ExecuteTask()
        {
            TfsBlockServices Services = (TfsBlockServices)TfsBlockServices.GetInstance();

            if (this.Clean)
                Services.Clear();

            this.ProvideServerConnection(Services);

            this.ProvideWorkSpace(Services);

            this.Tasks.Execute();

            if (this.Clean)
                Services.Clear();
        }

        #endregion

    }

}
