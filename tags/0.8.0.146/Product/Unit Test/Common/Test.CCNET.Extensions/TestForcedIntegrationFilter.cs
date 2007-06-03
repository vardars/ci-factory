using System;
using System.IO;
using System.Xml;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.Triggers;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using ThoughtWorks.CruiseControl.Remote.TestDoubles;
using TestDoubles;
using ThoughtWorks.CruiseControl.Core.IntegrationFilters;
using Tracker.CCNET.Plugin.IntegrationFilters;

public class TestForcedIntegrationFilter
{
    public void TestSerialization()
    {
        ForcedIntegrationFilter TestSubject = new ForcedIntegrationFilter();

        TrackerRequired ScrReq = new TrackerRequired();

        ScrReq.Condition = true;

        ScrReq.ConnectionInformation = new ConnectionInformation();
        ScrReq.ConnectionInformation.dbmsLoginMode = 2;
        ScrReq.ConnectionInformation.dbmsPassword = "p";
        ScrReq.ConnectionInformation.dbmsServer = "s";
        ScrReq.ConnectionInformation.dbmsType = "t";
        ScrReq.ConnectionInformation.dbmsUserName = "n";
        ScrReq.ConnectionInformation.Password = "p";
        ScrReq.ConnectionInformation.ProjectName = "n";
        ScrReq.ConnectionInformation.UserName = "n";

        ScrReq.QueryInforation = new Query();
        ScrReq.QueryInforation.Name = "q";

        ScrReq.WithModifications = true;

        TestSubject.Blocked = new IIntegrationFilter[1] { ScrReq };

        string Serialized = Serialize("forcedIntegrationFilter", TestSubject);
        System.Diagnostics.Debug.WriteLine(Serialized);

        Assert.IsNotNull(Serialized);

        ForcedIntegrationFilter Clone = (ForcedIntegrationFilter)Deserialize(Serialized);

        Assert.IsNotNull(Clone.Blocked);
        Assert.AreEqual(1, Clone.Blocked.Length);
        Assert.IsNotNull(Clone.Blocked[0]);
    }

    public static string Serialize(string reflectorType, object subject)
    {
        StringWriter buffer = new StringWriter();
        new ReflectorTypeAttribute(reflectorType).Write(new XmlTextWriter(buffer), subject);
        return buffer.ToString();
    }

    public static object Deserialize(string serialized)
    {
        object Subject = NetReflector.Read(serialized);
        return Subject;
    }
}
