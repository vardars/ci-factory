Option Explicit On
Option Strict On

Imports System.Xml
Imports System.Reflection

Imports NAnt.Core
Imports NAnt.Core.Types
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes

<ElementName("case")> _
Public Class CaseElement
    Inherits ElementTaskContainer

    Private _RightValue As String
    Private _Break As Boolean

    <TaskAttribute("break"), BooleanValidator()> _
    Public Property Break() As Boolean
        Get
            Return _Break
        End Get
        Set(ByVal value As Boolean)
            _Break = value
        End Set
    End Property

    <TaskAttribute("value", Required:=True)> _
    Public Property RightValue() As String
        Get
            Return _RightValue
        End Get
        Set(ByVal value As String)
            _RightValue = value
        End Set
    End Property

End Class
