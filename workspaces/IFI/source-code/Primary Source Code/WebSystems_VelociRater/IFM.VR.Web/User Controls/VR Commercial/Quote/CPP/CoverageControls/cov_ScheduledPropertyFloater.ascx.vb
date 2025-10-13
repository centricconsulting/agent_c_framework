Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_ScheduledPropertyFloater
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.spDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ScheduledPropertyDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.spCoinsurance, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ScheduledPropertyCoinsuranceTypeId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Quote IsNot Nothing Then
            If String.IsNullOrWhiteSpace(GoverningStateQuote.ScheduledPropertyDeductibleId) = False AndAlso GoverningStateQuote.ScheduledPropertyItems IsNot Nothing AndAlso GoverningStateQuote.ScheduledPropertyItems.Any() Then
                Me.chkScheduledPropertyFloater.Checked = True
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ScheduledPropertyDeductibleId) = False Then
                    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.spDeductible, GoverningStateQuote.ScheduledPropertyDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ScheduledPropertyDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.spDeductible, GoverningStateQuote.ScheduledPropertyDeductibleId)
                    End If
                End If
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ScheduledPropertyCoinsuranceTypeId) = False Then
                    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.spCoinsurance, GoverningStateQuote.ScheduledPropertyCoinsuranceTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ScheduledPropertyCoinsuranceTypeId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.spCoinsurance, GoverningStateQuote.ScheduledPropertyCoinsuranceTypeId)
                    End If
                End If
            Else
                Me.chkScheduledPropertyFloater.Checked = False
            End If
        End If

        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.chkScheduledPropertyFloater.Checked Then
            If Quote IsNot Nothing Then
                GoverningStateQuote.ScheduledPropertyDeductibleId = Me.spDeductible.SelectedValue
                GoverningStateQuote.ScheduledPropertyCoinsuranceTypeId = Me.spCoinsurance.SelectedValue
                Me.SaveChildControls()
            End If
        Else
            ClearControl()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.chkScheduledPropertyFloater.Checked Then
            Me.ValidationHelper.GroupName = "Scheduled Property Floater"
            If String.IsNullOrEmpty(spDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", spDeductible.ClientID)
            End If

            If String.IsNullOrEmpty(spCoinsurance.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Coinsurance", spCoinsurance.ClientID)
            End If

            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.ScheduledPropertyDeductibleId = ""
        GoverningStateQuote.ScheduledPropertyCoinsuranceTypeId = ""
        Me.ClearChildControls()
    End Sub

    Public Overrides Function hasSetting() As Boolean
        Return Me.chkScheduledPropertyFloater.Checked
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkScheduledPropertyFloater.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class