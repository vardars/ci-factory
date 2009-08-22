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
      body 						{ font: small verdana, arial, helvetica; color:#000000;	}
      .coverageReportTable		{ font-size: 9px; }
      .reportHeader 				{ padding: 5px 8px 5px 8px; font-size: 12px; border: 1px solid; margin: 0px;	}
      .titleText					{ font-weight: bold; font-size: 12px; white-space: nowrap; padding: 0px; margin: 1px; }
      .subtitleText 				{ font-size: 9px; font-weight: normal; padding: 0px; margin: 1px; white-space: nowrap; }
      .projectStatistics			{ font-size: 10px; border-left: #649cc0 1px solid; white-space: nowrap;	width: 40%;	}
      .heading					{ font-weight: bold; }
      .mainTableHeaderLeft 		{ border: #dcdcdc 1px solid; font-weight: bold;	padding-left: 5px; }
      .mainTableHeader 			{ border-bottom: 1px solid; border-top: 1px solid; border-right: 1px solid;	text-align: center;	}
      .mainTableGraphHeader 		{ border-bottom: 1px solid; border-top: 1px solid; border-right: 1px solid;	text-align: left; font-weight: bold; }
      .mainTableGraphHeader2 		{ border-bottom: 1px solid; border-top: 1px solid; border-right: 1px solid;	text-align: center; font-weight: bold; }
      .mainTableCellItem 			{ background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-left: 10px; padding-right: 10px; font-weight: bold; font-size: 10px; }
      .mainTableCellData 			{ background: #ffffff; border-right: #dcdcdc 1px solid;	text-align: center;	white-space: nowrap; }
      .mainTableCellPercent 		{ background: #ffffff; font-weight: bold; white-space: nowrap; text-align: right; padding-left: 10px; }
      .mainTableCellGraph 		{ background: #ffffff; border-right: #dcdcdc 1px solid; padding-right: 5px; }
      .mainTableCellBottom		{ border-bottom: #dcdcdc 1px solid;	}
      .childTableHeader 			{ border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-right: 1px solid;	font-weight: bold; padding-left: 10px; }
      .childTableCellIndentedItem { background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-right: 10px; font-size: 10px; }
      .exclusionTableCellItem 	{ background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-left: 10px; padding-right: 10px; }
      .projectTable				{ background: #a9d9f7; border-color: #649cc0; }
      .primaryTable				{ background: #d7eefd; border-color: #a4dafc; }
      .secondaryTable 			{ background: #f9e9b7; border-color: #f6d376; }
      .secondaryChildTable 		{ background: #fff6df; border-color: #f5e1b1; }
      .exclusionTable				{ background: #e1e1e1; border-color: #c0c0c0; }
      .exclusionCell				{ background: #f7f7f7; }
      .graphBarNotVisited			{ font-size: 2px; border:#9c9c9c 1px solid; background:#df0000; }
      .graphBarSatisfactory		{ font-size: 2px; border:#9c9c9c 1px solid;	background:#f4f24e; }
      .graphBarVisited			{ background: #00df00; font-size: 2px; border-left:#9c9c9c 1px solid; border-top:#9c9c9c 1px solid; border-bottom:#9c9c9c 1px solid; }
      .graphBarVisitedFully		{ background: #00df00; font-size: 2px; border:#9c9c9c 1px solid; }
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
        <xsl:value-of select="$URL"/>/NCoverReport.html
      </xsl:attribute>
      This option will not work correctly.  Unfortunately, your browser does not support Inline Frames
    </iframe>

  </xsl:template>

</xsl:stylesheet>