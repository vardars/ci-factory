namespace NCover.Interfaces
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("Value1: {Value1}, Value2 {Value2}, Value3 {Value3}")]
    public sealed class Tuple<T1, T2, T3>
    {
        [CompilerGenerated]
        private T1 _Value1;
        [CompilerGenerated]
        private T2 _Value2;
        [CompilerGenerated]
        private T3 _Value3;

        public Tuple(T1 v1, T2 v2, T3 v3)
        {
            this.Value1 = v1;
            this.Value2 = v2;
            this.Value3 = v3;
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
    }
}

