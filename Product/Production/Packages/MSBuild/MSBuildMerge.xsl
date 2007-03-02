<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE xsl:stylesheet [
  <!ENTITY nbsp "&#160;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml"/>

  <xsl:template match="/">
    <msbuilsummary>
      <errors>
        <xsl:for-each select="/msbuild//error">
          <error>
            <xsl:if test="@file != ''" >
              <xsl:value-of select="concat(@file,'&nbsp;(', @line, ',', @column, '):&nbsp;')"/>
            </xsl:if>
            <xsl:value-of select="concat('error&nbsp;', @code, ':&nbsp;', text())" />
          </error>
        </xsl:for-each>
      </errors>
      <warnings>
        <xsl:for-each select="/msbuild/warning">
          <warning>
            <xsl:if test="@file != ''" >
              <xsl:value-of select="concat(@file,'&nbsp;(', @line, ',', @column, '):&nbsp;')"/>
            </xsl:if>
            <xsl:value-of select="concat('warning&nbsp;', @code, ':&nbsp;', text())" />
          </warning>
        </xsl:for-each>
      </warnings>
      <projectinfo>
        <counts>
          <xsl:variable name="Total" select="count(/msbuild/project/target/task/target/task/project/target[@name = 'CoreCompile'])"/>
          <xsl:variable name="Skipped" select="count(/msbuild/project/target/task/target/task/project/target[@name = 'CoreCompile']/message[contains(text(), 'Skipping target')])" />
          <xsl:attribute name="total">
            <xsl:value-of select="$Total"/>
          </xsl:attribute>
          <xsl:attribute name="skipped">
            <xsl:value-of select="$Skipped" />
          </xsl:attribute>
          <xsl:attribute name="recompiled">
            <xsl:value-of select="$Total - $Skipped"/>
          </xsl:attribute>
        </counts>
      </projectinfo>
    </msbuilsummary>
  </xsl:template>
</xsl:stylesheet>