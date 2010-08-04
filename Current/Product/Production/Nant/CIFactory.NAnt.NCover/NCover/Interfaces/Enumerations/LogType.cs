namespace NCover.Interfaces.Enumerations
{
    using System;

    [Flags]
    public enum LogType
    {
        AssemVerification = 8,
        DocsAdded = 0x10,
        Instrument = 4,
        JITMessages = 0x20,
        MethodIL = 2,
        None = 0,
        Timing = 1
    }
}
