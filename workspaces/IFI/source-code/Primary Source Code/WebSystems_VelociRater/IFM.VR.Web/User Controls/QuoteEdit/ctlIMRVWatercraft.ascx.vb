Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation

Public Class ctlIMRVWatercraft
    Inherits VRControlBase

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property ActiveRVWaterIndex As String
        Get
            Return Me.hiddenRVWatercraft.Value
        End Get
        Set(value As String)
            Me.hiddenRVWatercraft.Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MainAccordionDivId = dvInitRVWatercraft.ClientID

        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(MainAccordionDivId, hiddenRVWatercraft, "false", True)
        VRScript.StopEventPropagation(lnkAddRVWater.ClientID)

        '' Checks to see if this is a Commercial Quote
        ''Added 9/5/18 for multi state MLW
        'Dim LOID As String = Quote.LiabilityOptionId
        'If SubQuoteFirst IsNot Nothing Then
        '    LOID = SubQuoteFirst.LiabilityOptionId
        'Else
        '    LOID = Quote.LiabilityOptionId
        'End If
        ''Updated 9/5/18 for multi state MLW
        ''If Quote.LiabilityOptionId = "2" Then
        'If LOID = "2" Then
        '    If MyLocation.SectionIICoverages IsNot Nothing Then
        '        If MyLocation.SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9).Count > 0 Then
        '            ' Inland Marine and RV/Watercraft is available
        '            ctlInlandMarine.ActiveIMHeader = True
        '            RefreshRVWatercraft()
        '            lnkAddRVWater.Visible = True
        '            lblUnavailable.Visible = False
        '        Else
        '            ' Inland Marine and RV/Watercraft is NOT available
        '            ctlInlandMarine.ActiveIMHeader = False
        '            dvInitRVWatercraft.Visible = True
        '            ctlRV_WatercraftList.Visible = False
        '            lnkAddRVWater.Visible = False
        '            lblUnavailable.Visible = True
        '        End If
        '    Else
        '        ctlInlandMarine.ActiveIMHeader = False
        '        dvInitRVWatercraft.Visible = True
        '        ctlRV_WatercraftList.Visible = False
        '        lnkAddRVWater.Visible = False
        '        lblUnavailable.Visible = True
        '    End If
        'Else
        '    ' Inland Marine and RV/Watercraft is available
        '    ctlInlandMarine.ActiveIMHeader = True
        '    RefreshRVWatercraft()
        '    lnkAddRVWater.Visible = True
        '    lblUnavailable.Visible = False
        'End If

        ctlInlandMarine.ActiveIMHeader = True
        RefreshRVWatercraft()
        lnkAddRVWater.Visible = True
        lblUnavailable.Visible = False
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            LoadStaticData()

            Dim RVWaterCnt As Integer = 1

            If MyLocation.RvWatercrafts IsNot Nothing Then
                If MyLocation.RvWatercrafts.Count > 1 Then
                    RVWaterCnt = MyLocation.RvWatercrafts.Count
                End If
            End If

            PopulateChildControls()
            RefreshRVWatercraft()

            If IsQuoteReadOnly() Then
                divEndorsementButtons.Visible = True
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

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                btnMakeAChange.Visible = False
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Save_FireSaveEvent()
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
    End Sub

    'Protected Sub btnRateLocation_Click(sender As Object, e As EventArgs) Handles btnRateIMRVWater.Click
    '    'RateIMRVWatercraft()
    '    Save_FireSaveEvent()
    '    RaiseEvent QuoteRateRequested()
    'End Sub

    Private Sub RateIMRVWatercraft() Handles ctlRV_WatercraftList.CommonRateRVWater, ctlInlandMarine.CommonRateIM
        'Save_FireSaveEvent() caused duplicate validations - Matt A 8-6-15
        Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    'Protected Sub btnSaveIMRVWater_Click(sender As Object, e As EventArgs) Handles btnSaveIMRVWater.Click
    '    SaveIMWatercraft()
    'End Sub

    Private Sub SaveWatercraft() Handles ctlRV_WatercraftList.CommonSaveRVWater
        Save_FireSaveEvent()
    End Sub

    Public Sub RefreshRVWatercraft() Handles ctlRV_WatercraftList.ToggleRVWaterHdr
        'Added 03/10/2020 for Home Endorsements task 38919 MLW 
        dvYoungestOperator.Attributes.Add("style", "display:none;")

        If MyLocation.RvWatercrafts IsNot Nothing AndAlso MyLocation.RvWatercrafts.Count > 0 Then
            dvInitRVWatercraft.Visible = False
            ctlRV_WatercraftList.Visible = True
            'Added 03/10/2020 for Home Endorsements task 38919 MLW
            If Me.IsQuoteEndorsement Then
                If ctlRV_WatercraftList IsNot Nothing AndAlso ctlRV_WatercraftList.ChildVrControls IsNot Nothing AndAlso ctlRV_WatercraftList.ChildVrControls.Count > 0 Then
                    For Each child As VRControlBase In ctlRV_WatercraftList.ChildVrControls
                        If TypeOf child Is ctlRV_Watercraft Then
                            If DirectCast(child, ctlRV_Watercraft).YouthfulChecked = True Then
                                dvYoungestOperator.Attributes.Add("style", "display:block;")
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        Else
            dvInitRVWatercraft.Visible = True
            ctlRV_WatercraftList.Visible = False
        End If
    End Sub

    Private Sub SetActivePanel(activePanel As String) Handles ctlRV_WatercraftList.ActivePanel
        ActiveRVWaterIndex = activePanel
    End Sub

    Protected Sub lnkAddRVWater_Click(sender As Object, e As EventArgs) Handles lnkAddRVWater.Click
        ctlRV_WatercraftList.Visible = True
        ctlRV_WatercraftList.AddNewRVWater()
    End Sub

    Protected Sub btnBillingInfo_Click(sender As Object, e As EventArgs) Handles btnBillingInfo.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub
End Class