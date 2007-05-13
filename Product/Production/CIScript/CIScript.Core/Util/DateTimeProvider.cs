// CIScript - A .NET build tool
// Copyright (C) 2001-2005 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Owen Rogers (exortech@gmail.com)

using System;

namespace CIScript.Core.Util {
    public class DateTimeProvider {
        public virtual DateTime Now {
            get { return DateTime.Now; }
        }
    }
}