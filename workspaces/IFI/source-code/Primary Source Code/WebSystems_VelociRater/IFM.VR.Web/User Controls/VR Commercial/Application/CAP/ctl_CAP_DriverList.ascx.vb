Imports IFM.PrimativeExtensions
Public Class ctl_CAP_DriverList
    Inherits VRControlBase

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 03/30/2021 for CAP Endorsements Task 52973 MLW

    'Added 03/30/2021 for CAP Endorsements Task 52973 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    'Added 03/15/2021 for CAP Endorsements Task 52977 MLW
    Public Property ActiveDriverIndex As String
        Get
            Return hdnAccordList.Value
        End Get
        Set(value As String)
            hdnAccordList.Value = value
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Added 11/30/2020 for CAP Endorsements Task 52980 MLW
        If Not IsQuoteReadOnly() AndAlso Not IsQuoteEndorsement() Then
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        End If
        'Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub AddDriverEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim DrvCtl As ctl_CAP_Driver = cntrl.FindControl("ctl_CAP_Driver")
            AddHandler DrvCtl.RemoveDriver, AddressOf DeleteDriver
            AddHandler DrvCtl.AddDriver, AddressOf AddDriver 'Added 12/03/2020 for CAP Endorsements Task 52973 MLW
            index += 1
        Next
    End Sub

    Public Overrides Sub Populate()
        divEndorsementMaxTransactionsMessage.Visible = False 'Added 03/09/2021 for CAP Endorsements task 52973 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver") OrElse IsQuoteReadOnly() Then 'Added 12/16/2020 for CAP Endorsements Task 52973 MLW

            Me.Repeater1.DataSource = Nothing
            Me.Repeater1.DataBind()

            If Me.Quote IsNot Nothing Then
                'Updated 03/11/2021 for CAP Endorsements task 52973 MLW with if stmnt
                If Not (IsQuoteEndorsement() OrElse IsQuoteReadOnly()) Then
                    'If Me.Quote.IsNotNull Then
                    ' If no drivers on the quote open to a new driver
                    'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
                    If GoverningStateQuote.Drivers Is Nothing Then GoverningStateQuote.Drivers = New List(Of QuickQuote.CommonObjects.QuickQuoteDriver)
                    If GoverningStateQuote.Drivers.Count <= 0 Then
                        GoverningStateQuote.Drivers.AddNew()
                        Save_FireSaveEvent(False)
                    End If
                End If

                'Updated 03/11/2021 for CAP Endorsements task 52973 MLW with if stmnt
                If GoverningStateQuote.Drivers IsNot Nothing AndAlso GoverningStateQuote.Drivers.Count > 0 Then
                    'Me.lblAccordHeader.Text = "Drivers (" & GoverningStateQuote.Drivers.Count.ToString & ")"
                    Me.Repeater1.DataSource = Me.GoverningStateQuote.Drivers
                    Me.Repeater1.DataBind()

                    Me.FindChildVrControls()

                    Dim lIndex As Int32 = 0
                    For Each cnt In Me.GatherChildrenOfType(Of ctl_CAP_Driver)
                        cnt.DriverIndex = lIndex
                        cnt.Populate()
                        lIndex += 1
                    Next
                End If

                'Added 11/30/2020 for CAP Endorsements Task 52980 MLW
                If IsQuoteReadOnly() Then
                    divActionButtons.Visible = False
                    divEndorsementButtons.Visible = True
                    Me.lblAccordHeader.Text = ""
                    Me.hdrDrivers.Visible = False

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
                    'Updated 11/30/2020 for CAP Endorsement Task 52973 MLW
                    divEndorsementButtons.Visible = False
                    If IsQuoteEndorsement() Then
                        divActionButtons.Visible = True
                        Me.lblAccordHeader.Text = ""
                        Me.hdrDrivers.Visible = False
                        btnVehicles.Visible = False
                        Dim transactionCount As Integer = ddh.GetEndorsementTransactionCount()
                        If transactionCount >= 3 Then
                            divEndorsementMaxTransactionsMessage.Visible = True
                            btnAddDriver.Visible = False
                        Else
                            divEndorsementMaxTransactionsMessage.Visible = False
                            btnAddDriver.Visible = True
                        End If
                    Else
                        divActionButtons.Visible = False
                    End If
                    Me.lblAccordHeader.Text = "Drivers (" & GoverningStateQuote.Drivers.Count.ToString & ")"
                End If
            End If
        End If
    End Sub

    Private Sub DeleteDriver(ByVal DrvIndex As Integer)
        If Quote IsNot Nothing Then
            'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
            If GoverningStateQuote.Drivers IsNot Nothing Then
                If GoverningStateQuote.Drivers.HasItemAtIndex(DrvIndex) Then
                    GoverningStateQuote.Drivers.RemoveAt(DrvIndex)
                    If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim endorsementSaveError As String = ""
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    End If
                    Populate()
                    Save_FireSaveEvent(False)
                    Me.hdnAccordList.Value = (Me.GoverningStateQuote.Drivers.Count - 1).ToString
                End If
            End If
        End If
    End Sub

    'Added 12/03/2020 for CAP Endorsements Task 52973 MLW
    Private Sub AddDriver()
        If Quote IsNot Nothing Then
            lnkNew_Click(Nothing, Nothing)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMainList.ClientID
            Me.ListAccordionDivId = Me.divList.ClientID
            Me.hdnAccordList.Value = 0
        End If
        AddDriverEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        'Added 12/23/2020 for CAP Endorsements Task 52973 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver") Then
            Me.SaveChildControls()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 12/23/2020 for CAP Endorsements Task 52973 MLW
        If IsOnAppPage OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver") Then
            MyBase.ValidateControl(valArgs)

            Me.ValidationHelper.GroupName = String.Format("Drivers")
            'Updated 03/11/2021 for CAP Endorsements Task 52973 MLW
            If Not IsQuoteEndorsement() Then
                'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
                If GoverningStateQuote.Drivers Is Nothing OrElse GoverningStateQuote.Drivers.Count <= 0 Then
                    Me.ValidationHelper.AddError("Quote must have at least one driver.", Nothing)
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    'Updated 11/30/2020 for CAP Endorsements Task 52973 MLW
    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click, btnAddDriver.Click
        'Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        ' Add Driver
        'updated 8/15/2018 from Quote.Drivers to use GoverningStateQuote; note: could set variable so it doesn't have to look it up every time
        If GoverningStateQuote.Drivers Is Nothing Then GoverningStateQuote.Drivers = New List(Of QuickQuote.CommonObjects.QuickQuoteDriver)
        Dim newDriver As New QuickQuote.CommonObjects.QuickQuoteDriver()
        GoverningStateQuote.Drivers.Add(newDriver)
        'Added 03/09/2021 for CAP Endorsements Task 52973 MLW
        If IsQuoteEndorsement() Then
            Dim DriverIndex As Integer = 0
            If GoverningStateQuote.Drivers.Count > 0 Then
                DriverIndex = GoverningStateQuote.Drivers.Count - 1
            End If
            ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, DriverIndex, newDriver)
        End If
        Populate()
        Save_FireSaveEvent(False)
        If GoverningStateQuote.Drivers IsNot Nothing AndAlso GoverningStateQuote.Drivers.Count > 0 Then
            Me.hdnAccordList.Value = (Me.GoverningStateQuote.Drivers.Count - 1).ToString
        Else
            Me.hdnAccordList.Value = "0"
        End If
    End Sub

    'Updated 11/30/2020 for CAP Endorsements Task 52973 MLW
    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, btnSaveVehicles.Click
        'Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent(True)
    End Sub

    'Added 11/30/2020 for CAP Endorsements task 52980 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 11/30/2020 for CAP Endorsements task 52980 MLW
    Protected Sub btnViewVehicles_Click(sender As Object, e As EventArgs) Handles btnViewVehicles.Click, btnVehicles.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
    End Sub

    'Added 11/30/2020 for CAP Endorsements task 52980 MLW
    Private Sub btnSaveAndRate_Click(sender As Object, e As EventArgs) Handles btnSaveAndRate.Click
        'Session("valuationValue") = "False"
        'Save_FireSaveEvent(False) 'Removed 01/28/2021 MLW - Causes duplicate validation messages
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

End Class