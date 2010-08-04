namespace NCover.Interfaces.Enumerations
{
    using NCover.Interfaces;
    using System;

    public enum CoverageReportType : long
    {
        [ForNCoverFeature(NCoverFeature.CoverageTrends)]
        Diff = 0x17L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.ReportHTML)]
        FullCoverageReport = 1L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits | NCoverFeature.CoverageComplexity)]
        MethodCCModuleClassCoverageTop = 0x15L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits | NCoverFeature.CoverageComplexity)]
        MethodCCModuleClassFailedCoverageTop = 0x13L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        MethodModule = 5L,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits), ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage)]
        MethodModuleNamespace = 9L,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits), ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage)]
        MethodModuleNamespaceClass = 11L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        MethodModuleNamespaceClassMethod = 7L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageMethodVisits)]
        MethodSourceCode = 13L,
        [ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageMethodVisits), ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage)]
        MethodSourceCodeClass = 15L,
        [ReportStyleMapping(ViewReportStyle.FunctionCoveragePercentage), ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageMethodVisits)]
        MethodSourceCodeClassMethod = 0x11L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage)]
        None = 0L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.ReportSummary)]
        Summary = 2L,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageComplexity)]
        SymbolCCByGroup = 0x18L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.CoverageForClassic | NCoverFeature.CoverageComplexity)]
        SymbolCCModuleClassCoverageTop = 20L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.CoverageForClassic | NCoverFeature.CoverageComplexity)]
        SymbolCCModuleClassFailedCoverageTop = 0x12L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        SymbolModule = 4L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        SymbolModuleNamespace = 8L,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic), ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage)]
        SymbolModuleNamespaceClass = 10L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        SymbolModuleNamespaceClassMethod = 6L,
        [ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageForClassic), ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage)]
        SymbolSourceCode = 12L,
        [ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageForClassic), ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage)]
        SymbolSourceCodeClass = 14L,
        [ForNCoverFeature(NCoverFeature.SourceFileReport | NCoverFeature.CoverageForClassic), ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage)]
        SymbolSourceCodeClassMethod = 0x10L,
        [ForNCoverFeature(NCoverFeature.CoverageTrends)]
        Trends = 0x16L,
        [ReportStyleMapping(ViewReportStyle.SequencePointCoveragePercentage), ForNCoverFeature(NCoverFeature.ReportUncovered)]
        UncoveredCodeSections = 3L
    }
}
