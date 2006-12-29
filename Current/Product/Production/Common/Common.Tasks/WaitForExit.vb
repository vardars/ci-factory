Imports System.Threading
Imports System.Collections
Imports System.IO
Imports System.Text
Imports NAnt.Core
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes
Imports System



<TaskName("waitforexit")> _
Public Class WaitForExit
    Inherits Task

    Private _taskNames As StringList

    <BuildElement("tasknames", Required:=True)> _
    Public Property TaskNames() As StringList
        Get
            Return _taskNames
        End Get
        Set(ByVal Value As StringList)
            _taskNames = Value
        End Set
    End Property

    Protected Overrides Sub ExecuteTask()
        Dim WorkerThread As Thread
        For Each TaskName As String In Me.TaskNames.StringItems.Values
            WorkerThread = AsyncExecList.Item(TaskName)
            If Not WorkerThread Is Nothing AndAlso WorkerThread.IsAlive Then
                WorkerThread.Join()
            End If
        Next
    End Sub
End Class

