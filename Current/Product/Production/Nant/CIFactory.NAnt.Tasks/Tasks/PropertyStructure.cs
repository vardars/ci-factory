using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("propertystructure")]
    public class PropertyStructure : PropertyTask
    {
        #region Fields

        private PropertyStructure[] _Children;

        #endregion

        #region Properties

        [BuildElementArray("propertystructure", ElementType = typeof(PropertyStructure))]
        public PropertyStructure[] Children
        {
            get { return _Children; }
            set
            {
                _Children = value;
            }
        }

        [TaskAttribute("value", Required = false, ExpandProperties = false)]
        [StringValidator(AllowEmpty = true)]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            if (!String.IsNullOrEmpty(this.Value))
            {
                base.ExecuteTask();
            }

            foreach (PropertyStructure Child in this.Children)
            {
                Child.PropertyName = String.Format("{0}.{1}", this.PropertyName, Child.PropertyName);
                Child.Execute();
            }
        }

        #endregion

    }
}
