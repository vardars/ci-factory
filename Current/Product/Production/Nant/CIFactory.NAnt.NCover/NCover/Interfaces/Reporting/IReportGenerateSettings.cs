namespace NCover.Interfaces.Reporting
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Collections.Generic;

    public interface IReportGenerateSettings : IReportGenerateAndPrepareCommon
    {
        void AddReportOutput(IReportOutputData report);
        void AddReportOutput(CoverageReportType reportType, ReportOutputFormats format);
        void ClearReports();
        void RemoveAllReports(IReportOutputData report);

        ViewFilterStyle FilterStyle { get; set; }

        int NumOfTopUncoveredToReport { get; set; }

        string OutputPath { get; set; }

        string ProjectName { get; set; }

        CoverageFilterType ReportDataFilter { get; set; }

        IEnumerable<IReportOutputData> Reports { get; }

        bool ShowExcludedFooter { get; set; }

        ViewSortStyle SortStyle { get; set; }

        string WorkingDirectory { get; set; }

        string XsltOverridePath { get; set; }
    }
}
