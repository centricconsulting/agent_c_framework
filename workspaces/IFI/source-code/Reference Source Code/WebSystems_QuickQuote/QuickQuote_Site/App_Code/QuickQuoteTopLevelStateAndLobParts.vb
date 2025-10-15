Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level state/lob part information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelStateAndLobParts 'added 7/26/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PackageParts As Generic.List(Of QuickQuotePackagePart)

        Private _MultiStateQuotes As List(Of QuickQuoteObject)

        'added 7/28/2018
        Private _OriginallyHadMultipleQuoteStates As Boolean
        Private _OriginalQuoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState)
        Private _OriginalGoverningState As QuickQuoteHelperClass.QuickQuoteState
        'added 7/31/2018
        Private _OriginallyInMultiStatePackageFormat As Boolean
        Private _OriginalPackageParts As List(Of QuickQuotePackagePart)

        'added 8/1/2018
        Private _NeedsMultiStateFormat As Boolean
        Private _LobIdToUse As String

        'added 9/17/2018
        Private _MasterPackageLocations As List(Of QuickQuoteLocation)
        Private _MasterPackageVehicles As List(Of QuickQuoteVehicle)
        'Private _MasterPackageAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        'added 10/18/2018
        Private _CGLPackageLocations As List(Of QuickQuoteLocation)
        Private _CPRPackageLocations As List(Of QuickQuoteLocation)
        Private _CIMPackageLocations As List(Of QuickQuoteLocation)
        Private _CRMPackageLocations As List(Of QuickQuoteLocation)
        Private _GARPackageLocations As List(Of QuickQuoteLocation)
        Private _CGLPackageVehicles As List(Of QuickQuoteVehicle)
        Private _CPRPackageVehicles As List(Of QuickQuoteVehicle)
        Private _CIMPackageVehicles As List(Of QuickQuoteVehicle)
        Private _CRMPackageVehicles As List(Of QuickQuoteVehicle)
        Private _GARPackageVehicles As List(Of QuickQuoteVehicle)
        'added 10/19/2018
        Private _MasterPackageModifiers As List(Of QuickQuoteModifier)
        Private _CGLPackageModifiers As List(Of QuickQuoteModifier)
        Private _CPRPackageModifiers As List(Of QuickQuoteModifier)
        Private _CIMPackageModifiers As List(Of QuickQuoteModifier)
        Private _CRMPackageModifiers As List(Of QuickQuoteModifier)
        Private _GARPackageModifiers As List(Of QuickQuoteModifier)

        'added 10/17/2018
        Private _CanUseLocationNumForMasterPartLocationReconciliation As Boolean
        Private _CanUseLocationNumForCGLPartLocationReconciliation As Boolean
        Private _CanUseLocationNumForCPRPartLocationReconciliation As Boolean
        Private _CanUseLocationNumForCIMPartLocationReconciliation As Boolean
        Private _CanUseLocationNumForCRMPartLocationReconciliation As Boolean
        Private _CanUseLocationNumForGARPartLocationReconciliation As Boolean
        Private _CanUseVehicleNumForMasterPartVehicleReconciliation As Boolean
        'added 10/18/2018
        Private _CanUseVehicleNumForCGLPartVehicleReconciliation As Boolean
        Private _CanUseVehicleNumForCPRPartVehicleReconciliation As Boolean
        Private _CanUseVehicleNumForCIMPartVehicleReconciliation As Boolean
        Private _CanUseVehicleNumForCRMPartVehicleReconciliation As Boolean
        Private _CanUseVehicleNumForGARPartVehicleReconciliation As Boolean
        Private _PayrollAmount As String

        Private _QuoteLevel As QuickQuoteHelperClass.QuoteLevel '12/30/2018 - moved here from TopLevelBaseCommonInfo object so it would not get Copied between topLevel and stateLevel quotes


        Public Property PackageParts As Generic.List(Of QuickQuotePackagePart)
            Get
                Return _PackageParts
            End Get
            Set(value As Generic.List(Of QuickQuotePackagePart))
                _PackageParts = value
            End Set
        End Property
        Public Property MultiStateQuotes As List(Of QuickQuoteObject)
            Get
                Return _MultiStateQuotes
            End Get
            Set(value As List(Of QuickQuoteObject))
                _MultiStateQuotes = value
            End Set
        End Property

        'added 7/28/2018
        Public ReadOnly Property OriginallyHadMultipleQuoteStates As Boolean
            Get
                Return _OriginallyHadMultipleQuoteStates
            End Get
        End Property
        Public ReadOnly Property OriginalQuoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState)
            Get
                Return _OriginalQuoteStates
            End Get
        End Property
        Public ReadOnly Property OriginalGoverningState As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return _OriginalGoverningState
            End Get
        End Property
        'added 7/31/2018
        Public ReadOnly Property OriginallyInMultiStatePackageFormat As Boolean
            Get
                Return _OriginallyInMultiStatePackageFormat
            End Get
        End Property
        Public ReadOnly Property OriginalPackageParts As List(Of QuickQuotePackagePart)
            Get
                Return _OriginalPackageParts
            End Get
        End Property

        'added 8/1/2018
        Public ReadOnly Property NeedsMultiStateFormat As Boolean
            Get
                Return _NeedsMultiStateFormat
            End Get
        End Property
        Public ReadOnly Property LobIdToUse As String
            Get
                Return _LobIdToUse
            End Get
        End Property

        'added 9/17/2018
        Public ReadOnly Property MasterPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _MasterPackageLocations
            End Get
        End Property
        Public ReadOnly Property MasterPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _MasterPackageVehicles
            End Get
        End Property
        'Public ReadOnly Property MasterPackageAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        '    Get
        '        Return _MasterPackageAdditionalInterests
        '    End Get
        'End Property
        'added 10/18/2018
        Public ReadOnly Property CGLPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _CGLPackageLocations
            End Get
        End Property
        Public ReadOnly Property CPRPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _CPRPackageLocations
            End Get
        End Property
        Public ReadOnly Property CIMPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _CIMPackageLocations
            End Get
        End Property
        Public ReadOnly Property CRMPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _CRMPackageLocations
            End Get
        End Property
        Public ReadOnly Property GARPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return _GARPackageLocations
            End Get
        End Property
        Public ReadOnly Property CGLPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _CGLPackageVehicles
            End Get
        End Property
        Public ReadOnly Property CPRPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _CPRPackageVehicles
            End Get
        End Property
        Public ReadOnly Property CIMPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _CIMPackageVehicles
            End Get
        End Property
        Public ReadOnly Property CRMPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _CRMPackageVehicles
            End Get
        End Property
        Public ReadOnly Property GARPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return _GARPackageVehicles
            End Get
        End Property
        'added 10/19/2018
        Public ReadOnly Property MasterPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _MasterPackageModifiers
            End Get
        End Property
        Public ReadOnly Property CGLPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _CGLPackageModifiers
            End Get
        End Property
        Public ReadOnly Property CPRPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _CPRPackageModifiers
            End Get
        End Property
        Public ReadOnly Property CIMPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _CIMPackageModifiers
            End Get
        End Property
        Public ReadOnly Property CRMPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _CRMPackageModifiers
            End Get
        End Property
        Public ReadOnly Property GARPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return _GARPackageModifiers
            End Get
        End Property

        'added 10/17/2018
        Public Property CanUseLocationNumForMasterPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForMasterPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForMasterPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForCGLPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForCGLPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForCGLPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForCPRPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForCPRPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForCPRPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForCIMPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForCIMPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForCIMPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForCRMPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForCRMPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForCRMPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForGARPartLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForGARPartLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForGARPartLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForMasterPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForMasterPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForMasterPartVehicleReconciliation = value
            End Set
        End Property
        'added 10/18/2018
        Public Property CanUseVehicleNumForCGLPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForCGLPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForCGLPartVehicleReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForCPRPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForCPRPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForCPRPartVehicleReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForCIMPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForCIMPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForCIMPartVehicleReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForCRMPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForCRMPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForCRMPartVehicleReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForGARPartVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForGARPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForGARPartVehicleReconciliation = value
            End Set
        End Property

        Public ReadOnly Property QuoteLevel As QuickQuoteHelperClass.QuoteLevel '12/30/2018 - moved here from TopLevelBaseCommonInfo object so it would not get Copied between topLevel and stateLevel quotes
            Get
                Return _QuoteLevel
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object) 'generic, but Parent will likely be TopLevelQuoteInfo
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _PackageParts = Nothing

            _MultiStateQuotes = Nothing

            'added 7/28/2018
            _OriginallyHadMultipleQuoteStates = False
            _OriginalQuoteStates = Nothing
            _OriginalGoverningState = QuickQuoteHelperClass.QuickQuoteState.None
            'added 7/31/2018
            _OriginallyInMultiStatePackageFormat = False
            _OriginalPackageParts = Nothing

            'added 8/1/2018
            _NeedsMultiStateFormat = False
            _LobIdToUse = ""

            'added 9/17/2018
            _MasterPackageLocations = Nothing
            _MasterPackageVehicles = Nothing
            '_MasterPackageAdditionalInterests = Nothing
            'added 10/18/2018
            _CGLPackageLocations = Nothing
            _CPRPackageLocations = Nothing
            _CIMPackageLocations = Nothing
            _CRMPackageLocations = Nothing
            _GARPackageLocations = Nothing
            _CGLPackageVehicles = Nothing
            _CPRPackageVehicles = Nothing
            _CIMPackageVehicles = Nothing
            _CRMPackageVehicles = Nothing
            _GARPackageVehicles = Nothing
            'added 10/19/2018
            _MasterPackageModifiers = Nothing
            _CGLPackageModifiers = Nothing
            _CPRPackageModifiers = Nothing
            _CIMPackageModifiers = Nothing
            _CRMPackageModifiers = Nothing
            _GARPackageModifiers = Nothing

            'added 10/17/2018
            _CanUseLocationNumForMasterPartLocationReconciliation = False
            _CanUseLocationNumForCGLPartLocationReconciliation = False
            _CanUseLocationNumForCPRPartLocationReconciliation = False
            _CanUseLocationNumForCIMPartLocationReconciliation = False
            _CanUseLocationNumForCRMPartLocationReconciliation = False
            _CanUseLocationNumForGARPartLocationReconciliation = False
            _CanUseVehicleNumForMasterPartVehicleReconciliation = False
            'added 10/18/2018
            _CanUseVehicleNumForCGLPartVehicleReconciliation = False
            _CanUseVehicleNumForCPRPartVehicleReconciliation = False
            _CanUseVehicleNumForCIMPartVehicleReconciliation = False
            _CanUseVehicleNumForCRMPartVehicleReconciliation = False
            _CanUseVehicleNumForGARPartVehicleReconciliation = False

            _QuoteLevel = QuickQuoteHelperClass.QuoteLevel.None '12/30/2018 - moved here from TopLevelBaseCommonInfo object so it would not get Copied between topLevel and stateLevel quotes

        End Sub

        'added 7/28/2018
        Protected Friend Sub Set_OriginallyHadMultipleQuoteStates(ByVal hadMultipleQuoteStates As Boolean)
            _OriginallyHadMultipleQuoteStates = hadMultipleQuoteStates
        End Sub
        Protected Friend Sub Set_OriginalQuoteStates(ByVal states As List(Of QuickQuoteHelperClass.QuickQuoteState))
            _OriginalQuoteStates = states
        End Sub
        Protected Friend Sub Set_OriginalGoverningState(ByVal governingState As QuickQuoteHelperClass.QuickQuoteState)
            _OriginalGoverningState = governingState
        End Sub
        'added 7/31/2018
        Protected Friend Sub Set_OriginallyInMultiStatePackageFormat(ByVal inMultiStatePackageFormat As Boolean)
            _OriginallyInMultiStatePackageFormat = inMultiStatePackageFormat
        End Sub
        Protected Friend Sub ArchiveAndClearPackageParts()
            _OriginalPackageParts = qqHelper.CloneObject(_PackageParts)
            'updated 8/5/2018 to test different way to backup packageParts since CloneObject appears to be duplicating things (i.e. locations, coverages, etc.); updated logic to clone 1 packagePart at-a-time instead of the whole list appears to have the same problem
            'If _PackageParts IsNot Nothing Then
            '    _OriginalPackageParts = New List(Of QuickQuotePackagePart)
            '    If _PackageParts.Count > 0 Then
            '        For Each pp As QuickQuotePackagePart In _PackageParts
            '            _OriginalPackageParts.Add(qqHelper.CloneObject(pp))
            '        Next
            '    End If
            'Else
            '    qqHelper.DisposePackageParts(_OriginalPackageParts)
            'End If
            qqHelper.DisposePackageParts(_PackageParts)
        End Sub

        'added 8/1/2018
        Protected Friend Sub Set_NeedsMultiStateFormat(ByVal needsMultiState As Boolean)
            _NeedsMultiStateFormat = needsMultiState
        End Sub
        Protected Friend Sub Set_LobIdToUse(ByVal lobId As String)
            _LobIdToUse = lobId
        End Sub

        'added 9/17/2018
        Protected Friend Sub Set_MasterPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _MasterPackageLocations = locs
        End Sub
        Protected Friend Sub Set_MasterPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _MasterPackageVehicles = vehs
        End Sub
        'Protected Friend Sub Set_MasterPackageAdditionalInterests(ByVal ais As List(Of QuickQuoteAdditionalInterest))
        '    _MasterPackageAdditionalInterests = ais
        'End Sub
        'added 10/18/2018
        Protected Friend Sub Set_CGLPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _CGLPackageLocations = locs
        End Sub
        Protected Friend Sub Set_CPRPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _CPRPackageLocations = locs
        End Sub
        Protected Friend Sub Set_CIMPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _CIMPackageLocations = locs
        End Sub
        Protected Friend Sub Set_CRMPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _CRMPackageLocations = locs
        End Sub
        Protected Friend Sub Set_GARPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            _GARPackageLocations = locs
        End Sub
        Protected Friend Sub Set_CGLPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _CGLPackageVehicles = vehs
        End Sub
        Protected Friend Sub Set_CPRPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _CPRPackageVehicles = vehs
        End Sub
        Protected Friend Sub Set_CIMPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _CIMPackageVehicles = vehs
        End Sub
        Protected Friend Sub Set_CRMPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _CRMPackageVehicles = vehs
        End Sub
        Protected Friend Sub Set_GARPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            _GARPackageVehicles = vehs
        End Sub
        'added 10/19/2018
        Protected Friend Sub Set_MasterPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _MasterPackageModifiers = mods
        End Sub
        Protected Friend Sub Set_CGLPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _CGLPackageModifiers = mods
        End Sub
        Protected Friend Sub Set_CPRPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _CPRPackageModifiers = mods
        End Sub
        Protected Friend Sub Set_CIMPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _CIMPackageModifiers = mods
        End Sub
        Protected Friend Sub Set_CRMPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _CRMPackageModifiers = mods
        End Sub
        Protected Friend Sub Set_GARPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            _GARPackageModifiers = mods
        End Sub

        Protected Friend Sub Set_QuoteLevel(ByVal level As QuickQuoteHelperClass.QuoteLevel) 'added 7/28/2018; 12/30/2018 - moved here from TopLevelBaseCommonInfo object so it would not get Copied between topLevel and stateLevel quotes
            _QuoteLevel = level
        End Sub


        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then

            Else
                str = "Nothing"
            End If
            Return str
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    'If _PackageParts IsNot Nothing Then
                    '    If _PackageParts.Count > 0 Then
                    '        For Each pp As QuickQuotePackagePart In _PackageParts
                    '            pp.Dispose()
                    '            pp = Nothing
                    '        Next
                    '        _PackageParts.Clear()
                    '    End If
                    '    _PackageParts = Nothing
                    'End If
                    'updated 7/31/2018 to use new common method
                    qqHelper.DisposePackageParts(_PackageParts)
                    qqHelper.DisposeQuickQuoteObjects(_MultiStateQuotes)

                    'added 7/28/2018
                    _OriginallyHadMultipleQuoteStates = Nothing
                    If _OriginalQuoteStates IsNot Nothing Then
                        If _OriginalQuoteStates.Count > 0 Then
                            For Each s As QuickQuoteHelperClass.QuickQuoteState In _OriginalQuoteStates
                                s = Nothing
                            Next
                            _OriginalQuoteStates.Clear()
                        End If
                        _OriginalQuoteStates = Nothing
                    End If
                    _OriginalGoverningState = Nothing
                    'added 7/31/2018
                    _OriginallyInMultiStatePackageFormat = Nothing
                    qqHelper.DisposePackageParts(_OriginalPackageParts)

                    'added 8/1/2018
                    _NeedsMultiStateFormat = Nothing
                    qqHelper.DisposeString(_LobIdToUse)

                    'added 9/17/2018
                    qqHelper.DisposeLocations(_MasterPackageLocations)
                    qqHelper.DisposeVehicles(_MasterPackageVehicles)
                    'qqHelper.DisposeAdditionalInterests(_MasterPackageAdditionalInterests)
                    'added 10/18/2018
                    qqHelper.DisposeLocations(_CGLPackageLocations)
                    qqHelper.DisposeLocations(_CPRPackageLocations)
                    qqHelper.DisposeLocations(_CIMPackageLocations)
                    qqHelper.DisposeLocations(_CRMPackageLocations)
                    qqHelper.DisposeLocations(_GARPackageLocations)
                    qqHelper.DisposeVehicles(_CGLPackageVehicles)
                    qqHelper.DisposeVehicles(_CPRPackageVehicles)
                    qqHelper.DisposeVehicles(_CIMPackageVehicles)
                    qqHelper.DisposeVehicles(_CRMPackageVehicles)
                    qqHelper.DisposeVehicles(_GARPackageVehicles)
                    'added 10/19/2018
                    qqHelper.DisposeModifiers(_MasterPackageModifiers)
                    qqHelper.DisposeModifiers(_CGLPackageModifiers)
                    qqHelper.DisposeModifiers(_CPRPackageModifiers)
                    qqHelper.DisposeModifiers(_CIMPackageModifiers)
                    qqHelper.DisposeModifiers(_CRMPackageModifiers)
                    qqHelper.DisposeModifiers(_GARPackageModifiers)

                    'added 10/17/2018
                    _CanUseLocationNumForMasterPartLocationReconciliation = Nothing
                    _CanUseLocationNumForCGLPartLocationReconciliation = Nothing
                    _CanUseLocationNumForCPRPartLocationReconciliation = Nothing
                    _CanUseLocationNumForCIMPartLocationReconciliation = Nothing
                    _CanUseLocationNumForCRMPartLocationReconciliation = Nothing
                    _CanUseLocationNumForGARPartLocationReconciliation = Nothing
                    _CanUseVehicleNumForMasterPartVehicleReconciliation = Nothing
                    'added 10/18/2018
                    _CanUseVehicleNumForCGLPartVehicleReconciliation = Nothing
                    _CanUseVehicleNumForCPRPartVehicleReconciliation = Nothing
                    _CanUseVehicleNumForCIMPartVehicleReconciliation = Nothing
                    _CanUseVehicleNumForCRMPartVehicleReconciliation = Nothing
                    _CanUseVehicleNumForGARPartVehicleReconciliation = Nothing

                    _QuoteLevel = Nothing '12/30/2018 - moved here from TopLevelBaseCommonInfo object so it would not get Copied between topLevel and stateLevel quotes

                    MyBase.Dispose()
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
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace


