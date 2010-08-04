namespace NCover.Interfaces.Enumerations
{
    using System;

    [Flags]
    public enum SymbolSearchPolicy
    {
        BuildPath = 4,
        ExecutingDir = 8,
        None = 0,
        Registry = 1,
        SymbolServer = 2
    }
}
