using System;
using System.Collections;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectIntegratorList : IEnumerable
	{
		IProjectIntegrator this[string projectName] { get; }
		int Count { get; }
        Dictionary<string, IProjectIntegrator> ProjectTable { get; }
	}
}
