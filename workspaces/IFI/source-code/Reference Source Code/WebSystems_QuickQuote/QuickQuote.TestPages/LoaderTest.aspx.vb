Imports System.IO
Imports System.Xml.Serialization
Imports IFM.PolicyLoader
Imports IFM.PolicyLoader.QuickQuote
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.Umbrella

Public Class LoaderTest
    Inherits System.Web.UI.Page
    Private shortAgencyCode As String = "1840"
    Private fullAgencyCode As String = "6013-1840"
    Private DiamondUser As String = "DonBrewtonTest"
    Private DiamondPass As String = "DonBrewtonTest1"

    Private securityToken As Diamond.Common.Services.DiamondSecurityToken

    Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim chc As New CommonHelperClass

    Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML

    Dim _handlerPovider As New UnderlyingPolicyPostOpHandlerProvider
    Dim _policyLoader As New QuickQuoteUnderlyingPolicyLoaderService(qqxml, _handlerPovider)

    Private Enum PolicyNumberCollectionTypeRowItemIdentifiers
        PrintXMLID = 0
        PolicyNumber = 1
        PrintDate = 2
        System = 3
        PolicyID = 4
        PrintCategoryText = 5
        PrintLink = 6
    End Enum

    Private Enum LookupType
        Quote
        Endorsement
        Policy
    End Enum

    Shared Function PolicyValidator(qqObj As QuickQuoteObject) As (IsValid As Boolean, Messages As List(Of String))

        Dim retval = (IsValid:=True, Messages:=New List(Of String))

        If qqObj Is Nothing Then
            retval.IsValid = False
            retval.Messages.Add("Policy Not found, or cannot be loaded")
        End If
        Return retval
    End Function


    Sub testLoad_QuoteIds()
        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        Dim quoteIdsToLoad = {"505023",
                            "505022",
                            "505021",
                            "505020",
                            "505019",
                            "505018",
                            "505017",
                            "505015",
                            "505014",
                            "505013",
                            "505012",
                            "505010",
                            "505007",
                            "505005",
                            "505004",
                            "505003",
                            "505002",
                            "505001",
                            "505000",
                            "504999",
                            "504997",
                            "504996",
                            "504995",
                            "504992"
                }

        For Each qId In quoteIdsToLoad
            qqxml.GetQuoteForSaveType(qId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)
        Next
    End Sub

    Sub testLoad_Diamond_PolicyNumbers()
        Dim diamondQuotes = {"QPUP12258",
                              "QPUP12256",
                              "QPUP12222",
                              "QPUP12221",
                              "QPUP12220",
                              "QPUP010494",
                              "QPUP010492",
                              "PUP1003436"
            }
        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        For Each policyNumber In diamondQuotes
            Debug.WriteLine($"quote/policy#: {policyNumber}")
            Quote = qqxml.ReadOnlyQuickQuoteObjectForPolicyInfo(policyNumber, errorMessage:=errorMessage, validateUserAccess:=QuickQuoteHelperClass.IsTestEnvironment())

        Next

    End Sub

    Sub testLoad_PolicyId_intoQQandRate(policyId As String, policyImageNum As String)
        Dim shortAgencyCode = Right(fullAgencyCode, 4)

        Dim quoteId = ""
        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        qqxml.CreateNewQuickQuoteFromDiamondPolicyImage(policyId, policyImageNum, quoteId, shortAgencyCode, errorMessage)

        qqxml = New QuickQuoteXML 'added 4/29/2021 to force reset of hasExecutedPrepareQuickQuoteForImageConversion (when calling CreateNewQuickQuoteFromDiamondPolicyImage immediately before this), which is needed for qqXml.PrepareQuickQuoteForImageConversion to call qqo.FinalizeQuickQuoteLight, which will set the NeedsMultiStateFormat flag

        qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)
        '        qqxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, quoteId, errorMessage)

        Dim strQQ As String = "" 'not being used here for anything other than debug purposes
        Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
        Dim ratedQSO As QuickQuoteObject = Nothing
        qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQSO, strRatedQQ, quoteId, errorMessage)
    End Sub
    Sub testLoad_PolicyId_intoQQandRate2()
        Dim qqHelper As New QuickQuoteHelperClass
        Dim quoteId = ""
        Dim policyId = ""
        Dim policyImageNum = ""
        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""


        'Dim newQuoteId As String = ""
        'Dim copyError As String = ""
        'qqxml.CopyQuote("505033", newQuoteId, copyError)
        'If qqHelper.IsPositiveIntegerString(newQuoteId) = True Then
        '    quoteId = newQuoteId
        'End If



        Dim successfullyLoadedIntoVR As Boolean = False
        Dim newOrExistingQuoteId As Integer = 0
        Dim loadIntoVrErrorMsg As String = ""
        Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing



        successfullyLoadedIntoVR = qqxml.SuccessfullyLoadedDiamondQuoteIntoVelociRater("QPUP12279", newOrExistingQuoteId, newQuote, loadIntoVrErrorMsg)
        If successfullyLoadedIntoVR = True AndAlso String.IsNullOrEmpty(loadIntoVrErrorMsg) = True AndAlso newOrExistingQuoteId > 0 AndAlso newQuote IsNot Nothing Then
            quoteId = newOrExistingQuoteId
        End If



        'Dim qqoRO As QuickQuoteObject = qqxml.ReadOnlyQuickQuoteObjectForPolicyInfo(policyNumber:="QPUP12279")
        'If qqoRO IsNot Nothing Then



        'End If
        'Exit Sub



        Dim shortAgencyCode = Right(fullAgencyCode, 4)



        'policyId = "2056357" 'Kevin's test from OldRelease (QPUP12258)
        'policyImageNum = "1"
        policyId = "1996076" 'PUP1009940 from OldRelease
        policyImageNum = "1"
        'qqxml.CreateNewQuickQuoteFromDiamondPolicyImage(policyId, policyImageNum, quoteId, shortAgencyCode, errorMessage)
        'quoteId = "505029" 'QPUP12270 in OldRelease; originally from policyId 2056357 (QPUP12258)
        'quoteId = "505031" 'originally from policyId 1996076 (PUP1009940)
        If qqHelper.IsPositiveIntegerString(quoteId) = True Then
            qqxml = New QuickQuoteXML 'added 4/29/2021 to force reset of hasExecutedPrepareQuickQuoteForImageConversion (when calling CreateNewQuickQuoteFromDiamondPolicyImage immediately before this), which is needed for qqXml.PrepareQuickQuoteForImageConversion to call qqo.FinalizeQuickQuoteLight, which will set the NeedsMultiStateFormat flag
            qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)
            If Quote IsNot Nothing Then
                '        qqxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, quoteId, errorMessage)



                'needed to get quoteId(s) 505029, 505031 to Save to Diamond; logic now applied to QuickQuoteXML
                'If Quote.ResidenceInfo IsNot Nothing Then
                '    If Quote.ResidenceInfo.HasData = False Then
                '        Quote.ResidenceInfo.Dispose()
                '        Quote.ResidenceInfo = Nothing
                '    End If
                'End If
                'If Quote.MultiStateQuotes IsNot Nothing AndAlso Quote.MultiStateQuotes.Count > 0 Then
                '    For Each msq As QuickQuoteObject In Quote.MultiStateQuotes
                '        If msq IsNot Nothing Then
                '            If msq.ResidenceInfo IsNot Nothing Then
                '                If msq.ResidenceInfo.HasData = False Then
                '                    msq.ResidenceInfo.Dispose()
                '                    msq.ResidenceInfo = Nothing
                '                End If
                '            End If
                '        End If
                '    Next
                'End If



                'needed for quoteId(s) 505031
                If qqh.IsValidDateString(Quote.EffectiveDate, mustBeGreaterThanDefaultDate:=True, defaultDate:=DateAdd(DateInterval.Day, -30, Date.Today)) = False Then
                    Quote.EffectiveDate = Date.Today.ToShortDateString
                End If



                Dim strQQ As String = "" 'not being used here for anything other than debug purposes
                Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
                Dim ratedQSO As QuickQuoteObject = Nothing
                qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQSO, strRatedQQ, quoteId, errorMessage)
                If String.IsNullOrWhiteSpace(errorMessage) = False Then



                End If
            End If
        End If
    End Sub
    Sub testLoad_QuoteId_andRate()
        Dim quoteId = "505057"
        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)

        Dim strQQ As String = "" 'not being used here for anything other than debug purposes
        Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
        Dim ratedQSO As QuickQuoteObject = Nothing
        qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQSO, strRatedQQ, quoteId, errorMessage)
        Debugger.Break()
    End Sub

    Sub TestLoad_QuoteId(quoteId As String, Optional doRate As Boolean = False)

        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        If String.IsNullOrWhiteSpace(quoteId) = False Then
            qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)
        End If


        If doRate Then
            Dim strQQ As String = "" 'not being used here for anything other than debug purposes
            Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
            Dim ratedQSO As QuickQuoteObject = Nothing
            qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQSO, strRatedQQ, quoteId, errorMessage)

        End If

        Debugger.Log(1, "DEBUG",
$"
QuoteId: {quoteId}
Quote status: {Quote.QuoteStatus}
Erorr Message: {errorMessage}
")
    End Sub

    Sub TestLoad_PolicyIdandImageNum(policyId As String, policyImageNum As String, Optional doRate As Boolean = False)

        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""
        Dim quoteId = ""
        Dim description = ""

        If Not String.IsNullOrWhiteSpace(policyId) AndAlso Not String.IsNullOrWhiteSpace(policyImageNum) Then
            qqxml.CreateNewQuickQuoteFromDiamondPolicyImage(policyId, policyImageNum, quoteId, fullAgencyCode, errorMessage, description)
            qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMessage)
        End If


        If doRate Then
            Dim strQQ As String = "" 'not being used here for anything other than debug purposes
            Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
            Dim ratedQSO As QuickQuoteObject = Nothing
            qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQSO, strRatedQQ, quoteId, errorMessage)

        End If

        Debugger.Log(1, "DEBUG",
$"
QuoteId: {quoteId}
Quote status: {Quote.QuoteStatus}
Erorr Message: {errorMessage}
")
    End Sub

    Sub testLoad_Quote_PolicyNumbersAsUnderlyingPolicies(ParamArray policyNumbersToTest As String())

        Dim Quote As New QuickQuoteObject
        Dim errorMessage = ""

        For Each policyNum As String In policyNumbersToTest

            Dim Request = New LoadPolicyRequest(Of QuickQuoteObject)(policyNum, Quote)
            Dim result = _policyLoader.LoadPolicy(Request, AddressOf PolicyValidator)

            Debugger.Break()
            Debugger.Log(1, "INFO",
$"Policy/QuoteNumber: {Request.PolicyNumber}
Loaded: {result.PolicyLoaded}
Validated: {result.PassedValidation}
LOB: {DirectCast(result.Previous?.Data, QuickQuoteObject)?.LobType}
#PolicyInfos: {result.Data?.PolicyInfos?.Count}
More results?: {result.Next IsNot Nothing}
Messages: {String.Join("\n", result.Messages)}
"
)

            '''***
            '''if a <see cref="LoadPolicyResult(Of T)">LoadPolicyResult</see> has a "Next" property that is non-null, 
            '''then multiple handlers were found and processed the request.
            '''cast the Next to an OperationResult <see cref="OperationResult"/> 
            '''to quickly get the proper data value from Next 
            '''or cast Next.Data to QuickQuoteUnderlyingPolicy
            '''


            'these are the same
            If result.Next IsNot Nothing Then
                Dim a As QuickQuoteUnderlyingPolicy = CType(result.Next.Data, QuickQuoteUnderlyingPolicy)


                Debugger.Log(1, "INFO",
$"
PrimaryPolicyNumber: {a.PrimaryPolicyNumber}
#PolicyInfos: {a.PolicyInfos?.Count}
"
)
                Dim result_cast As OperationResult = CType(result.Next, OperationResult)
                Dim b As QuickQuoteUnderlyingPolicy = result_cast.Data
                Debugger.Log(1, "INFO",
$"
PrimaryPolicyNumber: {b.PrimaryPolicyNumber}
#PolicyInfos: {b.PolicyInfos?.Count}
"
)

            End If



        Next
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim pvhc As New QuickQuotePropertyValuationHelperClass
        'Dim authId As String = ""
        'Dim authCode As String = ""
        'pvhc.SetE2ValueAuthIdAndCode("INFexternal", "MjXl17#2tT", "3000", authId:=authId, authCode:=authCode)
        'Exit Sub

        Dim policyNumbersUnderlying_PUP As String() = {
            "FAR1036646"}
        '"PPA2140003",
        '"FAR1034050",
        '"FAR1034030",
        '  "PPA2140001",
        '"PPA2140007",
        '    "FAR1034032",
        '    "QFAR46372",
        '    "HOM2143537",
        '    "QFAR46364",
        '    "QFAR46331",
        '    "HOM2143530",
        '    "DFR1019001",
        '    "PPA2139997",
        '    "QDFR31509",
        '    "QWCP25593",
        '    "QDFR31509",
        '    "QFAR46326",
        '    "PPA2103178",
        '    "HOM2143529",
        '    "PPA2008318",
        '    "HOM1001711",
        '    "PPA2020853",
        '    "DFR1007667",
        '    "WCP1000431",
        '    "FAR1000566",
        '    "DFR1007403",
        '    "CAP1002397 ",
        '    "HOM2058302"
        '}


        'QFAR46331 will have multiplw results
        testLoad_Quote_PolicyNumbersAsUnderlyingPolicies(policyNumbersUnderlying_PUP)


    End Sub

    'Private Sub getQuote(qid As String)
    '    If String.IsNullOrWhiteSpace(qid) = False Then
    '        GenericSetup(qid)

    '        If 1 = 1 Then

    '        End If
    '    End If
    'End Sub
    'Private Sub doRateQuote(qid As String)
    '    If String.IsNullOrWhiteSpace(qid) = False Then
    '        GenericSetup(qid)

    '        Dim page As String = "myPage"
    '        Dim key As String = "myKey"

    '        If Quote IsNot Nothing Then
    '            Quote.SetDevDictionaryItem(page, key, "value1")

    '            ReSaveOrReRate(qid, False, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
    '        End If
    '    End If
    'End Sub
    'Private Sub ReSaveOrReRate(ByVal qId As String, Optional ByVal ReloadForquoteId As Boolean = False, Optional ByVal saveOrRate As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, Optional ByVal saveOrRateType As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
    '    Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML
    '    Dim err As String = ""

    '    Dim strQQ As String = ""
    '    Dim ratedQQ As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
    '    Dim strRatedQQ As String = ""

    '    If ReloadForquoteId = True Then
    '        Quote = Nothing

    '        If qqh.IsNumericString(qId) = True Then
    '            'get existing
    '            qqxml.GetQuoteForSaveType(qId, saveOrRateType, Quote, err)
    '        End If
    '    End If

    '    If Quote IsNot Nothing Then
    '        If saveOrRate = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Save Then
    '            qqxml.SaveQuote(saveOrRateType, Quote, qId, err)
    '        Else
    '            qqxml.RateQuoteAndSave(saveOrRateType, Quote, strQQ, ratedQQ, strRatedQQ, qId, err) 'debug method w/ byref params for rated QuickQuoteObject and xml strings for the request and response
    '        End If
    '    End If

    '    If String.IsNullOrWhiteSpace(err) = False Then
    '        If 1 = 1 Then 'break here to check error message

    '        End If
    '    Else
    '        If 1 = 1 Then 'success!

    '        End If
    '    End If

    'End Sub
    'Private Sub GetLocation()
    '    If Quote IsNot Nothing AndAlso Quote.Locations(0) IsNot Nothing Then
    '        QuoteLocation = Quote.Locations(0)
    '    End If
    'End Sub
    'Private Sub Login()
    '    QuickQuote.CommonMethods.QuickQuoteHelperClass.CheckDiamondServicesToken()
    '    If Diamond.Web.BaseControls.SignedOnUserID <= 0 Then
    '        If 1 = 1 Then

    '        End If
    '    End If
    'End Sub
    'Private Sub GenericSetup(Optional quoteID As String = "")
    '    Dim errorMsg As String = ""
    '    Dim currentQuoteId As String = "227640" '"227244" '"227286" '"226158"
    '    Dim newQuoteId As String = ""
    '    Dim copyQuote As Boolean = False
    '    Dim quoteIdToUse As String = ""
    '    Dim valCount As Integer = 0
    '    Dim doRerate As Boolean = False
    '    Login()

    '    If String.IsNullOrWhiteSpace(quoteID) = False Then
    '        currentQuoteId = quoteID
    '    End If

    '    If String.IsNullOrWhiteSpace(currentQuoteId) = False Then
    '        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

    '        If String.IsNullOrWhiteSpace(errorMsg) Then
    '            If String.IsNullOrWhiteSpace(newQuoteId) Then
    '                If String.IsNullOrWhiteSpace(quoteID) Then
    '                    quoteIdToUse = currentQuoteId
    '                Else
    '                    quoteIdToUse = quoteID
    '                End If
    '            Else
    '                quoteIdToUse = newQuoteId
    '            End If
    '        End If
    '    End If
    'End Sub

    'Private Function CopyAndOrLoadQuote(QuoteId As String, Optional doCopyQuote As Boolean = False) As String
    '    Dim returnVar As String = ""

    '    If String.IsNullOrWhiteSpace(QuoteId) = False Then
    '        If doCopyQuote = True Then
    '            Dim newQuoteId As String = ""
    '            qqxml.CopyQuote(QuoteId, newQuoteId, errorMsg)

    '            If String.IsNullOrWhiteSpace(errorMsg) Then
    '                qqxml.GetQuote(newQuoteId, Quote, errorMsg)

    '                If String.IsNullOrWhiteSpace(newQuoteId) = False Then
    '                    returnVar = newQuoteId
    '                End If
    '            End If
    '        Else
    '            qqxml.GetQuoteForSaveType(QuoteId, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMsg, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrRatedXml.AlwaysUseRatedWhenAvailable, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrAppXml.UseDefault)
    '            'qqxml.GetQuote(QuoteId, Quote, errorMsg)
    '        End If
    '    End If

    '    Return returnVar
    'End Function
End Class

