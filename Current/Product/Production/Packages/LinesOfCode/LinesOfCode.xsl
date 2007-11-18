<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html"/>
  <xsl:template match="/">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr>
        <td height="42" class="sectionheader-container" colSpan="2">
            <div class="sectionheader">
              Line Count
            </div>
        </td>
      </tr>
    </table>

    <xsl:variable name="linecounts" select="/cruisecontrol/build/code-summaries/code-summary/linecount" />
    <xsl:if test="count($linecounts) > 0">
      
          <xsl:for-each select="$linecounts">
            <div>
              <br>
                <xsl:value-of select="@label" />
              </br>
              <br>
                <xsl:text>Total Lines: </xsl:text>
                <xsl:value-of select="@totalLineCount" />
              </br>
              <br>
                <xsl:text>Empty Lines: </xsl:text>
                <xsl:value-of select="@emptyLineCount" />
              </br>
              <br>
                <xsl:text>Commented Lines: </xsl:text>
                <xsl:value-of select="@commentLineCount" />
              </br>
            </div>
          </xsl:for-each>

    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
