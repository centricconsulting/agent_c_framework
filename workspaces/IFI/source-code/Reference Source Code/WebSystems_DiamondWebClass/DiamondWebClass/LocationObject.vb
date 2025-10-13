Public Class LocationObject
    Private _ApartmentNumber As String
    Private _City As String
    Private _CountryID As DCE.Country
    Private _Status As DCE.StatusCode
    Private _HouseNumber As String
    Private _NameAddressSource As DCE.NameAddressSource
    Private _POBox As String
    Private _State As Enums.State
    Private _StreetName As String
    Private _Zip As String
    Private _Other As String

    Public Sub New()
        setDefaults()
    End Sub

    Public Sub setDefaults()
        ApartmentNumber = String.Empty
        City = String.Empty
        CountryID = Diamond.Common.Enums.Country.UnitedStates
        Status = Diamond.Common.Enums.StatusCode.Active
        HouseNumber = String.Empty
        NameAddressSource = Diamond.Common.Enums.NameAddressSource.Location
        POBox = String.Empty
        State = Enums.State.INDIANA
        StreetName = String.Empty
        Zip = String.Empty
        Other = String.Empty
    End Sub

#Region "Prop"

    Public Property ApartmentNumber() As String
        Get
            Return _ApartmentNumber
        End Get
        Set(ByVal value As String)
            _ApartmentNumber = value
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

    Public Property CountryID() As DCE.Country
        Get
            Return _CountryID
        End Get
        Set(ByVal value As DCE.Country)
            _CountryID = value
        End Set
    End Property

    Public Property Status() As DCE.StatusCode
        Get
            Return _Status
        End Get
        Set(ByVal value As DCE.StatusCode)
            _Status = value
        End Set
    End Property

    Public Property HouseNumber() As String
        Get
            Return _HouseNumber
        End Get
        Set(ByVal value As String)
            _HouseNumber = value
        End Set
    End Property

    Public Property NameAddressSource() As DCE.NameAddressSource
        Get
            Return _NameAddressSource
        End Get
        Set(ByVal value As DCE.NameAddressSource)
            _NameAddressSource = value
        End Set
    End Property

    Public Property POBox() As String
        Get
            Return _POBox
        End Get
        Set(ByVal value As String)
            _POBox = value
        End Set
    End Property

    Public Property State() As Enums.State
        Get
            Return _State
        End Get
        Set(ByVal value As Enums.State)
            _State = value
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

    Public Property Zip() As String
        Get
            Return _Zip
        End Get
        Set(ByVal value As String)
            _Zip = value
        End Set
    End Property

    Public Property Other() As String
        Get
            Return _Other
        End Get
        Set(ByVal value As String)
            _Other = value
        End Set
    End Property

#End Region
End Class
