Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store credit score information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteCreditScore 'added 9/17/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _BankruptcyForeclosureIndicator As Boolean
        'CreditScoreNum
        'CreditScoreReasons
        Private _DateOfLatestBankruptcy As String
        Private _DateOfLatestReposession As String 'spelled to match Diamond xml; doesn't match _RepossessionIndicator below
        Private _Description As String
        'ExclusionCode
        'ExternallyGenerated as Boolean
        'Model
        Private _NonCreditDate As String
        Private _PersonalAutoReportingBureausTypeId As String 'may need matching PersonalAutoReportingBureausType variable/property
        'ReferenceNumber
        Private _ReportDate As String
        Private _RepossessionIndicator As Boolean 'spelled to match Diamond xml; doesn't match _DateOfLatestReposession above
        Private _RiskLevel As String
        Private _Score As String
        Private _UnitNum As String
        'external_id

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
        Public Property BankruptcyForeclosureIndicator As Boolean
            Get
                Return _BankruptcyForeclosureIndicator
            End Get
            Set(value As Boolean)
                _BankruptcyForeclosureIndicator = value
            End Set
        End Property
        Public Property DateOfLatestBankruptcy As String
            Get
                Return _DateOfLatestBankruptcy
            End Get
            Set(value As String)
                _DateOfLatestBankruptcy = value
                qqHelper.ConvertToShortDate(_DateOfLatestBankruptcy)
            End Set
        End Property
        Public Property DateOfLatestReposession As String 'spelled to match Diamond xml; doesn't match RepossessionIndicator below
            Get
                Return _DateOfLatestReposession
            End Get
            Set(value As String)
                _DateOfLatestReposession = value
                qqHelper.ConvertToShortDate(_DateOfLatestReposession)
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
        Public Property NonCreditDate As String
            Get
                Return _NonCreditDate
            End Get
            Set(value As String)
                _NonCreditDate = value
                qqHelper.ConvertToShortDate(_NonCreditDate)
            End Set
        End Property
        Public Property PersonalAutoReportingBureausTypeId As String
            Get
                Return _PersonalAutoReportingBureausTypeId
            End Get
            Set(value As String)
                _PersonalAutoReportingBureausTypeId = value
            End Set
        End Property
        Public Property ReportDate As String
            Get
                Return _ReportDate
            End Get
            Set(value As String)
                _ReportDate = value
                qqHelper.ConvertToShortDate(_ReportDate)
            End Set
        End Property
        Public Property RepossessionIndicator As Boolean 'spelled to match Diamond xml; doesn't match DateOfLatestReposession above
            Get
                Return _RepossessionIndicator
            End Get
            Set(value As Boolean)
                _RepossessionIndicator = value
            End Set
        End Property
        Public Property RiskLevel As String
            Get
                Return _RiskLevel
            End Get
            Set(value As String)
                _RiskLevel = value
            End Set
        End Property
        Public Property Score As String
            Get
                Return _Score
            End Get
            Set(value As String)
                _Score = value
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

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _BankruptcyForeclosureIndicator = False
            _DateOfLatestBankruptcy = ""
            _DateOfLatestReposession = ""
            _Description = ""
            _NonCreditDate = ""
            _PersonalAutoReportingBureausTypeId = ""
            _ReportDate = ""
            _RepossessionIndicator = False
            _RiskLevel = ""
            _Score = ""
            _UnitNum = ""
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
                    If _BankruptcyForeclosureIndicator <> Nothing Then
                        _BankruptcyForeclosureIndicator = Nothing
                    End If
                    If _DateOfLatestBankruptcy IsNot Nothing Then
                        _DateOfLatestBankruptcy = Nothing
                    End If
                    If _DateOfLatestReposession IsNot Nothing Then
                        _DateOfLatestReposession = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _NonCreditDate IsNot Nothing Then
                        _NonCreditDate = Nothing
                    End If
                    If _PersonalAutoReportingBureausTypeId IsNot Nothing Then
                        _PersonalAutoReportingBureausTypeId = Nothing
                    End If
                    If _ReportDate IsNot Nothing Then
                        _ReportDate = Nothing
                    End If
                    If _RepossessionIndicator <> Nothing Then
                        _RepossessionIndicator = Nothing
                    End If
                    If _RiskLevel IsNot Nothing Then
                        _RiskLevel = Nothing
                    End If
                    If _Score IsNot Nothing Then
                        _Score = Nothing
                    End If
                    If _UnitNum IsNot Nothing Then
                        _UnitNum = Nothing
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
