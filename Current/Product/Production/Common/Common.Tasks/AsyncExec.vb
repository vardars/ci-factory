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
        Dim WorkerThread As New Thread(New ThreadStart(AddressOf RunProcess))
        WorkerThread.IsBackground = True

        If Not Me.TaskName = String.Empty AndAlso Me.WaitForExit = False Then
            Log(Level.Warning, "You set the attribute taskname to {0} and waitforexit to false.  You will not be able to call the waitforexit task with the task name {0} with an error.  If you wanted to wait for this to exit please set waitforexit to true.", Me.TaskName)
        End If

        If Not Me.TaskName = String.Empty AndAlso Me.WaitForExit Then
            AsyncExecList.Add(Me.TaskName, WorkerThread)
        End If

        WorkerThread.Start()
    End Sub

    Private Sub RunProcess()
        Dim process1 As Process
        Try
            process1 = Me.StartProcess

            If Me.WaitForExit Then
                process1.WaitForExit(Me.TimeOut)

                If Not process1.HasExited Then
                    Try
                        process1.Kill()
                    Catch
                    End Try
                    Throw New BuildException(String.Format(CultureInfo.InvariantCulture, ResourceUtils.GetString("NA1118"), New Object() {Me.ProgramFileName, Me.TimeOut}), Me.Location)
                End If

                If (process1.ExitCode <> 0) Then
                    Throw New BuildException(String.Format(CultureInfo.InvariantCulture, ResourceUtils.GetString("NA1119"), New Object() {Me.ProgramFileName, process1.ExitCode}), Me.Location)
                End If
            End If
        Catch exception1 As BuildException
            If MyBase.FailOnError Then
                Throw
            End If

            Me.Log(Level.Error, exception1.Message)
        Finally
            If (Not Me.ResultProperty Is Nothing AndAlso Me.WaitForExit) Then
                Me.Properties.Item(Me.ResultProperty) = process1.ExitCode.ToString(CultureInfo.InvariantCulture)
            End If
        End Try
    End Sub



End Class
