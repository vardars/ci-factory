<?xml version="1.0"?>
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

    <xsl:variable name="quietandrecoverytimefile" select="concat('c:\projects\dod.ahlta\current\build\Artifacts\', $ArtifactFolderName, '\quietandrecoverytimehistory.xml')"/>
    <xsl:variable name="quietandrecoverytimedoc" select="document($quietandrecoverytimefile)"/>
    
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
  </style>
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
      <xsl:variable name="CompilerBug" select="count(integration[@status='CompilerBug'])" />
      <xsl:variable name="NoTrackers" select="count(integration/statistic[@name='BuildErrorMessage' and contains(text(), 'No Trackers')])" />
      <xsl:variable name="failureCount" select="$totalCount - ($successCount + $exceptionCount + $CompilerBug + $NoTrackers)"/>

      <xsl:variable name="TotalBuildTime" select="ms:SumTimes(integration/statistic[@name='Duration']/text())" />
      <xsl:variable name="TotalRecoveryTimeOther"      select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='Failure']]/@duration)" />
      <xsl:variable name="TotalRecoveryTimeNoTrackers" select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='NoTrackers']]/@duration)" />
      
      <xsl:variable name="totalCountForTheLast7Day" select="count(integration[@dayofyear > $dayofyear - 7])"/>
      <xsl:variable name="successCountForTheLast7Day" select="count(integration[@status='Success' and @dayofyear > $dayofyear - 7])"/>
      <xsl:variable name="exceptionCountForTheLast7Day" select="count(integration[@status='Exception' and @dayofyear > $dayofyear - 7])"/>
      <xsl:variable name="CompilerBugForTheLast7Day" select="count(integration[@status='CompilerBug' and @dayofyear > $dayofyear - 7])" />
      <xsl:variable name="NoTrackersForTheLast7Day" select="count(integration[@dayofyear > $dayofyear - 7]/statistic[@name='BuildErrorMessage' and contains(text(), 'No Trackers')])" />
      <xsl:variable name="failureCountForTheLast7Day" select="$totalCountForTheLast7Day - ($successCountForTheLast7Day + $exceptionCountForTheLast7Day + $CompilerBugForTheLast7Day + $NoTrackersForTheLast7Day)"/>

      <xsl:variable name="TotalBuildTimeForTheLast7Day" select="ms:SumTimes(integration[@dayofyear > $dayofyear - 7]/statistic[@name='Duration']/text())" />
      <xsl:variable name="TotalRecoveryTimeOtherForTheLast7Day"      select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='Failure' and @dayofyear > $dayofyear - 7]]/@duration)" />
      <xsl:variable name="TotalRecoveryTimeNoTrackersForTheLast7Day" select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='NoTrackers' and @dayofyear > $dayofyear - 7]]/@duration)" />
      
      <xsl:variable name="totalCountForTheDay" select="count(integration[@day=$day and @month=$month and @year=$year])"/>
			<xsl:variable name="successCountForTheDay" select="count(integration[@status='Success' and @day=$day and @month=$month and @year=$year])"/>
      <xsl:variable name="exceptionCountForTheDay" select="count(integration[@status='Exception' and @day=$day and @month=$month and @year=$year])"/>
      <xsl:variable name="CompilerBugForTheDay" select="count(integration[@status='CompilerBug' and @day=$day and @month=$month and @year=$year])" />
      <xsl:variable name="NoTrackersForTheDay" select="count(integration[@day=$day and @month=$month and @year=$year]/statistic[@name='BuildErrorMessage' and contains(text(), 'No Trackers')])" />
      <xsl:variable name="failureCountForTheDay" select="$totalCountForTheDay - ($successCountForTheDay + $exceptionCountForTheDay + $CompilerBugForTheDay + $NoTrackersForTheDay)"/>

      <xsl:variable name="TotalBuildTimeTimeForTheDay" select="ms:SumTimes(integration[@day=$day and @month=$month and @year=$year]/statistic[@name='Duration']/text())" />
      <xsl:variable name="TotalRecoveryTimeOtherForTheDay"      select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='Failure' and @day=$day and @month=$month and @year=$year]]/@duration)" />
      <xsl:variable name="TotalRecoveryTimeNoTrackersForTheDay" select="sum(($quietandrecoverytimedoc)//recoverytime[between/build[@possition=1 and @status='NoTrackers' and @day=$day and @month=$month and @year=$year]]/@duration)" />
			
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
					<th align="left">Number of Failed (other)</th>
					<td><xsl:value-of select="$failureCountForTheDay"/></td>
          <td>
            <xsl:value-of select="$failureCountForTheLast7Day"/>
          </td>
					<td><xsl:value-of select="$failureCount"/></td>
				</tr>
        <tr>
          <th align="left">Number of Failed (no trackers)</th>
          <td>
            <xsl:value-of select="$NoTrackersForTheDay"/>
          </td>
          <td>
            <xsl:value-of select="$NoTrackersForTheLast7Day"/>
          </td>
          <td>
            <xsl:value-of select="$NoTrackers"/>
          </td>
        </tr>
        <tr>
          <th align="left">Number of Failed (compiler bug)</th>
          <td>
            <xsl:value-of select="$CompilerBugForTheDay"/>
          </td>
          <td>
            <xsl:value-of select="$CompilerBugForTheLast7Day"/>
          </td>
          <td>
            <xsl:value-of select="$CompilerBug"/>
          </td>
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
        <tr>
          <th align="left">Time Spent Recovering from Failed Builds (other)</th>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeOtherForTheDay)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeOtherForTheLast7Day)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeOther)"/>
            <xsl:value-of select="' mins'"/>
          </td>
        </tr>
        <tr>
          <th align="left">Time Spent Recovering from Failed Builds (no trackers)</th>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeNoTrackersForTheDay)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeNoTrackersForTheLast7Day)"/>
            <xsl:value-of select="' mins'"/>
          </td>
          <td>
            <xsl:value-of select="round($TotalRecoveryTimeNoTrackers)"/>
            <xsl:value-of select="' mins'"/>
          </td>
        </tr>
			</table>
		</p>
    <hr/>

    <xsl:variable name="SuccessProgressChartUrl" select="concat('/dod.ahlta/Packages/Analytics/charts.swf?library_path=/dod.ahlta/Packages/Analytics/charts_library&amp;xml_source=/dod.ahlta Installs/', $ArtifactFolderName, '/SuccessProgress.xml')"/>
    <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
	      codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" 
	      WIDTH="400" 
	      HEIGHT="250" 
	      id="charts" 
	      ALIGN="">

      <PARAM NAME="movie" >
        <xsl:attribute name="value">
          <xsl:value-of select="$SuccessProgressChartUrl"/>
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
          <xsl:value-of select="$SuccessProgressChartUrl"/>
        </xsl:attribute>
      </EMBED>
    </OBJECT>

    <br/>
    <hr/>
    
    <xsl:variable name="BuildTimeHistoryChartDataChartUrl" select="concat('/dod.ahlta/Packages/Analytics/charts.swf?library_path=/dod.ahlta/Packages/Analytics/charts_library&amp;xml_source=/dod.ahlta Installs/', $ArtifactFolderName, '/BuildTimeHistoryChartData.xml')"/>
    <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
	      codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" 
	      WIDTH="400" 
	      HEIGHT="250" 
	      id="charts" 
	      ALIGN="">

      <PARAM NAME="movie" >
        <xsl:attribute name="value">
          <xsl:value-of select="$BuildTimeHistoryChartDataChartUrl"/>
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
          <xsl:value-of select="$BuildTimeHistoryChartDataChartUrl"/>
        </xsl:attribute>
      </EMBED>
    </OBJECT>

    <br/>
    <hr/>

    <xsl:variable name="QuietTimeHistoryChartDataChartUrl" select="concat('/dod.ahlta/Packages/Analytics/charts.swf?library_path=/dod.ahlta/Packages/Analytics/charts_library&amp;xml_source=/dod.ahlta Installs/', $ArtifactFolderName, '/QuietTimeHistoryChartData.xml')"/>
    <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
	      codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" 
	      WIDTH="400" 
	      HEIGHT="250" 
	      id="charts" 
	      ALIGN="">

      <PARAM NAME="movie" >
        <xsl:attribute name="value">
          <xsl:value-of select="$QuietTimeHistoryChartDataChartUrl"/>
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
          <xsl:value-of select="$QuietTimeHistoryChartDataChartUrl"/>
        </xsl:attribute>
      </EMBED>
    </OBJECT>

    <br/>
    <hr/>

    <xsl:variable name="QuietTimeHistoryLineChartDataChartUrl" select="concat('/dod.ahlta/Packages/Analytics/charts.swf?library_path=/dod.ahlta/Packages/Analytics/charts_library&amp;xml_source=/dod.ahlta Installs/', $ArtifactFolderName, '/QuietTimeHistoryLineChartData.xml')"/>
    <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
	      codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" 
	      WIDTH="400" 
	      HEIGHT="250" 
	      id="charts" 
	      ALIGN="">

      <PARAM NAME="movie" >
        <xsl:attribute name="value">
          <xsl:value-of select="$QuietTimeHistoryLineChartDataChartUrl"/>
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
          <xsl:value-of select="$QuietTimeHistoryLineChartDataChartUrl"/>
        </xsl:attribute>
      </EMBED>
    </OBJECT>

    <br/>
    <hr/>

    <xsl:variable name="RecoveryTimeHistoryChartDataChartUrl" select="concat('/dod.ahlta/Packages/Analytics/charts.swf?library_path=/dod.ahlta/Packages/Analytics/charts_library&amp;xml_source=/dod.ahlta Installs/', $ArtifactFolderName, '/RecoveryTimeHistoryChartData.xml')"/>
    <OBJECT classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
	      codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" 
	      WIDTH="400" 
	      HEIGHT="250" 
	      id="charts" 
	      ALIGN="">

      <PARAM NAME="movie" >
        <xsl:attribute name="value">
          <xsl:value-of select="$RecoveryTimeHistoryChartDataChartUrl"/>
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
          <xsl:value-of select="$RecoveryTimeHistoryChartDataChartUrl"/>
        </xsl:attribute>
      </EMBED>
    </OBJECT>
    <hr/>
		<p><pre><strong>Note: </strong>Only builds run with the statistics publisher enabled will appear on this page!</pre></p>
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
</xsl:stylesheet>

  