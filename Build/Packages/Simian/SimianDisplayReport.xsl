<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:param name="URL" select="/cruisecontrol/build/buildresults//target[@name='Deployment.SetUp']//target[@name='Deployment.EchoDeploymentWebPath']/task[@name='echo']/message"  />

  <xsl:template match="/">

    <script language="javascript" type="text/javascript">
      function iFrameHeight()
      {
      var h = 0;
      var w = 0;

      h = document.getElementById('blockrandom').contentDocument.height;
      alert("Doc height: " + h);
      document.getElementById('blockrandom').style.height = h + 60 + 'px';

      w = document.getElementById('blockrandom').contentDocument.width;
      alert("Doc width: " + w);
      //document.getElementById('blockrandom').style.width = w + 'px';

      alert("Doc all:" + document.all);
      if ( !document.all )
      {
      setTimeout(function(){iFrameHeight()}, 1000)
      }
      else if( document.all )
      {
      alert("Doc scroll height: " + document.frames('blockrandom').document.body.scrollHeight);
      alert("Doc scroll width: " + document.frames('blockrandom').document.body.scrollWidth);
      alert("Doc body height: " + document.frames('blockrandom').document.body.height);
      alert("Doc body width: " + document.frames('blockrandom').document.body.width);
      h = document.frames('blockrandom').document.body.scrollHeight;
      document.all.blockrandom.style.height = h + 20 + 'px';

      w = document.frames('blockrandom').document.body.scrollWidth;
      document.all.blockrandom.style.width = w + 20 + 'px';
      }
      }
    </script>


    <iframe
      onload="iFrameHeight()"		id="blockrandom"
      name="iframe"
      width="100%"
      scrolling="auto"
      align="top"
      frameborder="0"
      class="wrapper">
      <xsl:attribute name="src">
        <xsl:value-of select="$URL"/>/SimianReport.html
      </xsl:attribute>
      This option will not work correctly.  Unfortunately, your browser does not support Inline Frames
    </iframe>

  </xsl:template>

</xsl:stylesheet>