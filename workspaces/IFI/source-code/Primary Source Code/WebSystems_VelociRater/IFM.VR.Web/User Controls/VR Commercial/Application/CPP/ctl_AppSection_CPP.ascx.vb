Public Class ctl_AppSection_CPP
    Inherits VRControlBase

    Public Event App_Rate_ApplicationRatedSuccessfully()
    Public Event QuoteRated()
    Public Event AIChanged()


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdn_master_CPP_app, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
        AddHandler Me.ctl_AdditionalInterestList.AIChange, AddressOf HandleAIChange

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
        Me.ValidationHelper.GroupName = "Application"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctlApp_Rate.ApplicationRatedSuccessfully
        RaiseEvent App_Rate_ApplicationRatedSuccessfully()
    End Sub
    Private Sub ctl_App_Rate_ApplicationRated() Handles ctlApp_Rate.ApplicationRated
        RaiseEvent QuoteRated()
    End Sub

    Public Sub PopulateLocationList()
        ctl_CPR_App_LocationList.Populate()
    End Sub

    Public Sub PopulateIMList()
        Me.ctl_APP_CPP_IM.Populate()
    End Sub

    Protected Sub HandleAIChange()
        RaiseEvent AIChanged()
    End Sub
End Class