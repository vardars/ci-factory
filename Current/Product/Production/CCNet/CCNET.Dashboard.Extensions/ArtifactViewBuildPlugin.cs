using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;
using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{

    [ReflectorType("artifactViewBuildPlugin")]
    public class ArtifactViewBuildPlugin : ProjectConfigurableBuildPlugin
    {
        private readonly IActionInstantiator actionInstantiator;
        private string description = "no description set";
        private string actionName = "NoActionSet";
        private string _ArtifactRootUrl;

        public ArtifactViewBuildPlugin(IActionInstantiator actionInstantiator)
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

        public override INamedAction[] NamedActions
        {
            get
            {
                ArtifactViewBuildAction action = (ArtifactViewBuildAction)actionInstantiator.InstantiateAction(typeof(ArtifactViewBuildAction));
                action.ArtifactRootUrl = ArtifactRootUrl;
                return new INamedAction[] { new ImmutableNamedAction(actionName, action) };
            }
        }
    }

}
