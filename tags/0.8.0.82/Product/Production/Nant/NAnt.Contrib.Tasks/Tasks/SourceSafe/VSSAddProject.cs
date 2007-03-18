using System;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Contrib.Tasks.SourceSafe;

namespace NAnt.Contrib.Tasks.SourceSafe
{
    [TaskName("vssaddproject")]
    public class VSSAddProject : BaseTask
    {
        #region Fields

        private string _Comment;

        private string _ProjectName;

        #endregion

        #region Properties

        [TaskAttributeAttribute("comment", Required = false)]
        public string Comment
        {
            get { return _Comment; }
            set { _Comment = value; }
        }

        [TaskAttributeAttribute("project", Required = true)]
        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        #endregion

        #region Public Methods

        public void test()
        {
            try
            {
                this.Comment = "Testing";
                this.DBPath = new System.IO.FileInfo("C:\\Source Safe DataBases\\Test\\srcsafe.ini");
                this.Password = "password";
                this.UserName = "build";
                this.Path = "$/";
                this.ProjectName = "Test";
                this.ExecuteTask();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            base.Open();
            base.Item.NewSubproject(this.ProjectName, this.Comment);
        }

        #endregion

    }
}