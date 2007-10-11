using System;
namespace TestCoverageRunner
{
    public interface IReporting
    {
        void GenerateReport();
        System.Collections.Specialized.StringCollection ProductionAssemblies { get; set; }
        string ProjectName { get; set; }
        System.IO.Stream ReportStream { get; set; }
        System.Collections.Specialized.StringCollection TestAssemblies { get; set; }
    }
}
