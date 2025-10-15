Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store sub unit quoteback information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSubUnitQuotebackInfo 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Classification As String
        Private _Filler As String
        Private _Quoteback As String
        Private _UnitNumber As String

        Public Property Classification As String
            Get
                Return _Classification
            End Get
            Set(value As String)
                _Classification = value
            End Set
        End Property
        Public Property Filler As String
            Get
                Return _Filler
            End Get
            Set(value As String)
                _Filler = value
            End Set
        End Property
        Public Property Quoteback As String
            Get
                Return _Quoteback
            End Get
            Set(value As String)
                _Quoteback = value
            End Set
        End Property
        Public Property UnitNumber As String
            Get
                Return _UnitNumber
            End Get
            Set(value As String)
                _UnitNumber = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Classification = ""
            _Filler = ""
            _Quoteback = ""
            _UnitNumber = ""
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
                    If _Classification IsNot Nothing Then
                        _Classification = Nothing
                    End If
                    If _Filler IsNot Nothing Then
                        _Filler = Nothing
                    End If
                    If _Quoteback IsNot Nothing Then
                        _Quoteback = Nothing
                    End If
                    If _UnitNumber IsNot Nothing Then
                        _UnitNumber = Nothing
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
