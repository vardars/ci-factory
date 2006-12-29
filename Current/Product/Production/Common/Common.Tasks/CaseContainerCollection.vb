Option Explicit On
Option Strict On

Public Class CaseContainerCollection
    Inherits CollectionBase

    Public Sub New()

    End Sub

    Public Sub New(ByVal value As CaseContainerCollection)
        Me.InnerList.AddRange(value)
    End Sub

    Public Sub New(ByVal value As CaseElement())
        Me.InnerList.AddRange(value)
    End Sub

    Public Function Add(ByVal item As CaseElement) As Integer
        Return MyBase.List.Add(item)
    End Function

    Public Sub AddRange(ByVal items As CaseElement())
        Me.InnerList.AddRange(items)
    End Sub

    Public Sub AddRange(ByVal items As CaseContainerCollection)
        Me.InnerList.AddRange(items)
    End Sub
    Public Function Contains(ByVal item As CaseElement) As Boolean
        Return Me.InnerList.Contains(item)
    End Function
    Public Sub CopyTo(ByVal array As CaseElement(), ByVal index As Integer)
        Me.InnerList.CopyTo(array, index)
    End Sub
    Public Function IndexOf(ByVal item As CaseElement) As Integer
        Return Me.InnerList.IndexOf(item)
    End Function
    Public Sub Insert(ByVal index As Integer, ByVal item As CaseElement)
        Me.InnerList.Insert(index, item)
    End Sub
    Public Sub Remove(ByVal item As CaseElement)
        Me.InnerList.Remove(item)
    End Sub

    Public Property Item(ByVal index As Integer) As CaseElement
        Get
            Return DirectCast(Me.InnerList(index), CaseElement)
        End Get
        Set(ByVal value As CaseElement)
            Me.InnerList(index) = value
        End Set
    End Property


End Class

