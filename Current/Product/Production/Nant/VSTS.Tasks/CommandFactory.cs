using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class CommandFactory
    {

        private static MethodInfo _CreateCommandMethod;

        private static MethodInfo CreateCommandMethod
        {
            get
            {
                if (_CreateCommandMethod == null)
                {
                    foreach (Type Candidate in TestToolsHelper.CommandLineAssembly.GetTypes())
                    {
                        if (Candidate.FullName == "Microsoft.VisualStudio.TestTools.CommandLine.CommandFactory")
                        {
                            _CreateCommandMethod = Candidate.GetMethod("CreateCommand");
                            break;
                        }
                    }
                }
                return _CreateCommandMethod;
            }
        }

        public static Command CreateCommand(string switchName, string argument)
        {
            Object UnWrapedCommand;
            UnWrapedCommand = CreateCommandMethod.Invoke(null, new Object[2] { switchName, argument });
            return new Command(UnWrapedCommand);
        }
    }
}
