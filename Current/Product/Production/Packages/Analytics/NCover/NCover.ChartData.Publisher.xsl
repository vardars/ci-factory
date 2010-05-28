<?xml version="1.0"?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:exsl="http://exslt.org/common"
  exclude-result-prefixes="exsl">


  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:for-each select="/Builds/integration[position() > last()-200]">
      <xsl:variable name="SolutionTotalCoveragePercentage" select="statistic[@name='solutionTotalCoveragePercentage']/text()" />
      <xsl:variable name="SolutionFunctionCoveragePercentage" select="statistic[@name='solutionFunctionCoveragePercentage']/text()" />
      <number>
        <xsl:value-of select="position()"/>
      </number>
      <exsl:document href="Artifacts\NCover.SolutionTotalCoveragePercentage.xml" fragment="yes" append="yes" >
        <number>
          <xsl:value-of select="$SolutionTotalCoveragePercentage"/>
        </number>
      </exsl:document>
      <exsl:document href="Artifacts\NCover.SolutionFunctionCoveragePercentage.xml" fragment="yes" append="yes" >
        <number>
          <xsl:value-of select="$SolutionFunctionCoveragePercentage"/>
        </number>
      </exsl:document>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>
