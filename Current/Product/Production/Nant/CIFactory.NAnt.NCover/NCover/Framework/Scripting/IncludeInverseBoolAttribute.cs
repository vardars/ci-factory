namespace NCover.Framework.Scripting
{
    using System;

    public sealed class IncludeInverseBoolAttribute : IncludeValueAttribute
    {
        public override bool IncludeValue(object value)
        {
            return ((value is bool) && !((bool) value));
        }
    }
}
