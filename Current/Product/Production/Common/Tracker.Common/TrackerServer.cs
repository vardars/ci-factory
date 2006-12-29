#region Usings

using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Data;
using System.Xml;
using System.Runtime.InteropServices;

#endregion

namespace Tracker.Common
 {
     
    public class TrackerServer
    {


#region Fields

        private int _DBMSLoginMode;
        private string _DBMSPassword;
        private string _DBMSServer;
        private string _DBMSType;
        private string _DBMSUserName;
        private string _ProjectName;
        private string _UserName;
        private string _UserPWD;
        private IPVCSToolKit _ToolKit;
        private ServerHelper _Helper;

#endregion

#region Properties

        private ServerHelper Helper
        {
            get
            {
                if (_Helper == null)
                    _Helper = new ServerHelper();
                return _Helper;
            }
        }

        private IPVCSToolKit ToolKit
        {
            get
            {
                return _ToolKit;
            }
            set
            {
                if (_ToolKit == value)
                    return;
                _ToolKit = value;
            }
        }

        public int DBMSLoginMode
        {
            get
            {
                return this._DBMSLoginMode;
            }
            set
            {
                this._DBMSLoginMode = value;
            }
        }

        public string DBMSPassword
        {
            get
            {
                return this._DBMSPassword;
            }
            set
            {
                this._DBMSPassword = value;
            }
        }

        public string DBMSServer
        {
            get
            {
                return this._DBMSServer;
            }
            set
            {
                this._DBMSServer = value;
            }
        }

        public string DBMSType
        {
            get
            {
                return this._DBMSType;
            }
            set
            {
                this._DBMSType = value;
            }
        }

        public string DBMSUserName
        {
            get
            {
                return this._DBMSUserName;
            }
            set
            {
                this._DBMSUserName = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return this._ProjectName;
            }
            set
            {
                this._ProjectName = value;
            }
        }

        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        public string UserPWD
        {
            get
            {
                return this._UserPWD;
            }
            set
            {
                this._UserPWD = value;
            }
        }

        #endregion

#region Constructors


        /// <summary>
        /// Creates a new instance of TrackerServer
        /// </summary>
        /// <param name="dBMSLoginMode"></param>
        /// <param name="dBMSName"></param>
        /// <param name="dBMSPassword"></param>
        /// <param name="dBMSServer"></param>
        /// <param name="dBMSType"></param>
        /// <param name="dBMSUserName"></param>
        /// <param name="projectName"></param>
        /// <param name="userName"></param>
        /// <param name="userPWD"></param>
        /// <param name="toolKit"></param>
        public TrackerServer(int dBMSLoginMode, string dBMSPassword, string dBMSServer, string dBMSType, string dBMSUserName, string projectName, string userName, string userPWD, IPVCSToolKit toolKit)
        {
            _DBMSLoginMode = dBMSLoginMode;
            _DBMSPassword = dBMSPassword;
            _DBMSServer = dBMSServer;
            _DBMSType = dBMSType;
            _DBMSUserName = dBMSUserName;
            _ProjectName = projectName;
            _UserName = userName;
            _UserPWD = userPWD;
            _ToolKit = toolKit;
        }


        /// <summary>
        /// Creates a new instance of TrackerServer
        /// </summary>
        /// <param name="toolKit"></param>
        public TrackerServer(IPVCSToolKit toolKit)
        {
            _ToolKit = toolKit;
        }

#endregion
     
#region State Control

        public void Login()
        {
            this.ToolKit.Login(this.UserName, this.UserPWD, this.ProjectName, this.DBMSUserName, this.DBMSPassword, this.DBMSServer, this.DBMSType, this.DBMSLoginMode);
        }

        public void Logout()
        {
            this.ToolKit.Logout();
        }
        #endregion
        
#region Public Actions
        public void AddNote(int scrId, string noteTitle, string noteText)
        {
            int NoteHandle = 0;
            int RecordHandle = 0;
            int TransactionId;
            try
            {
                RecordHandle = this.ToolKit.GetSCRRecordHandle(scrId, 1);

                NoteHandle = this.ToolKit.GetNoteHandle(RecordHandle);

                this.ToolKit.BeginUpdate(RecordHandle);

                TransactionId = this.ToolKit.GetNoteTransactionId(NoteHandle);

                this.ToolKit.AddNote(noteTitle, noteText, NoteHandle);

                this.ToolKit.CommitRecord(TransactionId, RecordHandle);
            }
            catch
            {
                if (RecordHandle != 0)
                    this.ToolKit.CancelTransaction(RecordHandle);
            }
            finally
            {
                if (RecordHandle != 0)
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                if (NoteHandle != 0)
                    this.ToolKit.ReleaseNoteHandle(NoteHandle);
            }
        }

        public string GetDescription(int scrId)
        {
            int RecordHandle = 0;
            try
            {
                StringBuilder Builder = new StringBuilder();
                RecordHandle = this.ToolKit.GetSCRRecordHandle(scrId, 1);

                int Remainder = this.ToolKit.GetDescriptionLength(RecordHandle);

                while (Remainder != 0)
                {
                    string DescriptionPart = this.ToolKit.GetDescriptionPart(ref Remainder, RecordHandle);
                    Builder.Append(DescriptionPart);
                }
                return Builder.ToString();
            }
            finally
            {
                if (RecordHandle != 0)
                {
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                }
            }
        }

        
        //public Hashtable GetFieldNames()
        //{
        //    int FieldType = 0;
        //    Hashtable FieldTable = new Hashtable();
        //    string FieldName;

        //    this.ToolKit.GetFieldNamesExtracted();

        //    while (true)
        //    {
        //        int Status;
        //        FieldName = this.MakeBigEmptyString(255);
        //        Status = TrackerServer.TrkGetNextField(this.TrackerHandle, FieldName.Length, ref FieldName, ref FieldType);
        //        if ((StatusCodes)Status != StatusCodes.Success && (StatusCodes)Status != StatusCodes.EndOfList)
        //            this.CheckStatus("Unable to get next field information.", Status);
        //        else if ((StatusCodes)Status == StatusCodes.EndOfList)
        //            break;
        //        if (FieldType == 3)
        //        {
        //            FieldTable.Add(FieldName, typeof(int));
        //        }
        //        else
        //        {
        //            FieldTable.Add(FieldName, typeof(string));
        //        }
        //    }
        //    return FieldTable;
        //}

        public int GetFieldValueInteger(int scrId, string fieldName)
        {
            return (int)this.ToolKit.GetFieldValue(scrId, fieldName);
        }

        public string GetFieldValueString(int scrId, string fieldName)
        {
            return (string)this.ToolKit.GetFieldValue(scrId, fieldName);
        }

        public StringCollection GetNoteList(int scrId)
        {
            int NoteHandle = 0;
            int RecordHandle = 0;

            try
            {
                RecordHandle = this.ToolKit.GetSCRRecordHandle(scrId, 1);
                NoteHandle = this.ToolKit.GetNoteHandle(RecordHandle);

                this.ToolKit.InitalizeNoteList(NoteHandle);

                StringCollection NoteList = new StringCollection();
                while (this.ToolKit.GetNextNote(NoteHandle))
                {
                    string NoteTitle = this.ToolKit.GetNoteTitle(NoteHandle);
                    string NoteAuthor = this.ToolKit.GetNoteAuthor(NoteHandle);
                    string NoteCreationTime = this.ToolKit.GetNoteCreateTime(NoteHandle);
                    string NoteText = this.ToolKit.GetNoteText(NoteHandle);

                    NoteList.Add(string.Format("{0} ({1}) {2}, {3}", NoteTitle, NoteAuthor, NoteCreationTime, NoteText));
                }
                return NoteList;
            }
            finally
            {
                if (RecordHandle != 0)
                {
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                }
                if (NoteHandle != 0)
                {
                    this.ToolKit.ReleaseNoteHandle(NoteHandle);
                }
            }
        }

        public int[] GetSCRIDListFromQuery(string queryName)
        {
            int RecordHandle = 0;

            try
            {
                RecordHandle = this.ToolKit.AllocateRecordHandle();

                this.ToolKit.InitalizeRecordList(RecordHandle, queryName);

                ArrayList IdList = new ArrayList();
                while (this.ToolKit.GetNextScrId(RecordHandle))
                {
                    IdList.Add(this.ToolKit.GetSCRID(RecordHandle));
                }
                if (IdList.Count > 0)
                {
                    return (int[])IdList.ToArray(typeof(int));
                }
                return new int[0];
            }
            finally
            {
                if (RecordHandle != 0)
                {
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                }
            }
        }

        public void SaveNumericFieldValue(int scrId, string fieldName, int newValue)
        {
            int RecordHandle = 0;
            try
            {
                fieldName = fieldName.Trim();

                RecordHandle = this.ToolKit.GetSCRRecordHandle(scrId, 1);
                int FieldType = this.ToolKit.GetFieldType(fieldName);
                this.ToolKit.BeginUpdate(RecordHandle);

                int TransactionId = this.ToolKit.GetFieldTransactionId(fieldName, RecordHandle);

                if ((int)ServerHelper._TrkFieldType.TRK_FIELD_TYPE_NUMBER == FieldType)
                {
                    this.ToolKit.SaveNumericFieldValue(fieldName, newValue, RecordHandle);
                }
                else
                {
                    throw new InvalidOperationException("The field value is a string not a numeral.");
                }
                this.ToolKit.CommitRecord(TransactionId, RecordHandle);
            }
            finally
            {
                if (RecordHandle != 0)
                {
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                }
            }
        }

        public void SaveStringFieldValue(int scrId, string fieldName, string newValue)
        {
            int RecordHandle = 0;
            try
            {
                fieldName = fieldName.Trim();

                RecordHandle = this.ToolKit.GetSCRRecordHandle(scrId, 1);
                int FieldType = this.ToolKit.GetFieldType(fieldName);
                this.ToolKit.BeginUpdate(RecordHandle);

                int TransactionId = this.ToolKit.GetFieldTransactionId(fieldName, RecordHandle);

                if ((int)ServerHelper._TrkFieldType.TRK_FIELD_TYPE_NUMBER == FieldType)
                {
                    throw new InvalidOperationException("The field value is a string not a numeral.");
                }
                else
                {
                    this.ToolKit.SaveStringFieldValue(fieldName, newValue, RecordHandle);
                }
                this.ToolKit.CommitRecord(TransactionId, RecordHandle);
            }
            finally
            {
                if (RecordHandle != 0)
                {
                    this.ToolKit.ReleaseRecordHandle(RecordHandle);
                }
            }
        }

        public void NewRecordBegin(ref int RecordHandle)
        {
            RecordHandle = 0;

            try
            {
                RecordHandle = this.ToolKit.AllocateRecordHandle();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                this.ToolKit.ReleaseRecordHandle(RecordHandle);
                return;
            }

            try
            {
                this.ToolKit.NewRecordBegin(RecordHandle, 1);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                this.ToolKit.CancelTransaction(RecordHandle);
                return;
            }
        }

        public int NewRecordCommit(int RecordHandle)
        {
            try
            {
                int transactionID = 0;
                this.ToolKit.NewRecordCommit(RecordHandle, ref transactionID);
                int Id = this.ToolKit.GetNumericFieldValue(RecordHandle, "Id");

                return Id;
            }
            finally
            {
                this.ToolKit.ReleaseRecordHandle(RecordHandle);
            }
            return 0;
        }

        public void SaveStringFieldValue(string FieldName, string newValue, int RecordHandle)
        {
            this.ToolKit.SaveStringFieldValue(FieldName, newValue, RecordHandle);
        }
#endregion

    }
}