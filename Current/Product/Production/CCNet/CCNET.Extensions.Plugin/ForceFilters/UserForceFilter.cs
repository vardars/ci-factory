using System;
using System.Collections.Specialized;
using System.DirectoryServices;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.ForceFilters.UserFilter;

namespace CCNET.Extensions.ForceFilters
{
    [ReflectorType("userForceFilter")]
    public class UserForceFilter : IForceFilter
    {
        
#region Fields

        private string _DomainName;
        private string _TopLevelDomainName = "com";
        private string[] _Users = new string[0];
        private string[] _Groups = new string[0];
        private StringCollection _UserList;
        private string _UserName;
        private string _Password;
        private bool _DontCheck = false ;

#endregion
        
#region Properties

        [ReflectorProperty("dontCheck", Required = false)]
        public bool DontCheck
        {
            get
            {
                return _DontCheck;
            }
            set
            {
                _DontCheck = value;
            }
        }

        [ReflectorProperty("password", Required = true)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        [ReflectorProperty("userName", Required = true)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        private StringCollection UserList
        {
            get
            {
                if (_UserList == null)
                {
                    _UserList = new StringCollection();
                    _UserList.AddRange(this.Users);
                }
                return _UserList;
            }
            set
            {
                _UserList = value;
            }
        }

        [ReflectorArray("includedGroups", Required = false)]
        public string[] Groups
        {
            get
            {
                return _Groups;
            }
            set
            {
                _Groups = value;
            }
        }

        [ReflectorArray("includedUsers", Required = false)]
        public string[] Users
        {
            get
            {
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }

        [ReflectorProperty("domainName", Required = true)]
        public string DomainName
        {
            get
            {
                return _DomainName;
            }
            set
            {
                _DomainName = value;
            }
        }

        [ReflectorProperty("topLevelDomainName", Required = false)]
        public string TopLevelDomainName
        {
            get
            {
                return _TopLevelDomainName;
            }
            set
            {
                _TopLevelDomainName = value;
            }
        }


#endregion

#region IForceFilter Members

        public ForceFilterClientInfo GetClientInfo()
        {
            System.Security.Principal.IIdentity Identity;
            Identity = System.Threading.Thread.CurrentPrincipal.Identity;

            if (Identity.Name == null || Identity.Name == string.Empty)
                Identity = System.Security.Principal.WindowsIdentity.GetCurrent();

            UserInformation info = new UserInformation();
            info.Name = this.ExtractUserName(Identity.Name);

            if (this.Groups.Length != 0)
                info.Groups = this.GetADUserGroups(info.Name);
            
            return info;
        }

        public bool RequiresClientInformation
        {
            get { return true; }
        }

        public bool ShouldRunIntegration(ForceFilterClientInfo[] clientInfo, IIntegrationResult result)
        {
            UserInformation UserInfo = null;
            foreach (ForceFilterClientInfo Info in clientInfo)
            {
                if (Info is UserInformation)
                {
                    UserInfo = (UserInformation)Info;
                    break;
                }
            }
            
            if (UserInfo == null)
                throw new InvalidOperationException("No user information was found.");

            result.AddIntegrationProperty("CCNetForcedBy", UserInfo.Name);
            this.AddUserNameToResults(result, UserInfo);

            if (this.DontCheck)
                return true;

            bool ToRun = false;

            if (this.UserList.Contains(UserInfo.Name))
            {
                return true;
            }

            foreach (string GroupName in this.Groups)
            {
                if (UserInfo.Groups.Contains(GroupName))
                {
                    return true;
                }
            }

            Log.Info(string.Format("{0} is not allowed to force the build for project {1}.", UserInfo.Name, result.ProjectName));
            return ToRun;
        }

#endregion
        
#region Helpers

        private void AddUserNameToResults(IIntegrationResult result, UserInformation UserInfo)
        {
            result.AddTaskResult(string.Format("<ForcedBuildInformation UserName=\"{0}\" />", UserInfo.Name));
        }
        private StringCollection GetADUserGroups(string userName)
        {
            StringCollection groupsList = new StringCollection();

            string Path = string.Format("LDAP://dc={0},dc={1}", DomainName, TopLevelDomainName);
            string DomainUser = string.Format("{0}\\{1}", this.DomainName, this.UserName);
            DirectoryEntry Directory = new DirectoryEntry(Path, DomainUser, this.Password);
            DirectorySearcher search = new DirectorySearcher(Directory);
            search.Filter = String.Format("(SAMAccountName={0})", userName);
            SearchResult result = search.FindOne();
            
            if (result != null && result.Properties != null && result.Properties.Contains("memberOf"))
            {
                int groupCount = result.Properties["memberOf"].Count;
                for (int counter = 0; counter < groupCount; counter++)
                {
                    string Dirty = (string)result.Properties["memberOf"][counter];
                    Dirty = Dirty.Split(new char[] { ',' })[0];
                    string Clean = Dirty.Split(new char[] { '=' })[1];
                    groupsList.Add(Clean);
                }
            }
            
            return groupsList;
        }

        private string ExtractUserName(string path)
        {
            string[] userPath = path.Split(new char[] { '\\' });
            return userPath[userPath.Length - 1];
        }

#endregion

        public void Test()
        {
            this.UserName = "";
            this.Password = "";
            this.DomainName = "";
            ForceFilterClientInfo info = this.GetClientInfo();
            if (((UserInformation)info).Name != "")
                throw new Exception();
        }
    }
}
