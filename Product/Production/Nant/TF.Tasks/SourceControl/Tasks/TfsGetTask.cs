using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

using TF.Tasks.SourceControl.Types;
using TF.Tasks.SourceControl.Helpers;
using Microsoft.Win32;
using System.IO;

namespace TF.Tasks.SourceControl.Tasks
{
    [TaskName("tfsget")]
    public class TfsGetTask : Task
    {

        private static string PrivateAssemblyPath
        {
            get
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS", false))
                {
                    string str2 = key.GetValue("EnvironmentDirectory") as string;
                    return Path.Combine(str2, "PrivateAssemblies");
                }
            }
        }

		private static Assembly ControlsAssembly =
            Assembly.LoadFile(String.Format(@"{0}\Microsoft.TeamFoundation.VersionControl.Controls.dll", PrivateAssemblyPath));
		private static Type DialogResolveType = 
			ControlsAssembly.GetType("Microsoft.TeamFoundation.VersionControl.Controls.DialogResolve");
		private static MethodInfo ResolveConflictsMethod = DialogResolveType.GetMethod("ResolveConflicts",
					BindingFlags.NonPublic | BindingFlags.Static, null, new Type[2] { typeof(Workspace), typeof(string[]) }, null);
		private static PropertyInfo UnresolvedConflictCountProperty = 
			DialogResolveType.GetProperty("UnresolvedConflictCount", BindingFlags.Instance | BindingFlags.NonPublic);

        #region Fields

        private string _ServerItem;
        private VersionSpecElement _VersionSpec;
        private bool _All;
        private bool _OverWrite;
        private bool _Recursive;
        private TfsServerConnection _ServerConnection;
        private string _WorkspaceName;
        private string _LocalItem;
        private WorkspaceAssistant _WorkspaceHelper;
        private string _ResultFileSetRefId;
        private FileSet _ResultFileSet;
		private bool _Failed = false;
		private bool _IsInteractive = false;
		private List<string> _ListOfFilesGotten;

        #endregion

        #region Properties

		public List<string> ListOfFilesGotten
		{
			get
			{
				if (_ListOfFilesGotten == null)
					_ListOfFilesGotten = new List<string>();
				return _ListOfFilesGotten;
			}
			set
			{
				_ListOfFilesGotten = value;
			}
		}

		public bool Failed
		{
			get
			{
				return _Failed;
			}
			set
			{
				_Failed = value;
			}
		}

        public FileSet ResultFileSet
        {
            get
            {
                if (_ResultFileSet == null)
                {
                    if (!String.IsNullOrEmpty(this.ResultFileSetRefId) && this.Project.DataTypeReferences.Contains(this.ResultFileSetRefId))
                    {
                        _ResultFileSet = (FileSet)this.Project.DataTypeReferences[this.ResultFileSetRefId];
                    }
                    else
                    {
                        _ResultFileSet = new FileSet();
                        _ResultFileSet.RefID = this.ResultFileSetRefId;
                    }
                }
                return _ResultFileSet;
            }
            set
            {
                _ResultFileSet = value;
            }
        }

        public WorkspaceAssistant WorkspaceHelper
        {
            get
            {
                if (_WorkspaceHelper == null)
                    _WorkspaceHelper = new WorkspaceAssistant();
                return _WorkspaceHelper;
            }
            set
            {
                _WorkspaceHelper = value;
            }
        }

		[TaskAttribute("isinteractive", Required = false)]
		public bool IsInteractive
		{
			get
			{
				return _IsInteractive;
			}
			set
			{
				_IsInteractive = value;
			}
		}

        [TaskAttribute("resultfilesetrefid", Required = false)]
        public string ResultFileSetRefId
        {
            get
            {
                return _ResultFileSetRefId;
            }
            set
            {
                _ResultFileSetRefId = value;
            }
        }

        [TaskAttribute("all", Required = false), BooleanValidator]
        public bool All
        {
            get
            {
                return _All;
            }
            set
            {
                _All = value;
            }
        }

        [TaskAttribute("localitem", Required = false)]
        public string LocalItem
        {
            get
            {
                return _LocalItem;
            }
            set
            {
                _LocalItem = value;
            }
        }

        [TaskAttribute("workspacename", Required = false)]
        public string WorkspaceName
        {
            get
            {
                return _WorkspaceName;
            }
            set
            {
                _WorkspaceName = value;
            }
        }

