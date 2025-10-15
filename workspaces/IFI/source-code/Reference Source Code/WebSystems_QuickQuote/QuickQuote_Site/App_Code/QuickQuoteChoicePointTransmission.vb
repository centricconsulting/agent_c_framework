Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store choicepoint transmission information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteChoicePointTransmission 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _BirthDate As String
        Private _ChoicePointTransactionTypeId As String
        'Private _ChoicePointTransmissionDriverChoicePointMismatchIndicatorTypeLinks
        Private _ChoicePointTransmissionNum As String
        Private _CustomQuoteback As String
        Private _DLN As String
        'Private _DMVReturnedName
        Private _DataChoicePointReceived As String
        Private _DataChoicePointSent As String
        Private _DataWebserviceReceived As String
        Private _DataWebserviceSent As String
        Private _ManualClueDate As String
        Private _PackagePartNum As String
        Private _PortStateId As String
        Private _PredictedClearMVR As Boolean
        Private _ProcessingStatusCode As String
        Private _QuotebackGuid As String
        Private _ReceivedDate As String
        Private _ReferenceNumber As String
        Private _RenewalPreProcess As Boolean
        Private _SentDate As String
        Private _SystemGenerated As Boolean
        Private _ThirdPartyStatusId As String
        Private _ThirdPartyTypeId As String
        Private _UnitNum As String
        Private _UsersId As String
        Private _VinProcessingStatusCode As String

        'added 4/9/2019
        Private _AddedDate As String
        Private _PcAddedDate As String
        Private _LastModifiedDate As String
        Private _StfMessage As String
        Private _NameAddressSourceId As String
        Private _DmvReturnedName As String

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
        Public Property ChoicePointTransactionTypeId As String
            Get
                Return _ChoicePointTransactionTypeId
            End Get
            Set(value As String)
                _ChoicePointTransactionTypeId = value
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
        Public Property CustomQuoteback As String
            Get
                Return _CustomQuoteback
            End Get
            Set(value As String)
                _CustomQuoteback = value
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
        Public Property DataChoicePointReceived As String
            Get
                Return _DataChoicePointReceived
            End Get
            Set(value As String)
                _DataChoicePointReceived = value
            End Set
        End Property
        Public Property DataChoicePointSent As String
            Get
                Return _DataChoicePointSent
            End Get
            Set(value As String)
                _DataChoicePointSent = value
            End Set
        End Property
        Public Property DataWebserviceReceived As String
            Get
                Return _DataWebserviceReceived
            End Get
            Set(value As String)
                _DataWebserviceReceived = value
            End Set
        End Property
        Public Property DataWebserviceSent As String
            Get
                Return _DataWebserviceSent
            End Get
            Set(value As String)
                _DataWebserviceSent = value
            End Set
        End Property
        Public Property ManualClueDate As String
            Get
                Return _ManualClueDate
            End Get
            Set(value As String)
                _ManualClueDate = value
                qqHelper.ConvertToShortDate(_ManualClueDate)
            End Set
        End Property
        Public Property PackagePartNum As String
            Get
                Return _PackagePartNum
            End Get
            Set(value As String)
                _PackagePartNum = value
            End Set
        End Property
        Public Property PortStateId As String
            Get
                Return _PortStateId
            End Get
            Set(value As String)
                _PortStateId = value
            End Set
        End Property
        Public Property PredictedClearMVR As Boolean
            Get
                Return _PredictedClearMVR
            End Get
            Set(value As Boolean)
                _PredictedClearMVR = value
            End Set
        End Property
        Public Property ProcessingStatusCode As String
            Get
                Return _ProcessingStatusCode
            End Get
            Set(value As String)
                _ProcessingStatusCode = value
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
        Public Property ReferenceNumber As String
            Get
                Return _ReferenceNumber
            End Get
            Set(value As String)
                _ReferenceNumber = value
            End Set
        End Property
        Public Property RenewalPreProcess As Boolean
            Get
                Return _RenewalPreProcess
            End Get
            Set(value As Boolean)
                _RenewalPreProcess = value
            End Set
        End Property
        Public Property SentDate As String
            Get
                Return _SentDate
            End Get
            Set(value As String)
                _SentDate = value
                qqHelper.ConvertToShortDate(_SentDate)
            End Set
        End Property
        Public Property SystemGenerated As Boolean
            Get
                Return _SystemGenerated
            End Get
            Set(value As Boolean)
                _SystemGenerated = value
            End Set
        End Property
        Public Property ThirdPartyStatusId As String
            Get
                Return _ThirdPartyStatusId
            End Get
            Set(value As String)
                _ThirdPartyStatusId = value
            End Set
        End Property
        Public Property ThirdPartyTypeId As String
            Get
                Return _ThirdPartyTypeId
            End Get
            Set(value As String)
                _ThirdPartyTypeId = value
            End Set
        End Property
        Public Property UnitNum As String
            Get
                Return _UnitNum
            End Get
            Set(value As String)
                _UnitNum = value
            End Set
        End Property
        Public Property UsersId As String
            Get
                Return _UsersId
            End Get
            Set(value As String)
                _UsersId = value
            End Set
        End Property
        Public Property VinProcessingStatusCode As String
            Get
                Return _VinProcessingStatusCode
            End Get
            Set(value As String)
                _VinProcessingStatusCode = value
            End Set
        End Property

        'added 4/9/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
                qqHelper.ConvertToShortDate(_AddedDate)
            End Set
        End Property
        Public Property PcAddedDate As String
            Get
                Return _PcAddedDate
            End Get
            Set(value As String)
                _PcAddedDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property StfMessage As String
            Get
                Return _StfMessage
            End Get
            Set(value As String)
                _StfMessage = value
            End Set
        End Property
        Public Property NameAddressSourceId As String
            Get
                Return _NameAddressSourceId
            End Get
            Set(value As String)
                _NameAddressSourceId = value
            End Set
        End Property
        Public Property DmvReturnedName As String
            Get
                Return _DmvReturnedName
            End Get
            Set(value As String)
                _DmvReturnedName = value
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
            _ChoicePointTransactionTypeId = ""
            _ChoicePointTransmissionNum = ""
            _CustomQuoteback = ""
            _DLN = ""
            _DataChoicePointReceived = ""
            _DataChoicePointSent = ""
            _DataWebserviceReceived = ""
            _DataWebserviceSent = ""
            _ManualClueDate = ""
            _PackagePartNum = ""
            _PortStateId = ""
            _PredictedClearMVR = False
            _ProcessingStatusCode = ""
            _QuotebackGuid = ""
            _ReceivedDate = ""
            _ReferenceNumber = ""
            _RenewalPreProcess = False
            _SentDate = ""
            _SystemGenerated = False
            _ThirdPartyStatusId = ""
            _ThirdPartyTypeId = ""
            _UnitNum = ""
            _UsersId = ""
            _VinProcessingStatusCode = ""

            'added 4/9/2019
            _AddedDate = ""
            _PcAddedDate = ""
            _LastModifiedDate = ""
            _StfMessage = ""
            _NameAddressSourceId = ""
            _DmvReturnedName = ""
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
                    If _ChoicePointTransactionTypeId IsNot Nothing Then
                        _ChoicePointTransactionTypeId = Nothing
                    End If
                    If _ChoicePointTransmissionNum IsNot Nothing Then
                        _ChoicePointTransmissionNum = Nothing
                    End If
                    If _CustomQuoteback IsNot Nothing Then
                        _CustomQuoteback = Nothing
                    End If
                    If _DLN IsNot Nothing Then
                        _DLN = Nothing
                    End If
                    If _DataChoicePointReceived IsNot Nothing Then
                        _DataChoicePointReceived = Nothing
                    End If
                    If _DataChoicePointSent IsNot Nothing Then
                        _DataChoicePointSent = Nothing
                    End If
                    If _DataWebserviceReceived IsNot Nothing Then
                        _DataWebserviceReceived = Nothing
                    End If
                    If _DataWebserviceSent IsNot Nothing Then
                        _DataWebserviceSent = Nothing
                    End If
                    If _ManualClueDate IsNot Nothing Then
                        _ManualClueDate = Nothing
                    End If
                    If _PackagePartNum IsNot Nothing Then
                        _PackagePartNum = Nothing
                    End If
                    If _PortStateId IsNot Nothing Then
                        _PortStateId = Nothing
                    End If
                    If _PredictedClearMVR <> Nothing Then
                        _PredictedClearMVR = Nothing
                    End If
                    If _ProcessingStatusCode IsNot Nothing Then
                        _ProcessingStatusCode = Nothing
                    End If
                    If _QuotebackGuid IsNot Nothing Then
                        _QuotebackGuid = Nothing
                    End If
                    If _ReceivedDate IsNot Nothing Then
                        _ReceivedDate = Nothing
                    End If
                    If _ReferenceNumber IsNot Nothing Then
                        _ReferenceNumber = Nothing
                    End If
                    If _RenewalPreProcess <> Nothing Then
                        _RenewalPreProcess = Nothing
                    End If
                    If _SentDate IsNot Nothing Then
                        _SentDate = Nothing
                    End If
                    If _SystemGenerated <> Nothing Then
                        _SystemGenerated = Nothing
                    End If
                    If _ThirdPartyStatusId IsNot Nothing Then
                        _ThirdPartyStatusId = Nothing
                    End If
                    If _ThirdPartyTypeId IsNot Nothing Then
                        _ThirdPartyTypeId = Nothing
                    End If
                    If _UnitNum IsNot Nothing Then
                        _UnitNum = Nothing
                    End If
                    If _UsersId IsNot Nothing Then
                        _UsersId = Nothing
                    End If
                    If _VinProcessingStatusCode IsNot Nothing Then
                        _VinProcessingStatusCode = Nothing
                    End If

                    'added 4/9/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_PcAddedDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_StfMessage)
                    qqHelper.DisposeString(_NameAddressSourceId)
                    qqHelper.DisposeString(_DmvReturnedName)

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
