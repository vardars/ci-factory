<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:lxslt="http://xml.apache.org/xslt">

  <xsl:output method="html"/>
  
  <xsl:template match="/">
    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <!-- Modifications -->
      <tr>
        <td class="sectionheader" colspan="5">
          Deployment File(s)
        </td>
      </tr>
      <xsl:for-each select="/cruisecontrol/build/buildresults//target[@name='Deployment.PublishLink']">
        <xsl:call-template name="DeploymentFile">
          <xsl:with-param name="DeploymentWebPath" select=".//target[@name='Private.Deployment.EchoDeploymentFileWebPath']/task[@name='echo']/message"/>
          <xsl:with-param name="DeploymentWebName" select=".//target[@name='Private.Deployment.EchoDeploymentFileWebName']/task[@name='echo']/message"/>
        </xsl:call-template>
      </xsl:for-each>
      
    </table>
  </xsl:template>
  
  <xsl:template name="DeploymentFile">
    <xsl:param name="DeploymentWebPath"/>
    <xsl:param name="DeploymentWebName"/>
    <tr>
      <td class="section-data">
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="$DeploymentWebPath"/>
          </xsl:attribute>
          <xsl:value-of select="$DeploymentWebName"/>
        </a>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
