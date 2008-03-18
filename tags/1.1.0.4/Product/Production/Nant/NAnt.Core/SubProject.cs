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
    public class SubProject
    {
        #region Fields

        private string _ProjectName;

        private TargetCollection _Targets;

        #endregion

        #region Constructors

        public SubProject(string projectName)
        {
            _ProjectName = projectName;
        }

        #endregion

        #region Properties

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
