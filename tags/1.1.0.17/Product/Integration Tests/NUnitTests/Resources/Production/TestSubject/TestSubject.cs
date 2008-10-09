using System;


public class TestSubject
{
    // Methods
    public void FunctionToTest1(int value)
    {
        if (value == 1)
        {
            this.WriteString("Tested");
        }
    }

    public void WriteString(string str)
    {
        Console.WriteLine(str);
    }
}


