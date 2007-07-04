using System;
using System.Collections.Generic;
using System.Text;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using TF.Tasks.SourceControl.Types;
using CIFactory.NAnt.Types;

using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Client;

namespace TF.Tasks.Alerts
{
	[TaskName("tfsqueryalertsubscriptions")]
	public class QueryAlertSubscriptionsTask : Task
	{
		private IEventService _EventService;
		private TeamFoundationServer _Server;
		private TfsServerConnection _ServerConnection;

		public IEventService EventService
		{
			get
			{
				if (_EventService == null)
					_EventService = (IEventService)this.ServerConnection.TFS.GetService(typeof(IEventService));
				return _EventService;
			}
			set
			{
				_EventService = value;
			}
		}

		public TeamFoundationServer Server
		{
			get
			{
				return _Server;
			}
			set
			{
				_Server = value;
			}
		}

		[BuildElement("tfsserverconnection", Required = true)]
		public TfsServerConnection ServerConnection
		{
			get
			{
				return _ServerConnection;
			}
			set
			{
				_ServerConnection = value;
			}
		}

		private string _StringListRefId;

		[TaskAttribute("stringlistrefid", Required = true)]
		public string StringListRefId
		{
			get
			{
				return _StringListRefId;
			}
			set
			{
				_StringListRefId = value;
			}
		}

		protected override void ExecuteTask()
		{
			if (!this.Project.DataTypeReferences.Contains(this.StringListRefId))
				throw new BuildException(String.Format("The refid {0} is not defined.", this.StringListRefId));

			StringList RefStringList = (StringList)this.Project.DataTypeReferences[this.StringListRefId];

			string SubscriptionInfo;
			Subscription[] Subscriptions = this.EventService.EventSubscriptions(this.ServerConnection.TFS.AuthenticatedUserName);
			foreach (Subscription AlertSubscription in Subscriptions)
			{
				SubscriptionInfo = string.Format("ID '{0}', EventType '{1}', Address '{2}', Schedule '{3}', Type '{4}'", 
					AlertSubscription.ID, 
					AlertSubscription.EventType, 
					AlertSubscription.DeliveryPreference.Address, 
					AlertSubscription.DeliveryPreference.Schedule, 
					AlertSubscription.DeliveryPreference.Type);

				RefStringList.StringItems.Add(SubscriptionInfo, new StringItem(SubscriptionInfo));
			}
		}
	}
}
