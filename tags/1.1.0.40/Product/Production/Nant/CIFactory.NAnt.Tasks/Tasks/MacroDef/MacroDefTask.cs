using System;
using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using System.Text;

namespace Macrodef
{
    /// <summary>
    /// Defines a new task.
    /// </summary>
    /// <remarks>
    /// Derived from <a href="http://ant.apache.org/manual/CoreTasks/macrodef.html">Ant's macrodef task</a>.
    /// Defines a new task, called <see cref="name"/>, which uses the
    /// <see cref="StuffToDo"/> element as a template.
    /// The new task can have xml <see cref="Attributes"/> and xml child <see cref="Elements"/>.
    /// </remarks>
    /// <example>
    ///   <para>Simple Macro.</para>
    ///   <code>
    ///   <![CDATA[
    /// <macrodef name="mytask">
    ///		<sequential>
    ///			<echo messasge="mytask invoked!"/>
    ///		</sequential>
    /// </macrodef>
    /// <mytask/>
    ///   ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>Receive Parameters.</para>
    ///   <code>
    ///   <![CDATA[
    /// <macrodef name="assert-equals">
    ///   <attributes>
    ///     <attribute name="name"/>
    ///     <attribute name="expected"/>
    ///     <attribute name="actual"/>
    ///   </attributes>
    ///	  <sequential>
    ///     <fail if="${ expected != actual}" message="${name}: expected '${expected}' but was '${actual}'"/>
    ///   </sequential>
    /// </macrodef>
    ///   ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>Receive Callable Elements.</para>
    ///   <code>
    ///   <![CDATA[<macrodef name="macro-with-elements">
    ///		<elements>
    ///			<element name="echo"/>
    ///		</elements>
    ///		<sequential>
    ///			<echo message="before element1"/>
    ///			<element name="echo"/>
    ///			<echo message="after element1"/>
    ///		</sequential>
    ///	</macrodef>
    ///	<macro-with-elements>
    ///		<echo message="element1"/>
    ///	</macro-with-elements>
    ///   ]]>
    ///   </code>
    /// </example>
    [TaskName("macrodef")]
    public class MacroDefTask : Task
    {
        #region Readonly

        private static readonly string[] _defaultNamespaces = {
		                                                      	"System",
		                                                      	"System.Collections",
		                                                      	"System.Collections.Specialized",
		                                                      	"System.IO",
		                                                      	"System.Text",
		                                                      	"System.Text.RegularExpressions",
		                                                      	"NAnt.Core",
		                                                      	"NAnt.Core.Attributes"
		                                                      };

        #endregion

        #region Fields

        private ArrayList _attributes = new ArrayList();

        private ArrayList _elementgroups = new ArrayList();

        private ArrayList _elements = new ArrayList();

        private string _name;

        private MacroDefSequential _sequential;

        private Assembly compiledAssembly;

        private static IDictionary macrodefs = new Hashtable();

        private string typeName = "nant" + Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        #endregion

        #region Properties

        /// <summary>
        /// Attributes to the task - simple xml attributes on the macro invocation.
        /// </summary>
        [BuildElementCollection("attributes", "attribute", ElementType = typeof(MacroAttribute))]
        public ArrayList Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Attributes to the task - xml child elements of the macro invocation.
        /// </summary>
        [BuildElementCollection("elementgroups", "elementgroup", ElementType = typeof(MacroElementGroup))]
        public ArrayList ElementGroups
        {
            get { return _elementgroups; }
        }

        /// <summary>
        /// Attributes to the task - xml elements of the macro invocation.
        /// </summary>
        [BuildElementCollection("elements", "element", ElementType = typeof(MacroElement))]
        public ArrayList Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// The name of the macro.
        /// </summary>
        [TaskAttribute("name", Required = true)]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The tasks to execute when this macro is invoked.
        /// </summary>
        [BuildElement("sequential", Required = true)]
        public MacroDefSequential StuffToDo
        {
            get { return _sequential; }
            set { _sequential = value; }
        }

        #endregion

        #region Public Methods

        public static void ExecuteTask(string name, XmlNode xml, Task task)
        {
            MacroDefTask macrodef = (MacroDefTask)macrodefs[name];
            macrodef.Invoke(xml, task);
        }

        public CodeCompileUnit GenerateCode()
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            CodeNamespace nspace = CreateNamespaceWithDefaultImports();
            compileUnit.Namespaces.Add(nspace);

            CodeTypeDeclaration taskClassDeclaration = CreateTaskClassDeclaration();
            nspace.Types.Add(taskClassDeclaration);

            AddGeneratedCodeToTaskClass(taskClassDeclaration);

            return compileUnit;
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            macrodefs.Add(_name, this);

            CodeCompileUnit compileUnit = GenerateCode();
            SimpleCSharpCompiler simpleCSharpCompiler = new SimpleCSharpCompiler();

