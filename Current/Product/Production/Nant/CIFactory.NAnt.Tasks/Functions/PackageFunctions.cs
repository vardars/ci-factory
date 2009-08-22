using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using System.IO;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("package", "Package")]
    public class PackageFunctions : FunctionSetBase
    {
        private string _Common.Directory.Packages.Path = null;
        
        private string Common.Directory.Packages.Path
        {
            get
            {
                if (_Common.Directory.Packages.Path == null)
                    _Common.Directory.Packages.Path = this.Properties["Common.Directory.Packages.Path"];
                return _Common.Directory.Packages.Path;
            }
        }

        public PackageFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {

        }

        [Function("find-name-by-type")]
        public string FindNameByType(string type)
        {
            if (this.Properties.Contains("Package.Type." + type))
                return this.Properties["Package.Type." + type];

            FileSet fileset = new FileSet();
            fileset.Parent = this.Project;
            fileset.Project = this.Project;
            fileset.Includes.Add(String.Format(@"{0}\**\{1}.*.xml", this.Common.Directory.Packages.Path, type));
            return new DirectoryInfo(Path.GetDirectoryName(fileset.FileNames[0])).Name;
        }
    }
}
