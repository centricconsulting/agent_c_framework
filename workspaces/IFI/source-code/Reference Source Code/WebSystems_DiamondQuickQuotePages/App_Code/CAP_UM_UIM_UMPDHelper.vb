Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class CAP_UM_UIM_UMPDHelper
    Private Shared _UM_UIM_UMPDSettings As NewFlagItem
    Public Shared ReadOnly Property UM_UIM_UMPDSettings() As NewFlagItem
        Get
            If _UM_UIM_UMPDSettings Is Nothing Then
                _UM_UIM_UMPDSettings = New NewFlagItem("VR_CAP_UM_UIM_UMPD_Settings")
            End If
            Return _UM_UIM_UMPDSettings
        End Get
    End Property

    Public Shared Function IsCAP_UM_UIM_UMPD_ChangesAvailable(quote As QuickQuoteObject) As Boolean
        If quote IsNot Nothing Then
            Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto
            UM_UIM_UMPDSettings.OtherQualifiers = IsCorrectLOB

            Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UM_UIM_UMPDSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
        End If
        Return False
    End Function
End Class
