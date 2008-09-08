using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CIFactory.TargetProcess.NAnt.Tasks;
using CIFactory.NAnt.Types;
using Rhino.Mocks;
using System.IO;

namespace Test.CIFactory.NAnt.TargetProcess.Tasks
{
    
    [TestFixture]
    [TestsOn(typeof(TargetProcessReportTask))]
    public class TargetProcessReportTaskTests
    {

        [Test]
        public void TaskTest()
        {
            string ReportFilePath = @"C:\Temp\TpReport.xml";
            if (File.Exists(ReportFilePath))
                File.Delete(ReportFilePath);

            TargetProcessReportTask testSubject = new TargetProcessReportTask();

            testSubject.UserName = "maria";
            testSubject.Password = "boo";
            testSubject.RootServiceUrl = "http://agilex.tpondemand.com";

            testSubject.TaskIds = new StringList("1", "2", "3");

            testSubject.ReportFilePath = ReportFilePath;

            MockRepository mocks = new MockRepository();
            ITargetProcessHelper helpermock = mocks.StrictMock<ITargetProcessHelper>();
            testSubject.Helper = helpermock;

            Expect.Call(helpermock.UserName).PropertyBehavior();
            Expect.Call(helpermock.Password).PropertyBehavior();
            Expect.Call(helpermock.RootWebServiceUrl).PropertyBehavior();
            Expect.Call(
                helpermock.RetrieveEntity(1, "Task")
                ).Return(new Entity("description", "link", "name", "type", 1));
            Expect.Call(
                helpermock.RetrieveEntity(2, "Task")
                ).Return(new Entity("description2", "link2", "name2", "type2", 2));
            Expect.Call(
                helpermock.RetrieveEntity(3, "Task")
                ).Return(new Entity("description3", "link3", "name3", "type3", 3));
            mocks.ReplayAll();

            testSubject.GenerateReport();

            mocks.VerifyAll();
            FileAssert.Exists(ReportFilePath);
            
            string xml = File.ReadAllText(ReportFilePath);
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@Name", xml, "namename2name3");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@Id", xml, "123");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@Type", xml, "typetype2type3");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@HyperLink", xml, "http://agilex.tpondemand.comlinkhttp://agilex.tpondemand.comlink2http://agilex.tpondemand.comlink3");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity", xml, "descriptiondescription2description3");
        }

        
        public void IntegrationTest()
        {
            string ReportFilePath = @"C:\Temp\TpReport.xml";
            if (File.Exists(ReportFilePath))
                File.Delete(ReportFilePath);

            TargetProcessReportTask testSubject = new TargetProcessReportTask();

            testSubject.UserName = "flowersj";
            testSubject.Password = "password";
            testSubject.RootServiceUrl = "http://agilex.tpondemand.com";

            testSubject.TaskIds = new StringList("5166");
            testSubject.StoryIds = new StringList("5127", "5125", "5084");
            testSubject.BugIds = new StringList("4246", "3978");

            testSubject.ReportFilePath = ReportFilePath;

            testSubject.GenerateReport();

            FileAssert.Exists(ReportFilePath);

            string xml = File.ReadAllText(ReportFilePath);
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@Name", xml, "name");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@Type", xml, "type");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity/@HyperLink", xml, "link");
            XmlAssert.XPathEvaluatesTo(@"/TargetProcess/Entity", xml, "description");
        }
    }
}
