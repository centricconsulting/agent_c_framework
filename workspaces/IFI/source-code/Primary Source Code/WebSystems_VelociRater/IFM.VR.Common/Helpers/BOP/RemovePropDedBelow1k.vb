Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.BOP
    Public Class RemovePropDedBelow1k
        Private Shared _RemovePropDedBelow1kSettings As NewFlagItem
        Public Shared ReadOnly Property RemovePropDedBelow1kSettings() As NewFlagItem
            Get
                If _RemovePropDedBelow1kSettings Is Nothing Then
                    _RemovePropDedBelow1kSettings = New NewFlagItem("VR_BOP_RemovePropDedBelow1k_Settings")
                End If
                Return _RemovePropDedBelow1kSettings
            End Get
        End Property

        Const deductibleUpdatedMsg As String = "We've updated the deductible to meet our minimum requirement of $1,000."

        Public Shared Function RemovePropDedBelow1kEnabled() As Boolean
            Return RemovePropDedBelow1kSettings.EnabledFlag
        End Function

        Public Shared Function RemovePropDedBelow1kEffDate() As Date
            Return RemovePropDedBelow1kSettings.GetStartDateOrDefault("9/1/2024")
        End Function

        Public Shared Sub UpdatePropertyDeductible(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Dim qqHelper = New QuickQuoteHelperClass
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Set to 1k if lower than 1k and show message
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        For Each l In Quote.Locations
                            Select Case l.PropertyDeductibleId
                                Case "21", "22"
                                    '21 = 250, 22 = 500
                                    'QuickQuoteBuilding.PropertyDeductibleId
                                    l.PropertyDeductibleId = "24" '1,000
                                    NeedsWarningMessage = True
                                    For Each b In l.Buildings
                                        b.PropertyDeductibleId = "24" '1,000
                                    Next
                            End Select
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(deductibleUpdatedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Allow deductibles lower than 1k
            End Select
        End Sub

        Public Shared Function IsPropertyDeductibleBelow1kAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                RemovePropDedBelow1kSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RemovePropDedBelow1kSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
