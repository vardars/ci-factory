Option Explicit On
Option Strict On

Imports System.Threading
Imports System.Collections
Imports System.IO
Imports System.Text

Imports NAnt.Core
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes
Imports System

<TaskName("asyncexec")> _
Public Class AsyncExec
    Inherits ExecTask

    Private _taskName As String
    Private _OutputWriter As TextWriter
    Private _ErrorWriter As TextWriter

    <TaskAttribute("taskname", required:=True)> _
    Public Property TaskName() As String
        Get
            Return _taskName
        End Get
        Set(ByVal Value As String)
            _taskName = Value
        End Set
    End Property

    Public Overrides Property OutputWriter() As System.IO.TextWriter
        Get
            If _OutputWriter Is Nothing Then
                _OutputWriter = TextWriter.Null
            End If
            Return _OutputWriter
        End Get
        Set(ByVal value As System.IO.TextWriter)

        End Set
    End Property

    Public Overrides Property ErrorWriter() As System.IO.TextWriter
        Get
            If _ErrorWriter Is Nothing Then
                _ErrorWriter = TextWriter.Null
            End If
            Return _ErrorWriter
        End Get
        Set(ByVal value As System.IO.TextWriter)

        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Dim WorkerThread As New Thread(New ThreadStart(AddressOf MyBase.ExecuteTask))
        AsyncExecList.Add(Me.TaskName, WorkerThread)
        WorkerThread.Start()
    End Sub

End Class
