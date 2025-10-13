Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to governing state) for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState 'added 8/16/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass


        'PolicyLevel
        Private _AdditionalInterestListLinks As List(Of QuickQuoteAdditionalInterestListLink)
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _ClassificationCodes As List(Of QuickQuoteClassificationCode) '8/19/2018 - moved from IndividualState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
        Private _LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
        Private _OptionalCoverages As List(Of QuickQuoteOptionalCoverage) 'Moved from AppliedToAllStates 11/07/2019 bug 33751 MLW
        'moved from AlliedToAllStates 2/5/2019
        Private _ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage)
        Private _UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage

        'added 5/19/2021; moved from QuickQuoteObject
        Private _CopiedAnySourceAIsToTopLevelOnLastCheck As Boolean
        Private _RemovedAnySourceAIsFromTopLevelOnLastCheck As Boolean


        'PolicyLevel
        Public Property AdditionalInterestListLinks As List(Of QuickQuoteAdditionalInterestListLink)
            Get
                Return _AdditionalInterestListLinks
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterestListLink))
                _AdditionalInterestListLinks = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
            End Set
        End Property
        Public Property ClassificationCodes As List(Of QuickQuoteClassificationCode) '8/19/2018 - moved from IndividualState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
            Get
                Return _ClassificationCodes
            End Get
            Set(value As List(Of QuickQuoteClassificationCode))
                _ClassificationCodes = value
            End Set
        End Property
        Public Property LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
            Get
                Return _LossHistoryRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteLossHistoryRecord))
                _LossHistoryRecords = value
            End Set
        End Property
        'Moved from AppliedToAllStates 11/07/2019 bug 33751 MLW
        Public Property OptionalCoverages As List(Of QuickQuoteOptionalCoverage)
            Get
                Return _OptionalCoverages
            End Get
            Set(value As List(Of QuickQuoteOptionalCoverage))
                _OptionalCoverages = value
            End Set
        End Property
        'moved from AlliedToAllStates 2/5/2019
        Public Property ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage)
            Get
                Return _ScheduledPersonalPropertyCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledPersonalPropertyCoverage))
                _ScheduledPersonalPropertyCoverages = value
            End Set
        End Property
        Public Property UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage
            Get
                Return _UnscheduledPersonalPropertyCoverage
            End Get
            Set(value As QuickQuoteUnscheduledPersonalPropertyCoverage)
                _UnscheduledPersonalPropertyCoverage = value
            End Set
        End Property


        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        'Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        'Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        Public Sub New(Parent As Object) 'added 7/27/2018 to replace multiple constructors for different objects
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()


            'PolicyLevel
            _AdditionalInterestListLinks = Nothing
            _AdditionalInterests = Nothing
            _ClassificationCodes = Nothing '8/19/2018 - moved from IndividualState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
            _LossHistoryRecords = Nothing
            _OptionalCoverages = Nothing 'Moved from AppliedToAllStates 11/07/2019 bug 33751 MLW
            'moved from AlliedToAllStates 2/5/2019
            _ScheduledPersonalPropertyCoverages = Nothing
            _UnscheduledPersonalPropertyCoverage = Nothing

            'added 5/19/2021; moved from QuickQuoteObject
            _CopiedAnySourceAIsToTopLevelOnLastCheck = False
            _RemovedAnySourceAIsFromTopLevelOnLastCheck = False
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then

            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 5/19/2021; moved from QuickQuoteObject
        Protected Friend Function CopiedAnySourceAIsToTopLevelOnLastCheck() As Boolean
            Return _CopiedAnySourceAIsToTopLevelOnLastCheck
        End Function
        Protected Friend Function RemovedAnySourceAIsFromTopLevelOnLastCheck() As Boolean
            Return _RemovedAnySourceAIsFromTopLevelOnLastCheck
        End Function
        'new 5/19/2021
        Protected Friend Sub Set_CopiedAnySourceAIsToTopLevelOnLastCheck(ByVal val As Boolean)
            _CopiedAnySourceAIsToTopLevelOnLastCheck = val
        End Sub
        Protected Friend Sub Set_RemovedAnySourceAIsFromTopLevelOnLastCheck(ByVal val As Boolean)
            _RemovedAnySourceAIsFromTopLevelOnLastCheck = val
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).


                    'PolicyLevel
                    If _AdditionalInterestListLinks IsNot Nothing Then
                        If _AdditionalInterestListLinks.Count > 0 Then
                            For Each ll As QuickQuoteAdditionalInterestListLink In _AdditionalInterestListLinks
                                If ll Is Nothing Then
                                    ll.Dispose()
                                    ll = Nothing
                                End If
                            Next
                            _AdditionalInterestListLinks.Clear()
                        End If
                        _AdditionalInterestListLinks = Nothing
                    End If
                    qqHelper.DisposeAdditionalInterests(_AdditionalInterests)
                    If _ClassificationCodes IsNot Nothing Then '8/19/2018 - moved from IndividualState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
                        If _ClassificationCodes.Count > 0 Then
                            For Each c As QuickQuoteClassificationCode In _ClassificationCodes
                                c.Dispose()
                                c = Nothing
                            Next
                            _ClassificationCodes.Clear()
                        End If
                        _ClassificationCodes = Nothing
                    End If
                    If _LossHistoryRecords IsNot Nothing Then
                        If _LossHistoryRecords.Count > 0 Then
                            For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                                lh.Dispose()
                                lh = Nothing
                            Next
                            _LossHistoryRecords.Clear()
                        End If
                        _LossHistoryRecords = Nothing
                    End If
                    'Moved from AppliedToAllStates 11/07/2019 bug 33751 MLW
                    If _OptionalCoverages IsNot Nothing Then
                        If _OptionalCoverages.Count > 0 Then
                            For Each oc As QuickQuoteOptionalCoverage In _OptionalCoverages
                                oc.Dispose()
                                oc = Nothing
                            Next
                            _OptionalCoverages.Clear()
                        End If
                        _OptionalCoverages = Nothing
                    End If
                    'moved from AlliedToAllStates 2/5/2019
                    If _ScheduledPersonalPropertyCoverages IsNot Nothing Then
                        If _ScheduledPersonalPropertyCoverages.Count > 0 Then
                            For Each sp As QuickQuoteScheduledPersonalPropertyCoverage In _ScheduledPersonalPropertyCoverages
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ScheduledPersonalPropertyCoverages.Clear()
                        End If
                        _ScheduledPersonalPropertyCoverages = Nothing
                    End If
                    If _UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                        _UnscheduledPersonalPropertyCoverage.Dispose()
                        _UnscheduledPersonalPropertyCoverage = Nothing
                    End If

                    'added 5/19/2021; moved from QuickQuoteObject
                    _CopiedAnySourceAIsToTopLevelOnLastCheck = Nothing
                    _RemovedAnySourceAIsFromTopLevelOnLastCheck = Nothing

                    MyBase.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace

