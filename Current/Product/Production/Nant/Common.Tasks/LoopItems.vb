Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Attributes

Public MustInherit Class LoopItems
    Inherits DataTypeBase
    Implements IEnumerable

    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me.GetStrings
    End Function

    Protected MustOverride Function GetStrings() As IEnumerator

    Public Overridable Sub Executing(ByVal item As String)

    End Sub
End Class
