<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl = "http://www.w3.org/1999/XSL/Transform"
    xmlns = "http://schemas.microsoft.com/intellisense/ie5">
  <xsl:output method = "html"/>

  <xsl:param name = "applicationPath"/>

  <xsl:template match = "/">
    <xsl:value-of select="//fxcopsummary"/>
  </xsl:template>
</xsl:stylesheet>
