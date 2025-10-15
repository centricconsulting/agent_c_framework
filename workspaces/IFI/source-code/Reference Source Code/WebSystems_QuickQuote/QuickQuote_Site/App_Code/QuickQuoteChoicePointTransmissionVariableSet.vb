Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()> _
    Public Class QuickQuoteChoicePointTransmissionVariableSet 'added 5/1/2014 pm; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        Implements IDisposable

        Private _PolicyId As Integer
        Private _PolicyImageNum As Integer
        Private _UnitNum As Integer
        Private _ChoicePointTransmissionNum As Integer

        Public Property PolicyId As Integer
            Get
                Return _PolicyId
            End Get
            Set(value As Integer)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As Integer
            Get
                Return _PolicyImageNum
            End Get
            Set(value As Integer)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property UnitNum As Integer
            Get
                Return _UnitNum
            End Get
            Set(value As Integer)
                _UnitNum = value
            End Set
        End Property
        Public Property ChoicePointTransmissionNum As Integer
            Get
                Return _ChoicePointTransmissionNum
            End Get
            Set(value As Integer)
                _ChoicePointTransmissionNum = value
            End Set
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = 0
            _PolicyImageNum = 0
            _UnitNum = 0
            _ChoicePointTransmissionNum = 0
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    _PolicyId = Nothing
                    _PolicyImageNum = Nothing
                    _UnitNum = Nothing
                    _ChoicePointTransmissionNum = Nothing
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
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
