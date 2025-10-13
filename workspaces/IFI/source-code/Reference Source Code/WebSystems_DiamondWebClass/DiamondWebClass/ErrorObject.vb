Public Class ErrorObject
#Region "Var"
    Private _TechnicalErrorMsg As String
    Private _FriendlyErrorMsg As String
    Private _ErrorLevel As Enums.ErrorLevel
    Private _ErrorSystem As Enums.UserLocation
#End Region

    Public Sub New(ByVal errTech As String, ByVal errFriend As String, ByVal errLevel As Enums.ErrorLevel, ByVal errSys As Enums.UserLocation)
        _TechnicalErrorMsg = errTech
        _FriendlyErrorMsg = errFriend
        _ErrorLevel = errLevel
        _ErrorSystem = errSys
    End Sub

#Region "Prop"

    Public ReadOnly Property TechnicalErrorMsg() As String
        Get
            Return _TechnicalErrorMsg
        End Get
    End Property

    Public ReadOnly Property FriendlyErrorMsg() As String
        Get
            Return _FriendlyErrorMsg
        End Get
    End Property

    Public ReadOnly Property ErrorLevel() As Enums.ErrorLevel
        Get
            Return _ErrorLevel
        End Get
    End Property

    Public ReadOnly Property ErrorSystem() As Enums.UserLocation
        Get
            Return _ErrorSystem
        End Get
    End Property

#End Region
End Class
