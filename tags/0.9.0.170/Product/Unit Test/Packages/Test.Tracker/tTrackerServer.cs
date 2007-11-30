using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using Tracker;
using Tracker.Common;

namespace Test.Tracker
{
    [TestFixture]
    public class tTrackerServer
    {
        
#region Fields

        private TrackerServer _TrackerServer;
        private Stubs.ToolKitStub _ToolKit;

        #endregion

#region Properties

        public Stubs.ToolKitStub ToolKit
        {
            get
            {
                if (_ToolKit == null)
                    _ToolKit = new Stubs.ToolKitStub();
                return _ToolKit;
            }
            set
            {
                if (_ToolKit == value)
                    return;
                _ToolKit = value;
            }
        }

        public TrackerServer TrackerServer
        {
            get
            {
                if (_TrackerServer == null)
                    _TrackerServer = new TrackerServer(this.ToolKit);
                return _TrackerServer;
            }
            set { _TrackerServer = value; }
        }
        #endregion

#region Test TearDown

        [TearDown]
        public void TearDown()
        {
            this.TrackerServer = null;
            this.ToolKit = null;
        }
        #endregion

#region Logout

        [Test]
        public void tLogout()
        {
            this.TrackerServer.Logout();
            Assert.IsTrue(this.ToolKit.CalledLogout);
        }
        #endregion

#region Login

        [Test]
        public void tLogin()
        {
            string UserName = "Jay";
            this.TrackerServer.UserName = UserName;
            string Password = "password";
            this.TrackerServer.UserPWD = Password;
            string ProjectName = "Test";
            this.TrackerServer.ProjectName = ProjectName;
            int DBMSLoginMode = 2;
            this.TrackerServer.DBMSLoginMode = DBMSLoginMode;
            string DBMSPassword = "password";
            this.TrackerServer.DBMSPassword = DBMSPassword;
            string DBMSServer = "Server";
            this.TrackerServer.DBMSServer = DBMSServer;
            string DBMSType = "type";
            this.TrackerServer.DBMSType = DBMSType;

            this.TrackerServer.Login();

            Assert.IsTrue(this.ToolKit.CalledLogin);
            Assert.AreEqual(UserName, this.ToolKit.UserNamePassedToLogin);
            Assert.AreEqual(Password, this.ToolKit.PasswordPassedToLogin);
            Assert.AreEqual(ProjectName, this.ToolKit.ProjectNamePassedToLogin);
            Assert.AreEqual(DBMSLoginMode, this.ToolKit.DbmsLoginModePassedToLogin);
            Assert.AreEqual(DBMSPassword, this.ToolKit.DbmsPasswordPassedToLogin);
            Assert.AreEqual(DBMSServer, this.ToolKit.DbmsServerPassedToLogin);
            Assert.AreEqual(DBMSType, this.ToolKit.DbmsTypePassedToLogin);
        }
        #endregion
        
#region AddNote

        [Test]
        public void tAddNote()
        {
            int ScrId = 1234;
            string NoteTile = "NoteTitle";
            string NoteText = "Note Text";

            int RecordHandle = 2345;
            this.ToolKit.GetSCRRecordHandleReturn = RecordHandle;
            int NoteHandle = 3456;
            this.ToolKit.GetNoteHandleReturn = NoteHandle;
            int NoteTransactionId = 4567;
            this.ToolKit.GetNoteTransactionIdReturn = NoteTransactionId;

            this.TrackerServer.AddNote(ScrId, NoteTile, NoteText);

            this.ToolKit.CheckGetSCRRecordHandle(ScrId);
            this.ToolKit.CheckGetNoteHandle(RecordHandle);
            this.ToolKit.CheckBeginUpdate(RecordHandle);
            this.ToolKit.CheckGetNoteTransactionId(NoteHandle);
            this.ToolKit.CheckAddNote(NoteTile, NoteText, NoteHandle);
            this.ToolKit.CheckCommitRecord(RecordHandle, NoteTransactionId);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
            this.ToolKit.CheckReleaseNoteHandle(NoteHandle);
        }

#endregion
        
#region GetDescription

