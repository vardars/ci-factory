Option Explicit On
Option Strict On

Imports System.IO
Imports NAnt.Core
Imports NAnt.Core.Attributes

<TaskName("replace")> _
Public Class Replace
    Inherits Task

    Private _File As FileInfo
    Private _Filter As Filters.FilterChain


    <BuildElement("filterchain", Required:=True)> _
    Public Property Filter() As Filters.FilterChain
        Get
            Return _Filter
        End Get
        Set(ByVal Value As Filters.FilterChain)
            _Filter = Value
        End Set
    End Property


    <TaskAttributeAttribute("file", Required:=True)> _
    Public Property File() As FileInfo
        Get
            Return _File
        End Get
        Set(ByVal Value As FileInfo)
            _File = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Me.WriteWithFilters()
    End Sub

    Private Sub WriteWithFilters()
        Dim FileReader As StreamReader
        Dim Writer As StreamWriter
        Dim Reader As StringReader
        Dim ChainedReader As Filters.PhysicalTextReader
        Try
            FileReader = Me.File.OpenText
            Reader = New StringReader(FileReader.ReadToEnd)
            FileReader.Close()
            FileReader = Nothing
            Writer = Me.File.CreateText
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
            If Not FileReader Is Nothing Then
                FileReader.Close()
            End If
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

End Class
