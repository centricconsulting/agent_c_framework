Class DiaException
    Inherits Exception
    Public Sub New(ByVal errMsg As String, ByVal errLocation As String)
        m_errMsg = errMsg
        m_errLocation = errLocation
    End Sub 'New


    Private m_errMsg As String
    Public ReadOnly Property pr_errMsg() As String
        Get
            Return m_errMsg
        End Get
    End Property


    Private m_errLocation As String
    Public ReadOnly Property pr_errLocation() As String
        Get
            Return m_errLocation
        End Get
    End Property


End Class 'MyException
