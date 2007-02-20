<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:output method="html"/>

  <xsl:template match="/">

    <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
      <tr class="sectionheader">
        <td class="sectionheader" colspan="5">

          Trackers (<xsl:value-of select="count(//TrackerQuery/Tracker)"/>)
        </td>
      </tr>
      <xsl:call-template name="TrackerHeader">
        <xsl:with-param name="TrackerFields" select="//TrackerQuery/TrackerFields/Field" />
      </xsl:call-template>
      <xsl:call-template name="Trackers">
        <xsl:with-param name="TrackerNodes" select="//TrackerQuery/Tracker" />
      </xsl:call-template>
    </table>
  </xsl:template>

  <xsl:template name="TrackerHeader">
    <xsl:param name="TrackerFields"/>
    <tr>
      <xsl:for-each select="$TrackerFields">
        <td bgcolor="#2d4492" style="color:white">
          <xsl:value-of select="self::node()"/>
        </td>
      </xsl:for-each>
    </tr>
  </xsl:template>

  <xsl:template name="Trackers">
    <xsl:param name="TrackerNodes"/>
    <xsl:for-each select="$TrackerNodes">
      <tr>
        <xsl:if test="(position()) mod 2 = 0">
          <xsl:attribute name="class">section-oddrow</xsl:attribute>
        </xsl:if>
        <xsl:if test="(position()) mod 2 != 0">
          <xsl:attribute name="class">section-evenrow</xsl:attribute>
        </xsl:if>
        <xsl:for-each select="child::*">
          <!-- Get the node value -->
          <xsl:variable name="nodeval" select="."/>
          <td class=".section-data" width="5%" >
            <xsl:value-of select="$nodeval"/>
          </td>
        </xsl:for-each>
      </tr>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>