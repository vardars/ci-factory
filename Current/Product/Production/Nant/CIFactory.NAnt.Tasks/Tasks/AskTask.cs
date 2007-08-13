using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("ask")]
    public class AskTask : Task
    {
        #region Fields

        private string _AnswerProperty;
        private StringList _Options;
        private string _Question;
        private bool _ShowDialog = true;
        private string _Caption;

        #endregion

        #region Properties


        [TaskAttribute("caption", Required = false)]
        public string Caption
        {
            get
            {
                return _Caption;
            }
            set
            {
                _Caption = value;
            }
        }

        [TaskAttribute("showdialog", Required = false), BooleanValidator()]
        public bool ShowDialog
        {
            get
            {
                return _ShowDialog;
            }
            set
            {
                _ShowDialog = value;
            }
        }

        [BuildElement("options", Required = false)]
        public StringList Options
        {
            get { return _Options; }
            set { _Options = value; }
        }

        [TaskAttribute("answer", Required = true)]
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

        [TaskAttribute("question", Required = true)]
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
            string Answer;
            if (this.ShowDialog)
            {
                Ask.AskQuestion Questioner = new CIFactory.NAnt.Tasks.Ask.AskQuestion();
				int OptionCount = 0;
                foreach (String Choice in this.Options)
                {
					++OptionCount;
					if (OptionCount > 6)
						break;
                    Questioner.AddChoice(Choice);
                }
                Questioner.StateQuestion(this.Question);
                if (!String.IsNullOrEmpty(this.Caption))
                    Questioner.SetCaption(this.Caption);
                Questioner.ShowDialog();
                Answer = Questioner.GetAnswer();
            }
            else
            {
                Log(Level.Info, this.Question);
                Answer = Console.ReadLine();
            }
            this.Properties[this.AnswerProperty] = Answer;
        }

        #endregion

    }

    internal class Adhoctest
    {
        public void Test()
        {
            Ask.AskQuestion Questioner = new CIFactory.NAnt.Tasks.Ask.AskQuestion();
            Questioner.AddChoice("Install");
            Questioner.AddChoice("Don't Install");
            Questioner.AddChoice("Quit");
            Questioner.StateQuestion(string.Format("{0}{1}{0}", "I really have a lot to say.", Environment.NewLine));
            Questioner.ShowDialog();
            string Answer = Questioner.GetAnswer();
            Console.WriteLine(Answer);
        }
    }
}
