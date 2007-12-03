<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml"/>

	<xsl:template match="/">
    <integrationtestsummary>
      <tr>
        <td class="section-data">
          Test Count: <xsl:value-of select="count(//ReportContainer/FixtureReports/FixtureReport/TestReports/TestReport)"/>
        </td>
      </tr>
      <xsl:for-each select="//ReportContainer/FixtureReports/FixtureReport/TestReports/TestReport[@Passed = 'false']">
        <tr>
          <td class="section-error">
            <strong>
              <xsl:value-of select="@Name"/>
            </strong>
            <br/>
            <xsl:value-of select="FailureException"/>
          </td>
        </tr>
      </xsl:for-each>
    </integrationtestsummary>
</xsl:template>

</xsl:stylesheet>