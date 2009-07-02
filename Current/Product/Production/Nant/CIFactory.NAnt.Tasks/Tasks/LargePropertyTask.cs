using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;
using NAnt.Core.Types;
using NAnt.Core.Tasks;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("largeproperty")]
    public class LargePropertyTask : Task
    {
        #region Fields

        private string _PropertyNameName;
        private TextElement _TextValue;
        protected bool _readOnly = false;
        protected bool _dynamic = false;
        protected bool _overwrite = true;

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

        /// <summary>
        /// Specifies whether the property is read-only or not. 
        /// The default is <see langword="false" />.
        /// </summary>
        [TaskAttribute("readonly", Required = false)]
        [BooleanValidator()]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        /// <summary>
        /// Specifies whether references to other properties should not be 
        /// expanded when the value of the property is set, but expanded when
        /// the property is actually used.  By default, properties will be
        /// expanded when set.
        /// </summary>
        [TaskAttribute("dynamic", Required = false)]
        [BooleanValidator()]
        public bool Dynamic
        {
            get { return _dynamic; }
            set { _dynamic = value; }
        }

        /// <summary>
        /// Specifies whether the value of a property should be overwritten if
        /// the property already exists (unless the property is read-only). 
        /// The default is <see langword="true" />.
        /// </summary>
        [TaskAttribute("overwrite", Required = false)]
        [BooleanValidator()]
        public bool Overwrite
        {
            get { return _overwrite; }
            set { _overwrite = value; }
        }

        #endregion Public Instance Properties


        #region Protected Methods

        protected override void ExecuteTask()
        {
            PropertyTask Property = new PropertyTask(this.PropertyName, this.TextValue.Value, this.ReadOnly, this.Dynamic, this.Overwrite);
            Property.Parent = this.Parent;
            Property.Project = this.Project;
            Property.Execute();
        }

        #endregion

    }

}