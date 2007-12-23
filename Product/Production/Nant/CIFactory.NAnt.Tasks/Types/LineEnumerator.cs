using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace CIFactory.NAnt.Types
{
    public class LineEnumerator : IEnumerator
    {
        #region Fields

        private string _Current;

        private TextReader _reader;

        private Regex _regularEpression;

        #endregion

        #region Constructors

        public LineEnumerator(TextReader reader, Regex regularEpression)
        {
            _reader = reader;
            _regularEpression = regularEpression;
        }

        #endregion

        #region Properties

        public object Current
        {
            get { return this._Current; }
        }

        public TextReader Reader
        {
            get { return _reader; }
            set { _reader = value; }
        }

        public Regex RegularEpression
        {
            get { return _regularEpression; }
            set { _regularEpression = value; }
        }

        #endregion

        #region Public Methods

        public string GetNextLine()
        {
            string Line;
            while (true)
            {
                Line = this.Reader.ReadLine();
                if (Line == null)
                {
                    return null;
                }
                if (this.RegularEpression.IsMatch(Line))
                {
                    return Line;
                }
            }
        }

        public bool MoveNext()
        {
            this._Current = this.GetNextLine();
            if (this._Current == null)
            {
                this.Reader.Close();
                return false;
            }
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException("Reset is not suppoerted");
        }

        #endregion

    }
}
