using System;
using System.Collections;
using System.Text;

namespace Tracker.Common
{

    public class ServerHelper
    {

        #region Enums

        public enum _TrkError
        {
            TRK_SUCCESS = 0,
            TRK_E_VERSION_MISMATCH = 1,
            TRK_E_OUT_OF_MEMORY = 2,
            TRK_E_BAD_HANDLE = 3,
            TRK_E_BAD_INPUT_POINTER = 4,
            TRK_E_BAD_INPUT_VALUE = 5,
            TRK_E_DATA_TRUNCATED = 6,
            TRK_E_NO_MORE_DATA = 7,
            TRK_E_LIST_NOT_INITIALIZED = 8,
            TRK_E_END_OF_LIST = 9,
            TRK_E_NOT_LOGGED_IN = 10,
            TRK_E_SERVER_NOT_PREPARED = 11,
            TRK_E_BAD_DATABASE_VERSION = 12,
            TRK_E_UNABLE_TO_CONNECT = 13,
            TRK_E_UNABLE_TO_DISCONNECT = 14,
            TRK_E_UNABLE_TO_START_TIMER = 15,
            TRK_E_NO_DATA_SOURCES = 16,
            TRK_E_NO_PROJECTS = 17,
            TRK_E_WRITE_FAILED = 18,
            TRK_E_PERMISSION_DENIED = 19,
            TRK_E_SET_FIELD_DENIED = 20,
            TRK_E_ITEM_NOT_FOUND = 21,
            TRK_E_CANNOT_ACCESS_DATABASE = 22,
            TRK_E_CANNOT_ACCESS_QUERY = 23,
            TRK_E_CANNOT_ACCESS_INTRAY = 24,
            TRK_E_CANNOT_OPEN_FILE = 25,
            TRK_E_INVALID_DBMS_TYPE = 26,
            TRK_E_INVALID_RECORD_TYPE = 27,
            TRK_E_INVALID_FIELD = 28,
            TRK_E_INVALID_CHOICE = 29,
            TRK_E_INVALID_USER = 30,
            TRK_E_INVALID_SUBMITTER = 31,
            TRK_E_INVALID_OWNER = 32,
            TRK_E_INVALID_DATE = 33,
            TRK_E_INVALID_STORED_QUERY = 34,
            TRK_E_INVALID_MODE = 35,
            TRK_E_INVALID_MESSAGE = 36,
            TRK_E_VALUE_OUT_OF_RANGE = 37,
            TRK_E_WRONG_FIELD_TYPE = 38,
            TRK_E_NO_CURRENT_RECORD = 39,
            TRK_E_NO_CURRENT_NOTE = 40,
            TRK_E_NO_CURRENT_ATTACHED_FILE = 41,
            TRK_E_NO_CURRENT_ASSOCIATION = 42,
            TRK_E_NO_RECORD_BEGIN = 43,
            TRK_E_NO_MODULE = 44,
            TRK_E_USER_CANCELLED = 45,
            TRK_E_SEMAPHORE_TIMEOUT = 46,
            TRK_E_SEMAPHORE_ERROR = 47,
            TRK_E_INVALID_SERVER_NAME = 48,
            TRK_E_NOT_LICENSED = 49,
            TRK_E_RECORD_LOCKED = 50,
            TRK_E_RECORD_NOT_LOCKED = 51,
            TRK_E_UNMATCHED_PARENS = 52,
            TRK_E_NO_CURRENT_TRANSITION = 53,
            TRK_E_NO_CURRENT_RULE = 54,
            TRK_E_UNKNOWN_RULE = 55,
            TRK_E_RULE_ASSERTION_FAILED = 56,
            TRK_E_ITEM_UNCHANGED = 57,
            TRK_E_TRANSITION_NOT_ALLOWED = 58,
            TRK_E_NO_CURRENT_STYLESHEET = 59,
            TRK_E_NO_CURRENT_FORM = 60,
            TRK_E_NO_CURRENT_VALUE = 61,
            TRK_E_FORM_FIELD_ACCESS = 62,
            TRK_E_INVALID_QBID_STRING = 63,
            TRK_E_FORM_INVALID_FIELD = 64,
            TRK_E_PARTIAL_SUCCESS = 65,
            TRK_END_OF_LIST,
            TRK_NUMBER_OF_ERROR_CODES = TRK_END_OF_LIST - 1,

