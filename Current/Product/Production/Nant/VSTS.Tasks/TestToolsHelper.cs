using System;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Text;

namespace VSTS.Tasks
{
    public class TestToolsHelper
    {
        private static StringCollection HintDirectories = new StringCollection();
        private static Assembly _CommandLineAssembly;

        public static Assembly CommandLineAssembly
        {
            get
            {
                if (_CommandLineAssembly == null)
                {
                    foreach (Assembly Canidate in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (Canidate.GetName().Name == "Microsoft.VisualStudio.QualityTools.CommandLine")
                        {
                            HintDirectories.Add(Path.GetDirectoryName(Canidate.Location));
                            _CommandLineAssembly = Canidate;
                            break;
                        }
                    }
                }
                return _CommandLineAssembly;
            }
        }

        static TestToolsHelper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolver);
            Assembly.LoadFile(@"C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\PrivateAssemblies\Microsoft.VisualStudio.QualityTools.CommandLine.dll");
        }

        public static Type FindType(String fullName)
        {
            foreach (Type Canidate in TestToolsHelper.CommandLineAssembly.GetTypes())
            {
                if (Canidate.FullName == fullName)
                {
                    return Canidate;
                }
            }
            return null;
        }

        public static object CreateInstance(String fullName)
        {
            return CreateInstance(fullName, null);
        }

        public static object CreateInstance(String fullName, Object[] args)
        {
            foreach (Type Canidate in TestToolsHelper.CommandLineAssembly.GetTypes())
            {
                if (Canidate.FullName == fullName)
                {
                    return Activator.CreateInstance(Canidate, args);
                }
            }
            return null;
        }
            

        private static System.Reflection.Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            char[] chArray2 = new char[1] { ',' };
            char[] chArray1 = chArray2;
            string text1 = args.Name.Split(chArray1)[0];

            StringEnumerator enumerator1 = HintDirectories.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                string text2 = enumerator1.Current;
                string text3 = Path.Combine(text2, string.Format("{0}.dll", text1));
                if (File.Exists(text3))
                {
                    return Assembly.LoadFrom(text3);
                }
                text3 = Path.Combine(text2, string.Format("{0}.exe", text1));
                if (File.Exists(text3))
                {
                    return Assembly.LoadFrom(text3);
                }
            }
            return null;
        }
    }
}
