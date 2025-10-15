Public Class ctl_InlandMarine_HOM_App
    Inherits VRControlBase

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divInlandMarine.ClientID
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, hiddenActiveInlandMarine, "false")
        Me.VRScript.CreateJSBinding(Me.lnkClearInland.ClientID, "click", "return confirm(""Clear Inland Marine?"");")
        Me.VRScript.StopEventPropagation(Me.lnkSaveinland.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.ctlInlandMarine.Populate()
    End Sub

    Private Sub RecalculateIMCoverageTotal(optionalTotal) Handles ctlInlandMarine.RecalculateCoverageTotal
        lblIMChosen.Text = "(" + optionalTotal.ToString + ")"
        If Not IsPostBack And IsOnAppPage Then
            If IsNumeric(optionalTotal) AndAlso CInt(optionalTotal) > 0 Then
                Me.hiddenActiveInlandMarine.Value = "0"
            End If
        End If

    End Sub

    Public Overrides Function Save() As Boolean
        Me.ctlInlandMarine.Save()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
        'Me.ctlInlandMarineItem.ValidateForm() ' This should be using ValidateControl()
    End Sub

    Protected Sub lnkSaveinland_Click(sender As Object, e As EventArgs) Handles lnkSaveinland.Click
        Me.Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub

    Protected Sub lnkClearInland_Click(sender As Object, e As EventArgs) Handles lnkClearInland.Click
        Me.ctlInlandMarine.ClearControl()
    End Sub
End Class