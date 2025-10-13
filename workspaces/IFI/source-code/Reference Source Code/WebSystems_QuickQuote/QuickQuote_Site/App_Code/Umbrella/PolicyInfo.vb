Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    ''' <summary>
    ''' objects used to store policy information (on Umbrella Policies)
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteUnderlyingPolicy object (<see cref="QuickQuoteUnderlyingPolicy"/>) as a list</remarks>
    Public Class PolicyInfo 'added 4/20/2020 for PUP/FUP; previously had a class of the same name that was re-named to QuickQuoteThirdPartyPolicyInfo
        Inherits TrackedPolicyItem
        'Inherits QuickQuoteBaseGenericObject(Of Object)
        'Implements IDisposable
        Implements IPersonalAutoPolicy, ICommercialAutoPolicy, IFarmPolicy, IWorkersCompensationPolicy, IHomePolicy
        Implements IXmlSerializable, IReconcilable

        Public Const WCP_COMBINED_ACCIDENT_POLICY_EMPLOYEE_LIMIT_COVERAGE_CODE_ID As Integer = 10124

        Private _ExpirationDate As String

        Protected _listMetadata As New ConditionalWeakTable(Of IEnumerable(Of BasePolicyItem), ListMetaData)
        Private _CombinedSingleLimit As String
        Private _PersonalLiabilityLimit As String
        Private _EachAccident As String
        Private _DiseasePolicyLimit As String
        Private _DiseaseEachEmployee As String
        Private _BodilyInjuryLimitA As String
        Private _BodilyInjuryLimitB As String
        Private _PropertyDamageLimit As String
        Private _CombinedAccidentPolicyEmployeeLimitId As String
        'Private _OccurrenceLiabilityLimit As String
        'Private _AggregateLiabilityLimit As String
        Public Property PrimaryPolicyNumber As String
        'Public Property PersonalLiabilityLimit As String 'int
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                _qqHelper.ConvertToShortDate(_ExpirationDate)
            End Set
        End Property
        Public Property Company As String
        Public Property CompanyTypeId As String 'static data

        Public Property PolicyTypeId As String
            Get
                Return TypeId
            End Get
            Set(value As String)
                TypeId = value
            End Set
        End Property

        Public Property DiseaseEachEmployee As String Implements IWorkersCompensationPolicy.DiseaseEachEmployee
            Get
                Return _DiseaseEachEmployee
            End Get
            Set
                _DiseaseEachEmployee = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property DiseasePolicyLimit As String Implements IWorkersCompensationPolicy.DiseasePolicyLimit
            Get
                Return _DiseasePolicyLimit
            End Get
            Set
                _DiseasePolicyLimit = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property EachAccident As String Implements IWorkersCompensationPolicy.EachAccident
            Get
                Return _EachAccident
            End Get
            Set
                _EachAccident = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property
        Public Property CombinedAccidentPolicyEmployeeLimitId As String Implements IWorkersCompensationPolicy.CombinedAccidentPolicyEmployeeLimitId
            Get
                Return _CombinedAccidentPolicyEmployeeLimitId
            End Get
            Set
                _CombinedAccidentPolicyEmployeeLimitId = Value
            End Set
        End Property

        Public Property PersonalLiabilities As List(Of PersonalLiability) Implements IHomePolicy.PersonalLiabilities, IFarmPolicy.PersonalLiabilities
        Public Property PersonalLiabilityLimit As String Implements IHomePolicy.PersonalLiabilityLimit, IFarmPolicy.PersonalLiabilityLimit
            Get
                Return _PersonalLiabilityLimit
            End Get
            Set
                _PersonalLiabilityLimit = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property CombinedSingleLimit As String Implements IPersonalAutoPolicy.CombinedSingleLimit, ICommercialAutoPolicy.CombinedSingleLimit
            Get
                Return _CombinedSingleLimit
            End Get
            Set
                _CombinedSingleLimit = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property Drivers As List(Of Driver) Implements IPersonalAutoPolicy.Drivers, ICommercialAutoPolicy.Drivers
        Public Property Vehicles As List(Of Vehicle) Implements IPersonalAutoPolicy.Vehicles, ICommercialAutoPolicy.Vehicles
        Public Property BodilyInjuryLimitA As String Implements IPersonalAutoPolicy.BodilyInjuryLimitA
            Get
                Return _BodilyInjuryLimitA
            End Get
            Set
                _BodilyInjuryLimitA = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property BodilyInjuryLimitB As String Implements IPersonalAutoPolicy.BodilyInjuryLimitB
            Get
                Return _BodilyInjuryLimitB
            End Get
            Set
                _BodilyInjuryLimitB = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property PropertyDamageLimit As String Implements IPersonalAutoPolicy.PropertyDamageLimit
            Get
                Return _PropertyDamageLimit
            End Get
            Set
                _PropertyDamageLimit = _qqHelper.DiamondAmountFormat(Value)
            End Set
        End Property

        Public Property YouthfulOperators As List(Of YouthfulOperator) Implements IPersonalAutoPolicy.YouthfulOperators
        Public Property InvestmentProperties As List(Of Location) Implements IHomePolicy.InvestmentProperties
        Public Property MiscellaneousLiabilities As List(Of MiscellaneousLiability) Implements IHomePolicy.MiscellaneousLiabilities
        Public Property ProfessionalLiabilities As List(Of ProfessionalLiability) Implements IHomePolicy.ProfessionalLiabilities
        Public Property RecreationalVehicles As List(Of RecreationalVehicle) Implements IHomePolicy.RecreationalVehicles
        Public Property Watercrafts As List(Of Watercraft) Implements IHomePolicy.Watercrafts
        Public Property FarmLiabilities As List(Of FarmLiability) Implements IFarmPolicy.FarmLiabilities
        'Public Property OccurrenceLiabilityLimit As String Implements IFarmPolicy.AggregateLiabilityLimit  'not needed 5/20/2021
        '    Get
        '        Return _OccurrenceLiabilityLimit
        '    End Get
        '    Set
        '        _OccurrenceLiabilityLimit = _qqHelper.DiamondAmountFormat(Value)
        '    End Set
        'End Property

        'Public Property AggregateLiabilityLimit As String Implements IFarmPolicy.OccurrenceLiabilityLimit   'not needed 5/20/2021
        '    Get
        '        Return _AggregateLiabilityLimit
        '    End Get
        '    Set
        '        _AggregateLiabilityLimit = _qqHelper.DiamondAmountFormat(Value)
        '    End Set
        'End Property
