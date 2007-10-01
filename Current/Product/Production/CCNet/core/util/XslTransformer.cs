using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslTransformer : ITransformer
	{
        public string Transform(string input, string xslFilename, Dictionary<string, string> xslParams)
		{
            XslCompiledTransform transform = NewXslTransform(xslFilename);

            using (StringReader inputReader = new StringReader(input))
            {
                try
                {
                    StringWriter output = new StringWriter();
                    transform.Transform(new XPathDocument(inputReader), CreateXsltArgs(xslParams), output);
                    return output.ToString();
                }
                catch (XmlException ex)
                {
                    throw new CruiseControlException("Unable to execute transform: " + xslFilename, ex);
                }
            }
        }

        private static XslCompiledTransform NewXslTransform(string transformerFileName)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            LoadStylesheet(transform, transformerFileName);
            return transform;
        }

        private static XsltArgumentList CreateXsltArgs(Dictionary<string, string> xsltArgs)
        {
            XsltArgumentList args = new XsltArgumentList();
            if (xsltArgs != null)
            {
                foreach (string key in xsltArgs.Keys)
                {
                    args.AddParam(key.ToString(), "", xsltArgs[key]);
                }
            }
            return args;
        }

        private static void LoadStylesheet(XslCompiledTransform transform, string xslFileName)
        {
            XsltSettings settings = new XsltSettings(false, true);

            try
            {
                transform.Load(xslFileName, settings, new XmlUrlResolver());
            }
            catch (FileNotFoundException)
            {
                throw new CruiseControlException(string.Format("XSL stylesheet file not found: {0}", xslFileName));
            }
            catch (XsltException ex)
            {
                throw new CruiseControlException("Unable to load transform: " + xslFileName, ex);
            }
        }
    }
}
