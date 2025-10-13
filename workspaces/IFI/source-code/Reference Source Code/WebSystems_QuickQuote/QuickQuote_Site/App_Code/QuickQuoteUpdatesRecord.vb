Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store updates information (specific to location or building)
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation (<see cref="QuickQuoteLocation"/>) or QuickQuoteBuilding (<see cref="QuickQuoteBuilding"/>) objects</remarks>
    <Serializable()> _
    Public Class QuickQuoteUpdatesRecord 'added 7/31/2013 for HOM location (previously used on Building also)
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 7/31/2013

        Private _CentralHeatElectric As Boolean
        Private _CentralHeatGas As Boolean
        Private _CentralHeatOil As Boolean
        Private _CentralHeatOther As Boolean
        Private _CentralHeatOtherDescription As String
        Private _CentralHeatUpdateTypeId As String
        Private _CentralHeatUpdateYear As String
        Private _ImprovementsDescription As String
        Private _Electric100Amp As Boolean
        Private _Electric120Amp As Boolean
        Private _Electric200Amp As Boolean
        Private _Electric60Amp As Boolean
        Private _ElectricBurningUnit As Boolean
        Private _ElectricCircuitBreaker As Boolean
        Private _ElectricFuses As Boolean
        Private _ElectricSpaceHeater As Boolean
        Private _ElectricUpdateTypeId As String
        Private _ElectricUpdateYear As String
        Private _PlumbingCopper As Boolean
        Private _PlumbingGalvanized As Boolean
        Private _PlumbingPlastic As Boolean
        Private _PlumbingUpdateTypeId As String
        Private _PlumbingUpdateYear As String
        Private _RoofAsphaltShingle As Boolean
        Private _RoofMetal As Boolean
        Private _RoofOther As Boolean
        Private _RoofOtherDescription As String
        Private _RoofSlate As Boolean
        Private _RoofUpdateTypeId As String
        Private _RoofUpdateYear As String
        Private _RoofWood As Boolean
        Private _SupplementalHeatBurningUnit As Boolean
        Private _SupplementalHeatFireplace As Boolean
        Private _SupplementalHeatFireplaceInsert As Boolean
        Private _SupplementalHeatNA As Boolean
        Private _SupplementalHeatSolidFuel As Boolean
        Private _SupplementalHeatSpaceHeater As Boolean
        Private _SupplementalHeatUpdateTypeId As String
        Private _SupplementalHeatUpdateYear As String
        Private _WindowsUpdateTypeId As String
        Private _WindowsUpdateYear As String
        'added 7/31/2013 for HOM
        Private _InspectionDate As String
        Private _InspectionRemarks As String
        Private _InspectionUpdateTypeId As String 'may need matching InspectionUpdateType variable/property

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property CentralHeatElectric As Boolean
            Get
                Return _CentralHeatElectric
            End Get
            Set(value As Boolean)
                _CentralHeatElectric = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatGas As Boolean
            Get
                Return _CentralHeatGas
            End Get
            Set(value As Boolean)
                _CentralHeatGas = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOil As Boolean
            Get
                Return _CentralHeatOil
            End Get
            Set(value As Boolean)
                _CentralHeatOil = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOther As Boolean
            Get
                Return _CentralHeatOther
            End Get
            Set(value As Boolean)
                _CentralHeatOther = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOtherDescription As String
            Get
                Return _CentralHeatOtherDescription
            End Get
            Set(value As String)
                _CentralHeatOtherDescription = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property CentralHeatUpdateTypeId As String
            Get
                Return _CentralHeatUpdateTypeId
            End Get
            Set(value As String)
                _CentralHeatUpdateTypeId = value
            End Set
        End Property
        Public Property CentralHeatUpdateYear As String
            Get
                Return _CentralHeatUpdateYear
            End Get
            Set(value As String)
                _CentralHeatUpdateYear = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ImprovementsDescription As String
            Get
                Return _ImprovementsDescription
            End Get
            Set(value As String)
                _ImprovementsDescription = value
            End Set
        End Property
        Public Property Electric100Amp As Boolean
            Get
                Return _Electric100Amp
            End Get
            Set(value As Boolean)
                _Electric100Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric120Amp As Boolean
            Get
                Return _Electric120Amp
            End Get
            Set(value As Boolean)
                _Electric120Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric200Amp As Boolean
            Get
                Return _Electric200Amp
            End Get
            Set(value As Boolean)
                _Electric200Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric60Amp As Boolean
            Get
                Return _Electric60Amp
            End Get
            Set(value As Boolean)
                _Electric60Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricBurningUnit As Boolean
            Get
                Return _ElectricBurningUnit
            End Get
            Set(value As Boolean)
                _ElectricBurningUnit = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricCircuitBreaker As Boolean
            Get
                Return _ElectricCircuitBreaker
            End Get
            Set(value As Boolean)
                _ElectricCircuitBreaker = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricFuses As Boolean
            Get
                Return _ElectricFuses
            End Get
            Set(value As Boolean)
                _ElectricFuses = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricSpaceHeater As Boolean
            Get
                Return _ElectricSpaceHeater
            End Get
            Set(value As Boolean)
                _ElectricSpaceHeater = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property ElectricUpdateTypeId As String
            Get
                Return _ElectricUpdateTypeId
            End Get
            Set(value As String)
                _ElectricUpdateTypeId = value
            End Set
        End Property
        Public Property ElectricUpdateYear As String
            Get
                Return _ElectricUpdateYear
            End Get
            Set(value As String)
                _ElectricUpdateYear = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingCopper As Boolean
            Get
                Return _PlumbingCopper
            End Get
            Set(value As Boolean)
                _PlumbingCopper = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingGalvanized As Boolean
            Get
                Return _PlumbingGalvanized
            End Get
            Set(value As Boolean)
                _PlumbingGalvanized = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingPlastic As Boolean
            Get
                Return _PlumbingPlastic
            End Get
            Set(value As Boolean)
                _PlumbingPlastic = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property PlumbingUpdateTypeId As String
            Get
                Return _PlumbingUpdateTypeId
            End Get
            Set(value As String)
                _PlumbingUpdateTypeId = value
            End Set
        End Property
        Public Property PlumbingUpdateYear As String
            Get
                Return _PlumbingUpdateYear
            End Get
            Set(value As String)
                _PlumbingUpdateYear = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofAsphaltShingle As Boolean
            Get
                Return _RoofAsphaltShingle
            End Get
            Set(value As Boolean)
                _RoofAsphaltShingle = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofMetal As Boolean
            Get
                Return _RoofMetal
            End Get
            Set(value As Boolean)
                _RoofMetal = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofOther As Boolean
            Get
                Return _RoofOther
            End Get
            Set(value As Boolean)
                _RoofOther = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofOtherDescription As String
            Get
                Return _RoofOtherDescription
            End Get
            Set(value As String)
                _RoofOtherDescription = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofSlate As Boolean
            Get
                Return _RoofSlate
            End Get
            Set(value As Boolean)
                _RoofSlate = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property RoofUpdateTypeId As String
            Get
                Return _RoofUpdateTypeId
            End Get
            Set(value As String)
                _RoofUpdateTypeId = value
            End Set
        End Property
        Public Property RoofUpdateYear As String
            Get
                Return _RoofUpdateYear
            End Get
            Set(value As String)
                _RoofUpdateYear = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofWood As Boolean
            Get
                Return _RoofWood
            End Get
            Set(value As Boolean)
                _RoofWood = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatBurningUnit As Boolean
            Get
                Return _SupplementalHeatBurningUnit
            End Get
            Set(value As Boolean)
                _SupplementalHeatBurningUnit = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatFireplace As Boolean
            Get
                Return _SupplementalHeatFireplace
            End Get
            Set(value As Boolean)
                _SupplementalHeatFireplace = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatFireplaceInsert As Boolean
            Get
                Return _SupplementalHeatFireplaceInsert
            End Get
            Set(value As Boolean)
                _SupplementalHeatFireplaceInsert = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatNA As Boolean
            Get
                Return _SupplementalHeatNA
            End Get
            Set(value As Boolean)
                _SupplementalHeatNA = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatSolidFuel As Boolean
            Get
                Return _SupplementalHeatSolidFuel
            End Get
            Set(value As Boolean)
                _SupplementalHeatSolidFuel = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatSpaceHeater As Boolean
            Get
                Return _SupplementalHeatSpaceHeater
            End Get
            Set(value As Boolean)
                _SupplementalHeatSpaceHeater = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property SupplementalHeatUpdateTypeId As String
            Get
                Return _SupplementalHeatUpdateTypeId
            End Get
            Set(value As String)
                _SupplementalHeatUpdateTypeId = value
            End Set
        End Property
        Public Property SupplementalHeatUpdateYear As String
            Get
                Return _SupplementalHeatUpdateYear
            End Get
            Set(value As String)
                _SupplementalHeatUpdateYear = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property WindowsUpdateTypeId As String
            Get
                Return _WindowsUpdateTypeId
            End Get
            Set(value As String)
                _WindowsUpdateTypeId = value
            End Set
        End Property
        Public Property WindowsUpdateYear As String
            Get
                Return _WindowsUpdateYear
            End Get
            Set(value As String)
                _WindowsUpdateYear = value
                'If _WindowsUpdateTypeId = "" Then
                '    _WindowsUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        'added 7/31/2013 for HOM
        Public Property InspectionDate As String
            Get
                Return _InspectionDate
            End Get
            Set(value As String)
                _InspectionDate = value
                qqHelper.ConvertToShortDate(_InspectionDate)
            End Set
        End Property
        Public Property InspectionRemarks As String
            Get
                Return _InspectionRemarks
            End Get
            Set(value As String)
                _InspectionRemarks = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UpdateType table (0=N/A, 1=Partial, 2=Complete)</remarks>
        Public Property InspectionUpdateTypeId As String
            Get
                Return _InspectionUpdateTypeId
            End Get
            Set(value As String)
                _InspectionUpdateTypeId = value
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
            _CentralHeatElectric = False
            _CentralHeatGas = False
            _CentralHeatOil = False
            _CentralHeatOther = False
            _CentralHeatOtherDescription = ""
            _CentralHeatUpdateTypeId = ""
            _CentralHeatUpdateYear = ""
            _ImprovementsDescription = ""
            _Electric100Amp = False
            _Electric120Amp = False
            _Electric200Amp = False
            _Electric60Amp = False
            _ElectricBurningUnit = False
            _ElectricCircuitBreaker = False
            _ElectricFuses = False
            _ElectricSpaceHeater = False
            _ElectricUpdateTypeId = ""
            _ElectricUpdateYear = ""
            _PlumbingCopper = False
            _PlumbingGalvanized = False
            _PlumbingPlastic = False
            _PlumbingUpdateTypeId = ""
            _PlumbingUpdateYear = ""
            _RoofAsphaltShingle = False
            _RoofMetal = False
            _RoofOther = False
            _RoofOtherDescription = ""
            _RoofSlate = False
            _RoofUpdateTypeId = ""
            _RoofUpdateYear = ""
            _RoofWood = False
            _SupplementalHeatBurningUnit = False
            _SupplementalHeatFireplace = False
            _SupplementalHeatFireplaceInsert = False
            _SupplementalHeatNA = False
            _SupplementalHeatSolidFuel = False
            _SupplementalHeatSpaceHeater = False
            _SupplementalHeatUpdateTypeId = ""
            _SupplementalHeatUpdateYear = ""
            _WindowsUpdateTypeId = ""
            _WindowsUpdateYear = ""
            'added 7/31/2013 for HOM
            _InspectionDate = ""
            _InspectionRemarks = ""
            _InspectionUpdateTypeId = ""

            _DetailStatusCode = "" 'added 5/15/2019

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
                    If _CentralHeatElectric <> Nothing Then
                        _CentralHeatElectric = Nothing
                    End If
                    If _CentralHeatGas <> Nothing Then
                        _CentralHeatGas = Nothing
                    End If
                    If _CentralHeatOil <> Nothing Then
                        _CentralHeatOil = Nothing
                    End If
                    If _CentralHeatOther <> Nothing Then
                        _CentralHeatOther = Nothing
                    End If
                    If _CentralHeatOtherDescription IsNot Nothing Then
                        _CentralHeatOtherDescription = Nothing
                    End If
                    If _CentralHeatUpdateTypeId IsNot Nothing Then
                        _CentralHeatUpdateTypeId = Nothing
                    End If
                    If _CentralHeatUpdateYear IsNot Nothing Then
                        _CentralHeatUpdateYear = Nothing
                    End If
                    If _ImprovementsDescription IsNot Nothing Then
                        _ImprovementsDescription = Nothing
                    End If
                    If _Electric100Amp <> Nothing Then
                        _Electric100Amp = Nothing
                    End If
                    If _Electric120Amp <> Nothing Then
                        _Electric120Amp = Nothing
                    End If
                    If _Electric200Amp <> Nothing Then
                        _Electric200Amp = Nothing
                    End If
                    If _Electric60Amp <> Nothing Then
                        _Electric60Amp = Nothing
                    End If
                    If _ElectricBurningUnit <> Nothing Then
                        _ElectricBurningUnit = Nothing
                    End If
                    If _ElectricCircuitBreaker <> Nothing Then
                        _ElectricCircuitBreaker = Nothing
                    End If
                    If _ElectricFuses <> Nothing Then
                        _ElectricFuses = Nothing
                    End If
                    If _ElectricSpaceHeater <> Nothing Then
                        _ElectricSpaceHeater = Nothing
                    End If
                    If _ElectricUpdateTypeId IsNot Nothing Then
                        _ElectricUpdateTypeId = Nothing
                    End If
                    If _ElectricUpdateYear IsNot Nothing Then
                        _ElectricUpdateYear = Nothing
                    End If
                    If _PlumbingCopper <> Nothing Then
                        _PlumbingCopper = Nothing
                    End If
                    If _PlumbingGalvanized <> Nothing Then
                        _PlumbingGalvanized = Nothing
                    End If
                    If _PlumbingPlastic <> Nothing Then
                        _PlumbingPlastic = Nothing
                    End If
                    If _PlumbingUpdateTypeId IsNot Nothing Then
                        _PlumbingUpdateTypeId = Nothing
                    End If
                    If _PlumbingUpdateYear IsNot Nothing Then
                        _PlumbingUpdateYear = Nothing
                    End If
                    If _RoofAsphaltShingle <> Nothing Then
                        _RoofAsphaltShingle = Nothing
                    End If
                    If _RoofMetal <> Nothing Then
                        _RoofMetal = Nothing
                    End If
                    If _RoofOther <> Nothing Then
                        _RoofOther = Nothing
                    End If
                    If _RoofOtherDescription IsNot Nothing Then
                        _RoofOtherDescription = Nothing
                    End If
                    If _RoofSlate <> Nothing Then
                        _RoofSlate = Nothing
                    End If
                    If _RoofUpdateTypeId IsNot Nothing Then
                        _RoofUpdateTypeId = Nothing
                    End If
                    If _RoofUpdateYear IsNot Nothing Then
                        _RoofUpdateYear = Nothing
                    End If
                    If _RoofWood <> Nothing Then
                        _RoofWood = Nothing
                    End If
                    If _SupplementalHeatBurningUnit <> Nothing Then
                        _SupplementalHeatBurningUnit = Nothing
                    End If
                    If _SupplementalHeatFireplace <> Nothing Then
                        _SupplementalHeatFireplace = Nothing
                    End If
                    If _SupplementalHeatFireplaceInsert <> Nothing Then
                        _SupplementalHeatFireplaceInsert = Nothing
                    End If
                    If _SupplementalHeatNA <> Nothing Then
                        _SupplementalHeatNA = Nothing
                    End If
                    If _SupplementalHeatSolidFuel <> Nothing Then
                        _SupplementalHeatSolidFuel = Nothing
                    End If
                    If _SupplementalHeatSpaceHeater <> Nothing Then
                        _SupplementalHeatSpaceHeater = Nothing
                    End If
                    If _SupplementalHeatUpdateTypeId IsNot Nothing Then
                        _SupplementalHeatUpdateTypeId = Nothing
                    End If
                    If _SupplementalHeatUpdateYear IsNot Nothing Then
                        _SupplementalHeatUpdateYear = Nothing
                    End If
                    If _WindowsUpdateTypeId IsNot Nothing Then
                        _WindowsUpdateTypeId = Nothing
                    End If
                    If _WindowsUpdateYear IsNot Nothing Then
                        _WindowsUpdateYear = Nothing
                    End If
                    If _InspectionDate IsNot Nothing Then
                        _InspectionDate = Nothing
                    End If
                    If _InspectionRemarks IsNot Nothing Then
                        _InspectionRemarks = Nothing
                    End If
                    If _InspectionUpdateTypeId IsNot Nothing Then
                        _InspectionUpdateTypeId = Nothing
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
