Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store classification code information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteClassificationCode 'added 9/27/2012 for CPR
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 11/29/2012 for formatting ClassLimit

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _ClassCode As String
        Private _ClassDescription As String
        Private _PMA As String
        'added 11/29/2012 for CPR
        Private _ClassLimit As String
        Private _RateGroup As String
        'added 3/26/2015 for CRM... only lob so far that appears to use list... even though you just enter 1 from UI
        Private _ClassificationCodeNum As String

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
        Public Property ClassCode As String
            Get
                Return _ClassCode
            End Get
            Set(value As String)
                _ClassCode = value
            End Set
        End Property
        Public Property ClassDescription As String
            Get
                Return _ClassDescription
            End Get
            Set(value As String)
                _ClassDescription = value
            End Set
        End Property
        Public Property PMA As String
            Get
                Return _PMA
            End Get
            Set(value As String)
                _PMA = value
            End Set
        End Property
        Public Property ClassLimit As String
            Get
                Return _ClassLimit
            End Get
            Set(value As String)
                _ClassLimit = value
                'qqHelper.ConvertToLimitFormat(_ClassLimit)'testing no formatting 12/3/2012 to see if this helps issue
            End Set
        End Property
        Public Property RateGroup As String
            Get
                Return _RateGroup
            End Get
            Set(value As String)
                _RateGroup = value
            End Set
        End Property
        'added 3/26/2015 for CRM
        Public Property ClassificationCodeNum As String
            Get
                Return _ClassificationCodeNum
            End Get
            Set(value As String)
                _ClassificationCodeNum = value
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
            _ClassCode = ""
            _ClassDescription = ""
            _PMA = ""
            _ClassLimit = ""
            _RateGroup = ""
            'added 3/26/2015 for CRM
            _ClassificationCodeNum = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Function HasValidClassificationCodeNum() As Boolean 'added 3/26/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ClassificationCodeNum)
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
                    If _ClassCode IsNot Nothing Then
                        _ClassCode = Nothing
                    End If
                    If _ClassDescription IsNot Nothing Then
                        _ClassDescription = Nothing
                    End If
                    If _PMA IsNot Nothing Then
                        _PMA = Nothing
                    End If
                    If _ClassLimit IsNot Nothing Then
                        _ClassLimit = Nothing
                    End If
                    If _RateGroup IsNot Nothing Then
                        _RateGroup = Nothing
                    End If
                    'added 3/26/2015 for CRM
                    qqHelper.DisposeString(_ClassificationCodeNum)

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
