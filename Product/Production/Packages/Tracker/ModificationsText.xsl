<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="text"/>

    <xsl:variable name="modification.list" select="/ArrayOfModification/Modification"/>

    <xsl:template match="/">
      Modifications since last build (<xsl:value-of select="count($modification.list)"/>)
      
      <xsl:for-each select="$modification.list" >
        <xsl:value-of select="Type"/> - <xsl:value-of select="UserName"/> - <xsl:value-of select="FolderName"/> - <xsl:value-of select="FileName"/> - <xsl:value-of select="Comment"/> - <xsl:value-of select="ModifiedTime"/> *
      </xsl:for-each>
    </xsl:template>

</xsl:stylesheet>
