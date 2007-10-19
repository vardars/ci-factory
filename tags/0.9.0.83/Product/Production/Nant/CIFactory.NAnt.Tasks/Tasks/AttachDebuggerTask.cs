using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("attachdebugger")]
    public class AttachDebuggerTask : Task
    {

        protected override void ExecuteTask()
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
