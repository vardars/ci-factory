<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl = "http://www.w3.org/1999/XSL/Transform">

  <xsl:output method = "xml" omit-xml-declaration="yes"/>

  <xsl:param name = "applicationPath"/>
  <xsl:param name="CCNetServer"/>
  <xsl:param name="CCNetProject"/>
  <xsl:param name="CCNetBuild"/>

  <xsl:template match = "/">
    <xsl:variable name="stuff" select="//galliosummary" />
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
                  <xsl:text>sectionheader-container-error"</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>sectionheader-container"</xsl:text>
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
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_GallioTestReportCondensed=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\Gallio\logo.gif" class="sectionheader-title-image"/>
              <div class="sectionheader-text">
                Unit Tests (Executed: <xsl:value-of select="$stuff/@tests"/>, Failed: <xsl:value-of select="$stuff/@failures"/>, Skipped: <xsl:value-of select="$stuff/@skipped"/>, Assert Count: <xsl:value-of select="$stuff/@asserts"/>, Duration: <xsl:value-of select="format-number($stuff/@time, '0.0')"/> seconds)
              </div>
            </a>
          </td>
        </tr>
		<tr>
			<td colSpan="2">
				<xsl:copy-of select='//galliosummary//link[1]'/>
				<div class="gallio-report">
					<xsl:copy-of select='//galliosummary//node()[@id="Header"]'/>
					<xsl:copy-of select='//galliosummary//node()[@id="Statistics"]'/>
					<!--<xsl:copy-of select='//galliosummary//node()[@id="Summary"]'/>-->
				</div>
			</td>
		</tr>
	</table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
