using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CIFactory.NAnt.Tracker.Functions;
using CIFactory.NAnt.Types;

namespace Test.Tracker.Functions
{
    [TestFixture]
    public class TrackerFunctionsTest
    {
        [RowTest]
        [Row("no scr here", "")]
        [Row("12345 no scr here", "")]
        [Row("no scr here 12345", "")]
        [Row("12345", "12345")]
        [Row("12345 ", "12345")]
        [Row(" 12345", "12345")]
        [Row(" 12345 ", "12345")]
        [Row("12345 12346", "12345,12346")]
        [Row("12345 12346 ", "12345,12346")]
        [Row(" 12345 12346", "12345,12346")]
        [Row(" 12345 12346 ", "12345,12346")]
        [Row("12345,12346", "12345,12346")]
        [Row("12345, 12346", "12345,12346")]
        [Row("12345 , 12346", "12345,12346")]
        [Row("12345 ,12346", "12345,12346")]
        [Row("scr12345", "12345")]
        [Row("scr12345, scr 12346", "12345,12346")]
        [Row("scr #12345, scr 12346", "12345,12346")]
        [Row("scr#12345, scr 12346", "12345,12346")]
        [Row("scr# 12345, scr12346", "12345,12346")]
        [Row("#12345, scr 12346", "12345,12346")]
        [Row("#12345, scr 12346", "12345,12346")]
        [Row("# 12345, scr12346", "12345,12346")]
        [Row("# 12345, scr12346,12347 ,23456, I did something.. and look in scr 1 for this and scr 2 for that", "12345,12346")]
        public void ExtractScrNumbersTest(string comment, string expected)
        {
            StringList List = new StringList();
            TrackerFunctions TestSubject = new TrackerFunctions();
            TestSubject.GetScrNumbers(comment, List);
            string[] ExpectedList = expected.Split(',');
            foreach (string Item in List.StringItems.Values)
            {
                Assert.In(Item, ExpectedList);
            }
        }

        [Test]
        public void ExtractScrNumbersMultiLineCommentTest()
        {
            this.ExtractScrNumbersTest(string.Format("scr# 12345{0}scr12346", Environment.NewLine), "12345,12346");
        }

        public void Adhoc()
        {
            this.ExtractScrNumbersTest("scr# 12345, scr12346", "12345,12346");
        }
    }
}
