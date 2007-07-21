using System;
using System.Collections.Generic;
using System.Text;
using Tracker;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using Tracker.Common;

namespace Test.Tracker.Stubs
{
    
    public class ToolKitStub : IPVCSToolKit
    {
        #region IPVCSToolKit Members

#region Logout
        private bool _CalledLogout = false;

        public bool CalledLogout
        {
            get { return _CalledLogout; }
            set { _CalledLogout = value; }
        }
        public void Logout()
        {
            this._CalledLogout = true;
        }
        #endregion
        
#region Login

        private bool _CalledLogin = false;

        public bool CalledLogin
        {
            get
            {
                return _CalledLogin;
            }
            set
            {
                if (_CalledLogin == value)
                    return;
                _CalledLogin = value;
            }
        }

        private string _UserNamePassedToLogin;
        private string _PasswordPassedToLogin;
        private string _ProjectNamePassedToLogin;
        private string _DbmsUserNamePassedToLogin;
        private string _DbmsPasswordPassedToLogin;
        private string _DbmsServerPassedToLogin;
        private string _DbmsTypePassedToLogin;
        private int _DbmsLoginModePassedToLogin;

        public string UserNamePassedToLogin
        {
            get
            {
                return _UserNamePassedToLogin;
            }
            set
            {
                if (_UserNamePassedToLogin == value)
                    return;
                _UserNamePassedToLogin = value;
            }
        }
        public string PasswordPassedToLogin
        {
            get
            {
                return _PasswordPassedToLogin;
            }
            set
            {
                if (_PasswordPassedToLogin == value)
                    return;
                _PasswordPassedToLogin = value;
            }
        }
        public string ProjectNamePassedToLogin
        {
            get
            {
                return _ProjectNamePassedToLogin;
            }
            set
            {
                if (_ProjectNamePassedToLogin == value)
                    return;
                _ProjectNamePassedToLogin = value;
            }
        }
        public string DbmsUserNamePassedToLogin
        {
            get
            {
                return _DbmsUserNamePassedToLogin;
            }
            set
            {
                if (_DbmsUserNamePassedToLogin == value)
                    return;
                _DbmsUserNamePassedToLogin = value;
            }
        }
        public string DbmsPasswordPassedToLogin
        {
            get
            {
                return _DbmsPasswordPassedToLogin;
            }
            set
            {
                if (_DbmsPasswordPassedToLogin == value)
                    return;
                _DbmsPasswordPassedToLogin = value;
            }
        }
        public string DbmsServerPassedToLogin
        {
            get
            {
                return _DbmsServerPassedToLogin;
            }
            set
            {
                if (_DbmsServerPassedToLogin == value)
                    return;
                _DbmsServerPassedToLogin = value;
            }
        }
        public string DbmsTypePassedToLogin
        {
            get
            {
                return _DbmsTypePassedToLogin;
            }
            set
            {
                if (_DbmsTypePassedToLogin == value)
                    return;
                _DbmsTypePassedToLogin = value;
            }
        }
        public int DbmsLoginModePassedToLogin
        {
            get
            {
                return _DbmsLoginModePassedToLogin;
            }
            set
            {
                if (_DbmsLoginModePassedToLogin == value)
                    return;
                _DbmsLoginModePassedToLogin = value;
            }
        }

        public void Login(string userName, string password, string projectName, string dbmsUserName, string dbmsPassword, string dbmsServer, string dbmsType, int dbmsLoginMode)
        {
            this.CalledLogin = true;
            this.UserNamePassedToLogin = userName;
            this.PasswordPassedToLogin = password;
            this.ProjectNamePassedToLogin = projectName;
            this.DbmsLoginModePassedToLogin = dbmsLoginMode;
            this.DbmsPasswordPassedToLogin = dbmsPassword;
            this.DbmsServerPassedToLogin = dbmsServer;
            this.DbmsTypePassedToLogin = dbmsType;
            this.DbmsUserNamePassedToLogin = dbmsUserName;
        }
#endregion
        
#region Begin Update

        private bool _CalledBeginUpdate = false;

        public bool CalledBeginUpdate
        {
            get
            {
                return _CalledBeginUpdate;
            }
            set
            {
                if (_CalledBeginUpdate == value)
                    return;
                _CalledBeginUpdate = value;
            }
        }

        private int _RecordHandlePassedToBeginUpdate;

        public int RecordHandlePassedToBeginUpdate
        {
            get
            {
                return _RecordHandlePassedToBeginUpdate;
            }
            set
            {
                if (_RecordHandlePassedToBeginUpdate == value)
                    return;
                _RecordHandlePassedToBeginUpdate = value;
            }
        }

        public void BeginUpdate(int recordHandle)
        {
            this.CalledBeginUpdate = true;
            this.RecordHandlePassedToBeginUpdate = recordHandle;
        }

#endregion
        
#region Cancel Transaction

        private bool _CalledCancelTransaction;

        public bool CalledCancelTransaction
        {
            get { return _CalledCancelTransaction; }
            set { _CalledCancelTransaction = value; }
        }

        private int _RecordHandlePassedToCancelTransaction;

