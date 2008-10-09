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
    [TaskName("targetprocesscreateentity")]
    public class TargetProcessCreateEntityTask : Task
    {
        #region Fields

        private TargetProcessEntity _Entity;
        private ConnectionInformation _ConnectionInformation;

        #endregion

        #region Properties

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

        [TaskAttribute("entity", Required = true)]
        public TargetProcessEntity Entity
        {
            get
            {
                return this._Entity;
            }
            set
            {
                this._Entity = value;
            }
        }
        #endregion

        #region Protected Methods

        public void CreateEntity()
        {
            ServicesCF.ConnectionInformation = this.ConnectionInformation;
            this.Entity.Create(this.ConnectionInformation.RootServiceUrl, this.ConnectionInformation.UserName, this.ConnectionInformation.Password);
        }

        protected override void ExecuteTask()
        {
            this.CreateEntity();
        }

        #endregion

    }
}
