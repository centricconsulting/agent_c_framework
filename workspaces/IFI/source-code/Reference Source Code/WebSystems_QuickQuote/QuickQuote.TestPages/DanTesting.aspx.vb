'Imports System.Reflection
Imports Diamond.Common.Objects.ThirdParty
Imports Insuresoft.RuleEngine.Common.Objects.Execution

Partial Class DanTesting
    Inherits System.Web.UI.Page

    'Private conn As String = "Server=ifmsql;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=ifmtester;Max Pool Size=400;"
    'Private connDiamond As String = "Server=IFMDIASQL1;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=Diamond;Max Pool Size=400;"
    'Private connDiamondReports As String = "Server=IFMDIASQL1;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=IFM_Reports;Max Pool Size=400;"
    Private SaveLocation As String = "C:\Users\dagug\Desktop\Declarations"
    Private NoPDFFile As String = "C:\Users\dagug\Desktop\Declarations\NoPDFsFound.txt"
    Private ProgressTrackerFile As String = "C:\Users\dagug\Desktop\Declarations\ProgressTracker.txt"
    Private shortAgencyCode As String = "3308"
    Private fullAgencyCode As String = "6046-3308"
    Private DiamondUser As String = "PrintServices"
    Private DiamondPass As String = "PrintServices"
    Private errorMessage As String = ""
    Private inforcePolicyCount As Integer = 0
    Private noPDFAvailableList As New List(Of String)
    Private securityToken As Diamond.Common.Services.DiamondSecurityToken

    Private Enum FormsTypeIds
        HO2 = 1
        HO3 = 2
        HO3_HO15 = 3
        HO4 = 4
        HO6 = 5
        ML2 = 6
        ML4 = 7
        DP0001_FIRE = 8
        DP0001_FIRE_EC = 9
        DP0001_FIRE_EC_VMM = 10
        DP0002 = 11
        DP0003 = 12
        FO2 = 15
        FO3 = 16
        FO4 = 17
        FO0005 = 18
        SOM1 = 19
        SOM2 = 20
        HO5 = 21
        HO0002 = 22
        HO0003 = 23
        HO0004 = 25
        HO0005 = 24
        HO0006 = 26
    End Enum

    Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim chc As New CommonHelperClass
    Dim Quote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QuoteLocation As QuickQuote.CommonObjects.QuickQuoteLocation
    Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim errorMsg As String = ""
    Dim SectionCoveragesCount As Integer = 0
    Dim SectionCoveragesRemovedCount As Integer = 0
    Dim SectionCoveragesQQRemovedCount As Integer = 0

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

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
                Return Quote.Locations(0)
            End If
        End Get
    End Property

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Test3()
        'Test4()
        'Test5()
        'Test6()
        'Test7()
        'Test8()
        'Test11()
        'Test12()
        'Test13()
        'Test14()
        'Test15()
        'Test16()
        'Test17()
        'Test18()
        'getQuote("403459")
        'doRateQuote("403524")
        'Test20()
        'Test21()
        'NewCoEndorsementTest()
        'DonEndorsementTest()
        Test22()
    End Sub
    Sub Test22()
        Dim qID As String = "137"
        getQuote(qID)
        Dim polID = Quote.PolicyId
        qqxml.LoadThirdPartyDataForQuote_WithOptionalParams(qqxml.ThirdPartyDataType.CLUE_Property, Quote, Nothing)
    End Sub
    Sub Test21()
        Dim qID As String = "2271"

        'Get Policy from QQ Database
        'GenericSetup(qID)

        'Get Policy from Diamond Database
        Quote = qqxml.ReadOnlyQuickQuoteObjectForPolicyInfo(policyId:=2428191, policyImageNum:=1)

        If 1 = 1 Then
            Dim test1 = Quote.MultiStateQuotes(0).IRPM_CAP_Management
            Dim test2 = Quote.MultiStateQuotes(0).IRPM_CAP_ManagementDesc
            Dim test3 = Quote.MultiStateQuotes(0).IRPM_CAP_Employees
            Dim test4 = Quote.MultiStateQuotes(0).IRPM_CAP_EmployeesDesc
            Dim test5 = Quote.MultiStateQuotes(0).IRPM_CAP_Equipment
            Dim test6 = Quote.MultiStateQuotes(0).IRPM_CAP_EquipmentDesc
            Dim test7 = Quote.MultiStateQuotes(0).IRPM_CAP_SafetyOrganization
            Dim test8 = Quote.MultiStateQuotes(0).IRPM_CAP_SafetyOrganizationDesc

            Dim test11 = Quote.MultiStateQuotes(0).IRPM_CAP_Management_Phys_Damage
            Dim test12 = Quote.MultiStateQuotes(0).IRPM_CAP_ManagementDesc_Phys_Damage
            Dim test13 = Quote.MultiStateQuotes(0).IRPM_CAP_Employees_Phys_Damage
            Dim test14 = Quote.MultiStateQuotes(0).IRPM_CAP_EmployeesDesc_Phys_Damage
            Dim test15 = Quote.MultiStateQuotes(0).IRPM_CAP_Equipment_Phys_Damage
            Dim test16 = Quote.MultiStateQuotes(0).IRPM_CAP_EquipmentDesc_Phys_Damage
            Dim test17 = Quote.MultiStateQuotes(0).IRPM_CAP_SafetyOrganization_Phys_Damage
            Dim test18 = Quote.MultiStateQuotes(0).IRPM_CAP_SafetyOrganizationDesc_Phys_Damage

        End If



    End Sub


    Sub Test20()
        Dim err As String = ""
        Dim qId As String = ""
        Dim strQQ As String = ""
        Dim strRatedQQ As String = ""
        Dim ratedQQ As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim validationItems As New List(Of QuickQuote.CommonObjects.QuickQuoteValidationItem)

        'CopyQuoteAndChangeState("906", False, err, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
        'CopyQuote("906", False, err)

        'If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(err) Then
        '    'qqxml.SaveQuote(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, qId, err)
        '    qqxml.RateQuoteAndSave(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQQ, strRatedQQ, qId, err)
        '    validationItems = ratedQQ.ValidationItems
        '    If 1 = 1 Then

        '    End If
        'End If

        'CopyQuoteAndChangeState("905", False, err, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
        'Dim inQuote As New QuickQuote.CommonObjects.QuickQuoteObject
        'qqxml.GetQuoteForSaveType("905", QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, inQuote, errorMsg, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrRatedXml.AlwaysUseRatedWhenAvailable, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrAppXml.UseDefault)
        getQuote("1668")
        'getQuote("940") '- BOP
        'getQuote("976") '- BOP

        If String.IsNullOrWhiteSpace(errorMsg) AndAlso Quote IsNot Nothing Then
            'Dim toplevelProgramTypeId = Quote.ProgramTypeId
            'Dim topLevelProgramType = Quote.ProgramType
            'Dim subProgramTypeId = Quote.MultiStateQuotes(0).ProgramTypeId
            'Dim subProgramType = Quote.MultiStateQuotes(0).ProgramType
            'ChangeAgency(Quote, "13-1234", "572")
            'CopyPolicyholderToMultiStateApplicantIfNoApplicantsExist(Quote)
            'CopyLocationListAndChangeAddress(inQuote, Quote)
            '(Quote)
            'Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.Add(NewSectionCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB))
            'Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.RemoveAt(Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.Count - 1)
            'RemoveBlankCoverages()
            'Dim sc As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
            'sc = Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.Find(Function(x) x.CoverageCodeId = 80555)
            'Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.Remove(sc)
            'sc = NewCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType.Cosmetic_Damage_Exclusion)
            'sc.RoofSurfacing = True
            'sc.ExteriorWallSurfacing = True
            'sc.ExteriorDoorWindowSurfacing = True
            'Quote.MultiStateQuotes(0).Locations(0).SectionICoverages.Add(sc)
            'CopyBuildingsFromOriginalQuoteToNewQuote(inQuote, Quote)
            'Dim oce As QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE = NewCoverage(Of QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE)(QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E)
            'oce.ExteriorDoorWindowSurfacing = True
            'oce.ExteriorWallSurfacing = True
            'oce.RoofSurfacing = True
            'Quote.MultiStateQuotes(0).Locations(0).Buildings(0).OptionalCoverageEs.Add(oce)
            'Quote.MultiStateQuotes(0).Locations(0).Buildings(0).OptionalCoverageEs.RemoveAt(0)
            'Quote.MultiStateQuotes(0).Locations(0).Buildings(0).OptionalCoverageEs.Add(NewCoverage(Of QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE)(QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Farm_Mine_Subsidence))
            'Quote.MultiStateQuotes(0).Locations(0).SectionIICoverages.Add(NewCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio))
            'Quote.MultiStateQuotes(0).StopGapPayroll = "500"
            'Quote.MultiStateQuotes(0).StopGapLimitId = "311" '430, 313, 314 - Commercial; 311, 313, 314 - Farm
            'Quote.StopGapPayroll = "500"
            'Quote.StopGapLimitId = "430" '430, 313, 314
            'Quote.MultiStateQuotes(0).StopGapPayroll = "500"
            'Quote.MultiStateQuotes(0).StopGapLimitId = "430"
            'Quote.MultiStateQuotes(0).OccurrenceLiabilityLimitId = 33
            'Quote.MultiStateQuotes(0).HasHerbicidePersticideApplicator = False
            'Quote.ProgramTypeId = "61"
            'qqxml.RateQuoteAndSave(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, strQQ, ratedQQ, strRatedQQ, qId, err)

            Dim MSStopGapPayrollAmount As String = Quote.MultiStateQuotes(0).StopGapPayroll
            Dim MSStopGapLimitId As String = Quote.MultiStateQuotes(0).StopGapLimitId
            Dim MSPayroll As String = Quote.MultiStateQuotes(0).PayrollAmount
            Dim Payroll As String = Quote.PayrollAmount

            'Quote.MultiStateQuotes(0).StopGapPayroll = "25000"
            'Quote.MultiStateQuotes(0).PayrollAmount = "25000"
            'Quote.MultiStateQuotes(0).StopGapLimitId = "313"

            'Dim StopGapPayrollAmount = Quote.StopGapPayroll
            'Dim StopGapLimitId = Quote.StopGapLimitId

            'qqxml.SaveQuote(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, qId, err)

            If String.IsNullOrWhiteSpace(err) AndAlso ratedQQ IsNot Nothing Then
                validationItems = ratedQQ.ValidationItems
                If 1 = 1 Then

                End If
            Else
                If 1 = 1 Then
                    'Dim myQQId As String = ""
                    'Dim myErr As String = ""
                    'qqxml.SaveQuote(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, myQQId, myErr)
                End If
            End If
        Else
            If 1 = 1 Then

            End If
        End If

    End Sub

    Sub Test19()
        'LoadEndorsement(660636, 12, errorMessage)
        LoadEndorsement(58569, 31, errorMessage)

        'If Quote IsNot Nothing AndAlso Quote.DevDictionary IsNot Nothing Then
        '    If Quote.DevDictionary.Count > 0 Then
        '        Quote.RemoveDevDictionaryItem("global", "myTestKey")
        '    Else
        '        Quote.SetDevDictionaryItem("global", "myTestKey", "123")
        '    End If
        'End If

        'SaveEndorsement()
    End Sub

    Private Sub getQuote(qid As String)
        If String.IsNullOrWhiteSpace(qid) = False Then
            GenericSetup(qid)

            If 1 = 1 Then

            End If
        End If
    End Sub
    Private Sub doRateQuote(qid As String)
        If String.IsNullOrWhiteSpace(qid) = False Then
            GenericSetup(qid)
            If Quote IsNot Nothing Then
                ReSaveOrReRate(qid, False, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
            End If
        End If
    End Sub

    Private Sub Test18()
        'Dim key As String = "myTestPage@!@myTestKey"
        'Dim delimitedStringKeys As String = ""
        'Dim delimitedStringPages As String = ""

        'Dim list As String() = Split(key, QuickQuote.CommonMethods.QuickQuoteHelperClass.GetQQDevDictionaryPageAndKeyDelimiter, , CompareMethod.Text)

        'delimitedStringPages = Split(key, QuickQuote.CommonMethods.QuickQuoteHelperClass.GetQQDevDictionaryPageAndKeyDelimiter, , CompareMethod.Text)(0)
        'delimitedStringKeys = Split(key, QuickQuote.CommonMethods.QuickQuoteHelperClass.GetQQDevDictionaryPageAndKeyDelimiter, , CompareMethod.Text)(1)

    End Sub

    Private Sub Test17()
        Dim qID As String = "400493" ' "400490" ' "831" '"833"

        GenericSetup(qID)

        If Quote IsNot Nothing Then
            If 1 = 1 Then

            End If

            'If Quote IsNot Nothing Then
            '    ReSaveOrReRate(qID)
            'End If

            'Quote.RemoveDevDictionaryItem("myvrpage_uielementtest")

        End If

    End Sub

    Private Sub Test16()
        Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim successfullyLoadedIntoVR As Boolean = False
        Dim quoteNum As String = "QPPA600239".ToUpper()
        Dim newOrExistingQuoteId As Integer = 0
        Dim loadIntoVrErrorMsg As String = ""
        Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

        successfullyLoadedIntoVR = QQxml.SuccessfullyLoadedDiamondQuoteIntoVelociRater(quoteNum, newOrExistingQuoteId, newQuote, loadIntoVrErrorMsg)

    End Sub

    Private Sub Test15()
        Dim myqso As New QuickQuote.CommonObjects.QuickQuoteObject
        myqso.EffectiveDate = "05/31/2019"
        myqso.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
        myqso.QuickQuoteState = Nothing
        Dim test As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteStatesForQuote(myqso)
    End Sub

    Private Sub Test14()
        'Advanced Quote Discount Testing
        '833 - has indiana and illinois on quote - 8/10/2019
        '831 - Kentucky - 7/10/2019
        '274267
        Dim qID As String = "274267" ' "831" '"833"
        Dim mybool As Boolean

        'Dim minEffDate As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.MinimumEffectiveDateDaysFromToday()
        'Dim maxEffDate As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.MaximumEffectiveDateDaysFromToday()
        'Dim dateToUse As String = Date.Now().ToShortDateString()
        'Dim _QuoteHasMinimumEffectiveDate As Boolean = False
        'Dim _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean = False

        GenericSetup(qID)

        If Quote IsNot Nothing Then
            'Quote.EffectiveDate = "05/20/2019"
            'Dim uncrossableDateLineDict As New Dictionary(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions, String)
            'mybool = qqh.HasIFMLOBVersionUncrossableDateLineWithinRangeOfToday(Nothing, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, uncrossableDateLineDict)
            mybool = qqh.IsEffectiveDateChangeCrossingUncrossableDateLine(Quote, Quote.EffectiveDate, "7/5/2019", "")

            'For Each uncrossableKeyValuePair As KeyValuePair(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions, String) In uncrossableDateLineDict
            '    If IsDate(uncrossableKeyValuePair.Value) Then
            '        Dim uncrossableDate As Date = CDate(uncrossableKeyValuePair.Value)
            '        If uncrossableDate <> Nothing Then
            '            If dateToUse >= uncrossableDate Then
            '                If uncrossableDate > CDate(minEffDate) Then
            '                    _QuoteHasMinimumEffectiveDate = True
            '                    minEffDate = uncrossableDate
            '                    _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = True
            '                End If
            '            Else

            '            End If
            '        End If
            '    End If
            'Next

            'ReSaveOrReRate(qID, False, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
        End If

        If 1 = 1 Then

        End If

        'If Quote IsNot Nothing Then
        '    ReSaveOrReRate(qID)
        'End If
    End Sub

    Private Sub GetAgencyPolicyDeclarations()
        Dim allAgencyPolicies As New Data.DataTable
        Dim allInForcePolicies As New Data.DataTable

        allAgencyPolicies = GetAllAgencyPolcies()

        allInForcePolicies = GetAllAgencyInForcePolicies(allAgencyPolicies)

        SavePolicyDeclarations(allInForcePolicies)
    End Sub

    Private Function GetAllAgencyPolcies() As Data.DataTable
        Dim dt As Data.DataTable = Nothing

        'Using plc As New PolicyLookupCollection(PolicyLookupCollection.PolicyLookupSearchType.PolicyNumber)
        '    plc.connection = conn
        '    plc.connectionDiamond = connDiamond
        '    plc.connectionDiamondReports = connDiamondReports
        '    plc.AgencyCode = shortAgencyCode
        '    plc.PolicyNumberToSearch = ""
        '    plc.UseDiamondData = True
        '    dt = plc.GetResults()
        'End Using

        Return dt
    End Function

    Private Function GetAllAgencyInForcePolicies(dt As Data.DataTable) As Data.DataTable
        Dim myDT As Data.DataTable = Nothing

        If dt IsNot Nothing AndAlso dt.Rows IsNot Nothing AndAlso dt.Rows.Count > 1 Then
            myDT = dt.Copy()
            myDT.Rows.Clear()
            Dim policyStatus As String = ""
            For Each policy As Data.DataRow In dt.Rows
                policyStatus = policy(3).ToString()
                If policyStatus.Equals("in-force", StringComparison.OrdinalIgnoreCase) Then
                    myDT.ImportRow(policy)
                End If
            Next
        End If

        inforcePolicyCount = myDT.Rows.Count

        Return myDT
    End Function

    Private Sub SavePolicyDeclarations(dt As Data.DataTable)
        Dim policyNumber As String = ""
        Dim loopCount As Integer = 0
        Dim linkSubStringed As String = ""
        Dim currentProgress As String = ""
        Dim hasSQLError As Boolean = False

        For Each policy As Data.DataRow In dt.Rows
            policyNumber = policy(0)

            SavePolicyDeclaration(policyNumber, hasSQLError)

            If hasSQLError = True Then
                WriteToFile(ProgressTrackerFile, "SQLError! - " & errorMessage, False)
                Exit For
            Else
                loopCount += 1
                currentProgress = loopCount.ToString() & " / " & dt.Rows.Count
                WriteToFile(ProgressTrackerFile, currentProgress, False)
            End If
        Next
    End Sub

    Private Sub SavePolicyDeclaration(policyNumber As String, Optional ByRef hasSQLError As Boolean = False)
        Dim PolicyDecData As New Data.DataTable
        Dim policyID As String = ""
        Dim printXMLID As String = ""
        Dim printCategoryText As String = ""

        'Using pno As New PolicyNumberObject(policyNumber, conn, connDiamond)
        '    pno.GetPolicyInfo()
        '    policyID = pno.PolicyID
        'End Using

        'Using pndc As New IFMWebData.PolicyNumberDataCollection(policyNumber, IFMWebData.WebEnumerations.IFMPolicyNumberCollectionType.Declarations)
        '    PolicyDecData = pndc.GetData()
        '    If pndc.hasSQLerror = False Then
        '        If PolicyDecData IsNot Nothing AndAlso PolicyDecData.Rows IsNot Nothing AndAlso PolicyDecData.Rows.Count > 0 Then
        '            Dim latestDecRow As Data.DataRow = GetLatestDeclaration(PolicyDecData, policyID)

        '            printXMLID = latestDecRow.Item(PolicyNumberCollectionTypeRowItemIdentifiers.PrintXMLID)
        '            policyID = latestDecRow.Item(PolicyNumberCollectionTypeRowItemIdentifiers.PolicyID)
        '            printCategoryText = latestDecRow.Item(PolicyNumberCollectionTypeRowItemIdentifiers.PrintCategoryText)

        '            GetPDF(policyNumber, policyID, printXMLID, printCategoryText)
        '        Else
        '            WriteToFile(NoPDFFile, policyNumber & " - " & policyID & " - No PolicyNumberDataCollection data for policy", True)
        '        End If
        '    Else
        '        hasSQLError = True
        '        errorMessage = pndc.SQLerrorMessage
        '    End If
        'End Using
    End Sub

    Private Function GetLatestDeclaration(dt As Data.DataTable, policyID As String) As Data.DataRow
        Dim myRowNumber As Integer = Nothing
        Dim rowPolicyID As String = ""

        If dt IsNot Nothing AndAlso dt.Rows IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim loopCount As Integer = 0
            Dim myDate As Date = Date.MinValue

            For Each row As Data.DataRow In dt.Rows
                Dim rowDate As Date = Nothing
                rowPolicyID = row.Item(PolicyNumberCollectionTypeRowItemIdentifiers.PolicyID)
                If policyID.Equals(rowPolicyID, StringComparison.OrdinalIgnoreCase) Then
                    If Date.TryParse(row.Item(PolicyNumberCollectionTypeRowItemIdentifiers.PrintDate), rowDate) AndAlso rowDate > myDate Then
                        myRowNumber = loopCount
                        myDate = rowDate
                    End If
                End If
                loopCount += 1
            Next
        End If

        Return dt.Rows(myRowNumber)
    End Function

    Private Sub GetPDF(policyNumber As String, policyID As String, printXml As String, formDescription As String)
        Using diaPrint As New DiamondWebClass.DiamondPrinting
            Dim pdfByte As Byte()

            If securityToken Is Nothing OrElse securityToken.DiamUserId = 0 Then
                securityToken = diaPrint.loginDiamond(DiamondUser, DiamondPass)
            End If

            If String.IsNullOrWhiteSpace(policyID) = False AndAlso IsNumeric(policyID) = True Then
                pdfByte = diaPrint.printDec(securityToken, CInt(policyID), CInt(printXml), formDescription)
            Else
                pdfByte = diaPrint.printDec(Nothing, CInt(policyID), CInt(printXml), formDescription)
            End If

            If pdfByte IsNot Nothing Then
                SavePDF(pdfByte, policyNumber)
            Else
                WriteToFile(NoPDFFile, policyNumber & " - No PDFBytes returned for policy", True)
            End If
        End Using
    End Sub

    Private Sub SavePDF(myPDFByte As Byte(), policyNumber As String)
        System.IO.File.WriteAllBytes(SaveLocation & "\" & policyNumber & ".pdf", myPDFByte)
    End Sub

    Private Sub WriteToFile(fileToWriteTo As String, text As String, AppendText As Boolean)
        Dim file As System.IO.StreamWriter = Nothing
        file = My.Computer.FileSystem.OpenTextFileWriter(fileToWriteTo, AppendText)
        file.WriteLine(text)
        file.Close()
    End Sub

    Private Sub Test13()
        'Advanced Quote Discount Testing
        Dim qID As String = "273318"

        GenericSetup(qID)

        Dim test As Boolean = False

        'Quote.EffectiveDate = "05/20/2019"

        If Quote IsNot Nothing Then
            ReSaveOrReRate(qID, False, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
        End If

        If 1 = 1 Then

        End If

        'If Quote IsNot Nothing Then
        '    ReSaveOrReRate(qID)
        'End If
    End Sub

    Private Sub Test12()
        Dim qID As String = "227963"
        GenericSetup(qID)

        If Quote IsNot Nothing Then
            If GetListIfSomething(Quote.Locations) IsNot Nothing Then
                Dim secICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage) = Nothing
                Dim secIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = Nothing
                Dim secIandIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) = Nothing

                InitializeSectionCoveragesIfNeeded()

                'secICovs = Quote.Locations(0).SectionICoverages
                'secIICovs = Quote.Locations(0).SectionIICoverages
                secIandIICovs = Quote.Locations(0).SectionIAndIICoverages

                If GetListIfSomething(secIandIICovs) IsNot Nothing Then
                    If secIandIICovs.Find(Function(x) x.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage) IsNot Nothing Then
                        Dim first As Boolean = True
                        For Each student As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In (secIandIICovs.Find(Function(x) x.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage).AdditionalInterests)
                            'If first = True Then
                            '    'Dim qqXML As New QuickQuote.CommonMethods.QuickQuoteXML
                            '    'Dim diaAdditionalInterestList As New Diamond.Common.Objects.Policy.AdditionalInterestList
                            '    'qqXML.DiamondService_LoadAdditionalInterestList(student.ListId, diaAdditionalInterestList)
                            '    student.Address.StreetName = "SomeNonsensical Place of awesomeness byte"
                            '    student.Name.CommercialName1 = "My New Awesome"
                            '    student.OverwriteAdditionalInterestListInfoForDiamondId = True
                            '    'qqXML.DiamondService_CreateOrUpdateAdditionalInterestList(student, diaAdditionalInterestList)
                            '    first = False
                            'End If
                        Next

                        'secICovs.Add(studentawaycov)
                        'ReSaveOrReRate(qID)
                    End If
                End If

            End If
        End If

    End Sub

    Private Sub Test11()
        GenericSetup(227640)

        If Quote IsNot Nothing Then
            If Quote.Operators IsNot Nothing Then
                'If Quote.Operators.Count > 0 Then
                '    Quote.Operators = New List(Of QuickQuote.CommonObjects.QuickQuoteOperator)
                '    Quote.CopyPolicyholdersToOperators()
                'End If
            End If

            If MyLocation IsNot Nothing Then
                If MyLocation.RvWatercrafts IsNot Nothing AndAlso MyLocation.RvWatercrafts.Count > 0 Then

                End If
            End If
        End If
    End Sub

    Private Sub Test10()
        GenericSetup()
        'RemoveBlankCoverages()
        'Dim myEffDate As String = Quote.EffectiveDate
        RemoveSpecificCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades)
        'Dim secIcovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
        'secIcovs = Quote.Locations(0).SectionICoverages
        'Dim myGreenUpgradeCov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
        'If secIcovs.Find(Function(x) x.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades) Is Nothing Then
        '    myGreenUpgradeCov.IncreasedCostOfLossId = 1
        '    myGreenUpgradeCov.IncreasedLimit = 5000
        '    myGreenUpgradeCov.TotalLimit = 5000
        '    myGreenUpgradeCov.VegetatedRoof = True
        '    myGreenUpgradeCov.RelatedExpenseLimit = 7000
        '    myGreenUpgradeCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
        '    myGreenUpgradeCov.Description = "Green Upgrades"
        '    secIcovs.Add(myGreenUpgradeCov)
        'Else
        '    myGreenUpgradeCov = secIcovs.Find(Function(x) x.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades)
        '    myGreenUpgradeCov.IncreasedCostOfLossId = 1
        '    myGreenUpgradeCov.IncreasedLimit = 5000
        '    myGreenUpgradeCov.TotalLimit = 5000
        '    myGreenUpgradeCov.VegetatedRoof = True
        '    myGreenUpgradeCov.RelatedExpenseLimit = 7000
        '    myGreenUpgradeCov.Description = "Green Upgrades"
        'End If
        ReSaveOrReRate(Quote.Database_QuoteId, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)

    End Sub

    Private Sub Test9()
        Dim sIandIIcov_AL As New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
        sIandIIcov_AL.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
        sIandIIcov_AL.LiabilityIncreasedLimit = "200,000"

        Dim sIandIIcov_LA As New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
        sIandIIcov_LA.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
        sIandIIcov_LA.PropertyIncreasedLimit = "4,000"

        If 1 = 1 Then

        End If
    End Sub

    Private Sub GetLocation()
        If Quote IsNot Nothing AndAlso Quote.Locations(0) IsNot Nothing Then
            QuoteLocation = Quote.Locations(0)
        End If
    End Sub

    Private Sub GenericSetup(Optional quoteID As String = "")
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "227640" '"227244" '"227286" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        If String.IsNullOrWhiteSpace(quoteID) = False Then
            currentQuoteId = quoteID
        End If

        If String.IsNullOrWhiteSpace(currentQuoteId) = False Then
            newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

            If String.IsNullOrWhiteSpace(errorMsg) Then
                If String.IsNullOrWhiteSpace(newQuoteId) Then
                    If String.IsNullOrWhiteSpace(quoteID) Then
                        quoteIdToUse = currentQuoteId
                    Else
                        quoteIdToUse = quoteID
                    End If
                Else
                    quoteIdToUse = newQuoteId
                End If
            End If
        End If
    End Sub

    Public Function GetLOBsThatRequireEffectiveDateAtQuoteStart() As List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
        Dim LOBs As New List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
        Dim LOBsStringList As New List(Of String)
        Dim LobString As String = chc.ConfigurationAppSettingValueAsString("LOBsThatRequireEffectiveDateAtQuoteStart")
        If String.IsNullOrWhiteSpace(LobString) = False Then
            If LobString.Contains(",") Then
                LOBsStringList = LobString.Split(",").ToList()
            Else
                LOBsStringList.Add(LobString)
            End If

            For Each LOBType As System.Enum In System.Enum.GetValues(GetType(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType))
                If LOBsStringList.Contains(LOBType.ToString()) Then
                    LOBs.Add(LOBType)
                End If
            Next
        End If

        Return LOBs
    End Function

    Private Sub Test8()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "227244" '"227286" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            If Quote IsNot Nothing Then
                Dim myLobId As String = Quote.LobId
                Dim myLobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = qqh.ConvertQQLobIdToQQLobType(myLobId)
                Dim myLOBVersionsTest As List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions) = qqh.GetIFMLOBVersionsListThatHaveUncrossableDateLine()
                Dim myLOBTest As New List(Of List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType))
                Dim myDictTest As New Dictionary(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions, String)
                'If qqh.HasIFMLOBVersionUncrossableDateLineBeforeEffectiveDate(Quote, "8/1/2018", myDictTest) Then
                '    If 1 = 1 Then

                '    End If
                'End If

                'If qqh.HasIFMLOBVersionUncrossableDateLineBeforeEffectiveDate(Quote, "7/1/2018", myDictTest) Then
                '    If 1 = 1 Then

                '    End If
                'End If

                'If qqh.HasIFMLOBVersionUncrossableDateLineBeforeEffectiveDate(Quote, "6/10/2018", myDictTest) Then
                '    If 1 = 1 Then

                '    End If
                'End If

                If qqh.HasIFMLOBVersionUncrossableDateLineBeforeEffectiveDateAndIsOutsideAcceptableEffectiveDateRange(Quote, "12/1/2018", myDictTest) Then
                    If 1 = 1 Then

                    End If
                End If

                For Each version As QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions In myLOBVersionsTest
                    myLOBTest.Add(qqh.GetIFMLOBVersionLOBTypeRelationsListByEnum(version))
                Next

                If myLOBTest IsNot Nothing AndAlso myLOBTest.Count > 0 Then
                    If 1 = 1 Then

                    End If
                End If
            End If
        End If
    End Sub

    Private Sub Test7()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "227244" '"227286" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            If Quote IsNot Nothing Then
                If GetListIfSomething(Quote.Locations) Is Nothing Then
                    Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                    Dim myLoc As New QuickQuote.CommonObjects.QuickQuoteLocation
                    CreateGenericLocation(myLoc)
                End If
                Dim secICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage) = Nothing
                Dim secIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = Nothing
                Dim secIandIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) = Nothing

                InitializeSectionCoveragesIfNeeded()

                secICovs = Quote.Locations(0).SectionICoverages
                secIICovs = Quote.Locations(0).SectionIICoverages
                secIandIICovs = Quote.Locations(0).SectionIAndIICoverages

                'GetSectionICoverage()

                'RemoveBlankCoverages()
            End If

            ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)

            If 1 = 1 Then

            End If

        End If
    End Sub

    Private Sub Test6()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "226182" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            Dim myFormTypeName As String = qqh.GetShortFormName(Quote)

            If Quote IsNot Nothing Then
                If GetListIfSomething(Quote.Locations) Is Nothing Then
                    Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
                    Dim myLoc As New QuickQuote.CommonObjects.QuickQuoteLocation
                    CreateGenericLocation(myLoc)
                End If
                Dim secICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage) = Nothing
                Dim secIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = Nothing
                Dim secIandIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) = Nothing

                InitializeSectionCoveragesIfNeeded()

                secICovs = Quote.Locations(0).SectionICoverages
                secIICovs = Quote.Locations(0).SectionIICoverages
                secIandIICovs = Quote.Locations(0).SectionIAndIICoverages

                RemoveAllSectionCoveragesOfType(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)()

                Dim myGreenUpgradeCov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
                If secICovs.Find(Function(x) x.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades) Is Nothing Then
                    myGreenUpgradeCov.IncreasedCostOfLossId = 1
                    myGreenUpgradeCov.IncreasedLimit = 5000
                    myGreenUpgradeCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
                    myGreenUpgradeCov.Description = "Green Upgrades"
                    secICovs.Add(myGreenUpgradeCov)
                Else
                    myGreenUpgradeCov = secICovs.Find(Function(x) x.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades)
                    myGreenUpgradeCov.IncreasedCostOfLossId = 1
                    myGreenUpgradeCov.IncreasedLimit = 5000
                    myGreenUpgradeCov.Description = "Green Upgrades"
                End If

                Dim myPersonalPropertyReplacementCostCov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
                If secICovs.Find(Function(x) x.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades) Is Nothing Then
                    myPersonalPropertyReplacementCostCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                    secICovs.Add(myPersonalPropertyReplacementCostCov)
                End If
            End If

            ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)

            If 1 = 1 Then

            End If

        End If
    End Sub

    Private Sub Test5()
        Using pno As New PolicyNumberObject("CUP1002534", chc.ConfigurationAppSettingValueAsString("conn"), chc.ConfigurationAppSettingValueAsString("connDiamond"))
            pno.UseDiamondData = True
            pno.GetAllInsuredInfo = True
            pno.GetPolicyInfo()

            If 1 = 1 Then
                If pno.InsuredInfo IsNot Nothing Then

                End If
            End If
        End Using
    End Sub

    Private Sub Test4()
        Dim ccID As String = qqh.GetRelatedStaticDataValueForOptionValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing.ToString(), QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuote.CommonMethods.QuickQuoteHelperClass.PersOrComm.Pers)
        Dim test2 As String = qqh.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccID)
        If 1 = 1 Then

        End If
    End Sub

    Private Sub Test3()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "226182" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            Dim myFormTypeName As String = qqh.GetShortFormName(Quote)

            If Quote IsNot Nothing Then
                If String.IsNullOrWhiteSpace(Quote.PersonalLiabilityLimitId) Then
                    Quote.PersonalLiabilityLimitId = "262" '100000
                End If
                If String.IsNullOrWhiteSpace(Quote.MedicalPaymentsLimitid) Then
                    Quote.MedicalPaymentsLimitid = "170" '1000
                End If

                'RemoveSpecificCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades)
                'RemoveSpecificCoverage(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement)

                If GetListIfSomething(Quote.Locations) IsNot Nothing Then
                    Dim secICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage) = Nothing
                    Dim secIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = Nothing
                    Dim secIandIICovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) = Nothing

                    If Quote.Locations(0).SectionICoverages Is Nothing Then
                        Quote.Locations(0).SectionICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
                    End If

                    If Quote.Locations(0).SectionIICoverages Is Nothing Then
                        Quote.Locations(0).SectionIICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
                    End If

                    If Quote.Locations(0).SectionIAndIICoverages Is Nothing Then
                        Quote.Locations(0).SectionIAndIICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
                    End If

                    secICovs = Quote.Locations(0).SectionICoverages
                    secIICovs = Quote.Locations(0).SectionIICoverages
                    secIandIICovs = Quote.Locations(0).SectionIAndIICoverages

                    Dim relativeCov As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage = secIandIICovs.Find(Function(x) x.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence)
                    If relativeCov IsNot Nothing Then
                        If relativeCov.AdditionalInterests IsNot Nothing Then
                            relativeCov.AdditionalInterests(0).TypeId = ""
                            If 1 = 1 Then

                            End If
                            relativeCov.AdditionalInterests(0).TypeName = QuickQuote.CommonObjects.QuickQuoteAdditionalInterest.AdditionalInterestType.Relative
                            If 1 = 1 Then

                            End If
                        End If
                    End If

                    'Dim ai As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
                    'ai.TypeName = QuickQuote.CommonObjects.QuickQuoteAdditionalInterest.AdditionalInterestType.Relative
                    'ai.Description = "DescriptionForAI"

                    'Dim mycov As New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
                    'mycov.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                    'mycov.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                    'mycov.AdditionalInterests.Add(ai)
                    'mycov.EventEffDate = "7/1/2018"
                    'mycov.EventExpDate = "7/1/2019"

                    'Quote.Locations(0).SectionIAndIICoverages.Add(mycov)

                    'Dim myCov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
                    'myCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
                    'myCov.IncreasedLimit = "5000"
                    'SetCoverageTotalLimit(myCov)
                    'myCov.RelatedExpenseLimit = "15000"
                    'myCov.IncreasedCostofLossId = "1"
                    'Quote.Locations(0).SectionICoverages.Add(myCov)

                    'myCov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
                    'myCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                    'Quote.Locations(0).SectionICoverages.Add(myCov)
                End If
            End If

            'ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Save, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
            ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Save, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)

            newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

            If 1 = 1 Then

            End If

        End If
    End Sub

    Private Sub Test2()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "227197" ' "227199" '"227198" '"225900" '		QuoteNumber	"QHOM360964"	String
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        Dim covCLimit As String = ""
        Dim covCLimitIncreased As String = ""
        Dim covCLimitIncluded As String = ""

        If GetListIfSomething(Quote.Locations) IsNot Nothing Then
            covCLimit = Quote.Locations(0).C_PersonalProperty_Limit
            covCLimitIncreased = Quote.Locations(0).C_PersonalProperty_LimitIncreased
            covCLimitIncluded = Quote.Locations(0).C_PersonalProperty_LimitIncluded
        End If


        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            Quote.Locations(0).ProgramTypeId = 1

            Quote.Locations(0).A_Dwelling_Limit = "100000"
            Quote.Locations(0).A_Dwelling_LimitIncluded = "100000"

            Quote.Locations(0).B_OtherStructures_LimitIncluded = "10000"
            Quote.Locations(0).B_OtherStructures_Limit = "10000"

            Quote.Locations(0).C_PersonalProperty_Limit = "70000"
            Quote.Locations(0).C_PersonalProperty_LimitIncluded = "70000"

            ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)

        End If

    End Sub

    Private Sub Test1()
        Dim errorMsg As String = ""
        Dim currentQuoteId As String = "226182" '"226158"
        Dim newQuoteId As String = ""
        Dim copyQuote As Boolean = False
        Dim quoteIdToUse As String = ""
        Dim valCount As Integer = 0
        Dim doRerate As Boolean = False
        Login()

        newQuoteId = CopyAndOrLoadQuote(currentQuoteId, copyQuote)

        If String.IsNullOrWhiteSpace(errorMsg) Then
            If String.IsNullOrWhiteSpace(newQuoteId) Then
                quoteIdToUse = currentQuoteId
            Else
                quoteIdToUse = newQuoteId
            End If

            UpdateQuoteEffectiveDate()

            If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(errorMsg) Then
                GetLocationCoverages()
                RemoveBlankCoverages()
                ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
                If String.IsNullOrWhiteSpace(errorMsg) Then
                    GetLocationCoverages()
                    ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
                    If String.IsNullOrWhiteSpace(errorMsg) Then
                        GetLocationCoverages()
                        'RemoveBlankCoverages()
                        If SectionCoveragesRemovedCount > 0 Then
                            SectionCoveragesRemovedCount = 0
                            GetLocationCoverages()
                            ReSaveOrReRate(quoteIdToUse, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
                            If String.IsNullOrWhiteSpace(errorMsg) Then
                                GetLocationCoverages()
                            End If
                        Else
                            'No Blank coverages found, Yay!
                        End If
                    Else
                        If errorMsg Then
                            'ERRORS FOUND
                        End If
                    End If

                Else
                    If errorMsg Then
                        'Errors found!
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub InitializeSectionCoveragesIfNeeded()
        If GetListIfSomething(Quote.Locations) IsNot Nothing Then
            If Quote.Locations(0).SectionICoverages Is Nothing Then
                Quote.Locations(0).SectionICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
            End If

            If Quote.Locations(0).SectionIICoverages Is Nothing Then
                Quote.Locations(0).SectionIICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
            End If

            If Quote.Locations(0).SectionIAndIICoverages Is Nothing Then
                Quote.Locations(0).SectionIAndIICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
            End If
        End If
    End Sub

    Private Function GetSectionICoverage(CoverageType As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType) As QuickQuote.CommonObjects.QuickQuoteSectionICoverage
        Dim myCov As QuickQuote.CommonObjects.QuickQuoteSectionICoverage = Nothing
        If CoverageType <> QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType.None Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionICoverages) IsNot Nothing Then
                myCov = Quote.Locations(0).SectionICoverages.Find(Function(x) x.CoverageType = CoverageType)
                If myCov Is Nothing Then
                    myCov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
                    myCov.CoverageType = CoverageType
                    Quote.Locations(0).SectionICoverages.Add(myCov)
                End If
            End If
        End If
        Return myCov
    End Function

    Private Function GetSectionIICoverage(CoverageType As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType) As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
        Dim myCov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        If CoverageType <> QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.None Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionIICoverages) IsNot Nothing Then
                myCov = Quote.Locations(0).SectionIICoverages.Find(Function(x) x.CoverageType = CoverageType)
                If myCov Is Nothing Then
                    myCov = New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
                    myCov.CoverageType = CoverageType
                    Quote.Locations(0).SectionIICoverages.Add(myCov)
                End If
            End If
        End If
        Return myCov
    End Function

    Private Function GetSectionIandIICoverage(CoverageType As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
        Dim myCov As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage = Nothing
        If CoverageType <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionIAndIICoverages) IsNot Nothing Then
                myCov = Quote.Locations(0).SectionIAndIICoverages.Find(Function(x) x.MainCoverageType = CoverageType)
                If myCov Is Nothing Then
                    myCov = New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
                    myCov.MainCoverageType = CoverageType
                    Quote.Locations(0).SectionIAndIICoverages.Add(myCov)
                End If
            End If
        End If
        Return myCov
    End Function

    Private Sub CreateGenericLocation(myLoc As QuickQuote.CommonObjects.QuickQuoteLocation)
        If myLoc Is Nothing Then
            myLoc = New QuickQuote.CommonObjects.QuickQuoteLocation
        End If

        'myLoc.
    End Sub

    Private Sub ReSaveOrReRate(ByVal qId As String, Optional ByVal ReloadForquoteId As Boolean = False, Optional ByVal saveOrRate As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, Optional ByVal saveOrRateType As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML
        Dim err As String = ""

        Dim strQQ As String = ""
        Dim ratedQQ As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim strRatedQQ As String = ""

        If ReloadForquoteId = True Then
            Quote = Nothing

            If qqh.IsNumericString(qId) = True Then
                'get existing
                qqxml.GetQuoteForSaveType(qId, saveOrRateType, Quote, err)
            End If
        End If

        If Quote IsNot Nothing Then
            If saveOrRate = QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Save Then
                qqxml.SaveQuote(saveOrRateType, Quote, qId, err)
            Else
                qqxml.RateQuoteAndSave(saveOrRateType, Quote, strQQ, ratedQQ, strRatedQQ, qId, err) 'debug method w/ byref params for rated QuickQuoteObject and xml strings for the request and response
            End If
        End If

        If String.IsNullOrWhiteSpace(err) = False Then
            If 1 = 1 Then 'break here to check error message

            End If
        Else
            If 1 = 1 Then 'success!

            End If
        End If

    End Sub

    Private Sub SetCoverageTotalLimit(ByRef thisCov As Object)
        thisCov.TotalLimit = "0"

        If qqh.IsNumericString(thisCov.IncreasedLimit) Then
            thisCov.TotalLimit = (CDec(thisCov.TotalLimit) + CDec(thisCov.IncreasedLimit)).ToString()
        End If

        If qqh.IsNumericString(thisCov.IncludedLimit) Then
            thisCov.TotalLimit = (CDec(thisCov.TotalLimit) + CDec(thisCov.IncludedLimit)).ToString()
        End If

        If thisCov.TotalLimit = "0" Then
            thisCov.TotalLimit = ""
        End If
    End Sub

    Private Sub RemoveBlankCoverages()
        SectionCoveragesRemovedCount = 0
        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations)(0) IsNot Nothing Then
            Dim MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation = GetListIfSomething(Quote.Locations)(0)
            RemoveBlankDiamondSectionCoveragesObject(MyLocation)
            RemoveBlankQQSectionCoverages(MyLocation.SectionICoverages)
            RemoveBlankQQSectionCoverages(MyLocation.SectionIICoverages)
            RemoveBlankQQSectionCoverages(MyLocation.SectionIAndIICoverages)
        End If
    End Sub

    Private Sub RemoveAllSectionCoveragesOfType(Of T)()
        Dim exposureType As QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum

        If GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionICoverage) Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionICoverages) IsNot Nothing Then
                Quote.Locations(0).SectionICoverages.Clear()
            End If
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_I_Coverages
        ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionIICoverages) IsNot Nothing Then
                Quote.Locations(0).SectionIICoverages.Clear()
            End If
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_II_Coverages
        ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) Then
            If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionIAndIICoverages) IsNot Nothing Then
                Quote.Locations(0).SectionIAndIICoverages.Clear()
            End If
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_I_And_II_Coverages
        End If

        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing AndAlso GetListIfSomething(Quote.Locations(0).SectionCoverages) IsNot Nothing Then
            Quote.Locations(0).SectionCoverages.RemoveAll(Function(x) x.CoverageExposureType = exposureType)
        End If

    End Sub

    Private Sub RemoveSpecificCoverage(Of T)(sectionCoverageType As Object)
        Dim exposureType As QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum
        Dim myCov As Object = Nothing
        Dim covsToRemove = Nothing

        If GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionICoverage) Then
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_I_Coverages
            myCov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
            myCov.HOM_CoverageType = sectionCoverageType
            covsToRemove = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
            RemoveSpecificCoverageFromQQSectionICoverages(myCov, covsToRemove)
        ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) Then
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_II_Coverages
            myCov = New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
            myCov.HOM_CoverageType = sectionCoverageType
            covsToRemove = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
            RemoveSpecificCoverageFromQQSectionIICoverages(myCov, covsToRemove)
        ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) Then
            exposureType = QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum.Section_I_And_II_Coverages
            myCov = New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
            covsToRemove = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
            RemoveSpecificCoverageFromQQSectionIandIICoverages(myCov, covsToRemove)
        End If

        covsToRemove = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionCoverage)

        RemoveSpecificCoverageFromSectionCoverages(exposureType, myCov, covsToRemove)
    End Sub

    Private Sub RemoveSpecificCoverageFromSectionCoverages(exposureType As QuickQuote.CommonObjects.QuickQuoteSectionCoverage.CoverageExposureTypeEnum, myCov As Object, covsToRemove As List(Of QuickQuote.CommonObjects.QuickQuoteSectionCoverage))
        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing Then
            If GetListIfSomething(Quote.Locations(0).SectionCoverages) IsNot Nothing Then
                For Each cov As QuickQuote.CommonObjects.QuickQuoteSectionCoverage In Quote.Locations(0).SectionCoverages
                    If cov.CoverageExposureType = exposureType Then
                        If GetListIfSomething(cov.Coverages) IsNot Nothing Then
                            If cov.Coverages(0).CoverageCodeId = myCov.CoverageCodeId Then
                                covsToRemove.Add(cov)
                            End If
                        End If
                    End If
                Next
                RemoveItemsFromListUsingList(Of QuickQuote.CommonObjects.QuickQuoteSectionCoverage)(covsToRemove, Quote.Locations(0).SectionCoverages)
            End If
        End If
    End Sub

    Private Sub RemoveSpecificCoverageFromQQSectionICoverages(myCov As Object, covsToRemove As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage))
        Dim mySectionICoverage As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
        mySectionICoverage = DirectCast(myCov, QuickQuote.CommonObjects.QuickQuoteSectionICoverage)

        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing Then
            If GetListIfSomething(Quote.Locations(0).SectionICoverages) IsNot Nothing Then
                For Each cov As QuickQuote.CommonObjects.QuickQuoteSectionICoverage In Quote.Locations(0).SectionICoverages
                    If cov.HOM_CoverageType = mySectionICoverage.HOM_CoverageType Then
                        covsToRemove.Add(cov)
                    End If
                Next
                RemoveItemsFromListUsingList(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(covsToRemove, Quote.Locations(0).SectionICoverages)
            End If
        End If
    End Sub

    Private Sub RemoveSpecificCoverageFromQQSectionIICoverages(myCov As Object, covsToRemove As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage))
        Dim mySectionIICoverage As New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
        mySectionIICoverage = DirectCast(myCov, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)

        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing Then
            If GetListIfSomething(Quote.Locations(0).SectionIICoverages) IsNot Nothing Then
                For Each cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In Quote.Locations(0).SectionIICoverages
                    If cov.HOM_CoverageType = mySectionIICoverage.HOM_CoverageType Then
                        covsToRemove.Add(cov)
                    End If
                Next
                RemoveItemsFromListUsingList(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)(covsToRemove, Quote.Locations(0).SectionIICoverages)
            End If
        End If
    End Sub

    Private Sub RemoveSpecificCoverageFromQQSectionIandIICoverages(myCov As Object, covsToRemove As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage))
        Dim mySectionIandIICoverage As New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage
        mySectionIandIICoverage = DirectCast(myCov, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)

        If Quote IsNot Nothing AndAlso GetListIfSomething(Quote.Locations) IsNot Nothing Then
            If GetListIfSomething(Quote.Locations(0).SectionIAndIICoverages) IsNot Nothing Then
                For Each cov As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage In Quote.Locations(0).SectionIAndIICoverages
                    If cov.MainCoverageType = mySectionIandIICoverage.MainCoverageType Then
                        covsToRemove.Add(cov)
                    End If
                Next
                RemoveItemsFromListUsingList(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)(covsToRemove, Quote.Locations(0).SectionIAndIICoverages)
            End If
        End If
    End Sub

    Private Sub RemoveItemsFromListUsingList(Of T)(ListOfItemsToRemove As List(Of T), ListToRemoveItemsFrom As List(Of T))
        If GetListIfSomething(ListOfItemsToRemove) IsNot Nothing AndAlso GetListIfSomething(ListToRemoveItemsFrom) IsNot Nothing Then
            For Each item As T In ListOfItemsToRemove
                If ListToRemoveItemsFrom.Contains(item) Then
                    ListToRemoveItemsFrom.Remove(item)
                End If
            Next
        End If
    End Sub

    Private Sub RemoveBlankQQSectionCoverages(Of T)(MySectionCoverageList As List(Of T))
        If MySectionCoverageList IsNot Nothing AndAlso MySectionCoverageList.Count > 0 Then
            Dim covsToRemove As New List(Of Object)
            For Each cov As Object In MySectionCoverageList
                If GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionICoverage) Then
                    If String.IsNullOrWhiteSpace(cov.CoverageCodeId) OrElse cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.SectionICoverageType.None Then
                        covsToRemove.Add(cov)
                    End If
                ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) Then
                    If String.IsNullOrWhiteSpace(cov.CoverageCodeId) OrElse cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.None Then
                        covsToRemove.Add(cov)
                    End If
                ElseIf GetType(T) Is GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) Then
                    If String.IsNullOrWhiteSpace(cov.MainCoverageCodeId) OrElse cov.MainCoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None Then
                        covsToRemove.Add(cov)
                    End If
                End If
            Next
            If covsToRemove.Count > 0 Then
                For Each cov As Object In covsToRemove
                    If MySectionCoverageList.Contains(cov) Then
                        MySectionCoverageList.Remove(cov)
                        SectionCoveragesQQRemovedCount += 1
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub RemoveBlankDiamondSectionCoveragesObject(MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation)
        If GetListIfSomething(MyLocation.SectionCoverages) IsNot Nothing Then
            Dim sectionCovsToRemove As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionCoverage)
            For Each SectionCov As QuickQuote.CommonObjects.QuickQuoteSectionCoverage In MyLocation.SectionCoverages
                If GetListIfSomething(SectionCov.Coverages) IsNot Nothing Then
                    Dim covsToRemove As New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)
                    For Each cov As QuickQuote.CommonObjects.QuickQuoteCoverage In SectionCov.Coverages
                        If String.IsNullOrWhiteSpace(cov.CoverageCodeId) Then
                            covsToRemove.Add(cov)
                        End If
                    Next
                    If covsToRemove.Count > 0 Then
                        For Each cov As QuickQuote.CommonObjects.QuickQuoteCoverage In covsToRemove
                            If SectionCov.Coverages.Contains(cov) Then
                                SectionCov.Coverages.Remove(cov)
                            End If
                        Next
                        If SectionCov.Coverages.Count = 0 Then
                            SectionCov.Coverages = Nothing
                        End If
                    End If
                End If
                If SectionCov.Coverages Is Nothing Then
                    sectionCovsToRemove.Add(SectionCov)
                End If
            Next
            If sectionCovsToRemove.Count > 0 Then
                For Each cov As QuickQuote.CommonObjects.QuickQuoteSectionCoverage In sectionCovsToRemove
                    If MyLocation.SectionCoverages.Contains(cov) Then
                        MyLocation.SectionCoverages.Remove(cov)
                        SectionCoveragesRemovedCount += 1
                    End If
                Next
                If MyLocation.SectionCoverages.Count = 0 Then
                    MyLocation.SectionCoverages = Nothing
                End If
            End If
        End If
    End Sub


    Private Sub GetLocationCoverages()
        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
            Dim myLocation As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(0)
            Dim myLocationCoverages As List(Of QuickQuote.CommonObjects.QuickQuoteCoverage) = GetListIfSomething(myLocation.LocationCoverages)
            Dim mySectionICoverages As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage) = GetListIfSomething(myLocation.SectionICoverages)
            Dim mySectionIICoverages As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = GetListIfSomething(myLocation.SectionIICoverages)
            Dim mySectionIandIICoverages As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage) = GetListIfSomething(myLocation.SectionIAndIICoverages)
            Dim mySectionCoverages As List(Of QuickQuote.CommonObjects.QuickQuoteSectionCoverage) = GetListIfSomething(myLocation.SectionCoverages)
            SectionCoveragesCount = mySectionCoverages.Count
        End If
    End Sub

    Private Function GetListIfSomething(Of T)(myList As List(Of T)) As List(Of T)
        If myList IsNot Nothing AndAlso myList.Count > 0 Then
            Return myList
        Else
            Return Nothing
        End If
    End Function

    Private Function CopyAndOrLoadQuote(QuoteId As String, Optional doCopyQuote As Boolean = False) As String
        Dim returnVar As String = ""

        If String.IsNullOrWhiteSpace(QuoteId) = False Then
            If doCopyQuote = True Then
                Dim newQuoteId As String = ""
                qqxml.CopyQuote(QuoteId, newQuoteId, errorMsg)

                If String.IsNullOrWhiteSpace(errorMsg) Then
                    qqxml.GetQuote(newQuoteId, Quote, errorMsg)

                    If String.IsNullOrWhiteSpace(newQuoteId) = False Then
                        returnVar = newQuoteId
                    End If
                End If
            Else
                qqxml.GetQuoteForSaveType(QuoteId, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, errorMsg, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrRatedXml.AlwaysUseRatedWhenAvailable, QuickQuote.CommonMethods.QuickQuoteXML.QuoteOrAppXml.UseDefault)
                'qqxml.GetQuote(QuoteId, Quote, errorMsg)
            End If
        End If

        Return returnVar
    End Function

    Private Function LoadEndorsement(policyId As Integer, policyImageNum As Integer, errorMsg As String) As String
        Dim returnvar As String = ""

        Quote = qqxml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, policyImageNum, errorMsg)

        If String.IsNullOrWhiteSpace(errorMsg) = False Then
            If 1 = 1 Then

            End If
        End If

        Return returnvar
    End Function

    Private Sub SaveEndorsement()
        Dim qqResult As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        qqxml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(Quote, qqResult, True, errorMessage)

        If String.IsNullOrWhiteSpace(errorMessage) = False Then
            If 1 = 1 Then

            End If
        End If
    End Sub

    Private Sub UpdateQuoteEffectiveDate()
        If Quote IsNot Nothing AndAlso CDate(Quote.EffectiveDate) < CDate("7/1/2018") Then
            Quote.EffectiveDate = "7/2/2018"
            qqxml.RateQuoteAndSave(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, "", errorMsg)
            ReSaveOrReRate(Quote.Database_QuoteId, saveOrRate:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteValidationType.Rate, saveOrRateType:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote)
        End If
    End Sub

    Private Sub Login()
        'Using ratingService As New CRS.ComparativeRatingService
        '    'removed hard-coded test values 7/25/2012

        '    '<add key = "QuickQuoteTestUsername" value="dagug"/>
        '    '<add key = "QuickQuoteTestPassword" value="Problem342"/>
        '    '<add key = "QuickQuoteTestUserId" value="272"/>
        '    '<add key = "QuickQuoteTestAgencyCode" value="6013-1840"/>
        '    '<add key = "QuickQuoteTestAgencyId" value="17"/>

        '    Dim originator As String = "nxtech" '"web"'looks like nxtech might be required here
        '    Dim loginName As String = ""
        '    Dim loginPassword As String = ""
        '    If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" Then
        '        loginName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
        '    ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
        '        loginName = ConfigurationManager.AppSettings("QuickQuoteTestUsername").ToString
        '    End If
        '    If System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
        '        loginPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
        '    ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
        '        loginPassword = ConfigurationManager.AppSettings("QuickQuoteTestPassword").ToString
        '    End If

        '    Dim strToken As String = ""
        '    'If Diamond.Web.BaseControls.SignedOnUser IsNot Nothing AndAlso Diamond.Web.BaseControls.SignedOnUser.LoginName <> "" AndAlso Diamond.Web.BaseControls.SignedOnUser.Password <> "" Then
        '    '    loginName = Diamond.Web.BaseControls.SignedOnUser.LoginName
        '    '    loginPassword = Diamond.Web.BaseControls.SignedOnUser.Password
        '    'End If
        '    'updated 8/16/2012 to store token in session
        '    If System.Web.HttpContext.Current.Session("DiamondQuickQuoteToken") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondQuickQuoteToken").ToString <> "" Then
        '        strToken = System.Web.HttpContext.Current.Session("DiamondQuickQuoteToken").ToString
        '        'Else
        '        'updated 10/8/2012 to make sure loginName and loginPassword were present
        '    ElseIf loginName <> "" AndAlso loginPassword <> "" Then
        '        strToken = ratingService.GetAuthenticationToken(originator, loginName, loginPassword)

        '        'added 10/8/2012 just in case 1st call to service fails
        '        If strToken Is Nothing OrElse strToken = "" Then
        '            strToken = ratingService.GetAuthenticationToken(originator, loginName, loginPassword)
        '        End If
        '    End If

        '    'If strToken IsNot Nothing AndAlso strToken <> "" Then
        '    '    System.Web.HttpContext.Current.Session("DiamondQuickQuoteToken") = strToken
        '    '    Dim wf As CRS.WorkflowInformation = ratingService.GetAgencyWorkflowInformationEncrypted(originator, strToken, True, True, Quote.AgencyCode)
        '    '    If wf IsNot Nothing AndAlso IsNumeric(wf.WorkflowQueueId) = True Then
        '    '        Quote.WorkflowQueueId = wf.WorkflowQueueId
        '    '        'Session("AgencyID") = wf.AgencyId
        '    '    End If
        '    'End If
        'End Using

        QuickQuote.CommonMethods.QuickQuoteHelperClass.CheckDiamondServicesToken()
        If Diamond.Web.BaseControls.SignedOnUserID <= 0 Then
            If 1 = 1 Then

            End If
        End If
    End Sub
    Private Sub CheckQuickQuote3()

    End Sub

    Private Sub LoadTestQuickQuote4()
        Quote.EffectiveDate = "7/1/2018"
        Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal

        Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Quote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)

        Dim MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(0)
        MyLocation.FormTypeId = 22
        MyLocation.OccupancyCodeId = 1
        MyLocation.YearBuilt = 2015
        MyLocation.SquareFeet = 3000
        MyLocation.StructureTypeId = 13
        MyLocation.ConstructionTypeId = 1
        MyLocation.PrimaryResidence = True
        MyLocation.ProtectionClassId = 3
        MyLocation.FeetToFireHydrant = 4
        MyLocation.FireDistrictName = "AVON"
        MyLocation.DeductibleId = ""

        MyLocation.SectionICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
        MyLocation.SectionICoverages.Add(New QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
        MyLocation.SectionICoverages(0).HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises
    End Sub

    Private Sub LoadTestQuickQuote5()
        Quote.EffectiveDate = "7/1/2018"
        Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
        Dim loc As New QuickQuote.CommonObjects.QuickQuoteLocation
        'loc.StructureTypeId = 2
        loc.FormTypeId = 25
        Quote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Quote.Locations.Add(loc)

        Dim mycova As New QuickQuote.CommonObjects.QuickQuoteCoverage
        Dim myListOfCov As New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)

        mycova.LimitTypeId = 1
        mycova.Description = "yup"

        myListOfCov.Add(mycova)

        Quote.Locations(0).IncidentalDwellingCoverages = myListOfCov

        mycova = New QuickQuote.CommonObjects.QuickQuoteCoverage
        mycova.LimitTypeId = 2
        mycova.Description = "number2"
        Quote.Locations(0).IncidentalDwellingCoverages.Add(mycova)

        mycova = New QuickQuote.CommonObjects.QuickQuoteCoverage
        mycova.LimitTypeId = 3
        mycova.Description = "number3"
        Quote.Locations(0).IncidentalDwellingCoverages.Add(mycova)

        mycova = New QuickQuote.CommonObjects.QuickQuoteCoverage
        mycova.LimitTypeId = 4
        mycova.Description = "number4"
        Quote.Locations(0).IncidentalDwellingCoverages.Add(mycova)

        mycova = Quote.Locations(0).IncidentalDwellingCoverages(2)

        Quote.Locations(0).IncidentalDwellingCoverages.Remove(mycova)

        mycova = New QuickQuote.CommonObjects.QuickQuoteCoverage
        mycova.LimitTypeId = 5
        mycova.Description = "number5"
        Quote.Locations(0).IncidentalDwellingCoverages.Add(mycova)

        Quote.Locations(0).StructureTypeId = 2
        Quote.Locations(0).FormTypeId = 25
        Dim myList As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
        'Quote.Locations(0).SectionICoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)

        Dim myCov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage
        myCov.HOM_CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises
        myList.Add(myCov)

        Quote.Locations(0).SectionICoverages = myList

        Dim myString As String = Quote.Locations(0).SectionICoverages(0).GetQuoteObject.EffectiveDate
    End Sub

    Private Sub LoadTestQuickQuote7()
        'Load this id quoteid=158982
        Dim errorMsg As String = ""
        qqxml.GetQuote("158982", Quote, errorMsg)

        If Quote IsNot Nothing Then
            Dim credit As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing
            Dim results As String = ""
            errorMsg = ""
            qqxml.LoadCreditForQuote(Quote, credit, results, errorMsg)
        End If

    End Sub

    Private Sub LoadTestQuickQuote6()
        Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability

        Quote.Client.ClientId = "216844"

        'QuickQuoteObj.QuoteNumber = "Test #"
        Quote.QuoteDescription = "Test Desc"
        Quote.EffectiveDate = "7/1/2012"

        'testing producer stuff 7/19/2012 for App Gap (not working)
        Quote.AgencyProducerId = "6946" 'for only producer tied to 3000 code (agency_id 441)
        Quote.AgencyProducerCode = "00" 'for only producer tied to 3000 code (agency_id 441)

        Quote.ProgramTypeId = "54" 'CGL - Commercial General Liability - Standard
        Quote.OccurrenceLiabilityLimitId = "34" '500,000
        Quote.GeneralAggregateLimitId = "178" '600,000
        Quote.ProductsCompletedOperationsAggregateLimitId = "185" '1,500,000
        Quote.PersonalAndAdvertisingInjuryLimitId = "34" '500,000
        Quote.DamageToPremisesRentedLimitId = "10" '100,000
        Quote.MedicalExpensesLimitId = "15" '5,000

        Quote.HasBusinessMasterEnhancement = True

        Quote.AdditionalInsuredsManualCharge = "85.00" 'not setting prem in Diamond; okay now as-of 7/12/2012

        Quote.EmployeeBenefitsLiabilityText = "5" 'number of employees
        Quote.EmployeeBenefitsLiabilityOccurrenceLimitId = "34" '500,000
        Quote.HasHiredAuto = True
        Quote.HasNonOwnedAuto = True

        'liquor stuff isn't working as-of 7/13/2012 (Error in CalculateLiquorLiabilityPremium: Object reference not set to an instance of an object.)
        'fixed 7/18/2012
        Quote.LiquorLiabilityOccurrenceLimitId = "56" '1,000,000
        'QuickQuoteObj.LiquorLiabilityClassificationId = "58161" 'Restaurants or Hotels
        Quote.LiquorLiabilityClassificationId = "50911" 'Manufacturer, Wholesalers & Distributors
        Quote.LiquorSales = "30000.00"

        Quote.ProfessionalLiabilityCemetaryNumberOfBurials = "100"
        Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies = "200"
        Quote.ProfessionalLiabilityPastoralNumberOfClergy = "5"

        Quote.GLClassifications = New Generic.List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification) 'not getting rated prem in Diamond
        Dim gl As New QuickQuote.CommonObjects.QuickQuoteGLClassification
        gl.ClassCode = "50017"
        gl.ClassDescription = "Abrasives or Abrasive Products Mfg."
        gl.PremiumExposure = "10,000"
        gl.PremiumBase = "Gross Sales"
        gl.PremiumBaseShort = "s" 'added 11/26/2012 for testing (needed for dec)
        Quote.GLClassifications.Add(gl)
        Dim gl2 As New QuickQuote.CommonObjects.QuickQuoteGLClassification
        gl2.ClassCode = "51741"
        gl2.ClassDescription = "Candle Mfg"
        gl2.PremiumExposure = "20,000"
        gl2.PremiumBase = "Gross Sales"
        gl2.PremiumBaseShort = "s" 'added 11/26/2012 for testing (needed for dec)
        Quote.GLClassifications.Add(gl2)

        Quote.Client.Address.City = "Indianapolis"
        Quote.Client.Address.County = "Marion"
        Quote.Client.Address.HouseNum = "123"
        Quote.Client.Address.StreetName = "Test Street"
        Quote.Client.Address.Zip = "46227-0000"

        Quote.Client.Name.FirstName = "Don"
        Quote.Client.Name.LastName = "Test"
        Quote.Client.Name.SexId = "1"
        Quote.Client.Name.TypeId = "1" 'Personal

        Quote.Client.Phones = New Generic.List(Of QuickQuote.CommonObjects.QuickQuotePhone)
        Dim cp1 As New QuickQuote.CommonObjects.QuickQuotePhone
        cp1.Number = "(317)111-2222"
        Quote.Client.Phones.Add(cp1)

        Quote.Policyholder.Address.City = "Indianapolis"
        Quote.Policyholder.Address.County = "Marion"
        Quote.Policyholder.Address.HouseNum = "123"
        Quote.Policyholder.Address.StreetName = "Test Street"
        Quote.Policyholder.Address.Zip = "46227-0000"

        Quote.Policyholder.Name.BirthDate = "1/1/1982"
        Quote.Policyholder.Name.FirstName = "Don"
        Quote.Policyholder.Name.LastName = "Test"
        Quote.Policyholder.Name.SexId = "1"
        Quote.Policyholder.Name.TypeId = "1" 'Personal

        Quote.Policyholder.Phones = New Generic.List(Of QuickQuote.CommonObjects.QuickQuotePhone)
        Dim pp1 As New QuickQuote.CommonObjects.QuickQuotePhone
        pp1.Number = "(317)111-2222"
        Quote.Policyholder.Phones.Add(pp1)

        Quote.Locations = New Generic.List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Dim l1 As New QuickQuote.CommonObjects.QuickQuoteLocation
        l1.Description = "Test Loc 1"

        l1.Address.City = "Indianapolis"
        l1.Address.County = "Marion"
        l1.Address.HouseNum = "123"
        l1.Address.StreetName = "Test Street"
        l1.Address.Zip = "46227-0000"

        l1.GLClassifications = New Generic.List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification) 'not getting rated prem in Diamond
        Dim l1gl As New QuickQuote.CommonObjects.QuickQuoteGLClassification
        l1gl.ClassCode = "96611"
        l1gl.ClassDescription = "Interior Decorators"
        l1gl.PremiumExposure = "5,000"
        l1gl.PremiumBase = "Payroll"
        l1gl.PremiumBaseShort = "p" 'added 11/26/2012 for testing (needed for dec)
        l1.GLClassifications.Add(l1gl)
        Dim l1gl2 As New QuickQuote.CommonObjects.QuickQuoteGLClassification
        l1gl2.ClassCode = "57997"
        l1gl2.ClassDescription = "Photo Finishing Labs"
        l1gl2.PremiumExposure = "30,000"
        l1gl2.PremiumBase = "Gross Sales, Products/Complete"
        l1gl2.PremiumBaseShort = "s" 'added 11/26/2012 for testing (needed for dec)
        l1.GLClassifications.Add(l1gl2)

        Quote.Locations.Add(l1)

        'testing 8/1/2012
        Dim quoteId As String = ""
        Dim errorMsg As String = ""

        qqxml.RateQuoteAndSave(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, Quote, quoteId, errorMsg)

        If errorMsg = "" Then

        End If

        'LoadValuesFromQuickQuote()

        'Dim xmlDoc As XmlDocument
        'QQxml.BuildXml(quickQuote, xmlDoc)
        ''QQxml.OldBuildXml(quickQuote, xmlDoc)
        'If xmlDoc IsNot Nothing Then
        '    xmlDoc.Save("C:\Users\domin\Documents\QuickQuoteTests\Test2\GLTest8_GLClasses5WithPremBaseAndOtherCovsAndProducer2.xml")
        'End If

    End Sub

    Private Sub ChangeAgency(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal newAgencyCode As String, ByVal newAgencyId As String)
        If newAgencyCode <> "" AndAlso newAgencyId <> "" AndAlso qqo IsNot Nothing Then
            Dim isDifferentAgency As Boolean = False
            If newAgencyCode <> qqo.AgencyCode OrElse newAgencyId <> qqo.AgencyId Then
                isDifferentAgency = True
            End If

            qqo.AgencyCode = newAgencyCode
            qqo.AgencyId = newAgencyId

            'updated 10/2/2014
            If isDifferentAgency = True Then
                qqo.AgencyProducerCode = ""
                qqo.AgencyProducerId = ""
                If qqo.Client IsNot Nothing Then
                    qqo.Client.ClientId = ""
                End If
            End If
        Else
            errorMsg = "agency code and agency id are required"
        End If
    End Sub

    Private Sub CopyQuoteAndChangeState(quoteIdToCopy As String, removeUnderwritingQuestions As Boolean, ByRef copyErrMessage As String, newState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, Optional oldState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None, Optional CopyInformationFromOldStateToNewState As Boolean = True)
        CopyQuote(quoteIdToCopy, removeUnderwritingQuestions, copyErrMessage)
        If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(copyErrMessage) = True Then
            ChangeQuoteState(Quote, newState, oldState, CopyInformationFromOldStateToNewState)
        End If
    End Sub

    Private Sub CopyQuote(quoteIdToCopy As String, removeUnderwritingQuestions As Boolean, ByRef errMessage As String)
        If String.IsNullOrWhiteSpace(quoteIdToCopy) = False Then
            Quote = qqxml.CopyQuoteForTesting(quoteIdToCopy, removeUnderwritingQuestions, errMessage)
        End If
    End Sub

    'Only works with single state currently
    Private Sub ChangeQuoteState(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject, newState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, Optional oldState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None, Optional CopyInformationFromOldStateToNewState As Boolean = True)
        If newState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None AndAlso qqo.StateId <> QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(newState) Then
            'qqo.State = newState.ToString
            'qqo.StateId = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(newState)
            'qqo.QuickQuoteState = newState
            qqo.Set_QuoteStates(New List(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)({newState}), CopyInformationFromOldStateToNewState, oldState)
        End If
    End Sub
    Private Sub CopyPolicyholderToMultiStateApplicantIfNoApplicantsExist(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject)
        If qqo IsNot Nothing AndAlso qqo.Client IsNot Nothing AndAlso qqo.PackageParts IsNot Nothing AndAlso qqo.PackageParts.Count > 0 Then
            For Each mqqo As QuickQuote.CommonObjects.QuickQuoteObject In qqo.MultiStateQuotes
                If mqqo IsNot Nothing Then
                    'For Each p As QuickQuote.CommonObjects.QuickQuotePackagePart In mqqo.PackageParts
                    '    If p IsNot Nothing AndAlso p.Applicants Is Nothing OrElse p.Applicants.Count = 0 Then
                    '        If p.Applicants Is Nothing Then
                    '            p.Applicants = New List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)
                    '        End If
                    '        If p.Applicants.Count = 0 Then
                    '            p.Applicants.Add(New QuickQuote.CommonObjects.QuickQuoteApplicant)
                    '        End If
                    '        qqh.CopyQuickQuotePolicyholder1NameAddressEmailsAndPhonesToApplicant(qqo.Policyholder, p.Applicants(0))
                    '    End If
                    'Next
                    If mqqo.Applicants Is Nothing Then
                        mqqo.Applicants = New List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)
                    End If
                    If mqqo.Applicants.Count = 0 Then
                        mqqo.Applicants.Add(New QuickQuote.CommonObjects.QuickQuoteApplicant)
                    End If
                    qqh.CopyQuickQuotePolicyholder1NameAddressEmailsAndPhonesToApplicant(qqo.Policyholder, mqqo.Applicants(0))
                End If
            Next
        End If
    End Sub
    Private Sub CopyPolicyholderToPackagePartApplicantIfNoApplicantsExist(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject)
        If qqo IsNot Nothing AndAlso qqo.Policyholder IsNot Nothing AndAlso qqo.PackageParts IsNot Nothing AndAlso qqo.PackageParts.Count > 0 Then
            For Each p As QuickQuote.CommonObjects.QuickQuotePackagePart In qqo.PackageParts
                If p IsNot Nothing AndAlso p.Applicants Is Nothing OrElse p.Applicants.Count = 0 Then
                    If p.Applicants Is Nothing Then
                        p.Applicants = New List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)
                    End If
                    If p.Applicants.Count = 0 Then
                        p.Applicants.Add(New QuickQuote.CommonObjects.QuickQuoteApplicant)
                    End If
                    qqh.CopyQuickQuotePolicyholder1NameAddressEmailsAndPhonesToApplicant(qqo.Policyholder, p.Applicants(0))
                End If
            Next
        End If
    End Sub
    Private Sub CopyPolicyholderToApplicantIfNoApplicantsExist(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject)
        If qqo IsNot Nothing AndAlso qqo.Policyholder IsNot Nothing Then
            If qqo.Applicants Is Nothing Then
                qqo.Applicants = New List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)
            End If
            If qqo.Applicants.Count = 0 Then
                qqo.Applicants.Add(New QuickQuote.CommonObjects.QuickQuoteApplicant)
            End If
            qqh.CopyQuickQuotePolicyholder1NameAddressEmailsAndPhonesToApplicant(qqo.Policyholder, qqo.Applicants(0))
        End If
    End Sub
    Private Sub CopyLocationListAndChangeAddress(ByRef originalQuote As QuickQuote.CommonObjects.QuickQuoteObject, ByRef newQuote As QuickQuote.CommonObjects.QuickQuoteObject)
        If originalQuote IsNot Nothing AndAlso originalQuote.Locations IsNot Nothing AndAlso originalQuote.Locations.Count > 0 AndAlso newQuote IsNot Nothing Then
            If newQuote.Locations Is Nothing Then
                newQuote.Locations = New List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            End If
            Dim count As Integer = 1
            For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In originalQuote.Locations
                If newQuote.Locations.Count < count Then
                    newQuote.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation)
                End If
                Dim newLocation As QuickQuote.CommonObjects.QuickQuoteLocation = qqh.CloneObject(l)
                ChangeAddress(newLocation.Address)
                newQuote.Locations(count - 1) = newLocation
                count = count + 1
            Next
        End If
        qqh.CopyLocationsFromTopLevelToStateLevel(newQuote)
    End Sub

    Private Sub ChangeAllLocationAddresses(ByRef qqo As QuickQuote.CommonObjects.QuickQuoteObject)
        If qqo IsNot Nothing AndAlso qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
            ChangeLocationAddresses(qqo.Locations)
            qqh.CopyLocationsFromTopLevelToStateLevel(qqo)
        End If
    End Sub

    Private Sub ChangeLocationAddresses(ByRef Locations As List(Of QuickQuote.CommonObjects.QuickQuoteLocation))
        If Locations IsNot Nothing AndAlso Locations IsNot Nothing AndAlso Locations.Count > 0 Then
            For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In Locations
                ChangeAddress(l.Address)
            Next
        End If
    End Sub

    Private Sub ChangeAddress(ByRef Address As QuickQuote.CommonObjects.QuickQuoteAddress)
        If Address IsNot Nothing Then
            Address.City = "ATHENS"
            Address.County = "ATHENS"
            Address.State = "OHIO"
            Address.StateId = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio)
            Address.Zip = "45701-0000"
        End If
    End Sub

    Private Function NewCoverage(Of T As New)(coverageTypeEnum As Object) As T
        Dim c As New T
        qqh.SetPropertyValue(c, "CoverageType", coverageTypeEnum)
        Return c
    End Function

    Private Sub CopyBuildingsFromOriginalQuoteToNewQuote(originalQuote As QuickQuote.CommonObjects.QuickQuoteObject, newQuote As QuickQuote.CommonObjects.QuickQuoteObject)
        If originalQuote IsNot Nothing AndAlso GetListIfSomething(originalQuote.Locations) IsNot Nothing AndAlso newQuote IsNot Nothing AndAlso GetListIfSomething(newQuote.Locations) IsNot Nothing Then
            If originalQuote.Locations.Count = newQuote.Locations.Count Then
                Dim index As Integer = 0
                For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In originalQuote.Locations
                    If GetListIfSomething(originalQuote.Locations(0).Buildings) IsNot Nothing Then
                        newQuote.Locations(index).Buildings = l.Buildings
                    End If
                    index = index + 1
                Next
            Else
                If GetListIfSomething(originalQuote.Locations(0).Buildings) IsNot Nothing Then
                    newQuote.Locations(0).Buildings = originalQuote.Locations(0).Buildings
                End If
            End If
            qqh.CopyLocationsFromTopLevelToStateLevel(newQuote)
        End If
        'If originalQuote IsNot Nothing AndAlso GetListIfSomething(originalQuote.MultiStateQuotes) IsNot Nothing AndAlso GetListIfSomething(originalQuote.MultiStateQuotes(0).Locations) IsNot Nothing AndAlso GetListIfSomething(originalQuote.MultiStateQuotes(0).Locations(0).Buildings) IsNot Nothing AndAlso newQuote IsNot Nothing AndAlso GetListIfSomething(newQuote.MultiStateQuotes) IsNot Nothing AndAlso GetListIfSomething(newQuote.MultiStateQuotes(0).Locations) IsNot Nothing AndAlso GetListIfSomething(newQuote.MultiStateQuotes(0).Locations(0).Buildings) IsNot Nothing Then

        'End If
    End Sub

    Private Sub NewCoEndorsementTest()
        Dim polId As Integer = 2472452
        Dim polImageNum As Integer = 2
        Dim errMsg As String = ""
        Dim qqe As QuickQuote.CommonObjects.QuickQuoteObject = qqxml.QuickQuoteEndorsementForPolicyIdAndImageNum(polId, polImageNum, errorMessage:=errMsg)

        If qqe IsNot Nothing AndAlso String.IsNullOrWhiteSpace(errMsg) = False Then

        End If
    End Sub
    Private Sub DonEndorsementTest()
        Dim polId As Integer = 109910
        Dim polImageNum As Integer = 69
        Dim errMsg As String = ""
        Dim qqe As QuickQuote.CommonObjects.QuickQuoteObject = qqxml.QuickQuoteEndorsementForPolicyIdAndImageNum(polId, polImageNum, errorMessage:=errMsg)

        If qqe IsNot Nothing AndAlso String.IsNullOrWhiteSpace(errMsg) = False Then

        End If
    End Sub

End Class