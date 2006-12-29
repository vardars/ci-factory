<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:call-template name="CompileLog">
      <xsl:with-param name="Log" select="/cruisecontrol/build/log"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="CompileLog">
    <xsl:param name="Log"/>
    <xsl:for-each select="$Log/node()">
      <xsl:if test="name() = 'line'">
        <xsl:call-template name="Line">
          <xsl:with-param name="Text" select="text()"/>
        </xsl:call-template>
      </xsl:if>
      <xsl:if test="name() = 'builderror'">
        <xsl:call-template name="Error">
          <xsl:with-param name="Text" select="./message/text()"/>
        </xsl:call-template>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="Line">
    <xsl:param name="Text"/>
    <p><xsl:value-of select="$Text"/></p>
  </xsl:template>

  <xsl:template name="Error">
    <xsl:param name="Text"/>
    <p class="section-error"><xsl:value-of select="$Text"/></p>
  </xsl:template>

</xsl:stylesheet>
