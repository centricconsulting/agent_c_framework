Imports IFM.VR.Common.Helpers.CGL

Public Class ctl_CGL_LocationsWF
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler ctl_CGL_LocationList.LocationChanged, AddressOf HandleLocationChange
    End Sub

    Protected Sub HandleLocationChange(ByVal LocIndex As Integer)
        ctl_CGL_ClassCodeList.ReloadLocations()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnRate) Then
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub btnAddLocation_Click(sender As Object, e As EventArgs) Handles btnAddLocation.Click
        Me.ctl_CGL_LocationList.AddNewLocation()
    End Sub
End Class