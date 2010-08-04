namespace NCover.Framework.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=true)]
    public sealed class CommandLineSwitchAttribute : Attribute
    {
        public CommandLineSwitchAttribute()
        {
        }

        public CommandLineSwitchAttribute(string switchName)
        {
            this.Switch = switchName;
        }

        public string Switch { get; set; }

        public string SwitchDescription { get; set; }
    }
}
