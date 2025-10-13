Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_BuildersRisk
    Inherits VRControlBase

    Private _IM_Parent As ctl_CPP_InlandMarine
    Public Property IM_Parent() As ctl_CPP_InlandMarine
        Get
            Return _IM_Parent
        End Get
        Set(ByVal value As ctl_CPP_InlandMarine)
            _IM_Parent = value
        End Set
    End Property

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.brDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerAllPerilsDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If String.IsNullOrWhiteSpace(GoverningStateQuote.BuildersRiskDeductibleId) = False AndAlso (GoverningStateQuote.BuildersRiskScheduledLocations IsNot Nothing AndAlso GoverningStateQuote.BuildersRiskScheduledLocations.Any()) Then
            Me.chkBuildersRisk.Checked = True
        Else
            Me.chkBuildersRisk.Checked = False
        End If

        If String.IsNullOrWhiteSpace(GoverningStateQuote.BuildersRiskDeductibleId) = False Then
            'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
            If IsQuoteReadOnly() Then
                WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.brDeductible, GoverningStateQuote.BuildersRiskDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerAllPerilsDeductibleId)
            Else
                WebHelper_Personal.SetdropDownFromValue(Me.brDeductible, GoverningStateQuote.BuildersRiskDeductibleId)
            End If
        End If

        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.CreateJSBinding(Me.chkBuildersRisk, ctlPageStartupScript.JsEventType.onclick, "return doClearItem();", False)
        'Me.chkBuildersRisk.Attributes.Add("onclick", "return doClearItem(this)")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If chkBuildersRisk.Checked Then
            GoverningStateQuote.BuildersRiskDeductibleId = brDeductible.SelectedValue
            Me.SaveChildControls()
        Else
            ClearControl()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If chkBuildersRisk.Checked Then
            If String.IsNullOrEmpty(brDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", brDeductible.ClientID)
            End If
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.BuildersRiskDeductibleId = Nothing
        Me.ClearChildControls()
    End Sub

    'Public Sub clearSelf(ByVal sender As Object, ByVal e As EventArgs) Handles chkBuildersRisk.CheckedChanged
    '    If chkBuildersRisk.Checked = False Then
    '        'ClearControl()
    '        'Populate()
    '    End If
    'End Sub

    Public Overrides Function hasSetting() As Boolean
        Return Me.chkBuildersRisk.Checked
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkBuildersRisk.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class