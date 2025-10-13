Public Class ctl_HOM_App_Section
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event App_Rate_ApplicationRatedSuccessfully()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.div_master_hom_app.ClientID

        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, hiddendiv_master_hom_app, "0")
        'Me.VRScript.CreateAccordion(Me.divIM_RV.ClientID, Me.hdnIM_RV, "0")
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Application"
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctl_App_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctl_App_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub

End Class