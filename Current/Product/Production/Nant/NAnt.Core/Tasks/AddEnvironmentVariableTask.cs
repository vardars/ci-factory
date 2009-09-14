using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("addenvironmentvariable")]
    public class AddEnvironmentVariableTask : Task
    {
        private EnvironmentVariableCollection _environmentVariables = new EnvironmentVariableCollection();
        private string _RefId;

        [BuildElementArray("variable")]
        public EnvironmentVariableCollection EnvironmentVariables
        {
            get { return _environmentVariables; }
            set { _environmentVariables = value; }
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
            EnvironmentSet RefEnvironmentSet = null;
            if (!this.Project.DataTypeReferences.Contains(RefId))
            {
                RefEnvironmentSet = new EnvironmentSet();
                RefEnvironmentSet.ID = this.RefId;
                RefEnvironmentSet.Project = this.Project;
                RefEnvironmentSet.Parent = this.Project;
                RefEnvironmentSet.NamespaceManager = this.NamespaceManager;
                this.Project.DataTypeReferences.Add(RefEnvironmentSet.ID, RefEnvironmentSet);
            }

            RefEnvironmentSet = (EnvironmentSet)this.Project.DataTypeReferences[RefId];
            foreach (EnvironmentVariable EnvVar in EnvironmentVariables)
            {
                RefEnvironmentSet.EnvironmentVariables.Add(EnvVar);
            }

        }
    }
}
