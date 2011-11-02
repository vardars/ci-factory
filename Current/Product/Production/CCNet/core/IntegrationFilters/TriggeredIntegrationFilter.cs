using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

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
            Log.Info(string.Format("{0}.{1}:Begin", System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            this.Result = result;
			bool IsRunnable;
					
			IsRunnable = this.CheckIfRunnable(this.Allowed, true);
            Log.Info(string.Format("{0}.{1}:IsRunnable={1}", System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, IsRunnable.ToString()));
            if (!IsRunnable)
				return false;

			IsRunnable = this.CheckIfRunnable(this.Blocked, false);
            Log.Info(string.Format("{0}.{1}:IsRunnable={1}", System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, IsRunnable.ToString()));
            if (!IsRunnable)
				return false;

            Log.Info(string.Format("{0}.{1}:IsRunnable={1}", System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, IsRunnable.ToString()));
            return true;
		}

		#endregion
				
		#region Helpers

		public bool CheckIfRunnable(IIntegrationFilter[] filters, bool positive)
		{
			foreach (IIntegrationFilter filter in filters)
			{
                Log.Info(string.Format("{0}.{1}", System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
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
