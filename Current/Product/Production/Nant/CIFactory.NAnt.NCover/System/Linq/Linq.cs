namespace System.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    public static class Linq
    {
        public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequence)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (!sequence.Empty<T>())
            {
                foreach (T local in sequence)
                {
                    collection.Add(local);
                }
            }
            return collection;
        }

        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
        {
            return new T[] { head }.Concat<T>(tail);
        }

        public static IEnumerable<T> Distinct<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> singlePropertySelector)
        {
            return sequence.Distinct<T>(new DistinctEqualityComparer<T, TResult>(singlePropertySelector));
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> sequence, Func<T, T, bool> comparer)
        {
            return sequence.Distinct<T>(new DistinctEqualityComparer<T, bool>(comparer));
        }

        public static bool Empty<T>(this IEnumerable<T> sequence)
        {
            if (sequence != null)
            {
                return !sequence.Any<T>();
            }
            return true;
        }

        public static string Flatten(this IEnumerable<string> sequence, string seperator)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }
            if (seperator == null)
            {
                throw new ArgumentNullException("seperator");
            }
            StringBuilder builder = new StringBuilder(30);
            foreach (string str in sequence)
            {
                if (str != null)
                {
                    builder.Append(str);
                    builder.Append(seperator);
                }
            }
            if (builder.Length > 0)
            {
                builder.Length -= seperator.Length;
            }
            return builder.ToString();
        }

        public static bool NotEmpty<T>(this IEnumerable<T> sequence)
        {
            return !sequence.Empty<T>();
        }

#if (UNDECOMPILABLE)
        public static IDictionary<TKey, List<T>> Partition<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> pivotSelector)
        {
            return sequence.Aggregate<T, Dictionary<TKey, List<T>>>(new Dictionary<TKey, List<T>>(), delegate (Dictionary<TKey, List<T>> results, T element) {
                TKey key = base.pivotSelector(element);
                if (!results.ContainsKey(key))
                {
                    results[key] = new List<T>();
                }
                results[key].Add(element);
                return results;
            });
        }
#endif

        public static void Run<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T local in source)
            {
                action(local);
            }
        }

        public static IEnumerable<T> ThrowIf<T, E>(this IEnumerable<T> source, E exception, Func<IEnumerable<T>, bool> predicate) where E: Exception
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (predicate(source))
            {
                throw exception;
            }
            return source;
        }

        public static IEnumerable<T> ThrowIfEmpty<T, E>(this IEnumerable<T> source, E exception) where E: Exception
        {
            if (source.Empty<T>())
            {
                throw exception;
            }
            return source;
        }

        public static IEnumerable<T> ThrowIfNotSingle<T, E>(this IEnumerable<T> source, E exception) where E: Exception
        {
            if (source.Empty<T>() || (source.Take<T>(2).Count<T>() > 1))
            {
                throw exception;
            }
            return source;
        }

#if (UNDECOMPILABLE)
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return new <Zip>d__3<TFirst, TSecond, TResult>(-2) { <>3__first = first, <>3__second = second, <>3__resultSelector = resultSelector };
        }

        [CompilerGenerated]
        private sealed class <Zip>d__3<TFirst, TSecond, TResult> : IEnumerable<TResult>, IEnumerable, IEnumerator<TResult>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private TResult <>2__current;
            public IEnumerable<TFirst> <>3__first;
            public Func<TFirst, TSecond, TResult> <>3__resultSelector;
            public IEnumerable<TSecond> <>3__second;
            private int <>l__initialThreadId;
            public IEnumerator<TFirst> <firstIter>5__4;
            public IEnumerator<TSecond> <secondIter>5__5;
            public IEnumerable<TFirst> first;
            public Func<TFirst, TSecond, TResult> resultSelector;
            public IEnumerable<TSecond> second;

            [DebuggerHidden]
            public <Zip>d__3(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private bool MoveNext()
            {
                switch (this.<>1__state)
                {
                    case 0:
                        this.<>1__state = -1;
                        if (this.resultSelector == null)
                        {
                            throw new ArgumentNullException("resultSelector");
                        }
                        if (this.first.NotEmpty<TFirst>() && this.second.NotEmpty<TSecond>())
                        {
                            this.<firstIter>5__4 = this.first.GetEnumerator();
                            this.<secondIter>5__5 = this.second.GetEnumerator();
                            while (this.<firstIter>5__4.MoveNext() && this.<secondIter>5__5.MoveNext())
                            {
                                this.<>2__current = this.resultSelector(this.<firstIter>5__4.Current, this.<secondIter>5__5.Current);
                                this.<>1__state = 1;
                                return true;
                            Label_00A5:
                                this.<>1__state = -1;
                            }
                        }
                        break;

                    case 1:
                        goto Label_00A5;
                }
                return false;
            }

            [DebuggerHidden]
            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
            {
                System.Linq.Linq.<Zip>d__3<TFirst, TSecond, TResult> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (System.Linq.Linq.<Zip>d__3<TFirst, TSecond, TResult>) this;
                }
                else
                {
                    d__ = new System.Linq.Linq.<Zip>d__3<TFirst, TSecond, TResult>(0);
                }
                d__.first = this.<>3__first;
                d__.second = this.<>3__second;
                d__.resultSelector = this.<>3__resultSelector;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<TResult>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
            }

            TResult IEnumerator<TResult>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }
#endif

        private class DistinctEqualityComparer<T, TResult> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _comparer;
            private readonly Func<T, TResult> _singlePropertyComparer;

            public DistinctEqualityComparer(Func<T, TResult> singlePropertyComparer)
            {
                if (singlePropertyComparer == null)
                {
                    throw new ArgumentNullException("singlePropertyComparer");
                }
                this._singlePropertyComparer = singlePropertyComparer;
            }

            public DistinctEqualityComparer(Func<T, T, bool> comparer)
            {
                if (comparer == null)
                {
                    throw new ArgumentNullException("comparer");
                }
                this._comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }
                if ((x == null) || (y == null))
                {
                    return false;
                }
                if (this._comparer != null)
                {
                    return this._comparer(x, y);
                }
                TResult objA = this._singlePropertyComparer(x);
                TResult objB = this._singlePropertyComparer(y);
                return (object.ReferenceEquals(objA, objB) || (((objA != null) && (objB != null)) && objA.Equals(objB)));
            }

            public int GetHashCode(T obj)
            {
                if (this._singlePropertyComparer == null)
                {
                    return obj.GetHashCode();
                }
                TResult local = this._singlePropertyComparer(obj);
                if (local != null)
                {
                    return local.GetHashCode();
                }
                return 0;
            }
        }
    }
}
