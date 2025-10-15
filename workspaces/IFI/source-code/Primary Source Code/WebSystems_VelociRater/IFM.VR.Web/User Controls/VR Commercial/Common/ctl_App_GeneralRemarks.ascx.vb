Imports IFM.PrimativeExtensions

Public Class ctl_App_GeneralRemarks
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divGeneralRemarks.ClientID, Me.accordActive, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Quote.TransactionRemark IsNot Nothing AndAlso Quote.TransactionRemark.Trim <> "" Then
                txtRemarks.Text = Quote.TransactionRemark
            Else
                txtRemarks.Text = ""
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidationHelper.GroupName = "General Remarks"

        If txtRemarks.Text.Trim = "" Then
            Me.ValidationHelper.AddError(txtRemarks, "Missing Remarks", accordList)
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote.IsNotNull Then
            Quote.TransactionRemark = txtRemarks.Text
        End If
        Return True
    End Function

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class