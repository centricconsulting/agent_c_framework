Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports PopupMessageClass
Imports System.Web.UI

Namespace IFM.VR.Common.Helpers.CPR
    Public Class ValuationACVHelper
        Inherits FeatureFlagBase

        Private Shared _ValuationACVSettings As NewFlagItem
        Const CPPCPRValuationACVFirstWrittenDateKey As String = "VR_CPP_CPR_ValuationACV_FirstWrittenDate"
        Public Shared ReadOnly Property ValuationACVSettings() As NewFlagItem
            Get
                If _ValuationACVSettings Is Nothing Then
                    _ValuationACVSettings = New NewFlagItem("VR_CPP_CPR_ValuationACV_Settings")
                End If
                Return _ValuationACVSettings
            End Get
        End Property

        Const ValuationRCMsg As String = "We do not offer replacement cost valuation for dwelling exposures. Please review and adjust the requested coverage limits for all dwellings as needed."
        Public Shared Function ValuationACVEnabled() As Boolean
            Return ValuationACVSettings.EnabledFlag
        End Function

        Public Shared Function ValuationACVEffDate() As Date
            Return ValuationACVSettings.GetStartDateOrDefault("4/15/2025")
        End Function

        Public Const BuildingCoverage As String = "{F7A5D218-8C49-4EB2-B0B2-EF0B85A9E340}"
        Public Const PersonalPropertyCoverage As String = "{DFE8D2D6-AB64-460A-9C27-7B3D473E57A7}"
        Public Const PersonalPropertyOfOthers As String = "{EBAB6774-2E98-4F80-89AD-CF0268A14DF0}"

        Public Shared Sub UpdateValuationACV(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'if all buildings for this location are dwelling classifications & property in the open (PITO) valuation is Replacement Cost (RC), set PITO valuation to Actual Cash Value (ACV)
                    'Note: future changes will also apply ACV to the valution drop down if RC and dwelling classification for building coverage/personal property coverage/personal property of others
                    Dim NeedsWarningMessage As Boolean = False
                    Dim updatedLocation As Boolean = False
                    Dim updatedBuildingCoverage As Boolean = False
                    If Quote IsNot Nothing Then
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                            Dim locIndex As Integer = 0
                            For Each l As QuickQuoteLocation In Quote.Locations
                                updatedLocation = UpdateValuationACV_ForDwellingClass_ForLocation(l)
                                updatedBuildingCoverage = UpdateValuationACV_ForDwellingClass_ForBuildingCoverage(l)
                                If updatedLocation OrElse updatedBuildingCoverage Then
                                    NeedsWarningMessage = True
                                End If
                            Next
                        End If
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(ValuationRCMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Private Shared Function UpdateValuationACV_ForDwellingClass_ForBuildingCoverage(l As QuickQuoteLocation) As Object
            Dim valuationUpdated As Boolean = False
            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                For Each b In l.Buildings
                    Dim bcValuationUpdated As Boolean = UpdateValuation(b, BuildingCoverage)
                    Dim ppcValuationUpdated As Boolean = UpdateValuation(b, PersonalPropertyCoverage)
                    Dim ppoValuationUpdated As Boolean = UpdateValuation(b, PersonalPropertyOfOthers)
                    If bcValuationUpdated OrElse ppcValuationUpdated OrElse ppoValuationUpdated Then
                        valuationUpdated = True
                    End If
                Next
            End If
            Return valuationUpdated
        End Function

        Public Shared Function UpdateValuation(b As QuickQuoteBuilding, cov As String) As Boolean
            Dim valuationUpdated As Boolean = False
            If b IsNot Nothing Then
                Select Case cov
                    Case BuildingCoverage
                        If b.ClassificationCode IsNot Nothing AndAlso b.ClassificationCode.ClassCode IsNot Nothing _
                        AndAlso ValuationACVHelper.IsBuildingDwellingClass(b.ClassificationCode.ClassCode) _
                        AndAlso b.ValuationId = "1" Then
                            b.ValuationId = "2"
                            valuationUpdated = True
                        End If
                    Case PersonalPropertyCoverage
                        If b.PersPropCov_ClassificationCode IsNot Nothing AndAlso b.PersPropCov_ClassificationCode.ClassCode IsNot Nothing _
                        AndAlso ValuationACVHelper.IsBuildingDwellingClass(b.PersPropCov_ClassificationCode.ClassCode) _
                        AndAlso b.PersPropCov_ValuationId = "1" Then
                            b.PersPropCov_ValuationId = "2"
                            valuationUpdated = True
                        End If
                    Case PersonalPropertyOfOthers
                        If b.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso b.PersPropOfOthers_ClassificationCode.ClassCode IsNot Nothing _
                        AndAlso ValuationACVHelper.IsBuildingDwellingClass(b.PersPropOfOthers_ClassificationCode.ClassCode) _
                        AndAlso b.PersPropOfOthers_ValuationId = "1" Then
                            b.PersPropOfOthers_ValuationId = "2"
                            valuationUpdated = True
                        End If
                End Select
            End If
            Return valuationUpdated
        End Function

        Public Shared Function IsBuildingDwellingClass(classCode As String) As Boolean
            'dwelling class codes are 0196, 0197, 0198
            Dim isIt As Boolean = False
            If classCode.EqualsAny("0196", "0197", "0198") Then isIt = True
            Return isIt
        End Function

        Public Shared Function RemoveReplacementCostForDwellingClass(b As QuickQuoteBuilding) As Boolean
            'Building has dwelling class code, then it qualifies to remove Replacement Cost from the Valuation drop downs
            Dim removeIt As Boolean = False
            If (b.ClassificationCode IsNot Nothing AndAlso b.ClassificationCode.ClassCode IsNot Nothing AndAlso
            ValuationACVHelper.IsBuildingDwellingClass(b.ClassificationCode.ClassCode)) OrElse
            (b.PersPropCov_ClassificationCode IsNot Nothing AndAlso b.PersPropCov_ClassificationCode.ClassCode IsNot Nothing _
            AndAlso ValuationACVHelper.IsBuildingDwellingClass(b.PersPropCov_ClassificationCode.ClassCode)) _
            OrElse (b.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso b.PersPropOfOthers_ClassificationCode.ClassCode IsNot Nothing _
            AndAlso ValuationACVHelper.IsBuildingDwellingClass(b.PersPropOfOthers_ClassificationCode.ClassCode)) Then

                removeIt = True

            End If
            Return removeIt
        End Function

        Public Shared Function UpdateValuationACV_ForDwellingClass_ForLocation(l As QuickQuoteLocation) As Boolean
            Dim valuationUpdated As Boolean = False
            'Dim NeedsWarningMessage As Boolean = False
            Dim hasLocationWithAllBuildingsWithDwellingClassCode As Boolean = False
            Dim locationHasPITOValuationSetToRC As Boolean = False 'RC = Replacement Cost
            Dim pitoHasRCIndexList As New List(Of Integer)
            If l.PropertyInTheOpenRecords IsNot Nothing AndAlso l.PropertyInTheOpenRecords.Count > 0 Then
                Dim pitoIndex As Integer = 0
                For Each pito As QuickQuotePropertyInTheOpenRecord In l.PropertyInTheOpenRecords
                    If pito.ValuationId = "1" Then
                        '1 = Replacement Cost
                        locationHasPITOValuationSetToRC = True
                        pitoHasRCIndexList.Add(pitoIndex)
                    End If
                    pitoIndex += 1
                Next
            End If
            If locationHasPITOValuationSetToRC Then
                hasLocationWithAllBuildingsWithDwellingClassCode = CheckAllBuildingsHaveDwellingClass(l)
            End If
            If hasLocationWithAllBuildingsWithDwellingClassCode Then
                'Setting Property in the Open Valuation to Actual Cash Value
                For Each pito_index In pitoHasRCIndexList
                    If l.PropertyInTheOpenRecords.HasItemAtIndex(pito_index) Then
                        Dim pito_item As QuickQuotePropertyInTheOpenRecord = l.PropertyInTheOpenRecords(pito_index)
                        pito_item.ValuationId = "2" 'Actual Cash Value
                        valuationUpdated = True
                    End If
                Next
            End If
            Return valuationUpdated
        End Function

        Public Shared Function CheckAllBuildingsHaveDwellingClass(l As QuickQuoteLocation) As Boolean
            Dim hasLocationWithAllBuildingsWithDwellingClassCode As Boolean = False
            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                Dim totalNumberOfBuildings As Integer = l.Buildings.Count
                Dim buildingsWithDwellingClassCodeCount As Integer = 0
                For Each b As QuickQuoteBuilding In l.Buildings
                    If b IsNot Nothing AndAlso b.ClassificationCode IsNot Nothing AndAlso b.ClassificationCode.ClassCode IsNot Nothing Then
                        'dwelling class codes are 0196, 0197, 0198
                        If IsBuildingDwellingClass(b.ClassificationCode.ClassCode) Then
                            buildingsWithDwellingClassCodeCount += 1
                        End If
                    End If
                Next
                If totalNumberOfBuildings = buildingsWithDwellingClassCodeCount Then
                    'set flag to true for all buildings with dwelling class code
                    hasLocationWithAllBuildingsWithDwellingClassCode = True
                End If
            End If
            Return hasLocationWithAllBuildingsWithDwellingClassCode
        End Function

        Public Shared Sub ShowValuationRCPopupMessage(page As Page)
            Using popup As New PopupMessageObject(page, ValuationRCMsg)
                With popup
                    .Title = "Valuation for Dwelling"
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End Sub

        Public Shared Function IsValuationACVAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then

                ValuationACVSettings.OtherQualifiers = DoesQuoteQualifyByFirstWrittenDate(quote, CPPCPRValuationACVFirstWrittenDateKey)

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, ValuationACVSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
