Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store SAQ vehicle information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSAQImportedVehicle 'added 9/17/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AntiLockBrakeSystem As String
        Private _BICoverageLimitId As String
        Private _BasePrice As String
        Private _BodyStyle As String
        Private _CSLCoverageLimitId As String
        Private _ChoicePointTransmissionNum As String
        Private _CubicInchDisplacement As String
        Private _Make As String
        Private _Model As String
        Private _PDCoverageLimitId As String
        Private _PriceVariance As String
        Private _QuotebackGuid As String
        Private _ReceivedDate As String
        Private _Remarks As String
        Private _ReportedCoverageLimit As String
        Private _Restraints As String
        Private _SalvagedBranded As String
        Private _SaqImportedVehicleNum As String
        Private _SaqStatusTypeId As String
        Private _Security As String
        Private _Selected As Boolean
        Private _VehicleIndicator As String
        Private _Vin As String
        Private _Year As String
        'Private _entity

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
        Public Property AntiLockBrakeSystem As String
            Get
                Return _AntiLockBrakeSystem
            End Get
            Set(value As String)
                _AntiLockBrakeSystem = value
            End Set
        End Property
        Public Property BICoverageLimitId As String
            Get
                Return _BICoverageLimitId
            End Get
            Set(value As String)
                _BICoverageLimitId = value
            End Set
        End Property
        Public Property BasePrice As String
            Get
                Return _BasePrice
            End Get
            Set(value As String)
                _BasePrice = value
            End Set
        End Property
        Public Property BodyStyle As String
            Get
                Return _BodyStyle
            End Get
            Set(value As String)
                _BodyStyle = value
            End Set
        End Property
        Public Property CSLCoverageLimitId As String
            Get
                Return _CSLCoverageLimitId
            End Get
            Set(value As String)
                _CSLCoverageLimitId = value
            End Set
        End Property
        Public Property ChoicePointTransmissionNum As String
            Get
                Return _ChoicePointTransmissionNum
            End Get
            Set(value As String)
                _ChoicePointTransmissionNum = value
            End Set
        End Property
        Public Property CubicInchDisplacement As String
            Get
                Return _CubicInchDisplacement
            End Get
            Set(value As String)
                _CubicInchDisplacement = value
            End Set
        End Property
        Public Property Make As String
            Get
                Return _Make
            End Get
            Set(value As String)
                _Make = value
            End Set
        End Property
        Public Property Model As String
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        Public Property PDCoverageLimitId As String
            Get
                Return _PDCoverageLimitId
            End Get
            Set(value As String)
                _PDCoverageLimitId = value
            End Set
        End Property
        Public Property PriceVariance As String
            Get
                Return _PriceVariance
            End Get
            Set(value As String)
                _PriceVariance = value
            End Set
        End Property
        Public Property QuotebackGuid As String
            Get
                Return _QuotebackGuid
            End Get
            Set(value As String)
                _QuotebackGuid = value
            End Set
        End Property
        Public Property ReceivedDate As String
            Get
                Return _ReceivedDate
            End Get
            Set(value As String)
                _ReceivedDate = value
                qqHelper.ConvertToShortDate(_ReceivedDate)
            End Set
        End Property
        Public Property Remarks As String
            Get
                Return _Remarks
            End Get
            Set(value As String)
                _Remarks = value
            End Set
        End Property
        Public Property ReportedCoverageLimit As String
            Get
                Return _ReportedCoverageLimit
            End Get
            Set(value As String)
                _ReportedCoverageLimit = value
            End Set
        End Property
        Public Property Restraints As String
            Get
                Return _Restraints
            End Get
            Set(value As String)
                _Restraints = value
            End Set
        End Property
        Public Property SalvagedBranded As String
            Get
                Return _SalvagedBranded
            End Get
            Set(value As String)
                _SalvagedBranded = value
            End Set
        End Property
        Public Property SaqImportedVehicleNum As String
            Get
                Return _SaqImportedVehicleNum
            End Get
            Set(value As String)
                _SaqImportedVehicleNum = value
            End Set
        End Property
        Public Property SaqStatusTypeId As String
            Get
                Return _SaqStatusTypeId
            End Get
            Set(value As String)
                _SaqStatusTypeId = value
            End Set
        End Property
        Public Property Security As String
            Get
                Return _Security
            End Get
            Set(value As String)
                _Security = value
            End Set
        End Property
        Public Property Selected As Boolean
            Get
                Return _Selected
            End Get
            Set(value As Boolean)
                _Selected = value
            End Set
        End Property
        Public Property VehicleIndicator As String
            Get
                Return _VehicleIndicator
            End Get
            Set(value As String)
                _VehicleIndicator = value
            End Set
        End Property
        Public Property Vin As String
            Get
                Return _Vin
            End Get
            Set(value As String)
                _Vin = value
            End Set
        End Property
        Public Property Year As String
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _AntiLockBrakeSystem = ""
            _BICoverageLimitId = ""
            _BasePrice = ""
            _BodyStyle = ""
            _CSLCoverageLimitId = ""
            _ChoicePointTransmissionNum = ""
            _CubicInchDisplacement = ""
            _Make = ""
            _Model = ""
            _PDCoverageLimitId = ""
            _PriceVariance = ""
            _QuotebackGuid = ""
            _ReceivedDate = ""
            _Remarks = ""
            _ReportedCoverageLimit = ""
            _Restraints = ""
            _SalvagedBranded = ""
            _SaqImportedVehicleNum = ""
            _SaqStatusTypeId = ""
            _Security = ""
            _Selected = False
            _VehicleIndicator = ""
            _Vin = ""
            _Year = ""
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
                    If _AntiLockBrakeSystem IsNot Nothing Then
                        _AntiLockBrakeSystem = Nothing
                    End If
                    If _BICoverageLimitId IsNot Nothing Then
                        _BICoverageLimitId = Nothing
                    End If
                    If _BasePrice IsNot Nothing Then
                        _BasePrice = Nothing
                    End If
                    If _BodyStyle IsNot Nothing Then
                        _BodyStyle = Nothing
                    End If
                    If _CSLCoverageLimitId IsNot Nothing Then
                        _CSLCoverageLimitId = Nothing
                    End If
                    If _ChoicePointTransmissionNum IsNot Nothing Then
                        _ChoicePointTransmissionNum = Nothing
                    End If
                    If _CubicInchDisplacement IsNot Nothing Then
                        _CubicInchDisplacement = Nothing
                    End If
                    If _Make IsNot Nothing Then
                        _Make = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _PDCoverageLimitId IsNot Nothing Then
                        _PDCoverageLimitId = Nothing
                    End If
                    If _PriceVariance IsNot Nothing Then
                        _PriceVariance = Nothing
                    End If
                    If _QuotebackGuid IsNot Nothing Then
                        _QuotebackGuid = Nothing
                    End If
                    If _ReceivedDate IsNot Nothing Then
                        _ReceivedDate = Nothing
                    End If
                    If _Remarks IsNot Nothing Then
                        _Remarks = Nothing
                    End If
                    If _ReportedCoverageLimit IsNot Nothing Then
                        _ReportedCoverageLimit = Nothing
                    End If
                    If _Restraints IsNot Nothing Then
                        _Restraints = Nothing
                    End If
                    If _SalvagedBranded IsNot Nothing Then
                        _SalvagedBranded = Nothing
                    End If
                    If _SaqImportedVehicleNum IsNot Nothing Then
                        _SaqImportedVehicleNum = Nothing
                    End If
                    If _SaqStatusTypeId IsNot Nothing Then
                        _SaqStatusTypeId = Nothing
                    End If
                    If _Security IsNot Nothing Then
                        _Security = Nothing
                    End If
                    If _Selected <> Nothing Then
                        _Selected = Nothing
                    End If
                    If _VehicleIndicator IsNot Nothing Then
                        _VehicleIndicator = Nothing
                    End If
                    If _Vin IsNot Nothing Then
                        _Vin = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
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
