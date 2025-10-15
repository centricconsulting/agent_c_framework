Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store loss history detail information
    ''' </summary>
    ''' <remarks>used with QuickQuoteLossHistoryRecord (<see cref="QuickQuoteLossHistoryRecord"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteLossHistoryDetailRecord
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        '*8/4/2012
        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Amount As String
        Private _AmountReserved As String
        Private _ChoicePointClaimTypeId As String
        Private _LossDate As String
        Private _LossDescription As String
        Private _TypeOfLossId As String
        Private _LossHistoryDetailNum As String 'added 7/3/2014

        'added 7/7/2014 to make sure we're capturing everything that could come back in 3rd party reports
        Private _ClaimTypeCoverageDscr As String
        'Private _LossHistoryNum As String
        'Private _LossHistoryStatusTypeId As String 'Byte (?)
        Private _TypeOfLoss As String

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
        Public Property Amount As String
            Get
                Return _Amount
            End Get
            Set(value As String)
                _Amount = value
            End Set
        End Property
        Public Property AmountReserved As String
            Get
                Return _AmountReserved
            End Get
            Set(value As String)
                _AmountReserved = value
            End Set
        End Property
        Public Property ChoicePointClaimTypeId As String
            Get
                Return _ChoicePointClaimTypeId
            End Get
            Set(value As String)
                _ChoicePointClaimTypeId = value
            End Set
        End Property
        Public Property LossDate As String
            Get
                Return _LossDate
            End Get
            Set(value As String)
                _LossDate = value
                qqHelper.ConvertToShortDate(_LossDate)
            End Set
        End Property
        Public Property LossDescription As String
            Get
                Return _LossDescription
            End Get
            Set(value As String)
                _LossDescription = value
            End Set
        End Property
        Public Property TypeOfLossId As String
            Get
                Return _TypeOfLossId
            End Get
            Set(value As String)
                _TypeOfLossId = value
            End Set
        End Property
        Public Property LossHistoryDetailNum As String 'added 7/3/2014
            Get
                Return _LossHistoryDetailNum
            End Get
            Set(value As String)
                _LossHistoryDetailNum = value
            End Set
        End Property

        'added 7/7/2014
        Public Property ClaimTypeCoverageDscr As String
            Get
                Return _ClaimTypeCoverageDscr
            End Get
            Set(value As String)
                _ClaimTypeCoverageDscr = value
            End Set
        End Property
        'Public Property LossHistoryNum As String
        'Public Property LossHistoryStatusTypeId As String 'Byte (?)
        Public Property TypeOfLoss As String
            Get
                Return _TypeOfLoss
            End Get
            Set(value As String)
                _TypeOfLoss = value
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
            _PolicyId = ""
            _PolicyImageNum = ""
            _Amount = ""
            _AmountReserved = ""
            _ChoicePointClaimTypeId = ""
            _LossDate = ""
            _LossDescription = ""
            _TypeOfLossId = ""
            _LossHistoryDetailNum = "" 'added 7/3/2014

            'added 7/7/2014
            _ClaimTypeCoverageDscr = ""
            '_LossHistoryNum = ""
            '_LossHistoryStatusTypeId = "" 'Byte (?)
            _TypeOfLoss = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidLossHistoryDetailNum() As Boolean 'added 7/3/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_LossHistoryDetailNum)
        End Function


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
                    If _Amount IsNot Nothing Then
                        _Amount = Nothing
                    End If
                    If _AmountReserved IsNot Nothing Then
                        _AmountReserved = Nothing
                    End If
                    If _ChoicePointClaimTypeId IsNot Nothing Then
                        _ChoicePointClaimTypeId = Nothing
                    End If
                    If _LossDate IsNot Nothing Then
                        _LossDate = Nothing
                    End If
                    If _LossDescription IsNot Nothing Then
                        _LossDescription = Nothing
                    End If
                    If _TypeOfLossId IsNot Nothing Then
                        _TypeOfLossId = Nothing
                    End If
                    If _LossHistoryDetailNum IsNot Nothing Then
                        _LossHistoryDetailNum = Nothing
                    End If

                    'added 7/7/2014
                    If _ClaimTypeCoverageDscr IsNot Nothing Then
                        _ClaimTypeCoverageDscr = Nothing
                    End If
                    '_LossHistoryNum = ""
                    '_LossHistoryStatusTypeId = "" 'Byte (?)
                    If _TypeOfLoss IsNot Nothing Then
                        _TypeOfLoss = Nothing
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
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
