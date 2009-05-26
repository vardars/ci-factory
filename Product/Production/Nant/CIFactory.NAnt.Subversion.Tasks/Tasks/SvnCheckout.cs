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
    public enum AcceptCertificate
    {
        Reject,
        Permanently,
        Temporarily
    }

    [TaskName("svncheckout")]
    public class SvnCheckout : Task
    {
        #region Fields

        private SvnClient _Client;

        private string _Directory;

        private bool _Recurse;

        private SvnWcStatus2 _Status;

        #endregion

        #region Constructors

        public SvnCheckout()
        {
        }

        #endregion

        #region Properties

        private AcceptCertificate _AcceptCertificate;
        [TaskAttribute("acceptcertificate", Required = true)]
        public AcceptCertificate AcceptCertificate
        {
            get { return _AcceptCertificate; }
            set
            {
                _AcceptCertificate = value;
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

        private string _Url;
        [TaskAttribute("url", Required = true)]
        public string Url
        {
            get { return _Url; }
            set
            {
                _Url = value;
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

        private string _UserName;
        [TaskAttribute("username")]
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
            }
        }

        private string _Password;
        [TaskAttribute("password")]
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
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

        private SvnError UsernameAuth(out SvnAuthCredUsername cred, IntPtr baton, AprString realm, bool maySave, AprPool pool)
        {
            cred = SvnAuthCredUsername.Alloc(pool);
            cred.Username = new AprString(this.UserName, pool);
            cred.MaySave = maySave;

            return SvnError.NoError;
        }

        public SvnError SimpleAuth(out SvnAuthCredSimple cred, IntPtr baton, AprString realm, AprString username, bool maySave, AprPool pool)
        {
            cred = SvnAuthCredSimple.Alloc(pool);
            cred.Username = new AprString(this.UserName, pool);
            cred.Password = new AprString(this.Password, pool);
            cred.MaySave = maySave;
            return (SvnError.NoError);
        }

        public SvnError SslServerTrustAuth(out SvnAuthCredSslServerTrust cred,
                                           IntPtr baton, AprString realm,
                                           SvnAuthCredSslServerTrust.CertFailures failures,
                                           SvnAuthSslServerCertInfo certInfo,
                                           bool maySave, IntPtr pool)
        {
            this.Log(Level.Warning ,"Error validating server certificate for '{0}':", realm);
            if ((failures & SvnAuthCredSslServerTrust.CertFailures.UnknownCA) > 0)
                this.Log(Level.Warning, " - The certificate is not issued by a trusted authority");
            if ((failures & SvnAuthCredSslServerTrust.CertFailures.CNMismatch) > 0)
                this.Log(Level.Warning, " - The certificate hostname does not match");
            if ((failures & SvnAuthCredSslServerTrust.CertFailures.NotYetValid) > 0)
                this.Log(Level.Warning, " - The certificate is not yet valid");
            if ((failures & SvnAuthCredSslServerTrust.CertFailures.Expired) > 0)
                this.Log(Level.Warning, " - The certificate has expired");
            if ((failures & SvnAuthCredSslServerTrust.CertFailures.Other) > 0)
                this.Log(Level.Warning, " - The certificate has an unknown error");

            this.Log(Level.Warning, "Certificate informations:");
            this.Log(Level.Warning, "\tHostName:    " + certInfo.Hostname);
            this.Log(Level.Warning, "\tIssuer:      " + certInfo.IssuerDName);
            this.Log(Level.Warning, "\tValid From:  " + certInfo.ValidFrom);
            this.Log(Level.Warning, "\tValid Until: " + certInfo.ValidUntil);
            this.Log(Level.Warning, "\tFingerprint: " + certInfo.Fingerprint);

            cred = SvnAuthCredSslServerTrust.Alloc(pool);

            if (this.AcceptCertificate == AcceptCertificate.Reject)
            {
                cred.AcceptedFailures = 0;
                cred.MaySave = false;
            }
            else if (this.AcceptCertificate == AcceptCertificate.Temporarily)
            {
                cred.AcceptedFailures = failures;
                cred.MaySave = false;
            }
            else if (this.AcceptCertificate == AcceptCertificate.Permanently)
            {
                cred.AcceptedFailures = failures;
                cred.MaySave = true;
            }
             
            return (SvnError.NoError);
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

                    String AdminDir = ".svn";
                    if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SVN_ASP_DOT_NET_HACK")))
                        AdminDir = "_svn";

                    this.Client.SetWcAdmDir(AdminDir);
                    this.Client.AddSslClientCertFileProvider();
                    this.Client.AddSslClientCertPwFileProvider();
                    this.Client.AddPromptProvider(UsernameAuth, IntPtr.Zero, 0);
                    this.Client.AddPromptProvider(SimpleAuth, IntPtr.Zero, 0);
                    this.Client.AddPromptProvider(SslServerTrustAuth, IntPtr.Zero);
                    this.Client.AddSslServerTrustFileProvider();
                    this.Client.OpenAuth();

                    this.Client.Checkout2(this.Url, this.Directory, new SvnRevision(Svn.Revision.Unspecified), new SvnRevision(Svn.Revision.Head), this.Recurse, this.Externals);
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
