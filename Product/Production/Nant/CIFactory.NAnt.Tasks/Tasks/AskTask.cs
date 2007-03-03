using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("ask")]
    public class AskTask : Task
    {
        #region Fields

        private string _AnswerProperty;

        private string _Question;

        #endregion

        #region Properties

        [TaskAttribute("answer")]
        public string AnswerProperty
        {
            get
            {
                return _AnswerProperty;
            }
            set
            {
                _AnswerProperty = value;
            }
        }

        [TaskAttribute("question")]
        public string Question
        {
            get
            {
                return _Question;
            }
            set
            {
                _Question = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            Log(Level.Info, this.Question);
            String Answer = Console.ReadLine();
            this.Properties[this.AnswerProperty] = Answer;
        }

        #endregion

    }
}
