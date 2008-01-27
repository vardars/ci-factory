using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestClass
{
    // Methods
    [TestMethod]
    public void Test1()
    {
        TestSubject ClassUnderTest = new TestSubject();
        ClassUnderTest.FunctionToTest1(0);
        ClassUnderTest.FunctionToTest1(1);
        ClassUnderTest.FunctionToTest1(5);
    }
}

 

