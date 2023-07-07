Namespace Models

    Public Class MethodResult(Of T)
        Public Property Success As Boolean = False
        Public Property Data As T
        Public Property ErrorMessage As String
    End Class

End Namespace