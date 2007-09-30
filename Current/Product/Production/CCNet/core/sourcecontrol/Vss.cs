using System;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    [ReflectorType("vss")]
    public class Vss : ProcessSourceControl
    {
        #region Constants

        public const string DefaultProject = "$/";

        private const string RecursiveCommandLineOption = "-R";

        public const string SS_DIR_KEY = "SSDIR";

        public const string SS_EXE = "ss.exe";

        public const string SS_REGISTRY_KEY = "SCCServerPath";

        public const string SS_REGISTRY_PATH = @"Software\\Microsoft\\SourceSafe";

        #endregion

        #region Fields

        /// <summary>
        /// Gets or sets whether this repository should be labeled.
        /// </summary>
        [ReflectorProperty("applyLabel", Required = false)]
        public bool ApplyLabel = false;

        [ReflectorProperty("autoGetSource", Required = false)]
        public bool AutoGetSource = false;

        [ReflectorProperty("cleanCopy", Required = false)]
        public bool CleanCopy = true;

        private string executable;

        private IVssLocale locale;

        [ReflectorProperty("password", Required = false)]
        public string Password;

        [ReflectorProperty("project", Required = false)]
        public string Project = DefaultProject;

        private IRegistry registry;

        private string ssDir;

        private string tempLabel;

        [ReflectorProperty("username", Required = false)]
        public string Username;

        [ReflectorProperty("workingDirectory", Required = false)]
        public string WorkingDirectory;

        #endregion

        #region Constructors

        public Vss(IVssLocale locale, IHistoryParser historyParser, ProcessExecutor executor, IRegistry registry)
            : base(historyParser, executor)
        {
            this.registry = registry;
            this.locale = locale;
        }

        private Vss(IVssLocale locale)
            : this(locale, new VssHistoryParser(locale), new ProcessExecutor(), new Registry())
        { }

        public Vss()
            : this(new VssLocale(CultureInfo.CurrentCulture))
        { }

        #endregion

        #region Properties

        [ReflectorProperty("culture", Required = false)]
        public string Culture
        {
            get { return locale.CultureName; }
            set { locale.CultureName = value; }
        }

        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get
            {
                if (executable == null)
                    executable = GetExecutableFromRegistry();
                return executable;
            }
            set { executable = value; }
        }

        [ReflectorProperty("ssdir", Required = false)]
        public string SsDir
        {
            get { return ssDir; }
            set { ssDir = StringUtil.StripQuotes(value); }
        }

        #endregion

        #region Public Methods

        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            return GetModifications(CreateHistoryProcessInfo(from, to), from.StartTime, to.StartTime);
        }

        public override void GetSource(IIntegrationResult result)
        {
            CreateTemporaryLabel(result);

            if (!AutoGetSource) return;

            Log.Info("Getting source from VSS");
            Execute(NewProcessInfoWith(GetSourceArgs(result), result));
        }

        public override void LabelSourceControl(IIntegrationResult result)
        {
            if (!ApplyLabel) return;

            Execute(NewProcessInfoWith(LabelProcessInfoArgs(result), result));
            tempLabel = null;
        }

        #endregion

        #region Private Methods

        private void AppendUsernameAndPassword(ProcessArgumentBuilder builder)
        {
            builder.AppendIf(!StringUtil.IsBlank(Username), string.Format("-Y{0},{1}", Username, Password));
        }

        private ProcessInfo CreateHistoryProcessInfo(IIntegrationResult from, IIntegrationResult to)
        {
            return NewProcessInfoWith(HistoryProcessInfoArgs(from.StartTime, to.StartTime), to);
        }

        private void CreateTemporaryLabel(IIntegrationResult result)
        {
            if (ApplyLabel)
            {
                tempLabel = CreateTemporaryLabelName(result.StartTime);
                LabelSourceControlWith(tempLabel, result);
                result.AddIntegrationProperty("CCNetVSSTempLabel", tempLabel);
            }
        }

        private string CreateTemporaryLabelName(DateTime time)
        {
            return "CCNETUNVERIFIED" + time.ToString("MMddyyyyHHmmss");
        }

        private string DeleteLabelProcessInfoArgs()
        {
            return LabelProcessInfoArgs(string.Empty, tempLabel);
        }

        private string GetExecutableFromRegistry()
        {
            string comServerPath = registry.GetExpectedLocalMachineSubKeyValue(SS_REGISTRY_PATH, SS_REGISTRY_KEY);
            return Path.Combine(Path.GetDirectoryName(comServerPath), SS_EXE);
        }

        private string GetSourceArgs(IIntegrationResult result)
        {
            ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
            builder.AddArgument("get", Project);
            builder.AddArgument(RecursiveCommandLineOption);
            if (ApplyLabel)
            {
                builder.AddArgument("-VL" + tempLabel);
            }
            else
            {
                builder.AddArgument("-Vd" + locale.FormatCommandDate(result.StartTime));
            }
            AppendUsernameAndPassword(builder);
            builder.AppendArgument("-I-N -W -GF- -GTM");
            builder.AppendIf(CleanCopy, "-GWR");
            return builder.ToString();
        }

        private string HistoryProcessInfoArgs(DateTime from, DateTime to)
        {
            ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
            builder.AddArgument("history", Project);
            builder.AddArgument(RecursiveCommandLineOption);
            builder.AddArgument(string.Format("-Vd{0}~{1}", locale.FormatCommandDate(to), locale.FormatCommandDate(from)));
            AppendUsernameAndPassword(builder);
            builder.AddArgument("-I-Y");
            return builder.ToString();
        }

        private string LabelProcessInfoArgs(IIntegrationResult result)
        {
            if (result.Succeeded)
            {
                return LabelProcessInfoArgs(result.Label, tempLabel);
            }
            else
            {
                return DeleteLabelProcessInfoArgs();
            }
        }

        private string LabelProcessInfoArgs(string label, string oldLabel)
        {
            ProcessArgumentBuilder builder = new ProcessArgumentBuilder();
            builder.AddArgument("label", Project);
            builder.AddArgument("-L" + label);
            builder.AddArgument("-VL", "", oldLabel); // only append argument if old label is specified
            AppendUsernameAndPassword(builder);
            builder.AddArgument("-I-Y");
            return builder.ToString();
        }

        private void LabelSourceControlWith(string label, IIntegrationResult result)
        {
            Execute(NewProcessInfoWith(LabelProcessInfoArgs(label, null), result));
        }

        private ProcessInfo NewProcessInfoWith(string args, IIntegrationResult result)
        {
            string workingDirectory = result.BaseFromWorkingDirectory(WorkingDirectory);
            if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);

            ProcessInfo processInfo = new ProcessInfo(Executable, args, workingDirectory);
            if (SsDir != null)
            {
                processInfo.EnvironmentVariables[SS_DIR_KEY] = SsDir;
            }
            return processInfo;
        }

        #endregion

    }
}