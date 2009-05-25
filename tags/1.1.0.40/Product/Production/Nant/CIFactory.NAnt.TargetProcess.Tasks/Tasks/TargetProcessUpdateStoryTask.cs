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
    [TaskName("targetprocessupdatestory")]
    public class TargetProcessUpdateStoryTask : TargetProcessUpdateEntityBase
    {
        #region Fields

        private TargetProcessUserStory _StoryEntity;

        #endregion

        #region Properties

        public override TargetProcessEntity Entity
        {
            get { return this.StoryEntity; }
        }

        [BuildElement("story", Required = true)]
        public TargetProcessUserStory StoryEntity
        {
            get
            {
                return this._StoryEntity;
            }
            set
            {
                this._StoryEntity = value;
            }
        }

        #endregion
    }
}
