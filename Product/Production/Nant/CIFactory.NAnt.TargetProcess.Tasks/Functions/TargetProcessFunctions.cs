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

        [Function("get-current-iteration-id")]
        public int GetCurrentIterationId(String connectionRef, int projectId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(projectId, ConnectionInformation);

            return currentIteration.ID.Value;
        }

        [Function("get-current-iteration-name")]
        public string GetCurrentIterationName(String connectionRef, int projectId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(projectId, ConnectionInformation);

            return currentIteration.Name;
        }

        [Function("get-current-iteration-start-date")]
        public DateTime GetCurrentIterationStartDate(String connectionRef, int projectId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(projectId, ConnectionInformation);

            return currentIteration.StartDate.Value;
        }

        [Function("get-current-iteration-end-date")]
        public DateTime GetCurrentIterationEndDate(String connectionRef, int projectId)
        {
            if (!this.Project.DataTypeReferences.Contains(connectionRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", connectionRef));

            ConnectionInformation ConnectionInformation = (ConnectionInformation)this.Project.DataTypeReferences[connectionRef];

            IterationDTO currentIteration = GetCurrentIteration(projectId, ConnectionInformation);

            return currentIteration.EndDate.Value;
        }
    }
}
