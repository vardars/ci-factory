Option Explicit On
Option Strict On

Imports System.Threading
Imports System.Collections
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Diagnostics

Imports NAnt.Core
Imports NAnt.Core.Util
Imports NAnt.Core.Tasks
Imports NAnt.Core.Attributes
Imports System

<TaskName("asyncexec")> _
Public Class AsyncExec
    Inherits ExecTask

    Private _taskName As String = String.Empty
    Private _OutputWriter As TextWriter
    Private _ErrorWriter As TextWriter
    Private _waitForExit As Boolean = True
    Private _process As Process

    Private Property process() As Process
        Get
            Return _process
        End Get
        Set(ByVal value As Process)
            _process = value
        End Set
    End Property

    Public Overrides Property Output() As FileInfo
        Get
            Return Nothing
        End Get
        Set(ByVal value As FileInfo)
            Log(Level.Warning, "The output attribute is not used for the asyncexec task.  Please do something like pipe the output to a file.")
        End Set
    End Property


    <TaskAttribute("waitforexit", required:=False)> _
    Public Property WaitForExit() As Boolean
        Get
            Return _waitForExit
        End Get
        Set(ByVal Value As Boolean)
            _waitForExit = Value
        End Set
    End Property

    <TaskAttribute("taskname", required:=False)> _
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
        Me.Process = Me.StartProcess

        If Not Me.TaskName = String.Empty AndAlso Me.WaitForExit = False Then
            Log(Level.Warning, "You set the attribute taskname to {0} and waitforexit to false.  You will not be able to call the waitforexit task with the task name {0} with an error.  If you wanted to wait for this to exit please set waitforexit to true.", Me.TaskName)
        End If

        If Not Me.TaskName = String.Empty AndAlso Me.WaitForExit Then
            AsyncExecList.Add(Me.TaskName, Me)
        End If

        If Me.TaskName = String.Empty AndAlso Me.WaitForExit Then
            Me.Wait()
        End If
    End Sub

    Public Sub Wait()
        Try
            Me.Process.WaitForExit(Me.TimeOut)

            If Not Me.Process.HasExited Then
                Try
                    Me.Process.Kill()
                Catch
                End Try
                Throw New BuildException(String.Format("External Program {0} did not finish within {1} milliseconds.", New Object() {Me.ProgramFileName, Me.TimeOut}), Me.Location)
            End If

            If (Me.Process.ExitCode <> 0) Then
                Throw New BuildException(String.Format("External Program Failed: {0} (return code was {1})", New Object() {Me.ProgramFileName, Me.Process.ExitCode}), Me.Location)
            End If
        Catch exception1 As BuildException
            If MyBase.FailOnError Then
                Throw
            End If

            Me.Log(Level.Error, exception1.Message)
        Finally
            If (Not Me.ResultProperty Is Nothing AndAlso Me.WaitForExit) Then
                Me.Properties.Item(Me.ResultProperty) = Me.Process.ExitCode.ToString()
            End If
        End Try
    End Sub



End Class
