using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CIFactory.TargetProcess.NAnt.Tasks;
using CIFactory.NAnt.Types;
using Rhino.Mocks;
using System.IO;
using CIFactory.TargetProcess.NAnt;

namespace Test.CIFactory.NAnt.TargetProcess.Tasks
{
    [TestFixture]
    public class TargetProcessExtractFromCommentTests
    {
        [RowTest]
        [Row("no task here", "")]
        [Row("12345 no task here", "")]
        [Row("no task here 12345", "")]
        [Row("12345", "")]
        [Row("12345 ", "")]
        [Row(" 12345", "")]
        [Row(" 12345 ", "")]
        [Row("12345 12346", "")]
        [Row("12345 12346 ", "")]
        [Row(" 12345 12346", "")]
        [Row(" 12345 12346 ", "")]
        [Row("12345,12346", "")]
        [Row("12345, 12346", "")]
        [Row("12345 , 12346", "")]
        [Row("12345 ,12346", "")]
        [Row("task12345", "12345")]
        [Row("Task12345", "12345")]
        [Row("task12345, task 12346", "12345,12346")]
        [Row("task #12345, task 12346", "12345,12346")]
        [Row("task#12345, task 12346", "12345,12346")]
        [Row("task# 12345, task12346", "12345,12346")]
        [Row("#12345, task 12346", "12346")]
        [Row("#12345, task 12346", "12346")]
        [Row("# 12345, task12346", "12346")]
        [Row("# 12345, task12346,12347 ,23456, I did something.. and look in task 1 for this and task 2 for that", "12346")]
        public void ExtractScrNumbersTest(string comment, string expected)
        {
            StringList List = new StringList();
            TargetProcessExtractFromComment TestSubject = new TargetProcessExtractFromComment();
            TestSubject.EntityPrefix = "task";
            TestSubject.GetEntityNumbers(comment, List);
            string[] ExpectedList = new string[] { };
            if (!string.IsNullOrEmpty(expected))
                ExpectedList = expected.Split(',');
            foreach (string Item in List.StringItems.Values)
            {
                Assert.In(Item, ExpectedList, String.Format("Should not have found: '{0}' in '{1}'.", Item, comment));
            }
            foreach (string Item in ExpectedList)
            {
                Assert.In(Item, List.StringItems.Values, String.Format("Should not have found: '{0}' in '{1}'.", Item, comment));
            }
        }

        [Test]
        public void ExtractScrNumbersMultiLineCommentTest()
        {
            this.ExtractScrNumbersTest(string.Format("task# 12345{0}task12346", Environment.NewLine), "12345,12346");
        }

        public void Adhoc()
        {
            this.ExtractScrNumbersTest("task# 12345, task12346", "12345,12346");
        }
    }
}
