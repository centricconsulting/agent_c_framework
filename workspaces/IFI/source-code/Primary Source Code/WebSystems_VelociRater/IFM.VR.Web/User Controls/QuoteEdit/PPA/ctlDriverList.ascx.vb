Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class ctlDriverList
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Property ActiveDriverPane As String
        Get
            Return Me.hiddenActiveDriver.Value
        End Get
        Set(value As String)
            Me.hiddenActiveDriver.Value = value
        End Set
    End Property

    'added 6/18/2020
    Public ReadOnly Property ShouldGenericLossHistoryControlBeUsed As Boolean
        Get
            If IsOnAppPage = False AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso QuickQuoteHelperClass.PPA_CheckDictionaryKeyToOrderClueAtQuoteRate() = True AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divDriver.ClientID
            Populate()
        End If
        AttachDriverControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()
        If Me.Quote IsNot Nothing Then
            Me.VRScript.AddScriptLine("PolicyHolderCount = " + IFM.VR.Common.Helpers.QuickQuoteObjectHelper.PolicyHolderCount(Me.Quote).ToString() + ";")
            ' run at all startups to disable or enable policyholder1/2 in relationship to policyholder dd
            Me.VRScript.AddScriptLine("CheckDriverRelationshipToPolicyHolder();", True)
        End If

        '7-20-14 added for copy driver to policyholder logic - requested via pilot feedback
        Me.VRScript.AddVariableLine("var drivers_copy = new Array();")
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Drivers IsNot Nothing Then
                For Each d In Me.Quote.Drivers
                    Me.VRScript.AddVariableLine("drivers_copy.push(new DriverCopyObject('" + d.Name.FirstName.Replace("'", "\'") + "','" + d.Name.MiddleName.Replace("'", "\'") + "','" + d.Name.LastName.Replace("'", "\'") + "','" + d.Name.SuffixName.Replace(".", "") + "','" + d.Name.SexId + "','" + d.Name.BirthDate + "'));")
                Next
            End If
        End If

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.AddVariableLine("var DriverListControlDivTopMost = '" + Me.DriverListControlDivTopMost.ClientID + "';")
        Me.VRScript.AddVariableLine("function ShowDriver(index){SetActiveAccordionIndex('" + ListAccordionDivId + "',index);}") ' here to respond to treeview driver clicks
        Me.VRScript.CreateAccordion(ListAccordionDivId, hiddenActiveDriver, "0")
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'added 6/18/2020
        If ShouldGenericLossHistoryControlBeUsed = True Then
            Me.ctlLossHistoryGeneric.Visible = True
            Me.ctlLossHistoryGeneric.Populate()
        Else
            Me.ctlLossHistoryGeneric.Visible = False
        End If

        Me.Repeater1.DataSource = Nothing
        If Me.Quote IsNot Nothing Then
            Me.Repeater1.DataSource = Me.Quote.Drivers
            Me.Repeater1.DataBind()

            Me.FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.GatherChildrenOfType(Of ctlDriver_PPA)
                child.DriverIndex = index
                child.Populate()
                index += 1
            Next

            Me.btnSubmit.Visible = Me.Quote.Drivers.IsLoaded
        End If

        If Me.Quote IsNot Nothing Then
            If IsQuoteReadOnly() Then
                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
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
            If IsQuoteEndorsement() Then
                Me.btnRateDriver.Visible = True
            Else
                Me.btnRateDriver.Visible = False
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Drivers List"

        Me.ValidateChildControls(valArgs)

        'checks each driver if there are any
        Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.PolicyDriverLossesAndViolationValidator.ValidatePolicyDriversLossesAndViolations(Me.Quote, valArgs.ValidationType)
        For Each valItem In valList
            If valItem.IsWarning Then
                Me.ValidationHelper.AddWarning(valItem.Message)
            Else
                Me.ValidationHelper.AddError(valItem.Message)
            End If
        Next

        ' checks for no drivers
        Dim valDriverList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA.DriverListValidator.ValidateDriverList(Me.Quote, valArgs.ValidationType)
        For Each valItem In valDriverList
            If valItem.IsWarning Then
                Me.ValidationHelper.AddWarning(valItem.Message)
            Else
                Me.ValidationHelper.AddError(valItem.Message)
            End If
        Next

    End Sub

    Public Overrides Function Save() As Boolean
        For Each c In Me.ChildVrControls
            c.Save()
            c.Populate()
        Next
        Return True
    End Function

    Protected Sub AttachDriverControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleControl As ctlDriver_PPA = cntrl.FindControl("ctlDriver_PPAControl")
            AddHandler vehicleControl.DriverIndexRemoving, AddressOf driverControlRemoving
            AddHandler vehicleControl.NewDriverRequested, AddressOf driverControlNewDriverRequested
            index += 1
        Next
    End Sub

    Private Sub driverControlRemoving(index As Integer)
        Dim activePan As Int32 = 0
        If Int32.TryParse(Me.hiddenActiveDriver.Value, activePan) Then
            If activePan >= index Then
                Me.hiddenActiveDriver.Value = (activePan - 1).ToString()
            End If
        End If
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) '11-7-14 Matt A
        If Quote IsNot Nothing Then
            If Me.Quote.Drivers IsNot Nothing Then
                ' need to tell the list control to tell the other driver controls to save now
                ' you need to do this because the indexes will be different after the driver is removed
                'aiseEvent SaveRequested(False)

                ' if this driver is assigned to a vehicle remove him/her
                If Me.Quote.Vehicles IsNot Nothing Then
                    For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Me.Quote.Vehicles
                        If v.PrincipalDriverNum = (index).ToString() Then
                            v.PrincipalDriverNum = ""
                        End If
                        If v.OccasionalDriver1Num = (index).ToString() Then
                            v.OccasionalDriver1Num = ""
                        End If
                        If v.OccasionalDriver2Num = (index).ToString() Then
                            v.OccasionalDriver2Num = ""
                        End If
                        If v.OccasionalDriver3Num = (index).ToString() Then
                            v.OccasionalDriver3Num = ""
                        End If
                    Next
                End If

                'move assigned drivers on vehicles if needed
                IFM.VR.Common.Helpers.PPA.DriverListHelper.DoAssignedDriverCheck(Me.Quote, index)

                ' all the other drivers info is now saved so it is safe to remove this one
                Me.Quote.Drivers.RemoveAt(index)

                ' if there are more prim/occ drivers selected than rated drivers - then remove until it equals
                If Me.Quote.Vehicles IsNot Nothing Then
                    Dim ratedDriverCount As Integer = 0
                    If Me.Quote.Drivers IsNot Nothing Then
                        ratedDriverCount = (From d In Me.Quote.Drivers Where d.DriverExcludeTypeId = "1" Select d).Count()
                    End If

                    For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Me.Quote.Vehicles
                        Select Case ratedDriverCount
                            Case 0
                                v.PrincipalDriverNum = ""
                                v.OccasionalDriver1Num = ""
                                v.OccasionalDriver2Num = ""
                                v.OccasionalDriver3Num = ""
                            Case 1
                                v.OccasionalDriver1Num = ""
                                v.OccasionalDriver2Num = ""
                                v.OccasionalDriver3Num = ""
                            Case 2
                                v.OccasionalDriver2Num = ""
                                v.OccasionalDriver3Num = ""
                            Case 3
                                v.OccasionalDriver3Num = ""
                            Case Else
                                ' do nothing
                        End Select
                    Next

                End If

                ' need to tell the list to refresh to get new driver indexes
                Populate()
                Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) 'swapped order 11-7-14 Matt A
            End If

        End If

    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSaveAndGotoVehicles.Click, btnSubmit.Click
        'Me.Save_FireSaveEvent(True)
        Me.Save_FireSaveEvent()

        If sender Is btnSaveAndGotoVehicles Then
            If Me.ValidationSummmary.HasErrors = False Then
                Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
            End If
        End If
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoVehicles_Click(sender As Object, e As EventArgs) Handles btnViewGotoVehicles.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
    End Sub

    Protected Sub bnAddDriver_Click(sender As Object, e As EventArgs) Handles bnAddDriver.Click
        If Me.Quote IsNot Nothing Then
            Me.Quote.Drivers.AddNew()
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Me.Populate()
            Me.hiddenActiveDriver.Value = (Me.Quote.Drivers.Count() - 1).ToString()
        End If

    End Sub

    Private Sub driverControlNewDriverRequested()
        bnAddDriver_Click(Nothing, Nothing)
    End Sub

    'Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
    '    Dim DriverControl As ctlDriver_PPA = e.Item.FindControl("ctlDriver_PPAControl")
    '    Dim DLElement As TextBox = DriverControl.FindControl("txtDLNumber")
    '    Dim driver As QuickQuoteDriver = Quote.Drivers(e.Item.ItemIndex)

    '    If IsQuoteEndorsement() Then
    '        'If driver.HasValidDriverNum() = True AndAlso QQHelper.IsQuickQuoteDriverNewToImage(driver, Me.Quote.TransactionEffectiveDate, Me.Quote.EffectiveDate, Me.Quote.PCAdded_Date) = False Then
    '        'updated 7/25/2019 to use new IsQuickQuoteDriverNewToImage method
    '        If driver.HasValidDriverNum() = True AndAlso QQHelper.IsQuickQuoteDriverNewToImage(driver, Me.Quote) = False Then
    '            DLElement.Enabled = False
    '        End If

    '    End If
    'End Sub

    Private Sub btnRateDriver_Click(sender As Object, e As EventArgs) Handles btnRateDriver.Click
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub
End Class