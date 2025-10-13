Option Explicit On
Option Strict On
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Flags
Imports IFM.ControlFlags
Imports IFM.Configuration.Extensions
Imports System.Configuration
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass

Namespace IFM.VR.Common.Helpers
    Public Class LOBHelper
        Shared ReadOnly vr_lobs As New Dictionary(Of Int32, QuickQuoteLobType) From
        {
        {0, QuickQuoteLobType.None},
        {1, QuickQuoteLobType.AutoPersonal},
        {2, QuickQuoteLobType.HomePersonal},
        {3, QuickQuoteLobType.DwellingFirePersonal},
        {9, QuickQuoteLobType.CommercialGeneralLiability},
        {14, QuickQuoteLobType.UmbrellaPersonal},
        {17, QuickQuoteLobType.Farm},
        {20, QuickQuoteLobType.CommercialAuto},
        {21, QuickQuoteLobType.WorkersCompensation},
        {23, QuickQuoteLobType.CommercialPackage},
        {24, QuickQuoteLobType.CommercialGarage},
        {25, QuickQuoteLobType.CommercialBOP},
        {26, QuickQuoteLobType.CommercialCrime},
        {27, QuickQuoteLobType.CommercialUmbrella},
        {28, QuickQuoteLobType.CommercialProperty},
        {29, QuickQuoteLobType.CommercialInlandMarine}
        }
        Shared ReadOnly vr_governingStateInfos As New List(Of GoverningStateInfo)
        Shared ReadOnly vr_multiStateInfos As New List(Of MultiStateInfo)
        Shared ReadOnly ppaFlags As New LOB.PPA
        Shared Sub New()
            'seed data
            ' config keys for this stuff feels icky, hard coding them feels a bit wrong and database feels messy... kind of feels like a pick your poison thing really
            ' but how it is setup you could easily transition to a database if you needed
            ' it is kind of a mix of config key at high level and a database like structure ???
            SeedGoverningStateData()
            SeedMultiStateData()
        End Sub
        Private Shared Sub SeedGoverningStateData()

            'this list is used under the assumption that once a stateid is added for a state it will always be supported from that point on
            ' so the list is cumulative

            'seed Indiana
            For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                vr_governingStateInfos.Add(New GoverningStateInfo(LOBHelper.LOBIdToType(item.Value), DateTime.MinValue, States.Abbreviations.IN))
            Next

            'seed Illinois
            'Dim ILExcludedLobs As IEnumerable(Of QuickQuoteLobType) = {QuickQuoteLobType.HomePersonal, QuickQuoteLobType.DwellingFirePersonal}
            'Dim ILStartDate As DateTime = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate()
            'For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
            '    If ILExcludedLobs.Contains(LOBIdToType(item.Value)) = False Then
            '        vr_governingStateInfos.Add(New GoverningStateInfo(LOBIdToType(item.Value), ILStartDate, States.Abbreviations.IL))
            '    End If
            'Next

            ' New Seed Illinois
            GoverningStateSetState(States.Abbreviations.IL, IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate(), {QuickQuoteLobType.HomePersonal, QuickQuoteLobType.DwellingFirePersonal})

            ' New Seed Ohio - note that the list of lob's are EXCLUDED lobs
            GoverningStateSetState(States.Abbreviations.OH, IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate(), {QuickQuoteLobType.AutoPersonal, QuickQuoteLobType.CommercialUmbrella, QuickQuoteLobType.DwellingFirePersonal, QuickQuoteLobType.HomePersonal, QuickQuoteLobType.WorkersCompensation})

            'add OH PPA with it's earliest effective date, instead of the general multistate effective date
            ppaFlags.When(Function(x) x.OhioEnabled) _
                    .Do(Sub()
                            Dim minEffectiveDate As Date

                            'If Not Date.TryParse(ConfigurationManager.AppSettings.Get(LOB.PPA.OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE), minEffectiveDate) Then
                            '    minEffectiveDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate()
                            'End If
                            minEffectiveDate = ConfigurationManager.AppSettings.As(Of Date)(LOB.PPA.OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE, minEffectiveDate)
                            AddGoverningStateByLobTypeAndEffectiveDate(States.Abbreviations.OH, QuickQuoteLobType.AutoPersonal, minEffectiveDate)
                        End Sub)

            '''TODO:  can we change this? using this to initialize other values creates side effects
            Dim VrSupportedLobs = From lobId In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs() Select LOBIdToType(lobId.Value)

            'Updated 3/8/2022 for Task 73077 MLW
            'KY WC
            ''GoverningStateSetState(States.Abbreviations.KY, DateTime.Parse("5-1-2019"), VrSupportedLobs.Except({QuickQuoteLobType.WorkersCompensation})) '3/8/2022 was originally commented out before adding KY as Gov State for WCP
            If IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEnabled = True Then
                Dim KentuckyWCPGovStateStartDate As DateTime = IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEffectiveDate()
                GoverningStateSetState(States.Abbreviations.KY, KentuckyWCPGovStateStartDate, VrSupportedLobs.Except({QuickQuoteLobType.WorkersCompensation}))
            End If

            'future rules create a new key for startDate or use NamedVersions that use a key

        End Sub
        Private Shared Sub SeedMultiStateData()
            '''TODO: could probably stand to be refactored

            Dim VrSupportedLobs = From lobId In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs() Select LOBIdToType(lobId.Value)
            'this list is used under the assumption that the latest record trumps prior records
            Dim ExcludedLobs As IEnumerable(Of QuickQuoteLobType) = {QuickQuoteLobType.AutoPersonal, QuickQuoteLobType.HomePersonal, QuickQuoteLobType.DwellingFirePersonal}

            'seed Illinois
            Dim ILStartDate As DateTime = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate()
            Dim ILExcludedLobs As IEnumerable(Of QuickQuoteLobType) = {}
            For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                If ExcludedLobs.Contains(LOBIdToType(item.Value)) = False AndAlso ILExcludedLobs.Contains(LOBIdToType(item.Value)) = False Then
                    vr_multiStateInfos.Add(New MultiStateInfo(LOBIdToType(item.Value), ILStartDate, {States.Abbreviations.IN, States.Abbreviations.IL}))
                End If
            Next

            'seed Kentucky
            Dim KYStartDate As DateTime = CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate)
            'Dim KYStartDate As DateTime = CDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate"))
            Dim KYExcludedLobs As IEnumerable(Of QuickQuoteLobType) = VrSupportedLobs.Except({QuickQuoteLobType.WorkersCompensation})
            For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                If ExcludedLobs.Contains(LOBIdToType(item.Value)) = False AndAlso KYExcludedLobs.Contains(LOBIdToType(item.Value)) = False Then
                    vr_multiStateInfos.Add(New MultiStateInfo(LOBIdToType(item.Value), KYStartDate, {States.Abbreviations.KY}))
                End If
            Next
            'future rules create a new key for startDate or use NamedVersions that use a key

            'seed Ohio
            Dim OHStartDate As DateTime = IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate()
            Dim OHExcludedLobs As IEnumerable(Of QuickQuoteLobType)

            OHExcludedLobs = VrSupportedLobs.Except({QuickQuoteLobType.AutoPersonal, QuickQuoteLobType.CommercialUmbrella, QuickQuoteLobType.DwellingFirePersonal, QuickQuoteLobType.HomePersonal, QuickQuoteLobType.WorkersCompensation})


            For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                If ExcludedLobs.Contains(LOBIdToType(item.Value)) = False AndAlso OHExcludedLobs.Contains(LOBIdToType(item.Value)) = False Then
                    vr_multiStateInfos.Add(New MultiStateInfo(LOBIdToType(item.Value), OHStartDate, {States.Abbreviations.OH}))
                End If
            Next

            ' Commented out 3/14/22 per Bug 73382 MGB
            'add OH PPA with it's earliest effective date, instead of the general multistate effective date
            'ppaFlags.When(Function(x) x.OhioEnabled) _
            '        .Do(Sub()
            '                Dim minEffectiveDate As Date

            '                'If Not Date.TryParse(ConfigurationManager.AppSettings.Get(LOB.PPA.OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE), minEffectiveDate) Then
            '                '    minEffectiveDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate()
            '                'End If
            '                minEffectiveDate = ConfigurationManager.AppSettings.As(Of Date)(LOB.PPA.OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE, minEffectiveDate)
            '                AddMultistateStateByLobTypeAndEffectiveDate(States.Abbreviations.OH, QuickQuoteLobType.AutoPersonal, minEffectiveDate)
            '            End Sub)
        End Sub

        Private Shared Sub GoverningStateSetState(state As States.Abbreviations, startDate As DateTime, excludedLobs As IEnumerable(Of QuickQuoteLobType))
            For Each item In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                If excludedLobs.Contains(LOBIdToType(item.Value)) = False Then
                    vr_governingStateInfos.Add(New GoverningStateInfo(LOBIdToType(item.Value), startDate, state))
                End If
            Next
        End Sub

        Private Shared Sub AddGoverningStateByLobTypeAndEffectiveDate(state As States.Abbreviations, lobToAdd As QuickQuoteLobType, startDate As DateTime)
            vr_governingStateInfos.Add(New GoverningStateInfo(LOBIdToType(lobToAdd), startDate, state))
        End Sub

        Private Shared Sub AddMultistateStateByLobTypeAndEffectiveDate(state As States.Abbreviations, lobToAdd As QuickQuoteLobType, startDate As DateTime)
            vr_multiStateInfos.Add(New MultiStateInfo(LOBIdToType(lobToAdd), startDate, {state}))
        End Sub
        Public Sub New(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Me.LobId = topQuote.LobId.TryToGetInt32()
            Me.LobType = LOBIdToType(LobId)
        End Sub
        Public Sub New(lobId As Int32)
            Me.LobId = lobId
            Me.LobType = LOBIdToType(lobId)
        End Sub
        Public Sub New(lobType As QuickQuoteLobType)
            Me.LobType = lobType
            Me.LobId = LOBTypetoId(lobType)
        End Sub
        Public ReadOnly Property LobId As Int32 = 0
        Public ReadOnly Property LobType As QuickQuoteLobType = QuickQuoteLobType.None
        Public ReadOnly Property EffectiveDateMaxBack As Int32 = 30
        Public ReadOnly Property EffectiveDateMaxForward As Int32 = 90
        Public ReadOnly Property Prefix As String
            Get
                Return GetAbbreviatedLOBPrefix(Me.LobType)
            End Get
        End Property

        Public ReadOnly Property IsCommercialQuote As Boolean
            Get
                Select Case Me.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        ' Commercial Lines
                        Return True
                    Case Else
                        ' Personal Lines & Farm
                        Return False
                End Select
            End Get
        End Property

        Public ReadOnly Property IsMultistateCapableLob As Boolean
            Get
                Select Case Me.LobType
                    Case QuickQuoteLobType.AutoPersonal, QuickQuoteLobType.DwellingFirePersonal, QuickQuoteLobType.HomePersonal
                        Return False
                    Case Else
                        Return True
                End Select
            End Get
        End Property


        Public Function AcceptableGoverningStateIds(effectiveDate As DateTime) As IEnumerable(Of Int32)
            Return AcceptableGoverningStateIds(Me.LobType, effectiveDate)
        End Function
        Public Function AcceptableGoverningStateIds(effectiveDate As String) As IEnumerable(Of Int32)
            If IsDate(effectiveDate) Then
                Return AcceptableGoverningStateIds(CDate(effectiveDate))
            End If
            Return New List(Of Int32)
        End Function
        Public Function AcceptableGoverningStates(effectiveDate As DateTime) As IEnumerable(Of String)
            Dim stateIds = AcceptableGoverningStateIds(Me.LobType, effectiveDate)
            Dim stateList = States.GetStateInfosFromIds(stateIds)
            Dim stateAbbrevs = (From s In stateList Select s.Abbreviation)
            If stateAbbrevs.IsLoaded() Then
                Return stateAbbrevs
            End If
            Return New List(Of String)
        End Function

        Public Function AcceptableGoverningStates(effectiveDate As String) As IEnumerable(Of String)
            If IsDate(effectiveDate) Then
                Return AcceptableGoverningStates(CDate(effectiveDate))
            End If
            Return New List(Of String)
        End Function

        Public Function AcceptableGoverningStatesAsString(effectiveDate As String) As String
            Dim acceptableGoverningStateIds = Me.AcceptableGoverningStateIds(effectiveDate)
            Dim stateInfos = IFM.VR.Common.Helpers.States.GetStateInfosFromIds(acceptableGoverningStateIds)
            Dim stateNames = From s In stateInfos Select s.StateName
            'Return stateNames.JoinOr() a new extension method that will be available at some point in future
            If stateNames IsNot Nothing And stateNames.Any() Then
                Select Case stateNames.Count
                    Case 1
                        Return stateNames(0)
                    Case 2
                        Return $"{stateNames(0)} or {stateNames(1)}"
                    Case Else
                        Return String.Join(",", stateNames)
                End Select
            End If
            Return ""
        End Function

        Public Function AcceptableMultStateQuoteStateIds(effectiveDate As DateTime) As IEnumerable(Of Int32)
            Dim latestRecord = (From gs In vr_multiStateInfos Where gs.LobType = Me.LobType AndAlso effectiveDate >= gs.StartDate Select gs Order By gs.StartDate Descending).FirstOrDefault()

            If latestRecord IsNot Nothing Then
                Return latestRecord.SupportedStateIds
            End If

            Return New List(Of Int32)
        End Function
        Public Function AcceptableMultStateQuoteStateIds(effectiveDate As String) As IEnumerable(Of Int32)
            If IsDate(effectiveDate) Then
                Return AcceptableMultStateQuoteStateIds(CDate(effectiveDate))
            End If
            Return New List(Of Int32)
        End Function

        Public Function AcceptableMultStateQuoteStates(effectiveDate As DateTime) As IEnumerable(Of String)
            Dim latestRecord = (From gs In vr_multiStateInfos Where gs.LobType = Me.LobType AndAlso effectiveDate >= gs.StartDate Select gs Order By gs.StartDate Descending).FirstOrDefault()

            If latestRecord IsNot Nothing Then
                Dim stateIds = latestRecord.SupportedStateIds
                Dim stateList = States.GetStateInfosFromIds(stateIds)
                Dim stateAbbrevs = (From s In stateList Select s.Abbreviation)
                If stateAbbrevs.IsLoaded() Then
                    Return stateAbbrevs
                End If
                Return New List(Of String)
            End If

            Return New List(Of String)
        End Function
        Public Function AcceptableMultStateQuoteStates(effectiveDate As String) As IEnumerable(Of String)
            If IsDate(effectiveDate) Then
                Return AcceptableMultStateQuoteStates(CDate(effectiveDate))
            End If
            Return New List(Of String)
        End Function

        Public Shared Function LOBIsInCSVConfigKey(lobId As Int32, configKeyName As String) As Boolean
            Return LOBIsInCSVConfigKeyCore(LOBIdToType(lobId), configKeyName)
        End Function
        Public Shared Function LOBIsInCSVConfigKey(lobType As QuickQuoteLobType, configKeyName As String) As Boolean
            Return LOBIsInCSVConfigKeyCore(lobType, configKeyName)
        End Function

        Private Shared Function LOBIsInCSVConfigKeyCore(lobType As QuickQuoteLobType, configKeyName As String) As Boolean
            Dim returnVar As Boolean = False
            Dim LOBsAsCSV As String = ""
            Dim LOBs As New List(Of String)
            IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForString(configKeyName, LOBsAsCSV)
            If LOBsAsCSV.NoneAreNullEmptyorWhitespace() Then
                If LOBsAsCSV.Contains(",") Then
                    LOBs = LOBsAsCSV.CSVtoList()
                Else
                    LOBs.Add(LOBsAsCSV)
                End If

                If LOBs IsNot Nothing AndAlso LOBs.Count > 0 Then
                    For Each lob As String In LOBs
                        If lobType = GetLobTypeFromAbbreviatedPrefix(lob) Then
                            returnVar = True
                            Exit For
                        End If
                    Next
                End If
            End If

            Return returnVar
        End Function

        Public Shared Function LOBIdToType(lobId As Int32) As QuickQuoteLobType
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            'Return qqh.ConvertQQLobIdToQQLobType(lobId.ToString())
            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(lobId)
        End Function
        Public Shared Function LOBTypetoId(type As QuickQuoteLobType) As Int32
            If vr_lobs.ContainsValue(type) Then
                Return (From k In vr_lobs Where k.Value = type Select k.Key).FirstOrDefault()
            End If
            Return 0
        End Function

        Public Shared Function GetAbbreviatedLOBPrefix(lobType As QuickQuoteLobType) As String

            Select Case lobType
                Case QuickQuoteLobType.AutoPersonal
                    Return "PPA"
                Case QuickQuoteLobType.CommercialAuto
                    Return "CAP"
                Case QuickQuoteLobType.CommercialBOP
                    Return "BOP"
                Case QuickQuoteLobType.CommercialCrime
                    Return "CRM"
                Case QuickQuoteLobType.CommercialGarage
                    Return "GAR"
                Case QuickQuoteLobType.CommercialGeneralLiability
                    Return "CGL"
                Case QuickQuoteLobType.CommercialInlandMarine
                    Return "CIM"
                Case QuickQuoteLobType.CommercialPackage
                    Return "CPP"
                Case QuickQuoteLobType.CommercialProperty
                    Return "CPR"
                Case QuickQuoteLobType.CommercialUmbrella
                    Return "CUP"
                Case QuickQuoteLobType.DwellingFirePersonal
                    Return "DFR"
                Case QuickQuoteLobType.Farm
                    Return "FAR"
                Case QuickQuoteLobType.HomePersonal
                    Return "HOM"
                Case QuickQuoteLobType.WorkersCompensation
                    Return "WCP"
                Case QuickQuoteLobType.UmbrellaPersonal
                    Return "PUP"

            End Select
            Return "XXX"
        End Function

        Public Shared Function GetLobTypeFromAbbreviatedPrefix(lobAbb As String) As QuickQuoteLobType

            Select Case lobAbb.ToUpper()
                Case "PPA"
                    Return QuickQuoteLobType.AutoPersonal
                Case "CAP"
                    Return QuickQuoteLobType.CommercialAuto
                Case "BOP"
                    Return QuickQuoteLobType.CommercialBOP
                Case "CRM"
                    Return QuickQuoteLobType.CommercialCrime
                Case "GAR"
                    Return QuickQuoteLobType.CommercialGarage
                Case "CGL"
                    Return QuickQuoteLobType.CommercialGeneralLiability
                Case "CIM"
                    Return QuickQuoteLobType.CommercialInlandMarine
                Case "CPP"
                    Return QuickQuoteLobType.CommercialPackage
                Case "CPR"
                    Return QuickQuoteLobType.CommercialProperty
                Case "CUP"
                    Return QuickQuoteLobType.CommercialUmbrella
                Case "DFR"
                    Return QuickQuoteLobType.DwellingFirePersonal
                Case "FAR"
                    Return QuickQuoteLobType.Farm
                Case "HOM"
                    Return QuickQuoteLobType.HomePersonal
                Case "WCP"
                    Return QuickQuoteLobType.WorkersCompensation
                Case "PUP"
                    Return QuickQuoteLobType.UmbrellaPersonal
                Case "FUP"
                    Return QuickQuoteLobType.UmbrellaPersonal
            End Select
            Return QuickQuoteLobType.None
        End Function

        Public Shared Function GetLobFromPrefix_QuoteOrPolicy(PolicyNumber As String) As QuickQuoteLobType
            If Not String.IsNullOrWhiteSpace(PolicyNumber) AndAlso PolicyNumber.Length > 4 Then
                Dim prefix As String
                If PolicyNumber.ToLower().StartsWith("q") Then
                    prefix = PolicyNumber.Substring(1, 3).ToUpper()
                Else
                    prefix = PolicyNumber.Substring(0, 3)
                End If
                Return GetLobTypeFromAbbreviatedPrefix(prefix)
            End If
            Return QuickQuoteLobType.None
        End Function

        Public Shared Function AcceptableGoverningStateIds(lobType As QuickQuoteLobType, effectiveDate As DateTime) As IEnumerable(Of Int32)
            Dim stateIds = From gs In vr_governingStateInfos Where gs.LobType = lobType AndAlso effectiveDate >= gs.StartDate Select gs.SupportedStateId
            If stateIds IsNot Nothing AndAlso stateIds.Any() Then
                Return stateIds.Distinct()
            Else
                Return New List(Of Int32)
            End If
        End Function
        Public Shared Function AcceptableGoverningStateIds(lobType As QuickQuoteLobType, effectiveDate As String) As IEnumerable(Of Int32)
            If IsDate(effectiveDate) Then
                Return AcceptableGoverningStateIds(lobType, CDate(effectiveDate))
            End If
            Return New List(Of Int32)
        End Function

        Public Shared Function IsEffectiveDateAcceptableToLobAndGoverningState(effectiveDate As Date, lobType As QuickQuoteLobType, governingState As QuickQuoteState) As Boolean
            Dim retval As Boolean = False

            retval = vr_governingStateInfos.Any(Function(gs) gs.LobType = lobType AndAlso
                                                             effectiveDate >= gs.StartDate AndAlso
                                                             gs.SupportedStateId = DiamondStateIdForQuickQuoteState(governingState))
            Return retval
        End Function

        Public Shared Function GetEarliesttEffectiveDateForLobAndGoverningState(lobType As QuickQuoteLobType, governingState As QuickQuoteState) As Date
            Dim retval As Date = General.MultiStateStartDate()

            Dim gsLobEntry = vr_governingStateInfos.FirstOrDefault(Function(entry) entry.LobType = lobType AndAlso
                                                                                   entry.SupportedStateId = DiamondStateIdForQuickQuoteState(governingState))
            If gsLobEntry IsNot Nothing Then
                retval = gsLobEntry.StartDate
            End If

            Return retval
        End Function
    End Class

    Friend Class GoverningStateInfo
        Inherits LobRuleInfo
        Public ReadOnly Property SupportedStateId As Int32
        Public Sub New(LobType As QuickQuoteLobType, startDate As DateTime, stateId As Int32)
            MyBase.New(LobType, startDate)
            Me.SupportedStateId = stateId
        End Sub
    End Class

    Friend Class MultiStateInfo
        Inherits LobRuleInfo
        Public ReadOnly Property SupportedStateIds As IEnumerable(Of Int32)
        Public Sub New(LobType As QuickQuoteLobType, startDate As DateTime, stateIds As IEnumerable(Of Int32))
            MyBase.New(LobType, startDate)
            Me.SupportedStateIds = stateIds
        End Sub
    End Class

    Friend MustInherit Class LobRuleInfo
        Public ReadOnly Property StartDate As DateTime
        Public ReadOnly Property LobId As Int32 = 0
        Public ReadOnly Property LobType As QuickQuoteLobType = QuickQuoteLobType.None
        Public Sub New(LobType As QuickQuoteLobType, startDate As DateTime)
            Me.LobType = LobType
            Me.LobId = LOBHelper.LOBTypetoId(Me.LobType)
            Me.StartDate = startDate
        End Sub
        Public Sub New(LobId As Int32, startDate As DateTime)
            Me.LobId = LobId
            Me.LobType = LOBHelper.LOBIdToType(Me.LobId)
        End Sub
    End Class

    Public Class NamedVersion
        ' see if you can use something like this to centralize versioning of VR
        ' maybe use config keys to define the date
        ' should probably have a few properties around uncrossable dates and the like but not too much stuff
        Public Property StartDate As DateTime
        Public Enum NamedVersions
            Baseline = 0
            HOM2011 = 1
            PARAchute = 2
            StateExpansion_IL = 3
        End Enum
        Public Property Version As NamedVersions = NamedVersions.Baseline
    End Class

End Namespace

