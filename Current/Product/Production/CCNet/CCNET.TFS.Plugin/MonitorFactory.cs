using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;

namespace CCNET.TFS.Plugin
{

    public class MonitorFactory
    {
        private static Dictionary<string, VSTSMonitor> _Monitors = new Dictionary<string, VSTSMonitor>();

        private static Dictionary<string, VSTSMonitor> Monitors
        {
            get
            {
                return _Monitors;
            }
        }

        public static VSTSMonitor GetMonitor(TeamFoundationServer server, string stateFilePath, string projectPath, int port)
        {
            if (Monitors.ContainsKey(projectPath))
                return Monitors[projectPath];

            VSTSMonitor Monitor = new VSTSMonitor(server, stateFilePath, projectPath, port);
            Monitors.Add(projectPath, Monitor);
            return Monitor;
        }
    }

}
