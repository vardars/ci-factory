using System;
using System.IO;
using System.Text.RegularExpressions;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using NAnt.Contrib.Tasks.Perforce;

using Helper = NAnt.Contrib.Tasks.Perforce.Perforce;

namespace NAnt.Contrib.Functions.Perforce
{
    [FunctionSet("perforce", "sourcecontrol")]
    public class PerforceFunctions : FunctionSetBase
    {
        public PerforceFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {

        }

        [Function("workspace-exists")]
        public bool WorkspaceExists(string client, string port)
        {
            string output = Helper.getProcessOutput("p4", string.Format("-p {0} clients", port), null);
            string[] lines = output.Split('\n');
            foreach (string line in lines)
            {
                string[] s2 = line.Split(' ');   // poor manz regex
                if (s2.Length > 1)
                {
                    if (s2[1].Trim('\r') == client)
                        return true;
                }
            }
            return false;
        }

        [Function("get-changelist-number")]
        public string GetChangeListNumber(string changelistName)
        {
            return Helper.GetChangelistNumber(Helper.GetUserName(), Helper.GetClient(), changelistName, true);
        }
    }
}
