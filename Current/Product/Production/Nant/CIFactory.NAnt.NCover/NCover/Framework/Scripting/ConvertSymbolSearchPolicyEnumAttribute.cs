namespace NCover.Framework.Scripting
{
    using NCover.Interfaces.Enumerations;
    using System;

    public sealed class ConvertSymbolSearchPolicyEnumAttribute : ConvertValueAttribute
    {
        private static char[] _splits = new char[] { ',', ';', '|' };

        public ConvertSymbolSearchPolicyEnumAttribute(Type throwaway)
        {
        }

        public override object ConvertBack(string fromVal)
        {
            if (fromVal == null)
            {
                throw new ArgumentNullException("fromVal");
            }
            if (string.IsNullOrEmpty(fromVal))
            {
                throw new ArgumentOutOfRangeException("fromVal", "Expected a non-empty string");
            }
            string[] strArray = fromVal.Split(_splits, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("fromVal", "Unable to find elements of the SymbolSearchPolicy enum in '" + fromVal + "'");
            }
            SymbolSearchPolicy policy = (SymbolSearchPolicy) Enum.Parse(typeof(SymbolSearchPolicy), strArray[0]);
            for (int i = 1; i < strArray.Length; i++)
            {
                policy |= (SymbolSearchPolicy) Enum.Parse(typeof(SymbolSearchPolicy), strArray[i].Trim());
            }
            return policy;
        }

        public override string ConvertFrom(object fromVal)
        {
            if (fromVal == null)
            {
                throw new ArgumentNullException("fromVal");
            }
            return fromVal.ToString();
        }
    }
}
