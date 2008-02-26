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
    public class ChangeSetList : KeyedCollection<string, ChangeSet>
    {

        protected override string GetKeyForItem(ChangeSet item)
        {
            return item.Version;
        }

        public ChangeSet[] ToArray()
        {
            return ((List<ChangeSet>)this.Items).ToArray();
        }

        public void Add(string author, string version, string comment, string modifiedTime, Change change)
        {
            if (this.Contains(version))
            {
                this[version].AddChange(change);
            }
            else
            {
                this.Add(new ChangeSet(author, version, comment, modifiedTime, change));
            }
        }
    }
}