#Region "deprecated"
        'Private _DetailStatusCode As String
        'Private _PolicyInfoNum As String 'for reconciliation
        'Private _Company As String
        'Private _CompanyTypeId As String 'static data
        'Private _EffectiveDate As String
        'Private _ExpirationDate As String
        'Private _LinkNumber As String
        'Private _PersonalLiabilityLimit As String 'int
        'Private _PolicyTypeId As String 'static data
        'Private _PrimaryPolicyNumber As String

        'Private _Locations As List(Of QuickQuoteLocation)
        'Private _CanUseLocationNumForLocationReconciliation As Boolean

        'Private _PremiumFullterm As String      

        'Public Property DetailStatusCode As String
        '    Get
        '        Return _DetailStatusCode
        '    End Get
        '    Set(value As String)
        '        _DetailStatusCode = value
        '    End Set
        'End Property
        'Public Property PolicyInfoNum As String 'for reconciliation
        '    Get
        '        Return _PolicyInfoNum
        '    End Get
        '    Set(value As String)
        '        _PolicyInfoNum = value
        '    End Set
        'End Property
        'Public Property Company As String
        '    Get
        '        Return _Company
        '    End Get
        '    Set(value As String)
        '        _Company = value
        '    End Set
        'End Property
        'Public Property CompanyTypeId As String 'static data
        '    Get
        '        Return _CompanyTypeId
        '    End Get
        '    Set(value As String)
        '        _CompanyTypeId = value
        '    End Set
        'End Property
        'Public Property EffectiveDate As String
        '    Get
        '        Return _EffectiveDate
        '    End Get
        '    Set(value As String)
        '        _EffectiveDate = value
        '        qqHelper.ConvertToShortDate(_EffectiveDate)
        '    End Set
        'End Property
        'Public Property ExpirationDate As String
        '    Get
        '        Return _ExpirationDate
        '    End Get
        '    Set(value As String)
        '        _ExpirationDate = value
        '        qqHelper.ConvertToShortDate(_ExpirationDate)
        '    End Set
        'End Property
        'Public Property LinkNumber As String
        '    Get
        '        Return _LinkNumber
        '    End Get
        '    Set(value As String)
        '        _LinkNumber = value
        '    End Set
        'End Property
        'Public Property PersonalLiabilityLimit As String 'int
        '    Get
        '        Return _PersonalLiabilityLimit
        '    End Get
        '    Set(value As String)
        '        _PersonalLiabilityLimit = value
        '    End Set
        'End Property
        'Public Property PolicyTypeId As String 'static data
        '    Get
        '        Return _PolicyTypeId
        '    End Get
        '    Set(value As String)
        '        _PolicyTypeId = value
        '    End Set
        'End Property
        'Public Property PrimaryPolicyNumber As String
        '    Get
        '        Return _PrimaryPolicyNumber
        '    End Get
        '    Set(value As String)
        '        _PrimaryPolicyNumber = value
        '    End Set
        'End Property

        'Public Property Locations As List(Of QuickQuoteLocation)
        '    Get
        '        SetParentOfListItems(_Locations, "{D3S6E852-B1RC-4N73-685N-34C1E9E2B9I2}")
        '        Return _Locations
        '    End Get
        '    Set(value As List(Of QuickQuoteLocation))
        '        _Locations = value
        '        SetParentOfListItems(_Locations, "{D3S6E852-B1RC-4N73-685N-34C1E9E2B9I2}")
        '    End Set
        'End Property
        'Public Property CanUseLocationNumForLocationReconciliation As Boolean
        '    Get
        '        Return _CanUseLocationNumForLocationReconciliation
        '    End Get
        '    Set(value As Boolean)
        '        _CanUseLocationNumForLocationReconciliation = value
        '    End Set
        'End Property


        'Public Property PremiumFullterm As String
        '    Get
        '        Return _PremiumFullterm
        '    End Get
        '    Set(value As String)
        '        _PremiumFullterm = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PremiumFullterm)
        '    End Set
        'End Property



        'Public Property OccurrenceLimit As String
        'Public Property AggregateLimit As String

        'Public Property EachAccidentLimit As String
        'Public Property DiseasePolicyLimit As String
        'Public Property DiseaseEachEmployee As String

        'Public Sub New()
        '    MyBase.New()
        '    'SetDefaults()
        'End Sub

        'Public Sub New(Parent As QuickQuoteObject)
        '    MyBase.New()
        '    SetParent = Parent
        '    'SetDefaults()
        'End Sub
        'Private Sub SetDefaults()
        '    _DetailStatusCode = ""
        '    _PolicyInfoNum = "" 'for reconciliation
        '    _Company = ""
        '    _CompanyTypeId = "" 'static data
        '    _EffectiveDate = ""
        '    _ExpirationDate = ""
        '    _LinkNumber = ""
        '    _PersonalLiabilityLimit = ""
        '    _PolicyTypeId = "" 'static data
        '    _PrimaryPolicyNumber = ""
        '    '_Drivers = Nothing
        '    '_CanUseDriverNumForDriverReconciliation = False
        '    _Locations = Nothing
        '    _CanUseLocationNumForLocationReconciliation = False
        '    '_Vehicles = Nothing
        '    '_CanUseVehicleNumForVehicleReconciliation = False
        '    '_PersonalLiabilities = Nothing
        '    '_CanUsePersonalLiabilityNumForPersonalLiabilityReconciliation = False
        '    ' _PremiumFullterm = ""

        'End Sub
        'Public Function HasValidPolicyInfoNum() As Boolean 'added for reconciliation purposes
        '    Return qqHelper.IsValidQuickQuoteIdOrNum(_PolicyInfoNum)
        'End Function
        'Public Overridable Sub RunParseMethods()
        '    ParseThruLocations()
        'End Sub

        'Private Sub ParseThruLocations()
        '    'If _Locations IsNot Nothing AndAlso _Locations.Count > 0 Then
        '    '    For Each l As QuickQuoteLocation In _Locations
        '    '        'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
        '    '        If _CanUseLocationNumForLocationReconciliation = False Then
        '    '            If l.HasValidLocationNum = True Then
        '    '                _CanUseLocationNumForLocationReconciliation = True
        '    '                'Exit For 'needs to keep going so child Parse routines can be called
        '    '            End If
        '    '        End If
        '    '        l.RunParseMethods()
        '    '    Next
        '    'End If

        '    'note: this logic was taken from QuickQuoteObject.FinalizeQuickQuote_Original_SingleState()
        '    Dim hasProcessedLocations As Boolean = False

        '    'for getting diamondNums for packageParts; could also include None at the front of list and remove normal call below
        '    'Dim packagePartTypes As New List(Of QuickQuoteXML.QuickQuotePackagePartType)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.Package)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.InlandMarine)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.Crime)
        '    'packagePartTypes.Add(QuickQuoteXML.QuickQuotePackagePartType.Garage)

        '    If _Locations IsNot Nothing AndAlso _Locations.Count > 0 Then 'updated 7/18/2018 from _Locations; could have used QuickQuoteObject's Locations property, but we shouldn't need to go through Parent logic
        '        For Each Loc As QuickQuoteLocation In _Locations
        '            Loc.HasConvertedCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '            Loc.HasConvertedModifiers = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '            Loc.HasConvertedScheduledCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '            Loc.HasConvertedSectionCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '            Loc.HasConvertedIncidentalDwellingCoverages = False 'added 2/25/2015
        '            Loc.HasConvertedIncomeLosses = False 'added 2/26/2015
        '            Loc.ParseThruClassifications() 'added 8/20/2012
        '            If hasProcessedLocations = False Then 'added IF 4/13/2015
        '                Loc.ParseThruCoverages()
        '            End If
        '            Loc.ParseThruGLClassifications() 'added 8/22/2012
        '            If hasProcessedLocations = False Then 'added IF 4/13/2015
        '                Loc.ParseThruScheduledCoverages() 'added 3/20/2013 for CPR (specific to Property in the Open)
        '            End If
        '            Loc.ParseThruModifiers() 'added 7/31/2013 for HOM (specific to credits and surcharges)
        '            Loc.ParseThruSectionCoverages() 'added 8/1/2013 for HOM
        '            Loc.ParseThruInlandMarines() 'added 8/6/2013 for HOM
        '            'Loc.ParseThruRvWatercrafts(VersionAndLobInfo.Operators) 'added 8/6/2013 for HOM; 2/20/2014 - rv.HasConvertedCoverages being reset in parse method (for 1st rate; was previously only being done in FinalizeQuickQuoteLight); updated 10/30/2014 for policyLevelOperators and RvWatercraft; updated 7/21/2018 to pass in VersionAndLobInfo Prop... could use QuickQuoteObject's Prop but wouldn't need to go through Parent logic
        '            Loc.ParseThruRvWatercrafts() 'added 8/6/2013 for HOM; 2/20/2014 - rv.HasConvertedCoverages being reset in parse method (for 1st rate; was previously only being done in FinalizeQuickQuoteLight); updated 10/30/2014 for policyLevelOperators and RvWatercraft; updated 7/21/2018 to pass in VersionAndLobInfo Prop... could use QuickQuoteObject's Prop but wouldn't need to go through Parent logic
        '            Loc.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation
        '            Loc.ParseThruExclusions() 'added 10/14/2014 for reconciliation
        '            Loc.ParseThruPolicyUnderwritings() 'added 10/15/2014 for reconciliation
        '            Loc.ParseThruIncidentalDwellingCoverages() 'added 2/25/2015
        '            Loc.ParseThruAcreages() 'added 2/26/2015 for Farm
        '            Loc.ParseThruIncomeLosses() 'added 2/26/2015 for Farm
        '            Loc.ParseThruResidentNames() 'added 2/26/2015 for Farm

        '            'should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
        '            If _CanUseLocationNumForLocationReconciliation = False Then
        '                If Loc.HasValidLocationNum = True Then
        '                    _CanUseLocationNumForLocationReconciliation = True
        '                End If
        '            End If
        '            'shouldn't do anything since the other diamondNums aren't set yet; will need to remove code above if None is added to PackagePartTypes list
        '            'If packagePartTypes IsNot Nothing AndAlso packagePartTypes.Count > 0 Then
        '            '    For Each ppt As QuickQuoteXML.QuickQuotePackagePartType In packagePartTypes
        '            '        If CanUseLocationNumFlagForPackagePartType(ppt) = False Then
        '            '            If Loc.HasValidLocationNum(ppt) = True Then
        '            '                SetCanUseLocationNumFlagForPackagePartType(True, ppt)
        '            '            End If
        '            '        End If
        '            '    Next
        '            'End If

        '            If Loc.Buildings IsNot Nothing AndAlso Loc.Buildings.Count > 0 Then
        '                Loc.HasBuilding = True 'added 9/10/2012 PM for validation purposes
        '                For Each build As QuickQuoteBuilding In Loc.Buildings
        '                    build.HasConvertedClassifications = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '                    build.HasConvertedCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '                    build.HasConvertedScheduledCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
        '                    build.HasConvertedModifiers = False 'added 6/15/2015 for Farm
        '                    build.HasConvertedOptionalCoverageEs = False 'added 6/24/2015 for Farm
        '                    build.EffectiveDate = EffectiveDate '3/9/2017 - BOP stuff; updated from private variable to public property
        '                    'build.HasBusinessMasterEnhancement = HasBusinessMasterEnhancement 'updated 7/18/2018 from private variable to public property
        '                    'will just be backup for BOP since everything will be handled from page level logic (Each Location will get the PropDed set on the 1st building; policy level will get the value from the 1st location); will also ensure that building level continues to have value in the case Proposal uses it
        '                    'If qqHelper.IsPositiveIntegerString(build.PropertyDeductibleId) = False Then
        '                    '    If LobType = QuickQuoteLobType.CommercialBOP AndAlso qqHelper.IsPositiveIntegerString(Loc.PropertyDeductibleId) = True Then 'updated 7/18/2018 from private variable to public property for LobType
        '                    '        build.PropertyDeductibleId = Loc.PropertyDeductibleId
        '                    '    ElseIf qqHelper.IsPositiveIntegerString(PropertyDeductibleId) = True Then 'updated 7/18/2018 from private variable to public property
        '                    '        build.PropertyDeductibleId = PropertyDeductibleId
        '                    '    End If
        '                    'End If
        '                    'If LobType = QuickQuoteLobType.CommercialBOP AndAlso qqHelper.IsPositiveIntegerString(Loc.PropertyDeductibleId) = False Then 'updated 7/18/2018 from private variable to public property for LobType
        '                    '    If qqHelper.IsPositiveIntegerString(build.PropertyDeductibleId) = True Then
        '                    '        Loc.PropertyDeductibleId = build.PropertyDeductibleId
        '                    '    ElseIf qqHelper.IsPositiveIntegerString(PropertyDeductibleId) = True Then 'updated 7/18/2018 from private variable to public property
        '                    '        Loc.PropertyDeductibleId = PropertyDeductibleId
        '                    '    End If
        '                    'End If
        '                    'If qqHelper.IsPositiveIntegerString(PropertyDeductibleId) = False Then 'updated 7/18/2018 from private variable to public property
        '                    '    If LobType = QuickQuoteLobType.CommercialBOP AndAlso qqHelper.IsPositiveIntegerString(Loc.PropertyDeductibleId) = True Then 'updated 7/18/2018 from private variable to public property for LobType
        '                    '        PropertyDeductibleId = Loc.PropertyDeductibleId
        '                    '    ElseIf qqHelper.IsPositiveIntegerString(build.PropertyDeductibleId) = True Then
        '                    '        PropertyDeductibleId = build.PropertyDeductibleId
        '                    '    End If
        '                    'End If
        '                    'found out CPR can store protection class at location level (BOP using building level)
        '                    If Loc.ProtectionClassId = "" AndAlso build.ProtectionClassId <> "" Then
        '                        Loc.ProtectionClassId = build.ProtectionClassId
        '                    ElseIf Loc.ProtectionClassId <> "" AndAlso build.ProtectionClassId = "" Then
        '                        build.ProtectionClassId = Loc.ProtectionClassId
        '                    End If
        '                    'similar logic for feet to fire hydrant and miles to fire department (like protection class id)
        '                    If Loc.FeetToFireHydrant = "" AndAlso build.FeetToFireHydrant <> "" Then
        '                        Loc.FeetToFireHydrant = build.FeetToFireHydrant
        '                    ElseIf Loc.FeetToFireHydrant <> "" AndAlso build.FeetToFireHydrant = "" Then
        '                        build.FeetToFireHydrant = Loc.FeetToFireHydrant
        '                    End If
        '                    If Loc.MilesToFireDepartment = "" AndAlso build.MilesToFireDepartment <> "" Then
        '                        Loc.MilesToFireDepartment = build.MilesToFireDepartment
        '                    ElseIf Loc.MilesToFireDepartment <> "" AndAlso build.MilesToFireDepartment = "" Then
        '                        build.MilesToFireDepartment = Loc.MilesToFireDepartment
        '                    End If
        '                    If hasProcessedLocations = False Then 'added IF 4/13/2015
        '                        build.ParseThruCoverages()
        '                    End If
        '                    build.ParseThruClassifications()
        '                    If hasProcessedLocations = False Then 'added IF 4/13/2015
        '                        build.ParseThruScheduledCoverages() 'added 9/27/2012 for CPR
        '                    End If
        '                    build.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation
        '                    build.ParseThruModifiers() 'added 6/15/2015 for Farm
        '                    build.ParseThruOptionalCoverageEs() 'added 6/24/2015 for Farm
        '                    build.Calculate_CPR_Covs_TotalBuildingPremium() 'added 10/16/2012 for CPR
        '                    build.Calculate_CPR_Covs_TotalBuilding_EQ_Premium() 'added 11/15/2012 for CPR
        '                    build.Calculate_CPR_Covs_With_EQ_Premium() 'added 11/26/2012 for CPR

        '                    Loc.BuildingsTotal_PremiumFullterm = qqHelper.getSum(Loc.BuildingsTotal_PremiumFullterm, build.PremiumFullterm) 'might want to initialize before this FOR loop

        '                    'added 4/23/2014; should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
        '                    If Loc.CanUseFarmBarnBuildingNumForBuildingReconciliation = False Then
        '                        If build.HasValidFarmBarnBuildingNum = True Then
        '                            Loc.CanUseFarmBarnBuildingNumForBuildingReconciliation = True
        '                        End If
        '                    End If
        '                    'shouldn't do anything since the other diamondNums aren't set yet
        '                    'If packagePartTypes IsNot Nothing AndAlso packagePartTypes.Count > 0 Then
        '                    '    For Each ppt As QuickQuoteXML.QuickQuotePackagePartType In packagePartTypes
        '                    '        If Loc.CanUseFarmBarnBuildingNumFlagForPackagePartType(ppt) = False Then
        '                    '            If build.HasValidFarmBarnBuildingNum(ppt) = True Then
        '                    '                Loc.SetCanUseFarmBarnBuildingNumFlagForPackagePartType(True, ppt)
        '                    '            End If
        '                    '        End If
        '                    '    Next
        '                    'End If

        '                    'If build.BuildingClassifications IsNot Nothing AndAlso build.BuildingClassifications.Count > 0 Then
        '                    '    For Each bc As QuickQuoteClassification In build.BuildingClassifications
        '                    '        If bc IsNot Nothing Then
        '                    '            bc.QuoteEffectiveDate = EffectiveDate 'updated from Private variable To Public Property
        '                    '        End If
        '                    '    Next
        '                    'End If
        '                Next
        '            End If

        '            'to compare before/after address
        '            Loc.Set_OriginalAddress(qqHelper.CloneObject(Loc.Address))

        '            'added 11/1/2017
        '            Loc.Set_EquipmentBreakdownDeductibleIdBackup(Loc.EquipmentBreakdownDeductibleId)

        '            'only do it at singleState level or top level if there are no subQuotes
        '            'If QuoteLevel = helper.QuoteLevel.TopLevel AndAlso (TopLevelQuoteInfo.MultiStateQuotes Is Nothing OrElse TopLevelQuoteInfo.MultiStateQuotes.Count = 0) Then
        '            '    If Loc.Address IsNot Nothing AndAlso Loc.Address.HasData = False AndAlso System.Enum.IsDefined(GetType(helper.QuickQuoteState), QuickQuoteState) = True AndAlso QuickQuoteState <> helper.QuickQuoteState.None AndAlso Loc.Address.QuickQuoteState <> QuickQuoteState AndAlso helper.QuoteHasState(Me, Loc.Address.QuickQuoteState) = False Then
        '            '        Loc.Address.QuickQuoteState = QuickQuoteState
        '            '    End If
        '            'ElseIf QuoteLevel = helper.QuoteLevel.StateLevel Then
        '            '    If Loc.Address IsNot Nothing AndAlso Loc.Address.HasData = False AndAlso System.Enum.IsDefined(GetType(helper.QuickQuoteState), QuickQuoteState) = True AndAlso QuickQuoteState <> helper.QuickQuoteState.None AndAlso Loc.Address.QuickQuoteState <> QuickQuoteState Then
        '            '        Loc.Address.QuickQuoteState = QuickQuoteState
        '            '    End If
        '            'End If
        '        Next
        '    End If
        'End Sub

        'Public Overrides Function ToString() As String
        '    Dim str As String = ""
        '    If Me IsNot Nothing Then
        '        If String.IsNullOrWhiteSpace(Me.PrimaryPolicyNumber) = False Then
        '            str = qqHelper.appendText(str, "PrimaryPolicyNumber: " & Me.PrimaryPolicyNumber, vbCrLf)
        '        End If
        '        If String.IsNullOrWhiteSpace(Me.LinkNumber) = False Then
        '            str = qqHelper.appendText(str, "LinkNumber: " & Me.LinkNumber, vbCrLf)
        '        End If

        '        If Me.Locations IsNot Nothing AndAlso Me.Locations.Count > 0 Then
        '            str = qqHelper.appendText(str, Me.Locations.Count.ToString & " Locations", vbCrLf)
        '        End If


        '        If qqHelper.IsPositiveDecimalString(Me.PremiumFullterm) = False Then
        '            str = qqHelper.appendText(str, "PremiumFullterm: " & Me.PremiumFullterm, vbCrLf)
        '        End If

        '        '''TODO: add  other plicy info items
        '        '''
        '    Else
        '        str = "Nothing"
        '    End If
        '    Return str
        'End Function


        '#Region "IDisposable Support"
        '        Private disposedValue As Boolean ' To detect redundant calls

        '        ' IDisposable
        '        'Protected Overridable Sub Dispose(disposing As Boolean)
        '        Protected Overloads Sub Dispose(disposing As Boolean)
        '            If Not Me.disposedValue Then
        '                If disposing Then
        '                    ' TODO: dispose managed state (managed objects).
        '                    qqHelper.DisposeString(_DetailStatusCode)
        '                    qqHelper.DisposeString(_PolicyInfoNum) 'for reconciliation
        '                    qqHelper.DisposeString(_Company)
        '                    qqHelper.DisposeString(_CompanyTypeId) 'static data
        '                    qqHelper.DisposeString(_EffectiveDate)
        '                    qqHelper.DisposeString(_ExpirationDate)
        '                    qqHelper.DisposeString(_LinkNumber)
        '                    qqHelper.DisposeString(_PersonalLiabilityLimit)
        '                    qqHelper.DisposeString(_PolicyTypeId) 'static data
        '                    qqHelper.DisposeString(_PrimaryPolicyNumber)
        '                    'If _Drivers IsNot Nothing Then
        '                    '    If _Drivers.Count > 0 Then
        '                    '        For Each d As QuickQuoteDriver In _Drivers
        '                    '            If d IsNot Nothing Then
        '                    '                d.Dispose()
        '                    '                d = Nothing
        '                    '            End If
        '                    '        Next
        '                    '        _Drivers.Clear()
        '                    '    End If
        '                    '    _Drivers = Nothing
        '                    'End If
        '                    '_CanUseDriverNumForDriverReconciliation = Nothing
        '                    If _Locations IsNot Nothing Then
        '                        If _Locations.Count > 0 Then
        '                            For Each Loc As QuickQuoteLocation In _Locations
        '                                If Loc IsNot Nothing Then
        '                                    Loc.Dispose()
        '                                    Loc = Nothing
        '                                End If
        '                            Next
        '                            _Locations.Clear()
        '                        End If
        '                        _Locations = Nothing
        '                    End If
        '                    _CanUseLocationNumForLocationReconciliation = Nothing
        '                    'If _Vehicles IsNot Nothing Then
        '                    '    If _Vehicles.Count > 0 Then
        '                    '        For Each v As QuickQuoteVehicle In _Vehicles
        '                    '            If v IsNot Nothing Then
        '                    '                v.Dispose()
        '                    '                v = Nothing
        '                    '            End If
        '                    '        Next
        '                    '        _Vehicles.Clear()
        '                    '    End If
        '                    '    _Vehicles = Nothing
        '                    'End If
        '                    '_CanUseVehicleNumForVehicleReconciliation = Nothing
        '                    'If _PersonalLiabilities IsNot Nothing Then
        '                    '    If _PersonalLiabilities.Count > 0 Then
        '                    '        For Each l As PersonalLiability In _PersonalLiabilities
        '                    '            If l IsNot Nothing Then
        '                    '                l.Dispose()
        '                    '                l = Nothing
        '                    '            End If
        '                    '        Next
        '                    '        _PersonalLiabilities.Clear()
        '                    '    End If
        '                    '    _PersonalLiabilities = Nothing
        '                    'End If
        '                    '_CanUsePersonalLiabilityNumForPersonalLiabilityReconciliation = Nothing
        '                    qqHelper.DisposeString(_PremiumFullterm)

        '                    MyBase.Dispose()
        '                End If

        '                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '                ' TODO: set large fields to null.
        '            End If
        '            Me.disposedValue = True
        '        End Sub

        '        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        '        'Protected Overrides Sub Finalize()
        '        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '        '    Dispose(False)
        '        '    MyBase.Finalize()
        '        'End Sub

        '        ' This code added by Visual Basic to correctly implement the disposable pattern.
        '        'Public Sub Dispose() Implements IDisposable.Dispose
        '        Public Overrides Sub Dispose()
        '            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '            Dispose(True)
        '            GC.SuppressFinalize(Me)
        '        End Sub
        '#End Region
