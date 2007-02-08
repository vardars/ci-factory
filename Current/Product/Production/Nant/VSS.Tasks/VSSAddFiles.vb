Option Explicit On 
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Types
Imports NAnt.Core.Attributes
Imports NAnt.Contrib.Tasks.SourceSafe

<TaskName("vssaddfiles")> _
Public Class VSSAddFiles
    Inherits BaseTask

    Private _Comment As String
    Private _FileSet As FileSet

    <BuildElement("fileset")> _
    Public Property FileSet() As FileSet
        Get
            Return Me._FileSet
        End Get
        Set(ByVal Value As FileSet)
            Me._FileSet = Value
        End Set
    End Property

    <TaskAttributeAttribute("comment", Required:=False)> _
    Public Property Comment() As String
        Get
            Return _Comment
        End Get
        Set(ByVal Value As String)
            _Comment = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        
    End Sub

End Class
