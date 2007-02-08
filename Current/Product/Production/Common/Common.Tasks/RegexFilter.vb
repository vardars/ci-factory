Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Filters
Imports NAnt.Core.Attributes
Imports System
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections
Imports System.Collections.Generic

<ElementName("regexreplace")> _
Public Class RegexFilter
    Inherits Filter

    Private Delegate Function AcquireCharDelegate() As Integer

#Region "Fields"

    Private _ReadChar As AcquireCharDelegate
    Private _CharQueue As Queue(Of Integer)
    Private _LineQueue As Queue(Of Line)
    Private _Lines As Integer
    Private _IsPrimed As Boolean = False
    Private _AtEndOfStream As Boolean
    Private _Pattern As String
    Private _PatternFinder As Regex
    Private _Replacment As String

    Private NewLineFinder As New Regex("\n")

#End Region

#Region "Properties"

    Public Property PatternFinder() As Regex
        Get
            If _PatternFinder Is Nothing Then
                _PatternFinder = New Regex(Me.Pattern)
            End If
            Return _PatternFinder
        End Get
        Set(ByVal value As Regex)
            _PatternFinder = Value
        End Set
    End Property

    <TaskAttribute("replacment", Required:=True), StringValidator(AllowEmpty:=True)> _
    Public Property Replacment() As String
        Get
            Return _Replacment
        End Get
        Set(ByVal Value As String)
            _Replacment = Value
        End Set
    End Property

    <TaskAttribute("pattern", Required:=True), StringValidator(AllowEmpty:=False)> _
    Public Property Pattern() As String
        Get
            Return _Pattern
        End Get
        Set(ByVal Value As String)
            _Pattern = Value
        End Set
    End Property

    Public Property AtEndOfStream() As Boolean
        Get
            Return _AtEndOfStream
        End Get
        Set(ByVal Value As Boolean)
            _AtEndOfStream = Value
        End Set
    End Property

    Public Property LineQueue() As Queue(Of Line)
        Get
            If _LineQueue Is Nothing Then
                _LineQueue = New Queue(Of Line)
            End If
            Return _LineQueue
        End Get
        Set(ByVal value As Queue(Of Line))
            _LineQueue = value
        End Set
    End Property

    Public Property IsPrimed() As Boolean
        Get
            Return _IsPrimed
        End Get
        Set(ByVal value As Boolean)
            _IsPrimed = value
        End Set
    End Property

    <TaskAttribute("lines", Required:=False), Int32Validator()> _
    Public Property Lines() As Integer
        Get
            Return _Lines
        End Get
        Set(ByVal value As Integer)
            _Lines = value
        End Set
    End Property

    Public Property CharQueue() As Queue(Of Integer)
        Get
            Return _CharQueue
        End Get
        Set(ByVal value As Queue(Of Integer))
            _CharQueue = value
        End Set
    End Property

    Private Property ReadChar() As AcquireCharDelegate
        Get
            Return _ReadChar
        End Get
        Set(ByVal value As AcquireCharDelegate)
            _ReadChar = value
        End Set
    End Property

#End Region

    Public Overrides Sub Chain(ByVal parentChainedReader As NAnt.Core.Filters.ChainableReader)
        MyBase.Chain(parentChainedReader)
        'System.Diagnostics.Debugger.Break()
        Me.ReadChar = New AcquireCharDelegate(AddressOf MyBase.Read)
    End Sub

    Public Overrides Function Peek() As Integer
        Throw New ApplicationException("Peek currently is not supported.")
    End Function

    Public Overrides Function Read() As Integer
        Return Me.GetNextCharacter()
    End Function

    Public Function GetNextCharacter() As Integer
        If Me.CharQueue Is Nothing OrElse Me.CharQueue.Count = 0 Then
            Me.FillQueue()
        End If
        Return Me.CharQueue.Dequeue()
    End Function

    Public Sub FillQueue()
        If Not Me.IsPrimed Then
            Me.Prime()
        End If
        Me.ReadLine()
        Dim Line As Line = Me.LineQueue.Dequeue()
        Me.CharQueue = Line.Characters
        Me.ReplaceIfMatchFound(Line)
    End Sub

    Public Sub Prime()
        Dim LinesToRead As Integer = Me.Lines - 1
        While LinesToRead > 0
            Me.ReadLine()
            LinesToRead -= 1
        End While
        Me.IsPrimed = True
    End Sub

    Public Sub ReadLine()
        If Me.AtEndOfStream Then
            Return
        End If

        Dim Character As Integer
        Dim Line As New Line
        Me.LineQueue.Enqueue(Line)

        Do
            Character = MyBase.Read()
            Line.Append(Character)

            If Character = -1 Then
                'end of stream
                Me.AtEndOfStream = True
                Return
            End If

        Loop Until Me.IsEndOfLine(Character)
    End Sub

    Public Function IsEndOfLine(ByVal character As Integer) As Boolean
        return me.NewLineFinder.IsMatch(Microsoft.VisualBasic.Chr(character).ToString())
    End Function

    Public Sub ReplaceIfMatchFound(ByVal line As Line)
        Dim Canidate As New StringBuilder()
        Canidate.Append(line.Text)
        For Each NextLine As Line In Me.LineQueue
            Canidate.Append(NextLine.Text)
        Next
        If Me.PatternFinder.IsMatch(Canidate.ToString()) Then
            Dim Replaced As String
            Replaced = Me.PatternFinder.Replace(Canidate.ToString(), Me.Replacment)
            Me.CharQueue = New Queue(Of Integer)
            For Each Character As Char In Replaced
                Me.CharQueue.Enqueue(Microsoft.VisualBasic.Asc(Character))
            Next
            If Me.AtEndOfStream Then
                Me.CharQueue.Enqueue(-1)
            End If
            Me.IsPrimed = False
            Me.LineQueue.Clear()
        End If
    End Sub
End Class

Public Class Line

    Private _Builder As StringBuilder
    Private _Characters As Queue(Of Integer)

    Public ReadOnly Property Characters() As Queue(Of Integer)
        Get
            If _Characters Is Nothing Then
                _Characters = New Queue(Of Integer)
            End If
            Return _Characters
        End Get
    End Property

    Private ReadOnly Property Builder() As StringBuilder
        Get
            If _Builder Is Nothing Then
                _Builder = New StringBuilder()
            End If
            Return _Builder
        End Get
    End Property

    Public ReadOnly Property Text() As String
        Get
            Return Me.Builder.ToString()
        End Get
    End Property

    Public Sub Append(ByVal Character As Integer)
        If Not Character = -1 Then
            Me.Builder.Append(Microsoft.VisualBasic.Chr(Character))
        End If
        Me.Characters.Enqueue(Character)
    End Sub

End Class
