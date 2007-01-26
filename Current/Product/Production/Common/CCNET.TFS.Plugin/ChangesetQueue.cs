using System;
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

        #endregion

        #region Properties

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

        #region Methods

        public Changeset Dequeue()
        {
            lock (this._Sync)
            {
                Changeset Item = this.MyQueue.Dequeue();
                this.DequeueEvent(Item);
                return Item;
            }
        }

        public Changeset Peek()
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
                this.EnqueueEvent(set);
            }
        }

        #endregion

    }

}
