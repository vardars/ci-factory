<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl = "http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:XPath="urn:XPathScript">

  <msxsl:script language="C#" implements-prefix="XPath">
    public string InnerXml(XPathNavigator nav)
    {
    return nav.InnerXml;
    }
  </msxsl:script>

  <xsl:output method = "html"/>

  <xsl:param name = "applicationPath"/>

  <xsl:template match = "/">
    <xsl:variable name="stuff" select="//fxcopsummary" />
    <xsl:if test="$stuff/node()">
      <xsl:value-of select="XPath:InnerXml($stuff)" disable-output-escaping="yes"/>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
