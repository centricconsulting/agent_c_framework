Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class FeatureFlagBase
    Const NewCoInEffectiveDate As String = "VR_NewCO_In_EffectiveDate"
    Const NewCoGovStatesAllowedKey As String = "VR_NewCo_GovStatesAllowedKey"
    Const NewCoLobsAllowedKey As String = "VR_NewCo_LobsAllowedKey"
    Const NewCoCalendarDateKey As String = "VR_NewCo_CalendarDateKey"
    Const NewCoILEffectiveDate As String = "VR_NewCO_Il_EffectiveDate"
    Const NewCoOhEffectiveDate As String = "VR_NewCO_Oh_EffectiveDate"
    Const NewCoBopEffectiveDate As String = "VR_NewCO_BOP_EffectiveDate"
    Const NewCoBopCalendarDateKey As String = "VR_NewCo_BOP_CalendarDate"

    Public Shared Function SubQuoteFirst(quote As QuickQuoteObject) As QuickQuoteObject
        Dim qqh As New QuickQuoteHelperClass
        Return qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)
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

    Public Shared Function isNewBusiness(quote As QuickQuoteObject) As Boolean
        Return quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
    End Function

    Public Shared Function isEndorsement(quote As QuickQuoteObject) As Boolean
        Return quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
    End Function

    Public Shared Function DoesQuoteQualifyByFirstWrittenDate(quote As QuickQuoteObject, firstWrittenDateKey As String) As Boolean
        'FirstWrittenDate is for Endorsements Only - want to apply this change to policies that have been created on
        'the FirstWrittenDateKey or later, then endorsed. Prior policies first written before this date are grandfathered
        'in and do not want to apply these changes.

        Dim doesIt As Boolean = True

        If quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            If DateFromString(quote.FirstWrittenDate) < CalendarEffectiveDate(firstWrittenDateKey) Then
                doesIt = False
            End If
        End If

        Return doesIt
    End Function

    Public Shared Function DoesQuoteQualifyByDates(quote As QuickQuoteObject) As Boolean
        Dim MinQualifyDate = GetEarliestEffectiveDatePossible(quote)
        Return DateFromString(quote.EffectiveDate) >= MinQualifyDate
    End Function

    Public Shared Function DoesQuoteQualifyByState(quote As QuickQuoteObject, configKey As String) As Boolean
        If quote IsNot Nothing Then
            Dim StatesOnQuote = MultiStateHelper.MultistateQuoteStateIds(quote)
            'Dim StateEffectiveDatesGood = DoesStateQualifyByEffectiveDate(StatesOnQuote, quote.EffectiveDate)
            Return Not StatesOnQuote.Except(GoverningStatesAllowed(configKey)).Any() 'AndAlso StateEffectiveDatesGood
        End If
        Return False
    End Function
    Public Shared Function DoesQuoteQualifyByCalendarDate(quote As QuickQuoteObject, configKey As String) As Boolean
        Return DateTime.Now >= CalendarEffectiveDate(configKey)
    End Function

    Public Shared Function DoesQuoteQualifyByCreatedDate(quote As QuickQuoteObject, configKey As String) As Boolean
        Return DateFromString(quote.Database_QuickQuote_Inserted) < CalendarEffectiveDate(configKey)
    End Function

    Public Shared Function DoesQuoteQualifyByLob(quote As QuickQuoteObject, configKey As String) As Boolean
        Return quote IsNot Nothing AndAlso LobsAllowed(configKey).Contains(quote.LobId.TryToGetInt32)
    End Function

    Public Shared Function DoesQuoteQualifyByTransactionType(quote As QuickQuoteObject) As Boolean
        Return quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
    End Function

    Public Shared Function DoesStateQualifyByEffectiveDate(StatesOnQuote As IEnumerable(Of Integer), effectiveDate As String)
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
            Dim StatesOnQuote = MultiStateHelper.MultistateQuoteStateIds(quote)
            Return DoesStateQualifyByEffectiveDate(StatesOnQuote, effectiveDate)
        End If
        Return False
    End Function
    Public Shared Function GetListofIntegersFromAppSettingsKey(key As String) As List(Of Integer)
        Dim c As New CommonHelperClass
        Dim integerList As List(Of Integer) = New List(Of Integer)
        Dim integerString As String = c.ConfigurationAppSettingValueAsString(key)
        If Not String.IsNullOrWhiteSpace(integerString) Then
            integerList = c.ListOfIntegerFromString(integerString, ",")
        End If
        Return integerList
    End Function

    Public Shared Function GetListofDatesFromAppSettingsKey(key As String) As List(Of Date)
        Dim c As New CommonHelperClass
        Dim dateList As List(Of Date) = New List(Of Date)
        Dim dateString As String = c.ConfigurationAppSettingValueAsString(key)
        If Not String.IsNullOrWhiteSpace(dateString) Then
            Dim dateStringList = c.ListOfStringFromString(dateString, ",")
            For Each dateString In dateStringList
                dateList.Add(DateFromString(dateString))
            Next
        End If
        Return dateList
    End Function

    'Public Shared Function GetListofDatesFromAppSettingsKeyTuple(key As String) As (calendarDate As Date, effectiveDate As Date)
    '    Dim c As New CommonHelperClass
    '    Dim dateList = (calendarDate:=Date.MinValue, effectiveDate:=Date.MinValue)  'Tuple
    '    Dim dateString As String = c.ConfigurationAppSettingValueAsString(key)
    '    If Not String.IsNullOrWhiteSpace(dateString) Then
    '        Dim dateStringList = c.ListOfStringFromString(dateString, ",")
    '        dateList.calendarDate = DateFromString(dateStringList(0))
    '        dateList.effectiveDate = DateFromString(dateStringList(1))
    '    End If
    '    Return dateList 'Tuple
    'End Function

    Public Shared Function GetEarliestDateAllowed(quote As QuickQuoteObject, effectiveDate As String) As String
        Return GetEarliestEffectiveDatePossible(quote).ToShortDateString
    End Function

    Public Shared Function GetEarliestEffectiveDatePossible(quote As QuickQuoteObject) As Date
        Dim FinalDate As Date = Date.MinValue
        Dim StatesOnQuote = MultiStateHelper.MultistateQuoteStateIds(quote)
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
                Return DateFromString(GetConfigStringForKey(NewCoInEffectiveDate))
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

    'Public Shared Function GetStartDateByLobAndCompany(quote As QuickQuoteObject) As Date
    '    Select Case quote.LobType
    '        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP

    '    End Select
    'End Function

    Public Shared Function GetConfigStringForKey(key As String) As String
        Dim c As New CommonHelperClass
        Return c.ConfigurationAppSettingValueAsString(key)
    End Function
    Public Shared Function DateFromString(dateString As String) As Date
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
    Public Shared Function CalendarEffectiveDate(configKey As String) As DateTime
        Dim dateString As String = GetConfigStringForKey(configKey)
        Return DateFromString(dateString)
    End Function


    Public Shared Function GoverningStatesAllowed(configKey As String) As List(Of Integer)
        Return GetListofIntegersFromAppSettingsKey(configKey)
    End Function

    Public Shared Function LobsAllowed(configKey As String) As List(Of Integer)
        Return GetListofIntegersFromAppSettingsKey(configKey)
    End Function

    'Public Shared Function NewCoGoverningStateEffDate(Quote As QuickQuoteObject) As Date
    '    If Not String.IsNullOrWhiteSpace(Quote?.OriginalGoverningState) Then
    '        Dim GovStateEffDate As Date = DateFromString("1/1/1800")
    '        Select Case Quote.OriginalGoverningState
    '            Case QuickQuoteHelperClass.QuickQuoteState.Indiana
    '                Dim InStateEffectiveDate As String = GetConfigStringForKey(NewCoILEffectiveDate)
    '                GovStateEffDate = DateFromString(InStateEffectiveDate)
    '            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
    '                Dim IlStateEffectiveDate As String = GetConfigStringForKey(NewCoILEffectiveDate)
    '                GovStateEffDate = DateFromString(IlStateEffectiveDate)
    '            Case QuickQuoteHelperClass.QuickQuoteState.Kentucky
    '                Dim OHStateEffectiveDate As String = GetConfigStringForKey(NewCoOhEffectiveDate)
    '                GovStateEffDate = DateFromString(OHStateEffectiveDate)
    '        End Select
    '        Return GovStateEffDate
    '    End If
    'End Function
End Class
