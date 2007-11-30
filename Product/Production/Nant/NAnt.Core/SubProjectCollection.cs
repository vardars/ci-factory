using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using NAnt.Core.Tasks;
using NAnt.Core.Util;

namespace NAnt.Core
{
    public class SubProjectCollection : System.Collections.ObjectModel.KeyedCollection<string, SubProject>
    {
        protected override string GetKeyForItem(SubProject item)
        {
            return item.ProjectName;
        }
    }
}
