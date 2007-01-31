using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CCNET.TFS.Plugin
{

    public delegate void DequeueEventHandler(Changeset changeset);
    public delegate void EnqueueEventHandler(Changeset changeset);

    public class ChangesetQueue
    {

        #region Events

        public event DequeueEventHandler DequeueEvent;
        public event EnqueueEventHandler EnqueueEvent;

        #endregion

        #region Fields

        private Object _Sync = new object();
        private Queue<Changeset> _MyQueue;
        private Changeset _CurrentIntegrationSet;
        private bool _InIntegration = false ;
        private VersionControlServer _SourceControl;
        private string _ProjectPath;
        private int _LastChangeSetId;
        private bool _NeedToDequeue = false ;
        
        #endregion

        #region Properties

        private bool NeedToDequeue
        {
            get
            {
                return _NeedToDequeue;
            }
            set
            {
                _NeedToDequeue = value;
            }
        }

        private int LastChangeSetId
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
                return _SourceControl;
            }
            set
            {
                _SourceControl = value;
            }
        }

        private bool InIntegration
        {
            get
            {
                return _InIntegration;
            }
            set
            {
                _InIntegration = value;
            }
        }

        private Changeset CurrentIntegrationSet
        {
            get
            {
                return _CurrentIntegrationSet;
            }
            set
            {
                _CurrentIntegrationSet = value;
            }
        }

        private Queue<Changeset> MyQueue
        {
            get
            {
                if (_MyQueue == null)
                    _MyQueue = new Queue<Changeset>();
                return _MyQueue;
            }
        }

        public int Count
        {
            get
            {
                lock (this._Sync)
                {
                    return this.MyQueue.Count;
                }
            }
        }

        #endregion

        public ChangesetQueue(string projectPath, VersionControlServer sourceControl)
        {
            _ProjectPath = projectPath;
            _SourceControl = sourceControl;
        }

        #region Methods

        public void BeginIntegration()
        {
            if (!this.InIntegration)
            {
                if (this.Count > 0)
                {
                    this.CurrentIntegrationSet = this.Peek();
                    this.NeedToDequeue = true;
                }
                else
                {
                    this.NeedToDequeue = false;
                    IEnumerable Iterable = this.SourceControl.QueryHistory(this.ProjectPath, VersionSpec.Latest, 0, RecursionType.Full, null, new ChangesetVersionSpec(this.LastChangeSetId), VersionSpec.Latest, int.MaxValue, true, false);
                    SortedList<int, Changeset> ChangeSets = new SortedList<int, Changeset>();
                    foreach (Changeset Set in Iterable)
                    {
                        ChangeSets.Add(Set.ChangesetId, Set);
                    }
                    this.CurrentIntegrationSet = ChangeSets.Values[ChangeSets.Count - 1];
                }
                this.InIntegration = true;
            }
        }

        public void EndIntegration()
        {
            if (this.InIntegration && this.CurrentIntegrationSet != null)
            {
                this.LastChangeSetId = this.CurrentIntegrationSet.ChangesetId;
                this.CurrentIntegrationSet = null;
                
                if (this.NeedToDequeue)
                    this.Dequeue();
                
                this.InIntegration = false;
            }
        }

        public Changeset GetCurrentIntegrationSet()
        {
            return this.CurrentIntegrationSet;
        }

        private Changeset Dequeue()
        {
            lock (this._Sync)
            {
                Changeset Item = this.MyQueue.Dequeue();
                if (this.DequeueEvent != null)
                    this.DequeueEvent(Item);
                return Item;
            }
        }

        private Changeset Peek()
        {
            lock (this._Sync)
            {
                Changeset Item = this.MyQueue.Peek();
                return Item;
            }
        }

        public void Enqueue(Changeset set)
        {
            lock (this._Sync)
            {
                this.MyQueue.Enqueue(set);
                if (this.EnqueueEvent != null)
                    this.EnqueueEvent(set);
            }
        }

        #endregion

    }

}
