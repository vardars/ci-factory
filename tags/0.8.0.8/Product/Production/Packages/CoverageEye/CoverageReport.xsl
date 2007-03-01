<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
  xmlns:Coverage="http://tempuri.org/CoverageExclusions.xsd"
  xmlns:Path="urn:PathScript"
  xmlns:AssemblyCount="urn:AssemblyCountScript"
  xmlns:AssemblyStatisticsTable="urn:AssemblyTableScript"
  xmlns:TotalLines="urn:TotalLinesScript"
  xmlns:TotalCoveredLines="urn:TotalCoveredLinesScript"
  xmlns:Exclusions="urn:ExclusionsScript"
  xmlns:Type="urn:TypeScript">

  <xsl:import href="CoverageStuff.xsl"/>

  <xsl:param name="ExclusionFile" select="'PostExclusion.xml'"/>
  <xsl:variable name="Exclusions" select="document($ExclusionFile)"/>
  
	<xsl:output method="html"/>
  
  <xsl:variable name="WebPath" select="(//target[@name='Deployment.PublishFileSilently']//target[@name='Private.Deployment.EchoDeploymentWebPath']/task[@name='echo']/message)[1]"/>
	
  <xsl:template match="/">
    <xsl:apply-templates select="/cruisecontrol/build/root" mode="Statistics"/>
    <xsl:variable name="TotalPercentCoverage" select="round(TotalCoveredLines:Value() div TotalLines:Value() * 100)" />
		<HTML>
			<HEAD>
				<TITLE/>
        <STYLE>H4 { height:10px; font: 9pt Courier New;  }</STYLE>
			  </HEAD>
			  <BODY>
          <TABLE width="800px" align="left" border="0">
            <TR>
              <td>
                <h2>Summary</h2>
              </td>
            </TR>
            <TR>
              <td>
                <H4>
                  Assembly Count :<xsl:value-of select="AssemblyCount:Value()"/>
                </H4>
              </td>
            </TR>
            <TR>
              <td>
                <h3>Total Percent Coverage</h3>
              </td>
            </TR>
            <TR>
              <TD style="width:300px;" valign="bottom">
                <table>
                  <td>
                    <TABLE style="width:100%;" border="0" cellpadding="0" cellspacing="0">
                      <TR>
                        <xsl:call-template name="CreateBar">
                          <xsl:with-param name="width" select="$TotalPercentCoverage"></xsl:with-param>
                          <xsl:with-param name="colour">Green</xsl:with-param>
                        </xsl:call-template>
                        <xsl:call-template name="CreateBar">
                          <xsl:with-param name="width" select="100 - $TotalPercentCoverage"></xsl:with-param>
                          <xsl:with-param name="colour">Red</xsl:with-param>
                        </xsl:call-template>
                      </TR>
                    </TABLE>
                  </td>
                  <td>
                    <xsl:value-of select="TotalCoveredLines:Value()"/>/<xsl:value-of select="TotalLines:Value()"/>&#160;<xsl:value-of select="$TotalPercentCoverage"/>%
                  </td>
                </table>
              </TD>
            </TR>
            <TR>
              <td>
                <hr/>
                <hr/>
                <h2>Assemblies</h2>
                <hr/>
              </td>
            </TR>
            <TR>
              <td>
                <xsl:call-template name="AssemblyTable"/>
              </td>
            </TR>
            <TR>
              <td>
                <xsl:call-template name="AssemblyDetails"/>
              </td>
            </TR>
          </TABLE>
        </BODY>
      </HTML>
  </xsl:template>

  <xsl:template name="AssemblyTable">
    <table style="width:100%;" border="1" cellpadding="0" cellspacing="0">
      <tr>
        <td>Report</td>
        <td>Assembly</td>
        <td>Percent Coverage</td>
        <td>Lines Covered</td>
        <td>Total Lines</td>
      </tr>
      <xsl:for-each select="cruisecontrol/build/root/Assembly">
        <xsl:variable name="AssemblyName" select="Path:GetFileName(@AssemblyName)" />

        <xsl:if test="AssemblyStatisticsTable:ContainsAssembly($AssemblyName)">
          <tr>
            <td>
              <a>
                <xsl:attribute name="href">
                  <xsl:value-of select="$WebPath"/>/CoverageReport.<xsl:value-of select="$AssemblyName"/>.xml
                </xsl:attribute>Download
              </a>
            </td>
            <xsl:variable name="PercentCoverage" select="round(AssemblyStatisticsTable:AssemblyCoveredCountValue($AssemblyName) div AssemblyStatisticsTable:AssemblyLineCountValue($AssemblyName) * 100)" />
            <td>
              <a>
                <xsl:attribute name="href">#<xsl:value-of select="$AssemblyName"/>
              </xsl:attribute>
                <xsl:value-of select="$AssemblyName"/>
              </a>
            </td>
            <td>
              <xsl:value-of select="$PercentCoverage"/>%
            </td>
            <td>
              <xsl:value-of select="AssemblyStatisticsTable:AssemblyCoveredCountValue($AssemblyName)"/>
            </td>
            <td>
              <xsl:value-of select="AssemblyStatisticsTable:AssemblyLineCountValue($AssemblyName)"/>
            </td>
          </tr>
        </xsl:if>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="AssemblyDetails">
    <table>
      <xsl:for-each select="cruisecontrol/build/root/Assembly">
        <xsl:variable name="AssemblyName" select="Path:GetFileName(@AssemblyName)" />
        <xsl:if test="AssemblyStatisticsTable:ContainsAssembly($AssemblyName)">
          <tr>
            <td>
              <xsl:call-template name="AssemblyDetail"/>
            </td>
          </tr>
          <tr>
            <td border="1">
              <hr/>
              <hr/>
            </td>
          </tr>
        </xsl:if>
      </xsl:for-each>
    </table>
	</xsl:template>
	
  <xsl:template name="AssemblyDetail">
    <xsl:variable name="AssemblyName" select="Path:GetFileName(@AssemblyName)" />
    <xsl:variable name="PercentCoverage" select="round(AssemblyStatisticsTable:AssemblyCoveredCountValue($AssemblyName) div AssemblyStatisticsTable:AssemblyLineCountValue($AssemblyName) * 100)" />
		<TABLE width="800px" align="left" border="0">
			<TR>
				<td colspan="3">
					<H3>
            <a>
              <xsl:attribute name="name">
                <xsl:value-of select="$AssemblyName"/>
              </xsl:attribute>
            </a>
						Assembly Path :<xsl:value-of select="$AssemblyName"/>
					</H3>					
				</td>				
			</TR>
			<TR>
				<td>
					<H4>
						Total Coverage
					</H4>					
				</td>
				<td colspan="2">
					<H4>
						Total IL Instructions
					</H4>					
				</td>				
			</TR>
			<TR>				
				<TD style="width:300px;" valign="bottom">	
					<TABLE style="width:100%;" border="0" cellpadding="0" cellspacing="0">
					<TR>				
						<xsl:call-template name="CreateBar">
							<xsl:with-param name="width" select="$PercentCoverage"></xsl:with-param>
							<xsl:with-param name="colour">Green</xsl:with-param>
						</xsl:call-template>
						<xsl:call-template name="CreateBar">
							<xsl:with-param name="width" select="100 - $PercentCoverage"></xsl:with-param>
							<xsl:with-param name="colour">Red</xsl:with-param>
						</xsl:call-template>	
					</TR>
					</TABLE>		
				</TD>	
				<TD colspan="2" style="height:10px; font: 9pt Courier New;">
          <xsl:value-of select="AssemblyStatisticsTable:AssemblyCoveredCountValue($AssemblyName)"/>/<xsl:value-of select="AssemblyStatisticsTable:AssemblyLineCountValue($AssemblyName)"/>&#160;<xsl:value-of select="$PercentCoverage"/>%
        </TD>			
			</TR>
			<tr>
				<td colspan="3"><hr/></td>
			</tr>
			<TR>
				<TD  width="*">
					<H4>Coverage</H4>
				</TD>

				<TD width="10%">
					<H4>IL Count</H4>
				</TD>

				<TD width="40%">
					<H4>Function</H4>
				</TD>
			</TR>
      <xsl:for-each select="Function">
        <xsl:if test="Exclusions:IsMemberExcluded(@FunctionName) = 'include'">
          <xsl:call-template name="Function" />
        </xsl:if>
      </xsl:for-each>
		</TABLE>
	</xsl:template>
	
  <xsl:template name="Function">
		<TR style="height:1px">

			<TD style="width:300px;height:10px" valign="bottom">
				<TABLE style="width:100%;" border="0" cellpadding="0" cellspacing="0">
					<TR height='10px'>
						<xsl:call-template name="CreateBar">
							<xsl:with-param name="width" select="@PercentageCovered"></xsl:with-param>
							<xsl:with-param name="colour">Green</xsl:with-param>
						</xsl:call-template>
						<xsl:call-template name="CreateBar">
							<xsl:with-param name="width" select="100 - @PercentageCovered"></xsl:with-param>
							<xsl:with-param name="colour">Red</xsl:with-param>
						</xsl:call-template>
					</TR>
				</TABLE>
			</TD>			
			<TD style="height:10px; font: 9pt Courier New;" valign="bottom">

        <xsl:value-of select="@CoveredCount"/>/<xsl:value-of select="@InstructionCount"/>&#160;<xsl:value-of select="@PercentageCovered"/>%

      </TD>
			<TD style="height:10px; font: 9pt Courier New;" valign="bottom">
				
					<xsl:value-of select="@FunctionName"/>
				
			</TD>
		</TR>
		<xsl:apply-templates/>
	</xsl:template>
	
</xsl:stylesheet>
