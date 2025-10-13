Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports Diamond.Business.ThirdParty.tuxml
Imports IFM.VR.Common.Helpers.MultiState
Imports Insuresoft.RuleEngine.Common.Activities

Namespace IFM.VR.Common.Helpers
    Public Class NewCompanyIdHelper

        Const NewCoIdWarningMsg As String = ""
        Const NewCoIdRemovedMsg As String = ""

        Const NewCoGovStatesAllowedKey As String = "VR_NewCo_GovStatesAllowedKey"
        Const NewCoLobsAllowedKey As String = "VR_NewCo_LobsAllowedKey"
        Const NewCoCalendarDateKey As String = "VR_NewCo_CalendarDateKey"
        Const NewCoILEffectiveDate As String = "VR_NewCO_Il_EffectiveDate"
        Const NewCoOhEffectiveDate As String = "VR_NewCO_Oh_EffectiveDate"
        Const NewCoBopEffectiveDate As String = "VR_NewCO_BOP_EffectiveDate"
        Const NewCoBopCalendarDateKey As String = "VR_NewCo_BOP_CalendarDate"

        Private Shared _NewCoIdSettings As NewFlagItem
        Public Shared ReadOnly Property NewCoIdSettings() As NewFlagItem
            Get
                If _NewCoIdSettings Is Nothing Then
                    _NewCoIdSettings = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                End If
                Return _NewCoIdSettings
            End Get
        End Property


        Public Shared Function NewCoIdEnabled() As Boolean
            Return NewCoIdSettings.EnabledFlag
        End Function

        Public Shared Function NewCoIdEffDate() As Date
            Return NewCoIdSettings.GetStartDateOrDefault("1/1/2024")
        End Function
        Public Shared Function IsNewCompanyIdAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                NewCoIdSettings.OtherQualifiers = DoesQuoteQualifyByLob(quote) _
                    AndAlso DoesQuoteQualifyByState(quote) _
                    AndAlso DoesQuoteQualifyByCalendarDate(quote) _
                    AndAlso DoesQuoteQualifyByTransactionType(quote) _
                    AndAlso DoesQuoteQualifyByDates(quote)
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, NewCoIdSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function
        Public Shared Function IsNewCompany(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return quote.Company = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity
            End If
            Return False

        End Function

        Public Shared Function isDiamondNewCompany(quote As QuickQuoteObject) As Boolean
            'Return quote.Database_DiaCompany = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity
            Dim isIt As Boolean = False

            If quote IsNot Nothing Then
                If quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    isIt = (quote.Company = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity)
                Else
                    isIt = (quote.Database_DiaCompany = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity)
                End If
            End If

            Return isIt
        End Function

        Public Shared Function isNewCompanyLocked(quote As QuickQuoteObject) As Boolean
            Return isDiamondNewCompany(quote)
        End Function
        Public Shared Function IsEligibleForPotentialCompanyChange(ByVal quote As QuickQuoteObject) As Boolean
            Dim isIt As Boolean = False

            If quote IsNot Nothing AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                isIt = True
            End If

            Return isIt
        End Function
        Private Shared Function DoesQuoteQualifyByDates(quote As QuickQuoteObject) As Boolean
            Dim MinQualifyDate = GetEarliestEffectiveDatePossible(quote)
            Return DateFromString(quote.EffectiveDate) >= MinQualifyDate
        End Function

        Private Shared Function DoesQuoteQualifyByState(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim StatesOnQuote = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(quote)
                'Dim StateEffectiveDatesGood = DoesStateQualifyByEffectiveDate(StatesOnQuote, quote.EffectiveDate)
                Return Not StatesOnQuote.Except(GoverningStatesAllowed).Any() 'AndAlso StateEffectiveDatesGood
            End If
            Return False
        End Function

        Private Shared Function DoesQuoteQualifyByCalendarDate(quote As QuickQuoteObject) As Boolean
            Select Case quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Return DateTime.Now >= DateFromString(GetConfigStringForKey(NewCoBopCalendarDateKey))
                Case Else
                    Return DateTime.Now >= CalendarEffectiveDate()
            End Select
        End Function

        Private Shared Function DoesQuoteQualifyByLob(quote As QuickQuoteObject) As Boolean
            Return quote IsNot Nothing AndAlso LobsAllowed().Contains(quote.LobId.TryToGetInt32)
        End Function

        Private Shared Function DoesQuoteQualifyByTransactionType(quote As QuickQuoteObject) As Boolean
            Return quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
        End Function

        Private Shared Function DoesStateQualifyByEffectiveDate(StatesOnQuote As IEnumerable(Of Integer), effectiveDate As String)
            For Each state In StatesOnQuote
                Dim TestDate = GetStartDateByState(state)

                If DateFromString(effectiveDate) < TestDate Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Shared Function DoStatesQualifyByEffectiveDate(quote As QuickQuoteObject, effectiveDate As Date) As Boolean
            If quote IsNot Nothing Then
                Dim StatesOnQuote = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(quote)
                Return DoesStateQualifyByEffectiveDate(StatesOnQuote, effectiveDate)
            End If
            Return False
        End Function
        Private Shared Function GetListofIntegersFromAppSettingsKey(key As String) As List(Of Integer)
            Dim c As New CommonHelperClass
            Dim integerList As List(Of Integer) = New List(Of Integer)
            Dim integerString As String = c.ConfigurationAppSettingValueAsString(key)
            If Not String.IsNullOrWhiteSpace(integerString) Then
                integerList = c.ListOfIntegerFromString(integerString, ",")
            End If
            Return integerList
        End Function

        Public Shared Function GetEarliestDateAllowed(quote As QuickQuoteObject, effectiveDate As String) As String
            'Dim FinalFailDate As Date = Date.MinValue
            'Dim StatesOnQuote = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(quote)

            'For Each state In StatesOnQuote
            '    Dim TestDate = GetStartDateByState(state)

            '    If DateFromString(effectiveDate) < TestDate Then
            '        If TestDate <> Date.MinValue AndAlso TestDate > FinalFailDate Then
            '            FinalFailDate = TestDate
            '        End If
            '    End If
            'Next
            'Return FinalFailDate.ToShortDateString
            Return GetEarliestEffectiveDatePossible(quote).ToShortDateString
        End Function

        Public Shared Function GetEarliestEffectiveDatePossible(quote As QuickQuoteObject) As Date
            Dim FinalDate As Date = Date.MinValue
            Dim StatesOnQuote = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(quote)
            Dim TestDate = Date.MinValue

            ' Check for State Start Dates
            For Each state In StatesOnQuote
                TestDate = GetStartDateByState(state)
                TestAndUpdateForEarliestDate(TestDate, FinalDate)
            Next

            ' Check for LOB Start Dates
            TestDate = GetStartDateByLob(quote.LobType)
            TestAndUpdateForEarliestDate(TestDate, FinalDate)

            Return FinalDate
        End Function

        Public Shared Function TestAndUpdateForEarliestDate(testDate As Date, ByRef FinalDate As Date)
            If testDate <> Date.MinValue AndAlso testDate > FinalDate Then
                FinalDate = testDate
            End If
        End Function

        Public Shared Function GetStartDateByState(state As Integer) As Date
            Select Case state
                Case 16
                    Return DateFromString(NewCoIdSettings.StartDate)
                Case 15
                    Return DateFromString(GetConfigStringForKey(NewCoILEffectiveDate))
                Case 36
                    Return DateFromString(GetConfigStringForKey(NewCoOhEffectiveDate))
                Case Else
                    Return Date.MinValue
            End Select
        End Function

        Public Shared Function GetStartDateByLob(lobTypeId As QuickQuoteObject.QuickQuoteLobType) As Date
            Select Case lobTypeId
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Return DateFromString(GetConfigStringForKey(NewCoBopEffectiveDate))
                Case Else
                    Return Date.MinValue
            End Select
        End Function

        Private Shared Function GetConfigStringForKey(key As String) As String
            Dim c As New CommonHelperClass
            Return c.ConfigurationAppSettingValueAsString(key)
        End Function
        Private Shared Function DateFromString(dateString As String) As Date
            Dim c As New CommonHelperClass
            If c.IsDateString(dateString) Then
                Return dateString.ToDateTime
            End If
            Return Date.MinValue
        End Function

        ''' <summary>
        ''' Gets app config date string and returns a date.
        ''' </summary>
        ''' <returns>Date or MinDate if key not a date</returns>
        Private Shared Function CalendarEffectiveDate() As DateTime
            Dim dateString As String = GetConfigStringForKey(NewCoCalendarDateKey)
            Return DateFromString(dateString)
        End Function

        ''' <summary>
        ''' Used to change the Quote company ID between companies as long as Diamond company ID is not Indemnity(2)
        ''' Can be unlocked to force a change by using the optional unlock param.
        ''' </summary>
        ''' <param name="quote"></param>
        ''' <param name="companytype"></param>
        ''' <param name="unlock">True to force the company change to happen despite Diamond setting</param>
        Public Shared Sub ChangeCompanyType(ByRef quote As QuickQuoteObject, companytype As QuickQuoteHelperClass.QuickQuoteCompany, Optional unlock As Boolean = False)
            If quote IsNot Nothing AndAlso IsEligibleForPotentialCompanyChange(quote) Then
                If Not isDiamondNewCompany(quote) OrElse unlock Then
                    If companytype = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersMutual Then
                        ConvertNewCoToOldCo(quote)
                    Else
                        ConvertOldCoToNewCo(quote)
                    End If
                End If
            End If
        End Sub

        Private Shared Sub ConvertOldCoToNewCo(ByRef quote As QuickQuoteObject)
            quote.CompanyId = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity
            ''Locations Page
            ResetBuildingItems(quote)
            ''CGL Coverages Page
            ResetGlCoverageItems(quote)
            ''IRPM
            ResetIrpmItems(quote)
        End Sub

        Private Shared Sub ConvertNewCoToOldCo(ByRef quote As QuickQuoteObject)
            quote.CompanyId = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersMutual
            ''Locations Page
            ResetBuildingItems(quote)
            ''CGL Coverages Page
            ResetGlCoverageItems(quote)
            ''IRPM
            ResetIrpmItems(quote)


        End Sub

        Public Shared Function GoverningStatesAllowed() As List(Of Integer)
            Return GetListofIntegersFromAppSettingsKey(NewCoGovStatesAllowedKey)
        End Function

        Private Shared Function LobsAllowed() As List(Of Integer)
            Return GetListofIntegersFromAppSettingsKey(NewCoLobsAllowedKey)
        End Function

        Public Shared Function NewCoGoverningStateEffDate(Quote As QuickQuoteObject) As Date
            If Not String.IsNullOrWhiteSpace(Quote?.OriginalGoverningState) Then
                Dim GovStateEffDate As Date = DateFromString("1/1/1800")
                Select Case Quote.OriginalGoverningState
                    Case QuickQuoteHelperClass.QuickQuoteState.Indiana
                        GovStateEffDate = NewCoIdEffDate()
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                        Dim IlStateEffectiveDate As String = GetConfigStringForKey(NewCoILEffectiveDate)
                        GovStateEffDate = DateFromString(IlStateEffectiveDate)
                    Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
                        Dim OHStateEffectiveDate As String = GetConfigStringForKey(NewCoOhEffectiveDate)
                        GovStateEffDate = DateFromString(OHStateEffectiveDate)
                End Select
                Return GovStateEffDate
            End If
        End Function

        Private Shared Sub ResetBuildingItems(ByRef quote As QuickQuoteObject)
            For Each location As QuickQuoteLocation In quote?.Locations
                'Property In The Open - Earthquake Deductible
                For Each PropertyItem As QuickQuotePropertyInTheOpenRecord In location?.PropertyInTheOpenRecords
                    PropertyItem.EarthquakeDeductible = ""
                Next

                For Each building As QuickQuoteBuilding In location.Buildings
                    ''Clear Values
                    ''''Don Requested Maintaining these.
                    'building.YearBuilt = ""
                    'building.SquareFeet = ""
                    'building.NumberOfStories = ""

                    ''Clear EarthQuake if not matched in both NewCo and OldCo
                    Dim CommonEqClasses As New List(Of String) _
                        From {"9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "27"}
                    If Not CommonEqClasses.Contains(building.EarthquakeBuildingClassificationTypeId) Then
                        building.EarthquakeBuildingClassificationTypeId = ""
                    End If
                Next
            Next
        End Sub

        Private Shared Sub ResetGlCoverageItems(ByRef quote As QuickQuoteObject)
            If quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability _
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)
                SubQuoteFirst.OccurrenceLiabilityLimitId = "56"
                SubQuoteFirst.GeneralAggregateLimitId = "65"
                SubQuoteFirst.ProductsCompletedOperationsAggregateLimitId = "65"
                SubQuoteFirst.PersonalAndAdvertisingInjuryLimitId = "56"
            End If
        End Sub

        Private Shared Sub ResetIrpmItems(ByRef quote As QuickQuoteObject)
            Dim qqh As New QuickQuoteHelperClass
            Dim SubQuotes = qqh.MultiStateQuickQuoteObjects(quote)
            For Each stateQuote As QuickQuoteObject In SubQuotes
                ''Property
                stateQuote.IRPM_ManagementCooperation = ""
                stateQuote.IRPM_ManagementCooperationDesc = ""
                stateQuote.IRPM_Location = ""
                stateQuote.IRPM_LocationDesc = ""
                stateQuote.IRPM_BuildingFeatures = ""
                stateQuote.IRPM_BuildingFeaturesDesc = ""
                stateQuote.IRPM_Premises = ""
                stateQuote.IRPM_PremisesDesc = ""
                stateQuote.IRPM_Employees = ""
                stateQuote.IRPM_EmployeesDesc = ""
                stateQuote.IRPM_Protection = ""
                stateQuote.IRPM_ProtectionDesc = ""
                stateQuote.IRPM_CatostrophicHazards = ""
                stateQuote.IRPM_CatostrophicHazardsDesc = ""
                stateQuote.IRPM_ManagementExperience = ""
                stateQuote.IRPM_ManagementExperienceDesc = ""
                stateQuote.IRPM_Equipment = ""
                stateQuote.IRPM_EquipmentDesc = ""
                stateQuote.IRPM_MedicalFacilities = ""
                stateQuote.IRPM_MedicalFacilitiesDesc = ""
                stateQuote.IRPM_ClassificationPeculiarities = ""
                stateQuote.IRPM_ClassificationPeculiaritiesDesc = ""
                stateQuote.IRPM_CPR_Management = ""
                stateQuote.IRPM_CPR_ManagementDesc = ""
                stateQuote.IRPM_CPR_PremisesAndEquipment = ""
                stateQuote.IRPM_CPR_PremisesAndEquipmentDesc = ""
                ''Liability
                stateQuote.IRPM_GL_ManagementCooperation = ""
                stateQuote.IRPM_GL_ManagementCooperationDesc = ""
                stateQuote.IRPM_GL_Location = ""
                stateQuote.IRPM_GL_LocationDesc = ""
                stateQuote.IRPM_GL_Premises = ""
                stateQuote.IRPM_GL_PremisesDesc = ""
                stateQuote.IRPM_GL_Employees = ""
                stateQuote.IRPM_GL_EmployeesDesc = ""
                stateQuote.IRPM_GL_Equipment = ""
                stateQuote.IRPM_GL_EquipmentDesc = ""
                stateQuote.IRPM_GL_ClassificationPeculiarities = ""
                stateQuote.IRPM_GL_ClassificationPeculiaritiesDesc = ""
                'End If
            Next
        End Sub

        Public Shared Sub UpdateCompanyType(ByRef quote As QuickQuoteObject, ByRef ValidationErrors As List(Of WebValidationItem))
            If IsEligibleForPotentialCompanyChange(quote) = True Then
                If IsNewCompanyIdAvailable(quote) OrElse isDiamondNewCompany(quote) Then
                    If Not IsNewCompany(quote) Then
                        ChangeCompanyType(quote, QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity)

                        'If ValidationErrors IsNot Nothing Then
                        '    Dim i = New WebValidationItem(NewCoIdWarningMsg)
                        '    i.IsWarning = True
                        '    ValidationErrors.Add(i)
                        'End If
                    End If
                Else
                    If IsNewCompany(quote) AndAlso Not isDiamondNewCompany(quote) Then
                        ChangeCompanyType(quote, QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersMutual)

                        'No logic required
                        'If ValidationErrors IsNot Nothing Then
                        '    Dim i = New WebValidationItem(NewCoIdRemovedMsg)
                        '    i.IsWarning = True
                        '    ValidationErrors.Add(i)
                        'End If
                    End If
                End If
            End If
        End Sub

        Public Shared Sub UpdateNewCoId(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    ChangeCompanyType(Quote, QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity)

                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(NewCoIdWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    If Not isDiamondNewCompany(Quote) Then
                        ChangeCompanyType(Quote, QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersMutual)

                        If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(NewCoIdRemovedMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                    End If
            End Select
        End Sub


    End Class
End Namespace