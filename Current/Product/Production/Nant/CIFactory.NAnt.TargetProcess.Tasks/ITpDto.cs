using System;
using System.Collections.Generic;
using System.Text;

namespace CIFactory.TargetProcess.NAnt
{
    public interface ITpDto
    {
        string Name { get;set;}
        string Description { get;set;}
        System.Nullable<int> ID { get;set;}
        string EntityTypeName { get;set;}
        string ProjectName { get;set;}
    }
}
