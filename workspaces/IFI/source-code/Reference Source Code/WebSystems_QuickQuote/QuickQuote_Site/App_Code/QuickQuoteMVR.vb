Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store MVR information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteMVR 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _Details As List(Of QuickQuoteDetail)
        Private _Header As QuickQuoteHeader

        Public Property Details As List(Of QuickQuoteDetail)
            Get
                SetParentOfListItems(_Details, "{E4B024D9-4279-41D1-9E02-F84734D7D036}")
                Return _Details
            End Get
            Set(value As List(Of QuickQuoteDetail))
                _Details = value
                SetParentOfListItems(_Details, "{E4B024D9-4279-41D1-9E02-F84734D7D036}")
            End Set
        End Property
        Public Property Header As QuickQuoteHeader
            Get
                SetObjectsParent(_Header)
                Return _Header
            End Get
            Set(value As QuickQuoteHeader)
                _Header = value
                SetObjectsParent(_Header)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_Details = New List(Of QuickQuoteDetail)
            _Header = New QuickQuoteHeader
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
                    If _Details IsNot Nothing Then
                        If _Details.Count > 0 Then
                            For Each d As QuickQuoteDetail In _Details
                                d.Dispose()
                                d = Nothing
                            Next
                            _Details.Clear()
                        End If
                        _Details = Nothing
                    End If
                    If _Header IsNot Nothing Then
                        _Header.Dispose()
                        _Header = Nothing
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
