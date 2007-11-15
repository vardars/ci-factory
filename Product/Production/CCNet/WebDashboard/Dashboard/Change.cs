using System.Collections;
using System.Drawing;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class Change
    {
        /// <summary>
        /// Initializes a new instance of the Change class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        public Change(string type, string file, string path)
        {
            _Type = type;
            _File = file;
            _Path = path;
        }

        private string _Type;
        private string _File;
        private string _Path;
        public string File
        {
            get
            {
                return _File;
            }
            set
            {
                _File = value;
            }
        }
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
            }
        }
    }
}
