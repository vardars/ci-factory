namespace System.Xml
{
    using System;
    using System.Runtime.CompilerServices;

    public static class XmlWriterExtensions
    {
        public static void WriteElementString(this XmlWriter writer, string localname, bool value)
        {
            writer.WriteElementString(localname, value.ToString());
        }

        public static void WriteElementString(this XmlWriter writer, string localname, int value)
        {
            writer.WriteElementString(localname, value.ToString());
        }
    }
}
