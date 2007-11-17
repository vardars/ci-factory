using System;
using System.Collections;

namespace CIFactory.NAnt.Types
{
    public class CaseContainerCollection : CollectionBase
    {
        #region Constructors

        public CaseContainerCollection(CaseContainerCollection value)
        {
            this.InnerList.AddRange(value);
        }

        public CaseContainerCollection(CaseElement[] value)
        {
            this.InnerList.AddRange(value);
        }

        public CaseContainerCollection()
        {
        }

        #endregion

        #region Properties

        public CaseElement this[int index]
        {
            get { return (CaseElement)this.InnerList[index]; }
            set { this.InnerList[index] = value; }
        }

        #endregion

        #region Public Methods

        public int Add(CaseElement item)
        {
            return base.List.Add(item);
        }

        public void AddRange(CaseContainerCollection items)
        {
            this.InnerList.AddRange(items);
        }

        public void AddRange(CaseElement[] items)
        {
            this.InnerList.AddRange(items);
        }

        public bool Contains(CaseElement item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(CaseElement[] array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        public int IndexOf(CaseElement item)
        {
            return this.InnerList.IndexOf(item);
        }

        public void Insert(int index, CaseElement item)
        {
            this.InnerList.Insert(index, item);
        }

        public void Remove(CaseElement item)
        {
            this.InnerList.Remove(item);
        }

        #endregion

    }
}