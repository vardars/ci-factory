namespace NCover.Interfaces
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("Value1: {Value1}, Value2 {Value2}")]
    public sealed class Tuple<T1, T2>
    {
        [CompilerGenerated]
        private T1 _Value1;
        [CompilerGenerated]
        private T2 _Value2;

        public Tuple()
        {
        }

        public Tuple(T1 t1, T2 t2)
        {
            this.Value1 = t1;
            this.Value2 = t2;
        }

        public T1 Value1
        {
            [CompilerGenerated]
            get
            {
                return this._Value1;
            }
            [CompilerGenerated]
            set
            {
                this._Value1 = value;
            }
        }

        public T2 Value2
        {
            [CompilerGenerated]
            get
            {
                return this._Value2;
            }
            [CompilerGenerated]
            set
            {
                this._Value2 = value;
            }
        }
    }
}

