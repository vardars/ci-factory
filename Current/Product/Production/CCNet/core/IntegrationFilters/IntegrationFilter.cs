using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.IntegrationFilters
{
	[ReflectorType("integrationFilter")]
	public class IntegrationFilter : IIntegrationFilter
	{
		
		#region Fields

		private TriggeredIntegrationFilter _Triggered;
		private ForcedIntegrationFilter _Forced;
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

		[ReflectorProperty("triggeredIntegrationFilter", Required=false)]
		public TriggeredIntegrationFilter Triggered
		{
			get
			{
				return _Triggered;
			}
			set
			{
				_Triggered = value;
			}
		}

		[ReflectorProperty("forcedIntegrationFilter", Required=false)]
		public ForcedIntegrationFilter Forced
		{
			get
			{
				return _Forced;
			}
			set
			{
				_Forced = value;
			}
		}
		
		#endregion

		#region IIntegrationFilter Members

		public bool ShouldRunBuild(IIntegrationResult result)
		{
			this.Result = result;
			bool IsRunnable;
			
			if (result.BuildCondition != BuildCondition.ForceBuild)
			{
				IsRunnable = this.CheckIfRunnable(this.Triggered);
				if (!IsRunnable)
				{
					Log.Info("Trigger blocked by integration filter.");
					return false;
				}
			}

			if (result.BuildCondition == BuildCondition.ForceBuild)
			{
				IsRunnable = this.CheckIfRunnable(this.Forced);
				if (!IsRunnable)
				{
					Log.Info("Force blocked by integration filter.");
					return false;
				}
			}

			return true;
		}

		#endregion
	
		#region Helpers

		public bool CheckIfRunnable(IIntegrationFilter filter)
		{
			if (filter != null)
			{
				return filter.ShouldRunBuild(this.Result);
			}
			return true;
		}

		#endregion

		public void Test2()
		{
			Project TestProject = new Project();
            TestProject.Name = "Test Project";

			TestProject.IntegrationFilter = this;

			NetReflectorProjectSerializer Izer = new NetReflectorProjectSerializer();
			string Serialized = Izer.Serialize(TestProject);
			Console.WriteLine(Serialized);
			Project Clone = Izer.Deserialize(Serialized);

		}

	}

}