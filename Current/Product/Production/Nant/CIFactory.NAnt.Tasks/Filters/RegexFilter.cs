using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Filters
{

    [ElementName("regexreplace")]
    public class RegexFilter : Filter
    {
        #region Fields

        private bool _AtEndOfStream;

        private Queue<int> _CharQueue;

        private bool _IsPrimed = false;

        private Queue<Line> _LineQueue;

        private int _Lines;

        private string _Pattern;

        private Regex _PatternFinder;

        private AcquireCharDelegate _ReadChar;

        private string _Replacment;

        private Regex NewLineFinder = new Regex("\\n");

        #endregion

        #region Delegates

        private delegate int AcquireCharDelegate();

        #endregion

        #region Properties

        public bool AtEndOfStream
        {
            get { return _AtEndOfStream; }
            set { _AtEndOfStream = value; }
        }

        public Queue<int> CharQueue
        {
            get { return _CharQueue; }
            set { _CharQueue = value; }
        }

        public bool IsPrimed
        {
            get { return _IsPrimed; }
            set { _IsPrimed = value; }
        }

        public Queue<Line> LineQueue
        {
            get
            {
                if (_LineQueue == null)
                {
                    _LineQueue = new Queue<Line>();
                }
                return _LineQueue;
            }
            set { _LineQueue = value; }
        }

        [TaskAttribute("lines", Required = false), Int32Validator()]
        public int Lines
        {
            get { return _Lines; }
            set { _Lines = value; }
        }

        [TaskAttribute("pattern", Required = true), StringValidator(AllowEmpty = false)]
        public string Pattern
        {
            get { return _Pattern; }
            set { _Pattern = value; }
        }

        public Regex PatternFinder
        {
            get
            {
                if (_PatternFinder == null)
                {
                    _PatternFinder = new Regex(this.Pattern);
                }
                return _PatternFinder;
            }
            set { _PatternFinder = value; }
        }

        private AcquireCharDelegate ReadChar
        {
            get { return _ReadChar; }
            set { _ReadChar = value; }
        }

        [TaskAttribute("replacment", Required = true), StringValidator(AllowEmpty = true)]
        public string Replacment
        {
            get { return _Replacment; }
            set { _Replacment = value; }
        }

        #endregion

        #region Public Methods

        public override void Chain(ChainableReader parentChainedReader)
        {
            base.Chain(parentChainedReader);
            this.ReadChar = new AcquireCharDelegate(base.Read);
        }

        public void FillQueue()
        {
            if (!this.IsPrimed)
            {
                this.Prime();
            }
            this.ReadLine();
            Line Line = this.LineQueue.Dequeue();
            this.CharQueue = Line.Characters;
            this.ReplaceIfMatchFound(Line);
        }

        public int GetNextCharacter()
        {
            if (this.CharQueue == null || this.CharQueue.Count == 0)
            {
                this.FillQueue();
            }
            return this.CharQueue.Dequeue();
        }

        public bool IsEndOfLine(int character)
        {
            return this.NewLineFinder.IsMatch(Filter.ConvertIntToChar(character).ToString());
        }

        public override int Peek()
        {
            throw new ApplicationException("Peek currently is not supported.");
        }

        public void Prime()
        {
            int LinesToRead = this.Lines - 1;
            while (LinesToRead > 0)
            {
                this.ReadLine();
                LinesToRead -= 1;
            }
            this.IsPrimed = true;
        }

        public override int Read()
        {
            int NextCharacter = this.GetNextCharacter();
            if (NextCharacter == -1)
                this.AtEndOfStream = false;
            return NextCharacter;
        }

        public void ReadLine()
        {
            if (this.AtEndOfStream)
            {
                return;
            }
            int Character;
            Line Line = new Line();
            this.LineQueue.Enqueue(Line);
            do
            {
                Character = base.Read();
                Line.Append(Character);
                if (Character == -1)
                {
                    this.AtEndOfStream = true;
                    return;
                }
            }
            while (!(this.IsEndOfLine(Character)));
        }

        public void ReplaceIfMatchFound(Line line)
        {
            StringBuilder Canidate = new StringBuilder();
            Canidate.Append(line.Text);
            foreach (Line NextLine in this.LineQueue)
            {
                Canidate.Append(NextLine.Text);
            }
            if (this.PatternFinder.IsMatch(Canidate.ToString()))
            {
                string Replaced;
                Replaced = this.PatternFinder.Replace(Canidate.ToString(), this.Replacment);
                this.CharQueue = new Queue<int>();
                foreach (char Character in Replaced)
                {
                    this.CharQueue.Enqueue(Filter.ConvertCharToInt(Character));
                }
                if (this.AtEndOfStream)
                {
                    this.CharQueue.Enqueue(-1);
                }
                this.IsPrimed = false;
                this.LineQueue.Clear();
            }
        }

        #endregion

    }

}