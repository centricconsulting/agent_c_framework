
Public Class ctl_CGL_PolicyLevelCoverages
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub


    Public Overrides Sub LoadStaticData()

    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_CGL_Coverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            Select Case Quote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    btnSubmit.Text = "Save GL Policy Level Coverages"
                    btnSaveAndGotoLocations.Text = "Location"
                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    btnSubmit.Text = "Save Policy Level Coverages"
                    btnSaveAndGotoLocations.Text = "Locations Page"
                    Exit Select
            End Select

            'Added 12/6/2021 for CPP Endorsements Task 66567 MLW
            If IsQuoteReadOnly() Then
                divActionButtons.Visible = False
                divEndorsementButtons.Visible = True

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
            End If
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Populate()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnSaveAndGotoLocations.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnSaveAndGotoLocations) Then
            If Me.ValidationSummmary.HasErrors = False Then
                Select Case Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, "")
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.location, "")
                        Exit Select
                End Select
            End If
        End If
    End Sub

    'Added 12/6/2021 for CPP Endorsements task 66567 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 12/6/2021 for CPP Endorsements task 66567 MLW
    Protected Sub btnViewGLLocations_Click(sender As Object, e As EventArgs) Handles btnViewGLLocations.Click
        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CGL_Locations, "")
    End Sub
End Class