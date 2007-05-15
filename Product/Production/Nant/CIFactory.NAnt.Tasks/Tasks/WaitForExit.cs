using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;


namespace CIFactory.NAnt.Tasks
{
    [TaskName("waitforexit")]
    public class WaitForExit : Task
    {
        #region Fields

        private StringList _taskNames;

        #endregion

        #region Properties

        [BuildElement("tasknames", Required = true)]
        public StringList TaskNames
        {
            get { return _taskNames; }
            set { _taskNames = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            AsyncExec Worker;
            foreach (string TaskName in this.TaskNames.StringItems.Values)
            {
                Worker = AsyncExecList.GetTask(TaskName);
                Worker.Wait();
            }
        }

        #endregion

    }

}