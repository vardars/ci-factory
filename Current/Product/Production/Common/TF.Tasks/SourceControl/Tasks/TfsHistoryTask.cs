using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

using TF.Tasks.SourceControl.Types;

namespace TF.Tasks.SourceControl.Tasks
{
    [TaskName("tfshistory")]
    public class TfsHistoryTask : Task
    {

        #region Fields

        private string _ItemSpec;
        private VersionSpecElement _FromVersionSpec;
        private VersionSpecElement _ToVersionSpec;
        private TfsServerConnection _ServerConnection;
        private string _ReportFile;
        private bool _Recursive;

        #endregion

        #region Properties

        [TaskAttribute("recursive", Required = false), BooleanValidator]
        public bool Recursive
        {
            get
            {
                return _Recursive;
            }
            set
            {
                _Recursive = value;
            }
        }

        [TaskAttribute("reportfile", Required = true)]
        public string ReportFile
        {
            get
            {
                return _ReportFile;
            }
            set
            {
                _ReportFile = value;
            }
        }

        [BuildElement("fromversionspec", Required = false)]
        public VersionSpecElement FromVersionSpec
        {
            get
            {
                if (_FromVersionSpec == null)
                    _FromVersionSpec = new VersionSpecElement();
                return _FromVersionSpec;
            }
            set
            {
                _FromVersionSpec = value;
            }
        }

        [TaskAttribute("itemspec", Required = true)]
        public string ItemSpec
        {
            get
            {
                return _ItemSpec;
            }
            set
            {
                _ItemSpec = value;
            }
        }

        [BuildElement("toversionspec", Required = false)]
        public VersionSpecElement ToVersionSpec
        {
            get
            {
                if (_ToVersionSpec == null)
                    _ToVersionSpec = new VersionSpecElement();
                return _ToVersionSpec;
            }
            set
            {
                _ToVersionSpec = value;
            }
        }

        [BuildElement("tfsserverconnection", Required = true)]
        public TfsServerConnection ServerConnection
        {
            get
            {
                return _ServerConnection;
            }
            set
            {
                _ServerConnection = value;
            }
        }

        #endregion

        private RecursionType GetRecustiveOption()
        {
            RecursionType TypeOfRecursion = RecursionType.None;

            if (this.Recursive)
                TypeOfRecursion = RecursionType.Full;
            Log(Level.Verbose, "Type of Recursion is {0}", TypeOfRecursion.ToString());
            return TypeOfRecursion;
        }

        private VersionSpec GetFromVersion()
        {
            VersionSpec Spec = this.FromVersionSpec.GetVersionSpec();
            this.FixUpVersionSpec(ref Spec);
            return Spec;
        }

        private VersionSpec GetToVersion()
        {
            VersionSpec Spec = this.ToVersionSpec.GetVersionSpec();
            this.FixUpVersionSpec(ref Spec);
            return Spec;
        }

        public void FixUpVersionSpec(ref VersionSpec spec)
        {
            if (spec is LabelVersionSpec)
            {
                spec = this.ConvertToChangesetSpec((LabelVersionSpec)spec);
            }
        }

        public ChangesetVersionSpec ConvertToChangesetSpec(LabelVersionSpec spec)
        {
            VersionControlLabel Label = this.ServerConnection.SourceControl.QueryLabels(spec.Label, spec.Scope, null, true)[0];
            int HighestChangesetId = 0;

            foreach (Item item in Label.Items)
            {
                if (HighestChangesetId < item.ChangesetId)
                {
                    HighestChangesetId = item.ChangesetId;
                }
            }
            return new ChangesetVersionSpec(HighestChangesetId);
        }

        protected override void ExecuteTask()
        {
            RecursionType TypeOfRecursion = this.GetRecustiveOption();
            VersionSpec FromVersion = this.GetFromVersion();
            VersionSpec Version = FromVersion;
            if (Version == null)
                Version = VersionSpec.Latest;
            VersionSpec ToVersion = this.GetToVersion();

            using (XmlTextWriter DocWriter = new XmlTextWriter(this.ReportFile, System.Text.Encoding.UTF8))
            {
                DocWriter.Formatting = Formatting.Indented;
                DocWriter.WriteStartDocument();
                XmlWriterHelper Write = new XmlWriterHelper(DocWriter);
                Write.ElementBegining("History");

                IEnumerable ChangesetList =
                    this.ServerConnection.SourceControl.QueryHistory(
                        this.ItemSpec,
                        Version,
                        0,
                        TypeOfRecursion,
                        null,
                        FromVersion,
                        ToVersion,
                        int.MaxValue,
                        true,
                        false);

                foreach (Changeset CurrentChangeset in ChangesetList)
                {
                    Write.ElementBegining("ChangeSet");
                    Write.Attribute("ChangesetId", CurrentChangeset.ChangesetId.ToString());
                    Write.Attribute("Committer", CurrentChangeset.Committer);
                    Write.Attribute("Comment", CurrentChangeset.Comment);
                    Write.Attribute("CreationDate", CurrentChangeset.CreationDate.ToString());

                    Log(Level.Verbose, CurrentChangeset.ToString());

                    foreach (Change CurrentChange in CurrentChangeset.Changes)
                    {
                        Write.ElementBegining("Change");
                        Write.Attribute("ChangeType", CurrentChange.ChangeType.ToString());
                        Write.Attribute("CheckinDate", CurrentChange.Item.CheckinDate.ToString());
                        Write.Attribute("ItemType", CurrentChange.Item.ItemType.ToString());
                        Write.Attribute("ArtifactUri", CurrentChange.Item.ArtifactUri.ToString());
                        Write.Attribute("ServerItem", CurrentChange.Item.ServerItem);
                        Write.ElementEnd();
                    }
                    Write.ElementEnd();
                }
                Write.ElementEnd();
            }
        }
    }
}