Imports QuickQuote.CommonMethods
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 5/27/2015

Partial Class VR_e2Value_ReturnPage
    Inherits System.Web.UI.Page

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass
    Dim qqHelper As New QuickQuoteHelperClass 'added 5/27/2015

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim errorMsg As String = ""
            Me.lblResults.Text = "" 'added 5/27/2015
            If Request IsNot Nothing Then
                If Request.QueryString IsNot Nothing Then
                    Dim quoteId As String = ""
                    Dim lobId As String = ""
                    'pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId) 'for simple processing... will insert record into QuickQuotePropertyValuationResponse table but not Save anything back to quote
                    Dim wasSaveSuccessful As Boolean = False
                    'pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId, True, wasSaveSuccessful, errorMsg) 'for full processing... will insert record into QuickQuotePropertyValuationResponse table and Save basic results (LastCostEstimatorDate, RebuildCost) back to quote
                    ''pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId, True, wasSaveSuccessful, errorMsg, True) 'for full processing... will insert record into QuickQuotePropertyValuationResponse table and Save all results (LastCostEstimatorDate, RebuildCost, YearBuilt, ConstructionTypeId, SquareFeet, ArchitecturalStyle) back to quote
                    'updated 7/31/2015 to use new method that sets other params
                    Dim locationNum As Integer = 0
                    Dim buildingNum As Integer = 0
                    pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetVariables(Request.QueryString, quoteId, lobId, locationNum, buildingNum, True, wasSaveSuccessful, errorMsg) 'omitting saveAllReturnValues optional param
                    If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
                        If lobId <> "" AndAlso IsNumeric(lobId) = True Then
                            'success; redirect to LOB page... possibly use specific querystring param to load coverages screen or wherever e2Value results would be displayed
                            Select Case CInt(lobId)
                                Case 2 'Home Personal
                                    'Response.Redirect("VR3Home.aspx?quoteId=" & quoteId & "&section=pv") 'use config key if available
                                    'turned off auto-redirect 7/31/2015
                                    errorMsg = "Automatic redirect is currently turned off for this page; here's the link back to the Home page: " & "VR3Home.aspx?quoteId=" & quoteId & "&section=pv"
                                    'Case 3 'Dwelling Fire Personal
                                    '    Response.Redirect("VR3DwellingFire.aspx?quoteId=" & quoteId & "&section=pv") 'use config key if available or do the same as Case Else if DFR isn't being used yet
                                Case 17 'Farm; added 7/31/2015
                                    Dim farmLink As String = "VR3Farm.aspx?quoteId=" & quoteId & "&section=pv"
                                    If locationNum > 0 Then
                                        farmLink &= "&locationNum=" & locationNum.ToString
                                        If buildingNum > 0 Then
                                            farmLink &= "&buildingNum=" & buildingNum.ToString
                                        End If
                                    End If
                                    'Response.Redirect(farmLink)
                                    'turned off auto-redirect 7/31/2015
                                    errorMsg = "Automatic redirect is currently turned off for this page; here's the link back to the Farm page: " & farmLink
                                Case Else
                                    errorMsg = "no mapping for this lob (lobId " & lobId & ")"
                            End Select
                        Else
                            'lobId is missing or invalid
                            If errorMsg = "" Then
                                errorMsg = "insufficient parameters"
                            End If
                        End If
                    Else
                        'quoteId is missing or invalid
                        If errorMsg = "" Then
                            errorMsg = "insufficient parameters"
                        End If
                    End If
                    'added 5/27/2015
                    If Request.QueryString.Keys IsNot Nothing AndAlso Request.QueryString.Keys.Count > 0 Then
                        For Each key As String In Request.QueryString.Keys
                            If key <> "" Then
                                Dim keyString As String = key
                                Dim vals As String() = Request.QueryString.GetValues(key)
                                If vals IsNot Nothing AndAlso vals.Count > 0 Then
                                    keyString &= " = "
                                    Dim valCounter As Integer = 0
                                    For Each value As String In vals
                                        value = helper.UrlDecodedValue(value)
                                        valCounter += 1
                                        If valCounter > 1 Then
                                            keyString &= ", "
                                        End If
                                        keyString &= value
                                    Next
                                End If
                                Me.lblResults.Text = qqHelper.appendText(Me.lblResults.Text, keyString, "<br />")
                            End If
                        Next
                    End If
                Else
                    'querystring is nothing
                    errorMsg = "insufficient parameters"
                End If
            Else
                'request is nothing
                errorMsg = "insufficient parameters"
            End If

            'anything that gets this far should have an errorMsg
            If errorMsg = "" Then
                errorMsg = "problem processing e2Value response"
            End If
            'ShowError(errorMsg, True)
            ShowError(errorMsg) '5/20/2015 - using this for testing so it doesn't redirect
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
End Class
