Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'may not need

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store tiering information
    ''' </summary>
    ''' <remarks>related to 3rd party information and credit reports</remarks>
    <Serializable()> _
    Public Class QuickQuoteTieringInformation 'added 7/25/2014
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'may not need

        Private _Factor As String 'money in db; decimal in code
        Private _ManualTierReasonId As String 'int
        Private _NumberOfClaimsOverThreshold As String 'int
        Private _PriorBiLimits As String 'varchar 255 in db
        Private _RatedTier As String 'int
        Private _ReScoreAtRenewal As Boolean
        Private _TierAdjustmentTypeId As String 'int
        Private _TierLevelId As String 'int
        Private _TierOverride As Boolean
        Private _TieringInformationNum As String 'int
        Private _UnderwritingException As Boolean
        Private _UnitNum As String 'int
        Private _creditscore_num As String 'int

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property Factor As String
            Get
                Return _Factor
            End Get
            Set(value As String)
                _Factor = value
            End Set
        End Property
        Public Property ManualTierReasonId As String
            Get
                Return _ManualTierReasonId
            End Get
            Set(value As String)
                _ManualTierReasonId = value
            End Set
        End Property
        Public Property NumberOfClaimsOverThreshold As String
            Get
                Return _NumberOfClaimsOverThreshold
            End Get
            Set(value As String)
                _NumberOfClaimsOverThreshold = value
            End Set
        End Property
        Public Property PriorBiLimits As String
            Get
                Return _PriorBiLimits
            End Get
            Set(value As String)
                _PriorBiLimits = value
            End Set
        End Property
        Public Property RatedTier As String
            Get
                Return _RatedTier
            End Get
            Set(value As String)
                _RatedTier = value
            End Set
        End Property
        Public Property ReScoreAtRenewal As Boolean
            Get
                Return _ReScoreAtRenewal
            End Get
            Set(value As Boolean)
                _ReScoreAtRenewal = value
            End Set
        End Property
        Public Property TierAdjustmentTypeId As String
            Get
                Return _TierAdjustmentTypeId
            End Get
            Set(value As String)
                _TierAdjustmentTypeId = value
            End Set
        End Property
        Public Property TierLevelId As String
            Get
                Return _TierLevelId
            End Get
            Set(value As String)
                _TierLevelId = value
            End Set
        End Property
        Public Property TierOverride As Boolean
            Get
                Return _TierOverride
            End Get
            Set(value As Boolean)
                _TierOverride = value
            End Set
        End Property
        Public Property TieringInformationNum As String
            Get
                Return _TieringInformationNum
            End Get
            Set(value As String)
                _TieringInformationNum = value
            End Set
        End Property
        Public Property UnderwritingException As Boolean
            Get
                Return _UnderwritingException
            End Get
            Set(value As Boolean)
                _UnderwritingException = value
            End Set
        End Property
        Public Property UnitNum As String
            Get
                Return _UnitNum
            End Get
            Set(value As String)
                _UnitNum = value
            End Set
        End Property
        Public Property creditscore_num As String
            Get
                Return _creditscore_num
            End Get
            Set(value As String)
                _creditscore_num = value
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
            _Factor = ""
            _ManualTierReasonId = ""
            _NumberOfClaimsOverThreshold = ""
            _PriorBiLimits = ""
            _RatedTier = ""
            _ReScoreAtRenewal = False
            _TierAdjustmentTypeId = ""
            _TierLevelId = ""
            _TierOverride = False
            _TieringInformationNum = ""
            _UnderwritingException = False
            _UnitNum = ""
            _creditscore_num = ""

            _DetailStatusCode = "" 'added 5/15/2019
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
                    If _Factor IsNot Nothing Then
                        _Factor = Nothing
                    End If
                    If _ManualTierReasonId IsNot Nothing Then
                        _ManualTierReasonId = Nothing
                    End If
                    If _NumberOfClaimsOverThreshold IsNot Nothing Then
                        _NumberOfClaimsOverThreshold = Nothing
                    End If
                    If _PriorBiLimits IsNot Nothing Then
                        _PriorBiLimits = Nothing
                    End If
                    If _RatedTier IsNot Nothing Then
                        _RatedTier = Nothing
                    End If
                    If _ReScoreAtRenewal <> Nothing Then
                        _ReScoreAtRenewal = Nothing
                    End If
                    If _TierAdjustmentTypeId IsNot Nothing Then
                        _TierAdjustmentTypeId = Nothing
                    End If
                    If _TierLevelId IsNot Nothing Then
                        _TierLevelId = Nothing
                    End If
                    If _TierOverride <> Nothing Then
                        _TierOverride = Nothing
                    End If
                    If _TieringInformationNum IsNot Nothing Then
                        _TieringInformationNum = Nothing
                    End If
                    If _UnderwritingException <> Nothing Then
                        _UnderwritingException = Nothing
                    End If
                    If _UnitNum IsNot Nothing Then
                        _UnitNum = Nothing
                    End If
                    If _creditscore_num IsNot Nothing Then
                        _creditscore_num = Nothing
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
