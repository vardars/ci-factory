// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Ian McLean (ianm@activestate.com)
// Mitch Denny (mitch.denny@monash.net)

using System;
using System.Globalization;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;
using System.Xml.XPath;
using System.Reflection;
using System.Text;

namespace NAnt.Core.Tasks {
    /// <summary>
    /// Extracts text from an XML file at the location specified by an XPath 
    /// expression.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the XPath expression specifies multiple nodes the node index is used 
    /// to determine which of the nodes' text is returned.
    /// </para>
    /// </remarks>
    /// <example>
    ///   <para>
    ///   The example provided assumes that the following XML file (App.config)
    ///   exists in the current build directory.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration xmlns="http://www.gordic.cz/shared/project-config/v_1.0.0.0">
    ///     <appSettings>
    ///         <add key="server" value="testhost.somecompany.com" />
    ///     </appSettings>
    /// </configuration>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   The example will read the server value from the above
    ///   configuration file.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <xmlpeek
    ///     file="App.config"
    ///     xpath="/x:configuration/x:appSettings/x:add[@key = 'server']/@value"
    ///     property="configuration.server">
    ///     <namespaces>
    ///         <namespace prefix="x" uri="http://www.gordic.cz/shared/project-config/v_1.0.0.0" />
    ///     </namespaces>
    /// </xmlpeek>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("xmlpeek")]
    public class XmlPeekTask : Task {
        #region Private Instance Fields

        private FileInfo _xmlFile;
        private int? _nodeIndex = null;
        private string _property;
        private string _xPath;
        private XmlNamespaceCollection _namespaces = new XmlNamespaceCollection();
		private bool _OuterXml;

        #endregion Private Instance Fields

        #region Public Instance Properties

		/// <summary>
		/// Will grab the outterxml instead of the innerxml.
		/// </summary>
		[TaskAttribute("outerxml", Required = false), BooleanValidator()]
		public bool OuterXml
		{
			get { return _OuterXml; }
			set { _OuterXml = value; }
		}

        /// <summary>
        /// The name of the file that contains the XML document
        /// that is going to be peeked at.
        /// </summary>
        [TaskAttribute("file", Required=true)]
        public FileInfo XmlFile {
            get { return _xmlFile; }
            set { _xmlFile = value; }
        }

        /// <summary>
        /// The index of the node that gets its text returned when the query 
        /// returns multiple nodes.
        /// </summary>
        [TaskAttribute("nodeindex", Required=false)]
        public int NodeIndex {
            get { return (int)_nodeIndex; }
			set { _nodeIndex = (int)value; }
        }

        /// <summary>
        /// The property that receives the text representation of the XML inside 
        /// the node returned from the XPath expression.
        /// </summary>
        [TaskAttribute("property", Required=true)]
        [StringValidator(AllowEmpty=false)]
        public string Property {
            get { return _property; }
            set { _property = value; }
        }

        /// <summary>
        /// The XPath expression used to select which node to read.
        /// </summary>
        [TaskAttribute("xpath", Required=true)]
        [StringValidator(AllowEmpty=false)]
        public string XPath {
            get { return _xPath; }
            set { _xPath = value; }
        }

        /// <summary>
        /// Namespace definitions to resolve prefixes in the XPath expression.
        /// </summary>
        [BuildElementCollection("namespaces", "namespace")]
        public XmlNamespaceCollection Namespaces {
            get { return _namespaces; }
            set { _namespaces = value; }
        }

        #endregion Public Instance Properties

        #region Override implementation of Task

        /// <summary>
        /// Executes the XML peek task.
        /// </summary>
        protected override void ExecuteTask() {
            Log(Level.Info, "Peeking at '{0}' with XPath expression '{1}'.", 
                XmlFile.FullName,  XPath);

            // ensure the specified xml file exists
            if (!XmlFile.Exists) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                                                       ResourceUtils.GetString("NA1154"), XmlFile.FullName), Location);
            }

            try {
                XmlDocument document = LoadDocument(XmlFile.FullName);
                Properties[Property] = GetNodeContents(XPath, document);
            } catch (BuildException ex) {
                throw ex; // Just re-throw the build exceptions.
            } catch (Exception ex) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                    ResourceUtils.GetString("NA1153"), XmlFile.FullName), 
                    Location, ex);
            }
        }
        
        #endregion Override implementation of Task
        
        #region private Instance Methods

        /// <summary>
        /// Loads an XML document from a file on disk.
        /// </summary>
        /// <param name="fileName">The file name of the file to load the XML document from.</param>
        /// <returns>
        /// A <see cref="XmlDocument">document</see> containing
        /// the document object representing the file.
        /// </returns>
        private XmlDocument LoadDocument(string fileName)  {
            XmlDocument document = null;

            try {
                document = new XmlDocument();
                document.Load(fileName);
                return document;
            } catch (Exception ex) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                    ResourceUtils.GetString("NA1158"), fileName), Location, 
                    ex);
            }
        }

        private string GetNodeContents(string xpath, XmlDocument document) {
			object XPathResult = null;

            try {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(document.NameTable);
                foreach (XmlNamespace xmlNamespace in Namespaces) {
                    if (xmlNamespace.IfDefined && !xmlNamespace.UnlessDefined) {
                        nsMgr.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Uri);
                    }
                }
				XPathNavigator Navigator =  document.CreateNavigator();
				XPathExpression XPression = Navigator.Compile(xpath);
				XPression.SetContext(nsMgr);

				switch (XPression.ReturnType)
				{
					case XPathResultType.Boolean:
					case XPathResultType.Number:
					case XPathResultType.String:
						return Navigator.Evaluate(XPression).ToString();
					case XPathResultType.NodeSet:
						XPathNodeIterator Iterator = (XPathNodeIterator)Navigator.Select(XPression);
						if (Iterator == null || Iterator.Count == 0)
							throw new BuildException(string.Format(CultureInfo.InvariantCulture,
								ResourceUtils.GetString("NA1156"), xpath),
								Location);

						if (this._nodeIndex != null && this.NodeIndex >= Iterator.Count)
						{
							throw new BuildException(string.Format(CultureInfo.InvariantCulture,
								ResourceUtils.GetString("NA1157"), this.NodeIndex), Location);
						}

						StringBuilder ResultBuilder = new StringBuilder();
						int NodeCount = -1;
						while (Iterator.MoveNext())
						{
							if (this._nodeIndex != null)
							{
								++NodeCount;
								if (this.NodeIndex != NodeCount)
								{
									continue;
								}
							}
							if (this.OuterXml)
								ResultBuilder.Append(Iterator.Current.OuterXml);
							else
								ResultBuilder.Append(Iterator.Current.InnerXml);
						}
						return ResultBuilder.ToString();
					default:
                        throw new InvalidOperationException(string.Format("The xpath return type of {0} is not yet supported!", XPression.ReturnType));
				}
            } catch (Exception ex) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                    ResourceUtils.GetString("NA1155"), xpath), 
                    Location, ex);
            }
        }

        #endregion private Instance Methods
    }
}