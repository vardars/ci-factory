using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	[ReflectorType("configurablePlugin")]
	public class ConfigurablePlugin : IPlugin
	{
		private string description;
		private INamedAction[] namedActions;

		[ReflectorProperty("description")]
		public string LinkDescription
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

		[ReflectorArray("actions", Required=true)]
		public INamedAction[] NamedActions
		{
			get
			{
				return namedActions;
			}
			set
			{
				namedActions = value;
			}
		}

        #region IPlugin Members


        string IPlugin.ImageFileName
        {
            get
            {
                throw new System.Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new System.Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
