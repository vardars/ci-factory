using System;
using System.ServiceModel;

namespace CCNET.TFS.Plugin
{
    [ServiceContract(
        Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03")]
    public interface INotificationReciever
    {
        [XmlSerializerFormat()]
        [OperationContract(
            Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03/Notify")]
        void Notify(string eventXml, string tfsIdentityXml);
    }

}
