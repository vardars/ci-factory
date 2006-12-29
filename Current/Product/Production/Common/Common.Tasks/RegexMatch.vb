Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports NAnt.Core
Imports NAnt.Core.Attributes

<ElementName("regexmatch")> _
Public Class RegexMatch
    Inherits LoopItems

    Private _File As FileInfo

    <TaskAttribute("file", Required:=True)> _
    Public Property File() As FileInfo
        Get
            Return _File
        End Get
        Set(ByVal value As FileInfo)
            _File = value
        End Set
    End Property

    Protected Overrides Function GetStrings() As System.Collections.IEnumerator
        Dim Strings As New ArrayList



        Return DirectCast(Strings.ToArray(GetType(String)), String()).GetEnumerator
    End Function
End Class
