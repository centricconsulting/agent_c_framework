Public Class DiamondSecurityQuestion
    Implements IDisposable

#Region "Var"
    Private _QuestionText As String
    Private _QuestionID As Integer
    Private _AnswerText As String
#End Region

    Public Sub New(ByVal securityQuestion As DCO.Administration.UserSecurityQuestion)
        QuestionText = securityQuestion.Question
        QuestionID = securityQuestion.UserSecurityQuestionID
        AnswerText = String.Empty
    End Sub

    Public Sub New(ByVal securityQuestion As DCO.Administration.UsersUserSecurityQuestionLink)
        QuestionID = securityQuestion.UserSecurityQuestionID
        QuestionText = String.Empty
        AnswerText = securityQuestion.Answer
    End Sub

    Public Sub New(ByVal securityQuestion As DCO.Administration.UsersUserSecurityQuestionLinkForLoginName)
        QuestionID = securityQuestion.UserSecurityQuestionID
        QuestionText = securityQuestion.Question
        AnswerText = securityQuestion.Answer
    End Sub

    Public Sub New(ByVal qID As Integer, ByVal qText As String, ByVal aText As String)
        QuestionID = qID
        QuestionText = qText
        AnswerText = aText
    End Sub

#Region "Props"

    Public Property QuestionText() As String
        Get
            Return _QuestionText
        End Get
        Set(ByVal value As String)
            _QuestionText = value
        End Set
    End Property

    Public Property QuestionID() As Integer
        Get
            Return _QuestionID
        End Get
        Set(ByVal value As Integer)
            _QuestionID = value
        End Set
    End Property

    Public Property AnswerText() As String
        Get
            Return _AnswerText
        End Get
        Set(ByVal value As String)
            _AnswerText = value
        End Set
    End Property

#End Region

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If QuestionID > 0 Then QuestionID = 0
                If QuestionText IsNot Nothing Then QuestionText = Nothing
                If AnswerText IsNot Nothing Then AnswerText = Nothing
            End If
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
