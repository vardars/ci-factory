namespace NCover.Framework.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class IncludeWhenNotDefaultValueAttribute : IncludeValueAttribute
    {
        public IncludeWhenNotDefaultValueAttribute(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        public override bool IncludeValue(object value)
        {
            return !this.DefaultValue.Equals(value);
        }

        public object DefaultValue { get; private set; }
    }
}
