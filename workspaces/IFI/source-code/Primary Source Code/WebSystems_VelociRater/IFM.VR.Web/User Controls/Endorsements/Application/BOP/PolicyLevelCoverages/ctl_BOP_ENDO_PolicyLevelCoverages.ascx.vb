Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_ENDO_PolicyLevelCoverages
    Inherits VRControlBase


    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()
    Public Event AIChanged()
    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If IsQuoteReadOnly() Then
            Dim policyNumber As String = Me.Quote.PolicyNumber
            Dim imageNum As Integer = 0
            Dim policyId As Integer = 0
            Dim toolTip As String = "Make a change to this policy"
            Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
            If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
            End If

            divActionButtons.Visible = False
            divEndorsementButtons.Visible = True
            btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
            readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
            btnMakeAChange.ToolTip = toolTip
            btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_Endo_VehicleAdditionalInterestList.AIChange, AddressOf HandleAIChange
        AddHandler Me.ctl_Endo_VehicleAdditionalInterestList.UpdateTransactionReasonType, AddressOf UpdateTransactionReasonType
        AttachControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                Me.SaveChildControls()
        End Select

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub HandleEffectiveDateChange(ByVal NewDate As String, ByVal OldDate As String)
        Exit Sub
    End Sub
    Protected Sub AttachControlEvents()
        AddHandler ctl_Endo_VehicleAdditionalInterestList.AIChange, AddressOf HandleAIChange
        AddHandler ctl_Endo_VehicleAdditionalInterestList.UpdateTransactionReasonType, AddressOf UpdateTransactionReasonType
    End Sub

    Protected Sub UpdateTransactionReasonType(ddh As DevDictionaryHelper.DevDictionaryHelper)
        Quote.TransactionReasonId = 10168

    End Sub

    Protected Sub HandleAIChange()
        RaiseEvent AIChanged()
    End Sub

    Private Sub btnRatePolicyholder_Click(sender As Object, e As EventArgs) Handles btnRatePolicyholder.Click
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' fires Base save requested event
    End Sub

End Class