Public Class DiamondClaims_FNOL_Claimant
    Private _Status As DCE.StatusCode

    Private _Firstname As String
    Private _Middlename As String
    Private _Lastname As String

    Private _HouseNumber As String
    Private _StreetName As String
    Private _City As String
    Private _State As String
    Private _Zipcode As String

    Private _Homephone As String
    Private _BusinessPhone As String
    Private _CellPhone As String

    Private _Email As String

    Private _DwellingType As Enums.DwellingType
    Private _EstimatedAmount As Decimal
    Private _Location As LocationObject
    'Private _Location As nothing
    Private _LocationOfAccident As String

    Private _claimantTypeID As Integer

    'updated 4/29/2013 to use separate objects to store everything
    Private _Name As DiamondClaims_FNOL_Name
    Private _Address As DiamondClaims_FNOL_Address
    Private _ContactInfo As DiamondClaims_FNOL_ContactInfo

    Public Sub New()
        setDefaults()
    End Sub

    Public Sub setDefaults()
        Status = Diamond.Common.Enums.StatusCode.Active

        Firstname = String.Empty
        Middlename = String.Empty
        Lastname = String.Empty

        Housenumber = String.Empty
        Streetname = String.Empty
        City = String.Empty
        State = String.Empty
        Zipcode = String.Empty

        Homephone = String.Empty
        Businessphone = String.Empty
        Cellphone = String.Empty

        Email = String.Empty

        DwellingType = Enums.DwellingType.NotAvailable
        EstimatedAmount = 0D
        LocationOfAccident = String.Empty
        Location = New LocationObject
        Location = New LocationObject

        claimantTypeID = 0

        'updated 4/29/2013 to use separate objects to store everything
        _Name = New DiamondClaims_FNOL_Name
        _Address = New DiamondClaims_FNOL_Address
        _ContactInfo = New DiamondClaims_FNOL_ContactInfo
    End Sub

#Region "Prop"

    Public Property Status() As DCE.StatusCode
        Get
            Return _Status
        End Get
        Set(ByVal value As DCE.StatusCode)
            _Status = value
        End Set
    End Property

    Public Property Firstname() As String
        Get
            Return _Firstname
        End Get
        Set(ByVal value As String)
            _Firstname = value
        End Set
    End Property

    Public Property Middlename() As String
        Get
            Return _Middlename
        End Get
        Set(ByVal value As String)
            _Middlename = value
        End Set
    End Property

    Public Property Lastname() As String
        Get
            Return _Lastname
        End Get
        Set(ByVal value As String)
            _Lastname = value
        End Set
    End Property

    Public Property Housenumber() As String
        Get
            Return _HouseNumber
        End Get
        Set(ByVal value As String)
            _HouseNumber = value
        End Set
    End Property

    Public Property Streetname() As String
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

    Public Property State() As String
        Get
            Return _State
        End Get
        Set(ByVal value As String)
            _State = value
        End Set
    End Property

    Public Property Zipcode() As String
        Get
            Return _Zipcode
        End Get
        Set(ByVal value As String)
            _Zipcode = value
        End Set
    End Property

    Public Property Homephone() As String
        Get
            Return _Homephone
        End Get
        Set(ByVal value As String)
            _Homephone = value
        End Set
    End Property

    Public Property Businessphone() As String
        Get
            Return _BusinessPhone
        End Get
        Set(ByVal value As String)
            _BusinessPhone = value
        End Set
    End Property

    Public Property Cellphone() As String
        Get
            Return _CellPhone
        End Get
        Set(ByVal value As String)
            _CellPhone = value
        End Set
    End Property

    Public Property Email() As String
        Get
            Return _Email
        End Get
        Set(ByVal value As String)
            _Email = value
        End Set
    End Property

    Public Property DwellingType() As Enums.DwellingType
        Get
            Return _DwellingType
        End Get
        Set(ByVal value As Enums.DwellingType)
            _DwellingType = value
        End Set
    End Property

    Public Property EstimatedAmount() As Decimal
        Get
            Return _EstimatedAmount
        End Get
        Set(ByVal value As Decimal)
            _EstimatedAmount = value
        End Set
    End Property

    Public Property LocationOfAccident() As String
        Get
            Return _LocationOfAccident
        End Get
        Set(ByVal value As String)
            _LocationOfAccident = value
        End Set
    End Property

    Public Property Location() As LocationObject
        Get
            Return _Location
        End Get
        Set(ByVal value As LocationObject)
            _Location = value
        End Set
    End Property

    Public Property claimantTypeID() As Integer
        Get
            Return _claimantTypeID
        End Get
        Set(ByVal value As Integer)
            _claimantTypeID = value
        End Set
    End Property

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

End Class
