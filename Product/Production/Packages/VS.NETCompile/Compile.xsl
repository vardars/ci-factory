<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <xsl:output method="html"/>

    <xsl:template match="/">
   		<xsl:variable name="messages" select="/cruisecontrol//buildresults//message" />
   		<xsl:if test="count($messages) > 0">   	
			  <xsl:variable name="error.messages" select="$messages[(contains(text(), ' error ')) or @level='Error'] | /cruisecontrol//builderror/message | /cruisecontrol//internalerror/message" />
        <xsl:variable name="warning.messages" select="$messages[(contains(text(), ' warning ')) or @level='Warning']" />
        
        <table id="VSNETCompileErrorSection" class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
          <tr>
            <td class="sectionheader">
              Compiler Errors: (<a id="VSNETCompileErrorCount"></a>)
            </td>
          </tr>
          <tr>
            <td id="VSNETCompileErrors">
              <xsl:apply-templates select="$error.messages"/>
            </td>
          </tr>
        </table>
      
        <table id="VSNETCompileWarningSection" class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
            <tr>
                <td class="sectionheader">
                  Compiler Warnings(<a id="VSNETCompileWarningCount"></a>)
                </td>
            </tr>
            <tr>
              <td id="VSNETCompileWarnings">
                <div>
                  <a href="javascript:void(0)" class="dsphead" onclick="dsp(this, '+ Show Warnings', '+ Hide Warnings')">
                    <span class="dspchar">+ Show Warnings</span>
                  </a>
                </div>
                <div class="dspcont">
                  <xsl:apply-templates select="$warning.messages"/>
                </div>
              </td>
            </tr>
        </table>

        <script language="javascript" type="text/javascript">
          function PaintCountForSection(idCount, idPaint){
          var count = document.getElementById(idCount).getElementsByTagName("pre").length;
          var mycountainer=document.getElementById(idPaint)
          mycountainer.innerHTML=count;
          }

          function ShowIfCountSection(idCount, idSection)
          {
          var count = document.getElementById(idCount).getElementsByTagName("pre").length;
          if (count == 0)
          {
          document.getElementById(idSection).style.display = 'none';
          }
          }
          PaintCountForSection('VSNETCompileErrors', 'VSNETCompileErrorCount');
          ShowIfCountSection('VSNETCompileErrors', 'VSNETCompileErrorSection');
          PaintCountForSection('VSNETCompileWarnings', 'VSNETCompileWarningCount');
          ShowIfCountSection('VSNETCompileWarnings', 'VSNETCompileWarningSection');
        </script>
      </xsl:if>
    </xsl:template>

    <xsl:template match="message">
      <xsl:choose>
        <xsl:when test="contains(text(), 'warnaserror')">

        </xsl:when>
        <xsl:when test="contains(text(), 'errorreport')">

        </xsl:when>
        <xsl:otherwise>
          <pre class="section-error"><xsl:value-of select="text()"/></pre>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:template>

</xsl:stylesheet>

