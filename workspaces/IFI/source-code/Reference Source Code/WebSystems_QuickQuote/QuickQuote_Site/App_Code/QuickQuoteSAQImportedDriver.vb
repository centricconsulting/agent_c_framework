Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store SAQ driver information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSAQImportedDriver 'added 9/17/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _BirthDate As String
        Private _ChoicePointTransmissionNum As String
        Private _DLDate As String
        Private _DLN As String
        Private _DLNStateId As String
        Private _FirstName As String
        Private _LastName As String
        Private _MaritalStatus As String
        Private _MiddleName As String
        Private _PrefixName As String
        Private _QuotebackGuid As String
        Private _ReceivedDate As String
        Private _Remarks As String
        Private _SSN As String
        Private _SaqImportedDriverNum As String
        Private _SaqStatusTypeId As String
        Private _Selected As Boolean
        Private _SexId As String
        Private _SuffixName As String

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
        Public Property BirthDate As String
            Get
                Return _BirthDate
            End Get
            Set(value As String)
                _BirthDate = value
                qqHelper.ConvertToShortDate(_BirthDate)
            End Set
        End Property
        Public Property ChoicePointTransmissionNum As String
            Get
                Return _ChoicePointTransmissionNum
            End Get
            Set(value As String)
                _ChoicePointTransmissionNum = value
            End Set
        End Property
        Public Property DLDate As String
            Get
                Return _DLDate
            End Get
            Set(value As String)
                _DLDate = value
                qqHelper.ConvertToShortDate(_DLDate)
            End Set
        End Property
        Public Property DLN As String
            Get
                Return _DLN
            End Get
            Set(value As String)
                _DLN = value
            End Set
        End Property
        Public Property DLNStateId As String
            Get
                Return _DLNStateId
            End Get
            Set(value As String)
                _DLNStateId = value
            End Set
        End Property
        Public Property FirstName As String
            Get
                Return _FirstName
            End Get
            Set(value As String)
                _FirstName = value
            End Set
        End Property
        Public Property LastName As String
            Get
                Return _LastName
            End Get
            Set(value As String)
                _LastName = value
            End Set
        End Property
        Public Property MaritalStatus As String
            Get
                Return _MaritalStatus
            End Get
            Set(value As String)
                _MaritalStatus = value
            End Set
        End Property
        Public Property MiddleName As String
            Get
                Return _MiddleName
            End Get
            Set(value As String)
                _MiddleName = value
            End Set
        End Property
        Public Property PrefixName As String
            Get
                Return _PrefixName
            End Get
            Set(value As String)
                _PrefixName = value
            End Set
        End Property
        Public Property QuotebackGuid As String
            Get
                Return _QuotebackGuid
            End Get
            Set(value As String)
                _QuotebackGuid = value
            End Set
        End Property
        Public Property ReceivedDate As String
            Get
                Return _ReceivedDate
            End Get
            Set(value As String)
                _ReceivedDate = value
                qqHelper.ConvertToShortDate(_ReceivedDate)
            End Set
        End Property
        Public Property Remarks As String
            Get
                Return _Remarks
            End Get
            Set(value As String)
                _Remarks = value
            End Set
        End Property
        Public Property SSN As String
            Get
                Return _SSN
            End Get
            Set(value As String)
                _SSN = value
            End Set
        End Property
        Public Property SaqImportedDriverNum As String
            Get
                Return _SaqImportedDriverNum
            End Get
            Set(value As String)
                _SaqImportedDriverNum = value
            End Set
        End Property
        Public Property SaqStatusTypeId As String
            Get
                Return _SaqStatusTypeId
            End Get
            Set(value As String)
                _SaqStatusTypeId = value
            End Set
        End Property
        Public Property Selected As Boolean
            Get
                Return _Selected
            End Get
            Set(value As Boolean)
                _Selected = value
            End Set
        End Property
        Public Property SexId As String
            Get
                Return _SexId
            End Get
            Set(value As String)
                _SexId = value
            End Set
        End Property
        Public Property SuffixName As String
            Get
                Return _SuffixName
            End Get
            Set(value As String)
                _SuffixName = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _BirthDate = ""
            _ChoicePointTransmissionNum = ""
            _DLDate = ""
            _DLN = ""
            _DLNStateId = ""
            _FirstName = ""
            _LastName = ""
            _MaritalStatus = ""
            _MiddleName = ""
            _PrefixName = ""
            _QuotebackGuid = ""
            _ReceivedDate = ""
            _Remarks = ""
            _SSN = ""
            _SaqImportedDriverNum = ""
            _SaqStatusTypeId = ""
            _Selected = False
            _SexId = ""
            _SuffixName = ""
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
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _BirthDate IsNot Nothing Then
                        _BirthDate = Nothing
                    End If
                    If _ChoicePointTransmissionNum IsNot Nothing Then
                        _ChoicePointTransmissionNum = Nothing
                    End If
                    If _DLDate IsNot Nothing Then
                        _DLDate = Nothing
                    End If
                    If _DLN IsNot Nothing Then
                        _DLN = Nothing
                    End If
                    If _DLNStateId IsNot Nothing Then
                        _DLNStateId = Nothing
                    End If
                    If _FirstName IsNot Nothing Then
                        _FirstName = Nothing
                    End If
                    If _LastName IsNot Nothing Then
                        _LastName = Nothing
                    End If
                    If _MaritalStatus IsNot Nothing Then
                        _MaritalStatus = Nothing
                    End If
                    If _MiddleName IsNot Nothing Then
                        _MiddleName = Nothing
                    End If
                    If _PrefixName IsNot Nothing Then
                        _PrefixName = Nothing
                    End If
                    If _QuotebackGuid IsNot Nothing Then
                        _QuotebackGuid = Nothing
                    End If
                    If _ReceivedDate IsNot Nothing Then
                        _ReceivedDate = Nothing
                    End If
                    If _Remarks IsNot Nothing Then
                        _Remarks = Nothing
                    End If
                    If _SSN IsNot Nothing Then
                        _SSN = Nothing
                    End If
                    If _SaqImportedDriverNum IsNot Nothing Then
                        _SaqImportedDriverNum = Nothing
                    End If
                    If _SaqStatusTypeId IsNot Nothing Then
                        _SaqStatusTypeId = Nothing
                    End If
                    If _Selected <> Nothing Then
                        _Selected = Nothing
                    End If
                    If _SexId IsNot Nothing Then
                        _SexId = Nothing
                    End If
                    If _SuffixName IsNot Nothing Then
                        _SuffixName = Nothing
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
