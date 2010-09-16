// Changed this class to Singleton to Support SvnRevisionLabeller
namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
        private static volatile Configuration _instance = null;
        private ProjectList projects = new ProjectList();

        public static Configuration Instance()
        {
            if (_instance == null)
            {
                lock (typeof(Configuration))
                {
                    if (_instance == null)
                    {
                        _instance = new Configuration();
                    }
                }
            }
            return _instance;
        }

		public void AddProject(IProject project)
		{
			projects.Add(project);
		}

		public void DeleteProject(string name)
		{
			projects.Delete(name);
		}

		public IProjectList Projects
		{
			get { return projects; }
		}
	}
}