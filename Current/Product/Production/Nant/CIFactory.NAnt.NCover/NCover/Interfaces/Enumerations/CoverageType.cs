namespace NCover.Interfaces.Enumerations
{
    using System;

    [Flags]
    public enum CoverageType
    {
        Branch = 2,
        CyclomaticComplexity = 8,
        Decision = 4,
        MethodVisits = 0x10,
        None = 0,
        Symbol = 1
    }
}
