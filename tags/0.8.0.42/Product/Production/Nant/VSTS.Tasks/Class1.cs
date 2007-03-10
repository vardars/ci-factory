using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace VSTS.Tasks
{
    public class Class1
    {

        public void Attempt()
        {
            Executor TestExecutor = new Executor();
            
            TestContainerCommand TestContainer = new TestContainerCommand(@"C:\Projects\dod.ahlta\Current\Product\Unit Test\BusinessLayer\BusinessEntities.Test\bin\Dod.CHCSII.BusinessLayer.BusinessEntities.Test.dll");
            TestExecutor.Add(TestContainer);

            ResultsOutputCommand ResultsFile = new ResultsOutputCommand(@"C:\Projects\dod.ahlta\Current\Product\report.xml");
            TestExecutor.Add(ResultsFile);

            TestExecutor.ValidateCommands();
            TestExecutor.Execute();
        }
    }
}