        [Test]
        public void tGetDescription()
        {
            int ScrId = 1234;

            int RecordHandle = 2345;
            this.ToolKit.GetSCRRecordHandleReturn = RecordHandle;
            string Description = "This is a lot of text.This is a lot of text.This is a lot of text.This is a lot of text." +
                "This is a lot of text.This is a lot of text.This is a lot of text.This is a lot of text." +
                "This is a lot of text.This is a lot of text.This is a lot of text.This is a lot of text." +
                "This is a lot of text.This is a lot of text.This is a lot of text.This is a lot of text." +
                "This is a lot of text.This is a lot of text.This is a lot of text.This is a lot of text.";
            this.ToolKit.DescriptionReturn = Description;

            string ReturnedDescription;
            ReturnedDescription = this.TrackerServer.GetDescription(ScrId);

            Assert.AreEqual(Description, ReturnedDescription);

            this.ToolKit.CheckGetSCRRecordHandle(ScrId);
            this.ToolKit.CheckGetDescriptionLength(RecordHandle);
            this.ToolKit.CheckGetDescriptionPart(2, RecordHandle);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
        }

#endregion

#region GetFieldValueInteger

        [Test]
        public void tGetFieldValueInteger()
        {
            int ScrId = 1234;
            string FieldName = "FieldName";

            this.ToolKit.GetFieldValueReturn = 6789;

            int FieldValue;
            FieldValue = this.TrackerServer.GetFieldValueInteger(ScrId, FieldName);

            Assert.AreEqual(this.ToolKit.GetFieldValueReturn, FieldValue);
            this.ToolKit.CheckGetFieldValue(FieldName, ScrId);
        }

#endregion

#region GetFieldValueString

        [Test]
        public void tGetFieldValueString()
        {
            int ScrId = 1234;
            string FieldName = "FieldName";

            this.ToolKit.GetFieldValueReturn = "testing";

            string FieldValue;
            FieldValue = this.TrackerServer.GetFieldValueString(ScrId, FieldName);

            Assert.AreEqual(this.ToolKit.GetFieldValueReturn, FieldValue);
            this.ToolKit.CheckGetFieldValue(FieldName, ScrId);
        }

#endregion
        
#region GetNoteList

        [Test]
        public void tGetNoteList()
        {
            int ScrId = 1234;

            int RecordHandle = 2345;
            this.ToolKit.GetSCRRecordHandleReturn = RecordHandle;
            int NoteHandle = 3456;
            this.ToolKit.GetNoteHandleReturn = NoteHandle;
            List<Stubs.Note> NoteList;
            NoteList = new List<Test.Tracker.Stubs.Note>();
            this.ToolKit.NoteList = NoteList;
            Stubs.Note FirstNote = new Stubs.Note("First", "Body", DateTime.Now.ToString(), "me");
            Stubs.Note SecondNote = new Stubs.Note("Second", "Text", DateTime.Now.ToString(), "you");
            this.ToolKit.NoteList.Add(FirstNote);
            this.ToolKit.NoteList.Add(SecondNote);

            StringCollection ConcatenatedNoteList;
            ConcatenatedNoteList = this.TrackerServer.GetNoteList(ScrId);

            string FirstConcatenatedNote;
            string SecondConcatenatedNote;

            FirstConcatenatedNote = string.Format("{0} ({1}) {2}, {3}", NoteList[0].Title, NoteList[0].Author, NoteList[0].CreationTime, NoteList[0].Text);
            SecondConcatenatedNote = string.Format("{0} ({1}) {2}, {3}", NoteList[1].Title, NoteList[1].Author, NoteList[1].CreationTime, NoteList[1].Text);

            Assert.AreEqual(FirstConcatenatedNote, ConcatenatedNoteList[0]);
            Assert.AreEqual(SecondConcatenatedNote, ConcatenatedNoteList[1]);

            this.ToolKit.CheckGetSCRRecordHandle(ScrId);
            this.ToolKit.CheckGetNoteHandle(RecordHandle);
            this.ToolKit.CheckInitalizeNoteList(NoteHandle);
            this.ToolKit.CheckGetNextNote(NoteHandle);
            this.ToolKit.CheckGetNoteTitle(NoteHandle);
            this.ToolKit.CheckGetNoteText(NoteHandle);
            this.ToolKit.CheckGetNoteAuthor(NoteHandle);
            this.ToolKit.CheckGetNoteCreateTime(NoteHandle);
            this.ToolKit.CheckReleaseNoteHandle(NoteHandle);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
        }

#endregion
        
#region GetSCRIDListFromQuery

