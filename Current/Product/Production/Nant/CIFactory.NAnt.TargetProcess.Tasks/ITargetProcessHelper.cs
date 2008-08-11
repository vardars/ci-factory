using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

namespace CIFactory.TargetProcess.NAnt.Tasks
{
    public interface ITargetProcessHelper
    {
        string RootWebServiceUrl { get;set;}
        Entity RetrieveEntity(int id, string entityType);
        string UserName { get;set;}
        string Password { get;set;}
    }
}
