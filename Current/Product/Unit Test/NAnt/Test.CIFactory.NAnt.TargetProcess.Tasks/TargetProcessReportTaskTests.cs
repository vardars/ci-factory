using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CIFactory.TargetProcess.NAnt.Tasks;
using CIFactory.NAnt.Types;
using Rhino.Mocks;
using System.IO;
using CIFactory.TargetProcess.NAnt.Helpers;

namespace Test.CIFactory.NAnt.TargetProcess.Tasks
{    
    [TestFixture]
    [TestsOn(typeof(TargetProcessReportTask))]
    public class TargetProcessReportTaskTests
    {
        public void IntegrationTest()
        {
            string ReportFilePath = @"C:\Temp\TpReport.xml";
            if (File.Exists(ReportFilePath))
                File.Delete(ReportFilePath);

            TargetProcessReportTask testSubject = new TargetProcessReportTask();

            testSubject.ConnectionInformation.UserName = "flowersj";
            testSubject.ConnectionInformation.Password = "password";
            testSubject.ConnectionInformation.RootServiceUrl = "http://agilex.tpondemand.com";

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
