Imports NAnt.Core
Imports NAnt.Core.Attributes
Imports System.IO
Imports System



<TaskName("applyfilter")> _
Public Class ApplyFilterTask
    Inherits Task

    Private _Text As TextElement
    Private _Filter As Filters.FilterChain
    Private _OutProperty As String

    <TaskAttribute("propertyname", Required:=True)> _
    Public Property OutProperty() As String
        Get
            Return _OutProperty
        End Get
        Set(ByVal Value As String)
            _OutProperty = Value
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

    <BuildElement("filterchain", Required:=True)> _
    Public Property Filter() As Filters.FilterChain
        Get
            Return _Filter
        End Get
        Set(ByVal Value As Filters.FilterChain)
            _Filter = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Me.WriteWithFilters()
    End Sub

    Private Function GetWriter() As TextWriter
        Return New StringWriter()
    End Function

    Private Sub WriteWithFilters()
        Dim Writer As TextWriter
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
            Me.Properties(Me.OutProperty) = Writer.ToString()
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

End Class
