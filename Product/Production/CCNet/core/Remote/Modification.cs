using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Value object representing the data associated with a source control modification.
    /// </summary>
    [Serializable]
    [XmlRoot("modification")]
    public class Modification : IComparable
    {
        #region Fields

        private int _ChangeNumber;

        private string _Comment;

        private string _EmailAddress;

        private string _FileName;

        private string _FolderName;

        private DateTime _ModifiedTime;

        private string _Type = "unknown";

        private string _Url;

        private string _UserName;

        private string _Version = "";

        #endregion

        #region Properties

        public int ChangeNumber
        {
            get
            {
                return _ChangeNumber;
            }
            set
            {
                if (_ChangeNumber == value)
                    return;
                _ChangeNumber = value;
            }
        }

        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                if (_Comment == value)
                    return;
                _Comment = value;
            }
        }

        public string EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                _EmailAddress = value;
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName == value)
                    return;
                _FileName = value;
            }
        }

        public string FolderName
        {
            get
            {
                return _FolderName;
            }
            set
            {
                if (_FolderName == value)
                    return;
                _FolderName = value;
            }
        }

        public DateTime ModifiedTime
        {
            get
            {
                return _ModifiedTime;
            }
            set
            {
                if (_ModifiedTime == value)
                    return;
                _ModifiedTime = value;
            }
        }

        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url == value)
                    return;
                _Url = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (_UserName == value)
                    return;
                _UserName = value;
            }
        }

        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                if (_Version == value)
                    return;
                _Version = value;
            }
        }

        #endregion

        #region Public Methods

        public int CompareTo(Object o)
        {
            Modification modification = (Modification)o;
            return ModifiedTime.CompareTo(modification.ModifiedTime);
        }

        public override bool Equals(object obj)
        {
            return ReflectionUtil.ReflectionEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return ReflectionUtil.ReflectionToString(this);
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

            if (!(Version == null || Version.Length == 0)) writer.WriteElementString("version", Version);

            if (Url != null)
            {
                writer.WriteElementString("url", Url);
            }

            if (_EmailAddress != null)
            {
                writer.WriteElementString("email", _EmailAddress);
            }

            writer.WriteEndElement();
        }

        public string ToXml()
        {
            StringWriter writer = new StringWriter();
            ToXml(new XmlTextWriter(writer));
            return writer.ToString();
        }

        #endregion

    }
}