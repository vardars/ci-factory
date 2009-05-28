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
    [TaskName("svnupdate")]
    public class SvnUpdate : Task
    {
        #region Fields

        private SvnClient _Client;

        private string _Directory;

        private bool _Recurse;

        private SvnWcStatus2 _Status;

        #endregion

        #region Constructors

        public SvnUpdate()
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

        private bool _Externals;
        [TaskAttribute("externals"), BooleanValidator()]
        public bool Externals
        {
            get { return _Externals; }
            set
            {
                _Externals = value;
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
            this.Externals = true;
            this.Recurse = true;
            this.Directory = @"C:\Projects\NHINC\Current\Product";
            this.ExecuteTask();
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

                    this.Client.Update2(new string[1] { this.Directory }, new SvnRevision(Svn.Revision.Head), this.Recurse, this.Externals);
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
