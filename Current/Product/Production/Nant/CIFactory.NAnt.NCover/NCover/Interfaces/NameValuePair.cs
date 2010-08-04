namespace NCover.Interfaces
{
    using NCover.Interfaces.Reporting;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [DebuggerDisplay("Name: {Name}, Value {Value}")]
    public sealed class NameValuePair : IEquatable<NameValuePair>, INameValuePair
    {
        public NameValuePair() : this(null, null)
        {
        }

        public NameValuePair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public bool Equals(NameValuePair other)
        {
            if (other == null)
            {
                return false;
            }
            return (string.Equals(this.Name, other.Name) && string.Equals(this.Value, other.Value));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NameValuePair);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ("name:" + ((this.Name == null) ? "null" : this.Name) + " value:" + ((this.Value == null) ? "null" : this.Value));
        }

        [XmlElement("name", IsNullable=false)]
        public string Name { get; set; }

        [XmlElement("value", IsNullable=true)]
        public string Value { get; set; }
    }
}
