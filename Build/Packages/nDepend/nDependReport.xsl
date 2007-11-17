<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:param name="URL" select="/cruisecontrol/build/buildresults//target[@name='Deployment.SetUp']//target[@name='Deployment.EchoDeploymentWebPath']/task[@name='echo']/message"  />

  <xsl:template match="/">

    <script language="javascript" type="text/javascript">
      function inject()
      {
      var doc;
      if (typeof document.getElementById('buffer').document != 'undefined')
      {
      doc = window.frames['buffer'].document;
      }
      else if (typeof document.getElementById('buffer').contentDocument != 'undefined')
      {
      doc = document.getElementById('buffer').contentDocument;
      }

      if (typeof doc.body.innerHTML != 'undefined')
      {
      document.getElementById('injectionpoint').innerHTML = doc.body.innerHTML;
      }
      else
      {
      setTimeout(function(){inject()}, 1000);
      }
      }
    </script>

    <style type="text/css" xmlns="http://www.w3.org/TR/xhtml1/strict">

      BODY { font-family: Trebuchet MS; font-size: 10pt; }
      TD { font-family: Trebuchet MS; font-size: 10pt; }
      .title { font-size: 25pt; font-weight: bold; }
      .subtitle { color: #883333; font-size: 20pt; font-weight: bold; background-color: #CCCCCC }
      .subtitleref { color: blue; font-size: 10pt }
      .info {color: black; font-size: 10pt}
      .biginfo {color: black; font-size: 12pt ; font-weight: bold}
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
      .datacell_stat0 { color: #000055; background-color: #C0C0FF; text-align: right; }
      .datacell_stat1 { color: #000055; background-color: #D0D0FF; text-align: right; }
      .datacell_empty { color: #000055; background-color: #FFFFFF; text-align: right; }
      .cql_ok { color: #000000; background-color: #A0FFA0; text-align: left; font-size: 10pt ; font-weight: bold}
      .cql_warning { color: #000000; background-color: #FFFF70; text-align: left; font-size: 10pt ; font-weight: bold}
      .cql_error { color: #FFFFFF; background-color: #FF0000; text-align: left; font-size: 10pt ; font-weight: bold}

    </style>

    <div id="injectionpoint">
      <p>Loading...</p>
    </div>


    <iframe
      onload="inject();"
      id="buffer"
      style="display: none;"
      frameborder="0">
      <xsl:attribute name="src">
        <xsl:value-of select="$URL"/>/NDependReport.html
      </xsl:attribute>
      This option will not work correctly.  Unfortunately, your browser does not support Inline Frames
    </iframe>

  </xsl:template>

</xsl:stylesheet>