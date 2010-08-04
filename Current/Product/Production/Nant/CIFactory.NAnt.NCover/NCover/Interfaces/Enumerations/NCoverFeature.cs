namespace NCover.Interfaces.Enumerations
{
    using System;

    [Flags]
    public enum NCoverFeature : long
    {
        AlwaysDisabled = 1L,
        AlwaysEnabled = 0L,
        CoverageBranch = 0x4000L,
        CoverageComplexity = 0x8000L,
        CoverageDecision = 0x2000L,
        CoverageForClassic = 4L,
        CoverageForComplete = 0xd004L,
        CoverageLine = 2L,
        CoverageMethodVisits = 0x1000L,
        CoverageSymbol = 4L,
        CoverageTrends = 0x100000L,
        ExcludeAssemblies = 0x200000L,
        ExcludeAttributes = 0x100L,
        ExcludeMethodNames = 0x800L,
        ExcludeSourceFiles = 0x400L,
        ExcludeTypes = 0x200L,
        IncludeAssemblies = 0x80L,
        IncludeAttributes = 0x400000L,
        IncludeMethodNames = 0x2000000L,
        IncludeSourceFiles = 0x1000000L,
        IncludeTypes = 0x800000L,
        MergeCoverageData = 0x10000000L,
        MinimumCoverageRules = 0x40000000L,
        NCEExclude = 0x8000000L,
        NCEInclude = 0x4000000L,
        NeedsAnyLicense = 0x200000000L,
        ProfileApplication = 8L,
        ProfileService = 0x40000L,
        ReportGroupBy = 0x400000000L,
        ReportHTML = 0x10000L,
        ReportSummary = 0x20L,
        ReportUncovered = 0x20000L,
        SatisfactoryCoverageRules = 0x80000000L,
        SourceFileReport = 0x20000000L,
        x64Support = 0x40L,
        x86Support = 0x10L
    }
}
