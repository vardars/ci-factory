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
                    foreach (Type Canidate in TestToolsHelper.CommandLineAssembly.GetTypes())
                    {
                        if (Canidate.FullName == "Microsoft.VisualStudio.TestTools.CommandLine.CommandFactory")
                        {
                            _CreateCommandMethod = Canidate.GetMethod("CreateCommand");
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
