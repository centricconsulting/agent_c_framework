Imports IFM.VR.Common.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption



Namespace IFM.VR.Common.Helpers.CPP
    Public Class FunctionalReplacementCostHelper

        Private Shared _FunctionalReplacementCostSettings As NewFlagItem
        Public Shared ReadOnly Property FunctionalReplacementCostSettings() As NewFlagItem
            Get
                If _FunctionalReplacementCostSettings Is Nothing Then
                    _FunctionalReplacementCostSettings = New NewFlagItem("VR_CPP_CPR_FunctionalReplacementCost_Settings")
                End If
                Return _FunctionalReplacementCostSettings
            End Get
        End Property

        Const FunctionalReplacementCostUpdatedMsg As String = "The dwelling is removed from the blanket coverage because dwelling classifications are not eligible for blanket coverage."

        Public Shared Function FunctionalReplacementCostEnabled() As Boolean
            Return FunctionalReplacementCostSettings.EnabledFlag
        End Function

        Public Shared Function FunctionalReplacementCostEffDate() As Date
            Return FunctionalReplacementCostSettings.GetStartDateOrDefault("2/15/2025")
        End Function

        Public Shared Sub UpdateFunctionalReplacementCost(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        Dim NeedsWarningMessage As Boolean = False
                        ' Loop through all the buildings on the quote
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                                If L.Buildings IsNot Nothing AndAlso L.Buildings.Count > 0 Then
                                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                        If B.ClassificationCode.ClassCode = "0196" OrElse B.ClassificationCode.ClassCode = "0197" OrElse B.ClassificationCode.ClassCode = "0198" Then
                                            B.PersPropOfOthers_IncludedInBlanketCoverage = False
                                            B.IsBuildingValIncludedInBlanketRating = False
                                            B.PersPropCov_IncludedInBlanketCoverage = False
                                            NeedsWarningMessage = True
                                        End If
                                    Next
                                End If
                            Next
                        End If

                        'show message
                        If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(FunctionalReplacementCostUpdatedMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If

                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                End Select
            End If
        End Sub

        Public Shared Function IsFunctionalReplacementCostAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                    OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                FunctionalReplacementCostSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, FunctionalReplacementCostSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
