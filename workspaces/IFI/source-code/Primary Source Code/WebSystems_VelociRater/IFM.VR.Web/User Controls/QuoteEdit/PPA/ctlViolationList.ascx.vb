Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlViolationList
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW
    'This control is now also used for CAP Endorsements, updated for multi state 12/01/2020 MLW

    Public ReadOnly Property MyDriver As QuickQuote.CommonObjects.QuickQuoteDriver
        Get
            'Updated 12/01/2020 for CAP Endorsements Task 52973 MLW
            'If Me.Quote IsNot Nothing Then
            '    If Me.Quote.Drivers IsNot Nothing Then
            '        If Me.Quote.Drivers.Count >= Me.DriverIndex Then
            '            Return Me.Quote.Drivers(Me.DriverIndex)
            '        End If
            '    End If
            'End If
            If Me.Quote IsNot Nothing Then
                If (IsQuoteReadOnly() OrElse IsQuoteEndorsement()) AndAlso Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    If GoverningStateQuote.Drivers IsNot Nothing Then
                        If GoverningStateQuote.Drivers.Count >= Me.DriverIndex Then
                            Return GoverningStateQuote.Drivers(Me.DriverIndex)
                        End If
                    End If
                Else
                    If Me.Quote.Drivers IsNot Nothing Then
                        If Me.Quote.Drivers.Count >= Me.DriverIndex Then
                            Return Me.Quote.Drivers(Me.DriverIndex)
                        End If
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public Property DriverIndex As Int32
        Get
            If ViewState("vs_driverNum") Is Nothing Then
                ViewState("vs_driverNum") = -1
            End If
            Return CInt(ViewState("vs_driverNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_driverNum") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divViolationList.ClientID
        Me.ListAccordionDivId = Me.divViolations.ClientID
        If Not IsPostBack Then
            LoadStaticData()
            If IsOnAppPage Then
                Me.HiddenFieldMainAccord.Value = "false"
            End If
        End If

        If IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            Me.lnkAdd.Attributes.Add("style", "display: none;")
            Me.lnkBtnSave.Attributes.Add("style", "display: none;")
            Me.divViolationList.Attributes.Add("title", "No changes to Violations permitted.")
        End If

        AttachViolationControlEvents()

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If MyDriver IsNot Nothing AndAlso MyDriver.AccidentViolations IsNot Nothing AndAlso MyDriver.AccidentViolations.Any() Then
            'Updated 12/01/2020 for CAP Endorsements Task 52973 MLW
            If (IsQuoteReadOnly() OrElse IsQuoteEndorsement()) AndAlso Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                Me.VRScript.CreateAccordion(Me.MainAccordionDivId, HiddenFieldMainAccord, "0")
            Else
                Me.VRScript.CreateAccordion(Me.MainAccordionDivId, HiddenFieldMainAccord, "false")
            End If
            'Me.VRScript.CreateAccordion(Me.MainAccordionDivId, HiddenFieldMainAccord, "false")
            Me.VRScript.CreateAccordion(Me.ListAccordionDivId, HiddenField1, "0")
        Else
            'show disabled
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Nothing, "false", True)
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
        If AllowPopulate() Then
            If MyDriver IsNot Nothing AndAlso MyDriver.AccidentViolations IsNot Nothing AndAlso MyDriver.AccidentViolations.Any() Then
                Me.Repeater1.Visible = True
                Me.divViolations.Visible = True
                Me.Repeater1.DataSource = MyDriver.AccidentViolations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls() ' finds the just added controls do to the binding
                Dim index As Int32 = 0
                For Each child In Me.ChildVrControls
                    If TypeOf child Is ctlViolationItem Then
                        Dim c As ctlViolationItem = child
                        c.ViolationIndex = index
                        c.DriverIndex = Me.DriverIndex
                        c.Populate()
                        index += 1
                    End If
                Next

                Me.lblHeader.Text = String.Format("Violations ({0})", MyDriver.AccidentViolations.Count)
            Else
                Me.lblHeader.Text = "Violations"
                Me.divViolations.Visible = False
                Me.Repeater1.Visible = False
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
        If AllowValidateAndSave() Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Violations"

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
            If AllowValidateAndSave() Then
                For Each cntrl As VRControlBase In Me.ChildVrControls
                    cntrl.Save()
                    cntrl.Populate()
                Next
                Return True
            End If
        End If

        Return False
    End Function

    Private Sub AddViolationItem()
        If MyDriver IsNot Nothing Then
            If MyDriver.AccidentViolations Is Nothing Then
                MyDriver.AccidentViolations = New List(Of QuickQuoteAccidentViolation)()
            End If
            MyDriver.AccidentViolations.Add(New QuickQuote.CommonObjects.QuickQuoteAccidentViolation())

            Me.LockTree()
            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            Populate()
            Try
                Me.HiddenFieldMainAccord.Value = "0"
                Me.HiddenField1.Value = CInt(MyDriver.AccidentViolations.Count() - 1).ToString()
                Me.VRScript.AddScriptLine("$(""#" + Me.divViolations.ClientID + """).accordion({heightStyle: ""content"", active: " + Me.HiddenField1.Value + ", collapsible: true, activate: function(event, ui) { $(""#" + Me.HiddenField1.ClientID + """).val($(""#" + Me.divViolations.ClientID + """).accordion('option','active'));    } });")
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub AttachViolationControlEvents()
        ' need to wire up the remove events for each control every single time
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim violationItemControl As ctlViolationItem = cntrl.FindControl("ctlViolationItem")
            AddHandler violationItemControl.ItemRemoveRequest, AddressOf ItemRemoveRequest
        Next
    End Sub

    Private Sub ItemRemoveRequest(itemIndex As Int32)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' in memory save only
        If MyDriver IsNot Nothing AndAlso MyDriver.AccidentViolations IsNot Nothing Then
            If MyDriver.AccidentViolations.Count >= itemIndex Then
                MyDriver.AccidentViolations.RemoveAt(itemIndex)
                Me.LockTree()
            End If
        End If

        Me.Populate()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        AddViolationItem()
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    'Added 02/03/2021 for CAP Endorsements Task 52973 MLW
    Private Function AllowPopulate() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                If IsQuoteReadOnly() Then
                    Return True
                ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function

    'Added 02/03/2021 for CAP Endorsements Task 52973 MLW
    Private Function AllowValidateAndSave() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                'If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Driver" AndAlso IsNewDriverOnEndorsement(MyDriver) Then
                '    Return False
                'Else
                Return False
                'End If
            Case Else
                Return True
        End Select
    End Function

End Class