using System;
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

        private void CreateProperties(String name, XmlNodeList nodes)
        {
            foreach (XmlNode Child in nodes)
            {
                if (Child.HasChildNodes)
                {
                    String NewName = Child.LocalName;
                    if (!String.IsNullOrEmpty(name))
                        NewName = String.Format("{0}.{1}", name, Child.LocalName);

                    this.CreateProperties(NewName, Child.ChildNodes);
                } 
                else if (!String.IsNullOrEmpty(Child.LocalName) && Child.LocalName == "#text")
                {
                    String value = Child.Value;

                    AttributeList Attributes = new AttributeList(Child.ParentNode.Attributes);

                    Boolean @readonly = false;
                    if (Attributes.Contains("readonly"))
                        @readonly = Boolean.Parse(Attributes["readonly"].Value);

                    Boolean dynamic = false;
                    if (Attributes.Contains("dynamic"))
                        dynamic = Boolean.Parse(Attributes["dynamic"].Value);

                    Boolean overwrite = false;
                    if (Attributes.Contains("overwrite"))
                        overwrite = Boolean.Parse(Attributes["overwrite"].Value);

                    PropertyTask Property = new PropertyTask(name, value, @readonly, dynamic, overwrite);
                    Property.Parent = this.Parent;
                    Property.Project = this.Project;
                    Property.Execute();
                }
            }
        }
    }
}
