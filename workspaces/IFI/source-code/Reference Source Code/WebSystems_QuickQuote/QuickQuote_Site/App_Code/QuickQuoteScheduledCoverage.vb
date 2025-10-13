Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 1/22/2015

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store scheduled coverage information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteScheduledCoverage 'added 9/27/2012 for CPR
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 1/22/2015

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        Private _UICoverageScheduledCoverageParentTypeId As String
        'added 1/20/2015 (Comm IM / Crime; Farm)
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: currently just have parsing logic and Diamond service logic... no comparative rater logic (xml writing) since there currently aren't Write methods for Coverages or ScheduledCoverages (just being done from parent node); 1/22/2015 update: now have write methods for Coverages/ScheduledCoverages... not being used yet; 4/2/2015 note: being used as-of 1/26/2015
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _ScheduledCoverageNum As String 'added 1/22/2015 for reconciliation
        Private _Address As QuickQuoteAddress 'added 5/4/2017 for CIM (Golf)

        Private _DetailStatusCode As String 'added 5/15/2019

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
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05609}")
                Return _Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05609}")
            End Set
        End Property
        Public Property UICoverageScheduledCoverageParentTypeId As String
            Get
                Return _UICoverageScheduledCoverageParentTypeId
            End Get
            Set(value As String)
                _UICoverageScheduledCoverageParentTypeId = value
            End Set
        End Property
        'added 1/20/2015 (Comm IM / Crime; Farm)
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05610}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05610}")
            End Set
        End Property
        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property ScheduledCoverageNum As String 'added 1/22/2015 for reconciliation
            Get
                Return _ScheduledCoverageNum
            End Get
            Set(value As String)
                _ScheduledCoverageNum = value
            End Set
        End Property
        Public Property Address As QuickQuoteAddress 'added 5/4/2017 for CIM (Golf)
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
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

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            '_Coverages = New Generic.List(Of QuickQuoteCoverage)
            _Coverages = Nothing 'added 8/4/2014
            _UICoverageScheduledCoverageParentTypeId = ""
            'added 1/20/2015 (Comm IM / Crime; Farm)
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _ScheduledCoverageNum = "" 'added 1/22/2015 for reconciliation
            _Address = Nothing 'added 5/4/2017 for CIM (Golf)

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidScheduledCoverageNum() As Boolean 'added 1/22/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledCoverageNum)
        End Function
        'added 1/22/2015 for additionalInterests reconciliation; method is here and in QuickQuoteContractsEquipmentScheduledCoverage... can be called from either and passed to the other
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub

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
                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each c As QuickQuoteCoverage In _Coverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _UICoverageScheduledCoverageParentTypeId IsNot Nothing Then
                        _UICoverageScheduledCoverageParentTypeId = Nothing
                    End If
                    'added 1/20/2015 (Comm IM / Crime; Farm)
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    If _ScheduledCoverageNum IsNot Nothing Then 'added 1/22/2015 for reconciliation
                        _ScheduledCoverageNum = Nothing
                    End If
                    If _Address IsNot Nothing Then 'added 5/4/2017 for CIM (Golf)
                        _Address.Dispose()
                        _Address = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
