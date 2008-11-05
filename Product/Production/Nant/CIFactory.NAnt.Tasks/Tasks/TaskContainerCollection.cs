using System;
using System.Collections;
using NAnt.Core;


namespace CIFactory.NAnt.Tasks
{
    public class TaskContainerCollection : CollectionBase
    {
        #region Constructors

        public TaskContainerCollection()
        {
        }

        public TaskContainerCollection(TaskContainerCollection value)
        {
            this.InnerList.AddRange(value);
        }

        public TaskContainerCollection(TaskContainer[] value)
        {
            this.InnerList.AddRange(value);
        }

        #endregion

        #region Properties

        public TaskContainer this[int index]
        {
            get { return (TaskContainer)this.InnerList[index]; }
            set { this.InnerList[index] = value; }
        }

        #endregion

        #region Public Methods

        public int Add(TaskContainer item)
        {
            return base.List.Add(item);
        }

        public void AddRange(TaskContainer[] items)
        {
            this.InnerList.AddRange(items);
        }

        public void AddRange(TaskContainerCollection items)
        {
            this.InnerList.AddRange(items);
        }

        public bool Contains(TaskContainer item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(TaskContainer[] array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        public int IndexOf(TaskContainer item)
        {
            return this.InnerList.IndexOf(item);
        }

        public void Insert(int index, TaskContainer item)
        {
            this.InnerList.Insert(index, item);
        }

        public void Remove(TaskContainer item)
        {
            this.InnerList.Remove(item);
        }

        #endregion

    }
}