        public int RecordHandlePassedToCancelTransaction
        {
            get
            {
                return _RecordHandlePassedToCancelTransaction;
            }
            set
            {
                if (_RecordHandlePassedToCancelTransaction == value)
                    return;
                _RecordHandlePassedToCancelTransaction = value;
            }
        }

        public void CancelTransaction(int recordHandle)
        {
            this.CalledCancelTransaction = true;
            this.RecordHandlePassedToCancelTransaction = recordHandle;
        }
        #endregion
        
#region ReleaseNoteHandle

        private bool _CalledReleaseNoteHandle = false;

        public bool CalledReleaseNoteHandle
        {
            get { return _CalledReleaseNoteHandle; }
            set { _CalledReleaseNoteHandle = value; }
        }

        private int _NoteHandlePassedToReleaseNoteHandle;

        public int NoteHandlePassedToReleaseNoteHandle
        {
            get
            {
                return _NoteHandlePassedToReleaseNoteHandle;
            }
            set
            {
                if (_NoteHandlePassedToReleaseNoteHandle == value)
                    return;
                _NoteHandlePassedToReleaseNoteHandle = value;
            }
        }

        public void ReleaseNoteHandle(int noteHandle)
        {
            this.CalledReleaseNoteHandle = true;
            this.NoteHandlePassedToReleaseNoteHandle = noteHandle;
        }

        public void CheckReleaseNoteHandle(int noteHandle)
        {
            Assert.IsTrue(this.CalledReleaseNoteHandle);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToReleaseNoteHandle);
        }

#endregion

#region ReleaseRecordHandle

        private bool _CalledReleaseRecordHandle = false;

        public bool CalledReleaseRecordHandle
        {
            get
            {
                return _CalledReleaseRecordHandle;
            }
            set
            {
                if (_CalledReleaseRecordHandle == value)
                    return;
                _CalledReleaseRecordHandle = value;
            }
        }

        private int _RecordHandlePassedToReleaseRecordHandle;

        public int RecordHandlePassedToReleaseRecordHandle
        {
            get
            {
                return _RecordHandlePassedToReleaseRecordHandle;
            }
            set
            {
                if (_RecordHandlePassedToReleaseRecordHandle == value)
                    return;
                _RecordHandlePassedToReleaseRecordHandle = value;
            }
        }

        public void ReleaseRecordHandle(int recordHandle)
        {
            this.CalledReleaseRecordHandle = true;
            this.RecordHandlePassedToReleaseRecordHandle = recordHandle;
        }

        #endregion

#region CommitRecord

        private bool _CalledCommitRecord = false;

        public bool CalledCommitRecord
        {
            get
            {
                return _CalledCommitRecord;
            }
            set
            {
                if (_CalledCommitRecord == value)
                    return;
                _CalledCommitRecord = value;
            }
        }

        private int _TransactionIdPassedToCommitRecord;

        public int TransactionIdPassedToCommitRecord
        {
            get
            {
                return _TransactionIdPassedToCommitRecord;
            }
            set
            {
                if (_TransactionIdPassedToCommitRecord == value)
                    return;
                _TransactionIdPassedToCommitRecord = value;
            }
        }

        private int _RecordHandlePassedToCommitRecord;

        public int RecordHandlePassedToCommitRecord
        {
            get
            {
                return _RecordHandlePassedToCommitRecord;
            }
            set
            {
                if (_RecordHandlePassedToCommitRecord == value)
                    return;
                _RecordHandlePassedToCommitRecord = value;
            }
        }

        public void CommitRecord(int transactionID, int recordHandle)
        {
            this.CalledCommitRecord = true;
            this.TransactionIdPassedToCommitRecord = transactionID;
            this.RecordHandlePassedToCommitRecord = recordHandle;
        }

        #endregion
        
#region GetNoteHandle

        private bool _CalledGetNoteHandle = false;

        public bool CalledGetNoteHandle
        {
            get
            {
                return _CalledGetNoteHandle;
            }
            set
            {
                if (_CalledGetNoteHandle == value)
                    return;
                _CalledGetNoteHandle = value;
            }
        }

        private int _RecordHandlePassedToGetNoteHandle;

        public int RecordHandlePassedToGetNoteHandle
        {
            get
            {
                return _RecordHandlePassedToGetNoteHandle;
            }
            set
            {
                if (_RecordHandlePassedToGetNoteHandle == value)
                    return;
                _RecordHandlePassedToGetNoteHandle = value;
            }
        }

        private int _GetNoteHandleReturn;

        public int GetNoteHandleReturn
        {
            get
            {
                return _GetNoteHandleReturn;
            }
            set
            {
                if (_GetNoteHandleReturn == value)
                    return;
                _GetNoteHandleReturn = value;
            }
        }

        public int GetNoteHandle(int recordHandle)
        {
            this.CalledGetNoteHandle = true;
            this.RecordHandlePassedToGetNoteHandle = recordHandle;
            return this.GetNoteHandleReturn;
        }

        public void CheckGetNoteHandle(int recordHandle)
        {
            Assert.IsTrue(this.CalledGetNoteHandle);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToGetNoteHandle);
        }
#endregion
        
