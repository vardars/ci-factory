using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using Tracker;
using Tracker.Common;

namespace Test.Tracker
{
    [TestFixture]
    public class tServerHelper
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void tCheckStatus()
        {
            ServerHelper Helper = new ServerHelper();
            Helper.CheckStatus("This is a test of the emergency broadcasting system!", 40);
        }
    }
}
