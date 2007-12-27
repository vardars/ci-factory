using System;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using System.Globalization;
using NAnt.Core.Util;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("ifthenelse")]
    public class IfThenTask : Task
    {

        #region Fields

        private TaskContainer _Else;

        private IfTask[] _ElseIfs;
        private TaskContainer[] _ElseIf;

        private string _test = null;

        private TaskContainer _Then;

        #endregion

        #region Properties

        protected virtual bool ConditionsTrue
        {
            get
            {
                bool ret = true;

                if (Test != null)
                {
                    if (!Convert.ToBoolean(Test, CultureInfo.InvariantCulture))
                    {
                        return false;
                    }
                }

                return ret;
            }
        }

        [BuildElement("then", Required = true)]
        public TaskContainer Then
        {
            get { return _Then; }
            set { _Then = value; }
        }

        [BuildElementCollection("elseifs", "elseif", Required = false)]
        public IfTask[] ElseIfs
        {
            get
            {
                if (_ElseIfs == null)
                {
                    _ElseIfs = new IfTask[] { };
                }
                return _ElseIfs;
            }
            set { _ElseIfs = value; }
        }

        [BuildElementArray("elseif", Required = false)]
        [Obsolete("Use the <elseifs> child element instead.", false)]
        public TaskContainer[] OldElseIf
        {
            get
            {
                if (_ElseIf == null)
                {
                    _ElseIf = new TaskContainer[] { };
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

        /// <summary>
        /// Used to test arbitrary boolean expression.
        /// </summary>
        [TaskAttribute("test", Required = true)]
        [BooleanValidator()]
        public string Test
        {
            get { return _test; }
            set { _test = StringUtils.ConvertEmptyToNull(value); }
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
                TaskContainerCollection ElseIfList = new TaskContainerCollection();
                ElseIfList.AddRange(this.ElseIfs);
                ElseIfList.AddRange(this.OldElseIf);
                foreach (TaskContainer Possible in ElseIfList)
                {
                    if (Possible.IfDefined)
                    {
                        Possible.Execute();
                        Executed = true;
                        break;
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