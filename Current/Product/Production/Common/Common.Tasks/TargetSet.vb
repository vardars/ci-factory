Imports System.IO
Imports System.Xml
Imports NAnt.Core
Imports NAnt.Core.Types
Imports NAnt.Core.Attributes
Imports System
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections

<ElementName("targetset")> _
Public Class TargetSet
    Inherits LoopItems

#Region "Fields"

    Private _include As StringItem()
    Private _exclude As StringItem()
    Private _includeRegexs As ArrayList
    Private _excludeRegexs As ArrayList

#End Region

#Region "Properties"

    Public Property ExcludeRegexs() As ArrayList
        Get
            If _excludeRegexs Is Nothing Then
                _excludeRegexs = Me.ConvertPatternsToRegexs(Me.Exclude)
            End If
            Return _excludeRegexs
        End Get
        Set(ByVal value As ArrayList)
            _excludeRegexs = value
        End Set
    End Property

    Public Property IncludeRegexs() As ArrayList
        Get
            If _includeRegexs Is Nothing Then
                _includeRegexs = Me.ConvertPatternsToRegexs(Me.Include)
            End If
            Return _includeRegexs
        End Get
        Set(ByVal value As ArrayList)
            _includeRegexs = value
        End Set
    End Property

    <BuildElementArray("exclude", ElementType:=GetType(StringItem), Required:=False)> _
    Public Property Exclude() As StringItem()
        Get
            Return _exclude
        End Get
        Set(ByVal Value As StringItem())
            _exclude = Value
        End Set
    End Property

    <BuildElementArray("include", ElementType:=GetType(StringItem), Required:=False)> _
    Public Property Include() As StringItem()
        Get
            Return _include
        End Get
        Set(ByVal Value As StringItem())
            _include = Value
        End Set
    End Property

#End Region

#Region "Helpers"

    Private Function ConvertPatternsToRegexs(ByVal patterns As StringItem()) As ArrayList
        Dim Matchers As New ArrayList
        For Each RawPattern As StringItem In patterns
            Dim RegexPattern As New StringBuilder(RawPattern.StringValue)
            RegexPattern.Replace("\", "\\")
            RegexPattern.Replace(".", "\.")
            RegexPattern.Replace("*", ".*")
            RegexPattern.Replace("$", "\$")
            RegexPattern.Replace("^", "\^")
            RegexPattern.Replace("{", "\{")
            RegexPattern.Replace("[", "\[")
            RegexPattern.Replace("(", "\(")
            RegexPattern.Replace(")", "\)")
            RegexPattern.Replace("+", "\+")

            If RegexPattern.Length > 0 Then
                RegexPattern.Insert(0, "^")
                RegexPattern.Append("$")
            End If

            Dim PatternText As String = RegexPattern.ToString()

            If (PatternText.StartsWith("^.*")) Then
                PatternText = PatternText.Substring(3)
            End If
            If (PatternText.EndsWith(".*$")) Then
                PatternText = PatternText.Substring(0, RegexPattern.Length - 3)
            End If

            Matchers.Add(New Regex(PatternText))
        Next
        Return Matchers
    End Function

    Private Function LookForMatch(ByVal name As String, ByVal regexs As ArrayList) As Boolean
        For Each Matcher As Regex In regexs
            If Matcher.IsMatch(name) Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

    Protected Overrides Function GetStrings() As System.Collections.IEnumerator
        Dim TargetList As New ArrayList()

        For Each Canidate As Target In Me.Project.Targets
            If Me.LookForMatch(Canidate.Name, Me.IncludeRegexs) And Not Me.LookForMatch(Canidate.Name, Me.ExcludeRegexs) Then
                TargetList.Add(Canidate.Name)
            End If
        Next

        Return TargetList.GetEnumerator()
    End Function

End Class

