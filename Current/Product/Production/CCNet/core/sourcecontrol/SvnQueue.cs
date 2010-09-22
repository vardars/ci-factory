using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("svnQueue")]
	public class SvnQueue : Svn
        //, ProcessSourceControl
	{
        public override Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
        {
            Configuration config = Configuration.Instance();
            FileSvnQueue fsq = null;
            string LastRevisionBuilt = string.Empty;
            
            foreach (Project p in config.Projects)
            {
                if (p.Name.Equals(to.ProjectName))
                {
                    fsq = new FileSvnQueue(p);
                    LastRevisionBuilt = fsq.ReadLastSvnRevision();
                    break;
                }
            }

            ProcessResult result = Execute(NextRevisionProcessInfo(LastRevisionBuilt, to), to.ProjectName);
            from.StartTime = DateTime.Parse("01/01/01");
            Modification[] modifications = ParseModifications(result, from.StartTime, to.StartTime);
            Modification[] mod = new Modification[1];

            if (modifications.GetLength(0) > 1)
            {
                fsq.LastRevisionBuilt = modifications[1].ChangeNumber.ToString();
                mod[0] = modifications[1];
            }
            else if(modifications.GetLength(0) == 1)
            {
                if (modifications[0].ChangeNumber > int.Parse(fsq.LastRevisionBuilt))
                {
                    fsq.LastRevisionBuilt = modifications[0].ChangeNumber.ToString();
                    mod[0] = modifications[0];
                }
                else
                {
                    ArrayList mods = new ArrayList();
                    mod = (Modification[]) mods.ToArray(typeof(Modification));
                }
			}
			
            fsq.SaveLastSvnRevision();

            if (UrlBuilder != null)
            {
                UrlBuilder.SetupModification(modifications);
            }

            return mod;   
        }

//      HISTORY_COMMAND_FORMAT = "log TrunkUrl -r LastBuildRevision --verbose --xml --non-interactive";
        private ProcessInfo NextRevisionProcessInfo(string lastRevisionBuilt, IIntegrationResult result)
        {

            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("log");
            buffer.AddArgument(TrunkUrl);
            if (lastRevisionBuilt.Equals(string.Empty))
            {
                buffer.AppendArgument("-l1");
            }
            else
            {
                buffer.AppendArgument(string.Format("-r \"{0}:HEAD\"", lastRevisionBuilt));
            }
            buffer.AppendArgument("--verbose --xml");
            AppendCommonSwitches(buffer);
            return NewProcessInfo(buffer.ToString(), result);
        }

        //private Get
	}
}