Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes


<TaskName("loopthrough")> _
Public Class LoopThrough
    Inherits Task

    Private _Items As LoopItemContainer
    Private _Actions As TaskContainer
    Private _PropertyName As String

    <TaskAttribute("property", Required:=True)> _
    Public Property PropertyName() As String
        Get
            Return _PropertyName
        End Get
        Set(ByVal value As String)
            _PropertyName = value
        End Set
    End Property

    <BuildElement("items", Required:=True)> _
    Public Property Items() As LoopItemContainer
        Get
            Return _Items
        End Get
        Set(ByVal value As LoopItemContainer)
            _Items = value
        End Set
    End Property

    <BuildElement("do", Required:=True)> _
    Public Property Actions() As TaskContainer
        Get
            Return _Actions
        End Get
        Set(ByVal value As TaskContainer)
            _Actions = value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        For Each Item As String In Me.Items
            If Me.Properties.Contains(Me.PropertyName) Then
                Me.Properties(Me.PropertyName) = Item
            Else
                Me.Properties.Add(Me.PropertyName, Item)
            End If
            Me.Items.Items.Executing(Item)
            Me.Actions.Execute()
        Next
    End Sub

End Class
