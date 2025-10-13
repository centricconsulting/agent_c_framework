Imports QuickQuote.CommonObjects


Namespace IFM.VR.Common.Helpers.CPP
    Public Class RemoveBuildingLevelDeductibleHelper
        Inherits FeatureFlagBase
        Private Shared _RemoveBuildingLevelDeductibleSettings As NewFlagItem
        Public Shared ReadOnly Property RemoveBuildingLevelDeductibleSettings() As NewFlagItem
            Get
                If _RemoveBuildingLevelDeductibleSettings Is Nothing Then
                    _RemoveBuildingLevelDeductibleSettings = New NewFlagItem("VR_CPP_CPR_RemoveBuildingLevelDeductible_Settings")
                End If
                Return _RemoveBuildingLevelDeductibleSettings
            End Get
        End Property

        Const RemoveBuildingLevelDeductibleUpdatedMsg As String = "Effective July 15, 2025, the same property deductible applies to all coverages at a location. The default deductible is set to $1,000. Please review and adjust as needed, or contact your Underwriter for assistance."

        Public Shared Function RemoveBuildingLevelDeductibleEnabled() As Boolean
            Return RemoveBuildingLevelDeductibleSettings.EnabledFlag
        End Function

        Public Shared Function RemoveBuildingLevelDeductibleEffDate() As Date
            Return RemoveBuildingLevelDeductibleSettings.GetStartDateOrDefault("7/15/25")
        End Function

        Public Shared Sub UpdateRemoveBuildingLevelDeductible(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        Dim NeedsWarningMessage As Boolean = False
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                            For Each l In Quote.Locations
                                Dim CompareList As New List(Of String)
                                Dim allMatch As Boolean = True
                                Dim DeductibleBlanket As String = BlanketHelper_CPR_CPP.GetBlanketDeductibleID(SubQuoteFirst(Quote))
                                Dim DeductibleNoMatch As String = "9" '9 = 1,000
                                Dim HasBlanket As Boolean = False

                                If l.PropertyInTheOpenRecords IsNot Nothing AndAlso l.PropertyInTheOpenRecords.Count > 0 Then
                                    For Each pio In l.PropertyInTheOpenRecords
                                        If Not String.IsNullOrWhiteSpace(pio.DeductibleId) AndAlso pio.DeductibleId <> "0" Then
                                            CompareList.Add(pio.DeductibleId)
                                        End If
                                    Next
                                End If
                                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                    For Each b In l.Buildings
                                        If Not String.IsNullOrWhiteSpace(b.DeductibleId) AndAlso b.DeductibleId <> "0" Then
                                            CompareList.Add(b.DeductibleId)
                                        End If
                                        If Not String.IsNullOrWhiteSpace(b.PersPropCov_DeductibleId) AndAlso b.PersPropCov_DeductibleId <> "0" Then
                                            CompareList.Add(b.PersPropCov_DeductibleId)
                                        End If
                                        If Not String.IsNullOrWhiteSpace(b.PersPropOfOthers_DeductibleId) AndAlso b.PersPropOfOthers_DeductibleId <> "0" Then
                                            CompareList.Add(b.PersPropOfOthers_DeductibleId)
                                        End If
                                    Next
                                End If

                                For i As Integer = 1 To CompareList.Count - 1
                                    If CompareList(i) <> CompareList(0) Then
                                        allMatch = False
                                        Exit For
                                    End If
                                Next

                                If l.PropertyInTheOpenRecords IsNot Nothing AndAlso l.PropertyInTheOpenRecords.Count > 0 Then
                                    For Each pio In l.PropertyInTheOpenRecords
                                        If pio.IncludedInBlanketCoverage Then
                                            pio.DeductibleId = DeductibleBlanket
                                            HasBlanket = True
                                        ElseIf Not allMatch Then
                                            pio.DeductibleId = DeductibleNoMatch
                                        End If

                                    Next
                                End If

                                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                    For Each b In l.Buildings
                                        If b.IsBuildingValIncludedInBlanketRating Then
                                            b.DeductibleId = DeductibleBlanket
                                            HasBlanket = True
                                        ElseIf Not allMatch Then
                                            b.DeductibleId = DeductibleNoMatch
                                        End If
                                        If b.PersPropCov_IncludedInBlanketCoverage Then
                                            b.PersPropCov_DeductibleId = DeductibleBlanket
                                            HasBlanket = True
                                        ElseIf Not allMatch Then
                                            b.PersPropCov_DeductibleId = DeductibleNoMatch
                                        End If
                                        If b.PersPropOfOthers_IncludedInBlanketCoverage Then
                                            b.PersPropOfOthers_DeductibleId = DeductibleBlanket
                                            HasBlanket = True
                                        ElseIf Not allMatch Then
                                            b.PersPropOfOthers_DeductibleId = DeductibleNoMatch
                                        End If
                                    Next
                                End If

                                If HasBlanket Then
                                    l.DeductibleId = DeductibleBlanket
                                ElseIf Not allMatch Then
                                    l.DeductibleId = DeductibleNoMatch
                                    NeedsWarningMessage = True
                                End If

                            Next
                            If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                                Dim i = New WebValidationItem(RemoveBuildingLevelDeductibleUpdatedMsg)
                                i.IsWarning = True
                                ValidationErrors.Add(i)
                            End If
                        End If
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                End Select
            End If
        End Sub

        Public Shared Function HasBlanket(Quote As QuickQuoteObject) As Boolean
            Dim quoteToUse As QuickQuoteObject

            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                quoteToUse = Quote
            Else
                quoteToUse = SubQuoteFirst(Quote)
            End If
            Return quoteToUse.BlanketRatingOptionId IsNot Nothing AndAlso quoteToUse.BlanketRatingOptionId <> "0" AndAlso quoteToUse.BlanketRatingOptionId <> ""
        End Function

        Public Shared Function IsRemoveBuildingLevelDeductibleAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                RemoveBuildingLevelDeductibleSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RemoveBuildingLevelDeductibleSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
