namespace NCover.Interfaces.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class ParseExtensions
    {
        public const float WITHIN_EPSILON = 0.001f;

        public static bool CloseTo(this double original, double other)
        {
            return original.Within(0.0010000000474974513, other);
        }

        public static bool CloseTo(this float original, float other)
        {
            return original.Within(0.001f, other);
        }

        public static bool TryParseEnum<T>(object value, out T result) where T: struct
        {
            return TryParseEnum<T>(value as string, out result);
        }

        public static bool TryParseEnum<T>(string value, out T result) where T: struct
        {
            result = default(T);
            try
            {
                result = (T) Enum.Parse(typeof(T), value);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool Within(this double original, double epsilon, double other)
        {
            return (((original - epsilon) < other) && (other < (original + epsilon)));
        }

        public static bool Within(this float original, float epsilon, float other)
        {
            return (((original - epsilon) < other) && (other < (original + epsilon)));
        }
    }
}
