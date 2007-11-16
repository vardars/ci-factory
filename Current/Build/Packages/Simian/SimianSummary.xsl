<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.microsoft.com/intellisense/ie5">

	<xsl:output method="html"/>

	<xsl:variable name="simian.root" select="//simian"/>
	<xsl:variable name="simian.version" select="$simian.root/@version" />

	<xsl:template match="/">
    <simiansummary>
		  <xsl:if test="$simian.version!=''">
					  <xsl:apply-templates select="$simian.root//summary"/>
		  </xsl:if>
    </simiansummary>
	</xsl:template>
	
	<!-- Reports rules relating specifically to namespaces -->
	<xsl:template match="summary">
		<xsl:for-each select="./@*" >
			<tr>
				<td colspan="2" class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td colspan="2" class="section-data">
					<xsl:value-of select="."/>
				</td>
			</tr>
		</xsl:for-each>
	</xsl:template>
	
</xsl:stylesheet>
