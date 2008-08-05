using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    public class ConsoleRunner
    {
        #region Fields

        private readonly ArgumentParser _parser;

        private readonly ICruiseServerFactory _serverFactory;

        private ICruiseServer server;

        #endregion

        #region Constructors

        public ConsoleRunner(ArgumentParser parser, ICruiseServerFactory serverFactory)
        {
            _parser = parser;
            _serverFactory = serverFactory;
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            if (_parser.ShowHelp)
            {
                Log.Warning(ArgumentParser.Usage);
                return;
            }
            LaunchServer();
        }

        #endregion

        #region Private Methods

        private void HandleControlEvent(object sender, EventArgs args)
        {
            server.Dispose();
        }

        private void LaunchServer()
        {
            using (ConsoleEventHandler handler = new ConsoleEventHandler())
            {
                handler.OnConsoleEvent += new EventHandler(HandleControlEvent);

                using (server = _serverFactory.Create(_parser.UseRemoting, _parser.ConfigFile))
                {
                    if (_parser.Project == null)
                    {
                        server.Start();
                        server.WaitForExit();
                    }
                    else
                    {
                        server.ForceBuild(_parser.Project, null);
                        server.WaitForExit(_parser.Project);
                    }
                }
            }
        }

        #endregion

    }
}