using System;
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
    public enum Child
    {
        Replace,
        Merge
    }

    [TaskName("tfslabel")]
    public class TfsLabelTask : Task
    {

        #region Fields

        private string _LabelName;
        private string _Scope;
        private string _ItemSpec;
        private VersionSpecElement _VersionSpec;
        private string _Comment;
        private Child _Child;
        private bool _Recursive;
        private bool _Delete;
        private TfsServerConnection _ServerConnection;
        
#endregion

        #region Properties

        [BuildElement("versionspec", Required = false)]
        public VersionSpecElement VersionSpec
        {
            get
            {
                if (_VersionSpec == null)
                    _VersionSpec = new VersionSpecElement();
                return _VersionSpec;
            }
            set
            {
                _VersionSpec = value;
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

        [TaskAttribute("labelname", Required = true)]
        public string LabelName
        {
            get
            {
                return _LabelName;
            }
            set
            {
                _LabelName = value;
            }
        }

        [TaskAttribute("scope", Required = false)]
        public string Scope
        {
            get
            {
                return _Scope;
            }
            set
            {
                _Scope = value;
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

        [TaskAttribute("comment", Required = false)]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }

        [TaskAttribute("child", Required = false), BooleanValidator()]
        public Child Child
        {
            get
            {
                return _Child;
            }
            set
            {
                _Child = value;
            }
        }

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

        [TaskAttribute("delete", Required = false), BooleanValidator]
        public bool Delete
        {
            get
            {
                return _Delete;
            }
            set
            {
                _Delete = value;
            }
        }

        #endregion

        #region Methods

        private RecursionType GetRecustiveOption()
        {
            RecursionType TypeOfRecursion = RecursionType.None;

            if (this.Recursive)
                TypeOfRecursion = RecursionType.Full;
            return TypeOfRecursion;
        }

        protected override void ExecuteTask()
        {
            VersionControlLabel label = new VersionControlLabel(this.ServerConnection.SourceControl, this.LabelName, this.ServerConnection.SourceControl.AuthenticatedUser, this.Scope, this.Comment);
            RecursionType TypeOfRecursion = GetRecustiveOption();
            
            ItemSpec itemSpec = new ItemSpec(this.ItemSpec, TypeOfRecursion);
            LabelItemSpec[] labelItemSpec = new LabelItemSpec[] {  
                    new LabelItemSpec(itemSpec, this.VersionSpec.GetVersionSpec(), false)
                };

            this.ServerConnection.SourceControl.CreateLabel(label, labelItemSpec, LabelChildOption.Replace);
        }

        #endregion

    }
}
