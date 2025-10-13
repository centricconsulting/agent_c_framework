Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL
Public Class ctl_CPP_LiabilityCoverages
    Inherits VRControlBase
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "General Information"
        ValidateChildControls(valArgs)
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_CPR_Coverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

End Class