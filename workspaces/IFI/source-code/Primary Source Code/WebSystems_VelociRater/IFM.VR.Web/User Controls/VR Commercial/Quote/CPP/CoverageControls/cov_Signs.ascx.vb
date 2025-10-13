Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CIM

Public Class cov_Signs
    Inherits VRControlBase


    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.siDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SignsDeductibleId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.siValuation, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SignsValuationMethodTypeId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Me.chkSigns.Checked = False
        siDeductible.SelectedValue = Nothing
        siValuation.SelectedValue = Nothing
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If chkSigns.Checked = False AndAlso String.IsNullOrWhiteSpace(sq.SignsDeductibleId) = False Then
                Me.chkSigns.Checked = True
                If String.IsNullOrWhiteSpace(sq.SignsDeductibleId) = False Then
                    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.siDeductible, sq.SignsDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SignsDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.siDeductible, sq.SignsDeductibleId)
                    End If
                End If

                If String.IsNullOrWhiteSpace(sq.SignsValuationMethodTypeId) = False Then
                    WebHelper_Personal.SetdropDownFromValue(Me.siValuation, sq.SignsValuationMethodTypeId)
                End If

                siDeductible.SelectedValue = sq.SignsDeductibleId
                siValuation.SelectedValue = sq.SignsValuationMethodTypeId
            End If
        Next

        'Me.chkSigns.Checked = False
        'If Quote IsNot Nothing Then
        '    If String.IsNullOrWhiteSpace(GoverningStateQuote.SignsDeductibleId) = False AndAlso cov_Signs_Item.HasSignItems Then
        '        Me.chkSigns.Checked = True
        '    Else
        '        Me.chkSigns.Checked = False
        '    End If

        '    If String.IsNullOrWhiteSpace(GoverningStateQuote.SignsDeductibleId) = False Then
        '        WebHelper_Personal.SetdropDownFromValue(Me.siDeductible, GoverningStateQuote.SignsDeductibleId)
        '    End If

        '    If String.IsNullOrWhiteSpace(GoverningStateQuote.SignsValuationMethodTypeId) = False Then
        '        WebHelper_Personal.SetdropDownFromValue(Me.siValuation, GoverningStateQuote.SignsValuationMethodTypeId)
        '    End If
        'End If
        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.chkSigns.Checked Then
            'GoverningStateQuote.SignsDeductibleId = siDeductible.SelectedValue
            'GoverningStateQuote.SignsValuationMethodTypeId = siValuation.SelectedValue
            ClearPolicyLevelInfoOnSubQuotes()
            Me.SaveChildControls()
        Else
            ClearControl()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.chkSigns.Checked Then
            Me.ValidationHelper.GroupName = "Signs"
            If String.IsNullOrEmpty(siDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", siDeductible.ClientID)
            End If
            If String.IsNullOrEmpty(siValuation.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Valuation", siValuation.ClientID)
            End If
            If hasCheckedLocationsOnChildControls() = False Then
                Me.ValidationHelper.AddError("Missing Signs Location", divSignsOption.ClientID)
            End If
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        ClearPolicyLevelInfoOnSubQuotes()
        Me.ClearChildControls()
    End Sub

    Public Sub ClearPolicyLevelInfoOnSubQuotes()
        Quote.SignsDeductibleId = Nothing
        Quote.SignsValuationMethodTypeId = Nothing
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            sq.SignsDeductibleId = Nothing
            sq.SignsValuationMethodTypeId = Nothing
        Next
    End Sub

    Public Overrides Function hasSetting() As Boolean
        Return False 'We are setting packageparts in cov_Signs_Item_details
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkSigns.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub

    Public Function hasCheckedLocationsOnChildControls() As Boolean
        Dim counter As Int16 = 0
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            Dim repeater As Repeater = i.FindControl("siRepeater")
            For Each ri As RepeaterItem In repeater.Items
                Dim chkApply As CheckBox = ri.FindControl("chkApply")
                If chkApply.Checked Then
                    counter = counter + 1
                End If
            Next
        Next
        Return counter > 0
    End Function

    Private Sub cov_Signs_Item_Details_AddSignsPolicyItems(ThisState As QuickQuoteObject) Handles cov_Signs_Item.AddSignsPolicyLevelItems
        ThisState.CPP_Has_InlandMarine_PackagePart = True

        ThisState.SignsDeductibleId = siDeductible.SelectedValue
        ThisState.SignsValuationMethodTypeId = siValuation.SelectedValue

    End Sub
End Class