Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store section coverage information
    ''' </summary>
    ''' <remarks>could potentially be under several different objects; currently part of Location</remarks>
    <Serializable()> _
    Public Class QuickQuoteSectionCoverage 'added 8/1/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Enum CoverageExposureTypeEnum
            NA = 0
            Covered_Auto = 1
            Policy_Level_Coverages = 2
            Covered_Building = 4
            Covered_Location = 5
            Section_I_Coverages = 6
            Section_II_Coverages = 7
            Additional_Coverages = 8
            Covered_Motor = 9
            Barns_Buildings_And_Structures = 11
            Unscheduled_Farm_Personal_Property = 28
            Additional_Coverages_F_And_G = 29
            Farm_Liability = 30
            Home_Based_Business = 32
            Watercraft_PD = 55
            Covered_river = 58
            Covered_Motorcycle = 62
            Covered_Watercraft = 63
            Scheduled_Propert = 65
            Home_Based_Business_Property = 67
            Home_Based_Business_Liability = 68
            Inland_Marine = 69
            RVWaterCraft = 70
            Location_Optional_Coverage = 71
            Location_Optional_Liability = 72
            Location_Incidental_Dwelling = 73
            Farm_Incidental_Limits = 74
            Barns_Buildings_And_Structures_Optional_Coverage = 75
            Location_Supplemental_Surcharges = 76
            Barns_Buildings_And_Structures_Supplemental_Surcharges = 77
            Earthquake_And_WISS_Coverage = 78
            Farm_Machinery_Open_Perils = 79
            Peak_Season = 80
            Loss_Of_Income = 83
            Inland_Marine_Trips = 84
            Inland_Marine_Exhibits = 85
            Bicycles = 91
            Cameras = 92
            Coins = 93
            CoinsFine_Arts_Including_Breakage = 94
            Fine_Arts = 95
            Furs = 96
            Golf_Equipment = 97
            Guns = 98
            Jewelry = 99
            Miscellaneous_Property = 100
            Miscellaneous_Musical_Property_Blanket = 101
            Silverware = 102
            Stamps = 103
            Antiques_with_Breakage = 104
            Guns_Fired = 105
            Sports_Equipment = 106
            Section_I_And_II_Coverages = 118
            Scheduled_Farm_Personal_Property_Household_Contents_Replacement_Cost = 126
            Policy_Information_Coverages = 127
            Inland_Marine_LOB = 175
            RVWatercraft_Motor = 176
            RVWatercraft_Trailer = 177
            RVWatercraft_Equipment = 178
            Classification = 184
            PolicyInfomationCoverages = 185
            GL_Classification = 210
            Dwellings = 211
            Livestock = 212
            Home_Based_Business_Property_General = 213
            Home_Based_Business_Property_And_Liability = 214
        End Enum


        Private _Address As QuickQuoteAddress
        Private _Emails As List(Of QuickQuoteEmail)
        Private _Name As QuickQuoteName
        Private _Phones As List(Of QuickQuotePhone)
        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _BusinessName As String
        Private _BusinessType As String
        Private _ConstructionTypeId As String 'may need matching ConstructionType variable/property
        Private _CoverageExposureId As String
        Private _Coverages As List(Of QuickQuoteCoverage) 'mostly appears to include only 1 Coverage; found 3 for CoverageExposureId 118 test example
        Private _DescribedLocation As Boolean
        Private _Description As String
        Private _EffectiveDate As String
        Private _EmployeesFulltime As String
        Private _EmployeesParttime41To179Days As String
        Private _EmployeesParttimeOneTo40Days As String
        Private _EventEffDate As String
        Private _EventExpDate As String
        Private _InitialFarmPremises As Boolean
        Private _NumberOfFamilies As String
        Private _NumberOfLivestock As String
        Private _NumberOfPersonsReceivingCare As String
        Private _TheftExtension As Boolean
        'added 8/16/2013 for DFR
        Private _OccupancyCodeId As String 'may need matching OccupancyCode variable/property
        Private _ProtectionClassId As String 'may need matching ProtectionClass variable/property
        Private _UsageTypeId As String 'may need matching UsageType variable/property
        Private _NavigationPeriodEffDate As String
        Private _NavigationPeriodExpDate As String
        Private _NumberOfEmployees As String

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        'Private _SectionCoverageNum As String 'added 10/14/2014 for reconciliation; removed 10/29/2018
        Private _SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018

        'added 6/8/2015 for Farm (Section II Covs; Liability)
        Private _EstimatedReceipts As String
        Private _NumberOfDomesticEmployees As String
        Private _NumberOfEvents As String
        Private _NumberOfStalls As String
        Private _NumberOfDaysVacant As String 'Section I Covs; Property
        Private _NumberOfWells As String 'Section I Covs; Property
        'added 6/12/2015 for Farm (Section II Covs; Liability)
        Private _BusinessPursuitTypeId As String 'static data

        'added for HOM2018Upgrade
        Private _VegetatedRoof As Boolean
        Private _IncreasedCostOfLossId As String
        Private _IncreasedCostOfLoss As String
        Private _RelatedExpenseLimit As String

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 11/1/2021
        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String


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
        Public Property Emails As Generic.List(Of QuickQuoteEmail)
            Get
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05623}")
                Return _Emails
            End Get
            Set(value As Generic.List(Of QuickQuoteEmail))
                _Emails = value
                SetParentOfListItems(_Emails, "{663B7C7B-F2AC-4BF6-965A-D30F41A05623}")
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
        Public Property Phones As Generic.List(Of QuickQuotePhone)
            Get
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05624}")
                Return _Phones
            End Get
            Set(value As Generic.List(Of QuickQuotePhone))
                _Phones = value
                SetParentOfListItems(_Phones, "{663B7C7B-F2AC-4BF6-965A-D30F41A05624}")
            End Set
        End Property
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
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05625}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05625}")
            End Set
        End Property
        Public Property BusinessName As String
            Get
                Return _BusinessName
            End Get
            Set(value As String)
                _BusinessName = value
            End Set
        End Property
        Public Property BusinessType As String
            Get
                Return _BusinessType
            End Get
            Set(value As String)
                _BusinessType = value
            End Set
        End Property
        Public Property ConstructionTypeId As String
            Get
                Return _ConstructionTypeId
            End Get
            Set(value As String)
                _ConstructionTypeId = value
            End Set
        End Property
        Public Property CoverageExposureId As String
            Get
                Return _CoverageExposureId
            End Get
            Set(value As String)
                _CoverageExposureId = value
            End Set
        End Property
        Public Property CoverageExposureType As CoverageExposureTypeEnum
            Get
                If String.IsNullOrWhiteSpace(CoverageExposureId) = False Then
                    Return DirectCast(CInt(CoverageExposureId), CoverageExposureTypeEnum)
                Else
                    Return CoverageExposureTypeEnum.NA
                End If
            End Get
            Set(value As CoverageExposureTypeEnum)
                _CoverageExposureId = value
            End Set
        End Property
        Public Property Coverages As List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05626}")
                Return _Coverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05626}")
            End Set
        End Property
        Public Property DescribedLocation As Boolean
            Get
                Return _DescribedLocation
            End Get
            Set(value As Boolean)
                _DescribedLocation = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
                qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property
        Public Property EmployeesFulltime As String
            Get
                Return _EmployeesFulltime
            End Get
            Set(value As String)
                _EmployeesFulltime = value
            End Set
        End Property
        Public Property EmployeesParttime41To179Days As String
            Get
                Return _EmployeesParttime41To179Days
            End Get
            Set(value As String)
                _EmployeesParttime41To179Days = value
            End Set
        End Property
        Public Property EmployeesParttimeOneTo40Days As String
            Get
                Return _EmployeesParttimeOneTo40Days
            End Get
            Set(value As String)
                _EmployeesParttimeOneTo40Days = value
            End Set
        End Property
        Public Property EventEffDate As String
            Get
                Return _EventEffDate
            End Get
            Set(value As String)
                _EventEffDate = value
                qqHelper.ConvertToShortDate(_EventEffDate)
            End Set
        End Property
        Public Property EventExpDate As String
            Get
                Return _EventExpDate
            End Get
            Set(value As String)
                _EventExpDate = value
            End Set
        End Property
        Public Property ExteriorDoorWindowSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property ExteriorWallSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property InitialFarmPremises As Boolean
            Get
                Return _InitialFarmPremises
            End Get
            Set(value As Boolean)
                _InitialFarmPremises = value
            End Set
        End Property
        Public Property NumberOfFamilies As String
            Get
                Return _NumberOfFamilies
            End Get
            Set(value As String)
                _NumberOfFamilies = value
            End Set
        End Property
        Public Property NumberOfLivestock As String
            Get
                Return _NumberOfLivestock
            End Get
            Set(value As String)
                _NumberOfLivestock = value
            End Set
        End Property
        Public Property NumberOfPersonsReceivingCare As String
            Get
                Return _NumberOfPersonsReceivingCare
            End Get
            Set(value As String)
                _NumberOfPersonsReceivingCare = value
            End Set
        End Property
        Public Property RoofSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property TheftExtension As Boolean
            Get
                Return _TheftExtension
            End Get
            Set(value As Boolean)
                _TheftExtension = value
            End Set
        End Property
        Public Property OccupancyCodeId As String '
            Get
                Return _OccupancyCodeId
            End Get
            Set(value As String)
                _OccupancyCodeId = value
            End Set
        End Property
        Public Property ProtectionClassId As String '
            Get
                Return _ProtectionClassId
            End Get
            Set(value As String)
                _ProtectionClassId = value
            End Set
        End Property
        Public Property UsageTypeId As String '
            Get
                Return _UsageTypeId
            End Get
            Set(value As String)
                _UsageTypeId = value
            End Set
        End Property
        Public Property NavigationPeriodEffDate As String
            Get
                Return _NavigationPeriodEffDate
            End Get
            Set(value As String)
                _NavigationPeriodEffDate = value
                qqHelper.ConvertToShortDate(_NavigationPeriodEffDate)
            End Set
        End Property
        Public Property NavigationPeriodExpDate As String
            Get
                Return _NavigationPeriodExpDate
            End Get
            Set(value As String)
                _NavigationPeriodExpDate = value
                qqHelper.ConvertToShortDate(_NavigationPeriodExpDate)
            End Set
        End Property
        Public Property NumberOfEmployees As String
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
            End Set
        End Property

        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property

        Public Property SectionCoverageNum As String 'added 10/14/2014 for reconciliation
            Get
                'Return _SectionCoverageNum
                'updated 10/29/2018
                Return SectionCoverageNumGroup.DiamondNum
            End Get
            Set(value As String)
                '_SectionCoverageNum = value
                'updated 10/29/2018
                SectionCoverageNumGroup.DiamondNum = value
            End Set
        End Property

        Public Property SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018
            Get
                If _SectionCoverageNumGroup Is Nothing Then
                    _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me)
                Else
                    SetObjectsParent(_SectionCoverageNumGroup)
                End If
                Return _SectionCoverageNumGroup
            End Get
            Set(value As QuickQuoteDiamondNumGroup)
                _SectionCoverageNumGroup = value
                SetObjectsParent(_SectionCoverageNumGroup)
            End Set
        End Property

        'added 6/8/2015 for Farm (Section II Covs; Liability)
        Public Property EstimatedReceipts As String
            Get
                Return _EstimatedReceipts
            End Get
            Set(value As String)
                _EstimatedReceipts = value
                qqHelper.ConvertToLimitFormat(_EstimatedReceipts)
            End Set
        End Property
        Public Property NumberOfDomesticEmployees As String
            Get
                Return _NumberOfDomesticEmployees
            End Get
            Set(value As String)
                _NumberOfDomesticEmployees = value
            End Set
        End Property
        Public Property NumberOfEvents As String
            Get
                Return _NumberOfEvents
            End Get
            Set(value As String)
                _NumberOfEvents = value
            End Set
        End Property
        Public Property NumberOfStalls As String
            Get
                Return _NumberOfStalls
            End Get
            Set(value As String)
                _NumberOfStalls = value
            End Set
        End Property
        Public Property NumberOfDaysVacant As String 'Section I Covs; Property
            Get
                Return _NumberOfDaysVacant
            End Get
            Set(value As String)
                _NumberOfDaysVacant = value
            End Set
        End Property
        Public Property NumberOfWells As String 'Section I Covs; Property
            Get
                Return _NumberOfWells
            End Get
            Set(value As String)
                _NumberOfWells = value
            End Set
        End Property
        'added 6/12/2015 for Farm (Section II Covs; Liability)
        Public Property BusinessPursuitTypeId As String 'static data
            Get
                Return _BusinessPursuitTypeId
            End Get
            Set(value As String)
                _BusinessPursuitTypeId = value
            End Set
        End Property

        Public Property VegetatedRoof As Boolean
            Get
                Return _VegetatedRoof
            End Get
            Set(value As Boolean)
                _VegetatedRoof = value
            End Set
        End Property

        Public Property IncreasedCostOfLoss As String
            Get
                Return _IncreasedCostOfLoss
            End Get
            Set(value As String)
                _IncreasedCostOfLoss = value
                _IncreasedCostOfLossId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedCostOfLossId, _IncreasedCostOfLoss, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
            End Set
        End Property

        Public Property IncreasedCostofLossId As String
            Get
                Return _IncreasedCostOfLossId
            End Get
            Set(value As String)
                _IncreasedCostOfLossId = value
                _IncreasedCostOfLoss = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedCostOfLossId, _IncreasedCostOfLossId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
            End Set
        End Property
        Public Property RelatedExpenseLimit As String
            Get
                Return _RelatedExpenseLimit
            End Get
            Set(value As String)
                _RelatedExpenseLimit = value
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

        'added 11/1/2021
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
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


        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Address = New QuickQuoteAddress
            '_Emails = New Generic.List(Of QuickQuoteEmail)
            _Emails = Nothing 'added 8/4/2014
            _Name = New QuickQuoteName
            _Name.NameAddressSourceId = "10022" 'Section Coverage
            '_Phones = New Generic.List(Of QuickQuotePhone)
            _Phones = Nothing 'added 8/4/2014
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _BusinessName = ""
            _BusinessType = ""
            _ConstructionTypeId = ""
            _CoverageExposureId = ""
            '_Coverages = New List(Of QuickQuoteCoverage)
            _Coverages = Nothing 'added 8/4/2014
            _DescribedLocation = False
            _Description = ""
            _EffectiveDate = ""
            _EmployeesFulltime = ""
            _EmployeesParttime41To179Days = ""
            _EmployeesParttimeOneTo40Days = ""
            _EventEffDate = ""
            _EventExpDate = ""
            _InitialFarmPremises = False
            _NumberOfFamilies = ""
            _NumberOfLivestock = ""
            _NumberOfPersonsReceivingCare = ""
            _TheftExtension = False
            _OccupancyCodeId = ""
            _ProtectionClassId = ""
            _UsageTypeId = ""
            _NavigationPeriodEffDate = ""
            _NavigationPeriodExpDate = ""
            _NumberOfEmployees = ""

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            '_SectionCoverageNum = "" 'added 10/14/2014 for reconciliation; removed 10/29/2018
            _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me) 'added 10/29/2018

            'added 6/8/2015 for Farm (Section II Covs; Liability)
            _EstimatedReceipts = ""
            _NumberOfDomesticEmployees = ""
            _NumberOfEvents = ""
            _NumberOfStalls = ""
            _NumberOfDaysVacant = "" 'Section I Covs; Property
            _NumberOfWells = "" 'Section I Covs; Property
            'added 6/12/2015 for Farm (Section II Covs; Liability)
            _BusinessPursuitTypeId = ""

            _VegetatedRoof = False
            _IncreasedCostOfLossId = ""
            _IncreasedCostOfLoss = ""
            _RelatedExpenseLimit = ""

            _DetailStatusCode = "" 'added 5/15/2019

            ' added 11/1/2021
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""

        End Sub
        'added 4/29/2014 for additionalInterests reconciliation; not being used as-of now... canUse property is simply transferred over from Location SectionI, SectionII, and SectionIAndII coverages; will be needed if something else uses section coverages by itself; now being used in Location.ParseThruSectionCoverages... being called for sectionCoverage and then canUse prop is copied over to sectionI, sectionII, or sectionIAndII coverages
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Function HasValidSectionCoverageNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_SectionCoverageNum)
            'updated 10/29/2018
            Return SectionCoverageNumGroup.HasValidDiamondNum()
        End Function

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            Dim myList As New List(Of String)
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(CoverageExposureId) = False Then
                    str &= "CoverageExposureType: " & CoverageExposureType.ToString()
                End If

                If Me.Coverages IsNot Nothing AndAlso Me.Coverages.Count > 0 Then
                    If String.IsNullOrWhiteSpace(str) Then
                        str = "CoverageCodeIds: "
                    Else
                        str &= " CoverageCodeIds: "
                    End If
                    str &= String.Join(",", Me.Coverages.Select(Function(cov) cov.CoverageCodeId).ToArray())
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
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
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
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    If _BusinessName IsNot Nothing Then
                        _BusinessName = Nothing
                    End If
                    If _BusinessType IsNot Nothing Then
                        _BusinessType = Nothing
                    End If
                    If _ConstructionTypeId IsNot Nothing Then
                        _ConstructionTypeId = Nothing
                    End If
                    If _CoverageExposureId IsNot Nothing Then
                        _CoverageExposureId = Nothing
                    End If
                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each c As QuickQuoteCoverage In _Coverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _DescribedLocation <> Nothing Then
                        _DescribedLocation = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _EffectiveDate IsNot Nothing Then
                        _EffectiveDate = Nothing
                    End If
                    If _EmployeesFulltime IsNot Nothing Then
                        _EmployeesFulltime = Nothing
                    End If
                    If _EmployeesParttime41To179Days IsNot Nothing Then
                        _EmployeesParttime41To179Days = Nothing
                    End If
                    If _EmployeesParttimeOneTo40Days IsNot Nothing Then
                        _EmployeesParttimeOneTo40Days = Nothing
                    End If
                    If _EventEffDate IsNot Nothing Then
                        _EventEffDate = Nothing
                    End If
                    If _EventExpDate IsNot Nothing Then
                        _EventExpDate = Nothing
                    End If
                    If _InitialFarmPremises <> Nothing Then
                        _InitialFarmPremises = Nothing
                    End If
                    If _NumberOfFamilies IsNot Nothing Then
                        _NumberOfFamilies = Nothing
                    End If
                    If _NumberOfLivestock IsNot Nothing Then
                        _NumberOfLivestock = Nothing
                    End If
                    If _NumberOfPersonsReceivingCare IsNot Nothing Then
                        _NumberOfPersonsReceivingCare = Nothing
                    End If
                    If _TheftExtension <> Nothing Then
                        _TheftExtension = Nothing
                    End If
                    If _OccupancyCodeId IsNot Nothing Then
                        _OccupancyCodeId = Nothing
                    End If
                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If
                    If _UsageTypeId IsNot Nothing Then
                        _UsageTypeId = Nothing
                    End If
                    If _NavigationPeriodEffDate IsNot Nothing Then
                        _NavigationPeriodEffDate = Nothing
                    End If
                    If _NavigationPeriodExpDate IsNot Nothing Then
                        _NavigationPeriodExpDate = Nothing
                    End If
                    If _NumberOfEmployees IsNot Nothing Then
                        _NumberOfEmployees = Nothing
                    End If

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

                    'If _SectionCoverageNum IsNot Nothing Then 'added 10/14/2014 for reconciliation; removed 10/29/2018
                    '    _SectionCoverageNum = Nothing
                    'End If
                    qqHelper.DisposeQuickQuoteDiamondNumGroup(_SectionCoverageNumGroup) 'added 10/29/2018

                    'added 6/8/2015 for Farm (Section II Covs; Liability)
                    qqHelper.DisposeString(_EstimatedReceipts)
                    qqHelper.DisposeString(_NumberOfDomesticEmployees)
                    qqHelper.DisposeString(_NumberOfEvents)
                    qqHelper.DisposeString(_NumberOfStalls)
                    qqHelper.DisposeString(_NumberOfDaysVacant) 'Section I Covs; Property
                    qqHelper.DisposeString(_NumberOfWells) 'Section I Covs; Property
                    'added 6/12/2015 for Farm (Section II Covs; Liability)
                    qqHelper.DisposeString(_BusinessPursuitTypeId)

                    'HOM2018 Upgrade
                    qqHelper.DisposeString(_IncreasedCostOfLossId)
                    qqHelper.DisposeString(_IncreasedCostOfLoss)

                    If _VegetatedRoof <> Nothing Then
                        _VegetatedRoof = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    ' added 11/1/2021
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum)

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
