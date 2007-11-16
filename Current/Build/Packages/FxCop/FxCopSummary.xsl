<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  
  <xsl:output method = "xml"/>
  
  <xsl:param name = "applicationPath"/>
  <xsl:variable name = "fxcop.root" select = "/FxCopReport"/>
  <xsl:variable name = "message.list" select = "$fxcop.root//Messages"/>
  
  <xsl:template match = "/">

    <xsl:variable name = "message.list.count" select = "count($message.list)"/>
    <xsl:if test = "($message.list.count > 0)">
      <fxcopsummary>
        <xsl:attribute name="issues">
          <xsl:value-of select="$message.list.count" />
        </xsl:attribute>
          <tr>
            <td>Assemblies tested:</td>
            <td>
              <xsl:value-of select="count(.//Module)"/>
            </td>
          </tr>
          <tr>
            <td>Assembly violations:</td>
            <td>
              <xsl:value-of select="count(.//Module/Messages/Message)"/>
            </td>
          </tr>
          <tr>
            <td>Resource violations:</td>
            <td>
              <xsl:value-of select="count(.//Resources//Message)"/>
            </td>
          </tr>
          <tr>
            <td>Type violations:</td>
            <td>
              <xsl:value-of select="count(.//Type/Messages/Message)"/>
            </td>
          </tr>
          <tr>
            <td>Member violations:</td>
            <td>
              <xsl:value-of select="count(.//Member//Message)"/>
            </td>
          </tr>   
      </fxcopsummary>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
