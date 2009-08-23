using System;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Win32;

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
                    foreach (Assembly Candidate in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (Candidate.GetName().Name == "Microsoft.VisualStudio.QualityTools.CommandLine")
                        {
                            HintDirectories.Add(Path.GetDirectoryName(Candidate.Location));
                            _CommandLineAssembly = Candidate;
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

            string PrivateAssemblyPath = string.Empty;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0\Setup\VS", false))
            {
                string str2 = key.GetValue("EnvironmentDirectory") as string;
                PrivateAssemblyPath = Path.Combine(str2, "PrivateAssemblies");
            }

            Assembly.LoadFile(String.Format(@"{0}\Microsoft.VisualStudio.QualityTools.CommandLine.dll", PrivateAssemblyPath));
        }

        public static Type FindType(String fullName)
        {
            foreach (Type Candidate in TestToolsHelper.CommandLineAssembly.GetTypes())
            {
                if (Candidate.FullName == fullName)
                {
                    return Candidate;
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
            foreach (Type Candidate in TestToolsHelper.CommandLineAssembly.GetTypes())
            {
                if (Candidate.FullName == fullName)
                {
                    return Activator.CreateInstance(Candidate, args);
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
