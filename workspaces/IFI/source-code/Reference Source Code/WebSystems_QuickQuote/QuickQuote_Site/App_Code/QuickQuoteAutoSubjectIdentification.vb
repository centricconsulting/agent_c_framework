Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store auto subject identification information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteAutoSubjectIdentification 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Private _CurrentAddress As QuickQuoteThirdPartyDataAddress
        Private _DriversLicenseInfo As QuickQuoteDriversLicenseInfo
        Private _PersonalIdentification As QuickQuotePersonalIdentification
        Private _PolicyInfo As QuickQuoteThirdPartyPolicyInfo 'changed class name 4/20/2020 from QuickQuotePolicyInfo
        Private _PriorAddress As QuickQuoteThirdPartyDataAddress
        Private _PriorDriversLicenseInfo As QuickQuoteDriversLicenseInfo
        Private _SubUnitQuotebackInfo As QuickQuoteSubUnitQuotebackInfo

        Public Property CurrentAddress As QuickQuoteThirdPartyDataAddress
            Get
                SetObjectsParent(_CurrentAddress)
                Return _CurrentAddress
            End Get
            Set(value As QuickQuoteThirdPartyDataAddress)
                _CurrentAddress = value
                SetObjectsParent(_CurrentAddress)
            End Set
        End Property
        Public Property DriversLicenseInfo As QuickQuoteDriversLicenseInfo
            Get
                SetObjectsParent(_DriversLicenseInfo)
                Return _DriversLicenseInfo
            End Get
            Set(value As QuickQuoteDriversLicenseInfo)
                _DriversLicenseInfo = value
                SetObjectsParent(_DriversLicenseInfo)
            End Set
        End Property
        Public Property PersonalIdentification As QuickQuotePersonalIdentification
            Get
                SetObjectsParent(_PersonalIdentification)
                Return _PersonalIdentification
            End Get
            Set(value As QuickQuotePersonalIdentification)
                _PersonalIdentification = value
                SetObjectsParent(_PersonalIdentification)
            End Set
        End Property
        Public Property PolicyInfo As QuickQuoteThirdPartyPolicyInfo
            Get
                SetObjectsParent(_PolicyInfo)
                Return _PolicyInfo
            End Get
            Set(value As QuickQuoteThirdPartyPolicyInfo)
                _PolicyInfo = value
                SetObjectsParent(_PolicyInfo)
            End Set
        End Property
        Public Property PriorAddress As QuickQuoteThirdPartyDataAddress
            Get
                SetObjectsParent(_PriorAddress)
                Return _PriorAddress
            End Get
            Set(value As QuickQuoteThirdPartyDataAddress)
                _PriorAddress = value
                SetObjectsParent(_PriorAddress)
            End Set
        End Property
        Public Property PriorDriversLicenseInfo As QuickQuoteDriversLicenseInfo
            Get
                SetObjectsParent(_PriorDriversLicenseInfo)
                Return _PriorDriversLicenseInfo
            End Get
            Set(value As QuickQuoteDriversLicenseInfo)
                _PriorDriversLicenseInfo = value
                SetObjectsParent(_PriorDriversLicenseInfo)
            End Set
        End Property
        Public Property SubUnitQuotebackInfo As QuickQuoteSubUnitQuotebackInfo
            Get
                SetObjectsParent(_SubUnitQuotebackInfo)
                Return _SubUnitQuotebackInfo
            End Get
            Set(value As QuickQuoteSubUnitQuotebackInfo)
                _SubUnitQuotebackInfo = value
                SetObjectsParent(_SubUnitQuotebackInfo)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _CurrentAddress = New QuickQuoteThirdPartyDataAddress
            _DriversLicenseInfo = New QuickQuoteDriversLicenseInfo
            _PersonalIdentification = New QuickQuotePersonalIdentification
            _PolicyInfo = New QuickQuoteThirdPartyPolicyInfo
            _PriorAddress = New QuickQuoteThirdPartyDataAddress
            _PriorDriversLicenseInfo = New QuickQuoteDriversLicenseInfo
            _SubUnitQuotebackInfo = New QuickQuoteSubUnitQuotebackInfo
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
                    If _CurrentAddress IsNot Nothing Then
                        _CurrentAddress.Dispose()
                        _CurrentAddress = Nothing
                    End If
                    If _DriversLicenseInfo IsNot Nothing Then
                        _DriversLicenseInfo.Dispose()
                        _DriversLicenseInfo = Nothing
                    End If
                    If _PersonalIdentification IsNot Nothing Then
                        _PersonalIdentification.Dispose()
                        _PersonalIdentification = Nothing
                    End If
                    If _PolicyInfo IsNot Nothing Then
                        _PolicyInfo.Dispose()
                        _PolicyInfo = Nothing
                    End If
                    If _PriorAddress IsNot Nothing Then
                        _PriorAddress.Dispose()
                        _PriorAddress = Nothing
                    End If
                    If _PriorDriversLicenseInfo IsNot Nothing Then
                        _PriorDriversLicenseInfo.Dispose()
                        _DriversLicenseInfo = Nothing
                    End If
                    If _SubUnitQuotebackInfo IsNot Nothing Then
                        _SubUnitQuotebackInfo.Dispose()
                        _SubUnitQuotebackInfo = Nothing
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
