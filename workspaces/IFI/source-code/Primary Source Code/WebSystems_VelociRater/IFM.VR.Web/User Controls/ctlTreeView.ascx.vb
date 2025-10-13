Imports System.Data
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers
Imports IFM.PrimativeExtensions.IFMExtensions
Imports IFM.VR.Common.Helpers.GenericHelper
Imports IFM.ControlFlags
Imports IFM.VR.Flags
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports Diamond.Common.Objects.Billing
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.FARM
Imports IFM.VR.Common.Helpers.CPP

Public Class ctlTreeView
    Inherits VrControlBaseEssentials

    Enum TreeViewSection
        None = 0
        Policyholders = 1
        Drivers = 2
        Vehicles = 3
        Locations = 4
        Coverages = 5
        UnderwritingQuestions = 6 'added 3/18/2014
        QuoteSummary = 7 'added 3/27/2014
        Discounts = 8 'added 4/1/2014; not sure if there will be a section to show
        Surcharges = 9 'added 4/1/2014; not sure if there will be a section to show
        Application = 10 'added 5/15/2014
        ApplicationSummary = 11 'added 5/19/2014
        Residence = 12 'added 6/12/2014
        PolicyLevelCoverages = 13 'added 7/27/2015 for Farm
        FarmPersonalProperty = 14 'added 7/27/2015 for Farm
        InlandMarineAndRvWatercraft = 15 'added 7/27/2015 for Farm
        IRPM = 16 'added 7/27/2015 for Farm
        CPP_CPR_Coverages = 17  ' Added 2/15/18 for CPP
        CPP_CPR_Locations = 18  ' Added 2/15/18 for CPP
        CPP_CGL_Coverages = 19  ' Added 2/15/18 for CPP
        CPP_CGL_Locations = 20  ' Added 2/15/18 for CPP
        PrintHistory = 21 'added 6/13/2019 for Endorsements/ReadOnly
        PolicyHistory = 22 'added 6/13/2019 for Endorsements/ReadOnly
        BillingInformation = 23 'added 6/13/2019 for Endorsements/ReadOnly
    End Enum
    Enum CreditReportEntityType
        Driver = 1
        Applicant = 2
    End Enum
    Enum QuoteOrRatedQuoteType
        Quote = 1
        RatedQuote = 2
    End Enum

    Dim chc As New CommonHelperClass
    Dim qqxml As New QuickQuoteXML
    Private _InEditMode As Boolean
    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 03/30/2021 for CAP Endorsements Task 52973 MLW

    Private _flags As List(Of IFeatureFlag)

#Region "Events"
    Public Event QuoteUpdated(ByVal sender As Object, ByVal e As EventArgs)
    Public Event RatedQuoteUpdated(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ShowPolicyholders(ByVal sender As Object, ByVal e As EventArgs)
    Public Event EditPolicyholder(ByVal policyholderNumber As Integer)
    Public Event NewPolicyholder(ByVal policyholderNumber As Integer)

    Public Event ShowDrivers(ByVal sender As Object, ByVal e As EventArgs)
    Public Event EditDriver(ByVal driverNumber As Integer)
    Public Event NewDriver(ByVal driverNumber As Integer)

    Public Event ShowVehicles(ByVal sender As Object, ByVal e As EventArgs)
    Public Event EditVehicle(ByVal vehicleNumber As Integer)
    Public Event NewVehicle(ByVal vehicleNumber As Integer)

    Public Event ShowLocations(ByVal sender As Object, ByVal e As EventArgs)
    Public Event EditLocation(ByVal locationNumber As Integer)
    Public Event NewLocation(ByVal locationNumber As Integer)

    Public Event ShowCoverages(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowQuoteSummary(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ShowUnderwritingQuestions(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ShowDiscounts(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowSurcharges(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ShowApplication(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowApplicationSummary(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ViewDriverCreditReport(ByVal driverNumber As Integer)
    Public Event ViewDriverMvrReport(ByVal driverNumber As Integer)
    Public Event ViewClueAutoReport(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ViewApplicantCreditReport(ByVal applicantNumber As Integer)
    Public Event ViewCluePropertyReport(ByVal sender As Object, ByVal e As EventArgs)

    Public Event ShowResidence(ByVal sender As Object, ByVal e As EventArgs)

    Public Event EditLocationBuilding(ByVal locationNumber As Integer, ByVal buildingNumber As Integer)
    Public Event EditLocationDwelling(ByVal locationNumber As Integer)
    'added 7/27/2015 for Farm
    Public Event ShowPolicyLevelCoverages(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowFarmPersonalProperty(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowInlandMarineAndRvWatercraft(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowIRPM(ByVal sender As Object, ByVal e As EventArgs)
    'added 7/28/2015 for Farm
    Public Event ClearDwelling(ByVal locationNumber As Integer)

    Public Event ShowFileUpload(ByVal sender As Object, ByVal e As EventArgs)
    'added 10/17/2016 for Verisk Protection Class (HOM and DFR)
    Public Event ViewVeriskProtectionClassReport(ByVal locationNumber As Integer)

    ' CPP Events - Added 2/15/18 MGB
    Public Event ShowInlandMarine()
    Public Event ShowCrime()
    Public Event DeleteCPP_InlandMarine()
    Public Event DeleteCPP_Crime()
    ' CPR
    Public Event ShowCPPCPRCoverages()
    Public Event ShowCPPCPRLocations(ByVal LocationIndex As Integer)
    Public Event EditCPPCPRLocation(ByVal LocationIndex As Integer)
    Public Event NewCPPCPRLocation(ByVal LocationIndex As Integer)
    Public Event DeleteCPPCPRLocation(ByVal LocationIndex As Integer)
    Public Event EditCPPCPRLocationBuilding(ByVal LocationIndex As Integer, ByVal BuildingIndex As Integer)
    ' CGL
    Public Event ShowCPPCGLCoverages()
    Public Event ShowCPPCGLLocations(ByVal LocationNumber As Integer)
    Public Event EditCPPCGLLocation(ByVal LocationIndex As Integer)
    Public Event NewCPPCGLLocation(ByVal LocationIndex As Integer)
    Public Event DeleteCPPCGLLocation(ByVal LocationIndex As Integer)
    Public Event EditCPPCGLLocationBuilding(ByVal LocationIndex As Integer, ByVal BuildingIndex As Integer)

    Public Event EffectiveDateChanging(NewEffectiveDate As String, OldEffectiveDate As String)
    Public Event EffectiveDateChangedFromTree(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType, ByVal newEffectiveDate As String, ByVal oldEffectiveDate As String) 'added 2/18/2020; specific to scenario where date is changed while on Summary screen, warranting a re-rate; pre-existing event is already being used for NewBusinessQuoting and shouldn't be highjacked for this since it will also be used for Endorsements

    'added 6/13/2019 for Endorsements/ReadOnly
    Public Event ShowPrintHistory(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowPolicyHistory(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ShowBillingInformation(ByVal sender As Object, ByVal e As EventArgs)

    Public Event PopulateInflationGuard() 'Added 10/20/2022 for task 77257 MLW
    Public Event PopulateCPRBuildingInformation()
    Public Event PopulateCAPCoverages()
    Public Event PopulateBOPCoverages()
    Public Event PopulateDFRCoverages()
    Public Event PopulateCPRCoverages()
    Public Event PopulateBOPBuildingInformation()
    Public Event RemoveFunctionalReplacementCost()
#End Region

#Region "Properties"
    'Added 03/30/2021 for CAP Endorsements Task 52973 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public Property InEditMode As Boolean
        Get
            If _InEditMode = Nothing Then
                If Me.hdnInEditModeFlag.Value IsNot Nothing Then
                    _InEditMode = QQHelper.BitToBoolean(Me.hdnInEditModeFlag.Value)
                Else
                    _InEditMode = False
                End If
            End If
            Return _InEditMode
        End Get
        Set(value As Boolean)
            _InEditMode = value
            If _InEditMode <> Nothing AndAlso _InEditMode = True Then
                Me.hdnInEditModeFlag.Value = "true" 'changed from yes to true 1/24/2014
            Else
                Me.hdnInEditModeFlag.Value = "false" 'changed from no to false 1/24/2014
            End If
        End Set
    End Property

    Public ReadOnly Property UseQuoteNumberHeader As Boolean
        Get
            Dim _UseQuoteNumberHeader As Boolean = False
            'decided to use hidden field instead so javascript could access it
            If Me.hdnUseQuoteNumberHeader.Value <> "" Then
                _UseQuoteNumberHeader = QQHelper.BitToBoolean(Me.hdnUseQuoteNumberHeader.Value)
            Else
                If ConfigurationManager.AppSettings("VR_Tree_UseQuoteNumberHeader") IsNot Nothing AndAlso ConfigurationManager.AppSettings("VR_Tree_UseQuoteNumberHeader").ToString <> "" Then
                    _UseQuoteNumberHeader = QQHelper.BitToBoolean(ConfigurationManager.AppSettings("VR_Tree_UseQuoteNumberHeader").ToString)
                Else
                    _UseQuoteNumberHeader = False
                End If

                'decided to use hidden field instead so javascript could access it
                Me.hdnUseQuoteNumberHeader.Value = LCase(_UseQuoteNumberHeader.ToString) 'needs to be lowercase since js is looking for 'true'
            End If
            Return _UseQuoteNumberHeader
        End Get
    End Property

#Region "Section Enabled"
    Public Property IsQuoteDescriptionSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnQuoteDescriptionSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnQuoteDescriptionSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsEffectiveDateSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnEffectiveDateSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnEffectiveDateSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsPolicyholderSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsDriverSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnDriverSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnDriverSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsVehicleSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnVehicleSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnVehicleSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsLocationSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnLocationSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnLocationSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCoverageSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCoverageSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCoverageSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsQuoteSummarySectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnQuoteSummarySection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnQuoteSummarySection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsUnderwritingQuestionSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnUnderwritingQuestionSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnUnderwritingQuestionSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsApplicationSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnApplicationSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnApplicationSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsApplicationSummarySectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnApplicationSummarySection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnApplicationSummarySection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCreditReportsSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCreditReportSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCreditReportSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsMvrReportsSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnMvrReportSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnMvrReportSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsClueReportsSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnClueReportSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnClueReportSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsResidenceSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnResidenceSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnResidenceSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsPolicyLevelCoverageSectionEnabled As Boolean 'added 7/27/2015 for Farm
        Get
            Return Not IsDisabledText(Me.hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsFarmPersonalPropertySectionEnabled As Boolean 'added 7/27/2015 for Farm
        Get
            Return Not IsDisabledText(Me.hdnFarmPersonalPropertySection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnFarmPersonalPropertySection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsInlandMarineAndRvWatercraftSectionEnabled As Boolean 'added 7/27/2015 for Farm
        Get
            Return Not IsDisabledText(Me.hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsIRPMSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnIRPMSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnIRPMSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsFileUploadSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnFileUploadSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnFileUploadSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCPRLocationsSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCPRLocationsSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCPRLocationsSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCPRPolicyLevelCoverageSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCPRPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCPRPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCGLLocationsSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCGLLocationSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCGLLocationSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCGLPolicyLevelCoverageSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCGLPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCGLPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCPRDetailHeaderSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCPRDetailHeaderSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCPRDetailHeaderSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCPPCGLDetailHeaderSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCPPCGLDetailHeaderSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCPPCGLDetailHeaderSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsInlandMarineSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnInlandMarineSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnInlandMarineSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsCrimeSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnCrimeSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnCrimeSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property

    'added 6/13/2019 for Endorsements/ReadOnly
    Public Property IsBillingInfoSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnBillingInfoSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnBillingInfoSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsPrintHistorySectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnPrintHistSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnPrintHistSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
    Public Property IsPolicyHistorySectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnPolicyHistSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnPolicyHistSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property

    'added 12/19/2022
    Public Property IsRouteToUWSectionEnabled As Boolean
        Get
            Return Not IsDisabledText(Me.hdnRouteToUWSection_EnabledOrDisabledFlag.Value)
        End Get
        Set(value As Boolean)
            Me.hdnRouteToUWSection_EnabledOrDisabledFlag.Value = GetEnabledDisabledText(value)
        End Set
    End Property
#End Region

#Region "EffectiveDate"
    Private _minDate As Integer = -1000
    Private _maxDate As Integer = -1000
    Public ReadOnly Property MinimumEffectiveDateDaysFromToday As Integer
        Get
            If _minDate = -1000 Then
                _minDate = QuickQuoteHelperClass.MinimumEffectiveDateDaysFromToday()
            End If

            Return _minDate
        End Get
    End Property
    Public ReadOnly Property MaximumEffectiveDateDaysFromToday As Integer
        Get
            If _maxDate = -1000 Then
                _maxDate = QuickQuoteHelperClass.MaximumEffectiveDateDaysFromToday()
            End If

            Return _maxDate
        End Get
    End Property
    Public Property MinimumEffectiveDate As String
        Get
            If QQHelper.IsDateString(Me.hdnMinimumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnMinimumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnMinimumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnMinimumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDate As String
        Get
            If QQHelper.IsDateString(Me.hdnMaximumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnMaximumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnMaximumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnMaximumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MinimumEffectiveDateAllQuotes As String
        Get
            If QQHelper.IsDateString(Me.hdnMinimumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnMinimumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnMinimumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnMinimumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDateAllQuotes As String
        Get
            If QQHelper.IsDateString(Me.hdnMaximumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnMaximumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If QQHelper.IsDateString(value) = True Then
                Me.hdnMaximumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnMaximumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property QuoteHasMinimumEffectiveDate As Boolean
        Get
            Return QQHelper.BitToBoolean(Me.hdnQuoteHasMinimumEffectiveDate.Value)
        End Get
        Set(value As Boolean)
            Me.hdnQuoteHasMinimumEffectiveDate.Value = value.ToString
        End Set
    End Property
    Public Property MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean
        Get
            Return QQHelper.BitToBoolean(Me.hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value)
        End Get
        Set(value As Boolean)
            Me.hdnMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value = value.ToString
        End Set
    End Property
    'added 5/16/2023
    Public Property BeforeDateMsg As String
        Get
            Return Me.hdnBeforeDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnBeforeDateMsg.Value = value
        End Set
    End Property
    Public Property AfterDateMsg As String
        Get
            Return Me.hdnAfterDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnAfterDateMsg.Value = value
        End Set
    End Property

    'added 2/17/2020; taken from ctlPolicyInfo.ascx.vb
    Private _EndorsementPastDateDays As Integer

    Public Property EndorsementPastDateDays() As Integer
        Get
            If _EndorsementPastDateDays = 0 Then
                _EndorsementPastDateDays = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysBack()
            End If
            Return _EndorsementPastDateDays
        End Get
        Set(ByVal value As Integer)
            _EndorsementPastDateDays = value
        End Set
    End Property

    Private _EndorsementFutureDateDays As Integer

    Public Property EndorsementFutureDateDays() As Integer
        Get
            If _EndorsementFutureDateDays = 0 Then
                _EndorsementFutureDateDays = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysForward()
            End If
            Return _EndorsementFutureDateDays
        End Get
        Set(ByVal value As Integer)
            _EndorsementFutureDateDays = value
        End Set
    End Property

    Private _EndorsementPastDate As Date

    Public Property EndorsementPastDate() As Date
        Get
            _EndorsementPastDate = Date.Today.AddDays(EndorsementPastDateDays)
            Return _EndorsementPastDate
        End Get
        Set(ByVal value As Date)
            _EndorsementPastDate = value
        End Set
    End Property

    Private _EndorsementFutureDate As Date

    Public Property EndorsementFutureDate() As Date
        Get
            _EndorsementFutureDate = Date.Today.AddDays(EndorsementFutureDateDays)
            Return _EndorsementFutureDate
        End Get
        Set(ByVal value As Date)
            _EndorsementFutureDate = value
        End Set
    End Property
#End Region

#End Region

    Private Sub ctlTreeView_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Me.hdnInEditModeFlag.Value = "false" ' Can't think of any scenario where the tree needs to retain its lock after postback - other controls can set the tree to lock on postback but the tree doesn't need to remember that from postback to postback
        Me.lblFileUploadCount.Text = Me.hdnTreeFileUploadCount.Value
    End Sub

    Public Sub New()
        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True
        If Page.IsPostBack = False Then
        End If

        'added 3/4/2014; moved outside of IsPostBack = False block so it would happen every time (since it seems to lose it [or it gets moved to BDP image] after the 1st time)
        Dim bdpTextbox As TextBox = Me.bdpEffectiveDate.FindControl("textbox")
        If bdpTextbox IsNot Nothing Then
            bdpTextbox.Attributes.Add("onblur", "BasicDatePickerTextboxOnblur(this);")
            bdpTextbox.Attributes.Add("onkeydown", "return (event.keyCode!=13);")
        End If
    End Sub

    Public Sub RefreshQuote()
        '****  DO NOT WRAP IN NULL CHECKS   
        '****  They do some logic if they are null so you want to always do them
        LoadTreeView()
    End Sub

    Public Sub RefreshRatedQuote()
        '****  DO NOT WRAP IN NULL CHECKS   
        '****  They do some logic if they are null so you want to always do them
        LoadRatedQuoteIntoTree()
    End Sub

    Private Sub LoadTreeView()
        UpdateViewMode() 'added 5/19/2014 to set to defaults

        Me.SwitchToUnRatedQuote()

        Dim hasPHs As Boolean = False
        Dim hasDrivers As Boolean = False
        Dim hasVehicles As Boolean = False
        Dim hasLocations As Boolean = False
        Dim hasCoverages As Boolean = False
        Dim hasUnderwritingQuestions As Boolean = False
        Dim hasTreeCoverages As Boolean = False
        Dim hasCreditReports As Boolean = False
        Dim hasMvrReports As Boolean = False
        Dim hasClueReports As Boolean = False
        Dim hasResidences As Boolean = False
        Me.pnlTreeView.Visible = False
        Me.pnlTreeViewError.Visible = False

        Me.lblNumberOfDrivers.Text = "0"
        Me.lblNumberOfVehicles.Text = "0"
        Me.lblNumberOfLocations.Text = "0"
        Me.lblNumberOfPolicyholders.Text = "0"
        Me.AddPolicyholderArea.Visible = False
        Me.lblNumberOfInlandMarinesAndRvWatercrafts.Text = "0"
        Me.lblFileUploadCount.Text = "0"
        Me.lblQuoteDescription.Text = ""
        Me.lblEffectiveDate.Text = ""
        Me.lblQuoteDescription.ToolTip = "Edit Quote Description" 'added 5/13/2014 to reset it to what's in the markup... so that the previous tooltip (which may contain the description) isn't persisted
        Me.lblEffectiveDate.ToolTip = "Edit Effective Date" 'added 5/13/2014 to reset it to what's in the markup... so that the previous tooltip (which may contain the effective date) isn't persisted
        InEditMode = False
        '3/5/2014 - updated to use display so javascript could still find them
        Me.QuoteDescriptionViewSection.Style.Add("display", "inline")
        Me.hdnQuoteDescriptionViewSect_Display.Value = "inline"
        Me.QuoteDescriptionEditSection.Style.Add("display", "none")
        Me.hdnQuoteDescriptionEditSect_Display.Value = "none"
        Me.EffectiveDateViewSection.Style.Add("display", "inline")
        Me.hdnEffectiveDateViewSect_Display.Value = "inline"
        Me.EffectiveDateEditSection.Style.Add("display", "none")
        Me.hdnEffectiveDateEditSect_Display.Value = "none"
        'If Me.Quote Is Nothing OrElse Me.Quote.QuoteNumber = "" Then
        '    Me.hdnQuoteNumber.Value = ""
        'End If
        'updated 2/15/2019 to also work for Endorsements and ReadOnly images
        If Me.Quote Is Nothing OrElse Me.Quote.PolicyNumber = "" Then
            Me.hdnQuoteNumber.Value = ""
        End If
        Me.hdnOriginalQuoteDescription.Value = ""
        Me.hdnOriginalEffectiveDate.Value = ""

        Me.liDrivers.Visible = False
        Me.liVehicles.Visible = False
        Me.liLocations.Visible = False
        Me.liCoverages.Visible = False
        Me.liUnderwritingQuestions.Visible = False
        Me.liQuoteSummary.Visible = False
        Me.liApplication.Visible = False
        Me.liApplicationSummary.Visible = False
        Me.liCreditReports.Visible = False
        Me.liMvrReports.Visible = False
        Me.liClueReports.Visible = False
        Me.liResidences.Visible = False
        'added 6/13/2019 for Endorsements/ReadOnly
        Me.liBillingInformation.Visible = False
        Me.liPrintHistory.Visible = False
        Me.liPolicyHistory.Visible = False
        Me.MainBillingInfoSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change
        Me.MainPrintHistSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change
        Me.MainPolicyHistSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change

        If Me.IsOnAppPage Then
            Me.TotalPremiumSection_Application.Visible = False
        Else
            Me.TotalPremiumSection.Visible = False
        End If
        Me.VehiclesPremiumSection.Visible = False
        Me.LocationsPremiumSection.Visible = False
        If Me.IsOnAppPage Then
            IsApplicationSummarySectionEnabled = False 'might add tooltip so user knows it's not available until re-rate
        Else
            IsQuoteSummarySectionEnabled = False 'might add tooltip so user knows it's not available until re-rate
        End If

        'added 7/27/2015 for Farm
        Me.liPolicyLevelCoverages.Visible = False
        Me.MainPolicyLevelCoverageSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change
        Me.liFarmPersonalProperty.Visible = False
        Me.MainFarmPersonalPropertySectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change
        Me.liInlandMarineAndRvWatercraft.Visible = False
        Me.MainInlandMarineAndRvWatercraftSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change
        Me.liIRPM.Visible = False
        Me.MainIRPMSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change

        ' Added FileUpload Matt A 12-3-2015
        Me.liFileUpload.Visible = False
        Me.MainFileUploadSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change

        'added 12/19/2022
        Me.liRouteToUW.Visible = False
        Me.MainRouteToUWSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden") 'note: could also default in markup since it probably won't change

        'added 9/16/2015
        Me.QuoteTypeSection.Visible = False
        Me.lblQuoteType.Text = ""

        ' Added 2/13/18 for CPP - MGB
        Me.liCPPCPRDetailHeader.Visible = False
        Me.liCPPCPRCoverages.Visible = False
        Me.liCPPCPRLocations.Visible = False
        Me.liCPPCGLDetailHeader.Visible = False

        If Me.Quote IsNot Nothing Then
            Me.pnlTreeView.Visible = True

            'added 2/22/2019
            UpdateTreeForViewMode(Me.Quote)

            With Me.Quote

                'Me.hdnQuoteNumber.Value = .QuoteNumber
                'If Me.hdnQuoteNumber.Value = "" AndAlso Me.Quote IsNot Nothing Then
                '    Me.hdnQuoteNumber.Value = Me.Quote.QuoteNumber
                'End If
                'updated 2/15/2019 to also work for Endorsements and ReadOnly images
                Me.hdnQuoteNumber.Value = .PolicyNumber
                '6/18/2019 note: if polNum starts w/ Q, quoteNum would be most accurate for image (since it could be different in the case of Diamond's copy quote, which creates a new quoteNum [imgQuoteNum] under same polNum [masterQuoteNum])
                If Me.hdnQuoteNumber.Value = "" AndAlso Me.Quote IsNot Nothing Then
                    Me.hdnQuoteNumber.Value = Me.Quote.PolicyNumber
                End If
                Me.hdnOriginalQuoteDescription.Value = .QuoteDescription
                If String.IsNullOrWhiteSpace(Me.hdnOriginalQuoteDescription.Value) = True AndAlso (Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then 'added IF 2/15/2019
                    Me.hdnOriginalQuoteDescription.Value = .TransactionRemark
                End If
                If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 2/15/2019; original logic in ELSE
                    Me.hdnOriginalEffectiveDate.Value = .TransactionEffectiveDate
                    If QQHelper.IsValidDateString(Me.hdnOriginalEffectiveDate.Value) = False Then
                        Me.hdnOriginalEffectiveDate.Value = .EffectiveDate
                    End If
                    'added 6/14/2019
                    If String.IsNullOrWhiteSpace(Me.Quote.PolicyNumber) = False AndAlso Left(UCase(Me.Quote.PolicyNumber), 1) = "Q" Then 'added IF 6/18/2019
                        Me.lblQuoteOrPolicy.Text = "Quote"
                    Else
                        Me.lblQuoteOrPolicy.Text = "Policy"
                    End If
                    'Me.lblDescriptionOrRemarks.Text = "Remarks" 'added 7/5/2019
                    Me.DescriptionOrRemarksText.InnerHtml = "Remarks" 'added 7/5/2019
                Else
                    Me.hdnOriginalEffectiveDate.Value = .EffectiveDate
                    'added 6/14/2019
                    Me.lblQuoteOrPolicy.Text = "Quote"
                    'Me.lblDescriptionOrRemarks.Text = "Description" 'added 7/5/2019
                    Me.DescriptionOrRemarksText.InnerHtml = "Description" 'added 7/5/2019
                End If
                ResetQuoteDescriptionAndEffectiveDateToOriginalLabels(tranType:=Me.Quote.QuoteTransactionType, termEffDate:=Me.Quote.EffectiveDate, termExpDate:=Me.Quote.ExpirationDate, tranEffDate:=Me.Quote.TransactionEffectiveDate, tranExpDate:=Me.Quote.TransactionExpirationDate) 'updated 7/23/2019 w/ optional params

                'added 12/19/2022
                UpdateTreeForRouteToUW()

                SetMinimumAndMaximumEffectiveDates(Me.Quote)

                Me.DetermineIsIRPMSectionEnabled()

                If .LobType <> Nothing AndAlso .LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    Select Case .LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            Me.liVehicles.Visible = True
                            Me.liUnderwritingQuestions.Visible = True 'added 3/18/2014; 5/19/2014 note: now on App side... just below Quote Summary
                            Me.liQuoteSummary.Visible = True
                            Me.liApplication.Visible = True
                            Me.liApplicationSummary.Visible = True
                            Me.liFileUpload.Visible = True 'added 12-3-2015 Matt A
                            Select Case Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                    Me.liDrivers.Visible = True
                                    Me.liCoverages.Visible = True
                                    Me.liCreditReports.Visible = True 'added 5/19/2014
                                    Me.liMvrReports.Visible = True 'added 5/20/2014
                                    Me.liClueReports.Visible = True 'added 5/20/2014
                                    Exit Select
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                                    'Updated 11/23/2020 for CAP Endorsements Tasks 52975, 52977 and 52983 MLW
                                    Me.liPolicyLevelCoverages.Visible = True
                                    If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                                        Me.liDrivers.Visible = True
                                        Me.liIRPM.Visible = False
                                        Me.liMvrReports.Visible = True
                                    Else
                                        Me.liIRPM.Visible = True
                                        Me.IrpmTitle.InnerText = "Credits/Debits"
                                        Me.warningGif.Alt = "Credits/Debits Information Required"
                                        Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                                    End If
                                    'Me.liIRPM.Visible = True
                                    'Me.liPolicyLevelCoverages.Visible = True
                                    'Me.IrpmTitle.InnerText = "Credits/Debits"
                                    'Me.warningGif.Alt = "Credits/Debits Information Required"
                                    'Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                                    Exit Select
                            End Select
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            Me.liUnderwritingQuestions.Visible = True '5/19/2014 note: now on App side... just below Quote Summary
                            Me.liQuoteSummary.Visible = True
                            Me.liApplication.Visible = True
                            Me.liApplicationSummary.Visible = True
                            Me.liResidences.Visible = True 'added 6/12/2014
                            Me.liCoverages.Visible = True 'added 6/12/2014

                            Me.liUnderwritingQuestions.Visible = True 'added 6/12/2014
                            Me.liClueReports.Visible = True 'added here 10/23/2015; previously just for HOM
                            If .LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then 'added 9/29/2014
                                Me.liCreditReports.Visible = True
                            End If

                            Me.liFileUpload.Visible = True 'added 12-3-2015 Matt A
                        Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            liCoverages.Visible = True
                            liQuoteSummary.Visible = True
                            liIRPM.Visible = True
                            liUnderwritingQuestions.Visible = True
                            liApplication.Visible = True
                            liApplicationSummary.Visible = True
                            liFileUpload.Visible = True
                            'Change IRPM to Credits/Debits
                            Me.liIRPM.Visible = True
                            Me.IrpmTitle.InnerText = "Credits/Debits"
                            Me.warningGif.Alt = "Credits/Debits Information Required"
                            Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                            Exit Select

                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage  ' Added for CPP 2/13/18 MGB
                            'Updated 12/21/2021 for CPP Endorsements Tasks 64575, 65345, 66484 MLW
                            If IsQuoteEndorsement() Then
                                Me.liCPPCPRDetailHeader.Visible = False
                                Me.liCPPCPRCoverages.Visible = False
                                Me.liCPPCGLDetailHeader.Visible = False
                                Me.liCPPCGLCoverages.Visible = False
                                Me.liCrime.Visible = False
                                Me.CPPCPRLocationsTitle.InnerText = "Property Locations" 'Added 12/21/2021 for CP Endorsements Task 66834 MLW
                                Me.CPPCGLLocationsTitle.InnerText = "GL Locations/Class Codes" 'Added 12/21/2021 for CP Endorsements Task 66834 MLW
                                Me.CPPCGLLocationsTip.Attributes.Item("Title") = "Show GL Class Codes" 'Added 12/22/2021 for CP Endorsements Task 66834 MLW
                                'Added 12/22/2021 for CP Endorsements Task 66834 MLW
                                ' 04/29/2022 CAH - Removed IM because VR and AI Manager Mismatch in Diamond.
                                'If (GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any()) OrElse TypeOfEndorsement() = EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder Then
                                '    Me.liInlandMarine.Visible = True
                                'Else
                                Me.liInlandMarine.Visible = False
                                'End If
                            Else
                                Me.liCPPCPRDetailHeader.Visible = True
                                Me.liCPPCPRCoverages.Visible = True
                                Me.liCPPCGLDetailHeader.Visible = True
                                Me.liCPPCGLCoverages.Visible = True
                                Me.liCrime.Visible = True
                                Me.liInlandMarine.Visible = True
                            End If
                            Me.liCPPCPRLocations.Visible = True
                            Me.liCPPCGLLocations.Visible = True
                            Me.liQuoteSummary.Visible = True
                            Me.liApplication.Visible = True
                            Me.liApplicationSummary.Visible = True
                            Me.liUnderwritingQuestions.Visible = True
                            ' Me.liIRPM.Visible = True 'removed 11/23/2021 as duplicate MLW, also shown below
                            'Me.liInlandMarine.Visible = True ' moved above to if statement for endorsements MLW 12/22/2021

                            liFileUpload.Visible = True
                            'Updated 11/23/2021 for CPP Endorsements Tasks 65408 and 64513 MLW
                            If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                                Me.liIRPM.Visible = False
                            Else
                                Me.liIRPM.Visible = True
                                Me.IrpmTitle.InnerText = "Credits/Debits"
                                Me.warningGif.Alt = "Credits/Debits Information Required"
                                Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                                ' Show the IM/Crime delete buttons if the quote has IM/Crime
                                Me.im_DeleteButtonArea.Visible = GoverningStateQuote.CPP_Has_InlandMarine_PackagePart AndAlso Me.SubQuoteFirst.HasContractorsEnhancement = False AndAlso Me.SubQuoteFirst.HasFoodManufacturersEnhancement = False
                                Me.crime_DeleteButtonArea.Visible = Quote.CPP_Has_Crime_PackagePart
                            End If
                            'Me.liIRPM.Visible = True
                            'Me.IrpmTitle.InnerText = "Credits/Debits"
                            'Me.warningGif.Alt = "Credits/Debits Information Required"
                            'Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                            '' Show the IM/Crime delete buttons if the quote has IM/Crime
                            'Me.im_DeleteButtonArea.Visible = Quote.CPP_Has_InlandMarine_PackagePart AndAlso Me.SubQuoteFirst.HasContractorsEnhancement = False
                            'Me.crime_DeleteButtonArea.Visible = Quote.CPP_Has_Crime_PackagePart
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.Farm
                            Me.liLocations.Visible = True
                            Me.liQuoteSummary.Visible = True
                            Me.liApplication.Visible = True
                            Me.liApplicationSummary.Visible = True
                            If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then Me.liPolicyLevelCoverages.Visible = True ' Matt A 12-5-2015 for CGL
                            If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then Me.liPolicyLevelCoverages.Visible = True
                            Me.liUnderwritingQuestions.Visible = True
                            If .LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then  ' added 4/24/17 MGB
                                'Updated 09/20/2021 for BOP Endorsements task 63922 and 63816 MLW
                                'Me.liIRPM.Visible = True
                                If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                                    Me.liIRPM.Visible = False
                                    If IsQuoteEndorsement() Then
                                        Me.liPolicyLevelCoverages.Visible = False
                                    End If
                                Else
                                    Me.liIRPM.Visible = True
                                End If
                                Me.liFileUpload.Visible = True ' added 5/12/2017 CHawley
                            End If
                            If .LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then 'added 7/27/2015
                                Me.liPolicyLevelCoverages.Visible = True
                                Me.liFarmPersonalProperty.Visible = True
                                Me.liInlandMarineAndRvWatercraft.Visible = True
                                Me.liIRPM.Visible = True
                                Me.liCreditReports.Visible = True 'added 8/7/2015
                                If FarmClueHelper.IsFarClueAvailable(Quote) Then
                                    Me.liClueReports.Visible = True
                                End If

                                'added 8/11/2015
                                Dim programType = Me.GetProgramType()

                                If UCase(programType) = "FARM LIABILITY" Then
                                    Me.liFarmPersonalProperty.Visible = False
                                    Me.liInlandMarineAndRvWatercraft.Visible = False
                                End If

                                'added 9/16/2015
                                If String.IsNullOrEmpty(programType) = False Then
                                    Dim phNameType As String = ""
                                    If .Policyholder?.Name IsNot Nothing AndAlso QQHelper.IsValidQuickQuoteIdOrNum(.Policyholder.Name.TypeId) = True Then
                                        phNameType = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, .Policyholder.Name.TypeId)
                                        If UCase(phNameType).Contains("PERSONAL") = True Then
                                            If Me.SubQuoteFirst.LiabilityOptionId <> "2" Then ' added if on 2-16-2016 Bug 6380
                                                phNameType = "Personal"
                                            Else
                                                If .Locations(0).HobbyFarmCredit = False Then
                                                    phNameType = "Personal"
                                                Else
                                                    phNameType = "Commercial"
                                                End If
                                            End If
                                        ElseIf UCase(phNameType).Contains("COMMERCIAL") = True Then
                                            phNameType = "Commercial"
                                        Else
                                            phNameType = ""
                                        End If
                                    End If

                                    Dim hobbyFarm As String = ""
                                    If .Locations(0).HobbyFarmCredit Then
                                        hobbyFarm = "- Hobby Farm"
                                    Else
                                        hobbyFarm = ""
                                    End If

                                    Me.lblQuoteType.Text = $"{phNameType} {programType} {hobbyFarm}"
                                    If String.IsNullOrEmpty(Me.lblQuoteType.Text) = False Then
                                        Me.QuoteTypeSection.Visible = True
                                    End If
                                End If
                                Me.liFileUpload.Visible = True
                            End If
                            Select Case Quote.LobType
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                                    Me.liIRPM.Visible = True
                                    Me.liPolicyLevelCoverages.Visible = True
                                    Me.liFileUpload.Visible = True
                                    Me.IrpmTitle.InnerText = "Credits/Debits"
                                    Me.warningGif.Alt = "Credits/Debits Information Required"
                                    Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                                    Me.liIRPM.Visible = True
                                    Me.liFileUpload.Visible = True
                                    Me.IrpmTitle.InnerText = "Credits/Debits"
                                    Me.warningGif.Alt = "Credits/Debits Information Required"
                                    Me.IrpmTip.Attributes.Item("Title") = "Show Credits/Debits"
                            End Select
                        Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                            liCoverages.Visible = True
                            liQuoteSummary.Visible = True
                            liApplication.Visible = True
                            liApplicationSummary.Visible = True
                            liFileUpload.Visible = True
                            Exit Select
                    End Select
                End If

                'added 8/22/2019
                Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = GetQuoteTransactionTypeFlagForTree()
                If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                        'note: added runat="server" to QuoteDescriptionHeader span in markup
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "title") 'remove "Edit Quote Description"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "class") 'remove "clickableHeader"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "onclick")

                        Me.lblQuoteDescription.ToolTip = "" 'remove "Edit Quote Description"
                        Me.lblQuoteDescription.CssClass = "" 'remove "clickable"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromWebControl(Me.lblQuoteDescription, "OnClick")

                        'added 6/13/2019 for Endorsements/ReadOnly
                        Me.liBillingInformation.Visible = True
                        Me.liPrintHistory.Visible = False 'Task 40643
                        Me.liPolicyHistory.Visible = True
                        Me.liQuoteSummary.Visible = False
                        Me.liFileUpload.Visible = False

                        'added 7/15/2019
                        If Request IsNot Nothing AndAlso Request.ServerVariables IsNot Nothing AndAlso Request.ServerVariables.Count > 0 AndAlso Request.ServerVariables("SCRIPT_NAME") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.ServerVariables("SCRIPT_NAME").ToString) = False Then
                            Dim currPage As String = UCase(Request.ServerVariables("SCRIPT_NAME").ToString)
                            If currPage.Contains("VREPOLICYINFO") = True Then
                                If Me.IsOnAppPage Then 'disable the sections that are normally enabled
                                    EnableDisableApplicationSectionHeaders(False)
                                Else
                                    EnableDisableQuoteSectionHeaders(False)
                                End If
                                'added 7/18/2019; remove other sections that aren't disabled with above calls; no event handlers on Page, so they wouldn't work anyway
                                Me.liCreditReports.Visible = False
                                Me.liMvrReports.Visible = False
                                Me.liClueReports.Visible = False
                                Me.liBillingInformation.Visible = False
                                Me.liPrintHistory.Visible = False
                                Me.liPolicyHistory.Visible = False
                            End If
                        End If
                    End If

                    If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse QuickQuoteHelperClass.Endorsements_AllowTransactionEffectiveDateChange() = False Then 'added IF 2/14/2020
                        'note: added runat="server" to EffectiveDateHeader span in markup
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.EffectiveDateHeader, "title") 'remove "Edit Effective Date"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.EffectiveDateHeader, "class") 'remove "clickableHeader"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.EffectiveDateHeader, "onclick")

                        Me.lblEffectiveDate.ToolTip = "" 'remove "Edit Effective Date"
                        Me.lblEffectiveDate.CssClass = "" 'remove "clickable"
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromWebControl(Me.lblEffectiveDate, "OnClick")
                    End If

                    'moved here 6/13/2019 from below
                    Me.liUnderwritingQuestions.Visible = False
                    Me.liApplication.Visible = False
                    Me.liApplicationSummary.Visible = False

                    'added 3/29/2019
                    If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        'removed 6/13/2019; now being done for Endorsements and ReadOnly above
                        'Me.liUnderwritingQuestions.Visible = False
                        'Me.liApplication.Visible = False
                        'Me.liApplicationSummary.Visible = False

                        'Updated 10/13/2021 for BOP Endorsements Task 63816 MLW
                        ''Added 11/23/2020 for CAP Endorsements Task 52977 MLW
                        'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        If IsCommercialQuote() Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "title") 'remove "Edit Quote Description"
                            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "class") 'remove "clickableHeader"
                            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(Me.QuoteDescriptionHeader, "onclick")

                            Me.lblQuoteDescription.ToolTip.Replace("Quote Description", "Remarks")
                            Me.lblQuoteDescription.CssClass = "" 'remove "clickable"
                            IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromWebControl(Me.lblQuoteDescription, "OnClick")
                        End If

                        Me.qs_ValidationErrorImage.Alt = Me.qs_ValidationErrorImage.Alt.Replace("Quote", "Change")
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddAttributeToHtmlControl(Me.qs_ValidationErrorImage, "title", Me.qs_ValidationErrorImage.Alt)
                        Me.imgBtnQuoteSummary.ToolTip = Me.imgBtnQuoteSummary.ToolTip.Replace("Quote", "Change")
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddAttributeToGenericControl(Me.qs_Header, "title", "Show Change Summary")
                        Me.qs_Header.InnerHtml = Me.qs_Header.InnerHtml.Replace("Quote", "Change")

                        'added 6/27/2019
                        IsFileUploadSectionEnabled = True

                        'added 7/5/2019
                        'IFM.VR.Web.Helpers.WebHelper_Personal.AddAttributeToWebControl(Me.lblDescriptionOrRemarks, "id", Me.lblDescriptionOrRemarks.ID) 'needed so javascript can find id; didn't work... will use span; was looking at the wrong level, so this really wouldn't have been needed anyway
                        IFM.VR.Web.Helpers.WebHelper_Personal.ReplaceTextInAttributeValueForGenericControl(Me.QuoteDescriptionHeader, "title", "Quote Description", "Remarks")
                        Me.lblQuoteDescription.ToolTip = Me.lblQuoteDescription.ToolTip.Replace("Quote Description", "Remarks")
                        Me.imgBtnEditQuoteDescription.ToolTip = Me.imgBtnEditQuoteDescription.ToolTip.Replace("Quote Description", "Remarks")
                        Me.imgBtnSaveQuoteDescription.ToolTip = Me.imgBtnSaveQuoteDescription.ToolTip.Replace("Quote Description", "Remarks")
                        Me.imgBtnCancelSaveQuoteDescription.ToolTip = Me.imgBtnCancelSaveQuoteDescription.ToolTip.Replace("Quote Description", "Remarks")
                        Me.imgBtnCancelSaveQuoteDescription.OnClientClick = Me.imgBtnCancelSaveQuoteDescription.OnClientClick.Replace("Quote Description", "Remarks")

                        'added 7/12/2019
                        If .IsBillingEndorsement() = True Then
                            Me.liCreditReports.Visible = False
                            Me.liMvrReports.Visible = False
                            Me.liClueReports.Visible = False
                            Me.liBillingInformation.Visible = True

                            If Me.IsOnAppPage Then 'disable the sections that are normally enabled
                                EnableDisableApplicationSectionHeaders(False)
                            Else
                                EnableDisableQuoteSectionHeaders(False)
                                're-enable specified sections as needed
                                Dim enabledDisabledText As String = GetEnabledDisabledText(True)
                                Me.hdnQuoteDescriptionSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                            End If
                        End If
                    End If

                    'Endorsement Specific Changes by LOB - CAH 8/7/2020
                    Select Case .LobType
                        Case QuickQuoteObject.QuickQuoteLobType.Farm
                            Me.liIRPM.Visible = False
                        Case Else
                            'Me.liIRPM.Visible = True
                    End Select

                End If

                If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019; original logic in ELSE
                    Me.AddPolicyholderArea.Visible = False
                    'added 6/14/2019
                    Me.AddCPPCPRLocationSection.Style.Add("visibility", "hidden")
                    Me.AddDriverSection.Style.Add("visibility", "hidden")
                    Me.AddVehicleSection.Style.Add("visibility", "hidden")
                    Me.AddLocationSection.Style.Add("visibility", "hidden")
                Else
                    Me.AddPolicyholderArea.Visible = True
                    'added 6/14/2019
                    'Me.AddCPPCPRLocationSection.Style.Add("visibility", "visible") 'moved below to showHide for commercial & endorsements 12/22/2021 MLW
                    'Updated 03/11/2021 and 04/02/2021 for CAP Endorsements Task 52977 MLW
                    Dim showHideAddDriverSection As String = "visible"
                    Dim showHideAddVehicleSection As String = "visible"
                    Dim showHideAddLocationSection As String = "visible" 'Added 10/12/2021 for BOP Endorsements Task 63816 MLW
                    Dim showHideAddCPPCPRLocationSection As String = "visible" 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
                    If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        Dim endorsementDriverTransactionCount As Integer = 0
                        If TypeOfEndorsement() = "Add/Delete Driver" Then
                            endorsementDriverTransactionCount = ddh.GetEndorsementTransactionCount()
                        End If
                        'Dim endorsementDriverTransactionCount As Integer = ddh.GetEndorsementTransactionCount()
                        If IsQuoteEndorsement() AndAlso endorsementDriverTransactionCount >= 3 Then
                            showHideAddDriverSection = "hidden"
                        Else
                            showHideAddDriverSection = "visible"
                        End If

                        Dim endorsementVehicleTransactionCount As Integer = 0
                        If TypeOfEndorsement() = "Add/Delete Vehicle" Then
                            endorsementVehicleTransactionCount = ddh.GetEndorsementVehicleTransactionCount()
                        End If
                        'Dim endorsementVehicleTransactionCount As Integer = ddh.GetEndorsementVehicleTransactionCount()
                        If endorsementVehicleTransactionCount >= 3 OrElse TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                            showHideAddVehicleSection = "hidden"
                        Else
                            showHideAddVehicleSection = "visible"
                        End If
                    Else
                        showHideAddDriverSection = "visible"
                        showHideAddVehicleSection = "visible"
                    End If
                    'Added 10/12/2021 for BOP Endorsements Task 63816 MLW
                    If IsQuoteEndorsement() AndAlso IsCommercialQuote() Then
                        showHideAddLocationSection = "hidden"
                        showHideAddCPPCPRLocationSection = "hidden" 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
                    Else
                        showHideAddLocationSection = "visible"
                        showHideAddCPPCPRLocationSection = "visible" 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
                    End If
                    Me.AddDriverSection.Style.Add("visibility", showHideAddDriverSection)
                    'Me.AddDriverSection.Style.Add("visibility", "visible")
                    Me.AddVehicleSection.Style.Add("visibility", showHideAddVehicleSection)
                    'Me.AddVehicleSection.Style.Add("visibility", "visible")
                    Me.AddLocationSection.Style.Add("visibility", showHideAddLocationSection) 'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                    'Me.AddLocationSection.Style.Add("visibility", "visible")
                    Me.AddCPPCPRLocationSection.Style.Add("visibility", showHideAddCPPCPRLocationSection) 'Updated 12/22/2021 for CPP Endorsements Task 66834 MLW

                End If

                Dim hasPH1 As Boolean = False
                Dim hasPH2 As Boolean = False
                If .Policyholder IsNot Nothing AndAlso .Policyholder.HasData = True AndAlso ((.Policyholder.Name IsNot Nothing AndAlso .Policyholder.Name.HasData = True) OrElse (.Policyholder.Address IsNot Nothing AndAlso .Policyholder.Address.HasData = True) OrElse (.Policyholder.Emails.IsLoaded()) OrElse (.Policyholder.Phones.IsLoaded())) Then 'may not need to check HasData
                    hasPHs = True
                    hasPH1 = True
                    Me.lblNumberOfPolicyholders.Text = "1"
                End If
                If .Policyholder2 IsNot Nothing AndAlso .Policyholder2.HasData = True AndAlso ((.Policyholder2.Name IsNot Nothing AndAlso .Policyholder2.Name.HasData = True) OrElse (.Policyholder2.Address IsNot Nothing AndAlso .Policyholder2.Address.HasData = True) OrElse (.Policyholder2.Emails.IsLoaded()) OrElse (.Policyholder2.Phones.IsLoaded())) Then 'may not need to check HasData
                    hasPHs = True
                    hasPH2 = True
                    Me.lblNumberOfPolicyholders.Text = "2"
                    Me.AddPolicyholderArea.Visible = False
                End If
                If hasPHs = True Then
                    Dim ph1Applicant As QuickQuoteApplicant = Nothing
                    Dim ph2Applicant As QuickQuoteApplicant = Nothing
                    Dim ph1AppNum As Integer = 0
                    Dim ph2AppNum As Integer = 0

                    If Me.GoverningStateQuote?.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Any() Then
                        Dim appCounter As Integer = 0
                        For Each app As QuickQuoteApplicant In Me.GoverningStateQuote.Applicants
                            appCounter += 1
                            Dim isPolicyholder1 As Boolean = False
                            Dim isPolicyholder2 As Boolean = False
                            If QQHelper.IsQuickQuoteApplicantPolicyholder(app, isPolicyholder1, isPolicyholder2) = True Then
                                If ph1Applicant Is Nothing AndAlso isPolicyholder1 = True Then
                                    ph1Applicant = app
                                    ph1AppNum = appCounter
                                ElseIf ph2Applicant Is Nothing AndAlso isPolicyholder2 = True Then
                                    ph2Applicant = app
                                    ph2AppNum = appCounter
                                End If
                            End If
                            If ph1Applicant IsNot Nothing AndAlso ph2Applicant IsNot Nothing Then
                                Exit For
                            End If
                        Next
                    End If

                    Dim dtPolicyholders As New DataTable
                    dtPolicyholders.Columns.AddStrings("PolicyholderDescription", "PolicyholderNumber", "Applicant", "ApplicantNumber", "Applicant2", "Applicant2Number")
                    Dim drPH1 As DataRow
                    drPH1 = dtPolicyholders.NewRow
                    drPH1.Item("PolicyholderNumber") = "1"
                    If hasPH1 = True Then
                        drPH1.Item("PolicyholderDescription") = If(.Policyholder?.Name?.DisplayName IsNot Nothing AndAlso .Policyholder.Name.DisplayName.IsNullEmptyorWhitespace() = False, .Policyholder.Name.DisplayName, "Policyholder 1")
                    Else
                        drPH1.Item("PolicyholderDescription") = "Policyholder 1"
                    End If

                    If ph1Applicant?.Name?.DisplayName IsNot Nothing AndAlso ph1Applicant.Name.DisplayName.IsNullEmptyorWhitespace() = False AndAlso UCase(ph1Applicant.Name.DisplayName) <> UCase(drPH1.Item("PolicyholderDescription").ToString) Then
                        drPH1.Item("Applicant") = ph1Applicant.Name.DisplayName
                        drPH1.Item("ApplicantNumber") = ph1AppNum.ToString
                        If hasPH2 = False AndAlso ph2Applicant?.Name?.DisplayName IsNot Nothing AndAlso ph2Applicant.Name.DisplayName.IsNullEmptyorWhitespace() = False Then
                            drPH1.Item("Applicant2") = ph2Applicant.Name.DisplayName
                            drPH1.Item("Applicant2Number") = ph2AppNum.ToString
                        Else
                            drPH1.Item("Applicant2") = ""
                            drPH1.Item("Applicant2Number") = ""
                        End If
                    Else
                        drPH1.Item("Applicant") = ""
                        drPH1.Item("ApplicantNumber") = ""
                        drPH1.Item("Applicant2") = ""
                        drPH1.Item("Applicant2Number") = ""
                    End If

                    dtPolicyholders.Rows.Add(drPH1)
                    If hasPH2 = True Then
                        Dim drPH2 As DataRow
                        drPH2 = dtPolicyholders.NewRow
                        drPH2.Item("PolicyholderNumber") = "2"
                        drPH2.Item("PolicyholderDescription") = If(.Policyholder2?.Name?.DisplayName IsNot Nothing AndAlso .Policyholder2.Name.DisplayName.IsNullEmptyorWhitespace() = False, .Policyholder2.Name.DisplayName, "Policyholder 2")
                        drPH2.Item("Applicant2") = ""
                        drPH2.Item("Applicant2Number") = ""

                        If ph2Applicant?.Name?.DisplayName IsNot Nothing AndAlso ph2Applicant.Name.DisplayName.IsNullEmptyorWhitespace() = False AndAlso UCase(ph2Applicant.Name.DisplayName) <> UCase(drPH2.Item("PolicyholderDescription").ToString) Then
                            drPH2.Item("Applicant") = ph2Applicant.Name.DisplayName
                            drPH2.Item("ApplicantNumber") = ph2AppNum.ToString
                        Else
                            drPH2.Item("Applicant") = ""
                            drPH2.Item("ApplicantNumber") = ""
                        End If

                        dtPolicyholders.Rows.Add(drPH2)
                    End If

                    Me.rptPolicyholders.DataSource = dtPolicyholders
                    Me.rptPolicyholders.DataBind()
                End If

                'removed 6/28/2019; now being done from LoadDrivers method call below
                'If Me.GoverningStateQuote IsNot Nothing Then
                '    'drivers
                '    If Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count > 0 Then
                '        Me.lblNumberOfDrivers.Text = Me.GoverningStateQuote.Drivers.Count.ToString 'added 1/9/2014
                '        hasDrivers = True
                '        Dim dtDrivers As New DataTable
                '        dtDrivers.Columns.AddStrings("DriverDescription", "DriverNumber")
                '        Dim driverNum As Integer = 0

                '        For Each d As QuickQuoteDriver In Me.GoverningStateQuote.Drivers
                '            driverNum += 1
                '            Dim drDriver As DataRow
                '            drDriver = dtDrivers.NewRow
                '            If d.Name.DisplayName <> "" Then
                '                drDriver.Item("DriverDescription") = d.Name.DisplayName
                '            Else
                '                drDriver.Item("DriverDescription") = "Driver " & driverNum.ToString
                '            End If
                '            drDriver.Item("DriverNumber") = driverNum.ToString 'added 1/3/2014
                '            dtDrivers.Rows.Add(drDriver)
                '        Next
                '        Me.rptDrivers.DataSource = dtDrivers
                '        Me.rptDrivers.DataBind()
                '    End If
                'End If

                If .Locations.IsLoaded() Then
                    Select Case .LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            ' CPP ONLY - Added 2/15/18 MGB
                            ' CPP-CPR
                            Me.lblCPPCPRNumberOfLocations.Text = Me.Quote.Locations.Count.ToString
                            hasLocations = True
                            Dim dtLocations As New DataTable
                            dtLocations.Columns.AddStrings("LocationDescription", "LocationNumber")
                            Dim CPRlocationNum As Integer = 0

                            For Each l As QuickQuoteLocation In .Locations
                                CPRlocationNum += 1
                                Dim drLocation As DataRow
                                drLocation = dtLocations.NewRow
                                If String.IsNullOrEmpty(l.Address.HouseNum) = False OrElse String.IsNullOrEmpty(l.Address.StreetName) = False Then
                                    drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                ElseIf l.Acreages.IsLoaded() AndAlso ((String.IsNullOrEmpty(l.Acreages(0).Section) = False AndAlso l.Acreages(0).Section <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Twp) = False AndAlso l.Acreages(0).Twp <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Range) = False AndAlso l.Acreages(0).Range <> "0")) Then
                                    Dim strDisplayAddress As String = If(String.IsNullOrEmpty(l.Acreages(0).Section) = False, l.Acreages(0).Section, "S") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Twp) = False, l.Acreages(0).Twp, "T") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Range) = False, l.Acreages(0).Range, "R")
                                    Dim cityStateZip As String = QQHelper.appendText(l.Address.City, l.Address.State, ", ")
                                    cityStateZip = QQHelper.appendText(cityStateZip, l.Address.Zip, " ")
                                    strDisplayAddress = QQHelper.appendText(strDisplayAddress, cityStateZip, vbCrLf)
                                    drLocation.Item("LocationDescription") = strDisplayAddress
                                Else
                                    If l.Address.DisplayAddress.NotEqualsAny("", "IN", "00000", "00000-0000") Then
                                        drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                    Else
                                        drLocation.Item("LocationDescription") = "Location " & CPRlocationNum.ToString
                                    End If
                                End If
                                drLocation.Item("LocationNumber") = CPRlocationNum.ToString
                                dtLocations.Rows.Add(drLocation)
                            Next
                            Me.rptCPPCPRLocations.DataSource = dtLocations
                            Me.rptCPPCPRLocations.DataBind()

                            ' CPP-CGL
                            Me.lblCPPCGLNumberOfLocations.Text = Me.Quote.Locations.Count.ToString
                            hasLocations = True
                            Dim dtCGLLocations As New DataTable
                            dtCGLLocations.Columns.AddStrings("LocationDescription", "LocationNumber")
                            Dim CGLlocationNum As Integer = 0

                            For Each l As QuickQuoteLocation In .Locations
                                CGLlocationNum += 1
                                Dim drLocation As DataRow
                                drLocation = dtCGLLocations.NewRow
                                If String.IsNullOrEmpty(l.Address.HouseNum) = False OrElse String.IsNullOrEmpty(l.Address.StreetName) = False Then
                                    drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                ElseIf l.Acreages IsNot Nothing AndAlso l.Acreages.Count > 0 AndAlso ((String.IsNullOrEmpty(l.Acreages(0).Section) = False AndAlso l.Acreages(0).Section <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Twp) = False AndAlso l.Acreages(0).Twp <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Range) = False AndAlso l.Acreages(0).Range <> "0")) Then
                                    Dim strDisplayAddress As String = If(String.IsNullOrEmpty(l.Acreages(0).Section) = False, l.Acreages(0).Section, "S") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Twp) = False, l.Acreages(0).Twp, "T") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Range) = False, l.Acreages(0).Range, "R")
                                    Dim cityStateZip As String = QQHelper.appendText(l.Address.City, l.Address.State, ", ")
                                    cityStateZip = QQHelper.appendText(cityStateZip, l.Address.Zip, " ")
                                    strDisplayAddress = QQHelper.appendText(strDisplayAddress, cityStateZip, vbCrLf)
                                    drLocation.Item("LocationDescription") = strDisplayAddress
                                Else
                                    If l.Address.DisplayAddress.NotEqualsAny("", "IN", "00000", "00000-0000") Then
                                        drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                    Else
                                        drLocation.Item("LocationDescription") = "Location " & CGLlocationNum.ToString
                                    End If
                                End If
                                drLocation.Item("LocationNumber") = CGLlocationNum.ToString
                                dtCGLLocations.Rows.Add(drLocation)
                            Next
                            Me.rptCPPCGLLocations.DataSource = dtCGLLocations
                            Me.rptCPPCGLLocations.DataBind()

                            Exit Select
                        Case Else
                            ' ALL LINES EXCEPT CPP
                            Me.lblNumberOfLocations.Text = Me.Quote.Locations.Count.ToString 'added 1/9/2014
                            hasLocations = True
                            Dim dtLocations As New DataTable
                            dtLocations.Columns.AddStrings("LocationDescription", "LocationNumber")
                            Dim locationNum As Integer = 0

                            'added 7/27/2015 for Farm (IM and RvWatercraft); may not need
                            Dim hasInlandMarine As Boolean = False 'may need to declare at the beginning of method instead of just inside Locations IF block
                            Dim hasRvWatercraft As Boolean = False 'may need to declare at the beginning of method instead of just inside Locations IF block

                            Dim locImCount As Integer = 0
                            Dim locRvCount As Integer = 0

                            For Each l As QuickQuoteLocation In .Locations
                                locationNum += 1
                                Dim drLocation As DataRow
                                drLocation = dtLocations.NewRow


                                If String.IsNullOrEmpty(l.Address.HouseNum) = False OrElse String.IsNullOrEmpty(l.Address.StreetName) = False Then
                                    drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                ElseIf l.Acreages IsNot Nothing AndAlso l.Acreages.Count > 0 AndAlso ((String.IsNullOrEmpty(l.Acreages(0).Section) = False AndAlso l.Acreages(0).Section <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Twp) = False AndAlso l.Acreages(0).Twp <> "0") OrElse (String.IsNullOrEmpty(l.Acreages(0).Range) = False AndAlso l.Acreages(0).Range <> "0")) Then
                                    Dim strDisplayAddress As String = If(String.IsNullOrEmpty(l.Acreages(0).Section) = False, l.Acreages(0).Section, "S") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Twp) = False, l.Acreages(0).Twp, "T") & "/" & If(String.IsNullOrEmpty(l.Acreages(0).Range) = False, l.Acreages(0).Range, "R")
                                    Dim cityStateZip As String = QQHelper.appendText(l.Address.City, l.Address.State, ", ")
                                    cityStateZip = QQHelper.appendText(cityStateZip, l.Address.Zip, " ")
                                    strDisplayAddress = QQHelper.appendText(strDisplayAddress, cityStateZip, vbCrLf)
                                    drLocation.Item("LocationDescription") = strDisplayAddress
                                Else
                                    If l.Address.DisplayAddress.NotEqualsAny("", "IN", "00000", "00000-0000") Then
                                        drLocation.Item("LocationDescription") = l.Address.DisplayAddress
                                    Else
                                        drLocation.Item("LocationDescription") = "Location " & locationNum.ToString
                                    End If
                                End If
                                drLocation.Item("LocationNumber") = locationNum.ToString 'added 1/3/2014
                                dtLocations.Rows.Add(drLocation)

                                'added 7/27/2015 for Farm (IM and RvWatercraft); may not need
                                If l.InlandMarines.IsLoaded() Then
                                    hasInlandMarine = True
                                    locImCount = l.InlandMarines.Count
                                End If
                                If l.RvWatercrafts.IsLoaded() Then
                                    hasRvWatercraft = True
                                    locRvCount = locRvCount + l.RvWatercrafts.Count
                                End If
                            Next
                            Me.rptLocations.DataSource = dtLocations
                            Me.rptLocations.DataBind()

                            Me.lblNumberOfInlandMarinesAndRvWatercrafts.Text = (locImCount + locRvCount).ToString
                            Me.lblNumberOfInlandMarinesAndRvWatercrafts.ToolTip = ""
                            If locImCount > 0 Then
                                Dim imText As String = $"{locImCount} Inland Marine{If(locImCount > 1, "s", "")}"
                                Me.lblNumberOfInlandMarinesAndRvWatercrafts.ToolTip = QQHelper.appendText(Me.lblNumberOfInlandMarinesAndRvWatercrafts.ToolTip, imText, "; ")
                            End If
                            If locRvCount > 0 Then
                                Dim rvText As String = $"{locRvCount} Rv/Watercraft{If(locRvCount > 1, "s", "")}"
                                Me.lblNumberOfInlandMarinesAndRvWatercrafts.ToolTip = QQHelper.appendText(Me.lblNumberOfInlandMarinesAndRvWatercrafts.ToolTip, rvText, "; ")
                            End If

                            If .LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse .LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                                With .Locations(0) 'only uses location #1
                                    hasResidences = True 'may automatically set this to True when there's at least 1 location since Residence control has other fields besides the Location Address
                                    Dim dtResidences As New DataTable
                                    dtResidences.Columns.AddStrings("ResidenceDescription", "LocationNumber", "ProtectionClass", "FormType", "FormTypeId", "ProtectionClassSystemGenerated", "ProtectionClassId", "ProtectionClassSystemGeneratedId")

                                    Dim drResidence As DataRow
                                    drResidence = dtResidences.NewRow

                                    If .Address?.DisplayAddress IsNot Nothing AndAlso .Address.DisplayAddress.NotEqualsAny("", "IN", "00000", "00000-0000") Then
                                        drResidence.Item("ResidenceDescription") = .Address.DisplayAddress
                                    Else
                                        drResidence.Item("ResidenceDescription") = "Location 1"
                                    End If
                                    drResidence.Item("LocationNumber") = "1"
                                    drResidence.Item("ProtectionClass") = .ProtectionClass 'added 10/1/2014
                                    'Updated 11/30/17 for HOM Upgrade MLW
                                    If ((.FormTypeId.EqualsAny("22", "25")) AndAlso .StructureTypeId = "2") Then
                                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute) 'Matt A 10-27-15
                                        Dim a1 As New QuickQuoteStaticDataAttribute
                                        a1.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                                        a1.nvp_value = 2
                                        optionAttributes.Add(a1)
                                        drResidence.Item("FormType") = QQHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, .FormTypeId, Me.Quote.LobType)
                                    Else
                                        drResidence.Item("FormType") = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, .FormTypeId, Me.Quote.LobType) 'added 10/1/2014; probably doesn't need lobType optional param
                                    End If
                                    drResidence.Item("FormTypeId") = .FormTypeId 'added 10/2/2014
                                    drResidence.Item("ProtectionClassSystemGenerated") = .ProtectionClassSystemGenerated
                                    drResidence.Item("ProtectionClassId") = .ProtectionClassId
                                    drResidence.Item("ProtectionClassSystemGeneratedId") = .ProtectionClassSystemGeneratedId
                                    dtResidences.Rows.Add(drResidence)

                                    Me.rptResidences.DataSource = dtResidences
                                    Me.rptResidences.DataBind()
                                    'End If
                                End With
                            End If

                    End Select
                End If

                'added 1/27/2014
                If .LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                    If Me.SubQuoteFirst.HasBusinessMasterEnhancement = True Then
                        hasCoverages = True
                    End If
                    If Me.SubQuoteFirst.SelectMarketCredit = True Then
                        hasCoverages = True
                    End If
                    If Me.SubQuoteFirst.AutoHome = True Then
                        hasCoverages = True
                    End If
                    If Me.SubQuoteFirst.EmployeeDiscount = True Then
                        hasCoverages = True
                    End If
                    If Me.SubQuoteFirst.FacultativeReinsurance = True Then
                        hasCoverages = True
                    End If

                    If hasCoverages = False Then
                        If .Vehicles IsNot Nothing AndAlso .Vehicles.Count > 0 Then
                            For Each v As QuickQuoteVehicle In .Vehicles
                                With v
                                    If .Liability_UM_UIM_LimitId <> "" OrElse .MedicalPaymentsLimitId <> "" OrElse .ComprehensiveDeductibleLimitId <> "" OrElse .CollisionDeductibleLimitId <> "" OrElse .TowingAndLaborDeductibleLimitId <> "" OrElse .UninsuredMotoristLiabilityLimitId <> "" OrElse .BodilyInjuryLiabilityLimitId <> "" OrElse .PropertyDamageLimitId <> "" OrElse .UninsuredCombinedSingleLimitId <> "" OrElse .UninsuredBodilyInjuryLimitId <> "" OrElse .UninsuredMotoristPropertyDamageLimitId <> "" OrElse .UninsuredMotoristPropertyDamageDeductibleLimitId <> "" OrElse .HasPollutionLiabilityBroadenedCoverage = True OrElse .TransportationExpenseLimitId <> "" OrElse .HasAutoLoanOrLease = True OrElse .TapesAndRecordsLimitId <> "" OrElse .SoundEquipmentLimit <> "" OrElse .ElectronicEquipmentLimit <> "" OrElse .TripInterruptionLimitId <> "" OrElse .ComprehensiveCoverageOnly = True Then
                                        hasCoverages = True
                                    End If
                                End With

                                If hasCoverages = True Then
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                ElseIf .LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse .LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then 'added 6/12/2014
                    If Me.SubQuoteFirst.PersonalLiabilityLimitId <> "" OrElse Me.SubQuoteFirst.MedicalPaymentsLimitid <> "" Then 'might also check for numeric and maybe greater than 0
                        'policy level covs
                        hasCoverages = True
                    Else
                        If .Locations.IsLoaded() Then
                            For Each l As QuickQuoteLocation In .Locations
                                With l
                                    If .DeductibleLimitId <> "" OrElse .WindHailDeductibleLimitId <> "" OrElse .A_Dwelling_Limit <> "" OrElse .B_OtherStructures_Limit <> "" OrElse .C_PersonalProperty_Limit <> "" OrElse .C_PersonalProperty_Limit <> "" OrElse .TheftDeductibleLimitId <> "" Then
                                        'location covs
                                        hasCoverages = True
                                    ElseIf .MultiPolicyDiscount = True OrElse .MatureHomeownerDiscount = True OrElse .FireSmokeAlarm_LocalAlarmSystem = True OrElse .NewHomeDiscount = True OrElse .FireSmokeAlarm_CentralStationAlarmSystem = True OrElse .SelectMarketCredit = True OrElse .FireSmokeAlarm_SmokeAlarm = True OrElse .BurglarAlarm_LocalAlarmSystem = True OrElse .SprinklerSystem_AllExcept = True OrElse .BurglarAlarm_CentralStationAlarmSystem = True OrElse .SprinklerSystem_AllIncluding = True OrElse .TrampolineSurcharge = True OrElse .WoodOrFuelBurningApplianceSurcharge = True OrElse .SwimmingPoolHotTubSurcharge = True Then
                                        'credits/surcharges
                                        hasCoverages = True
                                    End If
                                End With

                                If hasCoverages = True Then
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                ElseIf .LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then 'added 9/15/2017
                    'If .Locations IsNot Nothing AndAlso .Locations.IsLoaded() AndAlso QQHelper.IsPositiveIntegerString(.Locations(0).Classifications(0).ClassificationTypeId) = True Then
                    'updated 9/21/2018
                    If .Locations IsNot Nothing AndAlso .Locations.IsLoaded() AndAlso .Locations(0).Classifications IsNot Nothing AndAlso .Locations(0).Classifications.Count > 0 AndAlso QQHelper.IsPositiveIntegerString(.Locations(0).Classifications(0).ClassificationTypeId) = True Then
                        hasCoverages = True
                    End If
                ElseIf .LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then ' 01/06/2021 CAH
                    If SubQuoteFirst.UnderlyingPolicies?.IsLoaded() Then
                        hasCoverages = True
                    End If
                End If

                If (Me.SubQuoteFirst.PolicyUnderwritings.IsLoaded()) OrElse (.Locations IsNot Nothing AndAlso .Locations.IsLoaded() AndAlso .Locations(0).PolicyUnderwritings IsNot Nothing AndAlso .Locations(0).PolicyUnderwritings.Any()) Then
                    hasUnderwritingQuestions = True
                End If
            End With

            LoadDrivers(QuoteOrRatedQuoteType.Quote, hasDrivers) 'added 6/28/2019 w/ removal of logic above
            LoadVehicles(QuoteOrRatedQuoteType.Quote, hasVehicles)
            LoadCreditReports(QuoteOrRatedQuoteType.Quote, hasCreditReports, Me.Quote, Me.GoverningStateQuote)
            LoadMvrReports(QuoteOrRatedQuoteType.Quote, hasMvrReports, Me.GoverningStateQuote)
            LoadClueReports(QuoteOrRatedQuoteType.Quote, hasClueReports) 'added 9/29/2014; only called from LoadTreeView

            If hasPHs = True Then
                Me.pnlPolicyholders.Visible = True
                Me.MainPolicyholderSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                Me.ph_checkMarkArea.Visible = True 'added 2/20/2014
                Me.ph_xImageArea.Visible = False
            Else
                Me.pnlPolicyholders.Visible = False
                Me.MainPolicyholderSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.ph_checkMarkArea.Visible = False 'added 2/20/2014
                Me.ph_xImageArea.Visible = True
            End If
            'If hasDrivers = True Then 'removed 6/28/2019; now being done from LoadDrivers method call above
            '    Me.pnlDrivers.Visible = True
            '    Me.MainDriverSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            '    Me.d_checkMarkArea.Visible = True 'added 2/20/2014
            '    Me.d_xImageArea.Visible = False
            'Else
            '    Me.pnlDrivers.Visible = False
            '    Me.MainDriverSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
            '    Me.d_checkMarkArea.Visible = False 'added 2/20/2014
            '    Me.d_xImageArea.Visible = True
            'End If

            If hasLocations = True Then
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        ' COMMERCIAL PACKAGE
                        ' CPP-CPR
                        Me.pnlCPPCPRLocations.Visible = True
                        Me.CPPCPRLocationsSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                        Me.CPPCPRLocations_checkMarkArea.Visible = False
                        Me.CPPCPRLocations_xImageArea.Visible = False

                        ' CPP-CGL
                        Me.pnlCPPCGLLocations.Visible = True
                        Me.CPPCGLLocationsSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                        Me.CPPCGLLocations_checkMarkArea.Visible = False
                        Me.CPPCGLLocations_xImageArea.Visible = False

                        Exit Select
                    Case Else
                        ' ALL OTHER LINES
                        Me.pnlLocations.Visible = True
                        Me.MainLocationSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                        Me.l_checkMarkArea.Visible = True 'added 2/20/2014
                        Me.l_xImageArea.Visible = False
                        Exit Select
                End Select
            Else
                Select Case Quote.LobType
                    ' MGB 2/21/18 - Added CASE to handle CPP locations
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        ' CPP-CPR
                        Me.pnlCPPCPRLocations.Visible = False
                        Me.CPPCPRLocationsSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                        Me.CPPCPRLocations_checkMarkArea.Visible = False
                        Me.CPPCPRLocations_xImageArea.Visible = False

                        ' CPP-CGL
                        Me.pnlCPPCGLLocations.Visible = False
                        Me.CPPCGLLocationsSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                        Me.CPPCGLLocations_checkMarkArea.Visible = False
                        Me.CPPCGLLocations_xImageArea.Visible = False

                        Exit Select
                    Case Else
                        Me.pnlLocations.Visible = False
                        Me.MainLocationSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                        Me.l_checkMarkArea.Visible = False 'added 2/20/2014
                        Me.l_xImageArea.Visible = True
                        Exit Select
                End Select
            End If
            If hasCoverages = True Then 'added 1/27/2014; has separate span for CheckMark image
                ' MGB 2/21/18 Added CASE for CPP Coverages
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        ' CPP-CPR
                        Me.CPPCPRCoverages_xImageArea.Visible = False
                        Me.CPPCPRCoverages_checkMarkArea.Visible = False

                        ' CPP-CGL
                        Me.CPPCGLCoverages_xImageArea.Visible = False
                        Me.CPPCGLCoverages_checkMarkArea.Visible = False
                        Exit Select
                    Case Else
                        Me.pnlCoverages.Visible = True
                        If hasTreeCoverages = True Then
                            Me.MainCoverageSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                        Else
                            Me.MainCoverageSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                            Me.pnlCoverages.Visible = False 'added 3/4/2014; doesn't need to show if it only has coverages that aren't in the tree
                        End If
                        Me.c_xImageArea.Visible = False
                        Me.c_checkMarkArea.Visible = True
                        Exit Select
                End Select
            Else
                Select Case Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Me.CPPCPRCoverages_xImageArea.Visible = False
                        Me.CPPCPRCoverages_checkMarkArea.Visible = False
                        Me.CPPCGLCoverages_xImageArea.Visible = False
                        Me.CPPCGLCoverages_checkMarkArea.Visible = False
                        Exit Select
                    Case Else
                        Me.pnlCoverages.Visible = False
                        Me.MainCoverageSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                        Me.c_xImageArea.Visible = True
                        Me.c_checkMarkArea.Visible = False
                        Exit Select
                End Select
            End If
            'added 3/18/2014
            Me.MainUnderwritingQuestionSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
            If hasUnderwritingQuestions = True Then
                Me.u_xImageArea.Visible = False
                Me.u_checkMarkArea.Visible = True
            Else
                Me.u_xImageArea.Visible = True
                Me.u_checkMarkArea.Visible = False
            End If

            Me.MainApplicationSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
            Me.a_xImageArea.Visible = False
            Me.a_checkMarkArea.Visible = False

            'added 5/19/2014 to initialize QuoteSummary and ApplicationSummary sections if they haven't been set yet... will maintain whatever's done in LoadRatedQuoteIntoTree method on subsequent Saves (calls to LoadTreeView)
            If Me.MainQuoteSummarySectionSubLists_expandCollapseImageArea.Style.Item("visibility") Is Nothing Then
                Me.MainQuoteSummarySectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.q_xImageArea.Visible = False
                Me.q_checkMarkArea.Visible = False
            End If
            If Me.MainApplicationSummarySectionSubLists_expandCollapseImageArea.Style.Item("visibility") Is Nothing Then
                Me.MainApplicationSummarySectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.as_xImageArea.Visible = False
                Me.as_checkMarkArea.Visible = False
            End If


            'added 6/12/2014
            If hasResidences = True Then
                Me.pnlResidences.Visible = True
                Me.MainResidenceSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                Me.r_xImageArea.Visible = False
                Me.r_checkMarkArea.Visible = True
            Else
                Me.pnlResidences.Visible = False
                Me.MainResidenceSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.r_xImageArea.Visible = True
                Me.r_checkMarkArea.Visible = False
            End If

            Me.hdnExpandOrCollapseAllFlag.Value = "expand" 'updated markup 3/12/2014 to give everything initial css displays of none... so both images don't show whenever this isn't called
            Me.hdnDeselectAllListItemsFlag.Value = "yes" 'added 1/22/2014

            'Me.hdnTreeFileUploadCount.Value = IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(Me.Quote.AgencyId, Me.QuoteId).Count.ToString()
            'updated 6/27/2019
            Dim quoteIdOrPolicyIdAndImageNum As String = ""
            If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.EndorsementPolicyIdAndImageNum
                ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.ReadOnlyPolicyIdAndImageNum
                ElseIf QQHelper.IsPositiveIntegerString(Me.Quote.PolicyId) = True Then
                    quoteIdOrPolicyIdAndImageNum = CInt(Me.Quote.PolicyId).ToString & "|" & QQHelper.IntegerForString(Me.Quote.PolicyImageNum).ToString
                End If
            Else
                If String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.QuoteId
                ElseIf QQHelper.IsPositiveIntegerString(Me.Quote.Database_QuoteId) = True Then
                    quoteIdOrPolicyIdAndImageNum = CInt(Me.Quote.Database_QuoteId).ToString
                End If
            End If
            If Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Me.hdnTreeFileUploadCount.Value = IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(Me.Quote.AgencyId, quoteIdOrPolicyIdAndImageNum).Count.ToString()
            End If
        Else
            'hide everything
            Me.pnlTreeViewError.Visible = True
            Me.hdnExpandOrCollapseAllFlag.Value = ""
            Me.hdnDeselectAllListItemsFlag.Value = "" 'added 1/22/2014
        End If

        'added 6/26/2014
        SetQuoteNumberLabelAndOptionallyShowHeader()

    End Sub

    Private Sub LoadRatedQuoteIntoTree()
        Me.SwitchToRatedQuote()

        Dim summaryListItem As HtmlGenericControl = Nothing '5/20/2014 note: not being used in this method; being set to visible in LoadTreeView method
        Dim summaryExpandCollapseImageArea As HtmlGenericControl = Nothing
        Dim summaryXImageArea As HtmlGenericControl = Nothing
        Dim summaryCheckMarkArea As HtmlGenericControl = Nothing
        Dim summaryTotalPremiumSection As HtmlGenericControl = Nothing
        Dim summaryTotalPremiumLabel As Label = Nothing
        Dim summaryPrefix As String = ""
        Dim summaryItemsRepeater As Repeater = Nothing
        Dim summaryItemsPanel As Panel = Nothing
        Dim summarySuccessfullyRatedFlag As HtmlInputHidden = Nothing 'added 6/3/2014

        If Me.IsOnAppPage Then
            summaryListItem = Me.liApplicationSummary '5/20/2014 note: not being used in this method; being set to visible in LoadTreeView method
            summaryExpandCollapseImageArea = Me.MainApplicationSummarySectionSubLists_expandCollapseImageArea
            summaryXImageArea = Me.as_xImageArea
            summaryCheckMarkArea = Me.as_checkMarkArea
            summaryTotalPremiumSection = Me.TotalPremiumSection_Application
            summaryTotalPremiumLabel = Me.lblTotalPremium_Application
            summaryPrefix = "Application"
            summaryItemsRepeater = Me.rptApplicationSummaryItems
            summaryItemsPanel = Me.pnlApplicationSummaryItems
            'IsApplicationSummarySectionEnabled
            summarySuccessfullyRatedFlag = Me.hdnApplicationSummarySection_SuccessfullyRatedFlag 'added 6/3/2014
        Else
            summaryListItem = Me.liQuoteSummary '5/20/2014 note: not being used in this method; being set to visible in LoadTreeView method
            summaryExpandCollapseImageArea = Me.MainQuoteSummarySectionSubLists_expandCollapseImageArea
            summaryXImageArea = Me.q_xImageArea
            summaryCheckMarkArea = Me.q_checkMarkArea
            summaryTotalPremiumSection = Me.TotalPremiumSection
            summaryTotalPremiumLabel = Me.lblTotalPremium
            summaryPrefix = "Quote"
            summaryItemsRepeater = Me.rptQuoteSummaryItems
            summaryItemsPanel = Me.pnlQuoteSummaryItems
            'IsQuoteSummarySectionEnabled
            summarySuccessfullyRatedFlag = Me.hdnQuoteSummarySection_SuccessfullyRatedFlag 'added 6/3/2014
        End If

        '5/19/2014 - added default logic... since Summary tab will always be visible now
        summaryExpandCollapseImageArea.Style.Add("visibility", "hidden")
        summaryItemsPanel.Visible = False
        summaryXImageArea.Visible = False
        summaryCheckMarkArea.Visible = False
        summaryTotalPremiumSection.Visible = False
        Me.VehiclesPremiumSection.Visible = False
        Me.LocationsPremiumSection.Visible = False
        summarySuccessfullyRatedFlag.Value = GetTrueOrFalseText(False) 'added 6/3/2014

        If Me.pnlTreeView.Visible = True AndAlso Me.Quote IsNot Nothing Then
            'only load if treeview is currently loaded and there is rated info

            'added 2/22/2019; may only be needed in normal LoadTreeView method as it shouldn't change
            UpdateTreeForViewMode(Me.Quote)

            'added 6/26/2014 to set Quote Number so it always present after Rate (case was identified where it didn't show up if a quote was copied and rated immediately... since it was only being set by QuoteObject property, which isn't necessarily reset after Rate)
            'If Me.hdnQuoteNumber.Value = "" AndAlso Me.Quote.QuoteNumber <> "" Then
            '    Me.hdnQuoteNumber.Value = Me.Quote.QuoteNumber
            '    'Me.lblQuoteDescription.Text = qqHelper.appendText(Me.hdnQuoteNumber.Value, Me.hdnOriginalQuoteDescription.Value, " - ")
            '    'updated 6/26/2014 to use common method
            '    SetQuoteDescriptionLabel()
            'End If
            'updated 2/15/2019 to also work for Endorsements and ReadOnly images
            If Me.hdnQuoteNumber.Value = "" AndAlso Me.Quote.PolicyNumber <> "" Then
                Me.hdnQuoteNumber.Value = Me.Quote.PolicyNumber
                SetQuoteDescriptionLabel()
            End If

            'added 12/19/2022
            UpdateTreeForRouteToUW()

            Me.DetermineIsIRPMSectionEnabled()

            If Me.IsOnAppPage Then
                IsApplicationSummarySectionEnabled = True 'might remove tooltip so user knows it's available
            Else
                IsQuoteSummarySectionEnabled = True 'might remove tooltip so user knows it's available
            End If

            Dim hasDiscounts As Boolean = False
            Dim RatingDiscounts As New List(Of String)
            Dim hasDefensiveDriver As Boolean = False
            Dim hasDriverTraining As Boolean = False
            Dim hasGoodStudent As Boolean = False
            Dim hasMatureDriver As Boolean = False
            Dim hasMultiPolicy As Boolean = False
            Dim hasMultiVehicle As Boolean = False
            Dim hasSelectMarket As Boolean = False
            Dim hasAntiTheft As Boolean = False
            Dim hasPassiveRestraint As Boolean = False
            Dim hasAdvancedQuoteDiscount As Boolean = False
            Dim hasPayPlanDiscount As Boolean = False 'Added 6/18/2019 for Bug 31002 MLW

            Dim hasSurcharges As Boolean = False
            Dim RatingSurcharges As New List(Of String)
            Dim hasAccident As Boolean = False
            Dim hasAccident_surchargeable As Boolean = False 'added 7/2/2014
            Dim hasViolation As Boolean = False
            Dim hasViolation_surchargeable As Boolean = False 'added 7/2/2014
            Dim hasOutOfStateVehicle As Boolean = False
            Dim hasInexperiencedOperator As Boolean = False
            Dim totalDiscountCount As Integer = 0 'added 7/2/2014
            Dim totalSurchargeCount As Integer = 0 'added 7/2/2014
            'added 10/29/2014 for HOM; could use new variables instead of old ones used for PPA (would have to write 'Accident' instead of 'Loss History' or 'Claim' to tree)
            Dim hasLossHistory As Boolean = False
            Dim hasLossHistory_surchargeable As Boolean = False
            'added 11/6/2014 for HOM
            Dim hasBurglarAlarm As Boolean = False
            Dim hasSmokeAlarm As Boolean = False
            Dim hasMatureHomeowner As Boolean = False
            'Dim hasMultiPolicy As Boolean = False
            Dim hasNewHome As Boolean = False
            'Dim hasSelectMarket As Boolean = False
            Dim hasSprinkler As Boolean = False
            Dim hasSwimmingPool As Boolean = False
            Dim hasTrampoline As Boolean = False
            Dim hasWoodBurning As Boolean = False
            'added 1/2/2015 for HOM
            Dim hasYouthfulOperator As Boolean = False
            Dim hasMobileHomeTieDown As Boolean = False
            Dim hasWatercraftAge As Boolean = False
            Dim hasAcvRoof As Boolean = False 'added 1/5/2015 for HOM
            Dim hasHobbyFarm As Boolean = False 'added 8/11/2015 for Farm
            Dim hasheatedBuilding As Boolean = False 'added 8/11/2015 for Farm
            '1/5/2015 note: also HOM credits for Customer Tier and Home Deductible, but no logic on when to show... may just need percentages found in Coverage Calc for viewable/printable summaries and tree doesn't matter
            If Me.Quote.Success = True Then
                'rating succeeded
                summaryTotalPremiumSection.Visible = True
                If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then  ' CAH 10/25/2017 - did as comment below suggested 
                    summaryTotalPremiumLabel.Text = Me.Quote.TotalQuotedPremium 'might use different property for WCP (like we did on comm QuoteSummary and Proposal)
                Else
                    'Dim premiumsToCombine As New List(Of String) From {Me.Quote.TotalQuotedPremium, Me.Quote.SecondInjuryFundQuotedPremium}
                    'summaryTotalPremiumLabel.Text = IFM.VR.Common.Helpers.QuickQuoteObjectHelper.SumMoneyStrings(premiumsToCombine)
                    'updated 11/20/2018 for multiState; Dec_WC_TotalPremiumDue combines TotalQuotedPremium and SecondInjuryFundQuotedPremium... need to SUM state parts since SecondInjuryFundQuotedPremium will only show up there
                    summaryTotalPremiumLabel.Text = QQHelper.GetSumForPropertyValues(Me.SubQuotes, Function() Quote.Dec_WC_TotalPremiumDue)
                End If
                'summaryTotalPremiumLabel.Text =  Me.Quote.TotalQuotedPremium 'might use different property for WCP (like we did on comm QuoteSummary and Proposal)
                summarySuccessfullyRatedFlag.Value = GetTrueOrFalseText(True) 'added 6/3/2014

                'IsApplicationSectionEnabled = True 'might remove tooltip so user knows it's available; removed 5/19/2014
                'IsUnderwritingQuestionSectionEnabled = True 'might remove tooltip so user knows it's available; removed 5/19/2014

                If Me.Quote.LobType <> Nothing AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            Me.VehiclesPremiumSection.Visible = True
                            'Me.lblVehiclesPremium.Text = Me.Quote.VehiclesTotal_PremiumFullTerm 'new property added 4/2/2014
                            'updated 11/20/2018 for MultiState; note: Vehicles are moved to top level after VehiclesTotal_PremiumFullTerm would be calc'd so need to SUM state parts
                            Me.lblVehiclesPremium.Text = QQHelper.GetSumForPropertyValues(Me.SubQuotes, Function() Me.Quote.VehiclesTotal_PremiumFullTerm, maintainFormattingOrDefaultValue:=True)

                            'added 4/3/2014
                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                                If Me.GoverningStateQuote.AutoHome = True Then
                                    hasMultiPolicy = True
                                    hasDiscounts = True
                                    RatingDiscounts.Add(If(VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote), "Auto/Home", "Multi Policy")) 'change Text for Parachute
                                    totalDiscountCount += 1 'added 7/2/2014
                                End If
                                If Me.GoverningStateQuote.MultiLineDiscount.TryToGetInt32() > 4 Then ' Parachute
                                    hasMultiPolicy = True
                                    hasDiscounts = True
                                    RatingDiscounts.Add("Multi Line")
                                    totalDiscountCount += 1
                                End If
                                If Me.GoverningStateQuote.SelectMarketCredit = True Then
                                    hasSelectMarket = True
                                    hasDiscounts = True
                                    RatingDiscounts.Add("Select Market")
                                    totalDiscountCount += 1 'added 7/2/2014
                                End If
                                ' New for Advanced Quote Discount Bug 30754 MGB 4/1/2019
                                If GoverningStateQuote.HasAdvancedQuoteDiscount Then
                                    hasAdvancedQuoteDiscount = True
                                    hasDiscounts = True
                                    RatingDiscounts.Add("Advanced Quote Discount Applied")
                                    totalDiscountCount += 1
                                End If

                                'Added 6/18/2019 Pay Plan Discount per bug 31002 MLW
                                Dim creditList = VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(Quote, False)
                                If creditList.Count <= 0 Then
                                    hasPayPlanDiscount = False
                                Else
                                    For inx As Integer = 0 To creditList.Count - 1
                                        If creditList(inx).Key = "Pay Plan Discount" AndAlso creditList(inx).Value < 1 Then
                                            hasPayPlanDiscount = True
                                            hasDiscounts = True
                                            RatingDiscounts.Add("Pay Plan")
                                            totalDiscountCount += 1
                                            Exit For
                                        Else
                                            hasPayPlanDiscount = False
                                        End If
                                    Next
                                End If

                                'updated 10/29/2014 to use new method (will pick up everything at policy and driver levels); removed below logic from Drivers section
                                Dim accCount As Integer = 0
                                Dim accCount_surchargeable As Integer = 0
                                EvaluateLossHistoriesForRatedQuote(QuoteOrRatedQuoteType.RatedQuote, accCount, accCount_surchargeable, Me.Quote, Me.GoverningStateQuote)
                                If accCount > 0 Then
                                    hasAccident = True
                                End If
                                If accCount_surchargeable > 0 Then
                                    hasAccident_surchargeable = True
                                    hasSurcharges = True
                                    RatingSurcharges.Add("Accident" & If(accCount_surchargeable > 1, "s", "") & " (" & accCount_surchargeable.ToString & ")")
                                    totalSurchargeCount += accCount_surchargeable
                                End If

                                If Me.GoverningStateQuote.Drivers.IsLoaded() Then
                                    'added 5/20/2014 for credit reports; 9/29/2014 - moved logic to LoadCreditReports method
                                    Dim driverNum As Integer = 0

                                    '7/1/2014 - updated to get counts for each type of discount/surcharge
                                    Dim defDriverCount As Integer = 0
                                    Dim driverTrainCount As Integer = 0
                                    Dim goodStudentCount As Integer = 0
                                    Dim matDriverCount As Integer = 0
                                    'Dim accCount As Integer = 0 'removed 10/29/2014; logic now being done above for entire quote (w/ new method call)
                                    'Dim accCount_surchargeable As Integer = 0 'removed 10/29/2014; logic now being done above for entire quote (w/ new method call)
                                    Dim vioCount As Integer = 0
                                    Dim vioCount_surchargeable As Integer = 0
                                    'Dim vioCount_flaggedAsSurcharged_3years As Integer = 0 'added 7/15/2014; removed 7/16/2014... probably won't use Surcharge flag anymore
                                    Dim vioCount_surchargeable_convertedFromMultipleMinors As Integer = 0 'added 7/16/2014
                                    Dim inexpOperCount As Integer = 0

                                    'added 7/16/2014
                                    Dim AccViol_ChildRestraint_TypeId As Integer = 47 'Failure to use Child Restraint System
                                    Dim AccViol_FinancialResponsibility_TypeId As Integer = 35 'Financial Responsibility; added 7/28/2014
                                    Dim AccViol_NoSurchargeList As New List(Of Integer)
                                    AccViol_NoSurchargeList.Add(5) 'All other Non-moving violations
                                    AccViol_NoSurchargeList.Add(6) 'License Restriction
                                    AccViol_NoSurchargeList.Add(42) 'Not at Fault Accident
                                    AccViol_NoSurchargeList.Add(35) 'Financial Responsibility (Unacceptable); may not include in this list since it would already be lumped in w/ Majors
                                    AccViol_NoSurchargeList.Add(36) 'MVR Record Clear
                                    AccViol_NoSurchargeList.Add(37) 'MVR Record Not Found
                                    AccViol_NoSurchargeList.Add(38) 'Unassigned MVR Code
                                    AccViol_NoSurchargeList.Add(44) 'No valid license
                                    AccViol_NoSurchargeList.Add(45) 'License violation
                                    AccViol_NoSurchargeList.Add(46) 'Violation on a probationary license
                                    AccViol_NoSurchargeList.Add(48) 'Texting while Driving
                                    AccViol_NoSurchargeList.Add(49) 'Use of Telecomm Device while operating
                                    AccViol_NoSurchargeList.Add(50) 'Carrying unsecured passengers in open area of vehicle

                                    For Each d As QuickQuoteDriver In Me.GoverningStateQuote.Drivers
                                        driverNum += 1 'added 5/20/2014 for credit reports

                                        If d.DefDriverDate <> "" AndAlso IsDate(d.DefDriverDate) = True AndAlso CDate(d.DefDriverDate) > CDate("1/1/1900") Then
                                            defDriverCount += 1
                                        End If
                                        If d.GoodStudent = True Then
                                            goodStudentCount += 1
                                        End If
                                        If d.AccPreventionCourse <> "" AndAlso IsDate(d.AccPreventionCourse) = True AndAlso CDate(d.AccPreventionCourse) > CDate("1/1/1900") Then
                                            matDriverCount += 1
                                        End If
                                        If d.AccidentViolations IsNot Nothing AndAlso d.AccidentViolations.Count > 0 Then 'may need to look at type to verify it's a violation
                                            vioCount += d.AccidentViolations.Count 'count also increment one on each loop below

                                            Dim pre_20140324_includedMinorCount As Integer = 0
                                            Dim post_20140324_PassiveRestraintCount As Integer = 0 'added 7/16/2014
                                            For Each av As QuickQuoteAccidentViolation In d.AccidentViolations
                                                'checking date 1st to make sure it's within 3 years
                                                If Me.Quote.EffectiveDate <> "" AndAlso IsDate(Me.Quote.EffectiveDate) = True AndAlso av.AvDate <> "" AndAlso IsDate(av.AvDate) = True AndAlso CDate(av.AvDate) >= DateAdd(DateInterval.Year, -3, CDate(Me.Quote.EffectiveDate)) Then
                                                    Dim avCat As String = av.AccidentsViolationsCategory 'added 7/16/2014
                                                    If UCase(avCat).Contains("MAJOR") = True OrElse UCase(avCat).Contains("UNACCEPTABLE") = True Then
                                                        If av.AccidentsViolationsTypeId <> "" AndAlso IsNumeric(av.AccidentsViolationsTypeId) = True AndAlso CInt(av.AccidentsViolationsTypeId) = AccViol_FinancialResponsibility_TypeId Then
                                                            If CDate(av.AvDate) >= CDate("3/24/2014") Then
                                                                'post 3/24/2014 logic
                                                            Else
                                                                'pre 3/24/2014 logic
                                                                vioCount_surchargeable += 1
                                                            End If
                                                        Else
                                                            vioCount_surchargeable += 1
                                                        End If
                                                    Else
                                                        'see AccViol_NoSurchargeList above
                                                        If av.AccidentsViolationsTypeId <> "" AndAlso IsNumeric(av.AccidentsViolationsTypeId) = True AndAlso AccViol_NoSurchargeList.Contains(CInt(av.AccidentsViolationsTypeId)) = False Then
                                                            'not in 'No Surcharge' list
                                                            If CDate(av.AvDate) >= CDate("3/24/2014") Then
                                                                'post 3/24/2014 logic
                                                                If CInt(av.AccidentsViolationsTypeId) <> AccViol_ChildRestraint_TypeId Then
                                                                    vioCount_surchargeable += 1
                                                                Else
                                                                    post_20140324_PassiveRestraintCount += 1
                                                                    '2 = 1 surcharge so only count even #s
                                                                    If post_20140324_PassiveRestraintCount Mod 2 = 0 Then 'will return remainder of counter divided by 2
                                                                        'even num
                                                                        vioCount_surchargeable += 1
                                                                        vioCount_surchargeable_convertedFromMultipleMinors += 1
                                                                    Else
                                                                        'odd num
                                                                    End If
                                                                End If
                                                            Else
                                                                'pre 3/24/2014 logic
                                                                If CInt(av.AccidentsViolationsTypeId) <> AccViol_ChildRestraint_TypeId Then
                                                                    pre_20140324_includedMinorCount += 1
                                                                    '2 = 1 surcharge so only count even #s
                                                                    If pre_20140324_includedMinorCount Mod 2 = 0 Then 'will return remainder of counter divided by 2
                                                                        'even num
                                                                        vioCount_surchargeable += 1
                                                                        vioCount_surchargeable_convertedFromMultipleMinors += 1
                                                                    Else
                                                                        'odd num
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Next
                                        End If
                                        If d.FirstPermit = True Then 'might be wrong property... maybe look at age
                                            inexpOperCount += 1
                                        End If

                                    Next
                                    '7/1/2014 - updated to get counts for each type of discount/surcharge
                                    If defDriverCount > 0 Then
                                        hasDefensiveDriver = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Defensive Driver (" & defDriverCount.ToString & ")") 'updated 7/2/2014 to show count in parenthesis
                                        totalDiscountCount += defDriverCount 'added 7/2/2014
                                    End If
                                    If driverTrainCount > 0 Then
                                        hasDriverTraining = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Driver Training (" & driverTrainCount.ToString & ")") 'updated 7/2/2014 to show count in parenthesis
                                        totalDiscountCount += driverTrainCount 'added 7/2/2014
                                    End If
                                    If goodStudentCount > 0 Then
                                        hasGoodStudent = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Good Student (" & goodStudentCount.ToString & ")") 'updated 7/2/2014 to show count in parenthesis
                                        totalDiscountCount += goodStudentCount 'added 7/2/2014
                                    End If
                                    If matDriverCount > 0 Then
                                        hasMatureDriver = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Mature Driver (" & matDriverCount.ToString & ")") 'updated 7/2/2014 to show count in parenthesis
                                        totalDiscountCount += matDriverCount 'added 7/2/2014
                                    End If
                                    If vioCount > 0 Then
                                        hasViolation = True 'updated 7/2/2014 to set old flag
                                    End If
                                    If vioCount_surchargeable > 0 Then
                                        hasViolation_surchargeable = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add("Violation" & If(vioCount_surchargeable > 1, "s", "") & " (" & vioCount_surchargeable.ToString & ")") 'updated 7/2/2014 to show count in parenthesis and add 's' if more than 1
                                        totalSurchargeCount += vioCount_surchargeable 'added 7/2/2014
                                    End If
                                    If inexpOperCount > 0 Then
                                        hasInexperiencedOperator = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add("Inexperienced Operator (" & inexpOperCount.ToString & ")") 'updated 7/2/2014 to show count in parenthesis
                                        totalSurchargeCount += inexpOperCount 'added 7/2/2014
                                    End If
                                End If
                                '5/20/2014 note: shouldn't need to do anything here for CLUE reports... being done in LoadTreeView method
                                If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                                    '7/1/2014 - updated to get counts for each type of discount/surcharge
                                    Dim multiVehCount As Integer = 0
                                    'Dim vehAntiTheftCount As Integer = 0 'CAH Bug31053 - no longer rated 
                                    'Dim vehPassRestCount As Integer = 0 'CAH Bug31053 - no longer rated
                                    Dim outOfStateVehCount As Integer = 0
                                    For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                                        If v.MultiCar = True Then
                                            multiVehCount += 1
                                        End If
                                        'If v.AntiTheftTypeId <> "" Then 'CAH Bug31053 - no longer rated
                                        '    Dim antiTheftType As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.AntiTheftTypeId, v.AntiTheftTypeId)
                                        '    If antiTheftType <> "" AndAlso UCase(antiTheftType) <> "NONE" Then
                                        '        vehAntiTheftCount += 1
                                        '    End If
                                        'End If
                                        'If v.RestraintTypeId <> "" ThenCAH Bug31053 - no longer rated
                                        '    Dim restraintType As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.RestraintTypeId, v.RestraintTypeId)
                                        '    If restraintType <> "" AndAlso UCase(restraintType) <> "NONE" Then
                                        '        vehPassRestCount += 1
                                        '    End If
                                        'End If
                                        If v.DriverOutOfStateSurcharge = True Then
                                            outOfStateVehCount += 1
                                        End If
                                    Next
                                    '7/1/2014 - updated to get counts for each type of discount/surcharge
                                    If multiVehCount > 0 Then
                                        hasMultiVehicle = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add($"Multi Vehicle ({multiVehCount})")
                                        totalDiscountCount += multiVehCount
                                    End If
                                    'If vehAntiTheftCount > 0 Then 'CAH Bug31053 - no longer rated
                                    '    hasAntiTheft = True
                                    '    hasDiscounts = True
                                    '    RatingDiscounts.Add($"Vehicle Anti-Theft ({vehAntiTheftCount.ToString})")
                                    '    totalDiscountCount += vehAntiTheftCount
                                    'End If
                                    'If vehPassRestCount > 0 Then 'CAH Bug31053 - no longer rated
                                    '    hasPassiveRestraint = True
                                    '    hasDiscounts = True
                                    '    RatingDiscounts.Add($"Vehicle Passive Restraint ({vehPassRestCount})")
                                    '    totalDiscountCount += vehPassRestCount
                                    'End If
                                    If outOfStateVehCount > 0 Then
                                        hasOutOfStateVehicle = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add($"Out of State Vehicle ({outOfStateVehCount})")
                                        totalSurchargeCount += outOfStateVehCount
                                    End If
                                End If
                            End If

                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuoteObject.QuickQuoteLobType.Farm
                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                                Dim lossHistoryCount As Integer = 0
                                Dim lossHistoryCount_surchargeable As Integer = 0
                                EvaluateLossHistoriesForRatedQuote(QuoteOrRatedQuoteType.RatedQuote, lossHistoryCount, lossHistoryCount_surchargeable, Me.Quote, Me.GoverningStateQuote)
                                If lossHistoryCount > 0 Then
                                    hasLossHistory = True 'may not use or may use for all and not just individual LOBs
                                End If
                                If lossHistoryCount_surchargeable > 0 Then
                                    hasLossHistory_surchargeable = True
                                    hasSurcharges = True
                                    RatingSurcharges.Add($"Loss Histor{If(lossHistoryCount_surchargeable > 1, "ies", "y")} ({lossHistoryCount_surchargeable})")
                                    totalSurchargeCount += lossHistoryCount_surchargeable
                                End If

                                'updated 11/6/2014
                                If Me.Quote.Locations.IsLoaded() Then
                                    'should only have 1 location per policy, but logic is written this way in case some of it is re-used for other LOBs (w/ multiple locations) in the future
                                    Dim burglarAlarmCount As Integer = 0
                                    Dim smokeAlarmCount As Integer = 0
                                    Dim matureHomeownerCount As Integer = 0
                                    Dim multiPolicyCount As Integer = 0
                                    Dim newHomeCount As Integer = 0
                                    Dim selectMarketCount As Integer = 0
                                    Dim sprinklerCount As Integer = 0
                                    Dim swimmingPoolCount As Integer = 0
                                    Dim trampolineCount As Integer = 0
                                    Dim woodBurningCount As Integer = 0
                                    'added 1/2/2015 for HOM
                                    Dim youthfulOperatorCount As Integer = 0
                                    Dim mobileHomeTieDownCount As Integer = 0
                                    Dim watercraftAgeCount As Integer = 0
                                    Dim acvRoofCount As Integer = 0 'added 1/5/2015 for HOM
                                    Dim hobbyFarmCount As Integer = 0 'added 8/11/2015 for Farm
                                    Dim heatedBuildingCount As Integer = 0 'added 8/11/2015 for Farm
                                    '1/5/2015 note: also HOM credits for Customer Tier and Home Deductible, but no logic on when to show... may just need percentages found in Coverage Calc for viewable/printable summaries and tree doesn't matter
                                    For Each rl As QuickQuoteLocation In Me.Quote.Locations
                                        With rl
                                            If .BurglarAlarm_CentralStationAlarmSystem = True Then
                                                burglarAlarmCount += 1
                                            End If
                                            If .BurglarAlarm_LocalAlarmSystem = True Then
                                                burglarAlarmCount += 1
                                            End If
                                            If .BurglarAlarm_CentralAlarmSystem = True Then
                                                burglarAlarmCount += 1
                                            End If
                                            If .PoliceDepartmentTheftAlarm = True Then
                                                burglarAlarmCount += 1
                                            End If
                                            If .FireSmokeAlarm_CentralStationAlarmSystem = True Then
                                                smokeAlarmCount += 1
                                            End If
                                            If .FireSmokeAlarm_LocalAlarmSystem = True Then
                                                smokeAlarmCount += 1
                                            End If
                                            If .FireSmokeAlarm_SmokeAlarm = True Then
                                                smokeAlarmCount += 1
                                            End If
                                            If .FireSmokeAlarm_CentralAlarmSystem = True Then
                                                smokeAlarmCount += 1
                                            End If
                                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso .FireDepartmentAlarm = True Then 'probably doesn't need LOB, but that's the only case where it will change the label from Smoke Alarm to Fire/Smoke Alarm
                                                smokeAlarmCount += 1
                                            End If
                                            If .MatureHomeownerDiscount = True Then
                                                matureHomeownerCount += 1
                                            End If
                                            If .MultiPolicyDiscount = True Then
                                                multiPolicyCount += 1
                                            End If
                                            If .NewHomeDiscount = True Then
                                                newHomeCount += 1
                                            End If
                                            If .SelectMarketCredit = True Then
                                                selectMarketCount += 1
                                            End If
                                            If .SprinklerSystem_AllExcept = True Then
                                                sprinklerCount += 1
                                            End If
                                            If .SprinklerSystem_AllIncluding = True Then
                                                sprinklerCount += 1
                                            End If
                                            If .SwimmingPoolHotTubSurcharge = True Then
                                                swimmingPoolCount += 1
                                            End If
                                            If .TrampolineSurcharge = True Then
                                                trampolineCount += 1
                                            End If
                                            If .WoodOrFuelBurningApplianceSurcharge = True Then
                                                woodBurningCount += 1
                                            End If
                                            If .HobbyFarmCredit = True Then 'added 8/11/2015 for Farm
                                                hobbyFarmCount += 1
                                            End If

                                            'added 1/2/2015
                                            If Me.Quote.EffectiveDate <> "" AndAlso IsDate(Me.Quote.EffectiveDate) = True Then
                                                If .RvWatercrafts.IsLoaded() Then
                                                    For Each rRv As QuickQuoteRvWatercraft In .RvWatercrafts
                                                        With rRv
                                                            If .Operators.IsLoaded() Then 'may need to compare w/ policy-level Operators... if name info (dob) is only there
                                                                For Each rO As QuickQuoteOperator In .Operators
                                                                    With rO
                                                                        If .Name IsNot Nothing Then
                                                                            With .Name
                                                                                If .BirthDate <> "" AndAlso IsDate(.BirthDate) = True Then
                                                                                    If CDate(.BirthDate) >= DateAdd(DateInterval.Year, -24, CDate(Me.Quote.EffectiveDate)) Then
                                                                                        youthfulOperatorCount += 1
                                                                                    End If
                                                                                End If
                                                                            End With
                                                                        End If
                                                                    End With
                                                                Next
                                                            End If
                                                            If .Year <> "" AndAlso IsNumeric(.Year) = True AndAlso CInt(.Year) > 0 AndAlso .RvWatercraftTypeId <> "" AndAlso IsNumeric(.RvWatercraftTypeId) = True Then
                                                                If CInt(.RvWatercraftTypeId) = 1 Then 'could also use static data file logic to look for description (1 = Watercraft)
                                                                    Dim effYear As Integer = CDate(Me.Quote.EffectiveDate).Year
                                                                    Dim rvYearPlus15 As Integer = CInt(CInt(.Year) + 15)
                                                                    If rvYearPlus15 <= effYear Then
                                                                        watercraftAgeCount += 1
                                                                    End If
                                                                End If
                                                            End If
                                                        End With
                                                    Next
                                                End If
                                            End If

                                            'added 1/2/2015
                                            If .FormTypeId <> "" AndAlso IsNumeric(.FormTypeId) = True AndAlso .StructureTypeId <> "" AndAlso IsNumeric(.StructureTypeId) = True AndAlso .MobileHomeTieDownTypeId <> "" AndAlso IsNumeric(.MobileHomeTieDownTypeId) = True Then
                                                If .FormTypeId.EqualsAny("6", "7", "22", "25") Then '(6 = ML-2 - Mobile Home Owner Occupied; 7 = ML-4 - Mobile Home Tenant Occupied)
                                                    If .StructureTypeId.EqualsAny("2", "8", "14") Then '(2 = Mobile Home [ignoreForLists="Yes"]; 8 = Mobile Manufactured [ignoreForLists="Yes"]; 14 = Mobile Manufactured [Pre-fab in db])
                                                        If .MobileHomeTieDownTypeId.EqualsAny("1", "2") Then '(1 = Full; 2 = Chassis)
                                                            mobileHomeTieDownCount += 1
                                                        End If
                                                    End If
                                                End If
                                            End If

                                            'added 1/5/2015
                                            If .SectionICoverages IsNot Nothing AndAlso .SectionICoverages.Count > 0 Then
                                                For Each sIc As QuickQuoteSectionICoverage In .SectionICoverages
                                                    Select Case sIc.CoverageType
                                                        Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing 'may need to handle for ActualCashValueLossSettlement too
                                                            acvRoofCount += 1
                                                    End Select
                                                Next
                                            End If

                                            'added 8/11/2015 for Farm; may not use
                                            If .Buildings IsNot Nothing AndAlso .Buildings.Count > 0 Then
                                                For Each rb As QuickQuoteBuilding In .Buildings
                                                    With rb
                                                        'updated 8/11/2015 to count individually
                                                        If .SprinklerSystem_AllExcept = True Then
                                                            sprinklerCount += 1
                                                        End If
                                                        If .SprinklerSystem_AllIncluding = True Then
                                                            sprinklerCount += 1
                                                        End If
                                                        If .HeatedBuildingSurchargeGasElectric = True Then
                                                            heatedBuildingCount += 1
                                                        End If
                                                        If .HeatedBuildingSurchargeOther = True Then
                                                            heatedBuildingCount += 1
                                                        End If
                                                        If .ExposedInsulationSurcharge = True Then
                                                            heatedBuildingCount += 1
                                                        End If
                                                    End With
                                                Next
                                            End If
                                        End With
                                    Next
                                    If burglarAlarmCount > 0 Then
                                        hasBurglarAlarm = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add($"Burglar Alarm{If(burglarAlarmCount > 1, $" ({burglarAlarmCount})", "")}")
                                        totalDiscountCount += burglarAlarmCount
                                    End If
                                    If smokeAlarmCount > 0 Then
                                        hasSmokeAlarm = True
                                        hasDiscounts = True
                                        'updated 8/11/2015 for Farm
                                        Dim smokeAlarmPrefix As String = "Smoke"
                                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                                            smokeAlarmPrefix = "Fire/" & smokeAlarmPrefix
                                        End If
                                        RatingDiscounts.Add($"{smokeAlarmPrefix} Alarm{If(smokeAlarmCount > 1, $" ({smokeAlarmCount})", "")}")
                                        totalDiscountCount += smokeAlarmCount
                                    End If
                                    If matureHomeownerCount > 0 Then
                                        hasMatureHomeowner = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add($"Mature Homeowner{If(matureHomeownerCount > 1, $" ({matureHomeownerCount})", "")}")
                                        totalDiscountCount += matureHomeownerCount
                                    End If
                                    If multiPolicyCount > 0 Then
                                        hasMultiPolicy = True
                                        hasDiscounts = True
                                        Dim countText = If(multiPolicyCount > 1, $" ({multiPolicyCount})", "")
                                        RatingDiscounts.Add(If(VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote), $"Auto/Home{countText}", $"Multi Policy{countText}"))
                                        totalDiscountCount += multiPolicyCount
                                    End If
                                    If Me.GoverningStateQuote.MultiLineDiscount.TryToGetInt32() > 4 Then ' Parachute
                                        hasMultiPolicy = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Multi Line")
                                        totalDiscountCount += 1
                                    End If
                                    If newHomeCount > 0 Then
                                        hasNewHome = True
                                        hasDiscounts = True
                                        If IFM.VR.Common.Helpers.HOM.BuildingFactorHelper.IsBuildingFactorAvailable(Quote) AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                            'Show as Building Factor for Home only
                                            RatingDiscounts.Add($"Building Factor{If(newHomeCount > 1, $" ({newHomeCount})", "")}")
                                        Else
                                            'This will show as New Home for all lines of business but Home. Note: when removing the IsBuildingFactorAvailable flag after it expires and we do code cleanup, keep the LobType check.
                                            RatingDiscounts.Add($"New Home{If(newHomeCount > 1, $" ({newHomeCount})", "")}")
                                        End If
                                        totalDiscountCount += newHomeCount
                                    End If
                                    If selectMarketCount > 0 Then
                                        hasSelectMarket = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Select Market" & If(selectMarketCount > 1, " (" & selectMarketCount & ")", ""))
                                        totalDiscountCount += selectMarketCount
                                    End If
                                    If sprinklerCount > 0 Then
                                        hasSprinkler = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Sprinkler" & If(sprinklerCount > 1, " (" & sprinklerCount & ")", ""))
                                        totalDiscountCount += sprinklerCount
                                    End If
                                    If swimmingPoolCount > 0 Then
                                        hasSwimmingPool = True
                                        hasSurcharges = True
                                        If FARM.SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) Then
                                            RatingSurcharges.Add("Swimming Pool / Hot Tub")
                                            totalSurchargeCount += 1
                                        Else
                                            RatingSurcharges.Add("Swimming Pool / Hot Tub" & If(swimmingPoolCount > 1, " (" & swimmingPoolCount & ")", ""))
                                            totalSurchargeCount += swimmingPoolCount

                                        End If

                                    End If
                                    If trampolineCount > 0 Then
                                        hasTrampoline = True
                                        hasSurcharges = True
                                        If FARM.TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote) Then
                                            RatingSurcharges.Add("Trampoline")
                                            totalSurchargeCount += 1
                                        Else
                                            RatingSurcharges.Add("Trampoline" & If(trampolineCount > 1, " (" & trampolineCount & ")", ""))
                                            totalSurchargeCount += trampolineCount
                                        End If
                                    End If
                                    If woodBurningCount > 0 Then
                                        hasWoodBurning = True
                                        hasSurcharges = True
                                        If FARM.WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote) Then
                                            RatingSurcharges.Add("Wood Burning Appliance")
                                            totalSurchargeCount += 1
                                        Else
                                            RatingSurcharges.Add("Wood Burning Appliance" & If(woodBurningCount > 1, " (" & woodBurningCount & ")", ""))
                                            totalSurchargeCount += woodBurningCount
                                        End If
                                    End If

                                    If youthfulOperatorCount > 0 Then 'added 1/2/2015
                                        hasYouthfulOperator = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add("Youthful Operator" & If(youthfulOperatorCount > 1, " (" & youthfulOperatorCount & ")", ""))
                                        totalSurchargeCount += youthfulOperatorCount
                                    End If
                                    If mobileHomeTieDownCount > 0 Then 'added 1/2/2015
                                        hasMobileHomeTieDown = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Mobile Home Tie Down" & If(mobileHomeTieDownCount > 1, " (" & mobileHomeTieDownCount & ")", ""))
                                        totalDiscountCount += mobileHomeTieDownCount
                                    End If
                                    If watercraftAgeCount > 0 Then 'added 1/2/2015
                                        hasWatercraftAge = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add("Age of Watercraft" & If(watercraftAgeCount > 1, " (" & watercraftAgeCount & ")", ""))
                                        totalSurchargeCount += watercraftAgeCount
                                    End If
                                    If acvRoofCount > 0 Then 'added 1/5/2015
                                        hasAcvRoof = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("ACV Roof" & If(acvRoofCount > 1, " (" & acvRoofCount & ")", ""))
                                        totalDiscountCount += acvRoofCount
                                    End If
                                    If hobbyFarmCount > 0 Then 'added 8/11/2015 for Farm
                                        hasHobbyFarm = True
                                        hasDiscounts = True
                                        RatingDiscounts.Add("Hobby Farm Credit" & If(hobbyFarmCount > 1, " (" & hobbyFarmCount & ")", ""))
                                        totalDiscountCount += hobbyFarmCount
                                    End If
                                    If heatedBuildingCount > 0 Then 'added 8/11/2015 for Farm
                                        hasheatedBuilding = True
                                        hasSurcharges = True
                                        RatingSurcharges.Add("Heating" & If(heatedBuildingCount > 1, " (" & heatedBuildingCount & ")", ""))
                                        totalSurchargeCount += heatedBuildingCount
                                    End If
                                End If
                            End If
                    End Select

                    'added 8/14/2015
                    If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso IsEndorsementRelated() = False Then
                        Dim irpmFactor As Decimal = QQHelper.IRPM_CreditDebitFactor(Me.GoverningStateQuote)
                        If irpmFactor = CDec(1) Then
                            'nothing changed or net is the same
                        Else
                            If irpmFactor < CDec(1) Then
                                'less than 1
                                hasDiscounts = True
                                RatingDiscounts.Add("IRPM")
                                totalDiscountCount += 1
                            Else
                                'more than 1
                                hasSurcharges = True
                                RatingSurcharges.Add("IRPM")
                                totalSurchargeCount += 1
                            End If
                        End If
                    End If

                End If
            Else
                'rating failed
                If Me.Quote.LobType <> Nothing AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    End Select
                End If
            End If

            'added validation error logic 4/2/2014... could just put this in rating failed ELSE block above
            Dim hasValidationErrors As Boolean = False
            'updated 4/3/2014
            Dim ValidationErrors As New List(Of String)
            Dim hasValidationWarnings As Boolean = False
            Dim ValidationWarnings As New List(Of String)
            Dim hasValidationMessages As Boolean = False
            Dim ValidationMessages As New List(Of String)
            If Me.Quote.ValidationItems IsNot Nothing AndAlso Me.Quote.ValidationItems.Count > 0 Then
                For Each vi As QuickQuoteValidationItem In Me.Quote.ValidationItems
                    'updated 4/3/2014 to make sure there's a message
                    If vi.Message <> "" Then
                        If vi.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Then 'updated 4/3/2014 to make sure there's a message
                            hasValidationErrors = True
                            'Exit For 'remove exit if all errors need to be captured; commented 4/3/2014
                            ValidationErrors.Add(vi.Message)
                        ElseIf vi.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationWarning Then
                            'Updated 12/28/2021 for Task 52950 MLW
                            If Not IsQuoteEndorsement() AndAlso vi.Message.ToUpper.Contains("Territory updated from".ToUpper) Then
                                'do not add
                                Continue For
                            ElseIf vi.Message.ToUpper.Contains("Transunion Vehicle History".ToUpper) Then ' exclude "Transunion Vehicle History" warning bug 52928 BD
                                'do not add
                                Continue For
                            Else
                                hasValidationWarnings = True
                                ValidationWarnings.Add(vi.Message)
                            End If
                            'hasValidationWarnings = True
                            'ValidationWarnings.Add(vi.Message)
                        Else 'None or NonApplicable or Other
                            'Updated 12/23/2021 for Task 52950 MLW
                            If Not IsQuoteEndorsement() AndAlso vi.Message.ToUpper.Contains("Territory updated from".ToUpper) Then
                                'do not add
                                Continue For
                            ElseIf vi.Message.ToUpper.Contains("Transunion Vehicle History".ToUpper) Then '
                                'do not add
                                Continue For
                            Else
                                hasValidationMessages = True
                                ValidationMessages.Add(vi.Message)
                            End If
                            'hasValidationMessages = True
                            'ValidationMessages.Add(vi.Message)
                        End If
                    End If
                Next
            End If

            If hasValidationErrors = True Then
                summaryXImageArea.Visible = True
            Else
                summaryXImageArea.Visible = False
            End If
            If hasValidationErrors = True OrElse hasValidationWarnings = True OrElse hasValidationMessages = True OrElse hasDiscounts = True OrElse hasSurcharges = True Then
                Dim dtSummaryItems As New DataTable
                dtSummaryItems.Columns.AddStrings($"{summaryPrefix}SummaryItemDescription", $"{summaryPrefix}SummaryItemIdentifier", $"{summaryPrefix}SummaryItemCount")

                If hasDiscounts = True Then
                    Dim drSummaryItem As DataRow
                    drSummaryItem = dtSummaryItems.NewRow
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemDescription") = $"Discounts ({totalDiscountCount})"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemIdentifier") = "Discounts"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemCount") = RatingDiscounts.Count.ToString
                    dtSummaryItems.Rows.Add(drSummaryItem)
                End If
                If hasSurcharges = True Then
                    Dim drSummaryItem As DataRow
                    drSummaryItem = dtSummaryItems.NewRow
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemDescription") = $"Surcharges ({totalSurchargeCount})"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemIdentifier") = "Surcharges"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemCount") = RatingSurcharges.Count.ToString
                    dtSummaryItems.Rows.Add(drSummaryItem)
                End If
                If hasValidationErrors = True Then
                    Dim drSummaryItem As DataRow
                    drSummaryItem = dtSummaryItems.NewRow
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemDescription") = $"Errors ({ValidationErrors.Count})"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemIdentifier") = "Errors"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemCount") = ValidationErrors.Count.ToString
                    dtSummaryItems.Rows.Add(drSummaryItem)
                End If
                If hasValidationWarnings = True Then
                    Dim drSummaryItem As DataRow
                    drSummaryItem = dtSummaryItems.NewRow
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemDescription") = $"Warnings ({ValidationWarnings.Count})"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemIdentifier") = "Warnings"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemCount") = ValidationWarnings.Count.ToString
                    dtSummaryItems.Rows.Add(drSummaryItem)
                End If
                If hasValidationMessages = True Then
                    Dim drSummaryItem As DataRow
                    drSummaryItem = dtSummaryItems.NewRow
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemDescription") = $"Messages ({ValidationMessages.Count})"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemIdentifier") = "Messages"
                    drSummaryItem.Item($"{summaryPrefix}SummaryItemCount") = ValidationMessages.Count.ToString
                    dtSummaryItems.Rows.Add(drSummaryItem)
                End If

                summaryItemsRepeater.DataSource = dtSummaryItems
                summaryItemsRepeater.DataBind()

                'now loop through rpt items and bind sub grids; could also do w/ rptQuoteSummaryItems_ItemDataBound event, but it wouldn't have the lists from here
                For Each i As RepeaterItem In summaryItemsRepeater.Items
                    Dim lblSummaryItemDescription As Label = i.FindControl($"lbl{summaryPrefix}SummaryItemDescription")
                    Dim lblSummaryItemIdentifier As Label = i.FindControl($"lbl{summaryPrefix}SummaryItemIdentifier")
                    Dim lblSummaryItemCount As Label = i.FindControl($"lbl{summaryPrefix}SummaryItemCount")
                    Dim pnlSummarySubItems As Panel = i.FindControl($"pnl{summaryPrefix}SummarySubItems")
                    Dim rptSummarySubItems As Repeater = i.FindControl($"rpt{summaryPrefix}SummarySubItems")
                    Dim SummaryItemSubLists_expandCollapseImageArea As HtmlGenericControl = i.FindControl($"{summaryPrefix}SummaryItemSubLists_expandCollapseImageArea")

                    If lblSummaryItemDescription IsNot Nothing AndAlso lblSummaryItemDescription.Text <> "" Then
                        If lblSummaryItemDescription.ToolTip <> "" Then
                            If TooltipContainsDescription(lblSummaryItemDescription.ToolTip, $" ({lblSummaryItemDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                lblSummaryItemDescription.ToolTip &= $" ({lblSummaryItemDescription.Text})"
                            End If
                        Else
                            lblSummaryItemDescription.ToolTip = lblSummaryItemDescription.Text
                        End If
                    End If
                    If lblSummaryItemIdentifier IsNot Nothing AndAlso lblSummaryItemIdentifier.Text <> "" Then
                        Dim okayToContinue As Boolean = False
                        Select Case UCase(lblSummaryItemIdentifier.Text)
                            Case "ERRORS", "WARNINGS", "MESSAGES", "DISCOUNTS", "SURCHARGES"
                                okayToContinue = True
                        End Select
                        If okayToContinue = True Then
                            If pnlSummarySubItems IsNot Nothing Then
                                pnlSummarySubItems.Visible = False

                                If lblSummaryItemCount IsNot Nothing AndAlso lblSummaryItemCount.Text <> "" AndAlso IsNumeric(lblSummaryItemCount.Text) = True Then
                                    If SummaryItemSubLists_expandCollapseImageArea IsNot Nothing AndAlso rptSummarySubItems IsNot Nothing Then
                                        SummaryItemSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

                                        If CInt(lblSummaryItemCount.Text) > 0 Then
                                            pnlSummarySubItems.Visible = True
                                            SummaryItemSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                                            Dim dtSummarySubItems As New DataTable
                                            dtSummarySubItems.Columns.AddStrings($"{summaryPrefix}SummarySubItemDescription")

                                            Select Case UCase(lblSummaryItemIdentifier.Text)
                                                Case "ERRORS"
                                                    For Each ve As String In ValidationErrors
                                                        Dim drSummarySubItem As DataRow
                                                        drSummarySubItem = dtSummarySubItems.NewRow
                                                        drSummarySubItem.Item($"{summaryPrefix}SummarySubItemDescription") = ve
                                                        dtSummarySubItems.Rows.Add(drSummarySubItem)
                                                    Next
                                                Case "WARNINGS"
                                                    For Each vw As String In ValidationWarnings
                                                        Dim drSummarySubItem As DataRow
                                                        drSummarySubItem = dtSummarySubItems.NewRow
                                                        drSummarySubItem.Item($"{summaryPrefix}SummarySubItemDescription") = vw
                                                        dtSummarySubItems.Rows.Add(drSummarySubItem)
                                                    Next
                                                Case "MESSAGES"
                                                    For Each vm As String In ValidationMessages
                                                        Dim drSummarySubItem As DataRow
                                                        drSummarySubItem = dtSummarySubItems.NewRow
                                                        drSummarySubItem.Item($"{summaryPrefix}SummarySubItemDescription") = vm
                                                        dtSummarySubItems.Rows.Add(drSummarySubItem)
                                                    Next
                                                Case "DISCOUNTS"
                                                    For Each rd As String In RatingDiscounts
                                                        Dim drSummarySubItem As DataRow
                                                        drSummarySubItem = dtSummarySubItems.NewRow
                                                        drSummarySubItem.Item($"{summaryPrefix}SummarySubItemDescription") = rd
                                                        dtSummarySubItems.Rows.Add(drSummarySubItem)
                                                    Next
                                                Case "SURCHARGES"
                                                    For Each rs As String In RatingSurcharges
                                                        Dim drSummarySubItem As DataRow
                                                        drSummarySubItem = dtSummarySubItems.NewRow
                                                        drSummarySubItem.Item($"{summaryPrefix}SummarySubItemDescription") = rs
                                                        dtSummarySubItems.Rows.Add(drSummarySubItem)
                                                    Next
                                            End Select

                                            rptSummarySubItems.DataSource = dtSummarySubItems
                                            rptSummarySubItems.DataBind()

                                            For Each si As RepeaterItem In rptSummarySubItems.Items
                                                Dim lblSummarySubItemDescription As Label = si.FindControl($"lbl{summaryPrefix}SummarySubItemDescription")
                                                If lblSummarySubItemDescription IsNot Nothing AndAlso lblSummarySubItemDescription.Text <> "" Then
                                                    If lblSummarySubItemDescription.ToolTip <> "" Then
                                                        If TooltipContainsDescription(lblSummarySubItemDescription.ToolTip, $" ({lblSummarySubItemDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                                            lblSummarySubItemDescription.ToolTip &= $" ({lblSummarySubItemDescription.Text})"
                                                        End If
                                                    Else
                                                        lblSummarySubItemDescription.ToolTip = lblSummarySubItemDescription.Text
                                                    End If
                                                End If
                                            Next
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next

                summaryItemsPanel.Visible = True
                summaryExpandCollapseImageArea.Style.Add("visibility", "visible")
            Else
                summaryItemsPanel.Visible = False
                summaryExpandCollapseImageArea.Style.Add("visibility", "hidden")
            End If

            'added 6/28/2019 for Endorsements since Rate could order MVR/CLUE and require pulling in new accViols/lossHists
            'If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            'updated 6/23/2020; note: GetEditableLossFlagForTree may not be the best name, but it checks what we want
            If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse GetEditableLossFlagForTree() = False Then
                Dim hasDrivers As Boolean = False
                LoadDrivers(QuoteOrRatedQuoteType.RatedQuote, hasDrivers)
            End If

            '1/2/2015 - now calling method for Vehicles... handling everything from 1 spot instead of having different logic in LoadTreeView and LoadRatedQuoteIntoTree; was previously only loading for normal QuoteObject until update to get Vehicle classCode
            Dim hasVehicles As Boolean = False
            LoadVehicles(QuoteOrRatedQuoteType.RatedQuote, hasVehicles)

            '9/29/2014 - new credit/mvr reports logic... now handling everything from 1 spot instead of having different logic in LoadTreeView and LoadRatedQuoteIntoTree
            Dim hasCreditReports As Boolean = False
            LoadCreditReports(QuoteOrRatedQuoteType.RatedQuote, hasCreditReports, Me.Quote, Me.GoverningStateQuote)
            Dim hasMvrReports As Boolean = False
            LoadMvrReports(QuoteOrRatedQuoteType.RatedQuote, hasMvrReports, Me.GoverningStateQuote)
            '9/29/2014 note: LoadClueReports is only called from LoadTreeView

            'added 8/21/2017
            Select Case QQHelper.IntegerForString(Me.Quote.Database_QuoteStatusId)
                Case 15, 16, 17, 18 'Quote Stopped, Quote Killed, App Stopped, App Killed
                    'hide premium on these statuses (specifically Quote Stopped and App Stopped)
                    summaryTotalPremiumSection.Visible = False
                    Me.VehiclesPremiumSection.Visible = False
                    Me.LocationsPremiumSection.Visible = False

                    'added 9/2/2017
                    If Me.IsOnAppPage Then
                        IsApplicationSummarySectionEnabled = False 'might show tooltip so user knows it's not available... won't be showing anyway
                    Else
                        IsQuoteSummarySectionEnabled = False 'might show tooltip so user knows it's not available... won't be showing anyway
                    End If
                    IsIRPMSectionEnabled = False
            End Select
        Else
            If Me.IsOnAppPage Then
                IsApplicationSummarySectionEnabled = False 'might show tooltip so user knows it's not available... won't be showing anyway
            Else
                IsQuoteSummarySectionEnabled = False 'might show tooltip so user knows it's not available... won't be showing anyway
            End If
        End If

        'added 6/26/2014
        SetQuoteNumberLabelAndOptionallyShowHeader()

        Me.SwitchToUnRatedQuote()
    End Sub

    Private Sub SwitchToUnRatedQuote()
        'IMPORTANT
        ' need to switch to make Me.Quote use the unrated quote object
        ' also need to clear any subquote cache
        Me.UseRatedQuoteImage = False
        Me.SubQuoteListRefresh()
    End Sub

    Private Sub SwitchToRatedQuote()
        'IMPORTANT
        ' need to switch to make Me.Quote use the rated quote object
        ' also need to clear any subquote cache
        Me.UseRatedQuoteImage = True
        Me.SubQuoteListRefresh()
    End Sub

    Private Sub DetermineIsIRPMSectionEnabled()
        'Sub - Change to IRPM functionality by LOB (enabled/disabled) - CH 7/24/2017
        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500")) Then
                    IsIRPMSectionEnabled = False
                Else
                    'If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False OrElse IsQuoteSummarySectionEnabled = True OrElse IsApplicationSummarySectionEnabled = True Then
                    'updated 10/26/2020 (Interoperability)
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False OrElse hasIRPMAdjustment() OrElse IsQuoteSummarySectionEnabled = True OrElse IsApplicationSummarySectionEnabled = True Then
                        IsIRPMSectionEnabled = True
                    End If
                End If
                If IsIRPMSectionEnabled = True Then
                    If Me.GetProgramType() = "FARM LIABILITY" OrElse (Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count > 0 AndAlso Me.Quote.Locations(0).FarmTypeHobby = True) Then
                        IsIRPMSectionEnabled = False
                    End If
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500")) And Not hasIRPMAdjustment() Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False Or hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If Me.hdnQuoteNumber.Value = "" OrElse (Me.Quote.Vehicles IsNot Nothing And QuickQuoteObjectHelper.GetMotorizedVehicleCount(Me.Quote.Vehicles) < 2) OrElse QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = True And Not hasIRPMAdjustment() Then
                    IsIRPMSectionEnabled = False
                Else
                    If (Me.Quote.Vehicles IsNot Nothing And QuickQuoteObjectHelper.GetMotorizedVehicleCount(Me.Quote.Vehicles) > 1) Or hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

            Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("1500")) And Not hasIRPMAdjustment() Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False Or hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

            Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500")) And Not hasIRPMAdjustment() Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False Or hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

            Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500")) And Not hasIRPMAdjustment() Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False Or hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage 'added 12/2/2020 so CPP won't hit ELSE... and can use normal logic that looks for IRPM adjustment; note: wrote IF statement a little different than other CASEs (will leave others the same for now)
                ' NOTE: CPP IRPM eligibility is based on the sum of the CPR & CGL premiums, not the entire quote premium BUG 63093 8/10/21 MGB
                Dim CGLPrem As Integer = 0
                Dim CPRPrem As Integer = 0
                Dim IRPMPrem As Integer = 0
                If Integer.TryParse(Me.SubQuoteFirst.CPP_GL_PackagePart_QuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, CGLPrem) _
                                AndAlso Integer.TryParse(Me.SubQuoteFirst.CPP_CPR_PackagePart_QuotedPremium, Globalization.NumberStyles.Currency, Globalization.CultureInfo.CurrentCulture, CPRPrem) Then
                    IRPMPrem = CPRPrem + CGLPrem
                Else
                    IRPMPrem = 0
                End If

                If (Me.hdnQuoteNumber.Value = "" OrElse (IRPMPrem < CInt("500"))) AndAlso (Not hasIRPMAdjustment()) Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False OrElse hasIRPMAdjustment() Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

                ' Old code
                'If (Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500"))) AndAlso (Not hasIRPMAdjustment()) Then
                '    IsIRPMSectionEnabled = False
                'Else
                '    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False OrElse hasIRPMAdjustment() Then
                '        IsIRPMSectionEnabled = True
                '    End If
                'End If

            Case Else
                If Me.hdnQuoteNumber.Value = "" OrElse (QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False AndAlso CDec(Me.Quote.TotalQuotedPremium) < CDec("500")) Then
                    IsIRPMSectionEnabled = False
                Else
                    If QQHelper.IsZeroPremium(Me.Quote.TotalQuotedPremium) = False OrElse IsQuoteSummarySectionEnabled = True OrElse IsApplicationSummarySectionEnabled = True Then
                        IsIRPMSectionEnabled = True
                    End If
                End If

        End Select
        If IrpmVisibilityByClasscodeHelper.IsIrpmVisibilityAvailable(Quote) Then
            If AllLines.IRPM_ClasscodeCheck.IsUnwantedClassCodePresent(Quote) Then
                IsIRPMSectionEnabled = False
            End If
        End If

    End Sub

    Private Function GetProgramType() As String

        Dim MyVehicle As New QuickQuote.CommonObjects.QuickQuoteVehicle()
        If Me.SubQuoteForVehicle(MyVehicle).QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
            'do something special
        End If


        If QQHelper.IsNumericString(Me.SubQuoteFirst.ProgramTypeId) = True Then
            Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, Me.SubQuoteFirst.ProgramTypeId)
        ElseIf Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count > 0 AndAlso QQHelper.IsNumericString(Me.Quote.Locations(0).ProgramTypeId) = True Then
            Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, Me.Quote.Locations(0).ProgramTypeId)
        End If
        Return String.Empty
    End Function

#Region "Save/Rate"
    Private Sub SaveCurrentQuoteObjectAndRaiseUpdatedEvent(ByVal sender As Object, ByVal e As System.EventArgs, Optional ByVal sectionToShow As TreeViewSection = TreeViewSection.None) 'added sectionToShow optional param 1/22/2014
        Dim saveError As String = String.Empty
        If SuccessfulSave(saveError) = True Then
            'launch commDataPrefill on effDate Save if needed
            'If TypeOf sender Is ImageButton AndAlso Me.IsSummaryWorkflow = True Then
            '    Dim imgBtn As ImageButton = CType(sender, ImageButton)
            '    If imgBtn.ClientID = Me.imgBtnSaveEffectiveDate.ClientID Then
            '        If WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(Me.Quote) = True Then
            '            Dim ih As New IntegrationHelper
            '            If ih.HasAnyCommercialDataPrefillOrders(Me.Quote) = False AndAlso WebHelper_Personal.IsQuoteIdOrPolicyImageInSessionFromCommDataPrefillError(Me.QuoteIdOrPolicyIdPipeImageNumber) = False Then
            '                WebHelper_Personal.RedirectToQuotePage(Me.Quote.QuoteTransactionType, Me.Quote.LobType, quoteId:=QQHelper.IntegerForString(Me.QuoteId), policyId:=Me.EndorsementPolicyId, policyImageNum:=Me.EndorsementPolicyImageNum)
            '                Exit Sub
            '            End If
            '        End If
            '    End If
            'End If
            If TypeOf sender Is ImageButton Then
                Dim imgBtn As ImageButton = CType(sender, ImageButton)
                If imgBtn.ClientID = Me.imgBtnSaveEffectiveDate.ClientID Then
                    If WebHelper_Personal.IsCommercialDataPrefillPopupAvailableForQuote(Me.Quote, expectedIsSummaryWorkflowValue:=True, request:=Request, qIdOrPIdAndImgNum:=Me.QuoteIdOrPolicyIdPipeImageNumber) = True Then
                        WebHelper_Personal.RedirectToQuotePage(Me.Quote.QuoteTransactionType, Me.Quote.LobType, quoteId:=QQHelper.IntegerForString(Me.QuoteId), policyId:=Me.EndorsementPolicyId, policyImageNum:=Me.EndorsementPolicyImageNum)
                        Exit Sub
                    End If
                End If
            End If

            RaiseEvent QuoteUpdated(sender, e)
            ShowSection(sender, e, sectionToShow)
        Else
            If String.IsNullOrEmpty(saveError) = True Then
                saveError = "There was a problem saving the quote."
            End If
            Me.VRScript.ShowAlert(saveError)
        End If
        Me.LoadTreeView()
    End Sub

    Private Function SuccessfulSave(Optional ByRef errorMessage As String = "") As Boolean
        Dim saveSuccess As Boolean = False
        errorMessage = String.Empty
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
            'no Save needed
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            saveSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=errorMessage)
            IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Me.Session)
        Else
            saveSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, errorMessage)
            'added 9/2/2017
            IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Me.Session)
        End If
        Return saveSuccess
    End Function
#End Region

#Region "UI Events"
    Protected Sub btnExpandAll_Click(sender As Object, e As System.EventArgs) Handles btnExpandAll.Click
        Me.hdnExpandOrCollapseAllFlag.Value = "expand"
    End Sub
    Private Sub btnToggleViewMode_Click(sender As Object, e As EventArgs) Handles btnToggleViewMode.Click
    End Sub

    Protected Sub imgBtnEditQuoteDescription_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnEditQuoteDescription.Click
        If Me.CanNotAccessSection(IsQuoteDescriptionSectionEnabled, "You cannot access the Quote Description at this time.") Then
            Exit Sub
        End If
        Me.QuoteDescriptionEditSection.Style.Add("display", "inline")
        Me.hdnQuoteDescriptionEditSect_Display.Value = "inline"
        Me.QuoteDescriptionViewSection.Style.Add("display", "none")
        Me.hdnQuoteDescriptionViewSect_Display.Value = "none"
        Me.txtQuoteDescription.Text = Me.hdnOriginalQuoteDescription.Value
        Page.SetFocus(Me.txtQuoteDescription) 'added 1/15/2014
    End Sub
    Protected Sub imgBtnSaveQuoteDescription_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnSaveQuoteDescription.Click
        If String.IsNullOrWhiteSpace(Me.txtQuoteDescription.Text) Then 'updated 7/28/2014 from Me.txtQuoteDescription.Text = "" so blank space wouldn't be allowed
            Me.VRScript.ShowAlert("please enter the quote description")
            Page.SetFocus(Me.txtQuoteDescription)
        Else
            'added 7/28/2019 for Endorsements
            Dim okayToSave As Boolean = True
            If Me.Quote IsNot Nothing AndAlso Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Dim ValList = IFM.VR.Validation.ObjectValidation.AllLines.EndorsementValidator.ValidateEndorsementRemarks(Me.txtQuoteDescription.Text)
                If ValList IsNot Nothing AndAlso ValList.Count > 0 AndAlso ValList(0) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ValList(0).Message) = False Then
                    okayToSave = False
                    Me.VRScript.ShowAlert(ValList(0).Message)
                    Page.SetFocus(Me.txtQuoteDescription)
                End If
            End If

            If okayToSave = True Then 'added IF 7/28/2019; originally happening every time
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Me.Quote.TransactionRemark = Me.txtQuoteDescription.Text 'might allow this to be updated
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e)
                    Else
                        Me.Quote.QuoteDescription = Me.txtQuoteDescription.Text
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e)
                    End If
                End If
                InEditMode = False
                Me.hdnTitleFlag_QuoteDescriptionHeader.Value = "on" 'added 2/25/2014
            End If
        End If
    End Sub
    Protected Sub imgBtnCancelSaveQuoteDescription_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnCancelSaveQuoteDescription.Click
        ResetQuoteDescriptionAndEffectiveDateToOriginalLabels()
        Me.QuoteDescriptionViewSection.Style.Add("display", "inline")
        Me.hdnQuoteDescriptionViewSect_Display.Value = "inline"
        Me.QuoteDescriptionEditSection.Style.Add("display", "none")
        Me.hdnQuoteDescriptionEditSect_Display.Value = "none"
        InEditMode = False
    End Sub

    Protected Sub imgBtnEditEffectiveDate_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnEditEffectiveDate.Click
        If Me.CanNotAccessSection(IsEffectiveDateSectionEnabled, "You cannot access the Effective Date at this time.") Then
            Exit Sub
        End If
        Me.EffectiveDateEditSection.Style.Add("display", "inline")
        Me.hdnEffectiveDateEditSect_Display.Value = "inline"
        Me.EffectiveDateViewSection.Style.Add("display", "none")
        Me.hdnEffectiveDateViewSect_Display.Value = "none"
        If Me.hdnOriginalEffectiveDate.Value <> "" AndAlso IsDate(Me.hdnOriginalEffectiveDate.Value) = True Then
            Me.bdpEffectiveDate.SelectedDate = CDate(Me.hdnOriginalEffectiveDate.Value)
        End If
        Page.SetFocus(Me.bdpEffectiveDate)
    End Sub
    Protected Sub imgBtnSaveEffectiveDate_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnSaveEffectiveDate.Click

        If Me.bdpEffectiveDate.Text = "" Then
            Me.VRScript.ShowAlert("please enter the effective date")
            Page.SetFocus(Me.bdpEffectiveDate)
        ElseIf Me.bdpEffectiveDate.IsDate = False Then
            Dim validDateMsg As String = "please enter a valid date"
            If Me.bdpEffectiveDate.DateFormat <> "" Then
                validDateMsg &= $" in the following format: {Me.bdpEffectiveDate.DateFormat}"
            End If
            Me.VRScript.ShowAlert(validDateMsg)
            Page.SetFocus(Me.bdpEffectiveDate)
        Else
            'valid date; now verify minimum and maximum dates
            Dim minEffDateAllQuotes As String = MinimumEffectiveDateAllQuotes
            Dim maxEffDateAllQuotes As String = MaximumEffectiveDateAllQuotes
            Dim minEffDate As String = MinimumEffectiveDate
            Dim maxEffDate As String = MaximumEffectiveDate
            Dim isOkayToUpdate As Boolean = False
            Dim submittedEffectiveDate As Date = Me.bdpEffectiveDate.SelectedDate
            Dim governingState As QuickQuoteState = Me.Quote.GoverningStateQuoteFor()?.QuickQuoteState
            Dim lobDisplayText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.LobId, $"{Me.Quote.LobId}")

            If IsDate(minEffDate) = True AndAlso IsDate(maxEffDate) = True Then
                'these should always be valid dates
                If CDate(Me.bdpEffectiveDate.Text) >= CDate(minEffDate) AndAlso CDate(Me.bdpEffectiveDate.Text) <= CDate(maxEffDate) Then
                    If String.IsNullOrWhiteSpace(hdnOriginalEffectiveDate.Value) = False Then
                        Dim myError As String = ""
                        If _flags.WithFlags(Of LOB.PPA).OhioEnabled AndAlso
                           Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
                           governingState = QuickQuoteState.Ohio AndAlso
                           Not LOBHelper.IsEffectiveDateAcceptableToLobAndGoverningState(submittedEffectiveDate, Me.Quote.LobType, governingState) Then
                            'adding this to quickly enable LOB/State-based earliest effective date
                            Dim earliestEffectiveDate = LOBHelper.GetEarliesttEffectiveDateForLobAndGoverningState(Me.Quote.LobType, governingState)
                            myError = $"The effective date for {lobDisplayText} Quotes in {governingState} must be on Or after {earliestEffectiveDate:MM/dd/yyyy}"

                            Me.VRScript.ShowAlert(myError)
                            Page.SetFocus(Me.bdpEffectiveDate)
                        ElseIf Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso governingState = QuickQuoteState.Kentucky AndAlso Not LOBHelper.IsEffectiveDateAcceptableToLobAndGoverningState(submittedEffectiveDate, Me.Quote.LobType, governingState) Then
                            'Added 3/10/2022 for KY WCP Task 73809 MLW
                            Dim earliestKYWCPGovStateEffectiveDate = CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEffectiveDate)
                            myError = $"The effective date for {lobDisplayText} Quotes in {governingState} must be on Or after {earliestKYWCPGovStateEffectiveDate:MM/dd/yyyy}"
                            Me.VRScript.ShowAlert(myError)
                            Page.SetFocus(Me.bdpEffectiveDate)
                        ElseIf QQHelper.IsEffectiveDateChangeCrossingUncrossableDateLine(Quote, hdnOriginalEffectiveDate.Value, bdpEffectiveDate.Text, myError) Then
                            'Check if there is an uncrossable date line set by a new VR version. If so, display the appropriate error message.
                            Me.VRScript.ShowAlert(myError)
                            Page.SetFocus(Me.bdpEffectiveDate)
                        ElseIf NewCompanyIdHelper.isNewCompanyLocked(Quote) AndAlso submittedEffectiveDate < NewCompanyIdHelper.NewCoGoverningStateEffDate(Quote) Then
                            Dim earliestNewCoEffectiveDate = NewCompanyIdHelper.NewCoGoverningStateEffDate(Quote)
                            myError = $"The effective date for rated Indiana Farmers Indemnity Quotes must be on or after {earliestNewCoEffectiveDate:MM/dd/yyyy}"
                            Me.VRScript.ShowAlert(myError)
                            Page.SetFocus(Me.bdpEffectiveDate)
                            'ElseIf NewCompanyIdHelper.isNewCompanyLocked(Quote) AndAlso NewCompanyIdHelper.DoStatesQualifyByEffectiveDate(Quote, submittedEffectiveDate) = False Then
                            '    Dim earliestNewCoEffectiveDate = NewCompanyIdHelper.GetEarliestDateAllowed(Quote, submittedEffectiveDate)
                            '    myError = $"The effective date to rate this Indiana Farmers Indemnity Quote must be on or after {earliestNewCoEffectiveDate:MM/dd/yyyy}"
                            '    Me.VRScript.ShowAlert(myError)
                            '    Page.SetFocus(Me.bdpEffectiveDate)
                        ElseIf NewCompanyIdHelper.isNewCompanyLocked(Quote) Then
                            Dim earliestNewCoEffectiveDate = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                            If submittedEffectiveDate < earliestNewCoEffectiveDate Then
                                myError = $"The effective date to rate this Indiana Farmers Indemnity Quote must be on or after {earliestNewCoEffectiveDate:MM/dd/yyyy}"
                                Me.VRScript.ShowAlert(myError)
                                Page.SetFocus(Me.bdpEffectiveDate)
                            Else
                                isOkayToUpdate = True
                            End If



                        Else
                            isOkayToUpdate = True
                        End If
                    End If
                Else
                    If CDate(Me.bdpEffectiveDate.Text) < CDate(minEffDate) Then
                        If MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = True OrElse (minEffDate = minEffDateAllQuotes AndAlso QuoteHasMinimumEffectiveDate = True) Then
                            Me.VRScript.ShowAlert($"Contact Underwriting for quotes before {minEffDate}; date set to {minEffDate}.")
                            Me.bdpEffectiveDate.SelectedDate = CDate(minEffDate)
                            isOkayToUpdate = True
                        Else
                            If MinimumEffectiveDateDaysFromToday < 0 AndAlso MaximumEffectiveDateDaysFromToday > 0 Then
                                Me.VRScript.ShowAlert($"effective date must be within the last {MinimumEffectiveDateDaysFromToday.ToString.Replace("-", "")} days")
                                Page.SetFocus(Me.bdpEffectiveDate)
                            Else
                                Me.VRScript.ShowAlert($"effective date must be between {minEffDate} and {maxEffDate}")
                                Page.SetFocus(Me.bdpEffectiveDate)
                            End If
                        End If
                    ElseIf CDate(Me.bdpEffectiveDate.Text) > CDate(maxEffDate) Then
                        'no LOB-specific max effDates yet so this one should not default to max date
                        If MinimumEffectiveDateDaysFromToday < 0 AndAlso MaximumEffectiveDateDaysFromToday > 0 Then
                            Me.VRScript.ShowAlert($"effective date must be within the next {MaximumEffectiveDateDaysFromToday} days")
                            Page.SetFocus(Me.bdpEffectiveDate)
                        Else
                            Me.VRScript.ShowAlert($"effective date must be between {minEffDate} and {maxEffDate}")
                            Page.SetFocus(Me.bdpEffectiveDate)
                        End If
                    Else
                        'shouldn't get here
                        If MinimumEffectiveDateDaysFromToday < 0 AndAlso MaximumEffectiveDateDaysFromToday > 0 Then
                            Me.VRScript.ShowAlert($"effective date must be within the last {MinimumEffectiveDateDaysFromToday.ToString.Replace("-", "")} days or next {MaximumEffectiveDateDaysFromToday.ToString} days")
                            Page.SetFocus(Me.bdpEffectiveDate)
                        Else
                            'minimum date must be in the future and maximum date must be in the past
                            Me.VRScript.ShowAlert($"effective date must be between {minEffDate} and {maxEffDate}")
                            Page.SetFocus(Me.bdpEffectiveDate)
                        End If
                    End If
                End If
            Else
                'shouldn't ever get here
                If CDate(Me.bdpEffectiveDate.Text) < DateAdd(DateInterval.Day, MinimumEffectiveDateDaysFromToday, Date.Today) OrElse CDate(Me.bdpEffectiveDate.Text) > DateAdd(DateInterval.Day, MaximumEffectiveDateDaysFromToday, Date.Today) Then
                    Me.VRScript.ShowAlert($"effective date must be within the last {Math.Abs(MinimumEffectiveDateDaysFromToday)} days or next {MaximumEffectiveDateDaysFromToday} days")
                    Page.SetFocus(Me.bdpEffectiveDate)
                Else
                    'update
                    isOkayToUpdate = True
                End If
            End If

            Dim effDate As DateTime
            If DateTime.TryParse(bdpEffectiveDate.Text, effDate) Then
                If StopOnOrBeforeDate(Me.Quote.LobType) > DateTime.MinValue Then
                    If (effDate <= StopOnOrBeforeDate(Me.Quote.LobType)) Then
                        'stop
                        isOkayToUpdate = False
                        Me.VRScript.ShowAlert(StopOnOrBeforeDateMsg(Me.Quote.LobType))
                    End If
                End If

                If StopOnOrAfterDate(Me.Quote.LobType) < DateTime.MaxValue Then
                    If StopOnOrAfterDate(Me.Quote.LobType) < DateTime.MaxValue Then
                        If (effDate >= StopOnOrAfterDate(Me.Quote.LobType)) Then
                            'stop
                            isOkayToUpdate = False
                            Me.VRScript.ShowAlert(StopOnOrAfterDateMsg(Me.Quote.LobType))
                        End If
                    End If

                End If
            End If

            ' stop multistate effective date jumping
            If isOkayToUpdate Then
                Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(Me.Quote.LobType)
                If lobHelper.IsMultistateCapableLob Then
                    If IsDate(hdnOriginalEffectiveDate.Value) AndAlso IsDate(bdpEffectiveDate.Text) Then
                        Dim currentEffectiveDate As DateTime = CDate(hdnOriginalEffectiveDate.Value)
                        Dim newEffecctiveDate As DateTime = CDate(bdpEffectiveDate.Text)
                        If (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(currentEffectiveDate) AndAlso IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(newEffecctiveDate) = False) Then
                            ' went from multistate to single state
                            isOkayToUpdate = False
                            Me.VRScript.ShowAlert($"Minimum effective date is {IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate.ToShortDateString()}")
                        End If
                        If (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(currentEffectiveDate) = False AndAlso IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(newEffecctiveDate)) Then
                            'went from single state to multistate
                            isOkayToUpdate = False
                            Me.VRScript.ShowAlert($"Maximum effective date is {IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate.AddDays(-1).ToShortDateString()}")
                        End If
                    End If
                End If

            End If

            'added 2/18/2020
            If isOkayToUpdate = True AndAlso Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                If qqxml.Diamond_IsNewTransactionEffectiveDateOkay(QQHelper.IntegerForString(Me.Quote.PolicyId), QQHelper.IntegerForString(Me.Quote.PolicyImageNum), Me.bdpEffectiveDate.SelectedDate.ToShortDateString) = False Then
                    isOkayToUpdate = False
                    Dim tranDateErrorMessage As String = "change effective date must stay within current policy term and version"
                    Dim revertedBack As Boolean = False
                    If QQHelper.IsValidDateString(Me.hdnOriginalEffectiveDate.Value, mustBeGreaterThanDefaultDate:=True) = True Then
                        Me.bdpEffectiveDate.SelectedDate = Me.hdnOriginalEffectiveDate.Value
                        revertedBack = True
                    ElseIf QQHelper.IsValidDateString(Me.Quote.TransactionEffectiveDate, mustBeGreaterThanDefaultDate:=True) = True Then
                        Me.bdpEffectiveDate.SelectedDate = Me.Quote.TransactionEffectiveDate
                        revertedBack = True
                    End If
                    If revertedBack = True Then
                        tranDateErrorMessage &= "; reverted back to " & Me.bdpEffectiveDate.SelectedDate.ToShortDateString
                    End If
                    Me.VRScript.ShowAlert(tranDateErrorMessage)
                End If
            End If

            If isOkayToUpdate = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        'no save; would be Me.Quote.TransactionEffectiveDate if anything
                        'added 2/18/2020
                        RaiseEvent EffectiveDateChangedFromTree(QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, Me.bdpEffectiveDate.SelectedDate.ToShortDateString, Me.Quote.TransactionEffectiveDate)
                        Me.Quote.TransactionEffectiveDate = Me.bdpEffectiveDate.SelectedDate.ToShortDateString
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e)
                    Else
                        RaiseEvent EffectiveDateChanging(Me.bdpEffectiveDate.SelectedDate.ToShortDateString, Me.Quote.EffectiveDate)
                        RaiseEvent EffectiveDateChangedFromTree(QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, Me.bdpEffectiveDate.SelectedDate.ToShortDateString, Me.Quote.EffectiveDate)
                        Me.Quote.EffectiveDate = Me.bdpEffectiveDate.SelectedDate.ToShortDateString
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e)
                        'Added 10/20/2022 for task 77527 MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty) AndAlso CPR.CPR_InflationGuardHelper.InflationGuardNo2Enabled = "True" Then
                            RaiseEvent PopulateInflationGuard()
                        End If
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty) Then
                            If CPR.CPRRemovePropDedBelow1k.RemovePropDedBelow1kEnabled Then
                                RaiseEvent PopulateCPRCoverages()
                            End If
                            RaiseEvent PopulateCPRBuildingInformation()
                            If FunctionalReplacementCostHelper.FunctionalReplacementCostEnabled() Then
                                RaiseEvent RemoveFunctionalReplacementCost()
                            End If
                        End If
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                            RaiseEvent PopulateCAPCoverages()
                        End If
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                            If BOP.CyberCoverageHelper.CyberCoverageEnabled() = True Then
                                RaiseEvent PopulateBOPCoverages()
                            End If
                            If BOP.RemovePropDedBelow1k.RemovePropDedBelow1kEnabled() Then
                                RaiseEvent PopulateBOPBuildingInformation()
                            End If
                        End If
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal AndAlso DFR.PolicyDeductibleNotLessThan1k.PolicyDeductibleNotLessThan1kEnabled() = True Then
                            'to remove the 250 and 500 deductible limit options from the drop down when changing the date in the tree
                            RaiseEvent PopulateDFRCoverages()
                        End If
                    End If
                End If
                InEditMode = False
                Me.hdnTitleFlag_EffectiveDateHeader.Value = "on" 'added 2/25/2014
            Else
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 2/15/2019; original logic in ELSE
                        If QQHelper.IsValidDateString(Me.Quote.TransactionEffectiveDate) = True Then
                            Me.bdpEffectiveDate.SelectedDate = Me.Quote.TransactionEffectiveDate
                        Else
                            Me.bdpEffectiveDate.SelectedDate = Me.Quote.EffectiveDate
                        End If
                    Else
                        Me.bdpEffectiveDate.SelectedDate = Me.Quote.EffectiveDate
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnCancelSaveEffectiveDate_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnCancelSaveEffectiveDate.Click
        ResetQuoteDescriptionAndEffectiveDateToOriginalLabels()
        Me.EffectiveDateViewSection.Style.Add("display", "inline")
        Me.hdnEffectiveDateViewSect_Display.Value = "inline"
        Me.EffectiveDateEditSection.Style.Add("display", "none")
        Me.hdnEffectiveDateEditSect_Display.Value = "none"
        InEditMode = False
    End Sub

    Protected Sub imgBtnSaveFormType_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblLocationNumber") 'may be at different level since currIB is inside LI; this would be at the same level as parent UL
            Dim ddlFormType As DropDownList = currIB.Parent.FindControl("ddlFormType")
            'identify correct location and update or clear address accordingly
            If ddlFormType IsNot Nothing AndAlso lblLocationNumber IsNot Nothing AndAlso Me.Quote IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                If ddlFormType.SelectedValue = "" OrElse IsNumeric(ddlFormType.SelectedValue) = False Then
                    Me.VRScript.ShowAlert("please select a form type")
                    Page.SetFocus(ddlFormType)
                Else
                    Dim updatedLocationFormType As Boolean = False

                    With Me.Quote.Locations(CInt(lblLocationNumber.Text) - 1)
                        'may need to evaluate what form type currently is and what it's being changed to... in case only certain combinations are valid... or different things need to happen (i.e. wipe out all location info in this case but only wipe out section coverages or something else in another case)
                        .FormTypeId = ddlFormType.SelectedValue
                        updatedLocationFormType = True
                    End With

                    If updatedLocationFormType = True Then
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Residence)
                        InEditMode = False
                        'note: quoteDesc and effDate saves also set hdnTitleFlag value to "on"; not currently being used here
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub imgBtnAddPolicyholder_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsPolicyholderSectionEnabled, "You cannot access the Policyholders at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            AddPolicyholder()
        End If
    End Sub
    Protected Sub imgBtnEditPolicyholder_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsPolicyholderSectionEnabled, "You cannot access the Policyholders at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblPolicyholderNumber As Label = currIB.Parent.FindControl("lblPolicyholderNumber")

            If lblPolicyholderNumber IsNot Nothing AndAlso IsNumeric(lblPolicyholderNumber.Text) = True Then
                RaiseEvent EditPolicyholder(CInt(lblPolicyholderNumber.Text))
            End If
        End If
    End Sub
    Protected Sub imgBtnRemovePolicyholder_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'not being used yet
        If Me.CanNotAccessSection(IsPolicyholderSectionEnabled, "You cannot access the Policyholders at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblPolicyholderNumber As Label = currIB.Parent.FindControl("lblPolicyholderNumber")

            If lblPolicyholderNumber IsNot Nothing AndAlso IsNumeric(lblPolicyholderNumber.Text) = True Then
                If CInt(lblPolicyholderNumber.Text) = 1 OrElse CInt(lblPolicyholderNumber.Text) = 2 Then
                    If Me.Quote IsNot Nothing Then
                        If CInt(lblPolicyholderNumber.Text) = 2 Then
                            If Me.Quote.Policyholder2.HasData = False Then
                                Me.VRScript.ShowAlert("Policyholder 2 is already blank")
                            Else
                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Policyholders) 'updated 1/22/2014 to send new optional param
                            End If
                        Else
                            If Me.Quote.Policyholder.HasData = False Then
                                Me.VRScript.ShowAlert("Policyholder 1 is already blank")
                            Else
                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Policyholders) 'updated 1/22/2014 to send new optional param
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnPolicyholders_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnPolicyholders.Click
        If Me.CanNotAccessSection(IsPolicyholderSectionEnabled, "You cannot access the Policyholders at this time.") Then
            Exit Sub
        End If

        'If Me.lblNumberOfPolicyholders.Text <> "" AndAlso IsNumeric(Me.lblNumberOfPolicyholders.Text) = True AndAlso CInt(Me.lblNumberOfPolicyholders.Text) = 0 Then
        'updated 2/22/2019
        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso Me.lblNumberOfPolicyholders.Text <> "" AndAlso IsNumeric(Me.lblNumberOfPolicyholders.Text) = True AndAlso CInt(Me.lblNumberOfPolicyholders.Text) = 0 Then
            AddPolicyholder()
        Else
            RaiseEvent ShowPolicyholders(sender, e)
        End If
    End Sub
    Protected Sub imgBtnClearPolicyholder_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsPolicyholderSectionEnabled, "You cannot access the Policyholders at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblPolicyholderNumber As Label = currIB.Parent.FindControl("lblPolicyholderNumber")
            If lblPolicyholderNumber IsNot Nothing AndAlso IsNumeric(lblPolicyholderNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If (CInt(lblPolicyholderNumber.Text) = 1 AndAlso Me.Quote.Policyholder IsNot Nothing) OrElse (CInt(lblPolicyholderNumber.Text) = 2 AndAlso Me.Quote.Policyholder2 IsNot Nothing) Then
                        Dim qqPH As QuickQuotePolicyholder = Nothing
                        If CInt(lblPolicyholderNumber.Text) = 2 Then
                            'ph2
                            qqPH = Me.Quote.Policyholder2
                        Else
                            'ph1
                            qqPH = Me.Quote.Policyholder
                        End If
                        qqPH.Dispose()
                        qqPH = Nothing
                        qqPH = New QuickQuotePolicyholder

                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Policyholders) 'updated 1/22/2014 to send new optional param
                    End If
                End If
            End If
        End If
    End Sub


    Protected Sub imgBtnDrivers_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnDrivers.Click
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If

        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019
            Dim raiseNewEvent As Boolean = False
            If Me.lblNumberOfDrivers.Text <> "" AndAlso IsNumeric(Me.lblNumberOfDrivers.Text) = True Then
                If CInt(Me.lblNumberOfDrivers.Text) = 0 Then
                    'Updated 03/19/2021 for CAP Endorsements Task 52977 MLW
                    If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        raiseNewEvent = True
                    End If
                    'raiseNewEvent = True
                End If
                'ElseIf Me.Quote IsNot Nothing AndAlso (Me.GoverningStateQuote?.Drivers Is Nothing OrElse Me.GoverningStateQuote.Drivers.Any()) Then
                'updated 10/6/2018
            ElseIf Me.Quote IsNot Nothing AndAlso (Me.GoverningStateQuote?.Drivers Is Nothing OrElse Me.GoverningStateQuote.Drivers.Any() = False) Then
                'Updated 03/19/2021 for CAP Endorsements Task 52977 MLW
                If Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    raiseNewEvent = True
                End If
                'raiseNewEvent = True
            End If
            If raiseNewEvent = True Then
                RaiseNewDriverEvent()
                Exit Sub
            End If
        End If

        RaiseEvent ShowDrivers(sender, e)
    End Sub
    Protected Sub imgBtnAddDriver_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            'updated 2/4/2014
            RaiseNewDriverEvent()
        End If
    End Sub
    Protected Sub imgBtnEditDriver_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblDriverNumber As Label = currIB.Parent.FindControl("lblDriverNumber")

            If lblDriverNumber IsNot Nothing AndAlso IsNumeric(lblDriverNumber.Text) = True Then
                RaiseEvent EditDriver(CInt(lblDriverNumber.Text))
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveDriver_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblDriverNumber As Label = currIB.Parent.FindControl("lblDriverNumber")

            If lblDriverNumber IsNot Nothing AndAlso IsNumeric(lblDriverNumber.Text) = True Then
                If Me.GoverningStateQuote IsNot Nothing Then
                    If Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(lblDriverNumber.Text) Then
                        'see if any vehicles have driver assigned
                        If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                            For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                                If v.OccasionalDriver3Num <> "" AndAlso IsNumeric(v.OccasionalDriver3Num) = True AndAlso CInt(v.OccasionalDriver3Num) = CInt(lblDriverNumber.Text) Then
                                    v.OccasionalDriver3Num = ""
                                End If
                                If v.OccasionalDriver2Num <> "" AndAlso IsNumeric(v.OccasionalDriver2Num) = True AndAlso CInt(v.OccasionalDriver2Num) = CInt(lblDriverNumber.Text) Then
                                    v.OccasionalDriver2Num = ""
                                End If
                                If v.OccasionalDriver1Num <> "" AndAlso IsNumeric(v.OccasionalDriver1Num) = True AndAlso CInt(v.OccasionalDriver1Num) = CInt(lblDriverNumber.Text) Then
                                    v.OccasionalDriver1Num = ""
                                End If
                                If v.PrincipalDriverNum <> "" AndAlso IsNumeric(v.PrincipalDriverNum) = True AndAlso CInt(v.PrincipalDriverNum) = CInt(lblDriverNumber.Text) Then
                                    v.PrincipalDriverNum = ""
                                End If
                                MoveUpVehicleDrivers(v)
                            Next
                        End If

                        'Added 03/11/2021 for CAP Endorsements Task 52973 MLW
                        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                            Dim myDriver As QuickQuoteDriver = Nothing
                            Dim driverIndex As Integer = CInt(lblDriverNumber.Text) - 1
                            If GoverningStateQuote.Drivers.HasItemAtIndex(driverIndex) Then
                                myDriver = GoverningStateQuote.Drivers(driverIndex)
                            End If
                            If IsNewDriverOnEndorsement(myDriver) Then
                                ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, driverIndex, myDriver)
                            Else
                                ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, driverIndex, myDriver)
                            End If
                            Dim endorsementsRemarksHelper = New Helpers.EndorsementsRemarksHelper(ddh)
                            Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(Helpers.EndorsementsRemarksHelper.RemarksType.Driver)
                            Quote.TransactionRemark = updatedRemarks
                        End If

                        Me.GoverningStateQuote.Drivers.RemoveAt(CInt(lblDriverNumber.Text) - 1)

                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Drivers) 'updated 1/22/2014 to send new optional param
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveDriverAccidentViolation_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblDriverNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblDriverNumber") 'Parent.Parent.Parent needed; 1st parent: rptDriverAccidentViolations, 2nd parent: pnlDriverAccidentViolations, 3rd parent: rptDrivers (or repeater ItemTemplate)
            Dim lblDriverAccidentViolationNumber As Label = currIB.Parent.FindControl("lblDriverAccidentViolationNumber")

            If IsNumeric(lblDriverNumber.Text) = True AndAlso IsNumeric(lblDriverAccidentViolationNumber.Text) = True Then
                If Me.GoverningStateQuote?.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(lblDriverNumber.Text) Then
                    With Me.GoverningStateQuote.Drivers.Item(CInt(lblDriverNumber.Text) - 1)
                        If .AccidentViolations IsNot Nothing AndAlso .AccidentViolations.Count >= CInt(lblDriverAccidentViolationNumber.Text) Then
                            .AccidentViolations.RemoveAt(CInt(lblDriverAccidentViolationNumber.Text) - 1)
                            SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Drivers) 'updated 1/22/2014 to send new optional param
                        End If
                    End With
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveDriverLossHistory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsDriverSectionEnabled, "You cannot access the Drivers at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblDriverNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblDriverNumber") 'Parent.Parent.Parent needed; 1st parent: rptDriverLossHistories, 2nd parent: pnlDriverLossHistories, 3rd parent: rptDrivers (or repeater ItemTemplate)
            Dim lblDriverLossHistoryNumber As Label = currIB.Parent.FindControl("lblDriverLossHistoryNumber")

            If lblDriverNumber IsNot Nothing AndAlso IsNumeric(lblDriverNumber.Text) = True AndAlso lblDriverLossHistoryNumber IsNot Nothing AndAlso IsNumeric(lblDriverLossHistoryNumber.Text) = True Then
                If Me.GoverningStateQuote IsNot Nothing Then
                    If Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(lblDriverNumber.Text) Then
                        With Me.GoverningStateQuote.Drivers.Item(CInt(lblDriverNumber.Text) - 1)
                            If CountEvenIfNull(.LossHistoryRecords) >= CInt(lblDriverLossHistoryNumber.Text) Then
                                .LossHistoryRecords.RemoveAt(CInt(lblDriverLossHistoryNumber.Text) - 1)
                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Drivers) 'updated 1/22/2014 to send new optional param
                            End If
                        End With
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveLocationBuilding_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblLocationNumber") 'Parent.Parent.Parent needed; 1st parent: rptLocationBuildings, 2nd parent: pnlLocationBuildings, 3rd parent: rptLocations (or repeater ItemTemplate)
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblBuildingNumber")

            If IsNumeric(lblLocationNumber.Text) = True AndAlso IsNumeric(lblBuildingNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If CountEvenIfNull(Me.Quote?.Locations) >= CInt(lblLocationNumber.Text) Then
                        With Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1)
                            If CountEvenIfNull(.Buildings) >= CInt(lblBuildingNumber.Text) Then
                                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then ' Matt A 8-5-15
                                    IFM.VR.Common.Helpers.FARM.FarmBuildingHelper.RemoveFarmBuilding(Me.Quote, CInt(lblLocationNumber.Text) - 1, CInt(lblBuildingNumber.Text) - 1)
                                Else
                                    .Buildings.RemoveAt(CInt(lblBuildingNumber.Text) - 1)
                                End If

                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Locations) 'updated 1/22/2014 to send new optional param
                            End If
                        End With
                    End If
                End If
            End If
        End If
    End Sub


    Protected Sub imgBtnVehicles_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnVehicles.Click
        If Me.CanNotAccessSection(IsVehicleSectionEnabled, "You cannot access the Vehicles at this time.") Then
            Exit Sub
        End If

        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019
            'updated 2/4/2014
            Dim raiseNewEvent As Boolean = False
            If Me.lblNumberOfVehicles.Text <> "" AndAlso IsNumeric(Me.lblNumberOfVehicles.Text) = True Then
                If CInt(Me.lblNumberOfVehicles.Text) = 0 Then
                    'Updated 04/06/2021 for CAP Endorsements Task 52977 MLW
                    If Not IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        raiseNewEvent = True
                    End If
                End If
            ElseIf Me.Quote IsNot Nothing AndAlso (Me.Quote.Vehicles Is Nothing OrElse Me.Quote.Vehicles.Count = 0) Then
                'Updated 04/06/2021 for CAP Endorsements Task 52977 MLW
                If Not IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    raiseNewEvent = True
                End If
            End If
            If raiseNewEvent = True Then
                RaiseNewVehicleEvent()
                Exit Sub
            End If
        End If

        RaiseEvent ShowVehicles(sender, e)
    End Sub
    Protected Sub imgBtnAddVehicle_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsVehicleSectionEnabled, "You cannot access the Vehicles at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            'updated 2/4/2014
            RaiseNewVehicleEvent()
        End If
    End Sub
    Protected Sub imgBtnEditVehicle_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsVehicleSectionEnabled, "You cannot access the Vehicles at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblVehicleNumber As Label = currIB.Parent.FindControl("lblVehicleNumber")

            If lblVehicleNumber IsNot Nothing AndAlso IsNumeric(lblVehicleNumber.Text) = True Then
                RaiseEvent EditVehicle(CInt(lblVehicleNumber.Text))
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveVehicle_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsVehicleSectionEnabled, "You cannot access the Vehicles at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblVehicleNumber As Label = currIB.Parent.FindControl("lblVehicleNumber")

            If lblVehicleNumber IsNot Nothing AndAlso IsNumeric(lblVehicleNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count >= CInt(lblVehicleNumber.Text) Then
                        'Added 04/02/2021 for CAP Endorsements Task 52974 MLW
                        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                            Dim myVehicle As QuickQuoteVehicle
                            Dim vehicleIndex As Integer = CInt(lblVehicleNumber.Text) - 1
                            If Me.Quote.Vehicles.HasItemAtIndex(vehicleIndex) Then
                                myVehicle = Me.Quote.Vehicles(vehicleIndex)
                                If IsNewVehicleOnEndorsement(myVehicle) Then
                                    ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, vehicleIndex, myVehicle)
                                    Dim hasAddedVehicle As Boolean = False
                                    Dim vehicleList = ddh.GetVehicleDictionary
                                    For Each vehicle In vehicleList
                                        If vehicle.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                                            hasAddedVehicle = True
                                            Exit For
                                        End If
                                    Next
                                    If hasAddedVehicle = False Then
                                        Quote.TransactionReasonId = 10168 'Endorsement Change Dec Only
                                    End If
                                Else
                                    ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, vehicleIndex, myVehicle)
                                End If
                                Dim endorsementsRemarksHelper = New Helpers.EndorsementsRemarksHelper(ddh)
                                Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(Helpers.EndorsementsRemarksHelper.RemarksType.Vehicle)
                                Quote.TransactionRemark = updatedRemarks
                            End If
                        End If
                        Me.Quote.Vehicles.RemoveAt(CInt(lblVehicleNumber.Text) - 1)
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Vehicles) 'updated 1/22/2014 to send new optional param
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveVehicleDriver_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsVehicleSectionEnabled, "You cannot access the Vehicles at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblVehicleNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblVehicleNumber") 'Parent.Parent.Parent needed; 1st parent: rptVehicleDrivers, 2nd parent: pnlVehicleDrivers, 3rd parent: rptVehicles (or repeater ItemTemplate)
            Dim lblVehicleDriverIdentifier As Label = currIB.Parent.FindControl("lblVehicleDriverIdentifier")
            Dim lblDriverNumber As Label = currIB.Parent.FindControl("lblDriverNumber")

            If lblVehicleNumber IsNot Nothing AndAlso IsNumeric(lblVehicleNumber.Text) = True Then ' AndAlso lblDriverNumber IsNot Nothing AndAlso IsNumeric(lblDriverNumber.Text) = True Then'should be able to clear out text regardless if value is number or not
                If Me.Quote?.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count >= CInt(lblVehicleNumber.Text) Then
                    If lblVehicleDriverIdentifier IsNot Nothing AndAlso lblVehicleDriverIdentifier.Text <> "" Then
                        Dim wasUpdated As Boolean = False
                        With Me.Quote.Vehicles.Item(CInt(lblVehicleNumber.Text) - 1)
                            Select Case UCase(lblVehicleDriverIdentifier.Text)
                                Case UCase("PrincipalDriver")
                                    If lblDriverNumber IsNot Nothing AndAlso lblDriverNumber.Text = .PrincipalDriverNum Then 'can verify that text is the same; not mandatory
                                        wasUpdated = True
                                        .PrincipalDriverNum = ""
                                    End If
                                Case UCase("OccasionalDriver1")
                                    If lblDriverNumber IsNot Nothing AndAlso lblDriverNumber.Text = .OccasionalDriver1Num Then 'can verify that text is the same; not mandatory
                                        wasUpdated = True
                                        .OccasionalDriver1Num = ""
                                    End If
                                Case UCase("OccasionalDriver2")
                                    If lblDriverNumber IsNot Nothing AndAlso lblDriverNumber.Text = .OccasionalDriver2Num Then 'can verify that text is the same; not mandatory
                                        wasUpdated = True
                                        .OccasionalDriver2Num = ""
                                    End If
                                Case UCase("OccasionalDriver3")
                                    If lblDriverNumber IsNot Nothing AndAlso lblDriverNumber.Text = .OccasionalDriver3Num Then 'can verify that text is the same; not mandatory
                                        wasUpdated = True
                                        .OccasionalDriver3Num = ""
                                    End If
                            End Select
                        End With
                        If wasUpdated = True Then
                            MoveUpVehicleDrivers(Me.Quote.Vehicles.Item(CInt(lblVehicleNumber.Text) - 1))

                            SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Vehicles) 'updated 1/22/2014 to send new optional param
                        End If
                    End If
                End If
            End If
        End If
    End Sub


    Protected Sub imgBtnLocations_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnLocations.Click
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If

        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019
            'updated 2/4/2014
            Dim raiseNewEvent As Boolean = False
            If Me.lblNumberOfLocations.Text <> "" AndAlso IsNumeric(Me.lblNumberOfLocations.Text) = True Then
                If CInt(Me.lblNumberOfLocations.Text) = 0 Then
                    raiseNewEvent = True
                End If
            ElseIf Me.Quote IsNot Nothing AndAlso (Me.Quote.Locations Is Nothing OrElse Me.Quote.Locations.Any()) Then
                raiseNewEvent = True
            End If
            If raiseNewEvent = True Then
                RaiseNewLocationEvent()
                Exit Sub
            End If
        End If

        RaiseEvent ShowLocations(sender, e)
    End Sub
    Protected Sub imgBtnAddLocation_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            'updated 2/4/2014
            RaiseNewLocationEvent()
        End If
    End Sub
    Protected Sub imgBtnEditLocation_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblLocationNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True Then
                RaiseEvent EditLocation(CInt(lblLocationNumber.Text))
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveLocation_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblLocationNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        If CInt(lblLocationNumber.Text) = 1 Then 'added IF/ELSE 7/20/2015; original logic in ELSE
                            Me.VRScript.ShowAlert("You cannot remove the primary Location.")
                            Exit Sub
                        Else
                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then ' Matt A 8-5-15
                                IFM.VR.Common.Helpers.FARM.FarmBuildingHelper.RemoveFarmLocation(Me.Quote, CInt(lblLocationNumber.Text) - 1)
                            Else
                                Me.Quote.Locations.RemoveAt(CInt(lblLocationNumber.Text) - 1)
                            End If
                            SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Locations) 'updated 1/22/2014 to send new optional param
                        End If
                    End If
                End If
            End If
        End If
    End Sub



    Protected Sub imgBtnRemoveLocationDwelling_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'added 7/21/2015 for Farm dwellings (location w/ valid form type)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblLocationNumber") 'Parent.Parent.Parent needed; 1st parent: rptLocationDwellings, 2nd parent: pnlLocationDwellings, 3rd parent: rptLocations (or repeater ItemTemplate)
            Dim lblDwellingNumber As Label = currIB.Parent.FindControl("lblDwellingNumber") 'this should always match LocationNumber

            If IsNumeric(lblLocationNumber.Text) = True AndAlso IsNumeric(lblDwellingNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        If CInt(lblLocationNumber.Text) = 1 AndAlso CInt(lblDwellingNumber.Text) = 1 Then 'added IF/ELSE 7/21/2015; original logic in ELSE
                            Me.VRScript.ShowAlert("You cannot remove the primary location dwelling.")
                            Exit Sub
                        Else
                            RaiseEvent ClearDwelling(CInt(lblLocationNumber.Text))
                        End If
                    End If
                End If
            End If
        End If
    End Sub




    Protected Sub imgBtnCoverages_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnCoverages.Click
        If Me.CanNotAccessSection(IsCoverageSectionEnabled, "You cannot access the Coverages at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowCoverages(sender, e)
    End Sub
    Protected Sub imgBtnRemoveCoverage_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsCoverageSectionEnabled, "You cannot access the Coverages at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblCoverageDescription As Label = currIB.Parent.FindControl("lblCoverageDescription")

            If lblCoverageDescription IsNot Nothing Then
                If Me.Quote IsNot Nothing Then
                    Dim foundCoverage As Boolean = False
                    'identify correct coverage and update or clear value accordingly
                    Select Case UCase(lblCoverageDescription.Text)
                        Case "AUTO ENHANCEMENT"
                            foundCoverage = True
                            Me.GoverningStateQuote.HasBusinessMasterEnhancement = False
                        Case "SELECT MARKET CREDIT"
                            foundCoverage = True
                            Me.GoverningStateQuote.SelectMarketCredit = False
                        Case "MULTI POLICY DISCOUNT"
                            foundCoverage = True
                            Me.GoverningStateQuote.AutoHome = False
                        Case "EMPLOYEE DISCOUNT"
                            foundCoverage = True
                            Me.GoverningStateQuote.EmployeeDiscount = False
                        Case "FACULTATIVE"
                            foundCoverage = True
                            Me.GoverningStateQuote.FacultativeReinsurance = False
                    End Select

                    If foundCoverage = True Then
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Coverages) 'updated 1/22/2014 to send new optional param
                    End If
                End If
            End If
        End If
    End Sub


    Protected Sub imgBtnViewMvrReport_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsMvrReportsSectionEnabled, "You cannot access the MVR Reports at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblMvrReportEntityType As Label = currIB.Parent.FindControl("lblMvrReportEntityType")
            Dim lblMvrReportEntityNumber As Label = currIB.Parent.FindControl("lblMvrReportEntityNumber") 'QQ sequential num
            Dim lblMvrReportUnitNumber As Label = currIB.Parent.FindControl("lblMvrReportUnitNumber") 'Diamond num

            If lblMvrReportEntityType IsNot Nothing AndAlso lblMvrReportEntityNumber IsNot Nothing AndAlso IsNumeric(lblMvrReportEntityNumber.Text) = True Then
                If UCase(lblMvrReportEntityType.Text) = "DRIVER" Then
                    RaiseEvent ViewDriverMvrReport(CInt(lblMvrReportEntityNumber.Text))
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnViewClueReport_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsClueReportsSectionEnabled, "You cannot access the Clue Reports at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblClueReportEntityType As Label = currIB.Parent.FindControl("lblClueReportEntityType")
            Dim lblClueReportUnitNumber As Label = currIB.Parent.FindControl("lblClueReportUnitNumber") 'Diamond num; not currently being used

            If lblClueReportEntityType IsNot Nothing Then
                If UCase(lblClueReportEntityType.Text) = "AUTO" Then
                    RaiseEvent ViewClueAutoReport(sender, e)
                ElseIf UCase(lblClueReportEntityType.Text) = "PROPERTY" Then 'added 9/25/2014 for HOM
                    RaiseEvent ViewCluePropertyReport(sender, e)
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnViewCreditReport_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsCreditReportsSectionEnabled, "You cannot access the Credit/Tiering at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblCreditReportEntityType As Label = currIB.Parent.FindControl("lblCreditReportEntityType")
            Dim lblCreditReportEntityNumber As Label = currIB.Parent.FindControl("lblCreditReportEntityNumber") 'QQ sequential num
            Dim lblCreditReportUnitNumber As Label = currIB.Parent.FindControl("lblCreditReportUnitNumber") 'Diamond num

            If lblCreditReportEntityType IsNot Nothing AndAlso lblCreditReportEntityNumber IsNot Nothing AndAlso IsNumeric(lblCreditReportEntityNumber.Text) = True Then
                If UCase(lblCreditReportEntityType.Text) = "DRIVER" Then
                    RaiseEvent ViewDriverCreditReport(CInt(lblCreditReportEntityNumber.Text))
                ElseIf UCase(lblCreditReportEntityType.Text) = "APPLICANT" Then 'added 9/25/2014 for HOM
                    RaiseEvent ViewApplicantCreditReport(CInt(lblCreditReportEntityNumber.Text))
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnViewVeriskReport_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsResidenceSectionEnabled, "You cannot access the Verisk Reports at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber_VeriskReport As Label = currIB.Parent.FindControl("lblLocationNumber_VeriskReport")

            If lblLocationNumber_VeriskReport IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(lblLocationNumber_VeriskReport.Text) = True Then
                RaiseEvent ViewVeriskProtectionClassReport(CInt(lblLocationNumber_VeriskReport.Text))
            Else
                RaiseEvent ViewVeriskProtectionClassReport(1)
            End If
        End If
    End Sub


    Private Sub imgBtnQuoteSummary_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnQuoteSummary.Click
        If Me.CanNotAccessSection(IsQuoteSummarySectionEnabled, "You cannot access the Quote Summary at this time.") Then
            Exit Sub
        End If

        RaiseEvent ShowQuoteSummary(sender, e)
    End Sub

    Protected Sub imgBtnUnderwritingQuestions_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles imgBtnUnderwritingQuestions.Click
        If Me.CanNotAccessSection(IsUnderwritingQuestionSectionEnabled, "You cannot access the Underwriting Questions at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowUnderwritingQuestions(sender, e)
    End Sub

    Private Sub imgBtnApplication_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnApplication.Click
        If Me.CanNotAccessSection(IsApplicationSectionEnabled, "You cannot access the Application at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowApplication(sender, e)
    End Sub

    Private Sub imgBtnApplicationSummary_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnApplicationSummary.Click
        If Me.CanNotAccessSection(IsApplicationSummarySectionEnabled, "You cannot access the Application Summary at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowApplicationSummary(sender, e)
    End Sub

    Private Sub imgBtnFileUpload_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnFileUpload.Click 'Matt A 12-3-2015
        If Me.CanNotAccessSection(IsFileUploadSectionEnabled, "You cannot access the File Upload at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowFileUpload(sender, e)
    End Sub



    Private Sub imgBtnResidence_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnResidence.Click
        If Me.CanNotAccessSection(IsResidenceSectionEnabled, "You cannot access the Property at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowResidence(sender, e)
    End Sub
    Protected Sub imgBtnClearResidence_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsResidenceSectionEnabled, "You cannot access the Property at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblResidenceDescription As Label = currIB.Parent.FindControl("lblResidenceDescription")
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblLocationNumber")

            'identify correct location and update or clear address accordingly
            If lblLocationNumber IsNot Nothing AndAlso Me.Quote IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                Dim updatedLocationAddress As Boolean = False

                With Me.Quote.Locations(CInt(lblLocationNumber.Text) - 1) '1/7/2015 - corrected index to use number - 1
                    If .Address IsNot Nothing AndAlso .Address.HasData = True Then 'may not use .HasData here
                        .Address.Dispose()
                        .Address = Nothing
                        .Address = New QuickQuoteAddress
                        updatedLocationAddress = True 'added 1/7/2015
                    End If
                End With

                If updatedLocationAddress = True Then
                    SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Residence)
                End If
            End If
        End If
    End Sub



    Protected Sub imgBtnEditLocationBuilding_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If

        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblLocationNumber") 'Parent.Parent.Parent needed; 1st parent: rptLocationBuildings, 2nd parent: pnlLocationBuildings, 3rd parent: rptLocations (or repeater ItemTemplate)
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblBuildingNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso lblBuildingNumber IsNot Nothing AndAlso IsNumeric(lblBuildingNumber.Text) = True Then
                RaiseEvent EditLocationBuilding(CInt(lblLocationNumber.Text), CInt(lblBuildingNumber.Text))
            End If
        End If
    End Sub
    Protected Sub imgBtnEditLocationDwelling_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsLocationSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblLocationNumber") 'Parent.Parent.Parent needed; 1st parent: rptLocationDwellings, 2nd parent: pnlLocationDwellings, 3rd parent: rptLocations (or repeater ItemTemplate)
            Dim lblDwellingNumber As Label = currIB.Parent.FindControl("lblDwellingNumber") 'same as LocationNumber

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso lblDwellingNumber IsNot Nothing AndAlso IsNumeric(lblDwellingNumber.Text) = True Then
                RaiseEvent EditLocationDwelling(CInt(lblLocationNumber.Text))
            End If
        End If
    End Sub

    'added 7/27/2015 for Farm
    Private Sub imgBtnPolicyLevelCoverages_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnPolicyLevelCoverages.Click
        If Me.CanNotAccessSection(IsPolicyLevelCoverageSectionEnabled, "You cannot access the Policy Level Coverages at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowPolicyLevelCoverages(sender, e)
    End Sub
    Private Sub imgBtnFarmPersonalProperty_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnFarmPersonalProperty.Click
        If Me.CanNotAccessSection(IsFarmPersonalPropertySectionEnabled, "You cannot access the Personal Property Coverages at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowFarmPersonalProperty(sender, e)
    End Sub
    Private Sub imgBtnInlandMarineAndRvWatercraft_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnInlandMarineAndRvWatercraft.Click
        If Me.CanNotAccessSection(IsInlandMarineAndRvWatercraftSectionEnabled, "You cannot access the Inland Marine or Rv/Watercraft at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowInlandMarineAndRvWatercraft(sender, e)
    End Sub


    Private Sub imgBtnIRPM_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnIRPM.Click
        If Me.CanNotAccessSection(IsIRPMSectionEnabled, "You cannot access the IRPM at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowIRPM(sender, e)
    End Sub

    Private Sub imgBtnCPPCPRCovs_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnCPPCPRCovs.Click, imgBtnCPPCPRHeaderClick.Click
        If Me.CanNotAccessSection(IsCPPCPRPolicyLevelCoverageSectionEnabled, "You cannot access the Policy Level Coverages at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowCPPCPRCoverages()
    End Sub


    Private Sub imgBtnCPPCPRLocations_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnCPPCPRLocations.Click
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If

        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019
            Dim raiseNewEvent As Boolean = False
            If Me.lblCPPCPRNumberOfLocations.Text <> "" AndAlso IsNumeric(Me.lblCPPCPRNumberOfLocations.Text) Then
                If CInt(Me.lblCPPCPRNumberOfLocations.Text) = 0 Then
                    raiseNewEvent = True
                End If
            ElseIf Me.Quote IsNot Nothing AndAlso (Me.Quote.Locations Is Nothing OrElse Me.Quote.Locations.Count = 0) Then
                raiseNewEvent = True
            End If
        End If

        RaiseEvent ShowCPPCPRLocations(0)
    End Sub
    Protected Sub imgBtnRemoveCPPCPRLocationBuilding_Click1(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblCPPCPRLocationNumber")
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblCPPCPRBuildingNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso lblBuildingNumber IsNot Nothing AndAlso IsNumeric(lblBuildingNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        With Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1)
                            If .Buildings IsNot Nothing AndAlso .Buildings.Count >= CInt(lblBuildingNumber.Text) Then
                                .Buildings.RemoveAt(CInt(lblBuildingNumber.Text) - 1)
                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.CPP_CPR_Locations)
                            End If
                        End With
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnEditCPPCPRLocation_Click1(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblCPPCPRLocationNumber")
            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) Then
                RaiseEvent EditCPPCPRLocation(CInt(lblLocationNumber.Text) - 1)
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveCPPCPRLocation_Click1(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblCPPCPRLocationNumber")
            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        If CInt(lblLocationNumber.Text) = 1 Then
                            Me.VRScript.ShowAlert("You cannot remove the primary Location.")
                            Exit Sub
                        Else
                            Me.Quote.Locations.RemoveAt(CInt(lblLocationNumber.Text) - 1)
                            SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.CPP_CPR_Locations)
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnAddCPPCPRLocation_Click1(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            RaiseNewLocationEvent()
        End If
    End Sub
    Protected Sub imgBtnEditCPPCPRLocationBuilding_Click1(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCPRLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblCPPCPRLocationNumber")
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblCPPCPRBuildingNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) AndAlso lblBuildingNumber IsNot Nothing AndAlso IsNumeric(lblBuildingNumber.Text) Then
                RaiseEvent EditCPPCPRLocationBuilding(CInt(lblLocationNumber.Text) - 1, CInt(lblBuildingNumber.Text) - 1)
            End If
        End If
    End Sub
    Protected Sub imgBtnAddCPPCGLLocation_Click(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            RaiseNewLocationEvent()
        End If
    End Sub
    Protected Sub imgBtnRemoveCPPCGLLocation_Click(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblCPPCGLLocationNumber")
            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        If CInt(lblLocationNumber.Text) = 1 Then
                            Me.VRScript.ShowAlert("You cannot remove the primary Location.")
                            Exit Sub
                        Else
                            Me.Quote.Locations.RemoveAt(CInt(lblLocationNumber.Text) - 1)
                            SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.CPP_CGL_Locations)
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnEditCPPCGLLocation_Click(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.FindControl("lblCPPCGLLocationNumber")
            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) Then
                RaiseEvent EditCPPCGLLocation(CInt(lblLocationNumber.Text) - 1)
            End If
        End If
    End Sub
    Protected Sub imgBtnRemoveCPPCGLLocationBuilding_Click(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblCPPCGLLocationNumber")
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblCPPCGLBuildingNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso lblBuildingNumber IsNot Nothing AndAlso IsNumeric(lblBuildingNumber.Text) = True Then
                If Me.Quote IsNot Nothing Then
                    If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                        With Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1)
                            If .Buildings IsNot Nothing AndAlso .Buildings.Count >= CInt(lblBuildingNumber.Text) Then
                                .Buildings.RemoveAt(CInt(lblBuildingNumber.Text) - 1)
                                SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.CPP_CGL_Locations)
                            End If
                        End With
                    End If
                End If
            End If
        End If
    End Sub
    Protected Sub imgBtnEditCPPCGLLocationBuilding_Click(sender As Object, e As ImageClickEventArgs)
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblLocationNumber As Label = currIB.Parent.Parent.Parent.FindControl("lblCPPCGLLocationNumber")
            Dim lblBuildingNumber As Label = currIB.Parent.FindControl("lblCPPCGLBuildingNumber")

            If lblLocationNumber IsNot Nothing AndAlso IsNumeric(lblLocationNumber.Text) AndAlso lblBuildingNumber IsNot Nothing AndAlso IsNumeric(lblBuildingNumber.Text) Then
                RaiseEvent EditCPPCGLLocationBuilding(CInt(lblLocationNumber.Text) - 1, CInt(lblBuildingNumber.Text) - 1)
            End If
        End If
    End Sub
    Private Sub imgBtnCPPCGLCovs_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnCPPCGLCovs.Click, imgBtnCPPCGLHeaderClick.Click
        If Me.CanNotAccessSection(IsCPPCGLPolicyLevelCoverageSectionEnabled, "You cannot access the Policy Level Coverages at this time.") Then
            Exit Sub
        End If
        If InEditMode = True Then
            Me.VRScript.ShowAlert("This functionality is currently locked.")
            Exit Sub
        End If
        If IsCPPCGLPolicyLevelCoverageSectionEnabled = False Then
            Me.VRScript.ShowAlert("You cannot access Policy Level Coverages at this time.")
            Exit Sub
        End If

        RaiseEvent ShowCPPCGLCoverages()
    End Sub
    Private Sub imgBtnCPPCGLLocations_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnCPPCGLLocations.Click
        If Me.CanNotAccessSection(IsCPPCGLLocationsSectionEnabled, "You cannot access the Locations at this time.") Then
            Exit Sub
        End If

        If GetQuoteTransactionTypeFlagForTree() <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019
            Dim raiseNewEvent As Boolean = False
            If Me.lblCPPCGLNumberOfLocations.Text <> "" AndAlso IsNumeric(Me.lblCPPCGLNumberOfLocations.Text) Then
                If CInt(Me.lblCPPCGLNumberOfLocations.Text) = 0 Then
                    raiseNewEvent = True
                End If
            ElseIf Me.Quote IsNot Nothing AndAlso (Me.Quote.Locations Is Nothing OrElse Me.Quote.Locations.Count = 0) Then
                raiseNewEvent = True
            End If
        End If

        RaiseEvent ShowCPPCGLLocations(0)
    End Sub

    Private Sub imgBtnCrime_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnCrime.Click
        If InEditMode = True Then
            Me.VRScript.ShowAlert("This functionality is currently locked.")
            Exit Sub
        End If

        RaiseEvent ShowCrime()
    End Sub

    Private Sub imgBtnInlandMarine_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnInlandMarine.Click
        If InEditMode = True Then
            Me.VRScript.ShowAlert("This functionality is currently locked.")
            Exit Sub
        End If

        RaiseEvent ShowInlandMarine()
    End Sub

    ''' <summary>
    ''' CPP Delete Inland Marine button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnDeleteIM_Click(sender As Object, e As EventArgs)
        RaiseEvent DeleteCPP_InlandMarine()
    End Sub

    ''' <summary>
    ''' CPP Delete Crime button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnDeleteCrime_Click(sender As Object, e As EventArgs)
        RaiseEvent DeleteCPP_Crime()
    End Sub
    Protected Sub imgBtnRemovePropertyLossHistory_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Me.CanNotAccessSection(IsResidenceSectionEnabled, "You cannot access the Residence at this time.") Then
            Exit Sub
        End If
        If TypeOf sender Is ImageButton Then
            Dim currIB As ImageButton = CType(sender, ImageButton)
            Dim lblPropertyLossHistoryCounter As Label = currIB.Parent.FindControl("lblPropertyLossHistoryCounter") 'incremental count for entire list
            Dim lblPropertyLossHistoryLevel As Label = currIB.Parent.FindControl("lblPropertyLossHistoryLevel") 'Policy or Applicant
            Dim lblPropertyLossHistoryLevelNumber As Label = currIB.Parent.FindControl("lblPropertyLossHistoryLevelNumber") '0 for Policy or Applicant Number
            Dim lblPropertyLossHistoryLevelCounter As Label = currIB.Parent.FindControl("lblPropertyLossHistoryLevelCounter") 'incremental count for level

            If lblPropertyLossHistoryLevel IsNot Nothing AndAlso lblPropertyLossHistoryLevelNumber IsNot Nothing AndAlso IsNumeric(lblPropertyLossHistoryLevelNumber.Text) = True AndAlso lblPropertyLossHistoryLevelCounter IsNot Nothing AndAlso IsNumeric(lblPropertyLossHistoryLevelCounter.Text) = True Then
                If Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote.HasLossHistories = True Then 'may not need to check HasLossHistories property
                    Dim okayToSave As Boolean = False
                    If UCase(lblPropertyLossHistoryLevel.Text) = "POLICY" Then
                        If Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count >= CInt(lblPropertyLossHistoryLevelCounter.Text) Then
                            Me.GoverningStateQuote.LossHistoryRecords.RemoveAt(CInt(lblPropertyLossHistoryLevelCounter.Text) - 1)
                            okayToSave = True
                        End If
                    ElseIf UCase(lblPropertyLossHistoryLevel.Text) = "APPLICANT" Then
                        If Me.GoverningStateQuote.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Count >= CInt(lblPropertyLossHistoryLevelNumber.Text) Then
                            With Me.GoverningStateQuote.Applicants.Item(CInt(lblPropertyLossHistoryLevelNumber.Text) - 1)
                                If .LossHistoryRecords IsNot Nothing AndAlso .LossHistoryRecords.Count >= CInt(lblPropertyLossHistoryLevelCounter.Text) Then
                                    .LossHistoryRecords.RemoveAt(CInt(lblPropertyLossHistoryLevelCounter.Text) - 1)
                                    okayToSave = True
                                End If
                            End With
                        End If
                    End If
                    If okayToSave = True Then
                        SaveCurrentQuoteObjectAndRaiseUpdatedEvent(sender, e, TreeViewSection.Residence) 'updated 1/22/2014 to send new optional param
                    End If
                End If
            End If
        End If
    End Sub

#End Region

    Private Sub ShowSection(ByVal sender As Object, ByVal e As System.EventArgs, ByVal sectionToShow As TreeViewSection)
        If sectionToShow <> Nothing AndAlso sectionToShow <> TreeViewSection.None Then
            Select Case sectionToShow
                Case TreeViewSection.Policyholders
                    RaiseEvent ShowPolicyholders(sender, e)
                Case TreeViewSection.Drivers
                    RaiseEvent ShowDrivers(sender, e)
                Case TreeViewSection.Vehicles
                    RaiseEvent ShowVehicles(sender, e)
                Case TreeViewSection.Locations
                    RaiseEvent ShowLocations(sender, e)
                Case TreeViewSection.Coverages
                    RaiseEvent ShowCoverages(sender, e)
                Case TreeViewSection.UnderwritingQuestions
                    RaiseEvent ShowUnderwritingQuestions(sender, e)
                Case TreeViewSection.QuoteSummary
                    RaiseEvent ShowQuoteSummary(sender, e)
                Case TreeViewSection.Discounts
                    RaiseEvent ShowDiscounts(sender, e)
                Case TreeViewSection.Surcharges
                    RaiseEvent ShowSurcharges(sender, e)
                Case TreeViewSection.Application
                    RaiseEvent ShowApplication(sender, e)
                Case TreeViewSection.ApplicationSummary
                    RaiseEvent ShowApplicationSummary(sender, e)
                Case TreeViewSection.Residence 'added 6/12/2014
                    RaiseEvent ShowResidence(sender, e)
                Case TreeViewSection.PolicyLevelCoverages 'added 7/27/2015 for Farm
                    RaiseEvent ShowPolicyLevelCoverages(sender, e)
                Case TreeViewSection.FarmPersonalProperty 'added 7/27/2015 for Farm
                    RaiseEvent ShowFarmPersonalProperty(sender, e)
                Case TreeViewSection.InlandMarineAndRvWatercraft 'added 7/27/2015 for Farm
                    RaiseEvent ShowInlandMarineAndRvWatercraft(sender, e)
                Case TreeViewSection.IRPM 'added 7/27/2015 for Farm
                    RaiseEvent ShowIRPM(sender, e)
                Case TreeViewSection.CPP_CPR_Coverages
                    RaiseEvent ShowCPPCPRCoverages()
                Case TreeViewSection.CPP_CPR_Locations
                    RaiseEvent ShowCPPCPRLocations(0)
                Case TreeViewSection.CPP_CGL_Coverages
                    RaiseEvent ShowCPPCGLCoverages()
                Case TreeViewSection.CPP_CGL_Locations
                    RaiseEvent ShowCPPCGLLocations(0)
                Case TreeViewSection.PrintHistory 'added 6/13/2019 for Endorsements/ReadOnly
                    RaiseEvent ShowPrintHistory(sender, e)
                Case TreeViewSection.PolicyHistory 'added 6/13/2019 for Endorsements/ReadOnly
                    RaiseEvent ShowPolicyHistory(sender, e)
                Case TreeViewSection.BillingInformation 'added 6/13/2019 for Endorsements/ReadOnly
                    RaiseEvent ShowBillingInformation(sender, e)
            End Select
        End If
    End Sub

#Region "Repeater Item Binding"
    Private Sub rptPolicyholders_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPolicyholders.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim imgBtnClearPolicyholder As ImageButton = e.Item.FindControl("imgBtnClearPolicyholder")
            Dim PolicyholderSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("PolicyholderSubLists_expandCollapseImageArea")
            Dim lblPolicyholderDescription As Label = e.Item.FindControl("lblPolicyholderDescription")
            Dim xSpanClearPolicyholder As HtmlGenericControl = e.Item.FindControl("xSpanClearPolicyholder")

            Dim lblApplicant As Label = e.Item.FindControl("lblApplicant")
            Dim lblApplicantNumber As Label = e.Item.FindControl("lblApplicantNumber")
            Dim ulPolicyholderSubItems As HtmlGenericControl = e.Item.FindControl("ulPolicyholderSubItems")
            Dim liPolicyholderSubItem_Applicant As HtmlGenericControl = e.Item.FindControl("liPolicyholderSubItem_Applicant")

            Dim lblApplicant2 As Label = e.Item.FindControl("lblApplicant2")
            Dim lblApplicant2Number As Label = e.Item.FindControl("lblApplicant2Number")
            Dim liPolicyholderSubItem_Applicant2 As HtmlGenericControl = e.Item.FindControl("liPolicyholderSubItem_Applicant2")

            If imgBtnClearPolicyholder IsNot Nothing Then
                imgBtnClearPolicyholder.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to clear this policyholder?');}")

                'added 2/22/2019
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnClearPolicyholder, "visibility", "hidden")
                End If
            End If

            PolicyholderSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblPolicyholderDescription IsNot Nothing AndAlso lblPolicyholderDescription.Text <> "" Then
                If lblPolicyholderDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblPolicyholderDescription.ToolTip, $" ({lblPolicyholderDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblPolicyholderDescription.ToolTip &= $" ({lblPolicyholderDescription.Text})"
                    End If
                Else
                    lblPolicyholderDescription.ToolTip = lblPolicyholderDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblPolicyholderDescription.ToolTip) = False AndAlso lblPolicyholderDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblPolicyholderDescription.ToolTip = lblPolicyholderDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditPolicyholder since it's always hidden
                End If
                If imgBtnClearPolicyholder IsNot Nothing Then
                    If imgBtnClearPolicyholder.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnClearPolicyholder.ToolTip, $" ({lblPolicyholderDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnClearPolicyholder.ToolTip &= $" ({lblPolicyholderDescription.Text})"
                        End If
                    Else
                        imgBtnClearPolicyholder.ToolTip = $"Clear {lblPolicyholderDescription.Text}"
                    End If
                End If
                If xSpanClearPolicyholder IsNot Nothing Then
                    If xSpanClearPolicyholder.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanClearPolicyholder.Attributes.Item("title"), $" ({lblPolicyholderDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanClearPolicyholder.Attributes.Item("title") &= $" ({lblPolicyholderDescription.Text})"
                        End If
                    Else
                        xSpanClearPolicyholder.Attributes.Item("title") = $"Clear {lblPolicyholderDescription.Text}"
                    End If
                End If
            End If

            'added 2/22/2019
            If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If xSpanClearPolicyholder IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanClearPolicyholder, "visibility", "hidden")
                End If
            End If

            If lblApplicant IsNot Nothing AndAlso ulPolicyholderSubItems IsNot Nothing AndAlso liPolicyholderSubItem_Applicant IsNot Nothing AndAlso lblApplicant.Text <> "" Then
                ulPolicyholderSubItems.Visible = True
                liPolicyholderSubItem_Applicant.Visible = True
                If lblApplicant2 IsNot Nothing AndAlso liPolicyholderSubItem_Applicant2 IsNot Nothing AndAlso lblApplicant2.Text <> "" Then 'added 6/8/2015
                    liPolicyholderSubItem_Applicant2.Visible = True
                Else
                    If liPolicyholderSubItem_Applicant2 IsNot Nothing Then 'added 6/8/2015; could even be wrapped inside ul IF since this is a child... or could simply be omitted
                        liPolicyholderSubItem_Applicant2.Visible = False
                    End If
                End If
            Else
                If ulPolicyholderSubItems IsNot Nothing Then
                    ulPolicyholderSubItems.Visible = False
                End If
                If liPolicyholderSubItem_Applicant IsNot Nothing Then 'could even be wrapped inside ul IF since this is a child... or could simply be omitted
                    liPolicyholderSubItem_Applicant.Visible = False
                End If
                If liPolicyholderSubItem_Applicant2 IsNot Nothing Then 'added 6/8/2015; could even be wrapped inside ul IF since this is a child... or could simply be omitted
                    liPolicyholderSubItem_Applicant2.Visible = False
                End If
            End If

        End If
    End Sub
    Protected Sub rptDrivers_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDrivers.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim imgBtnRemoveDriver As ImageButton = e.Item.FindControl("imgBtnRemoveDriver")
            If imgBtnRemoveDriver IsNot Nothing Then
                imgBtnRemoveDriver.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this driver?');}")

                'Updated 03/15/2021 for CAP Endorsements Task 52977 MLW     
                Dim endorsementDriverTransactionCount As Integer = 0
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso TypeOfEndorsement() = "Add/Delete Driver" Then
                    endorsementDriverTransactionCount = ddh.GetEndorsementTransactionCount()
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso endorsementDriverTransactionCount >= 3) Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveDriver, "visibility", "hidden")
                End If
                ''added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveDriver, "visibility", "hidden")
                'End If
            End If

            Dim lblDriverNumber As Label = e.Item.FindControl("lblDriverNumber")
            Dim pnlDriverAccidentViolations As Panel = e.Item.FindControl("pnlDriverAccidentViolations")
            Dim rptDriverAccidentViolations As Repeater = e.Item.FindControl("rptDriverAccidentViolations")
            Dim DriverSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("DriverSubLists_expandCollapseImageArea")
            Dim pnlDriverLossHistories As Panel = e.Item.FindControl("pnlDriverLossHistories")
            Dim rptDriverLossHistories As Repeater = e.Item.FindControl("rptDriverLossHistories")
            Dim lblDriverDescription As Label = e.Item.FindControl("lblDriverDescription")
            Dim xSpanRemoveDriver As HtmlGenericControl = e.Item.FindControl("xSpanRemoveDriver")

            pnlDriverAccidentViolations.Visible = False
            DriverSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
            pnlDriverLossHistories.Visible = False

            'added 3/18/2014
            If Not String.IsNullOrWhiteSpace(lblDriverDescription?.Text) Then
                If lblDriverDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblDriverDescription.ToolTip, $" ({lblDriverDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblDriverDescription.ToolTip &= $" ({lblDriverDescription.Text})"
                    End If
                Else
                    lblDriverDescription.ToolTip = lblDriverDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblDriverDescription.ToolTip) = False AndAlso lblDriverDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblDriverDescription.ToolTip = lblDriverDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditDriver since it's always hidden
                End If
                If imgBtnRemoveDriver IsNot Nothing Then
                    If imgBtnRemoveDriver.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnRemoveDriver.ToolTip, $" ({lblDriverDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnRemoveDriver.ToolTip &= $" ({lblDriverDescription.Text})"
                        End If
                    Else
                        imgBtnRemoveDriver.ToolTip = $"Remove {lblDriverDescription.Text}"
                    End If
                End If
                If xSpanRemoveDriver IsNot Nothing Then
                    If xSpanRemoveDriver.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanRemoveDriver.Attributes.Item("title"), $" ({lblDriverDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanRemoveDriver.Attributes.Item("title") &= $" ({lblDriverDescription.Text})"
                        End If
                    Else
                        xSpanRemoveDriver.Attributes.Item("title") = $"Remove {lblDriverDescription.Text}"
                    End If
                End If
            End If

            'added 2/22/2019
            If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If xSpanRemoveDriver IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveDriver, "visibility", "hidden")
                End If
            End If

            If lblDriverNumber.Text <> "" AndAlso IsNumeric(lblDriverNumber.Text) = True AndAlso Me.GoverningStateQuote?.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(lblDriverNumber.Text) Then
                'Added 03/15/2021 for CAP Endorsements Task 52977 MLW 
                Dim endorsementDriverTransactionCount As Integer = 0
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso TypeOfEndorsement() = "Add/Delete Driver" Then
                    endorsementDriverTransactionCount = ddh.GetEndorsementTransactionCount()
                End If
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso endorsementDriverTransactionCount >= 3 Then
                    'NOTE: do not need to remove the delete icon for vehicles and additional interests since the driver section is disabled for those types of endorsements.
                    Dim myDriver As QuickQuote.CommonObjects.QuickQuoteDriver = Me.GoverningStateQuote.Drivers.Item(CInt(lblDriverNumber.Text) - 1)
                    If Not IsNewDriverOnEndorsement(myDriver) Then
                        If xSpanRemoveDriver IsNot Nothing Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveDriver, "visibility", "hidden")
                        End If
                    End If
                End If

                With Me.GoverningStateQuote.Drivers.Item(CInt(lblDriverNumber.Text) - 1)
                    If .AccidentViolations IsNot Nothing AndAlso .AccidentViolations.Count > 0 Then
                        pnlDriverAccidentViolations.Visible = True
                        DriverSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                        Dim dt As New DataTable
                        dt.Columns.AddStrings("DriverAccidentViolationDescription", "DriverAccidentViolationNumber")

                        Dim accViolNum As Integer = 0
                        For Each av As QuickQuoteAccidentViolation In .AccidentViolations
                            accViolNum += 1
                            Dim newRow As DataRow = dt.NewRow
                            Dim accViolDesc As String = QQHelper.appendText(av.AvDate, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuoteHelperClass.QuickQuotePropertyName.AccidentsViolationsTypeId, av.AccidentsViolationsTypeId), " - ")
                            newRow.Item("DriverAccidentViolationDescription") = If(accViolDesc <> "", accViolDesc, "Accident/Violation " & accViolNum.ToString)
                            newRow.Item("DriverAccidentViolationNumber") = accViolNum.ToString
                            dt.Rows.Add(newRow)
                        Next

                        'moved outside of IF to bind message row when there's no accs/viols
                        rptDriverAccidentViolations.DataSource = dt
                        rptDriverAccidentViolations.DataBind()
                        SetupDriverAccidentViolationsRepeater(rptDriverAccidentViolations)
                    End If

                    If .LossHistoryRecords IsNot Nothing AndAlso .LossHistoryRecords.Count > 0 Then
                        pnlDriverLossHistories.Visible = True
                        DriverSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                        Dim dt As New DataTable
                        dt.Columns.AddStrings("DriverLossHistoryDescription", "DriverLossHistoryNumber", "DriverLossHistorySourceId")

                        Dim lossHistNum As Integer = 0
                        For Each lh As QuickQuoteLossHistoryRecord In .LossHistoryRecords
                            lossHistNum += 1
                            Dim newRow As DataRow = dt.NewRow

                            Dim lossTypeOrDesc As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, lh.TypeOfLossId)
                            If lossTypeOrDesc = "" OrElse UCase(lossTypeOrDesc) = "N/A" Then
                                lossTypeOrDesc = lh.LossDescription
                                If lossTypeOrDesc = "" AndAlso lh.ClaimNumber <> "" Then 'backup logic to use claim # if nothing else is available
                                    lossTypeOrDesc = Trim(lh.ClaimNumber)
                                End If
                            End If
                            Dim lossHistDesc As String = QQHelper.appendText(lh.LossDate, lossTypeOrDesc, " - ")
                            'updated 7/2/2014 to show amount when available
                            If QQHelper.IsZeroPremium(lh.Amount) = False Then
                                lossHistDesc = QQHelper.appendText(lossHistDesc, FormatCurrency(lh.Amount, 2).ToString, "; ") 'currently using FormatCurrency but may update property to use ConvertToQuotedPremiumFormat method at some point
                            End If
                            newRow.Item("DriverLossHistoryDescription") = If(lossHistDesc <> "", lossHistDesc, "Loss History " & lossHistNum.ToString)
                            newRow.Item("DriverLossHistoryNumber") = lossHistNum.ToString
                            newRow.Item("DriverLossHistorySourceId") = lh.LossHistorySourceId
                            dt.Rows.Add(newRow)
                        Next

                        rptDriverLossHistories.DataSource = dt
                        rptDriverLossHistories.DataBind()
                        SetupDriverLossHistoriesRepeater(rptDriverLossHistories)
                    End If
                End With
            End If
        End If
    End Sub
    Protected Sub rptVehicles_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVehicles.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim lblVehicleNumber As Label = e.Item.FindControl("lblVehicleNumber")
            Dim pnlVehicleDrivers As Panel = e.Item.FindControl("pnlVehicleDrivers")
            Dim rptVehicleDrivers As Repeater = e.Item.FindControl("rptVehicleDrivers")
            Dim imgBtnRemoveVehicle As ImageButton = e.Item.FindControl("imgBtnRemoveVehicle")
            Dim VehicleSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("VehicleSubLists_expandCollapseImageArea")
            Dim lblVehicleDescription As Label = e.Item.FindControl("lblVehicleDescription")
            Dim xSpanRemoveVehicle As HtmlGenericControl = e.Item.FindControl("xSpanRemoveVehicle")

            If imgBtnRemoveVehicle IsNot Nothing Then
                imgBtnRemoveVehicle.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this vehicle?');}")

                'Updated 04/02/2021 for CAP Endorsements Task 52977 MLW       
                Dim endorsementVehicleTransactionCount As Integer = 0
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" Then
                    endorsementVehicleTransactionCount = ddh.GetEndorsementVehicleTransactionCount()
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso (endorsementVehicleTransactionCount >= 3 OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
                    'NOTE: do not need to remove the delete icon for drivers since the vehicle section is disabled for that type of endorsement.
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveVehicle, "visibility", "hidden")
                End If
                ''added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveVehicle, "visibility", "hidden")
                'End If
            End If

            pnlVehicleDrivers.Visible = False
            VehicleSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            'added 3/18/2014
            If lblVehicleDescription IsNot Nothing AndAlso lblVehicleDescription.Text <> "" Then
                If lblVehicleDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblVehicleDescription.ToolTip, $" ({lblVehicleDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblVehicleDescription.ToolTip &= " (" & lblVehicleDescription.Text & ")"
                    End If
                Else
                    lblVehicleDescription.ToolTip = lblVehicleDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblVehicleDescription.ToolTip) = False AndAlso lblVehicleDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblVehicleDescription.ToolTip = lblVehicleDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditVehicle since it's always hidden
                End If
                If imgBtnRemoveVehicle IsNot Nothing Then
                    If imgBtnRemoveVehicle.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnRemoveVehicle.ToolTip, $" ({lblVehicleDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnRemoveVehicle.ToolTip &= " (" & lblVehicleDescription.Text & ")"
                        End If
                    Else
                        imgBtnRemoveVehicle.ToolTip = "Remove " & lblVehicleDescription.Text
                    End If
                End If
                If xSpanRemoveVehicle IsNot Nothing Then
                    If xSpanRemoveVehicle.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanRemoveVehicle.Attributes.Item("title"), $" ({lblVehicleDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanRemoveVehicle.Attributes.Item("title") &= " (" & lblVehicleDescription.Text & ")"
                        End If
                    Else
                        xSpanRemoveVehicle.Attributes.Item("title") = "Remove " & lblVehicleDescription.Text
                    End If
                End If
            End If

            'added 2/22/2019
            If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If xSpanRemoveVehicle IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveVehicle, "visibility", "hidden")
                End If
            End If

            If lblVehicleNumber.Text <> "" AndAlso IsNumeric(lblVehicleNumber.Text) = True AndAlso Me.Quote?.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count >= CInt(lblVehicleNumber.Text) Then
                'Added 04/02/2021 for CAP Endorsements Task 52977 MLW  
                Dim endorsementVehicleTransactionCount As Integer = 0
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" Then
                    endorsementVehicleTransactionCount = ddh.GetEndorsementVehicleTransactionCount()
                End If
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso (endorsementVehicleTransactionCount >= 3 OrElse TypeOfEndorsement() = "Add/Delete Additional Interest") Then
                    'NOTE: do not need to remove the delete icon for drivers since the vehicle section is disabled for that type of endorsement.
                    Dim myVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle = Me.Quote.Vehicles.Item(CInt(lblVehicleNumber.Text) - 1)
                    If Not IsNewVehicleOnEndorsement(myVehicle) Then
                        If xSpanRemoveVehicle IsNot Nothing Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveVehicle, "visibility", "hidden")
                        End If
                    End If
                End If

                ''added 6/14/2019 for Endorsements/ReadOnly
                'Dim checkDriverAssignment As Boolean = True
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                '    If PPA.PPA_General.IsParachuteQuote(Me.Quote) = True Then
                '        checkDriverAssignment = False
                '    End If
                'End If

                'If checkDriverAssignment = True Then 'added 6/14/2019 for Endorsements/ReadOnly; previously happening every time
                '    Dim hasPrincipal As Boolean = False
                '    Dim hasOcc1 As Boolean = False
                '    Dim hasOcc2 As Boolean = False
                '    Dim hasOcc3 As Boolean = False
                '    With Me.Quote.Vehicles.Item(CInt(lblVehicleNumber.Text) - 1)
                '        If .PrincipalDriverNum <> "" AndAlso IsNumeric(.PrincipalDriverNum) = True AndAlso CInt(.PrincipalDriverNum) > 0 AndAlso Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(.PrincipalDriverNum) Then
                '            hasPrincipal = True
                '        End If
                '        If .OccasionalDriver1Num <> "" AndAlso IsNumeric(.OccasionalDriver1Num) = True AndAlso CInt(.OccasionalDriver1Num) > 0 AndAlso Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(.OccasionalDriver1Num) Then
                '            hasOcc1 = True
                '        End If
                '        If .OccasionalDriver2Num <> "" AndAlso IsNumeric(.OccasionalDriver2Num) = True AndAlso CInt(.OccasionalDriver2Num) > 0 AndAlso Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(.OccasionalDriver2Num) Then
                '            hasOcc2 = True
                '        End If
                '        If .OccasionalDriver3Num <> "" AndAlso IsNumeric(.OccasionalDriver3Num) = True AndAlso CInt(.OccasionalDriver3Num) > 0 AndAlso Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count >= CInt(.OccasionalDriver3Num) Then
                '            hasOcc3 = True
                '        End If

                '        If hasPrincipal = True OrElse hasOcc1 = True OrElse hasOcc2 = True OrElse hasOcc3 = True Then
                '            pnlVehicleDrivers.Visible = True
                '            Dim dt As New DataTable
                '            dt.Columns.AddStrings("VehicleDriverDescription", "VehicleDriverIdentifier", "DriverNumber")

                '            If hasPrincipal = True Then
                '                Dim newRow As DataRow = dt.NewRow
                '                newRow.Item("VehicleDriverDescription") = If(Me.GoverningStateQuote.Drivers.Item(CInt(.PrincipalDriverNum) - 1).Name.DisplayName <> "", Me.GoverningStateQuote.Drivers.Item(CInt(.PrincipalDriverNum) - 1).Name.DisplayName, $"Driver { .PrincipalDriverNum}") & " (principal)"
                '                newRow.Item("VehicleDriverIdentifier") = "PrincipalDriver"
                '                newRow.Item("DriverNumber") = .PrincipalDriverNum
                '                dt.Rows.Add(newRow)
                '            End If
                '            If hasOcc1 = True Then
                '                Dim newRow As DataRow = dt.NewRow
                '                newRow.Item("VehicleDriverDescription") = If(Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver1Num) - 1).Name.DisplayName <> "", Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver1Num) - 1).Name.DisplayName, $"Driver { .OccasionalDriver1Num}") & " (occasional 1)"
                '                newRow.Item("VehicleDriverIdentifier") = "OccasionalDriver1"
                '                newRow.Item("DriverNumber") = .OccasionalDriver1Num
                '                dt.Rows.Add(newRow)
                '            End If
                '            If hasOcc2 = True Then
                '                Dim newRow As DataRow = dt.NewRow
                '                newRow.Item("VehicleDriverDescription") = If(Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver2Num) - 1).Name.DisplayName <> "", Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver2Num) - 1).Name.DisplayName, $"Driver { .OccasionalDriver2Num}") & " (occasional 2)"
                '                newRow.Item("VehicleDriverIdentifier") = "OccasionalDriver2"
                '                newRow.Item("DriverNumber") = .OccasionalDriver2Num
                '                dt.Rows.Add(newRow)
                '            End If
                '            If hasOcc3 = True Then
                '                Dim newRow As DataRow = dt.NewRow
                '                newRow.Item("VehicleDriverDescription") = If(Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver3Num) - 1).Name.DisplayName <> "", Me.GoverningStateQuote.Drivers.Item(CInt(.OccasionalDriver3Num) - 1).Name.DisplayName, $"Driver { .OccasionalDriver3Num}") & " (occasional 3)"
                '                newRow.Item("VehicleDriverIdentifier") = "OccasionalDriver3"
                '                newRow.Item("DriverNumber") = .OccasionalDriver3Num
                '                dt.Rows.Add(newRow)
                '            End If

                '            rptVehicleDrivers.DataSource = dt
                '            rptVehicleDrivers.DataBind()
                '            SetupVehicleDriversRepeater(rptVehicleDrivers)
                '            VehicleSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                '        End If
                '    End With
                'End If
            End If
        End If
    End Sub
    Private Sub rptCPPCPRLocations_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCPPCPRLocations.ItemDataBound
        Dim imgBtnRemoveLocation As ImageButton = Nothing
        Dim LocationSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = Nothing
        Dim lblLocationNumber As Label = Nothing
        Dim pnlLocationBuildings As Panel = Nothing
        Dim rptLocationBuildings As Repeater = Nothing
        Dim lblLocationDescription As Label = Nothing
        Dim xSpanRemoveLocation As HtmlGenericControl = Nothing
        Dim pnlLocationDwellings As Panel = Nothing
        Dim rptLocationDwellings As Repeater = Nothing

        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' COMMERCIAL PACKAGE - PROPERTY
            imgBtnRemoveLocation = e.Item.FindControl("imgBtnRemoveCPPCPRLocation")
            LocationSubLists_expandCollapseImageArea = e.Item.FindControl("CPPCPRLocationSubLists_expandCollapseImageArea")

            If imgBtnRemoveLocation IsNot Nothing Then
                imgBtnRemoveLocation.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this location?');}")

                'added 2/22/2019
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveLocation, "visibility", "hidden")
                End If
            End If

            lblLocationNumber = e.Item.FindControl("lblCPPCPRLocationNumber")
            pnlLocationBuildings = e.Item.FindControl("pnlCPPCPRLocationBuildings")
            rptLocationBuildings = e.Item.FindControl("rptCPPCPRLocationBuildings")
            lblLocationDescription = e.Item.FindControl("lblCPPCPRLocationDescription")
            xSpanRemoveLocation = e.Item.FindControl("xSpanRemoveCPPCPRLocation")

            pnlLocationBuildings.Visible = False
            LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            ' Don't show delete on 1st location
            If xSpanRemoveLocation IsNot Nothing Then
                xSpanRemoveLocation.Style.Add("visibility", "hidden")

                If lblLocationNumber IsNot Nothing AndAlso QQHelper.IsNumericString(lblLocationNumber.Text) AndAlso CInt(lblLocationNumber.Text) > 1 Then
                    xSpanRemoveLocation.Style.Add("visibility", "visible")
                End If
                'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW                
                ''added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveLocation, "visibility", "hidden")
                End If
            End If

            If lblLocationDescription IsNot Nothing AndAlso lblLocationDescription.Text <> "" Then
                If lblLocationDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblLocationDescription.ToolTip, $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblLocationDescription.ToolTip &= $" ({lblLocationDescription.Text})"
                    End If
                Else
                    lblLocationDescription.ToolTip = lblLocationDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblLocationDescription.ToolTip) = False AndAlso lblLocationDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblLocationDescription.ToolTip = lblLocationDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditCPPCPRLocation since it's always hidden
                End If
                If imgBtnRemoveLocation IsNot Nothing Then
                    If imgBtnRemoveLocation.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnRemoveLocation.ToolTip, $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnRemoveLocation.ToolTip &= $" ({lblLocationDescription.Text})"
                        End If
                    Else
                        imgBtnRemoveLocation.ToolTip = $"Remove {lblLocationDescription.Text}"
                    End If
                End If
                If xSpanRemoveLocation IsNot Nothing Then
                    If xSpanRemoveLocation.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanRemoveLocation.Attributes.Item("title"), $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanRemoveLocation.Attributes.Item("title") &= $" ({lblLocationDescription.Text})"
                        End If
                    Else
                        xSpanRemoveLocation.Attributes.Item("title") = $"Remove {lblLocationDescription.Text}"
                    End If
                End If
            End If

            If lblLocationNumber.Text <> "" AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                With Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1)
                    If CountEvenIfNull(.Buildings) > 0 Then
                        pnlLocationBuildings.Visible = True
                        LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                        Dim dt As New DataTable
                        dt.Columns.AddStrings("LocationBuildingDescription", "BuildingNumber")

                        Dim buildingNum As Integer = 0
                        For Each b As QuickQuoteBuilding In .Buildings
                            buildingNum += 1
                            Dim newRow As DataRow = dt.NewRow
                            newRow.Item("LocationBuildingDescription") = BuildingDescription(b, buildingNum)
                            newRow.Item("BuildingNumber") = buildingNum.ToString
                            dt.Rows.Add(newRow)
                        Next

                        rptLocationBuildings.DataSource = dt
                        rptLocationBuildings.DataBind()
                        SetupLocationBuildingsRepeater(rptLocationBuildings)
                    End If
                End With
            End If
        End If
    End Sub
    Private Sub rptCPPCGLLocations_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCPPCGLLocations.ItemDataBound
        Dim imgBtnRemoveLocation As ImageButton = Nothing
        Dim LocationSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = Nothing
        Dim lblLocationNumber As Label = Nothing
        Dim pnlLocationBuildings As Panel = Nothing
        Dim rptLocationBuildings As Repeater = Nothing
        Dim lblLocationDescription As Label = Nothing
        Dim xSpanRemoveLocation As HtmlGenericControl = Nothing
        Dim pnlLocationDwellings As Panel = Nothing
        Dim rptLocationDwellings As Repeater = Nothing

        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            ' COMMERCIAL PACKAGE - LIABILITY
            LocationSubLists_expandCollapseImageArea = e.Item.FindControl("CPPCGLLocationSubLists_expandCollapseImageArea")
            lblLocationNumber = e.Item.FindControl("lblCPPCGLLocationNumber")
            lblLocationDescription = e.Item.FindControl("lblCPPCGLLocationDescription")
            LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblLocationDescription IsNot Nothing AndAlso lblLocationDescription.Text <> "" Then
                If lblLocationDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblLocationDescription.ToolTip, $" ({lblLocationDescription.Text})") = False Then
                        lblLocationDescription.ToolTip &= $" ({lblLocationDescription.Text})"
                    End If
                Else
                    lblLocationDescription.ToolTip = lblLocationDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblLocationDescription.ToolTip) = False AndAlso lblLocationDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblLocationDescription.ToolTip = lblLocationDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditCPPCGLLocation since it's always hidden
                End If
            End If
            'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
            'added 2/22/2019
            'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
            If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                imgBtnRemoveLocation = e.Item.FindControl("imgBtnRemoveCPPCGLLocation")
                If imgBtnRemoveLocation IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveLocation, "visibility", "hidden")
                End If
                xSpanRemoveLocation = e.Item.FindControl("xSpanRemoveCPPCGLLocation")
                If xSpanRemoveLocation IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveLocation, "visibility", "hidden")
                End If
            End If
        End If
    End Sub
    Protected Sub rptLocations_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLocations.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim imgBtnRemoveLocation As ImageButton = e.Item.FindControl("imgBtnRemoveLocation")
            Dim LocationSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("LocationSubLists_expandCollapseImageArea")

            If imgBtnRemoveLocation IsNot Nothing Then
                imgBtnRemoveLocation.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this location?');}")

                'added 2/22/2019
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveLocation, "visibility", "hidden")
                End If
            End If


            Dim lblLocationNumber As Label = e.Item.FindControl("lblLocationNumber")
            Dim pnlLocationBuildings As Panel = e.Item.FindControl("pnlLocationBuildings")
            Dim rptLocationBuildings As Repeater = e.Item.FindControl("rptLocationBuildings")

            Dim lblLocationDescription As Label = e.Item.FindControl("lblLocationDescription")
            Dim xSpanRemoveLocation As HtmlGenericControl = e.Item.FindControl("xSpanRemoveLocation")
            Dim pnlLocationDwellings As Panel = e.Item.FindControl("pnlLocationDwellings") 'added 7/21/2015 for Farm dwellings (location w/ valid form type)
            Dim rptLocationDwellings As Repeater = e.Item.FindControl("rptLocationDwellings") 'added 7/21/2015 for Farm dwellings (location w/ valid form type)

            pnlLocationBuildings.Visible = False
            pnlLocationDwellings.Visible = False 'added 7/21/2015 for Farm dwellings (location w/ valid form type)

            LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            'added 7/21/2015 for Farm (to not show delete on 1st location)
            If xSpanRemoveLocation IsNot Nothing Then
                xSpanRemoveLocation.Style.Add("visibility", "hidden")

                If lblLocationNumber IsNot Nothing AndAlso QQHelper.IsNumericString(lblLocationNumber.Text) AndAlso CInt(lblLocationNumber.Text) > 1 Then
                    xSpanRemoveLocation.Style.Add("visibility", "visible")
                End If
                'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                ''added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveLocation, "visibility", "hidden")
                End If
            End If

            If String.IsNullOrWhiteSpace(lblLocationDescription?.Text) = False Then
                If lblLocationDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblLocationDescription.ToolTip, $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblLocationDescription.ToolTip &= $" ({lblLocationDescription.Text})"
                    End If
                Else
                    lblLocationDescription.ToolTip = lblLocationDescription.Text
                End If
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblLocationDescription.ToolTip) = False AndAlso lblLocationDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                    lblLocationDescription.ToolTip = lblLocationDescription.ToolTip.Replace("Edit", "View")
                    'not handling for imgBtnEditLocation since it's always hidden
                End If
                If imgBtnRemoveLocation IsNot Nothing Then
                    If imgBtnRemoveLocation.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnRemoveLocation.ToolTip, $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnRemoveLocation.ToolTip &= " (" & lblLocationDescription.Text & ")"
                        End If
                    Else
                        imgBtnRemoveLocation.ToolTip = "Remove " & lblLocationDescription.Text
                    End If
                End If
                If xSpanRemoveLocation IsNot Nothing Then
                    If xSpanRemoveLocation.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanRemoveLocation.Attributes.Item("title"), $" ({lblLocationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanRemoveLocation.Attributes.Item("title") &= $" ({lblLocationDescription.Text})"
                        End If
                    Else
                        xSpanRemoveLocation.Attributes.Item("title") = "Remove " & lblLocationDescription.Text
                    End If
                End If
            End If

            If lblLocationNumber.Text <> "" AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count >= CInt(lblLocationNumber.Text) Then
                With Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1)
                    If QQHelper.IsNumericString(.FormTypeId) = True Then
                        Dim formType As String
                        If .FormTypeId.EqualsAny("22", "25") AndAlso .StructureTypeId = "2" Then
                            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            Dim a1 As New QuickQuoteStaticDataAttribute
                            a1.nvp_name = "StructureTypeId" 'need structure type 2 to determine if new form type is mobile or not for HO 0002 and HO 0004 MLW
                            a1.nvp_value = 2
                            optionAttributes.Add(a1)
                            formType = QQHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, .FormTypeId, Me.Quote.LobType)
                        Else
                            formType = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, .FormTypeId, Me.Quote.LobType)
                        End If

                        If UCase(formType).NotEqualsAny("", "UNASSIGNED", "NONE", "N/A") Then
                            pnlLocationDwellings.Visible = True

                            LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                            Dim dt As New DataTable
                            dt.Columns.AddStrings("LocationDwellingDescription", "DwellingNumber")

                            Dim newRow As DataRow = dt.NewRow
                            newRow.Item("LocationDwellingDescription") = LocationDwellingDescription(Me.Quote.Locations.Item(CInt(lblLocationNumber.Text) - 1), formType)
                            newRow.Item("DwellingNumber") = lblLocationNumber.Text
                            dt.Rows.Add(newRow)

                            rptLocationDwellings.DataSource = dt
                            rptLocationDwellings.DataBind()
                            SetupLocationDwellingsRepeater(rptLocationDwellings)
                        End If
                    End If

                    If .Buildings.CountEvenIfNull() > 0 Then
                        pnlLocationBuildings.Visible = True
                        LocationSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")

                        Dim dt As New DataTable
                        dt.Columns.AddStrings("LocationBuildingDescription", "BuildingNumber")

                        Dim buildingNum As Integer = 0
                        For Each b As QuickQuoteBuilding In .Buildings
                            buildingNum += 1
                            Dim newRow As DataRow = dt.NewRow
                            newRow.Item("LocationBuildingDescription") = BuildingDescription(b, buildingNum)
                            newRow.Item("BuildingNumber") = buildingNum.ToString
                            dt.Rows.Add(newRow)
                        Next

                        rptLocationBuildings.DataSource = dt
                        rptLocationBuildings.DataBind()
                        SetupLocationBuildingsRepeater(rptLocationBuildings)
                    End If
                End With
            End If
        End If
    End Sub


    Private Sub rptCoverages_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCoverages.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim imgBtnRemoveCoverage As ImageButton = e.Item.FindControl("imgBtnRemoveCoverage")
            Dim CoverageSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("CoverageSubLists_expandCollapseImageArea")
            Dim lblCoverageDescription As Label = e.Item.FindControl("lblCoverageDescription")
            Dim xSpanRemoveCoverage As HtmlGenericControl = e.Item.FindControl("xSpanRemoveCoverage")

            If imgBtnRemoveCoverage IsNot Nothing Then
                imgBtnRemoveCoverage.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this coverage?');}")

                'added 2/22/2019
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveCoverage, "visibility", "hidden")
                End If
            End If

            CoverageSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblCoverageDescription IsNot Nothing AndAlso lblCoverageDescription.Text <> "" Then
                If lblCoverageDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblCoverageDescription.ToolTip, " (" & lblCoverageDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblCoverageDescription.ToolTip &= " (" & lblCoverageDescription.Text & ")"
                    End If
                Else
                    lblCoverageDescription.ToolTip = lblCoverageDescription.Text
                End If
                If imgBtnRemoveCoverage IsNot Nothing Then
                    If imgBtnRemoveCoverage.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnRemoveCoverage.ToolTip, " (" & lblCoverageDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnRemoveCoverage.ToolTip &= " (" & lblCoverageDescription.Text & ")"
                        End If
                    Else
                        imgBtnRemoveCoverage.ToolTip = "Remove " & lblCoverageDescription.Text
                    End If
                End If
                If xSpanRemoveCoverage IsNot Nothing Then
                    If xSpanRemoveCoverage.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanRemoveCoverage.Attributes.Item("title"), " (" & lblCoverageDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanRemoveCoverage.Attributes.Item("title") &= " (" & lblCoverageDescription.Text & ")"
                        End If
                    Else
                        xSpanRemoveCoverage.Attributes.Item("title") = "Remove " & lblCoverageDescription.Text
                    End If
                End If
            End If

            'added 2/22/2019
            If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If xSpanRemoveCoverage IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveCoverage, "visibility", "hidden")
                End If
            End If

        End If
    End Sub

    Private Sub rptMvrReports_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptMvrReports.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim MvrReportSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("MvrReportSubLists_expandCollapseImageArea")
            Dim lblMvrReportDescription As Label = e.Item.FindControl("lblMvrReportDescription")
            MvrReportSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblMvrReportDescription IsNot Nothing AndAlso lblMvrReportDescription.Text <> "" Then
                If lblMvrReportDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblMvrReportDescription.ToolTip, " (" & lblMvrReportDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblMvrReportDescription.ToolTip &= " (" & lblMvrReportDescription.Text & ")"
                    End If
                Else
                    lblMvrReportDescription.ToolTip = lblMvrReportDescription.Text
                End If
            End If
        End If
    End Sub
    Private Sub rptClueReports_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptClueReports.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ClueReportSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("ClueReportSubLists_expandCollapseImageArea")
            Dim lblClueReportDescription As Label = e.Item.FindControl("lblClueReportDescription")

            ClueReportSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblClueReportDescription IsNot Nothing AndAlso lblClueReportDescription.Text <> "" Then
                If lblClueReportDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblClueReportDescription.ToolTip, " (" & lblClueReportDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblClueReportDescription.ToolTip &= " (" & lblClueReportDescription.Text & ")"
                    End If
                Else
                    lblClueReportDescription.ToolTip = lblClueReportDescription.Text
                End If
            End If
        End If
    End Sub
    Private Sub rptCreditReports_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCreditReports.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim CreditReportSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("CreditReportSubLists_expandCollapseImageArea")
            Dim lblCreditReportDescription As Label = e.Item.FindControl("lblCreditReportDescription")
            CreditReportSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblCreditReportDescription IsNot Nothing AndAlso lblCreditReportDescription.Text <> "" Then
                If lblCreditReportDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblCreditReportDescription.ToolTip, " (" & lblCreditReportDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblCreditReportDescription.ToolTip &= " (" & lblCreditReportDescription.Text & ")"
                    End If
                Else
                    lblCreditReportDescription.ToolTip = lblCreditReportDescription.Text
                End If
            End If
        End If
    End Sub
    Private Sub rptResidences_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptResidences.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim imgBtnClearResidence As ImageButton = e.Item.FindControl("imgBtnClearResidence")
            Dim ResidenceSubLists_expandCollapseImageArea As HtmlControls.HtmlGenericControl = e.Item.FindControl("ResidenceSubLists_expandCollapseImageArea")
            Dim lblResidenceDescription As Label = e.Item.FindControl("lblResidenceDescription")
            Dim xSpanClearResidence As HtmlGenericControl = e.Item.FindControl("xSpanClearResidence")
            Dim ulPropertySubItems As HtmlGenericControl = e.Item.FindControl("ulPropertySubItems")
            Dim liPropertySubItem_ProtectionClass As HtmlGenericControl = e.Item.FindControl("liPropertySubItem_ProtectionClass")
            Dim lblProtectionClass As Label = e.Item.FindControl("lblProtectionClass")
            Dim liPropertySubItem_FormType As HtmlGenericControl = e.Item.FindControl("liPropertySubItem_FormType")
            Dim lblFormType As Label = e.Item.FindControl("lblFormType")
            Dim lblFormTypeId As Label = e.Item.FindControl("lblFormTypeId")
            Dim ddlFormType As DropDownList = e.Item.FindControl("ddlFormType")
            Dim liPropertySubItem_FormTypeId As HtmlGenericControl = e.Item.FindControl("liPropertySubItem_FormTypeId")
            Dim hdnOriginalFormType As HtmlInputHidden = e.Item.FindControl("hdnOriginalFormType")
            Dim hdnOriginalFormTypeId As HtmlInputHidden = e.Item.FindControl("hdnOriginalFormTypeId")

            Dim lblLocationNumber As Label = e.Item.FindControl("lblLocationNumber")
            Dim pnlPropertyLossHistories As Panel = e.Item.FindControl("pnlPropertyLossHistories")
            Dim rptPropertyLossHistories As Repeater = e.Item.FindControl("rptPropertyLossHistories")

            Dim liPropertySubItem_ProtectionClass_VeriskReport As HtmlGenericControl = e.Item.FindControl("liPropertySubItem_ProtectionClass_VeriskReport")
            Dim lblProtectionClass_VeriskReport As Label = e.Item.FindControl("lblProtectionClass_VeriskReport")
            Dim lblProtectionClassSystemGenerated_VeriskReport As Label = e.Item.FindControl("lblProtectionClassSystemGenerated_VeriskReport")
            Dim lblProtectionClassId_VeriskReport As Label = e.Item.FindControl("lblProtectionClassId_VeriskReport")
            Dim lblProtectionClassSystemGeneratedId_VeriskReport As Label = e.Item.FindControl("lblProtectionClassSystemGeneratedId_VeriskReport")
            Dim lblLocationNumber_VeriskReport As Label = e.Item.FindControl("lblLocationNumber_VeriskReport")

            Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = GetQuoteTransactionTypeFlagForTree() 'added 3/3/2020

            If imgBtnClearResidence IsNot Nothing Then
                imgBtnClearResidence.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to clear this Property?');}")

                'added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                'updated 3/3/2020 to use variable
                If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnClearResidence, "visibility", "hidden")
                End If
            End If

            ResidenceSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")

            If lblResidenceDescription IsNot Nothing AndAlso lblResidenceDescription.Text <> "" Then
                If lblResidenceDescription.ToolTip <> "" Then
                    If TooltipContainsDescription(lblResidenceDescription.ToolTip, " (" & lblResidenceDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        lblResidenceDescription.ToolTip &= " (" & lblResidenceDescription.Text & ")"
                    End If
                Else
                    lblResidenceDescription.ToolTip = lblResidenceDescription.Text
                End If
                If imgBtnClearResidence IsNot Nothing Then
                    If imgBtnClearResidence.ToolTip <> "" Then
                        If TooltipContainsDescription(imgBtnClearResidence.ToolTip, " (" & lblResidenceDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            imgBtnClearResidence.ToolTip &= " (" & lblResidenceDescription.Text & ")"
                        End If
                    Else
                        imgBtnClearResidence.ToolTip = "Clear " & lblResidenceDescription.Text
                    End If
                End If
                If xSpanClearResidence IsNot Nothing Then
                    If xSpanClearResidence.Attributes.Item("title") <> "" Then
                        If TooltipContainsDescription(xSpanClearResidence.Attributes.Item("title"), " (" & lblResidenceDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            xSpanClearResidence.Attributes.Item("title") &= " (" & lblResidenceDescription.Text & ")"
                        End If
                    Else
                        xSpanClearResidence.Attributes.Item("title") = "Clear " & lblResidenceDescription.Text
                    End If
                End If
            End If

            'added 2/22/2019
            'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
            'updated 3/3/2020 to use variable
            If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If xSpanClearResidence IsNot Nothing Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanClearResidence, "visibility", "hidden")
                End If
            End If

            If ulPropertySubItems IsNot Nothing Then
                Dim hasSubItems As Boolean = False

                If liPropertySubItem_ProtectionClass IsNot Nothing Then
                    If lblProtectionClass IsNot Nothing AndAlso lblProtectionClass.Text <> "" Then
                        liPropertySubItem_ProtectionClass.Visible = True
                        hasSubItems = True

                        If liPropertySubItem_ProtectionClass_VeriskReport IsNot Nothing Then
                            Dim showVeriskProtClassListItem As Boolean = False
                            If lblProtectionClass_VeriskReport IsNot Nothing Then
                                'can set to normal Protection Class or System Generated if needed
                                lblProtectionClass_VeriskReport.Text = lblProtectionClass.Text
                                If lblProtectionClassSystemGeneratedId_VeriskReport IsNot Nothing Then
                                    If QQHelper.IsPositiveIntegerString(lblProtectionClassSystemGeneratedId_VeriskReport.Text) = True Then
                                        'this means we have a system generated protection class
                                        'showVeriskProtClassListItem = True
                                        'updated 3/3/2020 to only show for New Business quoting and not Endorsements/ReadOnly
                                        If qqTranType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso qqTranType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                            showVeriskProtClassListItem = True
                                        End If
                                    End If
                                End If
                                If lblLocationNumber IsNot Nothing AndAlso lblLocationNumber_VeriskReport IsNot Nothing Then
                                    If QQHelper.IsPositiveIntegerString(lblLocationNumber.Text) = True Then
                                        lblLocationNumber_VeriskReport.Text = lblLocationNumber.Text
                                    End If
                                End If
                            End If
                            If showVeriskProtClassListItem = True Then
                                liPropertySubItem_ProtectionClass_VeriskReport.Visible = True
                                liPropertySubItem_ProtectionClass.Visible = False
                            Else
                                liPropertySubItem_ProtectionClass_VeriskReport.Visible = False
                            End If
                        End If
                    Else
                        liPropertySubItem_ProtectionClass.Visible = False
                    End If
                End If
                If liPropertySubItem_FormType IsNot Nothing Then
                    liPropertySubItem_FormType.Visible = False
                    If lblFormType IsNot Nothing Then
                        If hdnOriginalFormType IsNot Nothing Then
                            hdnOriginalFormType.Value = lblFormType.Text
                        End If
                        If lblFormType.Text <> "" Then
                            liPropertySubItem_FormType.Visible = True
                            hasSubItems = True
                        End If
                    Else
                        liPropertySubItem_FormType.Visible = False
                    End If

                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    'updated 3/3/2020 to use variable
                    If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveAttributeFromGenericControl(liPropertySubItem_FormType, "onclick")
                    End If
                End If

                If liPropertySubItem_FormTypeId IsNot Nothing Then
                    If lblFormTypeId IsNot Nothing AndAlso ddlFormType IsNot Nothing Then
                        If hdnOriginalFormTypeId IsNot Nothing Then
                            hdnOriginalFormTypeId.Value = lblFormTypeId.Text
                        End If
                        'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/22/2019; original logic in ELSE
                        'updated 7/19/2019 to also have formType change disabled for Endorsements
                        'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 2/22/2019; original logic in ELSE
                        'updated 3/3/2020 to use variable
                        If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then 'added IF 2/22/2019; original logic in ELSE
                            liPropertySubItem_FormTypeId.Visible = False
                        Else
                            If ddlFormType.Items Is Nothing OrElse ddlFormType.Items.Count = 0 Then
                                Dim lobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
                                If Me.Quote IsNot Nothing Then
                                    lobType = Me.Quote.LobType
                                End If
                                If lblFormTypeId.Text <> "" Then 'added IF 1/8/2015; original logic in ELSE... now loading all form types when nothing's there or acceptable ones for the current form type
                                    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                                    Dim a1 As New QuickQuoteStaticDataAttribute
                                    a1.nvp_name = "FromFormTypeId"
                                    a1.nvp_value = lblFormTypeId.Text
                                    optionAttributes.Add(a1)
                                    QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(ddlFormType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, QuickQuoteStaticDataOption.SortBy.None, lobType)
                                Else
                                    QQHelper.LoadStaticDataOptionsDropDown(ddlFormType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, QuickQuoteStaticDataOption.SortBy.None, lobType)
                                End If
                            End If
                            If ddlFormType.Items IsNot Nothing AndAlso ddlFormType.Items.Count > 0 Then
                                liPropertySubItem_FormTypeId.Visible = True
                                hasSubItems = True
                                Dim dropdownHasOtherOptions As Boolean = True 'added 1/8/2015
                                If lblFormTypeId.Text <> "" AndAlso ddlFormType.Items.FindByValue(lblFormTypeId.Text) IsNot Nothing Then
                                    ddlFormType.SelectedValue = lblFormTypeId.Text
                                    If ddlFormType.Items.Count > 1 Then 'added 1/8/2015
                                        dropdownHasOtherOptions = True
                                    Else
                                        dropdownHasOtherOptions = False
                                    End If
                                End If

                                If liPropertySubItem_FormType IsNot Nothing AndAlso dropdownHasOtherOptions = True Then
                                    If liPropertySubItem_FormType.Visible = False AndAlso lblFormType IsNot Nothing AndAlso lblFormType.Text = "" Then
                                        liPropertySubItem_FormType.Visible = True
                                    End If

                                    If lblFormType IsNot Nothing Then
                                        If lblFormType.Text <> "" Then
                                            liPropertySubItem_FormType.Attributes.Item("title") = "Edit Form Type (" & lblFormType.Text & ")"
                                        Else
                                            liPropertySubItem_FormType.Attributes.Item("title") = "Select Form Type"
                                        End If
                                    Else
                                        liPropertySubItem_FormType.Attributes.Item("title") = "Edit Form Type"
                                    End If
                                    liPropertySubItem_FormType.Attributes.Item("class") = "clickable"
                                End If
                            Else
                                liPropertySubItem_FormTypeId.Visible = False
                            End If
                        End If
                    End If
                End If

                If hasSubItems = True Then
                    ulPropertySubItems.Visible = True
                Else
                    ulPropertySubItems.Visible = False
                End If
            End If

            'added 11/6/2014
            If pnlPropertyLossHistories IsNot Nothing AndAlso rptPropertyLossHistories IsNot Nothing Then
                pnlPropertyLossHistories.Visible = False
                'should only have 1, but making sure code just runs this logic for the 1st one
                If lblLocationNumber IsNot Nothing AndAlso lblLocationNumber.Text <> "" AndAlso IsNumeric(lblLocationNumber.Text) = True AndAlso Me.Quote IsNot Nothing AndAlso CInt(lblLocationNumber.Text) = 1 Then
                    With Me.Quote
                        If Me.GoverningStateQuote.HasLossHistories = True Then
                            Dim lossHistoryCounter As Integer = 0

                            Dim dt As New DataTable
                            dt.Columns.AddStrings("PropertyLossHistoryDescription", "PropertyLossHistoryCounter", "PropertyLossHistoryLevel", "PropertyLossHistoryLevelNumber", "PropertyLossHistoryLevelCounter", "PropertyLossHistorySourceId")

                            If Me.GoverningStateQuote.LossHistoryRecords.IsLoaded Then
                                Dim policyLevelLossHistoryCount As Integer = 0
                                For Each p_lh As QuickQuoteLossHistoryRecord In Me.GoverningStateQuote.LossHistoryRecords
                                    lossHistoryCounter += 1
                                    policyLevelLossHistoryCount += 1

                                    Dim newRow As DataRow = dt.NewRow
                                    Dim lossTypeOrDesc As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, p_lh.TypeOfLossId)
                                    If lossTypeOrDesc = "" OrElse UCase(lossTypeOrDesc) = "N/A" Then
                                        lossTypeOrDesc = p_lh.LossDescription
                                        If lossTypeOrDesc = "" AndAlso p_lh.ClaimNumber <> "" Then 'backup logic to use claim # if nothing else is available
                                            lossTypeOrDesc = Trim(p_lh.ClaimNumber)
                                        End If
                                    End If
                                    Dim lossHistDesc As String = QQHelper.appendText(p_lh.LossDate, lossTypeOrDesc, " - ")
                                    If QQHelper.IsZeroPremium(p_lh.Amount) = False Then
                                        lossHistDesc = QQHelper.appendText(lossHistDesc, FormatCurrency(p_lh.Amount, 2).ToString, "; ") 'currently using FormatCurrency but may update property to use ConvertToQuotedPremiumFormat method at some point
                                    End If
                                    newRow.Item("PropertyLossHistoryDescription") = If(lossHistDesc <> "", lossHistDesc, "Policy Loss History " & policyLevelLossHistoryCount.ToString)
                                    newRow.Item("PropertyLossHistoryCounter") = lossHistoryCounter.ToString
                                    newRow.Item("PropertyLossHistoryLevel") = "Policy"
                                    newRow.Item("PropertyLossHistoryLevelNumber") = "0"
                                    newRow.Item("PropertyLossHistoryLevelCounter") = policyLevelLossHistoryCount.ToString
                                    newRow.Item("PropertyLossHistorySourceId") = p_lh.LossHistorySourceId
                                    dt.Rows.Add(newRow)
                                Next
                            End If
                            If Me.GoverningStateQuote.Applicants.IsLoaded() Then
                                Dim applicantCounter As Integer = 0
                                For Each a As QuickQuoteApplicant In Me.GoverningStateQuote.Applicants
                                    applicantCounter += 1
                                    If a.LossHistoryRecords IsNot Nothing AndAlso a.LossHistoryRecords.Count > 0 Then
                                        Dim applicantLossHistoryCount As Integer = 0
                                        For Each a_lh As QuickQuoteLossHistoryRecord In a.LossHistoryRecords
                                            lossHistoryCounter += 1
                                            applicantLossHistoryCount += 1

                                            Dim newRow As DataRow = dt.NewRow
                                            Dim lossTypeOrDesc As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, a_lh.TypeOfLossId)
                                            If lossTypeOrDesc = "" OrElse UCase(lossTypeOrDesc) = "N/A" Then
                                                lossTypeOrDesc = a_lh.LossDescription
                                                If lossTypeOrDesc = "" AndAlso a_lh.ClaimNumber <> "" Then 'backup logic to use claim # if nothing else is available
                                                    lossTypeOrDesc = Trim(a_lh.ClaimNumber)
                                                End If
                                            End If
                                            Dim lossHistDesc As String = QQHelper.appendText(a_lh.LossDate, lossTypeOrDesc, " - ")
                                            If QQHelper.IsZeroPremium(a_lh.Amount) = False Then
                                                lossHistDesc = QQHelper.appendText(lossHistDesc, FormatCurrency(a_lh.Amount, 2).ToString, "; ") 'currently using FormatCurrency but may update property to use ConvertToQuotedPremiumFormat method at some point
                                            End If
                                            newRow.Item("PropertyLossHistoryDescription") = If(lossHistDesc <> "", lossHistDesc, "Applicant " & applicantCounter.ToString & " Loss History " & applicantLossHistoryCount.ToString)
                                            newRow.Item("PropertyLossHistoryCounter") = lossHistoryCounter.ToString
                                            newRow.Item("PropertyLossHistoryLevel") = "Applicant"
                                            newRow.Item("PropertyLossHistoryLevelNumber") = applicantCounter.ToString
                                            newRow.Item("PropertyLossHistoryLevelCounter") = applicantLossHistoryCount.ToString
                                            newRow.Item("PropertyLossHistorySourceId") = a_lh.LossHistorySourceId
                                            dt.Rows.Add(newRow)
                                        Next

                                    End If
                                Next
                            End If

                            If lossHistoryCounter > 0 Then
                                pnlPropertyLossHistories.Visible = True
                                rptPropertyLossHistories.DataSource = dt
                                rptPropertyLossHistories.DataBind()
                                SetupPropertyLossHistoriesRepeater(rptPropertyLossHistories)
                            End If
                        End If
                    End With
                End If
            End If

        End If
    End Sub

#End Region

#Region "Setup Repeaters"
    Private Sub SetupVehicleDriversRepeater(ByVal rptVehicleDrivers As Repeater)
        If rptVehicleDrivers IsNot Nothing Then
            For Each i As RepeaterItem In rptVehicleDrivers.Items
                Dim imgBtnRemoveVehicleDriver As ImageButton = i.FindControl("imgBtnRemoveVehicleDriver")
                'added 3/18/2014
                Dim lblVehicleDriverDescription As Label = i.FindControl("lblVehicleDriverDescription")
                Dim xSpanRemoveVehicleDriver As HtmlGenericControl = i.FindControl("xSpanRemoveVehicleDriver")

                If imgBtnRemoveVehicleDriver IsNot Nothing Then
                    imgBtnRemoveVehicleDriver.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this vehicle driver?');}")

                    'added 2/22/2019
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveVehicleDriver, "visibility", "hidden")
                    End If
                End If

                'added 3/18/2014
                If lblVehicleDriverDescription IsNot Nothing AndAlso lblVehicleDriverDescription.Text <> "" Then
                    If lblVehicleDriverDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblVehicleDriverDescription.ToolTip, " (" & lblVehicleDriverDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblVehicleDriverDescription.ToolTip &= " (" & lblVehicleDriverDescription.Text & ")"
                        End If
                    Else
                        lblVehicleDriverDescription.ToolTip = lblVehicleDriverDescription.Text
                    End If
                    If imgBtnRemoveVehicleDriver IsNot Nothing Then
                        If imgBtnRemoveVehicleDriver.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemoveVehicleDriver.ToolTip, " (" & lblVehicleDriverDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemoveVehicleDriver.ToolTip &= " (" & lblVehicleDriverDescription.Text & ")"
                            End If
                        Else
                            imgBtnRemoveVehicleDriver.ToolTip = "Remove " & lblVehicleDriverDescription.Text
                        End If
                    End If
                    If xSpanRemoveVehicleDriver IsNot Nothing Then
                        If xSpanRemoveVehicleDriver.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemoveVehicleDriver.Attributes.Item("title"), " (" & lblVehicleDriverDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemoveVehicleDriver.Attributes.Item("title") &= " (" & lblVehicleDriverDescription.Text & ")"
                            End If
                        Else
                            xSpanRemoveVehicleDriver.Attributes.Item("title") = "Remove " & lblVehicleDriverDescription.Text
                        End If
                    End If
                End If

                'added 2/22/2019
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    If xSpanRemoveVehicleDriver IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveVehicleDriver, "visibility", "hidden")
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SetupDriverAccidentViolationsRepeater(ByVal rptDriverAccidentViolations As Repeater)
        If rptDriverAccidentViolations IsNot Nothing Then
            For Each i As RepeaterItem In rptDriverAccidentViolations.Items
                Dim imgBtnRemoveDriverAccidentViolation As ImageButton = i.FindControl("imgBtnRemoveDriverAccidentViolation")
                Dim lblDriverAccidentViolationDescription As Label = i.FindControl("lblDriverAccidentViolationDescription")
                Dim xSpanRemoveDriverAccidentViolation As HtmlGenericControl = i.FindControl("xSpanRemoveDriverAccidentViolation")

                If imgBtnRemoveDriverAccidentViolation IsNot Nothing Then
                    imgBtnRemoveDriverAccidentViolation.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this driver accident/violation?');}")

                    Dim lblDriverAccidentViolationNumber As Label = i.FindControl("lblDriverAccidentViolationNumber")
                    If lblDriverAccidentViolationNumber IsNot Nothing Then
                        If lblDriverAccidentViolationNumber.Text <> "" AndAlso IsNumeric(lblDriverAccidentViolationNumber.Text) = True Then
                            imgBtnRemoveDriverAccidentViolation.Visible = True
                        Else
                            imgBtnRemoveDriverAccidentViolation.Visible = False
                        End If
                    End If

                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    'updated 6/13/2019 to also prevent delete for Endorsements
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveDriverAccidentViolation, "visibility", "hidden")
                    End If
                End If

                'added 3/18/2014
                If lblDriverAccidentViolationDescription IsNot Nothing AndAlso lblDriverAccidentViolationDescription.Text <> "" Then
                    If lblDriverAccidentViolationDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblDriverAccidentViolationDescription.ToolTip, $" ({lblDriverAccidentViolationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblDriverAccidentViolationDescription.ToolTip &= $" ({lblDriverAccidentViolationDescription.Text})"
                        End If
                    Else
                        lblDriverAccidentViolationDescription.ToolTip = lblDriverAccidentViolationDescription.Text
                    End If
                    If imgBtnRemoveDriverAccidentViolation IsNot Nothing Then
                        If imgBtnRemoveDriverAccidentViolation.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemoveDriverAccidentViolation.ToolTip, $" ({lblDriverAccidentViolationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemoveDriverAccidentViolation.ToolTip &= $" ({lblDriverAccidentViolationDescription.Text})"
                            End If
                        Else
                            imgBtnRemoveDriverAccidentViolation.ToolTip = $"Remove {lblDriverAccidentViolationDescription.Text}"
                        End If
                    End If
                    If xSpanRemoveDriverAccidentViolation IsNot Nothing Then
                        If xSpanRemoveDriverAccidentViolation.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemoveDriverAccidentViolation.Attributes.Item("title"), $" ({lblDriverAccidentViolationDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemoveDriverAccidentViolation.Attributes.Item("title") &= $" ({lblDriverAccidentViolationDescription.Text})"
                            End If
                        Else
                            xSpanRemoveDriverAccidentViolation.Attributes.Item("title") = $"Remove {lblDriverAccidentViolationDescription.Text}"
                        End If
                    End If
                End If

                'added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                'updated 6/13/2019 to also prevent delete for Endorsements
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If xSpanRemoveDriverAccidentViolation IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveDriverAccidentViolation, "visibility", "hidden")
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SetupDriverLossHistoriesRepeater(ByVal rptDriverLossHistories As Repeater)
        If rptDriverLossHistories IsNot Nothing Then

            'added 6/17/2020 for PPA CLUE Auto
            Dim editableLossFlag As Boolean = GetEditableLossFlagForTree()
            Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = GetQuoteTransactionTypeFlagForTree()

            For Each i As RepeaterItem In rptDriverLossHistories.Items
                Dim imgBtnRemoveDriverLossHistory As ImageButton = i.FindControl("imgBtnRemoveDriverLossHistory")
                Dim lblDriverLossHistoryDescription As Label = i.FindControl("lblDriverLossHistoryDescription")
                Dim xSpanRemoveDriverLossHistory As HtmlGenericControl = i.FindControl("xSpanRemoveDriverLossHistory")

                If imgBtnRemoveDriverLossHistory IsNot Nothing Then
                    imgBtnRemoveDriverLossHistory.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this driver loss history?');}")

                    Dim lblDriverLossHistoryNumber As Label = i.FindControl("lblDriverLossHistoryNumber")
                    If lblDriverLossHistoryNumber IsNot Nothing Then
                        If IsNumeric(lblDriverLossHistoryNumber.Text) Then
                            imgBtnRemoveDriverLossHistory.Visible = True
                        Else
                            imgBtnRemoveDriverLossHistory.Visible = False
                        End If
                    End If

                    If editableLossFlag Then
                        Dim lblDriverLossHistorySourceId As Label = i.FindControl("lblDriverLossHistorySourceId")
                        If lblDriverLossHistorySourceId IsNot Nothing Then
                            Select Case lblDriverLossHistorySourceId.Text
                                Case "1", "4", "7", "8"
                                    editableLossFlag = False
                            End Select
                        End If
                    End If

                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    'updated 6/13/2019 to also prevent delete for Endorsements
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    'updated 6/17/2020
                    If editableLossFlag = False OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveDriverLossHistory, "visibility", "hidden")
                    End If
                End If

                'added 3/18/2014
                If lblDriverLossHistoryDescription IsNot Nothing AndAlso lblDriverLossHistoryDescription.Text <> "" Then
                    If lblDriverLossHistoryDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblDriverLossHistoryDescription.ToolTip, $" ({lblDriverLossHistoryDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblDriverLossHistoryDescription.ToolTip &= $" ({lblDriverLossHistoryDescription.Text})"
                        End If
                    Else
                        lblDriverLossHistoryDescription.ToolTip = lblDriverLossHistoryDescription.Text
                    End If
                    If imgBtnRemoveDriverLossHistory IsNot Nothing Then
                        If imgBtnRemoveDriverLossHistory.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemoveDriverLossHistory.ToolTip, $" ({lblDriverLossHistoryDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemoveDriverLossHistory.ToolTip &= $" ({lblDriverLossHistoryDescription.Text})"
                            End If
                        Else
                            imgBtnRemoveDriverLossHistory.ToolTip = $"Remove {lblDriverLossHistoryDescription.Text}"
                        End If
                    End If
                    If xSpanRemoveDriverLossHistory IsNot Nothing Then
                        If xSpanRemoveDriverLossHistory.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemoveDriverLossHistory.Attributes.Item("title"), $" ({lblDriverLossHistoryDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemoveDriverLossHistory.Attributes.Item("title") &= $" ({lblDriverLossHistoryDescription.Text})"
                            End If
                        Else
                            xSpanRemoveDriverLossHistory.Attributes.Item("title") = $"Remove {lblDriverLossHistoryDescription.Text}"
                        End If
                    End If
                End If

                'added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                'updated 6/13/2019 to also prevent delete for Endorsements
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                'updated 6/17/2020
                If editableLossFlag = False OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If xSpanRemoveDriverLossHistory IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveDriverLossHistory, "visibility", "hidden")
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SetupLocationBuildingsRepeater(ByVal rptLocationBuildings As Repeater)
        If rptLocationBuildings IsNot Nothing Then
            For Each i As RepeaterItem In rptLocationBuildings.Items
                Dim imgBtnRemoveLocationBuilding As ImageButton = i.FindControl("imgBtnRemoveLocationBuilding")

                Dim lblLocationBuildingDescription As Label = i.FindControl("lblLocationBuildingDescription")
                Dim xSpanRemoveLocationBuilding As HtmlGenericControl = i.FindControl("xSpanRemoveLocationBuilding")
                'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    xSpanRemoveLocationBuilding = i.FindControl("xSpanRemoveCPPCPRLocationBuilding")
                End If

                If imgBtnRemoveLocationBuilding IsNot Nothing Then
                    imgBtnRemoveLocationBuilding.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this location building?');}")

                    'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                    ''added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveLocationBuilding, "visibility", "hidden")
                    End If
                End If

                If lblLocationBuildingDescription IsNot Nothing AndAlso lblLocationBuildingDescription.Text <> "" Then
                    If lblLocationBuildingDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblLocationBuildingDescription.ToolTip, $" ({lblLocationBuildingDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblLocationBuildingDescription.ToolTip &= $" ({lblLocationBuildingDescription.Text})"
                        End If
                    Else
                        lblLocationBuildingDescription.ToolTip = lblLocationBuildingDescription.Text
                    End If
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblLocationBuildingDescription.ToolTip) = False AndAlso lblLocationBuildingDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                        lblLocationBuildingDescription.ToolTip = lblLocationBuildingDescription.ToolTip.Replace("Edit", "View")
                        'not handling for imgBtnEditLocationBuilding since it's always hidden
                    End If
                    If imgBtnRemoveLocationBuilding IsNot Nothing Then
                        If imgBtnRemoveLocationBuilding.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemoveLocationBuilding.ToolTip, $" ({lblLocationBuildingDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemoveLocationBuilding.ToolTip &= $" ({lblLocationBuildingDescription.Text})"
                            End If
                        Else
                            imgBtnRemoveLocationBuilding.ToolTip = $"Remove {lblLocationBuildingDescription.Text}"
                        End If
                    End If
                    If xSpanRemoveLocationBuilding IsNot Nothing Then
                        If xSpanRemoveLocationBuilding.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemoveLocationBuilding.Attributes.Item("title"), $" ({lblLocationBuildingDescription.Text})") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemoveLocationBuilding.Attributes.Item("title") &= $" ({lblLocationBuildingDescription.Text})"
                            End If
                        Else
                            xSpanRemoveLocationBuilding.Attributes.Item("title") = $"Remove {lblLocationBuildingDescription.Text}"
                        End If
                    End If
                End If
                'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                ''added 2/22/2019
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                    If xSpanRemoveLocationBuilding IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveLocationBuilding, "visibility", "hidden")
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SetupLocationDwellingsRepeater(ByVal rptLocationDwellings As Repeater) 'added 7/21/2015 for Farm dwellings (location w/ valid form type)
        If rptLocationDwellings IsNot Nothing Then
            For Each i As RepeaterItem In rptLocationDwellings.Items
                Dim imgBtnRemoveLocationDwelling As ImageButton = i.FindControl("imgBtnRemoveLocationDwelling")

                Dim lblLocationDwellingDescription As Label = i.FindControl("lblLocationDwellingDescription")
                Dim xSpanRemoveLocationDwelling As HtmlGenericControl = i.FindControl("xSpanRemoveLocationDwelling")
                Dim lblDwellingNumber As Label = i.FindControl("lblDwellingNumber")

                If imgBtnRemoveLocationDwelling IsNot Nothing Then
                    imgBtnRemoveLocationDwelling.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this location dwelling?');}")
                    'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemoveLocationDwelling, "visibility", "hidden")
                    End If
                End If

                'added 7/21/2015 for Farm (to not show delete on 1st location)
                If xSpanRemoveLocationDwelling IsNot Nothing Then
                    xSpanRemoveLocationDwelling.Style.Add("visibility", "hidden")

                    If lblDwellingNumber IsNot Nothing AndAlso QQHelper.IsNumericString(lblDwellingNumber.Text) AndAlso CInt(lblDwellingNumber.Text) > 1 Then
                        xSpanRemoveLocationDwelling.Style.Add("visibility", "visible")
                    End If
                    'Updated 10/12/2021 for BOP Endorsements Task 63816 MLW
                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse (IsQuoteEndorsement() AndAlso IsCommercialQuote()) Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemoveLocationDwelling, "visibility", "hidden")
                    End If
                End If

                If lblLocationDwellingDescription IsNot Nothing AndAlso lblLocationDwellingDescription.Text <> "" Then
                    If lblLocationDwellingDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblLocationDwellingDescription.ToolTip, " (" & lblLocationDwellingDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblLocationDwellingDescription.ToolTip &= " (" & lblLocationDwellingDescription.Text & ")"
                        End If
                    Else
                        lblLocationDwellingDescription.ToolTip = lblLocationDwellingDescription.Text
                    End If
                    If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(lblLocationDwellingDescription.ToolTip) = False AndAlso lblLocationDwellingDescription.ToolTip.Contains("Edit") = True Then 'added 6/14/2019
                        lblLocationDwellingDescription.ToolTip = lblLocationDwellingDescription.ToolTip.Replace("Edit", "View")
                        'not handling for imgBtnEditLocationDwelling since it's always hidden
                    End If
                    If imgBtnRemoveLocationDwelling IsNot Nothing Then
                        If imgBtnRemoveLocationDwelling.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemoveLocationDwelling.ToolTip, " (" & lblLocationDwellingDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemoveLocationDwelling.ToolTip &= " (" & lblLocationDwellingDescription.Text & ")"
                            End If
                        Else
                            imgBtnRemoveLocationDwelling.ToolTip = "Remove " & lblLocationDwellingDescription.Text
                        End If
                    End If
                    If xSpanRemoveLocationDwelling IsNot Nothing Then
                        If xSpanRemoveLocationDwelling.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemoveLocationDwelling.Attributes.Item("title"), " (" & lblLocationDwellingDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemoveLocationDwelling.Attributes.Item("title") &= " (" & lblLocationDwellingDescription.Text & ")"
                            End If
                        Else
                            xSpanRemoveLocationDwelling.Attributes.Item("title") = "Remove " & lblLocationDwellingDescription.Text
                        End If
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub SetupPropertyLossHistoriesRepeater(ByVal rptPropertyLossHistories As Repeater)
        If rptPropertyLossHistories IsNot Nothing Then

            'added 6/17/2020 for PPA CLUE Auto (just making it available for other LOBs if needed)
            Dim editableLossFlag As Boolean = GetEditableLossFlagForTree()
            Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = GetQuoteTransactionTypeFlagForTree()

            For Each i As RepeaterItem In rptPropertyLossHistories.Items
                Dim imgBtnRemovePropertyLossHistory As ImageButton = i.FindControl("imgBtnRemovePropertyLossHistory")
                Dim lblPropertyLossHistoryDescription As Label = i.FindControl("lblPropertyLossHistoryDescription")
                Dim xSpanRemovePropertyLossHistory As HtmlGenericControl = i.FindControl("xSpanRemovePropertyLossHistory")

                If imgBtnRemovePropertyLossHistory IsNot Nothing Then
                    imgBtnRemovePropertyLossHistory.Attributes.Add("onClick", "javascript: if (InEditMode() == true){alert('This functionality is currently locked.'); return false;}else{return confirm('Are you sure you want to remove this Property loss history?');}")

                    Dim lblPropertyLossHistoryCounter As Label = i.FindControl("PropertyLossHistoryCounter")
                    If lblPropertyLossHistoryCounter IsNot Nothing Then
                        '11/6/2014 note: this won't actually do anything since the image button is hidden; would also need to toggle visibility style of xSpan
                        If lblPropertyLossHistoryCounter.Text <> "" AndAlso IsNumeric(lblPropertyLossHistoryCounter.Text) = True Then
                            'normal... okay
                            imgBtnRemovePropertyLossHistory.Visible = True
                        Else
                            'message row; delete button not needed
                            imgBtnRemovePropertyLossHistory.Visible = False
                        End If
                    End If

                    If editableLossFlag Then
                        Dim lblPropertyLossHistorySourceId As Label = i.FindControl("lblPropertyLossHistorySourceId")
                        If lblPropertyLossHistorySourceId IsNot Nothing Then
                            Select Case lblPropertyLossHistorySourceId.Text
                                Case "1", "4", "7", "8"
                                    editableLossFlag = False
                            End Select
                        End If
                    End If

                    'added 2/22/2019
                    'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    'updated 6/17/2020; also needed to be hidden for Endorsements
                    If editableLossFlag = False OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(imgBtnRemovePropertyLossHistory, "visibility", "hidden")
                    End If
                End If

                'added 3/18/2014
                If lblPropertyLossHistoryDescription IsNot Nothing AndAlso lblPropertyLossHistoryDescription.Text <> "" Then
                    If lblPropertyLossHistoryDescription.ToolTip <> "" Then
                        If TooltipContainsDescription(lblPropertyLossHistoryDescription.ToolTip, " (" & lblPropertyLossHistoryDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                            lblPropertyLossHistoryDescription.ToolTip &= " (" & lblPropertyLossHistoryDescription.Text & ")"
                        End If
                    Else
                        lblPropertyLossHistoryDescription.ToolTip = lblPropertyLossHistoryDescription.Text
                    End If
                    If imgBtnRemovePropertyLossHistory IsNot Nothing Then
                        If imgBtnRemovePropertyLossHistory.ToolTip <> "" Then
                            If TooltipContainsDescription(imgBtnRemovePropertyLossHistory.ToolTip, " (" & lblPropertyLossHistoryDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                imgBtnRemovePropertyLossHistory.ToolTip &= " (" & lblPropertyLossHistoryDescription.Text & ")"
                            End If
                        Else
                            imgBtnRemovePropertyLossHistory.ToolTip = "Remove " & lblPropertyLossHistoryDescription.Text
                        End If
                    End If
                    If xSpanRemovePropertyLossHistory IsNot Nothing Then
                        If xSpanRemovePropertyLossHistory.Attributes.Item("title") <> "" Then
                            If TooltipContainsDescription(xSpanRemovePropertyLossHistory.Attributes.Item("title"), " (" & lblPropertyLossHistoryDescription.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                                xSpanRemovePropertyLossHistory.Attributes.Item("title") &= " (" & lblPropertyLossHistoryDescription.Text & ")"
                            End If
                        Else
                            xSpanRemovePropertyLossHistory.Attributes.Item("title") = "Remove " & lblPropertyLossHistoryDescription.Text
                        End If
                    End If
                End If

                'added 2/22/2019
                'updated 6/17/2020; also needed to be hidden for Endorsements
                'If GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If editableLossFlag = False OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If xSpanRemovePropertyLossHistory IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(xSpanRemovePropertyLossHistory, "visibility", "hidden")
                    End If
                End If
            Next
        End If
    End Sub
#End Region

#Region "Third Party Reports"
    Private Sub LoadCreditReports(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef hasCreditReports As Boolean, ByVal qq As QuickQuoteObject, ByVal governingstate As QuickQuoteObject)
        hasCreditReports = False

        If qq IsNot Nothing AndAlso Me.liCreditReports.Visible = True Then
            Dim entityType As CreditReportEntityType = Nothing
            If qq.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                entityType = CreditReportEntityType.Driver
            Else
                entityType = CreditReportEntityType.Applicant
            End If
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote AndAlso Me.Quote IsNot Nothing AndAlso Me.pnlCreditReports.Visible = True Then
                'already loaded credit reports from LoadRatedQuoteIntoTree; doesn't need to rebind repeater to datatable but will switch hasCreditReports to True so other logic will work the same
                hasCreditReports = True
            Else
                'updated 11/7/2014 for TieringInfo
                Me.lblRatedTier.Text = ""
                If Me.SubQuoteFirst.TieringInformation IsNot Nothing AndAlso Me.SubQuoteFirst.TieringInformation.RatedTier <> "" Then '1/8/2015 note: policy level uses rated tier and individual applicant tiers are shown w/ tierLevelId (tierLevelId = 0 at policy level)
                    Me.lblRatedTier.Text = "Rated Tier " & Me.SubQuoteFirst.TieringInformation.RatedTier
                End If

                Dim dtCreditReports As New DataTable
                dtCreditReports.Columns.AddStrings("CreditReportDescription", "CreditReportEntityType", "CreditReportEntityNumber", "CreditReportUnitNumber")
                Dim hasPolicyholder1CreditReport As Boolean = False
                Dim hasPolicyholder2CreditReport As Boolean = False
                Dim qqNum As Integer = 0

                If entityType = CreditReportEntityType.Driver Then
                    If governingstate.Drivers IsNot Nothing AndAlso governingstate.Drivers.Count > 0 Then
                        For Each d As QuickQuoteDriver In governingstate.Drivers
                            qqNum += 1
                            If hasPolicyholder1CreditReport = False OrElse hasPolicyholder2CreditReport = False Then
                                If d.HasValidDriverNum = True AndAlso d.RelationshipTypeId <> "" AndAlso IsNumeric(d.RelationshipTypeId) = True Then
                                    'updated 11/7/2014 for TieringInfo
                                    AddCreditReportRowToDataTable(dtCreditReports, CreditReportEntityType.Driver, d.Name, CInt(d.RelationshipTypeId), d.TieringInformation, qqNum, d.DriverNum, hasPolicyholder1CreditReport, hasPolicyholder2CreditReport)
                                End If
                            End If
                        Next
                    End If
                Else
                    'assume Applicant
                    If Me.GoverningStateQuote.Applicants IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants.Count > 0 Then
                        For Each a As QuickQuoteApplicant In Me.GoverningStateQuote.Applicants
                            qqNum += 1
                            If hasPolicyholder1CreditReport = False OrElse hasPolicyholder2CreditReport = False Then
                                If a.HasValidApplicantNum = True AndAlso a.RelationshipTypeId <> "" AndAlso IsNumeric(a.RelationshipTypeId) = True Then
                                    'updated 11/7/2014 for TieringInfo
                                    AddCreditReportRowToDataTable(dtCreditReports, CreditReportEntityType.Applicant, a.Name, CInt(a.RelationshipTypeId), a.TieringInformation, qqNum, a.ApplicantNum, hasPolicyholder1CreditReport, hasPolicyholder2CreditReport)
                                End If
                            End If
                        Next
                    End If
                End If

                If hasPolicyholder1CreditReport = True OrElse hasPolicyholder2CreditReport = True Then
                    hasCreditReports = True
                    Me.rptCreditReports.DataSource = dtCreditReports
                    Me.rptCreditReports.DataBind()
                End If
            End If
        End If

        If Me.lblRatedTier.Text <> "" Then
            Me.RatedTierSection.Visible = True
        Else
            Me.RatedTierSection.Visible = False
        End If

        If hasCreditReports = True Then
            Me.pnlCreditReports.Visible = True
            Me.MainCreditReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            Me.cr_checkMarkArea.Visible = True
            Me.cr_xImageArea.Visible = False
            IsCreditReportsSectionEnabled = True 'may not use here
        Else 'was previously only being done from LoadTreeView (not LoadRatedQuoteIntoTree); could use IF to only do when checking normal Me.Quote and not RatedQuoteObject
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote Then 'added IF 9/29/2014 so existing section never gets wiped out when loading rated quote (if it failed or something)
                Me.pnlCreditReports.Visible = False
                Me.MainCreditReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.cr_checkMarkArea.Visible = False
                Me.cr_xImageArea.Visible = False 'would be set to True if coded like other sections... may use True if we want to indicate that credit reports are eventually needed (will currently be ordered at Rate)
                IsCreditReportsSectionEnabled = False 'may not use here
            End If
        End If
    End Sub
    Private Sub AddCreditReportRowToDataTable(ByRef dtCreditReports As DataTable, ByVal entityType As CreditReportEntityType, ByVal qqName As QuickQuoteName, ByVal relationshipTypeId As Integer, ByVal tieringInfo As QuickQuoteTieringInformation, ByVal qqNum As Integer, ByVal diaNum As Integer, ByRef hasPolicyholder1CreditReport As Boolean, ByRef hasPolicyholder2CreditReport As Boolean)
        If (relationshipTypeId = 8 AndAlso hasPolicyholder1CreditReport = False) OrElse (relationshipTypeId = 5 AndAlso hasPolicyholder2CreditReport = False) Then
            Dim entityText As String = ""
            If entityType = CreditReportEntityType.Driver Then
                entityText = "Driver"
            Else
                entityText = "Applicant"
            End If

            Dim drCreditReport As DataRow
            drCreditReport = dtCreditReports.NewRow
            If qqName IsNot Nothing AndAlso qqName.DisplayName <> "" Then
                drCreditReport.Item("CreditReportDescription") = qqName.DisplayName
            Else
                drCreditReport.Item("CreditReportDescription") = entityText & " " & qqNum.ToString
            End If

            If tieringInfo IsNot Nothing Then
                Dim lobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
                If Me.Quote IsNot Nothing Then
                    lobType = Me.Quote.LobType
                End If

                If lobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    If tieringInfo.RatedTier <> "" Then
                        drCreditReport.Item("CreditReportDescription") &= " (Rated Tier " & tieringInfo.RatedTier & ")"
                    ElseIf tieringInfo.TierLevelId <> "" Then
                        drCreditReport.Item("CreditReportDescription") &= " (Tier Level " & tieringInfo.TierLevelId & ")"
                    End If
                Else
                    If tieringInfo.TierLevelId <> "" Then
                        drCreditReport.Item("CreditReportDescription") &= " (Tier Level " & tieringInfo.TierLevelId & ")"
                    ElseIf tieringInfo.RatedTier <> "" Then
                        drCreditReport.Item("CreditReportDescription") &= " (Rated Tier " & tieringInfo.RatedTier & ")"
                    End If
                End If
            End If
            If relationshipTypeId = 8 Then
                hasPolicyholder1CreditReport = True
            Else
                hasPolicyholder2CreditReport = True
            End If
            drCreditReport.Item("CreditReportEntityType") = entityText
            drCreditReport.Item("CreditReportEntityNumber") = qqNum.ToString
            drCreditReport.Item("CreditReportUnitNumber") = diaNum.ToString
            dtCreditReports.Rows.Add(drCreditReport)
        End If
    End Sub
    Private Sub LoadMvrReports(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef hasMvrReports As Boolean, ByVal governingState As QuickQuoteObject)
        hasMvrReports = False

        'Updated 04/15/2021 for CAP Endorsements Task 52977 MLW
        If governingState IsNot Nothing AndAlso Me.liMvrReports.Visible = True AndAlso MvrEnabled() = True Then
            'If governingState IsNot Nothing AndAlso Me.liMvrReports.Visible = True Then
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote AndAlso Me.Quote IsNot Nothing AndAlso Me.pnlMvrReports.Visible = True Then
                'already loaded mvr reports from LoadRatedQuoteIntoTree; doesn't need to rebind repeater to datatable but will switch hasMvrReports to True so other logic will work the same
                hasMvrReports = True
            Else
                'If Me.IsOnAppPage AndAlso governingState.Drivers IsNot Nothing AndAlso governingState.Drivers.Count > 0 Then
                'updated 6/14/2019 for Endorsements/ReadOnly
                If (Me.IsOnAppPage OrElse (Me.Quote IsNot Nothing AndAlso (Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage))) AndAlso governingState.Drivers IsNot Nothing AndAlso governingState.Drivers.Count > 0 Then
                    Dim dtMvrReports As New DataTable
                    dtMvrReports.Columns.AddStrings("MvrReportDescription", "MvrReportEntityType", "MvrReportEntityNumber", "MvrReportUnitNumber")
                    Dim qqNum As Integer = 0

                    For Each d As QuickQuoteDriver In governingState.Drivers
                        qqNum += 1
                        'can only order mvr reports on rated drivers (DriverExcludeTypeId = "1" 'Rated); may update to use static data table
                        If d.HasValidDriverNum = True AndAlso d.DriverExcludeTypeId <> "" AndAlso IsNumeric(d.DriverExcludeTypeId) = True Then
                            'Updated 02/10/2021 for CAP Endorsements Task 52977 MLW - MVR now also reports on DriverExcludeTYpeId = 4 Included CAP Endorsement NEW drivers only. PPA does not have option for Included (4).
                            'If CInt(d.DriverExcludeTypeId) = 1 Then
                            If CInt(d.DriverExcludeTypeId) = 1 OrElse CInt(d.DriverExcludeTypeId) = 2 OrElse (CInt(d.DriverExcludeTypeId) = 4) Then
                                'added 6/14/2019 for Endorsements/ReadOnly
                                Dim addRow As Boolean = False
                                If Me.IsOnAppPage = True Then
                                    addRow = True
                                ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                                    If Me.Quote.PolicyCurrentStatus <> QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Pending Then
                                        'all reports should be ordered if not Pending; may need to update to handle for denied or some other status that could be an un-finished quote
                                        addRow = True
                                    Else
                                        'pending status
                                        If QQHelper.IntegerForString(Me.Quote.TransactionTypeId) = 2 Then
                                            'NewBusiness
                                            If String.IsNullOrWhiteSpace(Me.Quote.QuoteNumber) = False AndAlso String.IsNullOrWhiteSpace(Me.Quote.PolicyNumber) = False AndAlso Left(UCase(Me.Quote.PolicyNumber), 1) <> "Q" Then
                                                'promoted; should be finished w/ report ordering
                                                addRow = True
                                            Else
                                                'no quoteNum found or not promoted
                                                'could either check for something that would only be on App side (since we order MVR on app side) or look for accViol that would be from MVR (clear/noHit; no source to check like we have for CLUE)
                                                If QQHelper.HasClearOrNotHitQuickQuoteAccidentViolationRecord(d.AccidentViolations) = True Then
                                                    'clear and noHit records should only come from MVR
                                                    addRow = True
                                                End If
                                            End If
                                        Else
                                            'not NewBusiness
                                            If QQHelper.IsPositiveDecimalString(Me.Quote.TotalQuotedPremium) = True Then
                                                'image has been rated; assume that all saved drivers are okay... would need to either look for existing report or know if quote was rated since driver was added to know if report really should be there
                                                addRow = True
                                            Else
                                                'quote doesn't appear to be rated; just show report for pre-existing drivers
                                                'If QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote.TransactionEffectiveDate, Me.Quote.EffectiveDate, Me.Quote.PCAdded_Date) = False Then
                                                'updated 7/25/2019 to use new method
                                                If QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote) = False Then
                                                    addRow = True
                                                Else
                                                    'new driver; could look for accViol that would be from MVR (clear/noHit; no source to check like we have for CLUE)... since we don't allow entry for endorsements, any found should be from MVR
                                                    If d.AccidentViolations IsNot Nothing AndAlso d.AccidentViolations.Count > 0 Then
                                                        addRow = True
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    'Updated 04/16/2021 for CAP Endorsements Task 52977 MLW - NOTE: CAP NB does not have the MVR Reports section, only endorsements
                                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                                        If QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote) Then
                                            addRow = True
                                        End If
                                    Else
                                        If QQHelper.IsPositiveDecimalString(Me.Quote.TotalQuotedPremium) = True Then
                                            'endorsement has been rated; assume that all saved drivers are okay... would need to either look for existing report or know if quote was rated since driver was added to know if report really should be there
                                            addRow = True
                                        Else
                                            'quote doesn't appear to be rated; just show report for pre-existing drivers
                                            'If QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote.TransactionEffectiveDate, Me.Quote.EffectiveDate, Me.Quote.PCAdded_Date) = False Then
                                            'updated 7/25/2019 to use new method
                                            If QQHelper.IsQuickQuoteDriverNewToImage(d, Me.Quote) = False Then
                                                addRow = True
                                            Else
                                                'new driver; could look for accViol that would be from MVR (clear/noHit; no source to check like we have for CLUE)... since we don't allow entry for endorsements, any found should be from MVR
                                                If d.AccidentViolations IsNot Nothing AndAlso d.AccidentViolations.Count > 0 Then
                                                    addRow = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If

                                If addRow = True Then 'added IF 6/14/2019 for Endorsements/ReadOnly; previously happening every time if on app page
                                    hasMvrReports = True

                                    Dim drMvrReport As DataRow
                                    drMvrReport = dtMvrReports.NewRow
                                    If d.Name IsNot Nothing AndAlso d.Name.DisplayName <> "" Then
                                        drMvrReport.Item("MvrReportDescription") = d.Name.DisplayName
                                    Else
                                        drMvrReport.Item("MvrReportDescription") = "Driver " & qqNum.ToString
                                    End If
                                    drMvrReport.Item("MvrReportEntityType") = "Driver"
                                    drMvrReport.Item("MvrReportEntityNumber") = qqNum
                                    drMvrReport.Item("MvrReportUnitNumber") = d.DriverNum
                                    dtMvrReports.Rows.Add(drMvrReport)
                                End If
                            End If
                        End If
                    Next

                    If hasMvrReports = True Then
                        Me.rptMvrReports.DataSource = dtMvrReports
                        Me.rptMvrReports.DataBind()
                    End If
                End If
            End If
        End If

        If hasMvrReports = True Then
            Me.pnlMvrReports.Visible = True
            Me.MainMvrReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            Me.mvr_checkMarkArea.Visible = True
            Me.mvr_xImageArea.Visible = False
            IsMvrReportsSectionEnabled = True 'may not use here
        Else 'was previously only being done from LoadTreeView (not LoadRatedQuoteIntoTree); could use IF to only do when checking normal Me.Quote and not RatedQuoteObject
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote Then 'added IF 9/29/2014 so existing section never gets wiped out when loading rated quote (if it failed or something)
                Me.pnlMvrReports.Visible = False
                Me.MainMvrReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.mvr_checkMarkArea.Visible = False
                Me.mvr_xImageArea.Visible = False 'would be set to True if coded like other sections... may use True if we want to indicate that credit reports are eventually needed (will currently be ordered at Rate)
                IsMvrReportsSectionEnabled = False 'may not use here
            End If
        End If
    End Sub

    'Added 04/15/2021 for CAP Endorsements Task 52977 MLW
    Private Function MvrEnabled() As Boolean
        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" Then
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Private Sub LoadClueReports(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef hasClueReports As Boolean)
        hasClueReports = False

        If Me.Quote IsNot Nothing AndAlso Me.liClueReports.Visible = True Then
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote AndAlso Me.pnlClueReports.Visible = True Then
                'already loaded mvr reports; doesn't need to rebind repeater to datatable but will switch hasClueReports to True so other logic will work the same
                hasClueReports = True
            Else
                'If Me.IsOnAppPage AndAlso Me.Quote.QuoteNumber <> "" Then
                'updated 2/15/2019 to also work for Endorsements and ReadOnly images
                'If Me.IsOnAppPage AndAlso Me.Quote.PolicyNumber <> "" Then
                'updated 6/17/2019 for Endorsements/ReadOnly
                If (Me.IsOnAppPage OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) AndAlso Me.Quote.PolicyNumber <> "" Then
                    'added 6/17/2019 for Endorsements/ReadOnly
                    Dim addRow As Boolean = False
                    If Me.IsOnAppPage = True Then
                        addRow = True
                    Else
                        If Me.Quote.PolicyCurrentStatus <> QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Pending Then
                            'all reports should be ordered if not Pending; may need to update to handle for denied or some other status that could be an un-finished quote
                            addRow = True
                        Else
                            'pending status
                            If QQHelper.IntegerForString(Me.Quote.TransactionTypeId) = 2 Then
                                'NewBusiness
                                If String.IsNullOrWhiteSpace(Me.Quote.QuoteNumber) = False AndAlso String.IsNullOrWhiteSpace(Me.Quote.PolicyNumber) = False AndAlso Left(UCase(Me.Quote.PolicyNumber), 1) <> "Q" Then
                                    'promoted; should be finished w/ report ordering
                                    addRow = True
                                Else
                                    'no quoteNum found or not promoted
                                    'could either check for something that would only be on App side or look for lossHist that would be from CLUE (check source)
                                    Dim clueSourceIds As New List(Of Integer)
                                    clueSourceIds.Add(1) 'CLUE Report
                                    clueSourceIds.Add(4) 'Overridden CLUE
                                    clueSourceIds.Add(7) 'Summary - CLUE
                                    clueSourceIds.Add(8) 'Summary - Overridden CLUE
                                    If QQHelper.QuoteHasQuickQuoteLossHistoryRecordForAnySourceIdInList(Me.Quote, clueSourceIds) = True Then
                                        'sources in list should indicate a CLUE report exists
                                        addRow = True
                                    End If
                                End If
                            Else
                                'not NewBusiness; all reports should already be there
                                addRow = True
                            End If
                        End If
                    End If

                    If addRow = True Then 'added IF 6/17/2019 for Endorsements/ReadOnly; previously happening every time if on app page
                        hasClueReports = True
                        Dim dtClueReports As New DataTable
                        dtClueReports.Columns.AddStrings("ClueReportDescription", "ClueReportEntityType", "ClueReportUnitNumber")

                        Dim drClueReport As DataRow
                        drClueReport = dtClueReports.NewRow
                        'drClueReport.Item("ClueReportDescription") = Me.Quote.QuoteNumber
                        'updated 2/15/2019 to also work for Endorsements and ReadOnly images
                        drClueReport.Item("ClueReportDescription") = Me.Quote.PolicyNumber
                        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then 'added IF 9/25/2014 to hold original logic; ELSE was added for HOM
                            drClueReport.Item("ClueReportEntityType") = "Auto"
                        Else
                            drClueReport.Item("ClueReportEntityType") = "Property"
                        End If
                        drClueReport.Item("ClueReportUnitNumber") = "0"
                        dtClueReports.Rows.Add(drClueReport)

                        Me.rptClueReports.DataSource = dtClueReports
                        Me.rptClueReports.DataBind()
                    End If
                End If
            End If
        End If

        If hasClueReports = True Then
            Me.pnlClueReports.Visible = True
            Me.MainClueReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            Me.clue_checkMarkArea.Visible = True
            Me.clue_xImageArea.Visible = False
            IsClueReportsSectionEnabled = True 'may not use here
        Else 'was previously only being done from LoadTreeView (not LoadRatedQuoteIntoTree); could use IF to only do when checking normal Me.Quote and not RatedQuoteObject
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote Then 'added IF 9/29/2014 so existing section never gets wiped out when loading rated quote (if it failed or something)
                Me.pnlClueReports.Visible = False
                Me.MainClueReportSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.clue_checkMarkArea.Visible = False
                Me.clue_xImageArea.Visible = False 'would be set to True if coded like other sections... may use True if we want to indicate that credit reports are eventually needed (will currently be ordered at Rate)
                IsClueReportsSectionEnabled = False 'may not use here
            End If
        End If
    End Sub
    Private Sub ctlTreeView_ViewVeriskProtectionClassReport(locationNumber As Integer) Handles Me.ViewVeriskProtectionClassReport
        Dim Err As String = Nothing
        Dim ReportFile As Byte() = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOM_DFR_GetPCCReport(Me.Quote, Err, True)
        If ReportFile IsNot Nothing Then
            Response.ContentType = "application/pdf"
            'Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ProtectionClassReport_{0}.pdf", Me.Quote.QuoteNumber))
            'updated 2/15/2019 to also work for Endorsements and ReadOnly images
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ProtectionClassReport_{0}.pdf", Me.Quote.PolicyNumber))
            Response.BinaryWrite(ReportFile)
        Else
            Me.VRScript.ShowAlert("alert('" + Server.HtmlEncode(Err) + "');")
        End If
    End Sub
#End Region

#Region "Random Helpers"
    'added IF 5/13/2014 for use on tooltips/titles... to prevent possibility of description being duplicated
    Private Function WordContainsText(ByVal textToEvaluate As String, ByVal textToFind As String) As Boolean
        If textToEvaluate <> "" AndAlso textToFind <> "" AndAlso UCase(textToEvaluate).Contains(UCase(textToFind)) = True Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function TooltipContainsDescription(ByVal tooltip As String, ByVal description As String) As Boolean
        Return WordContainsText(tooltip, description)
        'currently only works if description is the same as what it previously was... no match found when it changes (i.e. like on Quote Description or Effective Date tooltips)
        'now using more generic method that just looks for " ("; just going back to previous logic since the main problem was that the tooltip wasn't being reset after a Save... now being set to 'Edit Quote Description' or 'Edit Effective Date' in LoadTreeView method
        'Return TooltipAppearsToContainDescription(tooltip)
    End Function
    Private Function TooltipAppearsToContainDescription(ByVal tooltip As String, Optional ByVal descriptionText As String = " (") As Boolean
        If tooltip <> "" AndAlso descriptionText <> "" AndAlso UCase(tooltip).Contains(UCase(descriptionText)) = True Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetEnabledDisabledText(ByVal enabled As Boolean) As String
        If enabled = False Then
            Return "disabled"
        Else
            Return "enabled"
        End If
    End Function
    Private Function IsDisabledText(ByVal enabledOrDisabledText As String) As Boolean
        Return enabledOrDisabledText = GetEnabledDisabledText(False)
    End Function
    Private Function GetTrueOrFalseText(ByVal trueOrFalse As Boolean) As String 'set return type 1/6/2015
        If trueOrFalse = Nothing Then 'may not be needed... if False evaluates as Nothing
            trueOrFalse = False
        End If
        Return LCase(trueOrFalse).ToString
    End Function
    Public Sub SetMinimumAndMaximumEffectiveDates(Optional ByVal qqo As QuickQuoteObject = Nothing)
        Dim minEffDateAllQuotes As String = DateAdd(DateInterval.Day, MinimumEffectiveDateDaysFromToday, Date.Today).ToShortDateString
        Dim maxEffDateAllQuotes As String = DateAdd(DateInterval.Day, MaximumEffectiveDateDaysFromToday, Date.Today).ToShortDateString

        'added 2/17/2020
        Dim altMinDate As String = ""
        Dim altMaxDate As String = ""
        SetAlternateMinAndMaxDatesForQuote(qqo, altMinDate, altMaxDate)
        If QQHelper.IsValidDateString(altMinDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso QQHelper.IsValidDateString(altMaxDate, mustBeGreaterThanDefaultDate:=True) = True Then
            minEffDateAllQuotes = altMinDate
            maxEffDateAllQuotes = altMaxDate
        End If

        'added 5/17/2023
        If qqo IsNot Nothing AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            Dim beforeDtMsg As String = ""
            Dim afterDtMsg As String = ""
            Helpers.WebHelper_Personal.Check_NewBusiness_Min_and_Max_Dates(minEffDateAllQuotes, maxEffDateAllQuotes, lob:=qqo.LobType, beforeMinDateMsg:=beforeDtMsg, afterMaxDateMsg:=afterDtMsg)
            BeforeDateMsg = beforeDtMsg
            AfterDateMsg = afterDtMsg
        End If

        Dim minEffDate As String = minEffDateAllQuotes
        Dim maxEffDate As String = maxEffDateAllQuotes

        Dim _QuoteHasMinimumEffectiveDate As Boolean = False
        Dim _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean = False

        QuoteHasMinimumEffectiveDate = _QuoteHasMinimumEffectiveDate
        MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes
        MinimumEffectiveDateAllQuotes = minEffDateAllQuotes
        MaximumEffectiveDateAllQuotes = maxEffDateAllQuotes
        MinimumEffectiveDate = minEffDate
        MaximumEffectiveDate = maxEffDate
    End Sub
    Private Sub SetAlternateMinAndMaxDatesForQuote(ByVal qqo As QuickQuoteObject, ByRef minDate As String, ByRef maxDate As String) 'added 2/17/2020
        minDate = ""
        maxDate = ""

        If qqo IsNot Nothing Then
            If qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Dim dtMin As Date = EndorsementPastDate
                Dim dtMax As Date = EndorsementFutureDate

                If QQHelper.IsValidDateString(dtMin.ToString, mustBeGreaterThanDefaultDate:=True) = True AndAlso QQHelper.IsValidDateString(dtMax.ToString, mustBeGreaterThanDefaultDate:=True) = True Then
                    If QQHelper.IsValidDateString(qqo.TransactionEffectiveDate, mustBeGreaterThanDefaultDate:=True) = True Then
                        Dim dtTran As Date = CDate(qqo.TransactionEffectiveDate)
                        If dtTran < dtMin Then
                            dtMin = dtTran
                        End If
                        If dtTran > dtMax Then
                            dtMax = dtTran
                        End If
                    End If

                    minDate = dtMin.ToShortDateString
                    maxDate = dtMax.ToShortDateString
                End If
            End If
        End If
    End Sub
    Private Function CanNotAccessSection(sectionEnabledIndicator As Boolean, errMsg As String) As Boolean
        'If InEditMode = False Then
        '    Me.VRScript.ShowAlert("This functionality is currently locked.")
        '    Return True
        'End If
        If sectionEnabledIndicator = False Then
            Me.VRScript.ShowAlert(errMsg)
            Return True
        End If
        Return False
    End Function
#End Region

    Private Sub ResetQuoteDescriptionAndEffectiveDateToOriginalLabels(Optional ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal termEffDate As String = "", Optional ByVal termExpDate As String = "", Optional ByVal tranEffDate As String = "", Optional ByVal tranExpDate As String = "") 'updated 7/23/2019 w/ optional params
        'SetQuoteDescriptionLabel()
        'Me.lblEffectiveDate.Text = Me.hdnOriginalEffectiveDate.Value

        'If Me.hdnOriginalQuoteDescription.Value <> "" Then
        '    If Me.lblQuoteDescription.ToolTip <> "" Then
        '        If TooltipContainsDescription(Me.lblQuoteDescription.ToolTip, " (" & Me.hdnOriginalQuoteDescription.Value & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
        '            Me.lblQuoteDescription.ToolTip &= " (" & Me.hdnOriginalQuoteDescription.Value & ")"
        '        End If
        '    Else
        '        Me.lblQuoteDescription.ToolTip = Me.hdnOriginalQuoteDescription.Value
        '    End If
        'End If
        'If Me.lblEffectiveDate.Text <> "" Then
        '    If Me.lblEffectiveDate.ToolTip <> "" Then
        '        If TooltipContainsDescription(Me.lblEffectiveDate.ToolTip, " (" & Me.lblEffectiveDate.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
        '            Me.lblEffectiveDate.ToolTip &= " (" & Me.lblEffectiveDate.Text & ")"
        '        End If
        '    Else
        '        Me.lblEffectiveDate.ToolTip = Me.lblEffectiveDate.Text
        '    End If
        'End If

        'updated 7/23/2019
        SetQuoteDescriptionLabel()
        If Me.hdnOriginalQuoteDescription.Value <> "" Then
            If Me.lblQuoteDescription.ToolTip <> "" Then
                If TooltipContainsDescription(Me.lblQuoteDescription.ToolTip, " (" & Me.hdnOriginalQuoteDescription.Value & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                    Me.lblQuoteDescription.ToolTip &= " (" & Me.hdnOriginalQuoteDescription.Value & ")"
                End If
            Else
                Me.lblQuoteDescription.ToolTip = Me.hdnOriginalQuoteDescription.Value
            End If
        End If

        Dim useOriginalEffDateLogic As Boolean = True
        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromWebControl(Me.lblEffectiveDate, "display")
        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromWebControl(Me.lblEffectiveDate, "vertical-align")
        IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromGenericControl(Me.lblEffDateTextBoldTag, "vertical-align")
        'IFM.VR.Web.Helpers.WebHelper_Personal.RemoveStyleFromWebControl(Me.lblEffectiveDate, "float")
        If tranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
            If QQHelper.IsValidDateString(termEffDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso QQHelper.IsValidDateString(termExpDate, mustBeGreaterThanDefaultDate:=True) = True Then
                useOriginalEffDateLogic = False
                Me.lblEffDateText.Text = "Policy Term"
                Me.lblEffectiveDate.Text = termEffDate & " - " & termExpDate

                If QQHelper.IsValidDateString(tranEffDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(tranEffDate) <> CDate(termEffDate) Then
                    Me.lblEffectiveDate.Text &= "<br />(effective " & tranEffDate & ")"
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(Me.lblEffectiveDate, "display", "inline-block")
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(Me.lblEffectiveDate, "vertical-align", "top")
                    IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToGenericControl(Me.lblEffDateTextBoldTag, "vertical-align", "top")
                    'IFM.VR.Web.Helpers.WebHelper_Personal.AddStyleToWebControl(Me.lblEffectiveDate, "float", "right")
                    'Me.lblEffectiveDate.Text = "<table><tr><td>"
                    'Me.lblEffectiveDate.Text &= termEffDate & " - " & termExpDate
                    'Me.lblEffectiveDate.Text &= "<br />(" & tranEffDate & ")"
                    'Me.lblEffectiveDate.Text &= "</td></tr></table>"
                End If
            End If
        End If

        If useOriginalEffDateLogic = True Then
            Me.lblEffDateText.Text = "Effective Date"
            Me.lblEffectiveDate.Text = Me.hdnOriginalEffectiveDate.Value
            If Me.lblEffectiveDate.Text <> "" Then
                If Me.lblEffectiveDate.ToolTip <> "" Then
                    If TooltipContainsDescription(Me.lblEffectiveDate.ToolTip, " (" & Me.lblEffectiveDate.Text & ")") = False Then 'added IF 5/13/2014 to prevent possibility of description being duplicated; was previously adding description every time
                        Me.lblEffectiveDate.ToolTip &= " (" & Me.lblEffectiveDate.Text & ")"
                    End If
                Else
                    Me.lblEffectiveDate.ToolTip = Me.lblEffectiveDate.Text
                End If
            End If
        End If

    End Sub
    Private Sub SetQuoteDescriptionLabel()
        If UseQuoteNumberHeader = False Then
            Me.lblQuoteDescription.Text = QQHelper.appendText(Me.hdnQuoteNumber.Value, Me.hdnOriginalQuoteDescription.Value, " - ")
        Else
            Me.lblQuoteDescription.Text = Me.hdnOriginalQuoteDescription.Value
        End If
    End Sub
    Private Sub SetQuoteNumberLabelAndOptionallyShowHeader()
        Me.lblQuoteNumber.Text = Me.hdnQuoteNumber.Value
        If Me.pnlTreeView.Visible = True AndAlso Me.lblQuoteNumber.Text <> "" AndAlso UseQuoteNumberHeader = True Then
            Me.liQuoteNumber.Visible = True
        Else
            Me.liQuoteNumber.Visible = False
        End If
    End Sub

    Private Sub AddPolicyholder()
        If Me.lblNumberOfPolicyholders.Text <> "" AndAlso IsNumeric(Me.lblNumberOfPolicyholders.Text) = True AndAlso Me.Quote IsNot Nothing Then
            Dim policyholderNumberToEdit As Integer = 0
            Select Case CInt(Me.lblNumberOfPolicyholders.Text)
                Case 0
                    'add 1st policyholder placeholder and change count to 1
                    'leave add area visible
                    policyholderNumberToEdit = 1
                    Me.lblNumberOfPolicyholders.Text = "1"
                    Me.AddPolicyholderArea.Visible = True
                Case 1
                    'add 2nd policyholder placeholder and change count to 2
                    'hide add area
                    policyholderNumberToEdit = 2
                    Me.lblNumberOfPolicyholders.Text = "2"
                    Me.AddPolicyholderArea.Visible = False
                Case Else
                    'button shouldn't have been showing in the 1st place

            End Select
            If policyholderNumberToEdit > 0 Then
                Dim okayToContinue As Boolean = True

                Dim ph1Applicant As QuickQuoteApplicant = Nothing
                Dim ph2Applicant As QuickQuoteApplicant = Nothing
                Dim ph1AppNum As Integer = 0
                Dim ph2AppNum As Integer = 0
                If Me.GoverningStateQuote.Applicants.IsLoaded Then
                    Dim appCounter As Integer = 0
                    For Each app As QuickQuoteApplicant In Me.GoverningStateQuote.Applicants
                        appCounter += 1
                        Dim isPolicyholder1 As Boolean = False
                        Dim isPolicyholder2 As Boolean = False
                        If QQHelper.IsQuickQuoteApplicantPolicyholder(app, isPolicyholder1, isPolicyholder2) = True Then
                            If ph1Applicant Is Nothing AndAlso isPolicyholder1 = True Then
                                ph1Applicant = app
                                ph1AppNum = appCounter
                            ElseIf ph2Applicant Is Nothing AndAlso isPolicyholder2 = True Then
                                ph2Applicant = app
                                ph2AppNum = appCounter
                            End If
                        End If
                        If ph1Applicant IsNot Nothing AndAlso ph2Applicant IsNot Nothing Then
                            Exit For
                        End If
                    Next
                End If

                Dim dtPolicyholders As New DataTable
                dtPolicyholders.Columns.AddStrings("PolicyholderDescription", "PolicyholderNumber", "Applicant", "ApplicantNumber", "Applicant2", "Applicant2Number")
                Dim drPH1 As DataRow
                drPH1 = dtPolicyholders.NewRow
                drPH1.Item("PolicyholderNumber") = "1"
                If policyholderNumberToEdit = 1 Then
                    okayToContinue = False
                    drPH1.Item("PolicyholderDescription") = "Policyholder 1"
                Else
                    drPH1.Item("PolicyholderDescription") = If(Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing AndAlso Me.Quote.Policyholder.Name.DisplayName <> "", Me.Quote.Policyholder.Name.DisplayName, "Policyholder 1")
                End If

                'added 7/20/2015 for FAR (should've been added 5/21/2015 when logic was updated in LoadTreeView method)
                If ph1Applicant IsNot Nothing AndAlso ph1Applicant.Name IsNot Nothing AndAlso ph1Applicant.Name.DisplayName <> "" AndAlso UCase(ph1Applicant.Name.DisplayName) <> UCase(drPH1.Item("PolicyholderDescription").ToString) Then
                    drPH1.Item("Applicant") = ph1Applicant.Name.DisplayName
                    drPH1.Item("ApplicantNumber") = ph1AppNum.ToString

                    If okayToContinue = False AndAlso ph2Applicant IsNot Nothing AndAlso ph2Applicant.Name IsNot Nothing AndAlso ph2Applicant.Name.DisplayName <> "" Then
                        drPH1.Item("Applicant2") = ph2Applicant.Name.DisplayName
                        drPH1.Item("Applicant2Number") = ph2AppNum.ToString
                    Else
                        drPH1.Item("Applicant2") = ""
                        drPH1.Item("Applicant2Number") = ""
                    End If
                Else
                    drPH1.Item("Applicant") = ""
                    drPH1.Item("ApplicantNumber") = ""
                    drPH1.Item("Applicant2") = ""
                    drPH1.Item("Applicant2Number") = ""
                End If

                dtPolicyholders.Rows.Add(drPH1)
                If okayToContinue = True Then
                    Dim drPH2 As DataRow
                    drPH2 = dtPolicyholders.NewRow
                    drPH2.Item("PolicyholderNumber") = "2"
                    If policyholderNumberToEdit = 2 Then
                        okayToContinue = False
                        drPH2.Item("PolicyholderDescription") = "Policyholder 2"
                    Else
                        drPH2.Item("PolicyholderDescription") = If(Me.Quote.Policyholder2 IsNot Nothing AndAlso Me.Quote.Policyholder2.Name IsNot Nothing AndAlso Me.Quote.Policyholder2.Name.DisplayName <> "", Me.Quote.Policyholder2.Name.DisplayName, "Policyholder 2")
                    End If

                    If ph2Applicant IsNot Nothing AndAlso ph2Applicant.Name IsNot Nothing AndAlso ph2Applicant.Name.DisplayName <> "" AndAlso UCase(ph2Applicant.Name.DisplayName) <> UCase(drPH2.Item("PolicyholderDescription").ToString) Then
                        drPH2.Item("Applicant") = ph2Applicant.Name.DisplayName
                        drPH2.Item("ApplicantNumber") = ph2AppNum.ToString
                    Else
                        drPH2.Item("Applicant") = ""
                        drPH2.Item("ApplicantNumber") = ""
                    End If
                    'added 7/20/2015; will never have multiple applicants under PH2 (should've been added 6/8/2015 when logic was updated in LoadTreeView method)
                    drPH2.Item("Applicant2") = ""
                    drPH2.Item("Applicant2Number") = ""

                    dtPolicyholders.Rows.Add(drPH2)
                End If

                Me.rptPolicyholders.DataSource = dtPolicyholders
                Me.rptPolicyholders.DataBind()


                Me.pnlPolicyholders.Visible = True
                Me.MainPolicyholderSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
                Me.ph_checkMarkArea.Visible = True 'added 2/20/2014
                Me.ph_xImageArea.Visible = False

                RaiseEvent EditPolicyholder(policyholderNumberToEdit)
            End If
        End If
    End Sub


    Private Sub MoveUpVehicleDrivers(ByRef vehicle As QuickQuoteVehicle)
        If vehicle IsNot Nothing Then
            With vehicle
                If .OccasionalDriver2Num = "" AndAlso .OccasionalDriver3Num <> "" Then
                    .OccasionalDriver2Num = .OccasionalDriver3Num
                    .OccasionalDriver3Num = ""
                End If
                If .OccasionalDriver1Num = "" AndAlso .OccasionalDriver2Num <> "" Then
                    .OccasionalDriver1Num = .OccasionalDriver2Num
                    .OccasionalDriver2Num = ""
                End If
            End With
        End If
    End Sub

    Private Sub LoadVehicles(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef hasVehicles As Boolean)
        hasVehicles = False


        If Me.Quote IsNot Nothing Then ' AndAlso Me.liVehicles.Visible = True Then 'using vehicles listItem since it'll only be visible if vehicles should show for the LOB in question
            If Me.Quote.Vehicles IsNot Nothing AndAlso Me.Quote.Vehicles.Count > 0 Then
                Me.lblNumberOfVehicles.Text = Me.Quote.Vehicles.Count.ToString 'added 1/9/2014
                hasVehicles = True
                Dim dtVehicles As New DataTable
                dtVehicles.Columns.AddStrings("VehicleDescription", "VehicleNumber")
                Dim vehicleNum As Integer = 0
                For Each v As QuickQuoteVehicle In Me.Quote.Vehicles
                    vehicleNum += 1
                    Dim drVehicle As DataRow
                    drVehicle = dtVehicles.NewRow
                    If v.Year <> "" OrElse v.Make <> "" OrElse v.Model <> "" Then
                        drVehicle.Item("VehicleDescription") = QQHelper.appendText(v.Year, QQHelper.appendText(v.Make, v.Model, " "), " ")
                        'class code should no longer appear in the vehicles tree view grid - Bug 43922 - ZTS 3/23/20
                        'If v.ClassCode <> "" Then 'added 12/30/2014
                        '    drVehicle.Item("VehicleDescription") &= " (Class: " & v.ClassCode & ")"
                        'End If
                    Else
                        'Updated 05/05/2021 for CAP Endorsements bug 61606 MLW
                        If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                            drVehicle.Item("VehicleDescription") = "0"
                        Else
                            drVehicle.Item("VehicleDescription") = "Vehicle " & vehicleNum.ToString
                        End If
                        'drVehicle.Item("VehicleDescription") = "Vehicle " & vehicleNum.ToString
                    End If
                    drVehicle.Item("VehicleNumber") = vehicleNum.ToString 'added 1/3/2014
                    dtVehicles.Rows.Add(drVehicle)
                Next
                Me.rptVehicles.DataSource = dtVehicles
                Me.rptVehicles.DataBind()
            End If

            'added 9/20/2017; vehicles not required for CAP when quote has GarageKeepers
            If hasVehicles = False AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                If Me.SubQuoteFirst.HasGarageKeepersCollision = True OrElse Me.SubQuoteFirst.HasGarageKeepersOtherThanCollision = True Then
                    hasVehicles = True
                End If
            End If
        End If

        If hasVehicles = False AndAlso quoteOrRatedQuote = QuoteOrRatedQuoteType.RatedQuote AndAlso Me.pnlVehicles.Visible = True Then
            hasVehicles = True 'couldn't pull from RatedQuote, but already pulled from normal QuoteObject
        End If

        If hasVehicles = True Then
            Me.pnlVehicles.Visible = True
            Me.MainVehicleSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            Me.v_checkMarkArea.Visible = True 'added 2/20/2014
            Me.v_xImageArea.Visible = False
        Else 'was previously only being done from LoadTreeView (not LoadRatedQuoteIntoTree); could use IF to only do when checking normal Me.Quote and not RatedQuoteObject
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote Then 'added IF 9/29/2014 so existing section never gets wiped out when loading rated quote (if it failed or something)
                Me.pnlVehicles.Visible = False
                Me.MainVehicleSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.v_checkMarkArea.Visible = False 'added 2/20/2014
                Me.v_xImageArea.Visible = True
            End If
        End If
    End Sub
    'added 6/28/2019 for common method that will be used for Quote and RatedQuote on Endorsements
    Private Sub LoadDrivers(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef hasDrivers As Boolean)
        hasDrivers = False

        If Me.GoverningStateQuote IsNot Nothing Then
            If Me.GoverningStateQuote.Drivers IsNot Nothing AndAlso Me.GoverningStateQuote.Drivers.Count > 0 Then
                Me.lblNumberOfDrivers.Text = Me.GoverningStateQuote.Drivers.Count.ToString
                hasDrivers = True
                Dim dtDrivers As New DataTable
                dtDrivers.Columns.AddStrings("DriverDescription", "DriverNumber")
                Dim driverNum As Integer = 0

                For Each d As QuickQuoteDriver In Me.GoverningStateQuote.Drivers
                    driverNum += 1
                    Dim drDriver As DataRow
                    drDriver = dtDrivers.NewRow
                    If d.Name.DisplayName <> "" Then
                        drDriver.Item("DriverDescription") = d.Name.DisplayName
                    Else
                        drDriver.Item("DriverDescription") = "Driver " & driverNum.ToString
                    End If
                    drDriver.Item("DriverNumber") = driverNum.ToString 'added 1/3/2014
                    dtDrivers.Rows.Add(drDriver)
                Next
                Me.rptDrivers.DataSource = dtDrivers
                Me.rptDrivers.DataBind()
            End If
        End If

        If hasDrivers = False AndAlso quoteOrRatedQuote = QuoteOrRatedQuoteType.RatedQuote AndAlso Me.pnlDrivers.Visible = True Then
            hasDrivers = True 'couldn't pull from RatedQuote, but already pulled from normal QuoteObject
        End If

        If hasDrivers = True Then
            Me.pnlDrivers.Visible = True
            Me.MainDriverSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "visible")
            Me.d_checkMarkArea.Visible = True
            Me.d_xImageArea.Visible = False
        Else
            If quoteOrRatedQuote = QuoteOrRatedQuoteType.Quote Then 'so existing section never gets wiped out when loading rated quote (if it failed or something)
                Me.pnlDrivers.Visible = False
                Me.MainDriverSectionSubLists_expandCollapseImageArea.Style.Add("visibility", "hidden")
                Me.d_checkMarkArea.Visible = False
                'Updated 03/11/2021 for CAP Endorsements Task 52973 MLW
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    'NOTE: d_xImageArea is the ! warning (incomplete.png) that drivers are required on the quote. For CAP endorsements, they can have a policy without drivers.
                    Me.d_xImageArea.Visible = False
                Else
                    Me.d_xImageArea.Visible = True
                End If
                'Me.d_xImageArea.Visible = True
            End If
        End If
    End Sub

    Private Function BuildingDescription(ByVal b As QuickQuoteBuilding, ByVal num As Integer) As String 'added 7/20/2015 for Farm
        Dim desc As String = ""

        If b IsNot Nothing Then
            If QQHelper.IsNumericString(b.FarmStructureTypeId) = True Then
                Dim farmStructureType As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, b.FarmStructureTypeId)
                If farmStructureType <> "" Then
                    desc = farmStructureType

                    Dim eLimit As String = b.E_Farm_Limit
                    If QQHelper.IsZeroPremium(eLimit) = False Then
                        eLimit = FormatCurrency(eLimit, 0)
                        desc &= " " & eLimit
                    End If
                End If
            End If

            If desc = "" Then
                Dim hayStorageText As String = ctl_FarmBuilding_Property.HayStorageText
                Dim updatedDesc As String = QQHelper.RemoveAllInstancesOfStringFromString(b.Description, hayStorageText)
                desc = updatedDesc
                If num > 0 Then
                    desc = QQHelper.appendText("Building " & num.ToString, updatedDesc, " - ")
                End If
            End If
        End If

        If desc = "" Then
            desc = "Building"
            If num > 0 Then
                desc &= " " & num.ToString
            End If
        End If

        Return desc
    End Function
    Private Function LocationDwellingDescription(ByVal l As QuickQuoteLocation, Optional ByVal formType As String = "") As String 'added 7/21/2015 for Farm
        Dim desc As String = ""

        If l IsNot Nothing Then

            If formType = "" Then
                If Me.Quote IsNot Nothing Then
                    'Updated 11/30/17 for HOM Upgrade MLW
                    If ((l.FormTypeId.EqualsAny("22", "25")) AndAlso l.StructureTypeId = "2" AndAlso Me.Quote.LobId = "2") Then
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim a1 As New QuickQuoteStaticDataAttribute
                        a1.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                        a1.nvp_value = 2
                        optionAttributes.Add(a1)
                        formType = QQHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, l.FormTypeId, Me.Quote.LobType)
                    Else
                        formType = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, l.FormTypeId, Me.Quote.LobType)
                    End If
                Else
                    formType = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, l.FormTypeId)
                End If
            End If

            If formType <> "" Then
                desc = formType

                Dim limitToUse As String = ""
                If UCase(formType) = "FO-4" Then
                    limitToUse = l.C_PersonalProperty_Limit
                Else
                    limitToUse = l.A_Dwelling_Limit
                End If
                If QQHelper.IsZeroPremium(limitToUse) = False Then
                    limitToUse = FormatCurrency(limitToUse, 0)
                    desc = QQHelper.appendText(desc, limitToUse, " ")
                End If
            End If

            If desc = "" Then
                desc = l.Description
                desc = QQHelper.appendText("Dwelling", desc, " - ")
            End If
        End If

        If desc = "" Then
            desc = QQHelper.appendText(formType, "Dwelling", " ")
        End If

        Return desc
    End Function



    'added 2/4/2014 to centralize logic in 1 spot
    Private Sub RaiseNewDriverEvent()
        If Me.Quote IsNot Nothing Then
            If Me.GoverningStateQuote.Drivers Is Nothing Then
                Me.GoverningStateQuote.Drivers = New List(Of QuickQuoteDriver)
            End If
            RaiseEvent NewDriver(Me.GoverningStateQuote.Drivers.Count + 1)
        End If
    End Sub
    Private Sub RaiseNewVehicleEvent()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles Is Nothing Then
                Me.Quote.Vehicles = New List(Of QuickQuoteVehicle)
            End If
            RaiseEvent NewVehicle(Me.Quote.Vehicles.Count + 1)
        End If
    End Sub
    Private Sub RaiseNewLocationEvent()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Locations Is Nothing Then
                Me.Quote.Locations = New List(Of QuickQuoteLocation)
            End If
            RaiseEvent NewLocation(Me.Quote.Locations.Count + 1)
        End If
    End Sub



    Private Sub UpdateViewMode()
        If Me.IsOnAppPage Then
            EnableDisableApplicationSectionHeaders(True)
            EnableDisableQuoteSectionHeaders(False)
        Else
            'Updated 10/13/2021 for BOP Endorsements Task 63816 MLW
            ''Added 12/21/2020 for CAP Endorsements Task 52977 MLW
            'If IsQuoteEndorsement() AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            If IsQuoteEndorsement() AndAlso IsCommercialQuote() Then
                EnableDisableEndorsementQuoteSectionHeaders(False)
            Else
                EnableDisableQuoteSectionHeaders(True)
                EnableDisableApplicationSectionHeaders(False)
            End If
            'EnableDisableQuoteSectionHeaders(True)
            'EnableDisableApplicationSectionHeaders(False)
        End If
    End Sub

    'Added 12/21/2020 for CAP Endorsements Task 52977 MLW
    Private Sub EnableDisableEndorsementQuoteSectionHeaders(ByVal enabled As Boolean)
        Dim enabledDisabledText As String = GetEnabledDisabledText(enabled)

        Me.hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = enabledDisabledText

        Dim typeOfEndorsement As String = QQDevDictionary_GetItem("Type_Of_Endorsement_Selected")
        'Updated 10/13/2021 for BOP Endorsements Task 63816 MLW
        Select Case typeOfEndorsement
            Case EndorsementStructures.EndorsementTypeString.CAP_AmendMailing,
                 EndorsementStructures.EndorsementTypeString.BOP_AmendMailing,
                 EndorsementStructures.EndorsementTypeString.CPP_AmendMailing
                Me.hdnVehicleSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnDriverSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnLocationSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 10/13/2021 for BOP Endorsements Task 63816 MLW
                Me.hdnCPPCPRLocationsSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
                Me.hdnCPPCGLLocationSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
                Me.hdnInlandMarineSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
            Case EndorsementStructures.EndorsementTypeString.CAP_AddDeleteVehicle, EndorsementStructures.EndorsementTypeString.CAP_AddDeleteAI
                Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnDriverSection_EnabledOrDisabledFlag.Value = enabledDisabledText
            Case EndorsementStructures.EndorsementTypeString.CAP_AddDeleteDriver
                Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnVehicleSection_EnabledOrDisabledFlag.Value = enabledDisabledText
            Case EndorsementStructures.EndorsementTypeString.BOP_AddDeleteLocationLienholder, 'Added 10/13/2021 for BOP Endorsements Task 63816 MLW 
                 EndorsementStructures.EndorsementTypeString.CPP_AddDeleteLocationLienholder 'Updated 12/21/2021 for CPP Endorsements Task 66834 MLW
                '"Add/Delete a location(s) or lienholder(s) on property"
                Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnInlandMarineSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
            Case EndorsementStructures.EndorsementTypeString.CPP_AddDeleteContractorsEquipmentLienholder 'Added 12/22/2021 for CPP Endorsements Task 66834 MLW
                Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnCPPCPRLocationsSection_EnabledOrDisabledFlag.Value = enabledDisabledText
                Me.hdnCPPCGLLocationSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'Added 12/21/2021 for CPP Endorsements Task 66834 MLW
        End Select
    End Sub
    Private Sub EnableDisableQuoteSectionHeaders(ByVal enabled As Boolean)
        Dim enabledDisabledText As String = GetEnabledDisabledText(enabled)

        Me.hdnQuoteDescriptionSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnEffectiveDateSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnPolicyholderSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnDriverSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnVehicleSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnLocationSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnCoverageSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnQuoteSummarySection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnCreditReportSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 5/19/2014; will probably enable regardless of Quote or App as long as there's something to show
        Me.hdnResidenceSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/12/2014
        Me.hdnPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 7/27/2015 for Farm
        Me.hdnFarmPersonalPropertySection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 7/27/2015 for Farm
        Me.hdnInlandMarineAndRvWatercraftSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 7/27/2015 for Farm

        Me.hdnCPPCPRLocationsSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnCPPCPRPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnCPPCGLLocationSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnCPPCGLPolicyLevelCoverageSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP

        Me.hdnCPPCPRDetailHeaderSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnCPPCGLDetailHeaderSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnInlandMarineSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
        Me.hdnCrimeSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 6/29/2018 for CPP
    End Sub
    Private Sub EnableDisableApplicationSectionHeaders(ByVal enabled As Boolean)
        Dim enabledDisabledText As String = GetEnabledDisabledText(enabled)

        Me.hdnUnderwritingQuestionSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnApplicationSection_EnabledOrDisabledFlag.Value = enabledDisabledText
        Me.hdnMvrReportSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 5/20/2014
        Me.hdnClueReportSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 5/20/2014
        Me.hdnApplicationSummarySection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 5/19/2014
        Me.hdnIRPMSection_EnabledOrDisabledFlag.Value = enabledDisabledText 'added 7/27/2015 for Farm
        Me.hdnFileUploadSection_EnabledOrDisabledFlag.Value = enabledDisabledText '12-3-2015 Matt A
    End Sub

    Private Sub EvaluateLossHistoriesForRatedQuote(ByVal quoteOrRatedQuote As QuoteOrRatedQuoteType, ByRef lossHistoryCount As Integer, ByRef surchargeableLossHistoryCount As Integer, ByVal qq As QuickQuoteObject, ByVal governingState As QuickQuoteObject)
        'may not initialize counts but shouldn't matter
        lossHistoryCount = 0
        surchargeableLossHistoryCount = 0

        If governingState IsNot Nothing AndAlso governingState.HasLossHistories = True Then 'may not need HasLossHistories check
            EvaluateLossHistoriesForRatedQuote(qq.LobType, governingState.LossHistoryRecords, lossHistoryCount, surchargeableLossHistoryCount)

            'may check LOB
            If governingState.Drivers IsNot Nothing AndAlso governingState.Drivers.Count > 0 Then
                For Each d As QuickQuoteDriver In governingState.Drivers
                    EvaluateLossHistoriesForRatedQuote(qq.LobType, d.LossHistoryRecords, lossHistoryCount, surchargeableLossHistoryCount)
                Next
            End If

            'may check LOB
            If qq.Applicants IsNot Nothing AndAlso governingState.Applicants.Count > 0 Then
                For Each a As QuickQuoteApplicant In governingState.Applicants
                    EvaluateLossHistoriesForRatedQuote(qq.LobType, a.LossHistoryRecords, lossHistoryCount, surchargeableLossHistoryCount)
                Next
            End If
        End If
    End Sub
    Private Sub EvaluateLossHistoriesForRatedQuote(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, ByVal lhs As List(Of QuickQuoteLossHistoryRecord), ByRef lossHistoryCount As Integer, ByRef surchargeableLossHistoryCount As Integer)
        If lhs IsNot Nothing AndAlso lhs.Count > 0 Then
            lossHistoryCount += lhs.Count 'could also increment one on each loop below

            Dim surchargeYearWindow As Double = CDbl(0)
            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal, QuickQuoteObject.QuickQuoteLobType.CommercialAuto 'may not need CAP
                    surchargeYearWindow = CDbl(-3)
                Case Else
                    surchargeYearWindow = CDbl(-5)
            End Select

            For Each lh As QuickQuoteLossHistoryRecord In lhs
                If lh.LossHistorySurchargeId <> "" AndAlso IsNumeric(lh.LossHistorySurchargeId) = True AndAlso CInt(lh.LossHistorySurchargeId) = 1 AndAlso Me.Quote.EffectiveDate <> "" AndAlso IsDate(Me.Quote.EffectiveDate) = True AndAlso lh.LossDate <> "" AndAlso IsDate(lh.LossDate) = True AndAlso CDate(lh.LossDate) >= DateAdd(DateInterval.Year, surchargeYearWindow, CDate(Me.Quote.EffectiveDate)) Then 'may update to use static data list to see if value has corresponding text of 'Surcharge'
                    surchargeableLossHistoryCount += 1
                End If
            Next
        End If
    End Sub

    Public Function EffDate_BasicDatePicker_ClientId_For_Focus() As String
        Dim clientId As String = Me.bdpEffectiveDate.ClientID

        Dim bdpTextbox As TextBox = Me.bdpEffectiveDate.FindControl("TextBox")
        If bdpTextbox IsNot Nothing AndAlso String.IsNullOrEmpty(bdpTextbox.ClientID) = False Then
            clientId = bdpTextbox.ClientID
        End If

        Return clientId
    End Function

    Private Function hasIRPMAdjustment() As Boolean
        'If Me.RatedQuote?.ScheduledRatings IsNot Nothing Then
        '    For Each item In Me.RatedQuote.ScheduledRatings
        '        If item.RiskFactor IsNot Nothing AndAlso item.RiskFactor <> "1.000" Then
        '            Return True
        '        End If
        '    Next
        'End If
        'Return False
        'Me.GoverningStateQuote()
        'updated 10/5/2018; probably need Helper Property for RatedSubQuotes, etc.
        'Dim ratedSubQuotes As List(Of QuickQuoteObject) = QQHelper.MultiStateQuickQuoteObjects(Me.RatedQuote)
        'If ratedSubQuotes IsNot Nothing AndAlso ratedSubQuotes.Count > 0 AndAlso ratedSubQuotes.Item(0) IsNot Nothing AndAlso ratedSubQuotes.Item(0).ScheduledRatings IsNot Nothing AndAlso ratedSubQuotes.Item(0).ScheduledRatings.Count > 0 Then
        '    For Each item In ratedSubQuotes.Item(0).ScheduledRatings
        '        If item.RiskFactor IsNot Nothing AndAlso item.RiskFactor <> "1.000" Then
        '            Return True
        '        End If
        '    Next
        'End If

        'Updated 10-7-2018 to use me.subquotefirst. There is logic that makes sure it is using the right quote (rated vs unrated)
        'If Me.SubQuoteFirst?.ScheduledRatings?.Any() Then
        '    For Each item In Me.SubQuoteFirst.ScheduledRatings
        '        If item.RiskFactor IsNot Nothing AndAlso item.RiskFactor <> "1.000" Then
        '            Return True
        '        End If
        '    Next
        'End If
        'updated 12/2/2020 (Interoperability)
        If Me.SubQuoteFirst IsNot Nothing Then
            Dim qqXml As QuickQuoteXML = Nothing
            Dim scheduledRatings As List(Of QuickQuoteScheduledRating) = QQHelper.CloneObject(Me.SubQuoteFirst.ScheduledRatings)
            If Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Dim cprScheduledRatings As List(Of QuickQuoteScheduledRating) = Nothing
                Dim cglScheduledRatings As List(Of QuickQuoteScheduledRating) = Nothing
                If scheduledRatings IsNot Nothing AndAlso scheduledRatings.Count > 0 Then
                    cprScheduledRatings = (From s In scheduledRatings Where s.ScheduleRatingTypeId = "4").ToList()
                    cglScheduledRatings = (From s In scheduledRatings Where s.ScheduleRatingTypeId = "6").ToList()
                End If
                If cprScheduledRatings Is Nothing OrElse cprScheduledRatings.Count = 0 Then
                    If qqXml Is Nothing Then
                        qqXml = New QuickQuoteXML
                    End If
                    qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, cprScheduledRatings, packagePartType:=QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty, useConvertedFlag:=False)
                End If
                If cglScheduledRatings Is Nothing OrElse cglScheduledRatings.Count = 0 Then
                    If qqXml Is Nothing Then
                        qqXml = New QuickQuoteXML
                    End If
                    qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, cglScheduledRatings, packagePartType:=QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability, useConvertedFlag:=False)
                    'note: CGL has duplicates for ScheduledRatingTypeId 5 and 6, but we only need to load one
                    Dim allLiabilityRatings As List(Of QuickQuoteScheduledRating) = Nothing
                    qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, allLiabilityRatings, packagePartType:=QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability, useConvertedFlag:=False)
                    If allLiabilityRatings IsNot Nothing AndAlso allLiabilityRatings.Count > 0 Then
                        cglScheduledRatings = (From s In allLiabilityRatings Where s.ScheduleRatingTypeId = "6").ToList()
                    End If
                End If
                Dim hasAdjustment As Boolean = False
                If hasAdjustment = False Then
                    hasAdjustment = ScheduledRatingsListHasAdjustment(cprScheduledRatings)
                End If
                If hasAdjustment = False Then
                    hasAdjustment = ScheduledRatingsListHasAdjustment(cglScheduledRatings)
                End If
                Return hasAdjustment
            Else
                If scheduledRatings Is Nothing OrElse scheduledRatings.Count = 0 Then
                    If qqXml Is Nothing Then
                        qqXml = New QuickQuoteXML
                    End If
                    qqXml.QuoteObjectConversion_PolicyScheduledRatings(Me.SubQuoteFirst, scheduledRatings, useConvertedFlag:=False)
                End If
                Return ScheduledRatingsListHasAdjustment(scheduledRatings)
            End If
        End If

        Return False
    End Function
    Private Function ScheduledRatingsListHasAdjustment(ByVal scheduledRatings As List(Of QuickQuoteScheduledRating)) As Boolean 'new 12/2/2020 (Interoperability); added to consolidate logic since CPP now checks CPR/CGL packageParts separately
        Dim hasIt As Boolean = False

        If scheduledRatings IsNot Nothing AndAlso scheduledRatings.Count > 0 Then
            For Each sr As QuickQuoteScheduledRating In scheduledRatings
                If sr IsNot Nothing AndAlso QQHelper.IsNumericString(sr.RiskFactor) = True AndAlso sr.RiskFactor <> "1.000" Then
                    hasIt = True
                    Exit For
                End If
            Next
        End If

        Return hasIt
    End Function

    'added 2/21/2019
    Private Sub UpdateTreeForViewMode(Optional ByVal qqo As QuickQuoteObject = Nothing)
        Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
        Dim allowEditToLosses As Boolean = True 'added 6/17/2020
        If qqo IsNot Nothing Then
            qqTranType = qqo.QuoteTransactionType

            'added 6/17/2020
            If qqo.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True Then
                allowEditToLosses = False
            End If
        Else
            qqTranType = Me.QuoteTransactionType
        End If

        'If qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then

        'End If
        SetQuoteTransactionTypeFlagForTree(qqTranType)
        SetEditableLossFlagForTree(allowEditToLosses) 'added 6/17/2020

    End Sub
    Private Sub SetQuoteTransactionTypeFlagForTree(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType, Optional ByVal defaultToNewBusinessQuoteWhenInvalid As Boolean = True)
        If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteTransactionType), qqTranType) = True AndAlso qqTranType <> QuickQuoteObject.QuickQuoteTransactionType.None Then
            Me.hdnQuoteTransactionTypeFlag.Value = System.Enum.GetName(GetType(QuickQuoteObject.QuickQuoteTransactionType), qqTranType)
        Else
            If defaultToNewBusinessQuoteWhenInvalid = True Then
                Me.hdnQuoteTransactionTypeFlag.Value = "NewBusinessQuote"
            Else
                Me.hdnQuoteTransactionTypeFlag.Value = "None"
            End If
        End If
    End Sub
    Private Function GetQuoteTransactionTypeFlagForTree(Optional ByVal defaultToNewBusinessQuoteWhenInvalid As Boolean = True) As QuickQuoteObject.QuickQuoteTransactionType
        Dim qqTranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None

        If Me.hdnQuoteTransactionTypeFlag.Value IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Me.hdnQuoteTransactionTypeFlag.Value) = False Then
            Select Case UCase(Me.hdnQuoteTransactionTypeFlag.Value)
                Case "NONE"
                    qqTranType = QuickQuoteObject.QuickQuoteTransactionType.None
                Case UCase("NewBusinessQuote")
                    qqTranType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
                Case UCase("EndorsementQuote")
                    qqTranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                Case UCase("ReadOnlyImage")
                    qqTranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                Case Else
                    If System.Enum.TryParse(Of QuickQuoteObject.QuickQuoteTransactionType)(Me.hdnQuoteTransactionTypeFlag.Value, qqTranType) = False Then
                        qqTranType = QuickQuoteObject.QuickQuoteTransactionType.None
                    End If
            End Select
        End If

        If defaultToNewBusinessQuoteWhenInvalid = True Then
            If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteTransactionType), qqTranType) = False OrElse qqTranType = QuickQuoteObject.QuickQuoteTransactionType.None Then
                qqTranType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
            End If
        End If

        Return qqTranType
    End Function
    'added 6/17/2020
    Private editableLossFlagKeyName As String = "VR_EditableLossFlagForTree"
    Private Sub SetEditableLossFlagForTree(ByVal editable As Boolean)
        Dim strEditable As String = editable.ToString
        If ViewState.Item(editableLossFlagKeyName) IsNot Nothing Then
            ViewState.Item(editableLossFlagKeyName) = strEditable
        Else
            ViewState.Add(editableLossFlagKeyName, strEditable)
        End If
    End Sub
    Private Function GetEditableLossFlagForTree() As Boolean
        Dim editable As Boolean = False

        If ViewState.Item(editableLossFlagKeyName) IsNot Nothing Then
            editable = QQHelper.BitToBoolean(ViewState.Item(editableLossFlagKeyName).ToString)
        End If

        Return editable
    End Function

    Private Sub imgBtnBillingInformation_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnBillingInformation.Click
        If Me.CanNotAccessSection(IsBillingInfoSectionEnabled, "You cannot access Billing Information at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowBillingInformation(sender, e)
    End Sub

    Private Sub imgBtnPrintHistory_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnPrintHistory.Click
        If Me.CanNotAccessSection(IsPrintHistorySectionEnabled, "You cannot access Print History at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowPrintHistory(sender, e)
    End Sub

    Private Sub imgBtnPolicyHistory_Click(sender As Object, e As ImageClickEventArgs) Handles imgBtnPolicyHistory.Click
        If Me.CanNotAccessSection(IsPolicyHistorySectionEnabled, "You cannot access Policy History at this time.") Then
            Exit Sub
        End If
        RaiseEvent ShowPolicyHistory(sender, e)
    End Sub
    ''added 2/22/2019; 3/7/2019 - moved to WebHelper_Personal.vb
    'Private Sub AddStyleToWebControl(ByVal ctrl As WebControl, ByVal styleName As String, ByVal styleValue As String, Optional ByVal allowEmptyStyleValue As Boolean = False)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False AndAlso (String.IsNullOrWhiteSpace(styleValue) = False OrElse allowEmptyStyleValue = True) Then
    '        If ctrl.Style(styleName) IsNot Nothing Then
    '            ctrl.Style(styleName) = styleValue
    '        Else
    '            ctrl.Style.Add(styleName, styleValue)
    '        End If
    '    End If
    'End Sub
    'Private Sub AddStyleToGenericControl(ByVal ctrl As HtmlGenericControl, ByVal styleName As String, ByVal styleValue As String, Optional ByVal allowEmptyStyleValue As Boolean = False)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False AndAlso (String.IsNullOrWhiteSpace(styleValue) = False OrElse allowEmptyStyleValue = True) Then
    '        If ctrl.Style(styleName) IsNot Nothing Then
    '            ctrl.Style(styleName) = styleValue
    '        Else
    '            ctrl.Style.Add(styleName, styleValue)
    '        End If
    '    End If
    'End Sub
    'Private Sub RemoveStyleFromWebControl(ByVal ctrl As WebControl, ByVal styleName As String)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False Then
    '        If ctrl.Style(styleName) IsNot Nothing Then
    '            ctrl.Style.Remove(styleName)
    '        End If
    '    End If
    'End Sub
    'Private Sub RemoveStyleFromGenericControl(ByVal ctrl As HtmlGenericControl, ByVal styleName As String)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False Then
    '        If ctrl.Style(styleName) IsNot Nothing Then
    '            ctrl.Style.Remove(styleName)
    '        End If
    '    End If
    'End Sub
    'Private Sub AddAttributeToWebControl(ByVal ctrl As WebControl, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal allowEmptyAttributeValue As Boolean = False)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso (String.IsNullOrWhiteSpace(attributeValue) = False OrElse allowEmptyAttributeValue = True) Then
    '        If ctrl.Attributes(attributeName) IsNot Nothing Then
    '            ctrl.Attributes(attributeName) = attributeValue
    '        Else
    '            ctrl.Attributes.Add(attributeName, attributeValue)
    '        End If
    '    End If
    'End Sub
    'Private Sub AddAttributeToGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal allowEmptyAttributeValue As Boolean = False)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso (String.IsNullOrWhiteSpace(attributeValue) = False OrElse allowEmptyAttributeValue = True) Then
    '        If ctrl.Attributes(attributeName) IsNot Nothing Then
    '            ctrl.Attributes(attributeName) = attributeValue
    '        Else
    '            ctrl.Attributes.Add(attributeName, attributeValue)
    '        End If
    '    End If
    'End Sub
    'Private Sub RemoveAttributeFromWebControl(ByVal ctrl As WebControl, ByVal attributeName As String)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
    '        If ctrl.Attributes(attributeName) IsNot Nothing Then
    '            ctrl.Attributes.Remove(attributeName)
    '        End If
    '    End If
    'End Sub
    'Private Sub RemoveAttributeFromGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String)
    '    If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
    '        If ctrl.Attributes(attributeName) IsNot Nothing Then
    '            ctrl.Attributes.Remove(attributeName)
    '        End If
    '    End If
    'End Sub

    'added 12/19/2022
    Private Sub UpdateTreeForRouteToUW()
        Dim showRouteToUW As Boolean = False

        If Me.pnlTreeView.Visible = True Then
            If String.IsNullOrWhiteSpace(Me.hdnQuoteNumber.Value) = False AndAlso GetQuoteTransactionTypeFlagForTree() = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso Common.Helpers.GenericHelper.SaveToDiamondOnNewBusinessRouteToUnderwriting() = True Then
                showRouteToUW = True

                Me.hdnRouteCommOrPers.Value = "PERS" 'default
                If Me.Quote IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), Me.Quote.LobType) = True AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    Select Case Me.Quote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuoteObject.QuickQuoteLobType.CommercialCrime, QuickQuoteObject.QuickQuoteLobType.CommercialGarage, QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialUmbrella, QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            Me.hdnRouteCommOrPers.Value = "COMM"
                    End Select
                End If
            End If
        End If

        If showRouteToUW = True Then
            Me.liRouteToUW.Visible = True
        Else
            Me.liRouteToUW.Visible = False
        End If
    End Sub

End Class