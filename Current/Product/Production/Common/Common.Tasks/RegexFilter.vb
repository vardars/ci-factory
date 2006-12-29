Option Explicit On
Option Strict On

Imports NAnt.Core
Imports NAnt.Core.Filters
Imports NAnt.Core.Attributes

<ElementName("regexreplace")> _
Public Class RegexFilter
    Inherits Filter

    Public Overrides Sub Chain(ByVal parentChainedReader As NAnt.Core.Filters.ChainableReader)
        MyBase.Chain(parentChainedReader)
    End Sub

    Public Overrides Function Peek() As Integer

    End Function

    Public Overrides Function Read() As Integer

    End Function

End Class
