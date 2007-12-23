using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Xml;

using CIScript.Core.Attributes;



namespace CIScript.Core
{
	public class ElementTaskContainer : DataTypeBase
	{
		public ElementTaskContainer()
		{
			
		}

		#region Private Instance Fields

		private StringCollection _subXMLElements;

		#endregion Private Instance Fields

		#region Override implementation of Element

		/// <summary>
		/// Gets a value indicating whether the element is performing additional
		/// processing using the <see cref="XmlNode" /> that was use to 
		/// initialize the element.
		/// </summary>
		/// <value>
		/// <see langword="true" />, as a <see cref="TaskContainer" /> is
		/// responsable for creating tasks from the nested build elements.
		/// </value>
		protected override bool CustomXmlProcessing 
		{
			get { return true;}
		}

		#endregion Override implementation of Element

		#region Override implementation of Task

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementNode"></param>
		protected override void InitializeElement(XmlNode elementNode)

		{
			base.InitializeElement(elementNode);

			// Exclude any BuildElements (like FileSets, etc.) from our execution elements.
			// These build elements will be handled during the xml init of the task container (Element xmlinit code)
			_subXMLElements = new StringCollection();
			foreach (MemberInfo memInfo in this.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public)) 
			{
				if(memInfo.DeclaringType.Equals(typeof(object))) 
				{
					continue;
				}

				BuildElementAttribute buildElemAttr = (BuildElementAttribute) Attribute.GetCustomAttribute(memInfo, typeof(BuildElementAttribute), true);
				if (buildElemAttr != null) 
				{
					_subXMLElements.Add(buildElemAttr.Name);
				}
			}
		}

		public virtual void Execute()
		{
			ExecuteChildTasks();
		}

		#endregion Override implementation of Task

		#region Protected Instance Methods

		/// <summary>
		/// Creates and executes the embedded (child XML nodes) elements.
		/// </summary>
		/// <remarks>
		/// Skips any element defined by the host <see cref="Task" /> that has 
		/// a <see cref="BuildElementAttribute" /> defined.
		/// </remarks>
		protected virtual void ExecuteChildTasks() 
		{
			foreach (XmlNode childNode in XmlNode) 
			{
				//we only care about xmlnodes (elements) that are of the right namespace.
				if (!(childNode.NodeType == XmlNodeType.Element) || !childNode.NamespaceURI.Equals(NamespaceManager.LookupNamespace("ciscript"))) 
				{
					continue;
				}
                
				// ignore any private xml elements (by def. this includes any property with a BuildElementAttribute (name).
				if (IsPrivateXmlElement(childNode)) 
				{
					continue;
				}

				Task task = CreateChildTask(childNode);
				// for now, we should assume null tasks are because of incomplete metadata about the XML.
				if (task != null) 
				{
					task.Parent = this;
					task.Execute();
				}
			}
		}

		protected virtual Task CreateChildTask(XmlNode node) 
		{
			return Project.CreateTask(node);
		}
        
		protected virtual bool IsPrivateXmlElement(XmlNode node) 
		{
			return (_subXMLElements != null && _subXMLElements.Contains(node.Name));
		}

		protected virtual void AddPrivateXmlElementName(string name) 
		{
			if (_subXMLElements == null) 
			{
				_subXMLElements = new StringCollection();
			}

			if (!_subXMLElements.Contains(name)) 
			{
				_subXMLElements.Add(name);
			}
		}

		#endregion Protected Instance Methods
	}
}