#region GetNoteTransactionId

        private bool _CalledGetNoteTransactionId = false;

        public bool CalledGetNoteTransactionId
        {
            get
            {
                return _CalledGetNoteTransactionId;
            }
            set
            {
                if (_CalledGetNoteTransactionId == value)
                    return;
                _CalledGetNoteTransactionId = value;
            }
        }

        private int _NoteHandlePassedToGetNoteTransactionId;
        
        public int NoteHandlePassedToGetNoteTransactionId
        {
            get
            {
                return _NoteHandlePassedToGetNoteTransactionId;
            }
            set
            {
                if (_NoteHandlePassedToGetNoteTransactionId == value)
                    return;
                _NoteHandlePassedToGetNoteTransactionId = value;
            }
        }

        private int _GetNoteTransactionIdReturn;

        public int GetNoteTransactionIdReturn
        {
            get
            {
                return _GetNoteTransactionIdReturn;
            }
            set
            {
                if (_GetNoteTransactionIdReturn == value)
                    return;
                _GetNoteTransactionIdReturn = value;
            }
        }

        public int GetNoteTransactionId(int noteHandle)
        {
            this.CalledGetNoteTransactionId = true;
            this.NoteHandlePassedToGetNoteTransactionId = noteHandle;
            return this.GetNoteTransactionIdReturn;
        }

        public void CheckGetNoteTransactionId(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNoteTransactionId);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNoteTransactionId);
        }

#endregion
       
#region GetSCRID

        private bool _CalledGetSCRID = false;

        public bool CalledGetSCRID
        {
            get { return _CalledGetSCRID; }
            set { _CalledGetSCRID = value; }
        }

        private int _recordHandlePassedToGetSCRID;

        public int recordHandlePassedToGetSCRID
        {
            get
            {
                return _recordHandlePassedToGetSCRID;
            }
            set
            {
                if (_recordHandlePassedToGetSCRID == value)
                    return;
                _recordHandlePassedToGetSCRID = value;
            }
        }

        public int GetSCRID(int recordHandle)
        {
            this.CalledGetSCRID = true;
            this.recordHandlePassedToGetSCRID = recordHandle;
            return this.QueryIdList[this.PositionInQueryIdList];
        }

        public void CheckGetSCRID(int recordHandle)
        {
            Assert.IsTrue(this.CalledGetSCRID);
            Assert.AreEqual(recordHandle, this.recordHandlePassedToGetSCRID);
        }

#endregion
        
#region GetSCRRecordHandle

        private bool _CalledGetSCRRecordHandle = false;

        public bool CalledGetSCRRecordHandle
        {
            get
            {
                return _CalledGetSCRRecordHandle;
            }
            set
            {
                if (_CalledGetSCRRecordHandle == value)
                    return;
                _CalledGetSCRRecordHandle = value;
            }
        }

        private int _ScrIdPassedToGetSCRRecordHandle;

        public int ScrIdPassedToGetSCRRecordHandle
        {
            get { return _ScrIdPassedToGetSCRRecordHandle; }
            set { _ScrIdPassedToGetSCRRecordHandle = value; }
        }

        private int _GetSCRRecordHandleReturn;

        public int GetSCRRecordHandleReturn
        {
            get
            {
                return _GetSCRRecordHandleReturn;
            }
            set
            {
                if (_GetSCRRecordHandleReturn == value)
                    return;
                _GetSCRRecordHandleReturn = value;
            }
        }

        public int GetSCRRecordHandle(int scrId, int recordType)
        {
            this.CalledGetSCRRecordHandle = true;
            this.ScrIdPassedToGetSCRRecordHandle = scrId;
            return this.GetSCRRecordHandleReturn;
        }
        
#endregion

#region AddNote

        private bool _CalledAddNote = false;

        public bool CalledAddNote
        {
            get { return _CalledAddNote; }
            set { _CalledAddNote = value; }
        }

        private string _NoteTitlePassedToAddNote;

        public string NoteTitlePassedToAddNote
        {
            get
            {
                return _NoteTitlePassedToAddNote;
            }
            set
            {
                _NoteTitlePassedToAddNote = value;
            }
        }
        private string _NoteTextPassedToAddNote;

        public string NoteTextPassedToAddNote
        {
            get
            {
                return _NoteTextPassedToAddNote;
            }
            set
            {
                _NoteTextPassedToAddNote = value;
            }
        }
        private int _NoteHandlePassedToAddNote;

        public int NoteHandlePassedToAddNote
        {
            get
            {
                return _NoteHandlePassedToAddNote;
            }
            set
            {
                _NoteHandlePassedToAddNote = value;
            }
        }

        public void AddNote(string noteTitle, string noteText, int noteHandle)
        {
            this.CalledAddNote = true;
            this.NoteTitlePassedToAddNote = noteTitle;
            this.NoteTextPassedToAddNote = noteText;
            this.NoteHandlePassedToAddNote = noteHandle;
        }

        public void CheckAddNote(string noteTitle, string noteText, int noteHandle)
        {
            Assert.IsTrue(this.CalledAddNote);
            Assert.AreEqual(noteTitle, this.NoteTitlePassedToAddNote);
            Assert.AreEqual(noteText, this.NoteTextPassedToAddNote);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToAddNote);
        }

