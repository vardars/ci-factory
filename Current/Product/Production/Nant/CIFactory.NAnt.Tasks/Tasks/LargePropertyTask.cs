using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;
using NAnt.Core.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("largeproperty")]
    public class LargePropertyTask : Task
    {
        #region Fields

        private string _PropertyNameName;

        private TextElement _TextValue;

        #endregion

        #region Properties

        [TaskAttribute("name", Required = true)]
        public string PropertyName
        {
            get { return _PropertyNameName; }
            set
            {
                if (_PropertyNameName == value)
                {
                    return;
                }
                _PropertyNameName = value;
            }
        }

        [BuildElement("value", Required = true)]
        public TextElement TextValue
        {
            get { return _TextValue; }
            set { _TextValue = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.Properties[this.PropertyName] = this.TextValue.Value;
        }

        #endregion

    }

}