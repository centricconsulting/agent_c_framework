Public Class ctl_AppSection_fuppup
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
        If SubQuoteFirst.ProgramTypeId = "4" Then
            ctl_Farm_Remarks.Visible = False
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMain.ClientID
        End If

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