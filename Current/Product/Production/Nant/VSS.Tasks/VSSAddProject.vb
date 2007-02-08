Option Explicit On 
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes
Imports NAnt.Contrib.Tasks.SourceSafe

<TaskName("vssaddproject")> _
Public Class VSSAddProject
    Inherits BaseTask

    Private _ProjectName As String
    Private _Comment As String

    <TaskAttributeAttribute("comment", Required:=False)> _
    Public Property Comment() As String
        Get
            Return _Comment
        End Get
        Set(ByVal Value As String)
            _Comment = Value
        End Set
    End Property

    <TaskAttributeAttribute("project", Required:=True)> _
    Public Property ProjectName() As String
        Get
            Return _ProjectName
        End Get
        Set(ByVal Value As String)
            _ProjectName = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        MyBase.Open()
        MyBase.Item.NewSubproject(Me.ProjectName, Me.Comment)
    End Sub

    Public Sub test()
        Try
            Me.Comment = "Testing"
            Me.DBPath = New IO.FileInfo("C:\Source Safe DataBases\Test\srcsafe.ini")
            Me.Password = "password"
            Me.UserName = "build"
            Me.Path = "$/"
            Me.ProjectName = "Test"
            Me.ExecuteTask()
        Catch ex As Exception
            Stop
        End Try

    End Sub

End Class