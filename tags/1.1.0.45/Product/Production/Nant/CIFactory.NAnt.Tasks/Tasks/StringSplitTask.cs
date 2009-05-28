using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("stringsplit")]
    public class StringSplitTask : Task
    {
        private string _RefId;

        private string _StringToSplit;

        [TaskAttribute("stringtosplit", Required = true)]
        public string StringToSplit
        {
            get { return _StringToSplit; }
            set
            {
                _StringToSplit = value;
            }
        }


        private string _Delimiters;
        [TaskAttribute("delimiter", Required = true)]
        public string Delimiters
        {
            set
            {
                this._Delimiters = value;
            }
            get
            {
                return _Delimiters;
            }
        }

        [TaskAttribute("refid", Required = true)]
        public string RefId
        {
            get { return _RefId; }
            set
            {
                _RefId = value;
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

            foreach (String @string in this.StringToSplit.Split(this.Delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                RefStringList.StringItems.Add(@string, new StringItem(@string));
            }
        }
    }
}
