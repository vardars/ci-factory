<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:g="http://www.gallio.org/">
  <xsl:output method="html" indent="no" encoding="utf-8" omit-xml-declaration="yes" />
  <xsl:param name="resourceRoot" select="''" />

  <xsl:variable name="cssDir">./Packages/Gallio/css/</xsl:variable>
  <xsl:variable name="jsDir">./Packages/Gallio/js/</xsl:variable>
  <xsl:variable name="imgDir">./Packages/Gallio/img/</xsl:variable>
  <xsl:variable name="attachmentBrokerUrl">GallioAttachment.aspx?</xsl:variable>
  <xsl:variable name="condensed" select="0" />

  <xsl:template match="/">
    <xsl:apply-templates select="//g:report" mode="html-fragment" />
  </xsl:template>
  
  <!-- Include the base HTML / XHTML report template -->
  <xsl:include href="Gallio-Report.html+xhtml.xsl" />  
</xsl:stylesheet>
