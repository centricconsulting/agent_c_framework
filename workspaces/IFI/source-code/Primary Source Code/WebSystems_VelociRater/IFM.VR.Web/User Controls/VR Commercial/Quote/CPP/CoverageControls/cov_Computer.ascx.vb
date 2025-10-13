Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CIM

Public Class cov_Computer
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.cpDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerAllPerilsDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.cpValuation, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.cpCoinsurance, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerCoinsuranceTypeId, QuickQuoteStaticDataOption.SortBy.ValueAscending, Me.Quote.LobType)

        'Added 12/15/2021 for CPP Endorsements Task 66800 MLW
        If IsQuoteReadOnly() Then
            If Me.ddEqDeductible IsNot Nothing AndAlso Me.ddEqDeductible.Items IsNot Nothing Then
                If Me.ddEqDeductible.Items.FindByValue("250") Is Nothing Then
                    Me.ddEqDeductible.Items.Add(New ListItem("250", "250"))
                End If
                If Me.ddEqDeductible.Items.FindByValue("500") Is Nothing Then
                    Me.ddEqDeductible.Items.Add(New ListItem("500", "500"))
                End If
            End If
            If Me.ddMechanicalDeductible IsNot Nothing AndAlso Me.ddMechanicalDeductible.Items IsNot Nothing Then
                If Me.ddMechanicalDeductible.Items.FindByValue("250") Is Nothing Then
                    Me.ddMechanicalDeductible.Items.Add(New ListItem("250", "250"))
                End If
                If Me.ddMechanicalDeductible.Items.FindByValue("500") Is Nothing Then
                    Me.ddMechanicalDeductible.Items.Add(New ListItem("500", "500"))
                End If
            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Me.chkComputer.Checked = False
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If Me.chkComputer.Checked = False AndAlso String.IsNullOrWhiteSpace(sq.ComputerAllPerilsDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(sq.ComputerCoinsuranceTypeId) = False OrElse String.IsNullOrWhiteSpace(sq.ComputerValuationMethodTypeId) = False OrElse String.IsNullOrWhiteSpace(sq.ComputerEarthquakeVolcanicEruptionDeductible) = False OrElse String.IsNullOrWhiteSpace(sq.ComputerMechanicalBreakdownDeductible) = False OrElse hasBuildingComputers() = True Then
                Me.chkComputer.Checked = True

                'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                If IsQuoteReadOnly() Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.cpDeductible, sq.ComputerAllPerilsDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerAllPerilsDeductibleId)
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.cpValuation, sq.ComputerValuationMethodTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerValuationMethodTypeId)
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.cpCoinsurance, sq.ComputerCoinsuranceTypeId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ComputerCoinsuranceTypeId)
                Else
                    If String.IsNullOrWhiteSpace(sq.ComputerAllPerilsDeductibleId) = False Then
                        WebHelper_Personal.SetdropDownFromValue(Me.cpDeductible, sq.ComputerAllPerilsDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.cpDeductible, "8")
                    End If
                    WebHelper_Personal.SetdropDownFromValue(Me.cpValuation, sq.ComputerValuationMethodTypeId)
                    WebHelper_Personal.SetdropDownFromValue(Me.cpCoinsurance, sq.ComputerCoinsuranceTypeId)
                End If

                Me.chkEarthQuake.Checked = String.IsNullOrWhiteSpace(sq.ComputerEarthquakeVolcanicEruptionDeductible) = False
                WebHelper_Personal.SetdropDownFromValue(Me.ddEqDeductible, sq.ComputerEarthquakeVolcanicEruptionDeductible)

                Me.chkMechanicalBreakdown.Checked = String.IsNullOrWhiteSpace(sq.ComputerMechanicalBreakdownDeductible) = False
                WebHelper_Personal.SetdropDownFromValue(Me.ddMechanicalDeductible, sq.ComputerMechanicalBreakdownDeductible)
            End If
        Next

        'If String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerAllPerilsDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerCoinsuranceTypeId) = False OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerValuationMethodTypeId) = False OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerEarthquakeVolcanicEruptionDeductible) = False OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerMechanicalBreakdownDeductible) = False OrElse hasBuildingComputers() = True Then
        '    Me.chkComputer.Checked = True
        'Else
        '    Me.chkComputer.Checked = False
        'End If

        'If String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerAllPerilsDeductibleId) = False Then
        '    WebHelper_Personal.SetdropDownFromValue(Me.cpDeductible, GoverningStateQuote.ComputerAllPerilsDeductibleId)
        'Else
        '    WebHelper_Personal.SetdropDownFromValue(Me.cpDeductible, "8")
        'End If

        'WebHelper_Personal.SetdropDownFromValue(Me.cpValuation, GoverningStateQuote.ComputerValuationMethodTypeId)
        'WebHelper_Personal.SetdropDownFromValue(Me.cpCoinsurance, GoverningStateQuote.ComputerCoinsuranceTypeId)

        'Me.chkEarthQuake.Checked = String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerEarthquakeVolcanicEruptionDeductible) = False
        'WebHelper_Personal.SetdropDownFromValue(Me.ddEqDeductible, GoverningStateQuote.ComputerEarthquakeVolcanicEruptionDeductible)

        'Me.chkMechanicalBreakdown.Checked = String.IsNullOrWhiteSpace(GoverningStateQuote.ComputerMechanicalBreakdownDeductible) = False
        'WebHelper_Personal.SetdropDownFromValue(Me.ddMechanicalDeductible, GoverningStateQuote.ComputerMechanicalBreakdownDeductible)

        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If chkComputer.Checked Then
            ClearPolicyLevelInfoOnSubQuotes()
            SaveChildControls() 'Now saving Package parts on cov_Computer_Item
        Else
            ClearControl()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.chkComputer.Checked Then
            If String.IsNullOrEmpty(cpDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", cpDeductible.ClientID)
            End If

            If String.IsNullOrEmpty(cpCoinsurance.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Coinsurance", cpCoinsurance.ClientID)
            End If

            If String.IsNullOrEmpty(cpValuation.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Valuation", cpValuation.ClientID)
            End If
            If String.IsNullOrEmpty(ddEqDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Coinsurance", ddEqDeductible.ClientID)
            End If

            If String.IsNullOrEmpty(ddMechanicalDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Valuation", ddMechanicalDeductible.ClientID)
            End If
            Me.ValidateChildControls(valArgs)
        End If

    End Sub

    Public Overrides Sub ClearControl()
        ClearPolicyLevelInfoOnSubQuotes()
        Me.ClearChildControls()
    End Sub

    Public Sub ClearPolicyLevelInfoOnSubQuotes()
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            sq.ComputerAllPerilsDeductibleId = Nothing
            sq.ComputerCoinsuranceTypeId = Nothing
            sq.ComputerValuationMethodTypeId = Nothing
            sq.ComputerEarthquakeVolcanicEruptionDeductible = Nothing
            sq.ComputerMechanicalBreakdownDeductible = Nothing
        Next
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
        Return False 'PackageParts are added in cov_Computer_Item
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkComputer.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class