using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [TaskName("trackerquery")]
    public class TrackerQuery : BaseTrackerTask
    {
        private const string ID_LIST_DELIMITER = ",";

        // Methods
        public TrackerQuery()
        {
        }

        protected override void ExecuteTask()
        {
            ExecuteQuery();
        }

        public void ExecuteQuery()
        {
            if ((this.ScrIdsProperty == null) && (this.ScrCountProperty == null))
            {
                throw new BuildException("\"trackerquery\" was called, but no output was specified. Should you have specified: scridsproperty or scrcountproperty?");
            }
            this.Login();
            int[] IDs = this.TrackerServer.GetSCRIDListFromQuery(this.Query);
            if (this.ScrCountProperty != null)
            {
                if (IDs != null)
                {
                    this.Properties[this.ScrCountProperty] = IDs.Length.ToString();
                }
                else
                {
                    this.Properties[this.ScrCountProperty] = "0";
                }
            }
            if (this.ScrIdsProperty != null)
            {
                if (IDs != null)
                {
                    this.Properties[this.ScrIdsProperty] = this.FormatIdList(IDs);
                }
                else
                {
                    this.Properties[this.ScrIdsProperty] = "";
                }
            }
            this.Logout();
        }

        public string FormatIdList(int[] IDs)
        {
            if (IDs.Length == 0)
                return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.Append(IDs[0]);
            for (int i = 1; i < IDs.Length; ++i)
            {
                builder.Append(ID_LIST_DELIMITER);
                builder.Append(IDs[i]);
            }
            return builder.ToString();
         }

        internal void TestFormatIdList()
        {
            try
            {
                this.FormatIdList(new int[0]);
            }
            catch (Exception ex)
            {
                string stop = ex.Message;
            }
        }

        internal void Test()
        {
            this.Query = "For Me";
            this.ConnectionInformation = new ConnectionInformation();
            ConnectionInformation information = this.ConnectionInformation;
            information.DBMSLoginMode = 2;
            information.DBMSServer = "Jupiter";
            information.DBMSType = "Tracker SQL Server Sys";
            information.DBMSPassword = "tracker12";
            information.DBMSUserName = "tracker";
            information.ProjectName = "EF";
            information.UserName = "jflowers";
            information.UserPWD = "password";
            try
            {
                this.ExecuteTask();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
        }


        // Properties
        [TaskAttribute("query", Required=true)]
        public string Query
        {
            get
            {
                return this._Query;
            }
            set
            {
                this._Query = value;
            }
        }

        [TaskAttribute("countproperty", Required=false)]
        public string ScrCountProperty
        {
            get
            {
                return this._ScrCountProperty;
            }
            set
            {
                this._ScrCountProperty = value;
            }
        }

        [TaskAttribute("idsproperty", Required=false)]
        public string ScrIdsProperty
        {
            get
            {
                return this._ScrIdsProperty;
            }
            set
            {
                this._ScrIdsProperty = value;
            }
        }


        // Fields
        private string _Query;
        private string _ScrCountProperty;
        private string _ScrIdsProperty;
    }
}