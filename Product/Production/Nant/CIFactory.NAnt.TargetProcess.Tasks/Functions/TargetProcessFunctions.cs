using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.NAnt.DataTypes;
using CIFactory.TargetProcess.Common.IterationWebService;
using CIFactory.TargetProcess.Common.ProjectWebService;

namespace CIFactory.TargetProcess.NAnt.Functions
{
    [FunctionSet("tp", "target process")]
    public class TargetProcessFunctions : FunctionSetBase
    {
        
        #region Constructors

        public TargetProcessFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        public TargetProcessFunctions()
            : base(null, null)
        {
        }

        #endregion

        private static IterationDTO GetCurrentIteration(int projectId, ConnectionInformation ConnectionInformation)
        {
            ServicesCF.ConnectionInformation = ConnectionInformation;
            IterationService iterationService = ServicesCF.GetService<IterationService>();

            IterationDTO[] iterations = iterationService.RetrieveAll();

            List<IterationDTO> currentIterationList = iterations.Where(iteration => iteration.ParentProjectID == projectId && iteration.StartDate <= DateTime.Today && iteration.EndDate >= DateTime.Today).ToList<IterationDTO>();

            if (currentIterationList.Count == 0)
                throw new BuildException("Could not find the current iteration.");

            IterationDTO currentIteration = currentIterationList[0];
            return currentIteration;
        }
        
        private static int FindProjectId(string projectName)
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "select from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { projectName });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", projectName));

            return projects[0].ProjectID.Value;
        }

        [Function("get-current-iteration-id")]
        public int GetCurrentIterationId(String connectionRef, string projectName)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(FindProjectId(projectName), ConnectionInformation);

            return currentIteration.ID.Value;
        }

        [Function("get-current-iteration-name")]
        public string GetCurrentIterationName(String connectionRef, string projectName)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(FindProjectId(projectName), ConnectionInformation);

            return currentIteration.Name;
        }

        [Function("get-current-iteration-start-date")]
        public DateTime GetCurrentIterationStartDate(String connectionRef, string projectName)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(FindProjectId(projectName), ConnectionInformation);

            return currentIteration.StartDate.Value;
        }

        [Function("get-current-iteration-end-date")]
        public DateTime GetCurrentIterationEndDate(String connectionRef, string projectName)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(FindProjectId(projectName), ConnectionInformation);

            return currentIteration.EndDate.Value;
        }
    }
}
