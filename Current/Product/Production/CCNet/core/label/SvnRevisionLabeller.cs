using System;
using System.Text.RegularExpressions;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Remote;
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
        public string Generate(IIntegrationResult currentResult, IIntegrationResult resultLastBuild)
        {
            string label = LabelPrefix + "UNKNOWN";
            if (resultLastBuild.LastSuccessfulIntegrationLabel != "UNKNOWN")
            {
                label = resultLastBuild.LastSuccessfulIntegrationLabel;
            }

            Configuration config = Configuration.Instance();
            
            foreach (Project p in config.Projects)
            {
                if (currentResult.ToString().Contains(p.Name))
                {
                    int greatest = 0;
                    foreach (Modification modification in p.SourceControl.GetModifications(resultLastBuild, currentResult))
                    {
                        if (modification.ChangeNumber > greatest)
                            greatest = modification.ChangeNumber;
                    }
                    if (greatest > 0)
                        label = LabelPrefix + greatest.ToString();
                    break;
                }
            }

            if (label == resultLastBuild.LastSuccessfulIntegrationLabel)
            {
                label = this.IncrementLabel(label);
            }

            return label;
        }

        private string IncrementLabel(string label)
        {
            int current = 0;
            Match match = Regex.Match(label, @"^(" + LabelPrefix + @"\d+)\.(\d+)$");
            if (match.Success)
            {
                current = Int32.Parse(match.Groups[2].Value);
                label = match.Groups[1].Value;
            }
            return String.Format("{0}.{1}", label, current + 1);
        }

        public void Run(IIntegrationResult result)
        {
            result.Label = Generate(result, result.PreviousIntegrationResult);
        }
    }
}
