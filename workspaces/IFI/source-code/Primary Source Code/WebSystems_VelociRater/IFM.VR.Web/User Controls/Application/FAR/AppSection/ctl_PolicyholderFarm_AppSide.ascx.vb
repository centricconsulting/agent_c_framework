Public Class ctl_PolicyholderFarm_AppSide
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        'Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Are your sure you want to clear this item?", ctlPageStartupScript.JsEventType.onclick)

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            Me.Visible = Me.Quote.Policyholder.Name.TypeId = "2"
            Me.txtDBA.Text = Me.Quote.Policyholder.Name.DoingBusinessAsName
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible Then
            If Me.Quote IsNot Nothing Then
                Me.Quote.Policyholder.Name.DoingBusinessAsName = Me.txtDBA.Text.ToUpper().Trim()
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class