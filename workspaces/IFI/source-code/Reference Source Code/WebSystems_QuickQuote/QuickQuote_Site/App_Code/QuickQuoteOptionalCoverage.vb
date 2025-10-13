Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store optional coverage information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteOptionalCoverage 'added 2/21/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        'added 5/12/2015
        Enum QuickQuoteOptionalCoverageType
            None

            'using coverage code desc
            Farm_4_H_and_FFAAnimals'80129; 4-H and FFA Animals
            Farm_F_G_Extra_Expense '80140; Extra Expense
            Farm_F_G_Farm_Sprinkler_Leakage '80143; Farm Sprinkler Leakage
            Farm_Property_in_Transit '70034; Property in Transit
            Farm_Sheep '80134; Sheep Additional Perils
            Farm_Suffocation_of_Livestock '70038; Suffocation of Livestock

            WindstormOrHail_DairyAndFarmProductsintheOpen '80116; Windstorm or Hail - Dairy and Farm Products in the Open

            Farm_Suffocation_of_Livestock_Cattle '80568
            Farm_Suffocation_of_Livestock_Equine '80569
            Farm_Suffocation_of_Livestock_Poultry '80567
            Farm_Suffocation_of_Livestock_Swine '80566
            'using caption
            '4_HAndFFAAnimals '80129; Farm_4-H_and_FFA Animals 'note: would need to fix this one as Enum value can't start w/ #
            'ExtraExpense '80140; Farm_F_G_Extra_Expense
            'FarmSprinklerLeakage '80143; Farm_F_G_Farm_Sprinkler_Leakage
            'PropertyinTransit '70034; Farm_Property_in_Transit
            'SheepAdditionalPerils '80134; Farm_Sheep
            'SuffocationofLivestock '70038; Farm_Suffocation_of_Livestock
            'WindstormOrHail_DairyAndFarmProductsintheOpen '80116; Windstorm or Hail - Dairy and Farm Products in the Open
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Coverage As QuickQuoteCoverage
        Private _Description As String
        'added 5/12/2015
        Private _CoverageType As QuickQuoteOptionalCoverageType
        Private _CoverageCodeId As String
        Private _IncreasedLimit As String
        Private _IncludedLimit As String 'added for Property in Transit... only option that doesn't allow increased limit... ManualLimitIncluded and ManualLimitAmount = 5000
        Private _OriginalCost As String
        Private _Premium As String
        Private _OptionalCoveragesNum As String 'added for reconciliation

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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        'added 5/12/2015
        Public Property CoverageType As QuickQuoteOptionalCoverageType
            Get
                Return _CoverageType
            End Get
            Set(value As QuickQuoteOptionalCoverageType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> QuickQuoteOptionalCoverageType.None Then
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(QuickQuoteOptionalCoverageType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
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
                    If System.Enum.TryParse(Of QuickQuoteOptionalCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = QuickQuoteOptionalCoverageType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteOptionalCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(QuickQuoteOptionalCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                End If
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
        Public Property IncludedLimit As String
            Get
                Return _IncludedLimit
            End Get
            Set(value As String)
                _IncludedLimit = value
                qqHelper.ConvertToLimitFormat(_IncludedLimit)
            End Set
        End Property
        Public Property OriginalCost As String
            Get
                Return _OriginalCost
            End Get
            Set(value As String)
                _OriginalCost = value
                qqHelper.ConvertToLimitFormat(_OriginalCost)
            End Set
        End Property
        Public Property Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Premium)
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property OptionalCoveragesNum As String 'added for reconciliation
            Get
                Return _OptionalCoveragesNum
            End Get
            Set(value As String)
                _OptionalCoveragesNum = value
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

        Public Property ExteriorDoorWindowSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property ExteriorWallSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property RoofSurfacing As Boolean 'added 06/26/2020 for Ohio Farm

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Coverage = New QuickQuoteCoverage
            _Description = ""
            'added 5/12/2015
            _CoverageType = QuickQuoteOptionalCoverageType.None
            _CoverageCodeId = ""
            _IncreasedLimit = ""
            _IncludedLimit = ""
            _OriginalCost = ""
            _Premium = ""
            _OptionalCoveragesNum = "" 'added for reconciliation

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Sub CheckCoverage()
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                'Select Case _Coverage.CoverageCodeId
                '    Case "70038" 'Edit: Farm_Suffocation_of_Livestock; example had ManualLimitAmount, ManualLimitIncreased, OriginalCost

                '    Case "80143" 'Edit: Farm_F_G_Farm_Sprinkler_Leakage; example had ManualLimitAmount, ManualLimitIncreased, OriginalCost

                '        'code generated from query 5/12/2015
                '    Case "80129" 'Edit: Farm_4-H_and_FFA Animals
                '    Case "80140" 'Edit: Farm_F_G_Extra_Expense
                '    Case "80143" 'Edit: Farm_F_G_Farm_Sprinkler_Leakage
                '    Case "70034" 'Edit: Farm_Property_in_Transit; note: doesn't allow IncreasedLimit... sets ManualLimitIncluded and ManualLimitAmount to 5000
                '    Case "80134" 'Edit: Farm_Sheep
                '    Case "70038" 'Edit: Farm_Suffocation_of_Livestock
                '    Case "80116" 'Edit: Windstorm or Hail - Dairy and Farm Products in the Open
                'End Select
                'updated 5/12/2015; note: these shouldn't be saved w/o CoverageCodeId or CoverageType
                CoverageCodeId = _Coverage.CoverageCodeId
                IncreasedLimit = Coverage.ManualLimitIncreased
                IncludedLimit = Coverage.ManualLimitIncluded
                OriginalCost = Coverage.OriginalCost
                Premium = Coverage.FullTermPremium
            End If
        End Sub
        Public Function HasValidOptionalCoveragesNum() As Boolean 'added 5/12/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_OptionalCoveragesNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim f As String = ""
                    f = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.CoverageType <> QuickQuoteOptionalCoverageType.None Then
                        f &= " (" & System.Enum.GetName(GetType(QuickQuoteOptionalCoverageType), Me.CoverageType) & ")"
                    End If
                    str = qqHelper.appendText(str, f, vbCrLf)
                End If
                If Me.Premium <> "" Then
                    str = qqHelper.appendText(str, "Premium: " & Me.Premium, vbCrLf)
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
                    qqHelper.DisposeString(_Description)
                    'added 5/12/2015
                    _CoverageType = Nothing
                    qqHelper.DisposeString(_CoverageCodeId)
                    qqHelper.DisposeString(_IncreasedLimit)
                    qqHelper.DisposeString(_IncludedLimit)
                    qqHelper.DisposeString(_OriginalCost)
                    qqHelper.DisposeString(_Premium)
                    qqHelper.DisposeString(_OptionalCoveragesNum) 'added for reconciliation

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
