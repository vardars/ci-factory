<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html"/>

  <xsl:template match="/">
    <xsl:if test="count(//History/ChangeSet/Change) > 0">
      <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
        <tr class="sectionheader">
          <td class="sectionheader" colspan="5">
            All Modifications made since last build (<xsl:value-of select="count(//History/ChangeSet/Change)"/>)
          </td>
        </tr>
        <xsl:apply-templates select="//History/ChangeSet/Change">
          <xsl:sort select="parent::ChangeSet/@CreationDate" order="descending" data-type="text" />
        </xsl:apply-templates>
      </table>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Change">
    <tr>
      <xsl:if test="position() mod 2=0">
        <xsl:attribute name="class">section-oddrow</xsl:attribute>
      </xsl:if>
      <xsl:if test="position() mod 2!=0">
        <xsl:attribute name="class">section-evenrow</xsl:attribute>
      </xsl:if>

      <td class="section-data" valign="top">
        <xsl:value-of select="parent::ChangeSet/@Committer"/>
      </td>
      <td class="section-data" valign="top">
        <xsl:value-of select="@ChangeType"/>
      </td>
      <td class="section-data" valign="top">
        <xsl:value-of select="@ServerItem"/>
      </td>
      <td class="section-data" valign="top">
        <xsl:value-of select="parent::ChangeSet/@Comment"/>
      </td>
      <td class="section-data" valign="top">
        <xsl:value-of select="parent::ChangeSet/@CreationDate"/>
      </td>
      <td class="section-data" valign="top">Version <xsl:value-of select="parent::ChangeSet/@ChangesetId"/></td>
    </tr>
  </xsl:template>
  
</xsl:stylesheet>