<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:variable name="AssemblyCount" select="count(//TestCoverage/Assembly)" />
  <xsl:variable name="TotalCoveredMemeberCount" select="count(//TestCoverage/Assembly/Class/Member[@TestMethod != ''])" />
  <xsl:variable name="TotalMemeberCount" select="count(//TestCoverage/Assembly/Class/Member)" />
  <xsl:variable name="TotalPercentCoverage" select="round($TotalCoveredMemeberCount div $TotalMemeberCount * 100)" />

  <xsl:template match="/">
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
                Assembly Count :<xsl:value-of select="$AssemblyCount"/>
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
                        <xsl:with-param name="width" select="100 - ($TotalPercentCoverage)"></xsl:with-param>
                        <xsl:with-param name="colour">Red</xsl:with-param>
                      </xsl:call-template>
                    </TR>
                  </TABLE>
                </td>
                <td>
                  <xsl:value-of select="$TotalCoveredMemeberCount"/>/<xsl:value-of select="$TotalMemeberCount"/>&#160;<xsl:value-of select="$TotalPercentCoverage"/>%
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
        <td>Assembly</td>
        <td>Percent Coverage</td>
        <td>Members Covered</td>
        <td>Total Memebers</td>
      </tr>
      <xsl:for-each select="//TestCoverage/Assembly">
        <xsl:variable name="AssemblyName" select="@Name" />
        <xsl:variable name="CoveredMemeberCount" select="count(Class/Member[@TestMethod != ''])" />
        <xsl:variable name="MemeberCount" select="count(Class/Member)" />
        <xsl:variable name="PercentCoverage" select="round($CoveredMemeberCount div $MemeberCount * 100)" />
        
          <tr>
            <td>
              <a>
                <xsl:attribute name="href">
                  #<xsl:value-of select="$AssemblyName"/>
                </xsl:attribute>
                <xsl:value-of select="$AssemblyName"/>
              </a>
            </td>
            <td>
              <xsl:value-of select="$PercentCoverage"/>%
            </td>
            <td>
              <xsl:value-of select="$CoveredMemeberCount"/>
            </td>
            <td>
              <xsl:value-of select="$MemeberCount"/>
            </td>
          </tr>
        
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="AssemblyDetails">
    <table>
      <xsl:for-each select="//TestCoverage/Assembly">
        <xsl:variable name="AssemblyName" select="@Name" />
        
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
        
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="AssemblyDetail">
    <xsl:variable name="AssemblyName" select="@Name" />
    <xsl:variable name="CoveredMemeberCount" select="count(Class/Member[@TestMethod != ''])" />
    <xsl:variable name="MemeberCount" select="count(Class/Member)" />
    <xsl:variable name="PercentCoverage" select="round($CoveredMemeberCount div $MemeberCount * 100)" />
    <TABLE width="800px" align="left" border="0">
      <TR>
        <td colspan="3">
          <H2>
            <a>
              <xsl:attribute name="name">
                <xsl:value-of select="$AssemblyName"/>
              </xsl:attribute>
            </a>
            Assembly :<xsl:value-of select="$AssemblyName"/>
          </H2>
        </td>
      </TR>
      <TR>
        <td>
          <H4>
            Members Covered
          </H4>
        </td>
        <td colspan="2">
          <H4>
            Total Members
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
          <xsl:value-of select="$CoveredMemeberCount"/>/<xsl:value-of select="$MemeberCount"/>&#160;<xsl:value-of select="$PercentCoverage"/>%
        </TD>
      </TR>
      <xsl:for-each select="Class">
        <tr>
          <td colspan="3">
            <hr/>
          </td>
        </tr>
        <TR>
          <td colspan="3">
            <H3>
              Class: <xsl:value-of select="@FullName"/>
            </H3>
          </td>
        </TR>
        <TR>
          <td colspan="3">
            <H3>
              Test Fixture: <xsl:value-of select="@TestFixture"/>
            </H3>
          </td>
        </TR>
        <tr>
          <td colspan="3">
            <hr/>
          </td>
        </tr>
        <TR>
          <TD  width="*">
            <H4>Covered</H4>
          </TD>

          <TD width="10%">
            <H4>Test</H4>
          </TD>

          <TD width="40%">
            <H4>Memeber</H4>
          </TD>
        </TR>
        <xsl:for-each select="Member">  
          <xsl:call-template name="Function" />
        </xsl:for-each>
      </xsl:for-each>
    </TABLE>
  </xsl:template>

  <xsl:template name="Function">
    <xsl:variable name="PercentageCovered" >
      <xsl:choose>
        <xsl:when test="@TestMethod != ''">
          <xsl:value-of select="100"/>
        </xsl:when>
        <xsl:otherwise >
          <xsl:value-of select="0"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <TR style="height:1px">

      <TD style="width:300px;height:10px" valign="bottom">
        <TABLE style="width:100%;" border="0" cellpadding="0" cellspacing="0">
          <TR height='10px'>
            <xsl:choose>
              <xsl:when test="$PercentageCovered = '100'" >
                <xsl:call-template name="CreateBar">
                  <xsl:with-param name="width" select="$PercentageCovered"></xsl:with-param>
                  <xsl:with-param name="colour">Green</xsl:with-param>
                </xsl:call-template>    
              </xsl:when>
              <xsl:otherwise>
                <xsl:call-template name="CreateBar">
                  <xsl:with-param name="width" select="100 - $PercentageCovered"></xsl:with-param>
                  <xsl:with-param name="colour">Red</xsl:with-param>
                </xsl:call-template>    
              </xsl:otherwise>
            </xsl:choose>
          </TR>
        </TABLE>
      </TD>
      <TD style="height:10px; font: 9pt Courier New;" valign="bottom">

        <xsl:value-of select="@TestMethod"/>

      </TD>
      <TD style="height:10px; font: 9pt Courier New;" valign="bottom">

        <xsl:value-of select="@Name"/>

      </TD>
    </TR>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template name="CreateBar">
    <xsl:param name="width"></xsl:param>
    <xsl:param name="colour"></xsl:param>
    <xsl:element name="TD">
      <xsl:attribute name="bgcolor">
        <xsl:value-of select="$colour"/>
      </xsl:attribute>
      <xsl:attribute name="height">10px</xsl:attribute>
      <xsl:attribute name="width">
        <xsl:value-of select="$width*3"/>
      </xsl:attribute>
      <xsl:attribute name="onmouseover">
        window.event.srcElement.title='<xsl:value-of select="$width"/>%'
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet> 
