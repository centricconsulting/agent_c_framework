Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store scheduled personal property coverage information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteScheduledPersonalPropertyCoverage 'added 2/19/2015
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        'added 5/8/2015
        Enum QuickQuoteScheduledPersonalPropertyCoverageType
            None

            'using coverage code desc
            ATV '80120; ATV
            DirtBikes '80117; Dirt Bikes
            FarmMachineryDescribed_OpenPerils '80121; Farm Machinery Described - Open Perils
            Farm_F_Computers '70060; Computers (farm use only)
            Farm_F_Earthquake '40086; Coverage F - Earthquake; 5/8/2015 note: this one should not pull up in list; only used for EQ cov for any of the other coverageTypes
            Farm_F_Farm_Products_and_Supplies '70059; Farm Products and Supplies
            Farm_F_Grain '70056; Grain
            Farm_F_Hay_in_Barn '70057; Hay
            Farm_F_Hay_in_the_Open '70058; Hay in the Open
            Farm_F_Irrigation_Equipment '70063; Irrigation Equipment
            Farm_F_Tobacco '70062; Tobacco
            Farm_F_Windmills_and_Windchargers '70061; Windmills and Wind chargers
            Farm_Rented_or_borrowed_Equipment '70153; Rented or Borrowed Equipment
            GrainintheOpen '80131; Grain in the Open
            Livestock '80122; Livestock
            LocationF_DescribedMachinery '40029; Farm Machinery Described
            LocationF_MachineryNotDescribed '40030; Machinery Not Described
            MiscellaneousFarmPersonalProperty '80118; Miscellaneous Farm Personal Property
            Poultry '80124; Poultry
            ReproductiveMaterials '80119; Reproductive Materials
            ShowHorsesAndShowPonies '80123; Show Horses and Show Ponies

            'using caption
            'ATV '80120; ATV
            'Computers_farmuseonly '70060; Farm_F_Computers
            'CoverageF_Earthquake '40086; Farm_F_Earthquake; 5/8/2015 note: this one should not pull up in list; only used for EQ cov for any of the other coverageTypes
            'DirtBikes '80117; Dirt Bikes
            'FarmMachineryDescribed '40029; Location F. Described Machinery
            'FarmMachineryDescribed_OpenPerils '80121; Farm Machinery Described - Open Perils
            'FarmProductsAndSupplies '70059; Farm_F_Farm_Products_and_Supplies
            'Grain '70056; Farm_F_Grain
            'GrainintheOpen '80131; Grain in the Open
            'Hay '70057; Farm_F_Hay_in_Barn
            'HayintheOpen '70058; Farm_F_Hay_in_the_Open
            'IrrigationEquipment '70063; Farm_F_Irrigation_Equipment
            'Livestock '80122; Livestock
            'MachineryNotDescribed '40030; Location F. Machinery Not Described
            'MiscellaneousFarmPersonalProperty '80118; Miscellaneous Farm Personal Property
            'Poultry '80124; Poultry
            'RentedOrBorrowedEquipment '70153; Farm_Rented_or_borrowed_Equipment
            'ReproductiveMaterials '80119; Reproductive Materials
            'ShowHorsesAndShowPonies '80123; Show Horses and Show Ponies
            'Tobacco '70062; Farm_F_Tobacco
            'WindmillsAndWindchargers '70061; Farm_F_Windmills_and_Windchargers
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Coverage As QuickQuoteCoverage
        Private _EarthquakeCoverage As QuickQuoteCoverage
        Private _ScheduledFarmPersonalPropertyNum As String 'for reconciliation
        Private _PeakSeasonCoverages As List(Of QuickQuoteCoverage) 'added 2/21/2015
        Private _HasEarthquakeCoverage As Boolean 'added 2/21/2015

        'added 5/8/2015
        Private _CoverageType As QuickQuoteScheduledPersonalPropertyCoverageType
        Private _CoverageCodeId As String
        'added 5/11/2015
        Private _PeakSeasons As List(Of QuickQuotePeakSeason)
        Private _IncreasedLimit As String
        Private _Description As String
        Private _MainCoveragePremium As String
        Private _EarthquakePremium As String
        Private _PeakSeasonsPremium As String
        Private _TotalPremium As String

        Private _DetailStatusCode As String 'added 5/1/2017 since these may stay on rated image after flagging for delete

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
        Public Property ScheduledFarmPersonalPropertyNum As String 'for reconciliation
            Get
                Return _ScheduledFarmPersonalPropertyNum
            End Get
            Set(value As String)
                _ScheduledFarmPersonalPropertyNum = value
            End Set
        End Property
        Public Property PeakSeasonCoverages As List(Of QuickQuoteCoverage) 'added 2/21/2015
            Get
                SetObjectsParent(_PeakSeasonCoverages)
                Return _PeakSeasonCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _PeakSeasonCoverages = value
                SetObjectsParent(_PeakSeasonCoverages)
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

        'added 5/8/2015
        Public Property CoverageType As QuickQuoteScheduledPersonalPropertyCoverageType
            Get
                Return _CoverageType
            End Get
            Set(value As QuickQuoteScheduledPersonalPropertyCoverageType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> QuickQuoteScheduledPersonalPropertyCoverageType.None Then
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteScheduledPersonalPropertyCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(QuickQuoteScheduledPersonalPropertyCoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                End If
            End Set
        End Property
        Public Property CoverageCodeId As String
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
                If IsNumeric(_CoverageCodeId) = True AndAlso _CoverageCodeId <> "0" Then
                    If System.Enum.TryParse(Of QuickQuoteScheduledPersonalPropertyCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteScheduledPersonalPropertyCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = QuickQuoteScheduledPersonalPropertyCoverageType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteScheduledPersonalPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteScheduledPersonalPropertyCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(QuickQuoteScheduledPersonalPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteScheduledPersonalPropertyCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                End If
            End Set
        End Property
        'added 5/11/2015
        Public Property PeakSeasons As List(Of QuickQuotePeakSeason)
            Get
                SetParentOfListItems(_PeakSeasons, "{663B7C7B-F2AC-4BF6-965A-D30F41A05622}")
                Return _PeakSeasons
            End Get
            Set(value As List(Of QuickQuotePeakSeason))
                _PeakSeasons = value
                SetParentOfListItems(_PeakSeasons, "{663B7C7B-F2AC-4BF6-965A-D30F41A05622}")
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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
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

        Public Property DetailStatusCode As String 'added 5/1/2017 since these may stay on rated image after flagging for delete
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
            _ScheduledFarmPersonalPropertyNum = ""
            '_PeakSeasonCoverages = New List(Of QuickQuoteCoverage) 'added 2/21/2015
            _PeakSeasonCoverages = Nothing
            _HasEarthquakeCoverage = False 'added 2/21/2015

            'added 5/8/2015
            _CoverageType = QuickQuoteScheduledPersonalPropertyCoverageType.None
            _CoverageCodeId = ""
            'added 5/11/2015
            '_PeakSeasons = New List(Of QuickQuotePeakSeason)
            _PeakSeasons = Nothing
            _IncreasedLimit = ""
            _Description = ""
            _MainCoveragePremium = ""
            _EarthquakePremium = ""
            _PeakSeasonsPremium = ""
            _TotalPremium = ""

            _DetailStatusCode = "" 'added 5/1/2017 since these may stay on rated image after flagging for delete
        End Sub
        Public Sub CheckCoverage()
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                'Select Case _Coverage.CoverageCodeId
                '    Case "70060" 'Edit: Farm_F_Computers; ManualLimitAmount, ManualLimitIncreased, CoverageLimitId (-1 in example), Description on example

                '    Case "80131" 'Edit: Grain in the Open; added 2/21/2015 from example xml... ManualLimitAmount, ManualLimitIncreased, CoverageLimitId (-1 in example), Description on example

                '    Case "80122" 'Edit: Livestock; added 2/21/2015 from example xml... ManualLimitAmount, ManualLimitIncreased, CoverageLimitId (-1 in example), Description on example

                '    Case "70058" 'Edit: Farm_F_Hay_in_the_Open; added 5/7/2015 from example xml... ManualLimitAmount, ManualLimitIncreased, Description; no CoverageLimitId

                '    Case "80120" 'Edit: ATV; added 5/7/2015 from example xml... ManualLimitAmount, ManualLimitIncreased, Description; no CoverageLimitId

                'End Select
                'updated 5/8/2015; note: these shouldn't be saved w/o CoverageCodeId or CoverageType
                CoverageCodeId = _Coverage.CoverageCodeId
                'added 5/11/2015
                IncreasedLimit = Coverage.ManualLimitIncreased
                Description = Coverage.Description
                MainCoveragePremium = Coverage.FullTermPremium
            End If
        End Sub
        Public Sub CheckEarthquakeCoverage()
            If _EarthquakeCoverage IsNot Nothing AndAlso _EarthquakeCoverage.CoverageCodeId <> "" Then
                Select Case _EarthquakeCoverage.CoverageCodeId
                    Case "40086" 'CheckBox: Farm_F_Earthquake; no special fields set on example... cov must not have been there since there was no Checkbox node; 2/21/2015 note: Checkbox = true when present
                        HasEarthquakeCoverage = _EarthquakeCoverage.Checkbox 'added 2/21/2015; note: doesn't necessarily need to check CoverageCodeId
                        If _HasEarthquakeCoverage = True Then 'added 5/11/2015
                            EarthquakePremium = _EarthquakeCoverage.FullTermPremium
                        End If
                End Select
            End If
        End Sub
        Public Function HasValidScheduledFarmPersonalPropertyNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledFarmPersonalPropertyNum)
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
            SetTotalPremium()
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim c As String = ""
                    c = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.CoverageType <> QuickQuoteScheduledPersonalPropertyCoverageType.None Then
                        c &= " (" & System.Enum.GetName(GetType(QuickQuoteScheduledPersonalPropertyCoverageType), Me.CoverageType) & ")"
                    End If
                    str = qqHelper.appendText(str, c, vbCrLf)
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
                    qqHelper.DisposeString(_ScheduledFarmPersonalPropertyNum)
                    qqHelper.DisposeCoverages(_PeakSeasonCoverages) 'added 2/21/2015
                    _HasEarthquakeCoverage = Nothing 'added 2/21/2015

                    'added 5/8/2015
                    _CoverageType = Nothing
                    qqHelper.DisposeString(_CoverageCodeId)
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
                    qqHelper.DisposeString(_Description)
                    qqHelper.DisposeString(_MainCoveragePremium)
                    qqHelper.DisposeString(_EarthquakePremium)
                    qqHelper.DisposeString(_PeakSeasonsPremium)
                    qqHelper.DisposeString(_TotalPremium)

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/1/2017 since these may stay on rated image after flagging for delete

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
