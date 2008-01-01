#region Copyright © 2006 Grant Drake. All rights reserved.
/*
Copyright © 2006 Grant Drake. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using NAnt.Core.Util;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NCoverExplorer.NAntTasks
{
    [TaskName("ncover")]
    public class NCoverTask : ExternalProgramBase
    {
        // Fields
        private string _additionalArgs = string.Empty;
        private FileSet _assemblyFiles = new FileSet();
        private string _assemblyList = string.Empty;
        private string _commandLineArgs = string.Empty;
        private string _commandLineExe = string.Empty;
        private string _coverageFile = "Coverage.xml";
        private string _coverageHtmlDirectory = string.Empty;
        private NCoverCoverageType _coverageType = (NCoverCoverageType.Branch | NCoverCoverageType.SequencePoint);
        private string _excludeAttributes = string.Empty;
        private bool _excludeCompilerCode = true;
        private string _fileExclusionPatterns = string.Empty;
        private string _logFile = "Coverage.log";
        private NCoverLogLevel _logLevel = NCoverLogLevel.Quiet;
        private string _methodExclusionPatterns = string.Empty;
        private NCoverInfo _ncoverInfo;
        private string _profiledProcessModule = string.Empty;
        private bool _profileIIS = false;
        private string _profileService = string.Empty;
        private string _programArguments;
        private string _projectName = string.Empty;
        private bool _registerProfiler = true;
        private int _serviceTimeoutSeconds;
        private string _settingsFile = string.Empty;
        private int _symbolSearchPolicy = 15;
        private string _typeExclusionPatterns = string.Empty;
        private string _workingDirectory = string.Empty;
        private const string DefaultApplicationName = "NCover.Console.exe";

        // Methods
        public NCoverTask()
        {
            this.ExeName = "NCover.Console.exe";
            this._programArguments = string.Empty;
        }

        private string _AppendAssemblyNames(string assemblyList, string[] fileNames)
        {
            foreach (string str in fileNames)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
                assemblyList = assemblyList + ";" + fileNameWithoutExtension;
            }
            if (assemblyList.StartsWith(";"))
            {
                assemblyList = assemblyList.Substring(1);
            }
            return assemblyList;
        }

        private string[] _ConvertStringCollectionToArray(StringCollection files)
        {
            ArrayList list = new ArrayList();
            foreach (string str in files)
            {
                list.Add(str);
            }
            return (string[])list.ToArray(typeof(string));
        }

        private void _CreateNCoverInfo()
        {
            string[] fileNames = this._ConvertStringCollectionToArray(this._assemblyFiles.FileNames);
            this._ncoverInfo = new NCoverInfo();
            this._ncoverInfo.SetNCoverPathAndVersion(this.ExeName, true);
            this._ncoverInfo.AppToProfileExe = this._commandLineExe;
            this._ncoverInfo.AppToProfileArgs = this._commandLineArgs;
            this._ncoverInfo.WorkingDirectory = this._workingDirectory;
            this._ncoverInfo.AssemblyList = this._AppendAssemblyNames(this._assemblyList, fileNames);
            this._ncoverInfo.CoverageFile = this._coverageFile;
            this._ncoverInfo.LogLevel = this._logLevel;
            this._ncoverInfo.LogFile = this._logFile;
            this._ncoverInfo.ExcludeAttributes = this._excludeAttributes;
            this._ncoverInfo.ProfileIIS = this._profileIIS;
            this._ncoverInfo.ProfileService = this._profileService;
            this._ncoverInfo.RegisterCoverLib = this._registerProfiler;
            this._ncoverInfo.ProfiledProcessModule = this._profiledProcessModule;
            this._ncoverInfo.ProjectName = this._projectName;
            this._ncoverInfo.CoverageHtmlPath = this._coverageHtmlDirectory;
            this._ncoverInfo.ExcludeAutoGenCode = this._excludeCompilerCode;
            this._ncoverInfo.CoverageType = this._coverageType;
            this._ncoverInfo.SymbolSearchPolicy = this._symbolSearchPolicy;
            this._ncoverInfo.AdditionalArgs = this._additionalArgs;
            this._ncoverInfo.TypeExclusionPatterns = this._typeExclusionPatterns;
            this._ncoverInfo.MethodExclusionPatterns = this._methodExclusionPatterns;
            this._ncoverInfo.FileExclusionPatterns = this._fileExclusionPatterns;
            this._ncoverInfo.ServiceTimeout = this._serviceTimeoutSeconds;
        }

        private void _OnProcessExited(object sender, EventArgs e)
        {
            if (File.Exists(this._settingsFile))
            {
                this.Log(Level.Verbose, "Deleting settings file: " + this._settingsFile);
                File.Delete(this._settingsFile);
            }
            if (this._registerProfiler)
            {
                NCoverUtilities.UnregisterNCover();
            }
        }

        protected override void PrepareProcess(Process process)
        {
            this._settingsFile = Path.GetTempFileName() + ".ncoversettings";
            this.Log(Level.Verbose, "Creating settings file: " + this._settingsFile);
            string str = NCoverSettingsFileWriter.BuildTempSettingsXmlFile(this._ncoverInfo, this._settingsFile);
            this._programArguments = string.Format("{0} \"{1}\" {2}", str, this._settingsFile, this._additionalArgs);
            this.Log(Level.Verbose, "Working directory: {0}", new object[] { this.BaseDirectory });
            this.Log(Level.Verbose, "Arguments: {0}", new object[] { this._programArguments });
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string str2 = File.ReadAllText(this._settingsFile);
                this.Log(Level.Verbose, "Contents of NCover settings file:");
                this.Log(Level.Verbose, str2);
            }
            base.PrepareProcess(process);
        }

        protected override Process StartProcess()
        {
            if (!Path.IsPathRooted(this.ExeName))
            {
                this.ExeName = this.Project.GetFullPath(this.ExeName);
            }
            this._CreateNCoverInfo();
            if (this._registerProfiler)
            {
                this.Log(Level.Verbose, "Registering profiler");
                NCoverUtilities.RegisterNCover(Path.GetDirectoryName(this.ExeName), this._ncoverInfo.VersionNumber);
            }
            Process process = base.StartProcess();
            process.Exited += new EventHandler(this._OnProcessExited);
            return process;
        }

        // Properties
        [TaskAttribute("additionalArgs", Required = false)]
        public string AdditionalArgs
        {
            get
            {
                return this._additionalArgs;
            }
            set
            {
                this._additionalArgs = value;
            }
        }

        [BuildElement("assemblies")]
        public FileSet Assemblies
        {
            get
            {
                return this._assemblyFiles;
            }
            set
            {
                this._assemblyFiles = value;
            }
        }

        [TaskAttribute("assemblyList")]
        public string AssemblyList
        {
            get
            {
                return this._assemblyList;
            }
            set
            {
                this._assemblyList = value;
            }
        }

        [TaskAttribute("autoExclude"), BooleanValidator]
        public bool AutoExclude
        {
            get
            {
                return this._excludeCompilerCode;
            }
            set
            {
                this._excludeCompilerCode = value;
            }
        }

        [TaskAttribute("commandLineArgs", Required = false)]
        public string CommandLineArgs
        {
            get
            {
                return this._commandLineArgs;
            }
            set
            {
                this._commandLineArgs = value;
            }
        }

        [TaskAttribute("commandLineExe"), StringValidator(AllowEmpty = false)]
        public string CommandLineExe
        {
            get
            {
                return this._commandLineExe;
            }
            set
            {
                this._commandLineExe = value;
            }
        }

        [TaskAttribute("coverageFile", Required = false)]
        public string CoverageFile
        {
            get
            {
                return this._coverageFile;
            }
            set
            {
                this._coverageFile = value;
            }
        }

        [TaskAttribute("coverageHtmlDirectory", Required = false)]
        public string CoverageHtmlDirectory
        {
            get
            {
                return this._coverageHtmlDirectory;
            }
            set
            {
                this._coverageHtmlDirectory = value;
            }
        }

        [TaskAttribute("coverageType", Required = false)]
        public NCoverCoverageType CoverageType
        {
            get
            {
                return this._coverageType;
            }
            set
            {
                this._coverageType = value;
            }
        }

        [TaskAttribute("excludeAttributes")]
        public string ExcludeAttributes
        {
            get
            {
                return this._excludeAttributes;
            }
            set
            {
                this._excludeAttributes = value;
            }
        }

        [StringValidator(AllowEmpty = false), TaskAttribute("program", Required = false)]
        public override string ExeName
        {
            get
            {
                return base.ExeName;
            }
            set
            {
                base.ExeName = value;
            }
        }

        [TaskAttribute("fileExclusionPatterns", Required = false)]
        public string FileExclusionPatterns
        {
            get
            {
                return this._fileExclusionPatterns;
            }
            set
            {
                this._fileExclusionPatterns = value;
            }
        }

        [TaskAttribute("logFile", Required = false)]
        public string LogFile
        {
            get
            {
                return this._logFile;
            }
            set
            {
                this._logFile = value;
            }
        }

        [TaskAttribute("logLevel")]
        public NCoverLogLevel LogLevel
        {
            get
            {
                return this._logLevel;
            }
            set
            {
                this._logLevel = value;
            }
        }

        [TaskAttribute("methodExclusionPatterns", Required = false)]
        public string MethodExclusionPatterns
        {
            get
            {
                return this._methodExclusionPatterns;
            }
            set
            {
                this._methodExclusionPatterns = value;
            }
        }

        [TaskAttribute("profiledProcessModule", Required = false)]
        public string ProfiledProcessModule
        {
            get
            {
                return this._profiledProcessModule;
            }
            set
            {
                this._profiledProcessModule = value;
            }
        }

        [TaskAttribute("profileIIS"), BooleanValidator]
        public bool ProfileIIS
        {
            get
            {
                return this._profileIIS;
            }
            set
            {
                this._profileIIS = value;
            }
        }

        [TaskAttribute("profileService")]
        public string ProfileService
        {
            get
            {
                return this._profileService;
            }
            set
            {
                this._profileService = value;
            }
        }

        public override string ProgramArguments
        {
            get
            {
                return this._programArguments;
            }
        }

        [TaskAttribute("projectName", Required = false)]
        public string ProjectName
        {
            get
            {
                return this._projectName;
            }
            set
            {
                this._projectName = value;
            }
        }

        [TaskAttribute("registerProfiler", Required = false), BooleanValidator]
        public bool RegisterProfiler
        {
            get
            {
                return this._registerProfiler;
            }
            set
            {
                this._registerProfiler = value;
            }
        }

        [TaskAttribute("serviceTimeout", Required = false)]
        public int ServiceTimeout
        {
            get
            {
                return this._serviceTimeoutSeconds;
            }
            set
            {
                this._serviceTimeoutSeconds = value;
            }
        }

        [TaskAttribute("symbolSearchPolicy", Required = false)]
        public int SymbolSearchPolicy
        {
            get
            {
                return this._symbolSearchPolicy;
            }
            set
            {
                this._symbolSearchPolicy = value;
            }
        }

        [TaskAttribute("typeExclusionPatterns", Required = false)]
        public string TypeExclusionPatterns
        {
            get
            {
                return this._typeExclusionPatterns;
            }
            set
            {
                this._typeExclusionPatterns = value;
            }
        }

        [TaskAttribute("workingDirectory", Required = false)]
        public string WorkingDirectory
        {
            get
            {
                return this._workingDirectory;
            }
            set
            {
                this._workingDirectory = value;
            }
        }
    }
    
}