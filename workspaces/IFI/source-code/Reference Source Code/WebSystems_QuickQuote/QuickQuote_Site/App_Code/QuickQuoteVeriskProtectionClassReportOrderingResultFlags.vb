Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteVeriskProtectionClassReportOrderingResultFlags 'added 10/22/2016

        'added 11/22/2017; not currently needed as-of 11/27/2017
        Public Enum ProtectionClassMismatchType
            None = 0
            Address = 1
            FireHydrantDistanceId = 2
            AddressAndFireHydrantDistanceId = 3
        End Enum

        Public Property RatingSuccess As Boolean = False
        Public Property DiamondLogicDefinitelySetProtClassSysGenIdOnRatingFailure As Boolean = False
        Public Property ManuallyOrderedAnyReports As Boolean = False
        Public Property ManuallyFoundAnyExistingReports As Boolean = False
        Public Property SuccessfullyUpdatedAnyQuickQuoteLocations As Boolean = False
        Public Property SuccessfullySavedUpdatesBackToQuickQuote As Boolean = False
        Public Property ProblemWithDiamondAddressNotMatchingQuickQuote As Boolean = False
        Public Property UpdatedAnyDiamondLocationsFromQuickQuoteBecauseOfMismatch As Boolean = False
        Public Property AttemptedDiamondRate As Boolean = False
        Public Property AttemptedQuickQuoteSave As Boolean = False
        Public Property AttemptedDiamondSave As Boolean = False
        Public Property SuccessfullySavedUpdatesBackToDiamond As Boolean = False
        Public Property AttemptedReportOrderingAndOrCheckingFunctionality As Boolean = False
        Public Property ProblemWithDiamondFireHydrantDistanceIdNotMatchingQuickQuote As Boolean = False 'added 11/22/2017
        'Public Property MismatchType As ProtectionClassMismatchType = ProtectionClassMismatchType.None 'added 11/22/2017; not currently needed as-of 11/27/2017
    End Class
End Namespace
