using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNET.TFS.Plugin
{
    [ReflectorType("vstsRollBack")]
    public class RollBackTask : ITask
	{

        #region Fields

        private ProcessExecutor _Executor;
        private string _Executable;
        private string _BaseDirectory;
        private ChangesetQueue _ChangesetQueue;
        private Changeset _BadChangeSet;
        private string _ProjectPath;

        #endregion

        #region Properties

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

        public Changeset BadChangeSet
        {
            get
            {
                return _BadChangeSet;
            }
            set
            {
                _BadChangeSet = value;
            }
        }

        public ChangesetQueue ChangesetQueue
        {
            get
            {
                if (_ChangesetQueue == null)
                    _ChangesetQueue = QueueFactory.GetChangesetQueue(this.ProjectPath, null);
                return _ChangesetQueue;
            }
            set
            {
                _ChangesetQueue = value;
            }
        }

        public ProcessExecutor Executor
        {
            get
            {
                if (_Executor == null)
                    _Executor = new ProcessExecutor();
                return _Executor;
            }
            set
            {
                _Executor = value;
            }
        }

        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get
            {
                if (String.IsNullOrEmpty(_Executable))
                    _Executable = String.Format(@"{0}\Microsoft Team Foundation Server Power Toys\TFPT.exe", Environment.GetEnvironmentVariable("ProgramFiles"));
                return _Executable;
            }
            set
            {
                _Executable = value;
            }
        }

        [ReflectorProperty("baseDirectory", Required = true)]
        public string BaseDirectory
        {
            get
            {
                return _BaseDirectory;
            }
            set
            {
                _BaseDirectory = value;
            }
        }

        #endregion

        #region Constructors

        public RollBackTask()
        {
            this.ChangesetQueue.DequeueEvent += new DequeueEventHandler(OnDequeueEvent);
        }

        #endregion

        #region Queue Listener

        private void OnDequeueEvent(Changeset changeset)
        {
            this.BadChangeSet = changeset;
        }

        #endregion

        #region Methods

        public void Run(IIntegrationResult result)
        {
            if (result.Succeeded)
                return;

            if (this.BadChangeSet == null)
            {
                Log.Debug("The changeset to rollback has not been set.");
                return;
            }

            string Arguments = string.Format("rollback /changeset:{0} /noprompt", this.BadChangeSet.ChangesetId);
            ProcessInfo Info = new ProcessInfo(this.Executable, Arguments, this.BaseDirectory);

            try
            {
                ProcessResult RollBackResult = this.Executor.Execute(Info, result.ProjectName);
                result.AddTaskResult(new ProcessTaskResult(RollBackResult));
            }
            catch (Exception ProcessException)
            {
                throw new BuilderException(this, string.Format("Unable to RollBack: {0}\n{1}", Info, ProcessException), ProcessException);
            }
            finally
            {
                this.BadChangeSet = null;
            }
        }

        #endregion
    }
}
