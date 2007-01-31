using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNET.TFS.Plugin
{
    public class QueueFactory
    {
        private static Dictionary<string, ChangesetQueue> _Queues = new Dictionary<string, ChangesetQueue>();

        private static Dictionary<string, ChangesetQueue> Queues
        {
            get
            {
                return _Queues;
            }
        }

        public static ChangesetQueue GetChangesetQueue(string projectPath, VersionControlServer sourceControl)
        {
            if (Queues.ContainsKey(projectPath))
                return Queues[projectPath];

            ChangesetQueue Queue = new ChangesetQueue(projectPath, sourceControl);
            Queues.Add(projectPath, Queue);
            return Queue;
        }

    }

}
