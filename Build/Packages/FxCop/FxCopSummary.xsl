<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://schemas.microsoft.com/intellisense/ie5"
    xmlns:Exclusions="http://schemas.microsoft.com/intellisense/ie5">
  
  <xsl:output method = "html"/>
  
  <xsl:param name = "applicationPath"/>
  <xsl:variable name = "fxcop.root" select = "//FxCopReport"/>
  <xsl:variable name = "message.list" select = "$fxcop.root//Messages"/>
  
  <xsl:template match = "/">

    <xsl:variable name = "message.list.count" select = "count($message.list)"/>
    <xsl:if test = "($message.list.count > 0)">
      <fxcopsummary>
        <table
                    class = "section-table"
                    cellSpacing = "0"
                    cellPadding = "2"
                    width = "98%"
                    border = "0">
          <tr>
            <td class="sectionheader" colSpan="2">
              FxCop Summary (<xsl:value-of select="count($message.list.count)" /> Issues)
            </td>
          </tr>
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
        </table>      
      </fxcopsummary>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
