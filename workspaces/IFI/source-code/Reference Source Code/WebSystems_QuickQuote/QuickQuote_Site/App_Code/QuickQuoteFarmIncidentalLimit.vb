Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store farm incidental limit information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteOjbect object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteFarmIncidentalLimit 'added 5/12/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Enum QuickQuoteFarmIncidentalLimitType
            None

            'using coverage code desc
            Farm_Debris_Removal '70149; Debris Removal
            Farm_Farm_Records '70151; Farm Records
            Farm_Fire_Department_Service_Charge '70142; Fire Department Service Charge
            Farm_Glass_Breakage_in_Cabs '70156; Glass Breakage in Cabs
            Farm_Pollutant_Clean_Up_and_Removal '70152; Pollutant Clean-Up and Removal
            Farm_Property_in_Care_Custody_or_Control_of_Common_Carrier '70155; Property in Care, Custody or Control of Common Carrier
            Farm_Signs '70159; Signs - All Other
            Farm_Signs_Electric '80114; Signs - Electric

            'using caption
            'DebrisRemoval '70149; Farm_Debris_Removal
            'FarmRecords '70151; Farm_Farm_Records
            'FireDepartmentServiceCharge '70142; Farm_Fire_Department_Service_Charge
            'GlassBreakageinCabs '70156; Farm_Glass_Breakage_in_Cabs
            'PollutantClean_UpAndRemoval '70152; Farm_Pollutant_Clean_Up_and_Removal
            'PropertyinCareCustodyOrControlofCommonCarrier '70155; Farm_Property_in_Care_Custody_or_Control_of_Common_Carrier
            'Signs_AllOther '70159; Farm_Signs
            'Signs_Electric '80114; Farm_Signs_Electric
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        'Private _Coverage As QuickQuoteCoverage
        Private _CoverageType As QuickQuoteFarmIncidentalLimitType
        Private _CoverageCodeId As String
        Private _IncreasedLimitId As String 'static data
        Private _IncreasedLimit As String
        Private _IncludedLimit As String
        Private _TotalLimit As String
        Private _Premium As String

        'Public Property Coverage As QuickQuoteCoverage
        '    Get
        '        Return _Coverage
        '    End Get
        '    Set(value As QuickQuoteCoverage)
        '        _Coverage = value
        '    End Set
        'End Property
        Public Property CoverageType As QuickQuoteFarmIncidentalLimitType
            Get
                Return _CoverageType
            End Get
            Set(value As QuickQuoteFarmIncidentalLimitType)
                _CoverageType = value
                If _CoverageType <> Nothing AndAlso _CoverageType <> QuickQuoteFarmIncidentalLimitType.None Then
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteFarmIncidentalLimit, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, System.Enum.GetName(GetType(QuickQuoteFarmIncidentalLimitType), _CoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
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
                    If System.Enum.TryParse(Of QuickQuoteFarmIncidentalLimitType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteFarmIncidentalLimit, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType), _CoverageType) = False Then
                        _CoverageType = QuickQuoteFarmIncidentalLimitType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteFarmIncidentalLimitType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteFarmIncidentalLimit, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType)) = True Then
                    '    _CoverageType = System.Enum.Parse(GetType(QuickQuoteFarmIncidentalLimitType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteFarmIncidentalLimit, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType))
                    'End If
                End If
            End Set
        End Property
        Public Property IncreasedLimitId As String 'static data
            Get
                Return _IncreasedLimitId
            End Get
            Set(value As String)
                _IncreasedLimitId = value
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
        Public Property TotalLimit As String
            Get
                Return _TotalLimit
            End Get
            Set(value As String)
                _TotalLimit = value
                qqHelper.ConvertToLimitFormat(_TotalLimit)
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

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_Coverage = New QuickQuoteCoverage
            _CoverageType = QuickQuoteFarmIncidentalLimitType.None
            _CoverageCodeId = ""
            _IncreasedLimitId = ""
            _IncreasedLimit = ""
            _IncludedLimit = ""
            _TotalLimit = ""
            _Premium = ""
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim f As String = ""
                    f = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.CoverageType <> QuickQuoteFarmIncidentalLimitType.None Then
                        f &= " (" & System.Enum.GetName(GetType(QuickQuoteFarmIncidentalLimitType), Me.CoverageType) & ")"
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
                    'qqHelper.DisposeCoverage(_Coverage)
                    _CoverageType = Nothing
                    qqHelper.DisposeString(_CoverageCodeId)
                    qqHelper.DisposeString(_IncreasedLimitId)
                    qqHelper.DisposeString(_IncreasedLimit)
                    qqHelper.DisposeString(_IncludedLimit)
                    qqHelper.DisposeString(_TotalLimit)
                    qqHelper.DisposeString(_Premium)

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
