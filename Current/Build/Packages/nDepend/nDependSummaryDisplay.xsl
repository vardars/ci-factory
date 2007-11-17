<?xml version="1.0"?>
<xsl:stylesheet
    version = "1.0"
    xmlns:xsl = "http://www.w3.org/1999/XSL/Transform">

  <xsl:output method = "html"/>

  <xsl:param name = "applicationPath"/>
  <xsl:param name="CCNetServer"/>
  <xsl:param name="CCNetProject"/>
  <xsl:param name="CCNetBuild"/>

  <xsl:template match = "/">
    <xsl:variable name="stuff" select="//ndependsummary" />
    <xsl:if test="$stuff/node()">
      <style type="text/css">
        <![CDATA[
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
      <table
                    class = "section-table"
                    cellSpacing = "0"
                    cellPadding = "2"
                    width = "98%"
                    border = "0">
        <tr>
          <td height="42" class="sectionheader-container" colSpan="2">
            <a STYLE="TEXT-DECORATION: NONE; color: 403F8D;" onmouseover="this.style.color = '#7bcf15'" onmouseout="this.style.color = '#403F8D'">
              <xsl:attribute name="href">
                /<xsl:value-of select="$CCNetServer" />/default.aspx?_action_NDependReport=true&amp;server=<xsl:value-of select="$CCNetServer" />&amp;project=<xsl:value-of select="$CCNetProject" />&amp;build=<xsl:value-of select="$CCNetBuild" />
              </xsl:attribute>
              <img src="Packages\nDepend\logo.gif" class="sectionheader-title-image"/>
              <div class="sectionheader">
                NDepend Summary
              </div>
            </a>
          </td>
        </tr>
        <xsl:copy-of select="//ndependsummary"/>
      </table>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
