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
            Log.Info(string.Format("{0}:Begin", System.Reflection.MethodBase.GetCurrentMethod().Name));
            string label = LabelPrefix + "UNKNOWN";
            Log.Info(string.Format("{0}:label={1}", System.Reflection.MethodBase.GetCurrentMethod().Name,label));
            //if (resultLastBuild.LastIntegrationLabel != "UNKNOWN")
            //{
            //    label = resultLastBuild.LastIntegrationLabel;
            //}

            if (resultLastBuild.Label != "UNKNOWN")
            {
                label = resultLastBuild.Label;
            }

            Configuration config = Configuration.Instance();
            
            Log.Info(string.Format("{0}:Enumerating projects (Begin)", System.Reflection.MethodBase.GetCurrentMethod().Name));
            Log.Info(string.Format("{0}:Looking for currentResult='{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, currentResult.ToString()));
            

            

            foreach (Project p in config.Projects)
            {
                Log.Info(string.Format("{0}:Examining project '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.Name));

                if (currentResult.ToString().Contains(p.Name))
                {

                    if (p.SourceControl != null)
                    {
                        Log.Info(string.Format("{0}:p.SourceControl.GetType().Name='{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.SourceControl.GetType().Name));
                        Log.Info(string.Format("{0}:(FilteredSourceControl)(p.SourceControl)).SourceControlProvider.GetType().Name='{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.SourceControl.GetType().Name));
                    }
                    else
                    {
                        Log.Warning(string.Format("{0}:Project '{1}' SourceControl is null ", System.Reflection.MethodBase.GetCurrentMethod().Name, p.Name));
                    }

                    if ((p.SourceControl != null
                            && p.SourceControl.GetType().Name.ToLower().Equals("svnqueue"))
                            ||
                            (p.SourceControl != null
                            && p.SourceControl.GetType().Name.ToLower().Equals("filteredsourcecontrol") &&
                            ((FilteredSourceControl)(p.SourceControl)).SourceControlProvider.GetType().Name.ToLower().Equals("svnqueue")))
                    {
                        Log.Info(string.Format("{0}:p.SourceControl.GetType().Name.ToLower().Equals('svnqueue') for project '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.Name));

                        FileSvnQueue fsq = new FileSvnQueue(p);

                        Log.Info(string.Format("{0}:fsq '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, fsq.ToString()));
                        Log.Info(string.Format("{0}:fsq.ReadLastSvnRevision() = '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, fsq.ReadLastSvnRevision()));
                        label = LabelPrefix + fsq.ReadLastSvnRevision();
                        Log.Info(string.Format("{0}:label = '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, label));
                    }
                    else
                    {
                        int greatest = 0;
                        Log.Info(string.Format("{0}:p.SourceControl.GetModifications(resultLastBuild, currentResult) = '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.SourceControl.GetModifications(resultLastBuild, currentResult).Length.ToString()));

                        foreach (Modification modification in p.SourceControl.GetModifications(resultLastBuild, currentResult))
                        {
                            Log.Info(string.Format("{0}:modification-->{1},{2}", System.Reflection.MethodBase.GetCurrentMethod().Name, modification.ChangeNumber, modification.UserName));
                            if (modification.ChangeNumber > greatest)
                                greatest = modification.ChangeNumber;
                        }
                        if (greatest > 0)
                        {
                            label = LabelPrefix + greatest.ToString();
                        }
                    }
                    break;
                }
                else
                {
                    Log.Info(string.Format("{0}:currentResult.ToString().Contains(p.Name) for project '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, p.Name));
                }
            }

            Log.Info(string.Format("{0}:Enumerating projects (End)", System.Reflection.MethodBase.GetCurrentMethod().Name));
            Log.Info(string.Format("{0}:label = '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, label));

            if (label == resultLastBuild.LastSuccessfulIntegrationLabel)
            {
                label = this.IncrementLabel(label);
            }

            Log.Info(string.Format("{0}:label = '{1}' (after final if)", System.Reflection.MethodBase.GetCurrentMethod().Name, label));

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
