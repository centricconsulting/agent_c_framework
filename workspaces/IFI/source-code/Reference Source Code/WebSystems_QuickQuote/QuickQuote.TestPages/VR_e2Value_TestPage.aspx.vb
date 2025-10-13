Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Partial Class VR_e2Value_TestPage
    Inherits System.Web.UI.Page

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass
    Dim qqXml As New QuickQuoteXML
    Dim qq As QuickQuoteObject = Nothing
    Dim qqHelper As New QuickQuoteHelperClass
    Dim errorMsg As String = ""

    'Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    '    If Page.IsPostBack = False Then
    '        Dim errorMsg As String = ""
    '        If Request IsNot Nothing Then
    '            If Request.QueryString IsNot Nothing Then
    '                If Request.QueryString("quoteId") IsNot Nothing Then
    '                    Dim quoteId As String = Request.QueryString("quoteId").ToString
    '                    If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
    '                        Dim qq As QuickQuoteObject = Nothing
    '                        qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, errorMsg)
    '                        If qq IsNot Nothing Then
    '                            'use this if you want to save valuation responses back to quote from quote page instead of e2Value return page
    '                            Dim wasAnythingNewLoaded As Boolean = False
    '                            Dim didSaveOccur As Boolean = False
    '                            pvHelper.SaveAllPropertyValuationResponsesBackToQuote(qq, wasAnythingNewLoaded, didSaveOccur, errorMsg) 'to Save basic results (LastCostEstimatorDate, RebuildCost) back to quote
    '                            'pvHelper.SaveAllPropertyValuationResponsesBackToQuote(qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, True) 'to Save all results (LastCostEstimatorDate, RebuildCost, YearBuilt, ConstructionTypeId, SquareFeet, ArchitecturalStyle) back to quote

    '                            'use this to initiate e2Value process... save new record in PropertyValuationRequest table (and PropertyValuation table if necessary... doesn't already exist or existing valuation doesn't match current quote info) and get url to e2Value site
    '                            Dim e2ValueUrl As String = ""
    '                            pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl) 'simple processing... defaults locationNum to 1
    '                            'Dim wasSaveSuccessful As Boolean = False
    '                            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, 1, True, wasSaveSuccessful, errorMsg) 'same as previous but allows you to determine errorMsg
    '                            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, 1, True, wasSaveSuccessful, errorMsg, True) 'same as previous but will resend all params if the valuation already exists
    '                            If e2ValueUrl <> "" Then
    '                                'success; redirect to e2Value site
    '                                Response.Redirect(e2ValueUrl)
    '                            Else
    '                                'e2Value url was not determined
    '                                If errorMsg = "" Then
    '                                    errorMsg = "problem initiating e2Value request"
    '                                End If
    '                            End If
    '                        Else
    '                            'QuickQuoteObject was not loaded
    '                            If errorMsg = "" Then
    '                                errorMsg = "problem loading quote"
    '                            End If
    '                        End If
    '                    Else
    '                        'quoteId is missing or invalid
    '                        If errorMsg = "" Then
    '                            errorMsg = "insufficient parameters"
    '                        End If
    '                    End If
    '                Else
    '                    'no querystring param for quoteId
    '                    errorMsg = "insufficient parameters"
    '                End If
    '            Else
    '                'querystring is nothing
    '                errorMsg = "insufficient parameters"
    '            End If
    '        Else
    '            'request is nothing
    '            errorMsg = "insufficient parameters"
    '        End If

    '        'anything that gets this far should have an errorMsg
    '        If errorMsg = "" Then
    '            errorMsg = "problem processing e2Value response"
    '        End If
    '        ShowError(errorMsg, True)
    '    End If
    'End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim test As String = ""
            test = Server.MapPath("VR_e2Value_ReturnPage.aspx")
            Dim test2 As String = ""
            test2 = HttpRuntime.AppDomainAppPath
            Dim test3 As String = ""
            'test3 = HttpContext.Current.Application.ToString
            test3 = HttpContext.Current.Server.MapPath("VR_e2Value_ReturnPage.aspx")
            Dim test4 As String = ""
            test4 = AppDomain.CurrentDomain.BaseDirectory & "VR_e2Value_ReturnPage.aspx"
            Dim test5 As String = ""
            test5 = HttpContext.Current.Request.ApplicationPath
            Dim test6 As String = ""
            test6 = HttpContext.Current.Request.Url.AbsoluteUri
            Dim test7 As String = ""
            Dim test8 As String = ""
            Dim lastSlash As Integer = test6.LastIndexOf("/")
            If lastSlash >= 0 Then
                test7 = Left(test6, lastSlash + 1)
                test8 = Right(test6, Len(test6) - Len(test7))
            End If
            If test <> "" OrElse test2 <> "" OrElse test3 <> "" OrElse test4 <> "" OrElse test5 <> "" OrElse test6 <> "" Then

            End If
            'errorMsg = ""
            'Me.lblQuoteDetails.Text = ""
            'If Request IsNot Nothing Then
            '    If Request.QueryString IsNot Nothing Then
            '        If Request.QueryString("quoteId") IsNot Nothing Then
            '            Dim quoteId As String = Request.QueryString("quoteId").ToString
            '            If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
            '                qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, errorMsg)
            '                If qq IsNot Nothing Then
            '                    Me.lblQuoteDetails.Text = "Quote Loaded... quoteId = " & quoteId
            '                    If qq.QuoteNumber <> "" Then
            '                        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "quoteNumber = " & qq.QuoteNumber, "; ")
            '                    End If
            '                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
            '                        Dim locCounter As Integer = 0
            '                        For Each l As QuickQuoteLocation In qq.Locations
            '                            locCounter += 1
            '                            If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.db_propertyValuationId <> "" Then
            '                                Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "location #" & locCounter.ToString & " propertyValuationId = " & l.PropertyValuation.db_propertyValuationId, "<br />")
            '                                If l.PropertyValuation.VendorValuationId <> "" Then
            '                                    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "VendorValuationId = " & l.PropertyValuation.VendorValuationId, "; ")
            '                                End If
            '                                If l.PropertyValuation.Request IsNot Nothing AndAlso l.PropertyValuation.Request.db_propertyValuationRequestId <> "" Then
            '                                    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "propertyValuationRequestId = " & l.PropertyValuation.Request.db_propertyValuationRequestId, "; ")
            '                                End If
            '                                If l.PropertyValuation.Response IsNot Nothing AndAlso l.PropertyValuation.Response.db_propertyValuationResponseId <> "" Then
            '                                    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "propertyValuationResponseId = " & l.PropertyValuation.Response.db_propertyValuationResponseId, "; ")
            '                                    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "response loadedBackIntoQuote = " & l.PropertyValuation.Response.db_loadedBackIntoQuote.ToString, "; ")
            '                                End If
            '                            Else
            '                                Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "no property valuation for location #" & locCounter.ToString, "<br />")
            '                            End If
            '                        Next
            '                    Else
            '                        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "no locations", "<br />")
            '                    End If

            '                    If ViewState.Item("qq") IsNot Nothing Then
            '                        ViewState.Item("qq") = qq
            '                    Else
            '                        ViewState.Add("qq", qq)
            '                    End If
            '                Else
            '                    'QuickQuoteObject was not loaded
            '                    If errorMsg = "" Then
            '                        errorMsg = "problem loading quote"
            '                    End If
            '                End If
            '            Else
            '                'quoteId is missing or invalid
            '                If errorMsg = "" Then
            '                    errorMsg = "insufficient parameters"
            '                End If
            '            End If
            '        Else
            '            'no querystring param for quoteId
            '            errorMsg = "insufficient parameters"
            '        End If
            '    Else
            '        'querystring is nothing
            '        errorMsg = "insufficient parameters"
            '    End If
            'Else
            '    'request is nothing
            '    errorMsg = "insufficient parameters"
            'End If

            'If errorMsg <> "" Then
            '    ShowError(errorMsg, True)
            'End If

            'added 5/27/2015
            Session("e2ValueTest_ReturnUrl") = Nothing
            Session("e2ValueTest_AppendReturnStructuresRpc") = Nothing

            'moved logic to separate method so it could be reloaded
            'TestBuildingUpdate() 'added 8/31/2015 for testing building valuations (estimator dependent on FarmStructureTypeId... dwellings go to Pronto Lite Residential; others go to Farm & Ranch)
            LoadPage()
        End If
    End Sub
    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub
    Private Sub LoadPage()
        errorMsg = ""
        Me.lblQuoteDetails.Text = ""
        If Request IsNot Nothing Then
            If Request.QueryString IsNot Nothing Then
                If Request.QueryString("quoteId") IsNot Nothing Then
                    Dim quoteId As String = Request.QueryString("quoteId").ToString
                    If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
                        qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, errorMsg)
                        If qq IsNot Nothing Then
                            Me.lblQuoteDetails.Text = "Quote Loaded... quoteId = " & quoteId
                            If qq.QuoteNumber <> "" Then
                                Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "quoteNumber = " & qq.QuoteNumber, "; ")
                            End If
                            If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                                Dim locCounter As Integer = 0
                                For Each l As QuickQuoteLocation In qq.Locations
                                    locCounter += 1
                                    'If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.db_propertyValuationId <> "" Then
                                    '    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "location #" & locCounter.ToString & " propertyValuationId = " & l.PropertyValuation.db_propertyValuationId, "<br />")
                                    '    If l.PropertyValuation.VendorValuationId <> "" Then
                                    '        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "VendorValuationId = " & l.PropertyValuation.VendorValuationId, "; ")
                                    '    End If
                                    '    If l.PropertyValuation.Request IsNot Nothing AndAlso l.PropertyValuation.Request.db_propertyValuationRequestId <> "" Then
                                    '        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "propertyValuationRequestId = " & l.PropertyValuation.Request.db_propertyValuationRequestId, "; ")
                                    '    End If
                                    '    If l.PropertyValuation.Response IsNot Nothing AndAlso l.PropertyValuation.Response.db_propertyValuationResponseId <> "" Then
                                    '        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "propertyValuationResponseId = " & l.PropertyValuation.Response.db_propertyValuationResponseId, "; ")
                                    '        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "response loadedBackIntoQuote = " & l.PropertyValuation.Response.db_loadedBackIntoQuote.ToString, "; ")
                                    '    End If
                                    'Else
                                    '    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "no property valuation for location #" & locCounter.ToString, "<br />")
                                    'End If
                                    'updated 7/29/2015 to use new method and to include building stuff
                                    Dim buildCounter As Integer = 0
                                    Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, PropertyValuationDetails(l.PropertyValuation, locCounter, buildCounter), "<br />")
                                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                        buildCounter = 0
                                        For Each b As QuickQuoteBuilding In l.Buildings
                                            buildCounter += 1
                                            Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, PropertyValuationDetails(b.PropertyValuation, locCounter, buildCounter), "<br />")
                                        Next
                                    Else
                                        Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "no buildings for location #" & locCounter.ToString, "<br />")
                                    End If
                                Next
                            Else
                                Me.lblQuoteDetails.Text = qqHelper.appendText(Me.lblQuoteDetails.Text, "no locations", "<br />")
                            End If

                            If ViewState.Item("qq") IsNot Nothing Then
                                ViewState.Item("qq") = qq
                            Else
                                ViewState.Add("qq", qq)
                            End If
                        Else
                            'QuickQuoteObject was not loaded
                            If errorMsg = "" Then
                                errorMsg = "problem loading quote"
                            End If
                        End If
                    Else
                        'quoteId is missing or invalid
                        If errorMsg = "" Then
                            errorMsg = "insufficient parameters"
                        End If
                    End If
                Else
                    'no querystring param for quoteId
                    errorMsg = "insufficient parameters"
                End If
            Else
                'querystring is nothing
                errorMsg = "insufficient parameters"
            End If
        Else
            'request is nothing
            errorMsg = "insufficient parameters"
        End If

        If errorMsg <> "" Then
            'ShowError(errorMsg, True)
            ShowError(errorMsg) '5/20/2015 - using this for testing so it doesn't redirect
        End If
    End Sub
    'added function 7/29/2015 for re-use
    Private Function PropertyValuationDetails(ByVal pv As QuickQuotePropertyValuation, ByVal locNum As Integer, Optional ByVal buildNum As Integer = 0) As String
        Dim details As String = ""

        Dim locAndBuildNumText As String = "location #" & locNum.ToString
        If buildNum > 0 Then
            locAndBuildNumText &= " / building #" & buildNum.ToString
        End If

        Dim locNumToUse As Integer = 1
        Dim buildNumToUse As Integer = 0
        If Request.QueryString("locationNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString) = True Then
            locNumToUse = CInt(Request.QueryString("locationNum").ToString)
        ElseIf Request.QueryString("locationNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNumber").ToString) = True Then
            locNumToUse = CInt(Request.QueryString("locationNumber").ToString)
        End If
        If Request.QueryString("buildingNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString) = True Then
            buildNumToUse = CInt(Request.QueryString("buildingNum").ToString)
        ElseIf Request.QueryString("buildingNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNumber").ToString) = True Then
            buildNumToUse = CInt(Request.QueryString("buildingNumber").ToString)
        End If

        If locNum = locNumToUse AndAlso buildNum = buildNumToUse Then
            locAndBuildNumText &= "<b> (target)</b>"
        End If

        If pv IsNot Nothing AndAlso pv.db_propertyValuationId <> "" Then
            details = qqHelper.appendText(details, locAndBuildNumText & " propertyValuationId = " & pv.db_propertyValuationId, "<br />")
            If pv.VendorValuationId <> "" Then
                details = qqHelper.appendText(details, "VendorValuationId = " & pv.VendorValuationId, "; ")
            End If
            If pv.Request IsNot Nothing AndAlso pv.Request.db_propertyValuationRequestId <> "" Then
                details = qqHelper.appendText(details, "propertyValuationRequestId = " & pv.Request.db_propertyValuationRequestId, "; ")
            End If
            If pv.Response IsNot Nothing AndAlso pv.Response.db_propertyValuationResponseId <> "" Then
                details = qqHelper.appendText(details, "propertyValuationResponseId = " & pv.Response.db_propertyValuationResponseId, "; ")
                details = qqHelper.appendText(details, "response loadedBackIntoQuote = " & pv.Response.db_loadedBackIntoQuote.ToString, "; ")
            End If
        Else
            details = qqHelper.appendText(details, "no property valuation for " & locAndBuildNumText, "<br />")
        End If

        Return details
    End Function
    Private Sub SaveAllPropertyValuationResponsesBackToQuote()
        errorMsg = ""
        Dim reloadPage As Boolean = False
        If ViewState.Item("qq") IsNot Nothing Then
            qq = CType(ViewState.Item("qq"), QuickQuoteObject)

            'use this if you want to save valuation responses back to quote from quote page instead of e2Value return page
            Dim wasAnythingNewLoaded As Boolean = False
            Dim didSaveOccur As Boolean = False
            pvHelper.SaveAllPropertyValuationResponsesBackToQuote(qq, wasAnythingNewLoaded, didSaveOccur, errorMsg) 'to Save basic results (LastCostEstimatorDate, RebuildCost) back to quote
            'pvHelper.SaveAllPropertyValuationResponsesBackToQuote(qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, True) 'to Save all results (LastCostEstimatorDate, RebuildCost, YearBuilt, ConstructionTypeId, SquareFeet, ArchitecturalStyle) back to quote

            If errorMsg <> "" Then
                errorMsg = "Error: " & errorMsg
            End If
            errorMsg = qqHelper.appendText(errorMsg, "WasAnythingNewLoaded = " & wasAnythingNewLoaded.ToString & "; DidSaveOccur = " & didSaveOccur.ToString, "<br />")

            If didSaveOccur = True Then
                reloadPage = True
            End If
        Else
            errorMsg = "unable to load QuickQuoteObject from ViewState"
        End If

        ShowError(errorMsg)

        If reloadPage = True Then
            LoadPage()
        End If
    End Sub
    Private Sub InitiateE2Value()
        errorMsg = ""
        Me.urlSection.Visible = False 'added 5/27/2015
        If ViewState.Item("qq") IsNot Nothing Then
            qq = CType(ViewState.Item("qq"), QuickQuoteObject)

            Dim locNum As Integer = 1
            Dim buildNum As Integer = 0
            If Request.QueryString("locationNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString) = True Then
                locNum = CInt(Request.QueryString("locationNum").ToString)
            ElseIf Request.QueryString("locationNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNumber").ToString) = True Then
                locNum = CInt(Request.QueryString("locationNumber").ToString)
            End If
            If Request.QueryString("buildingNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString) = True Then
                buildNum = CInt(Request.QueryString("buildingNum").ToString)
            ElseIf Request.QueryString("buildingNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNumber").ToString) = True Then
                buildNum = CInt(Request.QueryString("buildingNumber").ToString)
            End If

            'added 5/27/2015
            If qq IsNot Nothing AndAlso Me.cbNewValuation.Checked = True Then
                If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                    If locNum > 1 OrElse buildNum > 0 Then 'added IF 7/29/2015; original logic in ELSE
                        If qq.Locations.Count >= locNum Then
                            With qq.Locations(locNum - 1)
                                If buildNum > 0 Then
                                    If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildNum Then
                                        With .Buildings(buildNum - 1)
                                            If .PropertyValuation IsNot Nothing Then
                                                .PropertyValuation.Dispose()
                                                .PropertyValuation = Nothing
                                            End If
                                        End With
                                    End If
                                Else
                                    If .PropertyValuation IsNot Nothing Then
                                        .PropertyValuation.Dispose()
                                        .PropertyValuation = Nothing
                                    End If
                                End If
                            End With
                        End If
                    Else
                        If qq.Locations(0).PropertyValuation IsNot Nothing Then
                            qq.Locations(0).PropertyValuation.Dispose()
                            qq.Locations(0).PropertyValuation = Nothing
                        End If
                    End If
                End If
            End If
            
            Session("e2ValueTest_ReturnUrl") = Nothing
            Session("e2ValueTest_AppendReturnStructuresRpc") = Nothing
            If Me.cbUseTestReturnPage.Checked = True Then
                'Session("e2ValueTest_ReturnUrl") = Server.MapPath("VR_e2Value_ReturnPage.aspx?")
                Session("e2ValueTest_ReturnUrl") = ""
                Dim currentUrl As String = HttpContext.Current.Request.Url.AbsoluteUri
                Dim currentDirectory As String = ""
                Dim currentPage As String = ""
                Dim lastSlash As Integer = currentUrl.LastIndexOf("/")
                If lastSlash >= 0 Then
                    currentDirectory = Left(currentUrl, lastSlash + 1)
                    currentPage = Right(currentUrl, Len(currentUrl) - Len(currentDirectory))
                    Session("e2ValueTest_ReturnUrl") = currentDirectory & "VR_e2Value_ReturnPage.aspx?"
                End If
            End If
            If Me.cbAppendReturnStructuresRpc.Checked = True Then
                Session("e2ValueTest_AppendReturnStructuresRpc") = "true"
            End If

            'use this to initiate e2Value process... save new record in PropertyValuationRequest table (and PropertyValuation table if necessary... doesn't already exist or existing valuation doesn't match current quote info) and get url to e2Value site
            Dim e2ValueUrl As String = ""
            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl) 'simple processing... defaults locationNum to 1
            'Dim wasSaveSuccessful As Boolean = False
            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, 1, True, wasSaveSuccessful, errorMsg) 'same as previous but allows you to determine errorMsg
            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, 1, True, wasSaveSuccessful, errorMsg, True) 'same as previous but will resend all params if the valuation already exists
            'If qq IsNot Nothing AndAlso qq.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then 'added IF 4/29/2015 for FAR testing... to resend all params on subsequent requests; original logic in ELSE; note: according to Angela Connolly (e2Value), P3R ignores client info in subsequent requests since the info (at least address) cannot be changed... HSD uses the client info since the data can be changed in e2Value
            '    Dim wasSaveSuccessful As Boolean = False
            '    pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, 1, True, wasSaveSuccessful, errorMsg, True)
            'Else
            '    pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl)
            'End If
            'updated 7/29/2015 to use locationNum and buildingNum querystring params; now locNum and buildNum variables are above
            'Dim locNum As Integer = 1
            'Dim buildNum As Integer = 0
            'If Request.QueryString("locationNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString) = True Then
            '    locNum = CInt(Request.QueryString("locationNum").ToString)
            'ElseIf Request.QueryString("locationNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNumber").ToString) = True Then
            '    locNum = CInt(Request.QueryString("locationNumber").ToString)
            'End If
            'If Request.QueryString("buildingNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString) = True Then
            '    buildNum = CInt(Request.QueryString("buildingNum").ToString)
            'ElseIf Request.QueryString("buildingNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNumber").ToString) = True Then
            '    buildNum = CInt(Request.QueryString("buildingNumber").ToString)
            'End If
            Dim wasSaveSuccessful As Boolean = False
            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, locNum, buildNum, True, wasSaveSuccessful, errorMsg, True)
            'updated 7/31/2015 for optional propertyType param
            Dim propertyType As QuickQuotePropertyValuationHelperClass.ValuationPropertyType = QuickQuotePropertyValuationHelperClass.ValuationPropertyType.DefaultByInfo
            'pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, locNum, buildNum, True, wasSaveSuccessful, errorMsg, propertyType, True) '8/3/2015 note: doesn't need last optional param (sendAllParamsForExistingValuation); removed 8/4/2015
            pvHelper.PopulateE2ValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, locNum, buildNum, True, wasSaveSuccessful, errorMsg, propertyType)

            If e2ValueUrl <> "" Then
                'success; redirect to e2Value site
                If Me.cbViewUrl.Checked = True Then 'added 5/27/2015; original logic in ELSE
                    Me.txtUrl.Text = e2ValueUrl
                    Me.urlSection.Visible = True
                Else
                    Response.Redirect(e2ValueUrl)
                End If
            Else
                'e2Value url was not determined
                If errorMsg = "" Then
                    errorMsg = "problem initiating e2Value request"
                End If
            End If
        Else
            errorMsg = "unable to load QuickQuoteObject from ViewState"
        End If

        If errorMsg <> "" Then 'should have something if it gets here... ELSE will have already redirected to e2Value
            ShowError(errorMsg)
        End If

    End Sub

    Protected Sub btnSaveResponses_Click(sender As Object, e As EventArgs) Handles btnSaveResponses.Click
        SaveAllPropertyValuationResponsesBackToQuote()
    End Sub

    Protected Sub btnInitiateE2Value_Click(sender As Object, e As EventArgs) Handles btnInitiateE2Value.Click
        InitiateE2Value()
    End Sub

    Protected Sub btnGoToE2Value_Click(sender As Object, e As EventArgs) Handles btnGoToE2Value.Click
        errorMsg = ""
        If Me.txtUrl.Text <> "" Then
            Response.Redirect(Me.txtUrl.Text)
        Else
            errorMsg = "No Url"
        End If
        If errorMsg <> "" Then
            ShowError(errorMsg)
        End If
    End Sub
    Private Sub TestBuildingUpdate()
        Dim quoteId As String = ""
        If Request.QueryString("quoteId") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("quoteId").ToString) = True Then
            quoteId = Request.QueryString("quoteId").ToString
        End If
        Dim locNumToUse As Integer = 1
        Dim buildNumToUse As Integer = 0
        If Request.QueryString("locationNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNum").ToString) = True Then
            locNumToUse = CInt(Request.QueryString("locationNum").ToString)
        ElseIf Request.QueryString("locationNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("locationNumber").ToString) = True Then
            locNumToUse = CInt(Request.QueryString("locationNumber").ToString)
        End If
        If Request.QueryString("buildingNum") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNum").ToString) = True Then
            buildNumToUse = CInt(Request.QueryString("buildingNum").ToString)
        ElseIf Request.QueryString("buildingNumber") IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(Request.QueryString("buildingNumber").ToString) = True Then
            buildNumToUse = CInt(Request.QueryString("buildingNumber").ToString)
        End If

        If quoteId = "24958" AndAlso locNumToUse = 1 AndAlso buildNumToUse = 3 Then
            Dim qqxml As New QuickQuoteXML
            Dim err As String = ""

            Dim strQQ As String = ""
            Dim quickQuote As QuickQuoteObject = Nothing
            qqxml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, err)
            If err <> "" Then
                err = "" 'just reset
            End If
            If quickQuote IsNot Nothing Then
                Dim qqBuilding As QuickQuoteBuilding = qqHelper.QuickQuoteBuildingForActiveNum(quickQuote, locNumToUse, buildNumToUse)
                If qqBuilding IsNot Nothing Then
                    With qqBuilding
                        If .FarmStructureTypeId = "12" Then 'Grain Bin
                            .FarmStructureTypeId = "18" 'Farm Dwelling
                        Else
                            .FarmStructureTypeId = "12" 'Grain Bin
                        End If
                    End With
                    qqxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, quoteId, err)
                    If err = "" Then
                        'okay

                    Else
                        'error
                    End If
                End If
            End If
        End If
    End Sub
End Class
