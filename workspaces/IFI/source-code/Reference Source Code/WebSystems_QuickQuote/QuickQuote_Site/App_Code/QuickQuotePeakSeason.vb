Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store peak season information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteScheduledPersonalPropertyCoverage object (<see cref="QuickQuoteScheduledPersonalPropertyCoverage"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuotePeakSeason 'added 5/11/2015 for Farm
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _EffectiveDate As String
        Private _ExpirationDate As String
        Private _IncreasedLimit As String
        Private _Description As String
        Private _Premium As String

        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
                qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                qqHelper.ConvertToShortDate(_ExpirationDate)
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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
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
            _EffectiveDate = ""
            _ExpirationDate = ""
            _IncreasedLimit = ""
            _Description = ""
            _Premium = ""
        End Sub


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_EffectiveDate)
                    qqHelper.DisposeString(_ExpirationDate)
                    qqHelper.DisposeString(_IncreasedLimit)
                    qqHelper.DisposeString(_Description)
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
