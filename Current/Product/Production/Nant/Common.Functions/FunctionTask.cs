using System;
using System.Collections;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Common.Functions
{
    [TaskName("function")]
    public class FunctionTask : Task
    {
        [TaskAttribute("execute", ExpandProperties=true, Required=true)]
        public String Execute
        {
            set
            {
            	 
            }
        }

        protected override void ExecuteTask()
        {
            
        }
    }
}
