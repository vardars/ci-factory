using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [TaskName("trackeraddnote")]
    public class TrackerAddNote : BaseTrackerTask
    {
        // Methods
        public TrackerAddNote()
        {
        }

        protected override void ExecuteTask()
        {
            this.Login();
            this.TrackerServer.AddNote(this.Id, this.Title, this.Text);
            this.Logout();
        }

        public void Test()
        {
            this.Id = 0x389;
            this.Title = "nAnt Test";
            this.Text = "This is a test of the new tracker nant task.";
            this.ConnectionInformation = new ConnectionInformation();
            ConnectionInformation information1 = this.ConnectionInformation;
            information1.DBMSLoginMode = 2;
            information1.DBMSServer = "Jupiter";
            information1.DBMSType = "Tracker SQL Server Sys";
            information1.DBMSPassword = "tracker12";
            information1.DBMSUserName = "tracker";
            information1.ProjectName = "EF";
            information1.UserName = "jflowers";
            information1.UserPWD = "password";
            information1 = null;
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
        [TaskAttribute("scrid", Required=true)]
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this._Id = value;
            }
        }

        [TaskAttribute("text", Required=true)]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
            }
        }

        [TaskAttribute("title", Required=true)]
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }


        // Fields
        private int _Id;
        private string _Text;
        private string _Title;
    }
}