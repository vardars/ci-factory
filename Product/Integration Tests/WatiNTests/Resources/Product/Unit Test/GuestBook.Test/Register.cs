using System;
using System.IO;
using System.Threading;
using WatiN.Core;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Exceptions;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using MbUnit.Framework;

namespace TestSpace
{
    [TestFixture(ApartmentState = ApartmentState.STA)]
    public class TestClass
    {

        private IE _Ie;
        public IE Ie
        {
            get
            {
                return _Ie;
            }
            set
            {
                _Ie = value;
            }
        }

        [Test]
        public void Register()
        {
            Ie = new IE("about:blank");
            Ie.GoTo("http://localhost/GuestBook/GuestBook.aspx");
            Ie.TextField(Find.ByName("name")).TypeText("Jay");
            Ie.TextField(Find.ByName("comments")).TypeText("Hello");
            Ie.Button(Find.ByName("save")).Click();

            Assert.AreEqual("Jay", Ie.TableCell(Find.By("innerText", "Jay")).Text, @"innerText does not match");
            Assert.AreEqual("Hello", Ie.TableCell(Find.By("innerText", "Hello")).Text, @"innerText does not match");
        }

        [TearDown]
        public void TearDown()
        {
            if (Ie != null)
                Ie.Close();
        }
    }
}

