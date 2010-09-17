using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationRunner : IIntegratable
	{
		public IIntegrationRunnerTarget target;
		private readonly IIntegrationResultManager resultManager;
		private readonly IQuietPeriod quietPeriod;
		private readonly Project _Project;

		public IIntegrationFilter IntegrationFilter
		{
			get
			{
				return _Project.IntegrationFilter;
			}
		}

		public IntegrationRunner(IIntegrationResultManager resultManager, IIntegrationRunnerTarget target, IQuietPeriod quietPeriod, Project project)
		{
			this.target = target;
			this.quietPeriod = quietPeriod;
			this.resultManager = resultManager;
			this._Project = project;
		}

		public IIntegrationResult RunIntegration(IIntegrationResult result)
		{
			IIntegrationResult lastResult = resultManager.LastIntegrationResult;

			this.CreateDirectoryIfItDoesntExist(result.WorkingDirectory);
			this.CreateDirectoryIfItDoesntExist(result.ArtifactDirectory);
			result.MarkStartTime();
			bool IsRunable = false;
			try
			{
				result.Modifications = this.GetModifications(lastResult, result);
				IsRunable = this.IntegrationFilter.ShouldRunBuild(result);
				if (IsRunable)
				{
                    result.Label = _Project.Labeller.Generate(result, lastResult);
					target.Activity = ProjectActivity.Building;
					target.SourceControl.GetSource(result);
					this.RunBuild(result);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				result.ExceptionResult = ex;
			}
			result.MarkEndTime();

			if (IsRunable)
				this.PostBuild(result);

			target.Activity = ProjectActivity.Sleeping;

			return result;
		}

		private Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			target.Activity = ProjectActivity.CheckingModifications;
			return quietPeriod.GetModifications(target.SourceControl, from, to);
		}

		private void CreateDirectoryIfItDoesntExist(string directory)
		{
			if (! Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}

		private void RunBuild(IIntegrationResult result)
		{
			Log.Info("Building");
			target.Run(result);
			Log.Info("Build complete: " + result.Status);
		}

		private void PostBuild(IIntegrationResult result)
		{
			if (this.ShouldPublishResult(result))
			{
				this.LabelSourceControl(result);
				target.PublishResults(result);
				resultManager.FinishIntegration();
			}
			Log.Info("Integration complete: " + result.EndTime);
		}

		private void LabelSourceControl(IIntegrationResult result)
		{
			try
			{
				target.SourceControl.LabelSourceControl(result);
			}
			catch (Exception e)
			{
				Log.Error(new CruiseControlException("Exception occurred while labelling source control provider.", e));
			}
		}

		private bool ShouldPublishResult(IIntegrationResult result)
		{
			IntegrationStatus integrationStatus = result.Status;
			if (integrationStatus == IntegrationStatus.Exception)
			{
				return target.PublishExceptions;
			}
			else
			{
				return integrationStatus != IntegrationStatus.Unknown;
			}
		}
	}
}