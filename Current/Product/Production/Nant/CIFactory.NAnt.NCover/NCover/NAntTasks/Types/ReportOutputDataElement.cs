namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NCover.Framework.Reports;
    using NCover.Interfaces.Enumerations;
    using System;

    [ElementName("report")]
    public class ReportOutputDataElement : Element
    {
        private ReportOutputData _backingField = new ReportOutputData();
        private ParamsCollection _params = new ParamsCollection();

        [TaskAttribute("format")]
        public ReportOutputFormats Format
        {
            get
            {
                return this._backingField.Format;
            }
            set
            {
                this._backingField.Format = value;
            }
        }

        [TaskAttribute("outputPath"), StringValidator(AllowEmpty=false)]
        public string OutputPath
        {
            get
            {
                return this._backingField.OutputPath;
            }
            set
            {
                this._backingField.OutputPath = value;
            }
        }

        [BuildElementCollection("params", "param")]
        public ParamsCollection Params
        {
            get
            {
                return this._params;
            }
        }

        public ReportOutputData Report
        {
            get
            {
                return this._backingField;
            }
        }

        [TaskAttribute("reportType", Required=true)]
        public CoverageReportType ReportType
        {
            get
            {
                return this._backingField.ReportType;
            }
            set
            {
                this._backingField.ReportType = value;
            }
        }
    }
}
