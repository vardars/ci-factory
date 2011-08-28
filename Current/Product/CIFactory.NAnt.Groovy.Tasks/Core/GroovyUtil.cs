using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
