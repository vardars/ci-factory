using System;
using System.Collections.Generic;
using System.Text;

namespace CIFactory.TargetProcess.NAnt.Helpers
{
    public interface ITargetProcessHelper
    {
        string RootWebServiceUrl { get;set;}
        Entity RetrieveEntity(int id, string entityType);
        string UserName { get;set;}
        string Password { get;set;}
    }
}
