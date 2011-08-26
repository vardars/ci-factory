using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    [XmlRoot(Namespace="", IsNullable=false, ElementName="ProjectStatus")]
    class ProjectStatus
    {
        [XmlElement ("_Forcee")]
        public string Forcee { get; set; }

        [XmlElement("_Modifications")]
        public string Modifications { get; set; }

        [XmlElement("activity")]
        public string Activity { get; set; }

        [XmlElement("buildCondition")]
        public string BuildCondition { get; set; }

        [XmlElement("buildStatus")]
        public string BuildStatus { get; set; }

        [XmlElement("currentBuildStartTime")]
        public string CurrentBuildStartTime { get; set; }

        [XmlElement("lastBuildDate")]
        public string LastBuildDate { get; set; }

        [XmlElement("lastBuildDuration")]
        public string LastBuildDuration { get; set; }

        [XmlElement("lastBuildLabel")]
        public string LastBuildLabel { get; set; }

        [XmlElement("lastSuccessfulBuildLabel")]
        public string LastSuccessfulBuildLabel { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("nextBuildTime")]
        public string NextBuildTime { get; set; }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("webURL")]
        public string WebURL { get; set; }


    }
}
