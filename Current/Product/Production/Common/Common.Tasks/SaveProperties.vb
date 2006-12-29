Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

Imports System.IO

<TaskName("saveproperties")> _
Public Class SaveProperties
    Inherits Task

    Public Enum FileFormat
        Include
        CommandLine
    End Enum

    Private _Format As FileFormat
    Private _File As FileInfo
    Private _PropertyList As SaveProperty()
    Private _Append As Boolean = False
    Private _ProjectName As String = "Bogas"

    <TaskAttribute("projectname", required:=False)> _
    Public Property ProjectName() As String
        Get
            Return _ProjectName
        End Get
        Set(ByVal value As String)
            _ProjectName = value
        End Set
    End Property

    <TaskAttribute("append", Required:=False), BooleanValidator()> _
    Public Property Append() As Boolean
        Get
            Return _Append
        End Get
        Set(ByVal value As Boolean)
            _Append = value
        End Set
    End Property

    <BuildElementArray("property", Required:=True)> _
    Public Property PropertyList() As SaveProperty()
        Get
            Return _PropertyList
        End Get
        Set(ByVal value As SaveProperty())
            _PropertyList = value
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

    <TaskAttribute("format", Required:=True)> _
    Public Property Format() As FileFormat
        Get
            Return _Format
        End Get
        Set(ByVal value As FileFormat)
            _Format = value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Select Case Me.Format
            Case FileFormat.CommandLine
                Me.CreateCommandLineFile()
            Case FileFormat.Include
                Me.CreateIncludeFile()
            Case Else
                Throw New BuildException(String.Format("Format {0} is not supported.", Me.Format.ToString))
        End Select
    End Sub

    Public Sub CreateIncludeFile()
        Dim Writer As StreamWriter
        Try
            Writer = Me.GetWriter
            Writer.WriteLine("<?xml version='1.0' encoding='utf-8' ?>")
            Writer.WriteLine(String.Format("<project xmlns='http://nant.sf.net/schemas/nant.xsd'  name='{0}'>", Me.ProjectName))
            For Each [Property] As SaveProperty In Me.PropertyList
                Writer.WriteLine(String.Format("<property name='{0}' value='{1}' />", [Property].PropertyName, [Property].PropertyValue))
            Next
            Writer.WriteLine("</project>")
        Finally
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
        End Try
    End Sub

    Public Sub CreateCommandLineFile()
        Dim Writer As StreamWriter
        Try
            Writer = Me.GetWriter
            For Each [Property] As SaveProperty In Me.PropertyList
                Writer.WriteLine(String.Format("-D:{0}=""{1}""", [Property].PropertyName, [Property].PropertyValue))
            Next
        Finally
            If Not Writer Is Nothing Then
                Writer.Close()
            End If
        End Try
    End Sub

    Private Function GetWriter() As StreamWriter
        If Me.Append Then
            Return Me.File.AppendText
        Else
            Return Me.File.CreateText
        End If
    End Function

End Class
