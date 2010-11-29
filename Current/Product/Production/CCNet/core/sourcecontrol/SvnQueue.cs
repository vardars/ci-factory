using System;
using System.Collections;
using System.Collections.Generic;
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
            List<Modification[]> revisions = GroupModifications(modifications);
            Modification[] mods = new Modification[0];
            
            if (revisions.Count > 1)
            {
                mods = revisions[1];
                fsq.LastRevisionBuilt = mods[0].ChangeNumber.ToString();
            }
            else if(revisions.Count == 1)
            {
                if (revisions[0][0].ChangeNumber > int.Parse(fsq.LastRevisionBuilt))
                {
                    mods = revisions[0];
                    fsq.LastRevisionBuilt = mods[0].ChangeNumber.ToString();                    
                }                
			}
			
            fsq.SaveLastSvnRevision();
            
            if (UrlBuilder != null)
            {
                UrlBuilder.SetupModification(modifications);
            }

            return mods;
        }

        private List<Modification[]> GroupModifications(Modification[] mods)
        {
            List<Modification[]> revisions = new List<Modification[]>();
            List<Modification> revModSet = new List<Modification>();
            int currentRevision = 0;
            
            foreach (Modification mod in mods)
            {
                if (currentRevision != mod.ChangeNumber)
                {
                    if (currentRevision != 0) revisions.Add(revModSet.ToArray());
                    currentRevision = mod.ChangeNumber;
                    revModSet = new List<Modification>();
                }

                revModSet.Add(mod);
            }

            if (currentRevision != 0) revisions.Add(revModSet.ToArray());
            return revisions;
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