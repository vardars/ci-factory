<?xml version="1.0"?>
<xsl:stylesheet 
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:g="http://www.gallio.org/"
  exclude-result-prefixes="g"
>

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:param name="resourceRoot" select="''" />

  <xsl:variable name="cssDir">./Packages/Gallio/css/</xsl:variable>
  <xsl:variable name="jsDir">./Packages/Gallio/js/</xsl:variable>
  <xsl:variable name="imgDir">./Packages/Gallio/img/</xsl:variable>
  <xsl:variable name="attachmentBrokerUrl">GallioAttachment.aspx?</xsl:variable>
  <xsl:variable name="condensed" select="1" />

  <xsl:include href="xsl/Gallio-Report.html+xhtml.xsl" />

  <xsl:variable name="Gallio.Statistics" select="//g:report/g:testPackageRun/g:statistics"/>
  <xsl:variable name="Gallio.Assert.Count" select="sum($Gallio.Statistics/@assertCount)"/>
  <xsl:variable name="Gallio.Test.Count" select="sum($Gallio.Statistics/@testCount)"/>
  <xsl:variable name="Gallio.Run.Count" select="sum($Gallio.Statistics/@runCount)"/>
  <xsl:variable name="Gallio.Failure.Count" select="sum($Gallio.Statistics/@failedCount)"/>
  <xsl:variable name="Gallio.Skipped.Count" select="sum($Gallio.Statistics/@skippedCount)"/>
  <xsl:variable name="Gallio.Inconclusive.Count" select="sum($Gallio.Statistics/@inconclusiveCount)"/>
  <xsl:variable name="Gallio.Time" select="sum($Gallio.Statistics/@duration)"/>

  <xsl:variable name="Gallio.Failure.List" select="//g:report/g:testPackageRun//g:testStepRun[g:result/g:outcome/@status='failed' and g:testStep/g:metadata/g:entry[@key='TestKind']/g:value='Test']"/>

  <xsl:template match="/">
    <galliosummary>
      <xsl:attribute name="tests">
        <xsl:value-of select="$Gallio.Test.Count"/>
      </xsl:attribute>
      <xsl:attribute name="failures">
        <xsl:value-of select="$Gallio.Failure.Count"/>
      </xsl:attribute>
      <xsl:attribute name="skipped">
        <xsl:value-of select="$Gallio.Skipped.Count"/>
      </xsl:attribute>
      <xsl:attribute name="asserts">
        <xsl:value-of select="$Gallio.Assert.Count"/>
      </xsl:attribute>
      <xsl:attribute name="time">
        <xsl:value-of select="$Gallio.Time"/>
      </xsl:attribute>
      
      <xsl:choose>
        <xsl:when test="$Gallio.Test.Count = 0">
          <tr>
            <td colspan="2" class="section-data">No Tests Run</td>
          </tr>
          <tr>
            <td colspan="2" class="section-error">This project doesn't have any tests</td>
          </tr>
        </xsl:when>

        <xsl:when test="$Gallio.Failure.Count = 0">
          <tr>
            <td colspan="2" class="section-data">All Tests Passed</td>
          </tr>
        </xsl:when>
      </xsl:choose>

      <tr>
        <td colspan="2"> </td>
      </tr>

      <xsl:if test="$Gallio.Failure.Count > 0">
        <tr>
          <td class="sectionheader" colspan="2">
            Unit Test Failure Details (<xsl:value-of select="$Gallio.Failure.Count"/>)
          </td>
        </tr>
        <tr>
          <td colspan="2">
            <div class="gallio-report">
              <!-- Technically a link element should not appear outside of the "head"
           but most browsers tolerate it and this gives us better out of the box
           support in embedded environments like CCNet since no changes need to
           be made to the stylesheets of the containing application.
      -->
              <link rel="stylesheet" type="text/css" href="{$cssDir}Gallio-Report.css" />
              <link rel="stylesheet" type="text/css" href="{$cssDir}Gallio-Report.generated.css" />
              <style type="text/css">
                html
                {
                margin: 0px 0px 0px 0px;
                padding: 0px 17px 0px 0px;
                overflow: auto;
                }
              </style>
              <script type="text/javascript" src="{$jsDir}Gallio-Report.js" />
              <xsl:if test="g:testPackageRun//g:testStepRun/g:testLog/g:attachments/g:attachment[@contentType='video/x-flv']">
                <script type="text/javascript" src="{$jsDir}swfobject.js" />
              </xsl:if>

              <xsl:apply-templates select="." mode="xhtml-body" />
            </div>
            <script type="text/javascript">reportLoaded();</script>
            <ul class="testStepRunContainer">
              <xsl:apply-templates select="$Gallio.Failure.List" mode="details" />
            </ul>
          </td>
        </tr>
        <tr>
          <td colspan="2"> </td>
        </tr>
      </xsl:if>
    </galliosummary>
  </xsl:template>

</xsl:stylesheet>

