Public Class ctl_App_Section
    Inherits VRControlBase

    Public Event QuoteRated()
    Public Event ApplicationRatedSuccessfully()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
        If Me.hasEsigOption = False Then
            ctl_Esignature.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

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
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub ctl_VehicleList_App_RequestPageRefresh() Handles ctl_VehicleList_App.RequestPageRefresh
        Me.ParentVrControl.Populate()
    End Sub

    Private Sub ctl_App_Rate_ApplicationRated() Handles ctl_App_Rate.ApplicationRated
        RaiseEvent QuoteRated() ' Informs tree
    End Sub
    Private Sub ctl_App_Rate_ApplicationRatedSuccessfully() Handles ctl_App_Rate.ApplicationRatedSuccessfully
        RaiseEvent ApplicationRatedSuccessfully()
    End Sub

End Class