using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("envvar", "Environment Variables")]
    public class EnvVarFunctions : FunctionSetBase
    {

        #region Constructors

        public EnvVarFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {

        }

        public EnvVarFunctions()
            : base(null, null)
        {

        }

        #endregion

        [Function("change-var")]
        public void ChangeVar(String refID, String name, String value)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            EnvironmentSet RefEnvironmentSet = (EnvironmentSet)this.Project.DataTypeReferences[refID];

            RefEnvironmentSet.EnvironmentVariables[name].Value = value;
        }
    }
}
