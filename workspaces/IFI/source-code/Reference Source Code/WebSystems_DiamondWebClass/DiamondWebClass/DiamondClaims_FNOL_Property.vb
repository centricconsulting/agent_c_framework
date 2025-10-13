Public Class DiamondClaims_FNOL_Property

    Private _LocationOfLossType As Enums.ClaimLocationOfLoss
    Private _DamageDescription As String
    Private _Status As DCE.StatusCode
    Private _DwellingType As Enums.DwellingType
    Private _EstimatedAmount As Decimal
    Private _Location As LocationObject
    'Private _Location As nothing
    Private _LocationOfAccident As String
    'updated 4/29/2013 to use separate objects to store everything
    Private _Address As DiamondClaims_FNOL_Address

    Public Sub New()
        setDefaults()
    End Sub

    Public Sub setDefaults()
        LocationOfLossType = Enums.ClaimLocationOfLoss.Unknown
        DamageDescription = String.Empty
        Status = Diamond.Common.Enums.StatusCode.Active
        DwellingType = Enums.DwellingType.NotAvailable
        EstimatedAmount = 0D
        LocationOfAccident = String.Empty
        Location = New LocationObject
        'updated 4/29/2013 to use separate objects to store everything
        _Address = New DiamondClaims_FNOL_Address
    End Sub

#Region "Prop"

    Public Property LocationOfLossType() As Enums.ClaimLocationOfLoss
        Get
            Return _LocationOfLossType
        End Get
        Set(ByVal value As Enums.ClaimLocationOfLoss)
            _LocationOfLossType = value
        End Set
    End Property

    Public Property DamageDescription() As String
        Get
            Return _DamageDescription
        End Get
        Set(ByVal value As String)
            _DamageDescription = value
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

    'updated 4/29/2013 to use separate objects to store everything
    Public Property Address As DiamondClaims_FNOL_Address
        Get
            Return _Address
        End Get
        Set(value As DiamondClaims_FNOL_Address)
            _Address = value
        End Set
    End Property

#End Region
End Class
