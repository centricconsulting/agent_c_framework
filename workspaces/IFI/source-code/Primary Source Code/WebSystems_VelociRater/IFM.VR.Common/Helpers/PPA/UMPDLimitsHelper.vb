Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA
    Public Class UMPDLimitsHelper
        Const UMPDLimitsNotAvailMsg As String = "Updated UMPD limits are not available prior to 12/01/2023. UMPD limit will be changed to the included $15,000 limit for quotes or endorsements amended to an effective date prior to 12/01/2023."
        
        Private Shared _UMPDLimitsSettings As NewFlagItem
        Public Shared ReadOnly Property UMPDLimitsSettings() As NewFlagItem
            Get
                If _UMPDLimitsSettings Is Nothing Then
                    _UMPDLimitsSettings = New NewFlagItem("VR_PPA_UMPDLimits_Settings")
                End If
                Return _UMPDLimitsSettings
            End Get
        End Property

        Public Shared Function UMPDLimitsEnabled() As Boolean
            Return UMPDLimitsSettings.EnabledFlag
        End Function

        Public Shared Sub UpdateUMPDLimits(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No Change       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Vehicles with UMPD selected, set back to 15,000 default
                    Dim NeedsWarningMessage As Boolean = False
                    If Quote IsNot Nothing AndAlso Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                        For each vehicle In Quote.Vehicles
                            If (vehicle.UninsuredMotoristPropertyDamageLimitId <> "0" AndAlso IsNullEmptyorWhitespace(vehicle.UninsuredMotoristPropertyDamageLimitId) = False) Then
                                NeedsWarningMessage = True
                                vehicle.UninsuredMotoristPropertyDamageLimitId = "48" '15,000
                            End If
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(UMPDLimitsNotAvailMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsUMPDLimitsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim IsCorrectState As Boolean = quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois
                UMPDLimitsSettings.OtherQualifiers = IsCorrectState
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UMPDLimitsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
