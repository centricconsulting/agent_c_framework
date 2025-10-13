Imports IFM.PrimativeExtensions
Imports IFM.Common.InputValidation.InputHelpers
Public Class WaterCraftOperatorItem
    Inherits VRControlBase

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
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
            Return Me.Quote?.Operators.GetItemAtIndex(OperatorIndex)
        End Get
    End Property

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.MyOperator IsNot Nothing Then
            Me.txtFirstName.Text = Coalesce(Me.MyOperator?.Name?.FirstName, String.Empty).ToUpper()
            Me.txtLastName.Text = Coalesce(Me.MyOperator?.Name?.LastName, String.Empty).ToUpper()
            Me.txtBirthDate.Text = RemovePossibleDefaultedDateValue(Coalesce(Of String)(Me.MyOperator?.Name?.BirthDate, String.Empty))
            lblAccordianHeader.Text = $"Operator #{Me.OperatorIndex + 1}"
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove this Operator?")
        Me.VRScript.CreateAccordion(Me.divAccord.ClientID, Me.HiddenFieldMainAccord, "0")
        Me.txtBirthDate.CreateWatermark("MM/DD/YYYY")
        Me.VRScript.CreateTextBoxFormatter(Me.txtBirthDate, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divAccord.ClientID
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = $"RV/Watercraft Operator #{Me.OperatorIndex + 1}"

        Dim accordLevels As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        'don't like this but PPA is too far along to change right now
        Dim validationType = Me.DefaultValidationType
        If (Me.IsQuoteEndorsement) Then
            validationType = Validation.ObjectValidation.ValidationItem.ValidationType.endorsement
        End If

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
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.MyOperator IsNot Nothing Then
            Me.MyOperator.Name.CreateIfNull()
            Me.MyOperator.Name.FirstName = Me.txtFirstName.Text.ToUpper()
            Me.MyOperator.Name.LastName = Me.txtLastName.Text.ToUpper()
            Me.MyOperator.Name.BirthDate = Me.txtBirthDate.Text
            Return True
        End If
        Return False
    End Function



    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        Me.Save_FireSaveEvent(False)
        IFM.VR.Common.Helpers.WaterCraftOperatorHelper.RemoveWatercraftOperator(Quote, Me.MyOperator)
        Me.ParentVrControl.Populate()
        Me.Save_FireSaveEvent(False)
    End Sub


End Class