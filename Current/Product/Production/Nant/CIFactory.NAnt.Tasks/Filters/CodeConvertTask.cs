using System;
using System.Collections.Generic;
using System.IO;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;

namespace CIFactory.NAnt.Filters
{
    [ElementName("codeconvert")]
    public class CodeConvert : Filter
    {
        #region Fields

        private Queue<int> _Characters;

        private SupportedLanguage _From;

        private SupportedLanguage _To;

        private AcquireCharDelegate ReadChar = null;

        #endregion

        #region Delegates

        private delegate int AcquireCharDelegate();

        #endregion

        #region Properties

        public Queue<int> Characters
        {
            get
            {
                if (_Characters == null)
                    _Characters = new Queue<int>();
                return _Characters;
            }
            set
            {
                _Characters = value;
            }
        }

        [TaskAttribute("from", Required = true)]
        public SupportedLanguage From
        {
            get
            {
                return _From;
            }
            set
            {
                _From = value;
            }
        }

        [TaskAttribute("to", Required = true)]
        public SupportedLanguage To
        {
            get
            {
                return _To;
            }
            set
            {
                _To = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Chain(ChainableReader chainedReader)
        {
            base.Chain(chainedReader);
            ReadChar = new AcquireCharDelegate(base.Read);
        }

        public string Convert()
        {
            TextReader Reader = this.GetReader();
            IParser Parser = ParserFactory.CreateParser(this.From, Reader);
            Parser.Parse();
            if (Parser.Errors.Count > 0)
            {
                Log(Level.Error, Parser.Errors.ErrorOutput);
                throw new BuildException("Errors parsing code.", this.Location);
            }
            CompilationUnit Tree = Parser.CompilationUnit;

            IOutputAstVisitor OutputVisitor = null;
            switch (this.To)
            {
                case SupportedLanguage.CSharp:
                    OutputVisitor = new CSharpOutputVisitor();
                    break;
                case SupportedLanguage.VBNet:
                    OutputVisitor = new VBNetOutputVisitor();
                    break;
            }
            Tree.AcceptVisitor(OutputVisitor, null);

            return OutputVisitor.Text;
        }

        public TextReader GetReader()
        {
            StringWriter Writer = new StringWriter();
            int ReadBit = base.Read();

            while (ReadBit != -1)
            {
                Writer.Write((Char)ReadBit);
                ReadBit = base.Read();
            }
            Writer.Flush();
            return new StringReader(Writer.ToString());
        }

        public override int Read()
        {
            if (this.Characters.Count == 0)
            {
                string ConvertedSource = this.Convert();
                this.FillQueue(ConvertedSource);
            }
            return this.Characters.Dequeue();
        }

        #endregion

        #region Private Methods

        private void FillQueue(string ConvertedSource)
        {
            foreach (Char Character in ConvertedSource)
            {
                this.Characters.Enqueue((int)Character);
            }
            this.Characters.Enqueue(-1);
        }

        #endregion

    }
}
