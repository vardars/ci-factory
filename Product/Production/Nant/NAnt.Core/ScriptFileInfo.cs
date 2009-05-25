using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using NAnt.Core.Tasks;
using NAnt.Core.Util;

namespace NAnt.Core
{
    public class ScriptFileInfo
    {
        #region Fields

        private XmlDocument _Document;
        private String _FilePath = null;
        private string _ProjectName;
        private TargetCollection _Targets;

        #endregion

        #region Constructors

        public ScriptFileInfo(string projectName, XmlDocument doc)
        {
            _ProjectName = projectName;
            _Document = doc;
        }

        #endregion

        #region Properties


        public XmlDocument Document
        {
            get { return _Document; }
            set { _Document = value; }
        }

        public String FilePath
        {
            get
            {
                if (_FilePath == null)
                {
                    Uri uri = new Uri(this.Document.BaseURI);
                    _FilePath = uri.LocalPath;
                }
                return _FilePath;
            }
        }

        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }

        public TargetCollection Targets
        {
            get
            {
                if (_Targets == null)
                    _Targets = new TargetCollection();
                return _Targets;
            }
            set
            {
                _Targets = value;
            }
        }

        #endregion

    }
}
