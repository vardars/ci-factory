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
    [TestsOn(typeof(TargetProcessCreateTaskTask))]
    public class TargetProcessCreateEntityTaskTests
    {
        public void AdhocTest()
        {
            TargetProcessCreateTaskTask testSubject = new TargetProcessCreateTaskTask();

            testSubject.ConnectionInformation.UserName = "flowersj";
            testSubject.ConnectionInformation.Password = "password";
            testSubject.ConnectionInformation.RootServiceUrl = "http://agilex.tpondemand.com";

            ServicesCF.ConnectionInformation = testSubject.ConnectionInformation;

            //TargetProcessTask task = new TargetProcessTask();
            //testSubject.TaskEntity = task;
            //task.EntityName = "Testing Creation";
            //task.Description = "This is a test creating a task from NAnt.";
            //task.UserStory = "EF Reference System Preparation";
            //task.TargetProcessProject = "EF Support";

            //testSubject.CreateEntity();

            //int id = task.TaskId;

            //task = new TargetProcessTask();
            //task.TaskId = id;
            //task.EntityName = "Testing Updating";
            //task.Description = "This is a test updating a task from NAnt.";
            //task.UserStory = "EF Reference System Preparation";
            //task.TargetProcessProject = "EF Support";
            //task.State = "Done";

            //task.Update();

            TargetProcessUserStory story = new TargetProcessUserStory();
            story.EntityName = "SUPPORT: Build Improvements (Split3)";
            story.TargetProcessProject = "NHIE-Gateway";
            story.State = "Done";
            story.Update();
        }
    }
}
