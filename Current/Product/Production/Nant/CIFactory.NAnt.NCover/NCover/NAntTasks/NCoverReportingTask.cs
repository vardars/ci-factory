namespace NCover.NAntTasks
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Tasks;
    using NAnt.Core.Types;
    using NCover.Framework.Reports;
    using NCover.Framework.Scripting;
    using NCover.Framework.Views;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Extensions;
    using NCover.NAntTasks.Types;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    [TaskName("ncoverreporting")]
    public class NCoverReportingTask : ExternalProgramBase
    {
        private string _configName;
        private FileSet _coverageFiles = new FileSet();
        private CoverageReportSetting _crs = new CoverageReportSetting();
        private ThresholdElementCollection _failBuildThresholds = new ThresholdElementCollection();
        private FilterElementCollection _filters = new FilterElementCollection();
        private StringBuilder _programArguments;
        private RebasedPathElementCollection _rebasedPaths = new RebasedPathElementCollection();
        private ReportOutputFormats _reportFormat;
        private ReportOutputDataCollection _reports = new ReportOutputDataCollection();
        private CoverageReportType _reportType;
        private ThresholdElementCollection _satisfactoryThresholds = new ThresholdElementCollection();
        private bool _usedlegacyreportparams;
        private bool _userSetConfigFile = false;

        public NCoverReportingTask()
        {
            this.ExeName = "NCover.Reporting.exe";
        }

        private void _OnProcessExited(object sender, EventArgs e)
        {
            if (File.Exists(this.SettingsFile) && !this._userSetConfigFile)
            {
                this.Log(Level.Verbose, "Deleting config file: " + this.SettingsFile);
                File.Delete(this.SettingsFile);
            }
        }

        private void _PopulateCollections()
        {
            this._crs.CoverageDataFileNames = new List<string>(this._coverageFiles.FileNames.Cast<string>());
            for (int i = 0; i < this._failBuildThresholds.Count; i++)
            {
                this._crs.AddFailBuildThreshold(this._failBuildThresholds[i].Threshold);
            }
            for (int j = 0; j < this._satisfactoryThresholds.Count; j++)
            {
                this._crs.AddSatisfactoryThreshold(this._satisfactoryThresholds[j].Threshold);
            }
            for (int k = 0; k < this._filters.Count; k++)
            {
                this._crs.AddFilter(this._filters[k].Filter);
            }
            for (int m = 0; m < this._reports.Count; m++)
            {
                ReportOutputData report = this._reports[m].Report;
                for (int num5 = 0; num5 < this._reports[m].Params.Count; num5++)
                {
                    report.XmlParams.Add(this._reports[m].Params[num5].Pair);
                }
                this._crs.AddReportOutput(report);
            }
            for (int n = 0; n < this._rebasedPaths.Count; n++)
            {
                RebasedPathElement element = this._rebasedPaths[n];
                this._crs.AddRebasedPath(new CoverageViewRebasedPath { SourcePath = element.FromPath, RebasedPath = element.ToPath });
            }
            if (this._usedlegacyreportparams)
            {
                ReportOutputData data2 = new ReportOutputData {
                    Format = this._reportFormat,
                    ReportType = this._reportType
                };
                this._crs.AddReportOutput(data2);
            }
        }

        protected override void PrepareProcess(Process process)
        {
            if ((this._coverageFiles == null) || (this._coverageFiles.FileNames.Count == 0))
            {
                throw new BuildException("No matching coverage files found in the <fileset> specified in your <ncoverreporting /> NAnt task.");
            }
            this._PopulateCollections();
            this._userSetConfigFile = !string.IsNullOrEmpty(this.SettingsFile);
            if (!this._userSetConfigFile)
            {
                this.SettingsFile = Path.GetTempFileName();
            }
            NCoverReportingSettingsFileWriter.BuildTempSettingsXmlFile(this._crs, this.SettingsFile);
            this._programArguments = new StringBuilder();
            this._programArguments.AppendFormat("//cr \"{0}\" ", this.SettingsFile);
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string message = File.ReadAllText(this.SettingsFile);
                this.Log(Level.Verbose, "Contents of NCoverExplorer settings file:");
                this.Log(Level.Verbose, message);
            }
            this.Log(Level.Verbose, "Working directory: {0}", new object[] { this.BaseDirectory });
            this.Log(Level.Verbose, "Arguments: {0}", new object[] { this.ProgramArguments });
            base.PrepareProcess(process);
        }

        protected override Process StartProcess()
        {
            if (!Path.IsPathRooted(this.ExeName))
            {
                this.ExeName = this.Project.GetFullPath(this.ExeName);
            }
            Process process = base.StartProcess();
            process.Exited += new EventHandler(this._OnProcessExited);
            return process;
        }

        [TaskAttribute("buildId", Required=false), StringValidator(AllowEmpty=false)]
        public string BuildId
        {
            get
            {
                return this._crs.BuildId;
            }
            set
            {
                this._crs.BuildId = value;
            }
        }

        [TaskAttribute("clearCoverageFilters"), BooleanValidator]
        public bool ClearCoverageFilters
        {
            get
            {
                return this._crs.ClearCoverageFilters;
            }
            set
            {
                this._crs.ClearCoverageFilters = value;
            }
        }

        [BuildElement("coverageDataPaths", Required=true)]
        public FileSet CoverageFiles
        {
            get
            {
                return this._coverageFiles;
            }
            set
            {
                this._coverageFiles = value;
            }
        }

        [BuildElementCollection("coverageFilters", "filter")]
        public FilterElementCollection CoverageFilters
        {
            get
            {
                return this._filters;
            }
        }

        [TaskAttribute("coverageTrendPath"), StringValidator(AllowEmpty=false)]
        public string CoverageTrendPath
        {
            get
            {
                return this._crs.CoverageTrendPath;
            }
            set
            {
                this._crs.CoverageTrendPath = value;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("program", Required=false)]
        public override string ExeName
        {
            get
            {
                return base.ExeName;
            }
            set
            {
                base.ExeName = value;
            }
        }

        [BuildElementCollection("minimumCoverage", "threshold")]
        public ThresholdElementCollection FailBuildThresholds
        {
            get
            {
                return this._failBuildThresholds;
            }
        }

        [TaskAttribute("hide"), StringValidator(AllowEmpty=false)]
        public string Filter
        {
            get
            {
                return this._crs.FilterStyle.ToString();
            }
            set
            {
                ViewFilterStyle style;
                if (!ParseExtensions.TryParseEnum<ViewFilterStyle>(value, out style))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid filter style.");
                }
                this._crs.FilterStyle = style;
            }
        }

        [TaskAttribute("ignoreFailedBuilds"), BooleanValidator]
        public bool IgnoreFailedBuilds
        {
            get
            {
                return this._crs.IgnoreFailedBuilds;
            }
            set
            {
                this._crs.IgnoreFailedBuilds = value;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("loadTrendPath")]
        public string LoadTrendPath
        {
            get
            {
                return this._crs.LoadTrendPath;
            }
            set
            {
                this._crs.LoadTrendPath = value;
            }
        }

        [TaskAttribute("maxFailedToShow"), Int32Validator(MinValue=0, Base=10)]
        public int MaxFailedToShow
        {
            get
            {
                return this._crs.MaxFailedToShow;
            }
            set
            {
                this._crs.MaxFailedToShow = value;
            }
        }

        [TaskAttribute("mergeFileName"), StringValidator(AllowEmpty=false)]
        public string MergeFileName
        {
            get
            {
                return this._crs.MergedCoverageDataFileName;
            }
            set
            {
                this._crs.MergedCoverageDataFileName = value;
            }
        }

        [BuildElement("mergeFilterMode")]
        public NCover.Interfaces.Enumerations.MergeFilterMode MergeFilterMode
        {
            get
            {
                return this._crs.MergeFilterMode;
            }
            set
            {
                this._crs.MergeFilterMode = value;
            }
        }

        [Int32Validator(Base=10, MinValue=1), TaskAttribute("maxTopUncoveredToReport")]
        public int NumOfTopUncoveredToReport
        {
            get
            {
                return this._crs.NumOfTopUncoveredToReport;
            }
            set
            {
                this._crs.NumOfTopUncoveredToReport = value;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("outputPath")]
        public string OutputPath
        {
            get
            {
                return this._crs.OutputPath;
            }
            set
            {
                this._crs.OutputPath = value;
            }
        }

        public override string ProgramArguments
        {
            get
            {
                return this._programArguments.ToString();
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("projectName")]
        public string ProjectName
        {
            get
            {
                return this._crs.ProjectName;
            }
            set
            {
                this._crs.ProjectName = value;
            }
        }

        [BuildElementCollection("rebasedPaths", "path")]
        public RebasedPathElementCollection RebasedPaths
        {
            get
            {
                return this._rebasedPaths;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("reportDataFilter")]
        public string ReportDataFilter
        {
            get
            {
                return this._crs.ReportDataFilter.ToString();
            }
            set
            {
                this._crs.ReportDataFilter = (CoverageFilterType) Enum.Parse(typeof(CoverageFilterType), value);
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("reportFormat")]
        public string ReportFormat
        {
            get
            {
                return this._reportFormat.ToString();
            }
            set
            {
                ConvertReportOutputFormatEnumAttribute attribute = new ConvertReportOutputFormatEnumAttribute(typeof(ReportOutputFormats));
                this._reportFormat = (ReportOutputFormats) attribute.ConvertBack(value);
                this._usedlegacyreportparams = true;
                this.Log(Level.Warning, "'reportFormat' has been deprecated and will be removed in v4.0. Use '<reports />' instead.");
            }
        }

        [BooleanValidator, TaskAttribute("reportInvalidFiles")]
        public bool ReportInvalidFiles
        {
            get
            {
                return this._crs.ReportInvalidFiles;
            }
            set
            {
                this._crs.ReportInvalidFiles = value;
            }
        }

        [BuildElementCollection("reports", "report")]
        public ReportOutputDataCollection Reports
        {
            get
            {
                return this._reports;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("reportType")]
        public string ReportType
        {
            get
            {
                return this._reportType.ToString();
            }
            set
            {
                CoverageReportType type;
                if (!ParseExtensions.TryParseEnum<CoverageReportType>(value, out type))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid report type.");
                }
                this._reportType = type;
                this._usedlegacyreportparams = true;
                this.Log(Level.Warning, "'reportType' has been deprecated and will be removed in v4.0. Use '<reports />' instead.");
            }
        }

        [BuildElementCollection("satisfactoryCoverage", "threshold")]
        public ThresholdElementCollection SatisfactoryThresholds
        {
            get
            {
                return this._satisfactoryThresholds;
            }
        }

        [StringValidator(AllowEmpty=false), BuildElement("settingsFile")]
        public string SettingsFile { get; set; }

        [StringValidator(AllowEmpty=false), TaskAttribute("sortBy")]
        public string Sort
        {
            get
            {
                return this._crs.SortStyle.ToString();
            }
            set
            {
                ViewSortStyle style;
                if (!ParseExtensions.TryParseEnum<ViewSortStyle>(value, out style))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid sort style.");
                }
                this._crs.SortStyle = style;
            }
        }

        [TaskAttribute("satCoverageFromMinCoverage"), BooleanValidator]
        public bool UseMinCoverageForSatCoverage
        {
            get
            {
                return this._crs.UseMinCoverageForSatCoverage;
            }
            set
            {
                this._crs.UseMinCoverageForSatCoverage = value;
            }
        }

        [TaskAttribute("workingDirectory"), StringValidator(AllowEmpty=false)]
        public string WorkingDirectory
        {
            get
            {
                return this._crs.WorkingDirectory;
            }
            set
            {
                this._crs.WorkingDirectory = value;
            }
        }

        [TaskAttribute("xsltOverridePath"), StringValidator(AllowEmpty=false)]
        public string XsltOverridePath
        {
            get
            {
                return this._crs.XsltOverridePath;
            }
            set
            {
                this._crs.XsltOverridePath = value;
            }
        }
    }
}
