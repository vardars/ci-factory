using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ServiceModel;

namespace CCNET.TFS.Plugin
{
    public delegate void NotifyChangeSetIdDelegate(int id, string eventXml);

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        ValidateMustUnderstand = false)]
    public class NotificationReciever : INotificationReciever
    {

        #region Events

        public static event NotifyChangeSetIdDelegate NotifyChangeSetId;

        #endregion

        #region Fields

        private Regex _Captor;

        #endregion

        #region Properties

        public Regex Captor
        {
            get
            {
                if (_Captor == null)
                    _Captor = new Regex(@"\<Number\>(?<Id>\d+)\</Number\>", RegexOptions.Compiled | RegexOptions.Multiline);
                return _Captor;
            }
            set
            {
                _Captor = value;
            }
        }

        #endregion

        #region Methods

        private int GetChangeSetId(string eventXml)
        {
            string ChangeSetIdString = this.Captor.Match(eventXml).Groups[1].Value;
            int ChangeSetId = int.Parse(ChangeSetIdString);
            return ChangeSetId;
        }

        public void Notify(string eventXml, string tfsIdentityXml)
        {
            int ChangeSetId = this.GetChangeSetId(eventXml);
            NotificationReciever.NotifyChangeSetId(ChangeSetId, eventXml);
        }

        #endregion

        #region Ad-Hoc Tests

        [Conditional("DEBUG")]
        public void AdHocTest()
        {
            String EventXml = System.IO.File.ReadAllText(@"C:\Documents and Settings\jflowers.CHCSII\Desktop\event.xml");
            int ChangeSetId = this.GetChangeSetId(EventXml);
            System.Diagnostics.Debug.Assert(ChangeSetId == 107);
        }

        #endregion

    }

}
