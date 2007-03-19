<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html"/>

  <xsl:variable name="AssemblyCount" select="count(//TestCoverage/Assembly)" />
  <xsl:variable name="CoveredMemeberCount" select="count(//TestCoverage/Assembly/Class/Member[@TestMethod != ''])" />
  <xsl:variable name="TotalMemeberCount" select="count(//TestCoverage/Assembly/Class/Member)" />
  <xsl:variable name="TotalPercentCoverage" select="round($CoveredMemeberCount div $TotalMemeberCount * 100)" />

  <xsl:template match="//TestCoverage">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr class="sectionheader">
        <td class="sectionheader" colspan="5">
          Test Coverage (<xsl:value-of select="$TotalPercentCoverage"/>%)
        </td>
      </tr>
      <TR>
        <td>
          Assembly Count :<xsl:value-of select="$AssemblyCount"/>
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
                    <xsl:with-param name="width" select="100 - ($TotalPercentCoverage)"></xsl:with-param>
                    <xsl:with-param name="colour">Red</xsl:with-param>
                  </xsl:call-template>
                </TR>
              </TABLE>
            </td>
            <td>
              <xsl:value-of select="$CoveredMemeberCount"/>/<xsl:value-of select="$TotalMemeberCount"/>&#160;<xsl:value-of select="$TotalPercentCoverage"/>%
            </td>
          </table>
        </TD>
      </TR>
    </table>
  </xsl:template>

  <xsl:template name="CreateBar">
    <xsl:param name="width"></xsl:param>
    <xsl:param name="colour"></xsl:param>
    <xsl:element name="TD">
      <xsl:attribute name="bgcolor">
        <xsl:value-of select="$colour"/>
      </xsl:attribute>
      <xsl:attribute name="height">10px</xsl:attribute>
      <xsl:attribute name="width">
        <xsl:value-of select="$width*3"/>
      </xsl:attribute>
      <xsl:attribute name="onmouseover">
        window.event.srcElement.title='<xsl:value-of select="$width"/>%'
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet> 
