using System.Collections;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Util;
using System.Globalization;

namespace Macrodef
{
    internal class MacroDefInvocation
    {
        #region Fields

        private readonly ArrayList attributeList;

        private readonly ArrayList elementgroups;

        private readonly ArrayList elements;

        private readonly XmlNode invocationXml;

        private readonly string name;

        private readonly MacroDefSequential sequential;

        private readonly Task task;

        #endregion

        #region Constructors

        public MacroDefInvocation(string name, Task task,
                                          XmlNode invocationXml,
                                          ArrayList attributeList,
                                          MacroDefSequential sequential,
                                          ArrayList elements,
                                          ArrayList elementgroups)
        {
            this.name = name;
            this.task = task;
            this.invocationXml = invocationXml;
            this.attributeList = attributeList;
            this.sequential = sequential;
            this.elements = elements;
            this.elementgroups = elementgroups;
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            task.Log(Level.Verbose, "Running '" + name + "'");
            task.Project.Indent();
            try
            {
                PropertyDictionary oldPropertyValues = new PropertyDictionary(null);

                if (attributeList != null)
                    SetUpProperties(attributeList, task, invocationXml, oldPropertyValues);

                if (sequential != null)
                {
                    XmlNode invocationTasks = CreateInvocationTasks();
                    ExecuteInvocationTasks(invocationTasks);
                }

                RestoreProperties(attributeList, task, oldPropertyValues);
            }
            finally
            {
                task.Project.Unindent();
            }
        }

        #endregion

        #region Protected Methods

        protected virtual Task CreateChildTask(XmlNode node)
        {
            return task.Project.CreateTask(node);
        }

        #endregion

        #region Private Methods

        private XmlNode CreateInvocationTasks()
        {
            XmlNode invocationTasks = sequential.SequentialXml;
            if (elements.Count > 0)
            {
                invocationTasks = invocationTasks.CloneNode(true);
                foreach (MacroElement element in elements)
                {
                    ReplaceMacroElementsInInvocationXml(element.name, invocationTasks);
                }
            }
            if (elementgroups.Count > 0)
            {
                invocationTasks = invocationTasks.CloneNode(true);
                foreach (MacroElementGroup elementgroup in elementgroups)
                {
                    ReplaceMacroElementGroupsInInvocationXml(elementgroup.name, invocationTasks);
                }
            }

            Log(Level.Verbose, "Effective macro definition: " + invocationTasks.InnerXml);
            return invocationTasks;
        }

