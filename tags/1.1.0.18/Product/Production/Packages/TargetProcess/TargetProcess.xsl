<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:template match="/">

    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
			<tr>
				<td height="42" class="sectionheader-container">
					<img src="images/SourceControl.gif" class="sectionheader-title-image" />
					<div class="sectionheader"  >
						TargetProcess Associated Items (<xsl:value-of select="count(//TargetProcess/Entity)"/>)
					</div>
				</td>
			</tr>
			<tr>
					<td bgcolor="#2d4492" style="color:white"></td>
			</tr>
      <xsl:call-template name="Entities">
        <xsl:with-param name="Entities" select="//TargetProcess/Entity" />
      </xsl:call-template>
    </table>
  </xsl:template>

  <xsl:template name="Entities">
    <xsl:param name="Entities"/>
    <xsl:for-each select="$Entities">
			<tr>
				<td class="section-data">
					<xsl:if test="position() mod 2=0">
						<xsl:attribute name="style">border-top: #808286 1px dotted;</xsl:attribute>
					</xsl:if>
					<span >
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="@HyperLink" />
							</xsl:attribute>
							<xsl:value-of select="@Type" /> # <xsl:value-of select="@Id" /> 
						</a>
					</span>

					<div>
						<a href="javascript:void(0)" class="dsphead" onclick="dsp(this, '+ Show Description', '+ Hide Description')">
							<span class="dspchar">+ Show Description</span>
						</a>
					</div>
					<div class="dspcont">
						<xsl:value-of select="text()" />
					</div>
				</td>
			</tr>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>