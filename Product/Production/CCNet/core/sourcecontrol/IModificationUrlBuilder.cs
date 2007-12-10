using System.ComponentModel;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	[TypeConverter(typeof (ExpandableObjectConverter))]
	public interface IModificationUrlBuilder
	{
		void SetupModification(Modification[] modifications);
	}
}