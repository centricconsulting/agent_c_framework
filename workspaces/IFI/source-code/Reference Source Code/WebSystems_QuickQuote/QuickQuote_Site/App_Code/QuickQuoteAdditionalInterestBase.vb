Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to hold base additional interest information
    ''' </summary>
    ''' <remarks>AIs can be tied to vehicles, buildings, etc.; base object holds everything except for aiList info (name/address/etc.)</remarks>
    <Serializable()>
    Public Class QuickQuoteAdditionalInterestBase 'added 4/1/2020
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim chc As New CommonHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _ATIMA As Boolean

        Private _Num As String 'started using 4/29/2014 for reconciliation
        Private _TypeId As String 'Diamond table AdditionalInterestType
        Private _TypeName As QuickQuoteAdditionalInterest.AdditionalInterestType = QuickQuoteAdditionalInterest.AdditionalInterestType.NA
        Private _BillTo As Boolean
        Private _Description As String
        Private _HasWaiverOfSubrogation As Boolean
        Private _ISAOA As Boolean
        Private _InterestInProperty As String
        Private _LoanAmount As String
        Private _LoanNumber As String
        Private _Other As String
        Private _TrustAgreementDate As String

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 12/23/2020
        Private _AddedDate As String
        Private _AddedImageNum As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String

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
        Public Property ATIMA As Boolean
            Get
                Return _ATIMA
            End Get
            Set(value As Boolean)
                _ATIMA = value
            End Set
        End Property

        Public Property Num As String 'started using 4/29/2014 for reconciliation
            Get
                Return _Num
            End Get
            Set(value As String)
                _Num = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's AdditionalInterestType table</remarks>
        Public Property TypeId As String '9/26/2013 note: AdditionalInterestType table
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
                If chc.IsNumericString(_TypeId) AndAlso chc.NumericStringComparison(_TypeId, CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                    _TypeName = CInt(_TypeId)
                Else
                    _TypeName = QuickQuoteAdditionalInterest.AdditionalInterestType.NA
                End If
            End Set
        End Property

        Public Property TypeName As QuickQuoteAdditionalInterest.AdditionalInterestType
            Get
                Return _TypeName
            End Get
            Set(value As QuickQuoteAdditionalInterest.AdditionalInterestType)
                _TypeName = value
                _TypeId = _TypeName
            End Set
        End Property
        Public Property BillTo As Boolean
            Get
                Return _BillTo
            End Get
            Set(value As Boolean)
                _BillTo = value
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
        Public Property HasWaiverOfSubrogation As Boolean
            Get
                Return _HasWaiverOfSubrogation
            End Get
            Set(value As Boolean)
                _HasWaiverOfSubrogation = value
            End Set
        End Property
        Public Property ISAOA As Boolean
            Get
                Return _ISAOA
            End Get
            Set(value As Boolean)
                _ISAOA = value
            End Set
        End Property
        Public Property InterestInProperty As String
            Get
                Return _InterestInProperty
            End Get
            Set(value As String)
                _InterestInProperty = value
            End Set
        End Property
        Public Property LoanAmount As String
            Get
                Return _LoanAmount
            End Get
            Set(value As String)
                _LoanAmount = value
            End Set
        End Property
        Public Property LoanNumber As String
            Get
                Return _LoanNumber
            End Get
            Set(value As String)
                _LoanNumber = value
            End Set
        End Property
        Public Property Other As String
            Get
                Return _Other
            End Get
            Set(value As String)
                _Other = value
            End Set
        End Property
        Public Property TrustAgreementDate As String
            Get
                Return _TrustAgreementDate
            End Get
            Set(value As String)
                _TrustAgreementDate = value
                qqHelper.ConvertToShortDate(_TrustAgreementDate)
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

        'added 12/23/2020
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property AddedImageNum As String
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property


        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _ATIMA = False

            _Num = ""
            _TypeId = ""
            _BillTo = False
            _Description = ""
            _HasWaiverOfSubrogation = False
            _ISAOA = False
            _InterestInProperty = ""
            _LoanAmount = ""
            _LoanNumber = ""
            _Other = ""
            _TrustAgreementDate = ""

            _DetailStatusCode = "" 'added 5/15/2019

            'added 12/23/2020
            _AddedDate = ""
            _AddedImageNum = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
        End Sub
        Public Function HasValidAdditionalInterestNum() As Boolean 'added 4/29/2014
            Return qqHelper.IsValidQuickQuoteIdOrNum(_Num)
        End Function
        Public Function HasInfo() As Boolean 'added 4/1/2020
            If qqHelper.IsPositiveIntegerString(_PolicyId) = True OrElse qqHelper.IsPositiveIntegerString(_PolicyImageNum) = True OrElse _ATIMA = True OrElse HasValidAdditionalInterestNum() = True OrElse qqHelper.IsPositiveIntegerString(_TypeId) = True OrElse _BillTo = True OrElse String.IsNullOrWhiteSpace(_Description) = False OrElse _HasWaiverOfSubrogation = True OrElse _ISAOA = True OrElse qqHelper.IsPositiveDecimalString(_InterestInProperty) = True OrElse qqHelper.IsPositiveDecimalString(_LoanAmount) = True OrElse String.IsNullOrWhiteSpace(_LoanNumber) = False OrElse String.IsNullOrWhiteSpace(_Other) = False OrElse qqHelper.IsValidDateString(_TrustAgreementDate, mustBeGreaterThanDefaultDate:=True) = True Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.TypeId <> "" Then
                    Dim ai As String = ""
                    ai = "TypeId: " & Me.TypeId
                    Dim aiType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, Me.TypeId)
                    If aiType <> "" Then
                        ai &= " (" & aiType & ")"
                    End If
                    str = qqHelper.appendText(str, ai, vbCrLf)
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
                    If _ATIMA <> Nothing Then
                        _ATIMA = Nothing
                    End If

                    If _Num IsNot Nothing Then
                        _Num = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If
                    If _BillTo <> Nothing Then
                        _BillTo = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _HasWaiverOfSubrogation <> Nothing Then
                        _HasWaiverOfSubrogation = Nothing
                    End If
                    If _ISAOA <> Nothing Then
                        _ISAOA = Nothing
                    End If
                    If _InterestInProperty IsNot Nothing Then
                        _InterestInProperty = Nothing
                    End If
                    If _LoanAmount IsNot Nothing Then
                        _LoanAmount = Nothing
                    End If
                    If _LoanNumber IsNot Nothing Then
                        _LoanNumber = Nothing
                    End If
                    If _Other IsNot Nothing Then
                        _Other = Nothing
                    End If
                    If _TrustAgreementDate IsNot Nothing Then
                        _TrustAgreementDate = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 12/23/2020
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_AddedImageNum)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)

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
