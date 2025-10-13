Imports System.Web.Services
Imports IFM.PrimativeExtensions
Imports PopupMessageClass
Imports QuickQuote.CommonMethods

Public Class MyVelocirater
    Inherits System.Web.UI.Page

    Public Const RecordsPerLob As Int32 = 8

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MaintainScrollPositionOnPostBack = True

        'SetLOBTextForMainButtons()

        If Not IsPostBack Then
            SetVRViewFromRequest()
            ' This guy (eml) is here for the redirect back to the MyVelocirater page from the EmailToUnderwriting control
            ' If the 'eml' parameter is there and = 1 then we display the thank you for your email dialog.
            ' MGB 6/14/17
            If Request("eml") IsNot Nothing AndAlso Request("eml") = "1" Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "test", "alert('Thank you for your question, our underwriting team will respond soon.');", True)
            End If

            If Request.QueryString("stats") Is Nothing Then
                Me.ctlVr3Stats.Visible = False
            End If
            If DirectCast(Me.Page.Master, VelociRater).IsStaff Then
                If Request.QueryString("cglNewLook") IsNot Nothing Then
                    Session("cglNewLook") = True
                End If

                If Request.QueryString("allowPullFromDiamond") IsNot Nothing Then ' when true user can pull quotes from diamond directly even if it is not from a comp rater
                    Session("allowPullFromDiamond") = True
                End If

                If Request.QueryString("allowUpdateFromDiamond") IsNot Nothing Then ' if true an option is added to the actions dropdown when what is in Diamond is newer than what is in VR database
                    Session("allowUpdateFromDiamond") = True
                End If
            End If

            ' COMMERCIAL LINES UPGRADE LOGIC --------------------------
            ' Bop
            If System.Configuration.ConfigurationManager.AppSettings("BOPNewLook") IsNot Nothing Then
                Session("bopNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("BOPNewLook"))
            End If
            If Request.QueryString("bopNewLook") IsNot Nothing Then
                Session("bopNewLook") = True
            End If
            ' Cap
            If System.Configuration.ConfigurationManager.AppSettings("CAPNewLook") IsNot Nothing Then
                Session("capNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("CAPNewLook"))
            End If
            If Request.QueryString("capNewLook") IsNot Nothing Then
                Session("capNewLook") = True
            End If
            ' WCP
            If System.Configuration.ConfigurationManager.AppSettings("WCPNewLook") IsNot Nothing Then
                Session("wcpNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("WCPNewLook"))
            End If
            If Request.QueryString("wcpNewLook") IsNot Nothing Then
                Session("wcpNewLook") = True
            End If
            ' CGL
            If System.Configuration.ConfigurationManager.AppSettings("CGLNewLook") IsNot Nothing Then
                Session("cglNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("CGLNewLook"))
            End If
            If Request.QueryString("cglNewLook") IsNot Nothing Then
                Session("cglNewLook") = True
            End If
            If System.Configuration.ConfigurationManager.AppSettings("CPRNewLook") IsNot Nothing Then
                Session("cprNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("CPRNewLook"))
            End If
            If Request.QueryString("cprNewLook") IsNot Nothing Then
                Session("cprNewLook") = True
            End If
            If System.Configuration.ConfigurationManager.AppSettings("CPPNewLook") IsNot Nothing Then
                Session("cppNewLook") = CBool(System.Configuration.ConfigurationManager.AppSettings("CPPNewLook"))
            End If
            If Request.QueryString("cppNewLook") IsNot Nothing Then
                Session("cppNewLook") = True
            End If
            ' END COMMERCIAL LINES UPGRADE LOGIC--------------------------


            '' Start App/Quote Kill LOGIC --------------------------
            'If Session("QuoteStopOrKill_ShowMsg") IsNot Nothing AndAlso String.IsNullOrEmpty(Session("QuoteStopOrKill_ShowMsg")) = False AndAlso CBool(Session("QuoteStopOrKill_ShowMsg")) = True Then
            '    Dim titleMsg = If(String.IsNullOrEmpty(Session("QuoteStopOrKill_titleMsg")) = False, Session("QuoteStopOrKill_titleMsg").ToString, "Important Notice")
            '    Dim bodyMsg = If(String.IsNullOrEmpty(Session("QuoteStopOrKill_bodyMsg")) = False, Session("QuoteStopOrKill_bodyMsg").ToString, "This quote is not eligible for coverage with Indiana Farmers Mutual Insurance Company.")
            '    Using popup As New PopupMessageObject(Page, bodyMsg, titleMsg)
            '        With popup
            '            .isFixedPositionOnScreen = True
            '            .ZIndexOfPopup = 2
            '            .isModal = True
            '            .Image = PopupMessageObject.ImageOptions.None
            '            .hideCloseButton = True
            '            .AddButton("OK", True)
            '            '.AddButton("OK", True, "", System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage"))
            '            ' Foward to home if titlebar "X" is visible
            '            '.AddPopupEvent(PopupMessageObject.PopupEventType.close, "window.location.replace('" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "')")
            '            .CreateDynamicPopUpWindow()
            '        End With
            '    End Using
            'End If
            ''Clear these session values
            'Session.Remove("QuoteStopOrKill_ShowMsg")
            'Session.Remove("QuoteStopOrKill_titleMsg")
            'Session.Remove("QuoteStopOrKill_bodyMsg")

            '' End App/Quote Kill LOGIC --------------------------



            If Request.QueryString("diaIntegrationToken") IsNot Nothing Then
                Dim xml As New QuickQuote.CommonMethods.QuickQuoteXML()
                Me.ImportDiamondQuote(xml.GetQuoteNumberForDiamondIntegrationToken(Request.QueryString("diaIntegrationToken")))
            End If

            If Request.QueryString("idpQuoteNumber") IsNot Nothing Then
                Dim xml As New QuickQuote.CommonMethods.QuickQuoteXML()
                Me.ImportDiamondQuote(Request.QueryString("idpQuoteNumber").ToString)
            End If

            'added for testing Endorsements 2/14/2019
            'If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("NewEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewEndorsementPolicyId").ToString) > 0 Then
            '    Dim newEndorsementErrorMsg As String = ""
            '    Dim newEndorsementImageNum As Integer = 0
            '    Dim qqEndorsement As QuickQuote.CommonObjects.QuickQuoteObject = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(CInt(Request.QueryString("NewEndorsementPolicyId").ToString), Date.Today.ToShortDateString, endorsementRemarks:="new endorsement for policyId " & CInt(Request.QueryString("NewEndorsementPolicyId").ToString).ToString, newPolicyImageNum:=newEndorsementImageNum, errorMessage:=newEndorsementErrorMsg)
            '    If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True AndAlso qqEndorsement IsNot Nothing AndAlso newEndorsementImageNum > 0 Then

            '    End If
            'End If
            'updated 2/27/2019 to accommodate other Endorsement testing
            'If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing Then
            '    If Request.QueryString("NewEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewEndorsementPolicyId").ToString) > 0 Then
            '        Dim newEndorsementErrorMsg As String = ""
            '        Dim newEndorsementImageNum As Integer = 0
            '        Dim qqEndorsement As QuickQuote.CommonObjects.QuickQuoteObject = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(CInt(Request.QueryString("NewEndorsementPolicyId").ToString), Date.Today.ToShortDateString, endorsementRemarks:="new endorsement for policyId " & CInt(Request.QueryString("NewEndorsementPolicyId").ToString).ToString, newPolicyImageNum:=newEndorsementImageNum, errorMessage:=newEndorsementErrorMsg)
            '        If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True AndAlso qqEndorsement IsNot Nothing AndAlso newEndorsementImageNum > 0 Then

            '        End If
            '    ElseIf Request.QueryString("EndorsementPolicyIdToDelete") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdToDelete").ToString) = False AndAlso IsNumeric(Request.QueryString("EndorsementPolicyIdToDelete").ToString) = True AndAlso CInt(Request.QueryString("EndorsementPolicyIdToDelete").ToString) > 0 Then
            '        Dim endorsementDeleteSuccess As Boolean = False
            '        Dim endorsementDeleteErrorMessage As String = ""
            '        endorsementDeleteSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullyDeletedEndorsementQuote(CInt(Request.QueryString("EndorsementPolicyIdToDelete").ToString), errorMessage:=endorsementDeleteErrorMessage)
            '        If endorsementDeleteSuccess = False OrElse String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then

            '        End If
            '    ElseIf Request.QueryString("EndorsementPolicyImageToDelete") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyImageToDelete").ToString) = False Then
            '        Dim endorsementPolicyIdToDelete As Integer = 0
            '        Dim endorsementPolicyImageNumToDelete As Integer = 0

            '        Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(Request.QueryString("EndorsementPolicyImageToDelete").ToString, delimiter:="|", positiveOnly:=True)
            '        If intList IsNot Nothing AndAlso intList.Count > 0 Then
            '            endorsementPolicyIdToDelete = intList(0)
            '            If intList.Count > 1 Then
            '                endorsementPolicyImageNumToDelete = intList(1)
            '            End If
            '        End If

            '        If endorsementPolicyIdToDelete > 0 Then
            '            Dim endorsementDeleteSuccess As Boolean = False
            '            Dim endorsementDeleteErrorMessage As String = ""
            '            endorsementDeleteSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullyDeletedEndorsementQuote(endorsementPolicyIdToDelete, policyImageNum:=endorsementPolicyImageNumToDelete, errorMessage:=endorsementDeleteErrorMessage)
            '            If endorsementDeleteSuccess = False OrElse String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then

            '            End If
            '        End If
            '    End If
            'End If
            'updated 3/8/2019 for loading from here
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing Then
                Dim roPolicyId As Integer = 0
                Dim roPolicyImageNum As Integer = 0
                Dim roQQO As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
                Dim endPolicyId As Integer = 0
                Dim endPolicyImageNum As Integer = 0
                Dim endQQO As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
                Dim loadErrorMessage As String = "" 'added 7/11/2019
                Dim loadErrorTitle As String = "" 'added 7/11/2019

                'If Request.QueryString("NewEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewEndorsementPolicyId").ToString) > 0 Then
                '    Dim newEndorsementErrorMsg As String = ""
                '    Dim newEndorsementImageNum As Integer = 0
                '    Dim latestEndorsementImageNum As Integer = 0
                '    endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(CInt(Request.QueryString("NewEndorsementPolicyId").ToString), Date.Today.ToShortDateString, endorsementRemarks:="new endorsement for policyId " & CInt(Request.QueryString("NewEndorsementPolicyId").ToString).ToString, newPolicyImageNum:=newEndorsementImageNum, latestPendingEndorsementImageNum:=latestEndorsementImageNum, errorMessage:=newEndorsementErrorMsg)
                '    If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True AndAlso endQQO IsNot Nothing AndAlso newEndorsementImageNum > 0 Then
                '        endPolicyId = CInt(Request.QueryString("NewEndorsementPolicyId").ToString)
                '        endPolicyImageNum = newEndorsementImageNum
                '    ElseIf latestEndorsementImageNum > 0 Then
                '        endPolicyId = CInt(Request.QueryString("NewEndorsementPolicyId").ToString)
                '        endPolicyImageNum = latestEndorsementImageNum
                '    End If
                If (Request.QueryString("NewEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewEndorsementPolicyId").ToString) > 0) OrElse (Request.QueryString("NewBillingUpdateEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) > 0) Then
                    Dim policyIdToUse As Integer = 0
                    Dim billingUpdateFlagToUse As Boolean = False
                    If Request.QueryString("NewBillingUpdateEndorsementPolicyId") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) = False AndAlso IsNumeric(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) = True AndAlso CInt(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString) > 0 Then
                        policyIdToUse = CInt(Request.QueryString("NewBillingUpdateEndorsementPolicyId").ToString)
                        billingUpdateFlagToUse = True
                    Else
                        policyIdToUse = CInt(Request.QueryString("NewEndorsementPolicyId").ToString)
                    End If

                    Dim newEndorsementErrorMsg As String = ""
                    Dim newEndorsementImageNum As Integer = 0
                    Dim latestEndorsementImageNum As Integer = 0
                    endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.NewEndorsementQuoteForPolicyIdTransactionDate(policyIdToUse, Date.Today.ToShortDateString, endorsementRemarks:="new" & If(billingUpdateFlagToUse = True, " billing update", "") & " endorsement for policyId " & policyIdToUse.ToString, newPolicyImageNum:=newEndorsementImageNum, latestPendingEndorsementImageNum:=latestEndorsementImageNum, errorMessage:=newEndorsementErrorMsg, isBillingUpdate:=billingUpdateFlagToUse)
                    If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = True AndAlso endQQO IsNot Nothing AndAlso newEndorsementImageNum > 0 Then
                        endPolicyId = policyIdToUse
                        endPolicyImageNum = newEndorsementImageNum
                    ElseIf latestEndorsementImageNum > 0 Then
                        endPolicyId = policyIdToUse
                        endPolicyImageNum = latestEndorsementImageNum
                    End If
                    If String.IsNullOrWhiteSpace(newEndorsementErrorMsg) = False Then 'added 7/11/2019
                        loadErrorMessage = newEndorsementErrorMsg
                        loadErrorTitle = "Error Creating New Endorsement"
                    End If
                ElseIf Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString, delimiter:="|", positiveOnly:=True)
                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                        endPolicyId = intList(0)
                        If intList.Count > 1 Then
                            endPolicyImageNum = intList(1)
                        End If
                    End If
                ElseIf Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString, delimiter:="|", positiveOnly:=True)
                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                        roPolicyId = intList(0)
                        If intList.Count > 1 Then
                            roPolicyImageNum = intList(1)
                        End If
                    End If
                ElseIf Request.QueryString("EndorsementPolicyIdToDelete") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdToDelete").ToString) = False AndAlso IsNumeric(Request.QueryString("EndorsementPolicyIdToDelete").ToString) = True AndAlso CInt(Request.QueryString("EndorsementPolicyIdToDelete").ToString) > 0 Then
                    Dim endorsementDeleteSuccess As Boolean = False
                    Dim endorsementDeleteErrorMessage As String = ""
                    endorsementDeleteSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullyDeletedEndorsementQuote(CInt(Request.QueryString("EndorsementPolicyIdToDelete").ToString), errorMessage:=endorsementDeleteErrorMessage)
                    If endorsementDeleteSuccess = False OrElse String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then

                    End If
                    If String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then 'added 7/11/2019
                        loadErrorMessage = endorsementDeleteErrorMessage
                        loadErrorTitle = "Error Deleting Endorsement"
                    End If
                ElseIf Request.QueryString("EndorsementPolicyImageToDelete") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyImageToDelete").ToString) = False Then
                    Dim endorsementPolicyIdToDelete As Integer = 0
                    Dim endorsementPolicyImageNumToDelete As Integer = 0

                    Dim intList As List(Of Integer) = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfIntegerFromString(Request.QueryString("EndorsementPolicyImageToDelete").ToString, delimiter:="|", positiveOnly:=True)
                    If intList IsNot Nothing AndAlso intList.Count > 0 Then
                        endorsementPolicyIdToDelete = intList(0)
                        If intList.Count > 1 Then
                            endorsementPolicyImageNumToDelete = intList(1)
                        End If
                    End If

                    If endorsementPolicyIdToDelete > 0 Then
                        Dim endorsementDeleteSuccess As Boolean = False
                        Dim endorsementDeleteErrorMessage As String = ""
                        endorsementDeleteSuccess = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullyDeletedEndorsementQuote(endorsementPolicyIdToDelete, policyImageNum:=endorsementPolicyImageNumToDelete, errorMessage:=endorsementDeleteErrorMessage)
                        If endorsementDeleteSuccess = False OrElse String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then

                        End If
                        If String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) = False Then 'added 7/11/2019
                            loadErrorMessage = endorsementDeleteErrorMessage + Environment.NewLine + Environment.NewLine + "Please contact your Underwriter."
                            loadErrorTitle = "Error Deleting Endorsement"

                        End If

                        If endorsementDeleteSuccess AndAlso String.IsNullOrWhiteSpace(endorsementDeleteErrorMessage) Then
                            loadErrorTitle = "Success"
                            loadErrorMessage = "Your Endorsement transaction has been deleted as requested."
                        End If

                    End If
                End If

                '6/11/2019 - added new variables for updated logic
                Dim polIdToUse As Integer = 0
                Dim polImgNumToUse As Integer = 0
                Dim tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None
                Dim quoteStatus As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType = Nothing
                Dim isBillingUpdate As Boolean = False

                Dim lobTypeToUse As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None
                'Dim imgQueryString As String = "" 'removed 6/11/2019; logic now to be performed by helper method
                If endPolicyId > 0 AndAlso endPolicyImageNum > 0 Then
                    Dim endorsementLoadErrorMessage As String = "" 'added 7/11/2019
                    If endQQO Is Nothing Then
                        endQQO = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(endPolicyId, endPolicyImageNum, errorMessage:=endorsementLoadErrorMessage)
                    End If
                    If endQQO IsNot Nothing Then
                        lobTypeToUse = endQQO.LobType
                        'imgQueryString = "?EndorsementPolicyIdAndImageNum=" & endPolicyId.ToString & "|" & endPolicyImageNum.ToString 'removed 6/11/2019; logic now to be performed by helper method

                        'added 3/25/2019; removed 6/11/2019; logic now to be performed by helper method
                        'If System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType), endQQO.QuoteStatus) = True Then
                        '    Select Case endQQO.QuoteStatus
                        '        Case QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppGapRated, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.AppGapRatingFailed, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteRated, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteStatusType.QuoteRatingFailed
                        '            imgQueryString &= "&" & IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs & "=" & IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                        '    End Select
                        'End If

                        '6/11/2019 - new logic for helper method
                        polIdToUse = endPolicyId
                        polImgNumToUse = endPolicyImageNum
                        tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        quoteStatus = endQQO.QuoteStatus
                        isBillingUpdate = endQQO.Database_IsBillingUpdate
                    End If
                    If String.IsNullOrWhiteSpace(loadErrorMessage) = True AndAlso String.IsNullOrWhiteSpace(endorsementLoadErrorMessage) = False Then 'added 7/11/2019
                        loadErrorMessage = endorsementLoadErrorMessage
                        loadErrorTitle = "Error Loading Endorsement"
                    End If
                ElseIf roPolicyId > 0 AndAlso roPolicyImageNum > 0 Then
                    Dim readOnlyLoadErrorMessage As String = "" 'added 7/11/2019
                    If roQQO Is Nothing Then
                        roQQO = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(roPolicyId, roPolicyImageNum, errorMessage:=readOnlyLoadErrorMessage)
                    End If
                    If roQQO IsNot Nothing Then
                        lobTypeToUse = roQQO.LobType
                        'imgQueryString = "?ReadOnlyPolicyIdAndImageNum=" & roPolicyId.ToString & "|" & roPolicyImageNum.ToString 'removed 6/11/2019; logic now to be performed by helper method
                        '6/11/2019 - new logic for helper method
                        polIdToUse = roPolicyId
                        polImgNumToUse = roPolicyImageNum
                        tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                    End If
                    If String.IsNullOrWhiteSpace(loadErrorMessage) = True AndAlso String.IsNullOrWhiteSpace(readOnlyLoadErrorMessage) = False Then 'added 7/11/2019
                        loadErrorMessage = readOnlyLoadErrorMessage
                        loadErrorTitle = "Error Loading Image"
                    End If
                End If
                'If String.IsNullOrWhiteSpace(imgQueryString) = False AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType), lobTypeToUse) = True AndAlso lobTypeToUse <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None Then
                '    Dim useAppPage As Boolean = False
                '    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                '    If Request.QueryString("App") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("App").ToString) = False AndAlso (UCase(Request.QueryString("App").ToString) = "YES" OrElse qqHelper.BitToBoolean(Request.QueryString("App").ToString) = True) Then
                '        useAppPage = True
                '    End If

                '    Select Case lobTypeToUse
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                '            If useAppPage = True Then
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") & imgQueryString)
                '            Else
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") & imgQueryString, True)
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                '            If useAppPage = True Then
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") & imgQueryString)
                '            Else
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") & imgQueryString, True)
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                '            If useAppPage = True Then
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") & imgQueryString)
                '            Else
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") & imgQueryString, True)
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                '            If useAppPage = True Then
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") & imgQueryString)
                '            Else
                '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") & imgQueryString, True)
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                '            If IFM.VR.Common.VRFeatures.NewLookBOPEnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3BOPApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                '            If IFM.VR.Common.VRFeatures.NewLookWCPEnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3WCPApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                '            If IFM.VR.Common.VRFeatures.NewLookCAPEnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3CAPApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                '            If IFM.VR.Common.VRFeatures.NewLookCGLEnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3CGLApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                '            If IFM.VR.Common.VRFeatures.NewLookCPREnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3CPRApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                '            If IFM.VR.Common.VRFeatures.NewLookCPPEnabled Then
                '                If useAppPage = True Then
                '                    Response.Redirect("VR3CPPApp.aspx" & imgQueryString)
                '                Else
                '                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") & imgQueryString, True)
                '                End If
                '            End If
                '    End Select
                'End If
                'updated 6/11/2019 to use new helper method
                If polIdToUse > 0 AndAlso polImgNumToUse > 0 Then
                    Dim useAppPage As Boolean = False
                    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    If Request.QueryString("App") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("App").ToString) = False AndAlso (UCase(Request.QueryString("App").ToString) = "YES" OrElse qqHelper.BitToBoolean(Request.QueryString("App").ToString) = True) Then
                        useAppPage = True
                    End If
                    Dim workflowQueryString As String = ""
                    If Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs).ToString) = False Then
                        workflowQueryString = Request.QueryString(IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs).ToString
                    End If

                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType, lobTypeToUse, policyId:=polIdToUse, policyImageNum:=polImgNumToUse, quoteStatus:=quoteStatus, goToApp:=useAppPage, workflowQueryString:=workflowQueryString, isBillingUpdate:=isBillingUpdate)
                Else 'added 7/11/2019
                    If String.IsNullOrWhiteSpace(loadErrorMessage) = False Then
                        'Me.ValidationHelper.AddError(loadErrorMessage)
                        If String.IsNullOrWhiteSpace(loadErrorTitle) = True Then
                            loadErrorTitle = "Processing Error"
                        End If
                        Using popup As New PopupMessageObject(Page, loadErrorMessage, loadErrorTitle)
                            With popup
                                .isFixedPositionOnScreen = True
                                .ZIndexOfPopup = 2
                                .isModal = True
                                .Image = PopupMessageObject.ImageOptions.None
                                .hideCloseButton = True
                                .AddButton("OK", True)
                                '.AddButton("OK", True, "", System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage"))
                                ' Foward to home if titlebar "X" is visible
                                '.AddPopupEvent(PopupMessageObject.PopupEventType.close, "window.location.replace('" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "')")
                                .CreateDynamicPopUpWindow()
                            End With
                        End Using
                    End If
                End If
            End If


            '*************** Feature Flags ***************
            If System.Configuration.ConfigurationManager.AppSettings("CPP_Endorsements") IsNot Nothing Then
                Dim chc = New CommonHelperClass
                If chc.ConfigurationAppSettingValueAsBoolean("CPP_Endorsements") Then
                    Dim li = New HtmlGenericControl("li")
                    li.InnerHtml = "Commercial CPP"
                    Me.EndorsementCapableLOBs.Controls.Add(li)
                End If
            End If

            'Added 05/05/2022 for Task 68026 BD
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If qqh.BitToBoolean(System.Configuration.ConfigurationManager.AppSettings("Task68026_CommercialBOP")) Then
                liCommercialBOP.Visible = True
                liCommBopEndSoon.Visible = False
            Else
                'Added 03/23/2022 for Task 74075 MLW
                If qqh.BitToBoolean(System.Configuration.ConfigurationManager.AppSettings("Task74075_BOPEndComingSoon")) Then
                    liCommBopEndSoon.Visible = True
                Else
                    liCommBopEndSoon.Visible = False
                End If
                liCommercialBOP.Visible = False
            End If

            SetPageDefaults()
        End If
        SetPageView()
    End Sub

    'Private Sub SetLOBTextForMainButtons()
    '    Dim availableLOBs As String = ""

    '    If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForString("VR_LOBsReadyForEndorsements", availableLOBs) Then
    '        Dim LOBs As List(Of String) = availableLOBs.CSVtoList
    '        Dim personalLines As New List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
    '        For Each lob As String In LOBs
    '            Dim thisLObType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
    '            thisLObType = IFM.VR.Common.Helpers.LOBHelper.GetLobTypeFromAbbreviatedPrefix(lob)
    '            'Dim lobhelp As New IFM.VR.Common.Helpers.LOBHelper()
    '            'IFM.VR.Web.Helpers.WebHelper_Personal.
    '        Next
    '    End If
    'End Sub



    Private Sub ctlQuoteSearch_SearchRequested(searchParameters As Common.QuoteSearch.QQSearchParameters) Handles ctlQuoteSearch.SearchRequested
        If searchParameters.searchInitiatedByAgencySwitch Then
            If hdnMyVRView IsNot Nothing AndAlso hdnMyVRView.Value.NoneAreNullEmptyorWhitespace() AndAlso hdnMyVRView.Value.Equals("Splash", StringComparison.OrdinalIgnoreCase) = False Then
                Search(searchParameters, True)
            End If
        Else
            Search(searchParameters, True)
        End If
    End Sub

    Private pageSw As New Stopwatch

    Private Sub Page_PreInit() Handles Me.PreInit
