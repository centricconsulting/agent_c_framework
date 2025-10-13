Imports IFM.PrimativeExtensions

Public Class ctl_Producer
    Inherits VRControlBase

    Public ReadOnly Property ProducerID() As String
        Get
            Return Me.ddProducer.ClientID
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divProducer.ClientID, Me.accordActive, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        Me.VRScript.CreateConfirmDialog(Me.lnkClearBase.ClientID, "Clear Agent Producer?")

    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddProducer.Items.Count = 0 Then
            Me.ddProducer.Items.Add("")
            For Each i In VR.Common.Helpers.AgencyProducers.GetProducersByAgencyId(Me.AgencyId)
                Me.ddProducer.Items.Add(New ListItem(String.Format("{0} - {1}", i.Name, i.Code), i.ProducerID))
            Next
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.Quote.IsNotNull Then
            ddProducer.SetFromValue(Me.Quote.AgencyProducerId)
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote.IsNotNull Then
            Me.Quote.AgencyProducerId = Me.ddProducer.SelectedValue
        End If
        Return True
    End Function

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkClearBase_Click(sender As Object, e As EventArgs) Handles lnkClearBase.Click
        Me.ddProducer.SelectedIndex = 0
    End Sub
End Class