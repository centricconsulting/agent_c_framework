Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store income loss information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation object (<see cref="QuickQuoteLocation"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteIncomeLoss 'added 2/25/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _CoinsuranceTypeId As String 'static data
        Private _Coverage As QuickQuoteCoverage
        Private _ExtendFarmIncomeOptionId As String 'static data; example has - 1
        Private _LossOfIncomeNum As String 'for reconciliation
        'added 12/11/2015
        Private _Limit As String
        Private _Description As String
        Private _QuotedPremium As String

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property CoinsuranceTypeId As String
            Get
                Return _CoinsuranceTypeId
            End Get
            Set(value As String)
                _CoinsuranceTypeId = value
            End Set
        End Property
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
        Public Property ExtendFarmIncomeOptionId As String
            Get
                Return _ExtendFarmIncomeOptionId
            End Get
            Set(value As String)
                _ExtendFarmIncomeOptionId = value
            End Set
        End Property
        Public Property LossOfIncomeNum As String
            Get
                Return _LossOfIncomeNum
            End Get
            Set(value As String)
                _LossOfIncomeNum = value
            End Set
        End Property
        'added 12/11/2015
        Public Property Limit As String
            Get
                Return _Limit
            End Get
            Set(value As String)
                _Limit = value
                qqHelper.ConvertToLimitFormat(_Limit)
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
        Public Property QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_QuotedPremium)
            End Get
            Set(value As String)
                _QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_QuotedPremium)
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
            _CoinsuranceTypeId = ""
            _Coverage = New QuickQuoteCoverage
            _ExtendFarmIncomeOptionId = ""
            _LossOfIncomeNum = ""
            'added 12/11/2015
            _Limit = ""
            _Description = ""
            _QuotedPremium = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidLossOfIncomeNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_LossOfIncomeNum)
        End Function
        Public Sub CheckCoverage()
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                Select Case _Coverage.CoverageCodeId
                    Case "70212" 'Edit: Loss Of Income; example had ManualLimitAmount, ManualLimitIncreased, and Description
                        '12/11/2015 note: this should be the coverage associated w/ all IncomeLosses
                        Limit = _Coverage.ManualLimitAmount 'entered limit is pushed into ManualLimitAmount and ManualLimitAmount
                        Description = _Coverage.Description
                        QuotedPremium = _Coverage.FullTermPremium
                End Select
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 12/11/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Limit <> "" Then
                    str = qqHelper.appendText(str, "Limit: " & Me.Limit, vbCrLf)
                End If
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
                End If
                If Me.QuotedPremium <> "" Then
                    str = qqHelper.appendText(str, "QuotedPremium: " & Me.QuotedPremium, vbCrLf)
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
                    qqHelper.DisposeString(_CoinsuranceTypeId)
                    qqHelper.DisposeCoverage(_Coverage)
                    qqHelper.DisposeString(_ExtendFarmIncomeOptionId)
                    qqHelper.DisposeString(_LossOfIncomeNum)
                    'added 12/11/2015
                    qqHelper.DisposeString(_Limit)
                    qqHelper.DisposeString(_Description)
                    qqHelper.DisposeString(_QuotedPremium)

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
