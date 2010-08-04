namespace NCover.Framework.Configuration
{
    using NCover.Framework;
    using NCover.Framework.Scripting;
    using NCover.Interfaces;
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, KeyValue("FlagPageTitle", "nant", "NAnt Options"), KeyValue("FlagPageTitle", "msbuild", "MSBuild Options"), KeyValue("AppName", "NCover.Console"), KeyValue("RootPath", "ncover-console"), XmlRoot("CoverageConfig", Namespace=""), GroupKey(1, GroupName="Services", RouteKey="services"), GroupKey(4, GroupName="Program Settings", RouteKey="program-settings"), KeyValue("FlagPageTitle", "configuration", "NCover Settings"), KeyValue("AppDescription", "NCover.Console provides numerous features for profiling applications, services, and IIS (ASP.NET) applications."), KeyValue("FlagPageTitle", "command-line", "Command Line Flags"), GroupKey(2, GroupName="Output", RouteKey="output"), GroupKey(3, GroupName="Profiling Options", RouteKey="profiling-options")]
    public class CoverageConfig : AbstractConfigFile, IXmlSerializable, ICoverageConfig, IConfig, ICloneable
    {
        public CoverageConfig()
        {
            base.FileVersion = "3.3.4";
            this.CoverageFile = "coverage.nccov";
            this.CoverageMetrics = CoverageType.MethodVisits | CoverageType.CyclomaticComplexity | CoverageType.Branch | CoverageType.Symbol;
            this.LogFile = "coverage.log";
            this.LoggingLevel = NCover.Interfaces.Enumerations.LogLevel.None;
            this.SymbolSearchPolicy = NCover.Interfaces.Enumerations.SymbolSearchPolicy.ExecutingDir | NCover.Interfaces.Enumerations.SymbolSearchPolicy.BuildPath | NCover.Interfaces.Enumerations.SymbolSearchPolicy.SymbolServer | NCover.Interfaces.Enumerations.SymbolSearchPolicy.Registry;
            this.ProjectName = "New Project";
            this.WaitForAppExit = 0;
            this.WaitForAppLoad = 1;
        }

        private static bool HasValidValues(List<string> values)
        {
            if (!values.NotEmpty<string>())
            {
                return false;
            }
            return values.Any<string>(str => !string.IsNullOrEmpty(str));
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string localName = reader.LocalName;
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "version")
                {
                    string text2 = reader.Value;
                }
            }
            reader.ReadStartElement();
            while (XmlSerializeHelper.MoreContent(reader, localName))
            {
                switch (reader.LocalName)
                {
                    case "WorkingDirectory":
                    {
                        this.AppWorkingDirectory = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "BuildId":
                    {
                        this.BuildId = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CommandLineArgs":
                    {
                        this.CommandLineArgs = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CommandLineExe":
                    {
                        this.CommandLineExe = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CoverageTrend":
                    {
                        this.CoverageTrend = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CoverageMetrics":
                    {
                        this.CoverageMetrics = XmlSerializeHelper.ReadEnumElement<CoverageType>(reader);
                        continue;
                    }
                    case "CoverageFile":
                    {
                        this.CoverageFile = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CoverAll":
                    {
                        this.CoverAllProcesses = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "CoverChildProcess":
                    {
                        this.CoverProcessNamesFilter = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "CoverServiceArguments":
                    {
                        this.CoverServiceArguments = XmlSerializeHelper.ReadArrayOfStrings(reader, "Argument").ToList<string>();
                        continue;
                    }
                    case "CoverServiceName":
                    {
                        this.CoverServiceName = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "ExpertMode":
                    case "LiteralMode":
                    {
                        this.DontModifySettingsForUsability = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "ExcludeAssemblies":
                    {
                        this.ExcludeAssemblies = XmlSerializeHelper.ReadArrayOfStrings(reader, "Assembly").ToList<string>();
                        continue;
                    }
                    case "ExcludeAttributes":
                    {
                        this.ExcludeAttributes = XmlSerializeHelper.ReadArrayOfStrings(reader, "Attribute").ToList<string>();
                        continue;
                    }
                    case "ExcludeTypes":
                    {
                        this.ExcludeTypeNames = XmlSerializeHelper.ReadArrayOfStrings(reader, "TypeName").ToList<string>();
                        continue;
                    }
                    case "ExcludeFiles":
                    {
                        this.ExcludeFilePaths = XmlSerializeHelper.ReadArrayOfStrings(reader, "SourceFile").ToList<string>();
                        continue;
                    }
                    case "ExcludeMethods":
                    {
                        this.ExcludeMethods = XmlSerializeHelper.ReadArrayOfStrings(reader, "MethodName").ToList<string>();
                        continue;
                    }
                    case "HtmlReportDir":
                    {
                        this.HtmlReportDir = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "IncludeAssemblies":
                    {
                        this.IncludeAssemblies = XmlSerializeHelper.ReadArrayOfStrings(reader, "Assembly").ToList<string>();
                        continue;
                    }
                    case "IncludeAttributes":
                    {
                        this.IncludeAttributes = XmlSerializeHelper.ReadArrayOfStrings(reader, "Attribute").ToList<string>();
                        continue;
                    }
                    case "IncludeAutoGenCode":
                    {
                        this.IncludeCompilerGeneratedCode = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "IncludeTypeNames":
                    case "IncludeTypes":
                    {
                        this.IncludeTypeNames = XmlSerializeHelper.ReadArrayOfStrings(reader, "TypeName").ToList<string>();
                        continue;
                    }
                    case "IncludeFilePaths":
                    case "IncludeFiles":
                    {
                        this.IncludeFilePaths = XmlSerializeHelper.ReadArrayOfStrings(reader, "SourceFile").ToList<string>();
                        continue;
                    }
                    case "LogFile":
                    {
                        this.LogFile = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "LoggingLevel":
                    {
                        this.LoggingLevel = XmlSerializeHelper.ReadEnumElement<NCover.Interfaces.Enumerations.LogLevel>(reader);
                        continue;
                    }
                    case "OnlyAssembliesWithSource":
                    {
                        this.OnlyAssembliesWithSource = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "ProjectName":
                    {
                        this.ProjectName = XmlSerializeHelper.ReadStringElement(reader);
                        continue;
                    }
                    case "RegisterForUser":
                    {
                        this.RegisterForUser = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "SetupForQuickTestPro":
                    case "SetupGlobalEnvironment":
                    {
                        this.SetupGlobalEnvironment = XmlSerializeHelper.ReadBooleanElement(reader);
                        continue;
                    }
                    case "SymbolSearchLocations":
                    {
                        this.SymbolSearchPolicy = XmlSerializeHelper.ReadEnumElement<NCover.Interfaces.Enumerations.SymbolSearchPolicy>(reader);
                        continue;
                    }
                    case "WaitForAppExit":
                    {
                        this.WaitForAppExit = XmlSerializeHelper.ReadIntElement(reader);
                        continue;
                    }
                    case "WaitForAppLoad":
                    {
                        this.WaitForAppLoad = XmlSerializeHelper.ReadIntElement(reader);
                        continue;
                    }
                }
                throw new InvalidOperationException("Unexpected element: '" + reader.LocalName + "' while deserializing " + base.GetType().Name);
            }
            if (!reader.EOF)
            {
                reader.ReadEndElement();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("version", base.FileVersion);
            writer.WriteElementString("WorkingDirectory", this.AppWorkingDirectory);
            writer.WriteElementString("BuildId", this.BuildId);
            writer.WriteElementString("CommandLineArgs", this.CommandLineArgs);
            writer.WriteElementString("CommandLineExe", this.CommandLineExe);
            writer.WriteElementString("CoverageTrend", this.CoverageTrend);
            writer.WriteElementString("CoverageMetrics", this.CoverageMetrics.ToString());
            writer.WriteElementString("CoverageFile", this.CoverageFile);
            writer.WriteElementString("CoverAll", this.CoverAllProcesses);
            writer.WriteElementString("CoverChildProcess", this.CoverProcessNamesFilter);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "CoverServiceArguments", "Argument", this.CoverServiceArguments);
            writer.WriteElementString("CoverServiceName", this.CoverServiceName);
            writer.WriteElementString("LiteralMode", this.DontModifySettingsForUsability);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "ExcludeAssemblies", "Assembly", this.ExcludeAssemblies);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "ExcludeAttributes", "Attribute", this.ExcludeAttributes);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "ExcludeTypes", "TypeName", this.ExcludeTypeNames);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "ExcludeFiles", "SourceFile", this.ExcludeFilePaths);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "ExcludeMethods", "MethodName", this.ExcludeMethods);
            writer.WriteElementString("HtmlReportDir", this.HtmlReportDir);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "IncludeAssemblies", "Assembly", this.IncludeAssemblies);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "IncludeAttributes", "Attribute", this.IncludeAttributes);
            writer.WriteElementString("IncludeAutoGenCode", this.IncludeCompilerGeneratedCode);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "IncludeTypes", "TypeName", this.IncludeTypeNames);
            XmlSerializeHelper.WriteArrayOfStrings(writer, "IncludeFiles", "SourceFile", this.IncludeFilePaths);
            writer.WriteElementString("LogFile", this.LogFile);
            writer.WriteElementString("LoggingLevel", this.LoggingLevel.ToString());
            writer.WriteElementString("OnlyAssembliesWithSource", this.OnlyAssembliesWithSource);
            writer.WriteElementString("ProjectName", this.ProjectName);
            writer.WriteElementString("RegisterForUser", this.RegisterForUser);
            writer.WriteElementString("SetupGlobalEnvironment", this.SetupGlobalEnvironment);
            writer.WriteElementString("SymbolSearchLocations", this.SymbolSearchPolicy.ToString());
            writer.WriteElementString("WaitForAppExit", this.WaitForAppExit);
            writer.WriteElementString("WaitForAppLoad", this.WaitForAppLoad);
        }

        [ScriptArgument("//w", ScriptOutputType.CommandLine), XmlElement("WorkingDirectory"), ScriptArgument("WorkingDirectory", ScriptOutputType.MSBuildNCover), CommandLineSwitch("//working-directory"), IncludeStringValue, GroupKey(4), ScriptArgument("workingDirectory", ScriptOutputType.NAntNCover)]
        public string AppWorkingDirectory { get; set; }

        [XmlIgnore]
        public bool BreakWhenProfilerLoaded { get; set; }

        [IncludeStringValue, ScriptArgument("buildId", ScriptOutputType.NAntNCover), ScriptArgument("BuildId", ScriptOutputType.MSBuildNCover), GroupKey(4), CommandLineSwitch("//build-id"), ScriptArgument("//bi", ScriptOutputType.CommandLine)]
        public string BuildId { get; set; }

        [XmlIgnore]
        public string CheckFileSystemPermissions { get; set; }

        [XmlElement("CommandLineArgs"), IncludeStringValue, ReplacesFeature(ScriptOutputType.MSBuildNCover, "CommandLineArgs"), ReplacesFeature(ScriptOutputType.NAntNCover, "commandLineArgs"), ForcePosition(2), ScriptArgument("", ScriptOutputType.CommandLine), ScriptArgument("TestRunnerArgs", ScriptOutputType.MSBuildNCover), ScriptArgument("testRunnerArgs", ScriptOutputType.NAntNCover)]
        public string CommandLineArgs { get; set; }

        [XmlElement("CommandLineExe", IsNullable=false), ScriptArgument("", ScriptOutputType.CommandLine), ReplacesFeature(ScriptOutputType.NAntNCover, "commandLineExe"), ForcePosition(1), ScriptArgument("testRunnerExe", ScriptOutputType.NAntNCover), ReplacesFeature(ScriptOutputType.MSBuildNCover, "CommandLineExe"), ScriptArgument("TestRunnerExe", ScriptOutputType.MSBuildNCover), IncludeStringValue]
        public string CommandLineExe { get; set; }

        [CommandLineSwitch("//xml"), XmlElement("CoverageFile"), GroupKey(2), ScriptArgument("CoverageFile", ScriptOutputType.MSBuildNCover), IncludeWhenNotDefaultValue("coverage.nccov"), ScriptArgument("//x", ScriptOutputType.CommandLine), ScriptArgument("coverageFile", ScriptOutputType.NAntNCover)]
        public string CoverageFile { get; set; }

        [IncludeWhenNotDefaultValue(0x1b), XmlElement("CoverageMetrics"), GroupKey(3), CommandLineSwitch("//coverage-type"), ScriptArgument("//ct", ScriptOutputType.CommandLine), ScriptArgument("CoverageMetrics", ScriptOutputType.MSBuildNCover), ScriptArgument("coverageMetrics", ScriptOutputType.NAntNCover), ConvertCoverageTypeEnum(typeof(CoverageType))]
        public CoverageType CoverageMetrics { get; set; }

        [ScriptArgument("AppendTrendTo", ScriptOutputType.MSBuildNCover), GroupKey(4), ScriptArgument("appendTrendTo", ScriptOutputType.NAntNCover), IncludeStringValue, ForNCoverFeature(NCoverFeature.CoverageTrends), CommandLineSwitch("//append-trend"), ScriptArgument("//at", ScriptOutputType.CommandLine)]
        public string CoverageTrend { get; set; }

        [XmlElement("CoverAll"), GroupKey(3), CommandLineSwitch("//coverall"), ScriptArgument("//coverall", ScriptOutputType.CommandLine), ScriptArgument("CoverAll", ScriptOutputType.MSBuildNCover), ScriptArgument("coverAll", ScriptOutputType.NAntNCover), IncludeWhenNotDefaultValue(false)]
        public bool CoverAllProcesses { get; set; }

        [ScriptArgument("CoverChildProcess", ScriptOutputType.MSBuildNCover), XmlElement("CoverChildProcess"), GroupKey(3), ReplacesFeature(ScriptOutputType.MSBuildNCover, "ProfiledProcessModule"), ReplacesFeature(ScriptOutputType.NAntNCover, "profiledProcessModule"), ScriptArgument("//pn", ScriptOutputType.CommandLine), ScriptArgument("coverChildProcess", ScriptOutputType.NAntNCover), IncludeStringValue]
        public string CoverProcessNamesFilter { get; set; }

        [IncludeStringArrayValue, XmlArray("CoverServiceArguments"), XmlArrayItem("Argument"), GroupKey(1), ForNCoverFeature(NCoverFeature.ProfileService), CommandLineSwitch("//service-argument"), ScriptArgument("//sa", ScriptOutputType.CommandLine), ScriptArgument("ServiceArguments", ScriptOutputType.MSBuildNCover), ScriptArgument("serviceArguments", ScriptOutputType.NAntNCover), ConvertStringArray]
        public List<string> CoverServiceArguments { get; set; }

        [CommandLineSwitch("//service"), XmlElement("CoverServiceName"), GroupKey(1), ForNCoverFeature(NCoverFeature.ProfileService), ScriptArgument("profileService", ScriptOutputType.NAntNCover), ScriptArgument("//svc", ScriptOutputType.CommandLine), IncludeStringValue, ScriptArgument("ProfileService", ScriptOutputType.MSBuildNCover)]
        public string CoverServiceName { get; set; }

        [XmlIgnore]
        public bool DisableILReplacement { get; set; }

        [IncludeBool, ScriptArgument("//literal", ScriptOutputType.CommandLine, UseValue=false), ScriptArgument("literalMode", ScriptOutputType.NAntNCover), GroupKey(4), XmlElement("LiteralMode"), ScriptArgument("LiteralMode", ScriptOutputType.MSBuildNCover)]
        public bool DontModifySettingsForUsability { get; set; }

        [XmlIgnore]
        public bool EnumerateDrivePermissions { get; set; }

        [XmlArrayItem("Assembly"), IncludeStringArrayValue, XmlArray("ExcludeAssemblies"), GroupKey(3), ForNCoverFeature(NCoverFeature.ExcludeAssemblies), CommandLineSwitch("//exclude-assemblies"), ScriptArgument("//eas", ScriptOutputType.CommandLine), ScriptArgument("ExcludeAssemblies", ScriptOutputType.MSBuildNCover), ScriptArgument("excludeAssemblies", ScriptOutputType.NAntNCover), ConvertStringArray]
        public List<string> ExcludeAssemblies { get; set; }

        [XmlArray("ExcludeAttributes"), CommandLineSwitch("//exclude-attributes"), XmlArrayItem("Attribute"), GroupKey(3), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.ExcludeAttributes), ScriptArgument("ExcludeAttributes", ScriptOutputType.MSBuildNCover), ScriptArgument("//ea", ScriptOutputType.CommandLine), ScriptArgument("excludeAttributes", ScriptOutputType.NAntNCover), ConvertStringArray, IncludeStringArrayValue]
        public List<string> ExcludeAttributes { get; set; }

        [XmlArray("ExcludeFiles"), CommandLineSwitch("//exclude-files"), ScriptArgument("//ef", ScriptOutputType.CommandLine), IncludeStringArrayValue, ConvertStringArray, XmlArrayItem("SourceFile"), GroupKey(3), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.ExcludeSourceFiles), ReplacesFeature(ScriptOutputType.MSBuildNCover, "FileExclusionsPatterns"), ReplacesFeature(ScriptOutputType.NAntNCover, "fileExclusionsPatterns"), ScriptArgument("ExcludeFiles", ScriptOutputType.MSBuildNCover), ScriptArgument("excludeFiles", ScriptOutputType.NAntNCover)]
        public List<string> ExcludeFilePaths { get; set; }

        [IncludeStringArrayValue, XmlArray("ExcludeMethods"), XmlArrayItem("MethodName"), GroupKey(3), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.ExcludeMethodNames), ReplacesFeature(ScriptOutputType.MSBuildNCover, "MethodExclusionsPatterns"), ReplacesFeature(ScriptOutputType.NAntNCover, "methodExclusionsPatterns"), CommandLineSwitch("//exclude-methods"), ScriptArgument("//em", ScriptOutputType.CommandLine), ScriptArgument("ExcludeMethods", ScriptOutputType.MSBuildNCover), ScriptArgument("excludeMethods", ScriptOutputType.NAntNCover), ConvertStringArray]
        public List<string> ExcludeMethods { get; set; }

        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.ExcludeTypes), IncludeStringArrayValue, XmlArray("ExcludeTypes"), XmlArrayItem("TypeName"), GroupKey(3), CommandLineSwitch("//exclude-types"), ScriptArgument("//et", ScriptOutputType.CommandLine), ReplacesFeature(ScriptOutputType.MSBuildNCover, "TypeExclusionsPatterns"), ReplacesFeature(ScriptOutputType.NAntNCover, "typeExclusionsPatterns"), ScriptArgument("ExcludeTypes", ScriptOutputType.MSBuildNCover), ScriptArgument("excludeTypes", ScriptOutputType.NAntNCover), ConvertStringArray]
        public List<string> ExcludeTypeNames { get; set; }

        [XmlIgnore]
        public bool HasCommandLineExe
        {
            get
            {
                return !string.IsNullOrEmpty(this.CommandLineExe);
            }
        }

        [XmlIgnore]
        public bool HasCoverageFileSpecified
        {
            get
            {
                return !string.IsNullOrEmpty(this.CoverageFile);
            }
        }

        [XmlIgnore]
        public bool HasCoverServiceArguments
        {
            get
            {
                return this.CoverServiceArguments.NotEmpty<string>();
            }
        }

        [XmlIgnore]
        public bool HasCoverServiceName
        {
            get
            {
                return !string.IsNullOrEmpty(this.CoverServiceName);
            }
        }

        [XmlIgnore]
        public bool HasExcludedAssemblies
        {
            get
            {
                return HasValidValues(this.ExcludeAssemblies);
            }
        }

        [XmlIgnore]
        public bool HasExcludedAttributes
        {
            get
            {
                return HasValidValues(this.ExcludeAttributes);
            }
        }

        [XmlIgnore]
        public bool HasExcludedFilePaths
        {
            get
            {
                return HasValidValues(this.ExcludeFilePaths);
            }
        }

        [XmlIgnore]
        public bool HasExcludedMethodNames
        {
            get
            {
                return HasValidValues(this.ExcludeMethods);
            }
        }

        [XmlIgnore]
        public bool HasExcludedTypeNames
        {
            get
            {
                return HasValidValues(this.ExcludeTypeNames);
            }
        }

        [XmlIgnore]
        public bool HasHtmlReportDirSpecified
        {
            get
            {
                return !string.IsNullOrEmpty(this.HtmlReportDir);
            }
        }

        [XmlIgnore]
        public bool HasIncludedAssemblies
        {
            get
            {
                return HasValidValues(this.IncludeAssemblies);
            }
        }

        [XmlIgnore]
        public bool HasIncludedAttributes
        {
            get
            {
                return HasValidValues(this.IncludeAttributes);
            }
        }

        [XmlIgnore]
        public bool HasIncludedFilePaths
        {
            get
            {
                return HasValidValues(this.IncludeFilePaths);
            }
        }

        [XmlIgnore]
        public bool HasIncludedTypeNames
        {
            get
            {
                return HasValidValues(this.IncludeTypeNames);
            }
        }

        [XmlIgnore]
        public bool HasLogFileSpecified
        {
            get
            {
                return !string.IsNullOrEmpty(this.LogFile);
            }
        }

        [XmlIgnore]
        public bool HasTrendSpecified
        {
            get
            {
                return !string.IsNullOrEmpty(this.CoverageTrend);
            }
        }

        [ScriptArgument("//h", ScriptOutputType.CommandLine), ScriptArgument("HtmlReportDir", ScriptOutputType.MSBuildNCover), ScriptArgument("htmlReportDir", ScriptOutputType.NAntNCover), XmlElement("HtmlReportDir"), IncludeStringValue, ReplacesFeature(ScriptOutputType.NAntNCover, "coverageHtmlDirectory"), CommandLineSwitch("//html"), ForNCoverFeature(NCoverFeature.ReportHTML), ReplacesFeature(ScriptOutputType.MSBuildNCover, "CoverageHtmlDirectory"), GroupKey(2)]
        public string HtmlReportDir { get; set; }

        [XmlArrayItem("Assembly"), IncludeStringArrayValue, XmlArray("IncludeAssemblies"), GroupKey(3), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.IncludeAssemblies), ReplacesFeature(ScriptOutputType.MSBuildNCover, "AssemblyList"), ReplacesFeature(ScriptOutputType.NAntNCover, "assemblyList"), CommandLineSwitch("//a"), CommandLineSwitch("//assemblies"), CommandLineSwitch("//include-assemblies"), ScriptArgument("//ias", ScriptOutputType.CommandLine), ScriptArgument("IncludeAssemblies", ScriptOutputType.MSBuildNCover), ConvertStringArray, ScriptArgument("includeAssemblies", ScriptOutputType.NAntNCover)]
        public List<string> IncludeAssemblies { get; set; }

        [ForNCoverFeature(NCoverFeature.IncludeAttributes), GroupKey(3), CommandLineSwitch("//include-attributes"), ScriptArgument("//ia", ScriptOutputType.CommandLine), ScriptArgument("IncludeAttributes", ScriptOutputType.MSBuildNCover), ScriptArgument("includeAttributes", ScriptOutputType.NAntNCover), XmlArrayItem("Attribute"), IncludeStringArrayValue, ConvertStringArray, XmlArray("IncludeAttributes")]
        public List<string> IncludeAttributes { get; set; }

        [IncludeBool, ScriptArgument("//na", ScriptOutputType.CommandLine, UseValue=false), GroupKey(3), ScriptArgument("IncludeAutoGenCode", ScriptOutputType.MSBuildNCover), ScriptArgument("includeAutoGenCode", ScriptOutputType.NAntNCover), XmlElement("IncludeAutoGenCode")]
        public bool IncludeCompilerGeneratedCode { get; set; }

        [ConvertStringArray, XmlArray("IncludeFiles"), IncludeStringArrayValue, XmlArrayItem("SourceFile"), GroupKey(3), ForNCoverFeature(NCoverFeature.IncludeSourceFiles), CommandLineSwitch("//include-files"), ScriptArgument("//if", ScriptOutputType.CommandLine), ScriptArgument("IncludeFiles", ScriptOutputType.MSBuildNCover), ScriptArgument("includeFiles", ScriptOutputType.NAntNCover)]
        public List<string> IncludeFilePaths { get; set; }

        [CommandLineSwitch("//include-types"), ConvertStringArray, ScriptArgument("//it", ScriptOutputType.CommandLine), IncludeStringArrayValue, XmlArray("IncludeTypes"), ScriptArgument("IncludeTypes", ScriptOutputType.MSBuildNCover), ScriptArgument("includeTypes", ScriptOutputType.NAntNCover), XmlArrayItem("TypeName"), GroupKey(3), ForNCoverFeature(NCoverFeature.IncludeTypes)]
        public List<string> IncludeTypeNames { get; set; }

        [XmlIgnore]
        public bool IsServiceIIS
        {
            get
            {
                return (this.HasCoverServiceName && this.CoverServiceName.IgnoreCaseEq("W3SVC"));
            }
        }

        [CommandLineSwitch("//log-file"), GroupKey(2), XmlElement("LogFile"), ScriptArgument("//l", ScriptOutputType.CommandLine), ScriptArgument("LogFile", ScriptOutputType.MSBuildNCover), ScriptArgument("logFile", ScriptOutputType.NAntNCover), IncludeWhenNotDefaultValue("coverage.log")]
        public string LogFile { get; set; }

        [CommandLineSwitch("//log-level"), ScriptArgument("//ll", ScriptOutputType.CommandLine), IncludeWhenNotDefaultValue(0), ScriptArgument("LogLevel", ScriptOutputType.MSBuildNCover), ConvertEnumValue(typeof(NCover.Interfaces.Enumerations.LogLevel)), GroupKey(2), ScriptArgument("logLevel", ScriptOutputType.NAntNCover)]
        public NCover.Interfaces.Enumerations.LogLevel LoggingLevel { get; set; }

        [XmlIgnore, GroupKey(2)]
        public LogType LoggingType { get; set; }

        [XmlIgnore]
        public MemoryDumpLevel MemoryDump { get; set; }

        [ScriptArgument("onlyAssembliesWithSource", ScriptOutputType.NAntNCover), XmlElement("OnlyAssembliesWithSource"), IncludeBool, ScriptArgument("//onlywithsource", ScriptOutputType.CommandLine, UseValue=false), GroupKey(3), ScriptArgument("OnlyAssembliesWithSource", ScriptOutputType.MSBuildNCover)]
        public bool OnlyAssembliesWithSource { get; set; }

        [IncludeStringValue, XmlElement("ProjectName"), ForNCoverFeature(NCoverFeature.ReportHTML), CommandLineSwitch("//project-name"), ScriptArgument("//p", ScriptOutputType.CommandLine), ScriptArgument("ProjectName", ScriptOutputType.MSBuildNCover), GroupKey(2), ScriptArgument("projectName", ScriptOutputType.NAntNCover)]
        public string ProjectName { get; set; }

        [ScriptArgument("RegisterProfiler", ScriptOutputType.MSBuildNCover), GroupKey(4), XmlElement("RegisterForUser"), IncludeBool, ScriptArgument("//reg", ScriptOutputType.CommandLine, UseValue=false), ScriptArgument("registerProfiler", ScriptOutputType.NAntNCover)]
        public bool RegisterForUser { get; set; }

        [XmlElement("SetupGlobalEnvironment"), IncludeBool, GroupKey(3), ScriptArgument("//setupglobalenvironment", ScriptOutputType.CommandLine, UseValue=false), ScriptArgument("SetupGlobalEnvironment", ScriptOutputType.MSBuildNCover), ScriptArgument("setupGlobalEnvironment", ScriptOutputType.NAntNCover)]
        public bool SetupGlobalEnvironment { get; set; }

        [ConvertSymbolSearchPolicyEnum(typeof(NCover.Interfaces.Enumerations.SymbolSearchPolicy)), IncludeWhenNotDefaultValue(15), ReplacesFeature(ScriptOutputType.MSBuildNCover, "SymbolSearchPolicy", "Now requires a string, not an integer. Acceptable values are \"Registry\", \"BuildPath\", \"ExecutingDir\", \"SymbolServer\", or any combination (comma delimited) thereof"), ReplacesFeature(ScriptOutputType.NAntNCover, "symbolSearchPolicy", "Now requires a string, not an integer. Acceptable values are \"Registry\", \"BuildPath\", \"ExecutingDir\", \"SymbolServer\", or any combination (comma delimited) thereof"), XmlElement("SymbolSearchLocations"), ScriptArgument("//ssp", ScriptOutputType.CommandLine), ScriptArgument("symbolSearchLocations", ScriptOutputType.NAntNCover), GroupKey(3), ScriptArgument("SymbolSearchLocations", ScriptOutputType.MSBuildNCover)]
        public NCover.Interfaces.Enumerations.SymbolSearchPolicy SymbolSearchPolicy { get; set; }

        [CommandLineSwitch("//service-timeout"), ScriptArgument("serviceTimeout", ScriptOutputType.NAntNCover), IncludeWhenNotDefaultValue(0), ScriptArgument("ServiceTimeout", ScriptOutputType.MSBuildNCover), ReplacesFeature(ScriptOutputType.NAntNCover, "", "Now interprets its parameter as the number of minutes to wait for the service to finish, not the number of seconds"), GroupKey(1), ForNCoverFeature(NCoverFeature.ProfileService), XmlElement("WaitForAppExit"), ScriptArgument("//st", ScriptOutputType.CommandLine), ReplacesFeature(ScriptOutputType.MSBuildNCover, "", "Now interprets its parameter as the number of minutes to wait for the service to finish, not the number of seconds")]
        public int WaitForAppExit { get; set; }

        [XmlElement("WaitForAppLoad"), IncludeWhenNotDefaultValue(1), CommandLineSwitch("//wait-for-app-load"), ScriptArgument("//wal", ScriptOutputType.CommandLine), ScriptArgument("waitForAppLoad", ScriptOutputType.NAntNCover), GroupKey(4), ScriptArgument("WaitForAppLoad", ScriptOutputType.MSBuildNCover)]
        public int WaitForAppLoad { get; set; }
    }
}
