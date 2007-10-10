using System;
using System.Xml;
using System.Collections;
using System.Reflection;
using NAnt.Core;
using NAnt.Core.Util;
using NAnt.Core.Attributes;
using System.Globalization;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Types
{
    [ElementName("items")]
    public class LoopItemContainer : DataTypeBase, IEnumerable
    {
        #region Fields

        private LoopItems _Items;

        #endregion

        #region Properties

        [BuildElement("loopitem", Required = true)]
        public LoopItems Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        #endregion

        #region Public Methods

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region Protected Methods

        protected override void InitializeXml(System.Xml.XmlNode elementNode, PropertyDictionary properties, FrameworkInfo framework)
        {
            this.XmlNode = elementNode;
            LoopItemsConfigurator configurator = new LoopItemsConfigurator(this, elementNode, properties, framework);
            configurator.Initialize();
        }

        #endregion

        private class LoopItemsConfigurator : Element.AttributeConfigurator
        {
            #region Constructors

            public LoopItemsConfigurator(Element element, XmlNode elementNode, PropertyDictionary properties, FrameworkInfo targetFramework)
                : base(element, elementNode, properties, targetFramework)
            {

            }

            #endregion

            #region Protected Methods

            protected override bool InitializeChildElement(System.Reflection.PropertyInfo propertyInfo)
            {
                XmlNode CurrentNode;
                object[] objArray1;
                BuildElementAttribute attribute1 = (BuildElementAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(BuildElementAttribute), false);
                if ((attribute1 == null))
                {
                    return false;
                }
                if ((this.ElementXml.OwnerDocument.DocumentElement == null))
                {
                    CurrentNode = this.ElementXml[attribute1.Name];
                }
                else
                {
                    CurrentNode = this.ElementXml[attribute1.Name, this.ElementXml.OwnerDocument.DocumentElement.NamespaceURI];
                }
                if (propertyInfo.Name == "Items")
                {
                    foreach (DataTypeBaseBuilder Builder in TypeFactory.DataTypeBuilders)
                    {
                        CurrentNode = this.ElementXml[Builder.DataTypeName];
                        if (CurrentNode != null)
                        {
                            System.Reflection.Assembly SubjectAssembly = System.Reflection.Assembly.LoadFrom(Builder.AssemblyFileName);
                            if (SubjectAssembly.GetType(Builder.ClassName).IsSubclassOf(typeof(LoopItems)))
                            {
                                break; // TODO: might not be correct. Was : Exit For
                            }
                            CurrentNode = null;
                        }
                    }
                }
                if (((CurrentNode == null) && attribute1.Required))
                {
                    objArray1 = new object[] { attribute1.Name, this.Name };
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, ResourceUtils.GetString("NA1013"), objArray1), this.Location);
                }
                if ((CurrentNode != null))
                {
                    this.UnprocessedChildNodes.Remove(CurrentNode.Name);
                    if (!attribute1.ProcessXml)
                    {
                        return true;
                    }
                    this.CreateChildBuildElement(propertyInfo, CurrentNode, this.Properties, this.TargetFramework);
                    if ((this.ElementXml.SelectNodes(("nant:" + attribute1.Name), this.NamespaceManager).Count > 1))
                    {
                        objArray1 = new object[] { this.Name, attribute1.Name };
                        throw new BuildException(string.Format(CultureInfo.InvariantCulture, "<{0} ... /> does not support multiple '{1}' child elements.", objArray1), this.Location);
                    }
                }
                return true;
            }

            #endregion

            #region Private Methods

            private Element CreateChildBuildElement(PropertyInfo propInfo, XmlNode xml, PropertyDictionary properties, FrameworkInfo framework)
            {
                MethodInfo getter = null;
                MethodInfo setter = null;
                Element childElement = null;
                Type elementType = null;
                setter = propInfo.GetSetMethod(true);
                getter = propInfo.GetGetMethod(true);
                if (getter != null)
                {
                    try
                    {
                        childElement = (Element)propInfo.GetValue(Element, null);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new BuildException(
                            string.Format(
                                "Property \"{0}\" for class \"{1}\" is backed by \"{2}\" which does not derive from \"{3}\".",
                                propInfo.Name,
                                Element.GetType().FullName,
                                propInfo.PropertyType.FullName,
                                typeof(Element).FullName),
                            ex
                        );
                    }
                    if (childElement == null)
                    {
                        if (setter == null)
                        {
                            throw new BuildException(string.Format("Property {0} cannot return null (if there is no set method) for class {1}", propInfo.Name, Element.GetType().FullName), Location);
                        }
                        else
                        {
                            getter = null;
                            this.Project.Log(Level.Debug, string.Format("{0}_get() returned null; will go the route of set method to populate.", propInfo.Name));
                        }
                    }
                    else
                    {
                        elementType = childElement.GetType();
                    }
                }
                if (getter == null && setter != null)
                {
                    elementType = setter.GetParameters()[0].ParameterType;
                    if (elementType.IsAbstract)
                    {
                        DataTypeBaseBuilder Builder;
                        Builder = TypeFactory.DataTypeBuilders[xml.Name];
                        if (Builder == null)
                        {
                            throw new InvalidOperationException(string.Format("Abstract type: {0} for {2}.{1}", elementType.Name, propInfo.Name, Name));
                        }
                        childElement = Builder.CreateDataTypeBase();
                    }
                    else
                    {
                        childElement = (Element)Activator.CreateInstance(elementType, (BindingFlags.NonPublic | (BindingFlags.Public | BindingFlags.Instance)), null, null, CultureInfo.InvariantCulture);
                    }
                }
                childElement = Element.InitializeBuildElement(Element, xml, childElement, elementType);
                DataTypeBase dataType = (DataTypeBase)childElement;
                if (dataType != null && xml.Attributes["refid"] != null)
                {
                    if (setter == null)
                    {
                        throw new BuildException(string.Format("DataType child element '{0}' in class '{1}' must define a set method.", propInfo.Name, this.GetType().FullName));
                    }
                    getter = null;
                }
                if (setter != null && getter == null)
                {
                    setter.Invoke(Element, new object[] { childElement });
                }
                return childElement;
            }

            #endregion

        }
    }

}