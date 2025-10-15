Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store loss history information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteLossHistoryRecord
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        '*8/4/2012
        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Amount As String
        Private _Catastrophic As Boolean
        Private _CatastrophicCode As String
        Private _ClaimControlId As String
        Private _ClaimNumber As String
        Private _Comments As String
        Private _LossDate As String
        Private _LossDescription As String

        Private _LossHistoryDetailRecords As Generic.List(Of QuickQuoteLossHistoryDetailRecord)

        Private _LossHistoryFaultId As String
        Private _LossHistoryLocationTypeId As String
        Private _LossHistorySourceId As String
        Private _LossHistorySurchargeId As String
        Private _ReserveAmount As String
        Private _TypeOfLossId As String
        Private _UninsuredUnderinsured As Boolean
        'Private _UnitNum As String '2/5/2015 note: w/ 527.900.200-44, UnitNum is now an IdValue object instead of Integer; removed 2/5/2015 like we did for AccidentViolation.UnitNum
        Private _WeatherRelated As Boolean

        Private _LossHistoryNum As String 'added 4/23/2014 for reconciliation
        Private _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation As Boolean 'added 7/3/2014

        'added 7/7/2014 to make sure we're capturing everything that could come back in 3rd party reports
        Private _ExternalCode As String
        Private _ExternalId As String
        Private _FirstAddedDate As String 'Date
        Private _GuaranteedRatePeriodEffectiveDate As String 'Date
        Private _IsPossibleRelatedClaim As Boolean
        'Private _LossHistoryDisplayNum As String
        'Private _LossHistoryStatusTypeId As String 'Byte (?)
        Private _ManualOverride As Boolean
        'Private _PackagePartNum As String
        'Private _PackageUnitNum As String 'IdValue
        Private _TypeOfLoss As String

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 7/24/2019
        Private _AddedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property Amount As String
            Get
                Return _Amount
            End Get
            Set(value As String)
                _Amount = value
                '7/2/2014 note: may update to use qqHelper.ConvertToQuotedPremiumFormat method at some point
            End Set
        End Property
        Public Property Catastrophic As Boolean
            Get
                Return _Catastrophic
            End Get
            Set(value As Boolean)
                _Catastrophic = value
            End Set
        End Property
        Public Property CatastrophicCode As String
            Get
                Return _CatastrophicCode
            End Get
            Set(value As String)
                _CatastrophicCode = value
            End Set
        End Property
        Public Property ClaimControlId As String
            Get
                Return _ClaimControlId
            End Get
            Set(value As String)
                _ClaimControlId = value
            End Set
        End Property
        Public Property ClaimNumber As String
            Get
                Return _ClaimNumber
            End Get
            Set(value As String)
                _ClaimNumber = value
            End Set
        End Property
        Public Property Comments As String
            Get
                Return _Comments
            End Get
            Set(value As String)
                _Comments = value
            End Set
        End Property
        Public Property LossDate As String
            Get
                Return _LossDate
            End Get
            Set(value As String)
                _LossDate = value
                qqHelper.ConvertToShortDate(_LossDate)
            End Set
        End Property
        Public Property LossDescription As String
            Get
                Return _LossDescription
            End Get
            Set(value As String)
                _LossDescription = value
            End Set
        End Property

        Public Property LossHistoryDetailRecords As Generic.List(Of QuickQuoteLossHistoryDetailRecord)
            Get
                SetParentOfListItems(_LossHistoryDetailRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05309}")
                Return _LossHistoryDetailRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteLossHistoryDetailRecord))
                _LossHistoryDetailRecords = value
                SetParentOfListItems(_LossHistoryDetailRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05309}")
            End Set
        End Property

        Public Property LossHistoryFaultId As String
            Get
                Return _LossHistoryFaultId
            End Get
            Set(value As String)
                _LossHistoryFaultId = value
            End Set
        End Property
        Public Property LossHistoryLocationTypeId As String
            Get
                Return _LossHistoryLocationTypeId
            End Get
            Set(value As String)
                _LossHistoryLocationTypeId = value
            End Set
        End Property
        Public Property LossHistorySourceId As String
            Get
                Return _LossHistorySourceId
            End Get
            Set(value As String)
                _LossHistorySourceId = value
            End Set
        End Property
        Public Property LossHistorySurchargeId As String
            Get
                Return _LossHistorySurchargeId
            End Get
            Set(value As String)
                _LossHistorySurchargeId = value
            End Set
        End Property
        Public Property ReserveAmount As String
            Get
                Return _ReserveAmount
            End Get
            Set(value As String)
                _ReserveAmount = value
            End Set
        End Property
        Public Property TypeOfLossId As String
            Get
                Return _TypeOfLossId
            End Get
            Set(value As String)
                _TypeOfLossId = value
            End Set
        End Property
        Public Property UninsuredUnderinsured As Boolean
            Get
                Return _UninsuredUnderinsured
            End Get
            Set(value As Boolean)
                _UninsuredUnderinsured = value
            End Set
        End Property
        'Public Property UnitNum As String 'removed 2/5/2015 like we did for AccidentViolation.UnitNum
        '    Get
        '        Return _UnitNum
        '    End Get
        '    Set(value As String)
        '        _UnitNum = value
        '    End Set
        'End Property
        Public Property WeatherRelated As Boolean
            Get
                Return _WeatherRelated
            End Get
            Set(value As Boolean)
                _WeatherRelated = value
            End Set
        End Property

        Public Property LossHistoryNum As String 'added 4/23/2014 for reconciliation
            Get
                Return _LossHistoryNum
            End Get
            Set(value As String)
                _LossHistoryNum = value
            End Set
        End Property
        Public Property CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation As Boolean 'added 7/3/2014
            Get
                Return _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation = value
            End Set
        End Property

        'added 7/7/2014
        Public Property ExternalCode As String
            Get
                Return _ExternalCode
            End Get
            Set(value As String)
                _ExternalCode = value
            End Set
        End Property
        Public Property ExternalId As String
            Get
                Return _ExternalId
            End Get
            Set(value As String)
                _ExternalId = value
            End Set
        End Property
        Public Property FirstAddedDate As String 'Date
            Get
                Return _FirstAddedDate
            End Get
            Set(value As String)
                _FirstAddedDate = value
                qqHelper.ConvertToShortDate(_FirstAddedDate)
            End Set
        End Property
        Public Property GuaranteedRatePeriodEffectiveDate As String 'Date
            Get
                Return _GuaranteedRatePeriodEffectiveDate
            End Get
            Set(value As String)
                _GuaranteedRatePeriodEffectiveDate = value
                qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
            End Set
        End Property
        Public Property IsPossibleRelatedClaim As Boolean
            Get
                Return _IsPossibleRelatedClaim
            End Get
            Set(value As Boolean)
                _IsPossibleRelatedClaim = value
            End Set
        End Property
        'Public Property LossHistoryDisplayNum As String
        'Public Property LossHistoryStatusTypeId As String 'Byte (?)
        Public Property ManualOverride As Boolean
            Get
                Return _ManualOverride
            End Get
            Set(value As Boolean)
                _ManualOverride = value
            End Set
        End Property
        'Public Property PackagePartNum As String
        'Public Property PackageUnitNum As String 'IdValue
        Public Property TypeOfLoss As String
            Get
                Return _TypeOfLoss
            End Get
            Set(value As String)
                _TypeOfLoss = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        'added 7/24/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
        Public Property AddedImageNum As String
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _Amount = ""
            _Catastrophic = False
            _CatastrophicCode = ""
            _ClaimControlId = ""
            _ClaimNumber = ""
            _Comments = ""
            _LossDate = ""
            _LossDescription = ""

            '_LossHistoryDetailRecords = new Generic.List(Of QuickQuoteLossHistoryDetailRecord)
            _LossHistoryDetailRecords = Nothing 'added 8/4/2014

            _LossHistoryFaultId = ""
            _LossHistoryLocationTypeId = ""
            _LossHistorySourceId = ""
            _LossHistorySurchargeId = ""
            _ReserveAmount = ""
            _TypeOfLossId = ""
            _UninsuredUnderinsured = False
            '_UnitNum = "" 'removed 2/5/2015 like we did for AccidentViolation.UnitNum
            _WeatherRelated = False

            _LossHistoryNum = "" 'added 4/23/2014 for reconciliation
            _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation = False 'added 7/3/2014

            'added 7/7/2014
            _ExternalCode = ""
            _ExternalId = ""
            _FirstAddedDate = "" 'Date
            _GuaranteedRatePeriodEffectiveDate = "" 'Date
            _IsPossibleRelatedClaim = False
            '_LossHistoryDisplayNum = ""
            '_LossHistoryStatusTypeId = "" 'Byte (?)
            _ManualOverride = False
            '_PackagePartNum = ""
            '_PackageUnitNum = "" 'IdValue
            _TypeOfLoss = ""

            _DetailStatusCode = "" 'added 5/15/2019

            'added 7/24/2019
            _AddedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""
        End Sub
        Public Function HasValidLossHistoryNum() As Boolean 'added 4/23/2014 for reconciliation purposes
            'If _LossHistoryNum <> "" AndAlso IsNumeric(_LossHistoryNum) = True AndAlso CInt(_LossHistoryNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            Return qqHelper.IsValidQuickQuoteIdOrNum(_LossHistoryNum)
        End Function
        'added 7/3/2014 for lossHistoryDetail reconciliation
        Public Sub ParseThruLossHistoryDetails()
            If _LossHistoryDetailRecords IsNot Nothing AndAlso _LossHistoryDetailRecords.Count > 0 Then
                For Each d As QuickQuoteLossHistoryDetailRecord In _LossHistoryDetailRecords
                    If _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation = False Then
                        If d.HasValidLossHistoryDetailNum = True Then
                            _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.TypeOfLossId <> "" Then
                    Dim l As String = ""
                    l = "TypeOfLossId: " & Me.TypeOfLossId
                    Dim lType As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, Me.TypeOfLossId)
                    If lType <> "" Then
                        l &= " (" & lType & ")"
                    End If
                    str = qqHelper.appendText(str, l, vbCrLf)
                End If
                If Me.LossDate <> "" Then
                    str = qqHelper.appendText(str, "LossDate: " & Me.LossDate, vbCrLf)
                End If
                If Me.LossDescription <> "" Then
                    str = qqHelper.appendText(str, "LossDescription: " & Me.LossDescription, vbCrLf)
                End If
                If Me.LossHistorySurchargeId <> "" Then
                    Dim s As String = ""
                    s = "LossHistorySurchargeId: " & Me.LossHistorySurchargeId
                    Dim sType As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuoteHelperClass.QuickQuotePropertyName.LossHistorySurchargeId, Me.LossHistorySurchargeId)
                    If sType <> "" Then
                        s &= " (" & sType & ")"
                    End If
                    str = qqHelper.appendText(str, s, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Amount IsNot Nothing Then
                        _Amount = Nothing
                    End If
                    If _Catastrophic <> Nothing Then
                        _Catastrophic = Nothing
                    End If
                    If _CatastrophicCode IsNot Nothing Then
                        _CatastrophicCode = Nothing
                    End If
                    If _ClaimControlId IsNot Nothing Then
                        _ClaimControlId = Nothing
                    End If
                    If _ClaimNumber IsNot Nothing Then
                        _ClaimNumber = Nothing
                    End If
                    If _Comments IsNot Nothing Then
                        _Comments = Nothing
                    End If
                    If _LossDate IsNot Nothing Then
                        _LossDate = Nothing
                    End If
                    If _LossDescription IsNot Nothing Then
                        _LossDescription = Nothing
                    End If

                    If _LossHistoryDetailRecords IsNot Nothing Then
                        If _LossHistoryDetailRecords.Count > 0 Then
                            For Each dr As QuickQuoteLossHistoryDetailRecord In _LossHistoryDetailRecords
                                dr.Dispose()
                                dr = Nothing
                            Next
                            _LossHistoryDetailRecords.Clear()
                        End If
                        _LossHistoryDetailRecords = Nothing
                    End If

                    If _LossHistoryFaultId IsNot Nothing Then
                        _LossHistoryFaultId = Nothing
                    End If
                    If _LossHistoryLocationTypeId IsNot Nothing Then
                        _LossHistoryLocationTypeId = Nothing
                    End If
                    If _LossHistorySourceId IsNot Nothing Then
                        _LossHistorySourceId = Nothing
                    End If
                    If _LossHistorySurchargeId IsNot Nothing Then
                        _LossHistorySurchargeId = Nothing
                    End If
                    If _ReserveAmount IsNot Nothing Then
                        _ReserveAmount = Nothing
                    End If
                    If _TypeOfLossId IsNot Nothing Then
                        _TypeOfLossId = Nothing
                    End If
                    If _UninsuredUnderinsured <> Nothing Then
                        _UninsuredUnderinsured = Nothing
                    End If
                    'If _UnitNum IsNot Nothing Then 'removed 2/5/2015 like we did for AccidentViolation.UnitNum
                    '    _UnitNum = Nothing
                    'End If
                    If _WeatherRelated <> Nothing Then
                        _WeatherRelated = Nothing
                    End If

                    If _LossHistoryNum IsNot Nothing Then 'added 4/23/2014 for reconciliation
                        _LossHistoryNum = Nothing
                    End If
                    If _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation <> Nothing Then 'added 7/3/2014
                        _CanUseLossHistoryDetailNumForLossHistoryDetailReconciliation = Nothing
                    End If

                    'added 7/7/2014
                    If _ExternalCode IsNot Nothing Then
                        _ExternalCode = Nothing
                    End If
                    If _ExternalId IsNot Nothing Then
                        _ExternalId = Nothing
                    End If
                    If _FirstAddedDate IsNot Nothing Then
                        _FirstAddedDate = Nothing
                    End If
                    If _GuaranteedRatePeriodEffectiveDate IsNot Nothing Then
                        _GuaranteedRatePeriodEffectiveDate = Nothing
                    End If
                    If _IsPossibleRelatedClaim <> Nothing Then
                        _IsPossibleRelatedClaim = Nothing
                    End If
                    '_LossHistoryDisplayNum = ""
                    '_LossHistoryStatusTypeId = "" 'Byte (?)
                    If _ManualOverride <> Nothing Then
                        _ManualOverride = Nothing
                    End If
                    '_PackagePartNum = ""
                    '_PackageUnitNum = "" 'IdValue
                    If _TypeOfLoss IsNot Nothing Then
                        _TypeOfLoss = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 7/24/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum)

                    MyBase.Dispose() 'added 8/4/2014
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
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
