using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Macrodef
{
    /// <summary>
    /// Contains the template for the macro - the tasks that should be executed when the macro is called.
    /// </summary>
    [ElementName("sequential")]
    public class MacroDefSequential : TaskContainer
    {
        #region Properties

        internal XmlNode SequentialXml
        {
            get { return XmlNode; }
        }

        protected override void ExecuteChildTasks()
        {
            
        }
        protected override void ExecuteTask()
        {
            
        }

        #endregion

    }
}
