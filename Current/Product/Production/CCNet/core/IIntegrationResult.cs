using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResult
	{
		string ProjectName { get; }
		BuildCondition BuildCondition { get; set; }
		string WorkingDirectory { get; set; }
		string Label { get; set; }
		string LastSuccessfulIntegrationLabel { get; }
		IntegrationStatus Status { get; set; }
		IntegrationStatus LastIntegrationStatus { get; }
		DateTime StartTime { get; set; }
		DateTime EndTime { get; }
		TimeSpan TotalIntegrationTime { get; }
		IList TaskResults { get; }
		DateTime LastModificationDate { get; }
		int LastChangeNumber { get; }
		Modification[] Modifications { get; set; }
		Exception ExceptionResult { get; set; }
		string ArtifactDirectory { get; set;}
		string ProjectUrl { get; set;}
		string TaskOutput { get; }

		void AddTaskResult(string result);
		void AddTaskResult(ITaskResult result);
		void AddIntegrationProperty(string key, string value);
		void RemoveIntegrationProperty(string key);
		bool IsInitial();
		bool HasModifications();
		bool Failed { get; }
		bool Fixed { get; }
		bool Succeeded { get; }
		void MarkStartTime();
		void MarkEndTime();
		string BaseFromArtifactsDirectory(string pathToBase);
		string BaseFromWorkingDirectory(string pathToBase);
		IDictionary IntegrationProperties { get; }

        [XmlIgnore]
        IIntegrationResult PreviousIntegrationResult { get; set; }
	}
}