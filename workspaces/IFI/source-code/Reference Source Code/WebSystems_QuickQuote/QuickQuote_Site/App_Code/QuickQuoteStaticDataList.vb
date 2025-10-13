Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store static data options (text and value)
    ''' </summary>
    ''' <remarks>used w/ Static Data xml file(s)</remarks>
    <Serializable()> _
    Public Class QuickQuoteStaticDataList 'added 11/19/2013; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        'Implements IDisposable

        Public Property isLobSpecific As Boolean = False
        Public Property isPersOrCommSpecific As Boolean = False
        Public Property Options As List(Of QuickQuoteStaticDataOption) = Nothing
        'added 11/20/2013
        Public Property ClassToUse As String = String.Empty
        Public Property PropertyToUse As String = String.Empty
        'added 12/26/2013
        Public Property HasOptionsToIgnoreForLists As Boolean = False

        '#Region "IDisposable Support"
        '    Private disposedValue As Boolean ' To detect redundant calls

        '    ' IDisposable
        '    Protected Overridable Sub Dispose(disposing As Boolean)
        '        If Not Me.disposedValue Then
        '            If disposing Then
        '                ' TODO: dispose managed state (managed objects).
        '                isLobSpecific = Nothing
        '                isPersOrCommSpecific = Nothing
        '                Options = Nothing
        '                ClassToUse = Nothing
        '                PropertyToUse = Nothing
        '                HasOptionsToIgnoreForLists = Nothing
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
