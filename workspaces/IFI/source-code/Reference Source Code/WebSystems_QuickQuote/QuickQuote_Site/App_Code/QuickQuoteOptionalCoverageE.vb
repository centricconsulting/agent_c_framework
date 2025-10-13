Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store optional coverage E information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteBuilding object (<see cref="QuickQuoteBuilding"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteOptionalCoverageE 'added 6/24/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Enum QuickQuoteOptionalCoverageEType
            None

            'added 6/25/2015
            'using coverage code desc
            Barn_Additional_Perils '80128; Additional Perils
            Barn_Earthquake '70160; Earthquake - Barns and Buildings
            Barn_Ordinance_or_Law '70166; Ordinance or Law
            Barn_Repair_or_Rebuilding_Requirement '70162; Repair or Rebuilding Requirement
            Barn_Replacement_Cost '70161; Replacement Cost
            Barn_Special_Form '70163; Special Form
            Barn_Theft_of_Building_Materials '70165; Theft of Building Materials
            Cosmetic_Damage_Exclusion_Coverage_E '80556
            Earthquake_Contents '80132; Earthquake - Contents
            Farm_Extra_Expense '70158; Extra Expense
            Farm_Mine_Subsidence '70026; Mine Subsidence
            Farm_Sprinkler_Leakage '80126; Farm Sprinkler Leakage
            LossOfIncome_Rents '80141; Loss of Income - Rents
            StructuresRentedtoOthers_Liability '20092; Structure Rented to Others  Liability

            'using caption
            'AdditionalPerils '80128; Barn_Additional_Perils
            'Earthquake_BarnsAndBuildings '70160; Barn_Earthquake
            'Earthquake_Contents '80132; Earthquake - Contents
            'ExtraExpense '70158; Farm_Extra_Expense
            'FarmSprinklerLeakage '80126; Farm_Sprinkler_Leakage
            'LossofIncome_Rents '80141; Loss Of Income - Rents
            'MineSubsidence '70026; Farm_Mine_Subsidence
            'OrdinanceOrLaw '70166; Barn_Ordinance_or_Law
            'RepairOrRebuildingRequirement '70162; Barn_Repair_or_Rebuilding_Requirement
            'ReplacementCost '70161; Barn_Replacement_Cost
            'SpecialForm '70163; Barn_Special_Form
            'StructureRentedtoOthersLiability '20092; Structures Rented to Others - Liability
            'TheftofBuildingMaterials '70165; Barn_Theft_of_Building_Materials

        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Coverage As QuickQuoteCoverage
        Private _Description As String
        Private _CoverageType As QuickQuoteOptionalCoverageEType
        Private _CoverageCodeId As String
        Private _IncreasedLimit As String
        'Private _IncludedLimit As String 'added for Property in Transit... only option that doesn't allow increased limit... ManualLimitIncluded and ManualLimitAmount = 5000
        'Private _OriginalCost As String
        Private _Premium As String
        Private _FarmBarnBuildingOptionalCoverageNum As String 'added for reconciliation

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
        Public Property CoverageType As QuickQuoteOptionalCoverageEType
            Get
                Return _CoverageType
            End Get
            Set(value As QuickQuoteOptionalCoverageEType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> QuickQuoteOptionalCoverageEType.None Then
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverageE, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(QuickQuoteOptionalCoverageEType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
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
                    If System.Enum.TryParse(Of QuickQuoteOptionalCoverageEType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverageE, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = QuickQuoteOptionalCoverageEType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteOptionalCoverageEType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverageE, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(QuickQuoteOptionalCoverageEType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteOptionalCoverageE, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
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
        'Public Property IncludedLimit As String
        '    Get
        '        Return _IncludedLimit
        '    End Get
        '    Set(value As String)
        '        _IncludedLimit = value
        '        qqHelper.ConvertToLimitFormat(_IncludedLimit)
        '    End Set
        'End Property
        'Public Property OriginalCost As String
        '    Get
        '        Return _OriginalCost
        '    End Get
        '    Set(value As String)
        '        _OriginalCost = value
        '        qqHelper.ConvertToLimitFormat(_OriginalCost)
        '    End Set
        'End Property
        Public Property Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Premium)
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property FarmBarnBuildingOptionalCoverageNum As String 'added for reconciliation
            Get
                Return _FarmBarnBuildingOptionalCoverageNum
            End Get
            Set(value As String)
                _FarmBarnBuildingOptionalCoverageNum = value
            End Set
        End Property

        Public Property ExteriorDoorWindowSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property ExteriorWallSurfacing As Boolean 'added 06/26/2020 for Ohio Farm
        Public Property RoofSurfacing As Boolean 'added 06/26/2020 for Ohio Farm

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
            _Description = ""
            _CoverageType = QuickQuoteOptionalCoverageEType.None
            _CoverageCodeId = ""
            _IncreasedLimit = ""
            '_IncludedLimit = ""
            '_OriginalCost = ""
            _Premium = ""
            _FarmBarnBuildingOptionalCoverageNum = "" 'added for reconciliation

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Sub CheckCoverage()
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                CoverageCodeId = _Coverage.CoverageCodeId
                IncreasedLimit = Coverage.ManualLimitIncreased
                'IncludedLimit = Coverage.ManualLimitIncluded
                'OriginalCost = Coverage.OriginalCost
                Description = Coverage.Description
                Premium = Coverage.FullTermPremium
            End If
        End Sub
        Public Function HasValidFarmBarnBuildingOptionalCoverageNum() As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(_FarmBarnBuildingOptionalCoverageNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim f As String = ""
                    f = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.CoverageType <> QuickQuoteOptionalCoverageEType.None Then
                        f &= " (" & System.Enum.GetName(GetType(QuickQuoteOptionalCoverageEType), Me.CoverageType) & ")"
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
                    _CoverageType = Nothing
                    qqHelper.DisposeString(_CoverageCodeId)
                    qqHelper.DisposeString(_IncreasedLimit)
                    'qqHelper.DisposeString(_IncludedLimit)
                    'qqHelper.DisposeString(_OriginalCost)
                    qqHelper.DisposeString(_Premium)
                    qqHelper.DisposeString(_FarmBarnBuildingOptionalCoverageNum) 'added for reconciliation

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
