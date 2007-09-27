using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Util
{

	[ReflectorType("timeOut")]
	public class Timeout
	{
		public static Timeout DefaultTimeout = new Timeout(600000);

		private int timeout;
		private TimeUnits unit = TimeUnits.MILLIS;

		public Timeout(): this(600000)
		{
		}

		public Timeout(int periodInMillis) : this(periodInMillis, TimeUnits.MILLIS)
		{}

		public Timeout(int period, TimeUnits unit)
		{
			this.timeout = period;
			if (unit != null) this.unit = unit;
		}

		public TimeUnits TimeUnits
		{
			get { return unit; }
		}

		[ReflectorProperty("timeout", Required=false)]
		public int Millis
		{
			get 
			{ 
				return unit.ToMillis(timeout); 
			}
			set
			{
				this.timeout = unit.FromMillis(value);
			}
		}
	}
}