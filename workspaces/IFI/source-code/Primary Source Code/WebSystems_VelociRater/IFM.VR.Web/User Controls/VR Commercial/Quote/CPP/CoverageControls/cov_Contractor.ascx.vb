Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_Contractor
    Inherits VRControlBase

    Private ReadOnly Property chkScheduledTools As CheckBox
        Get
            Dim ceBox As CheckBox = cov_Contractor_Item.FindControl("chkScheduledTools")
            Return ceBox
        End Get
    End Property

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ceDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SmallToolsDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ceCoinsurance, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ContractorsEquipmentScheduleCoinsuranceTypeId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If GoverningStateQuote() IsNot Nothing Then
            chkContractorsEquipment.Enabled = True
            'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
            If IsQuoteReadOnly() Then
                'deductible can come from any one of three places
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId) = False Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ceDeductible, GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SmallToolsDeductibleId)
                Else
                    If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId) = False Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ceDeductible, GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SmallToolsDeductibleId)
                    Else
                        If String.IsNullOrWhiteSpace(GoverningStateQuote.SmallToolsDeductibleId) = False Then
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ceDeductible, GoverningStateQuote.SmallToolsDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.SmallToolsDeductibleId)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ceCoinsurance, GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ContractorsEquipmentScheduleCoinsuranceTypeId)
                End If
            Else
                'deductible can come from any one of three places
                If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId) = False Then
                    WebHelper_Personal.SetdropDownFromValue(Me.ceDeductible, GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId)
                Else
                    If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId) = False Then
                        WebHelper_Personal.SetdropDownFromValue(Me.ceDeductible, GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId)
                    Else
                        If String.IsNullOrWhiteSpace(GoverningStateQuote.SmallToolsDeductibleId) = False Then
                            WebHelper_Personal.SetdropDownFromValue(Me.ceDeductible, GoverningStateQuote.SmallToolsDeductibleId)
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Then
                    WebHelper_Personal.SetdropDownFromValue(Me.ceCoinsurance, GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId)
                End If
            End If

            If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Then
                ' Other small tools - Enhancement
                Me.chkUnscheduledTools.Checked = True
                Me.txtUnscheduledToolsLimit.Text = GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit ' quote.SmallToolsAnyOneLossCatastropheLimit ' quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit
            Else
                'true small tools floater
                If String.IsNullOrWhiteSpace(GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit) = False Then
                    Me.chkUnscheduledTools.Checked = True
                    Me.txtUnscheduledToolsLimit.Text = GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit
                End If
            End If

            'Me.chkContractorsEquipment.Checked = False


            If GoverningStateQuote.HasContractorsEnhancement Then ' do this last
                chkContractorsEquipment.Checked = True
                chkContractorsEquipment.Enabled = False
                divContractorDetail.Style.Add("display", "")
                chkContractorsEquipment.ToolTip = "Required do to the inclusion of the Contractors Package Endorsement."
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
        If chkContractorsEquipment.Checked Then
            GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId = ceDeductible.SelectedValue

            If Me.chkUnscheduledTools.Checked Then
                If chkScheduledTools.Checked Then
                    'Non-small tools floater - Only valid if scheduled tools is selected
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = Me.txtUnscheduledToolsLimit.Text.Trim() '"2000" 'Me.txtUnscheduledToolsLimit.Text.Trim()
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerToolLimit = "2000"

                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = CIMHelper.ContractorEquipmentUnScheduledToolsRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit))

                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = Me.ceDeductible.SelectedValue

                    'START clear out small tools floater
                    GoverningStateQuote.SmallToolsDeductibleId = ""
                    GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit = ""
                    GoverningStateQuote.SmallToolsLimit = ""
                    GoverningStateQuote.SmallToolsRate = ""
                    GoverningStateQuote.SmallToolsIsEmployeeTools = False
                    GoverningStateQuote.SmallToolsIsToolsLeasedOrRented = False
                    'END clear out small tools floater
                Else
                    'small tools floater - only valid when scheduled tools is not selected
                    GoverningStateQuote.SmallToolsDeductibleId = Me.ceDeductible.SelectedValue
                    GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit = Me.txtUnscheduledToolsLimit.Text
                    GoverningStateQuote.SmallToolsLimit = "2000"
                    GoverningStateQuote.SmallToolsIsEmployeeTools = True
                    GoverningStateQuote.SmallToolsIsToolsLeasedOrRented = True

                    GoverningStateQuote.SmallToolsRate = CIMHelper.ContractorEquipmentUnScheduledToolsRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Quote.SmallToolsLimit))


                    'START clear out other small tools field
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = ""
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerToolLimit = ""
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = ""
                    GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = ""
                    ''END clear out other small tools field
                End If

            Else
                'quote.SmallToolsAnyOneLossCatastropheLimit = ""
                GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = ""
                GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = ""
                GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = ""
                GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerToolLimit = ""

                GoverningStateQuote.SmallToolsDeductibleId = ""
                GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit = ""
                GoverningStateQuote.SmallToolsLimit = ""
                GoverningStateQuote.SmallToolsRate = ""
                GoverningStateQuote.SmallToolsIsEmployeeTools = False
                GoverningStateQuote.SmallToolsIsToolsLeasedOrRented = False

            End If

            If chkScheduledTools.Checked Or chkUnscheduledTools.Checked Then
                GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId = Me.ceCoinsurance.SelectedValue
            Else
                GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId = Nothing
            End If

            Me.SaveChildControls()
        Else
            ClearControl()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.chkContractorsEquipment.Checked Then
            Me.ValidationHelper.GroupName = "Contractor Equipment" 'added 10/12/2020 (Interoperability project)

            If chkUnscheduledTools.Checked Then
                Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ceDeductible.SelectedItem.ToString)
                If String.IsNullOrEmpty(ceDeductible.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Deductible", ceDeductible.ClientID)
                End If

                If String.IsNullOrEmpty(ceCoinsurance.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Coinsurance", ceCoinsurance.ClientID)
                End If
                ' 3.8.121
                'If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtUnscheduledToolsLimit.Text) > 150000 And chkUnscheduledTools.Checked Then
                'updated 10/12/2020 (Interoperability project); changed from 150k to 500k
                If chkUnscheduledTools.Checked AndAlso IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtUnscheduledToolsLimit.Text) > 500000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtUnscheduledToolsLimit.ClientID)
                End If
                If deductibleAmount >= IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtUnscheduledToolsLimit.Text) Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtUnscheduledToolsLimit.ClientID)
                End If
                'If String.IsNullOrEmpty(txtUnscheduledToolsLimit.Text) And chkUnscheduledTools.Checked Then
                'updated 10/12/2020 (Interoperability project)
                If chkUnscheduledTools.Checked AndAlso String.IsNullOrEmpty(txtUnscheduledToolsLimit.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit", txtUnscheduledToolsLimit.ClientID)
                End If
            End If

            Me.ValidateChildControls(valArgs)

            'added 10/12/2020 so that only one ValidationHelper will be used by parent and child to keep all CE validations together in the same section
            If Me.cov_Contractor_Item IsNot Nothing AndAlso Me.cov_Contractor_Item.ValidationHelper IsNot Nothing Then
                Me.ValidationHelper.InsertFromOtherValidationHelper(Me.cov_Contractor_Item.ValidationHelper)
            End If
        End If

    End Sub

    Public Overrides Sub ClearControl()
        chkContractorsEquipment.Checked = False
        GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId = Nothing
        GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId = Nothing
        GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = Nothing
        GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit = Nothing
        Me.ClearChildControls()
    End Sub

    Public Overrides Function hasSetting() As Boolean
        Return Me.chkContractorsEquipment.Checked
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkContractorsEquipment.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class