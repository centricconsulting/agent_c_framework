Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteVeriskProtectionClassReportOrderingSettings 'added 10/22/2016
        Public Property UpdateDiamondLocationsFromQuickQuoteOnMismatch As Boolean = False
        Public Property ForceReportOrderingAndOrCheckingFunctionalityIfValidationPasses As Boolean = False
    End Class
End Namespace
