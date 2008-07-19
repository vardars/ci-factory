namespace Zeta.WebSpider.Spider
{
	#region Using directives.
	// ----------------------------------------------------------------------

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Xml;
	using Sgml;
	using System.Text.RegularExpressions;

	// ----------------------------------------------------------------------
	#endregion

	/////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Parses a single HTML resource for links.
	/// </summary>
	internal class ResourceParser
	{
		#region Public methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <param name="uriInfo">The URI info.</param>
		/// <param name="textContent">Content of the text.</param>
		public ResourceParser(
			SpiderSettings settings,
			UriResourceInformation uriInfo,
			string textContent )
		{
			_settings = settings;
			_uriInfo = uriInfo;
			_textContent = textContent;
		}

		/// <summary>
		/// Get all links from the text content.
		/// </summary>
		/// <returns></returns>
		public List<UriResourceInformation> ExtractLinks()
		{
			if ( string.IsNullOrEmpty( _textContent ) )
			{
				return new List<UriResourceInformation>();
			}
			else
			{
				XmlReader xml = GetDocReader( _textContent, _uriInfo.BaseUri );

				List<UriResourceInformation> result =
					DoExtractLinks( xml, _uriInfo );

				// Trace the extracted links.
				int index = 0;
				foreach ( UriResourceInformation information in result )
				{
					Trace.WriteLine(
						string.Format(
							@"Extracted link {0}/{1}: Found '{2}' in document at URI '{3}'.",
							index + 1,
							result.Count,
							information.AbsoluteUri.AbsoluteUri,
							_uriInfo.AbsoluteUri.AbsoluteUri ) );

					index++;
				}

				return result;
			}
		}

		// ------------------------------------------------------------------
		#endregion

		#region Private methods.
		// ------------------------------------------------------------------

		/// <summary>
		/// Does the extract links.
		/// </summary>
		/// <param name="xml">The XML.</param>
		/// <param name="uriInfo">The URI info.</param>
		/// <returns></returns>
		private List<UriResourceInformation> DoExtractLinks(
			XmlReader xml,
			UriResourceInformation uriInfo )
		{
			List<UriResourceInformation> links =
				new List<UriResourceInformation>();

			while ( xml.Read() )
			{
				switch ( xml.NodeType )
				{
					// Added 2006-03-27: Inside comments, too.
					case XmlNodeType.Comment:
						XmlReader childXml =
							GetDocReader( xml.Value, uriInfo.BaseUri );

						List<UriResourceInformation> childLinks =
							DoExtractLinks( childXml, uriInfo );
						links.AddRange( childLinks );
						break;

					// A node element.
					case XmlNodeType.Element:
						string[] linkAttributeNames;
						UriType linkType;
						// If this is a link element, store the URLs to modify.
						if ( IsLinkElement(
							xml.Name,
							out linkAttributeNames,
							out linkType ) )
						{
							while ( xml.MoveToNextAttribute() )
							{
								links.AddRange(
									ExtractStyleUrls(
									uriInfo.BaseUriWithFolder,
									xml.Name,
									xml.Value ) );

								foreach ( string a in linkAttributeNames )
								{
									if ( string.Compare( a, xml.Name, true ) == 0 )
									{
										string url = xml.Value;

										UriResourceInformation ui =
											new UriResourceInformation(
											_settings.Options,
											url,
											new Uri( url, UriKind.RelativeOrAbsolute ),
											uriInfo.BaseUriWithFolder,
											linkType );

										bool isOnSameSite =
											ui.IsOnSameSite( uriInfo.BaseUri );

										if ( (isOnSameSite ||
											!_settings.Options.StayOnSite) &&
											ui.IsProcessableUri )
										{
											links.Add( ui );
										}
									}
								}
							}
						}
						else
						{
							// Also, look for style attributes.
							while ( xml.MoveToNextAttribute() )
							{
								links.AddRange(
									ExtractStyleUrls(
									uriInfo.BaseUriWithFolder,
									xml.Name,
									xml.Value ) );
							}
						}
						break;
				}
			}

			return links;
		}

		/// <summary>
		/// Checks whether the given name is a HTML element (=tag) with
		/// a contained link. If true, linkAttributeNames contains a list
		/// of all attributes that are links.
		/// </summary>
		/// <returns>Returns true, if it is a link element,
		/// false otherwise.</returns>
		private static bool IsLinkElement(
			string name,
			out string[] linkAttributeNames,
			out UriType linkType )
		{
			foreach ( LinkElement e in LinkElement.LinkElements )
			{
				if ( string.Compare( name, e.Name, true ) == 0 )
				{
					linkAttributeNames = e.Attributes;
					linkType = e.LinkType;
					return true;
				}
			}

			linkAttributeNames = null;
			linkType = UriType.Resource;
			return false;
		}

		/// <summary>
		/// Detects URLs in styles.
		/// </summary>
		/// <param name="baseUri">The base URI.</param>
		/// <param name="attributeName">Name of the attribute.</param>
		/// <param name="attributeValue">The attribute value.</param>
		/// <returns></returns>
		private List<UriResourceInformation> ExtractStyleUrls(
			Uri baseUri,
			string attributeName,
			string attributeValue )
		{
			List<UriResourceInformation> result =
				new List<UriResourceInformation>();

			if ( string.Compare( attributeName, @"style", true ) == 0 )
			{
				if ( attributeValue != null &&
					attributeValue.Trim().Length > 0 )
				{
					MatchCollection matchs = Regex.Matches(
						attributeValue,
						@"url\s*\(\s*([^\)\s]+)\s*\)",
						RegexOptions.Singleline | RegexOptions.IgnoreCase );

					if ( matchs.Count > 0 )
					{
						foreach ( Match match in matchs )
						{
							if ( match != null && match.Success )
							{
								string url = match.Groups[1].Value;

								UriResourceInformation ui =
									new UriResourceInformation(
									_settings.Options,
									url,
									new Uri( url, UriKind.RelativeOrAbsolute ),
									baseUri,
									UriType.Resource );

								bool isOnSameSite =
									ui.IsOnSameSite( baseUri );

								if ( (isOnSameSite ||
									!_settings.Options.StayOnSite) &&
									ui.IsProcessableUri )
								{
									result.Add( ui );
								}
							}
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the doc reader.
		/// </summary>
		/// <param name="html">The HTML.</param>
		/// <param name="baseUri">The base URI.</param>
		/// <returns></returns>
		private static XmlReader GetDocReader(
			string html,
			Uri baseUri )
		{
			SgmlReader r = new SgmlReader();

			if ( baseUri != null &&
				!string.IsNullOrEmpty( baseUri.ToString() ) )
			{
				r.SetBaseUri( baseUri.ToString() );
			}
			r.DocType = @"HTML";
			r.InputStream = new StringReader( html );

			return r;
		}

		// ------------------------------------------------------------------
		#endregion

		#region Private variables.
		// ------------------------------------------------------------------

		private readonly SpiderSettings _settings;
		private readonly UriResourceInformation _uriInfo;
		private readonly string _textContent;

		// ------------------------------------------------------------------
		#endregion
	}

	/////////////////////////////////////////////////////////////////////////
}