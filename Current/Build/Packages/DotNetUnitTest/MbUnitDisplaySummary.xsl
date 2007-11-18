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
          <td height="42" colSpan="2">
            <xsl:attribute name="class">
              <xsl:choose>
                <xsl:when test="$stuff/@failures > 0">
                  <xsl:value-of select="sectionheader-container-error"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="sectionheader-container"/>
                </xsl:otherwise>
                </xsl:choose>
            </xsl:attribute>
            <a>
              <xsl:attribute name="style">
                <xsl:text>TEXT-DECORATION: NONE;</xsl:text>
                <xsl:choose>
                  <xsl:when test="$stuff/@failures > 0">
                    <xsl:text>color: #D13535;</xsl:text>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>color: #403F8D;</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <xsl:attribute name="onmouseover">
                <xsl:text>this.style.color = </xsl:text>
                <xsl:choose>
                  <xsl:when test="$stuff/@failures > 0">
                    <xsl:text>'#403F8D'</xsl:text>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>'#7bcf15'</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <xsl:attribute name="onmouseout">
                <xsl:text>this.style.color = </xsl:text>
                <xsl:choose>
                  <xsl:when test="$stuff/@failures > 0">
                    <xsl:text>'#D13535'</xsl:text>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>'#403F8D'</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_UnitTestDetailsBuildReport=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\DotNetUnitTest\logo.gif" class="sectionheader-title-image"/>
              <div class="sectionheader-text">
                Unit Tests (Executed: <xsl:value-of select="$stuff/@testcount"/>, Failed: <xsl:value-of select="$stuff/@failures"/>, Ignored: <xsl:value-of select="$stuff/@notrun"/>, Assert Count: <xsl:value-of select="$stuff/@AssertCount"/>, Duration: <xsl:value-of select="format-number($stuff/@time, '0.0')"/> seconds)
              </div>
            </a>
          </td>
        </tr>
        <xsl:copy-of select="//mbunitsummary"/>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
