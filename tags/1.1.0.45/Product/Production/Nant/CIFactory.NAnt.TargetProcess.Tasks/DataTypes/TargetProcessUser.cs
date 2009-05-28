using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using System.Runtime.Serialization;
using CIFactory.TargetProcess.Common.UserWebService;
using CIFactory.TargetProcess.NAnt.Helpers;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("user")]
    public class TargetProcessUser : Element
    {
        #region Fields

        private string _LoginName;

        #endregion

        #region Constructors

        public TargetProcessUser()
        {
        }

        public TargetProcessUser(string loginName)
        {
            this._LoginName = loginName;
        }

        protected TargetProcessUser(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion

        #region Properties

        [TaskAttribute("loginname", Required = true)]
        public string LoginName
        {
            get { return _LoginName; }
            set { _LoginName = value; }
        }
        
        #endregion

        public int GetId()
        {

            UserService userService = ServicesCF.GetService<UserService>();

            string hqlQuery = "select from User as user where user.Login = ?";
            UserDTO[] users = userService.Retrieve(hqlQuery, new object[] { this.LoginName });

            if (users.Length == 0)
                throw new BuildException(string.Format("Could not find a story named: '{0}'.", this.LoginName));

            return users[0].UserID.Value;
        }

    }
}
