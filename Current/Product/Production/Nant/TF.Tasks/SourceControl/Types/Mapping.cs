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

    [ElementName("mapping")]
    public class Mapping : Element
    {

        #region Fields

        private string _ServerItem;
        private string _LocalItem;
        private WorkingFolderType _MappingType = WorkingFolderType.Map;

        #endregion

        #region Properties

        [TaskAttribute("type")]
        public WorkingFolderType MappingType
        {
            get
            {
                return _MappingType;
            }
            set
            {
                _MappingType = value;
            }
        }

        [TaskAttribute("localitem")]
        public string LocalItem
        {
            get
            {
                return _LocalItem;
            }
            set
            {
                _LocalItem = value;
            }
        }

        [TaskAttribute("serveritem")]
        public string ServerItem
        {
            get
            {
                return _ServerItem;
            }
            set
            {
                _ServerItem = value;
            }
        }

        #endregion

        public WorkingFolder GetWorkingFolder()
        {
            return new WorkingFolder(this.ServerItem, this.LocalItem, this.MappingType);
        }

    }

}
