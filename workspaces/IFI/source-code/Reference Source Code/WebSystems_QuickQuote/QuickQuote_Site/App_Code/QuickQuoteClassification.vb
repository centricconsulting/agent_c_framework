Imports System.Configuration
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store classification information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteClassification
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 8/20/2012 for formatting QuotedPremium

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _ClassificationNum As String
        Private _ClassificationTypeId As String
        Private _CommodityPriceStabilizationFactor As String
        Private _Description As String
        Private _ManualPremium As String

        Private _IsLongshoreAndHarbor As Boolean
        Private _IsMedicalBenefitExclusion As Boolean
        Private _IsMedicalBenefitReimbursement As Boolean
        Private _IsOwner As Boolean
        Private _AnnualSalesReceipts As String
        Private _NumberOfEmployees As String
        Private _NumberOfExecutiveOfficers As String
        Private _NumberOfFullTimeEmployees As String
        Private _NumberOfPartTimeEmployees As String
        Private _Payroll As String
        Private _Rate As String

        'added 8/8/2012
        Private _ClassCode As String
        Private _ClassificationTypeIdUsedForClassCode As String

        'added 8/20/2012
        'Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        Private _Coverage As QuickQuoteCoverage
        Private _QuotedPremium As String

        'added 3/22/2017
        Private _PredominantOccupancy As Boolean

        'added 9/2/2017
        Private _BopClassDescription As String
        Private _BopClassCode As String
        Private _BopClassProgram As String
        Private _BopClassProgramAbbreviation As String
        Private _ClassificationTypeIdUsedForBopClassInfo As String
        Private _QuoteEffectiveDate As String
        Private _QuoteEffectiveDateUsedForBopClassInfo As String

        'added 12/6/2017
        Private _DescriptionFromLookup As String
        Private _ClassificationTypeIdUsedForDescriptionLookup As String
        Private _PreviousClassificationTypeId As String
        Private _PreviousDescription As String
        'added 12/18/2017
        Private _DescriptionManuallySet As Boolean

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
        Public Property ClassificationNum As String
            Get
                Return _ClassificationNum
            End Get
            Set(value As String)
                _ClassificationNum = value
            End Set
        End Property
        Public Property ClassificationTypeId As String
            Get
                Return _ClassificationTypeId
            End Get
            Set(value As String)
                _ClassificationTypeId = value
                'added 12/6/2017
                If String.IsNullOrWhiteSpace(_Description) = False AndAlso String.IsNullOrWhiteSpace(_ClassificationTypeIdUsedForDescriptionLookup) = False AndAlso _Description = _DescriptionFromLookup AndAlso _ClassificationTypeIdUsedForDescriptionLookup <> _ClassificationTypeId Then
                    'description is no longer valid; wipe out or revert to old
                    If String.IsNullOrWhiteSpace(_PreviousClassificationTypeId) = False AndAlso _PreviousClassificationTypeId = _ClassificationTypeId Then
                        Dim holdDesc As String = _DescriptionFromLookup
                        Dim holdClassTypeId As String = _ClassificationTypeIdUsedForDescriptionLookup
                        _DescriptionFromLookup = _PreviousDescription
                        _Description = _DescriptionFromLookup
                        _ClassificationTypeIdUsedForDescriptionLookup = _PreviousClassificationTypeId
                        _PreviousDescription = holdDesc
                        _PreviousClassificationTypeId = holdClassTypeId
                    Else
                        _PreviousDescription = _Description
                        _PreviousClassificationTypeId = _ClassificationTypeIdUsedForDescriptionLookup
                        _Description = ""
                        _DescriptionFromLookup = ""
                        _ClassificationTypeIdUsedForDescriptionLookup = ""
                    End If
                End If
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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
                'added 12/18/2017
                If String.IsNullOrWhiteSpace(_Description) = False Then
                    _DescriptionManuallySet = True
                End If
            End Set
        End Property
        Public Property ManualPremium As String
            Get
                Return _ManualPremium
            End Get
            Set(value As String)
                _ManualPremium = value
            End Set
        End Property

        Public Property IsLongshoreAndHarbor As Boolean
            Get
                Return _IsLongshoreAndHarbor
            End Get
            Set(value As Boolean)
                _IsLongshoreAndHarbor = value
            End Set
        End Property
        Public Property IsMedicalBenefitExclusion As Boolean
            Get
                Return _IsMedicalBenefitExclusion
            End Get
            Set(value As Boolean)
                _IsMedicalBenefitExclusion = value
            End Set
        End Property
        Public Property IsMedicalBenefitReimbursement As Boolean
            Get
                Return _IsMedicalBenefitReimbursement
            End Get
            Set(value As Boolean)
                _IsMedicalBenefitReimbursement = value
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
        Public Property AnnualSalesReceipts As String
            Get
                Return _AnnualSalesReceipts
            End Get
            Set(value As String)
                _AnnualSalesReceipts = value
            End Set
        End Property
        Public Property NumberOfEmployees As String
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
            End Set
        End Property
        Public Property NumberOfExecutiveOfficers As String
            Get
                Return _NumberOfExecutiveOfficers
            End Get
            Set(value As String)
                _NumberOfExecutiveOfficers = value
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
        Public Property Payroll As String
            Get
                Return _Payroll
            End Get
            Set(value As String)
                _Payroll = value
            End Set
        End Property
        Public Property Rate As String
            Get
                Return _Rate
            End Get
            Set(value As String)
                _Rate = value
            End Set
        End Property

        Public ReadOnly Property ClassCode As String
            Get
                If _ClassCode = "" OrElse (_ClassCode <> "" AndAlso _ClassificationTypeId <> _ClassificationTypeIdUsedForClassCode) Then
                    'lookup classcode
                    GetClassCodeForClassificationTypeId()
                End If
                Return _ClassCode
            End Get
            'Set(value As String)
            '    _ClassCode = value
            'End Set
        End Property

        'Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
        '    Get
        '        Return _Coverages
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteCoverage))
        '        _Coverages = value
        '    End Set
        'End Property
        Public Property Coverage As QuickQuoteCoverage
            Get
                Return _Coverage
            End Get
            Set(value As QuickQuoteCoverage)
                _Coverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10058</remarks>
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

        'added 3/22/2017
        Public Property PredominantOccupancy As Boolean
            Get
                Return _PredominantOccupancy
            End Get
            Set(value As Boolean)
                _PredominantOccupancy = value
            End Set
        End Property

        'added 9/2/2017
        Public ReadOnly Property BopClassDescription As String
            Get
                GetBopClassInfoForClassificationTypeId()
                Return _BopClassDescription
            End Get
            'Set(value As String)
            '    _BopClassDescription = value
            'End Set
        End Property
        Public ReadOnly Property BopClassCode As String
            Get
                GetBopClassInfoForClassificationTypeId()
                Return _BopClassCode
            End Get
            'Set(value As String)
            '    _BopClassCode = value
            'End Set
        End Property
        Public ReadOnly Property BopClassProgram As String
            Get
                GetBopClassInfoForClassificationTypeId()
                Return _BopClassProgram
            End Get
            'Set(value As String)
            '    _BopClassProgram = value
            'End Set
        End Property
        Public ReadOnly Property BopClassProgramAbbreviation As String
            Get
                GetBopClassInfoForClassificationTypeId()
                Return _BopClassProgramAbbreviation
            End Get
            'Set(value As String)
            '    _BopClassProgramAbbreviation = value
            'End Set
        End Property
        Public Property QuoteEffectiveDate As String
            Get
                Return _QuoteEffectiveDate
            End Get
            Set(value As String)
                _QuoteEffectiveDate = value
            End Set
        End Property
        'added 12/18/2017
        Public ReadOnly Property DescriptionManuallySet As Boolean
            Get
                Return _DescriptionManuallySet
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

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _ClassificationNum = ""
            _ClassificationTypeId = ""
            _CommodityPriceStabilizationFactor = ""
            _Description = ""
            _ManualPremium = ""

            _IsLongshoreAndHarbor = False
            _IsMedicalBenefitExclusion = False
            _IsMedicalBenefitReimbursement = False
            _IsOwner = False
            _AnnualSalesReceipts = ""
            _NumberOfEmployees = ""
            _NumberOfExecutiveOfficers = ""
            _NumberOfFullTimeEmployees = ""
            _NumberOfPartTimeEmployees = ""
            _Payroll = ""
            _Rate = ""

            _ClassCode = ""
            _ClassificationTypeIdUsedForClassCode = ""

            '_Coverages = New Generic.List(Of QuickQuoteCoverage)
            _Coverage = New QuickQuoteCoverage
            _QuotedPremium = ""

            'added 3/22/2017
            _PredominantOccupancy = False

            'added 9/2/2017
            _BopClassDescription = ""
            _BopClassCode = ""
            _BopClassProgram = ""
            _BopClassProgramAbbreviation = ""
            _ClassificationTypeIdUsedForBopClassInfo = ""
            _QuoteEffectiveDate = ""
            _QuoteEffectiveDateUsedForBopClassInfo = ""

            'added 12/6/2017
            _DescriptionFromLookup = ""
            _ClassificationTypeIdUsedForDescriptionLookup = ""
            _PreviousClassificationTypeId = ""
            _PreviousDescription = ""
            'added 12/18/2017
            _DescriptionManuallySet = False

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub

        Private Sub GetClassCodeForClassificationTypeId()
            _ClassificationTypeIdUsedForClassCode = _ClassificationTypeId

            If _ClassificationTypeId <> "" AndAlso IsNumeric(_ClassificationTypeId) = True Then
                Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
                    'sql.queryOrStoredProc = "SELECT CT.code FROM ClassificationType AS CT WITH (NOLOCK) WHERE classificationtype_id = " & CInt(_ClassificationTypeId)
                    'updated 6/15/2017 to pull descriptions for Diamond Proposals... when not done from VR
                    sql.queryOrStoredProc = "SELECT CT.code, CT.dscr FROM ClassificationType AS CT WITH (NOLOCK) WHERE classificationtype_id = " & CInt(_ClassificationTypeId)
                    Dim dr As Data.SqlClient.SqlDataReader = sql.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows = True Then
                        dr.Read()
                        _ClassCode = dr.Item("code").ToString.Trim
                        'updated 6/15/2017 to pull descriptions for Diamond Proposals... when not done from VR
                        Dim ctDesc As String = dr.Item("dscr").ToString.Trim
                        'If String.IsNullOrWhiteSpace(_Description) = True AndAlso String.IsNullOrWhiteSpace(ctDesc) = False Then
                        'updated 12/7/2017
                        If (String.IsNullOrWhiteSpace(_Description) = True AndAlso String.IsNullOrWhiteSpace(ctDesc) = False) OrElse (String.IsNullOrWhiteSpace(_Description) = False AndAlso String.IsNullOrWhiteSpace(_DescriptionFromLookup) = False AndAlso _Description = _DescriptionFromLookup) Then
                            If String.IsNullOrWhiteSpace(_Description) = False AndAlso String.IsNullOrWhiteSpace(_DescriptionFromLookup) = False AndAlso _Description = _DescriptionFromLookup Then 'added IF 12/7/2017
                                _PreviousDescription = _Description
                                _PreviousClassificationTypeId = _ClassificationTypeIdUsedForDescriptionLookup
                            End If
                            _Description = ctDesc
                            _DescriptionFromLookup = _Description 'added 12/6/2017
                            _ClassificationTypeIdUsedForDescriptionLookup = _ClassificationTypeId 'added 12/6/2017
                        End If
                    End If
                End Using
            End If

        End Sub
        'added 8/20/2012
        Public Sub ParseThruCoverage()
            If _Coverage IsNot Nothing Then
                If _Coverage.CoverageCodeId = "10058" Then 'Edit-Classification
                    QuotedPremium = _Coverage.FullTermPremium
                End If
            End If
        End Sub

        Public Function HasValidClassificationNum() As Boolean 'added 2/20/2017 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ClassificationNum)
        End Function

        'added 9/2/2017
        Public Sub GetBopClassInfoForClassificationTypeId(Optional ByVal forceCall As Boolean = False, Optional ByVal forceOverwriteWithCallToSetMethod As Boolean = True)
            If forceCall = True OrElse qqHelper.IsPositiveIntegerString(_ClassificationTypeIdUsedForBopClassInfo) = False OrElse _ClassificationTypeIdUsedForBopClassInfo <> _ClassificationTypeId OrElse _QuoteEffectiveDateUsedForBopClassInfo <> _QuoteEffectiveDate Then
                Dim bopClassDesc As String = ""
                Dim bopClassCd As String = ""
                Dim bopClassProg As String = ""
                Dim bopClassProgAbbrev As String = ""
                If qqHelper.IsDateString(_QuoteEffectiveDate) = True Then
                    qqHelper.SetBuildingClassificationType(_ClassificationTypeId, bopClassProg, bopClassDesc, bopClassCd, programAbbrev:=bopClassProgAbbrev, effectiveDate:=_QuoteEffectiveDate)
                Else
                    qqHelper.SetBuildingClassificationType(_ClassificationTypeId, bopClassProg, bopClassDesc, bopClassCd, programAbbrev:=bopClassProgAbbrev)
                End If
                If forceOverwriteWithCallToSetMethod = True OrElse String.IsNullOrWhiteSpace(_BopClassProgram) = False OrElse String.IsNullOrWhiteSpace(_BopClassDescription) = False OrElse String.IsNullOrWhiteSpace(_BopClassCode) = False OrElse String.IsNullOrWhiteSpace(_BopClassProgramAbbreviation) = False Then
                    _ClassificationTypeIdUsedForBopClassInfo = _ClassificationTypeId
                    _QuoteEffectiveDateUsedForBopClassInfo = _QuoteEffectiveDate

                    _BopClassDescription = bopClassDesc
                    _BopClassCode = bopClassCd
                    _BopClassProgram = bopClassProg
                    _BopClassProgramAbbreviation = bopClassProgAbbrev
                End If
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
                    If _ClassificationNum IsNot Nothing Then
                        _ClassificationNum = Nothing
                    End If
                    If _ClassificationTypeId IsNot Nothing Then
                        _ClassificationTypeId = Nothing
                    End If
                    If _CommodityPriceStabilizationFactor IsNot Nothing Then
                        _CommodityPriceStabilizationFactor = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _ManualPremium IsNot Nothing Then
                        _ManualPremium = Nothing
                    End If

                    If _IsLongshoreAndHarbor <> Nothing Then
                        _IsLongshoreAndHarbor = Nothing
                    End If
                    If _IsMedicalBenefitExclusion <> Nothing Then
                        _IsMedicalBenefitExclusion = Nothing
                    End If
                    If _IsMedicalBenefitReimbursement <> Nothing Then
                        _IsMedicalBenefitReimbursement = Nothing
                    End If
                    If _IsOwner <> Nothing Then
                        _IsOwner = Nothing
                    End If
                    If _AnnualSalesReceipts IsNot Nothing Then
                        _AnnualSalesReceipts = Nothing
                    End If
                    If _NumberOfEmployees IsNot Nothing Then
                        _NumberOfEmployees = Nothing
                    End If
                    If _NumberOfExecutiveOfficers IsNot Nothing Then
                        _NumberOfExecutiveOfficers = Nothing
                    End If
                    If _NumberOfFullTimeEmployees IsNot Nothing Then
                        _NumberOfFullTimeEmployees = Nothing
                    End If
                    If _NumberOfPartTimeEmployees IsNot Nothing Then
                        _NumberOfPartTimeEmployees = Nothing
                    End If
                    If _Payroll IsNot Nothing Then
                        _Payroll = Nothing
                    End If
                    If _Rate IsNot Nothing Then
                        _Rate = Nothing
                    End If

                    If _ClassCode IsNot Nothing Then
                        _ClassCode = Nothing
                    End If
                    If _ClassificationTypeIdUsedForClassCode IsNot Nothing Then
                        _ClassificationTypeIdUsedForClassCode = Nothing
                    End If

                    'If _Coverages IsNot Nothing Then
                    '    If _Coverages.Count > 0 Then
                    '        For Each cov As QuickQuoteCoverage In _Coverages
                    '            cov.Dispose()
                    '            cov = Nothing
                    '        Next
                    '        _Coverages.Clear()
                    '    End If
                    '    _Coverages = Nothing
                    'End If
                    If _Coverage IsNot Nothing Then
                        _Coverage.Dispose()
                        _Coverage = Nothing
                    End If
                    If _QuotedPremium IsNot Nothing Then
                        _QuotedPremium = Nothing
                    End If

                    'added 3/22/2017
                    _PredominantOccupancy = Nothing

                    'added 9/2/2017
                    qqHelper.DisposeString(_BopClassDescription)
                    qqHelper.DisposeString(_BopClassCode)
                    qqHelper.DisposeString(_BopClassProgram)
                    qqHelper.DisposeString(_BopClassProgramAbbreviation)
                    qqHelper.DisposeString(_ClassificationTypeIdUsedForBopClassInfo)
                    qqHelper.DisposeString(_QuoteEffectiveDate)
                    qqHelper.DisposeString(_QuoteEffectiveDateUsedForBopClassInfo)

                    'added 12/6/2017
                    qqHelper.DisposeString(_DescriptionFromLookup)
                    qqHelper.DisposeString(_ClassificationTypeIdUsedForDescriptionLookup)
                    qqHelper.DisposeString(_PreviousClassificationTypeId)
                    qqHelper.DisposeString(_PreviousDescription)
                    'added 12/18/2017
                    _DescriptionManuallySet = Nothing

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
