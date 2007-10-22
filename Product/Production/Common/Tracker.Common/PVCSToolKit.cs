using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace Tracker.Common
{
    public class PVCSToolKit : IPVCSToolKit
    {
        #region Fields

        private int _TrackerHandle;
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

        private int TrackerHandle
        {
            get
            {
                return this._TrackerHandle;
            }
            set
            {
                this._TrackerHandle = value;
            }
        }
        #endregion

        public PVCSToolKit()
        {

        }

        #region State Control

        public void Login(string userName, string password, string projectName, string dbmsUserName, string dbmsPassword, string dbmsServer, string dbmsType, int dbmsLoginMode)
        {
            int Handle = 0;
            int Status = PVCSToolKit.TrkHandleAlloc(ServerHelper.TRK_VERSION_ID, ref Handle);
            this.Helper.CheckStatus("Failed to allocate tracker handle.", Status);
            this.TrackerHandle = Handle;
            Status = PVCSToolKit.TrkProjectLogin(this.TrackerHandle, ref userName, ref password, ref projectName, ref dbmsType, ref dbmsServer, ref dbmsUserName, ref dbmsPassword, dbmsLoginMode);
            this.Helper.CheckStatus("Unable to login.", Status);
        }

        public void Logout()
        {
            int Status = PVCSToolKit.TrkProjectLogout(this.TrackerHandle);
            this.Helper.CheckStatus("Unable to logout.", Status);
            int Handle = this.TrackerHandle;
            PVCSToolKit.TrkHandleFree(ref Handle);
        }
        #endregion

        #region Public Actions

        public void AddNote(string noteTitle, string noteText, int noteHandle)
        {
            int RecordSize = 0;
            int Status = PVCSToolKit.TrkAddNewNote(noteHandle);
            this.Helper.CheckStatus("Unable to add new note.", Status);

            Status = PVCSToolKit.TrkSetNoteTitle(noteHandle, ref noteTitle);
            this.Helper.CheckStatus("Unable to set note title.", Status);

            Status = PVCSToolKit.TrkSetNoteData(noteHandle, noteText.Length, ref noteText, RecordSize);
            this.Helper.CheckStatus("Unable to set note text.", Status);
        }

        public void CancelTransaction(int recordHandle)
        {
            PVCSToolKit.TrkRecordCancelTransaction(recordHandle);
        }

        public void ReleaseNoteHandle(int noteHandle)
        {
            PVCSToolKit.TrkNoteHandleFree(ref noteHandle);
        }

        public void ReleaseRecordHandle(int recordHandle)
        {
            PVCSToolKit.TrkRecordHandleFree(ref recordHandle);
        }

        public int GetNoteHandle(int recordHandle)
        {
            int NoteHandle = 0;
            int Status = PVCSToolKit.TrkNoteHandleAlloc(recordHandle, ref NoteHandle);
            this.Helper.CheckStatus("Unable to get Note Handle.", Status);
            return NoteHandle;
        }

        public void BeginUpdate(int recordHandle)
        {
            int Status = PVCSToolKit.TrkUpdateRecordBegin(recordHandle);
            this.Helper.CheckStatus("Unable to begin record update.", Status);
        }

        public int GetNoteTransactionId(int noteHandle)
        {
            int TransactionId = 0;
            int Status = PVCSToolKit.TrkGetNoteTransactionID(noteHandle, ref TransactionId);
            if ((int)ServerHelper._TrkError.TRK_E_NO_CURRENT_NOTE != Status)
            {
                this.Helper.CheckStatus("Unable to get Transaction ID.", Status);
            }
            return TransactionId;
        }

        public void CommitRecord(int transactionID, int recordHandle)
        {
            int Status = PVCSToolKit.TrkUpdateRecordCommit(recordHandle, ref transactionID);
            this.Helper.CheckStatus("Unable to commit record.", Status);
        }

        public int GetSCRID(int recordHandle)
        {
            int num2 = 0;
            string text1 = "Id";
            int Status = PVCSToolKit.TrkGetNumericFieldValue(recordHandle, ref text1, ref num2);
            this.Helper.CheckStatus("Unable to retrieve field value.", Status);
            return num2;
        }

        public int AllocateRecordHandle()
        {
            int RecordHandle = 0;
            int Status = PVCSToolKit.TrkRecordHandleAlloc(this.TrackerHandle, ref RecordHandle);
            this.Helper.CheckStatus("Unable to retrieve SCR handle.", Status);
            return RecordHandle;
        }
        
        public int GetSCRRecordHandle(int scrId, [Optional] int recordType /* = 1 */)
        {
            int RecordHandle = this.AllocateRecordHandle();
            int Status = PVCSToolKit.TrkGetSingleRecord(RecordHandle, scrId, recordType);
            this.Helper.CheckStatus("Unable to retrieve SCR handle.", Status);
            return RecordHandle;
        }

        public int GetDescriptionLength(int recordHandle)
        {
            int Remainder = 0;
            int Status = PVCSToolKit.TrkGetDescriptionDataLength(recordHandle, ref Remainder);
            this.Helper.CheckStatus("Unable to get description length.", Status);
            return Remainder;
        }

        public string GetDescriptionPart(ref int remainder, int recordHandle)
        {
            string DescriptionPart = this.Helper.MakeBigEmptyString(ServerHelper.MAX_BUFFER_LENGTH);
            int Status = PVCSToolKit.TrkGetDescriptionData(recordHandle, DescriptionPart.Length, ref DescriptionPart, ref remainder);
            if (Status != (int)ServerHelper._TrkError.TRK_E_DATA_TRUNCATED)
            {
                this.Helper.CheckStatus("Unable to retrieve part of the description.", Status);
            }
            DescriptionPart = this.Helper.CleanupString(DescriptionPart);
            return DescriptionPart;
        }

        public int GetFieldType(string fieldName)
        {
            int FieldType = 0;
            int Status = PVCSToolKit.TrkGetFieldType(this.TrackerHandle, ref fieldName, 1, ref FieldType);
            this.Helper.CheckStatus("Unable to retrieve field type: " + fieldName, Status);
            return FieldType;
        }

        public object GetFieldValue(int scrId, string fieldName)
        {
            int RecordHandle = 0;

            try
            {
                int Status = 0;
                RecordHandle = this.GetSCRRecordHandle(scrId, 1);

                int FieldType = this.GetFieldType(fieldName);
                if ((int)ServerHelper._TrkFieldType.TRK_FIELD_TYPE_NUMBER == FieldType)
                {
                    int IntegerValue = 0;
                    Status = PVCSToolKit.TrkGetNumericFieldValue(RecordHandle, ref fieldName, ref IntegerValue);
                    this.Helper.CheckStatus("Unable to retrieve field value.", Status);
                    return IntegerValue;
                }
                else // ServerHelper._TrkFieldType.TRK_FIELD_TYPE_STRING or if other type, then convert to string
                {
                    string StringValue = this.Helper.MakeBigEmptyString(ServerHelper.MAX_BUFFER_LENGTH);
                    Status = PVCSToolKit.TrkGetStringFieldValue(RecordHandle, ref fieldName, StringValue.Length, ref StringValue);
                    this.Helper.CheckStatus("Unable to retrieve field value.", Status);
                    return this.Helper.CleanupString(StringValue);
                }
            }
            finally
            {
                if (0 != RecordHandle)
                {
                    this.ReleaseRecordHandle(RecordHandle);
                }
            }
        }

        public void InitalizeNoteList(int noteHandle)
        {
            int Status = PVCSToolKit.TrkInitNoteList(noteHandle);
            this.Helper.CheckStatus("Unable to initalize note list.", Status);
        }
        
        public bool GetNextNote(int noteHandle)
        {
            int Status = PVCSToolKit.TrkGetNextNote(noteHandle);
            if ((int)ServerHelper._TrkError.TRK_E_END_OF_LIST != Status)
            {
                this.Helper.CheckStatus("Unable to get next note.", Status);
            }
            return !((int)ServerHelper._TrkError.TRK_E_END_OF_LIST == Status);
        }
        
        public string GetNoteTitle(int noteHandle)
        {
            string NoteTitle = this.Helper.MakeBigEmptyString(ServerHelper.MAX_BUFFER_LENGTH);
            int Status = PVCSToolKit.TrkGetNoteTitle(noteHandle, NoteTitle.Length, ref    NoteTitle);
            this.Helper.CheckStatus("Unable to get note title.", Status);
            NoteTitle = this.Helper.CleanupString(NoteTitle);
            return NoteTitle;
        }
        
        public string GetNoteAuthor(int noteHandle)
        {
            string NoteText = this.Helper.MakeBigEmptyString(ServerHelper.MAX_BUFFER_LENGTH);
            int Status = PVCSToolKit.TrkGetNoteAuthor(noteHandle, NoteText.Length, ref NoteText);
            this.Helper.CheckStatus("Unable to get note author.", Status);
            NoteText = this.Helper.CleanupString(NoteText);
            return NoteText;
        }
        
        public string GetNoteCreateTime(int noteHandle)
        {
            int NoteCreationTime = 0;
            int Status = PVCSToolKit.TrkGetNoteCreateTime(noteHandle, ref NoteCreationTime);
            this.Helper.CheckStatus("Unable to get note creation time.", Status);

            return this.Helper.ConvertDateToString(NoteCreationTime);
        }
        
        public string GetNoteText(int noteHandle)
        {
            int Remainder = 1;
            StringBuilder NoteTextBuilder = new StringBuilder();
            while (Remainder != 0)
            {
                string NoteTextPart = this.Helper.MakeBigEmptyString(ServerHelper.MAX_BUFFER_LENGTH);
                int Status = PVCSToolKit.TrkGetNoteData(noteHandle, NoteTextPart.Length, ref NoteTextPart, ref Remainder);
                if (Remainder == 0)
                    break;
                if ((int)ServerHelper._TrkError.TRK_E_DATA_TRUNCATED != Status)
                {
                    this.Helper.CheckStatus("Unable to retrieve part of the note text.", Status);
                }
                NoteTextBuilder.Append(this.Helper.CleanupString(NoteTextPart));
            }
            return NoteTextBuilder.ToString();
        }

        public void InitalizeRecordList(int recordHandle, string queryName)
        {
            int TransactionId = 0;
            int NewTransactionId = 0;
            int Status = PVCSToolKit.TrkQueryInitRecordList(recordHandle, ref queryName, TransactionId, ref NewTransactionId);
            this.Helper.CheckStatus("Unable to initalize query record list. Query=" + queryName, Status);
        }

        public bool GetNextScrId(int recordHandle)
        {
            int Status = PVCSToolKit.TrkGetNextRecord(recordHandle);
            if ((int)ServerHelper._TrkError.TRK_E_END_OF_LIST != Status)
            {
                this.Helper.CheckStatus("Unable to get next SCR handle.", Status);
            }
            return !(((int)ServerHelper._TrkError.TRK_E_END_OF_LIST == Status));
        }

        public int GetFieldTransactionId(string fieldName, int recordHandle)
        {
            int TransactionId = 0;
            int Status = PVCSToolKit.TrkGetFieldTransactionID(recordHandle, ref fieldName, ref TransactionId);
            this.Helper.CheckStatus("Unable to get transaction id.", Status);
            return TransactionId;
        }

        public void SaveNumericFieldValue(string fieldName, int newValue, int recordHandle)
        {
            int Status = PVCSToolKit.TrkSetNumericFieldValue(recordHandle, ref fieldName, newValue);
            this.Helper.CheckStatus("Unable to save field value.", Status);
        }

        public void SaveStringFieldValue(string fieldName, string newValue, int recordHandle)
        {
            int Status = PVCSToolKit.TrkSetStringFieldValue(recordHandle, ref fieldName, ref newValue);
            this.Helper.CheckStatus("Unable to save field value: fieldName=" + fieldName + " newValue=" + newValue, Status);
        }

        public void NewRecordBegin(int recordHandle, int recordType)
        {
            int Status = PVCSToolKit.TrkNewRecordBegin(recordHandle, recordType);
            this.Helper.CheckStatus("Unable to submission of a new record.", Status);
        }

        public void NewRecordCommit(int recordHandle, ref int pNewTransactionID)
        {
            int Status = PVCSToolKit.TrkNewRecordCommit(recordHandle, ref pNewTransactionID);
            this.Helper.CheckStatus("Unable to submit a new record.", Status);
        }

        public int GetNumericFieldValue(int recordHandle, string fieldName)
        {
            int newValue = 0;
            int Status = PVCSToolKit.TrkGetNumericFieldValue(recordHandle, ref fieldName, ref newValue);
            this.Helper.CheckStatus("Unable to retrieve numeric field value: " + fieldName, Status);
            return newValue;
        }

        //private void GetFieldNamesExtracted()
        //{
        //    int Status = TrackerServer.TrkInitFieldList(this.TrackerHandle, 1);
        //    this.CheckStatus("Unable to initalize field list.", Status);
        //}
        #endregion

        #region Function Imports

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAddNewAssociation(int trkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAddNewAttachedFile(int TrkAttFileHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string FileName, int StorageMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAddNewNote(int TrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAssociationHandleAlloc(int trkRecordHandle, ref int pTrkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAssociationHandleFree(ref int pTrkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAttachedFileHandleAlloc(int trkRecordHandle, ref int pTrkAttFileHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkAttachedFileHandleFree(ref int pTrkAttFileHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkDeleteAssociation(int trkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkDeleteAttachedFile(int TrkAttFileHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkDeleteNote(int TrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkDeleteRecord(int trkRecordHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExportHandleAlloc(int trkHandle, ref int pTrkExportHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExportHandleFree(ref int pTrkExportHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExportOneRecord(int TrkExportHandle, int trkRecordHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExportRecordsBegin(int TrkExportHandle, int recordType, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fileToWriteTo, int trackerFormat, int delimiterCharacter, int separatorCharacter, int embeddedQuoteType, int dateFormat, int timeFormat);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExportRecordsClose(int TrkExportHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkExtractAttachedFile(int TrkAttFileHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string FileName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationModuleName(int trkAssociationHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string moduleName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationRevisionFixed(int trkAssociationHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string revisionFixed);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationRevisionFound(int trkAssociationHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string revisionFound);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationText(int trkAssociationHandle, int maxBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string description, ref int pDataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationTextLength(int trkAssociationHandle, ref int pDataBufferSize);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationTimeFixed(int trkAssociationHandle, ref int pTimeFixed);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationTimeFound(int trkAssociationHandle, ref int pTimeFound);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationTransactionID(int trkAssociationHandle, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAssociationUser(int trkAssociationHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAttachedFileName(int TrkAttFileHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string FileName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAttachedFileStorageMode(int TrkAttFileHandle, ref int pStorageMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAttachedFileTime(int TrkAttFileHandle, ref int pTimestamp);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetAttachedFileTransactionID(int TrkAttFileHandle, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetDescriptionAccessRights(int trkRecordHandle, ref int pAccessMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetDescriptionData(int trkRecordHandle, int maxBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string data, ref int pDataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetDescriptionDataLength(int trkRecordHandle, ref int pDataBufferSize);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetDescriptionTransactionID(int trkRecordHandle, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldAccessRights(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, ref int pAccessMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldDefaultNumericValue(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType, ref int pDefaultValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldDefaultStringValue(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string defaultValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldMaxDataLength(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType, ref int pMaxDataLength);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldRange(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType, ref int pMinValue, ref int pMaxValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldTransactionID(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetFieldType(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType, ref int pFieldType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetIniFile(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string FileName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetLoginDBMSName(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetLoginDBMSType(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetLoginProjectName(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string projectName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetLoginUserName(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextAssociation(int trkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextAttachedFile(int TrkAttFileHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextChoice(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string choiceName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextDBMSType(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextField(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, ref int pFieldType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextNote(int TrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextProject(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string projectName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextQueryName(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string queryName, ref int pRecordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextRecord(int trkRecordHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextRecordType(int trkHandle, ref int pRecordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNextUser(int trkHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteAuthor(int TrkNoteHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string authorName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteCreateTime(int TrkNoteHandle, ref int pCreateTime);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteData(int TrkNoteHandle, int maxBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string noteData, ref int pDataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteDataLength(int TrkNoteHandle, ref int pDataBufferSize);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteModifyTime(int TrkNoteHandle, ref int pModifyTime);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteTitle(int TrkNoteHandle, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string noteTitle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNoteTransactionID(int TrkNoteHandle, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNumericAttribute(int trkHandle, int attributeId, ref int pValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetNumericFieldValue(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, ref int pFieldValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetProjectTransactionID(int trkHandle, int recordType, ref int pTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetQueryRecordType(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string queryName, ref int pRecordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetRecordRecordType(int trkRecordHandle, ref int pRecordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetRecordTransactionID(int trkRecordHandle, ref int pSubmitTransactionID, ref int pUpdateTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetRecordTypeName(int trkHandle, int recordType, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string recordTypeName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetSingleRecord(int trkRecordHandle, int recordId, int recordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetStringFieldValue(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int fieldValueBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetUserEmail(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string emailAddress);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkGetUserFullName(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName, int bufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fullName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkHandleAlloc(int trkVersionID, ref int pTrkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkHandleFree(ref int pTrkHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkImportHandleAlloc(int trkHandle, ref int pTrkImportHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkImportHandleFree(ref int pTrkImportHandle);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkImportInit(int TrkImportHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fileToReadFrom, int trackerFormat, int recordType, int delimiterCharacter, int separatorCharacter, int dateFormat, int timeFormat);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkImportNewRecords(int TrkImportHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string errorLogFile);

        [DllImport("expdlln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkImportSetOptions(int TrkImportHandle, int choiceOption, int userOption, int numberOption, int dateOption, [MarshalAs(UnmanagedType.VBByRefStr)] ref string defaultDate);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitAssociationList(int trkAssociationHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitAttachedFileList(int TrkAttFileHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitChoiceList(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int recordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitDBMSTypeList(int trkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitFieldList(int trkHandle, int recordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitNoteList(int TrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitProjectList(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSType, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSUserName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSPassword, int DBMSLoginMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitQueryNameList(int trkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitRecordTypeList(int trkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInitUserList(int trkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkInTrayInitRecordList(int trkRecordHandle, int recordType, int transactionID, ref int pNewTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkNewRecordBegin(int trkRecordHandle, int recordType);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkNewRecordCommit(int trkRecordHandle, ref int pNewTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkNoteHandleAlloc(int trkRecordHandle, ref int pTrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkNoteHandleFree(ref int pTrkNoteHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkProjectLogin(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string password, [MarshalAs(UnmanagedType.VBByRefStr)] ref string projectName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSType, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSUserName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string DBMSPassword, int DBMSLoginMode);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkProjectLoginEx(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string password, [MarshalAs(UnmanagedType.VBByRefStr)] ref string projectName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string serverName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkProjectLogout(int trkHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkQueryInitRecordList(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string queryName, int transactionID, ref int pNewTransactionID);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkRecordCancelTransaction(int trkRecordHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkRecordHandleAlloc(int trkHandle, ref int pTrkRecordHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkRecordHandleFree(ref int pTrkRecordHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkRegisterCallback(int trkHandle, int pCallbackFunction, int linkOrder, int userData);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationModuleName(int trkAssociationHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string moduleName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationRevisionFixed(int trkAssociationHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string revisionFixed);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationRevisionFound(int trkAssociationHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string revisionFound);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationText(int trkAssociationHandle, int currentBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string description, int dataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationTimeFixed(int trkAssociationHandle, int timeFixed);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationTimeFound(int trkAssociationHandle, int timeFound);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetAssociationUser(int trkAssociationHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string userName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetDescriptionData(int trkRecordHandle, int currentBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string data, int dataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetIniFile(int trkHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string FileName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNoteAuthor(int TrkNoteHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string authorName);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNoteCreateTime(int TrkNoteHandle, int createTime);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNoteData(int TrkNoteHandle, int currentBufferSize, [MarshalAs(UnmanagedType.VBByRefStr)] ref string noteData, int dataRemaining);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNoteModifyTime(int TrkNoteHandle, int modifyTime);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNoteTitle(int TrkNoteHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string noteTitle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNumericAttribute(int trkHandle, int attributeId, int value);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetNumericFieldValue(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, int fieldValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkSetStringFieldValue(int trkRecordHandle, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string fieldValue);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkUnregisterCallback(int trkHandle, int pCallbackFunction);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkUpdateRecordBegin(int trkRecordHandle);

        [DllImport("trktooln", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int TrkUpdateRecordCommit(int trkRecordHandle, ref int pNewTransactionID);
        #endregion

    }

    public class FieldPair
    {
        private string _FieldName;
        private Type _FieldType;

        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                if (_FieldName == value)
                    return;
                _FieldName = value;
            }
        }
        public Type FieldType
        {
            get
            {
                return _FieldType;
            }
            set
            {
                if (_FieldType == value)
                    return;
                _FieldType = value;
            }
        }

        public FieldPair()
        {

        }

        /// <summary>
        /// Creates a new instance of FieldPair
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        public FieldPair(string fieldName, Type fieldType)
        {
            _FieldName = fieldName;
            _FieldType = fieldType;
        }
    }
}
