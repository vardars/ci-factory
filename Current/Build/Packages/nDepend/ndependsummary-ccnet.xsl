<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:output method="html"/>
	
	<xsl:template match="/">
    <ndependsummary>
		<style type="text/css">
		<![CDATA[
			#NDepend-report { font-family: Trebuchet MS; font-size: 10pt; }
      .title { background-color:#006; color:#FFF; }
      .subtitle { color: #883333; font-size: 10pt; font-weight: bold; background-color: #CCCCCC }
      .subtitleref { color: blue; font-size: 10pt }
      .info {color: black; font-size: 10pt}
      .biginfo {color: black; font-size: 10pt ; font-weight: bold}
      .infobold {color: black; font-size: 10pt ; font-weight: bold}
      .hdrcell_left  { color: #FFFFFF ; font-weight: bold; background-color: #B3B3B3; text-align: left;}
      .hdrcell_leftb  { color: #FFFFFF ; font-weight: bold; background-color: #939393; text-align: left;}
      .hdrcell_right { color: #FFFFFF ; font-weight: bold; background-color: #B3B3B3; text-align: right;}
      .hdrcell_rightb { color: #FFFFFF ; font-weight: bold; background-color: #939393; text-align: right;}
      .datacell_left0 { color: #000055; background-color: #DBDBDB; text-align: left; }
      .datacell_leftb0{ color: #000055; background-color: #BBBBBB; text-align: left; }
      .datacell_right0{ color: #000055; background-color: #DBDBDB; text-align: right; }
      .datacell_rightb0{ color: #000055; background-color: #BBBBBB; text-align: right; }
      .datacell_red0 { color: #000055; background-color: #FFBBBB; text-align: right; }
      .datacell_left1 { color: #000055; background-color: #EAEAEA; text-align: left; }
      .datacell_leftb1 { color: #000055; background-color: #CACACA; text-align: left; }
      .datacell_right1{ color: #000055; background-color: #EAEAEA; text-align: right; }
      .datacell_rightb1{ color: #000055; background-color: #CACACA; text-align: right; }
      .datacell_red1 { color: #000055; background-color: #FFCCCC; text-align: right; }
		]]>
		</style>
		
		<div id="NDepend-report">
			<xsl:apply-templates select="//NDepend"/>
		</div>
    </ndependsummary>
	</xsl:template>
	
	<xsl:template match="NDepend">
    <p>Put warnings, cyclical build order, and cql fail/warn messages here</p>
	</xsl:template>
	
</xsl:stylesheet>