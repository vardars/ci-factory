namespace NCover.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    public sealed class KeyValueAttribute : Attribute
    {
        public KeyValueAttribute()
        {
        }

        public KeyValueAttribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public KeyValueAttribute(string key, string subkey, string value)
        {
            this.Key = key;
            this.SubKey = subkey;
            this.Value = value;
        }

        public string Key { get; set; }

        public string SubKey { get; set; }

        public string Value { get; set; }
    }
}