#endregion
        
#region GetDescriptionLength

        private bool _CalledGetDescriptionLength = false;

        public bool CalledGetDescriptionLength
        {
            get { return _CalledGetDescriptionLength; }
            set { _CalledGetDescriptionLength = value; }
        }

        private int _RecordHandlePassedToGetDescriptionLength;

        public int RecordHandlePassedToGetDescriptionLength
        {
            get
            {
                return _RecordHandlePassedToGetDescriptionLength;
            }
            set
            {
                if (_RecordHandlePassedToGetDescriptionLength == value)
                    return;
                _RecordHandlePassedToGetDescriptionLength = value;
            }
        }

        private int _GetDescriptionLengthReturn;

        public int GetDescriptionLengthReturn
        {
            get
            {
                if (_GetDescriptionLengthReturn == 0)
                    _GetDescriptionLengthReturn = this.DescriptionReturn.Length;
                return _GetDescriptionLengthReturn;
            }
            set
            {
                if (_GetDescriptionLengthReturn == value)
                    return;
                _GetDescriptionLengthReturn = value;
            }
        }

        public int GetDescriptionLength(int recordHandle)
        {
            this.CalledGetDescriptionLength = true;
            this.RecordHandlePassedToGetDescriptionLength = recordHandle;
            return this.GetDescriptionLengthReturn;
        }

        public void CheckGetDescriptionLength(int recordHandle)
        {
            Assert.IsTrue(this.CalledGetDescriptionLength);
        }

#endregion
        
#region GetDescriptionPart

        private bool _CalledGetDescriptionPart = false;

        public bool CalledGetDescriptionPart
        {
            get { return _CalledGetDescriptionPart; }
            set { _CalledGetDescriptionPart = value; }
        }

        private int _RecordHandlePassedToGetDescriptionPart;

        public int RecordHandlePassedToGetDescriptionPart
        {
            get
            {
                return _RecordHandlePassedToGetDescriptionPart;
            }
            set
            {
                if (_RecordHandlePassedToGetDescriptionPart == value)
                    return;
                _RecordHandlePassedToGetDescriptionPart = value;
            }
        }

        private int _CallCounterGetDescriptionPart = 0;

        public int CallCounterGetDescriptionPart
        {
            get { return _CallCounterGetDescriptionPart; }
            set { _CallCounterGetDescriptionPart = value; }
        }

        private string _DescriptionReturn;

        public string DescriptionReturn
        {
            get
            {
                return _DescriptionReturn;
            }
            set
            {
                _DescriptionReturn = value;
            }
        }

        public string GetDescriptionPart(ref int remainder, int recordHandle)
        {
            this.CalledGetDescriptionPart = true;
            this.RecordHandlePassedToGetDescriptionPart = recordHandle;

            string Part;
            int PartSize = 255;
            if (PartSize > remainder)
                PartSize = remainder;
            Part = this.DescriptionReturn.Substring(this.CallCounterGetDescriptionPart * 255, PartSize);
            
            this.CallCounterGetDescriptionPart += 1;
            remainder = this.DescriptionReturn.Length - (this.CallCounterGetDescriptionPart * 255);
            if (remainder < 0)
                remainder = 0;

            return Part;
        }

        public void CheckGetDescriptionPart(int CallCount, int recordHandle)
        {
            Assert.IsTrue(this.CalledGetDescriptionPart);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToGetDescriptionPart);
            Assert.AreEqual(CallCount, this.CallCounterGetDescriptionPart);
        }

#endregion

#region GetFieldValue

        private bool _CalledGetFieldValue = false;

        public bool CalledGetFieldValue
        {
            get
            {
                return _CalledGetFieldValue;
            }
            set
            {
                if (_CalledGetFieldValue == value)
                    return;
                _CalledGetFieldValue = value;
            }
        }

        private int _ScrIdPassedToGetFieldValue;

        public int ScrIdPassedToGetFieldValue
        {
            get
            {
                return _ScrIdPassedToGetFieldValue;
            }
            set
            {
                if (_ScrIdPassedToGetFieldValue == value)
                    return;
                _ScrIdPassedToGetFieldValue = value;
            }
        }

        private string _FieldNamePassedToGetFieldValue;

        public string FieldNamePassedToGetFieldValue
        {
            get
            {
                return _FieldNamePassedToGetFieldValue;
            }
            set
            {
                if (_FieldNamePassedToGetFieldValue == value)
                    return;
                _FieldNamePassedToGetFieldValue = value;
            }
        }

        private object _GetFieldValueReturn;

        public object GetFieldValueReturn
        {
            get
            {
                return _GetFieldValueReturn;
            }
            set
            {
                if (_GetFieldValueReturn == value)
                    return;
                _GetFieldValueReturn = value;
            }
        }

        public object GetFieldValue(int scrId, string fieldName)
        {
            this.CalledGetFieldValue = true;
            this.ScrIdPassedToGetFieldValue = scrId;
            this.FieldNamePassedToGetFieldValue = fieldName;
            return this.GetFieldValueReturn;
        }

#endregion
        
