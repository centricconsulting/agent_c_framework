Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store 3rd party information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyData 'added 9/17/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AccidentViolations As List(Of QuickQuoteAccidentViolation)
        Private _ChoicePointTransmissions As List(Of QuickQuoteChoicePointTransmission)
        Private _CompanyId As String
        Private _CompanyStateLobId As String
        Private _CreditScores As List(Of QuickQuoteCreditScore)
        'Private _DHITransmissions
        Private _LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord) 'shown as LossHistory in xml
        Private _Request As QuickQuoteRequest
        Private _SAQImportedDrivers As List(Of QuickQuoteSAQImportedDriver)
        Private _SAQImportedVehicles As List(Of QuickQuoteSAQImportedVehicle)
        Private _VersionId As String

        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
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
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
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
        Public Property AccidentViolations As List(Of QuickQuoteAccidentViolation)
            Get
                SetParentOfListItems(_AccidentViolations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05640}")
                Return _AccidentViolations
            End Get
            Set(value As List(Of QuickQuoteAccidentViolation))
                _AccidentViolations = value
                SetParentOfListItems(_AccidentViolations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05640}")
            End Set
        End Property
        Public Property ChoicePointTransmissions As List(Of QuickQuoteChoicePointTransmission)
            Get
                SetParentOfListItems(_ChoicePointTransmissions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05641}")
                Return _ChoicePointTransmissions
            End Get
            Set(value As List(Of QuickQuoteChoicePointTransmission))
                _ChoicePointTransmissions = value
                SetParentOfListItems(_ChoicePointTransmissions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05641}")
            End Set
        End Property
        Public Property CompanyId As String
            Get
                Return _CompanyId
            End Get
            Set(value As String)
                _CompanyId = ""
            End Set
        End Property
        Public Property CompanyStateLobId As String
            Get
                Return _CompanyStateLobId
            End Get
            Set(value As String)
                _CompanyStateLobId = value
            End Set
        End Property
        Public Property CreditScores As List(Of QuickQuoteCreditScore)
            Get
                SetParentOfListItems(_CreditScores, "{663B7C7B-F2AC-4BF6-965A-D30F41A05642}")
                Return _CreditScores
            End Get
            Set(value As List(Of QuickQuoteCreditScore))
                _CreditScores = value
                SetParentOfListItems(_CreditScores, "{663B7C7B-F2AC-4BF6-965A-D30F41A05642}")
            End Set
        End Property
        'Public Property DHITransmissions
        Public Property LossHistoryRecords As List(Of QuickQuoteLossHistoryRecord)
            Get
                SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05643}")
                Return _LossHistoryRecords
            End Get
            Set(value As List(Of QuickQuoteLossHistoryRecord))
                _LossHistoryRecords = value
                SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05643}")
            End Set
        End Property
        Public Property Request As QuickQuoteRequest
            Get
                SetObjectsParent(_Request)
                Return _Request
            End Get
            Set(value As QuickQuoteRequest)
                _Request = value
                SetObjectsParent(_Request)
            End Set
        End Property
        Public Property SAQImportedDrivers As List(Of QuickQuoteSAQImportedDriver)
            Get
                SetParentOfListItems(_SAQImportedDrivers, "{D825238B-715F-4B74-AA78-F3525BF4CB24}")
                Return _SAQImportedDrivers
            End Get
            Set(value As List(Of QuickQuoteSAQImportedDriver))
                _SAQImportedDrivers = value
                SetParentOfListItems(_SAQImportedDrivers, "{D825238B-715F-4B74-AA78-F3525BF4CB24}")
            End Set
        End Property
        Public Property SAQImportedVehicles As List(Of QuickQuoteSAQImportedVehicle)
            Get
                SetParentOfListItems(_SAQImportedDrivers, "{D825238B-715F-4B74-AA78-F3525BF4CB25}")
                Return _SAQImportedVehicles
            End Get
            Set(value As List(Of QuickQuoteSAQImportedVehicle))
                _SAQImportedVehicles = value
                SetParentOfListItems(_SAQImportedDrivers, "{D825238B-715F-4B74-AA78-F3525BF4CB25}")
            End Set
        End Property
        Public Property VersionId As String
            Get
                Return _VersionId
            End Get
            Set(value As String)
                _VersionId = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AccidentViolations = New List(Of QuickQuoteAccidentViolation)
            _AccidentViolations = Nothing 'added 8/4/2014
            '_ChoicePointTransmissions = New List(Of QuickQuoteChoicePointTransmission)
            _ChoicePointTransmissions = Nothing 'added 8/4/2014
            _CompanyId = ""
            _CompanyStateLobId = ""
            '_CreditScores = New List(Of QuickQuoteCreditScore)
            _CreditScores = Nothing 'added 8/4/2014
            '_DHITransmissions = Nothing
            '_LossHistoryRecords = New List(Of QuickQuoteLossHistoryRecord)
            _LossHistoryRecords = Nothing 'added 8/4/2014
            _Request = New QuickQuoteRequest
            '_SAQImportedDrivers = New List(Of QuickQuoteSAQImportedDriver)
            _SAQImportedDrivers = Nothing 'added 8/4/2014
            '_SAQImportedVehicles = New List(Of QuickQuoteSAQImportedVehicle)
            _SAQImportedVehicles = Nothing 'added 8/4/2014
            _VersionId = ""
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
                    If _AddedDate IsNot Nothing Then
                        _AddedDate = Nothing
                    End If
                    If _LastModifiedDate IsNot Nothing Then
                        _LastModifiedDate = Nothing
                    End If
                    If _PCAdded_Date IsNot Nothing Then
                        _PCAdded_Date = Nothing
                    End If
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _AccidentViolations IsNot Nothing Then
                        If _AccidentViolations.Count > 0 Then
                            For Each av As QuickQuoteAccidentViolation In _AccidentViolations
                                av.Dispose()
                                av = Nothing
                            Next
                            _AccidentViolations.Clear()
                        End If
                        _AccidentViolations = Nothing
                    End If
                    If _ChoicePointTransmissions IsNot Nothing Then
                        If _ChoicePointTransmissions.Count > 0 Then
                            For Each cpt As QuickQuoteChoicePointTransmission In _ChoicePointTransmissions
                                cpt.Dispose()
                                cpt = Nothing
                            Next
                            _ChoicePointTransmissions.Clear()
                        End If
                        _ChoicePointTransmissions = Nothing
                    End If
                    If _CompanyId IsNot Nothing Then
                        _CompanyId = Nothing
                    End If
                    If _CompanyStateLobId IsNot Nothing Then
                        _CompanyStateLobId = Nothing
                    End If
                    If _CreditScores IsNot Nothing Then
                        If _CreditScores.Count > 0 Then
                            For Each cs As QuickQuoteCreditScore In _CreditScores
                                cs.Dispose()
                                cs = Nothing
                            Next
                            _CreditScores.Clear()
                        End If
                        _CreditScores = Nothing
                    End If
                    'If _DHITransmissions IsNot Nothing Then
                    '    _DHITransmissions = Nothing
                    'End If
                    If _LossHistoryRecords IsNot Nothing Then
                        If _LossHistoryRecords.Count > 0 Then
                            For Each l As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                                l.Dispose()
                                l = Nothing
                            Next
                            _LossHistoryRecords.Clear()
                        End If
                        _LossHistoryRecords = Nothing
                    End If
                    If _Request IsNot Nothing Then
                        _Request.Dispose()
                        _Request = Nothing
                    End If
                    If _SAQImportedDrivers IsNot Nothing Then
                        If _SAQImportedDrivers.Count > 0 Then
                            For Each d As QuickQuoteSAQImportedDriver In _SAQImportedDrivers
                                d.Dispose()
                                d = Nothing
                            Next
                            _SAQImportedDrivers.Clear()
                        End If
                        _SAQImportedDrivers = Nothing
                    End If
                    If _SAQImportedVehicles IsNot Nothing Then
                        If _SAQImportedVehicles.Count > 0 Then
                            For Each v As QuickQuoteSAQImportedVehicle In _SAQImportedVehicles
                                v.Dispose()
                                v = Nothing
                            Next
                            _SAQImportedVehicles.Clear()
                        End If
                        _SAQImportedVehicles = Nothing
                    End If
                    If _VersionId IsNot Nothing Then
                        _VersionId = Nothing
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
