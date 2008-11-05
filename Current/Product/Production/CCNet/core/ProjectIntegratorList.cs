using System;
using System.Collections;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorList : IProjectIntegratorList
	{
        private Dictionary<string, IProjectIntegrator> _integrators = new Dictionary<string, IProjectIntegrator>();

		public void Add(IProjectIntegrator integrator)
		{
			Add(integrator.Name, integrator);
		}

		public void Add(string name, IProjectIntegrator integrator)
		{
			_integrators[name] = integrator;
		}

		public IProjectIntegrator this[string projectName] 
		{ 
			get { return _integrators[projectName]; }
		}

		public int Count
		{
			get { return _integrators.Values.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _integrators.Values.GetEnumerator();
		}

        #region IProjectIntegratorList Members


        public Dictionary<string, IProjectIntegrator> ProjectTable
        {
            get { return this._integrators; }
        }

        #endregion
    }
}
