using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{

    [TaskName("stringadd")]
    public class StringAddTask : Task
    {
        private StringItem[] _Strings;
        private string _RefId;

        [TaskAttribute("refid", Required=true)]
        public string RefId
        {
            get { return _RefId; }
            set
            {
                _RefId = value;
            }
        }

        [BuildElementArray("string", ElementType = typeof(StringItem), Required=true)]
        public StringItem[] Strings
        {
            set
            {
                _Strings = value;
            }
            get
            {
                return _Strings;
            }
        }


        protected override void ExecuteTask()
        {
            StringList RefStringList = null;
            if (!this.Project.DataTypeReferences.Contains(RefId))
            {
                RefStringList = new StringList();
                RefStringList.ID = this.RefId;
                RefStringList.Project = this.Project;
                RefStringList.Parent = this.Project;
                RefStringList.NamespaceManager = this.NamespaceManager;
                this.Project.DataTypeReferences.Add(RefStringList.ID, RefStringList);
            }

            RefStringList = (StringList)this.Project.DataTypeReferences[RefId];
            foreach (StringItem @string in Strings)
            {
                RefStringList.StringItems.Add(@string.StringValue, @string);
            }
            
        }
    }
}
