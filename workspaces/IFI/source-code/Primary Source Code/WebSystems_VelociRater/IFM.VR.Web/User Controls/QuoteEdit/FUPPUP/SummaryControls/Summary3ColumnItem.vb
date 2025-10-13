Public Class Summary3ColumnItem
    Private _Coverage As String
    Public Property coverage() As String
        Get
            Return _Coverage
        End Get
        Set(ByVal value As String)
            _Coverage = value
        End Set
    End Property

    Private _Limit As String
    Public Property limit() As String
        Get
            Return _Limit
        End Get
        Set(ByVal value As String)
            _Limit = value
        End Set
    End Property

    Private _Premium As String
    Public Property premium() As String
        Get
            Return _Premium
        End Get
        Set(ByVal value As String)
            _Premium = value
        End Set
    End Property

    Public Property extra1 As String
    Public Property extra2 As String
    Public Property extra3 As String
    Public Property extra4 As String

    Public ReadOnly Property showExtraRow As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(If(If(extra1, extra2), If(extra3, extra4)))
        End Get
    End Property

    Public Sub New(coverageinput As String, limitinput As String, premiuminput As String)
        _Coverage = coverageinput
        _Limit = limitinput
        _Premium = premiuminput
    End Sub

End Class
