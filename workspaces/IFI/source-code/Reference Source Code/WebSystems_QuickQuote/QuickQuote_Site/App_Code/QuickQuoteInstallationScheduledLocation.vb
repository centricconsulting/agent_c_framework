Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' objects used to store installation scheduled location information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteObject object (<see cref="QuickQuoteObject"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuoteInstallationScheduledLocation 'added 1/28/2015
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Limit As String
        Private _AddressInfo As String
        Private _QuotedPremium As String

        Public Property Limit As String
            Get
                Return _Limit
            End Get
            Set(value As String)
                _Limit = value
                qqHelper.ConvertToLimitFormat(_Limit)
            End Set
        End Property
        Public Property AddressInfo As String
            Get
                Return _AddressInfo
            End Get
            Set(value As String)
                _AddressInfo = value
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

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Limit = ""
            _AddressInfo = ""
            _QuotedPremium = ""
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Limit <> "" Then
                    str = qqHelper.appendText(str, "Limit: " & Me.Limit, vbCrLf)
                End If
                If Me.AddressInfo <> "" Then
                    str = qqHelper.appendText(str, "AddressInfo: " & Me.AddressInfo, vbCrLf)
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
                    qqHelper.DisposeString(_Limit)
                    qqHelper.DisposeString(_AddressInfo)
                    qqHelper.DisposeString(_QuotedPremium)

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