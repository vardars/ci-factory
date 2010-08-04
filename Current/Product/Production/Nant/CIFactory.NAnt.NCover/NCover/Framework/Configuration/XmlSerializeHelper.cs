namespace NCover.Framework.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    internal static class XmlSerializeHelper
    {
        internal static object Deserialize(string content, Type type)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentOutOfRangeException("content", "Cannot be null or empty.");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
        }

        internal static bool MoreContent(XmlReader reader, string elementName)
        {
            if (reader.EOF)
            {
                return false;
            }
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                return !(reader.LocalName == elementName);
            }
            return true;
        }

        internal static IList<T> ReadArrayOfElement<T>(XmlReader reader, string arrayItemElementName, Func<XmlReader, T> deserializeArrayItem)
        {
            if (string.IsNullOrEmpty(arrayItemElementName))
            {
                throw new ArgumentOutOfRangeException("arrayItemElementName", "Cannot be null or empty.");
            }
            if (deserializeArrayItem == null)
            {
                throw new ArgumentNullException("deserializeArrayItem");
            }
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return new List<T>();
            }
            string localName = reader.LocalName;
            reader.ReadStartElement();
            List<T> list = new List<T>();
            while (!reader.EOF && !reader.LocalName.Equals(localName, StringComparison.Ordinal))
            {
                if (!reader.LocalName.Equals(arrayItemElementName))
                {
                    if (string.IsNullOrEmpty(reader.LocalName))
                    {
                        throw new InvalidOperationException("Cannot deserialize a child element of <" + arrayItemElementName + " /> that has no element name.");
                    }
                    throw new InvalidOperationException("Cannot deserialize the <" + reader.LocalName + " /> child element of <" + arrayItemElementName + " />");
                }
                T item = deserializeArrayItem(reader);
                list.Add(item);
            }
            if (!reader.EOF)
            {
                reader.ReadEndElement();
            }
            return list;
        }

        internal static IList<string> ReadArrayOfStrings(XmlReader reader, string arrayItemElementName)
        {
            return ReadArrayOfElement<string>(reader, arrayItemElementName, new Func<XmlReader, string>(XmlSerializeHelper.ReadStringElement));
        }

        internal static bool ReadBooleanElement(XmlReader reader)
        {
            string left = ReadStringElement(reader);
            if (left == null)
            {
                return false;
            }
            return left.IgnoreCaseEq(bool.TrueString);
        }

        internal static T ReadEnumElement<T>(XmlReader reader) where T: struct
        {
            string str = ReadStringElement(reader);
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }
            T local = default(T);
            try
            {
                return (T) Enum.Parse(typeof(T), str, true);
            }
            catch (ArgumentException)
            {
                if (typeof(T).IsEnum)
                {
                    return TryParseFlagsEnum<T>(str);
                }
                return default(T);
            }
            catch (OverflowException)
            {
                return default(T);
            }
        }

        internal static object ReadEnumElement(XmlReader reader, Type enumType)
        {
            object obj2 = null;
            try
            {
                obj2 = Enum.GetValues(enumType).GetValue(0);
            }
            catch
            {
                obj2 = null;
            }
            string str = ReadStringElement(reader);
            if (string.IsNullOrEmpty(str))
            {
                return obj2;
            }
            object obj3 = obj2;
            try
            {
                return Enum.Parse(enumType, str, true);
            }
            catch (ArgumentException)
            {
                return obj2;
            }
            catch (OverflowException)
            {
                return obj2;
            }
        }

        internal static float ReadFloatAttribute(string value)
        {
            float result = 0f;
            float.TryParse(value, out result);
            return result;
        }

        internal static float ReadFloatElement(XmlReader reader)
        {
            string s = ReadStringElement(reader);
            float result = 0f;
            float.TryParse(s, out result);
            return result;
        }

        internal static int ReadIntElement(XmlReader reader)
        {
            string s = ReadStringElement(reader);
            int result = 0;
            int.TryParse(s, out result);
            return result;
        }

        internal static string ReadStringElement(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return string.Empty;
            }
            return reader.ReadElementContentAsString();
        }

        internal static string Serialize(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            XmlWriterSettings settings = new XmlWriterSettings {
                CheckCharacters = true,
                CloseOutput = false,
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineOnAttributes = false
            };
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringBuilder output = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                new XmlSerializer(value.GetType()).Serialize(writer, value, namespaces);
            }
            return output.ToString();
        }

        private static T TryParseFlagsEnum<T>(string serialized) where T: struct
        {
            try
            {
                string str = serialized.Replace(' ', ',');
                return (T) Enum.Parse(typeof(T), str, true);
            }
            catch (ArgumentException)
            {
                return default(T);
            }
            catch (OverflowException)
            {
                return default(T);
            }
        }

        internal static void WriteArrayOfElement<T>(XmlWriter writer, string elementname, IList<T> elements, Action<XmlWriter, T> writeHelper)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (string.IsNullOrEmpty(elementname))
            {
                throw new ArgumentOutOfRangeException("elementname", "Cannot be null or empty.");
            }
            if (writeHelper == null)
            {
                throw new ArgumentNullException("writeHelper");
            }
            if (elements.NotEmpty<T>())
            {
                writer.WriteStartElement(elementname);
                for (int i = 0; i < elements.Count; i++)
                {
                    writeHelper(writer, elements[i]);
                }
                writer.WriteEndElement();
            }
        }

        internal static void WriteArrayOfStrings(XmlWriter writer, string arrayName, string arrayItemName, IList<string> arrayItems)
        {
            WriteArrayOfElement<string>(writer, arrayName, arrayItems, delegate (XmlWriter wrt, string itemValue) {
                wrt.WriteElementString(arrayItemName, itemValue);
            });
        }

        internal static void WriteChildElement(XmlWriter writer, string elementName, object childElement)
        {
            if (childElement != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings {
                    CheckCharacters = true,
                    CloseOutput = true,
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                StringBuilder output = new StringBuilder(100);
                using (XmlWriter.Create(output, settings))
                {
                    if (string.IsNullOrEmpty(elementName))
                    {
                        new XmlSerializer(childElement.GetType()).Serialize(writer, childElement, namespaces);
                    }
                    else
                    {
                        XmlRootAttribute root = new XmlRootAttribute(elementName);
                        new XmlSerializer(childElement.GetType(), root).Serialize(writer, childElement, namespaces);
                    }
                }
                writer.WriteRaw(output.ToString());
            }
        }
    }
}
