using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net;
using System.ServiceModel;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;

namespace CCNET.TFS.Plugin
{
    public enum MonitorStatus
    {
        Subscribed,
        Unsubscribed,
        Unknown
    }

    public class VSTSMonitor
    {

        #region Fields

        private Object _Sync = new object();
        private TeamFoundationServer _Server;
        private MonitorState _State;
        private ChangesetQueue _ChangesetQueue;
        private string _StateFilePath;
        private ServiceHost _ServiceHost;
        private IEventService _EventService;
        private VersionControlServer _SourceControl;
        private string _ProjectPath;
        private int _Port;
        private MonitorStatus _Status = MonitorStatus.Unknown;
        
        #endregion

        #region Properties

        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        public MonitorStatus Status
        {
            get
            {
                lock (this._Sync)
                    return _Status;
            }
        }

        public string ProjectPath
        {
            get
            {
                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
            }
        }

        public VersionControlServer SourceControl
        {
            get
            {
                if (null == _SourceControl)
                {
                    _SourceControl = (VersionControlServer)this.Server.GetService(typeof(VersionControlServer));
                }
                return _SourceControl;
            }
            set
            {
                _SourceControl = value;
            }
        }

        public IEventService EventService
        {
            get
            {
                if (_EventService == null)
                    _EventService = (IEventService)this.Server.GetService(typeof(IEventService));
                return _EventService;
            }
            set
            {
                _EventService = value;
            }
        }

        public ServiceHost ServiceHost
        {
            get
            {
                return _ServiceHost;
            }
            set
            {
                _ServiceHost = value;
            }
        }

        private string StateFilePath
        {
            get
            {
                return _StateFilePath;
            }
            set
            {
                _StateFilePath = value;
            }
        }

        public ChangesetQueue ChangesetQueue
        {
            get
            {
                if (_ChangesetQueue == null)
                    _ChangesetQueue = QueueFactory.GetChangesetQueue(this.ProjectPath, this.SourceControl);
                return _ChangesetQueue;
            }
            set
            {
                _ChangesetQueue = value;
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

        public MonitorState State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }

        #endregion

        #region Constructors

        public VSTSMonitor()
        {
        }

        public VSTSMonitor(TeamFoundationServer server, string stateFilePath, string projectPath, int port)
        {
            this.Server = server;
            this.StateFilePath = stateFilePath;
            this.ProjectPath = projectPath;
            this.Port = port;
            this.State = this.RetrieveState();
        }

        #endregion

        #region State File Managament

        public MonitorState RetrieveState()
        {
            this.ChangesetQueue.DequeueEvent += new DequeueEventHandler(OnDequeueEvent);
            XmlSerializer Serializer = new XmlSerializer(typeof(MonitorState));
            if (File.Exists(this.StateFilePath))
            {
                using (FileStream Reader = new FileStream(this.StateFilePath, FileMode.Open))
                {
                    return (MonitorState)Serializer.Deserialize(Reader);
                }
            }
            else
            {
                return new MonitorState();
            }
        }

        public void SaveState()
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(MonitorState));
            using (FileStream Writer = new FileStream(this.StateFilePath, FileMode.Create))
            {
                Serializer.Serialize(Writer, this.State);
            }
        }

        #endregion

        #region Helpers

        private Uri GetServiceUri()
        {
            UriBuilder ServiceUri = new UriBuilder();
            ServiceUri.Host = this.GetIpAddress();
            ServiceUri.Port = this.Port;
            ServiceUri.Path = "CCNET";
            return ServiceUri.Uri;
        }

        private string GetIpAddress()
        {
            string HostName = Dns.GetHostName();
            
            IPHostEntry Local = Dns.GetHostEntry( HostName );

            //TODO: @CPS Right now we just grab the first valid IPv4 address and return.  I assume this will be a problem with IPv6
            foreach (IPAddress IpAddress in Local.AddressList )
            {
                if (IpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return IpAddress.ToString();
                }
            }
            throw new Exception("No ip address found!");
        }

        private void StartListener(Uri uri)
        {
            this.ServiceHost = new ServiceHost(typeof(NotificationReciever), uri);
            this.ServiceHost.AddServiceEndpoint(typeof(INotificationReciever), new BasicHttpBinding(), uri);
            this.ServiceHost.Open();
        }

        private bool Subscribed()
        {
            bool AlreadySubscribed = false;
            Subscription[] Subscriptions = this.EventService.EventSubscriptions(this.Server.AuthenticatedUserName);
            foreach (Subscription Canidate in Subscriptions)
            {
                if (Canidate.ID == this.State.AlertId)
                {
                    AlreadySubscribed = true;
                    break;
                }
            }
            return AlreadySubscribed;
        }

        private DeliveryPreference GetSubscriptionInfo(string address)
        {
            DeliveryPreference SubscriptionInfo = new DeliveryPreference();

            SubscriptionInfo.Address = address;
            SubscriptionInfo.Type = DeliveryType.Soap;
            SubscriptionInfo.Schedule = DeliverySchedule.Immediate;
            return SubscriptionInfo;
        }

