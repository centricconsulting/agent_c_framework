Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteAdditionalInterestListLink 'added 5/23/2017
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _AdditionalInterestListId As String
        Private _DetailStatusCode As String
        Private _DisplayNum As String
        Private _OrderNum As String '/InternalValue
        Private _PolicyLevelNum As String

        Public Property AdditionalInterestListId As String
            Get
                Return _AdditionalInterestListId
            End Get
            Set(value As String)
                _AdditionalInterestListId = value
            End Set
        End Property
        Public Property DetailStatusCode As String
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property
        Public Property DisplayNum As String
            Get
                Return _DisplayNum
            End Get
            Set(value As String)
                _DisplayNum = value
            End Set
        End Property
        Public Property OrderNum As String '/InternalValue
            Get
                Return _OrderNum
            End Get
            Set(value As String)
                _OrderNum = value
            End Set
        End Property
        Public Property PolicyLevelNum As String
            Get
                Return _PolicyLevelNum
            End Get
            Set(value As String)
                _PolicyLevelNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AdditionalInterestListId = ""
            _DetailStatusCode = ""
            _DisplayNum = ""
            _OrderNum = "" '/InternalValue
            _PolicyLevelNum = ""
        End Sub
        Public Function HasValidOrderNum() As Boolean 'added for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_OrderNum)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_AdditionalInterestListId)
                    qqHelper.DisposeString(_DetailStatusCode)
                    qqHelper.DisposeString(_DisplayNum)
                    qqHelper.DisposeString(_OrderNum) '/InternalValue
                    qqHelper.DisposeString(_PolicyLevelNum)
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
