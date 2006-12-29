Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

Public Class SaveProperty
    Inherits Element

    Private _PropertyName As String
    Private _PropertyValue As String

    Public ReadOnly Property PropertyValue() As String
        Get
            If Not Me._PropertyValue Is Nothing Then
                Return Me._PropertyValue
            End If
            Return Me.Properties(Me.PropertyName)
        End Get
    End Property

    <TaskAttribute("name", Required:=True)> _
    Public Property PropertyName() As String
        Get
            Return _PropertyName
        End Get
        Set(ByVal value As String)
            _PropertyName = value
        End Set
    End Property

    <TaskAttribute("value", Required:=False)> _
    Public WriteOnly Property SetProperty() As String
        Set(ByVal value As String)
            Me._PropertyValue = value
        End Set
    End Property

End Class
