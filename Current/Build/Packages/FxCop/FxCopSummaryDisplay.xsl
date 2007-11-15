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
  <xsl:param name="CCNetServer"/>
  <xsl:param name="CCNetProject"/>
  <xsl:param name="CCNetBuild"/>

  <xsl:template match = "/">
    <xsl:variable name="stuff" select="//fxcopsummary" />
    <xsl:if test="$stuff/node()">
      <table
                    class = "section-table"
                    cellSpacing = "0"
                    cellPadding = "2"
                    width = "98%"
                    border = "0">
        <tr>
          <td class="sectionheader" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: 403F8D;" onmouseover="this.style.color = '#7bcf15'" onmouseout="this.style.color = '#403F8D'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_FxCopReport=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img style="float: left; border-style: none" src="Packages\FxCop\fxcop.ico" height="25" title="FxCop" alt="FxCop"/>
              <div style="position:relative; top: -10px;">
                FxCop Summary (<xsl:value-of select="$stuff/@issues" /> Issues)
              </div>
            </a>
          </td>
        </tr>
        <xsl:value-of select="XPath:InnerXml($stuff)" disable-output-escaping="yes"/>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
