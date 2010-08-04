namespace NCover.Interfaces.Reporting
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Collections.Generic;

    public interface IReportOutputData
    {
        bool ParamExists(string key);
        string ParamValue(string key);

        ReportOutputFormats Format { get; set; }

        string OutputPath { get; }

        IEnumerable<INameValuePair> Params { get; }

        CoverageReportType ReportType { get; set; }
    }
}
