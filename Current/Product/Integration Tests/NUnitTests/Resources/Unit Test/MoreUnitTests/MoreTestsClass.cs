using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

[TestFixture]
public class MoreTestsClass
{
    // Methods
    [Test]
    public void Test2()
    {
        MoreTestSubject ClassUnderTest = new MoreTestSubject();
        ClassUnderTest.FunctionToTest1(0);
        ClassUnderTest.FunctionToTest1(1);
        ClassUnderTest.FunctionToTest1(5);
    }
}

 

