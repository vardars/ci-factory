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
    [Serializable]
    public class NCoverInfo
    {
        // Fields
        private string _additionalArgs;
        private string _assemblyFiles = string.Empty;
        private string _commandLineArgs = string.Empty;
        private string _commandLineExe = string.Empty;
        private string _coverageFile = "Coverage.xml";
        private string _coverageHtmlPath = string.Empty;
        private NCoverCoverageType _coverageType;
        private string _excludeAttributes = string.Empty;
        private bool _excludeCompilerCode;
        private string _fileExclusionPatterns;
        private string _logFile = "Coverage.log";
        private NCoverLogLevel _logLevel = NCoverLogLevel.Normal;
        private string _methodExclusionPatterns;
        private string _ncoverPath;
        private string _profiledProcessModule;
        private bool _profileIIS = false;
        private string _profileService = string.Empty;
        private string _projectName;
        private bool _registerCoverLib = true;
        private int _serviceTimeOutSeconds;
        private int _symbolSearchPolicy;
        private string _typeExclusionPatterns;
        private int _versionNumber;
        private string _workingDirectory = string.Empty;
        private readonly string DefaultApplicationName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"NCover\NCover.Console.exe");
        private const string DefaultCoverageXml = "Coverage.xml";
        private const string DefaultLogFile = "Coverage.log";

        // Methods
        public NCoverInfo()
        {
            this._ncoverPath = this.DefaultApplicationName;
            this._profiledProcessModule = string.Empty;
            this._projectName = string.Empty;
            this._excludeCompilerCode = true;
            this._coverageType = NCoverCoverageType.Branch | NCoverCoverageType.SequencePoint;
            this._symbolSearchPolicy = 15;
            this._additionalArgs = string.Empty;
            this._typeExclusionPatterns = string.Empty;
            this._methodExclusionPatterns = string.Empty;
            this._fileExclusionPatterns = string.Empty;
        }

        public void SetNCoverPathAndVersion(string ncoverPath, bool throwExceptionIfNotFound)
        {
            this._ncoverPath = ncoverPath;
            this._versionNumber = 0;
            string versionForExecutable = PathSearch.GetVersionForExecutable(this._ncoverPath, throwExceptionIfNotFound);
            if (versionForExecutable.Length > 0)
            {
                this._versionNumber = int.Parse(versionForExecutable.Replace(".", ""));
            }
        }

        // Properties
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

        public string AppToProfileArgs
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

        public string AppToProfileExe
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

        public string AssemblyList
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

        public string CoverageHtmlPath
        {
            get
            {
                return this._coverageHtmlPath;
            }
            set
            {
                this._coverageHtmlPath = value;
            }
        }

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

        public bool ExcludeAutoGenCode
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

        public bool HasCoverageFileSpecified
        {
            get
            {
                return ((this._coverageFile.Length > 0) && (this._coverageFile != "Coverage.xml"));
            }
        }

        public bool IsLegacyNCover133
        {
            get
            {
                return (this._versionNumber < 150);
            }
        }

        public bool IsValidCommandArgsForNCoverVersion
        {
            get
            {
                if (!this.IsLegacyNCover133)
                {
                    return true;
                }
                int index = this._commandLineArgs.IndexOf("\"");
                return ((index == -1) || (((index == 0) && (this._commandLineArgs.Length > 1)) && (this._commandLineArgs.IndexOf("\"", 1) == (this._commandLineArgs.Length - 1))));
            }
        }

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

        public string NCoverExe
        {
            get
            {
                return this._ncoverPath;
            }
            set
            {
                this.SetNCoverPathAndVersion(value, false);
            }
        }

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

        [XmlIgnore]
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

        public string ReadSettingsSwitch
        {
            get
            {
                if (!this.IsLegacyNCover133)
                {
                    return "//r";
                }
                return "/r";
            }
        }

        public bool RegisterCoverLib
        {
            get
            {
                return this._registerCoverLib;
            }
            set
            {
                this._registerCoverLib = value;
            }
        }

        public int ServiceTimeout
        {
            get
            {
                return this._serviceTimeOutSeconds;
            }
            set
            {
                this._serviceTimeOutSeconds = value;
            }
        }

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

        public int VersionNumber
        {
            get
            {
                return this._versionNumber;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return this._workingDirectory;
            }
            set
            {
                this._workingDirectory = value.Trim();
                if (this._workingDirectory.EndsWith(@"\") && (this._workingDirectory.Length > 3))
                {
                    this._workingDirectory = this._workingDirectory.Substring(0, this._workingDirectory.Length - 1);
                }
            }
        }
    }
}
