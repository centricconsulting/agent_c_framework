Public Class DiamondClaims_FNOL_Address 'added 4/29/2013
    Implements IDisposable

    Private _HouseNumber As String
    Private _StreetName As String
    Private _City As String
    Private _StateId As String
    Private _ZipCode As String
    Private _County As String
    Private _AddressOther As String
    Private _PoBox As String
    Private _ApartmentNumber As String

    Public Property HouseNumber() As String
        Get
            Return _HouseNumber
        End Get
        Set(ByVal value As String)
            _HouseNumber = value
        End Set
    End Property
    Public Property StreetName() As String
        Get
            Return _StreetName
        End Get
        Set(ByVal value As String)
            _StreetName = value
        End Set
    End Property
    Public Property City() As String
        Get
            Return _City
        End Get
        Set(ByVal value As String)
            _City = value
        End Set
    End Property
    Public Property StateId() As String
        Get
            Return _StateId
        End Get
        Set(ByVal value As String)
            _StateId = value
        End Set
    End Property
    Public Property ZipCode() As String
        Get
            Return _ZipCode
        End Get
        Set(ByVal value As String)
            _ZipCode = value
        End Set
    End Property
    Public Property County As String
        Get
            Return _County
        End Get
        Set(value As String)
            _County = value
        End Set
    End Property
    Public Property AddressOther As String
        Get
            Return _AddressOther
        End Get
        Set(value As String)
            _AddressOther = value
        End Set
    End Property
    Public Property PoBox As String
        Get
            Return _PoBox
        End Get
        Set(value As String)
            _PoBox = value
        End Set
    End Property
    Public Property ApartmentNumber As String
        Get
            Return _ApartmentNumber
        End Get
        Set(value As String)
            _ApartmentNumber = value
        End Set
    End Property

    Public Sub New()
        setDefaults()
    End Sub

    Public Overridable Sub setDefaults()
        _HouseNumber = String.Empty
        _StreetName = String.Empty
        _City = String.Empty
        _StateId = String.Empty
        _ZipCode = String.Empty
        _County = String.Empty
        _AddressOther = String.Empty
        _PoBox = String.Empty
        _ApartmentNumber = String.Empty
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _HouseNumber IsNot Nothing Then
                    _HouseNumber = Nothing
                End If
                If _StreetName IsNot Nothing Then
                    _StreetName = Nothing
                End If
                If _City IsNot Nothing Then
                    _City = Nothing
                End If
                If _StateId IsNot Nothing Then
                    _StateId = Nothing
                End If
                If _ZipCode IsNot Nothing Then
                    _ZipCode = Nothing
                End If
                If _County IsNot Nothing Then
                    _County = Nothing
                End If
                If _AddressOther IsNot Nothing Then
                    _AddressOther = Nothing
                End If
                If _PoBox IsNot Nothing Then
                    _PoBox = Nothing
                End If
                If _ApartmentNumber IsNot Nothing Then
                    _ApartmentNumber = Nothing
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
