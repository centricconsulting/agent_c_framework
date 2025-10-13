Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.FARM
    Public Class CosDamHiddenHelper
        Private Shared _CosmeticDamageHiddenSettings As NewFlagItem
        Public Shared ReadOnly Property CosmeticDamageHiddenSettings() As NewFlagItem
            Get
                If _CosmeticDamageHiddenSettings Is Nothing Then
                    _CosmeticDamageHiddenSettings = New NewFlagItem("VR_FAR_CosmeticDamageHiddenForBuildingTypes_Settings")
                End If
                Return _CosmeticDamageHiddenSettings
            End Get
        End Property

        Const FarmCosDamForwardMsg As String = "Effective 03/01/2025 Cosmetic Damage Exclusion is no longer available for the following building types: Grain Legs, Grain Dryer, Well Pumps, Tanks, Private Power & Light Poles, Radio & Television Equipment, Windmill & Chargers and Outbuilding with Living Quarters. Coverage has been removed."

        Public Shared Function CosmeticDamageHiddenEnabled() As Boolean
            Return CosmeticDamageHiddenSettings.EnabledFlag
        End Function

        Public Shared Function CosmeticDamageHiddenEffDate() As Date
            Return CosmeticDamageHiddenSettings.GetStartDateOrDefault("3/1/2025")
        End Function

        Public Shared Sub UpdateCosmeticDamageHidden(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    Dim NeedsWarningMessage As Boolean = RemoveCosmeticDamageForBuildingTypes(Quote)
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(FarmCosDamForwardMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No changes
            End Select
        End Sub

        Public Shared Function RemoveCosmeticDamageForBuildingTypes(quote As QuickQuoteObject) As Boolean
            Dim NeedsWarningMessage As Boolean = False
            If quote IsNot Nothing AndAlso quote.Locations IsNot Nothing AndAlso quote.Locations.Count > 0 Then
                For Each l As QuickQuoteLocation In quote.Locations
                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                        For Each b As QuickQuoteBuilding In l.Buildings
                            If b IsNot Nothing AndAlso b.OptionalCoverageEs IsNot Nothing AndAlso b.OptionalCoverageEs.Count > 0 Then
                                Select Case b.FarmStructureTypeId
                                    Case "26", "27", "28", "33", "34", "35", "36", "37"
                                        Dim cosmeticDamageCov As QuickQuoteOptionalCoverageE = b.OptionalCoverageEs.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E)
                                        If cosmeticDamageCov IsNot Nothing Then
                                            b.OptionalCoverageEs.Remove(cosmeticDamageCov)
                                            NeedsWarningMessage = True
                                        End If
                                End Select
                            End If
                        Next
                    End If
                Next
            End If
            Return NeedsWarningMessage
        End Function

        Public Shared Function IsCosmeticDamageHiddenAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CosmeticDamageHiddenSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
