using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Reflection;
using TestCoverage;


namespace TestCoverageRunner
{
    public class XmlReporting : IReporting
    {
        
#region Fields

        private StringCollection _TestAssemblies;
        private StringCollection _ProcessedTestAssemblies;
        private StringCollection _ProductionAssemblies;
        private StringCollection _ProcessedProductionAssemblies;
        private Stream _ReportStream;
        private string _ProjectName;
        private FixtureTable _TestFixtures;
        private Hashtable _TestSuites;
        private static StringCollection HintDirectories = new StringCollection();

#endregion
        
#region Properties

        public StringCollection ProcessedTestAssemblies
        {
            get
            {
                if (_ProcessedTestAssemblies == null)
                    _ProcessedTestAssemblies = new StringCollection();
                return _ProcessedTestAssemblies;
            }
            set
            {
                _ProcessedTestAssemblies = value;
            }
        }

        public StringCollection ProcessedProductionAssemblies
        {
            get
            {
                if (_ProcessedProductionAssemblies == null)
                    _ProcessedProductionAssemblies = new StringCollection();
                return _ProcessedProductionAssemblies;
            }
            set
            {
                _ProcessedProductionAssemblies = value;
            }
        }

        public Hashtable TestSuites
        {
            get
            {
                if (_TestSuites == null)
                    _TestSuites = new Hashtable();
                return _TestSuites;
            }
            set
            {
                _TestSuites = value;
            }
        }

        private FixtureTable TestFixtures
        {
            get
            {
                if (_TestFixtures == null)
                    _TestFixtures = new FixtureTable();
                return _TestFixtures;
            }
            set
            {
                _TestFixtures = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }

        public StringCollection TestAssemblies
        {
            get
            {
                if (_TestAssemblies == null)
                    _TestAssemblies = new StringCollection();
                return _TestAssemblies;
            }
            set
            {
                _TestAssemblies = value;
            }
        }
        public StringCollection ProductionAssemblies
        {
            get
            {
                if (_ProductionAssemblies == null)
                    _ProductionAssemblies = new StringCollection();
                return _ProductionAssemblies;
            }
            set
            {
                _ProductionAssemblies = value;
            }
        }
        public Stream ReportStream
        {
            get
            {
                return _ReportStream;
            }
            set
            {
                _ReportStream = value;
            }
        }

#endregion
        
#region Constructors

        public XmlReporting()
        {

        }

        public XmlReporting(StringCollection testAssemblies, StringCollection productionAssemblies, Stream reportStream, string projectName)
        {
            _TestAssemblies = testAssemblies;
            _ProductionAssemblies = productionAssemblies;
            _ReportStream = reportStream;
            _ProjectName = projectName;
        }

#endregion

#region Report Genration

        public void GenerateReport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
            this.BuildTestSubjectsCovered();
            this.WriteXmlReport();
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyResolve);
        }

