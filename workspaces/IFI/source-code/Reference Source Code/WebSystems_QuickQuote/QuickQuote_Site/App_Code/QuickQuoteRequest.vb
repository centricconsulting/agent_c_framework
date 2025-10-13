Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store request information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteRequest 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _AutoDataPrefill As QuickQuoteAutoDataPrefill
        Private _ClueAuto As QuickQuoteClueAuto
        Private _ClueProperty As QuickQuoteClueProperty
        Private _CreditLocation As QuickQuoteCreditLocation
        Private _CreditVehicle As QuickQuoteCreditVehicle
        Private _MVR As QuickQuoteMVR
        Private _SAQ As QuickQuoteSAQ

        Public Property AutoDataPrefill As QuickQuoteAutoDataPrefill
            Get
                SetObjectsParent(_AutoDataPrefill)
                Return _AutoDataPrefill
            End Get
            Set(value As QuickQuoteAutoDataPrefill)
                _AutoDataPrefill = value
                SetObjectsParent(_AutoDataPrefill)
            End Set
        End Property
        Public Property ClueAuto As QuickQuoteClueAuto
            Get
                SetObjectsParent(_ClueAuto)
                Return _ClueAuto
            End Get
            Set(value As QuickQuoteClueAuto)
                _ClueAuto = value
                SetObjectsParent(_ClueAuto)
            End Set
        End Property
        Public Property ClueProperty As QuickQuoteClueProperty
            Get
                SetObjectsParent(_ClueProperty)
                Return _ClueProperty
            End Get
            Set(value As QuickQuoteClueProperty)
                _ClueProperty = value
                SetObjectsParent(_ClueProperty)
            End Set
        End Property
        Public Property CreditLocation As QuickQuoteCreditLocation
            Get
                SetObjectsParent(_CreditLocation)
                Return _CreditLocation
            End Get
            Set(value As QuickQuoteCreditLocation)
                _CreditLocation = value
                SetObjectsParent(_CreditLocation)
            End Set
        End Property
        Public Property CreditVehicle As QuickQuoteCreditVehicle
            Get
                SetObjectsParent(_CreditVehicle)
                Return _CreditVehicle
            End Get
            Set(value As QuickQuoteCreditVehicle)
                _CreditVehicle = value
                SetObjectsParent(_CreditVehicle)
            End Set
        End Property
        Public Property MVR As QuickQuoteMVR
            Get
                SetObjectsParent(_MVR)
                Return _MVR
            End Get
            Set(value As QuickQuoteMVR)
                _MVR = value
                SetObjectsParent(_MVR)
            End Set
        End Property
        Public Property SAQ As QuickQuoteSAQ
            Get
                SetObjectsParent(_SAQ)
                Return _SAQ
            End Get
            Set(value As QuickQuoteSAQ)
                _SAQ = value
                SetObjectsParent(_SAQ)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AutoDataPrefill = New QuickQuoteAutoDataPrefill
            _ClueAuto = New QuickQuoteClueAuto
            _ClueProperty = New QuickQuoteClueProperty
            _CreditLocation = New QuickQuoteCreditLocation
            _CreditVehicle = New QuickQuoteCreditVehicle
            _MVR = New QuickQuoteMVR
            _SAQ = New QuickQuoteSAQ
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
                    If _AutoDataPrefill IsNot Nothing Then
                        _AutoDataPrefill = Nothing
                    End If
                    If _ClueAuto IsNot Nothing Then
                        _ClueAuto = Nothing
                    End If
                    If _ClueProperty IsNot Nothing Then
                        _ClueProperty = Nothing
                    End If
                    If _CreditLocation IsNot Nothing Then
                        _CreditLocation = Nothing
                    End If
                    If _CreditVehicle IsNot Nothing Then
                        _CreditVehicle = Nothing
                    End If
                    If _MVR IsNot Nothing Then
                        _MVR = Nothing
                    End If
                    If _SAQ IsNot Nothing Then
                        _SAQ = Nothing
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
