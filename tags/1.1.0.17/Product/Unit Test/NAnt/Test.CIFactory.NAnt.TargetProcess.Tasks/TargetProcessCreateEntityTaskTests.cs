using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CIFactory.TargetProcess.NAnt.Tasks;
using CIFactory.NAnt.Types;
using Rhino.Mocks;
using System.IO;
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.NAnt.DataTypes;

namespace Test.CIFactory.NAnt.TargetProcess.Tasks
{
    [TestFixture]
    [TestsOn(typeof(TargetProcessCreateEntityTask))]
    public class TargetProcessCreateEntityTaskTests
    {
        public void AdhocTest()
        {
            TargetProcessCreateEntityTask testSubject = new TargetProcessCreateEntityTask();

            testSubject.ConnectionInformation.UserName = "flowersj";
            testSubject.ConnectionInformation.Password = "password";
            testSubject.ConnectionInformation.RootServiceUrl = "http://agilex.tpondemand.com";

            TargetProcessTask task = new TargetProcessTask();
            testSubject.Entity = task;
            task.EntityName = "Testing Creation";
            task.Description = "This is a test creating a task from NAnt.";
            task.UserStory = "EF Reference System Preparation";
            task.TargetProcessProject = "EF Support";
            TargetProcessUser user = new TargetProcessUser("flowersj");
            task.UsersToAssign = new TargetProcessUser[] { user };

            testSubject.CreateEntity();
        }
    }
}
