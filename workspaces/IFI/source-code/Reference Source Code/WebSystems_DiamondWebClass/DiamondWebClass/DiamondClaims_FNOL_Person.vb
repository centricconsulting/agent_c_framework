Public Class DiamondClaims_FNOL_Person 'Don Mink - added 4/25/2013
    Implements IDisposable

    'Private _FirstName As String
    'Private _MiddleName As String
    'Private _LastName As String
    ''added 4/26/2013
    'Private _CommercialName As String
    'Private _DbaName As String

    'Private _HouseNumber As String
    'Private _StreetName As String
    'Private _City As String
    'Private _StateId As String
    'Private _ZipCode As String
    ''added 4/26/2013
    'Private _County As String
    'Private _AddressOther As String
    'Private _PoBox As String
    'Private _ApartmentNumber As String

    'Private _HomePhone As String
    'Private _BusinessPhone As String
    'Private _CellPhone As String
    'Private _FaxPhone As String 'added 4/26/2013

    'Private _OtherEmail As String 'changed from just _Email 4/26/2013
    ''added 4/26/2013
    'Private _HomeEmail As String
    'Private _BusinessEmail As String

    'updated 4/29/2013 to use separate objects to store everything
    Private _Name As DiamondClaims_FNOL_Name
    Private _Address As DiamondClaims_FNOL_Address
    Private _ContactInfo As DiamondClaims_FNOL_ContactInfo

#Region "Prop"

    'Public Property FirstName() As String
    '    Get
    '        Return _FirstName
    '    End Get
    '    Set(ByVal value As String)
    '        _FirstName = value
    '    End Set
    'End Property

    'Public Property MiddleName() As String
    '    Get
    '        Return _MiddleName
    '    End Get
    '    Set(ByVal value As String)
    '        _MiddleName = value
    '    End Set
    'End Property

    'Public Property LastName() As String
    '    Get
    '        Return _LastName
    '    End Get
    '    Set(ByVal value As String)
    '        _LastName = value
    '    End Set
    'End Property
    'Public Property CommercialName As String
    '    Get
    '        Return _CommercialName
    '    End Get
    '    Set(value As String)
    '        _CommercialName = value
    '    End Set
    'End Property
    'Public Property DbaName As String
    '    Get
    '        Return _DbaName
    '    End Get
    '    Set(value As String)
    '        _DbaName = value
    '    End Set
    'End Property

    'Public Property HouseNumber() As String
    '    Get
    '        Return _HouseNumber
    '    End Get
    '    Set(ByVal value As String)
    '        _HouseNumber = value
    '    End Set
    'End Property

    'Public Property StreetName() As String
    '    Get
    '        Return _StreetName
    '    End Get
    '    Set(ByVal value As String)
    '        _StreetName = value
    '    End Set
    'End Property

    'Public Property City() As String
    '    Get
    '        Return _City
    '    End Get
    '    Set(ByVal value As String)
    '        _City = value
    '    End Set
    'End Property

    'Public Property StateId() As String
    '    Get
    '        Return _StateId
    '    End Get
    '    Set(ByVal value As String)
    '        _StateId = value
    '    End Set
    'End Property

    'Public Property ZipCode() As String
    '    Get
    '        Return _ZipCode
    '    End Get
    '    Set(ByVal value As String)
    '        _ZipCode = value
    '    End Set
    'End Property

    'Public Property HomePhone() As String
    '    Get
    '        Return _HomePhone
    '    End Get
    '    Set(ByVal value As String)
    '        _HomePhone = value
    '    End Set
    'End Property

    'Public Property BusinessPhone() As String
    '    Get
    '        Return _BusinessPhone
    '    End Get
    '    Set(ByVal value As String)
    '        _BusinessPhone = value
    '    End Set
    'End Property

    'Public Property CellPhone() As String
    '    Get
    '        Return _CellPhone
    '    End Get
    '    Set(ByVal value As String)
    '        _CellPhone = value
    '    End Set
    'End Property
    'Public Property FaxPhone As String
    '    Get
    '        Return _FaxPhone
    '    End Get
    '    Set(value As String)
    '        _FaxPhone = value
    '    End Set
    'End Property

    'Public Property OtherEmail() As String
    '    Get
    '        Return _OtherEmail
    '    End Get
    '    Set(ByVal value As String)
    '        _OtherEmail = value
    '    End Set
    'End Property
    'Public Property HomeEmail() As String
    '    Get
    '        Return _HomeEmail
    '    End Get
    '    Set(ByVal value As String)
    '        _HomeEmail = value
    '    End Set
    'End Property
    'Public Property BusinessEmail() As String
    '    Get
    '        Return _BusinessEmail
    '    End Get
    '    Set(ByVal value As String)
    '        _BusinessEmail = value
    '    End Set
    'End Property

    'Public Property County As String
    '    Get
    '        Return _County
    '    End Get
    '    Set(value As String)
    '        _County = value
    '    End Set
    'End Property
    'Public Property AddressOther As String
    '    Get
    '        Return _AddressOther
    '    End Get
    '    Set(value As String)
    '        _AddressOther = value
    '    End Set
    'End Property
    'Public Property PoBox As String
    '    Get
    '        Return _PoBox
    '    End Get
    '    Set(value As String)
    '        _PoBox = value
    '    End Set
    'End Property
    'Public Property ApartmentNumber As String
    '    Get
    '        Return _ApartmentNumber
    '    End Get
    '    Set(value As String)
    '        _ApartmentNumber = value
    '    End Set
    'End Property

    'updated 4/29/2013 to use separate objects to store everything
    Public Property Name As DiamondClaims_FNOL_Name
        Get
            Return _Name
        End Get
        Set(value As DiamondClaims_FNOL_Name)
            _Name = value
        End Set
    End Property
    Public Property Address As DiamondClaims_FNOL_Address
        Get
            Return _Address
        End Get
        Set(value As DiamondClaims_FNOL_Address)
            _Address = value
        End Set
    End Property
    Public Property ContactInfo As DiamondClaims_FNOL_ContactInfo
        Get
            Return _ContactInfo
        End Get
        Set(value As DiamondClaims_FNOL_ContactInfo)
            _ContactInfo = value
        End Set
    End Property

