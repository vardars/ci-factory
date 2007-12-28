using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using Softec;
using Softec.SubversionSharp;
using Softec.AprSharp;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("svncleantree")]
    public class SvnCleanTree : Task
    {
        #region Fields

        private SvnClient _Client;

        private string _Directory;

        private bool _Recurse;

        #endregion

        #region Constructors

        public SvnCleanTree()
        {
        }

        #endregion

        #region Properties

        public SvnClient Client
        {
            get
            {
                return _Client;
            }
            set
            {
                _Client = value;
            }
        }

        [TaskAttribute("directory", Required = true)]
        public string Directory
        {
            get
            {
                return _Directory;
            }
            set
            {
                _Directory = value;
            }
        }

        [TaskAttribute("recursive"), BooleanValidator()]
        public bool Recurse
        {
            get
            {
                return _Recurse;
            }
            set
            {
                _Recurse = value;
            }
        }

        #endregion

        #region Public Methods

        public void AddHocTest()
        {
            this.Directory = @"C:\Projects\CI Factory\Current\Third Party\CCNET";
            this.ExecuteTask();
        }

        public void Clean(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            if (status == null) throw new ArgumentNullException("status");

            if (status.TextStatus == SvnWcStatus.Kind.Unversioned || status.TextStatus == SvnWcStatus.Kind.Ignored)
            {
                string FullPath = Path.Combine(this.Directory, path.Value);
                if (File.Exists(FullPath))
                    File.Delete(FullPath);
                else if (System.IO.Directory.Exists(FullPath))
                    That.Computer.FileSystem.DeleteDirectory(FullPath, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents);
                else
                    throw new BuildException(string.Format("Something fishy, {0}, does not seem to exist yet an svn status syas it does.", FullPath), this.Location);
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            try
            {
                try
                {
                    AprPool p = Svn.PoolCreate();
                    SvnClientContext ctx = SvnClientContext.Create(p);
                    ctx.Config = SvnConfig.GetConfig(p);
                    this.Client = new SvnClient(ctx, p);

                    String AdminDir = ".svn";
                    if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SVN_ASP_DOT_NET_HACK")))
                        AdminDir = "_svn";

                    this.Client.SetWcAdmDir(AdminDir);
                    this.Client.AddSimpleProvider();
                    this.Client.AddUsernameProvider();
                    this.Client.AddSslServerTrustFileProvider();
                    this.Client.AddSslClientCertFileProvider();
                    this.Client.AddSslClientCertPwFileProvider();
                    this.Client.OpenAuth();
                    this.Client.Status2(this.Directory, new SvnRevision(Svn.Revision.Head), new SvnWcStatus2.Func(this.Clean), IntPtr.Zero, this.Recurse, true, false, false, false);
                }
                catch (Exception ex)
                {
                    throw new BuildException(ex.Message, this.Location, ex);
                }
            }
            finally
            {
                if (this.Client != null)
                    this.Client.Pool.Destroy();
            }

        }

        #endregion

    }
}
