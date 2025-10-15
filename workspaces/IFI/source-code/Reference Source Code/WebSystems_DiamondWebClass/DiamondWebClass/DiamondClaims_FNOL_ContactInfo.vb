Public Class DiamondClaims_FNOL_ContactInfo 'added 4/29/2013
    Implements IDisposable

    Private _HomePhone As String
    Private _BusinessPhone As String
    Private _CellPhone As String
    Private _FaxPhone As String

    Private _OtherEmail As String
    Private _HomeEmail As String
    Private _BusinessEmail As String

    Public Property HomePhone() As String
        Get
            Return _HomePhone
        End Get
        Set(ByVal value As String)
            _HomePhone = value
        End Set
    End Property
    Public Property BusinessPhone() As String
        Get
            Return _BusinessPhone
        End Get
        Set(ByVal value As String)
            _BusinessPhone = value
        End Set
    End Property
    Public Property CellPhone() As String
        Get
            Return _CellPhone
        End Get
        Set(ByVal value As String)
            _CellPhone = value
        End Set
    End Property
    Public Property FaxPhone As String
        Get
            Return _FaxPhone
        End Get
        Set(value As String)
            _FaxPhone = value
        End Set
    End Property

    Public Property OtherEmail() As String
        Get
            Return _OtherEmail
        End Get
        Set(ByVal value As String)
            _OtherEmail = value
        End Set
    End Property
    Public Property HomeEmail() As String
        Get
            Return _HomeEmail
        End Get
        Set(ByVal value As String)
            _HomeEmail = value
        End Set
    End Property
    Public Property BusinessEmail() As String
        Get
            Return _BusinessEmail
        End Get
        Set(ByVal value As String)
            _BusinessEmail = value
        End Set
    End Property

    Public Sub New()
        setDefaults()
    End Sub

    Public Overridable Sub setDefaults()
        _HomePhone = String.Empty
        _BusinessPhone = String.Empty
        _CellPhone = String.Empty
        _FaxPhone = String.Empty

        _OtherEmail = String.Empty
        _HomeEmail = String.Empty
        _BusinessEmail = String.Empty
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _HomePhone IsNot Nothing Then
                    _HomePhone = Nothing
                End If
                If _BusinessPhone IsNot Nothing Then
                    _BusinessPhone = Nothing
                End If
                If _CellPhone IsNot Nothing Then
                    _CellPhone = Nothing
                End If
                If _FaxPhone IsNot Nothing Then
                    _FaxPhone = Nothing
                End If

                If _OtherEmail IsNot Nothing Then
                    _OtherEmail = Nothing
                End If
                If _HomeEmail IsNot Nothing Then
                    _HomeEmail = Nothing
                End If
                If _BusinessEmail IsNot Nothing Then
                    _BusinessEmail = Nothing
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
