// CIScript - A .NET build tool
// Copyright (C) 2001-2005 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Owen Rogers (exortech@gmail.com)

using System;
using System.Collections;

namespace CIScript.Core.Util {
    public class StopWatchStack {
        private readonly DateTimeProvider _dtProvider;
        private readonly Stack _stack = new Stack();

        public StopWatchStack(DateTimeProvider dtProvider) {
            _dtProvider = dtProvider;
        }

        public void PushStart() {
            _stack.Push(new StopWatch(_dtProvider));
        }

        public TimeSpan PopStop() {
            return ((StopWatch) _stack.Pop()).Elapsed();
        }

        private class StopWatch {
            private readonly DateTimeProvider _dtProvider;
            private readonly DateTime _start;

            public StopWatch(DateTimeProvider dtProvider) {
                _dtProvider = dtProvider;
                _start = dtProvider.Now;
            }

            public TimeSpan Elapsed() {
                return _dtProvider.Now - _start;
            }
        }
    }
}