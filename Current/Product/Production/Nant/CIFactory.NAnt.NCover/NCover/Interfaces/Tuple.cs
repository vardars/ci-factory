using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NCover.Interfaces
{
    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new Tuple<T1, T2>(t1, t2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Tuple<T1, T2, T3>(t1, t2, t3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }
    }

	//[DebuggerDisplay("Value1: {Value1}, Value2 {Value2}")]
	//public sealed class Tuple<T1, T2>
	//{
	//    [CompilerGenerated]
	//    private T1 _Value1;
	//    [CompilerGenerated]
	//    private T2 _Value2;

	//    public Tuple()
	//    {
	//    }

	//    public Tuple(T1 t1, T2 t2)
	//    {
	//        this.Value1 = t1;
	//        this.Value2 = t2;
	//    }

	//    public T1 Value1
	//    {
	//        [CompilerGenerated]
	//        get
	//        {
	//            return this._Value1;
	//        }
	//        [CompilerGenerated]
	//        set
	//        {
	//            this._Value1 = value;
	//        }
	//    }

	//    public T2 Value2
	//    {
	//        [CompilerGenerated]
	//        get
	//        {
	//            return this._Value2;
	//        }
	//        [CompilerGenerated]
	//        set
	//        {
	//            this._Value2 = value;
	//        }
	//    }
	//}

}

