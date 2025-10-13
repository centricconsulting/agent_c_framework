Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.AvailabilityByDateOrVersion

Namespace IFM.VR.Common.Helpers.FARM
    Public Class OutbuildingLivingQuartersHelper
        Private Shared _OutbuildingLivingQuartersSettings As NewFlagItem
        Public Shared ReadOnly Property OutbuildingLivingQuartersSettings() As NewFlagItem
            Get
                If _OutbuildingLivingQuartersSettings Is Nothing Then
                    _OutbuildingLivingQuartersSettings = New NewFlagItem("VR_FAR_OutbuildingLivingQuarters_Date")
                End If
                Return _OutbuildingLivingQuartersSettings
            End Get
        End Property

        Public Shared Function isOutbuildingLivingQuartersAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim SubQuoteFirst As QuickQuoteObject
                If quote.MultiStateQuotes.IsLoaded Then
                    SubQuoteFirst = quote.MultiStateQuotes(0)
                Else
                    SubQuoteFirst = quote
                End If
                OutbuildingLivingQuartersSettings.OtherQualifiers = SubQuoteFirst.ProgramTypeId = "6" OrElse SubQuoteFirst.ProgramTypeId = "7"
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, OutbuildingLivingQuartersSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class



End Namespace
