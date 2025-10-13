Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store risk-level lob-specific information for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobRiskLevelInfo 'added 7/23/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'RiskLevel
        Private _Applicants As Generic.List(Of QuickQuoteApplicant)
        Private _Drivers As Generic.List(Of QuickQuoteDriver)
        Private _Locations As Generic.List(Of QuickQuoteLocation)
        Private _Vehicles As Generic.List(Of QuickQuoteVehicle)
        Private _Operators As List(Of QuickQuoteOperator)

        'RiskLevel
        Public Property Applicants As Generic.List(Of QuickQuoteApplicant)
            Get
                Return _Applicants
            End Get
            Set(value As Generic.List(Of QuickQuoteApplicant))
                _Applicants = value
            End Set
        End Property
        Public Property Drivers As Generic.List(Of QuickQuoteDriver)
            Get
                Return _Drivers
            End Get
            Set(value As Generic.List(Of QuickQuoteDriver))
                _Drivers = value
            End Set
        End Property
        Public Property Locations As Generic.List(Of QuickQuoteLocation)
            Get
                Return _Locations
            End Get
            Set(value As Generic.List(Of QuickQuoteLocation))
                _Locations = value
            End Set
        End Property
        Public Property Vehicles As Generic.List(Of QuickQuoteVehicle)
            Get
                Return _Vehicles
            End Get
            Set(value As Generic.List(Of QuickQuoteVehicle))
                _Vehicles = value
            End Set
        End Property
        Public Property Operators As List(Of QuickQuoteOperator)
            Get
                Return _Operators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                _Operators = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        'Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        'Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        Public Sub New(Parent As Object) 'added 7/27/2018 to replace multiple constructors for different objects
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            'RiskLevel
            _Applicants = Nothing
            _Drivers = Nothing
            _Locations = Nothing
            _Vehicles = Nothing
            _Operators = Nothing
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                'If Me.PackagePartTypeId <> "" Then
                '    Dim t As String = ""
                '    t = "PackagePartTypeId: " & Me.PackagePartTypeId
                '    If Me.PackagePartType <> "" Then
                '        t &= " (" & Me.PackagePartType & ")"
                '    End If
                '    str = qqHelper.appendText(str, t, vbCrLf)
                'End If
                'If Me.FullTermPremium <> "" Then
                '    str = qqHelper.appendText(str, "FullTermPremium: " & Me.FullTermPremium, vbCrLf)
                'End If
                If Me.Applicants IsNot Nothing AndAlso Me.Applicants.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Applicants.Count.ToString & " Applicants", vbCrLf)
                End If
                If Me.Drivers IsNot Nothing AndAlso Me.Drivers.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Drivers.Count.ToString & " Drivers", vbCrLf)
                End If
                If Me.Locations IsNot Nothing AndAlso Me.Locations.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Locations.Count.ToString & " Locations", vbCrLf)
                End If
                If Me.Vehicles IsNot Nothing AndAlso Me.Vehicles.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Vehicles.Count.ToString & " Vehicles", vbCrLf)
                End If
                If Me.Operators IsNot Nothing AndAlso Me.Operators.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Operators.Count.ToString & " Operators", vbCrLf)
                End If
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
                    'RiskLevel
                    If _Applicants IsNot Nothing Then
                        If _Applicants.Count > 0 Then
                            For Each a As QuickQuoteApplicant In _Applicants
                                a.Dispose()
                                a = Nothing
                            Next
                            _Applicants.Clear()
                        End If
                        _Applicants = Nothing
                    End If
                    If _Drivers IsNot Nothing Then
                        If _Drivers.Count > 0 Then
                            For Each d As QuickQuoteDriver In _Drivers
                                d.Dispose()
                                d = Nothing
                            Next
                            _Drivers.Clear()
                        End If
                        _Drivers = Nothing
                    End If
                    If _Locations IsNot Nothing Then
                        If _Locations.Count > 0 Then
                            For Each Loc As QuickQuoteLocation In _Locations
                                Loc.Dispose()
                                Loc = Nothing
                            Next
                            _Locations.Clear()
                        End If
                        _Locations = Nothing
                    End If
                    If _Vehicles IsNot Nothing Then
                        If _Vehicles.Count > 0 Then
                            For Each v As QuickQuoteVehicle In _Vehicles
                                v.Dispose()
                                v = Nothing
                            Next
                            _Vehicles.Clear()
                        End If
                        _Vehicles = Nothing
                    End If
                    If _Operators IsNot Nothing Then
                        If _Operators.Count > 0 Then
                            For Each o As QuickQuoteOperator In _Operators
                                o.Dispose()
                                o = Nothing
                            Next
                            _Operators.Clear()
                        End If
                        _Operators = Nothing
                    End If

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
