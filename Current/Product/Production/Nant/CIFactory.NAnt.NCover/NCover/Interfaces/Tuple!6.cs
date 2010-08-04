namespace NCover.Interfaces
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("Value1: {Value1}, Value2 {Value2}, Value3 {Value3}, Value4 {Value4}, Value5 {Value5}, Value6 {Value6}")]
    public sealed class Tuple<T1, T2, T3, T4, T5, T6>
    {
        [CompilerGenerated]
        private T1 _Value1;
        [CompilerGenerated]
        private T2 _Value2;
        [CompilerGenerated]
        private T3 _Value3;
        [CompilerGenerated]
        private T4 _Value4;
        [CompilerGenerated]
        private T5 _Value5;
        [CompilerGenerated]
        private T6 _Value6;

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

        public T3 Value3
        {
            [CompilerGenerated]
            get
            {
                return this._Value3;
            }
            [CompilerGenerated]
            set
            {
                this._Value3 = value;
            }
        }

        public T4 Value4
        {
            [CompilerGenerated]
            get
            {
                return this._Value4;
            }
            [CompilerGenerated]
            set
            {
                this._Value4 = value;
            }
        }

        public T5 Value5
        {
            [CompilerGenerated]
            get
            {
                return this._Value5;
            }
            [CompilerGenerated]
            set
            {
                this._Value5 = value;
            }
        }

        public T6 Value6
        {
            [CompilerGenerated]
            get
            {
                return this._Value6;
            }
            [CompilerGenerated]
            set
            {
                this._Value6 = value;
            }
        }
    }
}

