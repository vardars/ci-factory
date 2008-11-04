using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using System.Xml;
using System.Text.RegularExpressions;
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.NAnt.DataTypes;

namespace CIFactory.TargetProcess.NAnt.Tasks
{
    [TaskName("targetprocesscreatetask")]
    public class TargetProcessCreateTaskTask : TargetProcessCreateEntityBase
    {

        #region Fields

        private TargetProcessTask _TaskEntity;

        #endregion

        #region Properties

        public override TargetProcessEntity Entity
        {
            get { return this.TaskEntity; }
        }

        [BuildElement("task", Required = true)]
        public TargetProcessTask TaskEntity
        {
            get
            {
                return this._TaskEntity;
            }
            set
            {
                this._TaskEntity = value;
            }
        }

        #endregion

    }
}
