namespace NCover.Interfaces
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    public class CoverageTypeMappingAttribute : Attribute
    {
        public CoverageTypeMappingAttribute()
        {
        }

        public CoverageTypeMappingAttribute(CoverageMetric coverageType)
        {
            this.CoverageType = coverageType;
        }

        public static CoverageMetric Value(ViewReportStyle vrs)
        {
            Type type = typeof(ViewReportStyle);
            string name = vrs.ToString();
            return (from ca in type.GetField(name).GetCustomAttributes(typeof(CoverageTypeMappingAttribute), false).Cast<CoverageTypeMappingAttribute>() select ca.CoverageType).First<CoverageMetric>();
        }

        public CoverageMetric CoverageType { get; set; }
    }
}
