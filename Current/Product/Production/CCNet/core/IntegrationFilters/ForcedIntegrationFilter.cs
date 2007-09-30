using System;
using System.Xml;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.IntegrationFilters
{
		
	[ReflectorType("forcedIntegrationFilter")]
	public class ForcedIntegrationFilter : IIntegrationFilter
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

		public void AllowTest()
		{
			this.Result = new IntegrationResult();
			this.Result.Modifications = new Modification[1] { new Modification() };

			ModificationsRequired Filter = new ModificationsRequired();
			Filter.Condition = true;
			this.Allowed = new IIntegrationFilter[1] { Filter };

			bool IsRunnable;
			IsRunnable = this.CheckIfRunnable(this.Allowed, true);

			if (!IsRunnable)
				throw new Exception();
		}
	}

	public class Test
	{
		public void TestSerialization()
		{
			ForcedIntegrationFilter TestSubject = new ForcedIntegrationFilter();

			ModificationsRequired ModReq = new ModificationsRequired();
			ModReq.Condition = true;

			TestSubject.Blocked = new IIntegrationFilter[1] { ModReq };

			string Serialized = Serialize("forcedIntegrationFilter", TestSubject);

//			Assert.IsNotNull(Serialized);

			ForcedIntegrationFilter Clone = (ForcedIntegrationFilter)Deserialize(Serialized);
			
//			Assert.IsNotNull(Clone.Blocked);
//			Assert.AreEqual(1, Clone.Blocked.Length);
//			Assert.IsNotNull(Clone.Blocked[0]);
		}

		public static string Serialize(string reflectorType, object subject)
		{
			StringWriter buffer = new StringWriter();
			new ReflectorTypeAttribute(reflectorType).Write(new XmlTextWriter(buffer), subject);
			return buffer.ToString();
		}

		public static object Deserialize(string serialized)
		{
			object Subject = NetReflector.Read(serialized);
			return Subject;
		}
	}

	

}
