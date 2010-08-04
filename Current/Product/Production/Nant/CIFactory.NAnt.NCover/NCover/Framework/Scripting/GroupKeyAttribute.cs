namespace NCover.Framework.Scripting
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=true), DebuggerDisplay("k:{Key} rk: {RouteKey} g:{GroupName}")]
    public class GroupKeyAttribute : Attribute
    {
        public GroupKeyAttribute()
        {
        }

        public GroupKeyAttribute(int key)
        {
            this.Key = key;
        }

        public string GroupName { get; set; }

        public int Key { get; set; }

        public string RouteKey { get; set; }
    }
}
