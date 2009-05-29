using System;
using System.Linq;
using System.Collections.ObjectModel;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using System.Xml;

namespace CIFactory.NAnt.Tasks
{
    public class AttributeList : KeyedCollection<String, System.Xml.XmlAttribute>
    {

        public void AddList(System.Xml.XmlAttributeCollection attributes)
        {
            if (attributes == null)
                return;

            foreach (XmlAttribute Attribute in attributes)
            {
                this.Add(Attribute);
            }
        }

        public AttributeList() : base()
        {

        }

        public AttributeList(XmlAttributeCollection attributes)
        {
            this.AddList(attributes);
        }

        protected override string GetKeyForItem(System.Xml.XmlAttribute item)
        {
            return item.LocalName;
        }
    }

    [TaskName("propertystructure")]
    public class PropertyStructure : Task
    {

        #region Properties

        protected override bool CustomXmlProcessing
        {
            get { return true; }
        }

        #endregion

        #region Protected Methods

        protected override void InitializeXml(System.Xml.XmlNode elementNode, PropertyDictionary properties, FrameworkInfo framework)
        {
            base.InitializeXml(elementNode, properties, framework);
        }

        #endregion


        protected override void ExecuteTask()
        {
            this.CreateProperties(null, this.XmlNode.ChildNodes);
        }

        private bool NeedToProcessChildren(XmlNodeList children)
        {
            return children.OfType<XmlElement>().ToList<XmlElement>().Count > 0;
        }

        private void CreateProperties(String name, XmlNodeList nodes)
        {
            foreach (XmlNode Child in nodes)
            {
                if (this.NeedToProcessChildren(Child.ChildNodes))
                {
                    String NewName = Child.LocalName;
                    if (!String.IsNullOrEmpty(name))
                        NewName = String.Format("{0}.{1}", name, Child.LocalName);

                    this.CreateProperties(NewName, Child.ChildNodes);
                }
                else if (!String.IsNullOrEmpty(Child.InnerText))
                {
                    String value = Child.InnerText.Trim();

                    AttributeList Attributes = new AttributeList(Child.Attributes);

                    Boolean NoTrim = this.GetAttributeValue("notrim", Attributes);

                    if (NoTrim)
                        value = Child.InnerText;

                    Boolean @readonly = this.GetAttributeValue("readonly", Attributes);
                    Boolean dynamic = this.GetAttributeValue("dynamic", Attributes);
                    Boolean overwrite = this.GetAttributeValue("overwrite", Attributes);

                    String NewName = Child.LocalName;
                    if (!String.IsNullOrEmpty(name))
                        NewName = String.Format("{0}.{1}", name, Child.LocalName);

                    PropertyTask Property = new PropertyTask(NewName, value, @readonly, dynamic, overwrite);
                    Property.Parent = this.Parent;
                    Property.Project = this.Project;
                    Property.Execute();
                }
                else if (Child.Attributes.Count > 0)
                {
                    AttributeList Attributes = new AttributeList(Child.Attributes);

                    if (Attributes.Contains("value"))
                    {
                        String value = Attributes["value"].InnerText;

                        Boolean @readonly = this.GetAttributeValue("readonly", Attributes);
                        Boolean dynamic = this.GetAttributeValue("dynamic", Attributes);
                        Boolean overwrite = this.GetAttributeValue("overwrite", Attributes);

                        String NewName = Child.LocalName;
                        if (!String.IsNullOrEmpty(name))
                            NewName = String.Format("{0}.{1}", name, Child.LocalName);

                        PropertyTask Property = new PropertyTask(NewName, value, @readonly, dynamic, overwrite);
                        Property.Parent = this.Parent;
                        Property.Project = this.Project;
                        Property.Execute();
                    }
                }
            }
        }

        private Boolean GetAttributeValue(String name, AttributeList attributes)
        {
            Boolean value = false;
            if (attributes.Contains(name))
                value = Boolean.Parse(this.Expand(attributes[name].InnerText));
            return value;
        }

        private string Expand(String @string)
        {
            return this.Project.ExpandProperties(@string, this.Location);
        }
    }
}
