namespace NCover.Interfaces.Reporting
{
    using NCover.Interfaces.Common;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.View;
    using System;
    using System.Collections.Generic;

    public interface IReportPrepareSettings : IReportGenerateAndPrepareCommon
    {
        void AddFailBuildThreshold(ICoverageThreshold threshold);
        void AddFilter(ICoverageViewFilter filter);
        void AddRebasedPath(ICoverageViewRebasedPath path);
        void AddSatisfactoryThreshold(ICoverageThreshold threshold);
        void ClearFailBuildThresholds();
        void ClearFilters();
        void ClearSatisfactoryThresholds();
        void RemoveFailBuildThreshold(ICoverageThreshold threshold);
        void RemoveFilter(ICoverageViewFilter filter);
        void RemoveSatisfactoryThreshold(ICoverageThreshold threshold);
        void ReplaceFailBuildThreshold(ICoverageThreshold threshold);
        void ReplaceSatisfactoryThreshold(ICoverageThreshold threshold);

        string BuildId { get; set; }

        bool ClearCoverageFilters { get; set; }

        List<string> CoverageDataFileNames { get; set; }

        string CoverageTrendPath { get; set; }

        IEnumerable<ICoverageThreshold> FailBuildThresholds { get; }

        IEnumerable<ICoverageViewFilter> Filters { get; }

        bool HasAppendTrends { get; }

        bool HasCoverageDataFileNames { get; }

        bool HasFailBuildThresholds { get; }

        bool HasFilters { get; }

        bool HasLoadTrends { get; }

        bool HasSatisfactoryThresholds { get; }

        bool IgnoreFailedBuilds { get; set; }

        string LoadTrendPath { get; set; }

        int MaxFailedToShow { get; set; }

        string MergedCoverageDataFileName { get; set; }

        NCover.Interfaces.Enumerations.MergeFilterMode MergeFilterMode { get; set; }

        IEnumerable<ICoverageViewRebasedPath> RebasedPaths { get; }

        bool ReportInvalidFiles { get; set; }

        bool UseMinCoverageForSatCoverage { get; set; }
    }
}
