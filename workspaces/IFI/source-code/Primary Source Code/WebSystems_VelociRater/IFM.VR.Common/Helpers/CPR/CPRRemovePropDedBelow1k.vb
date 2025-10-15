Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CPR
    Public Class CPRRemovePropDedBelow1k
        Private Shared _RemovePropDedBelow1kSettings As NewFlagItem
        Public Shared ReadOnly Property RemovePropDedBelow1kSettings() As NewFlagItem
            Get
                If _RemovePropDedBelow1kSettings Is Nothing Then
                    _RemovePropDedBelow1kSettings = New NewFlagItem("VR_CPP_CPR_RemovePropDedBelow1k_Settings")
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
            If Quote IsNot Nothing Then
                Dim NeedsWarningMessage As Boolean = False
                Dim qqHelper = New QuickQuoteHelperClass
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                            For Each l In Quote.Locations
                                'Set blanket deductible id to 1k if lower than 1k
                                If l.DeductibleId = "8" Then '8=500
                                    l.DeductibleId = "9" '9=1000
                                    NeedsWarningMessage = True
                                End If
                                'Set property in the open deductible id to 1k if lower than 1k
                                If l.PropertyInTheOpenRecords IsNot Nothing AndAlso l.PropertyInTheOpenRecords.Count > 0 Then
                                    For Each pio In l.PropertyInTheOpenRecords
                                        If pio.DeductibleId = "8" Then '8=500
                                            pio.DeductibleId = "9" '9=1000
                                            NeedsWarningMessage = True
                                        End If
                                    Next
                                End If
                                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                    For Each b In l.Buildings
                                        'Set Building Coverage deductible id to 1k if lower than 1k
                                        If b.DeductibleId = "8" Then '8=500
                                            b.DeductibleId = "9" '9=1000
                                            NeedsWarningMessage = True
                                        End If
                                        'Set Personal Property Coverage deductible id to 1k if lower than 1k
                                        If b.PersPropCov_DeductibleId = "8" Then '8=500
                                            b.PersPropCov_DeductibleId = "9" '9=1000
                                            NeedsWarningMessage = True
                                        End If
                                        'Set Personal Property of Others deductible id to 1k if lower than 1k
                                        If b.PersPropOfOthers_DeductibleId = "8" Then '8=500
                                            b.PersPropOfOthers_DeductibleId = "9" '9=1000
                                            NeedsWarningMessage = True
                                        End If
                                    Next
                                End If
                            Next
                        End If

                        'show message
                        If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(deductibleUpdatedMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        'Allow deductibles lower than 1k
                End Select
            End If
        End Sub

        Public Shared Function IsPropertyDeductibleBelow1kAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage _
                OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                RemovePropDedBelow1kSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RemovePropDedBelow1kSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
