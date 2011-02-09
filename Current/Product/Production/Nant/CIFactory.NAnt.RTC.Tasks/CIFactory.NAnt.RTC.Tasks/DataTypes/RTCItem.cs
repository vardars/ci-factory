using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.RTC.NAnt.DataTypes
{
    [Serializable, ElementName("rtcitem")]
    class RTCItem : DataTypeBase
    {
        #region Fields

        private string _RTCQueryResult;

        private string _Description;

        private string _EntityId;

        private string _HyperLink;

        private string _Name;

        private string _EntityType;
                
        private string _State;

        private string _CurlExe;

        private string _Cookies;
                
        #endregion

        #region Properties

        public string RTCQueryResult
        {
            get 
            {
                //if (_RTCQueryResult == null) _RTCQueryResult = new XmlDocument();
                //return _RTCQueryResult.OuterXml;
                return _RTCQueryResult;
            }
            set 
            {
                //if (_RTCQueryResult == null) _RTCQueryResult = new XmlDocument();
                _RTCQueryResult = value;
                SetFieldValues();
            }
        }
        
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        
        public string EntityId
        {
            get { return _EntityId; }
            set { _EntityId = value; }
        }

        public string EntityName
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string EntityType
        {
            get { return _EntityType; }
            set { _EntityType = value; }
        }
                        
        public string HyperLink
        {
            get { return _HyperLink; }
            set { _HyperLink = value; }
        }
        
        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        public string Cookies
        {
            get { return this._Cookies; }
            set { this._Cookies = value; }
        }

        public string CurlExe
        {
            get { return this._CurlExe; }
            set { this._CurlExe = value; }
        }
        
        #endregion

        #region Public Methods

        public XmlDocument GenerateReport()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(document.CreateElement("Entity").OuterXml);
            XmlElement rootElement = document.DocumentElement;

            XmlAttribute nameAttribute = document.CreateAttribute("Name");
            nameAttribute.InnerText = Regex.Replace(EntityName, "ΓÇô", "-");
            rootElement.Attributes.Append(nameAttribute);

            XmlAttribute stateAttribute = document.CreateAttribute("State");
            stateAttribute.InnerText = (State == null ? string.Empty : State);
            rootElement.Attributes.Append(stateAttribute);

            XmlAttribute linkAttribute = document.CreateAttribute("HyperLink");
            linkAttribute.InnerText = HyperLink;
            rootElement.Attributes.Append(linkAttribute);

            XmlAttribute typeAttribute = document.CreateAttribute("Type");
            typeAttribute.InnerText = (EntityType == null ? string.Empty : EntityType);
            rootElement.Attributes.Append(typeAttribute);

            XmlAttribute idAttribute = document.CreateAttribute("Id");
            idAttribute.InnerText = EntityId;
            rootElement.Attributes.Append(idAttribute);

            XmlNode descriptionNode = document.CreateNode(XmlNodeType.Element, null, "Description", null);

            string entityDescription = String.Empty;
            if (Description != null)
            {
                entityDescription = Description;

                entityDescription = Regex.Replace(entityDescription, @"(?'pre'\</{0,})\w+\:(?'name'\w+)", "${pre}${name}");
                entityDescription = Regex.Replace(entityDescription, @"\&nbsp\;", @"&#0160;");
                entityDescription = Regex.Replace(entityDescription, @"namespaceuri\="".*""", "");
                entityDescription = Regex.Replace(entityDescription, "ΓÇô", "-");
            }

            try
            {
                descriptionNode.InnerXml = entityDescription;
            }
            catch (Exception ex)
            {
                Log(Level.Error, String.Format("The description for {0} # {1} is not well formed xml: {2}.", this.EntityType, this.EntityId, ex.Message));
                descriptionNode.InnerText = entityDescription;
            }

            rootElement.AppendChild(descriptionNode);

            return document;
        }

        #endregion

        #region Private Methods

        private void SetFieldValues()
        {
            try
            {
                EntityName = ExtractFieldValue("dc:title");
                if (EntityName != null)
                {
                    State = ResolveRdfResourceToValue("rtc_cm:state");
                    EntityType = ResolveRdfResourceToValue("dc:type");
                    Description = ExtractFieldValue("dc:description");
                }
            }
            catch
            {
                Exception ex = new Exception(string.Format("Error retrieving information about work item # {0} from RTC: {1}", EntityId, RTCQueryResult));
                throw (ex);
            }
        }

        private string ResolveRdfResourceToValue(string fieldName)
        {
            string rtcUrl= ExtractFieldValue(fieldName);

            if (rtcUrl == null) return null;

            rtcUrl = rtcUrl.Replace("rdf:resource=\"", "");

            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _CurlExe;
            p.StartInfo.Arguments = "-k  -b \"" + _Cookies + "\" -H \"Accept:application/x-oslc-cm-change-request+xml\" " +
                                    rtcUrl;
            p.Start();
            string resXml = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return ExtractFieldValue("dc:title", resXml);
        }

        private string ExtractFieldValue(string fieldName)
        {
            return ExtractFieldValue(fieldName, _RTCQueryResult);
        }

        private string ExtractFieldValue(string fieldName, string srcBuffer)
        {
            int beg = srcBuffer.IndexOf(fieldName) + fieldName.Length;

            if (beg < fieldName.Length) return null;

            int end = srcBuffer.IndexOf("/" + fieldName, beg);

            if (end < 0)
            {
                end = srcBuffer.IndexOf("/>", beg);
            }

            if (end <= beg)
            {
                return null;
            }
            else
            {
                return srcBuffer.Substring(beg + 1, end - beg - 2);
            }
        }

        #endregion
    }
}
