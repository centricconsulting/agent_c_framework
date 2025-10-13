Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers

Public Class ctlViolationItem
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Event ItemRemoveRequest(index As Int32)

    Public ReadOnly Property MyDriver As QuickQuoteDriver
        Get
            If Me.Quote IsNot Nothing Then
                'Updated 08/05/2021 for CAP Endorsements Bug 62069 MLW
                If (IsQuoteReadOnly() OrElse IsQuoteEndorsement()) AndAlso Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    If GoverningStateQuote.Drivers IsNot Nothing Then
                        If GoverningStateQuote.Drivers.Count >= Me.DriverIndex Then
                            Return GoverningStateQuote.Drivers(Me.DriverIndex)
                        End If
                    End If
                Else
                    If Me.Quote.Drivers IsNot Nothing Then
                        If Me.Quote.Drivers.Count > Me.DriverIndex Then
                            Return Me.Quote.Drivers(Me.DriverIndex)
                        End If
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MyViolation As QuickQuoteAccidentViolation
        Get
            If Me.MyDriver IsNot Nothing Then
                If Me.MyDriver.AccidentViolations IsNot Nothing Then
                    If Me.MyDriver.AccidentViolations.Count > Me.ViolationIndex Then
                        Return Me.MyDriver.AccidentViolations(Me.ViolationIndex)
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

    Public Property ViolationIndex As Int32
        Get
            If ViewState("vs_violationNum") Is Nothing Then
                ViewState("vs_violationNum") = -1
            End If
            Return CInt(ViewState("vs_violationNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_violationNum") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.ViolationIndex
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            'LoadStaticData()
        End If
        If Me.IsOnAppPage OrElse IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
            Me.lnkRemove.Attributes.Add("style", "display: none;")
            Me.txtViolationDate.Enabled = False
            Me.ddViolationType.Enabled = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateDatePicker(Me.txtViolationDate.ClientID, True)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove Violation Item?")
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddViolationType.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddViolationType, QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuotePropertyName.AccidentsViolationsTypeId, SortBy.TextAscending, Me.Quote.LobType)
        End If

    End Sub

    Public Overrides Sub Populate()
        If MyViolation IsNot Nothing Then
            'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
            If AllowPopulate() Then
                LoadStaticData()

                Dim typeOfViolation As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteAccidentViolation, QuickQuotePropertyName.AccidentsViolationsTypeId, MyViolation.AccidentsViolationsTypeId)

                SetdropDownFromValue_ForceSeletion(Me.ddViolationType, MyViolation.AccidentsViolationsTypeId, typeOfViolation)

                'set after dd above
                Try
                    Dim violoationTypeText As String = Me.ddViolationType.SelectedItem.Text
                    If Me.IsOnAppPage Then
                        violoationTypeText = EllipsisText(violoationTypeText, 60)
                    Else
                        violoationTypeText = EllipsisText(violoationTypeText, 40)
                    End If

                    Me.lblAccordianHeader.Text = String.Format("Violation #{0} - {1}", Me.ViolationIndex + 1, violoationTypeText)
                Catch ex As Exception
                End Try
                If IsDate(MyViolation.AvDate) Then
                    Me.txtViolationDate.Text = RemovePossibleDefaultedDateValue(MyViolation.AvDate)
                Else
                    'keep whatever is there now
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
        If AllowValidateAndSave() Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Driver #" + (Me.DriverIndex + 1).ToString() + " - Violation #" + (Me.ViolationIndex + 1).ToString()

            If Me.IsOnAppPage = False Then
                Dim accordLevels As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                'Dim accordLevels As New List(Of KeyValuePair(Of String, String))
                'accordLevels.Add(New KeyValuePair(Of String, String)(Me.ParentVrControl.ParentVrControl.MainAccordionDivId, Me.DriverIndex.ToString()))
                'accordLevels.Add(New KeyValuePair(Of String, String)(Me.ParentVrControl.MainAccordionDivId, "0"))
                'accordLevels.Add(New KeyValuePair(Of String, String)(Me.ParentVrControl.ListAccordionDivId, Me.ViolationIndex.ToString()))

                Dim valItems = ViolationValidator.ValidateViolation(Me.DriverIndex, Me.ViolationIndex, Me.Quote)
                If valItems.Any() Then
                    For Each v In valItems
                        Select Case v.FieldId
                            Case ViolationValidator.ViolationType
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddViolationType, v, accordLevels)
                            Case ViolationValidator.ViolationDate
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtViolationDate, v, accordLevels)
                            Case Else
                                If v.IsWarning Then
                                    Me.ValidationHelper.AddError(v.Message)
                                Else
                                    Me.ValidationHelper.AddWarning(v.Message)
                                End If
                        End Select
                    Next
                End If

            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible = True And Me.IsOnAppPage = False Then
            If MyViolation IsNot Nothing Then
                'Added 12/24/2020 for CAP Endorsements Task 52973 MLW
                If AllowValidateAndSave() Then
                    MyViolation.AccidentsViolationsTypeId = Me.ddViolationType.SelectedValue
                    MyViolation.AvDate = Me.txtViolationDate.Text.Trim()
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Me.IsOnAppPage = False Then
            Me.Save_FireSaveEvent(True)
        End If
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        If Quote IsNot Nothing And IsOnAppPage = False Then
            If MyDriver IsNot Nothing AndAlso MyDriver.AccidentViolations IsNot Nothing Then

                Try
                    RaiseEvent ItemRemoveRequest(Me.ViolationIndex)
                Catch ex As Exception
                End Try

            End If

        End If
    End Sub

    Public Overrides Sub ClearControl()
        Me.txtViolationDate.Text = ""
        Me.ddViolationType.SelectedIndex = -1

        MyBase.ClearControl()
    End Sub

    'Added 02/03/2021 for CAP Endorsement Task 52973 MLW
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

    'Added 02/03/2021 for CAP Endorsement Task 52973 MLW
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