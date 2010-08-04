namespace NCover.Interfaces.Enumerations
{
    using System;

    [Flags]
    public enum CoverageFilterType
    {
        Assembly = 1,
        Class = 4,
        Document = 0x10,
        Method = 8,
        Namespace = 2,
        None = 0,
        Threshold = 0x20
    }
}
