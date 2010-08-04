namespace NCover.Framework.Configuration
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Collections.Generic;

    public interface ICoverageConfig : IConfig, ICloneable
    {
        string AppWorkingDirectory { get; set; }

        bool BreakWhenProfilerLoaded { get; set; }

        string BuildId { get; set; }

        string CheckFileSystemPermissions { get; set; }

        string CommandLineArgs { get; set; }

        string CommandLineExe { get; set; }

        string CoverageFile { get; set; }

        CoverageType CoverageMetrics { get; set; }

        string CoverageTrend { get; set; }

        bool CoverAllProcesses { get; set; }

        string CoverProcessNamesFilter { get; set; }

        List<string> CoverServiceArguments { get; set; }

        string CoverServiceName { get; set; }

        bool DisableILReplacement { get; set; }

        bool DontModifySettingsForUsability { get; set; }

        bool EnumerateDrivePermissions { get; set; }

        List<string> ExcludeAssemblies { get; set; }

        List<string> ExcludeAttributes { get; set; }

        List<string> ExcludeFilePaths { get; set; }

        List<string> ExcludeMethods { get; set; }

        List<string> ExcludeTypeNames { get; set; }

        bool HasCommandLineExe { get; }

        bool HasCoverageFileSpecified { get; }

        bool HasCoverServiceArguments { get; }

        bool HasCoverServiceName { get; }

        bool HasExcludedAssemblies { get; }

        bool HasExcludedAttributes { get; }

        bool HasExcludedFilePaths { get; }

        bool HasExcludedMethodNames { get; }

        bool HasExcludedTypeNames { get; }

        bool HasHtmlReportDirSpecified { get; }

        bool HasIncludedAssemblies { get; }

        bool HasIncludedAttributes { get; }

        bool HasIncludedFilePaths { get; }

        bool HasIncludedTypeNames { get; }

        bool HasLogFileSpecified { get; }

        bool HasTrendSpecified { get; }

        string HtmlReportDir { get; set; }

        List<string> IncludeAssemblies { get; set; }

        List<string> IncludeAttributes { get; set; }

        bool IncludeCompilerGeneratedCode { get; set; }

        List<string> IncludeFilePaths { get; set; }

        List<string> IncludeTypeNames { get; set; }

        bool IsServiceIIS { get; }

        string LogFile { get; set; }

        NCover.Interfaces.Enumerations.LogLevel LoggingLevel { get; set; }

        LogType LoggingType { get; set; }

        MemoryDumpLevel MemoryDump { get; set; }

        bool OnlyAssembliesWithSource { get; set; }

        string ProjectName { get; set; }

        bool RegisterForUser { get; set; }

        bool SetupGlobalEnvironment { get; set; }

        NCover.Interfaces.Enumerations.SymbolSearchPolicy SymbolSearchPolicy { get; set; }

        int WaitForAppExit { get; set; }

        int WaitForAppLoad { get; set; }
    }
}
