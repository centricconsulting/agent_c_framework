Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store scheduled item information
    ''' </summary>
    ''' <remarks>can be used for PPA vehicles</remarks>
    <Serializable()> _
    Public Class QuickQuoteScheduledItem 'added 7/25/2013 for PPA
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'can't add from UI
        Private _Amount As String
        Private _Breakage As Boolean
        Private _BreakagePremiumFullTerm As String
        Private _Description As String
        Private _Dscr2 As String
        Private _ItemDate As String
        Private _PremiumFullTerm As String
        Private _ScheduledItemsCategoryId As String
        Private _ScheduledItemsComboId As String
        Private _ScheduledItemsTypeId As String

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
        Private _ScheduledItemsNum As String 'added 5/14/2014

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
        Public Property AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05621}")
                Return _AdditionalInterests
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05621}")
            End Set
        End Property
        Public Property Amount As String
            Get
                Return _Amount
                'updated 8/25/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_Amount)
            End Get
            Set(value As String)
                _Amount = value
                qqHelper.ConvertToQuotedPremiumFormat(_Amount) 'may not use
            End Set
        End Property
        Public Property Breakage As Boolean
            Get
                Return _Breakage
            End Get
            Set(value As Boolean)
                _Breakage = value
            End Set
        End Property
        Public Property BreakagePremiumFullTerm As String
            Get
                'Return _BreakagePremiumFullTerm
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BreakagePremiumFullTerm)
            End Get
            Set(value As String)
                _BreakagePremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_BreakagePremiumFullTerm)
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property Dscr2 As String
            Get
                Return _Dscr2
            End Get
            Set(value As String)
                _Dscr2 = value
            End Set
        End Property
        Public Property ItemDate As String
            Get
                Return _ItemDate
            End Get
            Set(value As String)
                _ItemDate = value
                qqHelper.ConvertToShortDate(_ItemDate)
            End Set
        End Property
        Public Property PremiumFullTerm As String
            Get
                'Return _PremiumFullTerm
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PremiumFullTerm)
            End Get
            Set(value As String)
                _PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_PremiumFullTerm)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ScheduledItemsCategory table</remarks>
        Public Property ScheduledItemsCategoryId As String 'None=-1; N/A=0; Animals=1; Coverage=2; etc.
            Get
                Return _ScheduledItemsCategoryId
            End Get
            Set(value As String)
                _ScheduledItemsCategoryId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ScheduledItemsCombo table (None=0, Covered=1, Excluded=2, N/A=3)</remarks>
        Public Property ScheduledItemsComboId As String 'None=0; Covered=1; Excluded=2; N/A=3
            Get
                Return _ScheduledItemsComboId
            End Get
            Set(value As String)
                _ScheduledItemsComboId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ScheduledItemsType table</remarks>
        Public Property ScheduledItemsTypeId As String 'None=0; Custom Equipment=1; N/A=20; Trailers=21; Sidecars=22; Windshield=23; etc.
            Get
                Return _ScheduledItemsTypeId
            End Get
            Set(value As String)
                _ScheduledItemsTypeId = value
            End Set
        End Property

        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property ScheduledItemsNum As String 'added 5/14/2014
            Get
                Return _ScheduledItemsNum
            End Get
            Set(value As String)
                _ScheduledItemsNum = value
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
            '_AdditionalInterests = New Generic.List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _Amount = ""
            _Breakage = False
            _BreakagePremiumFullTerm = ""
            _Description = ""
            _Dscr2 = ""
            _ItemDate = ""
            _PremiumFullTerm = ""
            _ScheduledItemsCategoryId = ""
            _ScheduledItemsComboId = ""
            _ScheduledItemsTypeId = ""

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014
            _ScheduledItemsNum = "" 'added 5/14/2014

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        'added 4/29/2014 for additionalInterests reconciliation
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
        Public Function HasValidScheduledItemsNum() As Boolean 'added 5/14/2014
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledItemsNum)
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
                    If _Amount IsNot Nothing Then
                        _Amount = Nothing
                    End If
                    If _Breakage <> Nothing Then
                        _Breakage = Nothing
                    End If
                    If _BreakagePremiumFullTerm IsNot Nothing Then
                        _BreakagePremiumFullTerm = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Dscr2 IsNot Nothing Then
                        _Dscr2 = Nothing
                    End If
                    If _ItemDate IsNot Nothing Then
                        _ItemDate = Nothing
                    End If
                    If _PremiumFullTerm IsNot Nothing Then
                        _PremiumFullTerm = Nothing
                    End If
                    If _ScheduledItemsCategoryId IsNot Nothing Then
                        _ScheduledItemsCategoryId = Nothing
                    End If
                    If _ScheduledItemsComboId IsNot Nothing Then
                        _ScheduledItemsComboId = Nothing
                    End If
                    If _ScheduledItemsTypeId IsNot Nothing Then
                        _ScheduledItemsTypeId = Nothing
                    End If

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If
                    If _ScheduledItemsNum IsNot Nothing Then 'added 5/14/2014
                        _ScheduledItemsNum = Nothing
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
