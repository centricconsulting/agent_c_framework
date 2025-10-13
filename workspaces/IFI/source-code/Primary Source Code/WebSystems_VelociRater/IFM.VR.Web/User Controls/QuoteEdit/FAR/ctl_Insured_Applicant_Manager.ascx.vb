Imports QuickQuote.CommonMethods
Public Class ctl_Insured_Applicant_Manager
    Inherits VRControlBase

    Public Event QuoteRateRequested()

    Public Property ActiveInsuredIndex As String
        Get
            Return Me.ctlIsuredList.ActiveInsuredIndex
        End Get
        Set(value As String)
            Me.ctlIsuredList.ActiveInsuredIndex = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Quote IsNot Nothing AndAlso Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing AndAlso Me.Quote.Policyholder.Name.TypeId = "2" Then
            Me.btnSave.Text = "Save Policyholders and Applicants"
        Else
            Me.btnSave.Text = "Save Policyholders"
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.ctlIsuredList.Visible Then
            Me.ctlIsuredList.ValidateControl(valArgs)
        End If

        If Me.ctlApplicantList.Visible Then
            Me.ctlApplicantList.ValidateControl(valArgs)
        End If
    End Sub

    Public Overrides Sub Populate()
        Me.bnAddApplicant.Visible = Me.ctlApplicantList.CanAddApplicant
        btnRate.Visible = False
        btnRate.Enabled = False
        btnRate_Endorsements.Visible = False
        btnRate_Endorsements.Enabled = False

        If Me.Quote IsNot Nothing Then
            If IsQuoteReadOnly() Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                'QuickQuoteHelperClass.configAppSettingValueAsString("")  'Unused CAH 07/21/2020
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True


                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
                If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                    btnRate_Endorsements.Visible = True
                End If
            Else
                If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                    btnRate.Visible = True
                    btnRate.Enabled = True
                End If
            End If
        End If

        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.ctlIsuredList.Visible Then
            Me.ctlIsuredList.Save()
            Me.ctlIsuredList.Populate()
        End If
        If Me.ctlApplicantList.Visible Then
            Me.ctlApplicantList.Save()
        End If

        Return True
    End Function

    Protected Sub bnAddApplicant_Click(sender As Object, e As EventArgs) Handles bnAddApplicant.Click
        Me.ctlApplicantList.AddApplicant()
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click, btnSaveAndGotoNextPage.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(Me.DefaultValidationType)))

        If ValidationSummmary.HasErrors = False Then
            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then ' Matt A -  8-6-15
                Me.ctl_OrderClueAndOrMVR.LoadReport(Web.ctl_OrderClueAndOrMVR.ReportRequestType.credit)
            End If

            If sender Is btnSaveAndGotoNextPage Then
                Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
            End If
        End If

    End Sub

    Private Sub ctl_OrderClueAndOrMVR_BroadcastGenericEvent(type As BroadCastEventType) Handles ctl_OrderClueAndOrMVR.BroadcastGenericEvent
        If type = BroadCastEventType.ThirdPartyReportOrdered Then
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    'Added 1/8/2019 for Bug 30790 MLW
    Private Sub ctlIsuredList_AboutToPopulateForStateChange(isFarmWithCommercialName As Boolean) Handles ctlIsuredList.AboutToPopulateForStateChange
        'RaiseEvent AboutToPopulateForStateChange(isFarmWithCommercialName)
        If isFarmWithCommercialName = True Then
            ctlApplicantList.Save()
        End If
    End Sub

    Private Sub ctl_OrderClueAndOrMVR_JustFinishedCreditOrder() Handles ctl_OrderClueAndOrMVR.JustFinishedCreditOrder 'added 9/27/2019
        Me.ctlIsuredList.SetClientIdTextboxFromQuoteIfNeeded()
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoDrivers_Click(sender As Object, e As EventArgs) Handles btnViewGotoDrivers.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
    End Sub

    Private Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click, btnRate.Click
        RaiseEvent QuoteRateRequested()
    End Sub
End Class