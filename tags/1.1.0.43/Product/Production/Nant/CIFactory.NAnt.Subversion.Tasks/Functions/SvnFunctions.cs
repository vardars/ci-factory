using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using PumaCode.SvnDotNet.SubversionSharp;
using PumaCode.SvnDotNet.AprSharp;
using System.IO;

namespace CIFactory.NAnt.Subversion.Tasks.Functions
{
    [FunctionSet("svn", "source control")]
    public class SvnFunctions : FunctionSetBase
    {
        #region Fields

        private SvnClient _Client;

        private bool _IsVersionedReturnValue;

        private string _QueryPath;

        #endregion

        #region Constructors

        public SvnFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {
        }

        public SvnFunctions()
            : base(null, null, null)
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

        public bool IsVersionedReturnValue
        {
            get
            {
                return _IsVersionedReturnValue;
            }
            set
            {
                _IsVersionedReturnValue = value;
            }
        }

        public string QueryPath
        {
            get
            {
                return _QueryPath;
            }
            set
            {
                _QueryPath = value;
            }
        }

        #endregion

        #region Public Methods

        public void AddHocTest()
        {
            bool VarName;
            VarName = this.IsVersioned(@"C:\Projects\NHINC\Current\Product");
            if (!VarName)
                throw new Exception("Expected something else!");
        }

        [Function("get-ignored-files")]
        public void GetIgnoredFiles(String refID, String directoryName, bool recursive)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FileSet RefFileSet = (FileSet)this.Project.DataTypeReferences[refID];

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
                AprArray crapList = this.Client.PropList(directoryName, new SvnRevision(Svn.Revision.Working), recursive);
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

                                RefFileSet.Includes.Add(FullPath);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (this.Client != null)
                    this.Client.Pool.Destroy();
            }
        }

        [Function("is-versioned")]
        public bool IsVersioned(string path)
        {
            this.QueryPath = path;
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
                    this.Client.Status2(this.QueryPath, new SvnRevision(Svn.Revision.Head), new SvnWcStatus2.Func(this.Record), IntPtr.Zero, false, true, false, false, true);
                    return this.IsVersionedReturnValue;
                }
                catch (SvnException SvnEx)
                {
                    if (SvnEx.AprErr == 155007)
                        return false;
                    throw new BuildException(SvnEx.Message, SvnEx);
                }
                catch (Exception ex)
                {
                    throw new BuildException(ex.Message, ex);
                }
            }
            finally
            {
                this.Client.Pool.Destroy();
            }
        }

        public void Record(IntPtr baton, SvnPath path, SvnWcStatus2 status)
        {
            if (path.Value.ToLower() == this.QueryPath.ToLower())
            {
                this.IsVersionedReturnValue = true;
                if (status.TextStatus == SvnWcStatus.Kind.Unversioned || status.TextStatus == SvnWcStatus.Kind.Ignored)
                {
                    this.IsVersionedReturnValue = false;
                }
            }
        }

        #endregion

    }
}
