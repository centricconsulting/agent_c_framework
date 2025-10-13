Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.CGL
Imports QuickQuote.CommonObjects
Public Class ctl_CPP_PropertyCoverages
    Inherits VRControlBase

    Public Event BlanketDeductibleChanged()
    Public Event BuildingZeroDeductibleChanged()
    Public Event AgreedAmountChanged(ByVal newvalue As Boolean)
    Public Event NeedToReloadCIMTransportation()
    Public Event NeedToClearCIMTransportation()

    Public Sub HandleBlanketDeductibleChanged()
        RaiseEvent BlanketDeductibleChanged()
    End Sub

    Public Sub HandleBuildingZeroDeductibleChanged()
        Me.ctl_CPP_PropertyCoverages.UpdateBlanketDeductibleFromBuildingZero()
    End Sub

    Private Sub HandleAgreedAmountChanged(ByVal newvalue As Boolean)
        RaiseEvent AgreedAmountChanged(newvalue)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        PopulateChildControls()
        Exit Sub
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddHandler Me.ctl_CPP_PropertyCoverages.BlanketDeductibleChanged, AddressOf HandleBlanketDeductibleChanged
        AddHandler Me.ctl_CPP_PropertyCoverages.AgreedAmountChanged, AddressOf HandleAgreedAmountChanged
        If Not IsPostBack Then
        End If
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
        ValidateChildControls(valArgs)
        Exit Sub
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        Me.ctl_CPP_PropertyCoverages.EffectiveDateChanging(NewDate, OldDate)
        Exit Sub
    End Sub

    Private Sub ctl_CPP_PropertyCoverages_NeedToClearCIMTransportation() Handles ctl_CPP_PropertyCoverages.NeedToClearCIMTransportation
        RaiseEvent NeedToClearCIMTransportation()
    End Sub

    Private Sub ctl_CPP_PropertyCoverages_NeedToReloadCIMTransportation() Handles ctl_CPP_PropertyCoverages.NeedToReloadCIMTransportation
        RaiseEvent NeedToReloadCIMTransportation()
    End Sub

    Public Sub PopulateCPRCoverages()
        Me.ctl_CPP_PropertyCoverages.LoadBlanketDeductible()
        Me.ctl_CPP_PropertyCoverages.PopulateBlanketDeductible()
    End Sub
End Class