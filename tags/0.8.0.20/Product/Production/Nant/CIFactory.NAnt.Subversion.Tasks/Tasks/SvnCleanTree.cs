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
        private SvnClient _Client;
        private string _Directory;
        private bool _Recurse;
        
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

        public SvnCleanTree()
        {
        }

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
                this.Client.Pool.Destroy();
            }

        }

        public void Clean(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
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

        public void AddHocTest()
        {
            this.Directory = @"C:\Projects\CI Factory\Current\Third Party\CCNET";
            this.ExecuteTask();
        }
    }
}
