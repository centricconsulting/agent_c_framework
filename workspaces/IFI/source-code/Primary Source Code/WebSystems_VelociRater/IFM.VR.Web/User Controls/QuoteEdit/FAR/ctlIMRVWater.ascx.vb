Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlIMRVWater
    Inherits VRControlBase

    Public Event QuoteRateRequested()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
    End Sub

    Protected Sub btnRateLocation_Click(sender As Object, e As EventArgs) Handles btnRateLocation.Click
        Save_FireSaveEvent()
        RaiseEvent QuoteRateRequested()
    End Sub
End Class