#region GetNoteText

        private bool _CalledGetNoteText = false;

        public bool CalledGetNoteText
        {
            get { return _CalledGetNoteText; }
            set { _CalledGetNoteText = value; }
        }

        private int _NoteHandlePassedToGetNoteText;

        public int NoteHandlePassedToGetNoteText
        {
            get
            {
                return _NoteHandlePassedToGetNoteText;
            }
            set
            {
                if (_NoteHandlePassedToGetNoteText == value)
                    return;
                _NoteHandlePassedToGetNoteText = value;
            }
        }

        public string GetNoteText(int noteHandle)
        {
            this.CalledGetNoteText = true;
            this.NoteHandlePassedToGetNoteText = noteHandle;
            return this.NoteList[this.PositionInNoteList].Text;
        }

        public void CheckGetNoteText(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNoteText);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNoteText);
        }

#endregion
        
#region GetNoteCreateTime

        private bool _CalledGetNoteCreateTime = false;

        public bool CalledGetNoteCreateTime
        {
            get { return _CalledGetNoteCreateTime; }
            set { _CalledGetNoteCreateTime = value; }
        }

        private int _NoteHandlePassedToGetNoteCreateTime;

        public int NoteHandlePassedToGetNoteCreateTime
        {
            get
            {
                return _NoteHandlePassedToGetNoteCreateTime;
            }
            set
            {
                if (_NoteHandlePassedToGetNoteCreateTime == value)
                    return;
                _NoteHandlePassedToGetNoteCreateTime = value;
            }
        }

        public string GetNoteCreateTime(int noteHandle)
        {
            this.CalledGetNoteCreateTime = true;
            this.NoteHandlePassedToGetNoteCreateTime = noteHandle;
            return this.NoteList[this.PositionInNoteList].CreationTime;
        }

        public void CheckGetNoteCreateTime(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNoteCreateTime);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNoteCreateTime);
        }

#endregion
        
#region GetNoteAuthor

        private bool _CalledGetNoteAuthor = false;

        public bool CalledGetNoteAuthor
        {
            get { return _CalledGetNoteAuthor; }
            set { _CalledGetNoteAuthor = value; }
        }

        private int _NoteHandlePassedToGetNoteAuthor;

        public int NoteHandlePassedToGetNoteAuthor
        {
            get
            {
                return _NoteHandlePassedToGetNoteAuthor;
            }
            set
            {
                if (_NoteHandlePassedToGetNoteAuthor == value)
                    return;
                _NoteHandlePassedToGetNoteAuthor = value;
            }
        }

        public string GetNoteAuthor(int noteHandle)
        {
            this.CalledGetNoteAuthor = true;
            this.NoteHandlePassedToGetNoteAuthor = noteHandle;
            return this.NoteList[this.PositionInNoteList].Author;
        }
        
        public void CheckGetNoteAuthor(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNoteAuthor);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNoteAuthor);
        }

#endregion
        
#region GetNoteTitle

        private bool _CalledGetNoteTitle = false;

        public bool CalledGetNoteTitle
        {
            get { return _CalledGetNoteTitle; }
            set { _CalledGetNoteTitle = value; }
        }

        private int _NoteHandlePassedToGetNoteTitle;

        public int NoteHandlePassedToGetNoteTitle
        {
            get
            {
                return _NoteHandlePassedToGetNoteTitle;
            }
            set
            {
                if (_NoteHandlePassedToGetNoteTitle == value)
                    return;
                _NoteHandlePassedToGetNoteTitle = value;
            }
        }

        public string GetNoteTitle(int noteHandle)
        {
            this.CalledGetNoteTitle = true;
            this.NoteHandlePassedToGetNoteTitle = noteHandle;
            return this.NoteList[this.PositionInNoteList].Title;
        }

        public void CheckGetNoteTitle(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNoteTitle);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNoteTitle);
        }

#endregion

#region GetNextNote

        private bool _CalledGetNextNote = false;

        public bool CalledGetNextNote
        {
            get { return _CalledGetNextNote; }
            set { _CalledGetNextNote = value; }
        }

        private int _NoteHandlePassedToGetNextNote;

        public int NoteHandlePassedToGetNextNote
        {
            get
            {
                return _NoteHandlePassedToGetNextNote;
            }
            set
            {
                if (_NoteHandlePassedToGetNextNote == value)
                    return;
                _NoteHandlePassedToGetNextNote = value;
            }
        }

        private List<Note> _NoteList;

        public List<Note> NoteList
        {
            get
            {
                return _NoteList;
            }
            set
            {
                _NoteList = value;
            }
        }

        private int _PositionInNoteList = -1;

        public int PositionInNoteList
        {
            get
            {
                return _PositionInNoteList;
            }
            set
            {
                if (_PositionInNoteList == value)
                    return;
                _PositionInNoteList = value;
            }
        }

        public bool GetNextNote(int noteHandle)
        {
            this.CalledGetNextNote = true;
            this.NoteHandlePassedToGetNextNote = noteHandle;
            if (this.PositionInNoteList + 1 == this.NoteList.Count)
                return false;
            this.PositionInNoteList += 1;
            return true;
        }
        
        public void CheckGetNextNote(int noteHandle)
        {
            Assert.IsTrue(this.CalledGetNextNote);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToGetNextNote);
        }

