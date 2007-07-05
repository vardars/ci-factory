using System;
using System.Xml;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;
using Exortech.NetReflector;

namespace CCNET.Extensions
{

    public class Zation
    {

        public static string Serialize(string reflectorType, object subject)
        {
            StringWriter buffer = new StringWriter();
            new ReflectorTypeAttribute(reflectorType).Write(new XmlTextWriter(buffer), subject);
            return buffer.ToString();
        }

        public static object Deserialize(string serialized)
        {
            return NetReflector.Read(serialized);
        }
    }

}
