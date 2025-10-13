Imports Diamond.Business.ThirdParty.IntegrationServicesProxy
Imports QuickQuote.CommonMethods

Public Class VR3E2ValueReturn
    Inherits System.Web.UI.Page

    Dim pvHelper As New QuickQuotePropertyValuationHelperClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim errorMsg As String = ""
            If Request IsNot Nothing Then
                If Request.QueryString IsNot Nothing Then
                    Dim quoteId As String = ""
                    Dim lobId As String = ""
                    Dim policyId As String = ""
                    Dim policyImageNum As String = ""

                    'pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId) 'for simple processing... will insert record into QuickQuotePropertyValuationResponse table but not Save anything back to quote
                    Dim wasSaveSuccessful As Boolean = False
                    'pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId, True, wasSaveSuccessful, errorMsg) 'for full processing... will insert record into QuickQuotePropertyValuationResponse table and Save basic results (LastCostEstimatorDate, RebuildCost) back to quote
                    ''pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(Request.QueryString, quoteId, lobId, True, wasSaveSuccessful, errorMsg, True) 'for full processing... will insert record into QuickQuotePropertyValuationResponse table and Save all results (LastCostEstimatorDate, RebuildCost, YearBuilt, ConstructionTypeId, SquareFeet, ArchitecturalStyle) back to quote
                    'updated 7/31/2015 to use new method that sets other params
                    Dim locationNum As Integer = 0
                    Dim buildingNum As Integer = 0
                    pvHelper.ProcessE2ValueResponseFromNameValueCollectionAndSetVariables(Request.QueryString, quoteId, lobId, locationNum, buildingNum, True, wasSaveSuccessful, errorMsg, policyId:=policyId, policyImageNum:=policyImageNum) 'omitting saveAllReturnValues optional param
                    If (quoteId <> "" AndAlso IsNumeric(quoteId) = True) OrElse
                        (policyId <> "" AndAlso IsNumeric(policyId) AndAlso policyImageNum <> "" AndAlso IsNumeric(policyImageNum)) Then

                        ' wipe session
                        Dim linkText As String
                        Dim linkId As String
                        If Not String.IsNullOrWhiteSpace(quoteId) Then
                            VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(quoteId) ' Matt A - 12-9-14
                            linkText = "quoteId"
                            linkId = quoteId
                        Else
                            VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(policyId, policyImageNum)
                            linkText = "EndorsementPolicyIdAndImageNum"
                            linkId = policyId & "|" & policyImageNum
                        End If

                        If lobId <> "" AndAlso IsNumeric(lobId) = True Then
                            'success; redirect to LOB page... possibly use specific querystring param to load coverages screen or wherever e2Value results would be displayed
                            Select Case CInt(lobId)
                                Case 2 'Home Personal
                                    Response.Redirect(String.Format("VR3Home.aspx?{0}={1}&{2}={3}", linkText, linkId, IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs, IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString())) 'use config key if available
                                Case 3 'Dwelling Fire
                                    Response.Redirect(String.Format("VR3DwellingFire.aspx?{0}={1}&{2}={3}", linkText, linkId, IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs, IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages.ToString())) 'use config key if available
                                Case 17 ' Farm
                                    Dim farmLink As String = String.Format("VR3Farm.aspx?{0}={1}&{2}={3}", linkText, linkId, IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs, IFM.VR.Common.Workflow.Workflow.WorkflowSection.location.ToString())
                                    If locationNum > 0 Then
                                        farmLink &= "&locationNum=" & locationNum.ToString
                                        If buildingNum > 0 Then
                                            farmLink &= "&buildingNum=" & buildingNum.ToString
                                        End If
                                    End If
                                    Response.Redirect(farmLink)
                                    'Case 3 'Dwelling Fire Personal
                                    '    Response.Redirect("VR3DwellingFire.aspx?quoteId=" & quoteId & "&section=pv") 'use config key if available or do the same as Case Else if DFR isn't being used yet
                                Case Else
                                    errorMsg = "No Mapping For This LOB"
                            End Select

                        Else
                            'lobId is missing or invalid
                            If errorMsg = "" Then
                                errorMsg = "Insufficient Parameters"
                            End If
                        End If
                    Else
                        'quoteId is missing or invalid
                        If errorMsg = "" Then
                            errorMsg = "Insufficient Parameters"
                        End If
                    End If
                Else
                    'querystring is nothing
                    errorMsg = "Insufficient Parameters"
                End If
            Else
                'request is nothing
                errorMsg = "Insufficient Parameters"
            End If

            'anything that gets this far should have an errorMsg
            If errorMsg = "" Then
                errorMsg = "Problem Processing E2Value Response"
            End If
            ShowError(errorMsg, True)
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