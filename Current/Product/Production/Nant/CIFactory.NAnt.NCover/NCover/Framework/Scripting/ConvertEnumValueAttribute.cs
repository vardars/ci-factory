namespace NCover.Framework.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ConvertEnumValueAttribute : ConvertValueAttribute
    {
        public ConvertEnumValueAttribute(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ConvertBack(string fromVal)
        {
            object obj2 = null;
            try
            {
                obj2 = Enum.Parse(this.EnumType, fromVal);
            }
            catch
            {
            }
            return obj2;
        }

        public override string ConvertFrom(object fromVal)
        {
            return fromVal.ToString();
        }

        public Type EnumType { get; private set; }
    }
}
