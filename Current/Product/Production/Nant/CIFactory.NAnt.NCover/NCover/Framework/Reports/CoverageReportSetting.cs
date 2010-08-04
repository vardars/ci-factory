namespace NCover.Framework.Reports
{
    using NCover.Framework;
    using NCover.Framework.Scripting;
    using NCover.Framework.Views;
    using NCover.Interfaces;
    using NCover.Interfaces.Common;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Extensions;
    using NCover.Interfaces.Reporting;
    using NCover.Interfaces.View;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [KeyValue("FlagPageTitle", "msbuild", "MSBuild Options"), XmlRoot(ElementName="CoverageReportSetting", Namespace=""), GroupKey(1, GroupName="Coverage Reporting", RouteKey="coverage-options"), KeyValue("FlagPageTitle", "configuration", "NCover Settings"), GroupKey(2, GroupName="Merging Coverage Data", RouteKey="merging"), KeyValue("FlagPageTitle", "command-line", "Command Line Flags"), KeyValue("FlagPageTitle", "nant", "NAnt Options"), GroupKey(3, GroupName="Build Control", RouteKey="build-server"), KeyValue("RootPath", "ncover-reporting"), KeyValue("AppName", "NCover.Reporting"), KeyValue("AppDescription", "NCover.Reporting provides numerous features for reporting on your coverage data.")]
    public class CoverageReportSetting : ICoverageReportSettings, IReportPrepareSettings, IReportGenerateSettings, IReportGenerateAndPrepareCommon
    {
        protected List<string> coverageDataFileNamesField;
        protected List<CoverageThreshold> failBuildThresholdsField;
        protected List<CoverageViewFilterBase> filtersField;
        public const string ReportingApplicationName = "NCover.Reporting.exe";
        protected List<CoverageThreshold> satisfactoryThresholdsField;

        public CoverageReportSetting()
        {
            this.NumOfTopUncoveredToReport = 10;
            this.CoverageDataFileNames = new List<string>();
            this.MaxFailedToShow = 50;
        }

        public void AddBatch(IEnumerable<ICoverageViewFilter> filters)
        {
            foreach (ICoverageViewFilter filter in filters)
            {
                this.AddFilter(filter);
            }
        }

        public void AddBatch(IEnumerable<IReportOutputData> reports)
        {
            foreach (IReportOutputData data in reports)
            {
                this.AddReportOutput(data);
            }
        }

        public void AddBatch(IEnumerable<ICoverageViewRebasedPath> paths)
        {
            foreach (ICoverageViewRebasedPath path in paths)
            {
                this.AddRebasedPath(path);
            }
        }

        public void AddBatchFailBuild(IEnumerable<ICoverageThreshold> thresholds)
        {
            foreach (ICoverageThreshold threshold in thresholds)
            {
                this.AddFailBuildThreshold(threshold);
            }
        }

        public void AddBatchSatisfactory(IEnumerable<ICoverageThreshold> thresholds)
        {
            foreach (ICoverageThreshold threshold in thresholds)
            {
                this.AddSatisfactoryThreshold(threshold);
            }
        }

        public virtual void AddFailBuildThreshold(ICoverageThreshold threshold)
        {
            this.AddThreshold(ref this.failBuildThresholdsField, threshold);
        }

        public virtual void AddFilter(ICoverageViewFilter filter)
        {
            if (this.filtersField == null)
            {
                this.filtersField = new List<CoverageViewFilterBase>();
            }
            if ((filter.FilterType != CoverageFilterType.Threshold) && !(from ff in this.filtersField
                where (((ff.FilterRuleData == filter.FilterRuleData) && (ff.FilterType == filter.FilterType)) && (ff.Include == filter.Include)) && (ff.RegexRule == filter.RegexRule)
                select ff).Any<CoverageViewFilterBase>())
            {
                this.filtersField.Add(new CoverageViewFilterBase { FilterRuleData = filter.FilterRuleData, FilterType = filter.FilterType, Include = filter.Include, RegexRule = filter.RegexRule });
            }
        }

        public virtual void AddRebasedPath(ICoverageViewRebasedPath path)
        {
            if (this.XmlRebasedPaths == null)
            {
                this.XmlRebasedPaths = new List<CoverageViewRebasedPath>();
            }
            this.XmlRebasedPaths.Add(new CoverageViewRebasedPath { RebasedPath = path.RebasedPath, SourcePath = path.SourcePath });
        }

		public virtual void AddReportOutput(IReportOutputData report)
		{
			if (XmlReports == null)
				XmlReports = new List<ReportOutputData>();

			var exists = (from x in XmlReports
						  where x.Format == report.Format
						  && x.ReportType == report.ReportType
						  && x.OutputPath == report.OutputPath
						  select x).Any();

			if (!exists)
			{

				var list = report.Params == null ? null : (from p in report.Params
														   select new NameValuePair(p.Name, p.Value)).ToList();

				XmlReports.Add(new ReportOutputData()
				{
					Format = report.Format,
					OutputPath = report.OutputPath,
					ReportType = report.ReportType,
					XmlParams = list
				});
			}
		}

        public virtual void AddReportOutput(CoverageReportType reportType, ReportOutputFormats format)
        {
            this.AddReportOutput(new ReportOutputData { Format = format, ReportType = reportType });
        }

        public virtual void AddSatisfactoryThreshold(ICoverageThreshold threshold)
        {
            this.AddThreshold(ref this.satisfactoryThresholdsField, threshold);
        }

        private void AddThreshold(ref List<CoverageThreshold> l, ICoverageThreshold threshold)
        {
            if (l == null)
            {
                l = new List<CoverageThreshold>();
            }
            if (!(from bb in l
                where ((bb.Value.Within(0.001f, threshold.Value) && (bb.CoverageType == threshold.CoverageType)) && (bb.Pattern == threshold.Pattern)) && (bb.ElementType == threshold.ElementType)
                select bb).Any<CoverageThreshold>())
            {
                l.Add(new CoverageThreshold { Value = threshold.Value, CoverageType = threshold.CoverageType, Pattern = threshold.Pattern, ElementType = threshold.ElementType });
            }
        }

        public virtual void ClearFailBuildThresholds()
        {
            this.failBuildThresholdsField = null;
        }

        public virtual void ClearFilters()
        {
            this.filtersField = null;
        }

        public virtual void ClearReports()
        {
            this.XmlReports = null;
        }

        public virtual void ClearSatisfactoryThresholds()
        {
            this.satisfactoryThresholdsField = null;
        }

        public virtual void RemoveAllReports(IReportOutputData report)
        {
            Predicate<ReportOutputData> match = null;
            if (this.XmlReports != null)
            {
                if (match == null)
                {
                    match = rr => ((rr.Format == report.Format) && (rr.ReportType == report.ReportType)) && (rr.OutputPath == report.OutputPath);
                }
                this.XmlReports.RemoveAll(match);
                if (this.XmlReports.Count < 1)
                {
                    this.ClearReports();
                }
            }
        }

        public virtual void RemoveFailBuildThreshold(ICoverageThreshold threshold)
        {
            this.RemoveThreshold(ref this.failBuildThresholdsField, threshold);
        }

        public virtual void RemoveFilter(ICoverageViewFilter filter)
        {
            Predicate<CoverageViewFilterBase> match = null;
            if (this.filtersField != null)
            {
                if (match == null)
                {
                    match = ff => (((ff.FilterRuleData == filter.FilterRuleData) && (ff.FilterType == filter.FilterType)) && (ff.Include == filter.Include)) && (ff.RegexRule == filter.RegexRule);
                }
                this.filtersField.RemoveAll(match);
            }
        }

        public virtual void RemoveSatisfactoryThreshold(ICoverageThreshold threshold)
        {
            this.RemoveThreshold(ref this.satisfactoryThresholdsField, threshold);
        }

        private void RemoveThreshold(ref List<CoverageThreshold> l, ICoverageThreshold threshold)
        {
            Predicate<CoverageThreshold> match = null;
            if (l != null)
            {
                if (match == null)
                {
                    match = tt => ((tt.Value.Within(0.001f, threshold.Value) && (tt.CoverageType == threshold.CoverageType)) && (tt.Pattern == threshold.Pattern)) && (tt.ElementType == threshold.ElementType);
                }
                l.RemoveAll(match);
            }
        }

        public virtual void ReplaceFailBuildThreshold(ICoverageThreshold threshold)
        {
            this.ReplaceThreshold(ref this.failBuildThresholdsField, threshold);
        }

        public virtual void ReplaceSatisfactoryThreshold(ICoverageThreshold threshold)
        {
            this.ReplaceThreshold(ref this.satisfactoryThresholdsField, threshold);
        }

        private void ReplaceThreshold(ref List<CoverageThreshold> l, ICoverageThreshold threshold)
        {
            Predicate<CoverageThreshold> match = null;
            if (l != null)
            {
                if (match == null)
                {
                    match = tt => ((tt.CoverageType == threshold.CoverageType) && (tt.Pattern == threshold.Pattern)) && (tt.ElementType == threshold.ElementType);
                }
                l.RemoveAll(match);
            }
            this.AddThreshold(ref l, threshold);
        }

        public virtual void SaveToFile(string fileName)
        {
            using (FileStream stream = File.Create(fileName))
            {
                this.SaveToStream(stream);
            }
        }

        public virtual void SaveToStream(Stream stream)
        {
            new XmlSerializer(base.GetType()).Serialize(stream, this);
        }

        [CommandLineSwitch("//build-id"), GroupKey(1), ScriptArgument("buildId", ScriptOutputType.NAntNCover), ScriptArgument("BuildId", ScriptOutputType.MSBuildNCover), ScriptArgument("//bi", ScriptOutputType.CommandLine)]
        public virtual string BuildId { get; set; }

        [ScriptArgument("ClearCoverageFilters", ScriptOutputType.MSBuildNCover), XmlElement("ClearCoverageFilters"), ScriptArgument("//ccf", ScriptOutputType.CommandLine), ScriptArgument("clearCoverageFilters", ScriptOutputType.NAntNCover), CommandLineSwitch("//clear-coverage-filters"), GroupKey(3)]
        public virtual bool ClearCoverageFilters { get; set; }

        [ScriptArgumentCollection("CoverageDataPaths", "CoverageDataPath", typeof(string), ScriptOutputType.MSBuildNCover), XmlArrayItem("CoverageDataPath"), ScriptArgumentCollection("coverageDataPaths", "coverageDataPath", typeof(string), ScriptOutputType.NAntNCover), GroupKey(1), ScriptArgument("", ScriptOutputType.CommandLine), XmlArray("CoverageDataPaths")]
        public virtual List<string> CoverageDataFileNames
        {
            get
            {
                return this.coverageDataFileNamesField;
            }
            set
            {
                this.coverageDataFileNamesField = value;
            }
        }

        [ScriptArgument("coverageTrendPath", ScriptOutputType.NAntNCover), XmlElement("CoverageTrendPath"), ScriptArgument("//at", ScriptOutputType.CommandLine), ScriptArgument("CoverageTrendPath", ScriptOutputType.MSBuildNCover), GroupKey(1), CommandLineSwitch("//append-trend")]
        public virtual string CoverageTrendPath { get; set; }

        [XmlIgnore]
        public virtual IEnumerable<ICoverageThreshold> FailBuildThresholds
        {
            get
            {
                if (this.failBuildThresholdsField == null)
                {
                    return new List<ICoverageThreshold>();
                }
                return this.failBuildThresholdsField.AsEnumerable<CoverageThreshold>().Cast<ICoverageThreshold>();
            }
        }

        [XmlIgnore]
        public string FileNameLoadedFrom { get; set; }

        [XmlIgnore]
        public virtual IEnumerable<ICoverageViewFilter> Filters
        {
            get
            {
                if (this.filtersField == null)
                {
                    return new List<ICoverageViewFilter>();
                }
                return this.filtersField.AsEnumerable<CoverageViewFilterBase>().Cast<ICoverageViewFilter>();
            }
        }

        [ConvertEnumValue(typeof(ViewFilterStyle)), ScriptArgument("hide", ScriptOutputType.NAntNCover), ScriptArgument("//hi", ScriptOutputType.CommandLine), GroupKey(1), CommandLineSwitch("//hide"), XmlElement("Hide"), ScriptArgument("Hide", ScriptOutputType.MSBuildNCover)]
        public virtual ViewFilterStyle FilterStyle { get; set; }

        [XmlIgnore]
        public virtual bool HasAppendTrends
        {
            get
            {
                return !string.IsNullOrEmpty(this.CoverageTrendPath);
            }
        }

        [XmlIgnore]
        public virtual bool HasCoverageDataFileNames
        {
            get
            {
                return ((this.coverageDataFileNamesField != null) && (this.coverageDataFileNamesField.Count > 0));
            }
        }

        [XmlIgnore]
        public virtual bool HasFailBuildThresholds
        {
            get
            {
                return ((this.failBuildThresholdsField != null) && (this.failBuildThresholdsField.Count > 0));
            }
        }

        [XmlIgnore]
        public virtual bool HasFilters
        {
            get
            {
                return ((this.filtersField != null) && (this.filtersField.Count > 0));
            }
        }

        [XmlIgnore]
        public virtual bool HasLoadTrends
        {
            get
            {
                return !string.IsNullOrEmpty(this.LoadTrendPath);
            }
        }

        [XmlIgnore]
        public virtual bool HasReports
        {
            get
            {
                return ((this.XmlReports != null) && (this.XmlReports.Count > 0));
            }
        }

        [XmlIgnore]
        public virtual bool HasSatisfactoryThresholds
        {
            get
            {
                return ((this.satisfactoryThresholdsField != null) && (this.satisfactoryThresholdsField.Count > 0));
            }
        }

        [ScriptArgument("//ifb", ScriptOutputType.CommandLine), XmlElement("IgnoreFailedBuilds"), CommandLineSwitch("//ignorefailedbuilds"), ScriptArgument("ignoreFailedBuilds", ScriptOutputType.NAntNCover), GroupKey(1), ScriptArgument("IgnoreFailedBuilds", ScriptOutputType.MSBuildNCover)]
        public virtual bool IgnoreFailedBuilds { get; set; }

        [XmlElement("LoadTrendPath"), ScriptArgument("//lt", ScriptOutputType.CommandLine), ScriptArgument("loadTrendPath", ScriptOutputType.NAntNCover), GroupKey(1), ScriptArgument("LoadTrendPath", ScriptOutputType.MSBuildNCover), CommandLineSwitch("//loadtrend")]
        public virtual string LoadTrendPath { get; set; }

        [ScriptArgument("maxFailedToShow", ScriptOutputType.NAntNCover), GroupKey(3), CommandLineSwitch("//showmaxfailed"), ScriptArgument("//smf", ScriptOutputType.CommandLine), ScriptArgument("MaxFailedToShow", ScriptOutputType.MSBuildNCover)]
        public virtual int MaxFailedToShow { get; set; }

        [GroupKey(2), ScriptArgument("//s", ScriptOutputType.CommandLine), XmlElement("MergedCoverageDataPath"), CommandLineSwitch("//save"), ScriptArgument("mergeFileName", ScriptOutputType.NAntNCover), ScriptArgument("MergeFileName", ScriptOutputType.MSBuildNCover)]
        public virtual string MergedCoverageDataFileName { get; set; }

        [CommandLineSwitch("//mergefiltermode"), ScriptArgument("MergeFilterMode", ScriptOutputType.MSBuildNCover), ScriptArgument("//mfm", ScriptOutputType.CommandLine), ScriptArgument("mergeFilterMode", ScriptOutputType.NAntNCover), GroupKey(2), XmlElement("MergeFilterMode")]
        public virtual NCover.Interfaces.Enumerations.MergeFilterMode MergeFilterMode { get; set; }

        [ScriptArgument("//tu", ScriptOutputType.CommandLine), ScriptArgument("maxTopUncoveredToReport", ScriptOutputType.NAntNCover), ScriptArgument("MaxTopUncoveredToReport", ScriptOutputType.MSBuildNCover), GroupKey(1), CommandLineSwitch("//topuncovered")]
        public virtual int NumOfTopUncoveredToReport { get; set; }

        [GroupKey(1), ScriptArgument("//op", ScriptOutputType.CommandLine), ScriptArgument("outputPath", ScriptOutputType.NAntNCover), XmlElement("OutputPath"), CommandLineSwitch("//outputpath"), ScriptArgument("OutputPath", ScriptOutputType.MSBuildNCover)]
        public virtual string OutputPath { get; set; }

        [XmlAttribute("version")]
        public int ParseVersion
        {
            get
            {
                return 3;
            }
            set
            {
            }
        }

        [ScriptArgument("projectName", ScriptOutputType.NAntNCover), ScriptArgument("ProjectName", ScriptOutputType.MSBuildNCover), GroupKey(1), CommandLineSwitch("//project"), ScriptArgument("//p", ScriptOutputType.CommandLine)]
        public virtual string ProjectName { get; set; }

        [ScriptArgument("//rbp", ScriptOutputType.CommandLine), ScriptArgument("rebasedPaths", ScriptOutputType.NAntNCover), XmlIgnore, CommandLineSwitch("//rebasedpath"), GroupKey(1), ScriptArgument("RebasedPaths", ScriptOutputType.MSBuildNCover)]
        public virtual IEnumerable<ICoverageViewRebasedPath> RebasedPaths
        {
            get
            {
                if (this.XmlRebasedPaths == null)
                {
                    this.XmlRebasedPaths = new List<CoverageViewRebasedPath>();
                }
                return this.XmlRebasedPaths.Cast<ICoverageViewRebasedPath>();
            }
        }

        [ScriptArgument("//rdf", ScriptOutputType.CommandLine), ScriptArgument("reportDataFilter", ScriptOutputType.NAntNCover), GroupKey(3), XmlElement("ReportDataFilter"), ScriptArgument("ReportDataFilter", ScriptOutputType.MSBuildNCover)]
        public virtual CoverageFilterType ReportDataFilter { get; set; }

        [ScriptArgument("reportInvalidFiles", ScriptOutputType.NAntNCover), GroupKey(3), XmlElement("ReportInvalidFiles"), CommandLineSwitch("//reportinvalidfiles"), ScriptArgument("//rif", ScriptOutputType.CommandLine), ScriptArgument("ReportInvalidFiles", ScriptOutputType.MSBuildNCover)]
        public virtual bool ReportInvalidFiles { get; set; }

        [XmlIgnore, Browsable(false)]
        public virtual IEnumerable<IReportOutputData> Reports
        {
            get
            {
                if (this.XmlReports == null)
                {
                    return new List<IReportOutputData>();
                }
                return this.XmlReports.Cast<IReportOutputData>();
            }
        }

        [XmlIgnore]
        public virtual IEnumerable<ICoverageThreshold> SatisfactoryThresholds
        {
            get
            {
                if (this.satisfactoryThresholdsField == null)
                {
                    return new List<ICoverageThreshold>();
                }
                return this.satisfactoryThresholdsField.AsEnumerable<CoverageThreshold>().Cast<ICoverageThreshold>();
            }
        }

        [XmlIgnore]
        public virtual bool ShowExcludedFooter { get; set; }

        [ConvertEnumValue(typeof(ViewSortStyle)), GroupKey(1), XmlElement("SortBy"), CommandLineSwitch("//sort"), ScriptArgument("//so", ScriptOutputType.CommandLine), ScriptArgument("SortBy", ScriptOutputType.MSBuildNCover), ScriptArgument("sortBy", ScriptOutputType.NAntNCover)]
        public virtual ViewSortStyle SortStyle { get; set; }

        [ScriptArgument("SatCoverageFromMinCoverage", ScriptOutputType.MSBuildNCover), GroupKey(3), ScriptArgument("//mcsc", ScriptOutputType.CommandLine), ScriptArgument("satCoverageFromMinCoverage", ScriptOutputType.NAntNCover), XmlElement("SatCoverageFromMinCoverage")]
        public virtual bool UseMinCoverageForSatCoverage { get; set; }

        [ScriptArgument("//w", ScriptOutputType.CommandLine), GroupKey(2), ScriptArgument("workingDirectory", ScriptOutputType.NAntNCover), ScriptArgument("WorkingDirectory", ScriptOutputType.MSBuildNCover)]
        public virtual string WorkingDirectory { get; set; }

        [Browsable(false), ScriptArgumentCollection("MinimumCoverage", "Threshold", typeof(CoverageThreshold), ScriptOutputType.MSBuildNCover), XmlArrayItem("Threshold"), ForNCoverFeature(NCoverFeature.MinimumCoverageRules), ScriptArgument("//mc", ScriptOutputType.CommandLine), XmlArray("MinimumCoverage"), GroupKey(3), ScriptArgumentCollection("minimumCoverage", "threshold", typeof(CoverageThreshold), ScriptOutputType.NAntNCover)]
        public virtual List<CoverageThreshold> XmlFailBuildThresholds
        {
            get
            {
                return this.failBuildThresholdsField;
            }
            set
            {
                this.failBuildThresholdsField = value;
            }
        }

        [ScriptArgumentCollection("coverageFilters", "filter", typeof(CoverageViewFilterBase), ScriptOutputType.NAntNCover), XmlArray("CoverageFilters"), GroupKey(3), Browsable(false), CommandLineSwitch("//coveragefilter"), ScriptArgument("//cf", ScriptOutputType.CommandLine), ScriptArgumentCollection("CoverageFilters", "Filter", typeof(CoverageViewFilterBase), ScriptOutputType.MSBuildNCover), XmlArrayItem("CoverageFilter")]
        public virtual List<CoverageViewFilterBase> XmlFilters
        {
            get
            {
                return this.filtersField;
            }
            set
            {
                this.filtersField = value;
            }
        }

        [XmlArrayItem("Path"), XmlArray("RebasedPaths")]
        public List<CoverageViewRebasedPath> XmlRebasedPaths { get; set; }

        [ScriptArgument("//or", ScriptOutputType.CommandLine), ScriptArgumentCollection("reports", "report", typeof(ReportOutputData), ScriptOutputType.NAntNCover), CommandLineSwitch("//outputreport"), XmlArrayItem("Report"), ScriptArgumentCollection("OutputReport", "Report", typeof(ReportOutputData), ScriptOutputType.MSBuildNCover), GroupKey(1), XmlArray("Reports")]
        public virtual List<ReportOutputData> XmlReports { get; set; }

        [ScriptArgumentCollection("satisfactoryCoverage", "threshold", typeof(CoverageThreshold), ScriptOutputType.NAntNCover), GroupKey(3), XmlArray("SatisfactoryCoverage"), XmlArrayItem("Threshold"), Browsable(false), ForNCoverFeature(NCoverFeature.SatisfactoryCoverageRules), ScriptArgument("//sct", ScriptOutputType.CommandLine), ScriptArgumentCollection("SatisfactoryCoverage", "Threshold", typeof(CoverageThreshold), ScriptOutputType.MSBuildNCover)]
        public virtual List<CoverageThreshold> XmlSatisfactoryThresholds
        {
            get
            {
                return this.satisfactoryThresholdsField;
            }
            set
            {
                this.satisfactoryThresholdsField = value;
            }
        }

        [XmlElement("XsltOverridePath"), GroupKey(1), ScriptArgument("xsltOverridePath", ScriptOutputType.NAntNCover), Browsable(false), ScriptArgument("//xslt", ScriptOutputType.CommandLine), ScriptArgument("XsltOverridePath", ScriptOutputType.MSBuildNCover)]
        public virtual string XsltOverridePath { get; set; }
    }
}
