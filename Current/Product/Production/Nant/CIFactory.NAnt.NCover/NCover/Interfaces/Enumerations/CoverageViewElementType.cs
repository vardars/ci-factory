namespace NCover.Interfaces.Enumerations
{
    using System;

    public enum CoverageViewElementType
    {
        Assembly = 4,
        Class = 0x10,
        Document = 1,
        InstrumentedPoint = 0x40,
        Method = 0x20,
        Module = 4,
        Namespace = 8,
        Property = 0x80,
        Unknown = 0,
        View = 2
    }
}
