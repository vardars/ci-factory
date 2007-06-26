using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// An object responsible for the continuous integration of a single project.
	/// This integrator, when running, coordinates the top-level life cycle of
	/// a project's integration.
	/// <list type="1">
	///		<item>The <see cref="ITrigger"/> instance is asked whether to build or not.</item>
	///		<item>If a build is required, the <see cref="IProject.RunIntegration(BuildCondition buildCondition)"/>
	///		is called.</item>
	/// </list>
	/// </summary>
	public class ProjectIntegrator : IProjectIntegrator, IDisposable
	{
		
		#region Fields
		
		private readonly IIntegratable _integratable;
		private ITrigger _trigger;
		private IProject _project;
		private bool _ShouldForceBuild;
		private Thread _thread;
		private ProjectIntegratorState _State = ProjectIntegratorState.Stopped;
		private readonly object _SyncObject = new object();
		private IIntegrationResult _IntegrationResult;
		private readonly IIntegrationResultManager resultManager;
		
		#endregion

		#region Properties

		private IIntegrationResult IntegrationResult
		{
			get
			{
				return _IntegrationResult;
			}
			set
			{
				_IntegrationResult = value;
			}
		}

		private object SyncObject
		{
			get
			{
				return _SyncObject;
			}
		}

		private bool ShouldForceBuild
		{
			get
			{
				return _ShouldForceBuild;
			}
			set
			{
				_ShouldForceBuild = value;
			}
		}

		public string Name
		{
			get { return _project.Name; }
		}

		public IProject Project
		{
			get { return _project; }
		}

		public ITrigger Trigger
		{
			get { return _trigger; }
		}

		public ProjectIntegratorState State
		{
			get { return _State; }
		}

		private ProjectIntegratorState SetState
		{
			set
			{
				_State = value;
			}
		}

		public bool IsRunning
		{
			get { return this.State == ProjectIntegratorState.Running; }
		}

		#endregion

		#region Constructors

		public ProjectIntegrator(IProject project) : this(new MultipleTrigger(project.Triggers), project, project)
		{
		}

		public ProjectIntegrator(ITrigger trigger, IIntegratable integratable, IProject project)
		{
			_trigger = trigger;
			_project = project;
			_integratable = integratable;
			this.resultManager = project.IntegrationResultManager;
		}

		#endregion
		
		#region Public Control

		public void Start()
		{
			lock (this.SyncObject)
			{
				if (IsRunning)
					return;

				this.SetState = ProjectIntegratorState.Running;
			}

			// multiple thread instances cannot be created
			if (_thread == null || _thread.ThreadState == ThreadState.Stopped)
			{
				_thread = new Thread(new ThreadStart(Run));
				_thread.Name = _project.Name;
			}

			// start thread if it's not running yet
			if (_thread.ThreadState != ThreadState.Running)
			{
				Log.Info("Starting integrator for project: " + _project.Name);
				_thread.Start();
			}
		}

		public bool ForceBuild(ForceFilterClientInfo[] clientInfo)
		{
			IIntegrationResult result = result = resultManager.StartNewIntegration();
			if (this.Project.ForceFilters != null && this.Project.ForceFilters.Length != 0)
			{
				foreach (IForceFilter Filter in this.Project.ForceFilters)
				{
					Boolean ToForce = Filter.ShouldRunIntegration(clientInfo, result);
					if (!ToForce)
						return false;
				}
			}
			Log.Info("Force Build for project: " + _project.Name);
			this.ShouldForceBuild = true;
			this.IntegrationResult = result;
			this.Start();
			return true;
		}


		/// <summary>
		/// Sets the scheduler's state to <see cref="ProjectIntegratorState.Stopping"/>, telling the scheduler to
		/// stop at the next possible point in time.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				Log.Info("Stopping integrator for project: " + _project.Name);
				this.SetState = ProjectIntegratorState.Stopping;
			} 
			else
			{
				this.SetState = ProjectIntegratorState.Stopped;
				_thread = null;
				Log.Info("Integrator for project: " + _project.Name + " is now stopped.");
			}
		}

		/// <summary>
		/// Asynchronously abort project by aborting the project thread.  This needs to be followed by a call to WaitForExit 
		/// to ensure that the abort has completed.
		/// </summary>
		public void Abort()
		{
			if (_thread != null)
			{
				Log.Info("Aborting integrator for project: " + _project.Name);
				_thread.Abort();
			}
		}

		public void WaitForExit()
		{
			if (_thread != null && _thread.IsAlive)
			{
				_thread.Join();
			}
		}

		
		#endregion
		
		#region Private Helpers

		/// <summary>
		/// Main integration loop, intended to be run in its own thread.
		/// </summary>
		private void Run()
		{
			Log.Info("Starting integration for project: " + _project.Name);
			try
			{
				// loop, until the integrator is stopped
				while (IsRunning)
				{
					Integrate();

					// sleep for a short while, to avoid hammering CPU
					Thread.Sleep(100);
				}
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
			}
			finally
			{
				this.Stop();
			}
		}

		private void Integrate()
		{
			// should we integrate this pass?
			bool ShouldIntegration = this.ShouldRunIntegration();
			if (ShouldIntegration)
			{
				try
				{
					_integratable.RunIntegration(this.IntegrationResult);
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}

				// notify the schedule whether the build was successful or not
				_trigger.IntegrationCompleted();
			}
			else
			{
				_trigger.IntegrationNotRun();
			}
		}

		private bool ShouldRunIntegration()
		{
			if (this.ShouldForceBuild)
			{
				this.ShouldForceBuild = false;
				this.IntegrationResult.BuildCondition = BuildCondition.ForceBuild;
				return true;
			}

			BuildCondition ShouldIntegration = _trigger.ShouldRunIntegration();
			
			if (ShouldIntegration != BuildCondition.NoBuild)
			{
				this.IntegrationResult = resultManager.StartNewIntegration();
				this.IntegrationResult.BuildCondition = ShouldIntegration;
				return true;
			}
			return false;
		}

		
		#endregion

		#region IDisposable

		/// <summary>
		/// Ensure that the scheduler's thread is terminated when this object is
		/// garbage collected.
		/// </summary>
		void IDisposable.Dispose()
		{
			Abort();
		}
	
		#endregion
	}
}