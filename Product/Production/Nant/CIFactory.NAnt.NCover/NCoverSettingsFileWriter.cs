#region Copyright © 2006 Grant Drake. All rights reserved.
/*
Copyright © 2006 Grant Drake. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NCoverExplorer.NAntTasks
{
    public static class NCoverSettingsFileWriter
    {
        private static void _BuildTempSettingsFileForNCover2xx(NCoverInfo ncoverInfo, string settingsFile)
        {
            using (Stream stream = File.Create(settingsFile))
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);
                xmlTextWriter.Indentation = 2;
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("ProfilerSettings");
                xmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlTextWriter.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                xmlTextWriter.WriteElementString("CommandLineExe", ncoverInfo.AppToProfileExe);
                if (!string.IsNullOrEmpty(ncoverInfo.AppToProfileArgs))
                {
                    xmlTextWriter.WriteElementString("CommandLineArgs", ncoverInfo.AppToProfileArgs);
                }
                if (!string.IsNullOrEmpty(ncoverInfo.WorkingDirectory))
                {
                    xmlTextWriter.WriteElementString("WorkingDirectory", ncoverInfo.WorkingDirectory);
                }
                _WriteAssemblyNodes(ncoverInfo.AssemblyList, xmlTextWriter);
                if (!string.IsNullOrEmpty(ncoverInfo.CoverageFile))
                {
                    xmlTextWriter.WriteElementString("CoverageXml", ncoverInfo.CoverageFile);
                    xmlTextWriter.WriteElementString("XmlFormat", NCoverXmlFormat.Xml2.ToString());
                }
                if (ncoverInfo.LogLevel != NCoverLogLevel.Quiet)
                {
                    if (string.IsNullOrEmpty(ncoverInfo.LogFile))
                    {
                        ncoverInfo.LogFile = "coverage.log";
                    }
                    xmlTextWriter.WriteElementString("NoLog", XmlConvert.ToString(false));
                    xmlTextWriter.WriteElementString("LogFile", ncoverInfo.LogFile);
                    bool flag = ncoverInfo.LogLevel == NCoverLogLevel.Verbose;
                    xmlTextWriter.WriteElementString("VerboseLog", flag.ToString().ToLower());
                }
                xmlTextWriter.WriteElementString("ProfileIIS", XmlConvert.ToString(ncoverInfo.ProfileIIS));
                if (!string.IsNullOrEmpty(ncoverInfo.ProfileService))
                {
                    xmlTextWriter.WriteElementString("ProfileService", ncoverInfo.ProfileService);
                }
                if (!string.IsNullOrEmpty(ncoverInfo.ProfiledProcessModule))
                {
                    xmlTextWriter.WriteElementString("ProfileProcessModule", ncoverInfo.ProfiledProcessModule);
                }
                if (!string.IsNullOrEmpty(ncoverInfo.ExcludeAttributes))
                {
                    foreach (string str in ncoverInfo.ExcludeAttributes.Split(new char[] { ';' }))
                    {
                        xmlTextWriter.WriteElementString("ExclusionAttributes", str);
                    }
                }
                if (!string.IsNullOrEmpty(ncoverInfo.TypeExclusionPatterns))
                {
                    foreach (string str2 in ncoverInfo.TypeExclusionPatterns.Split(new char[] { ';' }))
                    {
                        xmlTextWriter.WriteElementString("TypeExclusionPatterns", str2);
                    }
                }
                if (!string.IsNullOrEmpty(ncoverInfo.MethodExclusionPatterns))
                {
                    foreach (string str3 in ncoverInfo.MethodExclusionPatterns.Split(new char[] { ';' }))
                    {
                        xmlTextWriter.WriteElementString("MethodExclusionPatterns", str3);
                    }
                }
                if (!string.IsNullOrEmpty(ncoverInfo.FileExclusionPatterns))
                {
                    foreach (string str4 in ncoverInfo.FileExclusionPatterns.Split(new char[] { ';' }))
                    {
                        xmlTextWriter.WriteElementString("FileExclusionPatterns", str4);
                    }
                }
                xmlTextWriter.WriteElementString("RegisterForUser", ncoverInfo.RegisterCoverLib ? "true" : "false");
                if (!string.IsNullOrEmpty(ncoverInfo.CoverageHtmlPath))
                {
                    xmlTextWriter.WriteElementString("CoverageHtmlPath", ncoverInfo.CoverageHtmlPath);
                }
                if (!ncoverInfo.ExcludeAutoGenCode)
                {
                    xmlTextWriter.WriteElementString("AutoExclude", "false");
                }
                if (ncoverInfo.CoverageType != (NCoverCoverageType.Branch | NCoverCoverageType.SequencePoint))
                {
                    xmlTextWriter.WriteElementString("CoverageType", ncoverInfo.CoverageType.ToString());
                }
                if (ncoverInfo.SymbolSearchPolicy != 15)
                {
                    xmlTextWriter.WriteElementString("SymbolSearchPolicy", ncoverInfo.SymbolSearchPolicy.ToString());
                }
                if (ncoverInfo.ServiceTimeout > 0)
                {
                    xmlTextWriter.WriteElementString("ServiceTimeout", ncoverInfo.ServiceTimeout.ToString());
                }
                if (!string.IsNullOrEmpty(ncoverInfo.ProjectName))
                {
                    xmlTextWriter.WriteElementString("ProjectName", ncoverInfo.ProjectName);
                }
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();
                stream.Close();
            }
        }

        private static void _WriteAssemblyNodes(string assemblyList, XmlTextWriter xmlTextWriter)
        {
            StringCollection strings = new StringCollection();
            if (assemblyList.Length > 0)
            {
                foreach (string str in assemblyList.Split(new char[] { ';' }))
                {
                    string path = str.Trim();
                    if (path.Length > 0)
                    {
                        if (path.ToLower().EndsWith(".dll") || path.ToLower().EndsWith(".exe"))
                        {
                            path = Path.GetFileNameWithoutExtension(path);
                        }
                        if (!strings.Contains(path))
                        {
                            strings.Add(path);
                            xmlTextWriter.WriteElementString("Assemblies", path);
                        }
                    }
                }
            }
        }

        public static string BuildTempSettingsXmlFile(NCoverInfo ncoverInfo, string settingsFileName)
        {
            _BuildTempSettingsFileForNCover2xx(ncoverInfo, settingsFileName);
            return "//r";
        }

        public static string CreateSettingsFile(NCoverInfo ncoverInfo)
        {
            string tempFileName = FileUtilities.GetTempFileName(".settings");
            BuildTempSettingsXmlFile(ncoverInfo, tempFileName);
            return tempFileName;
        }

        public static string GetSettingsFileContent(NCoverInfo ncoverInfo)
        {
            string path = CreateSettingsFile(ncoverInfo);
            string str2 = File.ReadAllText(path);
            File.Delete(path);
            return str2;
        }
    }
}
