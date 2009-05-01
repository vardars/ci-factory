using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("function")]
    public class FunctionTask : Task
    {
        #region Properties

        [TaskAttribute("execute", ExpandProperties = true, Required = true)]
        public String ExecuteFunction
        {
            set
            {

            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {

        }

        #endregion

    }
}
