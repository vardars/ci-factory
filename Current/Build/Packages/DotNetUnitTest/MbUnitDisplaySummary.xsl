<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl = "http://www.w3.org/1999/XSL/Transform">

  <xsl:output method = "xml"/>

  <xsl:param name = "applicationPath"/>
  <xsl:param name="CCNetServer"/>
  <xsl:param name="CCNetProject"/>
  <xsl:param name="CCNetBuild"/>

  <xsl:template match = "/">
    <xsl:variable name="stuff" select="//mbunitsummary" />
    <xsl:if test="$stuff/node()">
      <table
          class = "section-table"
          cellSpacing = "0"
          cellPadding = "2"
          width = "98%"
          border = "0">
        <tr>
          <td height="42" class="sectionheader-container" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: 403F8D;" onmouseover="this.style.color = '#7bcf15'" onmouseout="this.style.color = '#403F8D'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_UnitTestDetailsBuildReport=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\DotNetUnitTest\logo.gif" class="sectionheader-title-image"/>
              <div class="sectionheader">
                Unit Tests (Executed: <xsl:value-of select="$stuff/@testcount"/>, Failed: <xsl:value-of select="$stuff/@failures"/>, Ignored: <xsl:value-of select="$stuff/@notrun"/>, Assert Count: <xsl:value-of select="$stuff/@AssertCount"/>, Duration: <xsl:value-of select="$stuff/@time"/> seconds)
              </div>
            </a>
          </td>
        </tr>
        <xsl:copy-of select="//mbunitsummary"/>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
