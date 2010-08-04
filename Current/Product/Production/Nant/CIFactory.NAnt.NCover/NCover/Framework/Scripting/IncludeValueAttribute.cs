namespace NCover.Framework.Scripting
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
    public abstract class IncludeValueAttribute : Attribute
    {
        protected IncludeValueAttribute()
        {
        }

        public abstract bool IncludeValue(object value);
    }
}
