using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class XslTransformer : ITransformer
	{
		public string Transform(string input, string transformerFileName)
		{
			using (StringReader inputReader = new StringReader(input))
			{
				try
				{
					XslTransform transform = new XslTransform();
					LoadStylesheet(transform, transformerFileName);
					StringWriter output = new StringWriter();
					transform.Transform(new XPathDocument(inputReader), null, output);
					return output.ToString();
				}
				catch (XmlException ex)
				{
					throw new CruiseControlException("Unable to execute transform: " + transformerFileName, ex);
				}
			}
		}

		private void LoadStylesheet(XslTransform transform, string xslFileName)
		{
			try
			{
				transform.Load(xslFileName);
			}
			catch (FileNotFoundException)
			{
				throw new CruiseControlException(string.Format("XSL stylesheet file not found: {0}", xslFileName));
			}
			catch (XmlException ex)
			{
				throw new CruiseControlException("Unable to load transform: " + xslFileName, ex);
			}
		}

		public void Test()
		{
			string input = File.OpenText(@"C:\Projects\dod.ahlta\Current\Build\server\dod.ahlta team1\Artifacts\buildlogs\log20061103143625Lbuild.Team1-1.0.0.647.xml").ReadToEnd();
			string output = this.Transform(input, @"C:\Temp\StatisticsPublisher.xsl");
			System.Diagnostics.Debug.WriteLine(output);
			
		}
	}
}
