using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Text;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Value object representing the data associated with a source control modification.
	/// </summary>
    [Serializable]
	[XmlRoot("modification")]
	public class Modification : IComparable
	{
		public string Type = "unknown";
		public string FileName;
		public string FolderName;
		public DateTime ModifiedTime;
		public string UserName;
		public int ChangeNumber;
		public string Version = "";
		public string Comment;
		public string Url;
		public string EmailAddress;

		public string ToXml()
		{
			StringWriter writer = new StringWriter();
			ToXml(new XmlTextWriter(writer));
			return writer.ToString();
		}

		public void ToXml(XmlWriter writer)
		{
			writer.WriteStartElement("modification");
			writer.WriteAttributeString("type", Type);
			writer.WriteElementString("filename", FileName);
			writer.WriteElementString("project", FolderName);
			writer.WriteElementString("date", ModifiedTime.ToString("yyyy-MM-dd HH:mm:ss"));
			writer.WriteElementString("user", UserName);
			writer.WriteElementString("comment", Comment);
			writer.WriteElementString("changeNumber", ChangeNumber.ToString());

			if (! (Version == null || Version.Length == 0)) writer.WriteElementString("version", Version);

            if (Url != null)
            {
                writer.WriteElementString("url", Url);
            }

            if (EmailAddress != null)
            {
                writer.WriteElementString("email", EmailAddress);
            }

			writer.WriteEndElement();
		}

		public int CompareTo(Object o)
		{
			Modification modification = (Modification) o;
			return ModifiedTime.CompareTo(modification.ModifiedTime);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return ReflectionUtil.ReflectionEquals(this, obj);
		}

		public override string ToString()
		{
			return ReflectionUtil.ReflectionToString(this);
		}

        private class ReflectionUtil
        {
            /// <summary>
            /// Utility class is not intended for instantiation.
            /// </summary>
            private ReflectionUtil() { }

            /// <summary>
            /// Gets a value indicating whether the types, fields and properties of the
            /// specified objects are equal.
            /// </summary>
            /// <param name="o1"></param>
            /// <param name="o2"></param>
            /// <returns></returns>
            public static bool ReflectionEquals(object o1, object o2)
            {
                return ValidateTypes(o1, o2) && ValidateFields(o1, o2) && ValidateProperties(o1, o2);
            }

            #region Private helper methods

            private static bool ValidateTypes(object o1, object o2)
            {
                return o2 != null && o1.GetType() == o2.GetType();
            }

            private static bool ValidateFields(object o1, object o2)
            {
                foreach (FieldInfo field in o1.GetType().GetFields())
                {
                    if (field.FieldType.IsArray)
                    {
                        return ValidateArrays((object[])field.GetValue(o1), (object[])field.GetValue(o2));
                    }
                    if (IsNotEqual(field.GetValue(o1), field.GetValue(o2)))
                    {
                        return false;
                    }
                }
                return true;
            }

            private static bool ValidateProperties(object o1, object o2)
            {
                foreach (PropertyInfo property in o1.GetType().GetProperties())
                {
                    if (property.PropertyType.IsArray)
                    {
                        return ValidateArrays((object[])property.GetValue(o1, null), (object[])property.GetValue(o2, null));
                    }
                    if (IsNotEqual(property.GetValue(o1, null), property.GetValue(o2, null)))
                    {
                        return false;
                    }
                }
                return true;
            }

            private static bool ValidateArrays(object[] o1, object[] o2)
            {
                if (o1 == null && o2 == null) return true;
                if (o1 == null || o2 == null) return false;
                if (o1.Length != o2.Length) return false;

                for (int i = 0; i < o1.Length; i++)
                {
                    if (IsNotEqual(o1[i], o2[i])) return false;
                }
                return true;
            }

            private static bool IsNotEqual(Object o1, Object o2)
            {
                return !IsEqual(o1, o2);
            }

            private static bool IsEqual(Object o1, Object o2)
            {
                return (o1 == null) ? o2 == null : o1.Equals(o2);
            }

            #endregion

            /// <summary>
            /// Uses reflection to compile a string representation of an object, by
            /// querying its type, field names/values, and property names/values.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static string ReflectionToString(object obj)
            {
                Type type = obj.GetType();
                StringBuilder buffer = new StringBuilder(type.Name);
                buffer.Append(": (");

                int count = 0;
                foreach (FieldInfo info in type.GetFields())
                {
                    if (count++ > 0) buffer.Append(",");
                    buffer.Append(info.Name).Append("=");
                    buffer.Append(info.GetValue(obj));
                }

                foreach (PropertyInfo info in type.GetProperties())
                {
                    if (count++ > 0) buffer.Append(",");
                    buffer.Append(info.Name).Append("=");
                    buffer.Append(info.GetValue(obj, null));
                }
                buffer.Append(")");
                return buffer.ToString();
            }
        }
	}
}