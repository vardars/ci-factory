using System;
using System.Threading;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{

    [TaskName("while")]
    public class WhileTask : Task
    {

        private TaskContainer _Actions;
        private string _Condition;

        [TaskAttribute("condition", ExpandProperties = false, Required = true)]
        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
            }
        }


        [BuildElement("do", Required = true)]
        public TaskContainer Actions
        {
            get { return _Actions; }
            set { _Actions = value; }
        }

        protected override void ExecuteTask()
        {
            BreakTask.Break = false;
            while (bool.Parse(this.Project.ExpandProperties(this.Condition, this.Location)))
            {
                this.Actions.Execute();
                if (BreakTask.Break)
                {
                    break;
                }
            }
        }

    }
}
