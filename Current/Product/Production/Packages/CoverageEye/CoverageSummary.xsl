<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:Coverage="http://tempuri.org/CoverageExclusions.xsd"
  xmlns:AssemblyCount="urn:AssemblyCountScript"
  xmlns:TotalLines="urn:TotalLinesScript"
  xmlns:TotalCoveredLines="urn:TotalCoveredLinesScript">

  <xsl:import href="CoverageStuff.xsl"/>

  <xsl:param name="ExclusionFile" select="'PostExclusion.xml'"/>
  <xsl:variable name="Exclusions" select="document($ExclusionFile)"/>
  
  <xsl:template match="/">
    <xsl:apply-templates select="/cruisecontrol/build/root" mode="Statistics"/>
    <xsl:variable name="TotalPercentCoverage" select="round(TotalCoveredLines:Value() div TotalLines:Value() * 100)" />
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr class="sectionheader">
        <td class="sectionheader" colspan="5">
          Coverage (<xsl:value-of select="$TotalPercentCoverage"/>%)
        </td>
      </tr>
      <TR>
        <td>
            Assembly Count :<xsl:value-of select="AssemblyCount:Value()"/>
        </td>
      </TR>
      <TR>
        <TD style="width:300px;" valign="bottom">
          <table>
            <td>
              <TABLE style="width:100%;" border="0" cellpadding="0" cellspacing="0">
                <TR>
                  <xsl:call-template name="CreateBar">
                    <xsl:with-param name="width" select="$TotalPercentCoverage"></xsl:with-param>
                    <xsl:with-param name="colour">Green</xsl:with-param>
                  </xsl:call-template>
                  <xsl:call-template name="CreateBar">
                    <xsl:with-param name="width" select="100 - $TotalPercentCoverage"></xsl:with-param>
                    <xsl:with-param name="colour">Red</xsl:with-param>
                  </xsl:call-template>
                </TR>
              </TABLE>
            </td>
            <td>
              <xsl:value-of select="TotalCoveredLines:Value()"/>/<xsl:value-of select="TotalLines:Value()"/>&#160;<xsl:value-of select="$TotalPercentCoverage"/>%
            </td>
          </table>
        </TD>
      </TR>
    </table>
  </xsl:template>

</xsl:stylesheet>