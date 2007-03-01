using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("loopthrough")]
    public class LoopThrough : Task
    {
        #region Fields

        private TaskContainer _Actions;

        private LoopItemContainer _Items;

        private string _PropertyName;

        #endregion

        #region Properties

        [BuildElement("do", Required = true)]
        public TaskContainer Actions
        {
            get { return _Actions; }
            set { _Actions = value; }
        }

        [BuildElement("items", Required = true)]
        public LoopItemContainer Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        [TaskAttribute("property", Required = true)]
        public string PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            foreach (string Item in this.Items)
            {
                if (this.Properties.Contains(this.PropertyName))
                {
                    this.Properties[this.PropertyName] = Item;
                }
                else
                {
                    this.Properties.Add(this.PropertyName, Item);
                }
                this.Items.Items.Executing(Item);
                this.Actions.Execute();
            }
        }

        #endregion

    }

}