// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Jay Flowers jayflowers.com


using System;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using NAnt.Core.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("loadpackages")]
    public class LoadPackagesTask : Task {
        #region Private Instance Fields

        private string _Common.Directory.Packages.Path = null;
        private PackageElement[] _Packages;

        #endregion Private Static Fields

        #region Public Instance Properties

        private string Common.Directory.Packages.Path
        {
            get
            {
                if (_Common.Directory.Packages.Path == null)
                    _Common.Directory.Packages.Path = this.Properties["Common.Directory.Packages.Path"];
                return _Common.Directory.Packages.Path;
            }
        }

        [BuildElementArray("package", ElementType = typeof(PackageElement), Required = true)]
        public PackageElement[] Packages
        {
            get { return _Packages; }
            set
            {
                _Packages = value;
            }
        }
        
        #endregion Public Instance Properties

        #region Override implementation of Task

        private void SetProperty(String name, String value)
        {
            if (this.Properties.Contains(name))
                this.Properties[name] = value;
            else
                this.Properties.Add(name, value);
        }

        protected override void ExecuteTask() {
            foreach (PackageElement package in this.Packages.Where(package => package.If == true && package.Unless == false))
            {
                if (package.PackageType != null)
                    this.SetProperty("Package.Type." + package.PackageType, package.PackageName);

                this.SetProperty("Package." + package.PackageName + ".Name", package.PackageName);

                String packageDirectoryPath = Path.Combine(this.Common.Directory.Packages.Path, package.PackageName);
                this.SetProperty("Package." + package.PackageName + ".Directory.Path", packageDirectoryPath);

                String PropertiesFilePath = GenerateFilePath("{0}.Properties.xml", package, packageDirectoryPath);
                this.SetProperty("Package." + package.PackageName + ".Properties.File.Loaded", false.ToString());
                this.SetProperty("Package." + package.PackageName + ".Properties.File.Path", PropertiesFilePath);
                if (File.Exists(PropertiesFilePath))
                {
                    IncludeScriptFile(package.PackageName, PropertiesFilePath);
                    this.Properties["Package." + package.PackageName + ".Properties.File.Loaded"] = true.ToString();
                }

                String MacrosFilePath = GenerateFilePath("{0}.MacroDefs.xml", package, packageDirectoryPath);
                this.SetProperty("Package." + package.PackageName + ".MacroDefs.File.Loaded", false.ToString());
                this.SetProperty("Package." + package.PackageName + ".MacroDefs.File.Path", MacrosFilePath);
                if (File.Exists(MacrosFilePath))
                {
                    IncludeScriptFile(package.PackageName, MacrosFilePath); ;
                    this.Properties["Package." + package.PackageName + ".MacroDefs.File.Loaded"] = true.ToString();
                }

                String TargetsFilePath = GenerateFilePath("{0}.Targets.xml", package, packageDirectoryPath);
                this.SetProperty("Package." + package.PackageName + ".Targets.File.Loaded", false.ToString());
                this.SetProperty("Package." + package.PackageName + ".Targets.File.Path", TargetsFilePath);
                if (!File.Exists(TargetsFilePath))
                {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                        ResourceUtils.GetString("NA1127"), TargetsFilePath), Location);
                }
                IncludeScriptFile(package.PackageName, TargetsFilePath);
                this.Properties["Package." + package.PackageName + ".Targets.File.Loaded"] = true.ToString();

                String CustomFilePath = GenerateFilePath("{0}.Custom.xml", package, packageDirectoryPath);
                this.SetProperty("Package." + package.PackageName + ".Custom.File.Loaded", false.ToString());
                this.SetProperty("Package." + package.PackageName + ".Custom.File.Path", CustomFilePath);
                if (File.Exists(CustomFilePath))
                {
                    IncludeScriptFile(package.PackageName, CustomFilePath); ;
                    this.Properties["Package." + package.PackageName + ".Custom.File.Loaded"] = true.ToString();
                }

            }
        }

        private String GenerateFilePath(String filePattern, PackageElement package, String packageDirectoryPath)
        {
            String fileName;
            if (package.PackageType != null)
                fileName = String.Format(filePattern, package.PackageType);
            else
                fileName = String.Format(filePattern, package.PackageName);
            
            String filePath = Path.Combine(packageDirectoryPath, fileName);
            if (!Path.IsPathRooted(filePath))
                filePath = Path.GetFullPath(Path.Combine(Project.BaseDirectory, filePath));
            return filePath;
        }

        private void IncludeScriptFile(String packageName, String filePath)
        {
            if (Project.ScriptFileInfoList.Count(delegate(ScriptFileInfo scriptInfo) { return scriptInfo.FilePath == filePath; }) == 1)
            {
                Log(Level.Warning, ResourceUtils.GetString("String_DuplicateInclude"), filePath);
                return;
            }

            Log(Level.Info, "Loading package {0} file {1}.", packageName, filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            Project.InitializeProjectDocument(doc);
        }

        #endregion Override implementation of Task
    }
}
