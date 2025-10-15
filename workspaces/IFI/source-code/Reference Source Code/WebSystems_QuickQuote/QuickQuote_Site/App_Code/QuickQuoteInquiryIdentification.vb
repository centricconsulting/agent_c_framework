Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store inquiry identification information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteInquiryIdentification 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _ChptAccountNumber As String
        Private _ChptAccountSuffix As String
        Private _ChptIncProductLine As String
        Private _DateOfOrder As String
        Private _ProductRequested As String
        Private _Quoteback As String
        Private _RecordVersionNumber As String
        Private _ReportCode As String
        Private _ReportType As String
        Private _ReportUsage As String
        Private _RequestHandlingIndicator As String
        Private _SpecialBillingId As String
        Private _UnitNumber As String

        Public Property ChptAccountNumber As String
            Get
                Return _ChptAccountNumber
            End Get
            Set(value As String)
                _ChptAccountNumber = value
            End Set
        End Property
        Public Property ChptAccountSuffix As String
            Get
                Return _ChptAccountSuffix
            End Get
            Set(value As String)
                _ChptAccountSuffix = value
            End Set
        End Property
        Public Property ChptIncProductLine As String
            Get
                Return _ChptIncProductLine
            End Get
            Set(value As String)
                _ChptIncProductLine = value
            End Set
        End Property
        Public Property DateOfOrder As String
            Get
                Return _DateOfOrder
            End Get
            Set(value As String)
                _DateOfOrder = value
                qqHelper.ConvertToShortDate(_DateOfOrder)
            End Set
        End Property
        Public Property ProductRequested As String
            Get
                Return _ProductRequested
            End Get
            Set(value As String)
                _ProductRequested = value
            End Set
        End Property
        Public Property Quoteback As String
            Get
                Return _Quoteback
            End Get
            Set(value As String)
                _Quoteback = value
            End Set
        End Property
        Public Property RecordVersionNumber As String
            Get
                Return _RecordVersionNumber
            End Get
            Set(value As String)
                _RecordVersionNumber = value
            End Set
        End Property
        Public Property ReportCode As String
            Get
                Return _ReportCode
            End Get
            Set(value As String)
                _ReportCode = value
            End Set
        End Property
        Public Property ReportType As String
            Get
                Return _ReportType
            End Get
            Set(value As String)
                _ReportType = value
            End Set
        End Property
        Public Property ReportUsage As String
            Get
                Return _ReportUsage
            End Get
            Set(value As String)
                _ReportUsage = value
            End Set
        End Property
        Public Property RequestHandlingIndicator As String
            Get
                Return _RequestHandlingIndicator
            End Get
            Set(value As String)
                _RequestHandlingIndicator = value
            End Set
        End Property
        Public Property SpecialBillingId As String
            Get
                Return _SpecialBillingId
            End Get
            Set(value As String)
                _SpecialBillingId = value
            End Set
        End Property
        Public Property UnitNumber As String
            Get
                Return _UnitNumber
            End Get
            Set(value As String)
                _UnitNumber = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _ChptAccountNumber = ""
            _ChptAccountSuffix = ""
            _ChptIncProductLine = ""
            _DateOfOrder = ""
            _ProductRequested = ""
            _Quoteback = ""
            _RecordVersionNumber = ""
            _ReportCode = ""
            _ReportType = ""
            _ReportUsage = ""
            _RequestHandlingIndicator = ""
            _SpecialBillingId = ""
            _UnitNumber = ""
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
                    If _ChptAccountNumber IsNot Nothing Then
                        _ChptAccountNumber = Nothing
                    End If
                    If _ChptAccountSuffix IsNot Nothing Then
                        _ChptAccountSuffix = Nothing
                    End If
                    If _ChptIncProductLine IsNot Nothing Then
                        _ChptIncProductLine = Nothing
                    End If
                    If _DateOfOrder IsNot Nothing Then
                        _DateOfOrder = Nothing
                    End If
                    If _ProductRequested IsNot Nothing Then
                        _ProductRequested = Nothing
                    End If
                    If _Quoteback IsNot Nothing Then
                        _Quoteback = Nothing
                    End If
                    If _RecordVersionNumber IsNot Nothing Then
                        _RecordVersionNumber = Nothing
                    End If
                    If _ReportCode IsNot Nothing Then
                        _ReportCode = Nothing
                    End If
                    If _ReportType IsNot Nothing Then
                        _ReportType = Nothing
                    End If
                    If _ReportUsage IsNot Nothing Then
                        _ReportUsage = Nothing
                    End If
                    If _RequestHandlingIndicator IsNot Nothing Then
                        _RequestHandlingIndicator = Nothing
                    End If
                    If _SpecialBillingId IsNot Nothing Then
                        _SpecialBillingId = Nothing
                    End If
                    If _UnitNumber IsNot Nothing Then
                        _UnitNumber = Nothing
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
