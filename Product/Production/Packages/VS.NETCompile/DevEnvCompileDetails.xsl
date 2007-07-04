<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:template match="/">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <xsl:for-each select="/cruisecontrol/build/log">
        <tr>
          <td>
            <a>
              <xsl:attribute name="href">
                #<xsl:value-of select="@configuration"/>
              </xsl:attribute>
              Go to <xsl:value-of select="@configuration"/>
            </a>
          </td>
        </tr>
      </xsl:for-each>
    </table>
    <br/>
    <br/>
    <br/>
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <xsl:for-each select="/cruisecontrol/build/log">
        <tr class="sectionheader">
          <td class="sectionheader">
            <a>
              <xsl:attribute name="name">
                <xsl:value-of select="@configuration"/>
              </xsl:attribute>
            </a>
            Configuration: <xsl:value-of select="@configuration"/> Time: <xsl:value-of select="@time"/>
          </td>
        </tr>
        <xsl:call-template name="CompileLog">
          <xsl:with-param name="Log" select="self::node()"/>
        </xsl:call-template>
      </xsl:for-each>
      <tr>
        <td>
          <hr/>
          <hr/>
        </td>
      </tr>
    </table>
  </xsl:template>

  <xsl:template name="CompileLog">
    <xsl:param name="Log"/>
    
    <xsl:for-each select="$Log/node()">
      <xsl:choose >
        <xsl:when test="contains(text(), '------ Build started:')">
          <xsl:call-template name="Project">
            <xsl:with-param name="Text" select="text()"/>
          </xsl:call-template>
        </xsl:when>
        <xsl:when test="name() = 'line'">
          <xsl:call-template name="Line">
            <xsl:with-param name="Text" select="text()"/>
          </xsl:call-template>
        </xsl:when>
        <xsl:when test="name() = 'builderror'">
          <xsl:call-template name="Error">
            <xsl:with-param name="Text" select="./message/text()"/>
          </xsl:call-template>
        </xsl:when>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="Project">
    <xsl:param name="Text"/>
    <tr class="sectionheader">
      <td class="sectionheader">
        <xsl:value-of select="substring-after(substring-before($Text, ' ------'), '------ Build started: ')"/>
      </td>
    </tr>
  </xsl:template>
  
  <xsl:template name="Line">
    <xsl:param name="Text"/>
    <tr class="section-data">
      <td class="section-data">
        <xsl:value-of select="$Text"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template name="Error">
    <xsl:param name="Text"/>
    <tr class="section-error">
      <td class="section-error">
        <xsl:value-of select="$Text"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
