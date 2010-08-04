namespace NCover.Framework
{
    using NCover.Interfaces.Enumerations;
    using System;

    public static class CoverageDefaults
    {
        public const string ConsoleFileExt = ".settings";
        public const string CoverageFile = "coverage.nccov";
        public static string[] CoverageFileExt = new string[] { "nccov", "xml" };
        public const CoverageType CoverageMetrics = (CoverageType.MethodVisits | CoverageType.CyclomaticComplexity | CoverageType.Branch | CoverageType.Symbol);
        public const CoverageType CoverageMetricsForClassic = CoverageType.Symbol;
        public const NCover.Interfaces.Enumerations.LogLevel DefaultLogLevel = NCover.Interfaces.Enumerations.LogLevel.None;
        public const LogType DefaultLogType = LogType.None;
        public const SymbolSearchPolicy DefaultSymbolSearchPolicy = (SymbolSearchPolicy.ExecutingDir | SymbolSearchPolicy.BuildPath | SymbolSearchPolicy.SymbolServer | SymbolSearchPolicy.Registry);
        public const bool IncludeGeneratedCode = false;
        public const string LogFile = "coverage.log";
        public static string[] ProjectFileExt = new string[] { "ncproj", "ncover" };
        public const string SettingsFile = "NCoverConsole.settings";
        public const string TrendFile = "coverage.trend";
        public static string[] TrendFileExt = new string[] { "nctrend", "trend" };
        public const int WaitForAppExit = 0;
        public const int WaitForAppLoad = 1;
    }
}
