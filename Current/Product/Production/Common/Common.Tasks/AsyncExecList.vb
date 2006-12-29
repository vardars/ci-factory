Imports System.Threading
Imports System.Collections
Imports System.IO
Imports System.Text
Imports NAnt.Core
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes
Imports System


Public Class AsyncExecList

    Private Shared _taskNames As Hashtable

    Private Shared Property TaskNames() As Hashtable
        Get
            If _taskNames Is Nothing Then
                _taskNames = New Hashtable()
            End If
            Return _taskNames
        End Get
        Set(ByVal Value As Hashtable)
            _taskNames = Value
        End Set
    End Property

    Shared Sub New()

    End Sub

    Public Shared Sub Add(ByVal name As String, ByVal task As Thread)
        TaskNames.Add(name, task)
    End Sub

    Public Shared Sub Remove(ByVal name As String)
        TaskNames.Remove(name)
    End Sub

    Public Shared ReadOnly Property Item(ByVal name As String) As Thread
        Get
            Return DirectCast(TaskNames(name), Thread)
        End Get
    End Property

End Class
