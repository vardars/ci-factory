using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyUtil
    {
        public static void runScript(java.lang.Class clazz)
        {
            java.lang.reflect.Method runMethod = clazz.getMethod("run", new java.lang.Class[] { });
            Object obj = clazz.newInstance();
            runMethod.invoke(obj, new object[]{});
        }


        // This is pretty ugly. The java reflection API doesn't give us a way to extract method argument
        // names, so we do some string parsing on the source file to get this information
        public static string[] extractTaskArgumentNames(string methodName, string source)
        {
            LinkedList<string> retval = new LinkedList<string>();

            string regex = "@task.*?" + methodName + ".*?\\((?<parameters>.*?)\\)";
            MatchCollection matches = Regex.Matches(source, regex, RegexOptions.Multiline | RegexOptions.Singleline);

            if (matches.Count > 1)
                throw new System.Exception("Found more than one method with name " + methodName + " in source file");

            foreach (Match match in matches)
            {
                string parameterList = match.Groups[1].Captures[0].Value;
                string[] parameters = Regex.Split(parameterList, "\\s*,\\s*");

                foreach (string str in parameters)
                {
                    string[] candidates = Regex.Split(str, "\\s+");
                    for (int i = candidates.Length - 1; i >= 0; i--)
                    {
                        if (new Regex("\\s+|^$").IsMatch(candidates[i]))
                            continue;
                        else
                        {
                            retval.AddLast(candidates[i]);
                            break;
                        }
                    }
                }
            }

            return retval.ToArray<string>();
        }


        public static java.lang.reflect.Method[] getMethodsAnnotatedWith(java.lang.Class clazz, string annotation)
        {
            LinkedList<java.lang.reflect.Method> retval = new LinkedList<java.lang.reflect.Method>();

            // @todo: fix this hack up. We should be matching the name of the annotation, not the
            // .toString() artifact
            foreach (java.lang.reflect.Method method in clazz.getDeclaredMethods())
                foreach(java.lang.annotation.Annotation ann in method.getDeclaredAnnotations())
                    if (ann.toString().Equals('@' + annotation + "()"))
                        retval.AddFirst(method);

            return retval.ToArray<java.lang.reflect.Method>();
        }
    }
}
