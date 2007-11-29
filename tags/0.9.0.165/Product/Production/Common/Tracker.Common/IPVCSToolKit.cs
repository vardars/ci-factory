using System;
namespace Tracker.Common
{
    public interface IPVCSToolKit
    {
        void Logout();
        void Login(string userName, string password, string projectName, string dbmsUserName, string dbmsPassword, string dbmsServer, string dbmsType, int dbmsLoginMode);
        void BeginUpdate(int recordHandle);
        void CancelTransaction(int recordHandle);
        void ReleaseNoteHandle(int noteHandle);
        void ReleaseRecordHandle(int recordHandle);
        void CommitRecord(int transactionID, int recordHandle);
        int GetNoteHandle(int recordHandle);
        int GetNoteTransactionId(int noteHandle);
        int GetSCRID(int recordHandle);
        int GetSCRRecordHandle(int scrId, int recordType);
        void AddNote(string noteTitle, string noteText, int noteHandle);
        int GetDescriptionLength(int recordHandle);
        string GetDescriptionPart(ref int remainder, int recordHandle);
        object GetFieldValue(int scrId, string fieldName);
        string GetNoteText(int noteHandle);
        string GetNoteCreateTime(int noteHandle);
        string GetNoteAuthor(int noteHandle);
        string GetNoteTitle(int noteHandle);
        bool GetNextNote(int noteHandle);
        void InitalizeNoteList(int noteHandle);
        int AllocateRecordHandle();
        void InitalizeRecordList(int recordHandle, string queryName);
        bool GetNextScrId(int recordHandle);
        int GetFieldType(string fieldName);
        int GetFieldTransactionId(string fieldName, int recordHandle);
        void SaveNumericFieldValue(string fieldName, int newValue, int recordHandle);
        void SaveStringFieldValue(string fieldName, string newValue, int recordHandle);
        int GetNumericFieldValue(int recordHandle, string fieldName);
        void NewRecordBegin(int recordHandle, int recordType);
        void NewRecordCommit(int recordHandle, ref int transactionID);
    }
}
