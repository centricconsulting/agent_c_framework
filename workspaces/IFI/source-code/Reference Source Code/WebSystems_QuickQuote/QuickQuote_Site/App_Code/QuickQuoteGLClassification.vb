Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store GL classification information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteGLClassification
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 8/22/2012 for formatting QuotedPremium

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _BuildingInformation As String
        Private _CLMRatingBaseId As String
        Private _CLMSubsectionId As String
        Private _ClassCode As String
        Private _ClassDescription As String
        Private _CommodityPriceStabilizationFactor As String
        Private _GLClassificationNum As String
        Private _IsOwner As Boolean
        Private _NumberOfFullTimeEmployees As String
        Private _NumberOfPartTimeEmployees As String
        Private _OwnerTypeId As String
        Private _PMA As String
        Private _PackagePartNum As String
        Private _PremiumBase As String
        Private _PremiumBaseShort As String
        Private _PremiumExposure As String

        'added 8/22/2012 for GL
        Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        Private _QuotedPremium As String
        'added 8/28/2012 for GL
        Private _PremisesQuotedPremium As String
        Private _ProductsQuotedPremium As String

        'added 11/5/2012 for GL (ELPa rates)
        Private _ManualElpaRate_Premises As String
        Private _ManualElpaRate_Products As String
        'added 11/26/2012 for GL
        Private _Subline336_ExcludeProductsCompletedOperations As Boolean

        'added 8/19/2018
        Private _QuickQuoteState As QuickQuoteHelperClass.QuickQuoteState
        Private _QuoteStateTakenFrom As QuickQuoteHelperClass.QuickQuoteState

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 6/18/2025
        Private _AddedDate As String
        Private _LastModifiedDate As String
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
        Public Property BuildingInformation As String
            Get
                Return _BuildingInformation
            End Get
            Set(value As String)
                _BuildingInformation = value
            End Set
        End Property
        Public Property CLMRatingBaseId As String
            Get
                Return _CLMRatingBaseId
            End Get
            Set(value As String)
                _CLMRatingBaseId = value
            End Set
        End Property
        Public Property CLMSubsectionId As String
            Get
                Return _CLMSubsectionId
            End Get
            Set(value As String)
                _CLMSubsectionId = value
            End Set
        End Property
        Public Property ClassCode As String
            Get
                Return _ClassCode
            End Get
            Set(value As String)
                _ClassCode = value
            End Set
        End Property
        Public Property ClassDescription As String
            Get
                Return _ClassDescription
            End Get
            Set(value As String)
                _ClassDescription = value
            End Set
        End Property
        Public Property CommodityPriceStabilizationFactor As String
            Get
                Return _CommodityPriceStabilizationFactor
            End Get
            Set(value As String)
                _CommodityPriceStabilizationFactor = value
            End Set
        End Property
        Public Property GLClassificationNum As String
            Get
                Return _GLClassificationNum
            End Get
            Set(value As String)
                _GLClassificationNum = value
            End Set
        End Property
        Public Property IsOwner As Boolean
            Get
                Return _IsOwner
            End Get
            Set(value As Boolean)
                _IsOwner = value
            End Set
        End Property
        Public Property NumberOfFullTimeEmployees As String
            Get
                Return _NumberOfFullTimeEmployees
            End Get
            Set(value As String)
                _NumberOfFullTimeEmployees = value
            End Set
        End Property
        Public Property NumberOfPartTimeEmployees As String
            Get
                Return _NumberOfPartTimeEmployees
            End Get
            Set(value As String)
                _NumberOfPartTimeEmployees = value
            End Set
        End Property
        Public Property OwnerTypeId As String
            Get
                Return _OwnerTypeId
            End Get
            Set(value As String)
                _OwnerTypeId = value
            End Set
        End Property
        Public Property PMA As String
            Get
                Return _PMA
            End Get
            Set(value As String)
                _PMA = value
            End Set
        End Property
        Public Property PackagePartNum As String
            Get
                Return _PackagePartNum
            End Get
            Set(value As String)
                _PackagePartNum = value
            End Set
        End Property
        Public Property PremiumBase As String
            Get
                Return _PremiumBase
            End Get
            Set(value As String)
                _PremiumBase = value
            End Set
        End Property
        Public Property PremiumBaseShort As String
            Get
                Return _PremiumBaseShort
            End Get
            Set(value As String)
                _PremiumBaseShort = value
            End Set
        End Property
        Public Property PremiumExposure As String
            Get
                Return _PremiumExposure
            End Get
            Set(value As String)
                _PremiumExposure = value
                qqHelper.ConvertToLimitFormat(_PremiumExposure) 'added 10/9/2012
            End Set
        End Property

        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05301}")
                Return _Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05301}")
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80150, 80151, and 80152</remarks>
        Public Property QuotedPremium As String
            Get
                'Return _QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_QuotedPremium)
            End Get
            Set(value As String)
                _QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80150</remarks>
        Public Property PremisesQuotedPremium As String
            Get
                'Return _PremisesQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PremisesQuotedPremium)
            End Get
            Set(value As String)
                _PremisesQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PremisesQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80152</remarks>
        Public Property ProductsQuotedPremium As String
            Get
                'Return _ProductsQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_ProductsQuotedPremium)
            End Get
            Set(value As String)
                _ProductsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ProductsQuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80150</remarks>
        Public Property ManualElpaRate_Premises As String
            Get
                Return _ManualElpaRate_Premises
            End Get
            Set(value As String)
                _ManualElpaRate_Premises = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80152</remarks>
        Public Property ManualElpaRate_Products As String
            Get
                Return _ManualElpaRate_Products
            End Get
            Set(value As String)
                _ManualElpaRate_Products = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80152</remarks>
        Public Property Subline336_ExcludeProductsCompletedOperations As Boolean
            Get
                Return _Subline336_ExcludeProductsCompletedOperations
            End Get
            Set(value As Boolean)
                _Subline336_ExcludeProductsCompletedOperations = value
            End Set
        End Property

        'added 8/19/2018
        Public Property QuickQuoteState As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return _QuickQuoteState
            End Get
            Set(value As QuickQuoteHelperClass.QuickQuoteState)
                _QuickQuoteState = value
            End Set
        End Property
        Public ReadOnly Property QuoteStateTakenFrom As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return _QuoteStateTakenFrom
            End Get
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        'added 6/18/2025
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
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
        Public Property AddedImageNum As String 'added 7/31/2019
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
            _BuildingInformation = ""
            _CLMRatingBaseId = ""
            _CLMSubsectionId = ""
            _ClassCode = ""
            _ClassDescription = ""
            _CommodityPriceStabilizationFactor = ""
            _GLClassificationNum = ""
            _IsOwner = False
            _NumberOfFullTimeEmployees = ""
            _NumberOfPartTimeEmployees = ""
            _OwnerTypeId = ""
            _PMA = ""
            _PackagePartNum = ""
            _PremiumBase = ""
            _PremiumBaseShort = ""
            _PremiumExposure = ""

            '_Coverages = New Generic.List(Of QuickQuoteCoverage)
            _Coverages = Nothing 'added 8/4/2014
            _QuotedPremium = ""
            _PremisesQuotedPremium = ""
            _ProductsQuotedPremium = ""

            _ManualElpaRate_Premises = ""
            _ManualElpaRate_Products = ""
            _Subline336_ExcludeProductsCompletedOperations = False

            'added 8/19/2018
            _QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.None
            _QuoteStateTakenFrom = QuickQuoteHelperClass.QuickQuoteState.None

            _DetailStatusCode = "" 'added 5/15/2019

            ' added 6/18/2025
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""
        End Sub

        'added 8/22/2012
        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruCoverages()
            If _Coverages IsNot Nothing AndAlso _Coverages.Count > 0 Then
                For Each cov As QuickQuoteCoverage In _Coverages

                    'below code is specific to GL
                    Select Case cov.CoverageCodeId
                        Case "80150" 'CheckBox:  Commercial GL Subline 334 (premises)
                            PremisesQuotedPremium = cov.FullTermPremium
                            QuotedPremium = qqHelper.getSum(_QuotedPremium, _PremisesQuotedPremium)
                            _ManualElpaRate_Premises = cov.ManualElpaRate 'added 11/5/2012 for GL ELPa Rates
                        Case "80151" 'CheckBox:  Commercial GL Subline 335
                            QuotedPremium = qqHelper.getSum(_QuotedPremium, cov.FullTermPremium)
                        Case "80152" 'CheckBox:  Commercial GL Subline 336 (products)
                            ProductsQuotedPremium = cov.FullTermPremium
                            QuotedPremium = qqHelper.getSum(_QuotedPremium, _ProductsQuotedPremium)
                            _ManualElpaRate_Products = cov.ManualElpaRate 'added 11/5/2012 for GL ELPa Rates
                            _Subline336_ExcludeProductsCompletedOperations = cov.ExcludeProductsCompletedOperations 'added 11/26/2012 for GL
                    End Select

                Next
            End If
        End Sub

        'added 8/19/2018
        Protected Friend Sub Set_QuoteStateTakenFrom(ByVal qqState As QuickQuoteHelperClass.QuickQuoteState)
            _QuoteStateTakenFrom = qqState
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
                    If _BuildingInformation IsNot Nothing Then
                        _BuildingInformation = Nothing
                    End If
                    If _CLMRatingBaseId IsNot Nothing Then
                        _CLMRatingBaseId = Nothing
                    End If
                    If _CLMSubsectionId IsNot Nothing Then
                        _CLMSubsectionId = Nothing
                    End If
                    If _ClassCode IsNot Nothing Then
                        _ClassCode = Nothing
                    End If
                    If _ClassDescription IsNot Nothing Then
                        _ClassDescription = Nothing
                    End If
                    If _CommodityPriceStabilizationFactor IsNot Nothing Then
                        _CommodityPriceStabilizationFactor = Nothing
                    End If
                    If _GLClassificationNum IsNot Nothing Then
                        _GLClassificationNum = Nothing
                    End If
                    If _IsOwner <> Nothing Then
                        _IsOwner = Nothing
                    End If
                    If _NumberOfFullTimeEmployees IsNot Nothing Then
                        _NumberOfFullTimeEmployees = Nothing
                    End If
                    If _NumberOfPartTimeEmployees IsNot Nothing Then
                        _NumberOfPartTimeEmployees = Nothing
                    End If
                    If _OwnerTypeId IsNot Nothing Then
                        _OwnerTypeId = Nothing
                    End If
                    If _PMA IsNot Nothing Then
                        _PMA = Nothing
                    End If
                    If _PackagePartNum IsNot Nothing Then
                        _PackagePartNum = Nothing
                    End If
                    If _PremiumBase IsNot Nothing Then
                        _PremiumBase = Nothing
                    End If
                    If _PremiumBaseShort IsNot Nothing Then
                        _PremiumBaseShort = Nothing
                    End If
                    If _PremiumExposure IsNot Nothing Then
                        _PremiumExposure = Nothing
                    End If

                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each cov As QuickQuoteCoverage In _Coverages
                                cov.Dispose()
                                cov = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _QuotedPremium IsNot Nothing Then
                        _QuotedPremium = Nothing
                    End If
                    If _PremisesQuotedPremium IsNot Nothing Then
                        _PremisesQuotedPremium = Nothing
                    End If
                    If _ProductsQuotedPremium IsNot Nothing Then
                        _ProductsQuotedPremium = Nothing
                    End If

                    If _ManualElpaRate_Premises IsNot Nothing Then
                        _ManualElpaRate_Premises = Nothing
                    End If
                    If _ManualElpaRate_Products IsNot Nothing Then
                        _ManualElpaRate_Products = Nothing
                    End If
                    If _Subline336_ExcludeProductsCompletedOperations <> Nothing Then
                        _Subline336_ExcludeProductsCompletedOperations = Nothing
                    End If

                    'added 6/18/2025
                    If _AddedDate IsNot Nothing Then
                        _AddedDate = Nothing
                    End If
                    If _LastModifiedDate IsNot Nothing Then
                        _LastModifiedDate = Nothing
                    End If
                    If _PCAdded_Date IsNot Nothing Then
                        _PCAdded_Date = Nothing
                    End If
                    If _AddedImageNum <> Nothing Then
                        _AddedImageNum = Nothing
                    End If


                    'added 8/19/2018
                    _QuickQuoteState = Nothing
                    _QuoteStateTakenFrom = Nothing

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
