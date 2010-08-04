namespace NCover.Framework
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    public sealed class ReplacesFeatureAttribute : Attribute
    {
        public ReplacesFeatureAttribute(ScriptOutputType outputType, string oldPropertyName)
        {
            this.OutputType = outputType;
            this.OldPropertyName = oldPropertyName;
        }

        public ReplacesFeatureAttribute(ScriptOutputType outputType, string oldPropertyName, string change)
        {
            this.OutputType = outputType;
            this.OldPropertyName = oldPropertyName;
            this.Change = change;
        }

        public string Change { get; set; }

        public string OldPropertyName { get; set; }

        public ScriptOutputType OutputType { get; set; }
    }
}
