<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html"/>

  <xsl:template match="/">

    <xsl:variable name="TotalProjects" select="//msbuilsummary/projectinfo/counts/@total" />
    <xsl:variable name="SkippedProjects" select="//msbuilsummary/projectinfo/counts/@skipped" />
    <xsl:variable name="RecompiledProjects" select="//msbuilsummary/projectinfo/counts/@recompiled" />

    <xsl:if test="$TotalProjects > 0">
      <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
        <tr>
          <td class="sectionheader">
            Projects Rebuilt: (<xsl:value-of select="$RecompiledProjects"/>)
          </td>
        </tr>
        <tr>
          <td>
            <p>
              Total Projects: <xsl:value-of select="$TotalProjects"/>
            </p>
            <p>
              Projects Skipped: <xsl:value-of select="$SkippedProjects"/>
            </p>
          </td>
        </tr>
      </table>
    </xsl:if>

    <xsl:variable name="error.messages" select="//msbuilsummary/errors/error" />
    <xsl:variable name="error.count" select="count($error.messages)"/>
    <xsl:variable name="warning.messages" select="//msbuilsummary/warnings/warning" />
    <xsl:variable name="warning.count" select="count($warning.messages)"/>

    <xsl:if test="$error.count > 0">
      <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
        <tr>
          <td class="sectionheader-error">
            Compiler Errors: (<xsl:value-of select="$error.count"/>)
          </td>
        </tr>
        <tr>
          <td>
            <xsl:for-each select="$error.messages" >
              <pre class="section-error">
                <xsl:value-of select="text()"/>
              </pre>
            </xsl:for-each>
          </td>
        </tr>
      </table>
    </xsl:if>

    <xsl:if test="$warning.count > 0">
      <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
        <tr>
          <td class="sectionheader-warning">
            Compiler Warnings: (<xsl:value-of select="$warning.count"/>)
          </td>
        </tr>
        <tr>
          <td>
            <xsl:for-each select="$warning.messages" >
              <pre class="section-warning">
                <xsl:value-of select="text()"/>
              </pre>
            </xsl:for-each>
          </td>
        </tr>
      </table>
    </xsl:if>

  </xsl:template>


</xsl:stylesheet>