        private void ExecuteInvocationTasks(XmlNode invocationTasks)
        {
            foreach (XmlNode childNode in invocationTasks)
            {
                if (!(childNode.NodeType == XmlNodeType.Element) ||
                    !childNode.NamespaceURI.Equals(task.NamespaceManager.LookupNamespace("nant")))
                {
                    continue;
                }

                if (TypeFactory.TaskBuilders.Contains(childNode.Name))
                {
                    Task childTask = CreateChildTask(childNode);
                    if (childTask != null)
                    {
                        childTask.Parent = this;
                        childTask.Execute();
                    }
                }
                else if (TypeFactory.DataTypeBuilders.Contains(childNode.Name))
                {
                    DataTypeBase dataType = task.Project.CreateDataTypeBase(childNode);
                    task.Project.Log(Level.Verbose, "Adding a {0} reference with id '{1}'.",
                        childNode.Name, dataType.ID);
                    if (!task.Project.DataTypeReferences.Contains(dataType.ID))
                    {
                        task.Project.DataTypeReferences.Add(dataType.ID, dataType);
                    }
                    else
                    {
                        task.Project.DataTypeReferences[dataType.ID] = dataType; // overwrite with the new reference.
                    }
                }
                else
                {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                        ResourceUtils.GetString("NA1071"),
                        childNode.Name));
                }

            }
        }

        private XmlElement GetInvocationElementDefinition(string elementName)
        {
            XmlElement invocationElementDefinition =
                invocationXml.SelectSingleNode("nant:" + elementName, task.NamespaceManager) as XmlElement;
            if (invocationElementDefinition == null)
                throw new BuildException("Element '" + elementName + "' must be defined");
            return invocationElementDefinition;
        }

        private XmlElement GetInvocationElementGroupDefinition(string elementGroupName)
        {
            XmlElement invocationElementDefinition =
                invocationXml.SelectSingleNode("nant:" + elementGroupName, task.NamespaceManager) as XmlElement;
            if (invocationElementDefinition == null)
                throw new BuildException("ElementGroup '" + elementGroupName + "' must be defined");
            return invocationElementDefinition;
        }

        private void Log(Level level, string s)
        {
            task.Log(level, s);
        }

        private void ReplaceElementGroupPlaceHolderWithInvocationContents(XmlElement invocationElementGroupDefinition, XmlElement elementGroupPlaceHolder)
        {
            XmlNode parentElement = elementGroupPlaceHolder.ParentNode;

            Log(Level.Verbose, "Replacing elementgroup " + elementGroupPlaceHolder.OuterXml + " in " + parentElement.OuterXml);

            foreach (XmlNode definitionStep in invocationElementGroupDefinition.ChildNodes)
            {
                parentElement.InsertBefore(parentElement.OwnerDocument.ImportNode(definitionStep, true), elementGroupPlaceHolder);
            }

            parentElement.RemoveChild(elementGroupPlaceHolder);
        }

        private void ReplaceElementPlaceHolderWithInvocationContents(XmlElement invocationElementDefinition,
                                                                             XmlElement elementPlaceHolder)
        {
            XmlNode parentElement = elementPlaceHolder.ParentNode;

            Log(Level.Verbose, "Replacing element " + elementPlaceHolder.OuterXml + " in " + parentElement.OuterXml);
            parentElement.InsertBefore(parentElement.OwnerDocument.ImportNode(invocationElementDefinition, true), elementPlaceHolder);
            parentElement.RemoveChild(elementPlaceHolder);
        }

        private void ReplaceMacroElementGroupsInInvocationXml(string elementGroupName, XmlNode invocationTasks)
        {
            XmlNodeList elementGroupPlaceholders = invocationTasks.SelectNodes("//nant:elementgroup[@name='" + elementGroupName + "']", task.NamespaceManager);
            Log(Level.Verbose,
                "Inserting " + elementGroupPlaceholders.Count + " call(s) of '" + elementGroupName + "' in " + invocationTasks.InnerXml);

            if (elementGroupPlaceholders.Count > 0)
            {
                XmlElement invocationElementGroupDefinition = GetInvocationElementGroupDefinition(elementGroupName);

                foreach (XmlElement elementGroupPlaceholder in elementGroupPlaceholders)
                {
                    ReplaceElementGroupPlaceHolderWithInvocationContents(invocationElementGroupDefinition, elementGroupPlaceholder);
                }
            }
        }

        private void ReplaceMacroElementsInInvocationXml(string elementName, XmlNode invocationTasks)
        {
            XmlNodeList elementPlaceholders = invocationTasks.SelectNodes("//nant:element[@name='" + elementName + "']", task.NamespaceManager);
            Log(Level.Verbose,
                "Inserting " + elementPlaceholders.Count + " call(s) of '" + elementName + "' in " + invocationTasks.InnerXml);

            if (elementPlaceholders.Count > 0)
            {
                XmlElement invocationElementDefinition = GetInvocationElementDefinition(elementName);

                foreach (XmlElement elementPlaceholder in elementPlaceholders)
                {
                    ReplaceElementPlaceHolderWithInvocationContents(invocationElementDefinition, elementPlaceholder);
                }
            }
        }

        private static void RestoreProperties(ArrayList attributeList, Task task, PropertyDictionary oldValues)
        {
            PropertyDictionary projectProperties = task.Project.Properties;
            foreach (MacroAttribute macroAttribute in attributeList)
            {
                string localPropertyName = macroAttribute.LocalPropertyName;
                string oldValue = oldValues[localPropertyName];

                if (projectProperties.Contains(localPropertyName))
                    projectProperties.Remove(localPropertyName);
                if (oldValue != null)
                    projectProperties.Add(localPropertyName, oldValue);
            }
        }

        private static void SetUpProperties(ArrayList attributeList, Task task, XmlNode xml,
                                                    PropertyDictionary oldPropertyValues)
        {
            PropertyDictionary projectProperties = task.Project.Properties;
            StringBuilder logMessage = new StringBuilder();
            foreach (MacroAttribute macroAttribute in attributeList)
            {
                string attributeName = macroAttribute.name;
                XmlAttribute xmlAttribute = xml.Attributes[attributeName];
                string value = null;
                if (xmlAttribute != null)
                {
                    value = projectProperties.ExpandProperties(xmlAttribute.Value, null);
                }
                else if (macroAttribute.defaultValue != null)
                {
                    value = macroAttribute.defaultValue;
                }

                string localPropertyName = macroAttribute.LocalPropertyName;

                task.Log(Level.Debug, "Setting property " + localPropertyName + " to " + value);
                if (logMessage.Length > 0)
                    logMessage.Append(", ");
                logMessage.Append(localPropertyName);
                logMessage.Append(" = '");
                logMessage.Append(value);
                logMessage.Append("'");

                if (projectProperties.Contains(localPropertyName))
                {
                    oldPropertyValues.Add(localPropertyName, projectProperties[localPropertyName]);
                    projectProperties.Remove(localPropertyName);
                }
                if (value != null)
                    projectProperties.Add(localPropertyName, value);
            }

            task.Log(Level.Info, logMessage.ToString());
        }

        #endregion

    }
}
