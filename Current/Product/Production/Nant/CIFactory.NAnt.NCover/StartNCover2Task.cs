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
using System.Security.Policy;
using System.Reflection;
using System.Threading;

namespace NCoverExplorer.NAntTasks
{


    [Serializable]
	[TaskName("startncover2")]
    public class StartNCover2Task : Task
    {
        // Fields
        public static readonly object SyncLock = new object();

        private static bool _Signal;

        private string _additionalArgs = string.Empty;
        private FileSet _assemblyFiles = new FileSet();
        private string _assemblyList = string.Empty;
        private string _coverageFile = "Coverage.xml";
        private string _coverageHtmlDirectory = string.Empty;
        private NCoverCoverageType _coverageType = (NCoverCoverageType.Branch | NCoverCoverageType.SequencePoint);
        private string _excludeAttributes = string.Empty;
        private bool _excludeCompilerCode = true;
        private string _ExeName;
        private string _fileExclusionPatterns = string.Empty;
        private string _logFile = "Coverage.log";
        private NCoverLogLevel _logLevel = NCoverLogLevel.Quiet;
        private static Type _Main;
        private static Assembly _NCoverAssembly;
        private static AppDomain _NCoverAppDomain;
        private string _methodExclusionPatterns = string.Empty;
        private NCoverInfo _ncoverInfo;
        private string _profiledProcessModule = string.Empty;
        private bool _profileIIS = false;
        private string _profileService = string.Empty;
        private string _projectName = string.Empty;
        private bool _registerProfiler = true;
        private int _serviceTimeoutSeconds;
        private string _settingsFile = string.Empty;
        private int _symbolSearchPolicy = 15;
        private string _typeExclusionPatterns = string.Empty;
        private const string DefaultApplicationName = "NCover.Console.exe";

        // Methods
        public StartNCover2Task()
        {
            this.ExeName = "NCover.Console.exe";
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

        private void CreateNCoverInfo()
        {
            string[] fileNames = this._ConvertStringCollectionToArray(this._assemblyFiles.FileNames);
            this._ncoverInfo = new NCoverInfo();
            this._ncoverInfo.SetNCoverPathAndVersion(this.ExeName, true);
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

        private  void Prepare()
        {
            this._settingsFile = Path.GetTempFileName() + ".ncoversettings";
            this.Log(Level.Verbose, "Creating settings file: " + this._settingsFile);
            string str = NCoverSettingsFileWriter.BuildTempSettingsXmlFile(this._ncoverInfo, this._settingsFile);
            
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string str2 = File.ReadAllText(this._settingsFile);
                this.Log(Level.Verbose, "Contents of NCover settings file:");
                this.Log(Level.Verbose, str2);
            }
        }

        private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs e)
        {
            Assembly assembly2;
            try
            {
                string[] strArray = e.Name.Split(new char[] { ',' });
                assembly2 = Assembly.LoadFrom(Path.GetDirectoryName(this.ExeName) + @"\" + strArray[0] + ".dll");
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Failed resolving assembly", exception);
            }
            return assembly2;
        }

        protected override void ExecuteTask()
        {
            this.CreateNCoverInfo();
            if (this._registerProfiler)
            {
                this.Log(Level.Verbose, "Registering profiler");
                NCoverUtilities.RegisterNCover(Path.GetDirectoryName(this.ExeName), this._ncoverInfo.VersionNumber);
            }
            this.Prepare();

            Evidence securityInfo = new Evidence(AppDomain.CurrentDomain.Evidence);
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(this.ExeName);

            NCoverAppDomain = AppDomain.CreateDomain("NCover", securityInfo, setup);

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.AssemblyResolveHandler);

            NCoverAssembly = NCoverAppDomain.Load(Assembly.LoadFile(this.ExeName).GetName().Name);

            ConsoleMain = null;

            foreach (Type type in NCoverAssembly.GetTypes())
            {
                if (type.Name == "ConsoleMain")
                {
                    ConsoleMain = type;
                    break;
                }
            }

            this.SubvertConsoleOutput();

            Thread NCoverWorker = new Thread(new ThreadStart(Run));
            NCoverWorker.Start();
        }

        private void Run()
        {
            ConsoleMain.InvokeMember("Main", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { new string[] { "//r", this._settingsFile } });

            this.RestorConsoleOutput();

            lock (SyncLock)
            {
                Signal = true;
            }

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


        private TextWriter _StandardOut;
        private TextWriter StandardOut
        {
            get
            {
                return _StandardOut;
            }
            set
            {
                _StandardOut = value;
            }
        }

        private static StringWriter _Captured;
        public static StringWriter Captured
        {
            get
            {
                if (_Captured == null)
                    _Captured = new StringWriter();
                return _Captured;
            }
            set
            {
                _Captured = value;
            }
        }

        private void SubvertConsoleOutput()
        {
            this.StandardOut = Console.Out;
            FieldInfo OutFieldInfo = typeof(Console).GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            OutFieldInfo.SetValue(null, Captured);
        }

        private void RestorConsoleOutput()
        {
            FieldInfo OutFieldInfo = typeof(Console).GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            OutFieldInfo.SetValue(null, this.StandardOut);
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
        public string ExeName
        {
            get
            {
                return _ExeName;
            }
            set
            {
                _ExeName = value;
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

        public static Type ConsoleMain
        {
            get
            {
                return _Main;
            }
            set
            {
                _Main = value;
            }
        }

        public static AppDomain NCoverAppDomain
        {
            get
            {
                return _NCoverAppDomain;
            }
            set
            {
                _NCoverAppDomain = value;
            }
        }

        public static bool Signal
        {
            get
            {
                return _Signal;
            }
            set
            {
                _Signal = value;
            }
        }

        public static Assembly NCoverAssembly
        {
            get
            {
                return _NCoverAssembly;
            }
            set
            {
                _NCoverAssembly = value;
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
    }
}
