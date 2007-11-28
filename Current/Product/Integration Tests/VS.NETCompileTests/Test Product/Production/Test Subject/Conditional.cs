using System;

namespace TestSubject
{
	public class Conditional
	{
		public String ComplexeMethod(String name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name == String.Empty)
				throw new ArgumentException("Name can not be blank.");
			return String.Format("Hello {0}.", name);
		}
	}
}
