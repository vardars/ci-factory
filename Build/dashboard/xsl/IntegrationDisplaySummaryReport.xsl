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
    <xsl:variable name="stuff" select="//integrationtestsummary" />
    <xsl:if test="$stuff/node()">
      <table
          class = "section-table"
          cellSpacing = "0"
          cellPadding = "2"
          width = "98%"
          border = "0">
        <tr>
          <td height="42" class="sectionheader-container">
            <div class="sectionheader">Integration Test(s)</div>
          </td>
        </tr>
        <xsl:copy-of select="//integrationtestsummary"/>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
