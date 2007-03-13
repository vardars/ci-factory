using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;
using System.IO;
using System.Reflection;

namespace CCNet.Server.Aggregator
{
    public class Program
    {
        static void Main()
        {
            ICruiseManagerFactory remoteCruiseManagerFactory = new RemoteCruiseManagerFactory();
            string ConfigPath = GetSettingsFilename();
            AggregateCruiseServer Aggregator = new AggregateCruiseServer(remoteCruiseManagerFactory, ConfigPath);
            Console.ReadLine();
        }

        private static string GetSettingsFilename()
        {
            string oldFashionedSettingsFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.xml");
            if (File.Exists(oldFashionedSettingsFilename))
                return oldFashionedSettingsFilename;

            string OtherFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cctray-settings.xml");
            if (File.Exists(OtherFilename))
                return OtherFilename;

            string newSettingsFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "cctray-settings.xml");
            if (File.Exists(newSettingsFilename))
                return newSettingsFilename;

            throw new FileNotFoundException(string.Format("Could not find '{0}', '{1}', or '{2}'.", oldFashionedSettingsFilename, newSettingsFilename, OtherFilename));
        }
    }
}
