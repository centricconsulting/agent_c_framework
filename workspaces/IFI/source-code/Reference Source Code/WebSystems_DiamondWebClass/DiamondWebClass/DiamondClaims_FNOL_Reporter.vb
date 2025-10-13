Public Class DiamondClaims_FNOL_Reporter 'Don Mink - added 4/26/2013
    Inherits DiamondClaims_FNOL_Person
    Implements IDisposable

    'Private _Remarks As String

    'Public Property Remarks As String
    '    Get
    '        Return _Remarks
    '    End Get
    '    Set(value As String)
    '        _Remarks = value
    '    End Set
    'End Property

    Public Sub New()
        MyBase.New()
        setDefaults()
    End Sub

    'Public Overrides Sub setDefaults()
    Public Sub setDefaults() '6/4/2013 - changed to not override base sub
        '_Remarks = ""
    End Sub


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    'Protected Overridable Sub Dispose(disposing As Boolean)
    Protected Overloads Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                'If _Remarks IsNot Nothing Then
                '    _Remarks = Nothing
                'End If
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
    Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
