using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("userFilter")]
	public class UserFilter : IModificationFilter
	{
		[ReflectorArray("names")]
		public string[] UserNames = new string[0];

		public bool Accept(Modification m)
		{
			return Array.IndexOf(UserNames, m.UserName) >= 0;
		}
	}
}