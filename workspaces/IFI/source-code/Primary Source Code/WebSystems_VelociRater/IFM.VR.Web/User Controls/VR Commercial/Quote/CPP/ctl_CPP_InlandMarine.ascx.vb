Imports IFM.VR.Common.Helpers.CPP

Public Class ctl_CPP_InlandMarine
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        cov_BuildersRisk.IM_Parent = Me

        'Added 12/09/2021 for CPP Endorsements task 67310 MLW
        If IsQuoteReadOnly() Then
            divActionButtons.Visible = False
            divEndorsementButtons.Visible = True
            ctl_AdditionalInterestList.Visible = True
            ctl_IM_AssignedAIList.Visible = True 'Added 2/4/2022 for CPP Endorsements task 67310 MLW

            Dim policyNumber As String = Me.Quote.PolicyNumber
            Dim imageNum As Integer = 0
            Dim policyId As Integer = 0
            Dim toolTip As String = "Make a change to this policy"
            Dim readOnlyViewPageUrl As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
            If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
            End If

            btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
            readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
            btnMakeAChange.ToolTip = toolTip
            btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
        Else
            divActionButtons.Visible = True
            divEndorsementButtons.Visible = False
            ctl_AdditionalInterestList.Visible = False
            ctl_IM_AssignedAIList.Visible = False 'Added 2/4/2022 for CPP Endorsements task 67310 MLW
        End If

        Me.PopulateChildControls()
        Me.CheckHasPackage()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)
        Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID, False)
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#InlandMarineDiv"").accordion({collapsible: false, heightStyle: ""content""});")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub EffectiveDateChanged(qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String)
        cov_Transportation.HandleEffectiveDateChange(qqTranType, newEffectiveDate, oldEffectiveDate)
        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        cov_BuildersRisk.IM_Parent = Me
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            sq.CPP_Has_InlandMarine_PackagePart = False
        Next
        Me.CheckHasPackage()
        Me.SaveChildControls()
        Populate()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        cov_BuildersRisk.IM_Parent = Me
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            sq.CPP_Has_InlandMarine_PackagePart = False
        Next
        Me.ClearChildControls()
        PopulateChildControls()
        Save_FireSaveEvent()
        Populate()
    End Sub

    Public Sub Handle_DeleteIM_Request()
        ' Handle the Inland Marine delete request from the treeview
        Me.lnkClear_Click(Me, New EventArgs())
    End Sub

    Public Sub CheckHasPackage()

        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            If i.hasSetting() Then
                ' TODO: Does this need to save to all the subquotes?  Chad?
                GoverningStateQuote.CPP_Has_InlandMarine_PackagePart = True

                Return
            End If
        Next
    End Sub

    Public Function CheckHasOwnersOrTransportaion() As Boolean
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
                If i.hasSetting() AndAlso (i Is cov_OwnersCargo OrElse i Is cov_Transportation OrElse i Is cov_MotorTruckCargo) Then
                    Return True
                End If
            Else
                If i.hasSetting() AndAlso (i Is cov_OwnersCargo OrElse i Is cov_Transportation) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click, lnkSave.Click
        Me.Save_FireSaveEvent()
        Populate()
        If sender.Equals(btnRate) Then
            Session("CPPCPRCheckACVEventTrigger") = "RateButton"
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnToCrime.Click
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.Crime, "")
        End If
    End Sub

    Public Sub ClearTransportation()
        cov_Transportation.ClearControl()
    End Sub
    Public Sub ReloadTransportation()
        cov_Transportation.Populate()
    End Sub

    'Added 12/09/2021 for CPP Endorsements task 67310 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 12/09/2021 for CPP Endorsements task 67310 MLW
    Protected Sub btnViewCrime_Click(sender As Object, e As EventArgs) Handles btnViewCrime.Click
        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.Crime, "")
    End Sub
End Class