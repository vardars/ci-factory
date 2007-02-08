Option Explicit On 
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

<ElementName("text")> _
Public Class TextElement
    Inherits Element

    Private _Value As String
    Private _Xml As Boolean = False
    Private _Expand As Boolean = False

    <TaskAttribute("xml", Required:=False), BooleanValidator()> _
    Public Property Xml() As Boolean
        Get
            Return _Xml
        End Get
        Set(ByVal value As Boolean)
            _Xml = Value
        End Set
    End Property

    <TaskAttribute("expand", Required:=False), BooleanValidator()> _
    Public Property Expand() As Boolean
        Get
            Return _Expand
        End Get
        Set(ByVal value As Boolean)
            _Expand = value
        End Set
    End Property

    Public ReadOnly Property [Value]() As String
        Get
            If Me._Value Is Nothing Then
                If Me.Xml Then
                    Me._Value = Me.XmlNode.InnerXml
                Else
                    Me._Value = Me.XmlNode.InnerText
                End If
                If Me.Expand Then
                    Me._Value = Me.Project.ExpandProperties(Me._Value, Me.Location)
                End If
            End If
            Return Me._Value
        End Get
    End Property

    Protected Overrides ReadOnly Property CustomXmlProcessing() As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitializeXml(ByVal elementNode As System.Xml.XmlNode, ByVal properties As NAnt.Core.PropertyDictionary, ByVal framework As NAnt.Core.FrameworkInfo)
        MyBase.InitializeXml(elementNode, properties, framework)
    End Sub

End Class
