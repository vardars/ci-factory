using System;

namespace CCNET.TFS.Plugin
{
    [Serializable]
    public class MonitorState
    {

        #region Fields

        private int _AlertId;
        private int _LastChangeSetId;

        #endregion

        #region Properties

        public int LastChangeSetId
        {
            get
            {
                return _LastChangeSetId;
            }
            set
            {
                _LastChangeSetId = value;
            }
        }

        public int AlertId
        {
            get
            {
                return _AlertId;
            }
            set
            {
                _AlertId = value;
            }
        }

        #endregion
    
    }

}
