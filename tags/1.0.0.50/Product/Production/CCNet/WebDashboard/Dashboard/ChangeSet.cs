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
    public class ChangeSet
    {
        public static ChangeSet[] Convert(Modification[] modifications)
        {
            ChangeSetList ChangeTable = new ChangeSetList();

            foreach (Modification modification in modifications)
            {
                ChangeTable.Add(modification.UserName, modification.ChangeNumber.ToString(), modification.Comment, modification.ModifiedTime.ToString(), new Change(modification.Type, modification.FileName, modification.FolderName));
            }

            return ChangeTable.ToArray();
        }

        public ChangeSet(string author, string version, string comment, string modifiedTime)
        {
            _Author = author;
            _Version = version;
            _Comment = comment;
            _ModifiedTime = modifiedTime;
        }

        public ChangeSet(string author, string version, string comment, string modifiedTime, Change changes)
        {
            _Author = author;
            _Version = version;
            _Comment = comment;
            _ModifiedTime = modifiedTime;
            this.AddChange(changes);
        }

        public ChangeSet(string author, string version, string comment, string modifiedTime, Change[] changes)
        {
            _Author = author;
            _Version = version;
            _Comment = comment;
            _ModifiedTime = modifiedTime;
            _Changes = new List<Change>(changes);
        }

        public void AddChange(Change change)
        {
            if (_Changes == null)
                _Changes = new List<Change>();
            this._Changes.Add(change);
        }

        private string _Author;
        private string _Version;
        private string _Comment;
        private string _ModifiedTime;
        private List<Change> _Changes;

        public string ModifiedTime
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
        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                if (_Author == value)
                    return;
                _Author = value;
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
        public Change[] Changes
        {
            get
            {
                if (_Changes == null)
                    _Changes = new List<Change>();
                return _Changes.ToArray();
            }
        }
    }
}
