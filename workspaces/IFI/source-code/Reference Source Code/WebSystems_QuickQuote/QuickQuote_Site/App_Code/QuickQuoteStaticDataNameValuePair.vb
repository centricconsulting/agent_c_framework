Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store static data name/value pair
    ''' </summary>
    ''' <remarks>used w/ Static Data xml file(s)</remarks>
    <Serializable()> _
    Public Class QuickQuoteStaticDataNameValuePair 'added 11/26/2013; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        'Implements IDisposable

        Public Property Name As String = String.Empty
        Public Property Value As String = String.Empty

        '#Region "IDisposable Support"
        '    Private disposedValue As Boolean ' To detect redundant calls

        '    ' IDisposable
        '    Protected Overridable Sub Dispose(disposing As Boolean)
        '        If Not Me.disposedValue Then
        '            If disposing Then
        '                ' TODO: dispose managed state (managed objects).
        '                Name = Nothing
        '                Value = Nothing
        '            End If

        '            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '            ' TODO: set large fields to null.
        '        End If
        '        Me.disposedValue = True
        '    End Sub

        '    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        '    'Protected Overrides Sub Finalize()
        '    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    '    Dispose(False)
        '    '    MyBase.Finalize()
        '    'End Sub

        '    ' This code added by Visual Basic to correctly implement the disposable pattern.
        '    Public Sub Dispose() Implements IDisposable.Dispose
        '        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '        Dispose(True)
        '        GC.SuppressFinalize(Me)
        '    End Sub
        '#End Region

    End Class
End Namespace
