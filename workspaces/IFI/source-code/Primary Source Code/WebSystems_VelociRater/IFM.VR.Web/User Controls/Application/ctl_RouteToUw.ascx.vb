Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Imports System
Imports IFM.PolicyLoader
Imports IFM.VR.Common.Helpers
Imports Diamond.Business.ThirdParty.tuxml

Public Class ctl_RouteToUw
    Inherits VRControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Dim btnScript = "if (canSkipUwPrompt_" + divRouteToUw.ClientID + ") return true;var proceed = confirm(""Proceed with Route to Underwriting?""); if(proceed){OpenRoutePopup(""" + divRouteToUw.ClientID + """);} return false;"
        'Me.btnRouteToUw.Attributes.Add("onclick", btnScript)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return False
    End Function

    '    Protected Sub btnRouteToUw_Click(sender As Object, e As EventArgs) Handles btnRouteToUw.Click
    '        Dim QQxml As New QuickQuoteXML()
    '        Dim SourceText As String = " Routed from Velocirater."
    '        Dim errorDetails As String = String.Empty

    '        If Quote IsNot Nothing Then
    '            Select Case Quote.LobType
    '                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGarage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialUmbrella
    '                    SourceText = " Routed from VR Commercial."
    '                    Exit Select
    '                Case Else
    '                    SourceText = " Routed from VR Personal."
    '                    Exit Select
    '            End Select
    '        End If

    '        If Not IsNullEmptyorWhitespace(hdnVehicleInfo.Value) Then
    '            txtRouteMessage.Text &= " Invalid VIN found for " & hdnVehicleInfo.Value & "."
    '        End If

    '        Try ' Matt A 7-31-2017 to include all error msgs when routing to UW
    '            If TypeOf Me.Page.Master Is VelociRater Then
    '                Dim ratedquote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
    '                'Updated 08/09/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    '                'If ratedquote IsNot Nothing Then
    '                If ratedquote IsNot Nothing AndAlso ratedquote.ValidationItems IsNot Nothing Then
    '                    For Each v In ratedquote.ValidationItems
    '                        If v.ValidationSeverityTypeId = 1 Then
    '                            errorDetails += v.Message + " "
    '                        End If
    '                    Next
    '                End If
    '            End If
    '        Catch ex As Exception
    '#If DEBUG Then
    '            Debugger.Break()
    '#End If
    '        End Try

    '        'added 12/16/2022
    '        If Common.Helpers.GenericHelper.SaveToDiamondOnNewBusinessRouteToUnderwriting() = True AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
    '            Dim quoteId As String = Me.QuoteId
    '            If QQHelper.IsPositiveIntegerString(quoteId) = False AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.Database_QuoteId) = True Then
    '                quoteId = Me.Quote.Database_QuoteId
    '            End If
    '            If Common.QuoteSave.QuoteSaveHelpers.DiamondHasLatest(quoteId) = False Then
    '                Dim successfullySavedToDiamond As Boolean = False
    '                If Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedNewBusinessQuote(Me.Quote, quoteId, successfullySavedToDiamond:=successfullySavedToDiamond, saveType:=If(IsOnAppPage() = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote)) = False AndAlso successfullySavedToDiamond = False Then
    '                    errorDetails = QQHelper.appendText(errorDetails, "unable to save the latest quote information to Diamond; please verify in VelociRater", splitter:=" ")
    '                End If
    '            End If
    '        End If


    '        If QQxml.DiamondService_SuccessfullyRoutedToUnderwriting(Me.Quote, Me.txtRouteMessage.Text + " " + errorDetails.Trim() + " " + SourceText) = False Then
    '            MessageBoxVRPers.Show("Routing failed.", Response, ScriptManager.GetCurrent(Me.Page), Me.Page)
    '        Else
    '            ' remove this quote from recent quote log
    '            Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, Me.QuoteId)
    '            Me.LockForm()
    '            'MessageBoxVRPers.Show("Routing complete.", Response, ScriptManager.GetCurrent(Me.Page), Me.Page)
    '            Me.VRScript.AddScriptLine("alert('Routing is complete.'); window.location = 'myvelocirater.aspx';")
    '        End If

    '    End Sub
    Private Sub PerformRouteToUW()
        Dim QQxml As New QuickQuoteXML()
        Dim SourceText As String = "Routed from Velocirater."
        Dim errorDetails As String = String.Empty
        Dim splitterTxt As String = vbCrLf

        If Quote IsNot Nothing Then
            Select Case Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGarage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialUmbrella
                    SourceText = "Routed from VR Commercial."
                    Exit Select
                Case Else
                    SourceText = "Routed from VR Personal."
                    Exit Select
            End Select
        End If

        Dim invalidVinMsg As String = ""
        If Not IsNullEmptyorWhitespace(hdnVehicleInfo.Value) Then
            invalidVinMsg = "Invalid VIN found for " & hdnVehicleInfo.Value & "."
        End If

        Try
            If TypeOf Me.Page.Master Is VelociRater Then
                Dim ratedquote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                If ratedquote IsNot Nothing AndAlso ratedquote.ValidationItems IsNot Nothing Then
                    For Each v In ratedquote.ValidationItems
                        If v.ValidationSeverityTypeId = 1 OrElse v.ValidationSeverityTypeId = 2 Then
                            errorDetails = QQHelper.appendText(errorDetails, v.Message, splitter:=splitterTxt)
                        End If
                    Next
                End If
                ''note: code directly below will pull all validations and not just errors
                'If String.IsNullOrWhiteSpace(errorDetails) = True AndAlso Me.ValidationHelper.ValidationErrors IsNot Nothing AndAlso Me.ValidationHelper.ValidationErrors.Count > 0 Then
                '    For Each ve As WebValidationItem In Me.ValidationHelper.ValidationErrors
                '        If ve IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ve.Message) = False Then
                '            errorDetails = QQHelper.appendText(errorDetails, ve.Message, splitter:=splitterTxt)
                '        End If
                '    Next
                'End If
                'note: this code will pull the errors out of Me.ValidationHelper.ValidationErrors; this will likely not return anything since it's control-specific
                If String.IsNullOrWhiteSpace(errorDetails) = True Then
                    Dim errorLst As List(Of WebValidationItem) = Me.ValidationHelper.GetErrors()
                    If errorLst IsNot Nothing AndAlso errorLst.Count > 0 Then
                        For Each ve As WebValidationItem In errorLst
                            If ve IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ve.Message) = False Then
                                errorDetails = QQHelper.appendText(errorDetails, ve.Message, splitter:=splitterTxt)
                            End If
                        Next
                    End If
                    Dim warnLst As List(Of WebValidationItem) = Me.ValidationHelper.GetWarnings()
                    If warnLst IsNot Nothing AndAlso warnLst.Count > 0 Then
                        For Each vw As WebValidationItem In warnLst
                            If vw IsNot Nothing AndAlso String.IsNullOrWhiteSpace(vw.Message) = False Then
                                errorDetails = QQHelper.appendText(errorDetails, vw.Message, splitter:=splitterTxt)
                            End If
                        Next
                    End If
                End If
                If String.IsNullOrWhiteSpace(errorDetails) = True Then
                    Dim qsaValidationHelper As ControlValidationHelper = Me.QuoteSummaryActionsValidationHelper
                    If qsaValidationHelper IsNot Nothing Then
                        Dim summaryErrors As List(Of WebValidationItem) = qsaValidationHelper.GetErrors()
                        If summaryErrors IsNot Nothing AndAlso summaryErrors.Count > 0 Then
                            For Each se As WebValidationItem In summaryErrors
                                If se IsNot Nothing AndAlso String.IsNullOrWhiteSpace(se.Message) = False Then
                                    errorDetails = QQHelper.appendText(errorDetails, se.Message, splitter:=splitterTxt)
                                End If
                            Next
                        End If
                        Dim summaryWarnings As List(Of WebValidationItem) = qsaValidationHelper.GetWarnings()
                        If summaryWarnings IsNot Nothing AndAlso summaryWarnings.Count > 0 Then
                            For Each sw As WebValidationItem In summaryWarnings
                                If sw IsNot Nothing AndAlso String.IsNullOrWhiteSpace(sw.Message) = False Then
                                    errorDetails = QQHelper.appendText(errorDetails, sw.Message, splitter:=splitterTxt)
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
        End Try

        If Common.Helpers.GenericHelper.SaveToDiamondOnNewBusinessRouteToUnderwriting() = True AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            Dim quoteId As String = Me.QuoteId
            If QQHelper.IsPositiveIntegerString(quoteId) = False AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.Database_QuoteId) = True Then
                quoteId = Me.Quote.Database_QuoteId
            End If
            If Common.QuoteSave.QuoteSaveHelpers.DiamondHasLatest(quoteId) = False Then
                Dim successfullySavedToDiamond As Boolean = False
                If Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedNewBusinessQuote(Me.Quote, quoteId, successfullySavedToDiamond:=successfullySavedToDiamond, saveType:=If(IsOnAppPage() = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote)) = False AndAlso successfullySavedToDiamond = False Then
                    errorDetails = QQHelper.appendText(errorDetails, "unable to save the latest quote information to Diamond; please verify in VelociRater", splitter:=splitterTxt)
                End If
            End If
        End If
        ' Look for attachements add message to fullrouteText 
        ' IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(context.Request.QueryString("agencyId"), context.Request.QueryString("quoteId")
        Dim fullRouteTxt As String = QQHelper.appendText(Me.txtRouteMessage.Text, invalidVinMsg, splitter:=splitterTxt)
        fullRouteTxt = QQHelper.appendText(fullRouteTxt, errorDetails.Trim(), splitter:=splitterTxt)
        fullRouteTxt = QQHelper.appendText(fullRouteTxt, SourceText, splitter:=splitterTxt)
        If Me.Quote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(Me.Quote.PolicyId) Then
            If IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(Me.Quote.AgencyId, Me.QuoteIdOrPolicyIdPipeImageNumber).Any Then
                Dim UwText = "There are attachments for review."
                fullRouteTxt = QQHelper.appendText(fullRouteTxt, UwText, splitter:=splitterTxt)
            End If
        End If
        If QQxml.DiamondService_SuccessfullyRoutedToUnderwriting(Me.Quote, fullRouteTxt) = False Then
            MessageBoxVRPers.Show("Routing failed.", Response, ScriptManager.GetCurrent(Me.Page), Me.Page)
        Else
            ' remove this quote from recent quote log
            Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, Me.QuoteId)
            Me.LockForm()
            'MessageBoxVRPers.Show("Routing complete.", Response, ScriptManager.GetCurrent(Me.Page), Me.Page)
            Me.VRScript.AddScriptLine("alert('Routing is complete.'); window.location = 'myvelocirater.aspx';")
        End If
    End Sub

    Private Sub btnContinueRoute_Click(sender As Object, e As EventArgs) Handles btnContinueRoute.Click
        PerformRouteToUW()
    End Sub
End Class