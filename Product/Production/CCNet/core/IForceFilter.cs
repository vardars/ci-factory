using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public interface IForceFilter
	{
		bool ShouldRunIntegration(ForceFilterClientInfo[] clientInfo, IIntegrationResult result);
		ForceFilterClientInfo GetClientInfo();
		bool RequiresClientInformation{get;}
	}

}
