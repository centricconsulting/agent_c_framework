Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers

Namespace IFM.VR.Common.Helpers

    Public Enum CoverageSpecificClassCodes
        unknown = 0
        CPR_PPC = 1
        CPR_PPO = 2
        CPR_BC = 3
        CPR_BIC = 4
        CPR_SP = 5
    End Enum

    Public Class BlanketHelper_CPR_CPP
        Public Shared Function Get_SelectedBlanketCauseOfLossProperty(ByVal qso As QuickQuoteObject) As String
            'Todo - Public method that is expecting a state/subquote
            Dim propertyVal As String = "0"
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Select Case Get_CPRCPP_Blanket_ID(qso)
                        Case "1" 'combined
                            Return qso.BlanketBuildingAndContentsCauseOfLossTypeId
                        Case "2" 'building only
                            Return qso.BlanketBuildingCauseOfLossTypeId
                        Case "3" 'contents only
                            Return qso.BlanketContentsCauseOfLossTypeId
                        Case Else

                    End Select
                End If
            End If
            Return propertyVal
        End Function

        Public Shared Function Get_SelectedBlanketcoInsuranceProperty(ByVal qso As QuickQuoteObject) As String
            'Todo - Public method that is expecting a state/subquote
            Dim propertyVal As String = "0"
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Select Case Get_CPRCPP_Blanket_ID(qso)
                        Case "1" 'combined
                            Return qso.BlanketBuildingAndContentsCoinsuranceTypeId
                        Case "2" 'building only
                            Return qso.BlanketBuildingCoinsuranceTypeId
                        Case "3" 'contents only
                            Return qso.BlanketContentsCoinsuranceTypeId
                        Case Else

                    End Select
                End If
            End If
            Return propertyVal
        End Function

        Public Shared Function Get_SelectedBlanketValuationProperty(ByVal qso As QuickQuoteObject) As String
            'Todo - Public method that is expecting a state/subquote
            Dim propertyVal As String = "0"
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Select Case Get_CPRCPP_Blanket_ID(qso)
                        Case "1" 'combined
                            Return qso.BlanketBuildingAndContentsValuationId
                        Case "2" 'building only
                            Return qso.BlanketBuildingValuationId
                        Case "3" 'contents only
                            Return qso.BlanketContentsValuationId
                        Case Else

                    End Select
                End If
            End If
            Return propertyVal
        End Function

        Public Shared Function Get_SelectedBlanketIsAgreedValueProperty(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            Dim propertyVal As Boolean = False
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Select Case Get_CPRCPP_Blanket_ID(qso)
                        Case "1" 'combined
                            Return qso.BlanketBuildingAndContentsIsAgreedValue
                        Case "2" 'building only
                            Return qso.BlanketBuildingIsAgreedValue
                        Case "3" 'contents only
                            Return qso.BlanketContentsIsAgreedValue
                        Case Else

                    End Select
                End If
            End If
            Return propertyVal
        End Function


        Public Shared Function Has_CPRCPP_CombinedBlanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketBuildingAndContents Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        Public Shared Function Has_CPRCPP_BuildingOnlyBlanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketBuilding Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        Public Shared Function Has_CPRCPP_ContentsOnlyBlanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketContents Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        Public Shared Function Has_CPRCPP_BuildingOrCombinedBlanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketBuilding Or qso.HasBlanketBuildingAndContents Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        Public Shared Function Has_CPRCPP_ContentsOrCombinedBlanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketContents Or qso.HasBlanketBuildingAndContents Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function


        Public Shared Function Has_CPRCPP_Blanket(ByVal qso As QuickQuoteObject) As Boolean
            'Todo - Public method that is expecting a state/subquote
            If qso IsNot Nothing Then
                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    If qso.HasBlanketBuilding Or qso.HasBlanketContents Or qso.HasBlanketBuildingAndContents Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function


        Public Shared Function Get_CPRCPP_Blanket_ID(ByVal qso As QuickQuoteObject) As String
            'Todo - Public method that is expecting a state/subquote
            If Has_CPRCPP_Blanket(qso) Then
                If qso.HasBlanketBuilding Then
                    Return "2"
                End If
                If qso.HasBlanketBuildingAndContents Then
                    Return "1"
                End If
                If qso.HasBlanketContents Then
                    Return "3"
                End If
            End If
            Return "0"
        End Function


        Public Shared Sub Set_CPRCPP_Blanket_ID(ByVal qso As QuickQuoteObject, ByVal anewvalue As String)
            'Todo - Public method that is expecting a state/subquote
            qso.HasBlanketBuildingAndContents = False
            qso.HasBlanketBuilding = False
            qso.HasBlanketContents = False

            If anewvalue = "0" Then
                'do nothing
            End If

            If anewvalue = "1" Then
                qso.HasBlanketBuildingAndContents = True
            End If

            If anewvalue = "2" Then
                qso.HasBlanketBuilding = True
            End If

            If anewvalue = "3" Then
                qso.HasBlanketContents = True
            End If


        End Sub

        Public Shared Sub SetBlanketProperies(qso As QuickQuoteObject, CauseOfLossId As String, coInsuranceId As String, ValuationId As String)
            'Todo - Public method that is expecting a state/subquote
            If Has_CPRCPP_Blanket(qso) Then
                Dim blanketType As String = Get_CPRCPP_Blanket_ID(qso)

                'depending on the blanket type different coverages will get these automatic adjustments 
                'blanket types
                'combined, building only,contents only

                ' make sure all building coverages have blanket applied except business income
                ' make sure all the co-insurances are 6(90%) or 7(100%)

                ' Set all values to nothing
                qso.BlanketBuildingCauseOfLossTypeId = Nothing
                qso.BlanketBuildingCoinsuranceTypeId = Nothing
                qso.BlanketBuildingValuationId = Nothing
                qso.BlanketBuildingAndContentsCauseOfLossTypeId = Nothing
                qso.BlanketBuildingAndContentsCoinsuranceTypeId = Nothing
                qso.BlanketBuildingAndContentsValuationId = Nothing
                qso.BlanketBusinessIncomeCauseOfLossTypeId = Nothing
                qso.BlanketBusinessIncomeCoinsuranceTypeId = Nothing
                qso.BlanketBusinessIncomeValuationId = Nothing
                qso.BlanketContentsCauseOfLossTypeId = Nothing
                qso.BlanketContentsCoinsuranceTypeId = Nothing
                qso.BlanketContentsValuationId = Nothing

                ' Set values based on blanket type
                ' Building & Contents 1
                ' Building 2
                ' Contents Only 3
                Select Case blanketType
                    Case "1"
                        qso.BlanketBuildingAndContentsCauseOfLossTypeId = CauseOfLossId
                        qso.BlanketBuildingAndContentsCoinsuranceTypeId = coInsuranceId
                        qso.BlanketBuildingAndContentsValuationId = ValuationId
                        Exit Select
                    Case "2"
                        qso.BlanketBuildingCauseOfLossTypeId = CauseOfLossId
                        qso.BlanketBuildingCoinsuranceTypeId = coInsuranceId
                        qso.BlanketBuildingValuationId = ValuationId
                        Exit Select
                    Case "3"
                        qso.BlanketContentsCauseOfLossTypeId = CauseOfLossId
                        qso.BlanketContentsCoinsuranceTypeId = coInsuranceId
                        qso.BlanketContentsValuationId = ValuationId
                        Exit Select
                End Select
            End If

            Exit Sub
        End Sub

        ''' <summary>
        ''' Only use when changing the defaults of a location.
        ''' </summary>
        ''' <param name="MainQuote"></param>
        ''' <param name="StateQuote"></param>
        ''' <param name="oldCause"></param>
        ''' <param name="oldCoInsuarance"></param>
        ''' <param name="oldValuation"></param>
        ''' <param name="olddeductible"></param>
        ''' <remarks></remarks>
        Public Shared Sub PropagetLocationDefaults(MainQuote As QuickQuoteObject, StateQuote As QuickQuoteObject, oldCause As String, oldCoInsuarance As String, oldValuation As String, olddeductible As String)
            iPropagateBlanketChange(MainQuote, StateQuote, oldCause, oldCoInsuarance, oldValuation, olddeductible)
        End Sub

        Public Shared Sub PropagateBlanketChange(MainQuote As QuickQuoteObject, StateQuote As QuickQuoteObject)
            iPropagateBlanketChange(MainQuote, StateQuote)
        End Sub


        ''' <summary>
        ''' Only use when you have changed the blanket type or a property of the blanket that applied to building coverages or pito. 
        ''' Never send the optional parms they are only there for the location default chainging method above.
        ''' SHOULD REMAIN PRIVATE!!!!!!!!!!!!!!!!!!!!!!!!
        ''' </summary>
        ''' <param name="MainQuote"></param>
        ''' <param name="StateQuote"></param>
        ''' <param name="oldCause">STOP!</param>
        ''' <param name="oldCoInsuarance">STOP!</param>
        ''' <param name="oldValuation">STOP!</param>
        ''' <param name="olddeductible">Seriously STOP and look at comments!!!!!!!</param>
        ''' <remarks></remarks>
        Private Shared Sub iPropagateBlanketChange(MainQuote As QuickQuoteObject, StateQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional oldCause As String = "", Optional oldCoInsuarance As String = "", Optional oldValuation As String = "", Optional olddeductible As String = "")
            If Not Has_CPRCPP_Blanket(StateQuote) Then
                ClearAllBlanketInformation(StateQuote)
            End If

            Dim blanketType As String = Get_CPRCPP_Blanket_ID(StateQuote)

            If MainQuote.Locations IsNot Nothing Then
                For Each Loc As QuickQuoteLocation In MainQuote.Locations
                    If Loc.Buildings IsNot Nothing Then
                        For Each building As QuickQuoteBuilding In Loc.Buildings
                            ' BI
                            If String.IsNullOrWhiteSpace(building.Limit) = False Or building.EarthquakeApplies Then
                                ' does this blanket type apply to this coverage??
                                If blanketType = "1" Or blanketType = "2" Then
                                    building.IsBuildingValIncludedInBlanketRating = True
                                    building.CauseOfLossTypeId = Get_SelectedBlanketCauseOfLossProperty(StateQuote)
                                    building.CoinsuranceTypeId = Get_SelectedBlanketcoInsuranceProperty(StateQuote)
                                    building.ValuationId = Get_SelectedBlanketValuationProperty(StateQuote)
                                Else
                                    building.IsBuildingValIncludedInBlanketRating = False
                                    'go back to default?????
                                    Dim buildingCovHailId As String = building.OptionalWindstormOrHailDeductibleId

                                    ' is this a change at the location level defaults????????
                                    If oldCause <> String.Empty Then
                                        If building.CauseOfLossTypeId = oldCause AndAlso building.DeductibleId = olddeductible AndAlso building.CoinsuranceTypeId = oldCoInsuarance AndAlso building.ValuationId = oldValuation Then
                                            ' coverage is using defaults
                                            building.CauseOfLossTypeId = Loc.CauseOfLossTypeId
                                            building.CoinsuranceTypeId = Loc.CoinsuranceTypeId
                                            building.DeductibleId = Loc.DeductibleId
                                            building.ValuationId = Loc.ValuationMethodTypeId
                                            ' is using the default so use the deductibleID on this control
                                            buildingCovHailId = WindHailHelper.GetWindHailDeducID(building.Limit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, Loc.DeductibleId)
                                        Else
                                            ' not using the default deductible value from this page need to use the deductibleid stored on the building->builingcoverage
                                            buildingCovHailId = WindHailHelper.GetWindHailDeducID(building.Limit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, building.DeductibleId)
                                        End If
                                    End If

                                    ' this must always match the deductible
                                    building.OptionalTheftDeductibleId = building.DeductibleId

                                    ' only the building cov uses this logic for now '3-28-2013
                                    building.OptionalWindstormOrHailDeductibleId = buildingCovHailId
                                End If
                                'If building.IsAgreedValue Then ' Bug 4845 Must set coinsurance to 100% if agreed amount is true
                                '    building.CoinsuranceTypeId = "7"
                                'End If
                            Else
                                building.IsBuildingValIncludedInBlanketRating = Nothing
                            End If

                            'PPC
                            If String.IsNullOrWhiteSpace(building.PersPropCov_PersonalPropertyLimit) = False Or building.PersPropCov_EarthquakeApplies Then
                                ' does this blanket type apply to this coverage??
                                If blanketType = "1" Or blanketType = "3" Then
                                    building.PersPropCov_IncludedInBlanketCoverage = True
                                    building.PersPropCov_CauseOfLossTypeId = Get_SelectedBlanketCauseOfLossProperty(StateQuote)
                                    building.PersPropCov_CoinsuranceTypeId = Get_SelectedBlanketcoInsuranceProperty(StateQuote)
                                    building.PersPropCov_ValuationId = Get_SelectedBlanketValuationProperty(StateQuote)
                                Else
                                    building.PersPropCov_IncludedInBlanketCoverage = False
                                    'go back to default?????
                                    Dim windHailDedId As String = building.PersPropCov_OptionalWindstormOrHailDeductibleId
                                    If oldCause <> String.Empty Then
                                        If building.PersPropCov_CauseOfLossTypeId = oldCause AndAlso building.PersPropCov_DeductibleId = olddeductible AndAlso building.PersPropCov_CoinsuranceTypeId = oldCoInsuarance AndAlso building.PersPropCov_ValuationId = oldValuation Then
                                            ' coverage is using defaults
                                            building.PersPropCov_CauseOfLossTypeId = Loc.CauseOfLossTypeId
                                            building.PersPropCov_CoinsuranceTypeId = Loc.CoinsuranceTypeId
                                            building.PersPropCov_DeductibleId = Loc.DeductibleId
                                            building.PersPropCov_ValuationId = Loc.ValuationMethodTypeId
                                            windHailDedId = WindHailHelper.GetWindHailDeducID(building.PersPropCov_PersonalPropertyLimit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, Loc.DeductibleId) '7-12-13
                                        Else
                                            windHailDedId = WindHailHelper.GetWindHailDeducID(building.PersPropCov_PersonalPropertyLimit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, building.PersPropCov_DeductibleId) '7-12-13
                                        End If
                                    End If
                                    building.PersPropCov_OptionalWindstormOrHailDeductibleId = windHailDedId
                                End If
                                'If building.PersPropCov_IsAgreedValue Then ' Bug 4845 Must set coinsurance to 100% if agreed amount is true
                                '    building.PersPropCov_CoinsuranceTypeId = "7"
                                'End If

                            Else
                                building.PersPropCov_IncludedInBlanketCoverage = False
                            End If

                            'PPO
                            If String.IsNullOrWhiteSpace(building.PersPropOfOthers_PersonalPropertyLimit) = False Or building.PersPropOfOthers_EarthquakeApplies Then
                                ' does this blanket type apply to this coverage??
                                If blanketType = "1" Or blanketType = "3" Then
                                    building.PersPropOfOthers_IncludedInBlanketCoverage = True
                                    building.PersPropOfOthers_CauseOfLossTypeId = Get_SelectedBlanketCauseOfLossProperty(StateQuote)
                                    building.PersPropOfOthers_CoinsuranceTypeId = Get_SelectedBlanketcoInsuranceProperty(StateQuote)
                                    building.PersPropOfOthers_ValuationId = Get_SelectedBlanketValuationProperty(StateQuote)
                                Else
                                    building.PersPropOfOthers_IncludedInBlanketCoverage = False
                                    'go back to default?????
                                    Dim windHailDedId As String = building.PersPropOfOthers_OptionalWindstormOrHailDeductibleId
                                    If oldCause <> String.Empty Then
                                        If building.PersPropOfOthers_CauseOfLossTypeId = oldCause AndAlso building.PersPropOfOthers_DeductibleId = olddeductible AndAlso building.PersPropOfOthers_CoinsuranceTypeId = oldCoInsuarance AndAlso building.PersPropOfOthers_ValuationId = oldValuation Then
                                            ' coverage is using defaults
                                            building.PersPropOfOthers_CauseOfLossTypeId = Loc.CauseOfLossTypeId
                                            building.PersPropOfOthers_CoinsuranceTypeId = Loc.CoinsuranceTypeId
                                            building.PersPropOfOthers_DeductibleId = Loc.DeductibleId
                                            building.PersPropOfOthers_ValuationId = Loc.ValuationMethodTypeId
                                            windHailDedId = WindHailHelper.GetWindHailDeducID(building.PersPropOfOthers_PersonalPropertyLimit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, Loc.DeductibleId) '7-12-13
                                        Else
                                            windHailDedId = WindHailHelper.GetWindHailDeducID(building.PersPropOfOthers_PersonalPropertyLimit, Loc.WindstormOrHailPercentageDeductibleId, Loc.WindstormOrHailMinimumDollarDeductibleId, building.PersPropOfOthers_DeductibleId) '7-12-13
                                        End If
                                    End If
                                    building.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = windHailDedId
                                End If

                            Else
                                building.PersPropOfOthers_IncludedInBlanketCoverage = False
                            End If

                            'BIC
                            building.BusinessIncomeCov_IncludedInBlanketCoverage = False ' always false
                            ' blanket never applies to BIC
                            If String.IsNullOrWhiteSpace(building.BusinessIncomeCov_Limit) = False Then
                                ' coverage is applied
                                If oldCause <> String.Empty Then
                                    If building.BusinessIncomeCov_CauseOfLossTypeId = oldCause AndAlso building.CoinsuranceTypeId = oldCoInsuarance AndAlso building.ValuationId = oldValuation Then
                                        ' coverage is using defaults
                                        building.BusinessIncomeCov_CauseOfLossTypeId = Loc.CauseOfLossTypeId
                                        'build.BusinessIncomeCov_CoinsuranceTypeId = Me.ddCoInsurance.SelectedValue
                                        'building.deduc = Me.ddDeductible.selectedvalue
                                        'building.valuation = Me.ddValuation.selectedvalue
                                    End If
                                End If
                            End If
                        Next
                    End If

                    If Loc.PropertyInTheOpenRecords IsNot Nothing Then
                        For Each pito As QuickQuotePropertyInTheOpenRecord In Loc.PropertyInTheOpenRecords
                            If blanketType = "1" Or blanketType = "2" Then
                                pito.CauseOfLossTypeId = Get_SelectedBlanketCauseOfLossProperty(StateQuote)
                                pito.CoinsuranceTypeId = Get_SelectedBlanketcoInsuranceProperty(StateQuote)
                                pito.ValuationId = Get_SelectedBlanketValuationProperty(StateQuote)
                            Else
                                'go back to default?????
                                '
                                'If (oldCause <> "") Then
                                '    pito.CauseOfLossTypeId = Loc.CauseOfLossTypeId
                                '    pito.CoinsuranceTypeId = Loc.CoinsuranceTypeId
                                '    pito.ValuationId = Loc.ValuationMethodTypeId
                                'End If
                            End If
                        Next
                    End If
                Next
            End If

        End Sub

        Private Shared Sub ClearAllBlanketInformation(qso As QuickQuoteObject)
            'no blanket is applied make sure all buildings and pito do not have blanket applied
            qso.BlanketBuildingCauseOfLossTypeId = Nothing
            qso.BlanketBuildingCoinsuranceTypeId = Nothing
            qso.BlanketBuildingValuationId = Nothing
            qso.BlanketBuildingLimit = Nothing

            qso.BlanketBuildingAndContentsCauseOfLossTypeId = Nothing
            qso.BlanketBuildingAndContentsCoinsuranceTypeId = Nothing
            qso.BlanketBuildingAndContentsLimit = Nothing
            qso.BlanketBuildingAndContentsValuationId = Nothing

            qso.BlanketBusinessIncomeCauseOfLossTypeId = Nothing
            qso.BlanketBusinessIncomeCoinsuranceTypeId = Nothing
            qso.BlanketBusinessIncomeLimit = Nothing
            qso.BlanketBusinessIncomeValuationId = Nothing

            qso.BlanketContentsCauseOfLossTypeId = Nothing
            qso.BlanketContentsCoinsuranceTypeId = Nothing
            qso.BlanketContentsLimit = Nothing
            qso.BlanketContentsValuationId = Nothing

            If qso.Locations IsNot Nothing Then
                For Each Loc As QuickQuoteLocation In qso.Locations
                    If Loc.Buildings IsNot Nothing Then
                        For Each builing As QuickQuoteBuilding In Loc.Buildings
                            If builing IsNot Nothing Then
                                builing.IsBuildingValIncludedInBlanketRating = Nothing
                                builing.PersPropCov_IncludedInBlanketCoverage = Nothing
                                builing.PersPropOfOthers_IncludedInBlanketCoverage = Nothing
                                builing.BusinessIncomeCov_IncludedInBlanketCoverage = Nothing ' always false     
                                'go back to default?????
                            End If
                        Next
                    End If

                    If Loc.PropertyInTheOpenRecords IsNot Nothing Then
                        For Each pito As QuickQuotePropertyInTheOpenRecord In Loc.PropertyInTheOpenRecords
                            If pito IsNot Nothing Then
                                pito.IncludedInBlanketCoverage = Nothing
                                'go back to default?????
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        Public Shared Function HasBuildingCoverageBlanketResrictions(qso As QuickQuoteObject, coveragetype As CoverageSpecificClassCodes) As Boolean
            'Todo - Public method that is expecting a state/subquote
            Dim blanketCoInsuranceRestrictionApplies As Boolean = False
            If qso IsNot Nothing Then
                Select Case coveragetype
                    Case CoverageSpecificClassCodes.CPR_BC
                        If Has_CPRCPP_BuildingOrCombinedBlanket(qso) Then
                            blanketCoInsuranceRestrictionApplies = True
                        End If
                    Case (CoverageSpecificClassCodes.CPR_PPC)
                        If Has_CPRCPP_ContentsOrCombinedBlanket(qso) Then
                            blanketCoInsuranceRestrictionApplies = True
                        End If
                    Case CoverageSpecificClassCodes.CPR_PPO
                        If Has_CPRCPP_ContentsOrCombinedBlanket(qso) Then
                            blanketCoInsuranceRestrictionApplies = True
                        End If
                End Select
            End If

            Return blanketCoInsuranceRestrictionApplies
        End Function

        Public Shared Function GetBlanketLimit(qso As QuickQuoteObject) As Double
            'Todo - Public method that is expecting a state/subquote
            Dim limit As Double = 0.0
            If Has_CPRCPP_Blanket(qso) Then
                limit += If(String.IsNullOrWhiteSpace(qso.BlanketBuildingAndContentsLimit), 0.0, CDbl(qso.BlanketBuildingAndContentsLimit))
                limit += If(String.IsNullOrWhiteSpace(qso.BlanketBuildingLimit), 0.0, CDbl(qso.BlanketBuildingLimit))
                limit += If(String.IsNullOrWhiteSpace(qso.BlanketContentsLimit), 0.0, CDbl(qso.BlanketContentsLimit))
            End If
            Return limit
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="qso">SubQuoteFirst</param>
        ''' <returns>DeductibleId</returns>
        Public Shared Function GetBlanketDeductibleID(qso As QuickQuoteObject) As String
            'Todo - Public method that is expecting a state/subquote
            If Has_CPRCPP_Blanket(qso) Then
                Dim blanketType As String = Get_CPRCPP_Blanket_ID(qso)

                Dim NewBlanket As String
                Dim OldBlanket As String

                Select Case blanketType
                    Case "1" ' Building & Contents 1
                        NewBlanket = qso.BlanketBuildingAndContentsDeductibleID
                    Case "2" ' Building 2
                        NewBlanket = qso.BlanketBuildingDeductibleID
                    Case "3" ' Contents Only 3
                        NewBlanket = qso.BlanketContentsDeductibleID
                End Select

                If blanketType = "1" OrElse blanketType = "2" Then
                    ' Combined or Building
                    If qso.Locations(0).Buildings(0).DeductibleId <> "0" Then
                        OldBlanket = qso.Locations(0).Buildings(0).DeductibleId
                    End If
                Else
                    ' Property only
                    If qso.Locations(0).Buildings(0).PersPropCov_DeductibleId <> "0" Then
                        OldBlanket = qso.Locations(0).Buildings(0).PersPropCov_DeductibleId
                    End If
                End If

                If (String.IsNullOrWhiteSpace(NewBlanket) OrElse NewBlanket = "0") AndAlso (Not String.IsNullOrWhiteSpace(OldBlanket) AndAlso OldBlanket <> "0") Then
                    Return OldBlanket
                Else
                    Return NewBlanket
                End If

            End If
            Return ""
        End Function

        Public Shared Sub SetBlanketDeductibleID(qso As QuickQuoteObject, id As String)
            'Todo - Public method that is expecting a state/subquote
            If Has_CPRCPP_Blanket(qso) Then
                Dim blanketType As String = Get_CPRCPP_Blanket_ID(qso)

                qso.BlanketBuildingAndContentsDeductibleID = ""
                qso.BlanketBuildingDeductibleID = ""
                qso.BlanketContentsDeductibleID = ""

                Select Case blanketType
                    Case "1" ' Building & Contents 1
                        qso.BlanketBuildingAndContentsDeductibleID = id
                    Case "2" ' Building 2
                        qso.BlanketBuildingDeductibleID = id
                    Case "3" ' Contents Only 3
                        qso.BlanketContentsDeductibleID = id
                End Select
            End If
        End Sub

    End Class

End Namespace
