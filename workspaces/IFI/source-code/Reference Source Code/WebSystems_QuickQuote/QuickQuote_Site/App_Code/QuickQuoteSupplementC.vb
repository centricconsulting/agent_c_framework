Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store supplement c information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSupplementC 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Identifier1 As String
        Private _Identifier10 As String
        Private _Identifier2 As String
        Private _Identifier3 As String
        Private _Identifier4 As String
        Private _Identifier5 As String
        Private _Identifier6 As String
        Private _Identifier7 As String
        Private _Identifier8 As String
        Private _Identifier9 As String
        Private _Parameter1 As String
        Private _Parameter10 As String
        Private _Parameter2 As String
        Private _Parameter3 As String
        Private _Parameter4 As String
        Private _Parameter5 As String
        Private _Parameter6 As String
        Private _Parameter7 As String
        Private _Parameter8 As String
        Private _Parameter9 As String

        Public Property Identifier1 As String
            Get
                Return _Identifier1
            End Get
            Set(value As String)
                _Identifier1 = value
            End Set
        End Property
        Public Property Identifier10 As String
            Get
                Return _Identifier10
            End Get
            Set(value As String)
                _Identifier10 = value
            End Set
        End Property
        Public Property Identifier2 As String
            Get
                Return _Identifier2
            End Get
            Set(value As String)
                _Identifier2 = value
            End Set
        End Property
        Public Property Identifier3 As String
            Get
                Return _Identifier3
            End Get
            Set(value As String)
                _Identifier3 = value
            End Set
        End Property
        Public Property Identifier4 As String
            Get
                Return _Identifier4
            End Get
            Set(value As String)
                _Identifier4 = value
            End Set
        End Property
        Public Property Identifier5 As String
            Get
                Return _Identifier5
            End Get
            Set(value As String)
                _Identifier5 = value
            End Set
        End Property
        Public Property Identifier6 As String
            Get
                Return _Identifier6
            End Get
            Set(value As String)
                _Identifier6 = value
            End Set
        End Property
        Public Property Identifier7 As String
            Get
                Return _Identifier7
            End Get
            Set(value As String)
                _Identifier7 = value
            End Set
        End Property
        Public Property Identifier8 As String
            Get
                Return _Identifier8
            End Get
            Set(value As String)
                _Identifier8 = value
            End Set
        End Property
        Public Property Identifier9 As String
            Get
                Return _Identifier9
            End Get
            Set(value As String)
                _Identifier9 = value
            End Set
        End Property
        Public Property Parameter1 As String
            Get
                Return _Parameter1
            End Get
            Set(value As String)
                _Parameter1 = value
            End Set
        End Property
        Public Property Parameter10 As String
            Get
                Return _Parameter10
            End Get
            Set(value As String)
                _Parameter10 = value
            End Set
        End Property
        Public Property Parameter2 As String
            Get
                Return _Parameter2
            End Get
            Set(value As String)
                _Parameter2 = value
            End Set
        End Property
        Public Property Parameter3 As String
            Get
                Return _Parameter3
            End Get
            Set(value As String)
                _Parameter3 = value
            End Set
        End Property
        Public Property Parameter4 As String
            Get
                Return _Parameter4
            End Get
            Set(value As String)
                _Parameter4 = value
            End Set
        End Property
        Public Property Parameter5 As String
            Get
                Return _Parameter5
            End Get
            Set(value As String)
                _Parameter5 = value
            End Set
        End Property
        Public Property Parameter6 As String
            Get
                Return _Parameter6
            End Get
            Set(value As String)
                _Parameter6 = value
            End Set
        End Property
        Public Property Parameter7 As String
            Get
                Return _Parameter7
            End Get
            Set(value As String)
                _Parameter7 = value
            End Set
        End Property
        Public Property Parameter8 As String
            Get
                Return _Parameter8
            End Get
            Set(value As String)
                _Parameter8 = value
            End Set
        End Property
        Public Property Parameter9 As String
            Get
                Return _Parameter9
            End Get
            Set(value As String)
                _Parameter9 = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Identifier1 = ""
            _Identifier10 = ""
            _Identifier2 = ""
            _Identifier3 = ""
            _Identifier4 = ""
            _Identifier5 = ""
            _Identifier6 = ""
            _Identifier7 = ""
            _Identifier8 = ""
            _Identifier9 = ""
            _Parameter1 = ""
            _Parameter10 = ""
            _Parameter2 = ""
            _Parameter3 = ""
            _Parameter4 = ""
            _Parameter5 = ""
            _Parameter6 = ""
            _Parameter7 = ""
            _Parameter8 = ""
            _Parameter9 = ""
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
                    If _Identifier1 IsNot Nothing Then
                        _Identifier1 = Nothing
                    End If
                    If _Identifier10 IsNot Nothing Then
                        _Identifier10 = Nothing
                    End If
                    If _Identifier2 IsNot Nothing Then
                        _Identifier2 = Nothing
                    End If
                    If _Identifier3 IsNot Nothing Then
                        _Identifier3 = Nothing
                    End If
                    If _Identifier4 IsNot Nothing Then
                        _Identifier4 = Nothing
                    End If
                    If _Identifier5 IsNot Nothing Then
                        _Identifier5 = Nothing
                    End If
                    If _Identifier6 IsNot Nothing Then
                        _Identifier6 = Nothing
                    End If
                    If _Identifier7 IsNot Nothing Then
                        _Identifier7 = Nothing
                    End If
                    If _Identifier8 IsNot Nothing Then
                        _Identifier8 = Nothing
                    End If
                    If _Identifier9 IsNot Nothing Then
                        _Identifier9 = Nothing
                    End If
                    If _Parameter1 IsNot Nothing Then
                        _Parameter1 = Nothing
                    End If
                    If _Parameter10 IsNot Nothing Then
                        _Parameter10 = Nothing
                    End If
                    If _Parameter2 IsNot Nothing Then
                        _Parameter2 = Nothing
                    End If
                    If _Parameter3 IsNot Nothing Then
                        _Parameter3 = Nothing
                    End If
                    If _Parameter4 IsNot Nothing Then
                        _Parameter4 = Nothing
                    End If
                    If _Parameter5 IsNot Nothing Then
                        _Parameter5 = Nothing
                    End If
                    If _Parameter6 IsNot Nothing Then
                        _Parameter6 = Nothing
                    End If
                    If _Parameter7 IsNot Nothing Then
                        _Parameter7 = Nothing
                    End If
                    If _Parameter8 IsNot Nothing Then
                        _Parameter8 = Nothing
                    End If
                    If _Parameter9 IsNot Nothing Then
                        _Parameter9 = Nothing
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