#End Region

    Public Sub New()
        'setDefaults()
        setBaseDefaults() 'changed name 6/4/2013 since code was hitting setDefaults method for inheriting class
    End Sub

    'Public Overridable Sub setDefaults()
    Public Sub setBaseDefaults() 'changed name 6/4/2013
        '_FirstName = String.Empty
        '_MiddleName = String.Empty
        '_LastName = String.Empty
        '_CommercialName = String.Empty
        '_DbaName = String.Empty

        '_HouseNumber = String.Empty
        '_StreetName = String.Empty
        '_City = String.Empty
        '_StateId = String.Empty
        '_ZipCode = String.Empty
        '_County = String.Empty
        '_AddressOther = String.Empty
        '_PoBox = String.Empty
        '_ApartmentNumber = String.Empty

        '_HomePhone = String.Empty
        '_BusinessPhone = String.Empty
        '_CellPhone = String.Empty
        '_FaxPhone = String.Empty

        '_OtherEmail = String.Empty
        '_HomeEmail = String.Empty
        '_BusinessEmail = String.Empty

        'updated 4/29/2013 to use separate objects to store everything
        _Name = New DiamondClaims_FNOL_Name
        _Address = New DiamondClaims_FNOL_Address
        _ContactInfo = New DiamondClaims_FNOL_ContactInfo
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                'If _Firstname IsNot Nothing Then
                '    _Firstname = Nothing
                'End If
                'If _Middlename IsNot Nothing Then
                '    _Middlename = Nothing
                'End If
                'If _Lastname IsNot Nothing Then
                '    _Lastname = Nothing
                'End If
                'If _CommercialName IsNot Nothing Then
                '    _CommercialName = Nothing
                'End If
                'If _DbaName IsNot Nothing Then
                '    _DbaName = Nothing
                'End If

                'If _HouseNumber IsNot Nothing Then
                '    _HouseNumber = Nothing
                'End If
                'If _StreetName IsNot Nothing Then
                '    _StreetName = Nothing
                'End If
                'If _City IsNot Nothing Then
                '    _City = Nothing
                'End If
                'If _StateId IsNot Nothing Then
                '    _StateId = Nothing
                'End If
                'If _Zipcode IsNot Nothing Then
                '    _Zipcode = Nothing
                'End If
                'If _County IsNot Nothing Then
                '    _County = Nothing
                'End If
                'If _AddressOther IsNot Nothing Then
                '    _AddressOther = Nothing
                'End If
                'If _PoBox IsNot Nothing Then
                '    _PoBox = Nothing
                'End If
                'If _ApartmentNumber IsNot Nothing Then
                '    _ApartmentNumber = Nothing
                'End If

                'If _Homephone IsNot Nothing Then
                '    _Homephone = Nothing
                'End If
                'If _BusinessPhone IsNot Nothing Then
                '    _BusinessPhone = Nothing
                'End If
                'If _CellPhone IsNot Nothing Then
                '    _CellPhone = Nothing
                'End If
                'If _FaxPhone IsNot Nothing Then
                '    _FaxPhone = Nothing
                'End If

                'If _OtherEmail IsNot Nothing Then
                '    _OtherEmail = Nothing
                'End If
                'If _HomeEmail IsNot Nothing Then
                '    _HomeEmail = Nothing
                'End If
                'If _BusinessEmail IsNot Nothing Then
                '    _BusinessEmail = Nothing
                'End If

                'updated 4/29/2013 to use separate objects to store everything
                If _Name IsNot Nothing Then
                    _Name.Dispose()
                    _Name = Nothing
                End If
                If _Address IsNot Nothing Then
                    _Address.Dispose()
                    _Address = Nothing
                End If
                If _ContactInfo IsNot Nothing Then
                    _ContactInfo.Dispose()
                    _ContactInfo = Nothing
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
    Public Overridable Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