            compiledAssembly = simpleCSharpCompiler.CompileAssembly(compileUnit);
            LogGeneratedCode(simpleCSharpCompiler, compileUnit);

            TypeFactory.ScanAssembly(compiledAssembly, this);
        }

        #endregion

        #region Private Methods

        private static void AddDefaultImports(CodeNamespace nspace)
        {
            foreach (string nameSpace in _defaultNamespaces)
            {
                nspace.Imports.Add(new CodeNamespaceImport(nameSpace));
            }
        }

        private void AddGeneratedCodeToTaskClass(CodeTypeDeclaration taskClassDeclaration)
        {
            string codeBody =
                @"
				private System.Xml.XmlNode _node;

				protected override void ExecuteTask()
				{
					Macrodef.MacroDefTask.ExecuteTask(""" +
                _name +
                @""", _node, this);
				}
				
				protected override void InitializeXml(System.Xml.XmlNode elementNode, PropertyDictionary properties, FrameworkInfo framework) 
				{
					_node = elementNode;
                    this.Verbose = " + this.Verbose.ToString().ToLower() + @";
				}
			";
            taskClassDeclaration.Members.Add(new CodeSnippetTypeMember(codeBody));
            AddAttributesToClass(taskClassDeclaration);
            AddElementsToClass(taskClassDeclaration);
            AddElementGroupsToClass(taskClassDeclaration);
        }

        private void AddElementGroupsToClass(CodeTypeDeclaration taskClassDeclaration)
        {
            StringBuilder builder = new StringBuilder();

            foreach (MacroElementGroup elementGroup in this.ElementGroups)
            {
                builder.AppendFormat(
                        @"
private {1} _{0};

[BuildElementCollection(""{0}"", ""{3}"", ElementType = typeof({1}))]
public {1} {0}
{{
    get {{ return _{0}; }}
    set {{ _{0} = value; }}
}}
",
                    elementGroup.name,
                    elementGroup.Type,
                    elementGroup.Require.ToString().ToLower(),
                    elementGroup.ElementName
                );
            }

            taskClassDeclaration.Members.Add(new CodeSnippetTypeMember(builder.ToString()));
        }

        private void AddElementsToClass(CodeTypeDeclaration taskClassDeclaration)
        {
            StringBuilder builder = new StringBuilder();

            foreach (MacroElement element in this.Elements)
            {
                builder.AppendFormat(
                        @"
private {1} _{0};

[BuildElement(""{0}"", Required = {2})]
public {1} {0}
{{
    get {{ return _{0}; }}
    set {{ _{0} = value; }}
}}
",
                    element.name,
                    element.Type,
                    element.Require.ToString().ToLower()
                );
            }

            taskClassDeclaration.Members.Add(new CodeSnippetTypeMember(builder.ToString()));
        }

        private void AddAttributesToClass(CodeTypeDeclaration taskClassDeclaration)
        {
            StringBuilder builder = new StringBuilder();

            foreach (MacroAttribute attribute in this.Attributes)
            {
                builder.AppendFormat(
                        @"
private {1} _{0};

[TaskAttribute(""{0}"", Required = {2})]
public {1} {0}
{{
    get {{ return _{0}; }}
    set {{ _{0} = value; }}
}}
",
                    attribute.name,
                    attribute.Type,
                    attribute.Require.ToString().ToLower()
                );
            }

            taskClassDeclaration.Members.Add(new CodeSnippetTypeMember(builder.ToString()));
        }

        private static CodeNamespace CreateNamespaceWithDefaultImports()
        {
            CodeNamespace nspace = new CodeNamespace();
            AddDefaultImports(nspace);
            return nspace;
        }

        private CodeTypeDeclaration CreateTaskClassDeclaration()
        {
            CodeTypeDeclaration typeDecl = new CodeTypeDeclaration(typeName);

            typeDecl.IsClass = true;
            typeDecl.TypeAttributes = TypeAttributes.Public;

            typeDecl.BaseTypes.Add(typeof(Task));

            CodeAttributeDeclaration attrDecl = new CodeAttributeDeclaration("TaskName");
            attrDecl.Arguments.Add(new CodeAttributeArgument(
                                    new CodeVariableReferenceExpression("\"" + name + "\"")));

            typeDecl.CustomAttributes.Add(attrDecl);
            return typeDecl;
        }

        private void Invoke(XmlNode xml, Task task)
        {
            MacroDefInvocation invocation = new MacroDefInvocation(name, task, xml, _attributes, _sequential, _elements, _elementgroups);
            invocation.Execute();
        }

        private void LogGeneratedCode(SimpleCSharpCompiler simpleCSharpCompiler, CodeCompileUnit compileUnit)
        {
            Log(Level.Verbose, simpleCSharpCompiler.GetSourceCode(compileUnit));
            Type compiledType = compiledAssembly.GetType(typeName);
            Log(Level.Verbose, "Created type " + compiledType + " in " + compiledAssembly.Location);
        }

        #endregion

    }
}
