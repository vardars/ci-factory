<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html"/>
	
	<xsl:template match="/">
    <ndependsummary>
      <xsl:attribute name="warnings">
        <xsl:value-of select="count(//NDepend//Warning[@Category = '1'])" />
      </xsl:attribute>
		<xsl:apply-templates select="//NDepend"/>
    </ndependsummary>
	</xsl:template>
	
	<xsl:template match="NDepend">
    <xsl:choose>
      <xsl:when test="AssemblySortForCompilOrObfusk">
        <xsl:apply-templates select="AssemblySortForCompilOrObfusk" />
      </xsl:when>
      <xsl:otherwise>
        <tr>
          <td>At least one cycle exists in the assemblies dependencies. The build order cannot be computed.</td>
        </tr>
        <tr>
          <td></td>
        </tr>
      </xsl:otherwise>
    </xsl:choose>

    <tr>
      <td>
        <xsl:for-each select=".//Warning[@Category = '1']" >
          <pre class="section-warning">
            <xsl:value-of select="text()"/>
          </pre>
        </xsl:for-each>
      </td>
    </tr>
	</xsl:template>
	
</xsl:stylesheet>