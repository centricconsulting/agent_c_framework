Public Class ctl_AppSection_Farm
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateAccordion(Me.divIM_RV.ClientID, Me.hdnIM_RV, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
        If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
            If Me.Quote.Locations(0).ProgramTypeId = "8" Then
                Me.ctlFarmPersonalProperty.Visible = False
                Me.divIM_RV.Visible = False
            End If
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID

        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctl_App_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctl_App_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

End Class