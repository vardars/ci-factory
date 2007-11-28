<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html"/>

	<xsl:template match="/">
		<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
			<tr>
				<td height="42" class="sectionheader-container">
					<div class="sectionheader">Integration Test(s)</div>
				</td>
			</tr>
			<tr>
				<td class="section-data">
					Test Count: <xsl:value-of select="count(//ReportContainer/FixtureReports/FixtureReport/TestReports/TestReport)"/>
				</td>
			</tr>
			<xsl:for-each select="//ReportContainer/FixtureReports/FixtureReport/TestReports/TestReport[@Passed = 'false']">
				<tr>
					<td class="section-error"><strong><xsl:value-of select="@Name"/></strong><br/>
						<xsl:value-of select="FailureException"/>
					</td>
				</tr>
			</xsl:for-each>

		</table>
	</xsl:template>

</xsl:stylesheet>