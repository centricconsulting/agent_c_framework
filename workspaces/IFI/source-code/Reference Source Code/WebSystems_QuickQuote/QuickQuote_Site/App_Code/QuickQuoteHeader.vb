Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store header information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteHeader 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _ActivityFile As String
        Private _Amplified As String
        Private _ChptIntAccountNumber As String
        Private _ForcedDmv As String
        Private _PacketNumber As String
        Private _ProductCode As String
        Private _ReturnLocationDfsNumber As String
        Private _Soundexing As String
        Private _SpecialBillingId As String
        Private _StateCode As String
        Private _StateId As String
        Private _StatePostalCode As String
        Private _TypeCode As String

        Public Property ActivityFile As String
            Get
                Return _ActivityFile
            End Get
            Set(value As String)
                _ActivityFile = value
            End Set
        End Property
        Public Property Amplified As String
            Get
                Return _Amplified
            End Get
            Set(value As String)
                _Amplified = value
            End Set
        End Property
        Public Property ChptIntAccountNumber As String
            Get
                Return _ChptIntAccountNumber
            End Get
            Set(value As String)
                _ChptIntAccountNumber = value
            End Set
        End Property
        Public Property ForcedDmv As String
            Get
                Return _ForcedDmv
            End Get
            Set(value As String)
                _ForcedDmv = value
            End Set
        End Property
        Public Property PacketNumber As String
            Get
                Return _PacketNumber
            End Get
            Set(value As String)
                _PacketNumber = value
            End Set
        End Property
        Public Property ProductCode As String
            Get
                Return _ProductCode
            End Get
            Set(value As String)
                _ProductCode = value
            End Set
        End Property
        Public Property ReturnLocationDfsNumber As String
            Get
                Return _ReturnLocationDfsNumber
            End Get
            Set(value As String)
                _ReturnLocationDfsNumber = value
            End Set
        End Property
        Public Property Soundexing As String
            Get
                Return _Soundexing
            End Get
            Set(value As String)
                _Soundexing = value
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
        Public Property StateCode As String
            Get
                Return _StateCode
            End Get
            Set(value As String)
                _StateCode = value
            End Set
        End Property
        Public Property StateId As String
            Get
                Return _StateId
            End Get
            Set(value As String)
                _StateId = value
            End Set
        End Property
        Public Property StatePostalCode As String
            Get
                Return _StatePostalCode
            End Get
            Set(value As String)
                _StatePostalCode = value
            End Set
        End Property
        Public Property TypeCode As String
            Get
                Return _TypeCode
            End Get
            Set(value As String)
                _TypeCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _ActivityFile = ""
            _Amplified = ""
            _ChptIntAccountNumber = ""
            _ForcedDmv = ""
            _PacketNumber = ""
            _ProductCode = ""
            _ReturnLocationDfsNumber = ""
            _Soundexing = ""
            _SpecialBillingId = ""
            _StateCode = ""
            _StateId = ""
            _StatePostalCode = ""
            _TypeCode = ""
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
                    If _ActivityFile IsNot Nothing Then
                        _ActivityFile = Nothing
                    End If
                    If _Amplified IsNot Nothing Then
                        _Amplified = Nothing
                    End If
                    If _ChptIntAccountNumber IsNot Nothing Then
                        _ChptIntAccountNumber = Nothing
                    End If
                    If _ForcedDmv IsNot Nothing Then
                        _ForcedDmv = Nothing
                    End If
                    If _PacketNumber IsNot Nothing Then
                        _PacketNumber = Nothing
                    End If
                    If _ProductCode IsNot Nothing Then
                        _ProductCode = Nothing
                    End If
                    If _ReturnLocationDfsNumber IsNot Nothing Then
                        _ReturnLocationDfsNumber = Nothing
                    End If
                    If _Soundexing IsNot Nothing Then
                        _Soundexing = Nothing
                    End If
                    If _SpecialBillingId IsNot Nothing Then
                        _SpecialBillingId = Nothing
                    End If
                    If _StateCode IsNot Nothing Then
                        _StateCode = Nothing
                    End If
                    If _StateId IsNot Nothing Then
                        _StateId = Nothing
                    End If
                    If _StatePostalCode IsNot Nothing Then
                        _StatePostalCode = Nothing
                    End If
                    If _TypeCode IsNot Nothing Then
                        _TypeCode = Nothing
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