        [TaskAttribute("overwrite", Required = false), BooleanValidator]
        public bool OverWrite
        {
            get
            {
                return _OverWrite;
            }
            set        
            {
                _OverWrite = value;
            }
        }       

        [TaskAttribute("recursive", Required = false), BooleanValidator]
        public bool Recursive
        {
            get
            {
                return _Recursive;
            }
            set
            {
                _Recursive = value;
            }
        }

        [TaskAttribute("serveritem", Required = false)]
        public string ServerItem
        {
            get
            {
                return _ServerItem;
            }
            set
            {
                _ServerItem = value;
            }
        }

        [BuildElement("versionspec", Required = false)]
        public VersionSpecElement VersionSpec
        {
            get
            {
                if (_VersionSpec == null)
                    _VersionSpec = new VersionSpecElement();
                return _VersionSpec;
            }
            set
            {
                _VersionSpec = value;
            }
        }

        [BuildElement("tfsserverconnection", Required = true)]
        public TfsServerConnection ServerConnection
        {
            get
            {
                return _ServerConnection;
            }
            set
            {
                _ServerConnection = value;
            }
        }

        #endregion
        
        #region Methods

        private GetOptions GetGetOptions()
        {
            GetOptions Options = GetOptions.None;
            if (this.All && this.OverWrite)
            {
                Options = GetOptions.GetAll | GetOptions.Overwrite;
            }
            else if (this.All)
            {
                Options = GetOptions.GetAll;
            }
            else if (this.OverWrite)
            {
                Options = GetOptions.Overwrite;
            }
            return Options;
        }

		protected override void ExecuteTask()
		{
			//TODO: Adding the alternet use of a fileset instead of the LocalItem property
			Workspace MyWorkspace = this.WorkspaceHelper.GetWorkspace(this.WorkspaceName, this.LocalItem, this.ServerConnection);

			GetOptions Options = this.GetGetOptions();

			this.ServerConnection.SourceControl.Getting += new GettingEventHandler(OnGet);

			GetStatus Status = null;

			if (!String.IsNullOrEmpty(this.ServerItem))
			{
				RecursionType Recursion = RecursionType.None;
				if (this.Recursive)
					Recursion = RecursionType.Full;

				GetRequest GetReq = new GetRequest(new ItemSpec(this.ServerItem, Recursion), this.VersionSpec.GetVersionSpec());
				Status = MyWorkspace.Get(GetReq, Options);
			}
			else
			{
				Status = MyWorkspace.Get(this.VersionSpec.GetVersionSpec(), Options);
			}

			this.ServerConnection.SourceControl.Getting -= new GettingEventHandler(OnGet);

			if (this.IsInteractive && (Status.NumConflicts > 0 || Status.HaveResolvableWarnings))
			{
				Object ResolveObject = ResolveConflictsMethod.Invoke(null, new object[] { MyWorkspace, this.ListOfFilesGotten.ToArray() });
				int unresolvedConflictCount = (int)UnresolvedConflictCountProperty.GetValue(ResolveObject, null);
				if (unresolvedConflictCount == 0)
				{
					this.Failed = false;
					this.Log(Level.Verbose, "All conflicts have been resolved.");
				}
				else
				{
					this.Log(Level.Error, "{0} conflict(s) have not been resolved!", unresolvedConflictCount);
				}
			}


			if (this.Failed)
				throw new BuildException("Failed to get from TFS version control successfully!");
		}

        #endregion

		private void OnGet(object sender, GettingEventArgs e)
		{
			if (!String.IsNullOrEmpty(this.ResultFileSetRefId))
				this.ResultFileSet.Includes.Add(e.TargetLocalItem);
			this.ListOfFilesGotten.Add(e.TargetLocalItem);

			Level LogLevel = Level.Verbose;
			if (e.Status != OperationStatus.Replacing && e.Status != OperationStatus.Deleting && e.Status != OperationStatus.Getting)
			{
				LogLevel = Level.Error;
				this.Failed = true;
			}

			Log(LogLevel, "Status: '{0}',  ChangeType: '{1}', TargetLocalItem: '{2}' ServerItem: '{3}' Version: '{4}'", e.Status.ToString(), e.ChangeType.ToString(), e.TargetLocalItem, e.ServerItem, e.Version);
		}
    }
}
