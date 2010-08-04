namespace NCover.Interfaces
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ReportStyleMappingAttribute : Attribute
    {
        public ReportStyleMappingAttribute(ViewReportStyle vrs)
        {
            this.ReportStyle = vrs;
        }

        public ViewReportStyle ReportStyle { get; private set; }
    }
}
