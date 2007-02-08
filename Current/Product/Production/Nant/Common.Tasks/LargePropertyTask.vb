Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

Imports System.IO

<TaskName("largeproperty")> _
Public Class LargePropertyTask
    Inherits Task

    Private _PropertyNameName As String
    Private _TextValue As TextElement

    <TaskAttribute("name", Required:=True)> _
    Public Property PropertyName() As String
        Get
            Return _PropertyNameName
        End Get
        Set(ByVal value As String)
            If _PropertyNameName = value Then
                Return
            End If
            _PropertyNameName = value
        End Set
    End Property

    <BuildElement("value", Required:=True)> _
    Public Property TextValue() As TextElement
        Get
            Return _TextValue
        End Get
        Set(ByVal value As TextElement)
            _TextValue = value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Me.Properties(Me.PropertyName) = Me.TextValue.Value
    End Sub

End Class