        [Test]
        public void tGetSCRIDListFromQuery()
        {
            string QueryName = "QueryName";

            int RecordHandle = 4567;
            this.ToolKit.AllocateRecordHandleReturn = RecordHandle;
            int[] IdList = new int[2] { 1234, 2345 };
            this.ToolKit.QueryIdList = IdList;

            int[] ReturnedIdList;
            ReturnedIdList = this.TrackerServer.GetSCRIDListFromQuery(QueryName);

            Assert.AreEqual(IdList[0], ReturnedIdList[0]);
            Assert.AreEqual(IdList[1], ReturnedIdList[1]);

            this.ToolKit.CheckAllocateRecordHandle();
            this.ToolKit.CheckInitalizeRecordList(RecordHandle, QueryName);
            this.ToolKit.CheckGetNextScrId(RecordHandle);
            this.ToolKit.CheckGetSCRID(RecordHandle);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
        }

#endregion

#region SaveNumericFieldValue

        [Test]
        public void tSaveNumericFieldValue()
        {
            int ScrId = 1;
            string FieldName = "TestFieldName";
            int FieldValue = 3456;

            int RecordHandle = 1234;
            this.ToolKit.GetSCRRecordHandleReturn = RecordHandle;
            int FieldType = 3;
            this.ToolKit.GetFieldTypeReturn = FieldType;
            int TransactionId = 2345;
            this.ToolKit.GetFieldTransactionIdReturn = TransactionId;

            this.TrackerServer.SaveNumericFieldValue(ScrId, FieldName, FieldValue);

            this.ToolKit.CheckGetSCRRecordHandle(ScrId);
            this.ToolKit.CheckGetFieldType(FieldName);
            this.ToolKit.CheckBeginUpdate(RecordHandle);
            this.ToolKit.CheckGetFieldTransactionId(FieldName, RecordHandle);
            this.ToolKit.CheckSaveNumericFieldValue(FieldName, FieldValue, RecordHandle);
            this.ToolKit.CheckCommitRecord(RecordHandle, TransactionId);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
        }

        #endregion

#region SaveStringFieldValue

        
        [Test]
        public void tSaveStringFieldValue()
        {
            int ScrId = 1;
            string FieldName = "TestFieldName";
            string FieldValue = "TestFieldValue";

            int RecordHandle = 1234;
            this.ToolKit.GetSCRRecordHandleReturn = RecordHandle;
            int FieldType = 1;
            this.ToolKit.GetFieldTypeReturn = FieldType;
            int TransactionId = 2345;
            this.ToolKit.GetFieldTransactionIdReturn = TransactionId;

            this.TrackerServer.SaveStringFieldValue(ScrId, FieldName, FieldValue);
            
            this.ToolKit.CheckGetSCRRecordHandle(ScrId);
            this.ToolKit.CheckGetFieldType(FieldName);
            this.ToolKit.CheckBeginUpdate(RecordHandle);
            this.ToolKit.CheckGetFieldTransactionId(FieldName, RecordHandle);
            this.ToolKit.CheckSaveStringFieldValue(FieldName, FieldValue, RecordHandle);
            this.ToolKit.CheckCommitRecord(RecordHandle, TransactionId);
            this.ToolKit.CheckReleaseRecordHandle(RecordHandle);
        }

        #endregion

    }
}
