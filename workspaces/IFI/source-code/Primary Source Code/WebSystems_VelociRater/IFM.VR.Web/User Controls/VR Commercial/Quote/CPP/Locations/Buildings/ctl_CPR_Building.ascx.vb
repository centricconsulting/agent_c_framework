Imports IFM.PrimativeExtensions
Public Class ctl_CPR_Building
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Public ReadOnly Property ScrollToControlId
        Get
            Return lblAccordHeader.ClientID
        End Get
    End Property

    ''' <summary>
    ''' BUILDING NUMBER is the number of the building in a list of all buildings.
    ''' This is used when setting the defaults on the building coverages
    ''' ZERO BASED
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property BuildingNumber As Int32
        Get
            If Quote IsNot Nothing Then
                Dim LocNdx As Integer = -1
                Dim BldNdx As Integer = -1
                Dim BldNumber As Integer = -1

                ' Loop through all the buildings on the quote and number them
                If Quote.Locations IsNot Nothing Then
                    For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                        BldNdx = -1
                        LocNdx += 1
                        If L.Buildings IsNot Nothing Then
                            For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                BldNdx += 1
                                BldNumber += 1
                                If LocationIndex = LocNdx AndAlso BuildingIndex = BldNdx Then
                                    ' Return the number of the building that matches this control's building
                                    Return BldNumber
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                    ' If we got here we didn't find the building
                    Return -1
                Else
                    ' Locations is nothing
                    Return -1
                End If
            Else
                ' Quote is nothing
                Return -1
            End If
        End Get
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public ReadOnly Property BlanketCauseOfLossId As String
        Get
            If Quote IsNot Nothing Then
                If Quote.HasBlanketBuilding Then
                    Return Quote.BlanketBuildingCauseOfLossTypeId
                ElseIf Quote.HasBlanketBuildingAndContents Then
                    Return Quote.BlanketBuildingAndContentsCauseOfLossTypeId
                ElseIf Quote.HasBlanketContents Then
                    Return Quote.BlanketContentsCauseOfLossTypeId
                Else
                    Return MyBuilding.CauseOfLossTypeId
                End If
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property BlanketCoinsuranceId As String
        Get
            If Quote IsNot Nothing Then
                If Quote.HasBlanketBuilding Then
                    Return Quote.BlanketBuildingCoinsuranceTypeId
                End If
                If Quote.HasBlanketBuildingAndContents Then
                    Return Quote.BlanketBuildingAndContentsCoinsuranceTypeId
                End If
                If Quote.HasBlanketContents Then
                    Return Quote.BlanketContentsCoinsuranceTypeId
                End If
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property BlanketValuationId As String
        Get
            If Quote IsNot Nothing Then
                If Quote.HasBlanketBuilding Then
                    Return Quote.BlanketBuildingValuationId
                End If
                If Quote.HasBlanketBuildingAndContents Then
                    Return Quote.BlanketBuildingAndContentsValuationId
                End If
                If Quote.HasBlanketContents Then
                    Return Quote.BlanketContentsValuationId
                End If
            End If
            Return ""
        End Get
    End Property

    Public Property BCUseSpecificVisible As Boolean
        Get
            Return ViewState("vs_BCUseSpecificVisible")
        End Get
        Set(value As Boolean)
            ViewState("vs_BCUseSpecificVisible") = value
        End Set
    End Property

    Private Property BICUseSpecificVisible As Boolean
        Get
            Return ViewState("BICUseSpecificVisible")
        End Get
        Set(value As Boolean)
            ViewState("BICUseSpecificVisible") = value
        End Set
    End Property

    Private Property PPCUseSpecificVisible As Boolean
        Get
            Return ViewState("PPCUseSpecificVisible")
        End Get
        Set(value As Boolean)
            ViewState("PPCUseSpecificVisible") = value
        End Set
    End Property

    Private Property PPOUseSpecificVisible As Boolean
        Get
            Return ViewState("PPOUseSpecificVisible")
        End Get
        Set(value As Boolean)
            ViewState("PPOUseSpecificVisible") = value
        End Set
    End Property

    Private Enum CPRBuildingCoverageType
        BuildingCoverage
        BusinessIncomeCoverage
        PersonalPropertyCoverage
        PersonalPropertyOfOthersCoverage
    End Enum

    Private Structure DefaultBuildingCoverageValues_struct
        Public CauseOfLossId As String
        Public CoinsuranceId As String
        Public ValuationId As String
        Public DeductibleId As String
        Public GroupIValue As String
        Public GroupIIValue As String
        Public AgreedAmountValue As Boolean
        Public AgreedAmountEnabled As Boolean
    End Structure
    Private DefaultBCValues As DefaultBuildingCoverageValues_struct
    Private DefaultBICValues As DefaultBuildingCoverageValues_struct
    Private DefaultPPCValues As DefaultBuildingCoverageValues_struct
    Private DefaultPPOValues As DefaultBuildingCoverageValues_struct

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return BuildingIndex
        End Get
    End Property

    Public Event NewBuildingRequested(ByVal LocIndex As Integer)
    Public Event DeleteBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
    Public Event ClearBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divClassCodeLookup.ClientID, hdnCCAccord, "0")
        Me.VRScript.CreateAccordion(divCovs.ClientID, hdnCovsAccord, "0")

        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Building?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)

        SetDefaultBuildingCoverageValues()

        ' Building Coverage script variables
        Me.VRScript.AddVariableLine("Cpr.BuildingCoverageBindings.push(new Cpr.BuildingCoverageUiBinding(" & Me.BuildingIndex.ToString & ",'" & Me.ddBCCauseOfLoss.ClientID & "','" & ddBCCoInsurance.ClientID & "','" & ddBCValuation.ClientID & "','" & ddBCDeductible.ClientID & "','" & txtBCGroupI.ClientID & "','" & txtBCGroupII.ClientID & "','" & ddBICCauseOfLoss.ClientID & "','" & txtBICGroupI.ClientID & "','" & txtBICGroupII.ClientID & "','" & ddPPCCauseOfLoss.ClientID & "','" & ddPPCCoinsurance.ClientID & "','" & ddPPCValuation.ClientID & "','" & ddPPCDeductible.ClientID & "','" & txtPPCGroupI.ClientID & "','" & txtPPCGroupII.ClientID & "','" & ddPPOCauseOfLoss.ClientID & "','" & ddPPOCoinsurance.ClientID & "','" & ddPPOValuation.ClientID & "','" & ddPPODeductible.ClientID & "','" & txtPPOGroupI.ClientID & "','" & txtPPOGroupII.ClientID & "','" & chkBCAgreedAmount.ClientID & "','" & chkPPCAgreedAmount.ClientID & "'));")

        ' INFO
        VRScript.CreateJSBinding(chkINFEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.LocationEQCheckboxChanged('" & chkINFEarthquake.ClientID & "','" & trBCEarthquakeRow.ClientID & "','" & trBICEarthquakeRow.ClientID & "','" & trPPCEarthquakeRow.ClientID & "','" & trPPOEarthquakeRow.ClientID & "','" & trPPCEarthquakeLookupRow.ClientID & "','" & trPPOEarthquakeLookupRow.ClientID & "','" & trEarthquakeClassificationRow.ClientID & "');")

        ' BUILDING COVERAGE
        VRScript.CreateJSBinding(chkBuildingCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BC'," & BuildingNumber.ToString & ",'" & chkBuildingCoverage.ClientID & "','" & trBuildingCoverageDataRow.ClientID & "', '','','" & DefaultBCValues.CauseOfLossId & "','" & DefaultBCValues.CoinsuranceId & "','" & DefaultBCValues.ValuationId & "','" & DefaultBCValues.DeductibleId & "','" & DefaultBCValues.GroupIValue & "','" & DefaultBCValues.GroupIIValue & "','" & DefaultBCValues.AgreedAmountValue.ToString() & "','" & DefaultBCValues.AgreedAmountEnabled.ToString() & "');")
        VRScript.CreateJSBinding(chkBCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('" & Me.chkBCBlanketApplied.ClientID & "','" & Me.ddBCCauseOfLoss.ClientID & "','" & Me.ddBCCoInsurance.ClientID & "','" & Me.ddBCValuation.ClientID & "','" & Me.ddBCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','8');")
        VRScript.CreateJSBinding(chkBCUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkBCUseSpecificRates.ClientID & "','" & trBCGroupIRow.ClientID & "','" & trBCGroupIIRow.ClientID & "','" & trBCUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkBCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkBCAgreedAmount.ClientID & "','','','" & trBCAgreedAmountInfoRow.ClientID & "','');")

        ' BUSINESS INCOME COVERAGE
        VRScript.CreateJSBinding(chkBusinessIncomeCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BIC'," & BuildingNumber.ToString & ",'" & chkBusinessIncomeCoverage.ClientID & "','" & trBusinessIncomeDataRow.ClientID & "', '','','" & DefaultBICValues.CauseOfLossId & "','" & DefaultBICValues.CoinsuranceId & "','" & DefaultBICValues.ValuationId & "','" & DefaultBICValues.DeductibleId & "','" & DefaultBICValues.GroupIValue & "','" & DefaultBICValues.GroupIIValue & "','" & DefaultBICValues.AgreedAmountValue.ToString() & "','" & DefaultBICValues.AgreedAmountEnabled.ToString() & "');")
        VRScript.CreateJSBinding(chkBICUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkBICUseSpecificRates.ClientID & "','" & trBICGroupIRow.ClientID & "','" & trBICGroupIIRow.ClientID & "','" & trBICUseSpecificRatesInfoRow.ClientID & "','');")
        'VRScript.CreateJSBinding(rbBICMonthlyPeriod, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('" & rbBICMonthlyPeriod.ClientID & "','" & rbBICCoinsurance.ClientID & "','" & ddBICLimitType.ClientID & "');")
        'VRScript.CreateJSBinding(rbBICCoinsurance, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('" & rbBICMonthlyPeriod.ClientID & "','" & rbBICCoinsurance.ClientID & "','" & ddBICLimitType.ClientID & "');")
        'VRScript.CreateJSBinding(rbBICMonthlyPeriod, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('" & rbBICMonthlyPeriod.ClientID & "','" & rbBICCoinsurance.ClientID & "','" & ddBICLimitType.ClientID & "','" & hdnBICLimitTypeValues.ClientID & "');")
        'VRScript.CreateJSBinding(rbBICCoinsurance, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('" & rbBICMonthlyPeriod.ClientID & "','" & rbBICCoinsurance.ClientID & "','" & ddBICLimitType.ClientID & "','" & hdnBICLimitTypeValues.ClientID & "');")

        ' PERSONAL PROPERTY COVERAGE
        VRScript.CreateJSBinding(chkPersonalPropertyCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPC'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyCoverage.ClientID & "','" & trPersonalPropertyDataRow.ClientID & "', '', '','" & DefaultPPCValues.CauseOfLossId & "','" & DefaultPPCValues.CoinsuranceId & "','" & DefaultPPCValues.ValuationId & "','" & DefaultPPCValues.DeductibleId & "','" & DefaultPPCValues.GroupIValue & "','" & DefaultPPCValues.GroupIIValue & "','" & DefaultPPCValues.AgreedAmountValue.ToString() & "','" & DefaultPPCValues.AgreedAmountEnabled.ToString() & "');")
        VRScript.CreateJSBinding(chkPPCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('" & Me.chkPPCBlanketApplied.ClientID & "','" & Me.ddPPCCauseOfLoss.ClientID & "','" & Me.ddPPCCoinsurance.ClientID & "','" & Me.ddPPCValuation.ClientID & "','" & Me.ddPPCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','8');")
        VRScript.CreateJSBinding(chkPPCUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkPPCUseSpecificRates.ClientID & "','" & trPPCGroupIRow.ClientID & "','" & trPPCGroupIIRow.ClientID & "','" & trPPCUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkPPCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkPPCAgreedAmount.ClientID & "','','','" & trPPCAgreedAmountInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkPPCEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkPPCEarthquake.ClientID & "','" & trPPCEarthquakeLookupRow.ClientID & "','','','');")

        ' PERSONAL PROPERTY OF OTHERS COVERAGE
        VRScript.CreateJSBinding(chkPersonalPropertyOfOthers, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPO'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyOfOthers.ClientID & "','" & trPersonalPropertyOfOthersDataRow.ClientID & "', '', '','" & DefaultPPOValues.CauseOfLossId & "','" & DefaultPPOValues.CoinsuranceId & "','" & DefaultPPOValues.ValuationId & "','" & DefaultPPOValues.DeductibleId & "','" & DefaultPPOValues.GroupIValue & "','" & DefaultPPOValues.GroupIIValue & "','" & DefaultPPOValues.AgreedAmountValue.ToString() & "','" & DefaultPPOValues.AgreedAmountEnabled.ToString() & "');")
        VRScript.CreateJSBinding(chkPPOBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('" & Me.chkPPOBlanketApplied.ClientID & "','" & Me.ddPPOCauseOfLoss.ClientID & "','" & Me.ddPPOCoinsurance.ClientID & "','" & Me.ddPPOValuation.ClientID & "','" & Me.ddPPODeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','8');")
        VRScript.CreateJSBinding(chkPPOUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkPPOUseSpecificRates.ClientID & "','" & trPPOGroupIRow.ClientID & "','" & trPPOGroupIIRow.ClientID & "','" & trPPOUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkPPOEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('" & chkPPOEarthquake.ClientID & "','" & trPPOEarthquakeLookupRow.ClientID & "','','','');")

        ' Load BIC Limit Type ddl routine
        'Me.VRScript.AddVariableLine("function LoadBICLimitDropdown(){$('#" + Me.btnLoadBICLimitType.ClientID + "').click();}")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddINFConstruction.Items Is Nothing OrElse ddINFConstruction.Items.Count <= 0 Then
            ' Construction Type
            QQHelper.LoadStaticDataOptionsDropDown(ddINFConstruction, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, , Quote.LobType)

            ' Earthquake Building Classifications
            QQHelper.LoadStaticDataOptionsDropDown(ddEarthquakeClassification, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EarthquakeBuildingClassificationTypeId, , Quote.LobType)

            ' BUILDING COVERAGES (BC)
            ' Inflation Guard
            QQHelper.LoadStaticDataOptionsDropDown(ddBCInflationGuard, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, , Quote.LobType)
            ' Cause of loss
            QQHelper.LoadStaticDataOptionsDropDown(ddBCCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddBCCoInsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, , Quote.LobType)
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddBCValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsValuationId, , Quote.LobType)
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddBCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)

            ' BUSINESS INCOME COVERAGE (BIC)
            ' Limit Type
            ' Income Type
            QQHelper.LoadStaticDataOptionsDropDown(ddBICIncomeType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BusinessIncomeCov_BusinessIncomeTypeId, , Quote.LobType)
            ' Risk Type
            QQHelper.LoadStaticDataOptionsDropDown(ddBICRiskType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BusinessIncomeCov_RiskTypeId, , Quote.LobType)
            ' Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddBICCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BusinessIncomeCov_CauseOfLossTypeId, , Quote.LobType)

            ' PERSONAL PROPERTY COVERAGE (PPC)
            ' Property Type
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCPropertyType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_PropertyTypeId, , Quote.LobType)
            ' Risk Type
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCRiskType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_RiskTypeId, , Quote.LobType)
            ' Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCCoinsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, , Quote.LobType)
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_ValuationId, , Quote.LobType)
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddPPCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)

            ' PERSONAL PROPERTY OF OTHERS COVERAGE
            ' Risk Type
            QQHelper.LoadStaticDataOptionsDropDown(ddPPORiskType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropOfOthers_RiskTypeId, , Quote.LobType)
            ' Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddPPOCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddPPOCoinsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, , Quote.LobType)
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddPPOValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_ValuationId, , Quote.LobType)
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddPPODeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        Dim HasBlanket As Boolean = False
        Dim HasAgreedAmount = False

        LoadStaticData()

        ' Set the HasAgreedAmount flag
        ' Location 0 Building 0 IsAgreedAmount holds the default value for all the buildings
        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= 1 AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.Count >= 1 Then
            HasAgreedAmount = Quote.Locations(0).Buildings(0).IsAgreedValue
        End If

        ' Hide all of the controls that are not visible by default
        trEarthquakeClassificationRow.Attributes.Add("style", "display:none")
        trBuildingCoverageDataRow.Attributes.Add("style", "display:none")
        trBusinessIncomeDataRow.Attributes.Add("style", "display:none")
        trPersonalPropertyDataRow.Attributes.Add("style", "display:none")
        trPersonalPropertyOfOthersDataRow.Attributes.Add("style", "display:none")
        trBCBlanketInfoRow.Attributes.Add("style", "display:none")
        trBCAgreedAmountInfoRow.Attributes.Add("style", "display:none")
        trPPCAgreedAmountInfoRow.Attributes.Add("style", "display:none")
        trPPCBlanketAppliedInfoRow.Attributes.Add("style", "display:none")
        trPPCEarthquakeLookupRow.Attributes.Add("style", "display:none")
        trPPOBlanketAppliedInfoRow.Attributes.Add("style", "display:none")
        trPPOEarthquakeLookupRow.Attributes.Add("style", "display:none")

        ' The specific rates rows are always shown UNLESS N/A was chosen for blanekt coverage on the coverages page
        If Quote.HasBlanketBuilding OrElse Quote.HasBlanketBuildingAndContents OrElse Quote.HasBlanketContents Then
            HasBlanket = True
        End If

        ' Set defaults
        ' Earthquake Building Classification
        If ddEarthquakeClassification.Items IsNot Nothing Then
            For Each li As ListItem In ddEarthquakeClassification.Items
                If li.Value = "11" Then
                    ddEarthquakeClassification.SelectedIndex = -1
                    li.Selected = True
                    Exit For
                End If
            Next
        End If

        ' Set properties on the class code lookup control
        Me.ctl_CPR_ClassCodeLookup.ParentClassCodeTextboxId = Me.txtINFClassCode.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentDescriptionTextboxId = Me.txtINFDescription.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentIDHdnId = Me.hdnDIA_Id.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPMAIDHdnId = Me.hdnPMAID.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentGroupRateHdnId = Me.hdnGroupRate.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentClassLimitHdnId = Me.hdnClassLimit.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentYardRateHdnId = Me.hdnYardRateId.ClientID
        Me.ctl_CPR_ClassCodeLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_ClassCodeLookup.BuildingIndex = BuildingIndex

        Me.ctl_CPR_ClassCodeLookup.ParentBCUseCodeRowId = Me.trBCUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBCUseCodeInfoRowId = Me.trBCUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBCUseCodeGroupIRowId = Me.trBCGroupIRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBCUseCodeGroupIIRowId = Me.trBCGroupIIRow.ClientID

        Me.ctl_CPR_ClassCodeLookup.ParentBICUseCodeRowId = Me.trBICUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBICUseCodeInfoRowId = Me.trBICUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBICUseCodeGroupIRowId = Me.trBICGroupIRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentBICUseCodeGroupIIRowId = Me.trBICGroupIIRow.ClientID

        Me.ctl_CPR_ClassCodeLookup.ParentPPCUseCodeRowId = Me.trPPCUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPCUseCodeInfoRowId = Me.trPPCUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPCUseCodeGroupIRowId = Me.trPPCGroupIRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPCUseCodeGroupIIRowId = Me.trPPCGroupIIRow.ClientID

        Me.ctl_CPR_ClassCodeLookup.ParentPPOUseCodeRowId = Me.trPPOUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPOUseCodeInfoRowId = Me.trPPOUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPOUseCodeGroupIRowId = Me.trPPOGroupIRow.ClientID
        Me.ctl_CPR_ClassCodeLookup.ParentPPOUseCodeGroupIIRowId = Me.trPPOGroupIIRow.ClientID

        ' Set properties on the Earthquake Class Code lookup controls (we have two, one for PPC and one for PPO)
        ' PPC
        Me.ctl_CPR_PPC_EQCCLookup.ParentClassCodeTextboxId = Me.txtPPCEarthquakeClassification.ClientID
        Me.ctl_CPR_PPC_EQCCLookup.ParentHdnId = Me.hdnDIA_PPC_EQCC_Id.ClientID
        Me.ctl_CPR_PPC_EQCCLookup.ParentHdnRateGroupId = Me.hdn_PPC_RateGroup.ClientID
        Me.ctl_CPR_PPC_EQCCLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_PPC_EQCCLookup.BuildingIndex = BuildingIndex
        Me.ctl_CPR_PPC_EQCCLookup.LookupType = "PPC"
        Me.ctl_CPR_PPC_EQCCLookup.ParentEQPPCDataRow1Id = Me.trPPCEarthquakeLookupRow.ClientID
        ' PPO
        Me.ctl_CPR_PPO_EQCCLookup.ParentClassCodeTextboxId = Me.txtPPOEQClassification.ClientID
        Me.ctl_CPR_PPO_EQCCLookup.ParentHdnId = Me.hdnDIA_PPO_EQCC_Id.ClientID
        Me.ctl_CPR_PPO_EQCCLookup.ParentHdnRateGroupId = Me.hdn_PPO_RateGroup.ClientID
        Me.ctl_CPR_PPO_EQCCLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_PPO_EQCCLookup.BuildingIndex = BuildingIndex
        Me.ctl_CPR_PPO_EQCCLookup.LookupType = "PPO"
        Me.ctl_CPR_PPO_EQCCLookup.ParentEQPPODataRow1Id = Me.trPPOEarthquakeLookupRow.ClientID

        If MyBuilding.IsNotNull Then
            ' Don't show the delete button on the first building
            If BuildingIndex = 0 Then lnkDelete.Attributes.Add("style", "display:none") Else lnkDelete.Attributes.Add("style", "display:''")
            ' Set the building header
            Me.lblAccordHeader.Text = "Building # " & BuildingIndex + 1.ToString

            ' Set the class code value fields
            Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = MyBuilding.ClassificationCode
            If cc IsNot Nothing Then
                txtINFClassCode.Text = cc.ClassCode
                txtINFDescription.Text = cc.ClassDescription
                hdnDIA_Id.Value = cc.ClassificationCodeNum
                hdnPMAID.Value = cc.PMA
                hdnGroupRate.Value = cc.RateGroup
                hdnClassLimit.Value = cc.ClassLimit

                ' Set the Yard Rate Id
                ' It will be the same value for PPO or PPC
                Dim yrId As String = ""
                If MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId) Then yrId = MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId
                If MyBuilding.PersPropCov_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(MyBuilding.PersPropCov_DoesYardRateApplyTypeId) Then yrId = MyBuilding.PersPropCov_DoesYardRateApplyTypeId
                hdnYardRateId.Value = yrId
            End If
        End If

        ' Earthquake Building Classification
        If MyBuilding.EarthquakeBuildingClassificationTypeId <> "" AndAlso IsNumeric(MyBuilding.EarthquakeBuildingClassificationTypeId) Then
            chkINFEarthquake.Checked = True
            trEarthquakeClassificationRow.Attributes.Add("style", "display:''")
            ddEarthquakeClassification.SelectedValue = MyBuilding.EarthquakeBuildingClassificationTypeId
        Else
            chkINFEarthquake.Checked = False
            trEarthquakeClassificationRow.Attributes.Add("style", "display:none")
        End If

        '*** COVERAGES ***
        ' This routine will hide all of the specific rates rows if the building class code is one of a set of values (see method)
        Dim UseSpecificIsHidden As Boolean = False
        CheckBuildingClassCodeForSpecificRatesEligibility(UseSpecificIsHidden)

        ' Building Coverage
        If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) Then
            trBuildingCoverageDataRow.Attributes.Add("style", "display:''")
            chkBuildingCoverage.Checked = True
            txtBCBuildingLimit.Text = MyBuilding.Limit
            If MyBuilding.InflationGuardTypeId <> "" Then ddBCInflationGuard.SelectedValue = MyBuilding.InflationGuardTypeId
            If HasBlanket Then
                chkBCBlanketApplied.Enabled = True
                chkBCBlanketApplied.Checked = MyBuilding.IsBuildingValIncludedInBlanketRating
            Else
                chkBCBlanketApplied.Enabled = False
                chkBCBlanketApplied.Checked = False
            End If
            If chkBCBlanketApplied.Checked Then
                ddBCCauseOfLoss.Attributes.Add("disabled", "True")
                ddBCCoInsurance.Attributes.Add("disabled", "True")
                ddBCValuation.Attributes.Add("disabled", "True")
                ddBCDeductible.Attributes.Add("disabled", "True")
            Else
                ddBCCauseOfLoss.Attributes.Remove("disabled")
                ddBCCoInsurance.Attributes.Remove("disabled")
                ddBCValuation.Attributes.Remove("disabled")
                ddBCDeductible.Attributes.Remove("disabled")
            End If
            ' Use Specific Rates
            txtBCGroupI.Text = MyBuilding.Building_BusinessIncome_Group1_LossCost
            txtBCGroupII.Text = MyBuilding.Building_BusinessIncome_Group2_LossCost
            ' Cause of Loss
            If MyBuilding.CauseOfLossTypeId <> "" Then ddBCCauseOfLoss.SelectedValue = MyBuilding.CauseOfLossTypeId
            ' Co-Insurance
            If MyBuilding.CoinsuranceTypeId <> "" Then ddBCCoInsurance.SelectedValue = MyBuilding.CoinsuranceTypeId
            ' Valuation
            If MyBuilding.ValuationId <> "" Then ddBCValuation.SelectedValue = MyBuilding.ValuationId
            ' Deductible
            If MyBuilding.DeductibleId <> "" Then ddBCDeductible.SelectedValue = MyBuilding.DeductibleId
            ' Agreed Amount
            If HasAgreedAmount Then
                chkBCAgreedAmount.Checked = True
                chkBCAgreedAmount.Enabled = False
                chkPPCAgreedAmount.Checked = True
                chkPPCAgreedAmount.Enabled = False
            Else
                chkBCAgreedAmount.Checked = False
                chkBCAgreedAmount.Enabled = True
                chkPPCAgreedAmount.Checked = True
                chkPPCAgreedAmount.Enabled = False
            End If
            ' Earthquake
            If chkINFEarthquake.Checked Then
                trBCEarthquakeRow.Attributes.Add("style", "display:''")
                chkBCEarthquake.Checked = True
            Else
                trBCEarthquakeRow.Attributes.Add("style", "display:none")
                chkBCEarthquake.Checked = False
            End If
        End If

        ' Business Income Coverage
        If HasCoverage(CPRBuildingCoverageType.BusinessIncomeCoverage) Then
            chkBusinessIncomeCoverage.Checked = True
            trBusinessIncomeDataRow.Attributes.Add("style", "display''")

            ' If the quote has business income als, hide the Limit and Limit Type rows, check the BIC checkbox and disable it
            If Quote.HasBusinessIncomeALS Then
                chkBusinessIncomeCoverage.Checked = True
                chkBusinessIncomeCoverage.Enabled = False
                trBICLimit.Attributes.Add("style", "display:none")
                trBICLimitType.Attributes.Add("style", "display:none")
            Else
                chkBusinessIncomeCoverage.Enabled = True
                trBICLimit.Attributes.Add("style", "display:''")
                trBICLimitType.Attributes.Add("style", "display:''")
            End If

            txtBICBusinessIncomeLimit.Text = MyBuilding.BusinessIncomeCov_Limit

            ' Limit Type
            If MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "0" Then
                rbM.Checked = True
                LoadBICLimitTypeDropdown()
                If MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "" Then ddBICLimitType.SelectedValue = MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId
            End If
            If MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "0" Then
                rbC.Checked = True
                LoadBICLimitTypeDropdown()
                If MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "" Then ddBICLimitType.SelectedValue = MyBuilding.BusinessIncomeCov_CoinsuranceTypeId
            End If

            ' Income Type
            If MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId <> "0" Then
                ddBICIncomeType.SelectedValue = MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId
            Else
                ddBICIncomeType.SelectedValue = "1"
            End If
            ' Risk Type
            ddBICRiskType.SelectedValue = MyBuilding.BusinessIncomeCov_RiskTypeId

            If MyBuilding.BusinessIncomeCov_RiskTypeId <> "" Then ddBICRiskType.SelectedValue = MyBuilding.BusinessIncomeCov_RiskTypeId
            ' Use Specific Rates
            txtBICGroupI.Text = MyBuilding.Building_BusinessIncome_Group1_LossCost
            txtBICGroupII.Text = MyBuilding.Building_BusinessIncome_Group2_LossCost
            ' Cause of Loss
            If MyBuilding.BusinessIncomeCov_CauseOfLossTypeId <> "" Then ddBICCauseOfLoss.SelectedValue = MyBuilding.BusinessIncomeCov_CauseOfLossTypeId
            ' Earthquake
            If chkINFEarthquake.Checked Then
                trBICEarthquakeRow.Attributes.Add("style", "display:''")
                chkBICEarthquake.Checked = True
            Else
                trBICEarthquakeRow.Attributes.Add("style", "display:none")
                chkBICEarthquake.Checked = False
            End If
        End If

        ' Personal Property Coverage
        If HasCoverage(CPRBuildingCoverageType.PersonalPropertyCoverage) Then
            trPersonalPropertyDataRow.Attributes.Add("style", "display:''")
            chkPersonalPropertyCoverage.Checked = True
            If MyBuilding.PersPropCov_EarthquakeApplies Then
                chkPPCEarthquake.Checked = True
                trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
            End If

            txtPPCPersonalPropertyLimit.Text = MyBuilding.PersPropCov_PersonalPropertyLimit
            If MyBuilding.PersPropCov_PropertyTypeId <> "" Then ddPPCPropertyType.SelectedValue = MyBuilding.PersPropCov_PropertyTypeId
            If MyBuilding.PersPropCov_RiskTypeId <> "" Then ddPPCRiskType.SelectedValue = MyBuilding.PersPropCov_RiskTypeId
            If HasBlanket Then
                chkPPCBlanketApplied.Enabled = True
                chkPPCBlanketApplied.Checked = MyBuilding.PersPropCov_IncludedInBlanketCoverage
            Else
                chkPPCBlanketApplied.Enabled = False
                chkPPCBlanketApplied.Checked = False
            End If
            If chkPPCBlanketApplied.Checked Then
                ddPPCCauseOfLoss.Attributes.Add("disabled", "True")
                ddPPCCoinsurance.Attributes.Add("disabled", "True")
                ddPPCValuation.Attributes.Add("disabled", "True")
                ddPPCDeductible.Attributes.Add("disabled", "True")
            Else
                ddPPCCauseOfLoss.Attributes.Remove("disabled")
                ddPPCCoinsurance.Attributes.Remove("disabled")
                ddPPCValuation.Attributes.Remove("disabled")
                ddPPCDeductible.Attributes.Remove("disabled")
            End If
            ' Use Specific Rates
            txtPPCGroupI.Text = MyBuilding.PersonalProperty_Group1_LossCost
            txtPPCGroupII.Text = MyBuilding.PersonalProperty_Group2_LossCost
            If MyBuilding.PersPropCov_CauseOfLossTypeId <> "" Then ddPPCCauseOfLoss.SelectedValue = MyBuilding.PersPropCov_CauseOfLossTypeId
            If MyBuilding.PersPropCov_CoinsuranceTypeId <> "" Then ddPPCCoinsurance.SelectedValue = MyBuilding.PersPropCov_CoinsuranceTypeId
            If MyBuilding.PersPropCov_ValuationId <> "" Then ddPPCValuation.SelectedValue = MyBuilding.PersPropCov_ValuationId
            If MyBuilding.PersPropCov_DeductibleId <> "" Then ddPPCDeductible.SelectedValue = MyBuilding.PersPropCov_DeductibleId
            chkPPCAgreedAmount.Checked = MyBuilding.PersPropCov_IsAgreedValue
            chkPPCEarthquake.Checked = MyBuilding.PersPropCov_EarthquakeApplies
            If chkPPCEarthquake.Checked Then
                trPPCEarthquakeRow.Attributes.Add("style", "display:''")
                chkPPCEarthquake.Checked = True
            Else
                trPPCEarthquakeRow.Attributes.Add("style", "display:none")
                chkPPCEarthquake.Checked = False
            End If
            If chkPPCEarthquake.Checked Then
                trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
                txtPPCEarthquakeClassification.Text = GetEQClassificationDescriptionText(MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId)
                hdnDIA_PPC_EQCC_Id.Value = MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId
            End If
        End If

        ' Personal Property of Others Coverage
        If HasCoverage(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage) Then
            trPersonalPropertyOfOthersDataRow.Attributes.Add("style", "display:''")
            chkPersonalPropertyOfOthers.Checked = True
            If MyBuilding.PersPropCov_EarthquakeApplies Then
                trPPOEarthquakeLookupRow.Attributes.Add("style", "display:''")
                chkPPOEarthquake.Checked = True
            End If

            txtPPOPersonalPropertyLimit.Text = MyBuilding.PersPropOfOthers_PersonalPropertyLimit
            If MyBuilding.PersPropOfOthers_RiskTypeId <> "" Then ddPPORiskType.SelectedValue = MyBuilding.PersPropOfOthers_RiskTypeId
            If HasBlanket Then
                chkPPOBlanketApplied.Enabled = True
                chkPPOBlanketApplied.Checked = MyBuilding.PersPropOfOthers_IncludedInBlanketCoverage
            Else
                chkPPOBlanketApplied.Enabled = False
                chkPPOBlanketApplied.Checked = False
            End If
            If chkPPOBlanketApplied.Checked Then
                ddPPOCauseOfLoss.Attributes.Add("disabled", "True")
                If BlanketCauseOfLossId <> "" Then ddPPCCauseOfLoss.SelectedValue = BlanketCauseOfLossId
                ddPPOCoinsurance.Attributes.Add("disabled", "True")
                If BlanketCoinsuranceId <> "" Then ddPPCCoinsurance.SelectedValue = BlanketCoinsuranceId
                ddPPOValuation.Attributes.Add("disabled", "True")
                If BlanketValuationId <> "" Then ddPPCValuation.SelectedValue = BlanketValuationId
                ddPPODeductible.Attributes.Add("disabled", "True")
            Else
                ddPPOCauseOfLoss.Attributes.Remove("disabled")
                ddPPOCoinsurance.Attributes.Remove("disabled")
                ddPPOValuation.Attributes.Remove("disabled")
                ddPPODeductible.Attributes.Remove("disabled")
            End If
            ' Use Specific Rates
            txtPPOGroupI.Text = MyBuilding.PersonalProperty_Group1_LossCost
            txtPPOGroupII.Text = MyBuilding.PersonalProperty_Group2_LossCost
            If MyBuilding.PersPropOfOthers_CauseOfLossTypeId <> "" Then ddPPOCauseOfLoss.SelectedValue = MyBuilding.PersPropOfOthers_CauseOfLossTypeId
            If MyBuilding.PersPropOfOthers_CoinsuranceTypeId <> "" Then ddPPOCoinsurance.SelectedValue = MyBuilding.PersPropOfOthers_CoinsuranceTypeId
            If MyBuilding.PersPropOfOthers_ValuationId <> "" Then ddPPOValuation.SelectedValue = MyBuilding.PersPropOfOthers_ValuationId
            If MyBuilding.PersPropOfOthers_DeductibleId <> "" Then ddPPODeductible.SelectedValue = MyBuilding.PersPropOfOthers_DeductibleId
            If chkINFEarthquake.Checked Then
                trPPOEarthquakeRow.Attributes.Add("style", "display:''")
                chkPPOEarthquake.Checked = True
            Else
                trPPOEarthquakeRow.Attributes.Add("style", "display:none")
                chkPPOEarthquake.Checked = False
            End If
            If chkPPOEarthquake.Checked Then
                trPPOEarthquakeLookupRow.Attributes.Add("style", "display:''")
                txtPPOEQClassification.Text = GetEQClassificationDescriptionText(MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId)
                hdnDIA_PPO_EQCC_Id.Value = MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId
            End If
        End If

        Me.PopulateChildControls()
    End Sub

    ''' <summary>
    ''' If the current building is at Location 0 Building 0 and there are no other buildings on the quote returns true,
    ''' otherwise returns false.
    ''' </summary>
    ''' <returns></returns>
    Private Function IsFirstBuildingAndNoOtherBuildings() As Boolean
        If MyBuilding IsNot Nothing Then
            If LocationIndex = 0 AndAlso BuildingIndex = 0 Then
                Dim ndx As Integer = 0
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    ' Check for more buildings on the first location                    
                    If ndx = 0 And L.Buildings.Count > 1 Then
                        Return False
                    Else
                        ' All locations except the first - if any buildings found return false
                        If L.Buildings IsNot Nothing AndAlso L.Buildings.Count > 0 Then Return False
                    End If
                    ndx += 1
                Next
            End If
            ' If we got here then we didn't find any additional buildings
            Return True
        Else
            ' MyBuilding is nothing
            Return False
        End If
    End Function

    Private Function HasCoverage(ByVal CovType As CPRBuildingCoverageType) As Boolean
        Select Case CovType
            Case CPRBuildingCoverageType.BuildingCoverage
                If chkBuildingCoverage.Checked Then Return True
                If MyBuilding.Limit <> "" Then
                    Return True
                End If
                Exit Select
            Case CPRBuildingCoverageType.BusinessIncomeCoverage
                If chkBusinessIncomeCoverage.Checked Then Return True
                'If Quote.HasBusinessIncomeALS Then Return True
                If MyBuilding.BusinessIncomeCov_Limit IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_Limit <> "" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0" Then Return True
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyCoverage
                If chkPersonalPropertyCoverage.Checked Then Return True
                If MyBuilding.PersPropCov_PersonalPropertyLimit <> "" OrElse
                        MyBuilding.PersPropCov_PropertyTypeId <> "" Then
                    Return True
                End If
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                If chkPersonalPropertyOfOthers.Checked Then Return True
                If MyBuilding.PersPropOfOthers_PersonalPropertyLimit <> "" Then
                    Return True
                End If
                Exit Select
        End Select

        Return False
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If hdnBICLimitTypeValues.Value IsNot Nothing AndAlso hdnBICLimitTypeValues.Value <> "" Then
            LoadBICLimitTypeDropdown()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyBuilding IsNot Nothing Then
            Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = MyBuilding.ClassificationCode
            cc.ClassCode = txtINFClassCode.Text
            cc.PMA = hdnPMAID.Value
            cc.ClassDescription = txtINFDescription.Text
            cc.RateGroup = hdnGroupRate.Value
            cc.ClassLimit = hdnClassLimit.Value
            MyBuilding.ClassificationCode = cc
            MyBuilding.BusinessIncomeCov_ClassificationCode = cc
            MyBuilding.PersPropCov_ClassificationCode = cc
            MyBuilding.PersPropOfOthers_ClassificationCode = cc

            MyBuilding.ConstructionId = ddINFConstruction.SelectedValue

            ' Save the Yard Rate Id
            ' PPC
            If hdnYardRateId.Value = "-1" OrElse hdnYardRateId.Value = "" Then hdnYardRateId.Value = "0"
            If chkPersonalPropertyCoverage.Checked Then
                MyBuilding.PersPropCov_DoesYardRateApplyTypeId = hdnYardRateId.Value
            Else
                MyBuilding.PersPropCov_DoesYardRateApplyTypeId = ""
            End If
            ' PPO
            If chkPersonalPropertyOfOthers.Checked Then
                MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = hdnYardRateId.Value
            Else
                MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = ""
            End If

            ' If neither PPO or PPC are selected we still need to save the yard rate value so just drop it in one of the fields
            If MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = "" AndAlso MyBuilding.PersPropCov_DoesYardRateApplyTypeId = "" Then
                MyBuilding.PersPropCov_DoesYardRateApplyTypeId = hdnYardRateId.Value
            End If

            ' Earthquake Classification
            If chkINFEarthquake.Checked Then
                MyBuilding.EarthquakeBuildingClassificationTypeId = ddEarthquakeClassification.SelectedValue
            Else
                MyBuilding.EarthquakeBuildingClassificationTypeId = ""
            End If

            ' *** COVERAGES ***
            ' BUILDING COVERAGE
            If chkBuildingCoverage.Checked Then
                ' Limit
                MyBuilding.Limit = txtBCBuildingLimit.Text
                ' Inflation Guard
                MyBuilding.InflationGuardTypeId = ddBCInflationGuard.SelectedValue
                ' Blanket Applied
                If chkBCBlanketApplied.Checked Then
                    MyBuilding.IsBuildingValIncludedInBlanketRating = True
                Else
                    MyBuilding.IsBuildingValIncludedInBlanketRating = False
                End If
                ' Use Specific Rates
                SetSpecificRateValues(CPRBuildingCoverageType.BuildingCoverage)
                ' Cause of Loss
                MyBuilding.CauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                ' Co-Insurance
                MyBuilding.CoinsuranceTypeId = ddBCCoInsurance.SelectedValue
                ' Valuation
                MyBuilding.ValuationId = ddBCValuation.SelectedValue
                ' Deductible
                MyBuilding.DeductibleId = ddBCDeductible.SelectedValue

                ' Agreed Amount - Don't change the value if it's location 0 building 0
                ' because this is the value set on the coverages page
                If (Not LocationIndex = 0) AndAlso (Not BuildingIndex = 0) Then
                    MyBuilding.IsAgreedValue = chkBCAgreedAmount.Checked
                End If
                ' Earthquake
                MyBuilding.EarthquakeApplies = chkBCEarthquake.Checked

                ' If Blanket Applied set the Quote level blanket values
                If chkBCBlanketApplied.Checked Then
                    If Quote.HasBlanketBuilding Then
                        Quote.BlanketBuildingCauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketBuildingAndContents Then
                        Quote.BlanketBuildingAndContentsCauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketContents Then
                        Quote.BlanketContentsCauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                    End If
                End If
            Else
                MyBuilding.Limit = ""
                MyBuilding.InflationGuardTypeId = ""
                MyBuilding.IsBuildingValIncludedInBlanketRating = False
                ClearSpecificRateValues(CPRBuildingCoverageType.BuildingCoverage)
                MyBuilding.CauseOfLossTypeId = ""
                MyBuilding.CoinsuranceTypeId = ""
                MyBuilding.ValuationId = ""
                MyBuilding.DeductibleId = ""
                MyBuilding.IsAgreedValue = False
                MyBuilding.EarthquakeApplies = False
            End If

            ' BUSINESS INCOME COVERAGE
            If chkBusinessIncomeCoverage.Checked Then
                'Quote.HasBusinessIncomeALS = True  ' Don't set this here

                If Not Quote.HasBusinessIncomeALS Then
                    ' Building Income Limit
                    MyBuilding.BusinessIncomeCov_Limit = txtBICBusinessIncomeLimit.Text
                    ' Limit Type
                    If rbM.Checked Then
                        MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ddBICLimitType.SelectedValue
                        MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ""
                    End If
                    If rbC.Checked Then
                        MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ""
                        MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ddBICLimitType.SelectedValue
                        End If
                        ' Income Type
                        MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId = ddBICIncomeType.SelectedValue
                        ' Risk Type
                        MyBuilding.BusinessIncomeCov_RiskTypeId = ddBICRiskType.SelectedValue
                        ' Use Specific Rates
                        SetSpecificRateValues(CPRBuildingCoverageType.BusinessIncomeCoverage)
                        ' Cause of Loss
                        MyBuilding.BusinessIncomeCov_CauseOfLossTypeId = ddBICCauseOfLoss.SelectedValue
                        ' Earthquake
                        MyBuilding.BusinessIncomeCov_EarthquakeApplies = chkBICEarthquake.Checked
                    Else
                        MyBuilding.BusinessIncomeCov_Limit = "1"
                    MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ""
                    MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ""
                    MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = "3"  ' 3 = 72
                    MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId = ddBICIncomeType.SelectedValue
                    MyBuilding.BusinessIncomeCov_RiskTypeId = ddBICRiskType.SelectedValue
                    MyBuilding.BusinessIncomeCov_EarthquakeApplies = chkBICEarthquake.Checked
                End If
            Else
                MyBuilding.BusinessIncomeCov_ClassificationCode = Nothing
                MyBuilding.BusinessIncomeCov_Limit = ""
                MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ""
                MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ""
                MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId = ""
                MyBuilding.BusinessIncomeCov_RiskTypeId = ""
                ClearSpecificRateValues(CPRBuildingCoverageType.BusinessIncomeCoverage)
                MyBuilding.BusinessIncomeCov_CauseOfLossTypeId = ""
                MyBuilding.BusinessIncomeCov_EarthquakeApplies = False
                MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = ""
                MyBuilding.BusinessIncomeCov_IncludedInBlanketCoverage = False
            End If

            ' PERSONAL PROPERTY COVERAGE
            If chkPersonalPropertyCoverage.Checked Then
                ' Personal Property Limit
                MyBuilding.PersPropCov_PersonalPropertyLimit = txtPPCPersonalPropertyLimit.Text
                ' Property Type
                MyBuilding.PersPropCov_PropertyTypeId = ddPPCPropertyType.SelectedValue
                ' Risk Type
                MyBuilding.PersPropCov_RiskTypeId = ddPPCRiskType.SelectedValue
                ' Blanket Applied
                MyBuilding.PersPropCov_IncludedInBlanketCoverage = chkPPCBlanketApplied.Checked
                ' Use Specific Rates
                SetSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyCoverage)
                ' Cause of Loss
                MyBuilding.PersPropCov_CauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                ' Co-Insurance
                MyBuilding.PersPropCov_CoinsuranceTypeId = ddPPCCoinsurance.SelectedValue
                ' Valuation
                MyBuilding.PersPropCov_ValuationId = ddPPCValuation.SelectedValue
                ' Deductible
                MyBuilding.PersPropCov_DeductibleId = ddPPCDeductible.SelectedValue
                ' Agreed Amount
                MyBuilding.PersPropCov_IsAgreedValue = chkPPCAgreedAmount.Checked
                ' If Blanket Applied set the Quote level blanket values
                If chkPPCBlanketApplied.Checked Then
                    If Quote.HasBlanketBuilding Then
                        Quote.BlanketBuildingCauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketBuildingAndContents Then
                        Quote.BlanketBuildingAndContentsCauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketContents Then
                        Quote.BlanketContentsCauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                    End If
                End If

                ' Earthquake
                MyBuilding.PersPropCov_EarthquakeApplies = chkPPCEarthquake.Checked
                If MyBuilding.PersPropCov_EarthquakeApplies Then
                    trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
                    If hdnDIA_PPC_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPC_EQCC_Id.Value <> "" Then
                        MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = hdnDIA_PPC_EQCC_Id.Value
                    Else
                        ' Only clear the value if the PPO checkbox is not checked (they share the same value)
                        If Not chkPPOEarthquake.Checked Then MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
                    End If
                Else
                    trPPCEarthquakeLookupRow.Attributes.Add("style", "display:none")
                    MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
                End If
            Else
                ' Classification Code
                MyBuilding.PersPropCov_ClassificationCode = Nothing
                MyBuilding.PersPropCov_PersonalPropertyLimit = Nothing
                MyBuilding.PersPropCov_PropertyTypeId = Nothing
                MyBuilding.PersPropCov_RiskTypeId = Nothing
                MyBuilding.PersPropCov_IncludedInBlanketCoverage = False
                ClearSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyCoverage)
                MyBuilding.PersPropCov_CauseOfLossTypeId = ""
                MyBuilding.PersPropCov_CoinsuranceType = ""
                MyBuilding.PersPropCov_ValuationId = ""
                MyBuilding.PersPropCov_DeductibleId = ""
                MyBuilding.PersPropCov_IsAgreedValue = False
                MyBuilding.PersPropCov_EarthquakeApplies = False
                MyBuilding.PersPropCov_DoesYardRateApplyTypeId = ""
                trPPCEarthquakeLookupRow.Attributes.Add("style", "display:none")
            End If

            ' PERSONAL PROPERTY OF OTHERS
            If chkPersonalPropertyOfOthers.Checked Then
                ' Personal Property Limit
                MyBuilding.PersPropOfOthers_PersonalPropertyLimit = txtPPOPersonalPropertyLimit.Text
                ' Risk Type
                MyBuilding.PersPropOfOthers_RiskTypeId = ddPPORiskType.SelectedValue
                ' Blanket Applied
                MyBuilding.PersPropOfOthers_IncludedInBlanketCoverage = chkPPOBlanketApplied.Checked
                ' Use Specific Rates
                SetSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage)
                ' Cause of Loss
                MyBuilding.PersPropOfOthers_CauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                ' Co-Insurance
                MyBuilding.PersPropOfOthers_CoinsuranceTypeId = ddPPOCoinsurance.SelectedValue
                ' Valuation
                MyBuilding.PersPropOfOthers_ValuationId = ddPPOValuation.SelectedValue
                ' Deductible
                MyBuilding.PersPropOfOthers_DeductibleId = ddPPODeductible.SelectedValue

                ' If Blanket Applied set the Quote level blanket values
                If chkPPOBlanketApplied.Checked Then
                    If Quote.HasBlanketBuilding Then
                        Quote.BlanketBuildingCauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketBuildingAndContents Then
                        Quote.BlanketBuildingAndContentsCauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                    ElseIf Quote.HasBlanketContents Then
                        Quote.BlanketContentsCauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                    End If
                End If

                ' Earthquake
                MyBuilding.PersPropOfOthers_EarthquakeApplies = chkPPOEarthquake.Checked
                If MyBuilding.PersPropOfOthers_EarthquakeApplies Then
                    trPPOEarthquakeLookupRow.Attributes.Add("style", "display:''")
                    If hdnDIA_PPO_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPO_EQCC_Id.Value <> "" Then
                        MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = hdnDIA_PPO_EQCC_Id.Value
                    Else
                        If Not chkPPCEarthquake.Checked Then MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
                    End If
                Else
                    trPPOEarthquakeLookupRow.Attributes.Add("style", "display:none")
                    MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
                End If
            Else
                MyBuilding.PersPropOfOthers_ClassificationCode = Nothing
                MyBuilding.PersPropOfOthers_PersonalPropertyLimit = ""
                MyBuilding.PersPropOfOthers_RiskTypeId = ""
                MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = ""
                MyBuilding.PersPropOfOthers_IncludedInBlanketCoverage = False
                ClearSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage)
                MyBuilding.PersPropOfOthers_CauseOfLossTypeId = ""
                MyBuilding.PersPropOfOthers_CoinsuranceTypeId = ""
                MyBuilding.PersPropOfOthers_ValuationId = ""
                MyBuilding.PersPropOfOthers_DeductibleId = ""
                MyBuilding.PersPropOfOthers_EarthquakeApplies = False
                trPPOEarthquakeLookupRow.Attributes.Add("style", "display:none")
            End If
        End If

        Me.SaveChildControls()
        Return True
    End Function

    ''' <summary>
    ''' Gets the description for a Earthquake Classo
    ''' </summary>
    ''' <param name="Id"></param>
    ''' <returns></returns>
    Private Function GetEQClassificationDescriptionText(ByVal Id As String) As String
        Dim conn As New System.Data.SqlClient.SqlConnection
        Dim cmd As New System.Data.SqlClient.SqlCommand
        Dim rtn As Object = Nothing

        Try
            conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings("connDiamond")
            conn.Open()

            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM EarthquakeBuildingClassificationType WHERE EarthquakeBuildingClassificationType_id = " & Id

            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then Return rtn.ToString() Else Return Id
        Catch ex As Exception
            Return Id
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Sub SetSpecificRateValues(CovType As CPRBuildingCoverageType)
        Dim rtId As String = ""
        Select Case CovType
            Case CPRBuildingCoverageType.BuildingCoverage
                If chkBCUseSpecificRates.Checked Then rtId = "2" Else rtId = "1"
                MyBuilding.RatingTypeId = rtId
                MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBCGroupI.Text
                MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBCGroupII.Text
                Exit Select
            Case CPRBuildingCoverageType.BusinessIncomeCoverage
                If chkBICUseSpecificRates.Checked Then rtId = "2" Else rtId = "1"
                MyBuilding.BusinessIncomeCov_RatingTypeId = rtId
                MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBICGroupI.Text
                MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBICGroupII.Text
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyCoverage
                If chkPPCUseSpecificRates.Checked Then rtId = "2" Else rtId = "1"
                MyBuilding.PersPropCov_RatingTypeId = rtId
                MyBuilding.PersonalProperty_Group1_LossCost = txtPPCGroupI.Text
                MyBuilding.PersonalProperty_Group2_LossCost = txtPPCGroupII.Text
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                If chkPPOUseSpecificRates.Checked Then rtId = "2" Else rtId = "1"
                MyBuilding.PersPropOfOthers_RatingTypeId = rtId
                MyBuilding.PersonalProperty_Group1_LossCost = txtPPOGroupI.Text
                MyBuilding.PersonalProperty_Group2_LossCost = txtPPOGroupII.Text
                Exit Select
        End Select

        Exit Sub
    End Sub

    Private Sub ClearSpecificRateValues(CovType As CPRBuildingCoverageType)
        Select Case CovType
            Case CPRBuildingCoverageType.BuildingCoverage
                MyBuilding.RatingTypeId = ""
                Exit Select
            Case CPRBuildingCoverageType.BusinessIncomeCoverage
                MyBuilding.BusinessIncomeCov_RatingTypeId = ""
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyCoverage
                MyBuilding.PersPropCov_RatingTypeId = ""
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                MyBuilding.PersPropOfOthers_RatingTypeId = ""
                Exit Select
        End Select

        If (Not chkBCUseSpecificRates.Checked) AndAlso (Not chkBICUseSpecificRates.Checked) Then
            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
        End If

        If (Not chkPPCUseSpecificRates.Checked) AndAlso (Not chkPPOUseSpecificRates.Checked) Then
            MyBuilding.PersonalProperty_Group1_LossCost = ""
            MyBuilding.PersonalProperty_Group2_LossCost = ""
        End If

        Exit Sub
    End Sub


    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        '***  INFO SECTION ***
        Me.ValidationHelper.GroupName = " Location " & LocationIndex + 1.ToString & ", Building " & BuildingIndex + 1.ToString
        ' Class Code
        If txtINFClassCode.Text = "" Then
            Me.ValidationHelper.AddError(txtINFClassCode, "Missing Class Code", accordList)
        End If
        ' Construction
        If ddINFConstruction.SelectedIndex < 0 Then
            Me.ValidationHelper.AddError(ddINFConstruction, "Missing Construction", accordList)
        End If

        ' *** COVERAGES ***
        ' BUILDING COVERAGE
        If chkBuildingCoverage.Checked Then
            If txtBCBuildingLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtBCBuildingLimit, "Missing Building Limit", accordList)
            End If
            If BCUseSpecificVisible Then
                If txtBCGroupI.Text = "" Then
                    Me.ValidationHelper.AddError(txtBCGroupI, "Missing Specific Rates", accordList)
                End If
                If txtBCGroupII.Text = "" Then
                    Me.ValidationHelper.AddError(txtBCGroupII, "Missing Specific Rates", accordList)
                End If
            End If
        End If

        ' BUSINESS INCOME COVERAGE
        If Quote.HasBusinessIncomeALS Then
            If chkBusinessIncomeCoverage.Enabled Then
                If txtBICBusinessIncomeLimit.Text = "" Then
                    Me.ValidationHelper.AddError(txtBICBusinessIncomeLimit, "Missing Business Income Limit", accordList)
                End If
                If (Not rbM.Checked) AndAlso (Not rbC.Checked) Then
                    'If (rbBICMonthlyPeriod.Checked = False AndAlso rbBICCoinsurance.Checked = False) Then
                    Me.ValidationHelper.AddError("Missing Limit Type")
                        'Me.ValidationHelper.AddError(rbBICMonthlyPeriod, "Missing Limit Type", accordList)
                    Else
                        If ddBICLimitType.SelectedIndex <= 0 Then
                            Me.ValidationHelper.AddError(ddBICLimitType, "Missing Limit Type", accordList)
                        End If
                    End If
                End If
                If ddBICRiskType.SelectedValue = "" Then
                Me.ValidationHelper.AddError(ddBICRiskType, "Missing Risk Type", accordList)
            End If
            If BICUseSpecificVisible Then
                If txtBICGroupI.Text = "" Then
                    Me.ValidationHelper.AddError(txtBICGroupI, "Missing Specific Rates", accordList)
                End If
                If txtBICGroupII.Text = "" Then
                    Me.ValidationHelper.AddError(txtBICGroupII, "Missing Specific Rates", accordList)
                End If
            End If
        End If

        ' PERSONAL PROPERTY COVERAGE
        If chkPersonalPropertyCoverage.Checked Then
            If txtPPCPersonalPropertyLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCPersonalPropertyLimit, "Missing Personal Property Limit", accordList)
            End If
            If PPCUseSpecificVisible Then
                If txtPPCGroupI.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPCGroupI, "Missing Specific Rates", accordList)
                End If
                If txtPPCGroupII.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPCGroupII, "Missing Specific Rates", accordList)
                End If
            End If
            If chkPPCEarthquake.Checked Then
                If txtPPCEarthquakeClassification.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPCEarthquakeClassification, "Missing Earthquake Classification", accordList)
                End If
            End If
        End If

        ' PERSONAL PROPERTY OF OTHERS
        If chkPersonalPropertyOfOthers.Checked Then
            If txtPPOPersonalPropertyLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtPPOPersonalPropertyLimit, "Missing Personal Property Limit", accordList)
            End If
            If PPOUseSpecificVisible Then
                If txtPPOGroupI.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPOGroupI, "Missing Specific Rates", accordList)
                End If
                If txtPPOGroupII.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPOGroupII, "Missing Specific Rates", accordList)
                End If
            End If
            If chkPPOEarthquake.Checked Then
                If txtPPOEQClassification.Text = "" Then
                    Me.ValidationHelper.AddError(txtPPOEQClassification, "Missing Earthquake Classification", accordList)
                End If
            End If
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub CheckBuildingClassCodeForSpecificRatesEligibility(ByRef CCFound As Boolean)
        Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = MyBuilding.ClassificationCode
        Dim classcodes As New List(Of String)

        CCFound = False

        ' Load the list with all of the class codes that don't use specific rates
        classcodes.Add("0074")
        classcodes.Add("0075")
        classcodes.Add("0076")
        classcodes.Add("0077")
        classcodes.Add("0078")
        classcodes.Add("0196")
        classcodes.Add("0197")
        classcodes.Add("0198")
        classcodes.Add("0311")
        classcodes.Add("0312")
        classcodes.Add("0313")
        classcodes.Add("0321")
        classcodes.Add("0322")
        classcodes.Add("0323")
        classcodes.Add("0745")
        classcodes.Add("0746")
        classcodes.Add("0747")
        classcodes.Add("0511")
        classcodes.Add("0512")
        classcodes.Add("0520")
        classcodes.Add("0531")
        classcodes.Add("0541")
        classcodes.Add("0550")
        classcodes.Add("0561")
        classcodes.Add("0562")
        classcodes.Add("0563")
        classcodes.Add("0564")
        classcodes.Add("0565")
        classcodes.Add("0566")
        classcodes.Add("0567")
        classcodes.Add("0570")
        classcodes.Add("0580")
        classcodes.Add("0581")
        classcodes.Add("0582")
        classcodes.Add("0701")
        classcodes.Add("0702")
        classcodes.Add("0756")
        classcodes.Add("0757")
        classcodes.Add("0831")
        classcodes.Add("0832")
        classcodes.Add("0833")
        classcodes.Add("0834")
        classcodes.Add("0841")
        classcodes.Add("0842")
        classcodes.Add("0843")
        classcodes.Add("0844")
        classcodes.Add("0845")
        classcodes.Add("0846")
        classcodes.Add("0851")
        classcodes.Add("0852")
        classcodes.Add("0900")
        classcodes.Add("0911")
        classcodes.Add("0912")
        classcodes.Add("0913")
        classcodes.Add("0921")
        classcodes.Add("0922")
        classcodes.Add("0923")
        classcodes.Add("0931")
        classcodes.Add("0932")
        classcodes.Add("0933")
        classcodes.Add("0934")
        classcodes.Add("0940")
        classcodes.Add("0952")
        classcodes.Add("1000")
        classcodes.Add("1051")
        classcodes.Add("1052")
        classcodes.Add("1070")
        classcodes.Add("1150")
        classcodes.Add("1180")
        classcodes.Add("1211")
        classcodes.Add("1212")
        classcodes.Add("1213")
        classcodes.Add("1220")
        classcodes.Add("1230")
        classcodes.Add("1400")
        classcodes.Add("1650")
        classcodes.Add("1700")
        classcodes.Add("1751")
        classcodes.Add("1752")
        classcodes.Add("0533")
        classcodes.Add("2200")
        classcodes.Add("2350")
        classcodes.Add("2459")
        classcodes.Add("2800")
        classcodes.Add("3409")
        classcodes.Add("4809")

        ' Default to show all the specific rates rows with the checkboxes checked
        chkBCUseSpecificRates.Checked = True
        trBCUseSpecificRatesRow.Attributes.Add("style", "display:''")
        trBCUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
        trBCGroupIRow.Attributes.Add("style", "display:''")
        trBCGroupIIRow.Attributes.Add("style", "display:''")
        BCUseSpecificVisible = True

        chkBICUseSpecificRates.Checked = True
        trBICUseSpecificRatesRow.Attributes.Add("style", "display:''")
        trBICUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
        trBICGroupIRow.Attributes.Add("style", "display:''")
        trBICGroupIIRow.Attributes.Add("style", "display:''")
        BICUseSpecificVisible = True

        chkPPCUseSpecificRates.Checked = True
        trPPCUseSpecificRatesRow.Attributes.Add("style", "display:''")
        trPPCUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
        trPPCGroupIRow.Attributes.Add("style", "display:''")
        trPPCGroupIIRow.Attributes.Add("style", "display:''")
        PPCUseSpecificVisible = True

        chkPPOUseSpecificRates.Checked = True
        trPPOUseSpecificRatesRow.Attributes.Add("style", "display:''")
        trPPOUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
        trPPOGroupIRow.Attributes.Add("style", "display:''")
        trPPOGroupIIRow.Attributes.Add("style", "display:''")
        PPOUseSpecificVisible = True

        ' If the building class code is one in the list above then uncheck the checkboxes and hide all of the Use Specific Rates rows
        For Each c As String In classcodes
            If cc.ClassCode = c Then
                CCFound = True
                Me.VRScript.AddScriptLine($"$('#{trBCUseSpecificRatesRow.ClientID}').hide();")
                chkBCUseSpecificRates.Checked = False
                trBCUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trBCUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trBCGroupIRow.Attributes.Add("style", "display:none")
                trBCGroupIIRow.Attributes.Add("style", "display:none")
                BCUseSpecificVisible = False

                chkBICUseSpecificRates.Checked = False
                trBICUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trBICUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trBICGroupIRow.Attributes.Add("style", "display:none")
                trBICGroupIIRow.Attributes.Add("style", "display:none")
                BICUseSpecificVisible = False

                chkPPCUseSpecificRates.Checked = False
                trPPCUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trPPCUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trPPCGroupIRow.Attributes.Add("style", "display:none")
                trPPCGroupIIRow.Attributes.Add("style", "display:none")
                PPCUseSpecificVisible = False

                chkPPOUseSpecificRates.Checked = False
                trPPOUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trPPOUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trPPOGroupIRow.Attributes.Add("style", "display:none")
                trPPOGroupIIRow.Attributes.Add("style", "display:none")
                PPOUseSpecificVisible = False

                Exit Sub
            End If
        Next
    End Sub

    Private Sub SetDefaultBuildingCoverageValues()
        Dim defCOLId As String = "3"    ' Special form including theft
        Dim defCOINSId As String = "5"  ' 80%
        Dim defVALId As String = "1"    ' Replacement cost
        Dim defDEDId As String = "8"     ' 500
        Dim HasAgreedAmount = False

        DefaultBCValues = New DefaultBuildingCoverageValues_struct()
        DefaultBICValues = New DefaultBuildingCoverageValues_struct()
        DefaultPPCValues = New DefaultBuildingCoverageValues_struct()
        DefaultPPOValues = New DefaultBuildingCoverageValues_struct()

        ' Set defaults if no values on first building
        DefaultBCValues.CauseOfLossId = defCOLId
        DefaultBCValues.CoinsuranceId = defCOINSId
        DefaultBCValues.ValuationId = defVALId
        DefaultBCValues.DeductibleId = defDEDId
        DefaultBCValues.GroupIValue = ""
        DefaultBCValues.GroupIIValue = ""
        DefaultBCValues.AgreedAmountValue = False
        DefaultBCValues.AgreedAmountEnabled = True

        DefaultBICValues.CauseOfLossId = defCOLId
        DefaultBICValues.CoinsuranceId = defCOINSId
        DefaultBICValues.ValuationId = defVALId
        DefaultBICValues.DeductibleId = defDEDId
        DefaultBICValues.GroupIValue = ""
        DefaultBICValues.GroupIIValue = ""
        DefaultBICValues.AgreedAmountValue = False
        DefaultBICValues.AgreedAmountEnabled = True

        DefaultPPCValues.CauseOfLossId = defCOLId
        DefaultPPCValues.CoinsuranceId = defCOINSId
        DefaultPPCValues.ValuationId = defVALId
        DefaultPPCValues.DeductibleId = defDEDId
        DefaultPPCValues.GroupIValue = ""
        DefaultPPCValues.GroupIIValue = ""
        DefaultPPCValues.AgreedAmountValue = False
        DefaultPPCValues.AgreedAmountEnabled = True

        DefaultPPOValues.CauseOfLossId = defCOLId
        DefaultPPOValues.CoinsuranceId = defCOINSId
        DefaultPPOValues.ValuationId = defVALId
        DefaultPPOValues.DeductibleId = defDEDId
        DefaultPPOValues.GroupIValue = ""
        DefaultPPOValues.GroupIIValue = ""
        DefaultPPOValues.AgreedAmountValue = False
        DefaultPPOValues.AgreedAmountEnabled = True

        ' Set the defaults to the values on the first building OF THE CURRENT LOCATION
        If MyBuilding IsNot Nothing Then
            Dim defbldg As QuickQuote.CommonObjects.QuickQuoteBuilding = Quote.Locations(LocationIndex).Buildings(0)

            ' Set the HasAgreedAmount flag
            ' Location 0 Building 0 IsAgreedAmount holds the default value for all the buildings
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= 1 AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.Count >= 1 Then
                HasAgreedAmount = Quote.Locations(0).Buildings(0).IsAgreedValue
            End If

            ' Building Coverage
            If defbldg.CauseOfLossTypeId <> "" Then DefaultBCValues.CauseOfLossId = defbldg.CauseOfLossTypeId
            If defbldg.CoinsuranceTypeId <> "" Then DefaultBCValues.CoinsuranceId = defbldg.CoinsuranceTypeId
            If defbldg.ValuationId <> "" Then DefaultBCValues.ValuationId = defbldg.ValuationId
            If defbldg.DeductibleId <> "" Then DefaultBCValues.DeductibleId = defbldg.DeductibleId
            If defbldg.Building_BusinessIncome_Group1_LossCost <> "" Then DefaultBCValues.GroupIValue = defbldg.Building_BusinessIncome_Group1_LossCost
            If defbldg.Building_BusinessIncome_Group2_LossCost <> "" Then DefaultBCValues.GroupIIValue = defbldg.Building_BusinessIncome_Group2_LossCost
            If HasAgreedAmount Then
                DefaultBCValues.AgreedAmountValue = True
                DefaultBCValues.AgreedAmountEnabled = False
            End If

            ' Business Income Coverage
            If defbldg.BusinessIncomeCov_CauseOfLossTypeId <> "" Then DefaultBICValues.CauseOfLossId = defbldg.BusinessIncomeCov_CauseOfLossTypeId
            If defbldg.Building_BusinessIncome_Group1_LossCost <> "" Then DefaultBICValues.GroupIValue = defbldg.Building_BusinessIncome_Group1_LossCost
            If defbldg.Building_BusinessIncome_Group2_LossCost <> "" Then DefaultBICValues.GroupIIValue = defbldg.Building_BusinessIncome_Group2_LossCost
            If HasAgreedAmount Then
                DefaultBICValues.AgreedAmountValue = True
                DefaultBICValues.AgreedAmountEnabled = False
            End If

            ' Personal Property Coverage
            If defbldg.PersPropCov_CauseOfLossTypeId <> "" Then DefaultPPCValues.CauseOfLossId = defbldg.PersPropCov_CauseOfLossTypeId
            If defbldg.PersPropCov_CoinsuranceTypeId <> "" Then DefaultPPCValues.CoinsuranceId = defbldg.PersPropCov_CoinsuranceTypeId
            If defbldg.PersPropCov_ValuationId <> "" Then DefaultPPCValues.ValuationId = defbldg.PersPropCov_ValuationId
            If defbldg.PersPropCov_DeductibleId <> "" Then DefaultPPCValues.DeductibleId = defbldg.PersPropCov_DeductibleId
            If defbldg.PersonalProperty_Group1_LossCost <> "" Then DefaultPPCValues.GroupIValue = defbldg.PersonalProperty_Group1_LossCost
            If defbldg.PersonalProperty_Group2_LossCost <> "" Then DefaultPPCValues.GroupIIValue = defbldg.PersonalProperty_Group2_LossCost
            If HasAgreedAmount Then
                DefaultPPCValues.AgreedAmountValue = True
                DefaultPPCValues.AgreedAmountEnabled = False
            End If

            ' Personal Property of Others coverage
            If defbldg.PersPropOfOthers_CauseOfLossTypeId <> "" Then DefaultPPOValues.CauseOfLossId = defbldg.PersPropOfOthers_CauseOfLossTypeId
            If defbldg.PersPropOfOthers_CoinsuranceTypeId <> "" Then DefaultPPOValues.CoinsuranceId = defbldg.PersPropOfOthers_CoinsuranceTypeId
            If defbldg.PersPropOfOthers_ValuationId <> "" Then DefaultPPOValues.ValuationId = defbldg.PersPropOfOthers_ValuationId
            If defbldg.PersPropOfOthers_DeductibleId <> "" Then DefaultPPOValues.DeductibleId = defbldg.PersPropOfOthers_DeductibleId
            If defbldg.PersonalProperty_Group1_LossCost <> "" Then DefaultPPOValues.GroupIValue = defbldg.PersonalProperty_Group1_LossCost
            If defbldg.PersonalProperty_Group2_LossCost <> "" Then DefaultPPOValues.GroupIIValue = defbldg.PersonalProperty_Group2_LossCost
            If HasAgreedAmount Then
                DefaultPPOValues.AgreedAmountValue = True
                DefaultPPOValues.AgreedAmountEnabled = False
            End If
        End If
    End Sub

    Private Sub LoadBICLimitTypeDropdown()
        'Select Case hdnBICLimitTypeValues.Value
        '    Case "M"
        '        ddBICLimitType.Items.Clear()
        '        ddBICLimitType.Items.Add(New ListItem("", "0"))
        '        ddBICLimitType.Items.Add(New ListItem("1/3", "1"))
        '        ddBICLimitType.Items.Add(New ListItem("1/4", "2"))
        '        ddBICLimitType.Items.Add(New ListItem("1/6", "3"))
        '        Exit Select
        '    Case "C"
        '        ddBICLimitType.Items.Clear()
        '        ddBICLimitType.Items.Add(New ListItem("", "0"))
        '        ddBICLimitType.Items.Add(New ListItem("50%", "2"))
        '        ddBICLimitType.Items.Add(New ListItem("60%", "3"))
        '        ddBICLimitType.Items.Add(New ListItem("70%", "4"))
        '        ddBICLimitType.Items.Add(New ListItem("80%", "5"))
        '        ddBICLimitType.Items.Add(New ListItem("90%", "6"))
        '        ddBICLimitType.Items.Add(New ListItem("100%", "7"))
        '        ddBICLimitType.Items.Add(New ListItem("125%", "12"))
        '        Exit Select
        'End Select
        If rbM.Checked Then
            'If rbBICMonthlyPeriod.Checked Then
            ddBICLimitType.Items.Clear()
            ddBICLimitType.Items.Add(New ListItem("", "0"))
            ddBICLimitType.Items.Add(New ListItem("1/3", "1"))
            ddBICLimitType.Items.Add(New ListItem("1/4", "2"))
            ddBICLimitType.Items.Add(New ListItem("1/6", "3"))
        End If
        If rbC.Checked Then
            'If rbBICCoinsurance.Checked Then
            ddBICLimitType.Items.Clear()
            ddBICLimitType.Items.Add(New ListItem("", "0"))
            ddBICLimitType.Items.Add(New ListItem("50%", "2"))
            ddBICLimitType.Items.Add(New ListItem("60%", "3"))
            ddBICLimitType.Items.Add(New ListItem("70%", "4"))
            ddBICLimitType.Items.Add(New ListItem("80%", "5"))
            ddBICLimitType.Items.Add(New ListItem("90%", "6"))
            ddBICLimitType.Items.Add(New ListItem("100%", "7"))
            ddBICLimitType.Items.Add(New ListItem("125%", "12"))
        End If

        Exit Sub
    End Sub

    Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        RaiseEvent ClearBuildingRequested(LocationIndex, BuildingIndex)
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent DeleteBuildingRequested(LocationIndex, BuildingIndex)
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        RaiseEvent NewBuildingRequested(LocationIndex)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Me.ctl_CPR_ClassCodeLookup.Show()
    End Sub

    Private Sub btnPPCLookupEQClassification_Click(sender As Object, e As EventArgs) Handles btnPPCLookupEQClassification.Click
        Save_FireSaveEvent(False)
        Me.ctl_CPR_PPC_EQCCLookup.Show()
    End Sub

    Private Sub btnPPOLookupEQClassCode_Click(sender As Object, e As EventArgs) Handles btnPPOLookupEQClassCode.Click
        Save_FireSaveEvent(False)
        Me.ctl_CPR_PPO_EQCCLookup.Show()
    End Sub

    'Private Sub TEST(sender As Object, e As EventArgs) Handles rbTEST.CheckedChanged
    '    Dim i As Integer = -1
    'End Sub

    Private Sub HandleIncomeTypeChange(sender As Object, e As EventArgs) Handles rbM.CheckedChanged, rbC.CheckedChanged
        LoadBICLimitTypeDropdown()
    End Sub

End Class