#endregion
        
#region InitalizeNoteList

        private bool _CalledInitalizeNoteList = false;

        public bool CalledInitalizeNoteList
        {
            get { return _CalledInitalizeNoteList; }
            set { _CalledInitalizeNoteList = value; }
        }

        private int _NoteHandlePassedToInitalizeNoteList;

        public int NoteHandlePassedToInitalizeNoteList
        {
            get
            {
                return _NoteHandlePassedToInitalizeNoteList;
            }
            set
            {
                if (_NoteHandlePassedToInitalizeNoteList == value)
                    return;
                _NoteHandlePassedToInitalizeNoteList = value;
            }
        }

        public void InitalizeNoteList(int noteHandle)
        {
            this.CalledInitalizeNoteList = true;
            this.NoteHandlePassedToInitalizeNoteList = noteHandle;

        }

        public void CheckInitalizeNoteList(int noteHandle)
        {
            Assert.IsTrue(this.CalledInitalizeNoteList);
            Assert.AreEqual(noteHandle, this.NoteHandlePassedToInitalizeNoteList);
        }
        
#endregion
        
#region AllocateRecordHandle

        private bool _CalledAllocateRecordHandle = false;

        public bool CalledAllocateRecordHandle
        {
            get { return _CalledAllocateRecordHandle; }
            set { _CalledAllocateRecordHandle = value; }
        }

        private int _AllocateRecordHandleReturn;

        public int AllocateRecordHandleReturn
        {
            get
            {
                return _AllocateRecordHandleReturn;
            }
            set
            {
                if (_AllocateRecordHandleReturn == value)
                    return;
                _AllocateRecordHandleReturn = value;
            }
        }

        public int AllocateRecordHandle()
        {
            this.CalledAllocateRecordHandle = true;
            return this.AllocateRecordHandleReturn;
        }
        
        public void CheckAllocateRecordHandle()
        {
            Assert.IsTrue(this.CalledAllocateRecordHandle);
        }

#endregion
        
#region InitalizeRecordList

        private bool _CalledInitalizeRecordList = false;

        public bool CalledInitalizeRecordList
        {
            get { return _CalledInitalizeRecordList; }
            set { _CalledInitalizeRecordList = value; }
        }

        private int _RecordHandlePassedToInitalizeRecordList;

        public int RecordHandlePassedToInitalizeRecordList
        {
            get
            {
                return _RecordHandlePassedToInitalizeRecordList;
            }
            set
            {
                if (_RecordHandlePassedToInitalizeRecordList == value)
                    return;
                _RecordHandlePassedToInitalizeRecordList = value;
            }
        }

        private string _QueryNamePassedToInitalizeRecordList;

        public string QueryNamePassedToInitalizeRecordList
        {
            get
            {
                return _QueryNamePassedToInitalizeRecordList;
            }
            set
            {
                if (_QueryNamePassedToInitalizeRecordList == value)
                    return;
                _QueryNamePassedToInitalizeRecordList = value;
            }
        }

        public void InitalizeRecordList(int recordHandle, string queryName)
        {
            this.CalledInitalizeRecordList = true;
            this.QueryNamePassedToInitalizeRecordList = queryName;
            this.RecordHandlePassedToInitalizeRecordList = recordHandle;
        }

        public void CheckInitalizeRecordList(int recordHandle, string queryName)
        {
            Assert.IsTrue(this.CalledInitalizeRecordList);
            Assert.AreEqual(queryName, this.QueryNamePassedToInitalizeRecordList);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToInitalizeRecordList);
        }

#endregion
        
#region GetNextScrId

        private bool _CalledGetNextScrId = false;

        public bool CalledGetNextScrId
        {
            get { return _CalledGetNextScrId; }
            set { _CalledGetNextScrId = value; }
        }

        private int[] _QueryIdList;

        public int[] QueryIdList
        {
            get
            {
                return _QueryIdList;
            }
            set
            {
                if (_QueryIdList == value)
                    return;
                _QueryIdList = value;
            }
        }

        private int _PositionInQueryIdList = -1;

        public int PositionInQueryIdList
        {
            get
            {
                return _PositionInQueryIdList;
            }
            set
            {
                if (_PositionInQueryIdList == value)
                    return;
                _PositionInQueryIdList = value;
            }
        }

        private int _RecordHandlePassedToGetNextScrId;

        public int RecordHandlePassedToGetNextScrId
        {
            get
            {
                return _RecordHandlePassedToGetNextScrId;
            }
            set
            {
                if (_RecordHandlePassedToGetNextScrId == value)
                    return;
                _RecordHandlePassedToGetNextScrId = value;
            }
        }

        public bool GetNextScrId(int recordHandle)
        {
            this.CalledGetNextScrId = true;
            this.RecordHandlePassedToGetNextScrId = recordHandle;
            if (this.PositionInQueryIdList + 1 == this.QueryIdList.Length)
                return false;
            this.PositionInQueryIdList += 1;
            return true;
        }

        public void CheckGetNextScrId(int recordHandle)
        {
            Assert.IsTrue(this.CalledGetNextScrId);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToGetNextScrId);
        }