        private void WriteXmlReport()
        {
            XmlTextWriter Writer = new XmlTextWriter(this.ReportStream, System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartDocument();
            Writer.WriteStartElement("TestCoverage");

            Writer.WriteStartAttribute("Time", null);
            Writer.WriteString(System.DateTime.Now.ToString("F"));
            Writer.WriteEndAttribute();

            Writer.WriteStartAttribute("ProjectName", null);
            Writer.WriteString(this.ProjectName);
            Writer.WriteEndAttribute();

            Assembly Asm;

            foreach (String AsmFileName in this.ProductionAssemblies)
            {
                if (!this.ProcessedProductionAssemblies.Contains(Path.GetFileName(AsmFileName)))
                {
                    this.ProcessedProductionAssemblies.Add(Path.GetFileName(AsmFileName));
                    HintDirectories.Add(Path.GetDirectoryName(AsmFileName));
                    if (System.IO.File.Exists(AsmFileName))
                    {
                        Asm = Assembly.LoadFile(AsmFileName);
                    }
                    else
                    {
                        Asm = Assembly.Load(AsmFileName);
                    }

                    Writer.WriteStartElement("Assembly");

                    Writer.WriteStartAttribute("Name", null);
                    Writer.WriteString(Path.GetFileName(Asm.Location));
                    Writer.WriteEndAttribute();

                    String TestAssemblyName = string.Empty;
                    if (this.TestSuites.Contains(Path.GetFileName(Asm.Location)))
                    {
                        TestAssemblyName = (string)this.TestSuites[Path.GetFileName(Asm.Location)];
                    }

                    Writer.WriteStartAttribute("TestSuite", null);
                    Writer.WriteString(TestAssemblyName);
                    Writer.WriteEndAttribute();

                    foreach (Type ProdType in Asm.GetTypes())
                    {
                        this.ProcessType(ProdType, Writer);
                    }
                    Writer.WriteEndElement();
                }
            }
            Writer.WriteEndElement();
            Writer.WriteEndDocument();
            Writer.Flush();
        }

        private void BuildTestSubjectsCovered()
        {
            foreach (String AsmFileName in this.TestAssemblies)
            {
                if (!this.ProcessedTestAssemblies.Contains(Path.GetFileName(AsmFileName)))
                {
                    this.ProcessedTestAssemblies.Add(Path.GetFileName(AsmFileName));
                    HintDirectories.Add(Path.GetDirectoryName(AsmFileName));
                    Assembly Asm = Assembly.LoadFile(AsmFileName);

                    if (this.HasTestCoverage(Asm))
                    {
                        TestSubjectAssemblyAttribute AssemblyTestCoverage = this.GetAssemblyTestCoverage(Asm);
                        this.ProductionAssemblies.Add(AssemblyTestCoverage.Location);
                        this.TestSuites.Add(Path.GetFileName(AssemblyTestCoverage.Location), Path.GetFileName(Asm.Location));
                    }

                    foreach (Type TestType in Asm.GetTypes())
                    {
                        this.ProcessTestType(TestType);
                    }
                }
            }
        }

#endregion

#region Process Types

        private void ProcessType(Type prodType, XmlTextWriter writer)
        {
            if (!prodType.IsEnum || !prodType.IsInterface)
            {
                foreach (Type NestedType in prodType.GetNestedTypes())
                {
                    this.ProcessType(NestedType, writer);
                }

                TestFixture CurrentFixture = null;
                if (this.TestFixtures.Contains(prodType.FullName))
                {
                    CurrentFixture = this.TestFixtures[prodType.FullName];
                }


                writer.WriteStartElement("Class");

                writer.WriteStartAttribute("FullName", null);
                writer.WriteString(prodType.FullName);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("TestFixture", null);
                if (CurrentFixture == null)
                {
                    writer.WriteString(String.Empty);
                }
                else
                {
                    writer.WriteString(CurrentFixture.FullName);
                }

                writer.WriteEndAttribute();

                String MemberType;
                bool IsCoverable;
                foreach (MemberInfo Member in prodType.GetMembers())
                {
                    MemberType = String.Empty;
                    IsCoverable = false;
                    if (Member is ConstructorInfo)
                    {
                        IsCoverable = true;
                        MemberType = "Constructor";
                    }
                    else if (Member is MethodInfo)
                    {
                        if (!Regex.IsMatch(Member.Name, @"get_|set_"))
                        {
                            IsCoverable = true;
                            MemberType = "Method";
                        }
                    }
                    else if (Member is PropertyInfo)
                    {
                        IsCoverable = true;
                        MemberType = "Property";
                    }

                    if (IsCoverable && Member.DeclaringType == Member.ReflectedType)
                    {
                        writer.WriteStartElement("Member");

                        writer.WriteStartAttribute("Name", null);
                        writer.WriteString(Member.Name);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("MemberType", null);
                        writer.WriteString(MemberType);
                        writer.WriteEndAttribute();

                        writer.WriteStartAttribute("TestMethod", null);
                        if (CurrentFixture != null && CurrentFixture.Contains(Member.Name))
                        {
                            writer.WriteString(CurrentFixture[Member.Name]);
                        }
                        else
                        {
                            writer.WriteString(String.Empty);
                        }

                        writer.WriteEndAttribute();

                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }
        }

        private void ProcessTestType(Type testType)
        {
            if (!testType.IsEnum || !testType.IsInterface)
            {
                foreach (Type NestedType in testType.GetNestedTypes())
                {
                    this.ProcessTestType(NestedType);
                }
                if (this.HasTestCoverage(testType))
                {
                    TestSubjectClassAttribute ClassTestCoverage = this.GetClassTestCoverage(testType);
                    if (!this.TestFixtures.Contains(ClassTestCoverage.TestSubject.FullName))
                    {
                        this.TestFixtures.Add(ClassTestCoverage.TestSubject.FullName, new TestFixture(testType.FullName));
                    }
                    foreach (MethodInfo Method in testType.GetMethods())
                    {
                        if (this.HasTestCoverage(Method))
                        {
                            TestSubjectMemberAttribute MemberTestCoverage = this.GetMemberTestCoverage(Method);
                            if (!this.TestFixtures[ClassTestCoverage.TestSubject.FullName].Contains(MemberTestCoverage.MemeberName))
                                this.TestFixtures[ClassTestCoverage.TestSubject.FullName].Add(MemberTestCoverage.MemeberName, Method.Name);
                        }
                    }
                }
            }
        }
        
#endregion

#region Reflection Helpers

        private static System.Reflection.Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            char[] Separators = new char[1] { ',' };
            string AsmName = args.Name.Split(Separators)[0];
            foreach (string Directory in HintDirectories)
            {
                string AsmPath = Path.Combine(Directory, string.Format("{0}.dll", AsmName));
                if (System.IO.File.Exists(AsmPath))
                {
                    return Assembly.LoadFrom(AsmPath);
                }
                AsmPath = Path.Combine(Directory, string.Format("{0}.exe", AsmName));
                if (System.IO.File.Exists(AsmPath))
                {
                    return Assembly.LoadFrom(AsmPath);
                }
                AsmPath = Path.Combine(Directory, AsmName);
                if (System.IO.File.Exists(AsmPath))
                {
                    return Assembly.LoadFrom(AsmPath);
                }
            }
            return null;
        }

        private bool HasTestCoverage(Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(TestSubjectAssemblyAttribute), false).Length > 0;
        }

        private TestSubjectAssemblyAttribute GetAssemblyTestCoverage(Assembly assembly)
        {
            return (TestSubjectAssemblyAttribute)assembly.GetCustomAttributes(typeof(TestSubjectAssemblyAttribute), false)[0];
        }

        private bool HasTestCoverage(Type testType)
        {
            return testType.GetCustomAttributes(typeof(TestSubjectClassAttribute), false).Length > 0;
        }

        private TestSubjectClassAttribute GetClassTestCoverage(Type testType)
        {
            return (TestSubjectClassAttribute)testType.GetCustomAttributes(typeof(TestSubjectClassAttribute), false)[0];
        }

        private bool HasTestCoverage(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(TestSubjectMemberAttribute), false).Length > 0;
        }

        private TestSubjectMemberAttribute GetMemberTestCoverage(MethodInfo method)
        {
            return (TestSubjectMemberAttribute)method.GetCustomAttributes(typeof(TestSubjectMemberAttribute), false)[0];
        }

#endregion

        public void Test()
        {
            try
            {
                this.ReportStream = new FileStream(@"C:\temp\Report.xml", FileMode.Create);
                this.ProjectName = "Test";
                this.TestAssemblies.Add(@"C:\Projects\EF_COTS\1.6.1\Product\Unit Test\Test.EF.Pluggin\bin\Test.EF.Pluggin.dll");
                this.GenerateReport();
                this.ReportStream.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