            //
            // Export/Import error codes follow:
            TRKEXP_ERROR_CODE_BASE = 10000,
            TRKEXP_E_EXPORT_WRONG_VERSION = TRKEXP_ERROR_CODE_BASE,
            TRKEXP_E_EXPORTSET_NOT_INIT = 10001,
            TRKEXP_E_NO_EXPSET_NAME = 10002,
            TRKEXP_E_BAD_EXPSET_NAME = 10003,
            TRKEXP_E_EXPSET_FAIL_CREATE = 10004,
            TRKEXP_E_IMPORTMAP_NOT_INIT = 10005,
            TRKEXP_E_NO_IMPMAP_NAME = 10006,
            TRKEXP_E_BAD_IMPMAP_NAME = 10007,
            TRKEXP_E_IMPMAP_FAIL_CREATE = 10008,
            TRKEXP_E_IMP_VALIDATE_FAIL = 10009,
            TRKEXP_E_USER_NOEXIST = 10010,
            TRKEXP_E_USER_ADD = 10011,
            TRKEXP_E_IMPORT_NOT_INIT = 10012,
            TRKEXP_E_BAD_EMBEDDED_QUOTE_ARG = 10013,
            TRKEXP_E_BAD_DATEFORMAT_ARG = 10014,
            TRKEXP_E_BAD_TIMEFORMAT_ARG = 10015,
            TRKEXP_E_BAD_CHOICE_OPTION_ARG = 10016,
            TRKEXP_E_BAD_USER_OPTION_ARG = 10017,
            TRKEXP_E_BAD_NUMBER_OPTION_ARG = 10018,
            TRKEXP_E_BAD_DATE_OPTION_ARG = 10019,
            TRKEXP_E_ALL_NOTES_SELECTED = 10020,
            TRKEXP_E_READ_EXPORTHDR = 10021,
            TRKEXP_E_WRITE_EXPORTHDR = 10022,
            TRKEXP_E_READ_RECORDHDR = 10023,
            TRKEXP_E_WRITE_RECORDHDR = 10024,
            TRKEXP_E_WRITE_FIELD = 10025,
            TRKEXP_E_OPEN_FILE = 10026,
            TRKEXP_E_READ_FIELD = 10027,
            TRKEXP_E_READ_FIELD_WRONG_TYPE = 10028,
            TRKEXP_E_BAD_ITEM_TYPE = 10029,
            TRKEXP_E_READ_FROM_DB = 10030,
            TRKEXP_E_WRITE_TO_DB = 10031,
            TRKEXP_E_BAD_DATE = 10032,
            TRKEXP_E_BAD_CHOICE = 10033,
            TRKEXP_E_BAD_NUMBER = 10034,
            TRKEXP_E_OPEN_ERRORLOG = 10035,
            TRKEXP_E_BAD_ERRORLOG_PATH = 10036,
            TRKEXP_E_LOGGING_ERROR = 10037,
            TRKEXP_E_IMPORT_PERMISSION = 10038,
            TRKEXP_E_EXPORT_PERMISSION = 10039,
            TRKEXP_E_NEW_USER_PERMISSION = 10040,
            TRKEXP_E_CLOSE_ERRORLOG = 10041,
            TRKEXP_E_NEWCHOICE_SYSFLD = 10042,
            TRKEXP_E_USER_ALREADY_IN_GROUP = 10043,
            TRKEXP_E_BAD_STRING_OPTION_ARG = 10044,
            TRKEXP_E_STRING_TOO_LONG = 10045,
            TRKEXP_E_EXTRA_FIELDS = 10046,
            TRKEXP_END_OF_LIST,
            TRKEXP_NUMBER_OF_ERROR_CODES = TRKEXP_END_OF_LIST - 1,
            //
            // Internal error codes follow:
            // (Clients of the DLL should never see these.)
            TRK_INTERNAL_ERROR_CODE_BASE = 20000,
            TRK_E_INTERNAL_ERROR = TRK_INTERNAL_ERROR_CODE_BASE
        };

        public enum _TrkDBMSLoginMode
        {
            TRK_USE_INI_FILE_DBMS_LOGIN = 0,
            TRK_USE_SPECIFIED_DBMS_LOGIN = 1,
            TRK_USE_DEFAULT_DBMS_LOGIN = 2
        };

        public enum _TrkAttributeId
        {
            TRK_TRKTOOL_ATTRIBUTE_ID_BASE = 0,
            TRK_CANCEL_INTRAY = 1,
            TRK_CANCEL_QUERY = 2,
            TRK_CANCEL_IMPORT = 3,
            TRK_NO_KEEP_ALIVE = 4,
            TRK_NO_TIMER = 5,
            TRK_NO_RECORD_CACHE = 6,
            TRK_NO_RECORD_LOCK = 7,
            TRK_CONCURRENT_DB_TIMEOUT = 8,
            TRK_CACHE_PROJECT = 9,
            TRK_IGNORE_STATE_TRANSITION_RULES = 10,
            TRK_PROGRAM_TYPE = 11,
            TRK_NO_XREFRESH = 12,
            //
            // (Clients of the DLL are free to use values at or
            // above this threshhold.)
            TRK_USER_ATTRIBUTE_ID_BASE = 1000
        };

        // Provide an enumeration of all Fields defined in the
        // current project.

