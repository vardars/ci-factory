using ThoughtWorks.CruiseControl.Remote;
namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class NullUrlBuilder : IModificationUrlBuilder
	{
		public void SetupModification(Modification[] modifications)
		{}
	}
}