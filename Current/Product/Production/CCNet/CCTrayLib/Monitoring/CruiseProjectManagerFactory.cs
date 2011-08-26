using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
	{
		private IWebCruiseManagerFactory cruiseManagerFactory;

        public CruiseProjectManagerFactory(IWebCruiseManagerFactory cruiseManagerFactory)
        {
            this.cruiseManagerFactory = cruiseManagerFactory;
        }

		public ICruiseProjectManager Create(string serverUrl, string projectName)
		{
            ICruiseProjectManager cpm = new CruiseProjectManager(cruiseManagerFactory.GetCruiseManager(serverUrl), projectName);
            return cpm;
		}
	}
}