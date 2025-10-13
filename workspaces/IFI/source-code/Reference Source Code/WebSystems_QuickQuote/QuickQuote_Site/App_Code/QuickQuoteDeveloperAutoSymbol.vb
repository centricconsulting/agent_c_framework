Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store developer auto symbol information
    ''' </summary>
    ''' <remarks>not directly tied to xml; can be converted to QuickQuoteAutoSymbol objects (<see cref="QuickQuoteAutoSymbol"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteDeveloperAutoSymbol '8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        'Inherits QuickQuoteBaseGenericObject_DoesNotInheritBaseObject(Of Object) 'TODO: Dan - Needs Parent
        Implements IDisposable

        Private _HasSymbol1 As Boolean
        Private _HasSymbol2 As Boolean
        Private _HasSymbol3 As Boolean
        Private _HasSymbol4 As Boolean
        Private _HasSymbol7 As Boolean
        Private _HasSymbol8 As Boolean
        Private _HasSymbol9 As Boolean

        Public Property HasSymbol1 As Boolean
            Get
                Return _HasSymbol1
            End Get
            Set(value As Boolean)
                _HasSymbol1 = value
            End Set
        End Property
        Public Property HasSymbol2 As Boolean
            Get
                Return _HasSymbol2
            End Get
            Set(value As Boolean)
                _HasSymbol2 = value
            End Set
        End Property
        Public Property HasSymbol3 As Boolean
            Get
                Return _HasSymbol3
            End Get
            Set(value As Boolean)
                _HasSymbol3 = value
            End Set
        End Property
        Public Property HasSymbol4 As Boolean
            Get
                Return _HasSymbol4
            End Get
            Set(value As Boolean)
                _HasSymbol4 = value
            End Set
        End Property
        Public Property HasSymbol7 As Boolean
            Get
                Return _HasSymbol7
            End Get
            Set(value As Boolean)
                _HasSymbol7 = value
            End Set
        End Property
        Public Property HasSymbol8 As Boolean
            Get
                Return _HasSymbol8
            End Get
            Set(value As Boolean)
                _HasSymbol8 = value
            End Set
        End Property
        Public Property HasSymbol9 As Boolean
            Get
                Return _HasSymbol9
            End Get
            Set(value As Boolean)
                _HasSymbol9 = value
            End Set
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _HasSymbol1 = False
            _HasSymbol2 = False
            _HasSymbol3 = False
            _HasSymbol4 = False
            _HasSymbol7 = False
            _HasSymbol8 = False
            _HasSymbol9 = False
        End Sub
        Public Function HasAnySymbols() As Boolean
            If _HasSymbol1 = True OrElse _HasSymbol2 = True OrElse _HasSymbol3 = True OrElse _HasSymbol4 = True OrElse _HasSymbol7 = True OrElse _HasSymbol8 = True OrElse _HasSymbol9 = True Then
                Return True
            Else
                Return False
            End If
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _HasSymbol1 <> Nothing Then
                        _HasSymbol1 = Nothing
                    End If
                    If _HasSymbol2 <> Nothing Then
                        _HasSymbol2 = Nothing
                    End If
                    If _HasSymbol3 <> Nothing Then
                        _HasSymbol3 = Nothing
                    End If
                    If _HasSymbol4 <> Nothing Then
                        _HasSymbol4 = Nothing
                    End If
                    If _HasSymbol7 <> Nothing Then
                        _HasSymbol7 = Nothing
                    End If
                    If _HasSymbol8 <> Nothing Then
                        _HasSymbol8 = Nothing
                    End If
                    If _HasSymbol9 <> Nothing Then
                        _HasSymbol9 = Nothing
                    End If
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
