using System;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("xmlquery", "DataTypes")]
    public class XmlQueryFunctions : FunctionSetBase
    {
        #region Constructors

        public XmlQueryFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {
        }

        #endregion

        #region Public Methods

        [Function("count")]
        public int Count(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            XmlQuery RefXmlQuery = (XmlQuery)this.Project.DataTypeReferences[refID];

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(RefXmlQuery.File);

            XmlNamespaceManager Manager = new XmlNamespaceManager(XmlDoc.NameTable);
            foreach (XmlNamespace NameSpace in RefXmlQuery.Namespaces)
            {
                if (NameSpace.IfDefined && !NameSpace.UnlessDefined)
                    Manager.AddNamespace(NameSpace.Prefix, NameSpace.Uri);
            }

            return XmlDoc.SelectNodes(RefXmlQuery.Query, Manager).Count;
        }

        #endregion

    }
}
