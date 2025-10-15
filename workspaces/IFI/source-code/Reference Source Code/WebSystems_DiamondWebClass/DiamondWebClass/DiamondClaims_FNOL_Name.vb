Public Class DiamondClaims_FNOL_Name 'added 4/29/2013
    Implements IDisposable

    Private _FirstName As String
    Private _MiddleName As String
    Private _LastName As String
    Private _CommercialName As String
    Private _DbaName As String

    Public Property FirstName() As String
        Get
            Return _FirstName
        End Get
        Set(ByVal value As String)
            _FirstName = value
        End Set
    End Property
    Public Property MiddleName() As String
        Get
            Return _MiddleName
        End Get
        Set(ByVal value As String)
            _MiddleName = value
        End Set
    End Property
    Public Property LastName() As String
        Get
            Return _LastName
        End Get
        Set(ByVal value As String)
            _LastName = value
        End Set
    End Property
    Public Property CommercialName As String
        Get
            Return _CommercialName
        End Get
        Set(value As String)
            _CommercialName = value
        End Set
    End Property
    Public Property DbaName As String
        Get
            Return _DbaName
        End Get
        Set(value As String)
            _DbaName = value
        End Set
    End Property

    Public Sub New()
        setDefaults()
    End Sub

    Public Overridable Sub setDefaults()
        _FirstName = String.Empty
        _MiddleName = String.Empty
        _LastName = String.Empty
        _CommercialName = String.Empty
        _DbaName = String.Empty
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _FirstName IsNot Nothing Then
                    _FirstName = Nothing
                End If
                If _MiddleName IsNot Nothing Then
                    _MiddleName = Nothing
                End If
                If _LastName IsNot Nothing Then
                    _LastName = Nothing
                End If
                If _CommercialName IsNot Nothing Then
                    _CommercialName = Nothing
                End If
                If _DbaName IsNot Nothing Then
                    _DbaName = Nothing
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
