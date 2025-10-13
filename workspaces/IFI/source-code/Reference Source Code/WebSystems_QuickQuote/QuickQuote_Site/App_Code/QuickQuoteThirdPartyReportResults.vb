Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteThirdPartyReportResults 'added 4/4/2019
        Private _SuccessfullyOrderedAnyReports As Boolean = False
        Public ReadOnly Property SuccessfullyOrderedAnyReports As Boolean
            Get
                Return _SuccessfullyOrderedAnyReports
            End Get
        End Property
        Private _ErrorMessage As String = ""
        Public ReadOnly Property ErrorMessage As String
            Get
                Return _ErrorMessage
            End Get
        End Property
        Private _CaughtUnhandledExceptionOnAnyServiceCalls As Boolean = False
        Public ReadOnly Property CaughtUnhandledExceptionOnAnyServiceCalls As Boolean
            Get
                Return _CaughtUnhandledExceptionOnAnyServiceCalls
            End Get
        End Property
        Private _UnhandledExceptionErrorMessage As String = ""
        Public ReadOnly Property UnhandledExceptionErrorMessage As String
            Get
                Return _UnhandledExceptionErrorMessage
            End Get
        End Property
        Private _LookedForExistingReports As Boolean = False
        Public ReadOnly Property LookedForExistingReports As Boolean
            Get
                Return _LookedForExistingReports
            End Get
        End Property
        Private _FoundAnyExistingReports As Boolean = False
        Public ReadOnly Property FoundAnyExistingReports As Boolean
            Get
                Return _FoundAnyExistingReports
            End Get
        End Property
        Private _NumberOfExistingReportsFound As Integer = 0
        Public ReadOnly Property NumberOfExistingReportsFound As Integer
            Get
                Return _NumberOfExistingReportsFound
            End Get
        End Property
        Private _FoundAnyValidExistingReports As Boolean = False 'added 5/23/2019
        Public ReadOnly Property FoundAnyValidExistingReports As Boolean
            Get
                Return _FoundAnyValidExistingReports
            End Get
        End Property
        Private _NumberOfValidExistingReportsFound As Integer = 0
        Public ReadOnly Property NumberOfValidExistingReportsFound As Integer
            Get
                Return _NumberOfValidExistingReportsFound
            End Get
        End Property
        Private _FoundAnyInvalidExistingReports As Boolean = False 'added 5/23/2019
        Public ReadOnly Property FoundAnyInvalidExistingReports As Boolean
            Get
                Return _FoundAnyInvalidExistingReports
            End Get
        End Property
        Private _NumberOfInvalidExistingReportsFound As Integer = 0
        Public ReadOnly Property NumberOfInvalidExistingReportsFound As Integer
            Get
                Return _NumberOfInvalidExistingReportsFound
            End Get
        End Property
        Private _NumberOfNewReportOrdersAttempted As Integer = 0
        Public ReadOnly Property NumberOfNewReportOrdersAttempted As Integer
            Get
                Return _NumberOfNewReportOrdersAttempted
            End Get
        End Property
        Private _NumberOfReportsNotValidatedDueToDatabaseError As Integer = 0
        Public ReadOnly Property NumberOfReportsNotValidatedDueToDatabaseError As Integer
            Get
                Return _NumberOfReportsNotValidatedDueToDatabaseError
            End Get
        End Property
        Private _NumberOfNewReportOrdersSuccessful As Integer = 0
        Public ReadOnly Property NumberOfNewReportOrdersSuccessful As Integer
            Get
                Return _NumberOfNewReportOrdersSuccessful
            End Get
        End Property
        Private _ReloadedDiamondImage As Boolean = False
        Public ReadOnly Property ReloadedDiamondImage As Boolean
            Get
                Return _ReloadedDiamondImage
            End Get
        End Property
        Private _ReloadedQuickQuoteObject As Boolean = False
        Public ReadOnly Property ReloadedQuickQuoteObject As Boolean
            Get
                Return _ReloadedQuickQuoteObject
            End Get
        End Property

        'added 6/9/2020
        Private _LookedForNonReportSourceLossesOrViolations As Boolean = False
        Public ReadOnly Property LookedForNonReportSourceLossesOrViolations As Boolean
            Get
                Return _LookedForNonReportSourceLossesOrViolations
            End Get
        End Property
        Private _FoundAnyNonReportSourceLossesOrViolations As Boolean = False
        Public ReadOnly Property FoundAnyNonReportSourceLossesOrViolations As Boolean
            Get
                Return _FoundAnyNonReportSourceLossesOrViolations
            End Get
        End Property
        Private _NumberOfNonReportSourceLossesOrViolationsFound As Integer = 0
        Public ReadOnly Property NumberOfNonReportSourceLossesOrViolationsFound As Integer
            Get
                Return _NumberOfNonReportSourceLossesOrViolationsFound
            End Get
        End Property
        Private _NumberOfNonReportSourceLossesOrViolationsRemoved As Integer = 0
        Public ReadOnly Property NumberOfNonReportSourceLossesOrViolationsRemoved As Integer
            Get
                Return _NumberOfNonReportSourceLossesOrViolationsRemoved
            End Get
        End Property

        Public Sub New()
            _SuccessfullyOrderedAnyReports = False
            _ErrorMessage = ""
            _CaughtUnhandledExceptionOnAnyServiceCalls = False
            _UnhandledExceptionErrorMessage = ""
            _FoundAnyExistingReports = False
            _NumberOfExistingReportsFound = 0
            _FoundAnyValidExistingReports = False
            _NumberOfValidExistingReportsFound = 0
            _FoundAnyInvalidExistingReports = False
            _NumberOfInvalidExistingReportsFound = 0
            _NumberOfNewReportOrdersAttempted = 0
            _NumberOfReportsNotValidatedDueToDatabaseError = 0
            _NumberOfNewReportOrdersSuccessful = 0
            _ReloadedDiamondImage = False
            _ReloadedQuickQuoteObject = False

            'added 6/9/2020
            _LookedForNonReportSourceLossesOrViolations = False
            _FoundAnyNonReportSourceLossesOrViolations = False
            _NumberOfNonReportSourceLossesOrViolationsFound = 0
            _NumberOfNonReportSourceLossesOrViolationsRemoved = 0
        End Sub

        Protected Friend Sub Set_SuccessfullyOrderedAnyReports(ByVal success As Boolean)
            _SuccessfullyOrderedAnyReports = success
        End Sub
        Protected Friend Sub Set_ErrorMessage(ByVal errMsg As String)
            _ErrorMessage = errMsg
        End Sub
        Protected Friend Sub Set_CaughtUnhandledExceptionOnAnyServiceCalls(ByVal caughtError As Boolean)
            _CaughtUnhandledExceptionOnAnyServiceCalls = caughtError
        End Sub
        Protected Friend Sub Set_UnhandledExceptionErrorMessage(ByVal errMsg As String)
            _UnhandledExceptionErrorMessage = errMsg
        End Sub
        Protected Friend Sub Set_LookedForExistingReports(ByVal lookedForExisting As Boolean)
            _LookedForExistingReports = lookedForExisting
        End Sub
        Protected Friend Sub Set_FoundAnyExistingReports(ByVal foundExisting As Boolean)
            _FoundAnyExistingReports = foundExisting
        End Sub
        Protected Friend Sub Set_NumberOfExistingReportsFound(ByVal existingReportsNum As Integer)
            _NumberOfExistingReportsFound = existingReportsNum
        End Sub
        Protected Friend Sub Set_FoundAnyValidExistingReports(ByVal foundExisting As Boolean)
            _FoundAnyValidExistingReports = foundExisting
        End Sub
        Protected Friend Sub Set_NumberOfValidExistingReportsFound(ByVal existingReportsNum As Integer)
            _NumberOfValidExistingReportsFound = existingReportsNum
        End Sub
        Protected Friend Sub Set_FoundAnyInvalidExistingReports(ByVal foundExisting As Boolean)
            _FoundAnyInvalidExistingReports = foundExisting
        End Sub
        Protected Friend Sub Set_NumberOfInvalidExistingReportsFound(ByVal existingReportsNum As Integer)
            _NumberOfInvalidExistingReportsFound = existingReportsNum
        End Sub
        Protected Friend Sub Set_NumberOfNewReportOrdersAttempted(ByVal reportOrdersAttemptedNum As Integer)
            _NumberOfNewReportOrdersAttempted = reportOrdersAttemptedNum
        End Sub
        Protected Friend Sub Set_NumberOfReportsNotValidatedDueToDatabaseError(ByVal reportsNotValidatedNum As Integer)
            _NumberOfReportsNotValidatedDueToDatabaseError = reportsNotValidatedNum
        End Sub
        Protected Friend Sub Set_NumberOfNewReportOrdersSuccessful(ByVal reportOrdersSuccessfulNum As Integer)
            _NumberOfNewReportOrdersSuccessful = reportOrdersSuccessfulNum
        End Sub
        Protected Friend Sub Increment_NumberOfExistingReportsFound(ByVal existingReportsNumToAdd As Integer)
            _NumberOfExistingReportsFound += existingReportsNumToAdd
        End Sub
        Protected Friend Sub Increment_NumberOfValidExistingReportsFound(ByVal existingReportsNumToAdd As Integer)
            _NumberOfValidExistingReportsFound += existingReportsNumToAdd
        End Sub
        Protected Friend Sub Increment_NumberOfInvalidExistingReportsFound(ByVal existingReportsNumToAdd As Integer)
            _NumberOfInvalidExistingReportsFound += existingReportsNumToAdd
        End Sub
        Protected Friend Sub Increment_NumberOfNewReportOrdersAttempted(ByVal reportOrdersAttemptedNumToAdd As Integer)
            _NumberOfNewReportOrdersAttempted += reportOrdersAttemptedNumToAdd
        End Sub
        Protected Friend Sub Increment_NumberOfReportsNotValidatedDueToDatabaseError(ByVal reportsNotValidatedNumToAdd As Integer)
            _NumberOfReportsNotValidatedDueToDatabaseError += reportsNotValidatedNumToAdd
        End Sub
        Protected Friend Sub Increment_NumberOfNewReportOrdersSuccessful(ByVal reportOrdersSuccessfulNumToAdd As Integer)
            _NumberOfNewReportOrdersSuccessful += reportOrdersSuccessfulNumToAdd
        End Sub
        Protected Friend Sub Set_ReloadedDiamondImage(ByVal reloadedDiaImg As Boolean)
            _ReloadedDiamondImage = reloadedDiaImg
        End Sub
        Protected Friend Sub Set_ReloadedQuickQuoteObject(ByVal reloadedQQO As Boolean)
            _ReloadedQuickQuoteObject = reloadedQQO
        End Sub

        'added 6/9/2020
        Protected Friend Sub Set_LookedForNonReportSourceLossesOrViolations(ByVal lookedFor As Boolean)
            _LookedForNonReportSourceLossesOrViolations = lookedFor
        End Sub
        Protected Friend Sub Set_FoundAnyNonReportSourceLossesOrViolations(ByVal found As Boolean)
            _FoundAnyNonReportSourceLossesOrViolations = found
        End Sub
        Protected Friend Sub Set_NumberOfNonReportSourceLossesOrViolationsFound(ByVal num As Integer)
            _NumberOfNonReportSourceLossesOrViolationsFound = num
        End Sub
        Protected Friend Sub Increment_NumberOfNonReportSourceLossesOrViolationsFound(ByVal numToAdd As Integer)
            _NumberOfNonReportSourceLossesOrViolationsFound += numToAdd
        End Sub
        Protected Friend Sub Set_NumberOfNonReportSourceLossesOrViolationsRemoved(ByVal num As Integer)
            _NumberOfNonReportSourceLossesOrViolationsRemoved = num
        End Sub
        Protected Friend Sub Increment_NumberOfNonReportSourceLossesOrViolationsRemoved(ByVal numToAdd As Integer)
            _NumberOfNonReportSourceLossesOrViolationsRemoved += numToAdd
        End Sub
    End Class
End Namespace
