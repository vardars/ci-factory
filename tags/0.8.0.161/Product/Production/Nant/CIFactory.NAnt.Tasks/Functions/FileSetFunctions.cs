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

    [FunctionSet("fileset", "DataTypes")]
    public class FileSetFunctions : FunctionSetBase
    {
        #region Constructors

        public FileSetFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        #region Public Methods

        [Function("exclude-add")]
        public void AddExclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Excludes.Add(name);
        }

        [Function("include-add")]
        public void AddInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Includes.Add(name);
        }

        [Function("exclude-contains")]
        public Boolean ContainsExclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            return RefFileSet.Excludes.Contains(name);
        }

        [Function("include-contains")]
        public Boolean ContainsInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            return RefFileSet.Includes.Contains(name);
        }

        [Function("directory-count")]
        public int DirectoryCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Scan();
            return RefFileSet.DirectoryNames.Count;
        }

        [Function("exclude-count")]
        public int ExcludeCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            return RefFileSet.Excludes.Count;
        }

        [Function("file-count")]
        public int FileCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Scan();
            return RefFileSet.FileNames.Count;
        }

        [Function("include-count")]
        public int IncludeCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            return RefFileSet.Includes.Count;
        }

        [Function("exclude-remove")]
        public void RemoveExclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Excludes.Remove(name);
        }

        [Function("include-remove")]
        public void RemoveInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];
            RefFileSet.Includes.Remove(name);
        }

        [Function("get-name-at-index")]
        public string ToString(string refID, int index)
        {

            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];

            return RefFileSet.FileNames[index];
        }
        #endregion

    }
}
