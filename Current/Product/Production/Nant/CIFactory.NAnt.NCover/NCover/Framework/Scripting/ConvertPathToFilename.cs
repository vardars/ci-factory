namespace NCover.Framework.Scripting
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public sealed class ConvertPathToFilename : ConvertValueAttribute
    {
        public override string ConvertFrom(object fromVal)
        {
            string str = string.Empty;
            if (fromVal is string)
            {
                string str2 = fromVal as string;
                if (string.IsNullOrEmpty(str2))
                {
                    return str;
                }
                try
                {
                    return Path.GetFileName(str2);
                }
                catch (Exception exception)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("Exception encountered in 'ConvertPathToFilename.ConvertFrom()'{0}with arg '{1}'{0}{2}{0}-- StackTrace --{0}{3}{0}{0}", new object[] { Environment.NewLine, str2, exception.Message, exception.StackTrace });
                    for (Exception exception2 = exception; exception2 != null; exception2 = exception2.InnerException)
                    {
                        builder.AppendFormat("------Exception{0}{1}{0}-- StackTrace --{0}{2}{0}{0}", Environment.NewLine + "  ", exception.Message, exception.StackTrace);
                    }
                    Trace.WriteLine(builder.ToString());
                    return str;
                }
            }
            return str;
        }
    }
}
