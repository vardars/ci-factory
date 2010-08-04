namespace NCover.Framework.Scripting
{
    using System;
    using System.Collections.Generic;

    public sealed class IncludeStringArrayValueAttribute : IncludeValueAttribute
    {
        public override bool IncludeValue(object value)
        {
            if (value is string[])
            {
                string[] strArray = value as string[];
                return ((strArray != null) && (strArray.Length > 0));
            }
            if (!(value is List<string>))
            {
                return false;
            }
            List<string> list = value as List<string>;
            return ((list != null) && (list.Count > 0));
        }
    }
}