        public enum _TrkFieldType
        {
            TRK_FIELD_TYPE_NONE = 0,
            TRK_FIELD_TYPE_CHOICE = 1,
            TRK_FIELD_TYPE_STRING = 2,
            TRK_FIELD_TYPE_NUMBER = 3,
            TRK_FIELD_TYPE_DATE = 4,
            TRK_FIELD_TYPE_SUBMITTER = 5,
            TRK_FIELD_TYPE_OWNER = 6,
            TRK_FIELD_TYPE_USER = 7,
            TRK_FIELD_TYPE_ELAPSED_TIME = 8,
            TRK_FIELD_TYPE_STATE = 9
        };

        public enum _TrkFieldAccessMode
        {
            TRK_READ_ONLY = 0,
            TRK_READ_WRITE = 2
        };

        public enum _TrkFileStorageMode
        {
            TRK_FILE_BINARY = 0,
            TRK_FILE_ASCII = 1,
            TRK_FILE_GUESS = 2
        };

        public enum _TrkExportFormat
        {
            TRK_EXPORT_FORMAT_SIMPLE = 0,
            TRK_EXPORT_FORMAT_TRACKER = 1,
            TRK_EXPORT_FORMAT_PDIFF = 2,
            TRK_EXPORT_FORMAT_XML = 3
        };

        public enum _TrkEmbeddedQuote
        {
            TRK_DOUBLE_QUOTE = 1,
            TRK_BACKSLASH_QUOTE = 2
        };

        public enum _TrkDateFormat
        {
            TRK_CONTROL_PANEL_DATE = 1,
            TRK_DBASE_FORMAT = 2
        };

        public enum _TrkTimeFormat
        {
            TRK_CONTROL_PANEL_TIME = 1,
            TRK_24HOUR = 2,
            TRK_24HOUR_LEADING_ZERO = 3
        };

        public enum _TrkDateOption
        {
            TRK_FAIL_DATE = 0,
            TRK_SET_CURRENT = 1,
            TRK_SET_TO_SPECIFIED = 2
        };

        public enum _TrkChoiceOption
        {
            TRK_FAIL_CHOICE = 0,
            TRK_DEFAULT_CHOICE = 1,
            TRK_NEW_CHOICE = 2
        };

        public enum _TrkUserOption
        {
            TRK_FAIL_USER = 0,
            TRK_ADD_USER = 1,
            TRK_ADD_USER_WITH_GROUP = 2,
            TRK_DEFAULT_USER = 3
        };

        public enum _TrkNumberOption
        {
            TRK_FAIL_NUMBER = 0,
            TRK_DEFAULT_NUMBER = 1
        };

        public enum _TrkStringOption
        {
            TRK_FAIL_STRING = 0,
            TRK_TRUNCATE_STRING = 1,
            TRK_EMPTY_STRING = 2
        };

        public enum _TrkCallbackMessage
        {
            TRK_MSG_API_TRACE = 1,
            TRK_MSG_API_EXIT = 2,
            TRK_MSG_ODBC_ERROR = 3,
            TRK_MSG_INVALID_FIELD_VALUE = 4,
            TRK_MSG_DATA_TRUNCATED = 5,
            TRK_MSG_FORCE_LOGOUT = 6,
            TRK_MSG_IMPORT_ERROR = 7,
            TRK_MSG_INTRAY_PROGRESS = 8,
            TRK_MSG_QUERY_PROGRESS = 9,
            TRK_MSG_IMPORT_PROGRESS = 10,
            TRK_MSG_CONSTRAINT_VIOLATION = 11,
            TRK_LAST_CALLBACK_MSG
        };	// (not a message; marks end of list)

        public enum _TrkCallbackReturnCode
        {
            TRK_MSG_NOT_HANDLED = 0,
            TRK_MSG_HANDLED = 1
        };

        public enum _TrkLinkOrder
        {
            TRK_LIST_ADD_HEAD = 0,
            TRK_LIST_ADD_TAIL = 1
        };

        public enum StorageMode
        {
            BinaryMode,
            ASCIIMode
        }

        #endregion

        #region Const

        public const int TRK_MAX_STRING = 255;
        public const int TRK_VERSION_ID = 500001;

        public const double DATE_MODIFIER = 25568.791666666668;
        public const int MAX_BUFFER_LENGTH = 255;

        #endregion

        public ServerHelper()
        {

        }

        public void CheckStatus(string exceptionMessage, int status)
        {

            if ((int)_TrkError.TRK_SUCCESS == status)
            {
                return;
            }

            throw new InvalidOperationException(string.Format("{0}: Code = {1}", exceptionMessage, Enum.GetName(typeof(_TrkError), status)));
        }

        public string CleanupString(string dirtyString)
        {
            dirtyString = dirtyString.Replace("\0", "");
            return dirtyString.Trim();
        }

        public string ConvertDateToString(int dateTime)
        {
            return System.DateTime.FromOADate((((((double)dateTime) / 60) / 60) / 24) + DATE_MODIFIER).ToString();
        }

        public string MakeBigEmptyString(int size)
        {
            return new string('\0', size);
        }
    }
}
