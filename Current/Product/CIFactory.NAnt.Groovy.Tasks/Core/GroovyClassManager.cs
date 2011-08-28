using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyClassManager
    {
        private java.io.File _groovyHome;
        private java.lang.Class _groovyClassLoaderClass;
        private java.lang.ClassLoader _classLoader;

        public GroovyClassManager(java.io.File groovyHome)
        {
            this._groovyHome = groovyHome;
            this._classLoader = this.createClassLoader();
        }

        private java.lang.ClassLoader createClassLoader()
        {
            java.io.File[] jarFiles = this.getGroovyClasspath();

            java.net.URL[] jarFilesAsURLs = new java.net.URL[jarFiles.Length];
            for (int i = 0; i < jarFiles.Length; i++)
                jarFilesAsURLs[i] = jarFiles[i].toURI().toURL();
            java.net.URLClassLoader urlClassLoader = new java.net.URLClassLoader(jarFilesAsURLs, java.lang.ClassLoader.getSystemClassLoader());

            java.lang.Class groovyClassLoaderClass = java.lang.Class.forName("groovy.lang.GroovyClassLoader", true, urlClassLoader);
            java.lang.reflect.Method addClassPathMethod = groovyClassLoaderClass.getMethod("addClasspath", java.lang.Class.forName("java.lang.String"));
            java.lang.ClassLoader groovyClassLoader = (java.lang.ClassLoader)groovyClassLoaderClass.newInstance();
            foreach (java.io.File file in jarFiles)
                addClassPathMethod.invoke(groovyClassLoader, file.getAbsolutePath());
            this._groovyClassLoaderClass = groovyClassLoaderClass;
          
            return groovyClassLoader;
        }

        private java.io.File[] getGroovyClasspath()
        {
            LinkedList<java.io.File> retval = new LinkedList<java.io.File>();

            java.io.File libdir = new java.io.File(this._groovyHome.getAbsolutePath() + "/" + "lib");
            foreach (java.io.File file in libdir.listFiles())
                if (file.getName().EndsWith(".jar"))
                    retval.AddFirst(file);

            return retval.ToArray<java.io.File>();
        }

        public java.lang.Class parseClass(java.io.File file)
        {
            java.lang.Class[] args = new java.lang.Class[] { java.lang.Class.forName("java.io.File") };

            java.lang.reflect.Method parseClass = this._groovyClassLoaderClass.getMethod("parseClass", args);

            return (java.lang.Class)parseClass.invoke(this._classLoader, file);
        }

        public java.lang.Class parseClass(string text)
        {
            java.lang.Class[] args = new java.lang.Class[] { java.lang.Class.forName("java.lang.String") };

            java.lang.reflect.Method parseClass = this._groovyClassLoaderClass.getMethod("parseClass", args);

            return (java.lang.Class)parseClass.invoke(this._classLoader, text);
        }
    }
}
