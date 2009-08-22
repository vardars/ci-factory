<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:param name="URL" select="/cruisecontrol/build/buildresults//target[@name='Deployment.SetUp']//target[@name='Publish.EchoWebPath']/task[@name='echo']/message"  />

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

    <div id="injectionpoint">
      <p>Loading...</p>
    </div>


    <iframe
      onload="inject();"
      id="buffer"
      style="display: none;"
      frameborder="0">
      <xsl:attribute name="src">
        <xsl:value-of select="$URL"/>/FxCopReport.html
      </xsl:attribute>
      This option will not work correctly.  Unfortunately, your browser does not support Inline Frames
    </iframe>

  </xsl:template>

</xsl:stylesheet>