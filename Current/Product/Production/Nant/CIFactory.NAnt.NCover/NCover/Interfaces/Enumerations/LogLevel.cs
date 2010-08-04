namespace NCover.Interfaces.Enumerations
{
    using System;

    public enum LogLevel
    {
        Complete = 0x100,
        Diagnostic = 8,
        Diagnostic2 = 0x10,
        Diagnostic3 = 0x20,
        None = 0,
        Normal = 1,
        Verbose = 2
    }
}
