Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.AllLines
    Public Class BetterViewRatingHelper
        Inherits FeatureFlagBase
        Private Shared _BetterViewRatingSettings As NewFlagItem

        Const BetterViewLobsAllowedKey As String = "VR_AllLines_BetterView_LobsAllowedKey"
        Public Shared ReadOnly Property BetterViewRatingSettings() As NewFlagItem
            Get
                If _BetterViewRatingSettings Is Nothing Then
                    _BetterViewRatingSettings = New NewFlagItem("VR_AllLines_BetterView_PreLoadIntegrationCall_Settings")
                End If
                Return _BetterViewRatingSettings
            End Get
        End Property

        Public Shared Function IsBetterViewRatingAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                BetterViewRatingSettings.OtherQualifiers = DoesQuoteQualifyByLob(quote, "VR_AllLines_BetterView_LobsAllowedKey")
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, BetterViewRatingSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function



    End Class
End Namespace
