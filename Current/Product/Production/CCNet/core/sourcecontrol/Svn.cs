using System;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svn")]
	public class Svn : ProcessSourceControl
	{
		public const string DefaultExecutable = "svn.exe";
		internal static readonly string COMMAND_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

		public Svn(ProcessExecutor executor, IHistoryParser parser) : base(parser, executor)
		{}

		public Svn() : base(new SvnHistoryParser())
		{}

		[ReflectorProperty("webUrlBuilder", InstanceTypeKey="type", Required = false)]
		public IModificationUrlBuilder UrlBuilder;

		[ReflectorProperty("executable", Required = false)]
		public string Executable = DefaultExecutable;

		[ReflectorProperty("trunkUrl", Required = false)]
		public string TrunkUrl;

		[ReflectorProperty("workingDirectory", Required = false)]
		public string WorkingDirectory;

		[ReflectorProperty("tagOnSuccess", Required = false)]
		public bool TagOnSuccess = false;

		[ReflectorProperty("tagBaseUrl", Required = false)]
		public string TagBaseUrl;

		private string _Username;

        [ReflectorProperty("username", Required = false)]
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }

        private string _Password;

        [ReflectorProperty("password", Required = false)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = false;

		public string FormatCommandDate(DateTime date)
		{
			return date.ToUniversalTime().ToString(COMMAND_DATE_FORMAT, CultureInfo.InvariantCulture);
		}

		public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessResult result = Execute(NewHistoryProcessInfo(from, to), to.ProjectName);
			Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
			if (UrlBuilder != null)
			{
				UrlBuilder.SetupModification(modifications);
			}
			return modifications;
		}

		public override void LabelSourceControl(IIntegrationResult result)
		{
			if (TagOnSuccess && result.Succeeded)
			{
				Execute(NewLabelProcessInfo(result), result.ProjectName);
			}
		}

		public override void GetSource(IIntegrationResult result)
		{
			if (! AutoGetSource) return;

			if (DoesSvnDirectoryExist(result))
			{
				UpdateSource(result);
			}
			else
			{
				CheckoutSource(result);
			}
		}

        // Added to support SvnRevisionLabeller
        public ProcessResult GetInfo(IIntegrationResult result)
        {
            return Execute(NewInfoProcessInfo(result), result.ProjectName);
        }

		private void CheckoutSource(IIntegrationResult result)
		{
			if (StringUtil.IsBlank(TrunkUrl))
				throw new ConfigurationException("<trunkurl> configuration element must be specified in order to automatically checkout source from SVN.");
			Execute(NewCheckoutProcessInfo(result), result.ProjectName);
		}

		private ProcessInfo NewCheckoutProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("checkout");
			buffer.AddArgument(TrunkUrl);
			buffer.AddArgument(result.BaseFromWorkingDirectory(WorkingDirectory));
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), result);
		}

		private void UpdateSource(IIntegrationResult result)
		{
			Execute(NewGetSourceProcessInfo(result), result.ProjectName);
		}

		private bool DoesSvnDirectoryExist(IIntegrationResult result)
		{
			string svnDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), ".svn");
			string underscoreSvnDirectory = Path.Combine(result.BaseFromWorkingDirectory(WorkingDirectory), "_svn");
            return Directory.Exists(svnDirectory) || Directory.Exists(underscoreSvnDirectory);
		}

		private ProcessInfo NewGetSourceProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("update");
			AppendRevision(buffer, result.LastChangeNumber);
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), result);
		}

//		TAG_COMMAND_FORMAT = "copy --message "CCNET build label" "trunkUrl" "tagBaseUrl/label"
		private ProcessInfo NewLabelProcessInfo(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("copy");
			buffer.AppendArgument(TagMessage(result.Label));
			buffer.AddArgument(TagSource(result));
			buffer.AddArgument(TagDestination(result.Label));
			AppendRevision(buffer, result.LastChangeNumber);
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), result);
		}

//		HISTORY_COMMAND_FORMAT = "log TrunkUrl --revision \"{{{StartDate}}}:{{{EndDate}}}\" --verbose --xml --non-interactive";
		private ProcessInfo NewHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			buffer.AddArgument("log");
			buffer.AddArgument(TrunkUrl);
			buffer.AppendArgument(string.Format("-r \"{{{0}}}:{{{1}}}\"", FormatCommandDate(from.StartTime), FormatCommandDate(to.StartTime)));
			buffer.AppendArgument("--verbose --xml");
			AppendCommonSwitches(buffer);
			return NewProcessInfo(buffer.ToString(), to);
		}

        // Added to support SvnRevisionLabeller
        private ProcessInfo NewInfoProcessInfo(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("info");
            buffer.AddArgument(TrunkUrl);
            buffer.AppendArgument(string.Format("-r \"{{{0}}}\"",
                DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)));
            buffer.AppendArgument("--xml");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), result);
        }

		private static string TagMessage(string label)
		{
			return string.Format("-m \"CCNET build {0}\"", label);
		}

		private string TagSource(IIntegrationResult result)
		{
			if (result.LastChangeNumber == 0)
			{
				return result.BaseFromWorkingDirectory(WorkingDirectory).TrimEnd(Path.DirectorySeparatorChar);
			}
			return TrunkUrl;
		}

		private string TagDestination(string label)
		{
			return string.Format("{0}/{1}", TagBaseUrl, label);
		}

		protected void AppendCommonSwitches(ProcessArgumentBuilder buffer)
		{
			buffer.AddArgument("--username", Username);
			buffer.AddArgument("--password", Password);
			buffer.AddArgument("--non-interactive");
			buffer.AddArgument("--no-auth-cache");
		}

		private void AppendRevision(ProcessArgumentBuilder buffer, int revision)
		{
			buffer.AppendIf(revision > 0, "--revision {0}", revision.ToString());
		}

		protected ProcessInfo NewProcessInfo(string args, IIntegrationResult result)
		{
		    ProcessInfo processInfo = new ProcessInfo(Executable, args, result.BaseFromWorkingDirectory(WorkingDirectory));
		    processInfo.StreamEncoding = Encoding.UTF8;
		    return processInfo;
		}
	}
}