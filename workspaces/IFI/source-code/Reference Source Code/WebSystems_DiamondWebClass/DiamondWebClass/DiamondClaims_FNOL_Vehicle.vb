Public Class DiamondClaims_FNOL_Vehicle
#Region "Var"
    Private _LossIndicatorType As Enums.ClaimLossIndicatorType
    Private _DamageDescription As String
    Private _EstimatedAmountOfDamage As Decimal
    Private _InvolvedInLoss As Boolean
    Private _LicensePlate As String
    Private _LicenseState As Enums.State
    Private _Make As String
    Private _Model As String
    Private _UsedWithPermission As Boolean
    Private _Color As String
    Private _Drivable As Boolean
    Private _VIN As String
    Private _Year As Integer
    Private _Status As Diamond.Common.Enums.StatusCode

    Private _LocationOfAccidentStreet As String
    Private _LocationOfAccidentCity As String
    Private _LocationOfAccidentState As String
    Private _LocationOfAccidentZip As String

    Private _LossVehicleOwnerFirstName As String
    Private _LossVehicleOwnerMiddleName As String
    Private _LossVehicleOwnerLastName As String

    Private _LossVehicleOperatorFirstName As String
    Private _LossVehicleOperatorMiddleName As String
    Private _LossVehicleOperatorLastName As String

    'added 8/10/2017
    Private _LocationOfAccidentAddress As DiamondClaims_FNOL_Address
    Private _LossVehicleOwnerName As DiamondClaims_FNOL_Name
    Private _LossVehicleOperatorName As DiamondClaims_FNOL_Name

    Private _DrivableId As Integer
    Private _AirbagsDeployedTypeId As Integer
    Private _CCCEstimateQualificationId As Integer
    Private _CCCphone As String
    Private _LossAddress As DiamondClaims_FNOL_Address



#End Region

    Public Sub New()
        setDefaults()
    End Sub

    Public Sub New(ByVal veh As VehicleObject)
        setDefaults()
        Make = veh.Make
        Model = veh.Model
        VIN = veh.VIN
        Year = veh.Year
        Status = Diamond.Common.Enums.StatusCode.Active

    End Sub

    Public Sub New(ByVal veh As DCO.Policy.Vehicle)
        setDefaults()
        LicensePlate = veh.License
        LicenseState = veh.LicenseStateId
        Make = veh.Make
        Model = veh.Model
        VIN = veh.Vin
        Year = veh.Year
        Status = veh.DetailStatusCode
    End Sub

    Public Sub setDefaults()
        LossIndicatorType = Enums.ClaimLossIndicatorType.None
        Status = Diamond.Common.Enums.StatusCode.Active
        EstimatedAmountOfDamage = 0D
        InvolvedInLoss = False
        UsedWithPermission = False
        Drivable = False
        If DamageDescription Is Nothing Then DamageDescription = String.Empty
        If Color Is Nothing Then Color = String.Empty
        If Make Is Nothing Then Make = String.Empty
        If Model Is Nothing Then Model = String.Empty
        If VIN Is Nothing Then VIN = String.Empty
        If Year <= 0 Then Year = -1
        If LicensePlate Is Nothing Then LicensePlate = String.Empty
        If LicenseState <= 0 Then LicenseState = Enums.State.INDIANA

        If DrivableId <= 0 Then DrivableId = 0    ' 1=Yes, 2=No, -1=Unknown
        If AirbagsDeployedTypeId <= 0 Then AirbagsDeployedTypeId = 0   ' 1=Yes, 2=No, 3=Unknown
        If CCCEstimateQualificationId <= 0 Then CCCEstimateQualificationId = -1    '1=InsuredOptYes,  0 =InsuredOptNo 
        If CCCphone Is Nothing Then CCCphone = String.Empty

        'added 8/10/2017
        _LocationOfAccidentAddress = New DiamondClaims_FNOL_Address
        _LossVehicleOwnerName = New DiamondClaims_FNOL_Name
        _LossVehicleOperatorName = New DiamondClaims_FNOL_Name
    End Sub

