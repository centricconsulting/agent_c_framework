Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_InstallationFloater
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ifDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InstallationBlanketDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ifCoinsurance, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InstallationBlanketCoinsuranceTypeId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        chkInstallationFloater.Enabled = True
        If String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit) = False Or GoverningStateQuote.HasContractorsEnhancement Then
            Me.chkInstallationFloater.Checked = True
            Me.withoutPackage.Visible = True
            Me.withoutPackageLabel.Visible = True

            If String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketDeductibleId) = False Then
                'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                If IsQuoteReadOnly() Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ifDeductible, GoverningStateQuote.InstallationBlanketDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InstallationBlanketDeductibleId)
                Else
                    WebHelper_Personal.SetdropDownFromValue(Me.ifDeductible, GoverningStateQuote.InstallationBlanketDeductibleId)
                End If
            End If

            'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
            If IsQuoteReadOnly() Then
                If String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketCoinsuranceTypeId) = False Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ifCoinsurance, GoverningStateQuote.InstallationBlanketCoinsuranceTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.InstallationBlanketCoinsuranceTypeId)
                End If
            Else
                If String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketCoinsuranceTypeId) = False Then
                    WebHelper_Personal.SetdropDownFromValue(Me.ifCoinsurance, GoverningStateQuote.InstallationBlanketCoinsuranceTypeId)
                Else
                    If GoverningStateQuote.HasContractorsEnhancement Then
                        If String.IsNullOrWhiteSpace(GoverningStateQuote.InstallationBlanketCoinsuranceTypeId) = False Then
                            WebHelper_Personal.SetdropDownFromValue(Me.ifCoinsurance, "7")
                        End If
                    End If
                End If
            End If
            If GoverningStateQuote.HasContractorsEnhancement Then
                Me.txtIncreasedLimit.Text = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.InstallationBlanketLimit) > 10000, IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.InstallationBlanketLimit) - 10000, "")
                Me.txtCatLimitWith.Text = GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit
                Me.withPackage.Visible = True
                Me.withPackageLabel.Visible = True
                Me.withoutPackage.Visible = False
                Me.withoutPackageLabel.Visible = False
                chkInstallationFloater.Enabled = False
                divInstallationFloaterDetail.Style.Add("display", "")
            Else
                Me.txtJobSiteLimit.Text = GoverningStateQuote.InstallationBlanketLimit
                Me.txtCatLimitWithout.Text = GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit
                Me.withPackage.Visible = False
                Me.withPackageLabel.Visible = False
                Me.withoutPackage.Visible = True
                Me.withoutPackageLabel.Visible = True
            End If

        Else
            Me.chkInstallationFloater.Checked = False
            Me.withoutPackage.Visible = True
            Me.withoutPackageLabel.Visible = True
        End If


    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If chkInstallationFloater.Checked Then
            GoverningStateQuote.InstallationBlanketDeductibleId = Me.ifDeductible.SelectedValue
            GoverningStateQuote.InstallationBlanketCoinsuranceTypeId = Me.ifCoinsurance.SelectedValue

            If GoverningStateQuote.HasContractorsEnhancement Then '6-1-15
                GoverningStateQuote.InstallationBlanketLimit = (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtIncreasedLimit.Text.Trim()) + 10000).ToString()
            Else
                GoverningStateQuote.InstallationBlanketLimit = Me.txtJobSiteLimit.Text.Trim()
            End If

            'This 3.0 comes from the old look code.
            Dim catLimit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.InstallationBlanketLimit) * 3.0
            GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit = catLimit.ToString()

            Dim rate As Double = 0.0
            If GoverningStateQuote.HasContractorsEnhancement = False Then
                rate = CIMHelper.InstallationFloaterRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.InstallationBlanketLimit))
                Select Case Me.ifCoinsurance.SelectedValue
                    Case "6" '90%
                        rate = rate * 0.95
                    Case "7" '100%
                        rate = rate * 0.9
                    Case Else
                        '80% do nothing
                End Select
            Else
                rate = 0.55
            End If

            GoverningStateQuote.InstallationBlanketRate = rate
        Else
            ClearControl()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If chkInstallationFloater.Checked Then
            Me.ValidationHelper.GroupName = "Fine Arts Floater"
            Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ifDeductible.SelectedItem.ToString)

            Dim limitJobSiteAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtJobSiteLimit.Text)

            Dim catAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit)

            If String.IsNullOrEmpty(ifDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", ifDeductible.ClientID)
            End If

            If String.IsNullOrEmpty(ifCoinsurance.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", ifCoinsurance.ClientID)
            End If

            If GoverningStateQuote.HasContractorsEnhancement = False Then
                If String.IsNullOrEmpty(Me.txtJobSiteLimit.Text) Then
                    Me.ValidationHelper.AddError("Missing Jobsite Limit", txtJobSiteLimit.ClientID)
                End If
                'If limitJobSiteAmount > 50000 Then
                'updated 10/19/2020 (Interoperability)
                If limitJobSiteAmount > 100000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtJobSiteLimit.ClientID)
                End If
                'If catAmount > 150000 Then
                'updated 10/19/2020 (Interoperability)
                If catAmount > 300000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtCatLimitWithout.ClientID)
                End If
                If deductibleAmount >= limitJobSiteAmount Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtJobSiteLimit.ClientID)
                End If
            Else
                'If catAmount > 150000 Then
                'updated 10/19/2020 (Interoperability)
                If catAmount > 300000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtCatLimitWith.ClientID)
                End If

            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.InstallationBlanketDeductibleId = ""
        GoverningStateQuote.InstallationBlanketCoinsuranceTypeId = ""
        GoverningStateQuote.InstallationBlanketLimit = ""
        GoverningStateQuote.InstallationBlanketAnyOneLossCatastropheLimit = ""
        GoverningStateQuote.InstallationBlanketRate = ""
    End Sub

    Public Function hasBuildingComputers() As Boolean
        Dim buildingDataPresent As Boolean = False

        If Quote IsNot Nothing Then
            If Quote.Locations IsNot Nothing Then
                For Each location In Quote.Locations
                    If location.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In location.Buildings
                            If String.IsNullOrEmpty(building.ComputerHardwareLimit) = False OrElse String.IsNullOrEmpty(building.ComputerProgramsApplicationsAndMediaLimit) = False OrElse String.IsNullOrEmpty(building.ComputerBusinessIncomeLimit) = False Then
                                buildingDataPresent = True
                            End If
                        Next
                    End If
                Next
            End If
        End If
        Return buildingDataPresent
    End Function

    Public Overrides Function hasSetting() As Boolean
        Return Me.chkInstallationFloater.Checked
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkInstallationFloater.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class