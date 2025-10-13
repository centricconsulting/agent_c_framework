Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store driver information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteDriver 'added 8/29/2012
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String

        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _Phones As Generic.List(Of QuickQuotePhone) 'no longer sending to Diamond as-of 12/22/2014; re-rate could have issues since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there (doesn't get db key constraint error like BillingAddressee and Operator since Diamond doesn't appear to ever perform the initial save)
        Private _Emails As Generic.List(Of QuickQuoteEmail) 'no longer sending to Diamond as-of 12/22/2014; re-rate could have issues since image doesn't load them back in and our logic tries to add them new since it appears that nothing is there (doesn't get db key constraint error like BillingAddressee and Operator since Diamond doesn't appear to ever perform the initial save)

        'added 9/25/2012 for CAP
        Private _LicenseStatusId As String
        Private _DriverExcludeTypeId As String
        'added 7/25/2013 for PPA
        Private _RelationshipTypeId As String 'may need matching Type variable/property
        'added 1/2/2014 for PPA
        Private _AccPreventionCourse As String
        Private _DefDriverDate As String
        Private _DistantStudent As Boolean
        Private _FirstPermit As Boolean
        Private _GoodStudent As Boolean
        Private _LicenseTypeId As String 'list
        Private _MonthsLicensedInState As String
        Private _MonthsRevoked As String
        Private _MotorClub As Boolean
        Private _MotorMembershipDate As String
        Private _MotorcycleTrainingDisc As Boolean
        Private _MotorcycleYearsExperience As String
        Private _ReasonExclude As String
        Private _SchoolDistance As String
        Private _EnolDays As String
        Private _EnolEmployedByGarage As Boolean
        Private _EnolGovtBusiness As Boolean
        Private _EnolInsuredOrSpouse As Boolean
        Private _EnolMedpayExclude As Boolean
        Private _EnolMiles As String
        Private _EnolNamedInsured As Boolean
        Private _EnolPrimaryLiability As Boolean
        Private _EnolRegularUse As Boolean
        Private _EnolTerritoryNum As String
        Private _EnolVehicleUseTypeId As String 'list
        Private _ExtendedNonOwned As Boolean
        'added 1/14/2014 for PPA
        Private _AccidentViolations As List(Of QuickQuoteAccidentViolation)
        'added 3/3/2014 for PPA
        Private _LossHistoryRecords As List(Of QuickQuoteLossHistoryRecord)

        Private _DriverNum As String 'added 4/21/2014 for reconciliation purposes
        Private _HasDriverNameChanged As Boolean 'added 4/21/2014
        Private _HasLastNameChanged As Boolean 'added 5/12/2014
        Private _HasBirthDateChanged As Boolean 'added 5/12/2014

        Private _CanUseViolationNumForAccidentViolationReconciliation As Boolean 'added 4/23/2014
        Private _CanUseLossHistoryNumForLossHistoryReconciliation As Boolean 'added 4/23/2014

        Private _EmploymentInfo As QuickQuoteEmploymentInfo 'added 5/12/2014

        Private _TieringInformation As QuickQuoteTieringInformation 'added 7/28/2014

        Private _Preferred As Boolean 'added 8/1/2014 for testing

        'added 8/26/2014
        Private _MVR_Record_Not_Found As Boolean
        Private _MVR_Record_Clear As Boolean

        Private _Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014

        'added 5/27/2017 (for GAR; may be valid for other LOBs)
        Private _Coverages As List(Of QuickQuoteCoverage)
        Private _TotalCoveragesPremium As String

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 5/22/2019
        Private _AddedDate As String
        Private _EffDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String 'added 7/31/2019

        'added 5/13/2021
        Private _DisplayNum As Integer
        Private _OriginalDisplayNum As Integer

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05201}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05201}")
            End Set
        End Property
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05202}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05202}")
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's LicenseStatus table (0=None; 1=N/A; 2=Valid; 3=Suspended; 4=Revoked; 5=Expired; 6=Not Licensed)</remarks>
        Public Property LicenseStatusId As String '0=None; 1=N/A; 2=Valid; 3=Suspended; 4=Revoked; 5=Expired; 6=Not Licensed
            Get
                Return _LicenseStatusId
            End Get
            Set(value As String)
                _LicenseStatusId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's LicenseStatus table (0=N/A; 1=Rated; 2=NonRated; 3=Excluded; 4=Included; 5=Watch)</remarks>
        Public Property DriverExcludeTypeId As String '0=N/A; 1=Rated; 2=NonRated; 3=Excluded; 4=Included; 5=Watch
            Get
                Return _DriverExcludeTypeId
            End Get
            Set(value As String)
                _DriverExcludeTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RelationshipType table</remarks>
        Public Property RelationshipTypeId As String 'added 7/25/2013 for PPA: None=0; Policyholder=8
            Get
                Return _RelationshipTypeId
            End Get
            Set(value As String)
                _RelationshipTypeId = value
            End Set
        End Property
        'added 1/2/2014 for PPA
        Public Property AccPreventionCourse As String
            Get
                Return _AccPreventionCourse
            End Get
            Set(value As String)
                _AccPreventionCourse = value
                qqHelper.ConvertToShortDate(_AccPreventionCourse)
            End Set
        End Property
        Public Property DefDriverDate As String
            Get
                Return _DefDriverDate
            End Get
            Set(value As String)
                _DefDriverDate = value
                qqHelper.ConvertToShortDate(_DefDriverDate)
            End Set
        End Property
        Public Property DistantStudent As Boolean
            Get
                Return _DistantStudent
            End Get
            Set(value As Boolean)
                _DistantStudent = value
            End Set
        End Property
        Public Property FirstPermit As Boolean
            Get
                Return _FirstPermit
            End Get
            Set(value As Boolean)
                _FirstPermit = value
            End Set
        End Property
        Public Property GoodStudent As Boolean
            Get
                Return _GoodStudent
            End Get
            Set(value As Boolean)
                _GoodStudent = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's LicenseType table</remarks>
        Public Property LicenseTypeId As String 'list
            Get
                Return _LicenseTypeId
            End Get
            Set(value As String)
                _LicenseTypeId = value
            End Set
        End Property
        Public Property MonthsLicensedInState As String
            Get
                Return _MonthsLicensedInState
            End Get
            Set(value As String)
                _MonthsLicensedInState = value
            End Set
        End Property
        Public Property MonthsRevoked As String
            Get
                Return _MonthsRevoked
            End Get
            Set(value As String)
                _MonthsRevoked = value
            End Set
        End Property
        Public Property MotorClub As Boolean
            Get
                Return _MotorClub
            End Get
            Set(value As Boolean)
                _MotorClub = value
            End Set
        End Property
        Public Property MotorMembershipDate As String
            Get
                Return _MotorMembershipDate
            End Get
            Set(value As String)
                _MotorMembershipDate = value
                qqHelper.ConvertToShortDate(_MotorMembershipDate)
            End Set
        End Property
        Public Property MotorcycleTrainingDisc As Boolean
            Get
                Return _MotorcycleTrainingDisc
            End Get
            Set(value As Boolean)
                _MotorcycleTrainingDisc = value
            End Set
        End Property
        Public Property MotorcycleYearsExperience As String
            Get
                Return _MotorcycleYearsExperience
            End Get
            Set(value As String)
                _MotorcycleYearsExperience = value
            End Set
        End Property
        Public Property ReasonExclude As String
            Get
                Return _ReasonExclude
            End Get
            Set(value As String)
                _ReasonExclude = value
            End Set
        End Property
        Public Property SchoolDistance As String
            Get
                Return _SchoolDistance
            End Get
            Set(value As String)
                _SchoolDistance = value
            End Set
        End Property
        Public Property EnolDays As String
            Get
                Return _EnolDays
            End Get
            Set(value As String)
                _EnolDays = value
            End Set
        End Property
        Public Property EnolEmployedByGarage As Boolean
            Get
                Return _EnolEmployedByGarage
            End Get
            Set(value As Boolean)
                _EnolEmployedByGarage = value
            End Set
        End Property
        Public Property EnolGovtBusiness As Boolean
            Get
                Return _EnolGovtBusiness
            End Get
            Set(value As Boolean)
                _EnolGovtBusiness = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Relationship to the Insured radio button list on UI's Extended Non-Owned tab; Named Individuals and Resident Relatives option</remarks>
        Public Property EnolInsuredOrSpouse As Boolean
            Get
                Return _EnolInsuredOrSpouse
            End Get
            Set(value As Boolean)
                _EnolInsuredOrSpouse = value
            End Set
        End Property
        Public Property EnolMedpayExclude As Boolean
            Get
                Return _EnolMedpayExclude
            End Get
            Set(value As Boolean)
                _EnolMedpayExclude = value
            End Set
        End Property
        Public Property EnolMiles As String
            Get
                Return _EnolMiles
            End Get
            Set(value As String)
                _EnolMiles = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Relationship to the Insured radio button list on UI's Extended Non-Owned tab; Named Individual Only option</remarks>
        Public Property EnolNamedInsured As Boolean
            Get
                Return _EnolNamedInsured
            End Get
            Set(value As Boolean)
                _EnolNamedInsured = value
            End Set
        End Property
        Public Property EnolPrimaryLiability As Boolean
            Get
                Return _EnolPrimaryLiability
            End Get
            Set(value As Boolean)
                _EnolPrimaryLiability = value
            End Set
        End Property
        Public Property EnolRegularUse As Boolean
            Get
                Return _EnolRegularUse
            End Get
            Set(value As Boolean)
                _EnolRegularUse = value
            End Set
        End Property
        Public Property EnolTerritoryNum As String
            Get
                Return _EnolTerritoryNum
            End Get
            Set(value As String)
                _EnolTerritoryNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's VehicleUseType table</remarks>
        Public Property EnolVehicleUseTypeId As String 'list
            Get
                Return _EnolVehicleUseTypeId
            End Get
            Set(value As String)
                _EnolVehicleUseTypeId = value
            End Set
        End Property
        Public Property ExtendedNonOwned As Boolean
            Get
                Return _ExtendedNonOwned
            End Get
            Set(value As Boolean)
                _ExtendedNonOwned = value
            End Set
        End Property
        'added 1/14/2014
        Public Property AccidentViolations As List(Of QuickQuoteAccidentViolation)
            Get
                SetParentOfListItems(_AccidentViolations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05203}")
                Return _AccidentViolations
            End Get
            Set(value As List(Of QuickQuoteAccidentViolation))
                _AccidentViolations = value
                SetParentOfListItems(_AccidentViolations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05203}")
            End Set
        End Property
        'added 3/3/2014
        Public Property LossHistoryRecords As List(Of QuickQuoteLossHistoryRecord)
            Get
                SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05204}")
                Return _LossHistoryRecords
            End Get
            Set(value As List(Of QuickQuoteLossHistoryRecord))
                _LossHistoryRecords = value
                SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05204}")
            End Set
        End Property

        Public Property DriverNum As String 'added 4/21/2014 for reconciliation purposes
            Get
                Return _DriverNum
            End Get
            Set(value As String)
                _DriverNum = value
            End Set
        End Property
        Public Property HasDriverNameChanged As Boolean 'added 4/21/2014
            Get
                Return _HasDriverNameChanged
            End Get
            Set(value As Boolean)
                _HasDriverNameChanged = value
            End Set
        End Property
        Public Property HasLastNameChanged As Boolean 'added 5/12/2014
            Get
                Return _HasLastNameChanged
            End Get
            Set(value As Boolean)
                _HasLastNameChanged = value
            End Set
        End Property
        Public Property HasBirthDateChanged As Boolean 'added 5/12/2014
            Get
                Return _HasBirthDateChanged
            End Get
            Set(value As Boolean)
                _HasBirthDateChanged = value
            End Set
        End Property

        Public Property CanUseViolationNumForAccidentViolationReconciliation As Boolean 'added 4/23/2014
            Get
                Return _CanUseViolationNumForAccidentViolationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseViolationNumForAccidentViolationReconciliation = value
            End Set
        End Property
        Public Property CanUseLossHistoryNumForLossHistoryReconciliation As Boolean 'added 4/23/2014
            Get
                Return _CanUseLossHistoryNumForLossHistoryReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLossHistoryNumForLossHistoryReconciliation = value
            End Set
        End Property

        Public Property EmploymentInfo As QuickQuoteEmploymentInfo 'added 5/12/2014
            Get
                SetObjectsParent(_EmploymentInfo)
                Return _EmploymentInfo
            End Get
            Set(value As QuickQuoteEmploymentInfo)
                _EmploymentInfo = value
                SetObjectsParent(_EmploymentInfo)
            End Set
        End Property

        Public Property TieringInformation As QuickQuoteTieringInformation 'added 7/28/2014
            Get
                SetObjectsParent(_TieringInformation)
                Return _TieringInformation
            End Get
            Set(value As QuickQuoteTieringInformation)
                _TieringInformation = value
                SetObjectsParent(_TieringInformation)
            End Set
        End Property

        Public Property Preferred As Boolean 'added 8/1/2014 for testing; didn't seem to make a difference
            Get
                Return _Preferred
            End Get
            Set(value As Boolean)
                _Preferred = value
            End Set
        End Property

        'added 8/26/2014
        Public Property MVR_Record_Not_Found As Boolean
            Get
                Return _MVR_Record_Not_Found
            End Get
            Set(value As Boolean)
                _MVR_Record_Not_Found = value
            End Set
        End Property
        Public Property MVR_Record_Clear As Boolean
            Get
                Return _MVR_Record_Clear
            End Get
            Set(value As Boolean)
                _MVR_Record_Clear = value
            End Set
        End Property

        Public Property Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014
            Get
                SetParentOfListItems(_Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05205}")
                Return _Modifiers
            End Get
            Set(value As List(Of QuickQuoteModifier))
                _Modifiers = value
                SetParentOfListItems(_Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05205}")
            End Set
        End Property

        'added 5/27/2017 (for GAR; may be valid for other LOBs)
        Public Property Coverages As List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05206}")
                Return _Coverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05206}")
            End Set
        End Property
        Public Property TotalCoveragesPremium As String
            Get
                'Return _TotalCoveragesPremium
                Return qqHelper.QuotedPremiumFormat(_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalCoveragesPremium)
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        'added 5/22/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property EffDate As String
            Get
                Return _EffDate
            End Get
            Set(value As String)
                _EffDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
        Public Property AddedImageNum As String 'added 7/31/2019
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        'added 5/13/2021
        Public ReadOnly Property DisplayNum As Integer
            Get
                Return _DisplayNum
            End Get
        End Property
        Public ReadOnly Property OriginalDisplayNum As Integer
            Get
                Return _OriginalDisplayNum
            End Get
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""

            _Name = New QuickQuoteName
            _Name.NameAddressSourceId = "6" 'Driver '7/15/2014 note: should probably change to use static data list
            _Address = New QuickQuoteAddress
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014

            _LicenseStatusId = ""
            _DriverExcludeTypeId = ""
            _RelationshipTypeId = ""
            _AccPreventionCourse = ""
            _DefDriverDate = ""
            _DistantStudent = False
            _FirstPermit = False
            _GoodStudent = False
            _LicenseTypeId = ""
            _MonthsLicensedInState = ""
            _MonthsRevoked = ""
            _MotorClub = False
            _MotorMembershipDate = ""
            _MotorcycleTrainingDisc = False
            _MotorcycleYearsExperience = ""
            _ReasonExclude = ""
            _SchoolDistance = ""
            _EnolDays = ""
            _EnolEmployedByGarage = False
            _EnolGovtBusiness = False
            _EnolInsuredOrSpouse = False
            _EnolMedpayExclude = False
            _EnolMiles = ""
            _EnolNamedInsured = False
            _EnolPrimaryLiability = False
            _EnolRegularUse = False
            _EnolTerritoryNum = ""
            _EnolVehicleUseTypeId = ""
            _ExtendedNonOwned = False
            'added 1/14/2014
            '_AccidentViolations = New List(Of QuickQuoteAccidentViolation)
            _AccidentViolations = Nothing 'added 8/4/2014
            'added 3/3/2014
            '_LossHistoryRecords = New List(Of QuickQuoteLossHistoryRecord)
            _LossHistoryRecords = Nothing 'added 8/4/2014

            _DriverNum = "" 'added 4/21/2014 for reconciliation purposes
            _HasDriverNameChanged = False 'added 4/21/2014
            _HasLastNameChanged = False 'added 5/12/2014
            _HasBirthDateChanged = False 'added 5/12/2014

            _CanUseViolationNumForAccidentViolationReconciliation = False 'added 4/23/2014
            _CanUseLossHistoryNumForLossHistoryReconciliation = False 'added 4/23/2014

            _EmploymentInfo = New QuickQuoteEmploymentInfo 'added 5/12/2014

            _TieringInformation = New QuickQuoteTieringInformation 'added 7/28/2014

            _Preferred = False 'added 8/1/2014 for testing

            'added 8/26/2014
            _MVR_Record_Not_Found = False
            _MVR_Record_Clear = False

            'added 10/16/2014
            '_Modifiers = New List(Of QuickQuoteModifier)
            _Modifiers = Nothing

            'added 5/27/2017 (for GAR; may be valid for other LOBs)
            _Coverages = Nothing
            _TotalCoveragesPremium = ""

            _DetailStatusCode = "" 'added 5/15/2019

            'added 5/22/2019
            _AddedDate = ""
            _EffDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = "" 'added 7/31/2019

            'added 5/13/2021
            _DisplayNum = 0
            _OriginalDisplayNum = 0

        End Sub
        Public Function HasValidDriverNum() As Boolean 'added 4/21/2014 for reconciliation purposes
            'If _DriverNum <> "" AndAlso IsNumeric(_DriverNum) = True AndAlso CInt(_DriverNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_DriverNum)
        End Function
        'added 4/23/2014 for accidentViolation reconciliation
        Public Sub ParseThruAccidentViolations()
            If _AccidentViolations IsNot Nothing AndAlso _AccidentViolations.Count > 0 Then
                For Each av As QuickQuoteAccidentViolation In _AccidentViolations
                    '4/23/2014 note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseViolationNumForAccidentViolationReconciliation = False Then
                        If av.HasValidViolationNum = True Then
                            _CanUseViolationNumForAccidentViolationReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        'added 4/23/2014 for lossHistory reconciliation
        Public Sub ParseThruLossHistories()
            If _LossHistoryRecords IsNot Nothing AndAlso _LossHistoryRecords.Count > 0 Then
                For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                    '4/23/2014 note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseLossHistoryNumForLossHistoryReconciliation = False Then
                        If lh.HasValidLossHistoryNum = True Then
                            _CanUseLossHistoryNumForLossHistoryReconciliation = True
                            'Exit For 'removed 7/3/2014; needs to keep going so ParseThruLossHistoryDetails can be called for each one
                        End If
                    End If
                    lh.ParseThruLossHistoryDetails() 'added 7/3/2014
                Next
            End If
        End Sub

        'added 5/27/2017 for GAR (may also be used for other LOBs)
        Public Sub ParseThruCoverages()
            If _Coverages IsNot Nothing AndAlso _Coverages.Count > 0 Then
                For Each cov As QuickQuoteCoverage In _Coverages
                    TotalCoveragesPremium = qqHelper.getSum(_TotalCoveragesPremium, cov.FullTermPremium)
                    Select Case cov.CoverageCodeId
                        Case "10067" 'Combo: Drive Other Car Liability (CAP, GAR)

                        Case "10068" 'Combo: Drive Other Car Medical Payments (CAP, GAR)

                        Case "10069" 'Combo: Drive Other Car Uninsured Motorist Liability (CAP, GAR)

                        Case "10070" 'Combo: Drive Other Car Underinsured Motorist Liability (CAP, GAR)

                        Case "10071" 'CheckBox: Drive Other Car Comprehensive (CAP, GAR)

                        Case "10072" 'CheckBox: Drive Other Car Collision (CAP, GAR)


                    End Select
                Next
            End If
        End Sub

        'added 5/13/2021
        Protected Friend Sub Set_DisplayNum(ByVal dNum As Integer)
            _DisplayNum = dNum
            If _OriginalDisplayNum <= 0 Then
                _OriginalDisplayNum = _DisplayNum
            End If
        End Sub

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.RelationshipTypeId <> "" Then
                    Dim rel As String = ""
                    rel = "RelationshipTypeId: " & Me.RelationshipTypeId
                    Dim relType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, Me.RelationshipTypeId)
                    If relType <> "" Then
                        rel &= " (" & relType & ")"
                    End If
                    str = qqHelper.appendText(str, rel, vbCrLf)
                End If
                If Me.DriverExcludeTypeId <> "" Then
                    Dim exc As String = ""
                    exc = "DriverExcludeTypeId: " & Me.DriverExcludeTypeId
                    Dim excType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, Me.DriverExcludeTypeId)
                    If excType <> "" Then
                        exc &= " (" & excType & ")"
                    End If
                    str = qqHelper.appendText(str, exc, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If

                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _Phones IsNot Nothing Then
                        If _Phones.Count > 0 Then
                            For Each ph As QuickQuotePhone In _Phones
                                ph.Dispose()
                                ph = Nothing
                            Next
                            _Phones.Clear()
                        End If
                        _Phones = Nothing
                    End If
                    If _Emails IsNot Nothing Then
                        If _Emails.Count > 0 Then
                            For Each em As QuickQuoteEmail In _Emails
                                em.Dispose()
                                em = Nothing
                            Next
                            _Emails.Clear()
                        End If
                        _Emails = Nothing
                    End If

                    If _LicenseStatusId IsNot Nothing Then
                        _LicenseStatusId = Nothing
                    End If
                    If _DriverExcludeTypeId IsNot Nothing Then
                        _DriverExcludeTypeId = Nothing
                    End If
                    If _RelationshipTypeId IsNot Nothing Then
                        _RelationshipTypeId = Nothing
                    End If
                    If _AccPreventionCourse IsNot Nothing Then
                        _AccPreventionCourse = Nothing
                    End If
                    If _DefDriverDate IsNot Nothing Then
                        _DefDriverDate = Nothing
                    End If
                    If _DistantStudent <> Nothing Then
                        _DistantStudent = Nothing
                    End If
                    If _FirstPermit <> Nothing Then
                        _FirstPermit = Nothing
                    End If
                    If _GoodStudent <> Nothing Then
                        _GoodStudent = Nothing
                    End If
                    If _LicenseTypeId IsNot Nothing Then
                        _LicenseTypeId = Nothing
                    End If
                    If _MonthsLicensedInState IsNot Nothing Then
                        _MonthsLicensedInState = Nothing
                    End If
                    If _MonthsRevoked IsNot Nothing Then
                        _MonthsRevoked = Nothing
                    End If
                    If _MotorClub <> Nothing Then
                        _MotorClub = Nothing
                    End If
                    If _MotorMembershipDate IsNot Nothing Then
                        _MotorMembershipDate = Nothing
                    End If
                    If _MotorcycleTrainingDisc <> Nothing Then
                        _MotorcycleTrainingDisc = Nothing
                    End If
                    If _MotorcycleYearsExperience IsNot Nothing Then
                        _MotorcycleYearsExperience = Nothing
                    End If
                    If _ReasonExclude IsNot Nothing Then
                        _ReasonExclude = Nothing
                    End If
                    If _SchoolDistance IsNot Nothing Then
                        _SchoolDistance = Nothing
                    End If
                    If _EnolDays IsNot Nothing Then
                        _EnolDays = Nothing
                    End If
                    If _EnolEmployedByGarage <> Nothing Then
                        _EnolEmployedByGarage = Nothing
                    End If
                    If _EnolGovtBusiness <> Nothing Then
                        _EnolGovtBusiness = Nothing
                    End If
                    If _EnolInsuredOrSpouse <> Nothing Then
                        _EnolInsuredOrSpouse = Nothing
                    End If
                    If _EnolMedpayExclude <> Nothing Then
                        _EnolMedpayExclude = Nothing
                    End If
                    If _EnolMiles IsNot Nothing Then
                        _EnolMiles = Nothing
                    End If
                    If _EnolNamedInsured <> Nothing Then
                        _EnolNamedInsured = Nothing
                    End If
                    If _EnolPrimaryLiability <> Nothing Then
                        _EnolPrimaryLiability = Nothing
                    End If
                    If _EnolRegularUse <> Nothing Then
                        _EnolRegularUse = Nothing
                    End If
                    If _EnolTerritoryNum IsNot Nothing Then
                        _EnolTerritoryNum = Nothing
                    End If
                    If _EnolVehicleUseTypeId IsNot Nothing Then
                        _EnolVehicleUseTypeId = Nothing
                    End If
                    If _ExtendedNonOwned <> Nothing Then
                        _ExtendedNonOwned = Nothing
                    End If
                    'added 1/14/2014
                    If _AccidentViolations IsNot Nothing Then
                        If _AccidentViolations.Count > 0 Then
                            For Each av As QuickQuoteAccidentViolation In _AccidentViolations
                                av.Dispose()
                                av = Nothing
                            Next
                            _AccidentViolations.Clear()
                        End If
                        _AccidentViolations = Nothing
                    End If
                    'added 3/3/2014
                    If _LossHistoryRecords IsNot Nothing Then
                        If _LossHistoryRecords.Count > 0 Then
                            For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                                lh.Dispose()
                                lh = Nothing
                            Next
                            _LossHistoryRecords.Clear()
                        End If
                        _LossHistoryRecords = Nothing
                    End If

                    If _DriverNum IsNot Nothing Then 'added 4/21/2014 for reconciliation purposes
                        _DriverNum = Nothing
                    End If
                    If _HasDriverNameChanged <> Nothing Then 'added 4/21/2014
                        _HasDriverNameChanged = Nothing
                    End If
                    _HasLastNameChanged = Nothing 'added 5/12/2014
                    _HasBirthDateChanged = Nothing 'added 5/12/2014

                    If _CanUseViolationNumForAccidentViolationReconciliation <> Nothing Then 'added 4/23/2014
                        _CanUseViolationNumForAccidentViolationReconciliation = Nothing
                    End If
                    If _CanUseLossHistoryNumForLossHistoryReconciliation <> Nothing Then 'added 4/23/2014
                        _CanUseLossHistoryNumForLossHistoryReconciliation = Nothing
                    End If

                    If _EmploymentInfo IsNot Nothing Then
                        _EmploymentInfo.Dispose()
                        _EmploymentInfo = Nothing
                    End If

                    If _TieringInformation IsNot Nothing Then 'added 7/28/2014
                        _TieringInformation.Dispose()
                        _TieringInformation = Nothing
                    End If

                    _Preferred = Nothing 'added 8/1/2014 for testing

                    'added 8/26/2014
                    _MVR_Record_Not_Found = Nothing
                    _MVR_Record_Clear = Nothing

                    If _Modifiers IsNot Nothing Then 'added 10/16/2014
                        If _Modifiers.Count > 0 Then
                            For Each m As QuickQuoteModifier In _Modifiers
                                m.Dispose()
                                m = Nothing
                            Next
                            _Modifiers.Clear()
                        End If
                        _Modifiers = Nothing
                    End If

                    'added 5/27/2017 (for GAR; may be valid for other LOBs)
                    qqHelper.DisposeCoverages(_Coverages)
                    qqHelper.DisposeString(_TotalCoveragesPremium)

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 5/22/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_EffDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum) 'added 7/31/2019

                    'added 5/13/2021
                    _DisplayNum = Nothing
                    _OriginalDisplayNum = Nothing

                    MyBase.Dispose() 'added 8/4/2014
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
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
