namespace NCover.Framework.Scripting
{
    using System;
    using System.Collections.Generic;

    public sealed class ConvertStringArrayAttribute : ConvertValueAttribute
    {
        public override object ConvertBack(string fromVal)
        {
            return new List<string>(fromVal.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public override string ConvertFrom(object fromVal)
        {
            if (fromVal is string[])
            {
                string[] strArray = fromVal as string[];
                return string.Join(";", strArray);
            }
            if (fromVal is List<string>)
            {
                List<string> list = fromVal as List<string>;
                return string.Join(";", list.ToArray());
            }
            return string.Empty;
        }
    }
}
