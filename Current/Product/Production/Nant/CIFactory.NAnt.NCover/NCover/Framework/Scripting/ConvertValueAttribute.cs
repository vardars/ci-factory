namespace NCover.Framework.Scripting
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class ConvertValueAttribute : Attribute
    {
        public virtual object ConvertBack(string fromVal)
        {
            return fromVal;
        }

        public virtual string ConvertFrom(object fromVal)
        {
            return fromVal.ToString();
        }
    }
}