#End Region

        Public Sub New()
            MyBase.New()
            'SetDefaults()
        End Sub

        Public Sub New(Parent As QuickQuoteObject)
            MyBase.New()
            SetParent = Parent
            'SetDefaults()
        End Sub
        Public Sub New(parent As QuickQuoteUnderlyingPolicy)
            MyBase.New()
            With parent
                'Me.PolicyId = parent.PolicyId
                'Me.PolicyImageNum = PolicyImageNum
                'Me.PolicyItemNumber = $"{If(parent.PolicyInfos?.Count, 0) + 1}"
                Me.PrimaryPolicyNumber = parent.PrimaryPolicyNumber
                Me.EffectiveDate = parent.EffectiveDate
                Me.ExpirationDate = parent.ExpirationDate
            End With
            Me.SetParent = parent
        End Sub
#Region "IReconcilable"
        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of PolicyInfo)
                matched = matched AndAlso
                          _strCompare.Equals(Me.PrimaryPolicyNumber, .PrimaryPolicyNumber)
            End With
            Return matched
        End Function
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)

            With src.AsInstance(Of PolicyInfo)
                Me.Company = .Company
                Me.CompanyTypeId = .CompanyTypeId
                Me.ExpirationDate = .ExpirationDate
                'Personal/Commercial Auto              
                Me.Drivers.InitIfNothing.CopyFromCollection(.Drivers, setItemNumbers, _listMetadata.GetOrCreateValue(Me.Drivers).CanUsePolicyItemNumberForReconciliation)

                Me.Vehicles.InitIfNothing.CopyFromCollection(.Vehicles, setItemNumbers, _listMetadata.GetOrCreateValue(Me.Vehicles).CanUsePolicyItemNumberForReconciliation)
                'Comercial Auto
                Me.CombinedSingleLimit = .CombinedSingleLimit
                'Personal Auto
                If Me.YouthfulOperators Is Nothing Then

                End If
                Me.YouthfulOperators.InitIfNothing.CopyFromCollection(.YouthfulOperators, setItemNumbers, _listMetadata.GetOrCreateValue(Me.YouthfulOperators).CanUsePolicyItemNumberForReconciliation)
                Me.PropertyDamageLimit = .PropertyDamageLimit
                Me.BodilyInjuryLimitA = .BodilyInjuryLimitA
                Me.BodilyInjuryLimitB = .BodilyInjuryLimitB
                'Home
                Me.MiscellaneousLiabilities.InitIfNothing.CopyFromCollection(.MiscellaneousLiabilities, setItemNumbers, _listMetadata.GetOrCreateValue(Me.MiscellaneousLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.ProfessionalLiabilities.InitIfNothing.CopyFromCollection(.ProfessionalLiabilities, setItemNumbers, _listMetadata.GetOrCreateValue(Me.ProfessionalLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.InvestmentProperties.InitIfNothing.CopyFromCollection(.InvestmentProperties, setItemNumbers, _listMetadata.GetOrCreateValue(Me.InvestmentProperties).CanUsePolicyItemNumberForReconciliation)
                Me.Watercrafts.InitIfNothing.CopyFromCollection(.Watercrafts, setItemNumbers, _listMetadata.GetOrCreateValue(Me.Watercrafts).CanUsePolicyItemNumberForReconciliation)
                Me.RecreationalVehicles.InitIfNothing.CopyFromCollection(.RecreationalVehicles, setItemNumbers, _listMetadata.GetOrCreateValue(Me.RecreationalVehicles).CanUsePolicyItemNumberForReconciliation)
                'Home/Farm
                Me.PersonalLiabilities.InitIfNothing.CopyFromCollection(.PersonalLiabilities, setItemNumbers, _listMetadata.GetOrCreateValue(Me.PersonalLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.PersonalLiabilityLimit = .PersonalLiabilityLimit
                'Farm
                Me.FarmLiabilities.InitIfNothing.CopyFromCollection(.FarmLiabilities, setItemNumbers, _listMetadata.GetOrCreateValue(Me.FarmLiabilities).CanUsePolicyItemNumberForReconciliation)
                'Me.AggregateLiabilityLimit = .AggregateLiabilityLimit   'not needed 5/20/2021
                'Me.OccurrenceLiabilityLimit = .OccurrenceLiabilityLimit  'not needed 5/20/2021

                'Farm/Workers Comp
                Me.DiseaseEachEmployee = .DiseaseEachEmployee
                Me.DiseasePolicyLimit = .DiseasePolicyLimit
                Me.EachAccident = .EachAccident
                Me.CombinedAccidentPolicyEmployeeLimitId = .CombinedAccidentPolicyEmployeeLimitId
            End With
        End Sub
        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.ConvertToDiamondItem
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.PolicyInfo
            End If

            With DirectCast(diaItem, DCO.Policy.PolicyInfo)
                .DetailStatusCode = 1
                Integer.TryParse(Me.PolicyTypeId, .PolicyTypeId)
                'Integer.TryParse(Me.AggregateLiabilityLimit, .Aggregate)   'not needed 5/20/2021

                If _qqHelper.IsNumericString(Me.BodilyInjuryLimitA) Then
                    Integer.TryParse(Me.BodilyInjuryLimitA, .BodilyInjury1)
                    Integer.TryParse(Me.BodilyInjuryLimitB, .BodilyInjury2)
                    Integer.TryParse(Me.PropertyDamageLimit, .PropertyDamage)
                    .OptionalSplitLimits = True
                Else
                    .BodilyInjury1 = 0
                    .BodilyInjury2 = 0
                    .PropertyDamage = 0
                    .OptionalSplitLimits = False
                End If
                If _qqHelper.IsNumericString(Me.CombinedSingleLimit) Then
                    Integer.TryParse(Me.CombinedSingleLimit, .CombinedSingleLimit)
                    .OptionalCombinedSingleLimit = True
                Else
                    .CombinedSingleLimit = 0
                    .OptionalCombinedSingleLimit = False
                End If
                Integer.TryParse(Me.CompanyTypeId, .CompanyTypeId)
                Integer.TryParse(Me.DiseaseEachEmployee, .DiseaseEachEmployee)
                Integer.TryParse(Me.DiseasePolicyLimit, .DiseasePolicyLimit)
                Integer.TryParse(Me.EachAccident, .EachAccident)

                If _qqHelper.IsValidQuickQuoteIdOrNum(Me.CombinedAccidentPolicyEmployeeLimitId) Then
                    Dim capelCoverage As Coverage = .Coverages.FirstOrDefault(Function(coverage) coverage.CoverageCodeId = WCP_COMBINED_ACCIDENT_POLICY_EMPLOYEE_LIMIT_COVERAGE_CODE_ID)
                    If capelCoverage Is Nothing Then
                        capelCoverage = New Coverage With {.CoverageCodeId = WCP_COMBINED_ACCIDENT_POLICY_EMPLOYEE_LIMIT_COVERAGE_CODE_ID}
                    End If
                    capelCoverage.CoverageLimitId = Me.CombinedAccidentPolicyEmployeeLimitId
                End If


                'Date.TryParse(Me.EffectiveDate, .EffectiveDate)
                'Date.TryParse(Me.ExpirationDate, .ExpirationDate)
                'Integer.TryParse(Me.OccurrenceLiabilityLimit, .Occurrence)  'not needed 5/20/2021
                'Integer.TryParse(Me.PolicyId, .PolicyId)
                'Integer.TryParse(Me.PolicyImageNum, .PolicyImageNum)
                'Decimal.TryParse(Me.PremiumFullterm, .PremiumFullterm)
                Integer.TryParse(Me.PersonalLiabilityLimit, .PersonalLiabilityLimit)
                .PrimaryPolicyNumber = Me.PrimaryPolicyNumber?.ToUpper()

                Me.Drivers.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.Driver)(.Drivers, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.Drivers).CanUsePolicyItemNumberForReconciliation)
                Me.FarmLiabilities.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.FarmLiability)(.FarmLiabilities, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.FarmLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.InvestmentProperties.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.Location)(.Locations, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.InvestmentProperties).CanUsePolicyItemNumberForReconciliation)
                Me.MiscellaneousLiabilities.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.MiscellaneousLiability)(.MiscellaneousLiabilities, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.MiscellaneousLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.PersonalLiabilities.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.PersonalLiability)(.PersonalLiabilities, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.PersonalLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.ProfessionalLiabilities.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.ProfessionalLiability)(.ProfessionalLiabilities, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.ProfessionalLiabilities).CanUsePolicyItemNumberForReconciliation)
                Me.RecreationalVehicles.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.RecreationalVehicle)(.RecreationalVehicles, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.RecreationalVehicles).CanUsePolicyItemNumberForReconciliation)
                Me.Vehicles.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.Vehicle)(.Vehicles, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.Vehicles).CanUsePolicyItemNumberForReconciliation)
                Me.Watercrafts.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.Watercraft)(.Watercrafts, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.Watercrafts).CanUsePolicyItemNumberForReconciliation)
                Me.YouthfulOperators.InitIfNothing.ConvertToDiamondItemCollection(Of DCO.Policy.YouthfulOperator)(.YouthfulOperators, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag, _listMetadata.GetOrCreateValue(Me.YouthfulOperators).CanUsePolicyItemNumberForReconciliation)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchForDiamondItem
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.PolicyInfo)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.PolicyInfoNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.PolicyInfoNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.PolicyTypeId) = diaObj.PolicyTypeId AndAlso
                       Me.PrimaryPolicyNumber?.ToUpper() = diaObj.PrimaryPolicyNumber?.ToUpper() AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function

        Public Overrides Sub ParseThroughCollectionsAndSetFlags()
            Me.Drivers.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.Drivers).CanUsePolicyItemNumberForReconciliation)
            Me.FarmLiabilities.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.FarmLiabilities).CanUsePolicyItemNumberForReconciliation)
            Me.InvestmentProperties.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.InvestmentProperties).CanUsePolicyItemNumberForReconciliation)
            Me.MiscellaneousLiabilities.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.MiscellaneousLiabilities).CanUsePolicyItemNumberForReconciliation)
            Me.PersonalLiabilities.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.PersonalLiabilities).CanUsePolicyItemNumberForReconciliation)
            Me.ProfessionalLiabilities.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.ProfessionalLiabilities).CanUsePolicyItemNumberForReconciliation)
            Me.RecreationalVehicles.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.RecreationalVehicles).CanUsePolicyItemNumberForReconciliation)
            Me.Vehicles.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.Vehicles).CanUsePolicyItemNumberForReconciliation)
            Me.Watercrafts.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.Watercrafts).CanUsePolicyItemNumberForReconciliation)
            Me.YouthfulOperators.InitIfNothing.ParseThroughCollection_SetReconcilliationFlag(_listMetadata.GetOrCreateValue(Me.YouthfulOperators).CanUsePolicyItemNumberForReconciliation)
        End Sub
