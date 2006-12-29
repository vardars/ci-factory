Option Explicit On 
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

Imports System.IO

<TaskName("write")> _
Public Class WriteTask
    Inherits Task

    Private _OutFile As FileInfo
    Private _Text As TextElement
    Private _Filter As Filters.FilterChain
    Private _Append As Boolean = False

    <TaskAttributeAttribute("append", Required:=False), BooleanValidator()> _
    Public Property Append() As Boolean
        Get
            Return _Append
        End Get
        Set(ByVal Value As Boolean)
            _Append = Value
        End Set
    End Property

    <TaskAttributeAttribute("file", Required:=True)> _
    Public Property OutFile() As FileInfo
        Get
            Return _OutFile
        End Get
        Set(ByVal Value As FileInfo)
            _OutFile = Value
        End Set
    End Property

    <BuildElement("text", Required:=True)> _
    Public Property Text() As TextElement
        Get
            Return _Text
        End Get
        Set(ByVal Value As TextElement)
            _Text = Value
        End Set
    End Property

    <BuildElement("filterchain", Required:=False)> _
    Public Property Filter() As Filters.FilterChain
        Get
            Return _Filter
        End Get
        Set(ByVal Value As Filters.FilterChain)
            _Filter = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        If Me.Filter Is Nothing OrElse Me.Filter.Filters Is Nothing OrElse Me.Filter.Filters.Count = 0 Then
            Me.Write()
        Else
            Me.WriteWithFilters()
        End If
    End Sub

    Private Function GetWriter() As StreamWriter
        If Me.Append Then
            Return Me.OutFile.AppendText
        Else
            Return Me.OutFile.CreateText
        End If
    End Function

    Private Sub WriteWithFilters()
        Dim Writer As StreamWriter
        Dim Reader As StringReader
        Dim ChainedReader As Filters.PhysicalTextReader
        Try
            Writer = Me.GetWriter
            Reader = New StringReader(Me.Text.Value)
            ChainedReader = New Filters.PhysicalTextReader(Reader)
            Dim FilterReader As Filters.Filter = Me.Filter.GetBaseFilter(ChainedReader)
            Do While Not False
                Dim Focus As Integer = FilterReader.Read
                If Not (Focus > -1) Then
                    Exit Do
                End If
                Writer.Write(Microsoft.VisualBasic.ChrW(Focus))
            Loop
        Finally
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
            If Not ChainedReader Is Nothing Then
                ChainedReader.Close()
            End If
            If Not Reader Is Nothing Then
                Reader.Close()
            End If
        End Try
    End Sub

    Private Sub Write()
        Dim Writer As StreamWriter
        Try
            Writer = Me.GetWriter
            Writer.Write(Me.Text.Value)
        Finally
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
        End Try
    End Sub

End Class