Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CIM

Public Class cov_FineArtsFloater
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.faDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Me.chkFineArtsFloater.Checked = False
        Me.chkBreakage.Checked = False
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If Me.chkFineArtsFloater.Checked = False AndAlso String.IsNullOrWhiteSpace(sq.FineArtsDeductibleId) = False AndAlso (sq.FineArtsBreakageMarringOrScratching = True OrElse cov_FineArtsFloater_Item.HasArtItems) Then
                Me.chkFineArtsFloater.Checked = True

                If String.IsNullOrWhiteSpace(sq.FineArtsDeductibleId) = False Then
                    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.faDeductible, sq.FineArtsDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId)
                        Dim faDeductibleType As String
                        Select Case sq.FineArtsDeductibleCategoryTypeId
                            Case 3
                                faDeductibleType = "All Perils"
                            Case 8
                                faDeductibleType = "Specified Perils"
                            Case Else
                                faDeductibleType = "All Perils"
                        End Select
                        txtDeductibleType.Text = faDeductibleType
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.faDeductible, sq.FineArtsDeductibleId)
                    End If
                End If

                chkBreakage.Checked = sq.FineArtsBreakageMarringOrScratching

            End If
        Next

        'Me.chkFineArtsFloater.Checked = False
        'If GoverningStateQuote() IsNot Nothing Then
        '    If String.IsNullOrWhiteSpace(GoverningStateQuote.FineArtsDeductibleId) = False AndAlso (GoverningStateQuote.FineArtsBreakageMarringOrScratching = True OrElse cov_FineArtsFloater_Item.HasArtItems) Then
        '        Me.chkFineArtsFloater.Checked = True
        '    Else
        '        Me.chkFineArtsFloater.Checked = False
        '    End If

        '    If String.IsNullOrWhiteSpace(GoverningStateQuote.FineArtsDeductibleId) = False Then
        '        WebHelper_Personal.SetdropDownFromValue(Me.faDeductible, GoverningStateQuote.FineArtsDeductibleId)
        '    End If

        '    chkBreakage.Checked = GoverningStateQuote.FineArtsBreakageMarringOrScratching
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
        If Me.chkFineArtsFloater.Checked Then
            'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            '    sq.FineArtsDeductibleId = Me.faDeductible.SelectedValue
            '    sq.FineArtsBreakageMarringOrScratching = chkBreakage.Checked
            '    'The following is hardcoded by request. BRD 3.8.43
            '    sq.FineArtsDeductibleCategoryTypeId = "3" 'Value for "All Perils"

            'Next
            ClearPolicyLevelInfoOnSubQuotes()
            Me.SaveChildControls()
        Else
            ClearControl()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.chkFineArtsFloater.Checked Then
            Me.ValidationHelper.GroupName = "Fine Arts Floater"
            If String.IsNullOrEmpty(faDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", faDeductible.ClientID)
            End If
            'iterate child controls and add "chkApply"  If master is checked but no locations add validation errror
            If hasCheckedLocationsOnChildControls() = False Then
                Me.ValidationHelper.AddError("Missing Fine Arts Location", divFineArtsFloaterOption.ClientID)
            End If
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        'GoverningStateQuote.FineArtsBreakageMarringOrScratching = False
        'GoverningStateQuote.FineArtsDeductibleId = Nothing
        'GoverningStateQuote.FineArtsDeductibleCategoryTypeId = Nothing
        ClearPolicyLevelInfoOnSubQuotes()
        Me.ClearChildControls()
    End Sub

    Public Sub ClearPolicyLevelInfoOnSubQuotes()
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            sq.FineArtsBreakageMarringOrScratching = False
            sq.FineArtsDeductibleId = Nothing
            sq.FineArtsDeductibleCategoryTypeId = Nothing
        Next
    End Sub

    Public Overrides Function hasSetting() As Boolean
        Return False 'We are setting packageparts in cov_FineArtsFloater_Item_details
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkFineArtsFloater.Checked = False Then
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
            Dim repeater As Repeater = i.FindControl("faRepeater")
            For Each ri As RepeaterItem In repeater.Items
                Dim chkApply As CheckBox = ri.FindControl("chkApply")
                If chkApply.Checked Then
                    counter = counter + 1
                End If
            Next
        Next
        Return counter > 0
    End Function

    Private Sub cov_FineArtsFloater_Item_Details_AddFineArtsPolicyItems(ThisState As QuickQuoteObject) Handles cov_FineArtsFloater_Item.AddFineArtsPolicyLevelItems

        ThisState.CPP_Has_InlandMarine_PackagePart = True

        ThisState.FineArtsDeductibleId = Me.faDeductible.SelectedValue
        ThisState.FineArtsBreakageMarringOrScratching = chkBreakage.Checked
        'The following is hardcoded by request. BRD 3.8.43
        ThisState.FineArtsDeductibleCategoryTypeId = "3" 'Value for "All Perils"
    End Sub
End Class