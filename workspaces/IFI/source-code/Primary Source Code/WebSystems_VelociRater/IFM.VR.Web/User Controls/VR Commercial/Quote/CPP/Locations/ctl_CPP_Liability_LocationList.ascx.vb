Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL
Imports QuickQuote.CommonObjects
Public Class ctl_CPP_Liability_LocationList
    Inherits VRControlBase

    Public Event LocationChanged(ByVal LocIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Property ActiveLocationIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    Public Overrides Sub AddScriptWhenRendered()
        'If Me.divNewLocation.Visible Then
        '    Me.VRScript.CreateAccordion(Me.divNewLocation.ClientID, Nothing, "false", True)
        'Else
        'If Me.divLocationList.Visible Then
        '        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
        '    End If
        'End If

    End Sub
    Private Sub AddHandlers()
        'Dim index As Int32 = 0
        'For Each cntrl As RepeaterItem In Me.Repeater1.Items
        '    Dim LocControl As ctl_CPP_Liabilty_Location = cntrl.FindControl("ctl_CPPCGL_Location")
        '    AddHandler LocControl.LocationChanged, AddressOf HandleAddressChange
        '    AddHandler LocControl.AddLocationRequested, AddressOf AddNewLocation
        '    AddHandler LocControl.CopyLocationRequested, AddressOf CopyLocation
        '    AddHandler LocControl.DeleteLocationRequested, AddressOf DeleteLocation
        '    AddHandler LocControl.ClearLocationRequested, AddressOf ClearLocation
        '    index += 1
        'Next
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Protected Sub HandleAddressChange(ByVal LocIndex As Integer)
        RaiseEvent LocationChanged(LocIndex)
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            ' Don't show the rate button when contractors enhancement is selected
            'Updated 12/21/18 for multi state MLW
            'If Quote.HasContractorsEnhancement Then
            If SubQuoteFirst.HasContractorsEnhancement Then
                Me.btnRate.Visible = False
            Else
                Me.btnRate.Visible = True
            End If
            Me.divLocationList.Visible = False
            If Me.Quote.Locations.IsLoaded() Then
                Me.divLocationList.Visible = True
                Me.Repeater1.DataSource = Me.Quote.Locations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim index As Int32 = 0
                For Each Loc As ctl_CPP_Liabilty_Location In Me.GatherChildrenOfType(Of ctl_CPP_Liabilty_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                'Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If

            'Added 12/6/2021 for CPP Endorsements Task 67126 MLW
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
        AddHandlers()
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divLocationList.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean

        Me.SaveChildControls()
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub AddNewLocation()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.AddNew()
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub DeleteLocation(LocationIndex)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.Quote.Locations.RemoveAt(LocationIndex)
            Me.ParentVrControl.Populate()
            Me.Save_FireSaveEvent(False)
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub CopyLocation(LocationIndex)
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(LocationIndex) Then
            Dim newloc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(LocationIndex)
            Quote.Locations.Add(newloc)
            Save_FireSaveEvent()
            Populate_FirePopulateEvent()
            hdnAccord.Value = (Quote.Locations.Count - 1).ToString()
        End If
    End Sub

    Protected Sub ClearLocation(LocationIndex)
        ' Clear the Wind/Hail, Property In the Open, and Building controls
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnRate) Then
            Session("CPPCPRCheckACVEventTrigger") = "RateButton"
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub btnContinueIM_Click(sender As Object, e As EventArgs) Handles btnContinueIM.Click
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.InlandMarine, "")
        End If
    End Sub

    Private Sub btnContinueCRM_Click(sender As Object, e As EventArgs) Handles btnContinueCRM.Click
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.Crime, "")
        End If
    End Sub

    'Added 12/6/2021 for CPP Endorsements task 67126 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 12/6/2021 for CPP Endorsements task 67126 MLW
    Protected Sub btnViewInlandMarine_Click(sender As Object, e As EventArgs) Handles btnViewInlandMarine.Click
        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.InlandMarine, "")
    End Sub

End Class