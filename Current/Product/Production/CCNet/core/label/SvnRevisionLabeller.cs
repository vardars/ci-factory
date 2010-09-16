using System;
using System.Text.RegularExpressions;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
//using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// This labeller uses the Subversion Revision number to create a label. This labeller will only work if the source control
    /// is set to type svn in configuration file.
    /// </summary>
    /// <title>SVN Revision Labeller</title>
    /// <version>1.0</version>
    
    [ReflectorType("svnRevisionLabeller")]
    public class SvnRevisionLabeller : ILabeller
    {
        /// <summary>
        /// The string to be prepended onto the current revision number.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("prefix", Required = false)]
        public string LabelPrefix = string.Empty;

        /// <summary>
        /// Controls whether duplicate subsequent labels are permitted or not. If true, duplicate labels are left
        /// intact. If false, the label will always be suffixed with ".n", where "n" is incremented for each
        /// successive duplication. Defaults to true.
        /// </summary>
        /// <version>1.3</version>
        /// <default>true</default>
        //[ReflectorProperty("allowDuplicateSubsequentLabels", Required = false)]
        //public bool AllowDuplicateSubsequentLabels = true;

        /// <summary>
        /// Generate a label string from the current svn revision number.
        /// </summary>
        /// <param name="resultFromThisBuild">IntegrationResult object for the current build</param>
        /// <returns>the new label</returns>
        public string Generate(IIntegrationResult resultFromThisBuild)
        {
            Configuration config = Configuration.Instance();
            
            foreach (Project p in config.Projects)
            {
                if (resultFromThisBuild.ToString().Contains(p.Name))
                {
                    Svn svn = p.SourceControl as Svn;
                    ProcessResult result = svn.GetInfo(resultFromThisBuild);
                    XmlDocument info = new XmlDocument();
                    info.LoadXml(result.StandardOutput);
                    XmlNodeList xmlNL = info.GetElementsByTagName("commit");
                    return LabelPrefix + xmlNL[0].Attributes[0].Value;                    
                }
            }
            return string.Empty;
        }

        public void Run(IIntegrationResult result)
        {
            result.Label = Generate(result);
        }
    }
}
