<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:Coverage="http://tempuri.org/CoverageExclusions.xsd"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:Path="urn:PathScript"
  xmlns:AssemblyCount="urn:AssemblyCountScript"
  xmlns:AssemblyStatisticsTable="urn:AssemblyTableScript"
  xmlns:TotalLines="urn:TotalLinesScript"
  xmlns:TotalCoveredLines="urn:TotalCoveredLinesScript"
  xmlns:Exclusions="urn:ExclusionsScript"
  xmlns:Type="urn:TypeScript">

  <msxsl:script implements-prefix="Path" language="C#">
    <![CDATA[
    public string GetFileName(string path)
    {
      return System.IO.Path.GetFileName(path);
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="AssemblyCount" language="C#">
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

  <msxsl:script implements-prefix="AssemblyStatisticsTable" language="C#">
    <![CDATA[
    private static System.Collections.Hashtable LineList = new System.Collections.Hashtable();
    private static System.Collections.Hashtable CoveredList = new System.Collections.Hashtable();
    
    public string AddAssembly(string assemblyName)
    {
      LineList.Add(assemblyName, 0);
      CoveredList.Add(assemblyName, 0);
      return assemblyName;
    }
    
    public bool ContainsAssembly(string assemblyName)
    {
      return LineList.Contains(assemblyName);
    }
    
    public string AddToAssemblyLineCount(string assemblyName, int value)
    {
      LineList[assemblyName] = (int) LineList[assemblyName] + value;
      return LineList[assemblyName].ToString();
    }
    
    public string AssemblyLineCountValue(string assemblyName)
    {
      return LineList[assemblyName].ToString();
    }
    
    public string AddToAssemblyCoveredCount(string assemblyName, int value)
    {
      CoveredList[assemblyName] = (int) CoveredList[assemblyName] + value;
      return CoveredList[assemblyName].ToString();
    }
    
    public string AssemblyCoveredCountValue(string assemblyName)
    {
      return CoveredList[assemblyName].ToString();
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="TotalLines" language="C#">
    <![CDATA[
    private static int InternalValue = 0;
    
    public string Value()
    {
      return InternalValue.ToString();
    }
    
    public string Add(int value)
    {
      InternalValue = InternalValue + value;
      return InternalValue.ToString();
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="TotalCoveredLines" language="C#">
    <![CDATA[
    private static int InternalValue = 0;
    
    public string Value()
    {
      return InternalValue.ToString();
    }
    
    public string Add(int value)
    {
      InternalValue = InternalValue + value;
      return InternalValue.ToString();
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="Type" language="C#">
    <![CDATA[
    public string GetNameSpace(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("\\.\\w+\\.+\\w+$");
      return Rx.Replace(type, "");
    }
    public bool IsInnerClass(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("^\\w+\\.+\\w+$");
      return Rx.IsMatch(type);
    }
    public string GetInnerClassName(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("^(\\w+)\\.+\\w+$");
      return Rx.Match(type).Groups[1].Value;
    }
    public string GetClassName(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("\\.(\\w+)\\.+\\w+$");
      return Rx.Match(type).Groups[1].Value;
    }
    public string GetFullClassName(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("(.*)\\.+\\w+$");
      return Rx.Match(type).Groups[1].Value;
    }
    public string GetMemberName(string type)
    {
      System.Text.RegularExpressions.Regex Rx = new  System.Text.RegularExpressions.Regex("\\.+(\\w+)$");
      return Rx.Match(type).Groups[1].Value;
    }
    ]]>
  </msxsl:script>

  <msxsl:script implements-prefix="Exclusions" language="C#">
    <![CDATA[
    private static System.Collections.Hashtable NameSpaceList = new System.Collections.Hashtable();
    
    public string AddNameSpace(string nameSpace, bool exclude)
    {
      NameSpaceList.Add(nameSpace, exclude);
      return nameSpace;
    }
    
    public string IsNameSpaceExcluded(string nameSpace)
    {
      if (NameSpaceList.Contains(nameSpace))
      {
        if ((bool)NameSpaceList[nameSpace])
        {
          return "exclude";
        }
        else
        {
          return "include";
        }
      }
      else
      {
        return "unknown";
      }
    }
    
    private static System.Collections.Hashtable ClassList = new System.Collections.Hashtable();
    
    public string AddClass(string className, bool exclude)
    {
      ClassList.Add(className, exclude);
      return className;
    }
    
    public string IsClassExcluded(string className)
    {
      if (ClassList.Contains(className))
      {
        if ((bool)ClassList[className])
        {
          return "exclude";
        }
        else
        {
          return "include";
        }
      }
      else
      {
        return "unknown";
      }
    }
    
    private static System.Collections.Hashtable MemberList = new System.Collections.Hashtable();
    
    public string AddMember(string memberName, bool exclude)
    {
      MemberList.Add(memberName, exclude);
      return memberName;
    }
    
    public string IsMemberExcluded(string memberName)
    {
      if (MemberList.Contains(memberName))
      {
        if ((bool)MemberList[memberName])
        {
          return "exclude";
        }
        else
        {
          return "include";
        }
      }
      else
      {
        return "unknown";
      }
    }
    ]]>
  </msxsl:script>
  
  <xsl:template match="root" mode="Statistics">
    <xsl:for-each select="Assembly">

      <xsl:variable name="AssemblyName" select="Path:GetFileName(@AssemblyName)" />
      <xsl:variable name="AssemblyToBeExcluded" select="($Exclusions)/Coverage:Coverage/Coverage:exclusions/Coverage:assemblies/Coverage:assembly[@name = $AssemblyName]" />

      <xsl:if test="count($AssemblyToBeExcluded) = 0">
        <xsl:variable name="execute1" select="AssemblyCount:Increment()" />
        <xsl:variable name="execute2" select="AssemblyStatisticsTable:AddAssembly($AssemblyName)" />

        <xsl:for-each select="Function">

          <xsl:variable name="IsInnerClass" select="Type:IsInnerClass(@FunctionName)" />

          <xsl:choose>
            <xsl:when test="$IsInnerClass" >

              <xsl:variable name="InnerClassName" select="Type:GetInnerClassName(@FunctionName)" />
              <xsl:variable name="InnerClassToBeExcluded" select="($Exclusions)/Coverage:Coverage/Coverage:exclusions/Coverage:classes/Coverage:assembly[@name = $AssemblyName]/Coverage:namespace/Coverage:innerclass[@name = $InnerClassName]" />

              <xsl:if test="count($InnerClassToBeExcluded) = 0">
                <xsl:variable name="execute3" select="TotalLines:Add(@InstructionCount)" />
                <xsl:variable name="execute4" select="TotalCoveredLines:Add(@CoveredCount)" />
                <xsl:variable name="execute5" select="AssemblyStatisticsTable:AddToAssemblyLineCount($AssemblyName, @InstructionCount)" />
                <xsl:variable name="execute6" select="AssemblyStatisticsTable:AddToAssemblyCoveredCount($AssemblyName, @CoveredCount)" />
              </xsl:if>

            </xsl:when>
            <xsl:otherwise >

              <xsl:variable name="CurrentNameSpace" select="Type:GetNameSpace(@FunctionName)" />
              
              <xsl:if test="Exclusions:IsNameSpaceExcluded($CurrentNameSpace) = 'unknown'">
                <xsl:variable name="NameSpaceToBeExcluded" select="($Exclusions)/Coverage:Coverage/Coverage:exclusions/Coverage:namespaces/Coverage:assembly[@name = $AssemblyName]/Coverage:namespace[@name = $CurrentNameSpace]" />
                <xsl:variable name="execute3" select="Exclusions:AddNameSpace($CurrentNameSpace, count($NameSpaceToBeExcluded) != 0)" />
              </xsl:if>
              
              <xsl:if test="Exclusions:IsNameSpaceExcluded($CurrentNameSpace) = 'include'">

                <xsl:variable name="ClassName" select="Type:GetClassName(@FunctionName)" />
                <xsl:variable name="FullClassName" select="Type:GetFullClassName(@FunctionName)" />

                <xsl:if test="Exclusions:IsClassExcluded($FullClassName) = 'unknown'">
                  <xsl:variable name="ClassToBeExcluded" select="($Exclusions)/Coverage:Coverage/Coverage:exclusions/Coverage:classes/Coverage:assembly[@name = $AssemblyName]/Coverage:namespace[@name = $CurrentNameSpace]/Coverage:class[@name = $ClassName]" />
                  <xsl:variable name="execute3" select="Exclusions:AddClass($FullClassName, count($ClassToBeExcluded) != 0)" />
                </xsl:if>

                <xsl:if test="Exclusions:IsClassExcluded($FullClassName) = 'include'">

                  <xsl:variable name="MemberName" select="Type:GetMemberName(@FunctionName)" />

                  <xsl:if test="Exclusions:IsMemberExcluded(@FunctionName) = 'unknown'">
                    <xsl:variable name="MemberToBeExcluded" select="($Exclusions)/Coverage:Coverage/Coverage:exclusions/Coverage:members/Coverage:assembly[@name = $AssemblyName]/Coverage:namespace[@name = $CurrentNameSpace]/Coverage:class[@name = $ClassName]/Coverage:member[@name = $MemberName]" />
                    <xsl:variable name="execute3" select="Exclusions:AddMember(@FunctionName, count($MemberToBeExcluded) != 0)" />
                  </xsl:if>

                  <xsl:if test="Exclusions:IsMemberExcluded(@FunctionName) = 'include'">
                    <xsl:variable name="execute3" select="TotalLines:Add(@InstructionCount)" />
                    <xsl:variable name="execute4" select="TotalCoveredLines:Add(@CoveredCount)" />
                    <xsl:variable name="execute5" select="AssemblyStatisticsTable:AddToAssemblyLineCount($AssemblyName, @InstructionCount)" />
                    <xsl:variable name="execute6" select="AssemblyStatisticsTable:AddToAssemblyCoveredCount($AssemblyName, @CoveredCount)" />
                  </xsl:if>
                </xsl:if>
              </xsl:if>

            </xsl:otherwise>
          </xsl:choose>

        </xsl:for-each>

      </xsl:if>

    </xsl:for-each>
  </xsl:template>

  <xsl:template name="CreateBar">
    <xsl:param name="width"></xsl:param>
    <xsl:param name="colour"></xsl:param>
    <xsl:element name="TD">
      <xsl:attribute name="bgcolor">
        <xsl:value-of select="$colour"/>
      </xsl:attribute>
      <xsl:attribute name="height">10px</xsl:attribute>
      <xsl:attribute name="width">
        <xsl:value-of select="$width*3"/>
      </xsl:attribute>
      <xsl:attribute name="onmouseover">
        window.event.srcElement.title='<xsl:value-of select="$width"/>%'
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>