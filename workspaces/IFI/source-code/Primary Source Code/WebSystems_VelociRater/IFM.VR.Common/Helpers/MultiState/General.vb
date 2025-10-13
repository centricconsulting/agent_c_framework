Imports System.Runtime.CompilerServices
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports PopupMessageClass

Namespace IFM.VR.Common.Helpers.MultiState
    Public Class General

        ''' <summary>
        ''' This function and all calls to it can be removed after 3/1/2021
        ''' </summary>
        ''' <param name="myPage"></param>
        Public Shared Sub ShowOhioEffectiveDatePopup(ByRef myPage As Web.UI.Page)
            ' Use popup library
            Using popup As New PopupMessageObject(myPage, "You have selected Ohio as your state.  We cannot write in Ohio prior to 2-1-21.  Please change your effective date to 2-1-21 or later.")
                With popup
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
            Exit Sub
        End Sub

        Public Shared Function KentuckyWCPEffectiveDate() As String
            If System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate")) Then
                Return System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate").ToString
            End If

            Return "8/1/2019"
        End Function

        'Added 3/8/2022 for KY WCP Task 73077 MLW
        Public Shared Function KentuckyWCPGovStateEffectiveDate() As Date
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If System.Configuration.ConfigurationManager.AppSettings("VR_WCP_KY_GS_EffectiveDate") IsNot Nothing AndAlso
                QQHelper.IsValidDateString(System.Configuration.ConfigurationManager.AppSettings("VR_WCP_KY_GS_EffectiveDate").ToString) Then
                Return CDate(System.Configuration.ConfigurationManager.AppSettings("VR_WCP_KY_GS_EffectiveDate").ToString)
            End If
            Return CDate("5/1/2022")
        End Function

        'Added 3/15/2022 for KY WCP Task 73875 MLW
        Public Shared Function KentuckyWCPGovStateEnabled() As Boolean
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Return QQHelper.BitToBoolean(System.Configuration.ConfigurationManager.AppSettings("VR_WCP_KY_GS_Enabled"))
        End Function

        Public Shared Function IsMultistateCapableEffectiveDate(effectiveDate As String) As Boolean
            If IsDate(effectiveDate) Then
                Return IsMultistateCapableEffectiveDate(CDate(effectiveDate))
            End If
            Return False
        End Function
        Public Shared Function IsMultistateCapableEffectiveDate(effectiveDate As DateTime) As Boolean
            Return effectiveDate >= IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate
        End Function

        Public Shared Function MultiStateStartDate() As DateTime
            Dim VR_MultiStateEnabledDate As DateTime = DateTime.Parse("1-1-2019")
            If System.Configuration.ConfigurationManager.AppSettings("VR_MultiState_EffectiveDate") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.AppSettings("VR_MultiState_EffectiveDate").IsDate() Then
                VR_MultiStateEnabledDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_MultiState_EffectiveDate"))
            End If
            Return VR_MultiStateEnabledDate
        End Function

        ''' <summary>
        ''' Used before a quote type is defined like on the MyVelociRater page.
        ''' Do not use if a quote is available. 
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function VRMultiStateSupportEnabled() As Boolean
            Return System.Configuration.ConfigurationManager.AppSettings("VR_MultiStateEnabled") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.AppSettings("VR_MultiStateEnabled").ToUpper() = "TRUE"
        End Function


        'added 8/15/2018 for classes that don't have new multi-state properties in BaseControl
        ''' <summary>
        ''' Use Me.GoverningStateQuote if available.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <param name="subQuotes"></param>
        ''' <returns></returns>
        Public Shared Function GoverningStateQuote(ByRef topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional ByRef subQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As QuickQuote.CommonObjects.QuickQuoteObject
            Dim gsq As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If

            If topQuote IsNot Nothing Then
                Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                If subQuotes Is Nothing Then
                    subQuotes = qqHelper.MultiStateQuickQuoteObjects(topQuote) 'will at least contain qqo
                End If
                If subQuotes IsNot Nothing AndAlso subQuotes.Count > 0 Then 'just in case, but shouldn't be needed
                    If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), topQuote.QuickQuoteState) = True AndAlso topQuote.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                        gsq = qqHelper.QuickQuoteObjectForState(subQuotes, topQuote.QuickQuoteState, addToListIfNeeded:=False)
                    End If
                    If gsq Is Nothing Then
                        gsq = subQuotes.GetItemAtIndex(0)
                    End If
                End If
            End If
            'Todo - MS - next QuickQuote build will include this as QuickQuoteHelperClass method; will update to call helper method at that point

            Return gsq
        End Function

        ''' <summary>
        ''' Use Me.SubQuotes if available.
        ''' </summary>
        ''' <param name="qqo"></param>
        ''' <returns></returns>
        Public Shared Function SubQuotes(qqo As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Return QQHelper.MultiStateQuickQuoteObjects(qqo)
        End Function

        'this one is for C# since it doesn't seem to like omitting optional params
        Public Shared Function GoverningStateQuote_SubQuotesOmitted(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject) As QuickQuote.CommonObjects.QuickQuoteObject
            Return GoverningStateQuote(qqo)
        End Function

        Public Shared Function SubQuotesContainsState(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), stateAbbreviation As String) As Boolean
            Dim stateAbbreviations = (From s In SubQuotes Select s.State.Trim().ToUpper())
            Return stateAbbreviations.Contains(stateAbbreviation?.Trim().ToUpper())
        End Function

        Public Shared Function SubQuotesHasAnyState(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ParamArray stateTypes() As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Boolean
            If SubQuotes.IsLoaded AndAlso stateTypes.Any() Then
                'TODO-Once Dons new helper is available use it
                'Return QuickQuoteHelperClass.QuoteHasAnyStateFromList(topQuote, stateTypes.ToList())
                Return (From sq In SubQuotes Where stateTypes.Contains(sq.QuickQuoteState) Select sq).Any()
            End If
            Return False
        End Function

        Public Shared Function SubQuoteForState(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), stateType As QuickQuoteHelperClass.QuickQuoteState) As QuickQuote.CommonObjects.QuickQuoteObject
            If SubQuotes IsNot Nothing Then
                Return (From sp In SubQuotes Where sp.QuickQuoteState = stateType Select sp).FirstOrDefault()
            End If
            Return Nothing
        End Function


        Public Shared Function SubQuoteForState(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), stateAbbreviation As String) As QuickQuote.CommonObjects.QuickQuoteObject
            If SubQuotes IsNot Nothing Then
                Return (From sp In SubQuotes Where sp.State = stateAbbreviation?.Trim().ToUpper() Select sp).FirstOrDefault()
            End If
            Return Nothing
        End Function

        Public Shared Function SubQuoteForVehicle(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ByVal veh As QuickQuoteVehicle) As QuickQuoteObject 'would be used whenever vehicle controls need access to a quote to pull policy level information that could vary state
            'note: since the quote returned is based on the vehicle state, returned quote may not be accurate for all vehicles; could possibly store value in HttpContext if control only calls function for single vehicle as opposed to looping through vehicles and calling for each, which may not be accurate; may be best to store function result in private variable on control and re-use variable instead of subsequent calls to function
            If SubQuotes IsNot Nothing Then
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                'Return QQHelper.StateQuoteForVehicle(DirectCast(Me.SubQuotes, List(Of QuickQuoteObject)), veh, alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'this should always return a quote if SubQuotes has anything
                Return QQHelper.StateQuoteForVehicle(SubQuotes, veh, alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'this should always return a quote if SubQuotes has anything
            End If
            Return Nothing
        End Function

        Public Shared Function SubQuoteForLocation(SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ByVal loc As QuickQuoteLocation) As QuickQuoteObject 'would be used whenever location controls need access to a quote to pull policy level information that could vary state
            'note: since the quote returned is based on the location state, returned quote may not be accurate for all locations; could possibly store value in HttpContext if control only calls function for single location as opposed to looping through locations and calling for each, which may not be accurate; may be best to store function result in private variable on control and re-use variable instead of subsequent calls to function
            If SubQuotes IsNot Nothing Then
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                'Return QQHelper.StateQuoteForLocation(DirectCast(Me.SubQuotes, List(Of QuickQuoteObject)), loc, alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'this should always return a quote if SubQuotes has anything
                Return QQHelper.StateQuoteForLocation(SubQuotes, loc, alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'this should always return a quote if SubQuotes has anything
            End If
            Return Nothing
        End Function

        Public Shared Function MultistateQuoteStateIds(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As IEnumerable(Of Int32)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If SubQuotes Is Nothing Then
                SubQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            End If
            Return From sq In SubQuotes Where sq.StateId.TryToGetInt32() > 0 Select sq.StateId.TryToGetInt32()
        End Function

        ''' <summary>
        ''' Returns true if after excluding the governing state there are additional states that could be added to the quote.
        ''' Based on LOB and effective date.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <returns></returns>
        Public Shared Function QuoteHasMultistateOptionsAvailable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(topQuote.LobType)
            Dim AcceptableMultStateQuoteStateIdsExcludingCurrentGoverningState = lobHelper.AcceptableMultStateQuoteStateIds(topQuote.EffectiveDate).Except({CInt(topQuote.StateId)})

            If topQuote IsNot Nothing AndAlso AcceptableMultStateQuoteStateIdsExcludingCurrentGoverningState.IsLoaded() AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), topQuote.QuickQuoteState) = True AndAlso topQuote.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                Return True
            End If
            Return False
        End Function

        ' OHIO
        Public Shared Function IsOhioEffective(ByVal qt As QuickQuoteObject) As Boolean
            If qt IsNot Nothing AndAlso qt.EffectiveDate IsNot Nothing AndAlso IsDate(qt.EffectiveDate) Then
                If CDate(qt.EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Shared Function RemoveMotorizedVehiclesCoverages(ByRef Loc As QuickQuoteLocation) As Boolean
            If Loc IsNot Nothing AndAlso Loc.SectionIICoverages IsNot Nothing AndAlso Loc.SectionIICoverages.Count > 0 Then
remloop:
                For Each sc2 As QuickQuoteSectionIICoverage In Loc.SectionIICoverages
                    If sc2.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio Then
                        Loc.SectionIICoverages.Remove(sc2)
                        GoTo remloop
                    End If
                Next
            End If
            Return True
        End Function

        Public Shared Function RemoveOHPesticideHerbicideCoverages(ByRef sqs As List(Of QuickQuoteObject))
            If sqs IsNot Nothing AndAlso sqs.Count > 0 Then
                For Each sq As QuickQuoteObject In sqs
                    sq.HasHerbicidePersticideApplicator = False
                Next
            End If
            Return True
        End Function


        Public Shared Function RemoveOHStopGapCoverageFromSubquotes(ByRef sqs As List(Of QuickQuoteObject))
            If sqs IsNot Nothing AndAlso sqs.Count > 0 Then
                For Each sq As QuickQuoteObject In sqs
                    sq.StopGapLimitId = ""
                    sq.StopGapPayroll = ""
                Next
            End If
            Return True
        End Function

        Public Shared Function QuoteHasOHMineSubOnLocationDwelling(ByVal LOC As QuickQuoteLocation) As Boolean
            If LOC IsNot Nothing AndAlso LOC.SectionICoverages IsNot Nothing AndAlso LOC.SectionICoverages.Count > 0 Then
                If LOC.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB) IsNot Nothing Then Return True
            End If
            Return False
        End Function

        Public Shared Function QuoteHasILMineSubOnLocationDwelling(ByVal LOC As QuickQuoteLocation) As Boolean
            If LOC IsNot Nothing AndAlso LOC.SectionICoverages IsNot Nothing AndAlso LOC.SectionICoverages.Count > 0 Then
                If LOC.SectionICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB) IsNot Nothing Then Return True
            End If
            Return False
        End Function

    End Class

    Public Module Extensions

        <Extension()>
        Public Function SumPropertyValues(Of T)(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), Optional maintainFormating As Boolean = True, Optional verifyPropertyExists As Boolean = False) As String
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim subQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            Return QQHelper.GetSumForPropertyValues(subQuotes, expression, maintainFormating, verifyPropertyExists)
        End Function

        <Extension()>
        Public Function SumPropertyValues(Of T)(subQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), Optional maintainFormating As Boolean = True, Optional verifyPropertyExists As Boolean = False) As String
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Return QQHelper.GetSumForPropertyValues(subQuotes, expression, maintainFormating, verifyPropertyExists)
        End Function

        <Extension()>
        Public Function HasAnyTruePropertyValues(Of T)(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), Optional verifyPropertyExists As Boolean = False) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim subQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            Return QQHelper.HasAnyTruePropertyValues(subQuotes, expression, verifyPropertyExists)
        End Function

        <Extension()>
        Public Function HasAnyTruePropertyValues(Of T)(subQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), Optional verifyPropertyExists As Boolean = False) As Boolean
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Return QQHelper.HasAnyTruePropertyValues(subQuotes, expression, verifyPropertyExists)
        End Function

        <Extension()>
        Public Function HasAnyPropertyValuesMatchingString(Of T)(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), ByVal stringToMatch As String, Optional ByVal matchType As TextMatchType = TextMatchType.TextOnly_IgnoreCasing, Optional ByVal verifyPropertyExists As Boolean = False) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim subQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            Return QQHelper.HasAnyPropertyValuesMatchingString(subQuotes, expression, stringToMatch, matchType, verifyPropertyExists)
        End Function

        <Extension()>
        Public Function HasAnyPropertyValuesMatchingString(Of T)(subQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject), ByVal expression As System.Linq.Expressions.Expression(Of Func(Of T)), ByVal stringToMatch As String, Optional ByVal matchType As TextMatchType = TextMatchType.TextOnly_IgnoreCasing, Optional ByVal verifyPropertyExists As Boolean = False) As Boolean
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Return QQHelper.HasAnyPropertyValuesMatchingString(subQuotes, expression, stringToMatch, matchType, verifyPropertyExists)
        End Function


        ''' <summary>
        ''' Use Me.SubQuotes if available.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function SubQuotes(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Return General.SubQuotes(topQuote)
        End Function

    End Module

    Public Class NotTopLevelQuoteException
        Inherits Exception
        Private Const StandardMessage As String = "Expected top level quote but received state level quote."
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New()
            MyBase.New(StandardMessage)
        End Sub
    End Class

End Namespace