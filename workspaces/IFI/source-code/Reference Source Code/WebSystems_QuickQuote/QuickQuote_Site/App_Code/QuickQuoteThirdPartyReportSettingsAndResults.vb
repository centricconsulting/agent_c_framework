Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteThirdPartyReportSettingsAndResults 'added 4/4/2019

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Enum OrderReportFlag
            No = 0
            Yes = 1
            YesWhenNoReportExists = 2
            YesWhenNoReportExistsOrReportIsOlderThan90Days = 3
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDays = 4
            YesWhenCreditIsOrdered = 5
            YesWhenMvrIsOrdered = 6
            YesWhenClueIsOrdered = 7
            YesWhenCreditOrMvrIsOrdered = 8
            YesWhenCreditOrClueIsOrdered = 9
            YesWhenMvrOrClueIsOrdered = 10
            YesWhenCreditMvrOrClueIsOrdered = 11
            YesWhenNoReportExistsOrCreditIsOrdered = 12
            YesWhenNoReportExistsOrMvrIsOrdered = 13
            YesWhenNoReportExistsOrClueIsOrdered = 14
            YesWhenNoReportExistsOrCreditOrMvrIsOrdered = 15
            YesWhenNoReportExistsOrCreditOrClueIsOrdered = 16
            YesWhenNoReportExistsOrMvrOrClueIsOrdered = 17
            YesWhenNoReportExistsOrCreditMvrOrClueIsOrdered = 18
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrCreditIsOrdered = 19
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrMvrIsOrdered = 20
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrClueIsOrdered = 21
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrCreditOrMvrIsOrdered = 22
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrCreditOrClueIsOrdered = 23
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrMvrOrClueIsOrdered = 24
            YesWhenNoReportExistsOrReportIsOlderThan90DaysOrCreditMvrOrClueIsOrdered = 25
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrCreditIsOrdered = 26
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrMvrIsOrdered = 27
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrClueIsOrdered = 28
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrCreditOrMvrIsOrdered = 29
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrCreditOrClueIsOrdered = 30
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrMvrOrClueIsOrdered = 31
            YesWhenNoReportExistsOrReportIsOlderThanSpecifiedNumberOfDaysOrCreditMvrOrClueIsOrdered = 32
        End Enum
        Public Enum ReloadObjectAfterReportsType
            NoReload = 0
            ReloadAfterEachReport = 1
            ReloadAtEndAfterAllReports = 2
            ReloadAfterCreditOnly = 3
            ReloadAfterMvrOnly = 4
            ReloadAfterClueOnly = 5
            ReloadAfterCreditOrMvr = 6
            ReloadAfterCreditOrClue = 7
            ReloadAfterMvrOrClue = 8
            ReloadAfterCreditMvrOrClue = 9
            ReloadAtEndWhenCreditIsOrdered = 10
            ReloadAtEndWhenMvrIsOrdered = 11
            ReloadAtEndWhenClueIsOrdered = 12
            ReloadAtEndWhenCreditOrMvrIsOrdered = 13
            ReloadAtEndWhenCreditOrClueIsOrdered = 14
            ReloadAtEndWhenMvrOrClueIsOrdered = 15
            ReloadAtEndWhenCreditMvrOrClueIsOrdered = 16
        End Enum

        Public Property OrderCreditReports As OrderReportFlag = OrderReportFlag.No
        Public Property OrderMvrReports As OrderReportFlag = OrderReportFlag.No
        Public Property OrderClueReport As OrderReportFlag = OrderReportFlag.No
        Public Property SpecifiedNumberOfDaysForAnyReport As Integer = 0
        Public Property SpecifiedNumberOfDaysForCredit As Integer = 0
        Public Property SpecifiedNumberOfDaysForMvr As Integer = 0
        Public Property SpecifiedNumberOfDaysForClue As Integer = 0
        Public Property ReloadDiamondImageType As ReloadObjectAfterReportsType = ReloadObjectAfterReportsType.NoReload
        Public Property ReloadQuickQuoteObjectType As ReloadObjectAfterReportsType = ReloadObjectAfterReportsType.NoReload

        Private _OverallResults As QuickQuoteThirdPartyReportResults = New QuickQuoteThirdPartyReportResults
        Public ReadOnly Property OverallResults As QuickQuoteThirdPartyReportResults
            Get
                If _OverallResults Is Nothing Then
                    _OverallResults = New QuickQuoteThirdPartyReportResults
                End If
                Return _OverallResults
            End Get
        End Property
        Private _CreditResults As QuickQuoteThirdPartyReportResults = New QuickQuoteThirdPartyReportResults
        Public ReadOnly Property CreditResults As QuickQuoteThirdPartyReportResults
            Get
                If _CreditResults Is Nothing Then
                    _CreditResults = New QuickQuoteThirdPartyReportResults
                End If
                Return _CreditResults
            End Get
        End Property
        Private _MvrResults As QuickQuoteThirdPartyReportResults = New QuickQuoteThirdPartyReportResults
        Public ReadOnly Property MvrResults As QuickQuoteThirdPartyReportResults
            Get
                If _MvrResults Is Nothing Then
                    _MvrResults = New QuickQuoteThirdPartyReportResults
                End If
                Return _MvrResults
            End Get
        End Property
        Private _ClueResults As QuickQuoteThirdPartyReportResults = New QuickQuoteThirdPartyReportResults
        Public ReadOnly Property ClueResults As QuickQuoteThirdPartyReportResults
            Get
                If _ClueResults Is Nothing Then
                    _ClueResults = New QuickQuoteThirdPartyReportResults
                End If
                Return _ClueResults
            End Get
        End Property

        'added 6/9/2020
        Private _CreditSettings As QuickQuoteThirdPartyReportCreditSettings = New QuickQuoteThirdPartyReportCreditSettings
        Public Property CreditSettings As QuickQuoteThirdPartyReportCreditSettings
            Get
                If _CreditSettings Is Nothing Then
                    _CreditSettings = New QuickQuoteThirdPartyReportCreditSettings
                End If
                Return _CreditSettings
            End Get
            Set(value As QuickQuoteThirdPartyReportCreditSettings)
                _CreditSettings = value
            End Set
        End Property
        Private _MvrSettings As QuickQuoteThirdPartyReportMvrSettings = New QuickQuoteThirdPartyReportMvrSettings
        Public Property MvrSettings As QuickQuoteThirdPartyReportMvrSettings
            Get
                If _MvrSettings Is Nothing Then
                    _MvrSettings = New QuickQuoteThirdPartyReportMvrSettings
                End If
                Return _MvrSettings
            End Get
            Set(value As QuickQuoteThirdPartyReportMvrSettings)
                _MvrSettings = value
            End Set
        End Property
        Private _ClueSettings As QuickQuoteThirdPartyReportClueSettings = New QuickQuoteThirdPartyReportClueSettings
        Public Property ClueSettings As QuickQuoteThirdPartyReportClueSettings
            Get
                If _ClueSettings Is Nothing Then
                    _ClueSettings = New QuickQuoteThirdPartyReportClueSettings
                End If
                Return _ClueSettings
            End Get
            Set(value As QuickQuoteThirdPartyReportClueSettings)
                _ClueSettings = value
            End Set
        End Property

        Public Sub New(Optional ByVal transactionType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal saveType As CommonMethods.QuickQuoteXML.QuickQuoteSaveType = Nothing)
            _OrderCreditReports = OrderReportFlag.No
            _OrderMvrReports = OrderReportFlag.No
            _OrderClueReport = OrderReportFlag.No
            _SpecifiedNumberOfDaysForAnyReport = 0
            _SpecifiedNumberOfDaysForCredit = 0
            _SpecifiedNumberOfDaysForMvr = 0
            _SpecifiedNumberOfDaysForClue = 0
            _ReloadDiamondImageType = ReloadObjectAfterReportsType.NoReload
            _ReloadQuickQuoteObjectType = ReloadObjectAfterReportsType.NoReload
            _OverallResults = New QuickQuoteThirdPartyReportResults
            _CreditResults = New QuickQuoteThirdPartyReportResults
            _MvrResults = New QuickQuoteThirdPartyReportResults
            _ClueResults = New QuickQuoteThirdPartyReportResults

            'added 6/9/2020
            _CreditSettings = New QuickQuoteThirdPartyReportCreditSettings
            _MvrSettings = New QuickQuoteThirdPartyReportMvrSettings
            _ClueSettings = New QuickQuoteThirdPartyReportClueSettings

            If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteTransactionType), transactionType) = True AndAlso transactionType <> QuickQuoteObject.QuickQuoteTransactionType.None Then
                Select Case transactionType
                    Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        _OrderCreditReports = OrderReportFlag.YesWhenNoReportExists
                        _OrderMvrReports = OrderReportFlag.YesWhenNoReportExists
                        _OrderClueReport = OrderReportFlag.YesWhenMvrIsOrdered
                        _ReloadDiamondImageType = ReloadObjectAfterReportsType.ReloadAtEndWhenMvrOrClueIsOrdered
                    Case QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
                        If System.Enum.IsDefined(GetType(CommonMethods.QuickQuoteXML.QuickQuoteSaveType), saveType) = True Then
                            Select Case saveType
                                Case CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap
                                    _OrderMvrReports = OrderReportFlag.YesWhenNoReportExistsOrReportIsOlderThan90Days
                                    _OrderClueReport = OrderReportFlag.YesWhenNoReportExistsOrReportIsOlderThan90Days
                                    _ReloadDiamondImageType = ReloadObjectAfterReportsType.ReloadAtEndWhenMvrOrClueIsOrdered
                                Case CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote
                                    _OrderCreditReports = OrderReportFlag.YesWhenNoReportExistsOrReportIsOlderThan90Days
                            End Select
                        End If
                End Select
            End If
        End Sub

        Protected Friend Sub Set_OverallResults(ByVal results As QuickQuoteThirdPartyReportResults)
            _OverallResults = results
        End Sub
        Protected Friend Sub Set_CreditResults(ByVal results As QuickQuoteThirdPartyReportResults)
            _CreditResults = results
        End Sub
        Protected Friend Sub Set_MvrResults(ByVal results As QuickQuoteThirdPartyReportResults)
            _MvrResults = results
        End Sub
        Protected Friend Sub Set_ClueResults(ByVal results As QuickQuoteThirdPartyReportResults)
            _ClueResults = results
        End Sub

        Protected Friend Sub CombineReportSpecificResultsInOverallResults()
            If _OverallResults Is Nothing Then
                _OverallResults = New QuickQuoteThirdPartyReportResults
            End If

            If _CreditResults IsNot Nothing Then
                'With _CreditResults
                '    If .SuccessfullyOrderedAnyReports = True Then
                '        _OverallResults.Set_SuccessfullyOrderedAnyReports(True)
                '    End If
                '    If String.IsNullOrWhiteSpace(.ErrorMessage) = False Then
                '        _OverallResults.Set_ErrorMessage(qqHelper.appendText(_OverallResults.ErrorMessage, .ErrorMessage, splitter:="; "))
                '    End If
                '    If .CaughtUnhandledExceptionOnAnyServiceCalls = True Then
                '        _OverallResults.Set_CaughtUnhandledExceptionOnAnyServiceCalls(True)
                '    End If
                '    If String.IsNullOrWhiteSpace(.UnhandledExceptionErrorMessage) = False Then
                '        _OverallResults.Set_UnhandledExceptionErrorMessage(qqHelper.appendText(_OverallResults.UnhandledExceptionErrorMessage, .UnhandledExceptionErrorMessage, splitter:="; "))
                '    End If
                '    If .LookedForExistingReports = True Then
                '        _OverallResults.Set_LookedForExistingReports(True)
                '    End If
                '    If .FoundAnyExistingReports = True Then
                '        _OverallResults.Set_FoundAnyExistingReports(True)
                '    End If
                '    If .NumberOfExistingReportsFound > 0 Then
                '        _OverallResults.Set_NumberOfExistingReportsFound(_OverallResults.NumberOfExistingReportsFound + .NumberOfExistingReportsFound)
                '    End If
                '    If .NumberOfNewReportOrdersAttempted > 0 Then
                '        _OverallResults.Set_NumberOfNewReportOrdersAttempted(_OverallResults.NumberOfNewReportOrdersAttempted + .NumberOfNewReportOrdersAttempted)
                '    End If
                '    If .NumberOfReportsNotValidatedDueToDatabaseError > 0 Then
                '        _OverallResults.Set_NumberOfReportsNotValidatedDueToDatabaseError(_OverallResults.NumberOfReportsNotValidatedDueToDatabaseError + .NumberOfReportsNotValidatedDueToDatabaseError)
                '    End If
                '    If .NumberOfNewReportOrdersSuccessful > 0 Then
                '        _OverallResults.Set_NumberOfNewReportOrdersSuccessful(_OverallResults.NumberOfNewReportOrdersSuccessful + .NumberOfNewReportOrdersSuccessful)
                '    End If
                '    If .ReloadedDiamondImage = True Then
                '        _OverallResults.Set_ReloadedDiamondImage(True)
                '    End If
                '    If .ReloadedQuickQuoteObject = True Then
                '        _OverallResults.Set_ReloadedQuickQuoteObject(True)
                '    End If
                'End With
                'updated 6/9/2020 to call helper method
                CombineSpecificResultsInOverallResults(_CreditResults)
            End If
            If _MvrResults IsNot Nothing Then
                'With _MvrResults

                'End With
                'updated 6/9/2020 to call helper method
                CombineSpecificResultsInOverallResults(_MvrResults)
            End If
            If _ClueResults IsNot Nothing Then
                'With _ClueResults

                'End With
                'updated 6/9/2020 to call helper method
                CombineSpecificResultsInOverallResults(_ClueResults)
            End If
        End Sub
        'added 6/9/2020 to handle all results w/ one method; must've either forgotten to do this before or it was lost somehow
        Private Sub CombineSpecificResultsInOverallResults(ByVal results As QuickQuoteThirdPartyReportResults)
            If _OverallResults Is Nothing Then
                _OverallResults = New QuickQuoteThirdPartyReportResults
            End If

            If results IsNot Nothing Then
                With results
                    If .SuccessfullyOrderedAnyReports = True Then
                        _OverallResults.Set_SuccessfullyOrderedAnyReports(True)
                    End If
                    If String.IsNullOrWhiteSpace(.ErrorMessage) = False Then
                        _OverallResults.Set_ErrorMessage(qqHelper.appendText(_OverallResults.ErrorMessage, .ErrorMessage, splitter:="; "))
                    End If
                    If .CaughtUnhandledExceptionOnAnyServiceCalls = True Then
                        _OverallResults.Set_CaughtUnhandledExceptionOnAnyServiceCalls(True)
                    End If
                    If String.IsNullOrWhiteSpace(.UnhandledExceptionErrorMessage) = False Then
                        _OverallResults.Set_UnhandledExceptionErrorMessage(qqHelper.appendText(_OverallResults.UnhandledExceptionErrorMessage, .UnhandledExceptionErrorMessage, splitter:="; "))
                    End If
                    If .LookedForExistingReports = True Then
                        _OverallResults.Set_LookedForExistingReports(True)
                    End If
                    If .FoundAnyExistingReports = True Then
                        _OverallResults.Set_FoundAnyExistingReports(True)
                    End If
                    If .NumberOfExistingReportsFound > 0 Then
                        _OverallResults.Set_NumberOfExistingReportsFound(_OverallResults.NumberOfExistingReportsFound + .NumberOfExistingReportsFound)
                    End If
                    If .NumberOfNewReportOrdersAttempted > 0 Then
                        _OverallResults.Set_NumberOfNewReportOrdersAttempted(_OverallResults.NumberOfNewReportOrdersAttempted + .NumberOfNewReportOrdersAttempted)
                    End If
                    If .NumberOfReportsNotValidatedDueToDatabaseError > 0 Then
                        _OverallResults.Set_NumberOfReportsNotValidatedDueToDatabaseError(_OverallResults.NumberOfReportsNotValidatedDueToDatabaseError + .NumberOfReportsNotValidatedDueToDatabaseError)
                    End If
                    If .NumberOfNewReportOrdersSuccessful > 0 Then
                        _OverallResults.Set_NumberOfNewReportOrdersSuccessful(_OverallResults.NumberOfNewReportOrdersSuccessful + .NumberOfNewReportOrdersSuccessful)
                    End If
                    If .ReloadedDiamondImage = True Then
                        _OverallResults.Set_ReloadedDiamondImage(True)
                    End If
                    If .ReloadedQuickQuoteObject = True Then
                        _OverallResults.Set_ReloadedQuickQuoteObject(True)
                    End If

                    'added 6/9/2020
                    If .LookedForNonReportSourceLossesOrViolations = True Then
                        _OverallResults.Set_LookedForNonReportSourceLossesOrViolations(True)
                    End If
                    If .FoundAnyNonReportSourceLossesOrViolations = True Then
                        _OverallResults.Set_FoundAnyNonReportSourceLossesOrViolations(True)
                    End If
                    If .NumberOfNonReportSourceLossesOrViolationsFound > 0 Then
                        _OverallResults.Set_NumberOfNonReportSourceLossesOrViolationsFound(_OverallResults.NumberOfNonReportSourceLossesOrViolationsFound + .NumberOfNonReportSourceLossesOrViolationsFound)
                    End If
                    If .NumberOfNonReportSourceLossesOrViolationsRemoved > 0 Then
                        _OverallResults.Set_NumberOfNonReportSourceLossesOrViolationsRemoved(_OverallResults.NumberOfNonReportSourceLossesOrViolationsRemoved + .NumberOfNonReportSourceLossesOrViolationsRemoved)
                    End If
                End With
            End If
        End Sub

    End Class

    'added 6/9/2020
    <Serializable()>
    Public Class QuickQuoteThirdPartyReportCreditSettings

    End Class
    <Serializable()>
    Public Class QuickQuoteThirdPartyReportMvrSettings
        Public Property RemoveNonReportSourceViolationsAfterSuccessfulOrder As Boolean = False
        Public Property ReplaceViolationsOnQuickQuoteObjectAfterUpdate As Boolean = False
    End Class
    <Serializable()>
    Public Class QuickQuoteThirdPartyReportClueSettings
        Public Property RemoveNonReportSourceLossesAfterSuccessfulOrder As Boolean = False
        Public Property AssignUnassignedLossesToPH1DriverOrApplicant As Boolean = False
        Public Property ReplaceLossHistoriesOnQuickQuoteObjectAfterUpdate As Boolean = False
    End Class
End Namespace
