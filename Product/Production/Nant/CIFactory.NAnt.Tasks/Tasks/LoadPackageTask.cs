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
// Ian MacLean (ian@maclean.ms)
// Gerry Shaw (gerry_shaw@yahoo.com)
// Scott Hernandez (ScottHernandez@_yeah_not_really_@hotmail.com)
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

namespace CIFactory.NAnt.Tasks
{
    [TaskName("loadpackage")]
    public class LoadPackageTask : Task {
        #region Private Instance Fields

        private string _PackagesDirectory = null;
        private StringItem[] _Packages;

        #endregion Private Static Fields

        #region Public Instance Properties

        private string PackagesDirectory
        {
            get
            {
                if (_PackagesDirectory == null)
                    _PackagesDirectory = this.Properties["Common.Directory.Packages.Path"];
                return _PackagesDirectory;
            }
        }

        [BuildElementArray("packages", ElementType = typeof(StringItem), Required = true)]
        public StringItem[] Packages
        {
            get { return _Packages; }
            set
            {
                _Packages = value;
            }
        }
        
        #endregion Public Instance Properties

        #region Override implementation of Task

        protected override void ExecuteTask() {
            foreach (String packageName in this.Packages.Select(item => item.StringValue))
            {
                String packageDirectoryPath = Path.Combine(this.PackagesDirectory, packageName);
                String PropertiesFilePath = GenerateFilePath("Properties.{0}.xml", packageName);
                if (!File.Exists(PropertiesFilePath))
                {
                    IncludeScriptFile(packageName, PropertiesFilePath);
                }

                String MacrosFilePath = GenerateFilePath("Macros.{{0}}.xml", packageName);
                if (File.Exists(MacrosFilePath))
                {
                    IncludeScriptFile(packageName, MacrosFilePath);
                }

                String TargetsFilePath = GenerateFilePath("Target[s]{{0,1}}.{0}.xml", packageName);
                if (!File.Exists(TargetsFilePath))
                {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                        ResourceUtils.GetString("NA1127"), TargetsFilePath), Location);
                }
                IncludeScriptFile(packageName, TargetsFilePath);

                this.Properties.Add("Common.Package." + packageName + ".Path", packageDirectoryPath);
            }
        }

        private String GenerateFilePath(String filePattern, String packageDirectoryPath)
        {
            String fileName = String.Format(filePattern, packageDirectoryPath);
            String filePath = Path.Combine(PackagesDirectory, fileName);
            if (!Path.IsPathRooted(filePath))
                filePath = Path.GetFullPath(Path.Combine(Project.BaseDirectory, filePath));
            return filePath;
        }

        private void IncludeScriptFile(String packageName, String filePath)
        {
            if (Project.ScriptFileInfoList.Count(delegate(ScriptFileInfo scriptInfo) { return scriptInfo.FilePath == filePath; }) == 1)
            {
                Log(Level.Verbose, ResourceUtils.GetString("String_DuplicateInclude"), filePath);
                return;
            }

            Log(Level.Verbose, "Loading package {0} file {1}.", packageName, filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            Project.InitializeProjectDocument(doc);
        }

        #endregion Override implementation of Task
    }
}
