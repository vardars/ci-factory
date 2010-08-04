namespace System
{
    using System.Runtime.CompilerServices;

    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string left, string substring)
        {
            return (object.ReferenceEquals(left, substring) || (((left != null) && (substring != null)) && (left.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1)));
        }

        public static bool CultureAgnosticEquals(this string left, string right)
        {
            return left.CultureAgnosticEquals(right, false);
        }

        public static bool CultureAgnosticEquals(this string left, string right, bool ignoreCase)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            if ((left == null) || (right == null))
            {
                return false;
            }
            StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return left.Equals(right, comparisonType);
        }

        public static bool IgnoreCaseEq(this string left, string right)
        {
            return left.CultureAgnosticEquals(right, true);
        }

        public static string TrimEnd(this string left, string toTrim)
        {
            if (((!string.IsNullOrEmpty(left) && !string.IsNullOrEmpty(toTrim)) && (toTrim.Length <= left.Length)) && left.EndsWith(toTrim, StringComparison.OrdinalIgnoreCase))
            {
                return left.Substring(0, left.Length - toTrim.Length);
            }
            return left;
        }
    }
}