        #endregion

        #region Subscription Management

        public void Subscribe()
        {
            lock(this._Sync)
            {
                if (this.Status != MonitorStatus.Subscribed)
                {
                    this.UpdateQueue();

                    NotificationReciever.NotifyChangeSetId += new NotifyChangeSetIdDelegate(OnNotifyChangeSetId);

                    Uri ServiceUri = this.GetServiceUri();

                    this.StartListener(ServiceUri);

                    bool CurrentlySubscribed = this.Subscribed();

                    if (!CurrentlySubscribed)
                    {
                        DeliveryPreference SubscriptionInfo = this.GetSubscriptionInfo(ServiceUri.ToString());
                        this.State.AlertId = this.EventService.SubscribeEvent(this.Server.AuthenticatedUserName, "CheckinEvent", String.Empty, SubscriptionInfo);
                    }
                    this._Status = MonitorStatus.Subscribed;
                }
            }
        }

        public void Unsubscribe()
        {
            lock(this._Sync)
            {
                if (this.Status == MonitorStatus.Subscribed)
                {
                    bool CurrentlySubscribed = this.Subscribed();

                    if (CurrentlySubscribed)
                        this.EventService.UnsubscribeEvent(this.State.AlertId);

                    this.ServiceHost.Close();

                    NotificationReciever.NotifyChangeSetId -= new NotifyChangeSetIdDelegate(OnNotifyChangeSetId);

                    this.SaveState();
                }
            }
        }

        #endregion

        #region ChangeSet Queue Management

        private void OnNotifyChangeSetId(int id)
        {
            Changeset Set = this.SourceControl.GetChangeset(id);
            this.ChangesetQueue.Enqueue(Set);
        }

        private SortedList<int, Changeset> RetrieveChangeSetAfter(int id)
        {
            IEnumerable Iterable = this.SourceControl.QueryHistory(this.ProjectPath, VersionSpec.Latest, 0, RecursionType.Full, null, new ChangesetVersionSpec(id), VersionSpec.Latest, int.MaxValue, true, false);

            SortedList<int, Changeset> ChangeSets = new SortedList<int, Changeset>();
            foreach(Changeset Set in Iterable)
            {
                ChangeSets.Add(Set.ChangesetId, Set);
            }
            return ChangeSets;
        }

        private void UpdateQueue()
        {
            int LastChangeSetId = this.State.LastChangeSetId;
            if (LastChangeSetId == 0)
                return;
            SortedList<int, Changeset> ChangeSets = this.RetrieveChangeSetAfter(LastChangeSetId);
            foreach (Changeset Set in ChangeSets.Values)
            {
                this.ChangesetQueue.Enqueue(Set);
            }
        }

        private void OnDequeueEvent(Changeset changeset)
        {
            this.State.LastChangeSetId = changeset.ChangesetId;
            this.SaveState();
        }

        #endregion

        #region Ad-Hoc Testing

        [Conditional("DEBUG")]
        public void Play()
        {
            SortedList<int, string> ChangeSets = new SortedList<int, string>();
            ChangeSets.Add(1, "one");
            ChangeSets.Add(2, "two");
            ChangeSets.Add(3, "three");
            string Actual = ChangeSets.Values[0];
            Assert(Actual, "one");
        }

        [Conditional("DEBUG")]
        public void TestSubscribe()
        {
            this.Server = new TeamFoundationServer(@"http://vsts:8080");
            this.State = new MonitorState();
            this.Port = 4567;
            try
            {
                this.Subscribe();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            bool Continue = true;
            while (Continue)
            {
                System.Threading.Thread.Sleep(3000);
            }
            this.Unsubscribe();
        }

        [Conditional("DEBUG")]
        public void TestGetIpAddress()
        {
            string Actual = this.GetIpAddress();
            string Expected = "192.168.1.101";
            Assert(Actual, Expected);
        }

        [Conditional("DEBUG")]
        public void TestDeliveryPreference()
        {
            DeliveryPreference Info = this.GetSubscriptionInfo(@"http://me:1234/here");
            Assert(Info.Address, @"http://me:1234/here");
            Assert(Info.Schedule.ToString(), DeliverySchedule.Immediate.ToString());
            Assert(Info.Type.ToString(), DeliveryType.Soap.ToString());
        }

        [Conditional("DEBUG")]
        public void TestGetServiceUri()
        {
            this.Port = 1234;
            Uri Actual = this.GetServiceUri();
            Assert(Actual.ToString(), @"http://192.168.1.101:1234/CCNET");
        }

        [Conditional("DEBUG")]
        public void Assert(string actual, string expected)
        {
            Debug.Assert(actual == expected, string.Format("Actual is {0}, Expected is {1}", actual, expected));
        }

        #endregion

        #region Finallizer

        ~VSTSMonitor()
        {
            this.Unsubscribe();
        }

        #endregion

    }

}
