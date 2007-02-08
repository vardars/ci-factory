Option Explicit On
Option Strict On

Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports NAnt.Core
Imports NAnt.Core.Attributes
Imports System
Imports System.Text.RegularExpressions

<ElementName("regexmatch")> _
Public Class RegexMatch
    Inherits LoopItems

    Private _File As FileInfo
    Private _Pattern As String

    <TaskAttribute("pattern", Required:=False)> _
    Public Property Pattern() As String
        Get
            Return _Pattern
        End Get
        Set(ByVal value As String)
            _Pattern = value
        End Set
    End Property

    <TaskAttribute("file", Required:=True)> _
    Public Property File() As FileInfo
        Get
            Return _File
        End Get
        Set(ByVal value As FileInfo)
            _File = value
        End Set
    End Property

    Protected Overrides Function GetStrings() As IEnumerator
        Dim Enumerator As New LineEnumerator(Me.File.OpenText(), New Regex(Me.Pattern))
        Return Enumerator
    End Function
End Class

Public Class LineEnumerator
    Implements IEnumerator
#Region "Fields"

    Private _Current As String
    Private _reader As TextReader
    Private _regularEpression As Regex

#End Region

#Region "Properties"

    Public Property Reader() As TextReader
        Get
            Return _reader
        End Get
        Set(ByVal Value As TextReader)
            _reader = Value
        End Set
    End Property

    Public Property RegularEpression() As Regex
        Get
            Return _regularEpression
        End Get
        Set(ByVal Value As Regex)
            _regularEpression = Value
        End Set
    End Property

#End Region

#Region "Constructors"

    Public Sub New(ByVal reader As TextReader, ByVal regularEpression As Regex)
        _reader = reader
        _regularEpression = regularEpression
    End Sub

#End Region

    Public ReadOnly Property Current() As Object Implements IEnumerator.Current
        Get
            Return Me._Current
        End Get
    End Property

    Public Function GetNextLine() As String
        Dim Line As String
        While True
            Line = Me.Reader.ReadLine()

            If Line Is Nothing Then
                Return Nothing
            End If

            If Me.RegularEpression.IsMatch(Line) Then
                Return Line
            End If
        End While
        Return Nothing
    End Function

    Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
        Me._Current = Me.GetNextLine()
        If Me._Current Is Nothing Then
            Return False
            Me.Reader.Close()
        End If
        Return True
    End Function

    Public Sub Reset() Implements IEnumerator.Reset
        Throw New NotSupportedException("Reset is not suppoerted")
    End Sub

End Class
