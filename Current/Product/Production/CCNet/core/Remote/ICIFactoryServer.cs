using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

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
            UriTemplate = "/ForceBuild2?ProjectName={projectName}&ClientInfo={serializedClientInfo}"
        )]
        bool ForceBuild(string projectName, string serializedClientInfo);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetProject/{projectName}"
        )]
        string GetProject(string projectName);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetProjectProcessInformation/{projectName}"
        )]
        ProcessInformationList GetProjectProcessInformation(string projectName);

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
            UriTemplate = "/GetProjectsStatus"
        )]
        ProjectStatus[] GetProjectsStatus();

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
            UriTemplate = "/GetProjectsStatusLite"
        )]
        ProjectStatus[] GetProjectsStatusLite();

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/GetProjectStatusLite/{projectName}"
        )]
        ProjectStatus GetProjectStatusLite(string projectName);

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
        string[] GetMostRecentBuildNames(string projectName, string buildCount);

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
