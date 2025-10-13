Public Class CommercialQuoteHelper
    Public Shared Function IsCommercialLob(lobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As Boolean
        Select Case lobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation,
                 QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGarage
                ' Commercial Lines
                Return True
            Case Else
                ' Personal Lines & Farm
                Return False
        End Select
    End Function

End Class
