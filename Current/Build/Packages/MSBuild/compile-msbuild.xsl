<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="xml"/>
  <xsl:param name = "applicationPath"/>
  <xsl:param name="CCNetServer"/>
  <xsl:param name="CCNetProject"/>
  <xsl:param name="CCNetBuild"/>

  <xsl:template match="/">

    <xsl:variable name="TotalProjects" select="//msbuilsummary/projectinfo/counts/@total" />
    <xsl:variable name="SkippedProjects" select="//msbuilsummary/projectinfo/counts/@skipped" />
    <xsl:variable name="RecompiledProjects" select="//msbuilsummary/projectinfo/counts/@recompiled" />

    <xsl:if test="$TotalProjects > 0">
      <table
                    class = "section-table"
                    cellSpacing = "0"
                    cellPadding = "2"
                    width = "98%"
                    border = "0">
        <tr>
          <td height="42" class="sectionheader-container" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: 403F8D;" onmouseover="this.style.color = '#7bcf15'" onmouseout="this.style.color = '#403F8D'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_MSBuildCompileDetails=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\MSBuild\logo.ico" class="sectionheader-title-image"/>
              <div>
                Projects Rebuilt: (<xsl:value-of select="$RecompiledProjects"/>)
              </div>
            </a>
          </td>
        </tr>
        <tr>
          <td>
            Total Projects: <xsl:value-of select="$TotalProjects"/>
          </td>
        </tr>
        <tr>
          <td>
            Projects Skipped: <xsl:value-of select="$SkippedProjects"/>
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
          <td height="42" class="sectionheader-container-error" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: D13535;" onmouseover="this.style.color = '#403F8D'" onmouseout="this.style.color = '#D13535'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_MSBuildCompileDetails=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\MSBuild\logo.ico" class="sectionheader-title-image"/>
              <div>
                Compiler Errors: (<xsl:value-of select="$error.count"/>)
              </div>
            </a>
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
          <td height="42" class="sectionheader-container-warning" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: FF7700;" onmouseover="this.style.color = '#403F8D'" onmouseout="this.style.color = '#FF7700'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_MSBuildCompileDetails=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\MSBuild\logo.ico" class="sectionheader-title-image"/>
              <div>
                Compiler Warnings: (<xsl:value-of select="$warning.count"/>)
              </div>
            </a>
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
