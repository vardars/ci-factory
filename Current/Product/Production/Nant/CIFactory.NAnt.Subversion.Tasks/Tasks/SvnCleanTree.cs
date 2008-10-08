using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using PumaCode.SvnDotNet.SubversionSharp;
using PumaCode.SvnDotNet.AprSharp;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("svncleantree")]
    public class SvnCleanTree : Task
    {

        #region Fields

        private SvnClient _Client;

        private bool _DeleteIgnored;

        private string _Directory;

        private bool _Recurse;

        private SvnWcStatus2 _Status;

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

        [TaskAttribute("deleteignored"), BooleanValidator()]
        public bool DeleteIgnored
        {
            get { return _DeleteIgnored; }
            set
            {
                _DeleteIgnored = value;
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

        private SvnWcStatus2 Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
            }
        }

        #endregion

        #region Public Methods

        public void AddHocTest()
        {
            this.DeleteIgnored = true;
            this.Recurse = true;
            this.Directory = @"C:\Projects\NHINC\Current\Product";
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
                    throw new BuildException(string.Format("Something fishy, {0}, does not seem to exist yet an svn status says it does.", FullPath), this.Location);
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

                    if (this.DeleteIgnored)
                        this.DeleteIgnoredFiles();
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

        #region Private Methods

        private void DeleteIgnoredFiles()
        {
            AprArray crapList = this.Client.PropList(this.Directory, new SvnRevision(Svn.Revision.Working), this.Recurse);
            crapList.ElementType = typeof(SvnClientPropListItem);
            SvnClientPropListItem[] props = new SvnClientPropListItem[crapList.Count];
            crapList.CopyTo(props, 0);

            foreach (SvnClientPropListItem property in props)
            {
                foreach (AprHashEntry entry in property.PropHash)
                {
                    if (entry.KeyAsString == "svn:ignore")
                    {
                        foreach (String ignored in new SvnString(entry.Value).Data.Value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string FullPath = Path.Combine(property.NodeName.ToString(), ignored);

                            try
                            {
                                if (File.Exists(FullPath))
                                {
                                    if (OkayToDelete(FullPath))
                                        File.Delete(FullPath);
                                }
                                else if (System.IO.Directory.Exists(FullPath))
                                {
                                    if (OkayToDelete(FullPath))
                                        That.Computer.FileSystem.DeleteDirectory(FullPath, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents);
                                }
                            }
                            catch (IOException ex)
                            {
                                throw new BuildException(String.Format("Error trying to delete: '{0}'.", FullPath), this.Location);
                            }
                        }
                    }
                }
            }
        }

        private void HoldStatus(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            this.Status = status;
        }

        private bool OkayToDelete(string FullPath)
        {
            this.Status = null;
            this.Client.Status2(FullPath, new SvnRevision(Svn.Revision.Head), new SvnWcStatus2.Func(this.HoldStatus), IntPtr.Zero, false, true, false, false, false);

            if (this.Status == null)
                return true;

            bool okayToDelete = this.Status.TextStatus == SvnWcStatus.Kind.Unversioned || this.Status.TextStatus == SvnWcStatus.Kind.Ignored;
            return okayToDelete;
        }

        #endregion

    }
}
