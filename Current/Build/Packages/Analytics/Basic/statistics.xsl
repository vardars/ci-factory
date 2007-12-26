<?xml version="1.0"?>
<!DOCTYPE dashboard [
  <!ENTITY % entities SYSTEM "..\..\..\Entities.xml">

  %entities;
]>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:ms="urn:DateScripts"
  exclude-result-prefixes="ms msxsl">


  <msxsl:script implements-prefix="ms" language="C#">
    <![CDATA[
    
    public string FormatDate(string dateTime, string format)
    {
      return System.DateTime.Parse(dateTime).ToString(format);
    }
    
    public string SumTimes(System.Xml.XPath.XPathNodeIterator nodes)
    {
        System.TimeSpan Total = new System.TimeSpan();
        while (nodes.MoveNext())
        {
            string DurationString = nodes.Current.Value;
            System.TimeSpan Duration = System.TimeSpan.Parse(DurationString);
            Total = Total.Add(Duration);
        }
        return Total.TotalMinutes.ToString();
    }
    
    ]]>
  </msxsl:script>


  <xsl:output method="html"/>
	
  <xsl:template match="/statistics">
    <xsl:variable name="MostRecentIntegration" select="/statistics/integration[position() = last()]" />
    <xsl:variable name="ArtifactFolderName" select="ms:FormatDate($MostRecentIntegration/statistic[@name='StartTime']/text(), 'yyyyMMddHHmmss')" />

    <style>
      *.pass{
      background-color: #33ff99;
      }
      *.fail{
      background-color: #ff6600;
      }
      *.unknown{
      background-color: #ffffcc;
      }
      *.exception{
      background-color: #000000;
      color: #ffffff;
      }
      a.dsphead{
      text-decoration:none;
      margin-left:1.5em;
      }
      a.dsphead:hover{
      text-decoration:underline;
      }
      a.dsphead span.dspchar{
      font-family:monospace;
      font-weight:normal;
      }
      .dspcont{
      display:none;
      margin-left:1.5em;
      }
    </style>


    <script type="text/javascript">
      <![CDATA[
function dsp(loc){
   if(document.getElementById){
      var foc = loc.firstChild;
      foc = loc.firstChild.innerHTML ? loc.firstChild : loc.firstChild.nextSibling;
      foc.innerHTML = foc.innerHTML == '+ Show Chart' ? '- Hide Chart' : '+ Show Chart';
      foc = loc.parentNode.nextSibling.style ? loc.parentNode.nextSibling : loc.parentNode.nextSibling.nextSibling;
      foc.style.display = foc.style.display == 'block' ? 'none' : 'block';
    }
}  

if(!document.getElementById)
   document.write('<style type="text/css"><!--\n.dspcont{display:block;}\n//--></style>');

      ]]>
    </script>

    <noscript>
      <style type="text/css">
        .dspcont{display:block;}
      </style>
    </noscript>
		<p>
			Today is
			<xsl:variable name="day" select="$MostRecentIntegration/@day"/>
			<xsl:variable name="month" select="$MostRecentIntegration/@month"/>
			<xsl:variable name="year" select="$MostRecentIntegration/@year"/>
      <xsl:variable name="week" select="$MostRecentIntegration/@week"/>
      <xsl:variable name="dayofyear" select="$MostRecentIntegration/@dayofyear"/>
      
			<xsl:value-of select="$month"/>/<xsl:value-of select="$day"/>/<xsl:value-of select="$year"/> <br />
			
      <xsl:variable name="totalCount" select="count(integration)"/>
			<xsl:variable name="successCount" select="count(integration[@status='Success'])"/>
      <xsl:variable name="exceptionCount" select="count(integration[@status='Exception'])"/>
      <xsl:variable name="failureCount" select="$totalCount - ($successCount + $exceptionCount)"/>

      <xsl:variable name="TotalBuildTime" select="ms:SumTimes(integration/statistic[@name='Duration']/text())" />
      
      <xsl:variable name="totalCountForTheLast7Day" select="count(integration[@dayofyear > $dayofyear - 7])"/>
      <xsl:variable name="successCountForTheLast7Day" select="count(integration[@status='Success' and @dayofyear > $dayofyear - 7 and @year = $year])"/>
      <xsl:variable name="exceptionCountForTheLast7Day" select="count(integration[@status='Exception' and @dayofyear > $dayofyear - 7 and @year = $year])"/>
      <xsl:variable name="failureCountForTheLast7Day" select="$totalCountForTheLast7Day - ($successCountForTheLast7Day + $exceptionCountForTheLast7Day)"/>

      <xsl:variable name="TotalBuildTimeForTheLast7Day" select="ms:SumTimes(integration[@dayofyear > $dayofyear - 7 and @year = $year]/statistic[@name='Duration']/text())" />
      
      <xsl:variable name="totalCountForTheDay" select="count(integration[@day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="successCountForTheDay" select="count(integration[@status='Success' and @day=$day and @month=$month and @year=$year])"/>
      <xsl:variable name="exceptionCountForTheDay" select="count(integration[@status='Exception' and @day=$day and @month=$month and @year=$year])"/>
      <xsl:variable name="failureCountForTheDay" select="$totalCountForTheDay - ($successCountForTheDay + $exceptionCountForTheDay)"/>

      <xsl:variable name="TotalBuildTimeTimeForTheDay" select="ms:SumTimes(integration[@day=$day and @month=$month and @year=$year]/statistic[@name='Duration']/text())" />
      
      <table class="section-table" cellpadding="2" cellspacing="0" border="1">
        <tr class="sectionheader">
					<th>Integration Summary</th>
					<th>For Today</th>
          <th>For Last 7 Days</th>
					<th>Overall</th>
				</tr>
				<tr>
					<th align="left">Total Builds</th>
					<td><xsl:value-of select="$totalCountForTheDay"/></td>
          <td>
            <xsl:value-of select="$totalCountForTheLast7Day"/>
          </td>
					<td><xsl:value-of select="$totalCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Successful</th>
					<td><xsl:value-of select="$successCountForTheDay"/></td>
          <td>
            <xsl:value-of select="$successCountForTheLast7Day"/>
          </td>
					<td><xsl:value-of select="$successCount"/></td>
				</tr>
				<tr>
					<th align="left">Number of Failed</th>
					<td><xsl:value-of select="$failureCountForTheDay"/></td>
          <td>
            <xsl:value-of select="$failureCountForTheLast7Day"/>
          </td>
					<td><xsl:value-of select="$failureCount"/></td>
				</tr>
        <tr>
          <th align="left">Number of Exceptions</th>
          <td>
            <xsl:value-of select="$exceptionCountForTheDay"/>
          </td>
          <td>
            <xsl:value-of select="$exceptionCountForTheLast7Day"/>
          </td>
          <td>
            <xsl:value-of select="$exceptionCount"/>
          </td>
        </tr>
        <tr>
          <th align="left">Time Spent Building</th>
          <td>
            <xsl:value-of select="round($TotalBuildTimeTimeForTheDay)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalBuildTimeForTheLast7Day)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalBuildTime)"/>
            <xsl:value-of select="' mins'"/>
          </td>
        </tr>
			</table>
		</p>
    <hr/>

    <xsl:variable name="BaseChartUrl" select="concat('/&ProjectName;-&ProjectCodeLineName;/&PackagesDirectoryName;/Analytics/charts.swf?library_path=/&ProjectName;-&ProjectCodeLineName;/&PackagesDirectoryName;/Analytics/charts_library&amp;xml_source=/&ProjectName;-&ProjectCodeLineName;/&ArtifactRootDirectoryName;/', $ArtifactFolderName)"/>

    <table cellpadding="0" cellspacing="0" border="0">
      <tr>
        <td>
          <p style="width: 25em;">
            This chart displays the total number of builds over all time (Team1 begins in July and Team2 begins in late Sept).
            The count is not as important as the slope of the line.  You would like to see the yellow line moving up and the other lines flat.
          </p>
        </td>
        <td>
          <xsl:call-template name="ShowChart">
            <xsl:with-param name="Url" select="concat($BaseChartUrl, '/SuccessProgress.xml')"/>
          </xsl:call-template>
        </td>
      </tr>
    </table>

    <br/>
    <hr/>

    <table cellpadding="0" cellspacing="0" border="0">
      <tr>
        <td>
          <p style="width: 25em;">
            This chart displays the last 200 build times.  The yellow line is the total build time.
            The blue line is the compile time.  The others are shown as area instead of lines.  Generally they are small.
            There are other parts of the build that are not shown on the chart.  The parts depiced are the ones that have
            the greatest chance of causing long build times.
          </p>
        </td>
        <td>
          <xsl:call-template name="ShowChart">
            <xsl:with-param name="Url" select="concat($BaseChartUrl, '/BuildTimeHistoryChartData.xml')"/>
          </xsl:call-template>
        </td>
      </tr>
    </table>

    <br/>
    <hr/>
    
		<table  class="section-table" cellpadding="2" cellspacing="0" border="1" width="98%">
      <tr class="sectionheader">
				<th>Build Label</th>
				<th>Status</th>
				<xsl:for-each select="./integration[last()]/statistic">
					<th>
						<xsl:value-of select="./@name" />
					</th>
				</xsl:for-each>
			</tr>
			<xsl:for-each select="./integration">
				<xsl:sort select="position()" data-type="number" order="descending"/>
				<xsl:variable name="colorClass">
					<xsl:choose>
						<xsl:when test="./@status = 'Success'">pass</xsl:when>
						<xsl:when test="./@status = 'Unknown'" >unknown</xsl:when>
            <xsl:when test="./@status = 'Exception'" >exception</xsl:when>
						<xsl:otherwise>fail</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<tr>
          <xsl:if test="(position()) mod 2 = 0">
            <xsl:attribute name="class">section-oddrow</xsl:attribute>
          </xsl:if>
          <xsl:if test="(position()) mod 2 != 0">
            <xsl:attribute name="class">section-evenrow</xsl:attribute>
          </xsl:if>
					<th>
						<xsl:value-of select="./@build-label"/>
					</th>
					<th class="{$colorClass}">
						<xsl:value-of select="./@status"/>
					</th>
					<xsl:for-each select="./statistic">
						<td>
							<xsl:value-of select="."/>
						</td>
					</xsl:for-each>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
  
  <xsl:template name="ShowChart">
    <xsl:param name="Url" />

    <div>
      <a href="javascript:void(0)" class="dsphead" onclick="dsp(this)">
        <span class="dspchar">+ Show Chart</span>
      </a>
    </div>
    <div class="dspcont">
      <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
          codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0"
          WIDTH="400"
          HEIGHT="250"
          id="charts"
          ALIGN="">

        <PARAM NAME="movie" >
          <xsl:attribute name="value">
            <xsl:value-of select="$Url"/>
          </xsl:attribute>
        </PARAM>
        <PARAM NAME="quality" VALUE="high"/>
        <PARAM NAME="bgcolor" VALUE="#666666"/>

        <EMBED
               quality="high"
               bgcolor="#666666"
               WIDTH="700"
               HEIGHT="350"
               NAME="charts"
               ALIGN=""
               swLiveConnect="true"
               TYPE="application/x-shockwave-flash"
               PLUGINSPAGE="http://www.macromedia.com/go/getflashplayer">
          <xsl:attribute name="src">
            <xsl:value-of select="$Url"/>
          </xsl:attribute>
        </EMBED>
      </OBJECT>
    </div>
  </xsl:template>
</xsl:stylesheet>

  