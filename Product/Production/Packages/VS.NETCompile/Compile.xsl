<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:Coverage="http://tempuri.org/CoverageExclusions.xsd"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:ErrorCount="urn:ErrorCountScript"
  xmlns:WarningCount="urn:WarningCountScript">

  <msxsl:script implements-prefix="ErrorCount" language="C#">
    <![CDATA[
    private static int InternalValue = 0;
    
    public string Value()
    {
      return InternalValue.ToString();
    }
    
    public string Increment()
    {
      InternalValue = InternalValue + 1;
      return InternalValue.ToString();
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="WarningCount" language="C#">
    <![CDATA[
    private static int InternalValue = 0;
    
    public string Value()
    {
      return InternalValue.ToString();
    }
    
    public string Increment()
    {
      InternalValue = InternalValue + 1;
      return InternalValue.ToString();
    }
    ]]>
  </msxsl:script>

    <xsl:output method="html"/>

    <xsl:template match="/">
   		<xsl:variable name="messages" select="/cruisecontrol//buildresults//message" />
   		<xsl:if test="count($messages) > 0">   	
			  <xsl:variable name="error.messages" select="$messages[(contains(text(), ' error ')) or @level='Error'] | /cruisecontrol//builderror/message | /cruisecontrol//internalerror/message" />
        <xsl:variable name="warning.messages" select="$messages[(contains(text(), ' warning ')) or @level='Warning']" />

        <xsl:for-each select="$error.messages">
          <xsl:choose>
            <xsl:when test="contains(text(), 'warnaserror')">
              
            </xsl:when>
            <xsl:when test="contains(text(), 'errorreport')">

            </xsl:when>
            <xsl:otherwise>
              <xsl:variable name="execute1" select="ErrorCount:Increment()" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>

        <xsl:for-each select="$warning.messages">
          <xsl:choose>
            <xsl:when test="contains(text(), 'warnaserror')">

            </xsl:when>
            <xsl:when test="contains(text(), 'errorreport')">

            </xsl:when>
            <xsl:otherwise>
              <xsl:variable name="execute1" select="WarningCount:Increment()" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
        
        <xsl:if test="ErrorCount:Value() > 0">
          <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
            <tr>
              <td class="sectionheader">
                  Compiler Errors: (<xsl:value-of select="ErrorCount:Value()"/>)
              </td>
            </tr>
            <tr>
              <td>
                <xsl:apply-templates select="$error.messages"/>
              </td>
            </tr>
          </table>
        </xsl:if>
        
        <xsl:if test="WarningCount:Value() > 0">
            <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
                <tr>
                    <td class="sectionheader">
                        Compiler Warnings: (<xsl:value-of select="WarningCount:Value()"/>)
                    </td>
                </tr>
                <tr><td><xsl:apply-templates select="$warning.messages"/></td></tr>
            </table>
        </xsl:if>
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

