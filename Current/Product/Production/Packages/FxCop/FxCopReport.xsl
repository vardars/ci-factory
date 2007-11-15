<?xml version="1.0"?>
<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="user msxsl"
    >

  <xsl:output method="html"
    version="1.0"
  	indent="yes"/>

  <msxsl:script language="C#" implements-prefix="user">
    <![CDATA[
		public string MakeLongTextSplittable(string textToAlter)
		{	
			return textToAlter.Replace("(", "( ").Replace(")",") ").Replace(",", ", ").Replace(":", ": ");
		}
		
		public string ExtractCategory(string rawCategory)
		{
			return rawCategory.Replace("Microsoft.", "");
		}
		]]>
  </msxsl:script>

  <xsl:template match="/">
    <xsl:choose>
      <xsl:when test="//FxCopReport">
        <xsl:apply-templates select="//FxCopReport" />
      </xsl:when>
      <xsl:otherwise>
        <h2>FxCop Analysis Report</h2>
        <h3>No FxCop data available</h3>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="FxCopReport">
    <html>
      <head>
        <title>Analysis Report</title>
      </head>

      <style>
        .ColumnHeader {font-family: Verdana; font-size: 8pt; background-color:white; color: black}
        .CriticalError {font-family: Verdana; font-size: 8pt; color: darkred; font-weight: bold; text-align: center}
        .Error {font-family: Verdana; font-size: 8pt; color: royalblue; font-weight: bold; text-align: center}
        .CriticalWarning {font-family: Verdana; font-size: 8pt; color: green; font-weight: bold; text-align: center}
        .Warning {font-family: Verdana; font-size: 8pt; color: darkgray; font-weight: bold; text-align: center}
        .Information {font-family: Verdana; font-size: 8pt; color: black; font-weight: bold; text-align: center}
      </style>

      <script type="text/javascript">
        function toggleDiv(imgId, divId)
        {
        eDiv = document.getElementById(divId);
        eImg = document.getElementById(imgId);

        if ( eDiv.style.display == "none" )
        {
        eDiv.style.display="block";
        eImg.src="images/arrow_minus_small.gif";
        }
        else
        {
        eDiv.style.display = "none";
        eImg.src="images/arrow_plus_small.gif";
        }
        }
      </script>
      <body bgcolor="white" alink="Black" vlink="Black" link="Black">
        <div id="fxCopReport">
          <xsl:variable name="Issues" select="count(.//Message)"></xsl:variable>
          <div id="header">
            <h1>
              FxCop Version <xsl:value-of select="@Version" /> Analysis Report
            </h1>
            <h2>
              <xsl:value-of select="$Issues" /> Issues Detected
            </h2>
          </div>
          <div id="problemSummary">
            <h3>Summary</h3>
            <table>
              <tbody>
                <tr>
                  <td>Assemblies tested:</td>
                  <td>
                    <xsl:value-of select="count(.//Module)"/>
                  </td>
                </tr>
                <tr>
                  <td>Assembly violations:</td>
                  <td>
                    <xsl:value-of select="count(.//Module/Messages/Message)"/>
                  </td>
                </tr>
                <tr>
                  <td>Resource violations:</td>
                  <td>
                    <xsl:value-of select="count(.//Resources//Message)"/>
                  </td>
                </tr>
                <tr>
                  <td>Type violations:</td>
                  <td>
                    <xsl:value-of select="count(.//Type/Messages/Message)"/>
                  </td>
                </tr>
                <tr>
                  <td>Member violations:</td>
                  <td>
                    <xsl:value-of select="count(.//Member//Message)"/>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div id="breakdown">
            <xsl:if test="$Issues > 0">
              <h3>Breakdown by Assembly</h3>
              <xsl:for-each select="Targets/Target/Modules/Module">
                <xsl:apply-templates select="." />
              </xsl:for-each>
            </xsl:if>
          </div>

          <div id="rules">
            <h3>Rule Violations</h3>
            <xsl:if test="$Issues > 0">
              <table class="issuesSummary">
                <thead>
                  <tr>
                    <th class="leftCol">Rule</th>
                    <th class="rightCol">Issues</th>
                  </tr>
                </thead>
                <tbody>
                  <xsl:for-each select="Rules/Rule">
                    <xsl:sort select="@TypeName" order="ascending"/>
                    <xsl:variable name="Type" select="@TypeName" />
                    <xsl:variable name="Errors" select="count(//Message[@TypeName=$Type])" />
                    <xsl:if test ="$Errors > 0">
                      <tr>
                        <td class="leftCol">
                          <xsl:value-of select="Name/text()"/>
                        </td>
                        <td class="rightCol">
                          <xsl:value-of select ="$Errors"/>
                        </td>
                      </tr>
                    </xsl:if>
                  </xsl:for-each>
                </tbody>
              </table>
            </xsl:if>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Module">
    <xsl:variable name="divId">
      <xsl:value-of select="generate-id(@Name)" />
    </xsl:variable>

    <div class="module clickable">
      <xsl:attribute name="onclick">
        <xsl:text>toggleDiv('img-</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>','</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>')</xsl:text>
      </xsl:attribute>

      <img src="images/arrow_plus_small.gif" alt="Toggle display of Tests contained within this assembly">
        <xsl:attribute name="id">
          <xsl:text>img-</xsl:text>
          <xsl:value-of select="$divId" />
        </xsl:attribute>
      </img>
      <xsl:text>&#0160;</xsl:text>
      <xsl:value-of select="@Name" />
      <xsl:text>&#0160;(</xsl:text>
      <xsl:value-of select="count(.//Message)"/>
      <xsl:text>)</xsl:text>
    </div>
    <div>
      <xsl:attribute name="style">
        <xsl:text>display:none;</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:value-of select="$divId"/>
      </xsl:attribute>

      <xsl:if test ="Messages/Message">
        <div class="assembly" style="margin-left:20px">
          <xsl:text>Assembly</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Messages//Message)"/>
          <xsl:text>)</xsl:text>
          <ul style="margin-top:0px">
            <xsl:apply-templates select="Messages//Message" />
          </ul>
        </div>
      </xsl:if>

      <xsl:if test ="Namespaces/Namespace">
        <div class="types" style="margin-left:20px">
          <xsl:text>Types</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Namespaces//Message)"/>
          <xsl:text>)</xsl:text>
          <xsl:for-each select="Namespaces/Namespace">
            <xsl:apply-templates select="." />
          </xsl:for-each>
        </div>
      </xsl:if>
      <xsl:if test="Resources/Resource">
        <div class="resources" style="margin-left:20px">
          <xsl:text>Resources</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Resources//Message)"/>
          <xsl:text>)</xsl:text>
          <xsl:for-each select="Resources/Resource">
            <xsl:apply-templates select="." />
          </xsl:for-each>
        </div>
      </xsl:if>
      <xsl:if test="Namespaces/Namespace/Messages/Message">
        <div class="moduleLevel" style="margin-left:20px">
          <xsl:text>Module-level</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Messages/Message)"/>
          <xsl:text>)</xsl:text>

          <ul style="margin-top:0px">
            <xsl:apply-templates select="./Namespaces/Namespace/Messages/Message" />
          </ul>
        </div>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="Namespace">
    <xsl:variable name="Namespace" select="@Name"/>

    <xsl:for-each select="Types/Type">
      <xsl:apply-templates select=".">
        <xsl:with-param name="Namespace" select="$Namespace" />
      </xsl:apply-templates>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="Type">
    <xsl:param name="Namespace" />
    <xsl:variable name="divId">
      <xsl:value-of select="generate-id(@Name)" />
    </xsl:variable>

    <div class="type clickable">
      <xsl:attribute name="onclick">
        <xsl:text>toggleDiv('img-</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>','</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>')</xsl:text>
      </xsl:attribute>

      <img src="images/arrow_plus_small.gif" alt="Toggle display of Tests contained within this assembly">
        <xsl:attribute name="id">
          <xsl:text>img-</xsl:text>
          <xsl:value-of select="$divId" />
        </xsl:attribute>
      </img>

      <xsl:value-of select="$Namespace"/>
      <xsl:if test="string-length($Namespace) > 0">
        <xsl:text>.</xsl:text>
      </xsl:if>
      <xsl:value-of select="@Name" />
      <xsl:text>&#0160;(</xsl:text>
      <xsl:value-of select="count(.//Message)"/>
      <xsl:text>)</xsl:text>
    </div>
    <div>
      <xsl:attribute name="style">
        <xsl:text>display:none;</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:value-of select="$divId"/>
      </xsl:attribute>

      <xsl:if test="Messages/Message">
        <div class="typeLevel" style="margin-left:20px">
          <xsl:text>Type-level</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Messages/Message)"/>
          <xsl:text>)</xsl:text>
          <ul style="margin-top:0px">
            <xsl:apply-templates select="Messages/Message" />
          </ul>
        </div>
      </xsl:if>
      <xsl:if test="Members/Member">
        <div class="members" style="margin-left:20px">
          <xsl:text>Members</xsl:text>
          <xsl:text>&#0160;(</xsl:text>
          <xsl:value-of select="count(Members//Message)"/>
          <xsl:text>)</xsl:text>
          <xsl:for-each select="Members/Member">
            <xsl:apply-templates select="." />
          </xsl:for-each>
        </div>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="Member">
    <div class="member" style="margin-left:20px">
      <xsl:value-of select="user:MakeLongTextSplittable(@Name)" />
      <ul style="margin-top:0px">
        <xsl:apply-templates select=".//Messages/Message" />
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="Resource">
    <xsl:variable name="divId">
      <xsl:value-of select="generate-id(@Name)" />
    </xsl:variable>

    <div class="type clickable">
      <xsl:attribute name="onclick">
        <xsl:text>toggleDiv('img-</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>','</xsl:text>
        <xsl:value-of select="$divId" />
        <xsl:text>')</xsl:text>
      </xsl:attribute>

      <img src="images/arrow_plus_small.gif" alt="Toggle display of Tests contained within this assembly">
        <xsl:attribute name="id">
          <xsl:text>img-</xsl:text>
          <xsl:value-of select="$divId" />
        </xsl:attribute>
      </img>
      <xsl:value-of select="@Name" />
      <xsl:text>&#0160;(</xsl:text>
      <xsl:value-of select="count(.//Message)"/>
      <xsl:text>)</xsl:text>
    </div>
    <div>
      <xsl:attribute name="style">
        <xsl:text>display:none;</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="id">
        <xsl:value-of select="$divId"/>
      </xsl:attribute>

      <ul style="margin-top:0px">
        <xsl:apply-templates select="Messages/Message" />
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="Message">
    <li class="message">
      <span style="padding-right:10px">
        <xsl:attribute name="class">
          <xsl:value-of select="Issue/@Level" />
        </xsl:attribute>
        <xsl:value-of select="Issue/@Level" />:
      </span>
      <xsl:value-of select="user:MakeLongTextSplittable(Issue/text())" />
      <xsl:text>&#0160;[</xsl:text>
      <a target="_blank">
        <xsl:attribute name="href">
          http://www.gotdotnet.com/team/fxcop/Docs/Rules/<xsl:value-of select="user:ExtractCategory(@Category)" />/<xsl:value-of select="@TypeName" />.html
        </xsl:attribute>
        <xsl:value-of select="@TypeName" />
      </a>
      <xsl:text>]&#0160;</xsl:text>
    </li>
  </xsl:template>
</xsl:stylesheet>
