Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Public Class cov_Contractor_Item
    Inherits VRControlBase

    Private ReadOnly Property ContractorEquipmentCheckbox As CheckBox
        Get
            Dim ceBox As CheckBox = Parent.FindControl("chkContractorsEquipment")
            Return ceBox
        End Get
    End Property
    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If GoverningStateQuote() IsNot Nothing Then
            ContractorEquipmentCheckbox.Enabled = True

            If String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False AndAlso (GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any()) Or String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit) = False Then
                ContractorEquipmentCheckbox.Checked = True
            End If

            If (GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any()) AndAlso ContractorEquipmentCheckbox.Checked = True Then ' Or String.IsNullOrWhiteSpace(Quote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False Then
                Dim LimitFound As Boolean = False
                For Each ri As RepeaterItem In ceRepeater.Items
                    Dim txt As TextBox = ri.FindControl("txtLimit")
                    If txt IsNot Nothing Then
                        If txt.Text <> "" AndAlso IsNumeric(txt.Text) Then
                            LimitFound = True
                            Exit For
                        End If
                    End If
                Next
                If LimitFound Then Me.chkScheduledTools.Checked = True
            Else
                GoverningStateQuote.ContractorsEquipmentScheduledCoverages = New List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
                GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Add(New QuickQuoteContractorsEquipmentScheduledCoverage())
                GoverningStateQuote.ContractorsEquipmentScheduledCoverages(0).Description = "Scheduled Tools"
                Me.chkScheduledTools.Checked = False
            End If

            Me.ceRepeater.DataSource = GoverningStateQuote.ContractorsEquipmentScheduledCoverages
            Me.ceRepeater.DataBind()

            Me.txtRentedEquipment.Text = GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit

            If GoverningStateQuote.HasContractorsEnhancement Then ' do this last
                ContractorEquipmentCheckbox.Checked = True
                ContractorEquipmentCheckbox.Enabled = False
                chkScheduledTools.Checked = True
                chkScheduledTools.Enabled = False
                chkScheduledTools.ToolTip = "Required do to the inclusion of the Contractors Package Endorsement."
                divScheduledToolsDetail.Style.Add("display", "")
                If GoverningStateQuote.ContractorsEquipmentScheduledCoverages Is Nothing OrElse GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any() = False Then
                    GoverningStateQuote.ContractorsEquipmentScheduledCoverages = New List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
                    GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Add(New QuickQuoteContractorsEquipmentScheduledCoverage())
                    GoverningStateQuote.ContractorsEquipmentScheduledCoverages(0).Description = "Scheduled Tools"
                End If
            Else
                chkScheduledTools.Enabled = True
            End If

        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.chkScheduledTools.Checked Then
            Dim ceDeductible As DropDownList = Parent.FindControl("ceDeductible")
            Dim ceCoInsurance As DropDownList = Parent.FindControl("ceCoInsurance")
            Dim txtUnscheduledToolsLimit As TextBox = Parent.FindControl("txtUnscheduledToolsLimit")
            Dim totalLimit As Double = 0.0
            Dim scList As New List(Of QuickQuoteContractorsEquipmentScheduledCoverage)

            For Each item In ceRepeater.Items
                Dim txtLimit As TextBox = item.FindControl("txtLimit")
                Dim ceValuation As DropDownList = item.FindControl("ceValuation")
                Dim txtDescription As TextBox = item.FindControl("txtLocation")
                Dim sItem As New QuickQuoteContractorsEquipmentScheduledCoverage

                sItem.ManualLimitAmount = txtLimit.Text.Trim()
                sItem.ValuationMethodTypeId = ceValuation.SelectedValue
                sItem.Description = txtDescription.Text.Trim()

                scList.Add(sItem)

                totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(sItem.ManualLimitAmount)
            Next
            GoverningStateQuote.ContractorsEquipmentScheduledCoverages = scList


            Dim coInsuranceModifiers As New Dictionary(Of String, Double)
            coInsuranceModifiers.Add("5", 1.0) '80%
            coInsuranceModifiers.Add("6", 0.95) '90%
            coInsuranceModifiers.Add("7", 0.9) '100%

            If GoverningStateQuote.HasContractorsEnhancement Then
                ' rate is always .75
                GoverningStateQuote.ContractorsEquipmentScheduleRate = ".75"
            Else
                GoverningStateQuote.ContractorsEquipmentScheduleRate = (CIMHelper.ContractorEquipmentScheduledItemsRateTable.GetRateForLimit(totalLimit) * coInsuranceModifiers(ceCoInsurance.SelectedValue)).ToString()
            End If


            GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit = Me.txtRentedEquipment.Text.Trim()
            If (String.IsNullOrWhiteSpace(GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit) = False) Then
                GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersRate = CIMHelper.ContractorsEquipmentLeasedRentedFromOthersRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit))
            Else
                GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersRate = ""
            End If

            totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit) 'Matt A 6-26-15
            totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtUnscheduledToolsLimit.Text) 'Matt A 6-26-15

            'quote.ContractorsEquipmentLeasedRentedFromOthersRate = "1" - Matt A Add back after go live
            GoverningStateQuote.ContractorsEquipmentCatastropheLimit = totalLimit.ToString() '6-1-15

        Else
            GoverningStateQuote.ContractorsEquipmentScheduledCoverages = New List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
            'ClearControl()
        End If

        Return True
    End Function


    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        'added 10/12/2020 to re-use the same validationHelper; note: now being done from cov_Contractor.ascx.vb... calls Me.ValidationHelper.InsertFromOtherValidationHelper to copy in validations from cov_Contractor_Item
        'Dim valHelper As ControlValidationHelper = Nothing
        ''If Me.Parent IsNot Nothing AndAlso TypeOf Me.Parent Is cov_Contractor Then
        ''    valHelper = DirectCast(Me.Parent, cov_Contractor).ValidationHelper
        ''Else
        ''    valHelper = Me.ValidationHelper
        ''End If
        ''immediate parent is not cov_Contractor
        'Dim parent_cov_Contractor As cov_Contractor = Nothing
        'Dim keepLookingForCovContractorParent As Boolean = True
        'If Me.Parent IsNot Nothing Then
        '    Dim currParent As Control = Me.Parent
        '    Do Until keepLookingForCovContractorParent = False
        '        If currParent IsNot Nothing Then
        '            If TypeOf currParent Is cov_Contractor Then
        '                parent_cov_Contractor = DirectCast(currParent, cov_Contractor)
        '                keepLookingForCovContractorParent = False
        '            Else
        '                If currParent.Parent IsNot Nothing Then
        '                    currParent = currParent.Parent
        '                End If
        '            End If
        '        Else
        '            keepLookingForCovContractorParent = False
        '        End If
        '        If keepLookingForCovContractorParent = False Then
        '            Exit Do
        '        End If
        '    Loop
        'End If
        'If parent_cov_Contractor IsNot Nothing AndAlso parent_cov_Contractor.ValidationHelper IsNot Nothing Then
        '    valHelper = parent_cov_Contractor.ValidationHelper
        'Else
        '    valHelper = Me.ValidationHelper
        'End If

        Me.ValidationHelper.GroupName = "Contractor Equipment"
        'updated 10/12/2020
        'If String.IsNullOrWhiteSpace(valHelper.GroupName) = True Then
        '    valHelper.GroupName = "Contractor Equipment"
        'End If
        Dim deductibleControl As DropDownList = Parent.FindControl("ceDeductible")
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleControl.SelectedItem.Text)
        Dim ceCoInsuranceControl As DropDownList = Parent.FindControl("ceCoInsurance")
        Dim ceCoInsuranceAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(ceCoInsuranceControl.SelectedItem.Text)
        Dim unscheduledToolsControl As TextBox = Parent.FindControl("txtUnscheduledToolsLimit")
        Dim unscheduledToolsAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(unscheduledToolsControl.Text)
        Dim totalLimit As Double = 0.0
        Dim rentedLimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtRentedEquipment.Text)

        'added 11/9/2020 (Interoperability)
        Dim chkUnscheduledTools As CheckBox = Parent.FindControl("chkUnscheduledTools")
        If chkUnscheduledTools Is Nothing OrElse chkUnscheduledTools.Checked = False Then
            unscheduledToolsAmount = 0.0
        End If

        '3.8.120
        'If rentedLimitAmount > 150000 Then
        'updated 10/12/2020 (Interoperability project); changed from 150k to 500k
        If rentedLimitAmount > 500000 Then
            Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtRentedEquipment.ClientID)
            'valHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtRentedEquipment.ClientID)
        End If

        For Each ri As RepeaterItem In ceRepeater.Items
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
            Dim txtLocation As TextBox = ri.FindControl("txtLocation")
            Dim ceValuation As DropDownList = ri.FindControl("ceValuation")
            If Me.chkScheduledTools.Checked Then
                If String.IsNullOrEmpty(txtLimit.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit", txtLimit.ClientID)
                    'valHelper.AddError("Missing Limit", txtLimit.ClientID)
                End If
                If String.IsNullOrEmpty(txtLocation.Text) Then
                    Me.ValidationHelper.AddError("Missing  Description", txtLocation.ClientID)
                    'valHelper.AddError("Missing  Description", txtLocation.ClientID)
                End If
                If String.IsNullOrEmpty(ceValuation.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Valuation", ceValuation.ClientID)
                    'valHelper.AddError("Missing Valuation", ceValuation.ClientID)
                End If
                'Removed 6/24/2019 for Bug 28476 MLW
                '' 3.8.119
                'If LimitAmount > 150000 Then
                '    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
                'End If
                'added back in 10/19/2020 (Interoperability) w/ new limit
                If LimitAmount > 500000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
                End If
                ' 3.8.114
                If deductibleAmount >= LimitAmount Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimit.ClientID)
                    'valHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimit.ClientID)
                End If
                totalLimit += LimitAmount
                ' Total Limit - 3.8.113
            End If


            'If (totalLimit + rentedLimitAmount + unscheduledToolsAmount) > 1000000 Then 'removed 10/12/2020 (Interoperability project); now being done below once instead of on each line item
            '    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
            'End If
        Next

        'added 10/12/2020 (Interoperability project); now only validating total once instead of on each line item like was previously being done above
        If (totalLimit + rentedLimitAmount + unscheduledToolsAmount) > 1000000 Then
            Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.")
            'valHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.")
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddClick()
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Dim newItem As New QuickQuoteContractorsEquipmentScheduledCoverage()
            newItem.Description = String.Format("Scheduled Tools  #{0}", Me.GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Count)
            Me.GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Add(newItem)
            'Me.Quote.ContractorsEquipmentScheduledCoverages.AddNew()
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index)
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.ContractorsEquipmentScheduledCoverages.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub ceRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles ceRepeater.ItemCommand
        If e.CommandName = "lnkAdd" Then
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex)
        End If
    End Sub

    Private Sub ceRepeater_Add(sender As Object, e As RepeaterItemEventArgs) Handles ceRepeater.ItemDataBound

        Dim tool As Object = e.Item.DataItem

        Dim hardwareLimit As TextBox = e.Item.FindControl("txtHardwareLimit")
        Dim programLimit As TextBox = e.Item.FindControl("txtProgramsLimit")
        Dim businessLimit As TextBox = e.Item.FindControl("txtBusinessIncomeLimit")
        Dim chkApply As CheckBox = e.Item.FindControl("chkApply")

        Dim Index = e.Item.ItemIndex

        Dim Valuation As DropDownList = e.Item.FindControl("ceValuation")

        If Valuation IsNot Nothing Then
            ' added for bug 67868 Replacement Cost popup message 05/05/2022 BD
            Using popupSpecial As New PopupMessageClass.PopupMessageObject(Me.Page, "Replacement cost is only permitted on equipment that is 5 years old or newer.", "Replacement Cost Message")
                With popupSpecial
                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onchange
                    .DropDownValueToBindTo = "1"
                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                    .isModal = False
                    .AddButton("OK", True)
                    .width = 300
                    .height = 200
                    .AddControlToBindTo(Valuation)
                    .divId = "ValuationPopup"
                    .CreateDynamicPopUpWindow()
                End With
            End Using
            QQHelper.LoadStaticDataOptionsDropDown(Valuation, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        End If

        'Updated 2/7/2022 for bug 68706 MLW
        Dim indexBaseValue As Int32 = 0
        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task68706_CPP_InlandMarine_CE_Valuation_FirstItemFix")) = True Then
            indexBaseValue = -1
        End If
        If GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any() = True AndAlso Index > indexBaseValue AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Count > Index Then
            'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
            If IsQuoteReadOnly() Then
                WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Valuation, GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId)
            Else
                If GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId.IsNullEmptyorWhitespace() Then
                    Valuation.SelectedIndex = 0
                Else
                    Valuation.SelectedIndex = GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId
                End If
            End If
        End If
        'If GoverningStateQuote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Any() = True AndAlso Index > 0 AndAlso GoverningStateQuote.ContractorsEquipmentScheduledCoverages.Count > Index Then
        '    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
        '    If IsQuoteReadOnly() Then
        '        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Valuation, GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId)
        '    Else
        '        If GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId.IsNullEmptyorWhitespace() Then
        '            Valuation.SelectedIndex = 0
        '        Else
        '            Valuation.SelectedIndex = GoverningStateQuote.ContractorsEquipmentScheduledCoverages(Index).ValuationMethodTypeId
        '        End If
        '    End If
        'End If


    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.ContractorsEquipmentScheduleDeductibleId = ""
        GoverningStateQuote.ContractorsEquipmentScheduleCoinsuranceTypeId = ""
        GoverningStateQuote.ContractorsEquipmentScheduledCoverages = New List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
        GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersLimit = ""
        GoverningStateQuote.ContractorsEquipmentLeasedRentedFromOthersRate = ""
        GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = ""
        GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = ""
        GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = ""
        GoverningStateQuote.ContractorsEquipmentSmallToolsEndorsementPerToolLimit = ""
        GoverningStateQuote.SmallToolsDeductibleId = ""
        GoverningStateQuote.SmallToolsAnyOneLossCatastropheLimit = ""
        GoverningStateQuote.SmallToolsLimit = ""
        GoverningStateQuote.SmallToolsRate = ""
        GoverningStateQuote.ContractorsEquipmentScheduleRate = ""
        GoverningStateQuote.ContractorsEquipmentCatastropheLimit = "" '6-1-15
    End Sub

End Class