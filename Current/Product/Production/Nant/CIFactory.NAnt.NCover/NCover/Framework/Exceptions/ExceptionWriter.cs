namespace NCover.Framework.Exceptions
{
    using System;
    using System.Text;

    public static class ExceptionWriter
    {
        public static string GetCompleteInnerStackTrace(Exception ex)
        {
            if (ex == null)
            {
                return "No exception to create a stack trace for.";
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(ex.Message);
            builder.Append(Environment.NewLine);
            builder.Append(ex.StackTrace);
            builder.Append(Environment.NewLine);
            Exception innerException = ex.InnerException;
            int num = 0;
            while (innerException != null)
            {
                builder.AppendFormat("---- Inner Exception [{0}] ----{1}", ++num, Environment.NewLine);
                builder.Append(ex.Message);
                builder.Append(Environment.NewLine);
                builder.Append(ex.StackTrace);
                builder.Append(Environment.NewLine);
                innerException = innerException.InnerException;
            }
            return builder.ToString();
        }
    }
}
