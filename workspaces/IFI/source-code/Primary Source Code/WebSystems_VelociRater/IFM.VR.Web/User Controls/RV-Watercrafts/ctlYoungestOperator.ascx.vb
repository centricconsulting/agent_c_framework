Imports IFM.PrimativeExtensions
Imports IFM.Common.InputValidation.InputHelpers
Public Class ctlYoungestOperator
    Inherits VRControlBase

    'Added control on 02/19/2020 for Home Endorsements task 38919

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.OperatorIndex
        End Get
    End Property

    Public Property OperatorIndex As Int32
        Get
            If ViewState("vs_operatorIndex") Is Nothing Then
                ViewState("vs_operatorIndex") = -1
            End If
            Return CInt(ViewState("vs_operatorIndex"))
        End Get
        Set(value As Int32)
            ViewState("vs_operatorIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyOperator As QuickQuote.CommonObjects.QuickQuoteOperator
        Get
            Dim myOpNum As Integer = 0
            If QQHelper.IsPositiveIntegerString(hiddenOperatorId.Value) = True Then
                myOpNum = CInt(hiddenOperatorId.Value)
            Else
                Dim youngestOperatorNums = IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYoungestOperatorNums(Me.Quote)
                If youngestOperatorNums IsNot Nothing AndAlso youngestOperatorNums.Count > 0 AndAlso youngestOperatorNums(0) > 0 Then
                    Me.hiddenOperatorId.Value = youngestOperatorNums(0)
                    myOpNum = youngestOperatorNums(0)
                End If
            End If
            If myOpNum > 0 AndAlso Quote?.Operators IsNot Nothing AndAlso Me.Quote.Operators(myOpNum - 1) IsNot Nothing Then
                Return Me.Quote.Operators(myOpNum - 1)
            Else
                Return IFM.VR.Common.Helpers.WaterCraftOperatorHelper.GetYoungestOperator(Me.Quote)
            End If
        End Get
    End Property

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divYoungestOperator.ClientID, Me.HiddenFieldMainAccord, "0")
        Me.txtBirthDate.CreateWatermark("MM/DD/YYYY")
        Me.VRScript.CreateTextBoxFormatter(Me.txtBirthDate, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateConfirmDialog(lnkBtnClear.ClientID, "Clear Youngest Operator Information ?")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divYoungestOperator.ClientID
        If Not Me.IsQuoteEndorsement Then
            Me.Visible = False
            Me.HideFromParent = True
            Return
        End If
    End Sub

    Public Overrides Sub Populate()
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.IsQuoteEndorsement Then
            If Me.MyOperator IsNot Nothing Then
                Me.txtFirstName.Text = Coalesce(Me.MyOperator?.Name?.FirstName, String.Empty).ToUpper()
                Me.txtLastName.Text = Coalesce(Me.MyOperator?.Name?.LastName, String.Empty).ToUpper()
                Me.txtBirthDate.Text = RemovePossibleDefaultedDateValue(Coalesce(Of String)(Me.MyOperator?.Name?.BirthDate, String.Empty))
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.IsQuoteEndorsement AndAlso Me.Visible = True Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Youngest Operator in the Household"
            Me.ValidateChildControls(valArgs)

            Dim accordLevels As List(Of VRAccordionTogglePair) = Me.MyAccordionList

            Dim validationType = Me.DefaultValidationType
            If (Me.IsQuoteEndorsement) Then
                validationType = Validation.ObjectValidation.ValidationItem.ValidationType.endorsement
            End If

            If Me.MyOperator IsNot Nothing Then
                Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.ValidateRvWaterCraftOperator(Me.MyOperator, validationType)
                If valItems.Any() Then
                    For Each v In valItems
                        Select Case v.FieldId
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.FirstName
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstName, v, accordLevels)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.LastName
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLastName, v, accordLevels)
                            Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.RVWatercraftOperatorsValidator.BirthDate
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBirthDate, v, accordLevels)
                        End Select
                    Next
                End If
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.IsQuoteEndorsement Then
            If Me.MyOperator IsNot Nothing Then
                Me.MyOperator.Name.CreateIfNull()
                Me.MyOperator.Name.FirstName = Me.txtFirstName.Text.ToUpper()
                Me.MyOperator.Name.LastName = Me.txtLastName.Text.ToUpper()
                Me.MyOperator.Name.BirthDate = Me.txtBirthDate.Text
                If Me.Visible = False Then 'default date if needed when invisible
                    If QQHelper.IsValidDateString(Me.txtBirthDate.Text, mustBeGreaterThanDefaultDate:=True) = False Then
                        Me.MyOperator.Name.BirthDate = DateTime.Now.AddYears(-16).ToShortDateString()
                    End If
                End If
                Return True
            End If
        End If
        Return False
    End Function

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkBtnClear_Click(sender As Object, e As EventArgs) Handles lnkBtnClear.Click
        ClearControl()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtBirthDate.Text = ""
    End Sub

End Class