using System;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("ifthenelse")]
    public class IfThenTask : IfTask
    {
        #region Fields

        private TaskContainer _Else;

        private TaskContainerCollection _ElseIf;

        private TaskContainer _Then;

        #endregion

        #region Properties

        [BuildElement("then", Required = true)]
        public TaskContainer Then
        {
            get { return _Then; }
            set { _Then = value; }
        }

        [BuildElementArray("elseif", Required = false)]
        public TaskContainerCollection ElseIf
        {
            get
            {
                if (_ElseIf == null)
                {
                    _ElseIf = new TaskContainerCollection();
                }
                return _ElseIf;
            }
            set { _ElseIf = value; }
        }

        [BuildElement("else", Required = false)]
        public TaskContainer Else
        {
            get { return _Else; }
            set { _Else = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            if ((this.ConditionsTrue))
            {
                this.Then.Execute();
            }
            else
            {
                bool Executed = false;
                foreach (TaskContainer Possible in this.ElseIf)
                {
                    if (Possible.IfDefined)
                    {
                        Possible.Execute();
                        Executed = true;
                    }
                }
                if (!Executed & this.Else != null)
                {
                    this.Else.Execute();
                }
            }
        }

        #endregion

    }

}