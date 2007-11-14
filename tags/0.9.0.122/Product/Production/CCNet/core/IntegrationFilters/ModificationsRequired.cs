using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.IntegrationFilters
{
	[ReflectorType("modificationsRequired")]
	public class ModificationsRequired : IIntegrationFilter
	{
		private bool _Condition = true;
		
		[ReflectorProperty("condition", Required=false)]
		public bool Condition
		{
			get
			{
				return _Condition;
			}
			set
			{
				_Condition = value;
			}
		}

		#region IIntegrationFilter Members

		public bool ShouldRunBuild(IIntegrationResult result)
		{
			if (!this.Condition)
				return true;
			bool ResultHasModifications = result.HasModifications();
			return ResultHasModifications;
		}

		#endregion
	}
}
