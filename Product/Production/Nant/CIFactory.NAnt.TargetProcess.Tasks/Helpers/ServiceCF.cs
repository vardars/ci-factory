using System;
using System.Collections.Generic;
using System.Text;
using CIFactory.TargetProcess.NAnt.DataTypes;
using Microsoft.Web.Services3;
using TargetProcess.Common;

namespace CIFactory.TargetProcess.NAnt.Helpers
{
    public static class ServicesCF
    {
        private static ConnectionInformation _ConnectionInformation;
        public static ConnectionInformation ConnectionInformation
        {
            get { return _ConnectionInformation; }
            set
            {
                _ConnectionInformation = value;
            }
        }

        private static Dictionary<String, Object> Services = new Dictionary<string, object>();

        public static T GetService<T>() where T : WebServicesClientProtocol, new()
        {
            T service;
            if (Services.ContainsKey(typeof(T).Name))
            {
                service = (T) Services[typeof(T).Name];
            }
            else
            {
                service = new T { Url = ConnectionInformation.RootServiceUrl + "/Services/" + typeof(T).Name + ".asmx" };
                TpPolicy.ApplyAutheticationTicket(service, ConnectionInformation.UserName, ConnectionInformation.Password);
            }
            return service;
        }
        
    }
}
