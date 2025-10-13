Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store unscheduled personal property coverage information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteUnscheduledPersonalPropertyCoverage 'added 2/19/2015
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Coverage As QuickQuoteCoverage
        Private _EarthquakeCoverage As QuickQuoteCoverage
        Private _UnscheduledFarmPersonalPropertyNum As String 'for reconciliation; may not be needed for this one as I haven't seen a list for these yet (UnscheduledPersonalProperties in xml is a single object of type Unscheduled Personal Property Coverage)
        Private _PeakSeasonCoverages As List(Of QuickQuoteCoverage) 'added 2/21/2015
        Private _Exclusions As List(Of QuickQuoteExclusion) 'added 2/21/2015
        Private _CanUseExclusionNumForExclusionReconciliation As Boolean 'added 2/21/2015
        Private _IncludeTheft As Boolean 'added 2/21/2015
        Private _HasEarthquakeCoverage As Boolean 'added 2/21/2015
        Private _Description As String 'added 2/21/2015
        'added 5/11/2015
        Private _PeakSeasons As List(Of QuickQuotePeakSeason)
        Private _IncreasedLimit As String
        Private _MainCoveragePremium As String
        Private _EarthquakePremium As String
        Private _PeakSeasonsPremium As String
        Private _TotalPremium As String

        'added 5/5/2016 for Farm Machinery coverage option
        Private _IsLimitedPerilsExtendedCoverage As Boolean 'CoverageDetail

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property Coverage As QuickQuoteCoverage
            Get
                SetObjectsParent(_Coverage)
                Return _Coverage
            End Get
            Set(value As QuickQuoteCoverage)
                _Coverage = value
                SetObjectsParent(_Coverage)
            End Set
        End Property
        Public Property EarthquakeCoverage As QuickQuoteCoverage
            Get
                SetObjectsParent(_EarthquakeCoverage)
                Return _EarthquakeCoverage
            End Get
            Set(value As QuickQuoteCoverage)
                _EarthquakeCoverage = value
                SetObjectsParent(_EarthquakeCoverage)
            End Set
        End Property
        Public Property UnscheduledFarmPersonalPropertyNum As String 'for reconciliation; may not be needed for this one as I haven't seen a list for these yet (UnscheduledPersonalProperties in xml is a single object of type Unscheduled Personal Property Coverage)
            Get
                Return _UnscheduledFarmPersonalPropertyNum
            End Get
            Set(value As String)
                _UnscheduledFarmPersonalPropertyNum = value
            End Set
        End Property
        Public Property PeakSeasonCoverages As List(Of QuickQuoteCoverage) 'added 2/21/2015
            Get
                SetParentOfListItems(_PeakSeasonCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05650}")
                Return _PeakSeasonCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _PeakSeasonCoverages = value
                SetParentOfListItems(_PeakSeasonCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05650}")
            End Set
        End Property
        Public Property Exclusions As List(Of QuickQuoteExclusion) 'added 2/21/2015
            Get
                SetParentOfListItems(_Exclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05651}")
                Return _Exclusions
            End Get
            Set(value As List(Of QuickQuoteExclusion))
                _Exclusions = value
                SetParentOfListItems(_Exclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05651}")
            End Set
        End Property
        Public Property CanUseExclusionNumForExclusionReconciliation As Boolean 'added 2/21/2015
            Get
                Return _CanUseExclusionNumForExclusionReconciliation
            End Get
            Set(value As Boolean)
                _CanUseExclusionNumForExclusionReconciliation = value
            End Set
        End Property
        Public Property IncludeTheft As Boolean 'added 2/21/2015
            Get
                Return _IncludeTheft
            End Get
            Set(value As Boolean)
                _IncludeTheft = value
            End Set
        End Property
        Public Property HasEarthquakeCoverage As Boolean 'added 2/21/2015
            Get
                Return _HasEarthquakeCoverage
            End Get
            Set(value As Boolean)
                _HasEarthquakeCoverage = value
            End Set
        End Property
        Public Property Description As String 'added 2/21/2015
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        'added 5/11/2015
        Public Property PeakSeasons As List(Of QuickQuotePeakSeason)
            Get
                SetParentOfListItems(_PeakSeasons, "{663B7C7B-F2AC-4BF6-965A-D30F41A05652}")
                Return _PeakSeasons
            End Get
            Set(value As List(Of QuickQuotePeakSeason))
                _PeakSeasons = value
                SetParentOfListItems(_PeakSeasons, "{663B7C7B-F2AC-4BF6-965A-D30F41A05652}")
            End Set
        End Property
        Public Property IncreasedLimit As String
            Get
                Return _IncreasedLimit
            End Get
            Set(value As String)
                _IncreasedLimit = value
                qqHelper.ConvertToLimitFormat(_IncreasedLimit)
            End Set
        End Property
        Public Property MainCoveragePremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MainCoveragePremium)
            End Get
            Set(value As String)
                _MainCoveragePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MainCoveragePremium)
            End Set
        End Property
        Public Property EarthquakePremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_EarthquakePremium)
            End Get
            Set(value As String)
                _EarthquakePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EarthquakePremium)
            End Set
        End Property
        Public Property PeakSeasonsPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PeakSeasonsPremium)
            End Get
            Set(value As String)
                _PeakSeasonsPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PeakSeasonsPremium)
            End Set
        End Property
        Public Property TotalPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPremium)
            End Get
            Set(value As String)
                _TotalPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPremium)
            End Set
        End Property

        'added 5/5/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
        Public Property IsLimitedPerilsExtendedCoverage As Boolean 'CoverageDetail
            Get
                Return _IsLimitedPerilsExtendedCoverage
            End Get
            Set(value As Boolean)
                _IsLimitedPerilsExtendedCoverage = value
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
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Coverage = New QuickQuoteCoverage
            _EarthquakeCoverage = New QuickQuoteCoverage
            _UnscheduledFarmPersonalPropertyNum = ""
            '_PeakSeasonCoverages = New List(Of QuickQuoteCoverage) 'added 2/21/2015
            _PeakSeasonCoverages = Nothing
            '_Exclusions = New List(Of QuickQuoteExclusion) 'added 2/21/2015
            _Exclusions = Nothing
            _CanUseExclusionNumForExclusionReconciliation = False 'added 2/21/2015
            _IncludeTheft = False 'added 2/21/2015
            _HasEarthquakeCoverage = False 'added 2/21/2015
            _Description = "" 'added 2/21/2015
            'added 5/11/2015
            '_PeakSeasons = New List(Of QuickQuotePeakSeason)
            _PeakSeasons = Nothing
            _IncreasedLimit = ""
            _MainCoveragePremium = ""
            _EarthquakePremium = ""
            _PeakSeasonsPremium = ""
            _TotalPremium = ""

            'added 5/5/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
            _IsLimitedPerilsExtendedCoverage = False 'CoverageDetail

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Sub CheckCoverage()
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                'Select Case _Coverage.CoverageCodeId
                '    Case "70004" 'Edit: Farm_G_Blanket_Farm_PP; no special fields set on example... cov probably isn't applied on quote

                'End Select
                'updated 5/11/2015; note: CoverageCodeId is always the same here
                'CoverageCodeId = _Coverage.CoverageCodeId
                'added 5/11/2015
                IncreasedLimit = Coverage.ManualLimitIncreased
                MainCoveragePremium = Coverage.FullTermPremium

                'added 5/5/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
                IsLimitedPerilsExtendedCoverage = Coverage.IsLimitedPerilsExtendedCoverage 'CoverageDetail
            End If
        End Sub
        Public Sub CheckEarthquakeCoverage()
            If _EarthquakeCoverage IsNot Nothing AndAlso _EarthquakeCoverage.CoverageCodeId <> "" Then
                Select Case _EarthquakeCoverage.CoverageCodeId
                    Case "40087" 'CheckBox: Farm_G_Earthquake; no special fields set on example... cov must not have been there since there was no Checkbox node; 2/21/2015 note: Checkbox = true when present
                        HasEarthquakeCoverage = _EarthquakeCoverage.Checkbox 'added 2/21/2015; note: doesn't necessarily need to check CoverageCodeId
                        If _HasEarthquakeCoverage = True Then 'added 5/11/2015
                            EarthquakePremium = _EarthquakeCoverage.FullTermPremium
                        End If
                End Select
            End If
        End Sub
        Public Function HasValidUnscheduledFarmPersonalPropertyNum() As Boolean 'added for reconciliation purposes; may not be needed for this one as I haven't seen a list for these yet (UnscheduledPersonalProperties in xml is a single object of type Unscheduled Personal Property Coverage)
            Return qqHelper.IsValidQuickQuoteIdOrNum(_UnscheduledFarmPersonalPropertyNum)
        End Function
        Public Sub ParseThruPeakSeasonCoverages()
            If _PeakSeasonCoverages IsNot Nothing AndAlso _PeakSeasonCoverages.Count > 0 Then
                For Each c As QuickQuoteCoverage In _PeakSeasonCoverages
                    'Select Case c.CoverageCodeId
                    '    '2/21/2015 - added cov from sample xml
                    '    Case "70050" 'Edit: Farm_Peak_Season_F

                    'End Select
                    'updated 5/11/2015
                    If c IsNot Nothing Then
                        If _PeakSeasons Is Nothing Then
                            _PeakSeasons = New List(Of QuickQuotePeakSeason)
                        End If
                        Dim ps As New QuickQuotePeakSeason
                        With ps
                            .EffectiveDate = c.EffectiveDate
                            .ExpirationDate = c.ExpirationDate
                            .IncreasedLimit = c.ManualLimitIncreased
                            .Description = c.Description
                            .Premium = c.FullTermPremium
                            PeakSeasonsPremium = qqHelper.getSum(_PeakSeasonsPremium, .Premium)
                        End With
                        _PeakSeasons.Add(ps)
                    End If
                Next
            End If
        End Sub
        Public Sub ParseThruExclusions() 'added 2/21/2015
            If _Exclusions IsNot Nothing AndAlso _Exclusions.Count > 0 Then
                For Each e As QuickQuoteExclusion In _Exclusions
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseExclusionNumForExclusionReconciliation = False Then
                        If e.HasValidExclusionNum = True Then
                            _CanUseExclusionNumForExclusionReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Sub SetTotalPremium() 'added 5/11/2015
            _TotalPremium = ""
            TotalPremium = qqHelper.getSum(_TotalPremium, _MainCoveragePremium)
            TotalPremium = qqHelper.getSum(_TotalPremium, _EarthquakePremium)
            TotalPremium = qqHelper.getSum(_TotalPremium, _PeakSeasonsPremium)
        End Sub
        Public Sub CheckCoveragesAndSetTotalPremium() 'added 5/11/2015
            CheckCoverage()
            CheckEarthquakeCoverage()
            ParseThruPeakSeasonCoverages()
            ParseThruExclusions()
            SetTotalPremium()
        End Sub


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _Coverage IsNot Nothing Then
                        _Coverage.Dispose()
                        _Coverage = Nothing
                    End If
                    If _EarthquakeCoverage IsNot Nothing Then
                        _EarthquakeCoverage.Dispose()
                        _EarthquakeCoverage = Nothing
                    End If
                    qqHelper.DisposeString(_UnscheduledFarmPersonalPropertyNum)
                    qqHelper.DisposeCoverages(_PeakSeasonCoverages) 'added 2/21/2015
                    If _Exclusions IsNot Nothing Then 'added 2/21/2015
                        If _Exclusions.Count > 0 Then
                            For Each e As QuickQuoteExclusion In _Exclusions
                                e.Dispose()
                                e = Nothing
                            Next
                            _Exclusions.Clear()
                        End If
                        _Exclusions = Nothing
                    End If
                    _CanUseExclusionNumForExclusionReconciliation = Nothing 'added 2/21/2015
                    _IncludeTheft = Nothing 'added 2/21/2015
                    _HasEarthquakeCoverage = Nothing 'added 2/21/2015
                    qqHelper.DisposeString(_Description) 'added 2/21/2015
                    'added 5/11/2015
                    If _PeakSeasons IsNot Nothing Then
                        If _PeakSeasons.Count > 0 Then
                            For Each ps As QuickQuotePeakSeason In _PeakSeasons
                                ps.Dispose()
                                ps = Nothing
                            Next
                            _PeakSeasons.Clear()
                        End If
                        _PeakSeasons = Nothing
                    End If
                    qqHelper.DisposeString(_IncreasedLimit)
                    qqHelper.DisposeString(_MainCoveragePremium)
                    qqHelper.DisposeString(_EarthquakePremium)
                    qqHelper.DisposeString(_PeakSeasonsPremium)
                    qqHelper.DisposeString(_TotalPremium)

                    'added 5/5/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
                    _IsLimitedPerilsExtendedCoverage = Nothing 'CoverageDetail

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
