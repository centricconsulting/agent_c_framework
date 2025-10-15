Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store inquiry section information
    ''' </summary>
    ''' <remarks>related to 3rd party information</remarks>
    <Serializable()> _
    Public Class QuickQuoteInquirySection 'added 9/18/2013
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _InquiryIdentification As QuickQuoteInquiryIdentification
        Private _RequestorIdentification As QuickQuoteRequestorIdentification
        Private _SupplementA As QuickQuoteSupplementA
        Private _SupplementC As QuickQuoteSupplementC

        Public Property InquiryIdentification As QuickQuoteInquiryIdentification
            Get
                SetObjectsParent(_InquiryIdentification)
                Return _InquiryIdentification
            End Get
            Set(value As QuickQuoteInquiryIdentification)
                _InquiryIdentification = value
                SetObjectsParent(_InquiryIdentification)
            End Set
        End Property
        Public Property RequestorIdentification As QuickQuoteRequestorIdentification
            Get
                SetObjectsParent(_RequestorIdentification)
                Return _RequestorIdentification
            End Get
            Set(value As QuickQuoteRequestorIdentification)
                _RequestorIdentification = value
                SetObjectsParent(_RequestorIdentification)
            End Set
        End Property
        Public Property SupplementA As QuickQuoteSupplementA
            Get
                SetObjectsParent(_SupplementA)
                Return _SupplementA
            End Get
            Set(value As QuickQuoteSupplementA)
                _SupplementA = value
                SetObjectsParent(_SupplementA)
            End Set
        End Property
        Public Property SupplementC As QuickQuoteSupplementC
            Get
                SetObjectsParent(_SupplementC)
                Return _SupplementC
            End Get
            Set(value As QuickQuoteSupplementC)
                _SupplementC = value
                SetObjectsParent(_SupplementC)
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _InquiryIdentification = New QuickQuoteInquiryIdentification
            _RequestorIdentification = New QuickQuoteRequestorIdentification
            _SupplementA = New QuickQuoteSupplementA
            _SupplementC = New QuickQuoteSupplementC
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
                    If _InquiryIdentification IsNot Nothing Then
                        _InquiryIdentification.Dispose()
                        _InquiryIdentification = Nothing
                    End If
                    If _RequestorIdentification IsNot Nothing Then
                        _RequestorIdentification.Dispose()
                        _RequestorIdentification = Nothing
                    End If
                    If _SupplementA IsNot Nothing Then
                        _SupplementA.Dispose()
                        _SupplementA = Nothing
                    End If
                    If _SupplementC IsNot Nothing Then
                        _SupplementC.Dispose()
                        _SupplementC = Nothing
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
