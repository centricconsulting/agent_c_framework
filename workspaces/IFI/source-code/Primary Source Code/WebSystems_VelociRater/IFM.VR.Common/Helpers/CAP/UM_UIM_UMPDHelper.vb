Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CAP
    Public Class UM_UIM_UMPDHelper
        Inherits FeatureFlagBase

        Private Shared _UM_UIM_UMPDSettings As NewFlagItem
        Public Shared ReadOnly Property UM_UIM_UMPDSettings() As NewFlagItem
            Get
                If _UM_UIM_UMPDSettings Is Nothing Then
                    _UM_UIM_UMPDSettings = New NewFlagItem("VR_CAP_UM_UIM_UMPD_Settings")
                End If
                Return _UM_UIM_UMPDSettings
            End Get
        End Property

        Public Shared Function UM_UIM_UMPDEnabled() As Boolean
            Return UM_UIM_UMPDSettings.EnabledFlag
        End Function

        Public Shared Function UM_UIM_UMPDEffDate() As Date
            Return UM_UIM_UMPDSettings.GetStartDateOrDefault("8/1/2025")
        End Function

        Public Shared Sub UpdateUM_UIM_UMPD(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Dim SubQuotes As List(Of QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        If SubQuotes IsNot Nothing And SubQuotes.Count > 0 Then
                            For Each sq In SubQuotes
                                'translate old med pay ids to new
                                Select Case sq.MedicalPaymentsLimitid
                                    Case "11"
                                        '1,000
                                        sq.MedicalPaymentsLimitid = "170"
                                    Case "12"
                                        '2,000
                                        sq.MedicalPaymentsLimitid = "171"
                                    Case "15"
                                        '5,000
                                        sq.MedicalPaymentsLimitid = "173"
                                End Select
                            Next
                        End If

                        If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                            For Each vehicle In Quote.Vehicles
                                Dim garagingState = vehicle.GaragingAddress?.Address?.QuickQuoteState
                                If Quote.QuickQuoteState <> garagingState Then
                                    'If secondary state vehicles have UMPD, need to remove it as it won't be available beginning 8/1/2025
                                    vehicle.HasUninsuredMotoristPropertyDamage = False
                                    vehicle.UninsuredMotoristPropertyDamageLimitId = String.Empty
                                    vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = String.Empty
                                End If
                            Next
                        End If

                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        If SubQuotes IsNot Nothing And SubQuotes.Count > 0 Then
                            For Each sq In SubQuotes
                                'translate new med pay ids to old
                                Select Case sq.MedicalPaymentsLimitid
                                    Case "170"
                                        '1,000
                                        sq.MedicalPaymentsLimitid = "11"
                                    Case "171"
                                        '2,000
                                        sq.MedicalPaymentsLimitid = "12"
                                    Case "173"
                                        '5,000
                                        sq.MedicalPaymentsLimitid = "15"
                                End Select
                            Next
                        End If

                        If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                            For Each vehicle In Quote.Vehicles
                                Dim garagingState = vehicle.GaragingAddress?.Address?.QuickQuoteState
                                If garagingState = QuickQuoteHelperClass.QuickQuoteState.Indiana Then
                                    'No UMPD available for IN prior to 8/1/2025
                                    vehicle.HasUninsuredMotoristPropertyDamage = False
                                    vehicle.UninsuredMotoristPropertyDamageLimitId = String.Empty
                                    vehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = String.Empty
                                End If
                            Next
                        End If

                End Select
            End If
        End Sub

        Public Shared Function IsCAP_UM_UIM_UMPD_ChangesAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                UM_UIM_UMPDSettings.OtherQualifiers = IsCorrectLOB

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UM_UIM_UMPDSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