#If DEBUG Then
        pageSw.Start()
        Debug.WriteLine("MyVelocirater - PreInit")
#End If
    End Sub

    Private Sub Page_LoadComplete() Handles Me.LoadComplete
#If DEBUG Then
        Debug.WriteLine("MyVelocirater - LoadComplete")
#End If
    End Sub

    Private Sub Page_PreRender() Handles Me.PreRender
#If DEBUG Then
        Debug.WriteLine("MyVelocirater - PreRender")
#End If
    End Sub

    Private Sub Page_PreRenderComplete() Handles Me.PreRenderComplete
#If DEBUG Then
        Debug.WriteLine("MyVelocirater - PreRenderComplete")
#End If
    End Sub

    Private Sub Page_Unload() Handles Me.Unload
#If DEBUG Then
        pageSw.Stop()
        Debug.WriteLine("MyVelocirater - Unload - Page Response Time: " & pageSw.ElapsedMilliseconds)
#End If
    End Sub
    Private Sub Search(searchParameters As Common.QuoteSearch.QQSearchParameters, Optional SearchRequested As Boolean = False)
        Dim displays = New List(Of ctlQuoteSearchResults)
        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        Dim results As List(Of Common.QuoteSearch.QQSearchResult)
        Dim useSingleSQLQuery As Boolean = False

        displays.Add(ctlQuoteSearchResultsAuto)
        displays.Add(ctlQuoteSearchResultsHome)
        displays.Add(ctlQuoteSearchResultsFarm)
        displays.Add(ctlQuoteSearchResultsComm)
        displays.Add(ctlQuoteSearchResultsUmbrella)

        searchParameters.CurrentDiamondUserID = CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))
        searchParameters.IsStaff = DirectCast(Me.Page.Master, VelociRater).IsStaff
        searchParameters.HasEmployeeAccess = qqHelper.CanUserAccessEmployeePolicies()
        searchParameters.AgencyID = DirectCast(Me.Page.Master, VelociRater).AgencyID
        searchParameters.isGlobalSearch = True
        searchParameters.excludeSavedQuotes = False 'added 3/9/2021

        'added 3/9/2021; original logic in ELSE
        Dim validationErrorMsg As String = ""
        If (searchParameters.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes OrElse searchParameters.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.All) AndAlso searchParameters.AgencyID > 0 Then
            If qqHelper.IsAgencyIdInCancelledSessionList(searchParameters.AgencyID.ToString) = True Then
                If searchParameters.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes Then
                    Dim cancelledAgencyText As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.CancelledAgencyBaseText()
                    validationErrorMsg = "New Business Quoting is not permitted on " & cancelledAgencyText & " agencies."
                Else
                    searchParameters.excludeSavedQuotes = True
                End If
            End If
        End If
        If String.IsNullOrWhiteSpace(validationErrorMsg) = False Then
            Using popup As New PopupMessageObject(Page, validationErrorMsg, "Search Validation")
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

            'adding binding logic w/ forceEmptyLoad so the styles don't get messed up
            If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_UseSingleSQLQuery", useSingleSQLQuery) AndAlso useSingleSQLQuery = True Then
                Dim newLobList As New List(Of Integer)
                Dim newNonEndorseLobList As New List(Of Integer)

                'Load the user controls and get the LobList and NonEndorsementLobList from each control
                If searchParameters IsNot Nothing Then
                    For Each d In displays
                        If SearchRequested = True Then
                            d.PageResults = Nothing
                        End If

                        d.SearchParameters = searchParameters.Clone()

                        newLobList.AddRange(d.SearchParameters.LobIDsList)
                        newNonEndorseLobList.AddRange(d.SearchParameters.NonEndorsementReadyLobIds)
                    Next
                End If

                'Now load up our search Parameters with the full lob and NonEndorsementReadyLobs
                searchParameters.LobIDsList = newLobList
                searchParameters.NonEndorsementReadyLobIds = newNonEndorseLobList

                'Query for all results with a single database call
                'results = VR.Common.QuoteSearch.QuoteSearch.SearchBySearchParameters(searchParameters)
                results = New List(Of Common.QuoteSearch.QQSearchResult)

                'If it is a "saved page", only display the top 16 results
                If searchParameters.ShowAllOnPage = False AndAlso searchParameters.isSavedPage = True Then
                    searchParameters.NumberOfResultsToDisplay = 16
                End If

                'Load each control with the appropriate results for that control
                For Each myControl As ctlQuoteSearchResults In displays
                    If searchParameters.NumberOfResultsToDisplay > 0 Then
                        myControl.PageResults = results.FindAll(Function(x) myControl.SearchParameters.LobIDsList.Contains(x.LobId) OrElse myControl.SearchParameters.NonEndorsementReadyLobIds.Contains(x.LobId)).Take(searchParameters.NumberOfResultsToDisplay).ToList()
                    Else
                        myControl.PageResults = results.FindAll(Function(x) myControl.SearchParameters.LobIDsList.Contains(x.LobId) OrElse myControl.SearchParameters.NonEndorsementReadyLobIds.Contains(x.LobId))
                    End If
                    myControl.Populate(forceEmptyLoad:=True)
                Next
            Else
                'Old search method which queries the database once for each control populated
                If searchParameters IsNot Nothing Then
                    For Each d In displays
                        If SearchRequested = True Then
                            d.PageResults = Nothing
                        End If
                        d.SearchParameters = searchParameters
                        d.Populate(forceEmptyLoad:=True)
                    Next
                End If
            End If
        Else
            If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_UseSingleSQLQuery", useSingleSQLQuery) AndAlso useSingleSQLQuery = True Then
                Dim newLobList As New List(Of Integer)
                Dim newNonEndorseLobList As New List(Of Integer)

                'Load the user controls and get the LobList and NonEndorsementLobList from each control
                If searchParameters IsNot Nothing Then
                    For Each d In displays
                        If SearchRequested = True Then
                            d.PageResults = Nothing
                        End If

                        d.SearchParameters = searchParameters.Clone()

                        newLobList.AddRange(d.SearchParameters.LobIDsList)
                        newNonEndorseLobList.AddRange(d.SearchParameters.NonEndorsementReadyLobIds)
                    Next
                End If

                'Now load up our search Parameters with the full lob and NonEndorsementReadyLobs
                searchParameters.LobIDsList = newLobList
                searchParameters.NonEndorsementReadyLobIds = newNonEndorseLobList

                'Query for all results with a single database call
                results = VR.Common.QuoteSearch.QuoteSearch.SearchBySearchParameters(searchParameters)

                'If it is a "saved page", only display the top 16 results
                If searchParameters.ShowAllOnPage = False AndAlso searchParameters.isSavedPage = True Then
                    searchParameters.NumberOfResultsToDisplay = 16
                End If

                'Load each control with the appropriate results for that control
                For Each myControl As ctlQuoteSearchResults In displays
                    If searchParameters.NumberOfResultsToDisplay > 0 Then
                        myControl.PageResults = results.FindAll(Function(x) myControl.SearchParameters.LobIDsList.Contains(x.LobId) OrElse myControl.SearchParameters.NonEndorsementReadyLobIds.Contains(x.LobId)).Take(searchParameters.NumberOfResultsToDisplay).ToList()
                    Else
                        myControl.PageResults = results.FindAll(Function(x) myControl.SearchParameters.LobIDsList.Contains(x.LobId) OrElse myControl.SearchParameters.NonEndorsementReadyLobIds.Contains(x.LobId))
                    End If
                    myControl.Populate()
                Next
            Else
                'Old search method which queries the database once for each control populated
                If searchParameters IsNot Nothing Then
                    For Each d In displays
                        If SearchRequested = True Then
                            d.PageResults = Nothing
                        End If
                        d.SearchParameters = searchParameters
                        d.Populate()
                    Next
                End If
            End If
        End If

        If SearchRequested = True AndAlso searchParameters.searchInitiatedByAgencySwitch = False Then
            Dim themeBasedOffSearchType As Boolean = False
            If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_EndorsementSplashPageThemingBasedOffSearchType", themeBasedOffSearchType) AndAlso themeBasedOffSearchType = True Then
                'This code would keep the theming based on the search type. However, at this time, it was decided to revert to the grey ("ReadOnly") theme after any search regardless of page or search type.
                Select Case searchParameters.SearchType
                    Case Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates
                        hdnMyVRView.Value = "billingUpdates"
                    Case Common.QuoteSearch.QuoteSearch.SearchType.Changes
                        hdnMyVRView.Value = "savedChanges"
                    Case Common.QuoteSearch.QuoteSearch.SearchType.Policies
                        hdnMyVRView.Value = "globalSearch"
                    Case Common.QuoteSearch.QuoteSearch.SearchType.Quotes
                        hdnMyVRView.Value = "savedQuotes"
                    Case Common.QuoteSearch.QuoteSearch.SearchType.All
                        hdnMyVRView.Value = "globalSearch"
                End Select
            Else
                hdnMyVRView.Value = "globalSearch"
            End If

            SetPageView()
        End If
    End Sub

    Private Sub ImportDiamondQuote(diamondQuoteNumber As String)
        If diamondQuoteNumber.IsNullEmptyorWhitespace = False Then
            Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML()
            Dim successfullyLoadedIntoVR As Boolean = False
            Dim quoteNum As String = diamondQuoteNumber.ToUpper()
            Dim newOrExistingQuoteId As Integer = 0
            Dim loadIntoVrErrorMsg As String = ""

            Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

            successfullyLoadedIntoVR = QQxml.SuccessfullyLoadedDiamondQuoteIntoVelociRater(quoteNum, newOrExistingQuoteId, newQuote, loadIntoVrErrorMsg)
            If successfullyLoadedIntoVR = True AndAlso String.IsNullOrEmpty(loadIntoVrErrorMsg) = True AndAlso newOrExistingQuoteId > 0 AndAlso newQuote IsNot Nothing Then
                'success; should be able to just check for success True and quoteId > 0
                IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(newQuote.LobId), quoteId:=newQuote.Database_QuoteId, quoteStatus:=newQuote.Database_QuoteStatusId, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False, AdditionalQueryStringParams:="&showNowInVr=true")
                'Response.Redirect(IFM.VR.Common.QuoteSearch.QQSearchResult.GetViewUrl(newQuote.LobId, "2", newQuote.Database_QuoteId) + "&showNowInVr=true", True) 'default status to quote started Matt A 5/24/2016
            Else
                'see error message
            End If

        Else
            'nothing to do

        End If
    End Sub

    Private Sub SetPageView()
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        Select Case hdnMyVRView.Value
            Case "splash"
                dvSplash.Visible = True
                dvSearchResults.Visible = False
            Case "savedQuotes"
                _script.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""NewBusinessQuote"");", True)
                dvSplash.Visible = False
                dvSearchResults.Visible = True
            Case "savedChanges"
                _script.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""Endorsement"");", True)
                dvSplash.Visible = False
                dvSearchResults.Visible = True
            Case "billingUpdates"
                _script.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""PayplanChange"");", True)
                dvSplash.Visible = False
                dvSearchResults.Visible = True
            Case "globalSearch"
                _script.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(""ReadOnly"");", True)
                dvSplash.Visible = False
                dvSearchResults.Visible = True
        End Select
    End Sub

    Private Sub SetPageDefaults()
        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        Dim searchParams As New Common.QuoteSearch.QQSearchParameters

        searchParams.CurrentDiamondUserID = CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))
        searchParams.IsStaff = DirectCast(Me.Page.Master, VelociRater).IsStaff
        searchParams.HasEmployeeAccess = qqHelper.CanUserAccessEmployeePolicies()
        searchParams.AgencyID = DirectCast(Me.Page.Master, VelociRater).AgencyID

        Select Case hdnMyVRView.Value
            Case "splash"
            Case "savedQuotes"
                searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes
                searchParams.isSavedPage = True
                searchParams.TimeFrame = 90
                Search(searchParams)
            Case "savedChanges"
                searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Changes
                searchParams.isSavedPage = True
                Search(searchParams)
            Case "billingUpdates"
                searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates
                searchParams.isSavedPage = True
                Search(searchParams)
            Case "globalSearch"

        End Select
    End Sub

    Private Sub SetVRViewFromRequest()
        Dim chc As New CommonHelperClass
        hdnMyVRView.Value = "splash"
        If Me.Request("PageView").IsNotNull Then
            Select Case True
                Case chc.StringsAreEqual(Me.Request("PageView"), "savedQuotes")
                    hdnMyVRView.Value = "savedQuotes"
                Case chc.StringsAreEqual(Me.Request("PageView"), "savedChanges")
                    hdnMyVRView.Value = "savedChanges"
                Case chc.StringsAreEqual(Me.Request("PageView"), "billingUpdates")
                    hdnMyVRView.Value = "billingUpdates"
            End Select
        End If
    End Sub
End Class