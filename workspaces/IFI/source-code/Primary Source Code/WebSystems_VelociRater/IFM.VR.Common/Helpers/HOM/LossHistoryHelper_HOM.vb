Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
Imports IFM.VR.Common.Helpers.FARM

Namespace IFM.VR.Common.Helpers.HOM
    Public Class LossHistoryHelper_HOM

        Public Shared Function GetAllHOMLosses(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            If topQuote IsNot Nothing Then
                If topQuote.LobId = "2" OrElse topQuote.LobId = "3" Then
                    Dim lossList As New List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)()

                    'updated 8/15/2018 for multi-state; original logic is in IF; references to quote changed to governingStateQuote
                    Dim governingStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = MultiState.General.GoverningStateQuote(topQuote) 'should always return something
                    If governingStateQuote IsNot Nothing Then
                        If governingStateQuote.LossHistoryRecords IsNot Nothing Then
                            lossList.AddRange(governingStateQuote.LossHistoryRecords)
                        End If

                        If governingStateQuote.Applicants IsNot Nothing Then
                            For Each app In governingStateQuote.Applicants
                                If app.LossHistoryRecords IsNot Nothing Then
                                    lossList.AddRange(app.LossHistoryRecords)
                                End If
                            Next
                        End If
                    End If

                    Return If(lossList.Any(), lossList, Nothing)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function GetSurchargeableHOMLosses(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim effectiveDate As DateTime = If(IsDate(topQuote.EffectiveDate), CDate(topQuote.EffectiveDate), DateTime.Now)
            Dim lossList As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord) = GetAllHOMLosses(topQuote)

            If lossList IsNot Nothing Then
                'updated 11/16/17 for HOM Upgrade to include new form types - MLW
                If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) AndAlso topQuote.Locations.GetItemAtIndex(0).FormTypeId.NotEqualsAny("4", "5", "6", "7", "25", "26") Then ' BUG 6482 States - Surcharges do not apply to HO4, HO6, ML2 and ML4 form types
                    If topQuote.Locations(0).FormTypeId = "22" AndAlso topQuote.Locations(0).StructureTypeId = "2" Then
                    Else

                        ' values based on rating spec
                        '\\ifmsr1\PAS QA Shared\Diamond\Personal\PL Home & Mobile Home\Working Version\C44 5 x - Personal Home Rating Specification v8.6 061314.xls
                        Return (From lh As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In lossList
                                Where (lh.LossHistorySurchargeId <> "" AndAlso
                                   lh.LossHistorySurchargeId = "1") And
                               (IsDate(lh.LossDate) AndAlso
                                CDate(lh.LossDate).AddYears(5).AddDays(1) > effectiveDate) And
                            (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(lh.Amount) > 500.99)
                                Select lh).ToList()
                    End If

                End If
            End If
            Return Nothing
        End Function

        Public Shared Function GetAllHOMSurcharges(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of KeyValuePair(Of String, String))
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim surchargePercent As New List(Of KeyValuePair(Of String, String))
            If topQuote IsNot Nothing Then
                If topQuote.Locations IsNot Nothing AndAlso topQuote.Locations.Any() Then
                    If topQuote.Locations(0) IsNot Nothing Then

                        ' Age of RV/Watercraft
                        Dim oldWatercraft As Boolean = False
                        If topQuote.Locations(0).RvWatercrafts IsNot Nothing Then
                            For Each item As QuickQuote.CommonObjects.QuickQuoteRvWatercraft In topQuote.Locations(0).RvWatercrafts
                                If item.Year <> "" AndAlso item.RvWatercraftTypeId = "1" Then 'added AndAlso item.RvWatercraftTypeId = "1" for Bug 7275
                                    Dim modelYear As Integer = item.Year
                                    Dim todayYear As Integer = DateTime.Now.Year
                                    Dim span As Integer = todayYear - modelYear

                                    If span >= 15 Then
                                        oldWatercraft = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If

                        'If oldWatercraft Then
                        If oldWatercraft AndAlso topQuote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then 'Bug 40350
                            surchargePercent.Add(New KeyValuePair(Of String, String)("Age of Watercraft", "20%"))
                        End If

                        ' Loss
                        'updated 11/16/17 to include new form types for HOM Upgrade MLW - need to not include ML2 though? <-------------------
                        If topQuote.LobId <> "2" OrElse (topQuote.LobId = "2" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("1", "2", "3", "22", "23", "24")) Then ' BUG 6482 States - Surcharges only apply to HO2, HO3 and HO3 w/15 form types
                            If topQuote.Locations(0).FormTypeId = "22" AndAlso topQuote.Locations(0).StructureTypeId = "2" Then
                            Else
                                Dim surCharge As Integer = GetSurchargePercent(topQuote)
                                If surCharge > 0 Then
                                    surchargePercent.Add(New KeyValuePair(Of String, String)("Loss", surCharge.ToString() + "%"))
                                End If
                            End If

                        End If

                        ' Swimming Pool/Hot Tub - Home Only - Not on Farm
                        If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(topQuote) = False Then
                            If topQuote.Locations(0).SwimmingPoolHotTubSurcharge Then
                                surchargePercent.Add(New KeyValuePair(Of String, String)("Swimming Pool/Hot Tub", topQuote.Locations(0).SwimmingPoolHotTubSurchargeQuotePremium.ToString()))
                            End If
                        End If

                        ' Trampoline
                        If TrampolineUnitsHelper.isTrampolineUnitsAvailable(topQuote) = False Then
                            If topQuote.Locations(0).TrampolineSurcharge Then
                                surchargePercent.Add(New KeyValuePair(Of String, String)("Trampoline", topQuote.Locations(0).TrampolineSurchargeQuotePremium.ToString()))
                            End If
                        End If

                        ' Wood or Fuel Burning Appliance
                        If WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(topQuote) = False Then
                            If topQuote.Locations(0).WoodOrFuelBurningApplianceSurcharge Then
                                surchargePercent.Add(New KeyValuePair(Of String, String)("Wood or Fuel Burning", topQuote.Locations(0).WoodOrFuelBurningApplianceSurchargeQuotePremium.ToString()))
                            End If
                        End If

                        ' Youthful Driver
                        If topQuote.Locations(0).RvWatercrafts IsNot Nothing AndAlso topQuote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                            Dim effDate As Date = If(IsDate(topQuote.EffectiveDate), topQuote.EffectiveDate, DateTime.MinValue)
                            If topQuote.Locations(0).RvWatercrafts IsNot Nothing Then
                                For Each item As QuickQuote.CommonObjects.QuickQuoteRvWatercraft In topQuote.Locations(0).RvWatercrafts
                                    For Each assignedOperNum As QuickQuote.CommonObjects.QuickQuoteOperator In item.Operators
                                        Dim assingedOper As QuickQuote.CommonObjects.QuickQuoteOperator = topQuote.Operators.Find(Function(p) p.OperatorNum = assignedOperNum.OperatorNum)
                                        If assingedOper IsNot Nothing Then
                                            Dim dateSpan As TimeSpan = effDate.Subtract(assingedOper.Name.BirthDate)
                                            Dim duration As Integer = dateSpan.Duration().Days / 365

                                            If duration <= 24 Then
                                                surchargePercent.Add(New KeyValuePair(Of String, String)("Youthful Driver", "15%"))
                                            End If
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    End If
                    If Farm_General.hasAdditionalQuestionsForFarmItemNumberOfUnitsUpdate(topQuote) Then
                        Dim SwimmingPoolHotTubSurcharge_Total As Double
                        Dim TrampolineSurcharge_Total As Double
                        Dim WoodOrFuelBurningApplianceSurcharge_Total As Double
                        Dim NumberOfSwimmingPoolHotTub As Integer
                        Dim NumberOfTrampoline As Integer
                        Dim NumberOfWoodOrFuelBurningAppliance As Integer
                        For Each location In topQuote.Locations
                            ' Swimming Pool/Hot Tub
                            If location.SwimmingPoolHotTubSurcharge Then
                                SwimmingPoolHotTubSurcharge_Total += location.SwimmingPoolHotTubSurchargeQuotePremium.TryToGetDouble
                                NumberOfSwimmingPoolHotTub += location.SwimmingPoolHotTubSurcharge_NumberOfUnits
                            End If

                            ' Trampoline
                            If location.TrampolineSurcharge Then
                                TrampolineSurcharge_Total += location.TrampolineSurchargeQuotePremium.TryToGetDouble
                                NumberOfTrampoline += location.TrampolineSurcharge_NumberOfUnits
                            End If

                            ' Wood or Fuel Burning Appliance
                            If location.WoodOrFuelBurningApplianceSurcharge Then
                                WoodOrFuelBurningApplianceSurcharge_Total += location.WoodOrFuelBurningApplianceSurchargeQuotePremium.TryToGetDouble
                                NumberOfWoodOrFuelBurningAppliance += location.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits
                            End If
                        Next
                        ' Swimming Pool/Hot Tub
                        If NumberOfSwimmingPoolHotTub > 0 AndAlso SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(topQuote) Then
                            surchargePercent.Add(New KeyValuePair(Of String, String)("Swimming Pool/Hot Tub (" & NumberOfSwimmingPoolHotTub & ")", SwimmingPoolHotTubSurcharge_Total.TryToFormatAsCurreny))
                        End If

                        ' Trampoline
                        If NumberOfTrampoline > 0 AndAlso TrampolineUnitsHelper.isTrampolineUnitsAvailable(topQuote) Then
                            surchargePercent.Add(New KeyValuePair(Of String, String)("Trampoline (" & NumberOfTrampoline & ")", TrampolineSurcharge_Total.TryToFormatAsCurreny))
                        End If

                        ' Wood or Fuel Burning Appliance
                        If NumberOfWoodOrFuelBurningAppliance > 0 AndAlso WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(topQuote) Then
                            surchargePercent.Add(New KeyValuePair(Of String, String)("Wood or Fuel Burning (" & NumberOfWoodOrFuelBurningAppliance & ")", WoodOrFuelBurningApplianceSurcharge_Total.TryToFormatAsCurreny))
                        End If
                    End If
                End If
            End If

            Return surchargePercent
        End Function

        Public Shared Function GetSurchargePercent(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Integer
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            ' values based on rating spec
            '\\ifmsr1\PAS QA Shared\Diamond\Personal\PL Home & Mobile Home\Working Version\C44 5 x - Personal Home Rating Specification v8.6 061314.xls

            ' Use first written date or effective date or todays date as last resort
            Dim firstWritten As Date = If(IsDate(topQuote.FirstWrittenDate), topQuote.FirstWrittenDate, If(IsDate(topQuote.EffectiveDate), topQuote.EffectiveDate, DateTime.Now.ToShortDateString()))
            Dim surchargeAbleLosses As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord) = GetSurchargeableHOMLosses(topQuote)
            Dim surchargePercent As Integer = 0
            Dim policyAge_Years As Double = (If(IsDate(topQuote.EffectiveDate), CDate(topQuote.EffectiveDate), CDate(DateTime.Now.ToShortDateString())) - firstWritten).Days / 365.0

            If surchargeAbleLosses IsNot Nothing Then
                Select Case policyAge_Years
                    Case Double.MinValue To 4.9999999
                        Select Case surchargeAbleLosses.Count
                            Case 0
                                surchargePercent = 0
                            Case 1
                                surchargePercent = 10
                            Case 2
                                surchargePercent = 30
                            Case 3
                                surchargePercent = 60
                            Case 0
                                surchargePercent = 100
                        End Select
                    Case 5 To 9.9999
                        Select Case surchargeAbleLosses.Count
                            Case 0
                                surchargePercent = -7
                            Case 1
                                surchargePercent = -2
                            Case 2
                                surchargePercent = 25
                            Case 3
                                surchargePercent = 60
                            Case 0
                                surchargePercent = 100
                        End Select
                    Case Else 'greater than or equal to 10
                        Select Case surchargeAbleLosses.Count
                            Case 0
                                surchargePercent = -10
                            Case 1
                                surchargePercent = -5
                            Case 2
                                surchargePercent = 20
                            Case 3
                                surchargePercent = 60
                            Case 0
                                surchargePercent = 100
                        End Select

                End Select
            End If

            Return surchargePercent
        End Function

        'Public Shared Function GetSurchargeableLossCount(quote As QuickQuote.CommonObjects.QuickQuoteObject) As Int32
        '    Return GetSurchargeableHOMLosses(quote).Count()
        'End Function

    End Class
End Namespace