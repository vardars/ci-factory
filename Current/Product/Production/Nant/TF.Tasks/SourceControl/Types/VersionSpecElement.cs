using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

namespace TF.Tasks.SourceControl.Types
{
    
    public enum VersionSpecType
    {
        Changeset,
        Date,
        Label,
        Workspace,
        Latest,
        Null
    }

    [ElementName("versionspec")]
    public class VersionSpecElement : DataTypeBase
    {

        #region Fields

        private string _VersionSpecString;
        private VersionSpecType _VersionSpecType = VersionSpecType.Latest;

        #endregion

        #region Properties

        [TaskAttribute("type", Required = false)]
        public VersionSpecType VersionSpecType
        {
            get
            {
                return _VersionSpecType;
            }
            set
            {
                _VersionSpecType = value;
            }
        }

        [TaskAttribute("versionspec", Required = false)]
        public string VersionSpecString
        {
            get
            {
                return _VersionSpecString;
            }
            set
            {
                _VersionSpecString = value;
            }
        }

        #endregion

        public VersionSpec GetVersionSpec()
        {
            switch (this.VersionSpecType)
            {
                case VersionSpecType.Changeset:
                    return new ChangesetVersionSpec(this.VersionSpecString);
                case VersionSpecType.Date:
                    return new DateVersionSpec(DateTime.Parse(this.VersionSpecString));
                case VersionSpecType.Label:
                    String Label;
                    String Scope;
                    LabelSpec.Parse(this.VersionSpecString, null, false, out Label, out Scope);
                    return new LabelVersionSpec(Label, Scope);
                case VersionSpecType.Latest:
                    return VersionSpec.Latest;
            }
            return null;
        }
    }

}
