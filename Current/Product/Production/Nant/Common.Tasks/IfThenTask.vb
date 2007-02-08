Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes

<TaskName("ifthenelse")> _
Public Class IfThenTask
    Inherits IfTask

    Private _Then As TaskContainer
    Private _Else As TaskContainer
    Private _ElseIf As TaskContainerCollection

    <BuildElement("then", Required:=True)> _
        Public Property [Then]() As TaskContainer
        Get
            Return _Then
        End Get
        Set(ByVal value As TaskContainer)
            _Then = value
        End Set
    End Property

    <BuildElementArray("elseif", Required:=False)> _
        Public Property [ElseIf]() As TaskContainerCollection
        Get
            If _ElseIf Is Nothing Then
                _ElseIf = New TaskContainerCollection()
            End If
            Return _ElseIf
        End Get
        Set(ByVal value As TaskContainerCollection)
            _ElseIf = value
        End Set
    End Property

    <BuildElement("else", Required:=False)> _
        Public Property [Else]() As TaskContainer
        Get
            Return _Else
        End Get
        Set(ByVal value As TaskContainer)
            _Else = value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        If (Me.ConditionsTrue) Then
            Me.Then.Execute()
        Else
            Dim Executed As Boolean = False
            For Each Possible As TaskContainer In Me.ElseIf
                If Possible.IfDefined Then
                    Possible.Execute()
                    Executed = True
                End If
            Next
            If Not Executed And Not Me.Else Is Nothing Then
                Me.Else.Execute()
            End If
        End If
    End Sub

End Class

Public Class TaskContainerCollection
    Inherits CollectionBase

    Public Sub New()

    End Sub

    Public Sub New(ByVal value As TaskContainerCollection)
        Me.InnerList.AddRange(value)
    End Sub

    Public Sub New(ByVal value As TaskContainer())
        Me.InnerList.AddRange(value)
    End Sub

    Public Function Add(ByVal item As TaskContainer) As Integer
        Return MyBase.List.Add(item)
    End Function

    Public Sub AddRange(ByVal items As TaskContainer())
        Me.InnerList.AddRange(items)
    End Sub

    Public Sub AddRange(ByVal items As TaskContainerCollection)
        Me.InnerList.AddRange(items)
    End Sub
    Public Function Contains(ByVal item As TaskContainer) As Boolean
        Return Me.InnerList.Contains(item)
    End Function
    Public Sub CopyTo(ByVal array As TaskContainer(), ByVal index As Integer)
        Me.InnerList.CopyTo(array, index)
    End Sub
    Public Function IndexOf(ByVal item As TaskContainer) As Integer
        Return Me.InnerList.IndexOf(item)
    End Function
    Public Sub Insert(ByVal index As Integer, ByVal item As TaskContainer)
        Me.InnerList.Insert(index, item)
    End Sub
    Public Sub Remove(ByVal item As TaskContainer)
        Me.InnerList.Remove(item)
    End Sub

    Public Property Item(ByVal index As Integer) As TaskContainer
        Get
            Return DirectCast(Me.InnerList(index), TaskContainer)
        End Get
        Set(ByVal value As TaskContainer)
            Me.InnerList(index) = value
        End Set
    End Property


End Class