#endregion

#region GetFieldType

        private bool _CalledGetFieldType = false;

        public bool CalledGetFieldType
        {
            get { return _CalledGetFieldType; }
            set { _CalledGetFieldType = value; }
        }

        private string _FieldNamePassedToGetFieldType;

        public string FieldNamePassedToGetFieldType
        {
            get { return _FieldNamePassedToGetFieldType; }
            set { _FieldNamePassedToGetFieldType = value; }
        }

        private int _GetFieldTypeReturn;

        public int GetFieldTypeReturn
        {
            get
            {
                return _GetFieldTypeReturn;
            }
            set
            {
                if (_GetFieldTypeReturn == value)
                    return;
                _GetFieldTypeReturn = value;
            }
        }

        public int GetFieldType(string fieldName)
        {
            this.CalledGetFieldType = true;
            this.FieldNamePassedToGetFieldType = fieldName;
            return this.GetFieldTypeReturn;
        }

        #endregion
        
#region GetFieldTransactionId

        private bool _CalledGetFieldTransactionId = false;

        public bool CalledGetFieldTransactionId
        {
            get
            {
                return _CalledGetFieldTransactionId;
            }
            set
            {
                if (_CalledGetFieldTransactionId == value)
                    return;
                _CalledGetFieldTransactionId = value;
            }
        }

        private string _FieldNamePassedToGetFieldTransactionId;

        public string FieldNamePassedToGetFieldTransactionId
        {
            get
            {
                return _FieldNamePassedToGetFieldTransactionId;
            }
            set
            {
                if (_FieldNamePassedToGetFieldTransactionId == value)
                    return;
                _FieldNamePassedToGetFieldTransactionId = value;
            }
        }

        private int _RecordHandlePassedToGetFieldTransactionId;

        public int RecordHandlePassedToGetFieldTransactionId
        {
            get
            {
                return _RecordHandlePassedToGetFieldTransactionId;
            }
            set
            {
                if (_RecordHandlePassedToGetFieldTransactionId == value)
                    return;
                _RecordHandlePassedToGetFieldTransactionId = value;
            }
        }

        private int _GetFieldTransactionIdReturn;

        public int GetFieldTransactionIdReturn
        {
            get
            {
                return _GetFieldTransactionIdReturn;
            }
            set
            {
                if (_GetFieldTransactionIdReturn == value)
                    return;
                _GetFieldTransactionIdReturn = value;
            }
        }

        public int GetFieldTransactionId(string fieldName, int recordHandle)
        {
            this.CalledGetFieldTransactionId = true;
            this.FieldNamePassedToGetFieldTransactionId = fieldName;
            this.RecordHandlePassedToGetFieldTransactionId = recordHandle;
            return this.GetFieldTransactionIdReturn;
        }

#endregion
        
#region SaveNumericFieldValue

        private bool _CalledSaveNumericFieldValue = false;

        public bool CalledSaveNumericFieldValue
        {
            get
            {
                return _CalledSaveNumericFieldValue;
            }
            set
            {
                if (_CalledSaveNumericFieldValue == value)
                    return;
                _CalledSaveNumericFieldValue = value;
            }
        }

        private string _FieldNamePassedToSaveNumericFieldValue;

        public string FieldNamePassedToSaveNumericFieldValue
        {
            get
            {
                return _FieldNamePassedToSaveNumericFieldValue;
            }
            set
            {
                if (_FieldNamePassedToSaveNumericFieldValue == value)
                    return;
                _FieldNamePassedToSaveNumericFieldValue = value;
            }
        }

        private int _NewValuePassedToSaveNumericFieldValue;

        public int NewValuePassedToSaveNumericFieldValue
        {
            get
            {
                return _NewValuePassedToSaveNumericFieldValue;
            }
            set
            {
                if (_NewValuePassedToSaveNumericFieldValue == value)
                    return;
                _NewValuePassedToSaveNumericFieldValue = value;
            }
        }

        private int _RecordHandlePassedToSaveNumericFieldValue;

        public int RecordHandlePassedToSaveNumericFieldValue
        {
            get
            {
                return _RecordHandlePassedToSaveNumericFieldValue;
            }
            set
            {
                if (_RecordHandlePassedToSaveNumericFieldValue == value)
                    return;
                _RecordHandlePassedToSaveNumericFieldValue = value;
            }
        }

        public void SaveNumericFieldValue(string fieldName, int newValue, int recordHandle)
        {
            this.CalledSaveNumericFieldValue = true;
            this.FieldNamePassedToSaveNumericFieldValue = fieldName;
            this.NewValuePassedToSaveNumericFieldValue = newValue;
            this.RecordHandlePassedToSaveNumericFieldValue = recordHandle;
        }

#endregion

