using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.IntegrationFilters
{
		
	[ReflectorType("triggeredIntegrationFilter")]
	public class TriggeredIntegrationFilter : IIntegrationFilter
	{
					
		#region Fields

		private IIntegrationFilter[] _Allowed = new IIntegrationFilter[0];
		private IIntegrationFilter[] _Blocked = new IIntegrationFilter[0];
		private IIntegrationResult _Result;
				
		#endregion
				
		#region Properties

		private IIntegrationResult Result
		{
			get
			{
				return _Result;
			}
			set
			{
				_Result = value;
			}
		}

		[ReflectorArray("blocked", Required=false)]
		public IIntegrationFilter[] Blocked
		{
			get
			{
				return _Blocked;
			}
			set
			{
				_Blocked = value;
			}
		}

		[ReflectorArray("allowed", Required=false)]
		public IIntegrationFilter[] Allowed
		{
			get
			{
				return _Allowed;
			}
			set
			{
				_Allowed = value;
			}
		}
				
		#endregion

		#region IIntegrationFilter Members

		public bool ShouldRunBuild(IIntegrationResult result)
		{
			this.Result = result;
			bool IsRunnable;
					
			IsRunnable = this.CheckIfRunnable(this.Allowed, true);
			if (!IsRunnable)
				return false;

			IsRunnable = this.CheckIfRunnable(this.Blocked, false);
			if (!IsRunnable)
				return false;

			return true;
		}

		#endregion
				
		#region Helpers

		public bool CheckIfRunnable(IIntegrationFilter[] filters, bool positive)
		{
			foreach (IIntegrationFilter filter in filters)
			{
				bool ShouldRun = filter.ShouldRunBuild(this.Result);
				bool IsRunnable = ShouldRun == positive;
				if (!IsRunnable)
					return false;
			}
			return true;
		}

		#endregion
			
	}

}
