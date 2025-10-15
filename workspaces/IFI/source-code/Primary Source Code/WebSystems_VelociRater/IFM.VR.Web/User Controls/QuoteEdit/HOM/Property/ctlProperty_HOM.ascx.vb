Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports IFM.PrimativeExtensions

Public Class ctlProperty_HOM
    Inherits VRControlBase

    Public Event ResidenceChanged()
    Public Event PolicyHolderReloadNeeded()

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.PolicyInfoDiv.ClientID
        AddHandler Me.ctlPropertyAdditionalQuestions.PolicyholderReloadNeeded, AddressOf HandlePHReloadRequest

        If Not IsPostBack Then
            If Me.Quote IsNot Nothing Then
                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                    Me.ctlMobileHome.Visible = False
                    Me.ctlPropertyAdditionalQuestions.Visible = False
                End If
            End If
        End If
    End Sub

    Private Sub HandlePHReloadRequest()
        RaiseEvent PolicyHolderReloadNeeded()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddVariableLine(String.Format("var locationHeader_{0} = ""{1}"";", MyLocationIndex, Me.lblMainHeader.ClientID)) 'used to set the address text in this header - used by residence_address control
        Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenField1, "0", False)

        Me.VRScript.StopEventPropagation(Me.lnkClearGeneralInfo.ClientID, False)
        Me.VRScript.CreateConfirmDialog(Me.lnkClearGeneralInfo.ClientID, "Clear Property Page?")

        Me.VRScript.StopEventPropagation(Me.lnkSaveGeneralInfo.ClientID)
        Me.VRScript.CreateJSBinding(Me.lnkSaveGeneralInfo, ctlPageStartupScript.JsEventType.onclick, "DisableFormOnSaveRemoves();")

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Me.VRScript.AddTabIndexControl(Me.ctlProperty_Address.CountyClientID, True, False)
            Me.VRScript.AddTabIndexControl(Me.ctlResidence.YearBuiltID, False, True)
            Me.VRScript.AddTabIndexControl(Me.ctlResidence.UsageTypeID, True, False)
            Me.VRScript.AddTabIndexControl(Me.ctlProtectionClass_HOM.ProtectionClassID, True, True)
            Me.VRScript.AddTabIndexControl("input[id*='txtFeetToHydrant']", False, False)
            Me.VRScript.AddTabIndexControl("input[id*='txtMilesToFireDepartment']", True, False)
            Me.VRScript.AddTabIndexControl("select[id*='ddLostType']", False, True)
            Me.VRScript.AddTabIndexControl("input[id*='txtAmountOfLoss']", True, False)
            Me.VRScript.AddTabIndexControl(Me.btnSubmit.ClientID, False, True)

            Me.VRScript.BindTabIndexControls()
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.MyLocation IsNot Nothing Then

            Me.lblMainHeader.Text = String.Format("Property - {0} {1} {2}", Me.MyLocation.Address.HouseNum, Me.MyLocation.Address.StreetName, Me.MyLocation.Address.City)
            Me.lblMainHeader.Text = Me.lblMainHeader.Text.Ellipsis(70)

            'Updated 09/17/2019 for DFR Endorsements tasks 40277, 40272 MLW
            If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) AndAlso (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal) Then
                Me.ctl_AdditionalInterest_MiniSerach.Visible = True
                Me.ctlVehicleAdditionalInterestList.Visible = True
                Me.ctl_Billing_Info_PPA.Visible = True
            End If

            'Added for Home Endorsements Project Task 38924 MLW
            If Me.IsQuoteReadOnly Then
                Me.ctl_Billing_Info_PPA.Visible = False 'Added 10/04/2019 for Home & DFR Endorsements tasks 38924 and 40277
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QQHelper.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
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


            If IsQuoteEndorsement() Then
                Me.btnRateHome.Visible = True
            Else
                Me.btnRateHome.Visible = False
            End If

            Me.PopulateChildControls()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Property"
        'Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        'Added for Home Endorsements Project Task 38924 MLW
        If Not Me.IsQuoteReadOnly Then
            If Me.MyLocation IsNot Nothing Then
                Select Case Me.MyLocation.FormTypeId
                    Case "1", "2", "3", "4", "5", "22", "23", "24", "26" 'updated 11/15/17 for HOM Upgrade to include 23, 24, 26 MLW 'updated 2/9/18 to include 22 MLW
                        Me.MyLocation.ProgramTypeId = "1"
                    Case "6", "7"
                        'MOBILE HOME
                        Me.MyLocation.ProgramTypeId = "2"
                    Case "8", "9", "10", "11", "12"
                        'DWELLING FIRE
                        Me.MyLocation.ProgramTypeId = "3"
                    Case "25" 'added 11/15/17 for HOM Upgrade MLW
                        'home and mobile
                        If Me.MyLocation.StructureTypeId = "2" Then
                            Me.MyLocation.ProgramTypeId = "2"
                        Else
                            Me.MyLocation.ProgramTypeId = "1"
                        End If
                End Select

                Me.SaveChildControls()
            End If
        End If
        Return True
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, lnkSaveGeneralInfo.Click, btnSaveGotoNextSection.Click
        'all saves call this event sub so keep it generic
        Me.Save_FireSaveEvent(True)
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
            Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
        ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
        Else
            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId)
        End If
        Me.Populate()
        If sender Is btnSaveGotoNextSection And Me.ValidationSummmary.HasErrors() = False Then
            ' goto next page
            Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
        End If

    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoNextSection_Click(sender As Object, e As EventArgs) Handles btnViewGotoNextSection.Click
        'Added 7/11/2019 for Home Endorsements Project Task 38924 MLW
        'If sender Is btnViewGotoNextSection Then
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.coverages, "0")
        'End If
    End Sub

    Protected Sub lnkClearGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkClearGeneralInfo.Click
        Me.ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    Public Overrides Sub EffectiveDateChanged(ByVal NewDt As String, ByVal OldDt As String)
        Me.ctlProtectionClass_HOM.EffectiveDateChanged(NewDt, OldDt)
    End Sub

    Private Sub btnRateHome_Click(sender As Object, e As EventArgs) Handles btnRateHome.Click
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub
End Class