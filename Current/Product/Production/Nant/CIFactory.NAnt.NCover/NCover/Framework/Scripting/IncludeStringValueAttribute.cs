namespace NCover.Framework.Scripting
{
    using System;

    public sealed class IncludeStringValueAttribute : IncludeValueAttribute
    {
        public override bool IncludeValue(object value)
        {
            return ((value is string) && !string.IsNullOrEmpty(value as string));
        }
    }
}
