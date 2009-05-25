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
    public abstract class TargetProcessUpdateEntityBase : Task
    {
        private ConnectionInformation _ConnectionInformation;


        [BuildElement("connectioninformation", Required = false)]
        public ConnectionInformation ConnectionInformation
        {
            get
            {
                if (_ConnectionInformation == null)
                    _ConnectionInformation = new ConnectionInformation();
                return this._ConnectionInformation;
            }
            set
            {
                this._ConnectionInformation = value;
            }
        }

        public abstract TargetProcessEntity Entity { get; }

        #region Protected Methods

        public void UpdateEntity()
        {
            ServicesCF.ConnectionInformation = this.ConnectionInformation;
            this.Entity.Update();
        }

        protected override void ExecuteTask()
        {
            this.UpdateEntity();
        }

        #endregion
    }
}
