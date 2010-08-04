namespace NCover.Interfaces.Enumerations
{
    using System;

    public enum ScriptOutputType
    {
        None,
        CommandLine,
        NCoverSettings,
        MSBuildNCover,
        MSBuildExec,
        NAntNCover,
        NAntExec
    }
}
