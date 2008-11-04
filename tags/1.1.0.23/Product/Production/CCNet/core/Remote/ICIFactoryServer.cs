using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ThoughtWorks.CruiseControl.Remote
{
    [ServiceContract]
    public interface ICIFactoryServer
    {

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetHostServerName/{projectName}"
        )]
        string GetHostServerName(string projectName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetProjectStatus"
        )]
        ProjectStatus[] GetProjectStatus();

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetProjectStatus/{projectName}"
        )]
        ProjectStatus GetProjectStatus(string projectName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetLatestBuildName/{projectName}"
        )]
        string GetLatestBuildName(string projectName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetBuildNames/{projectName}"
        )]
        string[] GetBuildNames(string projectName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetMostRecentBuildNames/{projectName}/{buildCount}"
        )]
        string[] GetMostRecentBuildNames(string projectName, int buildCount);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetLog/{projectName}/{buildName}"
        )]
        string GetLog(string projectName, string buildName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetVersion"
        )]
        string GetVersion();

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetExternalLinks/{projectName}"
        )]
        ExternalLink[] GetExternalLinks(string projectName);
    }
}
