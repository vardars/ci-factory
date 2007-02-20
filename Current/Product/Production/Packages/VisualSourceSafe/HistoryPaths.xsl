<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:lxslt="http://xml.apache.org/xslt">

  <xsl:output method="text"/>

  <xsl:variable name="Count" select="count(/VssHistory/Entry)"/>

  <xsl:template match="/">
    <xsl:if test="$Count > 1">
      true
    </xsl:if>
    <xsl:if test="$Count &lt; 2">
      false
    </xsl:if>
  </xsl:template>

  <xsl:template match="/VssHistory/Entry">
    <xsl:value-of select="self::node()/@Path"/>
  </xsl:template>
  
</xsl:stylesheet>