Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store garaging address information
    ''' </summary>
    ''' <remarks>specific to auto</remarks>
    <Serializable()> _
    Public Class QuickQuoteGaragingAddress
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String

        Private _Address As QuickQuoteAddress

        Private _GaragedInside As Boolean
        Private _WithinCity As Boolean

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property

        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property

        Public Property GaragedInside As Boolean
            Get
                Return _GaragedInside
            End Get
            Set(value As Boolean)
                _GaragedInside = value
            End Set
        End Property
        Public Property WithinCity As Boolean
            Get
                Return _WithinCity
            End Get
            Set(value As Boolean)
                _WithinCity = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""

            _Address = New QuickQuoteAddress
            'updated 2/11/2014 to default Garaging Address state to blank instead of IN
            _Address.StateId = "" 'should also blank out Address.State since the StateId property automatically sets it to its corresponding value

            _GaragedInside = False
            _WithinCity = False

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub

        'added 3/6/2017 for 531.009... IS is now using GaragingAddress.Address for CLUE reports if available... they can't handle stateId 0 correctly anymore so we may have to default to 16 if we're creating Address record
        Public Function HasData() As Boolean
            If _GaragedInside = True OrElse _WithinCity = True OrElse (_Address IsNot Nothing AndAlso _Address.HasData = True) Then
                Return True
            Else
                Return False
            End If
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If

                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If

                    If _GaragedInside <> Nothing Then
                        _GaragedInside = Nothing
                    End If
                    If _WithinCity <> Nothing Then
                        _WithinCity = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