#region SaveStringFieldValue

        private bool _CalledSaveStringFieldValue = false;

        public bool CalledSaveStringFieldValue
        {
            get
            {
                return _CalledSaveStringFieldValue;
            }
            set
            {
                if (_CalledSaveStringFieldValue == value)
                    return;
                _CalledSaveStringFieldValue = value;
            }
        }

        private string _FieldNamePassedToSaveStringFieldValue;
        
        public string FieldNamePassedToSaveStringFieldValue
        {
            get
            {
                return _FieldNamePassedToSaveStringFieldValue;
            }
            set
            {
                if (_FieldNamePassedToSaveStringFieldValue == value)
                    return;
                _FieldNamePassedToSaveStringFieldValue = value;
            }
        }

        private string _NewValuePassedToSaveStringFieldValue;

        public string NewValuePassedToSaveStringFieldValue
        {
            get
            {
                return _NewValuePassedToSaveStringFieldValue;
            }
            set
            {
                if (_NewValuePassedToSaveStringFieldValue == value)
                    return;
                _NewValuePassedToSaveStringFieldValue = value;
            }
        }

        private int _RecordHandlePassedToSaveStringFieldValue;

        public int RecordHandlePassedToSaveStringFieldValue
        {
            get
            {
                return _RecordHandlePassedToSaveStringFieldValue;
            }
            set
            {
                if (_RecordHandlePassedToSaveStringFieldValue == value)
                    return;
                _RecordHandlePassedToSaveStringFieldValue = value;
            }
        }

        public void SaveStringFieldValue(string fieldName, string newValue, int recordHandle)
        {
            this.CalledSaveStringFieldValue = true;
            this.FieldNamePassedToSaveStringFieldValue = fieldName;
            this.NewValuePassedToSaveStringFieldValue = newValue;
            this.RecordHandlePassedToSaveStringFieldValue = recordHandle;
        }

        #endregion
        #endregion
        
#region Checks

        public void CheckGetFieldValue(string fieldName, int scrId)
        {
            Assert.IsTrue(this.CalledGetFieldValue);
            Assert.AreEqual(fieldName, this.FieldNamePassedToGetFieldValue);
            Assert.AreEqual(scrId, this.ScrIdPassedToGetFieldValue);
        }
        public void CheckGetSCRRecordHandle(int scrId)
        {
            Assert.IsTrue(this.CalledGetSCRRecordHandle);
            Assert.AreEqual(scrId, this.ScrIdPassedToGetSCRRecordHandle);
        }
        public void CheckGetFieldType(string fieldName)
        {
            Assert.IsTrue(this.CalledGetFieldType);
            Assert.AreEqual(fieldName, this.FieldNamePassedToGetFieldType);
        }
        public void CheckBeginUpdate(int recordHandle)
        {
            Assert.IsTrue(this.CalledBeginUpdate);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToBeginUpdate);
        }
        public void CheckGetFieldTransactionId(string fieldName, int recordHandle)
        {
            Assert.IsTrue(this.CalledGetFieldTransactionId);
            Assert.AreEqual(fieldName, this.FieldNamePassedToGetFieldTransactionId);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToGetFieldTransactionId);
        }
        public void CheckSaveStringFieldValue(string fieldName, string fieldValue, int recordHandle)
        {
            Assert.IsTrue(this.CalledSaveStringFieldValue);
            Assert.AreEqual(fieldName, this.FieldNamePassedToSaveStringFieldValue);
            Assert.AreEqual(fieldValue, this.NewValuePassedToSaveStringFieldValue);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToSaveStringFieldValue);
        }
        public void CheckSaveNumericFieldValue(string fieldName, int fieldValue, int recordHandle)
        {
            Assert.IsTrue(this.CalledSaveNumericFieldValue);
            Assert.AreEqual(fieldName, this.FieldNamePassedToSaveNumericFieldValue);
            Assert.AreEqual(fieldValue, this.NewValuePassedToSaveNumericFieldValue);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToSaveNumericFieldValue);
        }
        public void CheckCommitRecord(int recordHandle, int transactionId)
        {
            Assert.IsTrue(this.CalledCommitRecord);
            Assert.AreEqual(transactionId, this.TransactionIdPassedToCommitRecord);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToCommitRecord);
        }
        public void CheckReleaseRecordHandle(int recordHandle)
        {
            Assert.IsTrue(this.CalledReleaseRecordHandle);
            Assert.AreEqual(recordHandle, this.RecordHandlePassedToReleaseRecordHandle);
        }

#endregion


        #region IPVCSToolKit Members


        public int GetNumericFieldValue(int recordHandle, string fieldName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void NewRecordBegin(int recordHandle, int recordType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void NewRecordCommit(int recordHandle, ref int transactionID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    public class Note
    {
        private string _Title;
        private string _Text;
        private string _CreationTime;
        private string _Author;

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title == value)
                    return;
                _Title = value;
            }
        }
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (_Text == value)
                    return;
                _Text = value;
            }
        }
        public string CreationTime
        {
            get
            {
                return _CreationTime;
            }
            set
            {
                if (_CreationTime == value)
                    return;
                _CreationTime = value;
            }
        }
        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                if (_Author == value)
                    return;
                _Author = value;
            }
        }


        /// <summary>
        /// Creates a new instance of Note
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="creationTime"></param>
        /// <param name="author"></param>
        public Note(string title, string text, string creationTime, string author)
        {
            _Title = title;
            _Text = text;
            _CreationTime = creationTime;
            _Author = author;
        }
    }
}
