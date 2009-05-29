using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("loglevel")]
    public class LogLevelTask : Task
    {
        #region Fields

        private Level _LogLevel;

        private TaskContainer _Tasks;

        #endregion

        #region Properties

        [TaskAttribute("level", Required = true)]
        public Level LogLevel
        {
            get
            {
                return _LogLevel;
            }
            set
            {
                _LogLevel = value;
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

        #region Protected Methods

        protected override void ExecuteTask()
        {
            Level OldLevel = this.Project.Threshold;
            this.Project.Threshold = this.LogLevel;
            try
            {
                this.Tasks.Execute();
            }
            finally
            {
                this.Project.Threshold = OldLevel;
            }

        }

        #endregion

    }
}
