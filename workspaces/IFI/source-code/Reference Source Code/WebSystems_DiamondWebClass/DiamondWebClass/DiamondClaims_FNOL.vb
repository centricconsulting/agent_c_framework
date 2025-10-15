Public Class DiamondClaims_FNOL
    Implements IDisposable

#Region "Var"
    Private _SaveAttempt As DCE.Claims.SaveAttempt
    Private _StatusType As DCE.Claims.ClaimLnStatusType
    Private _UserID As Integer
    Private _LossDate As Date
    Private _ClaimType As DCE.Claims.ClaimType
    Private _PolicyID As Integer
    Private _PolicyImage As Integer
    Private _Description As String
    Private _ClaimLossType As Enums.ClaimLossType
    'Private _ClaimLossTypeId As Integer 'added 7/29/2017; removed 8/10/2017
    Private _ClaimFaultType As Enums.ClaimFaultType

    Private _ClaimNumber As String
    Private _ClaimControlID As Integer

    Private _Vehicles As List(Of DiamondClaims_FNOL_Vehicle)
    Private _Properties As List(Of DiamondClaims_FNOL_Property)
    Private _Claimants As List(Of DiamondClaims_FNOL_Claimant)

    Private _insuredFname As String
    Private _insuredLname As String
    Private _ClaimLocation As String

    Private _lossAddressHouseNum As String
    Private _lossAddressStreetName As String
    Private _lossAddressCity As String
    Private _lossAddressState As String
    Private _lossAddressZip As String
    'updated 4/29/2013 to use separate objects to store everything
    Private _LossAddress As DiamondClaims_FNOL_Address

    Private _insuredHomePhone As String
    Private _insuredBusinessPhone As String
    Private _insuredMobilePhone As String
    Private _insuredHomeEmail As String
    Private _insuredBusinessEmail As String
    Private _insuredOtherEmail As String

    Private _packagePartType As String

    Private _claimOfficeID As String
    Private _InsideAdjusterId As String
    Private _AdministratorId As String

    Private _Witnesses As List(Of DiamondClaims_FNOL_Witness) 'added 4/25/2013
    Private _Insured As DiamondClaims_FNOL_Insured 'added 4/26/2013
    Private _Reporter As DiamondClaims_FNOL_Reporter 'added 4/26/2013
    Private _SecondInsured As DiamondClaims_FNOL_Insured 'added 4/29/2013

    Private _ReportedDate As Date 'added 5/31/2013

    Private _PolicyVersionId As String 'added 3/11/2017
    Private _VersionId As String 'added 07/16/2024

    Private _ClaimSeverity_Id As String  ' added 12/9/2020 MGB

