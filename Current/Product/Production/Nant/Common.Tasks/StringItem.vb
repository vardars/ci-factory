Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes
Imports System

<ElementName("string")> _
Public Class StringItem
    Inherits Element
    Implements IComparable

    Private _StringValue As String
    Private _index As Integer

    Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal Value As Integer)
            _index = Value
        End Set
    End Property

    <TaskAttribute("value", Required:=True)> _
    Public Property StringValue() As String
        Get
            Return _StringValue
        End Get
        Set(ByVal value As String)
            _StringValue = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the StringItem class.
    ''' </summary>
    ''' <param name="stringValue"></param>
    Public Sub New(ByVal stringValue As String)
        _StringValue = stringValue
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the StringItem class.
    ''' </summary>
    Public Sub New()
    End Sub

    Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
        If obj Is Nothing Then
            Return -1
        End If

        If Not obj Is GetType(StringItem) Then
            Return -1
        End If

        Return Me.StringValue.CompareTo(DirectCast(obj, StringItem).StringValue)
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If

        If Not obj Is GetType(StringItem) Then
            Return False
        End If

        Return Me.StringValue.Equals(DirectCast(obj, StringItem).StringValue)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.StringValue.GetHashCode()
    End Function

End Class
