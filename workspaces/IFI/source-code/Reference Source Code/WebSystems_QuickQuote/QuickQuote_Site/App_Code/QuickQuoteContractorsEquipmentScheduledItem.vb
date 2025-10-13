Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store contractors equipment scheduled item information
    ''' </summary>
    ''' <remarks>equates to a specific coverage on the quote</remarks>
    <Serializable()> _
    Public Class QuickQuoteContractorsEquipmentScheduledItem
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Description As String
        Private _Limit As String
        Private _ValuationMethod As String
        Private _ValuationMethodId As String

        'added 8/7/2012 for App Gap; only used on Building in Diamond
        Private _AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)

        '*7/20/2012:  App Gap specs show that loss payees can be tied to each scheduled item
        'UI only allows for loss payees entered at the Location/Building level

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property Limit As String '5/29/2017 note: Coverage.ManualLimitAmount is money type in Diamond db; is Decimal in Business Object
            Get
                Return _Limit
            End Get
            Set(value As String)
                _Limit = value
                qqHelper.ConvertToLimitFormat(_Limit)
            End Set
        End Property
        Public Property ValuationMethod As String
            Get
                Return _ValuationMethod
            End Get
            Set(value As String)
                _ValuationMethod = value
                Select Case _ValuationMethod
                    Case "Replacement Cost"
                        _ValuationMethodId = "1"
                    Case "Actual Cash Value"
                        _ValuationMethodId = "2"
                    Case "Functional Building Valuation"
                        _ValuationMethodId = "3"
                    Case Else
                        _ValuationMethodId = ""
                End Select
            End Set
        End Property
        Public Property ValuationMethodId As String 'verified in database 7/3/2012
            Get
                Return _ValuationMethodId
            End Get
            Set(value As String)
                _ValuationMethodId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _ValuationMethod = ""
                If IsNumeric(_ValuationMethodId) = True Then
                    Select Case _ValuationMethodId
                        Case "1"
                            _ValuationMethod = "Replacement Cost"
                        Case "2"
                            _ValuationMethod = "Actual Cash Value"
                        Case "3"
                            _ValuationMethod = "Functional Building Valuation"
                    End Select
                End If
            End Set
        End Property

        Public Property AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04192}")
                Return _AdditionalInterests
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04192}")
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

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Description = ""
            _Limit = ""
            _ValuationMethod = ""
            _ValuationMethodId = ""

            '_AdditionalInterests = New Generic.List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

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

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Limit IsNot Nothing Then
                        _Limit = Nothing
                    End If
                    If _ValuationMethod IsNot Nothing Then
                        _ValuationMethod = Nothing
                    End If
                    If _ValuationMethodId IsNot Nothing Then
                        _ValuationMethodId = Nothing
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

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

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
