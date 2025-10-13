Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store subject section information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteSubjectSection 'added 9/19/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        'Private _AutoSubjectIdentification As QuickQuoteAutoSubjectIdentification '*may contain multiple (insuresoft collection according to intellisense; xml sample just has 1); this was specific to AutoDataPrefill sub-element
        Private _CurrentAddress As QuickQuoteThirdPartyDataAddress
        Private _PersonalIdentification As QuickQuotePersonalIdentification
        Private _PriorAddress As QuickQuoteThirdPartyDataAddress
        Private _SpousePersonalIdentification As QuickQuotePersonalIdentification

        'Public Property AutoSubjectIdentification As QuickQuoteAutoSubjectIdentification
        '    Get
        '        Return _AutoSubjectIdentification
        '    End Get
        '    Set(value As QuickQuoteAutoSubjectIdentification)
        '        _AutoSubjectIdentification = value
        '    End Set
        'End Property
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
        Public Property SpousePersonalIdentification As QuickQuotePersonalIdentification
            Get
                SetObjectsParent(_SpousePersonalIdentification)
                Return _SpousePersonalIdentification
            End Get
            Set(value As QuickQuotePersonalIdentification)
                _SpousePersonalIdentification = value
                SetObjectsParent(_SpousePersonalIdentification)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            '_AutoSubjectIdentification = New QuickQuoteAutoSubjectIdentification
            _CurrentAddress = New QuickQuoteThirdPartyDataAddress
            _PersonalIdentification = New QuickQuotePersonalIdentification
            _PriorAddress = New QuickQuoteThirdPartyDataAddress
            _SpousePersonalIdentification = New QuickQuotePersonalIdentification
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
                    'If _AutoSubjectIdentification IsNot Nothing Then
                    '    _AutoSubjectIdentification.Dispose()
                    '    _AutoSubjectIdentification = Nothing
                    'End If
                    If _CurrentAddress IsNot Nothing Then
                        _CurrentAddress = Nothing
                    End If
                    If _PersonalIdentification IsNot Nothing Then
                        _PersonalIdentification = Nothing
                    End If
                    If _PriorAddress IsNot Nothing Then
                        _PriorAddress = Nothing
                    End If
                    If _SpousePersonalIdentification IsNot Nothing Then
                        _SpousePersonalIdentification = Nothing
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