#End Region
#Region "IXmlSerialiable Support"

        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.Company)
                        Me.Company = reader.ReadElementContentAsString()
                    Case NameOf(Me.CompanyTypeId)
                        Me.CompanyTypeId = reader.ReadElementContentAsString()
                    Case NameOf(Me.ExpirationDate)
                        If reader.ReadToDescendant("DateTime") Then
                            Me.ExpirationDate = reader.ReadElementContentAsString()
                        End If
                        reader.ReadEndElement()
                    Case NameOf(Me.PrimaryPolicyNumber)
                        Me.PrimaryPolicyNumber = reader.ReadElementContentAsString()
                    Case "PolicyTypeId"
                        Me.TypeId = reader.ReadElementContentAsString()
                        'Personal Auto/Commercial Auto
                    Case NameOf(Me.Drivers)
                        Me.Drivers = Me.DeserializeList(Of Driver)(NameOf(Me.Drivers), reader)
                    Case NameOf(Me.Vehicles)
                        Me.Vehicles = Me.DeserializeList(Of Vehicle)(NameOf(Me.Vehicles), reader)
                        'Commercial Auto
                    Case "CombinedSingleLimit"
                        Me.CombinedSingleLimit = reader.ReadElementContentAsString()
                        'Personal Auto
                    Case NameOf(Me.YouthfulOperators)
                        Me.YouthfulOperators = Me.DeserializeList(Of YouthfulOperator)(NameOf(Me.YouthfulOperators), reader)
                    Case "PropertyDamage"
                        Me.PropertyDamageLimit = reader.ReadElementContentAsString()
                    Case "BodilyInjury1"
                        Me.BodilyInjuryLimitA = reader.ReadElementContentAsString()
                    Case "BodilyInjury2"
                        Me.BodilyInjuryLimitB = reader.ReadElementContentAsString()
                        'Home
                    Case NameOf(Me.MiscellaneousLiabilities)
                        Me.MiscellaneousLiabilities = Me.DeserializeList(Of MiscellaneousLiability)(NameOf(Me.MiscellaneousLiabilities), reader)
                    Case NameOf(Me.ProfessionalLiabilities)
                        Me.ProfessionalLiabilities = Me.DeserializeList(Of ProfessionalLiability)(NameOf(Me.ProfessionalLiabilities), reader)
                    Case "Locations"
                        Me.InvestmentProperties = Me.DeserializeList(Of Location)("Locations", reader)
                    Case NameOf(Me.Watercrafts)
                        Me.Watercrafts = Me.DeserializeList(Of Watercraft)(NameOf(Me.Watercrafts), reader)
                    Case NameOf(Me.RecreationalVehicles)
                        Me.RecreationalVehicles = Me.DeserializeList(Of RecreationalVehicle)(NameOf(Me.RecreationalVehicles), reader)
                        'Home/Farm
                    Case NameOf(Me.PersonalLiabilities)
                        Me.PersonalLiabilities = Me.DeserializeList(Of PersonalLiability)(NameOf(Me.PersonalLiabilities), reader)
                    Case NameOf(Me.PersonalLiabilityLimit)
                        Me.PersonalLiabilityLimit = reader.ReadElementContentAsString()
                        'Farm
                    Case NameOf(Me.FarmLiabilities)
                        Me.FarmLiabilities = Me.DeserializeList(Of FarmLiability)(NameOf(Me.FarmLiabilities), reader)
                    'Case "Aggregate"   'not needed 5/20/2021
                    '    Me.AggregateLiabilityLimit = reader.ReadElementContentAsString()
                    'Case "Occurrence"   'not needed 5/20/2021
                    '    Me.OccurrenceLiabilityLimit = reader.ReadElementContentAsString()
                        'Farm/Workers comp
                    Case NameOf(Me.DiseaseEachEmployee)
                        Me.DiseaseEachEmployee = reader.ReadElementContentAsString()
                    Case NameOf(Me.DiseasePolicyLimit)
                        Me.DiseasePolicyLimit = reader.ReadElementContentAsString()
                    Case NameOf(Me.EachAccident)
                        Me.EachAccident = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            With writer
                .WriteElementString(NameOf(Me.Company), Me.Company)
                .WriteElementString(NameOf(Me.CompanyTypeId), Me.CompanyTypeId)

                .WriteStartElement(NameOf(Me.ExpirationDate))
                .WriteElementString("DateTime", _qqHelper.DiamondDateFormat(Me.ExpirationDate))
                .WriteEndElement()

                .WriteElementString(NameOf(Me.PrimaryPolicyNumber), Me.PrimaryPolicyNumber)
                .WriteElementString("PolicyTypeId", Me.TypeId)

                'Personal Auto/Commercial Auto
                Me.SerializeList(Of Driver)(Me.Drivers, NameOf(Me.Drivers), writer)
                Me.SerializeList(Of Vehicle)(Me.Vehicles, NameOf(Me.Vehicles), writer)
                'Commercial Auto
                If _qqHelper.IsNumericString(Me.CombinedSingleLimit) Then
                    .WriteElementString("CombinedSingleLimit", Me.CombinedSingleLimit)
                    .WriteElementString("OptionalCombinedSingleLimit", True)
                End If
                'Personal Auto
                Me.SerializeList(Of YouthfulOperator)(Me.YouthfulOperators, NameOf(Me.YouthfulOperators), writer)
                If _qqHelper.IsNumericString(Me.BodilyInjuryLimitA) Then
                    .WriteElementString("BodilyInjury1", Me.BodilyInjuryLimitA)
                    .WriteElementString("BodilyInjury2", Me.BodilyInjuryLimitB)
                    .WriteElementString("PropertyDamage", Me.PropertyDamageLimit)
                    .WriteElementString("OptionalSplitLimits", True)
                End If
                'Home
                Me.SerializeList(Of MiscellaneousLiability)(Me.MiscellaneousLiabilities, NameOf(Me.MiscellaneousLiabilities), writer)
                Me.SerializeList(Of ProfessionalLiability)(Me.ProfessionalLiabilities, NameOf(Me.ProfessionalLiabilities), writer)
                Me.SerializeList(Of Location)(Me.InvestmentProperties, "Locations", writer)
                Me.SerializeList(Of Watercraft)(Me.Watercrafts, NameOf(Me.Watercrafts), writer)
                Me.SerializeList(Of RecreationalVehicle)(Me.RecreationalVehicles, NameOf(Me.RecreationalVehicles), writer)
                'Home/Farm
                Me.SerializeList(Of PersonalLiability)(Me.PersonalLiabilities, NameOf(Me.PersonalLiabilities), writer)
                .WriteElementString(NameOf(Me.PersonalLiabilityLimit), Me.PersonalLiabilityLimit)
                'Farm
                Me.SerializeList(Of FarmLiability)(Me.FarmLiabilities, NameOf(Me.FarmLiabilities), writer)
                '.WriteElementString("Aggregate", Me.AggregateLiabilityLimit)    'not needed 5/20/2021
                '.WriteElementString("Occurrence", Me.OccurrenceLiabilityLimit)  'not needed 5/20/2021

                'Farm/Workers Compensation
                .WriteElementString(NameOf(Me.DiseaseEachEmployee), Me.DiseaseEachEmployee)
                .WriteElementString(NameOf(Me.DiseasePolicyLimit), Me.DiseasePolicyLimit)
                .WriteElementString(NameOf(Me.EachAccident), Me.EachAccident)
            End With
        End Sub

#End Region

        Protected Class ListMetaData
            Public CanUsePolicyItemNumberForReconciliation As Boolean = False
        End Class
    End Class
End Namespace

