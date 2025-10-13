Imports IFM.VR.Common.Helpers.CPP
Imports QuickQuote.CommonObjects

Namespace Helpers
    Public Class FindAppliedAdditionalInterestList
        Public Function FindAppliedAI(quote As QuickQuoteObject) As List(Of LocationBuildingAIItem)
            Dim AIs As List(Of LocationBuildingAIItem) = New List(Of LocationBuildingAIItem)
            Dim locationIndex As Int32 = 0
            For Each location As QuickQuoteLocation In quote.Locations
                Dim buildingIndex As Int32 = 0
                If location.Buildings IsNot Nothing Then
                    For Each building As QuickQuoteBuilding In location.Buildings
                        If building.AdditionalInterests IsNot Nothing Then
                            Dim buildingAiIndex As Int32 = 0
                            For Each ai As QuickQuoteAdditionalInterest In building.AdditionalInterests
                                AIs.Add(New LocationBuildingAIItem(locationIndex, buildingIndex, buildingAiIndex, ai))
                                buildingAiIndex += 1
                            Next
                        End If
                        buildingIndex += 1
                    Next
                    locationIndex += 1
                End If

            Next
            Return AIs
        End Function

        'Added 2/4/2022 for CPP Endorsements task 67310 MLW
        Public Function FindAppliedInlandMarineAI(quote As QuickQuoteObject) As List(Of InlandMarineAIItem)
            'quote here should be governingStateQuote that is passed in
            Dim AIs As List(Of InlandMarineAIItem) = New List(Of InlandMarineAIItem)
            Dim aiIndex As Int32 = 0
            Dim ceAiIndex As Int32 = 0
            'Find Builders Risk AIs
            If quote.BuildersRiskAdditionalInterests IsNot Nothing AndAlso quote.BuildersRiskAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.BuildersRiskAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.BuildersRisk, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Computers AIs
            If quote.ComputerAdditionalInterests IsNot Nothing AndAlso quote.ComputerAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.ComputerAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.Computers, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Contractors Scheduled Equipement AIs
            If quote.ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso quote.ContractorsEquipmentScheduledCoverages.Count > 0 Then
                For Each ce In quote.ContractorsEquipmentScheduledCoverages
                    If ce.AdditionalInterests IsNot Nothing AndAlso ce.AdditionalInterests.Count > 0 Then
                        For Each ai As QuickQuoteAdditionalInterest In ce.AdditionalInterests
                            AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.ContractorsScheduledEquipment, aiIndex, ceAiIndex, ai))
                            aiIndex += 1
                            ceAiIndex += 1
                        Next
                    End If
                Next
            End If

            'Find Fine Arts AIs
            If quote.FineArtsAdditionalInterests IsNot Nothing AndAlso quote.FineArtsAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.FineArtsAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.FineArts, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Motor Truck Cargo AIs
            If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(quote) Then
                If quote.MotorTruckCargoUnScheduledVehicleAdditionalInterests IsNot Nothing AndAlso quote.MotorTruckCargoUnScheduledVehicleAdditionalInterests.Count > 0 Then
                    For Each ai As QuickQuoteAdditionalInterest In quote.MotorTruckCargoUnScheduledVehicleAdditionalInterests
                        AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.MotorTruckCargo, aiIndex, ceAiIndex, ai))
                        aiIndex += 1
                    Next
                End If
            Else
                If quote.MotorTruckCargoScheduledVehicleAdditionalInterests IsNot Nothing AndAlso quote.MotorTruckCargoScheduledVehicleAdditionalInterests.Count > 0 Then
                    For Each ai As QuickQuoteAdditionalInterest In quote.MotorTruckCargoScheduledVehicleAdditionalInterests
                        AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.MotorTruckCargo, aiIndex, ceAiIndex, ai))
                        aiIndex += 1
                    Next
                End If
            End If

            'Find Owners Cargo AIs
            If quote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests IsNot Nothing AndAlso quote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.OwnersCargoAnyOneOwnedVehicleAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.OwnersCargo, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Scheduled Property Floater AIs
            If quote.ScheduledPropertyAdditionalInterests IsNot Nothing AndAlso quote.ScheduledPropertyAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.ScheduledPropertyAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.ScheduledPropertyFloater, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Signs AIs
            If quote.SignsAdditionalInterests IsNot Nothing AndAlso quote.SignsAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.SignsAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.Signs, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            'Find Transportation AIs
            If quote.TransportationCatastropheAdditionalInterests IsNot Nothing AndAlso quote.TransportationCatastropheAdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In quote.TransportationCatastropheAdditionalInterests
                    AIs.Add(New InlandMarineAIItem(InlandMarineTypeString.Transportation, aiIndex, ceAiIndex, ai))
                    aiIndex += 1
                Next
            End If
            Return AIs
        End Function

        'Added 2/4/2022 for CPP Endorsements task 67310 MLW
        Structure InlandMarineTypeString
            Const BuildersRisk = "Builders Risk"
            Const Computers = "Computers"
            Const ContractorsScheduledEquipment = "Contractors Scheduled Equipment"
            Const FineArts = "Fine Arts"
            Const MotorTruckCargo = "Motor Truck Cargo"
            Const OwnersCargo = "Owners Cargo"
            Const ScheduledPropertyFloater = "Scheduled Property Floater"
            Const Signs = "Signs"
            Const Transportation = "Transportation"
        End Structure
    End Class
End Namespace
