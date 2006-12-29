Option Explicit On
Option Strict On

Imports System.IO
Imports System.Xml
Imports NAnt.Core
Imports NAnt.Core.Types
Imports NAnt.Core.Attributes

<ElementName("xmlquery")> _
Public Class XmlQuery
    Inherits LoopItems

    Private _Query As String
    Private _File As String
    Private _Namespaces As XmlNamespaceCollection


    <BuildElementCollection("namespaces", "namespace")> _
    Public Property Namespaces() As XmlNamespaceCollection
        Get
            Return Me._Namespaces
        End Get
        Set(ByVal value As XmlNamespaceCollection)
            Me._Namespaces = value
        End Set
    End Property

    <TaskAttribute("query", Required:=True)> _
    Public Property Query() As String
        Get
            Return _Query
        End Get
        Set(ByVal value As String)
            _Query = value
        End Set
    End Property

    <TaskAttribute("file", Required:=True)> _
    Public Property File() As String
        Get
            Return _File
        End Get
        Set(ByVal value As String)
            _File = value
        End Set
    End Property

    Protected Overrides Function GetStrings() As System.Collections.IEnumerator
        Dim Strings As New ArrayList
        Dim XmlDoc As New XmlDocument
        XmlDoc.Load(Me.File)

        Dim Manager As New XmlNamespaceManager(XmlDoc.NameTable)
        Dim [NameSpace] As XmlNamespace
        For Each [NameSpace] In Me.Namespaces
            If ([NameSpace].IfDefined AndAlso Not [NameSpace].UnlessDefined) Then
                Manager.AddNamespace([NameSpace].Prefix, [NameSpace].Uri)
            End If
        Next

        For Each Node As XmlNode In XmlDoc.SelectNodes(Me.Query, Manager)
            Strings.Add(Node.InnerXml)
        Next

        Return DirectCast(Strings.ToArray(GetType(String)), String()).GetEnumerator
    End Function

End Class