#End Region


    Public Sub New()
        setDefaults()
    End Sub

    Public Sub New(ByVal fnolUserId As Integer, ByVal fnolPolicyID As Integer, ByVal fnolPolicyImage As Integer, ByVal fnolLossDate As Date, ByVal fnolDescription As String, _
                   ByVal fnolSaveAttempt As DCE.Claims.SaveAttempt, ByVal fnolStatusType As DCE.Claims.ClaimLnStatusType, ByVal fnolClaimType As DCE.Claims.ClaimType, _
                   ByVal fnolClaimLossType As Enums.ClaimLossType, ByVal fnolClaimFaultType As Enums.ClaimFaultType, ByVal fnolInsuredFirstName As String, ByVal fnolInsuredLastName As String, ByVal fnolClaimLossLocation As String)
        setDefaults()
        SaveAttempt = fnolSaveAttempt
        StatusType = fnolStatusType
        UserID = fnolUserId
        LossDate = fnolLossDate
        ClaimType = fnolClaimType
        PolicyID = fnolPolicyID
        PolicyImage = fnolPolicyImage
        Description = fnolDescription
        ClaimLossType = fnolClaimLossType
        ClaimFaultType = fnolClaimFaultType

        insuredFirstName = fnolInsuredFirstName
        insuredLastName = fnolInsuredLastName
        ClaimLocation = fnolClaimLossLocation
    End Sub

    'for FNOL fast track 1/18/12
    Public Sub New(ByVal fnolUserId As Integer, ByVal fnolPolicyID As Integer, ByVal fnolPolicyImage As Integer, ByVal fnolLossDate As Date, ByVal fnolDescription As String, _
                   ByVal fnolSaveAttempt As DCE.Claims.SaveAttempt, ByVal fnolStatusType As DCE.Claims.ClaimLnStatusType, ByVal fnolClaimType As DCE.Claims.ClaimType, _
                   ByVal fnolClaimLossType As Enums.ClaimLossType)
        setDefaults()
        SaveAttempt = fnolSaveAttempt
        StatusType = fnolStatusType
        UserID = fnolUserId
        LossDate = fnolLossDate
        ClaimType = fnolClaimType
        PolicyID = fnolPolicyID
        PolicyImage = fnolPolicyImage
        Description = fnolDescription
        ClaimLossType = fnolClaimLossType
        
    End Sub

    Public Sub SetReadOnlyProperties(ByRef fnolObject As DiamondClaims_FNOL, ByVal fnolClaimNumber As String, ByVal fnolClaimControlId As Integer)
        fnolObject._ClaimNumber = fnolClaimNumber
        fnolObject._ClaimControlID = fnolClaimControlId
    End Sub

    Public Sub setDefaults()
        SaveAttempt = Diamond.Common.Enums.Claims.SaveAttempt.First
        StatusType = Diamond.Common.Enums.Claims.ClaimLnStatusType.NewClaim
        UserID = -1
        LossDate = Date.Today
        ClaimType = Diamond.Common.Enums.Claims.ClaimType.Normal
        PolicyID = -1
        PolicyImage = 1
        Description = String.Empty
        ClaimLossType = Enums.ClaimLossType.NotAvailable
        '_ClaimLossTypeId = 0 'added 7/29/2017; could omit since it will already be set by ClaimLossType prop on previous line... okay since same value; removed 8/10/2017
        ClaimFaultType = Enums.ClaimFaultType.Undetermined 'added default 4/29/2013

        Vehicles = New List(Of DiamondClaims_FNOL_Vehicle)
        Properties = New List(Of DiamondClaims_FNOL_Property)
        Claimants = New List(Of DiamondClaims_FNOL_Claimant)

        _ClaimNumber = String.Empty
        _ClaimControlID = -1

        insuredFirstName = String.Empty
        insuredLastName = String.Empty
        ClaimLocation = String.Empty

        'added more defaults 4/29/2013
        _lossAddressHouseNum = String.Empty
        _lossAddressStreetName = String.Empty
        _lossAddressCity = String.Empty
        _lossAddressState = String.Empty
        _lossAddressZip = String.Empty
        'updated 4/29/2013 to use separate objects to store everything
        _LossAddress = New DiamondClaims_FNOL_Address

        insuredHomePhone = String.Empty
        insuredBusinessPhone = String.Empty
        insuredMobilePhone = String.Empty

        insuredOtherEmail = String.Empty
        'added more defaults 4/29/2013
        insuredHomeEmail = String.Empty
        insuredBusinessEmail = String.Empty

        packagePartType = String.Empty

        claimOfficeID = String.Empty
        InsideAdjusterId = String.Empty
        AdministratorId = String.Empty

        _Witnesses = New List(Of DiamondClaims_FNOL_Witness) 'added 4/25/2013
        _Insured = New DiamondClaims_FNOL_Insured 'added 4/26/2013
        _Reporter = New DiamondClaims_FNOL_Reporter 'added 4/26/2013
        _SecondInsured = New DiamondClaims_FNOL_Insured 'added 4/29/2013

        '_ReportedDate = Date.Today 'added 5/31/2013; removed 6/25/2013 so we could configurably set to current date or loss date (since QA has an older system date and won't allow future reported dates)

        _PolicyVersionId = "" 'added 3/11/2017
        _VersionId = "" 'added 07/16/2024

    End Sub

#Region "Prop"

    Public Property SaveAttempt() As DCE.Claims.SaveAttempt
        Get
            Return _SaveAttempt
        End Get
        Set(ByVal value As DCE.Claims.SaveAttempt)
            _SaveAttempt = value
        End Set
    End Property

    Public Property StatusType() As DCE.Claims.ClaimLnStatusType
        Get
            Return _StatusType
        End Get
        Set(ByVal value As DCE.Claims.ClaimLnStatusType)
            _StatusType = value
        End Set
    End Property

    Public Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal value As Integer)
            _UserID = value
        End Set
    End Property

    Public Property LossDate() As Date
        Get
            Return _LossDate
        End Get
        Set(ByVal value As Date)
            _LossDate = value
        End Set
    End Property

    Public Property ClaimType() As DCE.Claims.ClaimType
        Get
            Return _ClaimType
        End Get
        Set(ByVal value As DCE.Claims.ClaimType)
            _ClaimType = value
        End Set
    End Property

    Public Property PolicyID() As Integer
        Get
            Return _PolicyID
        End Get
        Set(ByVal value As Integer)
            _PolicyID = value
        End Set
    End Property

    Public Property PolicyImage() As Integer
        Get
            Return _PolicyImage
        End Get
        Set(ByVal value As Integer)
            _PolicyImage = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    Public Property ClaimLossType() As Enums.ClaimLossType
        Get
            Return _ClaimLossType
        End Get
        Set(ByVal value As Enums.ClaimLossType)
            _ClaimLossType = value
            '_ClaimLossTypeId = CInt(_ClaimLossType) 'added 7/29/2017; removed 8/10/2017
        End Set
    End Property
    'Public Property ClaimLossTypeId As Integer 'added 7/29/2017; removed 8/10/2017
    '    Get
    '        Return _ClaimLossTypeId
    '    End Get
    '    Set(value As Integer)
    '        _ClaimLossTypeId = value
    '    End Set
    'End Property

    Public Property ClaimFaultType() As Enums.ClaimFaultType
        Get
            Return _ClaimFaultType
        End Get
        Set(ByVal value As Enums.ClaimFaultType)
            _ClaimFaultType = value
        End Set
    End Property

    Public ReadOnly Property ClaimNumber() As String
        Get
            Return _ClaimNumber
        End Get
    End Property

    Public ReadOnly Property ClaimControlId() As Integer
        Get
            Return _ClaimControlID
        End Get
    End Property

    Public Property Vehicles() As List(Of DiamondClaims_FNOL_Vehicle)
        Get
            Return _Vehicles
        End Get
        Set(ByVal value As List(Of DiamondClaims_FNOL_Vehicle))
            _Vehicles = value
        End Set
    End Property

    Public Property Properties() As List(Of DiamondClaims_FNOL_Property)
        Get
            Return _Properties
        End Get
        Set(ByVal value As List(Of DiamondClaims_FNOL_Property))
            _Properties = value
        End Set
    End Property

    Public Property Claimants As List(Of DiamondClaims_FNOL_Claimant)
        Get
            Return _Claimants
        End Get
        Set(ByVal value As List(Of DiamondClaims_FNOL_Claimant))
            _Claimants = value
        End Set
    End Property

    Public Property insuredFirstName() As String
        Get
            Return _insuredFname
        End Get
        Set(ByVal value As String)
            _insuredFname = value
        End Set
    End Property

    Public Property insuredLastName() As String
        Get
            Return _insuredLname
        End Get
        Set(ByVal value As String)
            _insuredLname = value
        End Set
    End Property

    Public Property ClaimLocation() As String
        Get
            Return _ClaimLocation
        End Get
        Set(ByVal value As String)
            _ClaimLocation = value
        End Set
    End Property

    Public Property lossAddressStreetName() As String
        Get
            Return _lossAddressStreetName
        End Get
        Set(ByVal value As String)
            _lossAddressStreetName = value
        End Set
    End Property

    Public Property lossAddressHouseNum() As String
        Get
            Return _lossAddressHouseNum
        End Get
        Set(ByVal value As String)
            _lossAddressHouseNum = value
        End Set
    End Property

    Public Property lossAddressCity() As String
        Get
            Return _lossAddressCity
        End Get
        Set(ByVal value As String)
            _lossAddressCity = value
        End Set
    End Property

    Public Property lossAddressState() As String
        Get
            Return _lossAddressState
        End Get
        Set(ByVal value As String)
            _lossAddressState = value
        End Set
    End Property

    Public Property lossAddressZip() As String
        Get
            Return _lossAddressZip
        End Get
        Set(ByVal value As String)
            _lossAddressZip = value
        End Set
    End Property
    'updated 4/29/2013 to use separate objects to store everything
    Public Property LossAddress As DiamondClaims_FNOL_Address
        Get
            Return _LossAddress
        End Get
        Set(value As DiamondClaims_FNOL_Address)
            _LossAddress = value
        End Set
    End Property

    Public Property insuredHomePhone() As String
        Get
            Return _insuredHomePhone
        End Get
        Set(ByVal value As String)
            _insuredHomePhone = value
        End Set
    End Property

    Public Property insuredBusinessPhone() As String
        Get
            Return _insuredBusinessPhone
        End Get
        Set(ByVal value As String)
            _insuredBusinessPhone = value
        End Set
    End Property

    Public Property insuredMobilePhone() As String
        Get
            Return _insuredMobilePhone
        End Get
        Set(ByVal value As String)
            _insuredMobilePhone = value
        End Set
    End Property

    Public Property insuredHomeEmail() As String
        Get
            Return _insuredHomeEmail
        End Get
        Set(ByVal value As String)
            _insuredHomeEmail = value
        End Set
    End Property

    Public Property insuredBusinessEmail() As String
        Get
            Return _insuredBusinessEmail
        End Get
        Set(ByVal value As String)
            _insuredBusinessEmail = value
        End Set
    End Property

    Public Property insuredOtherEmail() As String
        Get
            Return _insuredOtherEmail
        End Get
        Set(ByVal value As String)
            _insuredOtherEmail = value
        End Set
    End Property

    Public Property packagePartType() As String
        Get
            Return _packagePartType
        End Get
        Set(ByVal value As String)
            _packagePartType = value
        End Set
    End Property

    Public Property claimOfficeID() As String
        Get
            Return _claimOfficeID
        End Get
        Set(ByVal value As String)
            _claimOfficeID = value
        End Set
    End Property

    Public Property InsideAdjusterId() As String
        Get
            Return _InsideAdjusterId
        End Get
        Set(ByVal value As String)
            _InsideAdjusterId = value
        End Set
    End Property

    Public Property AdministratorId() As String
        Get
            Return _AdministratorId
        End Get
        Set(ByVal value As String)
            _AdministratorId = value
        End Set
    End Property

    Public Property Witnesses As List(Of DiamondClaims_FNOL_Witness) 'added 4/25/2013
        Get
            Return _Witnesses
        End Get
        Set(value As List(Of DiamondClaims_FNOL_Witness))
            _Witnesses = value
        End Set
    End Property
    Public Property Insured As DiamondClaims_FNOL_Insured 'added 4/26/2013
        Get
            Return _Insured
        End Get
        Set(value As DiamondClaims_FNOL_Insured)
            _Insured = value
        End Set
    End Property
    Public Property Reporter As DiamondClaims_FNOL_Reporter 'added 4/26/2013
        Get
            Return _Reporter
        End Get
        Set(value As DiamondClaims_FNOL_Reporter)
            _Reporter = value
        End Set
    End Property
    Public Property SecondInsured As DiamondClaims_FNOL_Insured 'added 4/29/2013
        Get
            Return _SecondInsured
        End Get
        Set(value As DiamondClaims_FNOL_Insured)
            _SecondInsured = value
        End Set
    End Property

    Public Property ReportedDate As Date 'added 5/31/2013
        Get
            Return _ReportedDate
        End Get
        Set(value As Date)
            _ReportedDate = value
        End Set
    End Property

    Public Property PolicyVersionId As String 'added 3/11/2017
        Get
            Return _PolicyVersionId
        End Get
        Set(value As String)
            _PolicyVersionId = value
        End Set
    End Property

    Public Property VersionId As String 'added on 07/16/2024
        Get
            Return _VersionId
        End Get
        Set(value As String)
            _VersionId = value
        End Set
    End Property

    Public Property ClaimSeverity_Id As String 'added 3/11/2017
        Get
            Return _ClaimSeverity_Id
        End Get
        Set(value As String)
            _ClaimSeverity_Id = value
        End Set
    End Property

