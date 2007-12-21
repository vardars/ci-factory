using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("switch")]
    public class SwitchTask : Task
    {
        #region Fields

        private CaseContainerCollection _Cases;

        private TaskContainer _Else;

        private string _LeftValue;

        #endregion

        #region Properties

        [BuildElementArray("case", Required = false)]
        public CaseContainerCollection Cases
        {
            get
            {
                if (_Cases == null)
                {
                    _Cases = new CaseContainerCollection();
                }
                return _Cases;
            }
            set { _Cases = value; }
        }

        [BuildElement("default", Required = false)]
        public TaskContainer Else
        {
            get { return _Else; }
            set { _Else = value; }
        }

        [TaskAttribute("value", Required = true)]
        public string LeftValue
        {
            get { return _LeftValue; }
            set { _LeftValue = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            foreach (CaseElement Case in this.Cases)
            {
                if (this.LeftValue == Case.RightValue)
                {
                    Case.Execute();
                    if (Case.Break)
                    {
                        return; // TODO: might not be correct. Was : Exit Sub
                    }
                }
            }
            this.Else.Execute();
        }

        #endregion

    }

}