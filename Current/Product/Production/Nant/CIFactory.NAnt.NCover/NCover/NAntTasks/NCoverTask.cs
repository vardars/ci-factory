namespace NCover.NAntTasks
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Tasks;
    using NAnt.Core.Types;
    using NCover.Framework;
    using NCover.Framework.Configuration;
    using NCover.Framework.Scripting;
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    [TaskName("ncover")]
    public class NCoverTask : ExternalProgramBase
    {
        private FileSet _assemblyFiles = new FileSet();
        private ICoverageConfig _config = new CoverageConfig();
        private string _programArguments = string.Empty;
        private bool _userSetConfigFile = false;
        private const string IISService = "W3SVC";

        public NCoverTask()
        {
            this.AdditionalArgs = string.Empty;
        }

        private string[] _ConvertStringCollectionToArray(StringCollection files)
        {
            ArrayList list = new ArrayList();
            foreach (string str in files)
            {
                list.Add(str);
            }
            return (string[]) list.ToArray(typeof(string));
        }

        private void _CreateNCoverInfo()
        {
            string str2;
            int num;
            string[] strArray = this._ConvertStringCollectionToArray(this._assemblyFiles.FileNames);
            string str = string.Join(";", strArray) + ";" + this.includeAssemblies;
            this.includeAssemblies = str;
            this._GetNCoverPathAndVersion(this.ExeName, true, out str2, out num);
        }

        private void _GetNCoverPathAndVersion(string ncoverPath, bool throwExceptionIfNotFound, out string NCoverPath, out int version)
        {
            NCoverPath = ncoverPath;
            version = 0;
            string versionForExecutable = PathSearch.GetVersionForExecutable(ncoverPath, throwExceptionIfNotFound);
            if (versionForExecutable.Length > 0)
            {
                version = int.Parse(versionForExecutable.Replace(".", ""));
            }
        }

        private void _LogDeprecatedPropertyMessage(string deprecatedName, string replacementName)
        {
            this.Log(Level.Info, "The '{0}' property has been deprecated and will be removed in v4.0. Please use the '{1}' property instead.", new object[] { deprecatedName, replacementName });
        }

        private void _LogObsoletePropertyMessage(string obsoleteName)
        {
            this.Log(Level.Info, "The '{0}' property is obsolete and will be removed in v4.0.", new object[] { obsoleteName });
        }

        private void _OnProcessExited(object sender, EventArgs e)
        {
            if (File.Exists(this.SettingsFile) && !this._userSetConfigFile)
            {
                this.Log(Level.Verbose, "Deleting settings file: " + this.SettingsFile);
                File.Delete(this.SettingsFile);
            }
        }

        protected override void PrepareProcess(Process process)
        {
            this._userSetConfigFile = !string.IsNullOrEmpty(this.SettingsFile);
            if (!this._userSetConfigFile)
            {
                this.SettingsFile = Path.GetTempFileName() + ".ncoversettings";
            }
            this.Log(Level.Verbose, "Creating settings file: " + this.SettingsFile);
            StringBuilder builder = new StringBuilder(100);
            foreach (Argument argument in this.Arguments)
            {
                builder.Append(argument.ToString() + " ");
            }
            this.AdditionalArgs = this.AdditionalArgs + " " + builder.ToString();
            this._config.Save(this.SettingsFile);
            this._programArguments = string.Format("//cr \"{0}\" {1}", this.SettingsFile, this.AdditionalArgs);
            this.Log(Level.Verbose, "Working directory: {0}", new object[] { this.BaseDirectory });
            this.Log(Level.Verbose, "Arguments: {0}", new object[] { this._programArguments });
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string message = File.ReadAllText(this.SettingsFile);
                this.Log(Level.Verbose, "Contents of NCover settings file:");
                this.Log(Level.Verbose, message);
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
            Process process = base.StartProcess();
            process.Exited += new EventHandler(this._OnProcessExited);
            return process;
        }

        [TaskAttribute("additionalArgs")]
        public string AdditionalArgs { get; set; }

        [TaskAttribute("appendTrendTo")]
        public string appendTrendTo
        {
            get
            {
                return this._config.CoverageTrend;
            }
            set
            {
                this._config.CoverageTrend = value;
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
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeAssemblies);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeAssemblies = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("assemblyList", "includeAssemblies");
            }
        }

        [TaskAttribute("autoExclude")]
        public bool AutoExclude
        {
            get
            {
                return !this._config.IncludeCompilerGeneratedCode;
            }
            set
            {
                this._config.IncludeCompilerGeneratedCode = !value;
                this._LogDeprecatedPropertyMessage("autoExclude", "includeAutoGenCode");
            }
        }

        [TaskAttribute("buildId")]
        public string buildId
        {
            get
            {
                return this._config.BuildId;
            }
            set
            {
                this._config.BuildId = value;
            }
        }

        [TaskAttribute("commandLineArgs")]
        public string CommandLineArgs
        {
            get
            {
                return this._config.CommandLineArgs;
            }
            set
            {
                this._config.CommandLineArgs = value;
                this._LogDeprecatedPropertyMessage("commandLineArgs", "testRunnerArgs");
            }
        }

        [TaskAttribute("commandLineExe")]
        public string CommandLineExe
        {
            get
            {
                return this._config.CommandLineExe;
            }
            set
            {
                this._config.CommandLineExe = value;
                this._LogDeprecatedPropertyMessage("commandLineExe", "testRunnerExe");
            }
        }

        [TaskAttribute("coverageFile")]
        public string coverageFile
        {
            get
            {
                return this._config.CoverageFile;
            }
            set
            {
                this._config.CoverageFile = value;
            }
        }

        [TaskAttribute("coverageHtmlDirectory")]
        public string CoverageHtmlDirectory
        {
            get
            {
                return this._config.HtmlReportDir;
            }
            set
            {
                this._config.HtmlReportDir = value;
                this._LogDeprecatedPropertyMessage("coverageHtmlDirectory", "htmlReportDir");
            }
        }

        [TaskAttribute("coverageMetrics")]
        public string coverageMetrics
        {
            get
            {
                ConvertCoverageTypeEnumAttribute attribute = new ConvertCoverageTypeEnumAttribute(typeof(NCover.Interfaces.Enumerations.CoverageType));
                return attribute.ConvertFrom(this._config.CoverageMetrics);
            }
            set
            {
                ConvertCoverageTypeEnumAttribute attribute = new ConvertCoverageTypeEnumAttribute(typeof(NCover.Interfaces.Enumerations.CoverageType));
                this._config.CoverageMetrics = (NCover.Interfaces.Enumerations.CoverageType) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("coverageType")]
        public NCover.Interfaces.Enumerations.CoverageType CoverageType
        {
            get
            {
                return this._config.CoverageMetrics;
            }
            set
            {
                this._config.CoverageMetrics = value;
                this._LogDeprecatedPropertyMessage("coverageType", "coverageMetrics");
            }
        }

        [TaskAttribute("coverAll")]
        public bool coverAll
        {
            get
            {
                return this._config.CoverAllProcesses;
            }
            set
            {
                this._config.CoverAllProcesses = value;
            }
        }

        [TaskAttribute("coverChildProcess")]
        public string coverChildProcess
        {
            get
            {
                return this._config.CoverProcessNamesFilter;
            }
            set
            {
                this._config.CoverProcessNamesFilter = value;
            }
        }

        [TaskAttribute("excludeAssemblies")]
        public string excludeAssemblies
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeAssemblies);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeAssemblies = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("excludeAttributes")]
        public string excludeAttributes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeAttributes);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeAttributes = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("excludedAssemblies")]
        public string ExcludedAssemblies
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeAssemblies);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeAssemblies = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("excludedAssemblies", "excludeAssemblies");
            }
        }

        [TaskAttribute("excludeFiles")]
        public string excludeFiles
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeFilePaths);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeFilePaths = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("excludeMethods")]
        public string excludeMethods
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeMethods);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeMethods = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("excludeTypes")]
        public string excludeTypes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeTypeNames);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeTypeNames = (List<string>) attribute.ConvertBack(value);
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("program", Required=false)]
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

        [TaskAttribute("expertMode")]
        public bool ExpertMode
        {
            get
            {
                return this._config.DontModifySettingsForUsability;
            }
            set
            {
                this._config.DontModifySettingsForUsability = value;
                this._LogDeprecatedPropertyMessage("literalMode", "expertMode");
            }
        }

        [TaskAttribute("fileExclusionsPatterns")]
        public string FileExclusionPatterns
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeFilePaths);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeFilePaths = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("fileExclusionPatterns", "excludeFiles");
            }
        }

        [TaskAttribute("htmlReportDir")]
        public string htmlReportDir
        {
            get
            {
                return this._config.HtmlReportDir;
            }
            set
            {
                this._config.HtmlReportDir = value;
            }
        }

        [TaskAttribute("includeAssemblies")]
        public string includeAssemblies
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeAssemblies);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeAssemblies = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("includeAttributes")]
        public string includeAttributes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeAttributes);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeAttributes = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("includeAutoGenCode")]
        public bool includeAutoGenCode
        {
            get
            {
                return this._config.IncludeCompilerGeneratedCode;
            }
            set
            {
                this._config.IncludeCompilerGeneratedCode = value;
            }
        }

        [TaskAttribute("includedAttributes")]
        public string IncludedAttributes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeAttributes);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeAttributes = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("includedAttributes", "includeAttributes");
            }
        }

        [TaskAttribute("includedFiles")]
        public string IncludedFiles
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeFilePaths);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeFilePaths = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("includedFiles", "includeFiles");
            }
        }

        [TaskAttribute("includedTypes")]
        public string IncludedTypes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeTypeNames);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeTypeNames = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("includedTypes", "includeTypes");
            }
        }

        [TaskAttribute("includeFiles")]
        public string includeFiles
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeFilePaths);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeFilePaths = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("includeTypes")]
        public string includeTypes
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.IncludeTypeNames);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.IncludeTypeNames = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("literal")]
        public bool Literal
        {
            get
            {
                return this._config.DontModifySettingsForUsability;
            }
            set
            {
                this._config.DontModifySettingsForUsability = value;
                this._LogDeprecatedPropertyMessage("literal", "expertMode");
            }
        }

        [TaskAttribute("literalMode")]
        public bool literalMode
        {
            get
            {
                return this._config.DontModifySettingsForUsability;
            }
            set
            {
                this._config.DontModifySettingsForUsability = value;
            }
        }

        [TaskAttribute("logFile")]
        public string logFile
        {
            get
            {
                return this._config.LogFile;
            }
            set
            {
                this._config.LogFile = value;
            }
        }

        [TaskAttribute("logLevel")]
        public string logLevel
        {
            get
            {
                ConvertEnumValueAttribute attribute = new ConvertEnumValueAttribute(typeof(NCover.Interfaces.Enumerations.LogLevel));
                return attribute.ConvertFrom(this._config.LoggingLevel);
            }
            set
            {
                ConvertEnumValueAttribute attribute = new ConvertEnumValueAttribute(typeof(NCover.Interfaces.Enumerations.LogLevel));
                this._config.LoggingLevel = (NCover.Interfaces.Enumerations.LogLevel) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("methodExclusionsPatterns")]
        public string MethodExclusionPatterns
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeMethods);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeMethods = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("methodExclusionPatterns", "excludeMethods");
            }
        }

        [TaskAttribute("onlyAssembliesWithSource")]
        public bool onlyAssembliesWithSource
        {
            get
            {
                return this._config.OnlyAssembliesWithSource;
            }
            set
            {
                this._config.OnlyAssembliesWithSource = value;
            }
        }

        [TaskAttribute("profiledProcessModule")]
        public string ProfiledProcessModule
        {
            get
            {
                return this._config.CoverProcessNamesFilter;
            }
            set
            {
                this._config.CoverProcessNamesFilter = value;
                this._LogDeprecatedPropertyMessage("profiledProcessModule", "coverChildProcess");
            }
        }

        [TaskAttribute("profileIIS")]
        public bool ProfileIIS
        {
            get
            {
                return (this._config.CoverServiceName == "W3SVC");
            }
            set
            {
                if (value)
                {
                    this._config.CoverServiceName = "W3SVC";
                }
                else if (this.ProfileIIS)
                {
                    this._config.CoverServiceName = null;
                }
            }
        }

        [TaskAttribute("profileService")]
        public string profileService
        {
            get
            {
                return this._config.CoverServiceName;
            }
            set
            {
                this._config.CoverServiceName = value;
            }
        }

        public override string ProgramArguments
        {
            get
            {
                return this._programArguments;
            }
        }

        [TaskAttribute("projectName")]
        public string projectName
        {
            get
            {
                return this._config.ProjectName;
            }
            set
            {
                this._config.ProjectName = value;
            }
        }

        [TaskAttribute("registerProfiler")]
        public bool registerProfiler
        {
            get
            {
                return this._config.RegisterForUser;
            }
            set
            {
                this._config.RegisterForUser = value;
            }
        }

        [TaskAttribute("serviceArguments")]
        public string serviceArguments
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.CoverServiceArguments);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.CoverServiceArguments = (List<string>) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("serviceTimeout")]
        public int serviceTimeout
        {
            get
            {
                return this._config.WaitForAppExit;
            }
            set
            {
                this._config.WaitForAppExit = value;
            }
        }

        [TaskAttribute("settingsFile")]
        public string SettingsFile { get; set; }

        [TaskAttribute("setupForQuickTestPro")]
        public bool setupForQuickTestPro
        {
            get
            {
                return this._config.SetupGlobalEnvironment;
            }
            set
            {
                this._config.SetupGlobalEnvironment = value;
                this._LogDeprecatedPropertyMessage("setupForQuickTestPro", "setupGlobalEnvironment");
            }
        }

        [TaskAttribute("setupGlobalEnvironment")]
        public bool setupGlobalEnvironment
        {
            get
            {
                return this._config.SetupGlobalEnvironment;
            }
            set
            {
                this._config.SetupGlobalEnvironment = value;
            }
        }

        [TaskAttribute("symbolSearchLocations")]
        public string symbolSearchLocations
        {
            get
            {
                ConvertSymbolSearchPolicyEnumAttribute attribute = new ConvertSymbolSearchPolicyEnumAttribute(typeof(NCover.Interfaces.Enumerations.SymbolSearchPolicy));
                return attribute.ConvertFrom(this._config.SymbolSearchPolicy);
            }
            set
            {
                ConvertSymbolSearchPolicyEnumAttribute attribute = new ConvertSymbolSearchPolicyEnumAttribute(typeof(NCover.Interfaces.Enumerations.SymbolSearchPolicy));
                this._config.SymbolSearchPolicy = (NCover.Interfaces.Enumerations.SymbolSearchPolicy) attribute.ConvertBack(value);
            }
        }

        [TaskAttribute("symbolSearchPolicy")]
        public int SymbolSearchPolicy
        {
            get
            {
                return (int) this._config.SymbolSearchPolicy;
            }
            set
            {
                this._config.SymbolSearchPolicy = (NCover.Interfaces.Enumerations.SymbolSearchPolicy) value;
                this._LogDeprecatedPropertyMessage("symbolSearchPolicy", "symbolSearchLocations");
            }
        }

        [TaskAttribute("testRunnerArgs")]
        public string testRunnerArgs
        {
            get
            {
                return this._config.CommandLineArgs;
            }
            set
            {
                this._config.CommandLineArgs = value;
            }
        }

        [TaskAttribute("testRunnerExe")]
        public string testRunnerExe
        {
            get
            {
                return this._config.CommandLineExe;
            }
            set
            {
                this._config.CommandLineExe = value;
            }
        }

        [TaskAttribute("typeExclusionPatterns")]
        public string TypeExclusionPatterns
        {
            get
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                return attribute.ConvertFrom(this._config.ExcludeTypeNames);
            }
            set
            {
                ConvertStringArrayAttribute attribute = new ConvertStringArrayAttribute();
                this._config.ExcludeTypeNames = (List<string>) attribute.ConvertBack(value);
                this._LogDeprecatedPropertyMessage("typeExclusionPatterns", "excludeTypes");
            }
        }

        [TaskAttribute("waitForAppLoad")]
        public int waitForAppLoad
        {
            get
            {
                return this._config.WaitForAppLoad;
            }
            set
            {
                this._config.WaitForAppLoad = value;
            }
        }

        [TaskAttribute("workingDirectory")]
        public string workingDirectory
        {
            get
            {
                return this._config.AppWorkingDirectory;
            }
            set
            {
                this._config.AppWorkingDirectory = value;
            }
        }
    }
}
