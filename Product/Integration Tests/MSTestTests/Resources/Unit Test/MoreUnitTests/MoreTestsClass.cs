using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MoreTestsClass
{
    // Methods
    [TestMethod]
    public void Test2()
    {
        MoreTestSubject ClassUnderTest = new MoreTestSubject();
        ClassUnderTest.FunctionToTest1(0);
        ClassUnderTest.FunctionToTest1(1);
        ClassUnderTest.FunctionToTest1(5);
    }
}

 

