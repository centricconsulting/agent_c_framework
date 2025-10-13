Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class ctl_CPR_ENDO_Building
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

    Public ReadOnly Property LocationPropertyDeductibleClientID As String
        Get
            Dim cId As String = ""
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_CPR_ENDO_BuildingList Then
                Dim bl As ctl_CPR_ENDO_BuildingList = CType(Me.ParentVrControl, ctl_CPR_ENDO_BuildingList)
                If bl.ParentVrControl IsNot Nothing AndAlso TypeOf bl.ParentVrControl Is ctl_CPR_ENDO_Location Then
                    cId = CType(bl.ParentVrControl, ctl_CPR_ENDO_Location).LocationPropertyDeductibleClientID
                End If
            End If
            Return cId
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

    Private Property BCSpecificUseChecked As Boolean
        Get
            If hdn_BC_UseSpecific_Checked.Value Is Nothing Then Return False
            Try
                Return CBool(hdn_BC_UseSpecific_Checked.Value)
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(value As Boolean)
            hdn_BC_UseSpecific_Checked.Value = value.ToString
        End Set
    End Property

    Private Property BICSpecificUseChecked As Boolean
        Get
            If hdn_BIC_UseSpecific_Checked.Value Is Nothing Then Return False
            Try
                Return CBool(hdn_BIC_UseSpecific_Checked.Value)
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(value As Boolean)
            hdn_BIC_UseSpecific_Checked.Value = value.ToString
        End Set
    End Property

    Private Property PPCSpecificUseChecked As Boolean
        Get
            If hdn_PPC_UseSpecific_Checked.Value Is Nothing Then Return False
            Try
                Return CBool(hdn_PPC_UseSpecific_Checked.Value)
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(value As Boolean)
            hdn_PPC_UseSpecific_Checked.Value = value.ToString
        End Set
    End Property

    Private Property PPOSpecificUseChecked As Boolean
        Get
            If hdn_PPO_UseSpecific_Checked.Value Is Nothing Then Return False
            Try
                Return CBool(hdn_PPO_UseSpecific_Checked.Value)
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(value As Boolean)
            hdn_PPO_UseSpecific_Checked.Value = value.ToString
        End Set
    End Property

    Private ReadOnly Property bldgQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Me.SubQuoteForLocation(MyLocation)
        End Get
    End Property

    Public ReadOnly Property ScrollToControlId
        Get
            Return lblAccordHeader.ClientID
        End Get
    End Property

    Public ReadOnly ClassificationCodeDictionaryName As String = "Commercial_CppEndorsementClassCode"

    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ClassificationCodeDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, ClassificationCodeDictionaryName)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property


    Public Event BuildingZeroDeductibleChanged()

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

    'Updated 12/20/18 for multi state bug 30442 MLW
    Public ReadOnly Property BlanketCauseOfLossId As String
        Get
            If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
                'If Quote.HasBlanketBuilding Then
                '    Return Quote.BlanketBuildingCauseOfLossTypeId
                'ElseIf Quote.HasBlanketBuildingAndContents Then
                '    Return Quote.BlanketBuildingAndContentsCauseOfLossTypeId
                'ElseIf Quote.HasBlanketContents Then
                '    Return Quote.BlanketContentsCauseOfLossTypeId
                'Else
                '    Return MyBuilding.CauseOfLossTypeId
                'End If
                If SubQuoteFirst.HasBlanketBuilding Then
                    Return SubQuoteFirst.BlanketBuildingCauseOfLossTypeId
                ElseIf SubQuoteFirst.HasBlanketBuildingAndContents Then
                    Return SubQuoteFirst.BlanketBuildingAndContentsCauseOfLossTypeId
                ElseIf SubQuoteFirst.HasBlanketContents Then
                    Return SubQuoteFirst.BlanketContentsCauseOfLossTypeId
                Else
                    Return MyBuilding.CauseOfLossTypeId
                End If
            End If
            Return ""
        End Get
    End Property

    'Updated 12/20/18 for multi state bug 30442 MLW
    Public ReadOnly Property BlanketCoinsuranceId As String
        Get
            If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
                'If Quote.HasBlanketBuilding Then
                '    Return Quote.BlanketBuildingCoinsuranceTypeId
                'End If
                'If Quote.HasBlanketBuildingAndContents Then
                '    Return Quote.BlanketBuildingAndContentsCoinsuranceTypeId
                'End If
                'If Quote.HasBlanketContents Then
                '    Return Quote.BlanketContentsCoinsuranceTypeId
                'End If
                If SubQuoteFirst.HasBlanketBuilding Then
                    Return SubQuoteFirst.BlanketBuildingCoinsuranceTypeId
                End If
                If SubQuoteFirst.HasBlanketBuildingAndContents Then
                    Return SubQuoteFirst.BlanketBuildingAndContentsCoinsuranceTypeId
                End If
                If SubQuoteFirst.HasBlanketContents Then
                    Return SubQuoteFirst.BlanketContentsCoinsuranceTypeId
                End If
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property BlanketDeductibleId As String
        Get
            'Updated 12/20/18 for multi state bug 30442 MLW
            If SubQuoteFirst IsNot Nothing Then
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    If SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketContents Then
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                            If MyLocation.DeductibleId <> "" Then Return MyLocation.DeductibleId Else Return "9"  ' 8 = 500 changed to 1000 per task 62836
                        Else
                            Return "9"   ' 8 = 500 changed to 1000 per task 62836
                        End If
                    Else
                        Return "9"
                    End If
                Else
                    If SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents Then
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                            If Quote.Locations(0).Buildings(0).DeductibleId <> "" Then Return Quote.Locations(0).Buildings(0).DeductibleId Else Return "9"  ' 8 = 500 9 = 1000
                        Else
                            Return "9"   ' 8 = 500 9 = 1000
                        End If
                        'Updated 12/20/18 for multi state bug 30442 MLW
                        'ElseIf Quote.HasBlanketContents Then
                    ElseIf SubQuoteFirst.HasBlanketContents Then
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                            If Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId <> "" Then Return Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId Else Return "9"  ' 8 = 500 9 = 1000
                        Else
                            Return "9"   ' 8 = 500, 9 = 1000
                        End If
                    Else
                        Return "9"
                    End If
                End If
            Else
                Return "9"
            End If
        End Get
    End Property

    'Updated 12/20/18 for multi state bug 30442 MLW
    Public ReadOnly Property BlanketValuationId As String
        Get
            'If Quote IsNot Nothing Then
            '    If Quote.HasBlanketBuilding Then
            '        Return Quote.BlanketBuildingValuationId
            '    End If
            '    If Quote.HasBlanketBuildingAndContents Then
            '        Return Quote.BlanketBuildingAndContentsValuationId
            '    End If
            '    If Quote.HasBlanketContents Then
            '        Return Quote.BlanketContentsValuationId
            '    End If
            'End If
            If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
                If SubQuoteFirst.HasBlanketBuilding Then
                    Return SubQuoteFirst.BlanketBuildingValuationId
                End If
                If SubQuoteFirst.HasBlanketBuildingAndContents Then
                    Return SubQuoteFirst.BlanketBuildingAndContentsValuationId
                End If
                If SubQuoteFirst.HasBlanketContents Then
                    Return SubQuoteFirst.BlanketContentsValuationId
                End If
            End If
            Return ""
        End Get
    End Property

    Public Property BCUseSpecificVisible As Boolean
        Get
            If hdn_BC_UseSpecific_Visible.Value Is Nothing Then Return False
            Return CBool(hdn_BC_UseSpecific_Visible.Value)
        End Get
        Set(value As Boolean)
            hdn_BC_UseSpecific_Visible.Value = value.ToString
        End Set
    End Property

    Private Property BICUseSpecificVisible As Boolean
        Get
            If hdn_BIC_UseSpecific_Visible.Value Is Nothing Then Return False
            Return CBool(hdn_BIC_UseSpecific_Visible.Value)
        End Get
        Set(value As Boolean)
            hdn_BIC_UseSpecific_Visible.Value = value.ToString
        End Set
    End Property

    Private Property PPCUseSpecificVisible As Boolean
        Get
            If hdn_PPC_UseSpecific_Visible.Value Is Nothing Then Return False
            Return CBool(hdn_PPC_UseSpecific_Visible.Value)
        End Get
        Set(value As Boolean)
            hdn_PPC_UseSpecific_Visible.Value = value.ToString
        End Set
    End Property

    Private Property PPOUseSpecificVisible As Boolean
        Get
            If hdn_PPO_UseSpecific_Visible.Value Is Nothing Then Return False
            Return CBool(hdn_PPO_UseSpecific_Visible.Value)
        End Get
        Set(value As Boolean)
            hdn_PPO_UseSpecific_Visible.Value = value.ToString
        End Set
    End Property

    Private ReadOnly Property IsBuildingZero As Boolean
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                If LocationIndex = 0 AndAlso BuildingIndex = 0 Then Return "True"
            End If
            Return "False"
        End Get
    End Property

    ''' <summary>
    ''' Provide a session variable to store selected yard rate
    ''' </summary>
    ''' <returns></returns>
    Private Property YardRateId As String
        Get
            Dim sessID As String = Me.Session.SessionID
            If Session("CPRYardRateId_" & sessID) IsNot Nothing Then Return Session("CPRYardRateId_" & sessID)
            Return ""
        End Get
        Set(value As String)
            Dim sessID As String = Me.Session.SessionID
            Session("CPRYardRateId_" & sessID) = value
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

    Public ReadOnly Property LimitDummyValue As String = "999959999"

    Public Event NewBuildingRequested(ByVal LocIndex As Integer)
    Public Event DeleteBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)
    Public Event ClearBuildingRequested(ByVal LocIndex As Integer, ByVal BldgIndex As Integer)

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        ' Check Ohio Mine sub when effective date changes
        PopulateMineSub(NewEffectiveDate)
        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim HasBlanket As Boolean = False
        Dim BlanketId As String = Nothing
        Dim BlanketText As String = Nothing
        Dim IsBuildingZero As String = "False"

        Me.VRScript.CreateAccordion(divClassCodeLookup.ClientID, hdnCCAccord, "0")
        Me.VRScript.CreateAccordion(divCovs.ClientID, hdnCovsAccord, "0")

        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Building?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSaveBuildingInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearBuildingInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSaveBuildingCoverages.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearBuildingCoverages.ClientID)

        SetDefaultBuildingCoverageValues()

        HasBlanket = QuoteHasBlanket(BlanketId, BlanketText)

        ' Building Coverage script variables
        Me.VRScript.AddVariableLine("Cpr.BuildingCoverageBindings.push(new Cpr.BuildingCoverageUiBinding(" & Me.BuildingIndex.ToString & ",'" & trEarthquakeClassificationRow.ClientID & "','" & Me.ddBCCauseOfLoss.ClientID & "','" & ddBCCoInsurance.ClientID & "','" &
                                    ddBCValuation.ClientID & "','" & ddBCDeductible.ClientID & "','" & txtBCGroupI.ClientID & "','" & txtBCGroupII.ClientID & "','" & ddBICCauseOfLoss.ClientID & "','" &
                                    txtBICGroupI.ClientID & "','" & txtBICGroupII.ClientID & "','" & trBICEarthquakeRow.ClientID & "','" & chkBICEarthquake.ClientID & "','" & ddPPCCauseOfLoss.ClientID & "','" & ddPPCCoinsurance.ClientID & "','" & ddPPCValuation.ClientID & "','" &
                                    ddPPCDeductible.ClientID & "','" & txtPPCGroupI.ClientID & "','" & txtPPCGroupII.ClientID & "','" & trPPCEarthquakeRow.ClientID & "','" & ddPPOCauseOfLoss.ClientID & "','" & ddPPOCoinsurance.ClientID & "','" &
                                    ddPPOValuation.ClientID & "','" & ddPPODeductible.ClientID & "','" & txtPPOGroupI.ClientID & "','" & txtPPOGroupII.ClientID & "','" & trPPOEarthquakeRow.ClientID & "','" & chkBCAgreedAmount.ClientID & "','" &
                                    chkPPCAgreedAmount.ClientID & "','" & trBCGroupIRow.ClientID & "','" & trBCGroupIIRow.ClientID & "','" & trBICGroupIRow.ClientID & "','" & trBICGroupIIRow.ClientID & "','" &
                                    trPPCGroupIRow.ClientID & "','" & trPPCGroupIIRow.ClientID & "','" & trPPOGroupIRow.ClientID & "','" & trPPOGroupIIRow.ClientID & "','" & chkBuildingCoverage.ClientID & "','" &
                                    chkBusinessIncomeCoverage.ClientID & "','" & chkPersonalPropertyCoverage.ClientID & "','" & chkPersonalPropertyOfOthers.ClientID & "','" & hdn_BC_UseSpecific_Visible.ClientID & "','" &
                                    hdn_BIC_UseSpecific_Visible.ClientID & "','" & hdn_PPC_UseSpecific_Visible.ClientID & "','" & hdn_PPO_UseSpecific_Visible.ClientID & "','" & chkPPCEarthquake.ClientID & "','" &
                                    txtPPCEarthquakeClassification.ClientID & "','" & trPPCEarthquakeLookupRow.ClientID & "','" & hdnDIA_PPC_EQCC_Id.ClientID & "','" & chkPPOEarthquake.ClientID & "','" &
                                    txtPPOEQClassification.ClientID & "','" & trPPOEarthquakeLookupRow.ClientID & "','" & hdnDIA_PPO_EQCC_Id.ClientID & "','" & txtINFClassCode.ClientID & "','" &
                                    txtINFDescription.ClientID & "','" & ddINFConstruction.ClientID & "','" & chkINFEarthquake.ClientID & "','" & trBCUseSpecificRatesRow.ClientID & "','" &
                                    trBICUseSpecificRatesRow.ClientID & "','" & trPPCUseSpecificRatesRow.ClientID & "','" & trPPOUseSpecificRatesRow.ClientID & "','" & chkBCUseSpecificRates.ClientID &
                                    "','" & chkBICUseSpecificRates.ClientID & "','" & chkPPCUseSpecificRates.ClientID & "','" & chkPPOUseSpecificRates.ClientID & "','" & trBCUseSpecificRatesInfoRow.ClientID &
                                    "','" & trBICUseSpecificRatesInfoRow.ClientID & "','" & trPPCUseSpecificRatesInfoRow.ClientID & "','" & trPPOUseSpecificRatesInfoRow.ClientID &
                                    "','" & hdn_PPC_UseSpecific_Checked.ClientID & "','" & hdn_PPO_UseSpecific_Checked.ClientID & "','" & hdn_BC_UseSpecific_Checked.ClientID & "','" & hdn_BIC_UseSpecific_Checked.ClientID &
                                    "','" & trBCBlanketRow.ClientID & "','" & chkBCBlanketApplied.ClientID & "','" & trPPCBlanketRow.ClientID & "','" & chkPPCBlanketApplied.ClientID & "','" & trPPOBlanketRow.ClientID & "','" & chkPPOBlanketApplied.ClientID &
                                    "','" & trBCBlanketInfoRow.ClientID & "','" & trPPCBlanketAppliedInfoRow.ClientID & "','" & trPPOBlanketAppliedInfoRow.ClientID & "','" & hdn_Agreed_BC.ClientID & "','" & hdn_Agreed_PPC.ClientID &
                                    "','" & trBCWindHailRow.ClientID & "','" & trPPCWindHailRow.ClientID & "','" & trPPOWindHailRow.ClientID & "','" & chkBCWindHail.ClientID & "','" & chkPPCWindHail.ClientID & "','" & chkPPOWindHail.ClientID & "'));")

        ' Add JS variables to hold the quote and Ohio effective dates - used for mine sub
        Me.VRScript.AddVariableLine("var CPRQuoteEffectiveDate = '" + Quote.EffectiveDate + "';")
        Me.VRScript.AddVariableLine("var CPROhioEffectiveDate = '" + IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate().ToShortDateString() + "';")
        'Me.VRScript.AddVariableLine("var CPROhioBuildingClassCode = '" + Me.txtINFClassCode.Text + "';")

        ' INFO
        VRScript.CreateJSBinding(chkINFEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.LocationEQCheckboxChanged('" & Me.BuildingNumber & "');")
        If IsNewCo() Then
            VRScript.CreateJSBinding(chkINFEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.ToggleEarthquakeDeductible('" & chkINFEarthquake.ClientID & "','" & trEarthquakeDeductibleRow.ClientID & "');")
            VRScript.CreateJSBinding(ddEarthquakeClassification, ctlPageStartupScript.JsEventType.onchange, "Cpr.ResetEarthquakeDeductibleMinimum('" & ddEarthquakeClassification.ClientID & "','" & ddEarthquakeDeductible.ClientID & "');")
        End If
        VRScript.CreateJSBinding(ddINFConstruction, ctlPageStartupScript.JsEventType.onchange, "Cpr.BuildingConstructionChanged('" & Me.BuildingNumber & "');")  ' Added 8/14/18 MGB

        Dim classCodeFound As Boolean = False
        Dim isOkToDefaultWindHail As Boolean = False
        If WindHailDefaultingHelper.IsWindHailDefaultingAvailable(Quote) Then
            isOkToDefaultWindHail = True
            If WindHailDefaultingHelper.CheckCPRCPPExemptCodes(MyBuilding) Then
                classCodeFound = True
            End If
        End If

        'VRScript.CreateJSBinding(ddOwnerOccupiedPercentage, ctlPageStartupScript.JsEventType.onchange, "Cpr.BuildingOccupiedPercentChanged('" & Me.BuildingNumber & "', '" & ddOwnerOccupiedPercentage.ClientID & "', '" & classCodeFound & "', '" & isOkToDefaultWindHail & "','" & BCWindHailText.ClientID & "');")  ' Added 3/17/25 BD

        Dim isBlanketAvailable As Boolean = False
        If FunctionalReplacementCostHelper.IsFunctionalReplacementCostAvailable(Quote) Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                If endorsementsPreexistHelper.IsPreexistingLocation(L) = False Then
                    isBlanketAvailable = True
                End If
            Next
        End If

        Dim showOwnerOccupiedPercentage As Boolean = False
        If OwnerOccupiedPercentageFieldHelper.IsOwnerOccupiedPercentageFieldAvailable(Quote) Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                If endorsementsPreexistHelper.IsPreexistingLocation(L) = False Then
                    showOwnerOccupiedPercentage = True
                End If
            Next
        End If

        Dim hideDeductible As Boolean = False
        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                If endorsementsPreexistHelper.IsPreexistingLocation(L) = False Then
                    hideDeductible = True
                End If
            Next
        End If

        Dim isValuationACVAvailable As Boolean = False
        If ValuationACVHelper.IsValuationACVAvailable(Quote) Then
            isValuationACVAvailable = True
        End If

        ' BUILDING COVERAGE
        VRScript.CreateJSBinding(chkBuildingCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BC'," & BuildingNumber.ToString & ",'" & chkBuildingCoverage.ClientID & "','" & trBuildingCoverageDataRow.ClientID & "','','','" & DefaultBCValues.CauseOfLossId & "','" & DefaultBCValues.CoinsuranceId & "','" & DefaultBCValues.ValuationId & "','" & DefaultBCValues.DeductibleId & "','" & DefaultBCValues.GroupIValue & "','" & DefaultBCValues.GroupIIValue & "','" & DefaultBCValues.AgreedAmountValue.ToString() & "','" & DefaultBCValues.AgreedAmountEnabled.ToString() & "','" & HasBlanket.ToString & "','" & BlanketText & "','" & isBlanketAvailable & "','" & isValuationACVAvailable & "','" & trOwnerOccupiedPercentageRow.ClientID & "','" & showOwnerOccupiedPercentage & "', '" & ddOwnerOccupiedPercentage.ClientID & "', '" & classCodeFound & "', '" & isOkToDefaultWindHail & "','" & trBCWindHailText.ClientID & "');")
        VRScript.CreateJSBinding(chkBCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('BC','" & Me.chkBCBlanketApplied.ClientID & "','" & Me.chkBCAgreedAmount.ClientID & "','" & trBCBlanketInfoRow.ClientID & "','" & Me.ddBCCauseOfLoss.ClientID & "','" & Me.ddBCCoInsurance.ClientID & "','" & Me.ddBCValuation.ClientID & "','" & Me.ddBCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & DefaultBCValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & hdn_Agreed_BC.ClientID & "','" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")
        VRScript.CreateJSBinding(chkBCUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BC'," & BuildingNumber.ToString() & ",'" & chkBCUseSpecificRates.ClientID & "','" & trBCGroupIRow.ClientID & "','" & trBCGroupIIRow.ClientID & "','" & trBCUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkBCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingAgreedAmountCheckboxChanged('" & chkBuildingCoverage.ClientID & "','" & chkBCBlanketApplied.ClientID & "','" & chkBCAgreedAmount.ClientID & "','" & trBCAgreedAmountInfoRow.ClientID & "','" & DefaultBCValues.AgreedAmountValue & "','" & DefaultBCValues.AgreedAmountEnabled.ToString & "','" & hdn_Agreed_BC.ClientID & "');")
        VRScript.CreateJSBinding(txtBCGroupI, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'BC');")
        VRScript.CreateJSBinding(txtBCGroupII, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'BC');")

        ' BUSINESS INCOME COVERAGE
        VRScript.CreateJSBinding(chkBusinessIncomeCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BIC'," & BuildingNumber.ToString & ",'" & chkBusinessIncomeCoverage.ClientID & "','" & trBusinessIncomeDataRow.ClientID & "', '','','" & DefaultBICValues.CauseOfLossId & "','" & DefaultBICValues.CoinsuranceId & "','" & DefaultBICValues.ValuationId & "','" & DefaultBICValues.DeductibleId & "','" & DefaultBICValues.GroupIValue & "','" & DefaultBICValues.GroupIIValue & "','" & DefaultBICValues.AgreedAmountValue.ToString() & "','" & DefaultBICValues.AgreedAmountEnabled.ToString() & "');")
        VRScript.CreateJSBinding(chkBICUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BIC'," & BuildingNumber.ToString & ",'" & chkBICUseSpecificRates.ClientID & "','" & trBICGroupIRow.ClientID & "','" & trBICGroupIIRow.ClientID & "','" & trBICUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(txtBICGroupI, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'BIC');")
        VRScript.CreateJSBinding(txtBICGroupII, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'BIC');")
        VRScript.CreateJSBinding(rbC, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('C','" & rbC.ClientID & "','" & ddBICLimitTypeM.ClientID & "','" & ddBICLimitTypeC.ClientID & "');")
        VRScript.CreateJSBinding(rbM, ctlPageStartupScript.JsEventType.onchange, "Cpr.BICLimitTypeChanged('M','" & rbM.ClientID & "','" & ddBICLimitTypeM.ClientID & "','" & ddBICLimitTypeC.ClientID & "');")

        ' PERSONAL PROPERTY COVERAGE
        VRScript.CreateJSBinding(chkPersonalPropertyCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPC'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyCoverage.ClientID & "','" & trPersonalPropertyDataRow.ClientID & "', '', '','" & DefaultPPCValues.CauseOfLossId & "','" & DefaultPPCValues.CoinsuranceId & "','" & DefaultPPCValues.ValuationId & "','" & DefaultPPCValues.DeductibleId & "','" & DefaultPPCValues.GroupIValue & "','" & DefaultPPCValues.GroupIIValue & "','" & DefaultPPCValues.AgreedAmountValue.ToString() & "','" & DefaultPPCValues.AgreedAmountEnabled.ToString() & "','" & HasBlanket.ToString & "','" & BlanketText & "','" & isBlanketAvailable & "','" & isValuationACVAvailable & "','" & trOwnerOccupiedPercentageRow.ClientID & "','" & showOwnerOccupiedPercentage & "', '" & ddOwnerOccupiedPercentage.ClientID & "', '" & classCodeFound & "', '" & isOkToDefaultWindHail & "','');")
        VRScript.CreateJSBinding(chkPPCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('PPC','" & Me.chkPPCBlanketApplied.ClientID & "','" & Me.chkPPCAgreedAmount.ClientID & "','" & trPPCBlanketAppliedInfoRow.ClientID & "','" & Me.ddPPCCauseOfLoss.ClientID & "','" & Me.ddPPCCoinsurance.ClientID & "','" & Me.ddPPCValuation.ClientID & "','" & Me.ddPPCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & DefaultPPCValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & hdn_Agreed_PPC.ClientID & "','" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")
        VRScript.CreateJSBinding(chkPPCUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPC'," & BuildingNumber.ToString & ",'" & chkPPCUseSpecificRates.ClientID & "','" & trPPCGroupIRow.ClientID & "','" & trPPCGroupIIRow.ClientID & "','" & trPPCUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(chkPPCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingAgreedAmountCheckboxChanged('" & chkPersonalPropertyCoverage.ClientID & "','" & chkPPCBlanketApplied.ClientID & "','" & chkPPCAgreedAmount.ClientID & "','" & trPPCAgreedAmountInfoRow.ClientID & "','" & DefaultPPCValues.AgreedAmountValue & "','" & DefaultPPCValues.AgreedAmountEnabled.ToString & "','" & hdn_Agreed_PPC.ClientID & "');")
        VRScript.CreateJSBinding(txtPPCGroupI, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'PPC');")
        VRScript.CreateJSBinding(txtPPCGroupII, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'PPC');")
        VRScript.CreateJSBinding(chkPPCEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.EQClassificationCheckboxChanged(" & Me.BuildingNumber & ",'PPC');")

        ' PERSONAL PROPERTY OF OTHERS COVERAGE
        VRScript.CreateJSBinding(chkPersonalPropertyOfOthers, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPO'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyOfOthers.ClientID & "','" & trPersonalPropertyOfOthersDataRow.ClientID & "', '', '','" & DefaultPPOValues.CauseOfLossId & "','" & DefaultPPOValues.CoinsuranceId & "','" & DefaultPPOValues.ValuationId & "','" & DefaultPPOValues.DeductibleId & "','" & DefaultPPOValues.GroupIValue & "','" & DefaultPPOValues.GroupIIValue & "','" & DefaultPPOValues.AgreedAmountValue.ToString() & "','" & DefaultPPOValues.AgreedAmountEnabled.ToString() & "','" & HasBlanket.ToString & "','" & BlanketText & "','" & isBlanketAvailable & "','" & isValuationACVAvailable & "','" & trOwnerOccupiedPercentageRow.ClientID & "','" & showOwnerOccupiedPercentage & "', '" & ddOwnerOccupiedPercentage.ClientID & "', '" & classCodeFound & "', '" & isOkToDefaultWindHail & "','');")
        VRScript.CreateJSBinding(chkPPOBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('PPO','" & Me.chkPPOBlanketApplied.ClientID & "','','" & trPPOBlanketAppliedInfoRow.ClientID & "','" & Me.ddPPOCauseOfLoss.ClientID & "','" & Me.ddPPOCoinsurance.ClientID & "','" & Me.ddPPOValuation.ClientID & "','" & Me.ddPPODeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & Me.DefaultPPOValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")
        VRScript.CreateJSBinding(chkPPOUseSpecificRates, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPO'," & BuildingNumber & ",'" & chkPPOUseSpecificRates.ClientID & "','" & trPPOGroupIRow.ClientID & "','" & trPPOGroupIIRow.ClientID & "','" & trPPOUseSpecificRatesInfoRow.ClientID & "','');")
        VRScript.CreateJSBinding(txtPPOGroupI, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'PPO');")
        VRScript.CreateJSBinding(txtPPOGroupII, ctlPageStartupScript.JsEventType.onchange, "Cpr.GroupFieldValueChanged(" & Me.BuildingNumber & ",'PPO');")
        VRScript.CreateJSBinding(chkPPOEarthquake, ctlPageStartupScript.JsEventType.onclick, "Cpr.EQClassificationCheckboxChanged(" & Me.BuildingNumber & ",'PPO');")

        If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
            ' BUILDING COVERAGE
            VRScript.CreateJSBinding(chkBCWindHail, ctlPageStartupScript.JsEventType.onchange, "Cpr.WindHailCheckboxChanged('" & chkBCWindHail.ClientID & "');")
            ' PERSONAL PROPERTY COVERAGE
            VRScript.CreateJSBinding(chkPPCWindHail, ctlPageStartupScript.JsEventType.onchange, "Cpr.WindHailCheckboxChanged('" & chkPPCWindHail.ClientID & "');")
            ' PERSONAL PROPERTY OF OTHERS COVERAGE
            VRScript.CreateJSBinding(chkPPOWindHail, ctlPageStartupScript.JsEventType.onchange, "Cpr.WindHailCheckboxChanged('" & chkPPOWindHail.ClientID & "');")
        End If

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddINFConstruction.Items Is Nothing OrElse ddINFConstruction.Items.Count <= 0 Then
            ' Construction Type
            QQHelper.LoadStaticDataOptionsDropDown(ddINFConstruction, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, , Quote.LobType)

            ' Earthquake Building Classifications
            QQHelper.LoadStaticDataOptionsDropDown(ddEarthquakeClassification, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EarthquakeBuildingClassificationTypeId, , Quote.LobType)
            If IsNewCo() Then
                Dim itemsToRemove As String() = {"25", "26", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44"}
                For Each i In itemsToRemove
                    Dim removeItem As ListItem = ddEarthquakeClassification.Items.FindByValue(i)
                    If removeItem IsNot Nothing Then
                        Me.ddEarthquakeClassification.Items.Remove(removeItem)
                    End If
                Next
                QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddEarthquakeDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EarthquakeDeductibleId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
                Dim removeBlank As ListItem = ddEarthquakeDeductible.Items.FindByValue("-1")
                If removeBlank IsNot Nothing Then
                    Me.ddEarthquakeDeductible.Items.Remove(removeBlank)
                End If
            End If

            ' BUILDING COVERAGES (BC)
            ' Inflation Guard
            QQHelper.LoadStaticDataOptionsDropDown(ddBCInflationGuard, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, , Quote.LobType)
            If MyBuilding IsNot Nothing Then
                'Added 10/20/2022 for task 77527 MLW
                ToggleInflationGuardOptions()
            End If
            ' Cause of loss
            QQHelper.LoadStaticDataOptionsDropDown(ddBCCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, , Quote.LobType)
            ' Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddBCCoInsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, , Quote.LobType)
            ' Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddBCValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketContentsValuationId, , Quote.LobType)
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
            'Removing Building covarage section
            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                    QQHelper.LoadStaticDataOptionsDropDown(ddBCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
                    QQHelper.LoadStaticDataOptionsDropDown(ddPPCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
                    QQHelper.LoadStaticDataOptionsDropDown(ddPPODeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
                Else
                    QQHelper.LoadStaticDataOptionsDropDown(ddBCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
                    QQHelper.LoadStaticDataOptionsDropDown(ddPPCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)
                    QQHelper.LoadStaticDataOptionsDropDown(ddPPODeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)
                End If
            Else
                QQHelper.LoadStaticDataOptionsDropDown(ddBCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(ddPPCDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(ddPPODeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersPropCov_DeductibleId, , Quote.LobType)
            End If


            If CPRRemovePropDedBelow1k.IsPropertyDeductibleBelow1kAvailable(Quote) Then
                Dim Item500 = New ListItem("500", "8")
                ddBCDeductible.Items.Remove(Item500)
                ddPPCDeductible.Items.Remove(Item500)
                ddPPODeductible.Items.Remove(Item500)
            End If
            'Owner Occupied Percentage
            QQHelper.LoadStaticDataOptionsDropDown(ddOwnerOccupiedPercentage, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OwnerOccupiedPercentageId, , Quote.LobType)


        End If
    End Sub

    ''' <summary>
    ''' Checks to see if blanket is set at loc 0 bldg 0.  If so returns true and sets the blanket type number and text
    ''' </summary>
    ''' <param name="BlanketTypeNumber"></param>
    ''' <param name="BlanketTypeText"></param>
    ''' <returns></returns>
    Private Function QuoteHasBlanket(ByRef BlanketTypeNumber As String, ByRef BlanketTypeText As String) As Boolean
        If bldgQuote IsNot Nothing Then
            If bldgQuote.HasBlanketBuilding OrElse bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketContents Then
                BlanketTypeNumber = GetBlanketTypeNumber(bldgQuote)
                Select Case BlanketTypeNumber
                    Case "1"
                        BlanketTypeText = "COMBINED"
                        Exit Select
                    Case "2"
                        BlanketTypeText = "BUILDING"
                        Exit Select
                    Case "3"
                        BlanketTypeText = "PROPERTY"
                        Exit Select
                End Select
                Return True
            Else
                BlanketTypeNumber = Nothing
                BlanketTypeText = Nothing
                Return False
            End If
        Else
            BlanketTypeNumber = Nothing
            BlanketTypeText = Nothing
            Return False
        End If

        Return "0"

    End Function

    Private Function GetBlanketTypeNumber(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject) As String
        If qo.HasBlanketBuilding OrElse qo.HasBlanketBuildingAndContents OrElse qo.HasBlanketContents Then
            If qo.HasBlanketBuilding Then
                Return "2"
            End If
            If qo.HasBlanketBuildingAndContents Then
                Return "1"
            End If
            If qo.HasBlanketContents Then
                Return "3"
            End If
        End If
        Return "0"
    End Function

    ''' <summary>
    ''' Call this routine anytime something changes to change the value of a default
    ''' Updates the building coverage js bindings
    ''' </summary>
    'Private Sub UpdateBuildingCoverageBindings()
    '    ' Recalculate the defauilt values
    '    SetDefaultBuildingCoverageValues()

    '    ' Update the bindings
    '    VRScript.CreateJSBinding(chkBuildingCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BC'," & BuildingNumber.ToString & ",'" & chkBuildingCoverage.ClientID & "','" & trBuildingCoverageDataRow.ClientID & "', '','','" & DefaultBCValues.CauseOfLossId & "','" & DefaultBCValues.CoinsuranceId & "','" & DefaultBCValues.ValuationId & "','" & DefaultBCValues.DeductibleId & "','" & DefaultBCValues.GroupIValue & "','" & DefaultBCValues.GroupIIValue & "','" & DefaultBCValues.AgreedAmountValue.ToString() & "','" & DefaultBCValues.AgreedAmountEnabled.ToString() & "');")
    '    VRScript.CreateJSBinding(chkBusinessIncomeCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BIC'," & BuildingNumber.ToString & ",'" & chkBusinessIncomeCoverage.ClientID & "','" & trBusinessIncomeDataRow.ClientID & "', '','','" & DefaultBICValues.CauseOfLossId & "','" & DefaultBICValues.CoinsuranceId & "','" & DefaultBICValues.ValuationId & "','" & DefaultBICValues.DeductibleId & "','" & DefaultBICValues.GroupIValue & "','" & DefaultBICValues.GroupIIValue & "','" & DefaultBICValues.AgreedAmountValue.ToString() & "','" & DefaultBICValues.AgreedAmountEnabled.ToString() & "');")
    '    VRScript.CreateJSBinding(chkPersonalPropertyCoverage, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPC'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyCoverage.ClientID & "','" & trPersonalPropertyDataRow.ClientID & "', '', '','" & DefaultPPCValues.CauseOfLossId & "','" & DefaultPPCValues.CoinsuranceId & "','" & DefaultPPCValues.ValuationId & "','" & DefaultPPCValues.DeductibleId & "','" & DefaultPPCValues.GroupIValue & "','" & DefaultPPCValues.GroupIIValue & "','" & DefaultPPCValues.AgreedAmountValue.ToString() & "','" & DefaultPPCValues.AgreedAmountEnabled.ToString() & "','" & "');")
    '    VRScript.CreateJSBinding(chkPersonalPropertyOfOthers, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPO'," & BuildingNumber.ToString & ",'" & chkPersonalPropertyOfOthers.ClientID & "','" & trPersonalPropertyOfOthersDataRow.ClientID & "', '', '','" & DefaultPPOValues.CauseOfLossId & "','" & DefaultPPOValues.CoinsuranceId & "','" & DefaultPPOValues.ValuationId & "','" & DefaultPPOValues.DeductibleId & "','" & DefaultPPOValues.GroupIValue & "','" & DefaultPPOValues.GroupIIValue & "','" & DefaultPPOValues.AgreedAmountValue.ToString() & "','" & DefaultPPOValues.AgreedAmountEnabled.ToString() & "');")
    '    VRScript.CreateJSBinding(chkBCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('BC'," & BuildingNumber.ToString & ",'" & chkBCAgreedAmount.ClientID & "','','','" & trBCAgreedAmountInfoRow.ClientID & "','', '', '', '', '', '','" & DefaultBCValues.AgreedAmountValue & "','" & DefaultBCValues.AgreedAmountEnabled & "');")
    '    VRScript.CreateJSBinding(chkPPCAgreedAmount, ctlPageStartupScript.JsEventType.onclick, "Cpr.BuildingCoverageCheckboxChanged('PPC'," & BuildingNumber & ",'" & chkPPCAgreedAmount.ClientID & "','','','" & trPPCAgreedAmountInfoRow.ClientID & "','','','','','','','" & DefaultPPCValues.AgreedAmountValue & "','" & DefaultPPCValues.AgreedAmountEnabled & "');")

    '    Exit Sub
    'End Sub

    Public Overrides Sub Populate()
        Dim HasBlanket As Boolean = False
        Dim StoredClassCode As DevDictionaryHelper.CppEndorsementClassCode

        If MyBuilding Is Nothing Then Exit Sub
        LoadStaticData()

        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)

        ctl_BOP_ENDO_App_Building.LocationIndex = Me.LocationIndex
        ctl_BOP_ENDO_App_Building.BuildingIndex = BuildingIndex

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
        trBCBlanketRow.Attributes.Add("style", "display:none")
        trPPCBlanketRow.Attributes.Add("style", "display:none")
        trPPOBlanketRow.Attributes.Add("style", "display:none")
        trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
        trEarthquakeDeductibleRow.Attributes.Add("style", "display:none")
        trOwnerOccupiedPercentageRow.Attributes.Add("style", "display:none")
        trBCWindHailRow.Attributes.Add("style", "display:none")
        trPPCWindHailRow.Attributes.Add("style", "display:none")
        trPPOWindHailRow.Attributes.Add("style", "display:none")

        ' Get the quote for this location
        If bldgQuote Is Nothing Then Exit Sub

        ' The specific rates rows are always shown UNLESS N/A was chosen for blanket coverage on the coverages page
        If bldgQuote.HasBlanketBuilding OrElse bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketContents Then
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

        Dim RiskTypeClassCode As String = String.Empty


        If MyBuilding.ClassificationCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode <> "" Then RiskTypeClassCode = MyBuilding.ClassificationCode.ClassCode
        If RiskTypeClassCode.IsNullEmptyorWhitespace AndAlso MyBuilding.PersPropCov_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode <> "" Then RiskTypeClassCode = MyBuilding.PersPropCov_ClassificationCode.ClassCode
        If RiskTypeClassCode.IsNullEmptyorWhitespace AndAlso MyBuilding.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode <> "" Then RiskTypeClassCode = MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode
        If RiskTypeClassCode.IsNullEmptyorWhitespace AndAlso MyBuilding.BusinessIncomeCov_ClassificationCode IsNot Nothing AndAlso String.IsNullOrWhiteSpace(MyBuilding.BusinessIncomeCov_ClassificationCode.ClassCode) = False Then RiskTypeClassCode = MyBuilding.BusinessIncomeCov_ClassificationCode.ClassCode

        StoredClassCode = ddh.GetClassificationCodeForLocationAndBuildingIndex(Me.LocationIndex.ToString, Me.BuildingIndex.ToString)
        If String.IsNullOrWhiteSpace(RiskTypeClassCode) Then
            If StoredClassCode IsNot Nothing Then
                RiskTypeClassCode = StoredClassCode.ClassCode
            End If
        End If

        ' Risk Type
        Select Case RiskTypeClassCode
            Case "0311", "0312", "0313", "0321", "0322", "0323", "0196", "0197", "0198", "0331", "0332", "0333", "0341", "0342", "0343"
                ' Risk Type  Default for Apt, Condo and Dwelling Class Codes
                ' ID 3 = Type 1 - Apartments and Condos
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPORiskType, "3")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCRiskType, "3")
                Exit Select
            Case "0702"
                ' Risk type default for Office
                ' ID 4 = Type 2 - Office
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPORiskType, "4")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCRiskType, "4")
                Exit Select
            Case Else
                ' All other class codes
                ' ID 5 = Type 3 - All Other Personal Property
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPORiskType, "5")
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCRiskType, "5") '"Type 3 - All Other Personal Property"
                Exit Select
        End Select

        If ValuationACVHelper.IsValuationACVAvailable(Quote) AndAlso QQHelper.IsQuickQuoteLocationNewToImage(MyLocation, Quote) _
                    AndAlso ValuationACVHelper.IsBuildingDwellingClass(RiskTypeClassCode) Then
            Dim removeRCFromBC As ListItem = ddBCValuation.Items.FindByValue("1")
            If removeRCFromBC IsNot Nothing Then
                ddBCValuation.Items.Remove(removeRCFromBC)
            End If
            Dim removeRCFromPPC As ListItem = ddPPCValuation.Items.FindByValue("1")
            If removeRCFromPPC IsNot Nothing Then
                ddPPCValuation.Items.Remove(removeRCFromPPC)
            End If
            Dim removeRCFromPPO As ListItem = ddPPOValuation.Items.FindByValue("1")
            If removeRCFromPPO IsNot Nothing Then
                ddPPOValuation.Items.Remove(removeRCFromPPO)
            End If
        End If

        'Functional Replacement Cost = 7
        If MyLocation IsNot Nothing AndAlso ValuationACVHelper.IsBuildingDwellingClass(RiskTypeClassCode) AndAlso endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
            Dim removeFRCFromBC As ListItem = ddBCValuation.Items.FindByValue("7")
            If removeFRCFromBC IsNot Nothing Then
                ddBCValuation.Items.Remove(removeFRCFromBC)
            End If
            Dim removeFRCFromPPC As ListItem = ddPPCValuation.Items.FindByValue("7")
            If removeFRCFromPPC IsNot Nothing Then
                ddPPCValuation.Items.Remove(removeFRCFromPPC)
            End If
            Dim removeFRCFromPPO As ListItem = ddPPOValuation.Items.FindByValue("7")
            If removeFRCFromPPO IsNot Nothing Then
                ddPPOValuation.Items.Remove(removeFRCFromPPO)
            End If
        End If

        ' Valuation
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddBCValuation, "1")      ' Default is Replacement Cost

        ' Cause of Loss
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddBCCauseOfLoss, "3")    ' Default is Special From Including Theft

        ' ** BIC Business Income Coverage
        ' Cause of Loss
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddBICCauseOfLoss, "3")    ' Default is Special From Including Theft

        ' ** PPC Personal Property Coverage
        ' Risk Type
        'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCRiskType, "5")   '  "Type 3 - All Other Personal Property"
        ' Property Type
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCPropertyType, "7")  ' Default is "Personal Property - Including Stock"
        ' Cause of Loss
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCCauseOfLoss, "3")    ' Default is Special From Including Theft

        ' Default is "Type 3 - All Other Personal Property"
        ' Property Type
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCPropertyType, "7")  ' Default is "Personal Property - Including Stock"
        ' Cause of Loss
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPCCauseOfLoss, "3")    ' Default is Special From Including Theft

        ' ** PPO Personal Property of Others
        ' Cause of Loss

        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPOCauseOfLoss, "3")    ' Default is Special From Including Theft

        'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddPPORiskType, "5")      ' Default is "Type 3 - All Other Personal condoProperty"

        'End Defaults

        ' Set properties on the class code lookup control
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentClassCodeTextboxId = Me.txtINFClassCode.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentDescriptionTextboxId = Me.txtINFDescription.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentIDHdnId = Me.hdnDIA_Id.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPMAIDHdnId = Me.hdnPMAID.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentGroupRateHdnId = Me.hdnGroupRate.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentClassLimitHdnId = Me.hdnClassLimit.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentYardRateHdnId = Me.hdnYardRateId.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_ENDO_ClassCodeLookup.BuildingIndex = BuildingIndex

        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBCUseCodeRowId = Me.trBCUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBCUseCodeInfoRowId = Me.trBCUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBCUseCodeGroupIRowId = Me.trBCGroupIRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBCUseCodeGroupIIRowId = Me.trBCGroupIIRow.ClientID

        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBICUseCodeRowId = Me.trBICUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBICUseCodeInfoRowId = Me.trBICUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBICUseCodeGroupIRowId = Me.trBICGroupIRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentBICUseCodeGroupIIRowId = Me.trBICGroupIIRow.ClientID

        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPCUseCodeRowId = Me.trPPCUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPCUseCodeInfoRowId = Me.trPPCUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPCUseCodeGroupIRowId = Me.trPPCGroupIRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPCUseCodeGroupIIRowId = Me.trPPCGroupIIRow.ClientID

        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPOUseCodeRowId = Me.trPPOUseSpecificRatesRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPOUseCodeInfoRowId = Me.trPPOUseSpecificRatesInfoRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPOUseCodeGroupIRowId = Me.trPPOGroupIRow.ClientID
        Me.ctl_CPR_ENDO_ClassCodeLookup.ParentPPOUseCodeGroupIIRowId = Me.trPPOGroupIIRow.ClientID

        ' Set properties on the Earthquake Class Code lookup controls (we have two, one for PPC and one for PPO)
        ' PPC
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.ParentClassCodeTextboxId = Me.txtPPCEarthquakeClassification.ClientID
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.ParentHdnId = Me.hdnDIA_PPC_EQCC_Id.ClientID
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.ParentHdnRateGroupId = Me.hdn_PPC_RateGroup.ClientID
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.BuildingIndex = BuildingIndex
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.LookupType = "PPC"
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.ParentEQPPCDataRow1Id = Me.trPPCEarthquakeLookupRow.ClientID
        ' PPO
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.ParentClassCodeTextboxId = Me.txtPPOEQClassification.ClientID
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.ParentHdnId = Me.hdnDIA_PPO_EQCC_Id.ClientID
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.ParentHdnRateGroupId = Me.hdn_PPO_RateGroup.ClientID
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.LocationIndex = LocationIndex
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.BuildingIndex = BuildingIndex
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.LookupType = "PPO"
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.ParentEQPPODataRow1Id = Me.trPPOEarthquakeLookupRow.ClientID

        If MyBuilding.IsNotNull Then
            ' Set the building header
            Me.lblAccordHeader.Text = "Building # " & BuildingIndex + 1.ToString

            ' CONSTRUCTION - Added 8/15/18 MGB
            SetFromValue(ddINFConstruction, MyBuilding.ConstructionId)

            ' Set the class code value fields
            ' Note that the classification codes should be set for all building coverages regardless if the coverage is selected
            Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = Nothing
            If MyBuilding.ClassificationCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode <> "" Then cc = MyBuilding.ClassificationCode
            If cc Is Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode <> "" Then cc = MyBuilding.PersPropCov_ClassificationCode
            If cc Is Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode <> "" Then cc = MyBuilding.PersPropOfOthers_ClassificationCode
            If cc Is Nothing AndAlso MyBuilding.BusinessIncomeCov_ClassificationCode IsNot Nothing AndAlso String.IsNullOrWhiteSpace(MyBuilding.BusinessIncomeCov_ClassificationCode.ClassCode) = False Then cc = MyBuilding.BusinessIncomeCov_ClassificationCode
            If cc Is Nothing OrElse String.IsNullOrWhiteSpace(cc.ClassCode) Then
                If StoredClassCode Is Nothing Then
                    StoredClassCode = ddh.GetClassificationCodeForLocationAndBuildingIndex(Me.LocationIndex.ToString, Me.BuildingIndex.ToString)
                    'StoredClassCode will be nothing if there is no value to return so test again as below
                End If
                If StoredClassCode IsNot Nothing Then
                    cc = StoredClassCode.ConvertToQuickQuoteClassificationCode()
                End If
                'Else
                '    'Deletes an existing CC in DD because we have one in the quote.
                '    ddh.DoesCCExist(Me.LocationIndex, Me.BuildingIndex, True)
            End If

            'Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = MyBuilding.ClassificationCode
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
        If MyBuilding.EarthquakeBuildingClassificationTypeId <> "" And MyBuilding.EarthquakeBuildingClassificationTypeId <> "0" AndAlso IsNumeric(MyBuilding.EarthquakeBuildingClassificationTypeId) Then
            chkINFEarthquake.Checked = True
            trEarthquakeClassificationRow.Attributes.Add("style", "display:''")
            If IsNewCo() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddEarthquakeClassification, MyBuilding.EarthquakeBuildingClassificationTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.EarthquakeBuildingClassificationTypeId, Quote.LobType)
                trEarthquakeDeductibleRow.Attributes.Add("style", "display:''")
                SetFromValue(ddEarthquakeDeductible, MyBuilding.EarthquakeDeductibleId)
                If ddEarthquakeClassification.SelectedValue.EqualsAny("16", "19", "20", "22", "23", "24") Then
                    If ddEarthquakeDeductible.Items.Contains(New ListItem("5%", "34")) Then
                        ddEarthquakeDeductible.Items.FindByValue("34").Attributes.Add("disabled", "disabled")
                    End If
                End If
            Else
                ddEarthquakeClassification.SelectedValue = MyBuilding.EarthquakeBuildingClassificationTypeId
                trEarthquakeDeductibleRow.Attributes.Add("style", "display:none")
            End If
        Else
            chkINFEarthquake.Checked = False
            trEarthquakeClassificationRow.Attributes.Add("style", "display:none")
            trEarthquakeDeductibleRow.Attributes.Add("style", "display:none")
        End If

        '*** COVERAGES ***
        BCUseSpecificVisible = False
        BICUseSpecificVisible = False
        PPCUseSpecificVisible = False
        PPOUseSpecificVisible = False

        Dim bcValuationUpdated As Boolean = False
        Dim ppcValuationUpdated As Boolean = False
        Dim ppoValuationUpdated As Boolean = False

        Dim bcDeductibleToUse As String = ""
        Dim ppcDeductibleToUse As String = ""
        Dim ppoDeductibleToUse As String = ""

        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            If Quote.Locations.IsLoaded() Then
                If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                    bcDeductibleToUse = MyLocation.DeductibleId
                    ppcDeductibleToUse = MyLocation.DeductibleId
                    ppoDeductibleToUse = MyLocation.DeductibleId
                Else
                    bcDeductibleToUse = MyBuilding.DeductibleId
                    ppcDeductibleToUse = MyBuilding.PersPropCov_DeductibleId
                    ppoDeductibleToUse = MyBuilding.PersPropOfOthers_DeductibleId
                End If
            End If
        Else
            bcDeductibleToUse = MyBuilding.DeductibleId
            ppcDeductibleToUse = MyBuilding.PersPropCov_DeductibleId
            ppoDeductibleToUse = MyBuilding.PersPropOfOthers_DeductibleId
        End If

        ' Building Coverage
        If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) Then
            trBuildingCoverageDataRow.Attributes.Add("style", "display:''")
            chkBuildingCoverage.Checked = True
            txtBCBuildingLimit.Text = MyBuilding.Limit
            If txtBCBuildingLimit.Text.ToAlphaNumeric = LimitDummyValue Then
                txtBCBuildingLimit.Text = String.Empty
            End If
            SetDropDownValue_ForceDiamondValue(ddBCInflationGuard, MyBuilding.InflationGuardTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.InflationGuardTypeId)
            'update inflation guard DD to set default option to 2 and remove N/A - ZTS 2/28/20 task 40488
            'If MyBuilding.InflationGuardTypeId <> "" Then
            '    If MyBuilding.InflationGuardTypeId = "0" Then
            '        ddBCInflationGuard.SelectedValue = "1"
            '    Else
            '        ddBCInflationGuard.SelectedValue = MyBuilding.InflationGuardTypeId
            '    End If
            'End If
            If Not ddBCInflationGuard.Items.FindByValue("0") Is Nothing Then ddBCInflationGuard.Items.Remove("N/A")
            If HasBlanket Then
                If bldgQuote.HasBlanketBuilding OrElse bldgQuote.HasBlanketBuildingAndContents Then
                    ' Only show the blanket row if the quote has building or building & contents blanket
                    chkBCBlanketApplied.Enabled = True
                    chkBCBlanketApplied.Checked = MyBuilding.IsBuildingValIncludedInBlanketRating
                    trBCBlanketRow.Attributes.Add("style", "display:''")
                End If
            Else
                chkBCBlanketApplied.Enabled = False
                chkBCBlanketApplied.Checked = False
            End If
            If chkBCBlanketApplied.Checked Then
                trBCBlanketInfoRow.Attributes.Add("style", "display:''")
                ddBCCauseOfLoss.Attributes.Add("disabled", "True")
                If BlanketCauseOfLossId <> "" Then SetdropDownFromValue(ddBCCauseOfLoss, BlanketCauseOfLossId)
                ddBCCoInsurance.Attributes.Add("disabled", "True")

                If BlanketCoinsuranceId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddBCCoInsurance, BlanketCoinsuranceId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
                End If
                ddBCValuation.Attributes.Add("disabled", "True")

                If BlanketValuationId <> "" Then SetdropDownFromValue(ddBCValuation, BlanketValuationId)
                ddBCDeductible.Attributes.Add("disabled", "True")
                If BlanketDeductibleId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddBCDeductible, BlanketDeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                End If
            Else
                trBCBlanketInfoRow.Attributes.Add("style", "display:none")
                ddBCCauseOfLoss.Attributes.Remove("disabled")
                ddBCCoInsurance.Attributes.Remove("disabled")
                ddBCValuation.Attributes.Remove("disabled")
                ddBCDeductible.Attributes.Remove("disabled")
            End If
            ' Use Specific Rates
            DisplaySpecificRates(CPRBuildingCoverageType.BuildingCoverage)
            ' Cause of Loss
            If MyBuilding.CauseOfLossTypeId <> "" Then SetdropDownFromValue(ddBCCauseOfLoss, MyBuilding.CauseOfLossTypeId)
            ' Co-Insurance
            If MyBuilding.CoinsuranceTypeId <> "" Then SetDropDownValue_ForceDiamondValue(ddBCCoInsurance, MyBuilding.CoinsuranceTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
            ' Valuation
            If ValuationACVHelper.IsValuationACVAvailable(Quote) AndAlso QQHelper.IsQuickQuoteLocationNewToImage(MyLocation, Quote) Then
                bcValuationUpdated = ValuationACVHelper.UpdateValuation(MyBuilding, ValuationACVHelper.BuildingCoverage)
            End If
            If MyBuilding.ValuationId <> "" Then SetdropDownFromValue(ddBCValuation, MyBuilding.ValuationId)
            ' Deductible
            If bcDeductibleToUse <> "" Then
                SetDropDownValue_ForceDiamondValue(ddBCDeductible, bcDeductibleToUse, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
            End If
            'Owner Occupied Percentage
            If MyBuilding.OwnerOccupiedPercentageId <> "" Then SetdropDownFromValue(ddOwnerOccupiedPercentage, MyBuilding.OwnerOccupiedPercentageId)
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
                PopulateCoverageWindHail(LocationWindHailHelper.BuildingCov)
            End If

            If WindHailDefaultingHelper.IsWindHailDefaultingAvailable(Quote) Then
                If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                    DefaultBuildingWindHailCoverage()
                Else
                    trBCWindHailText.Attributes.Add("style", "display:none")
                End If
            Else
                trBCWindHailText.Attributes.Add("style", "display:none")
            End If
        End If

        'Check for Illinois Condominium RiskGrade
        Dim CHC As CommonHelperClass = New CommonHelperClass
        If QQHelper.QuoteHasCondominiumRiskGrade(Quote) Then
            CHC.AddCSSClassToControl(ddBCCauseOfLoss, "IL_CondoRiskGrade_Col")
            CHC.AddCSSClassToControl(ddBCValuation, "IL_CondoRiskGrade_Val")
        End If

        ' Business Income Coverage
        If HasCoverage(CPRBuildingCoverageType.BusinessIncomeCoverage) Or bldgQuote.HasBusinessIncomeALS Then
            BICUseSpecificVisible = True
            chkBusinessIncomeCoverage.Checked = True
            trBusinessIncomeDataRow.Attributes.Add("style", "display''")

            ' If the quote has business income als, hide the Limit and Limit Type rows, check the BIC checkbox and disable it
            If bldgQuote.HasBusinessIncomeALS Then
                chkBusinessIncomeCoverage.Checked = True
                chkBusinessIncomeCoverage.Enabled = False
                trBICLimit.Attributes.Add("style", "display:none")
                trBICLimitTypeRow.Attributes.Add("style", "display:none")
            Else
                chkBusinessIncomeCoverage.Enabled = True
                trBICLimit.Attributes.Add("style", "display:''")
                trBICLimitTypeRow.Attributes.Add("style", "display:''")
            End If

            If IsNumeric(MyBuilding.BusinessIncomeCov_Limit) Then ' MyBuilding.BusinessIncomeCov_Limit <> "0" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "1" AndAlso
                txtBICBusinessIncomeLimit.Text = MyBuilding.BusinessIncomeCov_Limit
            Else
                txtBICBusinessIncomeLimit.Text = ""
            End If

            If txtBICBusinessIncomeLimit.Text.ToAlphaNumeric = LimitDummyValue Then
                txtBICBusinessIncomeLimit.Text = String.Empty
            End If

            ' Limit Type
            ddBICLimitTypeM.Attributes.Add("style", "display:none")
            ddBICLimitTypeC.Attributes.Add("style", "display:none")
            If MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "0" Then
                rbM.Checked = True
                ddBICLimitTypeM.Attributes.Add("style", "display:''")
                'LoadBICLimitTypeDropdown()
                If MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId <> "" Then SetdropDownFromValue(ddBICLimitTypeM, MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId)
            End If
            If MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "0" Then
                rbC.Checked = True
                ddBICLimitTypeC.Attributes.Add("style", "display:''")
                'LoadBICLimitTypeDropdown()
                If MyBuilding.BusinessIncomeCov_CoinsuranceTypeId <> "" Then SetdropDownFromValue(ddBICLimitTypeC, MyBuilding.BusinessIncomeCov_CoinsuranceTypeId)
            End If

            'Set Items for New Locations
            Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
            AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
            If bldgQuote.HasBusinessIncomeALS AndAlso AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation) = False Then
                Dim FirstLocation = Quote.Locations(0).Buildings(0)
                SetdropDownFromValue(ddBICIncomeType, FirstLocation.BusinessIncomeCov_BusinessIncomeTypeId)
                SetdropDownFromValue(ddBICRiskType, FirstLocation.BusinessIncomeCov_RiskTypeId)
                SetdropDownFromValue(ddBICCauseOfLoss, FirstLocation.BusinessIncomeCov_CauseOfLossTypeId)
                VRScript.FakeDisableSingleElement(ddBICCauseOfLoss)
                DisplaySpecificRates(CPRBuildingCoverageType.BusinessIncomeCoverage)
            Else
                ' Income Type
                If MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId <> "" AndAlso MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId <> "0" Then
                    SetdropDownFromValue(ddBICIncomeType, MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId)
                Else
                    SetdropDownFromValue(ddBICIncomeType, "1")
                End If
                ' Risk Type
                If MyBuilding.BusinessIncomeCov_RiskTypeId IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_RiskTypeId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddBICRiskType, MyBuilding.BusinessIncomeCov_RiskTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.BusinessIncomeCov_RiskTypeId)
                Else
                    SetdropDownFromValue(ddBICRiskType, "0")
                End If

                If MyBuilding.BusinessIncomeCov_RiskTypeId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddBICRiskType, MyBuilding.BusinessIncomeCov_RiskTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.BusinessIncomeCov_RiskTypeId)
                End If
                ' Use Specific Rates
                DisplaySpecificRates(CPRBuildingCoverageType.BusinessIncomeCoverage)
                ' Cause of Loss
                If MyBuilding.BusinessIncomeCov_CauseOfLossTypeId <> "" Then SetdropDownFromValue(ddBICCauseOfLoss, MyBuilding.BusinessIncomeCov_CauseOfLossTypeId)

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
            If txtPPCPersonalPropertyLimit.Text.ToAlphaNumeric = LimitDummyValue Then
                txtPPCPersonalPropertyLimit.Text = String.Empty
            End If
            If MyBuilding.PersPropCov_PropertyTypeId <> "" Then SetdropDownFromValue(ddPPCPropertyType, MyBuilding.PersPropCov_PropertyTypeId)
            If MyBuilding.PersPropCov_RiskTypeId <> "" Then SetdropDownFromValue(ddPPCRiskType, MyBuilding.PersPropCov_RiskTypeId)
            If HasBlanket Then
                If bldgQuote.HasBlanketContents OrElse bldgQuote.HasBlanketBuildingAndContents Then
                    ' Only show the blanket row if the quote has contents only or building & contents blanket
                    chkPPCBlanketApplied.Enabled = True
                    chkPPCBlanketApplied.Checked = MyBuilding.PersPropCov_IncludedInBlanketCoverage
                    trPPCBlanketRow.Attributes.Add("style", "display:''")
                End If
            Else
                chkPPCBlanketApplied.Enabled = False
                chkPPCBlanketApplied.Checked = False
            End If
            If chkPPCBlanketApplied.Checked Then
                trPPCBlanketAppliedInfoRow.Attributes.Add("style", "display:''")
                ddPPCCauseOfLoss.Attributes.Add("disabled", "True")
                If BlanketCauseOfLossId <> "" Then SetdropDownFromValue(ddPPCCauseOfLoss, BlanketCauseOfLossId)
                ddPPCCoinsurance.Attributes.Add("disabled", "True")
                If BlanketCoinsuranceId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddPPCCoinsurance, BlanketCoinsuranceId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
                End If
                ddPPCValuation.Attributes.Add("disabled", "True")
                If BlanketValuationId <> "" Then SetdropDownFromValue(ddPPCValuation, BlanketValuationId)
                ddPPCDeductible.Attributes.Add("disabled", "True")
                If BlanketDeductibleId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddPPCDeductible, BlanketDeductibleId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
                End If
            Else
                trPPCBlanketAppliedInfoRow.Attributes.Add("style", "display:none")
                ddPPCCauseOfLoss.Attributes.Remove("disabled")
                ddPPCCoinsurance.Attributes.Remove("disabled")
                ddPPCValuation.Attributes.Remove("disabled")
                ddPPCDeductible.Attributes.Remove("disabled")
            End If
            ' Use Specific Rates
            DisplaySpecificRates(CPRBuildingCoverageType.PersonalPropertyCoverage)
            If MyBuilding.PersPropCov_CauseOfLossTypeId <> "" Then
                SetDropDownValue_ForceDiamondValue(ddPPCCoinsurance, MyBuilding.PersPropCov_CoinsuranceTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
            End If
            If MyBuilding.PersPropCov_CoinsuranceTypeId <> "" Then SetdropDownFromValue(ddPPCCoinsurance, MyBuilding.PersPropCov_CoinsuranceTypeId)
            If ValuationACVHelper.IsValuationACVAvailable(Quote) AndAlso QQHelper.IsQuickQuoteLocationNewToImage(MyLocation, Quote) Then
                ppcValuationUpdated = ValuationACVHelper.UpdateValuation(MyBuilding, ValuationACVHelper.PersonalPropertyCoverage)
            End If
            If MyBuilding.PersPropCov_ValuationId <> "" Then SetdropDownFromValue(ddPPCValuation, MyBuilding.PersPropCov_ValuationId)
            If ppcDeductibleToUse <> "" Then
                SetDropDownValue_ForceDiamondValue(ddPPCDeductible, ppcDeductibleToUse, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
            End If
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
                PopulateCoverageWindHail(LocationWindHailHelper.PersonalPropCov)
            End If
        End If

        ' Personal Property of Others Coverage
        If HasCoverage(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage) Then
            trPersonalPropertyOfOthersDataRow.Attributes.Add("style", "display:''")
            chkPersonalPropertyOfOthers.Checked = True
            txtPPOPersonalPropertyLimit.Text = MyBuilding.PersPropOfOthers_PersonalPropertyLimit
            If txtPPOPersonalPropertyLimit.Text.ToAlphaNumeric = LimitDummyValue Then
                txtPPOPersonalPropertyLimit.Text = String.Empty
            End If
            If MyBuilding.PersPropOfOthers_RiskTypeId <> "" Then SetdropDownFromValue(ddPPORiskType, MyBuilding.PersPropOfOthers_RiskTypeId)
            If HasBlanket Then
                If bldgQuote.HasBlanketContents OrElse bldgQuote.HasBlanketBuildingAndContents Then
                    chkPPOBlanketApplied.Enabled = True
                    chkPPOBlanketApplied.Checked = MyBuilding.PersPropOfOthers_IncludedInBlanketCoverage
                    trPPOBlanketRow.Attributes.Add("style", "display:''")
                End If
            Else
                chkPPOBlanketApplied.Enabled = False
                chkPPOBlanketApplied.Checked = False
            End If
            If chkPPOBlanketApplied.Checked Then
                trPPOBlanketAppliedInfoRow.Attributes.Add("style", "display:''")
                ddPPOCauseOfLoss.Attributes.Add("disabled", "True")
                If BlanketCauseOfLossId <> "" Then SetdropDownFromValue(ddPPOCauseOfLoss, BlanketCauseOfLossId)
                ddPPOCoinsurance.Attributes.Add("disabled", "True")
                If BlanketCoinsuranceId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddPPOCoinsurance, BlanketCoinsuranceId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
                End If
                ddPPOValuation.Attributes.Add("disabled", "True")
                If BlanketValuationId <> "" Then SetdropDownFromValue(ddPPOValuation, BlanketValuationId)
                ddPPODeductible.Attributes.Add("disabled", "True")
                If BlanketDeductibleId <> "" Then
                    SetDropDownValue_ForceDiamondValue(ddPPODeductible, BlanketDeductibleId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
                End If
            Else
                trPPOBlanketAppliedInfoRow.Attributes.Add("style", "display:none")
                ddPPOCauseOfLoss.Attributes.Remove("disabled")
                ddPPOCoinsurance.Attributes.Remove("disabled")
                ddPPOValuation.Attributes.Remove("disabled")
                ddPPODeductible.Attributes.Remove("disabled")
            End If

            ' Use Specific Rates
            DisplaySpecificRates(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage)

            If MyBuilding.PersPropOfOthers_CauseOfLossTypeId <> "" Then SetdropDownFromValue(ddPPOCauseOfLoss, MyBuilding.PersPropOfOthers_CauseOfLossTypeId)
            If MyBuilding.PersPropOfOthers_CoinsuranceTypeId <> "" Then
                SetDropDownValue_ForceDiamondValue(ddPPOCoinsurance, MyBuilding.PersPropOfOthers_CoinsuranceTypeId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.CoinsuranceTypeId)
            End If
            If ValuationACVHelper.IsValuationACVAvailable(Quote) AndAlso QQHelper.IsQuickQuoteLocationNewToImage(MyLocation, Quote) Then
                ppoValuationUpdated = ValuationACVHelper.UpdateValuation(MyBuilding, ValuationACVHelper.PersonalPropertyOfOthers)
            End If
            If MyBuilding.PersPropOfOthers_ValuationId <> "" Then SetdropDownFromValue(ddPPOValuation, MyBuilding.PersPropOfOthers_ValuationId)
            If ppoDeductibleToUse <> "" Then
                SetDropDownValue_ForceDiamondValue(ddPPODeductible, ppoDeductibleToUse, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
            End If
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
                PopulateCoverageWindHail(LocationWindHailHelper.PersonalPropOfOthers)
            End If
        End If

        If bcValuationUpdated OrElse ppcValuationUpdated OrElse ppoValuationUpdated Then
            ValuationACVHelper.ShowValuationRCPopupMessage(Me.Page)
        End If

        ' Set the Agreed Amount checkbox values for the building coverages
        DisplayAgreedAmountCoverageCheckboxValues()

        ' MINE SUBSIDENCE - Added 11/27/18 for multi state MLW
        PopulateMineSub(Quote.EffectiveDate)
        'If MyBuilding.HasMineSubsidence = True Then
        '    chkMineSubsidence.Checked = True
        '    If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
        '        If MyLocation.Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
        '            If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County) = Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory Then
        '                If txtINFClassCode.Text = "0311" OrElse txtINFClassCode.Text = "0331" Then
        '                    trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:''")
        '                    txtMineSubNumberOfUnits.Text = MyBuilding.NumberOfUnitsPerBuilding
        '                End If
        '            End If
        '        End If
        '    End If
        'End If

        ' Display the Earthquake stuff
        DisplayEarthquakeInfo()

        Me.PopulateChildControls()

        If FunctionalReplacementCostHelper.IsFunctionalReplacementCostAvailable(Quote) Then
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                If endorsementsPreexistHelper.IsPreexistingLocation(L) = False Then
                    RemoveFunctionalReplacementCost()
                End If
            Next
        End If

        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            If MyLocation IsNot Nothing Then
                If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                    HideOrShowBuildingDeductibleDropDowns()
                End If
            End If
        End If


        If OwnerOccupiedPercentageFieldHelper.IsOwnerOccupiedPercentageFieldAvailable(Quote) Then
            PopulateOwnerOccupiedPercentageField()
        Else
            trOwnerOccupiedPercentageRow.Attributes.Add("style", "display:none")
            MyBuilding.OwnerOccupiedPercentageId = "-1" ' None
        End If
    End Sub


    Private Sub DefaultBuildingWindHailCoverage()
        If MyLocation IsNot Nothing AndAlso MyBuilding IsNot Nothing Then
            Dim ownerOccupiedPercentageId As String = Me.ddOwnerOccupiedPercentage.SelectedValue

            If chkBCWindHail IsNot Nothing Then
                If IsNullEmptyorWhitespace(MyLocation.WindHailDeductibleLimitId) = False Then '0 = N/A
                    If Not WindHailDefaultingHelper.CheckCPRCPPExemptCodes(MyBuilding) Then
                        If ownerOccupiedPercentageId <> "" AndAlso (ownerOccupiedPercentageId = "30" OrElse ownerOccupiedPercentageId = "31") Then
                            chkBCWindHail.Checked = True
                            chkBCWindHail.Enabled = False
                            If MyLocation.WindHailDeductibleLimitId = "0" Then
                                MyLocation.WindHailDeductibleLimitId = "32"
                            End If
                            trBCWindHailText.Attributes.Add("style", "display:''")
                        ElseIf ownerOccupiedPercentageId <> "" AndAlso ownerOccupiedPercentageId = "32" Then
                            If MyLocation.WindHailDeductibleLimitId = "0" Then
                                chkBCWindHail.Checked = False
                                chkBCWindHail.Enabled = False
                                'Else
                                '    chkBCWindHail.Checked = False
                                '    chkBCWindHail.Enabled = True
                            End If
                            trBCWindHailText.Attributes.Add("style", "display:none")
                        Else
                            trBCWindHailText.Attributes.Add("style", "display:none")
                        End If
                    Else
                        If MyLocation.WindHailDeductibleLimitId = "0" Then
                            chkBCWindHail.Checked = False
                            chkBCWindHail.Enabled = False
                        Else
                            chkBCWindHail.Checked = False
                            chkBCWindHail.Enabled = True
                        End If
                        trBCWindHailText.Attributes.Add("style", "display:none")
                    End If
                Else
                    trBCWindHailText.Attributes.Add("style", "display:none")
                End If
            Else
                trBCWindHailText.Attributes.Add("style", "display:none")
            End If
        End If
    End Sub


    Private Sub PopulateCoverageWindHail(covName As String)
        If MyLocation IsNot Nothing AndAlso MyBuilding IsNot Nothing Then
            Dim trRow As HtmlTableRow = Nothing
            Dim chkWindHail As CheckBox = Nothing
            Dim covWindHailDeductibleId As String = String.Empty
            Select Case covName
                Case LocationWindHailHelper.BuildingCov
                    trRow = trBCWindHailRow
                    chkWindHail = chkBCWindHail
                    covWindHailDeductibleId = MyBuilding.OptionalWindstormOrHailDeductibleId
                Case LocationWindHailHelper.PersonalPropCov
                    trRow = trPPCWindHailRow
                    chkWindHail = chkPPCWindHail
                    covWindHailDeductibleId = MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId
                Case LocationWindHailHelper.PersonalPropOfOthers
                    trRow = trPPOWindHailRow
                    chkWindHail = chkPPOWindHail
                    covWindHailDeductibleId = MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId
            End Select
            If trRow IsNot Nothing AndAlso chkWindHail IsNot Nothing Then
                trRow.Attributes.Add("style", "display:''")
                If IsNullEmptyorWhitespace(MyLocation.WindHailDeductibleLimitId) OrElse MyLocation.WindHailDeductibleLimitId = "0" Then
                    '0 = N/A
                    chkWindHail.Checked = False
                    chkWindHail.Enabled = False
                Else
                    chkWindHail.Enabled = True
                    Select Case covWindHailDeductibleId
                        Case "32", "33", "34"
                            '32=1%, 33=2%, 34=5%
                            chkWindHail.Checked = True
                        Case Else
                            chkWindHail.Checked = False
                    End Select
                End If
            End If
        End If
    End Sub

    Public Sub HideOrShowBuildingDeductibleDropDowns()
        If Quote IsNot Nothing Then
            If MyLocation IsNot Nothing Then
                ' Hide deductible dropdowns
                trBCDeductibleRow.Attributes.Add("style", "display:none")
                trPPCDeductibleRow.Attributes.Add("style", "display:none")
                trPPODeductibleRow.Attributes.Add("style", "display:none")
            End If
        End If
    End Sub

    Public Sub RemoveFunctionalReplacementCost()
        Dim HasBlanket As Boolean = False

        If bldgQuote Is Nothing Then Exit Sub

        ' The specific rates rows are always shown UNLESS N/A was chosen for blanket coverage on the coverages page
        If bldgQuote.HasBlanketBuilding OrElse bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketContents Then
            HasBlanket = True
        End If

        If Quote IsNot Nothing Then
            ' Loop through all the buildings on the quote and get the last one
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If L.Buildings IsNot Nothing AndAlso L.Buildings.Count > 0 Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            If HasBlanket Then
                                If txtINFClassCode.Text = "0196" OrElse txtINFClassCode.Text = "0197" OrElse txtINFClassCode.Text = "0198" Then
                                    If FunctionalReplacementCostHelper.IsFunctionalReplacementCostAvailable(Quote) Then
                                        chkBCBlanketApplied.Checked = False
                                        chkBCBlanketApplied.Enabled = False
                                        chkPPOBlanketApplied.Checked = False
                                        chkPPOBlanketApplied.Enabled = False
                                        chkPPCBlanketApplied.Checked = False
                                        chkPPCBlanketApplied.Enabled = False
                                        PPOBlanketText.Visible = True
                                        BCBlanketText.Visible = True
                                        PPCBlanketText.Visible = True
                                        trBCBlanketInfoRow.Visible = False
                                        trPPCBlanketAppliedInfoRow.Visible = False
                                        trPPOBlanketAppliedInfoRow.Visible = False
                                    Else
                                        chkBCBlanketApplied.Enabled = True
                                        chkPPOBlanketApplied.Enabled = True
                                        chkPPCBlanketApplied.Enabled = True
                                        PPOBlanketText.Visible = False
                                        BCBlanketText.Visible = False
                                        PPCBlanketText.Visible = False
                                        trBCBlanketInfoRow.Visible = True
                                        trPPCBlanketAppliedInfoRow.Visible = True
                                        trPPOBlanketAppliedInfoRow.Visible = True
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End If
    End Sub

    Public Sub PopulateOwnerOccupiedPercentageField()
        If Quote IsNot Nothing Then
            ' Loop through all the buildings on the quote and get the last one
            If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If L.Buildings IsNot Nothing AndAlso L.Buildings.Count > 0 Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            If chkBuildingCoverage.Checked = True Then
                                trOwnerOccupiedPercentageRow.Attributes.Add("style", "display:''")
                            Else
                                trOwnerOccupiedPercentageRow.Attributes.Add("style", "display:none")
                                MyBuilding.OwnerOccupiedPercentageId = "-1" ' None
                            End If
                        Next
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub PopulateMineSub(ByVal EffDt As DateTime)
        '''''''''
        ' NOTE! The visibility of the mine sub checkbox, info, and data rows are controlled by script function 'checkMineSub' in VRCPR.JS
        '''''''''
        If MyLocation.Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            ' Ohio Mine sub 
            ' Before Ohio effective date - don't show number of units
            trMineSubsidenceLimitInfo.Attributes.Add("style", "display:none;")
            trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:none")
            txtMineSubNumberOfUnits.Text = ""
            ' Checking and disabling of the checkbox etc are handled in script, we're just setting the number of units row here.
            If EffDt >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
                Select Case IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County)
                    ' MANDATORY MINE SUB
                    Case Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory
                        ' Show number of units if class code is 0311 or 0331
                        If txtINFClassCode.Text = "0311" OrElse txtINFClassCode.Text = "0331" Then
                            trMineSubsidenceNumberOfUnitsRow.Attributes.Add("style", "display:''")
                            txtMineSubNumberOfUnits.Text = MyBuilding.NumberOfUnitsPerBuilding
                        End If
                        If MyBuilding.Limit IsNot Nothing AndAlso IsNumeric(MyBuilding.Limit) Then
                            If CDec(MyBuilding.Limit) >= 300000 Then
                                trMineSubsidenceLimitInfo.Attributes.Add("style", "display:'';")
                            End If
                        End If
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If
        Else
            ' All lines but Ohio - there's also script for these
            If MyBuilding.HasMineSubsidence = True Then
                chkMineSubsidence.Checked = True
            End If
        End If
        Exit Sub
    End Sub

    Private Function IsLastBuilding()
        Dim MaxLocIndex As Integer = 0
        Dim MaxBldgIndex As Integer = 0
        Dim LastBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding = Nothing

        If Quote IsNot Nothing Then
            ' Loop through all the buildings on the quote and get the last one
            If Quote.Locations IsNot Nothing Then
                For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                    If L.Buildings IsNot Nothing Then
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            LastBuilding = B
                        Next
                    End If
                Next
            End If
        End If

        If LastBuilding IsNot Nothing Then
            If MyBuilding.Equals(LastBuilding) Then Return True Else Return False
        Else
            Return False
        End If

    End Function

    Private Sub DisplayEarthquakeInfo()
        ' Hide the EQ & EQ Classification Lookup rows initially
        trBICEarthquakeRow.Attributes.Add("style", "display:none")
        trPPCEarthquakeRow.Attributes.Add("style", "display:none")
        trPPCEarthquakeLookupRow.Attributes.Add("style", "display:none")
        trPPOEarthquakeRow.Attributes.Add("style", "display:none")
        trPPOEarthquakeLookupRow.Attributes.Add("style", "display:none")

        ' Clear out the hidden field values - Do this on the last building because all the buildings need to be checked
        If IsLastBuilding() Then Session("PopulateAfterEQLookup") = Nothing

        ' set the checkboxes to unchecked
        chkBICEarthquake.Checked = False
        chkPPCEarthquake.Checked = False
        chkPPOEarthquake.Checked = False

        If chkINFEarthquake.Checked Then
            If chkBusinessIncomeCoverage.Checked Then trBICEarthquakeRow.Attributes.Add("style", "display:''")
            If chkPersonalPropertyCoverage.Checked Then
                trPPCEarthquakeRow.Attributes.Add("style", "display:''")
            End If
            If chkPersonalPropertyOfOthers.Checked Then
                trPPOEarthquakeRow.Attributes.Add("style", "display:''")
            End If

            ' Set the BC/BIC earthquake checkboxes
            If MyBuilding.BusinessIncomeCov_EarthquakeApplies Then chkBICEarthquake.Checked = True

            ' If Earthquake Classification has been set on either PPC or PPO display it in the PPC coverage (if visible) and hide it in the PPC coverage.
            If chkPersonalPropertyCoverage.Checked Then
                chkPPCEarthquake.Checked = MyBuilding.PersPropCov_EarthquakeApplies
                If chkPPCEarthquake.Checked Then trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
            End If

            If chkPersonalPropertyOfOthers.Checked Then
                chkPPOEarthquake.Checked = MyBuilding.PersPropOfOthers_EarthquakeApplies
                If chkPPOEarthquake.Checked Then trPPOEarthquakeLookupRow.Attributes.Add("style", "display:''")
            End If

            Dim id As String = ""
            If hdnDIA_PPC_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPC_EQCC_Id.Value <> "" Then
                id = hdnDIA_PPC_EQCC_Id.Value
            Else
                If hdnDIA_PPO_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPO_EQCC_Id.Value <> "" Then
                    id = hdnDIA_PPO_EQCC_Id.Value
                Else
                    If MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId IsNot Nothing AndAlso MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId <> "" AndAlso MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId <> "0" Then
                        id = MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId
                        hdnDIA_PPC_EQCC_Id.Value = MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId
                    End If
                End If
            End If

            If chkPPCEarthquake.Checked AndAlso chkPPOEarthquake.Checked Then
                ' Set the description text
                txtPPCEarthquakeClassification.Text = GetEQClassificationDescriptionText(id)
                trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
                trPPOEarthquakeLookupRow.Attributes.Add("style", "display:none")
            ElseIf chkPPCEarthquake.Checked Then
                ' Set the description text
                txtPPCEarthquakeClassification.Text = GetEQClassificationDescriptionText(id)
                trPPCEarthquakeLookupRow.Attributes.Add("style", "display:''")
            ElseIf chkPPOEarthquake.Checked Then
                ' Set the description text
                txtPPOEQClassification.Text = GetEQClassificationDescriptionText(id)
                trPPOEarthquakeLookupRow.Attributes.Add("style", "display:''")
            End If
        End If

        Exit Sub
    End Sub

    Private Sub DisplayAgreedAmountCoverageCheckboxValues()
        Dim HasAgreedAmount As Boolean = False
        Dim HasBlanket As Boolean = False

        If bldgQuote Is Nothing Then Exit Sub

        ' Set the HasBlanket flag
        If bldgQuote.HasBlanketBuilding OrElse bldgQuote.HasBlanketBuildingAndContents OrElse bldgQuote.HasBlanketContents Then HasBlanket = True

        ' Set the HasAgreedAmount flag
        If HasBlanket Then
            If bldgQuote.HasBlanketBuilding Then
                HasAgreedAmount = bldgQuote.BlanketBuildingIsAgreedValue
            ElseIf bldgQuote.HasBlanketBuildingAndContents Then
                HasAgreedAmount = bldgQuote.BlanketBuildingAndContentsIsAgreedValue
            ElseIf bldgQuote.HasBlanketContents Then
                HasAgreedAmount = bldgQuote.BlanketContentsIsAgreedValue
            End If
        End If

        ' Set the HasAgreedAmount flag
        ' Set the Agreed Amount checkboxes
        chkBCAgreedAmount.Checked = False
        chkBCAgreedAmount.Attributes.Remove("disabled")
        trBCAgreedAmountInfoRow.Attributes.Add("style", "display:none")
        hdn_Agreed_BC.Value = False

        chkPPCAgreedAmount.Checked = False
        chkPPCAgreedAmount.Attributes.Remove("disabled")
        trPPCAgreedAmountInfoRow.Attributes.Add("style", "display:none")
        hdn_Agreed_PPC.Value = False

        If HasAgreedAmount Then
            ' ** AGREED AMOUNT CHECKED ON COVERAGES PAGE
            ' BUILDING COVERAGE
            If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) Then
                ' Quote has building coverage
                If chkBCBlanketApplied.Checked Then
                    chkBCAgreedAmount.Checked = True
                    hdn_Agreed_BC.Value = True
                    ' Disable the checkbox when blanket is checked
                    chkBCAgreedAmount.Attributes.Add("disabled", "True")
                Else
                    chkBCAgreedAmount.Checked = MyBuilding.IsAgreedValue
                    hdn_Agreed_BC.Value = MyBuilding.IsAgreedValue
                End If
            End If

            ' PERSONAL PROPERTY COVERAGE
            If HasCoverage(CPRBuildingCoverageType.PersonalPropertyCoverage) Then
                ' Quote has personal property coverage
                If chkPPCBlanketApplied.Checked Then
                    chkPPCAgreedAmount.Checked = True
                    hdn_Agreed_PPC.Value = True
                    chkPPCAgreedAmount.Attributes.Add("disabled", "True")
                Else
                    chkPPCAgreedAmount.Checked = MyBuilding.PersPropCov_IsAgreedValue
                    hdn_Agreed_PPC.Value = MyBuilding.PersPropCov_IsAgreedValue
                End If
            End If
        Else
            ' Agreed Amount UNCHECKED on Coverages page
            If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) Then
                If chkBCBlanketApplied.Checked Then
                    chkBCAgreedAmount.Checked = False
                    hdn_Agreed_BC.Value = False
                    chkBCAgreedAmount.Attributes.Add("disabled", "True")
                Else
                    chkBCAgreedAmount.Checked = MyBuilding.IsAgreedValue
                    hdn_Agreed_BC.Value = MyBuilding.IsAgreedValue
                End If
            End If
            If HasCoverage(CPRBuildingCoverageType.PersonalPropertyCoverage) Then
                If chkPPCBlanketApplied.Checked Then
                    chkPPCAgreedAmount.Checked = False
                    hdn_Agreed_PPC.Value = False
                    chkPPCAgreedAmount.Attributes.Add("disabled", "True")
                Else
                    chkPPCAgreedAmount.Checked = MyBuilding.PersPropCov_IsAgreedValue
                    hdn_Agreed_PPC.Value = MyBuilding.PersPropCov_IsAgreedValue
                End If
            End If
        End If

        ' Always show the info rows if it's respective checkbox is checked
        If chkBCAgreedAmount.Checked Then trBCAgreedAmountInfoRow.Attributes.Add("style", "display:''")
        If chkPPCAgreedAmount.Checked Then trPPCAgreedAmountInfoRow.Attributes.Add("style", "display:''")

        Exit Sub
    End Sub

    Private Sub DisplaySpecificRates(ByVal CovType As CPRBuildingCoverageType)
        ' Always display the specific rates info on the first BC/BIC and PPC/PPO coverage if it's been selected
        ' Do not show it on the second coverage if it's already on the first
        ' Determine whether the Building Class Code is eligible for specific rates
        Dim ShowUseSpecific As Boolean = SpecificRatesEligible()
        If ShowUseSpecific Then
            Select Case CovType
                Case CPRBuildingCoverageType.BuildingCoverage
                    chkBCUseSpecificRates.Checked = True
                    BCSpecificUseChecked = True
                    If MyBuilding.Building_BusinessIncome_Group1_LossCost <> "0" Then txtBCGroupI.Text = MyBuilding.Building_BusinessIncome_Group1_LossCost
                    If MyBuilding.Building_BusinessIncome_Group2_LossCost <> "0" Then txtBCGroupII.Text = MyBuilding.Building_BusinessIncome_Group2_LossCost
                    trBCUseSpecificRatesRow.Attributes.Add("style", "display:''")
                    trBCUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                    trBCGroupIRow.Attributes.Add("style", "display:''")
                    trBCGroupIIRow.Attributes.Add("style", "display:''")
                    BCUseSpecificVisible = True
                    Exit Select
                Case CPRBuildingCoverageType.BusinessIncomeCoverage
                    chkBICUseSpecificRates.Checked = True
                    BICSpecificUseChecked = True
                    If txtBCGroupI.Text <> "" OrElse txtBCGroupII.Text <> "" Then
                        trBICUseSpecificRatesRow.Attributes.Add("style", "display:''")
                        trBICUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                        trBICGroupIRow.Attributes.Add("style", "display:none")
                        trBICGroupIIRow.Attributes.Add("style", "display:none")
                        BICUseSpecificVisible = False
                    Else
                        If MyBuilding.Building_BusinessIncome_Group1_LossCost <> "0" Then txtBICGroupI.Text = MyBuilding.Building_BusinessIncome_Group1_LossCost
                        If MyBuilding.Building_BusinessIncome_Group2_LossCost <> "0" Then txtBICGroupII.Text = MyBuilding.Building_BusinessIncome_Group2_LossCost
                        trBICUseSpecificRatesRow.Attributes.Add("style", "display:''")
                        trBICUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                        trBICGroupIRow.Attributes.Add("style", "display:''")
                        trBICGroupIIRow.Attributes.Add("style", "display:''")
                        BICUseSpecificVisible = True
                    End If
                    Exit Select
                Case CPRBuildingCoverageType.PersonalPropertyCoverage
                    chkPPCUseSpecificRates.Checked = True
                    PPCSpecificUseChecked = True
                    If MyBuilding.PersonalProperty_Group1_LossCost <> "0" Then txtPPCGroupI.Text = MyBuilding.PersonalProperty_Group1_LossCost
                    If MyBuilding.PersonalProperty_Group2_LossCost <> "0" Then txtPPCGroupII.Text = MyBuilding.PersonalProperty_Group2_LossCost
                    trPPCUseSpecificRatesRow.Attributes.Add("style", "display:''")
                    trPPCUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                    trPPCGroupIRow.Attributes.Add("style", "display:''")
                    trPPCGroupIIRow.Attributes.Add("style", "display:''")
                    PPCUseSpecificVisible = True
                    Exit Select
                Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                    chkPPOUseSpecificRates.Checked = True
                    PPOSpecificUseChecked = True
                    If chkPersonalPropertyCoverage.Checked AndAlso (txtPPCGroupI.Text <> "" OrElse txtPPCGroupII.Text <> "") Then
                        trPPOUseSpecificRatesRow.Attributes.Add("style", "display:''")
                        trPPOUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                        trPPOGroupIRow.Attributes.Add("style", "display:none")
                        trPPOGroupIIRow.Attributes.Add("style", "display:none")
                        PPOUseSpecificVisible = False
                    Else
                        If MyBuilding.PersonalProperty_Group1_LossCost <> "0" Then txtPPOGroupI.Text = MyBuilding.PersonalProperty_Group1_LossCost
                        If MyBuilding.PersonalProperty_Group2_LossCost <> "0" Then txtPPOGroupII.Text = MyBuilding.PersonalProperty_Group2_LossCost
                        trPPOUseSpecificRatesRow.Attributes.Add("style", "display:''")
                        trPPOUseSpecificRatesInfoRow.Attributes.Add("style", "display:''")
                        trPPOGroupIRow.Attributes.Add("style", "display:''")
                        trPPOGroupIIRow.Attributes.Add("style", "display:''")
                        PPOUseSpecificVisible = True
                    End If
                    Exit Select
            End Select
        Else
            ' If not eligible for specific rates, hide the specific rates rows
            If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) Then
                Me.VRScript.AddScriptLine($"$('#{trBCUseSpecificRatesRow.ClientID}').hide();")
                chkBCUseSpecificRates.Checked = False
                BCSpecificUseChecked = False
                trBCUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trBCUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trBCGroupIRow.Attributes.Add("style", "display:none")
                trBCGroupIIRow.Attributes.Add("style", "display:none")
                BCUseSpecificVisible = False
            End If

            If HasCoverage(CPRBuildingCoverageType.BusinessIncomeCoverage) Then
                chkBICUseSpecificRates.Checked = False
                BICSpecificUseChecked = False
                trBICUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trBICUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trBICGroupIRow.Attributes.Add("style", "display:none")
                trBICGroupIIRow.Attributes.Add("style", "display:none")
                BICUseSpecificVisible = False
            End If

            If HasCoverage(CPRBuildingCoverageType.PersonalPropertyCoverage) Then
                chkPPCUseSpecificRates.Checked = False
                PPCSpecificUseChecked = False
                hdn_PPC_UseSpecific_Checked.Value = "false"
                trPPCUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trPPCUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trPPCGroupIRow.Attributes.Add("style", "display:none")
                trPPCGroupIIRow.Attributes.Add("style", "display:none")
                PPCUseSpecificVisible = False
            End If

            If HasCoverage(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage) Then
                chkPPOUseSpecificRates.Checked = False
                PPOSpecificUseChecked = False
                trPPOUseSpecificRatesRow.Attributes.Add("style", "display:none")
                trPPOUseSpecificRatesInfoRow.Attributes.Add("style", "display:none")
                trPPOGroupIRow.Attributes.Add("style", "display:none")
                trPPOGroupIIRow.Attributes.Add("style", "display:none")
                PPOUseSpecificVisible = False
            End If
        End If
    End Sub

    Private Function HasCoverage(ByVal CovType As CPRBuildingCoverageType) As Boolean
        If MyBuilding Is Nothing Then Return False

        Select Case CovType
            Case CPRBuildingCoverageType.BuildingCoverage
                If chkBuildingCoverage.Checked Then Return True
                If MyBuilding.Limit <> "" AndAlso MyBuilding.Limit <> "0" Then Return True
                If MyBuilding.CauseOfLossType IsNot Nothing AndAlso MyBuilding.CauseOfLossType <> "" AndAlso MyBuilding.CauseOfLossType <> "N/A" Then Return True
                Exit Select
            Case CPRBuildingCoverageType.BusinessIncomeCoverage
                If chkBusinessIncomeCoverage.Checked Then Return True
                If MyBuilding.BusinessIncomeCov_Limit IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_Limit <> "" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "N/A" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0" Then Return True ' AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0"
                If MyBuilding.BusinessIncomeCov_RiskType IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_RiskType <> "" AndAlso MyBuilding.BusinessIncomeCov_RiskType <> "N/A" Then Return True
                If MyBuilding.BusinessIncomeCov_CauseOfLossType IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_CauseOfLossType <> "" AndAlso MyBuilding.BusinessIncomeCov_CauseOfLossType <> "N/A" Then Return True
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyCoverage
                If chkPersonalPropertyCoverage.Checked Then Return True
                If MyBuilding.PersPropCov_PersonalPropertyLimit <> "" AndAlso MyBuilding.PersPropCov_PersonalPropertyLimit <> "0" OrElse MyBuilding.PersPropCov_PropertyTypeId <> "" Then Return True
                Exit Select
            Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                If chkPersonalPropertyOfOthers.Checked Then Return True
                If MyBuilding.PersPropOfOthers_PersonalPropertyLimit <> "" AndAlso MyBuilding.PersPropOfOthers_PersonalPropertyLimit <> "0" Then Return True
                If MyBuilding.PersPropOfOthers_RiskType IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_RiskType <> "" AndAlso MyBuilding.PersPropOfOthers_RiskType <> "N/A" Then Return True
                If MyBuilding.PersPropOfOthers_CoinsuranceType IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_CoinsuranceType <> "" AndAlso MyBuilding.PersPropOfOthers_CoinsuranceType <> "N/A" Then Return True
                Exit Select
        End Select

        Return False
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack Then
            If Session("PopulateAfterEQLookup") <> Nothing Then
                Populate()
            End If
        End If
    End Sub

    Private Function GetWindHailDeducID(limit As String, hailPercentSelectedID As String, minHaildedcId As String, standardDedcId As String) As String
        '  3-27-2013
        Dim returnDedutibleId As String = "0"
        If String.IsNullOrWhiteSpace(limit) = False Then
            ' need to determine this based on limit,property deduc, % , and min deduc
            Dim minDeducPerDDValAsPercent As Double = 0.0
            Select Case hailPercentSelectedID
                Case "32"
                    minDeducPerDDValAsPercent = 0.01
                Case "33"
                    minDeducPerDDValAsPercent = 0.02
                Case "34"
                    minDeducPerDDValAsPercent = 0.05
                Case Else
                    minDeducPerDDValAsPercent = 0.0 'N/A
            End Select

            Dim minWindHailDeducAsVal As Int32 = 0
            Select Case minHaildedcId
                Case "0"
                    minWindHailDeducAsVal = 0 'N/A
                Case "4"
                    minWindHailDeducAsVal = 250
                Case "8"
                    minWindHailDeducAsVal = 500
                Case "9"
                    minWindHailDeducAsVal = 1000
                Case "15"
                    minWindHailDeducAsVal = 2500
                Case "16"
                    minWindHailDeducAsVal = 5000
                Case "17"
                    minWindHailDeducAsVal = 10000
                Case "19"
                    minWindHailDeducAsVal = 25000
                Case Else
                    minWindHailDeducAsVal = 0
            End Select

            Dim standardDeducVal As Int32 = 0
            Select Case standardDedcId
                Case "0"
                    standardDeducVal = 0
                Case "4"
                    standardDeducVal = 250
                Case "8"
                    standardDeducVal = 500
                Case "9"
                    standardDeducVal = 1000
                Case "15"
                    standardDeducVal = 2500
                Case "16"
                    standardDeducVal = 5000
                Case "17"
                    standardDeducVal = 10000
                Case "19"
                    standardDeducVal = 25000
                Case Else
                    standardDeducVal = 0
            End Select

            Dim minPercentDeductibleVal As Int32 = CInt(CInt(limit) * minDeducPerDDValAsPercent)
            'If Not (minWindHailDeducAsVal = 0 Or standardDeducVal = 0 Or minPercentDeductibleVal = 0) Then
            Dim pairs As New List(Of KeyValuePair(Of Int32, String))
            pairs.Add(New KeyValuePair(Of Int32, String)(minWindHailDeducAsVal, minHaildedcId))
            pairs.Add(New KeyValuePair(Of Int32, String)(minPercentDeductibleVal, hailPercentSelectedID))
            pairs.Add(New KeyValuePair(Of Int32, String)(standardDeducVal, standardDedcId))

            'which is bigger ?
            'then determine the id of the largest and select that as the wind/hail deduc 
            returnDedutibleId = (From pair As KeyValuePair(Of Int32, String) In pairs Order By pair.Key Descending Select pair.Value).FirstOrDefault()
            'End If

        End If

        Return returnDedutibleId
    End Function


    Public Overrides Function Save() As Boolean
        Dim EndoHelper As EndorsementsPreexistingHelper = New EndorsementsPreexistingHelper(Quote)
        Dim isPreExsiting As Boolean = EndoHelper.IsPreexistingLocation(LocationIndex)

        If isPreExsiting Then Exit Function

        Dim bcDeductibleToUse As String = ""
        Dim ppcDeductibleToUse As String = ""
        Dim ppoDeductibleToUse As String = ""

        If MyBuilding IsNot Nothing Then
            Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = MyBuilding.ClassificationCode
            cc.ClassCode = txtINFClassCode.Text
            cc.PMA = hdnPMAID.Value
            cc.ClassDescription = txtINFDescription.Text
            cc.RateGroup = hdnGroupRate.Value
            cc.ClassLimit = hdnClassLimit.Value
            MyBuilding.ClassificationCode = cc

            Dim devDictionaryCC = New DevDictionaryHelper.CppEndorsementClassCode(LocationIndex, BuildingIndex, txtINFClassCode.Text, txtINFDescription.Text, String.Empty, hdnPMAID.Value, hdnGroupRate.Value, hdnClassLimit.Value, String.Empty)
            ddh.UpdateDevDictionaryClassificationCodeList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, devDictionaryCC)


            'Added 8/13/2019 for Bug 29924 MLW
            If MyBuilding.ClassificationCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode IsNot Nothing Then
                Select Case MyBuilding.ClassificationCode.ClassCode.ToString()
                    Case "0331", "0332", "0333", "0341", "0342", "0343"
                        MyBuilding.CoverageFormTypeId = 2
                End Select
            End If
            MyBuilding.BusinessIncomeCov_ClassificationCode = cc
            MyBuilding.PersPropCov_ClassificationCode = cc
            MyBuilding.PersPropOfOthers_ClassificationCode = cc
            Dim BlanketNum As String = ""
            Dim BlanketText As String = ""
            Dim COLId As String = Nothing
            Dim COINSId As String = Nothing
            Dim VALId As String = Nothing
            Dim DEDId As String = Nothing
            Dim LOCWindHailDeductibleID As String = Nothing

            MyBuilding.ConstructionId = ddINFConstruction.SelectedValue

            ' If classification has been set on building 0 then copy it to the location
            If LocationIndex = 0 AndAlso BuildingIndex = 0 Then
                If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) Then
                    Quote.Locations(0).ClassificationCode = Quote.Locations(0).Buildings(0).ClassificationCode
                End If
            End If

            ' YARD RATE LOGIC
            ' Updated 10/21/20 for Bug 51134 - MGB
            ' The hidden yard rate id field may be cleared in certain cases on save, so set the values
            ' on the coverages whenever a yard rate value was set by the Classification lookup control.  
            ' Later on in the save code, in the personal property saves, we will remove the value if 
            ' the selected class code Is ineligble.  This also fixes the issue where the workflow sets the 
            ' yard Rate before the property coverages (to which yard rate applies) are selected.
            If hdnYardRateId.Value = "-1" OrElse hdnYardRateId.Value = "" Then
                ' this value can be cleared inadvertantly by saves, so we don't clear the 
                ' coverage yard rate id's here, we do it below in the property coverage save sections.
                hdnYardRateId.Value = "0"
            Else
                ' A yard rate value was set by the class code lookup control. Store it to session.
                YardRateId = hdnYardRateId.Value
            End If

            ' Earthquake Classification
            If chkINFEarthquake.Checked Then
                MyBuilding.EarthquakeBuildingClassificationTypeId = ddEarthquakeClassification.SelectedValue
                MyBuilding.EarthquakeApplies = True
                If IsNewCo() Then
                    MyBuilding.EarthquakeDeductibleId = ddEarthquakeDeductible.SelectedValue
                Else
                    MyBuilding.EarthquakeDeductibleId = ""
                End If
            Else
                MyBuilding.EarthquakeBuildingClassificationTypeId = ""
                MyBuilding.EarthquakeApplies = False
                MyBuilding.EarthquakeDeductibleId = ""
            End If

            MyLocation.WindstormOrHailMinimumDollarDeductibleId = "0" ' NA

            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                If Quote.Locations.IsLoaded() Then
                    Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                    If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                        bcDeductibleToUse = MyLocation.DeductibleId
                        ppcDeductibleToUse = MyLocation.DeductibleId
                        ppoDeductibleToUse = MyLocation.DeductibleId
                    Else
                        bcDeductibleToUse = MyBuilding.DeductibleId
                        ppcDeductibleToUse = MyBuilding.PersPropCov_DeductibleId
                        ppoDeductibleToUse = MyBuilding.PersPropOfOthers_DeductibleId
                    End If
                End If
            Else
                bcDeductibleToUse = ddBCDeductible.SelectedValue
                ppcDeductibleToUse = ddPPCDeductible.SelectedValue
                ppoDeductibleToUse = ddPPODeductible.SelectedValue
            End If


            ' *** COVERAGES ***
            ' BUILDING COVERAGE
            If chkBuildingCoverage.Checked Then

                'Owner Occupied Percentage
                If OwnerOccupiedPercentageFieldHelper.IsOwnerOccupiedPercentageFieldAvailable(Quote) Then
                    MyBuilding.OwnerOccupiedPercentageId = ddOwnerOccupiedPercentage.SelectedValue
                Else
                    MyBuilding.OwnerOccupiedPercentageId = "-1" ' None
                End If
                ' Limit
                MyBuilding.Limit = txtBCBuildingLimit.Text
                If MyBuilding.Limit.IsNullEmptyorWhitespace Then
                    MyBuilding.Limit = LimitDummyValue
                End If
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

                If IsNewCo() AndAlso chkINFEarthquake.Checked Then
                    MyBuilding.BuildingCov_EarthquakeDeductibleId = ddEarthquakeDeductible.SelectedValue
                Else
                    MyBuilding.BuildingCov_EarthquakeDeductibleId = ""
                End If

                ' BLANKET DEDUCTIBLE FIELDS
                If QuoteHasBlanket(BlanketNum, BlanketText) Then
                    If chkBCBlanketApplied.Checked Then
                        If GetPolicyBlanketValues(COLId, COINSId, VALId, DEDId) Then
                            ' Included in blanket and blanket applied - use the fields on building zero
                            MyBuilding.CauseOfLossTypeId = COLId
                            ' Co-Insurance
                            MyBuilding.CoinsuranceTypeId = COINSId
                            ' Valuation
                            MyBuilding.ValuationId = VALId
                            ' Deductible
                            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                                MyBuilding.DeductibleId = bcDeductibleToUse
                                MyBuilding.OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkBCWindHail, bcDeductibleToUse)
                                MyBuilding.OptionalTheftDeductibleId = bcDeductibleToUse
                            Else
                                MyBuilding.DeductibleId = DEDId
                                MyBuilding.OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkBCWindHail, DEDId)
                                MyBuilding.OptionalTheftDeductibleId = DEDId
                            End If
                        End If
                    Else
                        ' Blanket applied not checked - save the values on the page
                        MyBuilding.CauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                        ' Co-Insurance
                        MyBuilding.CoinsuranceTypeId = ddBCCoInsurance.SelectedValue
                        ' Valuation
                        MyBuilding.ValuationId = ddBCValuation.SelectedValue
                        ' Deductible 
                        MyBuilding.DeductibleId = bcDeductibleToUse
                        MyBuilding.OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkBCWindHail, bcDeductibleToUse)
                        MyBuilding.OptionalTheftDeductibleId = bcDeductibleToUse
                    End If
                Else
                    ' Deductble post 07/15/2025
                    MyBuilding.DeductibleId = bcDeductibleToUse
                    MyBuilding.OptionalTheftDeductibleId = bcDeductibleToUse
                    ' NOT included in blanket, save the dropdown values
                    MyBuilding.CauseOfLossTypeId = ddBCCauseOfLoss.SelectedValue
                    ' Co-Insurance
                    MyBuilding.CoinsuranceTypeId = ddBCCoInsurance.SelectedValue
                    ' Valuation
                    MyBuilding.ValuationId = ddBCValuation.SelectedValue
                    MyBuilding.OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkBCWindHail, bcDeductibleToUse)
                End If

                ' Agreed Amount
                MyBuilding.IsAgreedValue = hdn_Agreed_BC.Value

                ' Mine Subsidence - Added 11/8/18 for multi state MLW
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                    Select Case Quote.Locations(LocationIndex).Address.QuickQuoteState
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                            ' Illinois
                            ' Always set the mine subsidence to whatever the checkbox is set to
                            MyBuilding.HasMineSubsidence = chkMineSubsidence.Checked
                            If MyBuilding.ClassificationCode.ClassCode = "0196" OrElse MyBuilding.ClassificationCode.ClassCode = "0197" OrElse MyBuilding.ClassificationCode.ClassCode = "0198" Then
                                MyBuilding.MineSubsidence_IsDwellingStructure = True
                            End If
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                            ' Indiana
                            'Only set the mine subsidence for eligible counties, otherwise turn it off
                            If IFM.VR.Common.Helpers.MineSubsidenceHelper.MineSubCountiesByStateAbbreviation("IN").Contains(Quote.Locations(LocationIndex).Address.County) Then
                                MyBuilding.HasMineSubsidence = chkMineSubsidence.Checked
                                If MyBuilding.ClassificationCode.ClassCode = "0196" OrElse MyBuilding.ClassificationCode.ClassCode = "0197" OrElse MyBuilding.ClassificationCode.ClassCode = "0198" Then
                                    MyBuilding.MineSubsidence_IsDwellingStructure = True
                                End If
                            Else
                                MyBuilding.HasMineSubsidence = False
                            End If
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                            If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County) = Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory Then
                                ' Must have mine sub
                                MyBuilding.HasMineSubsidence = True
                                If txtINFClassCode.Text = "0311" OrElse txtINFClassCode.Text = "0331" Then
                                    MyBuilding.NumberOfUnitsPerBuilding = txtMineSubNumberOfUnits.Text
                                Else
                                    MyBuilding.NumberOfUnitsPerBuilding = ""
                                End If
                            Else
                                ' Cannot have mine sub
                                MyBuilding.HasMineSubsidence = False
                                MyBuilding.NumberOfUnitsPerBuilding = ""
                            End If
                    End Select
                End If
            Else
                MyBuilding.ClassificationCode = New QuickQuoteClassificationCode
                MyBuilding.Limit = ""
                MyBuilding.InflationGuardTypeId = ""
                MyBuilding.IsBuildingValIncludedInBlanketRating = False
                ClearSpecificRateValues(CPRBuildingCoverageType.BuildingCoverage)
                MyBuilding.CauseOfLossTypeId = ""
                MyBuilding.CoinsuranceTypeId = ""
                MyBuilding.ValuationId = ""
                'MyBuilding.DeductibleId = ""
                'If Not (LocationIndex = 0 And BuildingIndex = 0) Then MyBuilding.IsAgreedValue = False  ' Don't save the agreed amount on building 0
                MyBuilding.EarthquakeApplies = False
                MyBuilding.BuildingCov_EarthquakeDeductibleId = ""
                MyBuilding.OptionalTheftDeductibleId = ""
                MyBuilding.OptionalWindstormOrHailDeductibleId = ""
                If LocationIndex = 0 And BuildingIndex = 0 Then
                    'Updated 12/20/18 for multi state bug 30442 MLW
                    If SubQuoteFirst IsNot Nothing Then
                        'If (Not Quote.HasBlanketBuilding) AndAlso (Not Quote.HasBlanketBuildingAndContents) Then
                        If (Not SubQuoteFirst.HasBlanketBuilding) AndAlso (Not SubQuoteFirst.HasBlanketBuildingAndContents) Then
                            ' Only reset the Deductible and Agreed Amount on building 0 if the quote does NOT have blanket type of building or combined
                            MyBuilding.DeductibleId = ""
                            MyBuilding.IsAgreedValue = False
                        End If
                    End If
                Else
                    MyBuilding.DeductibleId = ""
                    MyBuilding.IsAgreedValue = False
                End If
                'Added 11/29/18 for multi state MLW
                MyBuilding.HasMineSubsidence = False
            End If

            ' BUSINESS INCOME COVERAGE
            If chkBusinessIncomeCoverage.Checked Then
                ' Waiting Period Type
                ' Check if Coverage exists before first save.
                Dim isBusinessIncomeNew As Boolean = True
                If MyBuilding.BusinessIncomeCov_Limit IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_Limit <> "" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "N/A" AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0" Then isBusinessIncomeNew = False ' AndAlso MyBuilding.BusinessIncomeCov_Limit <> "0"
                If MyBuilding.BusinessIncomeCov_RiskType IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_RiskType <> "" AndAlso MyBuilding.BusinessIncomeCov_RiskType <> "N/A" Then isBusinessIncomeNew = False
                If MyBuilding.BusinessIncomeCov_CauseOfLossType IsNot Nothing AndAlso MyBuilding.BusinessIncomeCov_CauseOfLossType <> "" AndAlso MyBuilding.BusinessIncomeCov_CauseOfLossType <> "N/A" Then isBusinessIncomeNew = False

                If Not SubQuoteFirst.HasBusinessIncomeALS Then
                    ' Does NOT have Business Income ALS
                    ' Building Income Limit
                    MyBuilding.BusinessIncomeCov_Limit = txtBICBusinessIncomeLimit.Text
                    If MyBuilding.BusinessIncomeCov_Limit.IsNullEmptyorWhitespace Then
                        MyBuilding.BusinessIncomeCov_Limit = LimitDummyValue
                    End If

                    ' Limit Type
                    If rbM.Checked Then
                        MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ddBICLimitTypeM.SelectedValue
                        MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ""
                    End If
                    If rbC.Checked Then
                        MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ""
                        MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ddBICLimitTypeC.SelectedValue
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
                    If chkINFEarthquake.Checked Then
                        MyBuilding.BusinessIncomeCov_EarthquakeApplies = chkBICEarthquake.Checked
                    Else
                        MyBuilding.BusinessIncomeCov_EarthquakeApplies = False
                    End If

                    ' 04/11/2022 CAH - Don't overwrite Diamond value if present
                    ' Check if Coverage exists before first save.
                    If isBusinessIncomeNew Then
                        MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = "3" ' 3 = 72  
                    End If

                Else
                    ' HAS Business Income ALS
                    SetSpecificRateValues(CPRBuildingCoverageType.BusinessIncomeCoverage)
                    MyBuilding.BusinessIncomeCov_Limit = "1"
                    MyBuilding.BusinessIncomeCov_CoinsuranceTypeId = ""
                    MyBuilding.BusinessIncomeCov_MonthlyPeriodTypeId = ""
                    'If Quote.EffectiveDate < CDate("3/1/2020") Then
                    '    ' NOTE: When the effective date is on or after 3/1/2020 this value will be set on the coverages page dropdown MGB 3/2/2020 Task 41357
                    '    '       Prior to 3/1/2020 the value is always set to 72 hours
                    '    MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = "3"  ' 3 = 72  
                    'End If

                    ' 04/11/2022 CAH - Code differs from New Business for Endorsements.
                    If Quote.EffectiveDate < CDate("3/1/2020") Then
                        ' NOTE: When the effective date is on or after 3/1/2020 this value will be set on the coverages page dropdown MGB 3/2/2020 Task 41357

                        '       Prior to 3/1/2020 the value is always set to 72 hours
                        MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = "3"  ' 3 = 72  
                    Else
                        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(0) AndAlso Me.Quote.Locations.GetItemAtIndex(0).Buildings.HasItemAtIndex(0) Then
                            Dim alsWait = Me.Quote.Locations.GetItemAtIndex(0).Buildings.GetItemAtIndex(0).BusinessIncomeCov_WaitingPeriodTypeId
                            MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = alsWait
                        Else
                            If isBusinessIncomeNew Then
                                MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = "3" ' 3 = 72  
                            End If
                        End If
                    End If

                    MyBuilding.BusinessIncomeCov_BusinessIncomeTypeId = ddBICIncomeType.SelectedValue
                    MyBuilding.BusinessIncomeCov_RiskTypeId = ddBICRiskType.SelectedValue
                    MyBuilding.BusinessIncomeCov_CauseOfLossTypeId = ddBICCauseOfLoss.SelectedValue
                    If chkINFEarthquake.Checked Then
                        MyBuilding.BusinessIncomeCov_EarthquakeApplies = chkBICEarthquake.Checked
                    Else
                        MyBuilding.BusinessIncomeCov_EarthquakeApplies = False
                    End If
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
                'If Not Quote.HasBusinessIncomeALS Then MyBuilding.BusinessIncomeCov_WaitingPeriodTypeId = ""
                MyBuilding.BusinessIncomeCov_IncludedInBlanketCoverage = False
            End If

            SetPersonalPropertyEarthquakeValues()

            ' PERSONAL PROPERTY COVERAGE
            If chkPersonalPropertyCoverage.Checked Then
                ' Personal Property Limit
                MyBuilding.PersPropCov_PersonalPropertyLimit = txtPPCPersonalPropertyLimit.Text
                If MyBuilding.PersPropCov_PersonalPropertyLimit.IsNullEmptyorWhitespace Then
                    MyBuilding.PersPropCov_PersonalPropertyLimit = LimitDummyValue
                End If
                ' Property Type
                MyBuilding.PersPropCov_PropertyTypeId = ddPPCPropertyType.SelectedValue
                ' Risk Type
                MyBuilding.PersPropCov_RiskTypeId = ddPPCRiskType.SelectedValue
                ' Blanket Applied
                MyBuilding.PersPropCov_IncludedInBlanketCoverage = chkPPCBlanketApplied.Checked
                ' Use Specific Rates
                SetSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyCoverage)
                ' Yard rate - apply to coverage if the yard rate id has been set and the class code is one of the yard rate eligible class codes
                If YardRateId <> "" AndAlso IFM.VR.Common.Helpers.CPR.CPRBuildingClassCodeHelper.BuildingClassCodeIsYardRateEligible(MyBuilding.ClassificationCode.ClassCode) Then
                    MyBuilding.PersPropCov_DoesYardRateApplyTypeId = YardRateId
                Else
                    ' Not eligible for yard rate, clear value
                    MyBuilding.PersPropCov_DoesYardRateApplyTypeId = ""
                End If

                ' BLANKET DEDUCTIBLE FIELDS
                If QuoteHasBlanket(BlanketNum, BlanketText) Then
                    If chkPPCBlanketApplied.Checked Then
                        If GetPolicyBlanketValues(COLId, COINSId, VALId, DEDId) Then
                            ' Included in blanket and blanket applied - use the fields on building zero
                            MyBuilding.PersPropCov_CauseOfLossTypeId = COLId
                            ' Co-Insurance
                            MyBuilding.PersPropCov_CoinsuranceTypeId = COINSId
                            ' Valuation
                            MyBuilding.PersPropCov_ValuationId = VALId
                            ' Deductible
                            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                                MyBuilding.PersPropCov_DeductibleId = ppcDeductibleToUse
                                MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPCWindHail, ppcDeductibleToUse)
                                MyBuilding.PersPropCov_OptionalTheftDeductibleId = ppcDeductibleToUse
                            Else
                                MyBuilding.PersPropCov_DeductibleId = DEDId
                                MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPCWindHail, DEDId)
                                MyBuilding.PersPropCov_OptionalTheftDeductibleId = DEDId
                            End If
                        End If
                    Else
                        ' Blanket applied not checked - save the values on the page
                        MyBuilding.PersPropCov_CauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                        ' Co-Insurance
                        MyBuilding.PersPropCov_CoinsuranceTypeId = ddPPCCoinsurance.SelectedValue
                        ' Valuation
                        MyBuilding.PersPropCov_ValuationId = ddPPCValuation.SelectedValue
                        ' Deductibles
                        MyBuilding.PersPropCov_DeductibleId = ppcDeductibleToUse
                        MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPCWindHail, ppcDeductibleToUse)
                        MyBuilding.PersPropCov_OptionalTheftDeductibleId = ppcDeductibleToUse
                    End If
                Else
                    ' Deductble post 07/15/2025
                    MyBuilding.PersPropCov_DeductibleId = ppcDeductibleToUse
                    MyBuilding.PersPropCov_OptionalTheftDeductibleId = ppcDeductibleToUse
                    ' NOT included in blanket, save the dropdown values
                    MyBuilding.PersPropCov_CauseOfLossTypeId = ddPPCCauseOfLoss.SelectedValue
                    ' Co-Insurance
                    MyBuilding.PersPropCov_CoinsuranceTypeId = ddPPCCoinsurance.SelectedValue
                    ' Valuation
                    MyBuilding.PersPropCov_ValuationId = ddPPCValuation.SelectedValue
                    MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPCWindHail, ppcDeductibleToUse)
                End If

                ' Agreed Amount
                MyBuilding.PersPropCov_IsAgreedValue = hdn_Agreed_PPC.Value
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
                MyBuilding.PersPropCov_DoesYardRateApplyTypeId = ""
                MyBuilding.PersPropCov_OptionalTheftDeductibleId = ""
                MyBuilding.PersPropCov_OptionalWindstormOrHailDeductibleId = ""
                If LocationIndex = 0 And BuildingIndex = 0 Then
                    'Updated 12/20/18 for multi state bug 30442 MLW
                    If SubQuoteFirst IsNot Nothing Then
                        'If Not Quote.HasBlanketContents Then
                        If Not SubQuoteFirst.HasBlanketContents Then
                            ' Only reset the Deductible and Agreed Amount on building 0 if the quote does NOT have blanket type of personal property only
                            MyBuilding.PersPropCov_DeductibleId = ""
                            MyBuilding.PersPropCov_IsAgreedValue = False
                        End If
                    End If
                Else
                    MyBuilding.PersPropCov_DeductibleId = ""
                    MyBuilding.PersPropCov_IsAgreedValue = False
                End If
            End If

            ' PERSONAL PROPERTY OF OTHERS
            If chkPersonalPropertyOfOthers.Checked Then
                ' Personal Property Limit
                MyBuilding.PersPropOfOthers_PersonalPropertyLimit = txtPPOPersonalPropertyLimit.Text
                If MyBuilding.PersPropOfOthers_PersonalPropertyLimit.IsNullEmptyorWhitespace Then
                    MyBuilding.PersPropOfOthers_PersonalPropertyLimit = LimitDummyValue
                End If
                ' Risk Type
                MyBuilding.PersPropOfOthers_RiskTypeId = ddPPORiskType.SelectedValue
                ' Blanket Applied
                MyBuilding.PersPropOfOthers_IncludedInBlanketCoverage = chkPPOBlanketApplied.Checked
                ' Use Specific Rates
                SetSpecificRateValues(CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage)
                ' Yard rate - apply to coverage if the yard rate id has been set and the class code is one of the yard rate eligible class codes
                If YardRateId <> "" AndAlso IFM.VR.Common.Helpers.CPR.CPRBuildingClassCodeHelper.BuildingClassCodeIsYardRateEligible(MyBuilding.ClassificationCode.ClassCode) Then
                    MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = YardRateId
                Else
                    ' Not eligible for yard rate, clear value
                    MyBuilding.PersPropOfOthers_DoesYardRateApplyTypeId = ""
                End If

                ' BLANKET DEDUCTIBLE FIELDS
                If QuoteHasBlanket(BlanketNum, BlanketText) Then
                    If chkPPOBlanketApplied.Checked Then
                        If GetPolicyBlanketValues(COLId, COINSId, VALId, DEDId) Then
                            ' Included in blanket and blanket applied - use the fields on building zero
                            MyBuilding.PersPropOfOthers_CauseOfLossTypeId = COLId
                            ' Co-Insurance
                            MyBuilding.PersPropOfOthers_CoinsuranceTypeId = COINSId
                            ' Valuation
                            MyBuilding.PersPropOfOthers_ValuationId = VALId
                            ' Deductibles
                            If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                                MyBuilding.PersPropOfOthers_DeductibleId = ppoDeductibleToUse
                                MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPOWindHail, ppoDeductibleToUse)
                                MyBuilding.PersPropOfOthers_OptionalTheftDeductibleId = ppoDeductibleToUse
                            Else
                                MyBuilding.PersPropOfOthers_DeductibleId = DEDId
                                MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPOWindHail, DEDId)
                                MyBuilding.PersPropOfOthers_OptionalTheftDeductibleId = DEDId
                            End If
                        End If
                    Else
                        ' Blanket applied not checked - save the values on the page
                        MyBuilding.PersPropOfOthers_CauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                        ' Co-Insurance
                        MyBuilding.PersPropOfOthers_CoinsuranceTypeId = ddPPOCoinsurance.SelectedValue
                        ' Valuation
                        MyBuilding.PersPropOfOthers_ValuationId = ddPPOValuation.SelectedValue
                        ' Deductibles
                        MyBuilding.PersPropOfOthers_DeductibleId = ppoDeductibleToUse
                        MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPOWindHail, ppoDeductibleToUse)
                        MyBuilding.PersPropOfOthers_OptionalTheftDeductibleId = ppoDeductibleToUse
                    End If
                Else
                    ' Deductible 
                    MyBuilding.PersPropOfOthers_DeductibleId = ppoDeductibleToUse
                    MyBuilding.PersPropOfOthers_OptionalTheftDeductibleId = ppoDeductibleToUse
                End If

                ' NOT included in blanket, save the dropdown values
                MyBuilding.PersPropOfOthers_CauseOfLossTypeId = ddPPOCauseOfLoss.SelectedValue
                ' Co-Insurance
                MyBuilding.PersPropOfOthers_CoinsuranceTypeId = ddPPOCoinsurance.SelectedValue
                ' Valuation
                MyBuilding.PersPropOfOthers_ValuationId = ddPPOValuation.SelectedValue
                MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = SetBuildingWindHailDeductible(LOCWindHailDeductibleID, chkPPOWindHail, ppoDeductibleToUse)
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
                MyBuilding.PersPropOfOthers_OptionalTheftDeductibleId = ""
                MyBuilding.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = ""
            End If
        End If

        'Populate()
        Me.SaveChildControls()
        Populate()
        'Me.SaveChildControls()
        Return True
    End Function

    Private Function SetBuildingWindHailDeductible(ByRef LOCWindHailDeductibleID As String, chkWindHail As CheckBox, deductibleToUse As String) As String
        Dim covWindHailDeductibleId As String = String.Empty
        If HasLocationWindHailDeductible(LOCWindHailDeductibleID) Then
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso chkWindHail.Checked = False Then
                covWindHailDeductibleId = deductibleToUse
            Else
                covWindHailDeductibleId = LOCWindHailDeductibleID
            End If
        Else
            covWindHailDeductibleId = deductibleToUse
        End If
        Return covWindHailDeductibleId
    End Function

    ' (OLD PRE-MULISTATE CODE)
    'Private Function HasLocationWindHailDeductible(ByRef WindHailDedID As String) As Boolean
    '    If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) Then
    '        If Quote.Locations(0).WindHailDeductibleLimitId = "222" OrElse Quote.Locations(0).WindHailDeductibleLimitId = "223" OrElse Quote.Locations(0).WindHailDeductibleLimitId = "224" Then
    '            WindHailDedID = Quote.Locations(0).WindHailDeductibleLimitId
    '            Return True
    '        End If
    '    End If
    '    WindHailDedID = Nothing
    '    Return False
    'End Function

    ''' <summary>
    ''' Returns true if the location wind/hail deductible has been set
    ''' If it HAS been set, will return the location wind/hail deductible id in the passed ByRef parameter
    ''' If it has NOT been set then the returned deductible will be nothing
    ''' </summary>
    ''' <param name="WindHailDedID"></param>
    ''' <returns></returns>
    Private Function HasLocationWindHailDeductible(ByRef WindHailDedID As String) As Boolean
        If MyLocation IsNot Nothing Then
            If MyLocation.WindHailDeductibleLimitId = "32" OrElse MyLocation.WindHailDeductibleLimitId = "33" OrElse MyLocation.WindHailDeductibleLimitId = "34" Then
                WindHailDedID = MyLocation.WindHailDeductibleLimitId
                Return True
            End If
        End If
        WindHailDedID = Nothing
        Return False
    End Function

    ''' <summary>
    ''' Return the Cause of Loss, Coinsurance, Valuation and Deductible values based on the blanket coverage selected
    ''' </summary>
    ''' <param name="CauseOfLossId"></param>
    ''' <param name="CoinsuranceId"></param>
    ''' <param name="ValuationId"></param>
    ''' <param name="DeductibleId"></param>
    ''' <returns></returns>
    Private Function GetPolicyBlanketValues(ByRef CauseOfLossId As String, ByRef CoinsuranceId As String, ByRef ValuationId As String, ByRef DeductibleId As String) As Boolean
        CauseOfLossId = Nothing
        CoinsuranceId = Nothing
        ValuationId = Nothing
        DeductibleId = Nothing

        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) AndAlso SubQuoteFirst IsNot Nothing Then
            'Updated 12/20/18 for multi state bug 30442 MLW
            'If Quote.HasBlanketBuildingAndContents Then
            '    CauseOfLossId = Quote.BlanketBuildingAndContentsCauseOfLossTypeId
            '    CoinsuranceId = Quote.BlanketBuildingAndContentsCoinsuranceTypeId
            '    ValuationId = Quote.BlanketBuildingAndContentsValuationId
            '    DeductibleId = Quote.Locations(0).Buildings(0).DeductibleId
            '    Return True
            'ElseIf Quote.HasBlanketBuilding Then
            '    CauseOfLossId = Quote.BlanketBuildingCauseOfLossTypeId
            '    CoinsuranceId = Quote.BlanketBuildingCoinsuranceTypeId
            '    ValuationId = Quote.BlanketBuildingValuationId
            '    DeductibleId = Quote.Locations(0).Buildings(0).DeductibleId
            '    Return True
            'ElseIf Quote.HasBlanketContents Then
            '    CauseOfLossId = Quote.BlanketContentsCauseOfLossTypeId
            '    CoinsuranceId = Quote.BlanketContentsCoinsuranceTypeId
            '    ValuationId = Quote.BlanketContentsValuationId
            '    DeductibleId = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
            '    Return True
            'End If
            If SubQuoteFirst.HasBlanketBuildingAndContents Then
                CauseOfLossId = SubQuoteFirst.BlanketBuildingAndContentsCauseOfLossTypeId
                CoinsuranceId = SubQuoteFirst.BlanketBuildingAndContentsCoinsuranceTypeId
                ValuationId = SubQuoteFirst.BlanketBuildingAndContentsValuationId
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    DeductibleId = Quote.Locations(0).DeductibleId
                Else
                    DeductibleId = Quote.Locations(0).Buildings(0).DeductibleId
                End If
                Return True
            ElseIf SubQuoteFirst.HasBlanketBuilding Then
                CauseOfLossId = SubQuoteFirst.BlanketBuildingCauseOfLossTypeId
                CoinsuranceId = SubQuoteFirst.BlanketBuildingCoinsuranceTypeId
                ValuationId = SubQuoteFirst.BlanketBuildingValuationId
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    DeductibleId = Quote.Locations(0).DeductibleId
                Else
                    DeductibleId = Quote.Locations(0).Buildings(0).DeductibleId
                End If
                Return True
            ElseIf SubQuoteFirst.HasBlanketContents Then
                CauseOfLossId = SubQuoteFirst.BlanketContentsCauseOfLossTypeId
                CoinsuranceId = SubQuoteFirst.BlanketContentsCoinsuranceTypeId
                ValuationId = SubQuoteFirst.BlanketContentsValuationId
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    DeductibleId = Quote.Locations(0).DeductibleId
                Else
                    DeductibleId = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
                End If
                Return True
            End If
        End If

        Return False
    End Function

    Private Sub SetPersonalPropertyEarthquakeValues()
        If chkINFEarthquake.Checked Then
            MyBuilding.PersPropCov_EarthquakeApplies = False
            MyBuilding.PersPropOfOthers_EarthquakeApplies = False
            MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""

            If chkPersonalPropertyCoverage.Checked AndAlso chkPersonalPropertyOfOthers.Checked Then
                ' PPC and PPO
                If chkPPCEarthquake.Checked Then MyBuilding.PersPropCov_EarthquakeApplies = True
                If chkPPOEarthquake.Checked Then MyBuilding.PersPropOfOthers_EarthquakeApplies = True
                If chkPPCEarthquake.Checked OrElse chkPPOEarthquake.Checked Then SetPPCPPOEarthquakeClassificationValue()
            ElseIf chkPersonalPropertyCoverage.Checked Then
                ' PPC Only
                If chkPPCEarthquake.Checked Then
                    MyBuilding.PersPropCov_EarthquakeApplies = True
                    SetPPCPPOEarthquakeClassificationValue()
                End If
            ElseIf chkPersonalPropertyOfOthers.Checked Then
                ' PPO Only
                MyBuilding.PersPropOfOthers_EarthquakeApplies = True
                SetPPCPPOEarthquakeClassificationValue()
            Else
                ' Neither PPC or PPO is checked!
            End If

            If IsNewCo() Then
                If chkPPCEarthquake.Checked Then
                    MyBuilding.PersPropCov_EarthquakeDeductibleId = ddEarthquakeDeductible.SelectedValue
                Else
                    MyBuilding.PersPropCov_EarthquakeDeductibleId = ""
                End If
                If chkPPOEarthquake.Checked Then
                    MyBuilding.PersPropOfOthers_EarthquakeDeductibleId = ddEarthquakeDeductible.SelectedValue
                Else
                    MyBuilding.PersPropOfOthers_EarthquakeDeductibleId = ""
                End If
            Else
                MyBuilding.PersPropCov_EarthquakeDeductibleId = ""
                MyBuilding.PersPropOfOthers_EarthquakeDeductibleId = ""
            End If
        Else
            ' Earthquake is not checked on the building info section so we can't have EQ on PPC/PPO
            MyBuilding.PersPropCov_EarthquakeApplies = False
            MyBuilding.PersPropOfOthers_EarthquakeApplies = False
            MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
            MyBuilding.PersPropCov_EarthquakeDeductibleId = ""
            MyBuilding.PersPropOfOthers_EarthquakeDeductibleId = ""
        End If
    End Sub

    ''' <summary>
    ''' Gets the description for a Earthquake Classification
    ''' </summary>
    ''' <param name="Id"></param>
    ''' <returns></returns>
    Private Function GetEQClassificationDescriptionText(ByVal Id As String) As String
        'Dim conn As New System.Data.SqlClient.SqlConnection
        'Dim cmd As New System.Data.SqlClient.SqlCommand
        'Dim rtn As Object = Nothing
        Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()


        Try
            Dim spm As New IFM.VR.Common.Helpers.SPManager("connDiamondReports", "usp_Get_PersonalPropertyGradeType")
            spm.AddIntegerParamater("@Id", qqh.IntegerForString(Id))
            Dim tbl As DataTable = spm.ExecuteSPQuery()
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                Return tbl.Rows(0)("dscr").ToString()
            Else
                Return Id
            End If

            'conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings("connDiamond")
            'conn.Open()

            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT dscr FROM PersonalPropertyRateGradeType WHERE PersonalPropertyRateGradeType_id = " & Id

            'rtn = cmd.ExecuteScalar()
            'If rtn IsNot Nothing Then Return rtn.ToString() Else Return Id
        Catch ex As Exception
            Return Id
        Finally
            'If conn.State = ConnectionState.Open Then conn.Close()
            'conn.Dispose()
            'cmd.Dispose()
        End Try
    End Function

    Private Sub SetPPCPPOEarthquakeClassificationValue()
        ' NOTE THAT THE PERSONAL PROPERTY AND PERSONAL PROPERTY OF OTHERS COVERAGES BOTH USE THE SAME FIELD TO STORE THEIR VALUES

        ' Always use the first value if it exists
        MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = ""
        If hdnDIA_PPC_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPC_EQCC_Id.Value <> "" Then
            MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = hdnDIA_PPC_EQCC_Id.Value
        Else
            If hdnDIA_PPO_EQCC_Id.Value IsNot Nothing AndAlso hdnDIA_PPO_EQCC_Id.Value <> "" Then
                MyBuilding.PersonalProperty_EarthquakeRateGradeTypeId = hdnDIA_PPO_EQCC_Id.Value
            End If
        End If
    End Sub

    Private Sub SetSpecificRateValues(ByVal CovType As CPRBuildingCoverageType)
        If MyBuilding IsNot Nothing Then
            Select Case CovType
                Case CPRBuildingCoverageType.BuildingCoverage
                    'If chkBCUseSpecificRates.Checked Then
                    If BCSpecificUseChecked Then
                        MyBuilding.RatingTypeId = "2"
                        If txtBCGroupI.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBCGroupI.Text
                        ElseIf txtBICGroupI.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBICGroupI.Text
                        Else
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
                        End If
                        If txtBCGroupII.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBCGroupII.Text
                        ElseIf txtBICGroupII.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBICGroupII.Text
                        Else
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
                        End If
                    Else
                        MyBuilding.RatingTypeId = "1"
                        If MyBuilding.BusinessIncomeCov_RatingTypeId <> "2" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
                        End If
                    End If
                    Exit Select
                Case CPRBuildingCoverageType.BusinessIncomeCoverage
                    'If BICUseSpecificVisible Then
                    If BICSpecificUseChecked Then
                        'If chkBICUseSpecificRates.Checked Then
                        MyBuilding.BusinessIncomeCov_RatingTypeId = "2"
                        If txtBICGroupI.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBICGroupI.Text
                        ElseIf txtBCGroupI.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = txtBCGroupI.Text
                        Else
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
                        End If
                        If txtBICGroupII.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBICGroupII.Text
                        ElseIf txtBCGroupII.Text <> "" Then
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = txtBCGroupII.Text
                        Else
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
                        End If
                    Else
                        MyBuilding.BusinessIncomeCov_RatingTypeId = "1"
                        If MyBuilding.RatingTypeId <> "2" Then
                            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
                            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
                        End If
                    End If
                    Exit Select
                Case CPRBuildingCoverageType.PersonalPropertyCoverage
                    'If PPCUseSpecificVisible Then
                    If PPCSpecificUseChecked Then
                        MyBuilding.PersPropCov_RatingTypeId = "2"
                        If txtPPCGroupI.Text <> "" Then
                            MyBuilding.PersonalProperty_Group1_LossCost = txtPPCGroupI.Text
                        ElseIf txtPPOGroupI.Text <> "" Then
                            MyBuilding.PersonalProperty_Group1_LossCost = txtPPOGroupI.Text
                        Else
                            MyBuilding.PersonalProperty_Group1_LossCost = ""
                        End If
                        If txtPPCGroupII.Text <> "" Then
                            MyBuilding.PersonalProperty_Group2_LossCost = txtPPCGroupII.Text
                        ElseIf txtPPOGroupII.Text <> "" Then
                            MyBuilding.PersonalProperty_Group2_LossCost = txtPPOGroupII.Text
                        Else
                            MyBuilding.PersonalProperty_Group2_LossCost = ""
                        End If
                    Else
                        MyBuilding.PersPropCov_RatingTypeId = "1"
                        If MyBuilding.PersPropOfOthers_RatingTypeId <> "2" Then
                            MyBuilding.PersonalProperty_Group1_LossCost = ""
                            MyBuilding.PersonalProperty_Group2_LossCost = ""
                        End If
                    End If
                    Exit Select
                Case CPRBuildingCoverageType.PersonalPropertyOfOthersCoverage
                    'If PPOUseSpecificVisible Then
                    If PPOSpecificUseChecked Then
                        MyBuilding.PersPropOfOthers_RatingTypeId = "2"
                        If txtPPOGroupI.Text <> "" Then
                            MyBuilding.PersonalProperty_Group1_LossCost = txtPPOGroupI.Text
                        ElseIf txtPPCGroupI.Text <> "" Then
                            MyBuilding.PersonalProperty_Group1_LossCost = txtPPCGroupI.Text
                        Else
                            MyBuilding.PersonalProperty_Group1_LossCost = ""
                        End If
                        If txtPPOGroupII.Text <> "" Then
                            MyBuilding.PersonalProperty_Group2_LossCost = txtPPOGroupII.Text
                        ElseIf txtPPCGroupII.Text <> "" Then
                            MyBuilding.PersonalProperty_Group2_LossCost = txtPPCGroupII.Text
                        Else
                            MyBuilding.PersonalProperty_Group2_LossCost = ""
                        End If
                    Else
                        MyBuilding.PersPropOfOthers_RatingTypeId = "1"
                        MyBuilding.PersonalProperty_Group1_LossCost = txtPPOGroupI.Text
                        MyBuilding.PersonalProperty_Group2_LossCost = txtPPOGroupII.Text
                    End If
                    Exit Select
            End Select
        End If
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

        If (Not BCSpecificUseChecked) AndAlso (Not BICSpecificUseChecked) Then
            MyBuilding.Building_BusinessIncome_Group1_LossCost = ""
            MyBuilding.Building_BusinessIncome_Group2_LossCost = ""
        End If

        If (Not PPCSpecificUseChecked) AndAlso (Not PPOSpecificUseChecked) Then
            MyBuilding.PersonalProperty_Group1_LossCost = ""
            MyBuilding.PersonalProperty_Group2_LossCost = ""
        End If

        Exit Sub
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim EndoHelper As EndorsementsPreexistingHelper = New EndorsementsPreexistingHelper(Quote)
        Dim isPreExsiting As Boolean = EndoHelper.IsPreexistingLocation(LocationIndex)

        If isPreExsiting Then Exit Sub

        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        '***  INFO SECTION ***
        Me.ValidationHelper.GroupName = " Location " & LocationIndex + 1.ToString & ", Building " & BuildingIndex + 1.ToString
        ' Class Code
        If txtINFClassCode.Text = "" Then
            Me.ValidationHelper.AddError(txtINFClassCode, "Missing Class Code", accordList)
        End If
        ' Construction
        If ddINFConstruction.SelectedIndex <= 0 Then
            Me.ValidationHelper.AddError(ddINFConstruction, "Missing Construction", accordList)
        End If

        ' *****************
        ' *** COVERAGES ***
        ' *****************
        ' BC/BIC GROUP I/II
        'BC & BIC USE THE SAME SET OF FIELDS ON THE OBJECT
        If BCUseSpecificVisible AndAlso BICUseSpecificVisible Then
            ' BOTH BC/BIC selected
            ' One or the other of Group I must be filled
            If txtBCGroupI.Text = "" AndAlso txtBICGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtBCGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBCGroupI.Text) Then
                    Dim val As Decimal = CDec(txtBCGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBCGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBCGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            ' One or the other of Group II must be filled
            If txtBCGroupII.Text = "" AndAlso txtBICGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtBCGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBCGroupII.Text) Then
                    Dim val As Decimal = CDec(txtBCGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBCGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBCGroupII, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
        ElseIf BCUseSpecificVisible Then
            ' BC ONLY - Validate BC Group fields
            If txtBCGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtBCGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBCGroupI.Text) Then
                    Dim val As Decimal = CDec(txtBCGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBCGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBCGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            If txtBCGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtBCGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBCGroupII.Text) Then
                    Dim val As Decimal = CDec(txtBCGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBCGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBCGroupII, "Invalid Value.  Use .001 to 10", accordList)
                End If
            End If
        ElseIf BICUseSpecificVisible Then
            ' BIC ONLY - Validate BIC Group Fields
            If txtBICGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtBICGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBICGroupI.Text) Then
                    Dim val As Decimal = CDec(txtBICGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBICGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBICGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            If txtBICGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtBICGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtBICGroupII.Text) Then
                    Dim val As Decimal = CDec(txtBICGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtBICGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtBICGroupII, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
        End If

        ' BUILDING COVERAGE
        If chkBuildingCoverage.Checked Then
            If txtBCBuildingLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtBCBuildingLimit, "Missing Building Limit", accordList)
            ElseIf txtBCBuildingLimit.Text.TryToGetInt32 <= ddBCDeductible.SelectedItem.Text.TryToGetInt32 Then
                Me.ValidationHelper.AddError(txtBCBuildingLimit, "Must have Building Limit > Deductible", accordList)
            End If

            'Owner Occupied Percentage
            If OwnerOccupiedPercentageFieldHelper.IsOwnerOccupiedPercentageFieldAvailable(Quote) Then
                If ddOwnerOccupiedPercentage.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError(ddOwnerOccupiedPercentage, "Missing Owner Occupied Percentage", accordList)
                End If
            End If

            ' Mine sub - Ohio
            If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                If MyLocation.Address.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    If IFM.VR.Common.Helpers.MineSubsidenceHelper.GetOhioMineSubsidenceTypeByCounty(MyLocation.Address.County) = Common.Helpers.MineSubsidenceHelper.OhioMineSubsidenceType_enum.EligibleMandatory Then
                        If txtBCBuildingLimit.Text.IsNumeric AndAlso CDec(txtBCBuildingLimit.Text) > 300000 Then
                            ' TODO: CPR OHIO When building limit is greater than $300,000, set the mine sub limit to $300,000
                            'MyBuilding.??? = "300000"
                        End If
                        If txtINFClassCode.Text = "0311" OrElse txtINFClassCode.Text = "0331" Then
                            ' Number of units is required
                            If txtMineSubNumberOfUnits.Text.Trim = "" Then
                                Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Number of Units per building required for Class Code 0311, 0331.", accordList)
                            ElseIf (Not txtMineSubNumberOfUnits.Text.IsNumeric()) Then
                                Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Total number of units is invalid.", accordList)
                            ElseIf CInt(txtMineSubNumberOfUnits.Text) <= 0 OrElse CInt(txtMineSubNumberOfUnits.Text) > 4 Then
                                Me.ValidationHelper.AddError(txtMineSubNumberOfUnits, "Total number of units is invalid, must be a value between 1 and 4.", accordList)
                            End If
                        End If
                    End If
                End If
            End If
        End If

        ' BUSINESS INCOME COVERAGE
        If chkBusinessIncomeCoverage.Checked Then
            If Not bldgQuote.HasBusinessIncomeALS Then
                ' These fields are not displayed when the quote has Business Income ALS
                If txtBICBusinessIncomeLimit.Text = "" Then
                    Me.ValidationHelper.AddError(txtBICBusinessIncomeLimit, "Missing Business Income Limit", accordList)
                ElseIf txtBICBusinessIncomeLimit.Text.TryToGetInt32 = 0 Then
                    Me.ValidationHelper.AddError(txtBICBusinessIncomeLimit, "Must have Business Income Limit > 0", accordList)
                End If
                If rbM.Checked Then
                    If ddBICLimitTypeM.SelectedIndex <= 0 Then
                        Me.ValidationHelper.AddError(ddBICLimitTypeM, "Missing Limit Type", accordList)
                    End If
                ElseIf rbC.Checked Then
                    If ddBICLimitTypeC.SelectedIndex <= 0 Then
                        Me.ValidationHelper.AddError(ddBICLimitTypeM, "Missing Limit Type", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError("Missing Limit Type")
                End If
            End If
            If ddBICRiskType.SelectedValue = "" OrElse ddBICRiskType.SelectedValue = "0" Then
                Me.ValidationHelper.AddError(ddBICRiskType, "Missing Risk Type", accordList)
            End If
        End If

        ' PPC/PPO GROUP I/II
        ' PPC & PPO USE THE SAME SET OF FIELDS ON THE OBJECT
        If PPCUseSpecificVisible AndAlso PPOUseSpecificVisible Then
            ' BOTH PPC/PPO selected
            ' One or the other of Group I must be filled
            If txtPPCGroupI.Text = "" AndAlso txtPPOGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPCGroupI.Text) Then
                    Dim val As Decimal = CDec(txtPPCGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPCGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPCGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            ' One or the other of Group II must be filled
            If txtPPCGroupII.Text = "" AndAlso txtPPOGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPCGroupII.Text) Then
                    Dim val As Decimal = CDec(txtPPCGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPCGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPCGroupII, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
        ElseIf PPCUseSpecificVisible Then
            ' PPC ONLY - Validate PPC Group fields
            If txtPPCGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPCGroupI.Text) Then
                    Dim val As Decimal = CDec(txtPPCGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPCGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPCGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            If txtPPCGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPCGroupII.Text) Then
                    Dim val As Decimal = CDec(txtPPCGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPCGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPCGroupII, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
        ElseIf PPOUseSpecificVisible Then
            ' PPO ONLY - Validate PPO Group Fields
            If txtPPOGroupI.Text = "" Then
                Me.ValidationHelper.AddError(txtPPOGroupI, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPOGroupI.Text) Then
                    Dim val As Decimal = CDec(txtPPOGroupI.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPOGroupI, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPOGroupI, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
            If txtPPOGroupII.Text = "" Then
                Me.ValidationHelper.AddError(txtPPOGroupII, "Missing Specific Rates", accordList)
            Else
                If IsNumeric(txtPPOGroupII.Text) Then
                    Dim val As Decimal = CDec(txtPPOGroupII.Text)
                    If val < 0.001 OrElse val > 10 Then
                        Me.ValidationHelper.AddError(txtPPOGroupII, "Invalid Value, must be .001 to 10", accordList)
                    End If
                Else
                    Me.ValidationHelper.AddError(txtPPOGroupII, "Invalid Value, must be .001 to 10", accordList)
                End If
            End If
        End If

        ' VALIDATE PERSONAL PROPERTY EARTHQUAKE CLASSIFICATIONS
        If (chkPersonalPropertyCoverage.Checked AndAlso chkPPCEarthquake.Checked) Or (chkPersonalPropertyOfOthers.Checked AndAlso chkPPOEarthquake.Checked) Then
            ' One or the other of the EQ classifications must have a value
            If txtPPCEarthquakeClassification.Text = "" AndAlso txtPPOEQClassification.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCEarthquakeClassification, "Missing Earthquake Classification", accordList)
            End If
        End If

        ' PERSONAL PROPERTY COVERAGE
        If chkPersonalPropertyCoverage.Checked Then
            If txtPPCPersonalPropertyLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtPPCPersonalPropertyLimit, "Missing Personal Property Limit", accordList)
            ElseIf txtPPCPersonalPropertyLimit.Text.TryToGetInt32 <= ddPPCDeductible.SelectedItem.Text.TryToGetInt32 Then
                Me.ValidationHelper.AddError(txtPPCPersonalPropertyLimit, "Must have Personal Property Limit > Deductible", accordList)
            End If
        End If

        ' PERSONAL PROPERTY OF OTHERS
        If chkPersonalPropertyOfOthers.Checked Then
            If txtPPOPersonalPropertyLimit.Text = "" Then
                Me.ValidationHelper.AddError(txtPPOPersonalPropertyLimit, "Missing Personal Property of Others Limit", accordList)
            ElseIf txtPPOPersonalPropertyLimit.Text.TryToGetInt32 <= ddPPODeductible.SelectedItem.Text.TryToGetInt32 Then
                Me.ValidationHelper.AddError(txtPPOPersonalPropertyLimit, "Must have Personal Property Of Others Limit > Deductible", accordList)
            End If
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    ''' <summary>
    ''' Checks if the building class code is eligible for specific rates.
    ''' Also checks the construction value.  MGB 8-14-18
    ''' Returns True if so, False if not
    ''' </summary>
    ''' <returns></returns>
    Private Function SpecificRatesEligible() As Boolean
        Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = Nothing
        If MyBuilding.ClassificationCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.ClassificationCode.ClassCode <> "" Then cc = MyBuilding.ClassificationCode
        If MyBuilding.PersPropCov_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropCov_ClassificationCode.ClassCode <> "" Then cc = MyBuilding.PersPropCov_ClassificationCode
        If MyBuilding.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode IsNot Nothing AndAlso MyBuilding.PersPropOfOthers_ClassificationCode.ClassCode <> "" Then cc = MyBuilding.PersPropOfOthers_ClassificationCode

        ' 8-14-2018 Bug 28331
        ' If Building Construction Type is one of the following...
        '   - MASONRY-NON COMBUSTIBLE   (id 14)
        '   - MODIFIED FIRE RESISTIVE   (id 15)
        '   - FIRE RESISTIVE   (id 16)
        ' ...then we need to show specific rates regardless of what the class code is.
        If ddINFConstruction.SelectedValue = "14" OrElse ddINFConstruction.SelectedValue = "15" OrElse ddINFConstruction.SelectedValue = "16" Then Return True

        ' Load the list with all of the class codes that don't use specific rates
        Dim classcodes As New List(Of String)
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
        classcodes.Add("0331")
        classcodes.Add("0745")
        classcodes.Add("0746")
        classcodes.Add("0747")
        classcodes.Add("0511")
        classcodes.Add("0512")
        classcodes.Add("0520")
        classcodes.Add("0531")
        classcodes.Add("0532")
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
        classcodes.Add("0755")
        classcodes.Add("0756")
        classcodes.Add("0757")
        classcodes.Add("0831")
        classcodes.Add("0832")
        classcodes.Add("0833")
        classcodes.Add("0834")
        classcodes.Add("0841")
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

        ' If the building class code is one in the list above then uncheck the checkboxes and hide all of the Use Specific Rates rows
        For Each c As String In classcodes
            If cc IsNot Nothing Then
                If cc.ClassCode = c Then
                    Return False
                End If
            Else
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>
    ''' When the Agreed Amount changes on the coverage screen, we need to reflect that on any existing coverages
    ''' </summary>
    Public Sub HandleAgreedAmountChange(ByVal newvalue As Boolean)
        If HasCoverage(CPRBuildingCoverageType.BuildingCoverage) AndAlso chkBCBlanketApplied.Checked Then
            If newvalue Then
                chkBCAgreedAmount.Checked = True
                chkBCAgreedAmount.Attributes.Add("disabled", "true")
            Else
                chkBCAgreedAmount.Checked = False
                chkBCAgreedAmount.Attributes.Remove("disabled")
            End If
        Else
            chkBCAgreedAmount.Checked = False
            chkBCAgreedAmount.Attributes.Remove("disabled")
        End If

        If HasCoverage(CPRBuildingCoverageType.PersonalPropertyCoverage) AndAlso chkPPCBlanketApplied.Checked Then
            If newvalue Then
                chkPPCAgreedAmount.Checked = True
                chkPPCAgreedAmount.Attributes.Add("disabled", "true")
            Else
                chkPPCAgreedAmount.Checked = False
                chkPPCAgreedAmount.Attributes.Remove("disabled")
            End If
        Else
            chkPPCAgreedAmount.Checked = False
            chkPPCAgreedAmount.Attributes.Remove("disabled")
        End If

        'UpdateBuildingCoverageBindings()

        Exit Sub
    End Sub

    ''' <summary>
    ''' When the blanket deductible changes on the coverages page we need to change it on Location 0 Building 0 if that building has 
    ''' Building Coverage selected with Blanket checked
    ''' </summary>
    Public Sub HandleBlanketDeductibleChange()
        Dim HasBlanket As Boolean = False
        Dim BlanketId As String = Nothing
        Dim BlanketText As String = Nothing

        HasBlanket = QuoteHasBlanket(BlanketId, BlanketText)

        Dim hideDeductible As Boolean = False
        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            hideDeductible = True
        End If

        ' Need to update the bindings to pass the new blanket deductible value to the script
        VRScript.CreateJSBinding(chkBCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('BC','" & Me.chkBCBlanketApplied.ClientID & "','" & Me.chkBCAgreedAmount.ClientID & "','" & trBCBlanketInfoRow.ClientID & "','" & Me.ddBCCauseOfLoss.ClientID & "','" & Me.ddBCCoInsurance.ClientID & "','" & Me.ddBCValuation.ClientID & "','" & Me.ddBCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & DefaultBCValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & hdn_Agreed_BC.ClientID & "','" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")
        VRScript.CreateJSBinding(chkPPCBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('PPC','" & Me.chkPPCBlanketApplied.ClientID & "','" & Me.chkPPCAgreedAmount.ClientID & "','" & trPPCBlanketAppliedInfoRow.ClientID & "','" & Me.ddPPCCauseOfLoss.ClientID & "','" & Me.ddPPCCoinsurance.ClientID & "','" & Me.ddPPCValuation.ClientID & "','" & Me.ddPPCDeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & DefaultPPCValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & hdn_Agreed_PPC.ClientID & "', '" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")
        VRScript.CreateJSBinding(chkPPOBlanketApplied, ctlPageStartupScript.JsEventType.onclick, "Cpr.BlanketCheckboxChanged('PPO','" & Me.chkPPOBlanketApplied.ClientID & "','','" & trPPOBlanketAppliedInfoRow.ClientID & "','" & Me.ddPPOCauseOfLoss.ClientID & "','" & Me.ddPPOCoinsurance.ClientID & "','" & Me.ddPPOValuation.ClientID & "','" & Me.ddPPODeductible.ClientID & "','" & Me.BlanketCauseOfLossId & "','" & Me.BlanketCoinsuranceId & "','" & Me.BlanketValuationId & "','" & Me.BlanketDeductibleId & "','" & Me.DefaultPPOValues.AgreedAmountValue & "','" & BlanketText & "','" & IsBuildingZero & "','" & LocationPropertyDeductibleClientID & "','" & hideDeductible & "');")

        ' Only update the blanket deductible if this is location 0 building 0 AND blanket is applied
        If LocationIndex = 0 AndAlso BuildingIndex = 0 Then
            'Updated 12/20/18 for multi state bug 30442 MLW
            'If Quote.HasBlanketBuildingAndContents OrElse Quote.HasBlanketBuilding Then
            '    If chkBuildingCoverage.Checked Then
            '        Me.ddBCDeductible.SelectedValue = Quote.Locations(0).Buildings(0).DeductibleId
            '    End If
            'ElseIf Quote.HasBlanketContents Then
            '    If chkPersonalPropertyCoverage.Checked Then
            '        Me.ddPPCDeductible.SelectedValue = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
            '    End If
            'End If
            If SubQuoteFirst IsNot Nothing Then
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    If SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketBuilding Then
                        If chkBuildingCoverage.Checked Then
                            SetDropDownValue_ForceDiamondValue(ddBCDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        End If
                    ElseIf SubQuoteFirst.HasBlanketContents Then
                        If chkPersonalPropertyCoverage.Checked Then
                            SetDropDownValue_ForceDiamondValue(ddPPCDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.PersPropCov_DeductibleId)
                        End If
                    End If
                Else
                    If SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketBuilding Then
                        If chkBuildingCoverage.Checked Then
                            SetDropDownValue_ForceDiamondValue(ddBCDeductible, Quote.Locations(0).Buildings(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        End If
                    ElseIf SubQuoteFirst.HasBlanketContents Then
                        If chkPersonalPropertyCoverage.Checked Then
                            SetDropDownValue_ForceDiamondValue(ddPPCDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub SetDefaultBuildingCoverageValues()
        Dim defCOLId As String = "3"    ' Special form including theft
        Dim defCOINSId As String = "5"  ' 80%
        Dim defVALId As String = "1"    ' Replacement cost
        Dim defDEDId As String = "9"     ' Was 8 (500) changed to 9 (1000) per task 62836 
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
            Dim defbldg As QuickQuote.CommonObjects.QuickQuoteBuilding = MyLocation.Buildings(0)

            ' Set the HasAgreedAmount flag
            ' The Blanket Agreed Amount value is at the policy level on the quote
            If bldgQuote.HasBlanketBuilding Then
                HasAgreedAmount = bldgQuote.BlanketBuildingIsAgreedValue
            ElseIf bldgQuote.HasBlanketBuildingAndContents Then
                HasAgreedAmount = bldgQuote.BlanketBuildingAndContentsIsAgreedValue
            ElseIf bldgQuote.HasBlanketContents Then
                HasAgreedAmount = bldgQuote.BlanketContentsIsAgreedValue
            End If

            ' Location 0 Building 0 IsAgreedAmount holds the default value for all the buildings
            'If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= 1 AndAlso Quote.Locations(0).Buildings IsNot Nothing AndAlso Quote.Locations(0).Buildings.Count >= 1 Then
            '    If Quote.HasBlanketBuildingAndContents OrElse Quote.HasBlanketBuilding Then
            '        HasAgreedAmount = Quote.Locations(0).Buildings(0).IsAgreedValue
            '    ElseIf Quote.HasBlanketContents Then
            '        HasAgreedAmount = Quote.Locations(0).Buildings(0).PersPropCov_IsAgreedValue
            '    End If
            'End If

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

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        If BuildingIndex = 0 AndAlso LocationIndex = 0 Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "A1", "alert('A building is required on the first location.  All values for building 1 have been cleared.');", True)
            lnkClearBuildingInfo_Click(Me, New EventArgs())
            chkBuildingCoverage.Checked = False
            chkBusinessIncomeCoverage.Checked = False
            chkPersonalPropertyCoverage.Checked = False
            chkPersonalPropertyOfOthers.Checked = False
            Save_FireSaveEvent(False)
            Populate()
        Else
            ddh.DoesCCExist(Me.LocationIndex, Me.BuildingIndex, True)
            RaiseEvent DeleteBuildingRequested(LocationIndex, BuildingIndex)
        End If

        Exit Sub
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        RaiseEvent NewBuildingRequested(LocationIndex)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, lnkSaveBuildingInfo.Click, lnkSaveBuildingCoverages.Click, btnSave.Click
        Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnClassCodeLookup.Click
        Me.ctl_CPR_ENDO_ClassCodeLookup.Show()
    End Sub

    Private Sub btnPPCLookupEQClassification_Click(sender As Object, e As EventArgs) Handles btnPPCLookupEQClassification.Click
        Save_FireSaveEvent(False)
        Me.ctl_CPR_ENDO_PPC_EQCCLookup.Show("PPC")
    End Sub

    Private Sub btnPPOLookupEQClassCode_Click(sender As Object, e As EventArgs) Handles btnPPOLookupEQClassCode.Click
        Save_FireSaveEvent(False)
        Me.ctl_CPR_ENDO_PPO_EQCCLookup.Show("PPO")
    End Sub

    Private Sub lnkClearBuildingInfo_Click(sender As Object, e As EventArgs) Handles lnkClearBuildingInfo.Click
        txtINFClassCode.Text = ""
        txtINFDescription.Text = ""
        If ddINFConstruction.Items IsNot Nothing AndAlso ddINFConstruction.Items.Count > 0 Then ddINFConstruction.SelectedIndex = 0
        chkINFEarthquake.Checked = False
        trEarthquakeClassificationRow.Attributes.Add("Style", "Display:none")
        If ddEarthquakeClassification.Items IsNot Nothing AndAlso ddEarthquakeClassification.Items.Count > 0 Then ddEarthquakeClassification.SelectedIndex = 0
        If IsNewCo() Then
            trEarthquakeDeductibleRow.Attributes.Add("style", "display:none")
        End If
        ddh.DoesCCExist(Me.LocationIndex, Me.BuildingIndex, True)
        Dim endorsementSaveError As String = ""
        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
    End Sub

    Private Sub lnkClearBuildingCoverages_Click(sender As Object, e As EventArgs) Handles lnkClearBuildingCoverages.Click
        Me.chkBuildingCoverage.Checked = False
        'Added 11/29/18 for multi state MLW
        Me.chkMineSubsidence.Checked = False

        ' Clear out the risk type selection
        ddBICRiskType.SelectedValue = "0"
        ' Don't clear business income coverage if Business Income ALS is set on the coverages page because it's required
        'Updated 12/20/18 for multi state MLW
        If SubQuoteFirst IsNot Nothing Then
            'If Not Quote.HasBusinessIncomeALS Then
            If Not SubQuoteFirst.HasBusinessIncomeALS Then
                Me.chkBusinessIncomeCoverage.Checked = False
            End If
        End If

        Me.chkPersonalPropertyCoverage.Checked = False
        Me.chkPersonalPropertyOfOthers.Checked = False

        Save_FireSaveEvent(False)
        Populate()

        Exit Sub
    End Sub

    'Added 10/20/2022 for task 77527 MLW
    Public Sub ToggleInflationGuardOptions()
        If CPR_InflationGuardHelper.IsInflationGuardNo2Available(Quote) AndAlso IsQuoteReadOnly() = False Then
            'drop down option value 2 has id of 1
            Dim removeNo2 As ListItem = ddBCInflationGuard.Items.FindByText("2")
            If removeNo2 IsNot Nothing Then
                Me.ddBCInflationGuard.Items.Remove(removeNo2)
            End If
        Else
            Dim inflationGuardNo2 As ListItem = ddBCInflationGuard.Items.FindByText("2")
            If inflationGuardNo2 Is Nothing Then
                'So the options show in accending order - adding just the 2 option adds it to the bottom of the list
                ddBCInflationGuard.Items.Clear()
                QQHelper.LoadStaticDataOptionsDropDown(ddBCInflationGuard, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, , Quote.LobType)
            End If
        End If
    End Sub
End Class