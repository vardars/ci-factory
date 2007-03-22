using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core.Filters;

namespace CIFactory.NAnt.Filters
{
    public class Line
    {
        #region Fields

        private StringBuilder _Builder;

        private Queue<int> _Characters;

        #endregion

        #region Properties

        private StringBuilder Builder
        {
            get
            {
                if (_Builder == null)
                {
                    _Builder = new StringBuilder();
                }
                return _Builder;
            }
        }

        public Queue<int> Characters
        {
            get
            {
                if (_Characters == null)
                {
                    _Characters = new Queue<int>();
                }
                return _Characters;
            }
        }

        public string Text
        {
            get { return this.Builder.ToString(); }
        }

        #endregion

        #region Public Methods

        public void Append(int Character)
        {
            if (Character != -1)
            {
                this.Builder.Append(Filter.ConvertIntToChar(Character));
            }
            this.Characters.Enqueue(Character);
        }

        #endregion

    }
}
