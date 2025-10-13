Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.FARM
    Public Class FarmBuildingtypeforbuildingHelper
        Inherits FeatureFlagBase
        Private Shared _FARMBuildingTypeForBuildingSettings As NewFlagItem


        Public Shared ReadOnly Property FARMBuildingTypeForBuildingsSettings() As NewFlagItem
            Get
                If _FARMBuildingTypeForBuildingSettings Is Nothing Then
                    _FARMBuildingTypeForBuildingSettings = New NewFlagItem("VR_FARM_BuildingType_For_Building")
                End If
                Return _FARMBuildingTypeForBuildingSettings
            End Get
        End Property


        Public Shared Function FARMBuildingTypeForBuildingsEnabled() As Boolean
            Return FARMBuildingTypeForBuildingsSettings.EnabledFlag
        End Function


        Public Shared Function FARMBuildingTypeForBuildingsEffDate() As Date
            Return FARMBuildingTypeForBuildingsSettings.GetStartDateOrDefault("4/15/2025")
        End Function


        Public Shared Sub UpdateFARMBuildingTypeForBuildings(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                    UpdateDefaultBuildingType(Quote)

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No changes
            End Select
        End Sub


        Public Shared Sub UpdateDefaultBuildingType(Quote As QuickQuoteObject)
            Dim FarmStructureType1Ids As String() = {"10", "11", "13", "14", "15", "20"} '10- BARN,11 - CONFINEMENT BUILDING, 13- IMPLEMENT/MACHINE SHED, 14- OUTBUILDING, 15 - POLE BUILDING,20 - QUONSET BUILDING
            Dim FarmStructureType2Ids As String() = {"16", "28", "29", "30", "31", "37"} ' 16 -SILO, 28 - GRAIN LEG, 29 -POULTRY BUILDING, 30 - CRIB, 31- GRANARY, 37 - OUTBUILDING With LIVING QUARTERS
            Dim FarmStructureType3Ids As String() = {"21", "26"} ' //21 - PORTABLE STRUCTURE , 26 - WELL PUMP
            Dim FarmStructureType4Ids As String() = {"35", "36"} '35 - RADIO &amp; TELEVISION EQUIPMENT, 36 - WINDMILLS AND CHARGERS
            Dim FarmStructureType5Ids As String() = {"33"}  ' 33 - Tank

            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                For Each l As QuickQuoteLocation In Quote.Locations
                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                        For Each b As QuickQuoteBuilding In l.Buildings
                            If b IsNot Nothing Then
                                If FarmStructureType1Ids.Contains(b.FarmStructureTypeId) = True AndAlso b.FarmTypeId = "5" Then
                                    b.FarmTypeId = 0 'default value  0 Empty/Null
                                ElseIf FarmStructureType2Ids.Contains(b.FarmStructureTypeId) = True AndAlso
                                   (b.FarmTypeId = "5" OrElse b.FarmTypeId = "8") Then
                                    b.FarmTypeId = 0  'default value  0 Empty/Null
                                ElseIf FarmStructureType3Ids.Contains(b.FarmStructureTypeId) = True AndAlso
                                   (b.FarmTypeId = "5" OrElse b.FarmTypeId = "1" OrElse b.FarmTypeId = "2" OrElse b.FarmTypeId = "8") Then
                                    b.FarmTypeId = 3 'default value  3- type 3
                                ElseIf FarmStructureType4Ids.Contains(b.FarmStructureTypeId) = True AndAlso
                                    (b.FarmTypeId = "1" OrElse b.FarmTypeId = "2" OrElse b.FarmTypeId = "3" OrElse b.FarmTypeId = "8") Then
                                    b.FarmTypeId = 5 'default value  5 - N/A
                                ElseIf FarmStructureType5Ids.Contains(b.FarmStructureTypeId) = True AndAlso
                                    (b.FarmTypeId = "8") Then
                                    b.FarmTypeId = 0 'default value  0 - Empty/Null

                                End If
                            End If
                        Next
                    End If
                Next
            End If


        End Sub


        Public Shared Function IsFARMBuildingTypeForBuildingsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FARMBuildingTypeForBuildingsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