#Region "Prop"

    Public Property LossIndicatorType() As Enums.ClaimLossIndicatorType
        Get
            Return _LossIndicatorType
        End Get
        Set(ByVal value As Enums.ClaimLossIndicatorType)
            _LossIndicatorType = value
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

    Public Property EstimatedAmountOfDamage() As Decimal
        Get
            Return _EstimatedAmountOfDamage
        End Get
        Set(ByVal value As Decimal)
            _EstimatedAmountOfDamage = value
        End Set
    End Property

    Public Property InvolvedInLoss() As Boolean
        Get
            Return _InvolvedInLoss
        End Get
        Set(ByVal value As Boolean)
            _InvolvedInLoss = value
        End Set
    End Property

    Public Property LicensePlate() As String
        Get
            Return _LicensePlate
        End Get
        Set(ByVal value As String)
            _LicensePlate = value
        End Set
    End Property

    Public Property LicenseState() As Enums.State
        Get
            Return _LicenseState
        End Get
        Set(ByVal value As Enums.State)
            _LicenseState = value
        End Set
    End Property

    Public Property Make() As String
        Get
            Return _Make
        End Get
        Set(ByVal value As String)
            _Make = value
        End Set
    End Property

    Public Property Model() As String
        Get
            Return _Model
        End Get
        Set(ByVal value As String)
            _Model = value
        End Set
    End Property

    Public Property UsedWithPermission() As Boolean
        Get
            Return _UsedWithPermission
        End Get
        Set(ByVal value As Boolean)
            _UsedWithPermission = value
        End Set
    End Property

    Public Property Color() As String
        Get
            Return _Color
        End Get
        Set(ByVal value As String)
            _Color = value
        End Set
    End Property

    Public Property Drivable() As Boolean
        Get
            Return _Drivable
        End Get
        Set(ByVal value As Boolean)
            _Drivable = value
        End Set
    End Property

    Public Property VIN() As String
        Get
            Return _VIN
        End Get
        Set(ByVal value As String)
            _VIN = value
        End Set
    End Property

    Public Property Year() As Integer
        Get
            Return _Year
        End Get
        Set(ByVal value As Integer)
            _Year = value
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

    Public Property LossVehicleOperatorFirstName() As String
        Get
            Return _LossVehicleOperatorFirstName
        End Get
        Set(ByVal value As String)
            _LossVehicleOperatorFirstName = value
        End Set
    End Property

    Public Property LossVehicleOperatorMiddleName() As String
        Get
            Return _LossVehicleOperatorMiddleName
        End Get
        Set(ByVal value As String)
            _LossVehicleOperatorMiddleName = value
        End Set
    End Property

    Public Property LossVehicleOperatorLastName() As String
        Get
            Return _LossVehicleOperatorLastName
        End Get
        Set(ByVal value As String)
            _LossVehicleOperatorLastName = value
        End Set
    End Property

    Public Property LossVehicleOwnerFirstName() As String
        Get
            Return _LossVehicleOwnerFirstName
        End Get
        Set(ByVal value As String)
            _LossVehicleOwnerFirstName = value
        End Set
    End Property

    Public Property LossVehicleOwnerMiddleName() As String
        Get
            Return _LossVehicleOwnerMiddleName
        End Get
        Set(ByVal value As String)
            _LossVehicleOwnerMiddleName = value
        End Set
    End Property

    Public Property LossVehicleOwnerLastName() As String
        Get
            Return _LossVehicleOwnerLastName
        End Get
        Set(ByVal value As String)
            _LossVehicleOwnerLastName = value
        End Set
    End Property

    'added 8/10/2017
    Public Property LocationOfAccidentAddress As DiamondClaims_FNOL_Address
        Get
            Return _LocationOfAccidentAddress
        End Get
        Set(value As DiamondClaims_FNOL_Address)
            _LocationOfAccidentAddress = value
        End Set
    End Property
    Public Property LossVehicleOwnerName As DiamondClaims_FNOL_Name
        Get
            Return _LossVehicleOwnerName
        End Get
        Set(value As DiamondClaims_FNOL_Name)
            _LossVehicleOwnerName = value
        End Set
    End Property
    Public Property LossVehicleOperatorName As DiamondClaims_FNOL_Name
        Get
            Return _LossVehicleOperatorName
        End Get
        Set(value As DiamondClaims_FNOL_Name)
            _LossVehicleOperatorName = value
        End Set
    End Property
    Public Property DrivableId() As Integer
        Get
            Return _DrivableId
        End Get
        Set(ByVal value As Integer)
            _DrivableId = value
        End Set
    End Property

    Public Property AirbagsDeployedTypeId() As Integer
        Get
            Return _AirbagsDeployedTypeId
        End Get
        Set(ByVal value As Integer)
            _AirbagsDeployedTypeId = value
        End Set
    End Property

    Public Property CCCEstimateQualificationId() As Integer
        Get
            Return _CCCEstimateQualificationId
        End Get
        Set(ByVal value As Integer)
            _CCCEstimateQualificationId = value
        End Set
    End Property

    Public Property CCCphone() As String
        Get
            Return _CCCphone
        End Get
        Set(ByVal value As String)
            _CCCphone = value
        End Set
    End Property

    Public Property LossAddress As DiamondClaims_FNOL_Address
        Get
            Return _LossAddress
        End Get
        Set(value As DiamondClaims_FNOL_Address)
            _LossAddress = value
        End Set
    End Property

#End Region

End Class
