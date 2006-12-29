using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
    [ReflectorType("artifactXslBuildPlugin")]
    public class ArtifactXslBuildPlugin : ProjectConfigurableBuildPlugin
    {
        private readonly IActionInstantiator actionInstantiator;
        private string xslFileName = "";
        private string description = "no description set";
        private string actionName = "NoActionSet";
        private string _XmlFileName;
        private string _ArtifactRootUrl;

        public ArtifactXslBuildPlugin(IActionInstantiator actionInstantiator)
        {
            this.actionInstantiator = actionInstantiator;
        }

        // These 2 are separate due to inheritence / property monkey-ness
        [ReflectorProperty("description")]
        public string ConfiguredLinkDescription
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        // See note on ConfiguredLinkDescription
        public override string LinkDescription
        {
            get
            {
                return description;
            }
        }

        [ReflectorProperty("actionName")]
        public string ActionName
        {
            get
            {
                return actionName;
            }
            set
            {
                actionName = value;
            }
        }

        [ReflectorProperty("xslFileName")]
        public string XslFileName
        {
            get
            {
                return xslFileName;
            }
            set
            {
                xslFileName = value;
            }
        }

        [ReflectorProperty("artifactRootUrl")]
        public string ArtifactRootUrl
        {
            get
            {
                return _ArtifactRootUrl;
            }
            set
            {
                _ArtifactRootUrl = value;
            }
        }

        [ReflectorProperty("xmlFileName")]
        public string XmlFileName
        {
            get
            {
                return _XmlFileName;
            }
            set
            {
                _XmlFileName = value;
            }
        }

        public override INamedAction[] NamedActions
        {
            get
            {
                ArtifactXslReportBuildAction action = (ArtifactXslReportBuildAction)actionInstantiator.InstantiateAction(typeof(ArtifactXslReportBuildAction));
                action.XslFileName = XslFileName;
                action.XmlFileName = XmlFileName;
                action.ArtifactRootUrl = ArtifactRootUrl;
                return new INamedAction[] { new ImmutableNamedAction(actionName, action) };
            }
        }
    }
}