#End Region


    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If SaveAttempt > 0 Then SaveAttempt = 0
                If StatusType > 0 Then StatusType = 0
                If UserID > 0 Then UserID = 0
                LossDate = Nothing
                If ClaimType > 0 Then ClaimType = 0
                If PolicyID > 0 Then PolicyID = 0
                If PolicyImage > 0 Then PolicyImage = 0
                If Description IsNot Nothing Then Description = Nothing
                If ClaimLossType > 0 Then ClaimLossType = 0
                'If _ClaimLossTypeId > 0 Then _ClaimLossTypeId = 0 'added 7/29/2017; could omit since it will already be set by ClaimLossType prop on previous line... okay since same value; removed 8/10/2017
                If ClaimFaultType > 0 Then ClaimFaultType = 0
                Vehicles = Nothing
                Properties = Nothing
                insuredFirstName = Nothing
                insuredLastName = Nothing
                ClaimLocation = Nothing
                lossAddressStreetName = Nothing
                lossAddressCity = Nothing
                lossAddressState = Nothing
                lossAddressZip = Nothing
                'added 4/29/2013
                If _LossAddress IsNot Nothing Then
                    _LossAddress.Dispose()
                    _LossAddress = Nothing
                End If

                insuredHomePhone = Nothing
                insuredBusinessPhone = Nothing
                insuredMobilePhone = Nothing

                insuredOtherEmail = Nothing

                claimOfficeID = Nothing
                InsideAdjusterId = Nothing
                AdministratorId = Nothing

                _Witnesses = Nothing 'added 4/25/2013
                _Insured = Nothing 'added 4/26/2013
                _Reporter = Nothing 'added 4/26/2013
                _SecondInsured = Nothing 'added 4/29/2013

                _ReportedDate = Nothing 'added 5/31/2013

                If _PolicyVersionId IsNot Nothing Then 'added 3/11/2017
                    _PolicyVersionId = Nothing
                End If
                If _VersionId IsNot Nothing Then 'added 07/16/2024
                    _VersionId = Nothing
                End If

            End If
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class