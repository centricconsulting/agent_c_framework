Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.MultiState
    Public Class GlClassifications

        Public Shared Function SubQuoteStateIdsWithNoGlClassifications(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As IEnumerable(Of Int32)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If SubQuotes Is Nothing Then
                SubQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            End If
            Dim topQuoteStateIds = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(topQuote, SubQuotes)
            If topQuote.Locations IsNot Nothing AndAlso topQuote.Locations.Any() Then
                Dim statePartsWithGlClassifications = (From sq In SubQuotes Where sq.GLClassifications?.Count > 0 Select sq.StateId.TryToGetInt32())
                Dim locationStateIdsWithGlClassifications = (From l In topQuote.Locations Where l?.Address?.StateId.TryToGetInt32() > 0 And l?.GLClassifications?.Count() > 0 Select l.Address.StateId.TryToGetInt32()).Distinct()
                Return topQuoteStateIds.Except(locationStateIdsWithGlClassifications.Concat(statePartsWithGlClassifications).Distinct())
            End If
            Return topQuoteStateIds
        End Function

    End Class
End Namespace

